// ReSharper disable CheckNamespace

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using Exceedra.Common;
using Exceedra.Controls.DynamicGrid.Models;
using Exceedra.Controls.DynamicGrid.ViewModels;
using Exceedra.Controls.Messages;
using Exceedra.MultiSelectCombo.ViewModel;
using Exceedra.SingleSelectCombo.ViewModel;
using Model;
using Model.DataAccess;
using Model.DataAccess.Converters;
using Model.DataAccess.Generic;
using Model.Entity.Funds;
using Model.Entity.Generic;
using ViewHelper;
using WPF;
using WPF.Navigation;
using ICommand = System.Windows.Input.ICommand;
using System.Windows.Navigation;

namespace ViewModels
{
    public class FundViewModel : BaseViewModel
    {
        public FundViewModel(string fundIdx, bool isParentFund = false)
        {
            Products.PropertyChanged += OnPropertyChanged;
            Customers.PropertyChanged += CustomersOnPropertyChanged;


            CancelCommand = new ViewCommand(Cancel);
            ReloadCommand = new ViewCommand(Reload);
            _fundAccess = new FundAccess();
            FundIdx = fundIdx;
            MyIdx = fundIdx;
            _recalculateBackgroundWorker.DoWork += _listingsBackgroundWorker_DoWork;
            _recalculateBackgroundWorker.RunWorkerCompleted += _listingsBackgroundWorker_RunWorkerCompleted;
            //_recalculateBackgroundWorker.WorkerSupportsCancellation = true;

            IsLoading = true;
            PropertyChanged += OnPropertyChanged;

            App.Navigator.EnableNavigation(false);

            //load singletons
            if (string.IsNullOrEmpty(FundIdx))
            {
                FundDetails = new FundDetail();
                FundDetails.Amendable = true;
                FundDetails.ID = "0";
                FundDetails.IsParent = isParentFund;
            }
            else
            {
                LoadFund();
            }

            LoadTypes();

            //load customer levels
            LoadCustomerLevels();

            //load prod levels
            LoadProductLevels();

            //load status           
            LoadStatuses();

            //load notes
            LoadNotes();

            //load impacts
            LoadFundValues();

            //load linked events
            LoadDynamicLinks();

            //load Other info
            LoadOtherInfo();

            //load summary table
            // dynamic grid magic
            LoadSummaryTable();

            //load impacts dropdown
            LoadImpacts(true);

            //Loads Transfer Log dynamic grid
            LoadTransferLog();

            TabVisibilityFromIsParent();


            IsLoading = false;

        }

        private bool _runAgain;
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_watchList.Contains(e.PropertyName))
            {
                if (_recalculateBackgroundWorker.IsBusy)
                    _runAgain = true;
                else    //_recalculateBackgroundWorker.CancelAsync();
                    _recalculateBackgroundWorker.RunWorkerAsync();

                if (e.PropertyName == "Date_Start")
                {
                    LoadProducts();
                }
            }
        }

        BackgroundWorker _recalculateBackgroundWorker = new BackgroundWorker();
        void _listingsBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //if (Values != null)
            //{
            //    Values.IsRunning = true;
            //    Values.PanelMainMessage = "Recalculating...";
            //}

            CultureInfo before = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(App.CurrentLang.LanguageCode);

                LoadFundValues();
                LoadImpacts();

                ReApplyUnsavedRows();

                //Values.IsRunning = false;
            }
            finally
            {
                Thread.CurrentThread.CurrentUICulture = before;
            }
            
        }

        void _listingsBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_runAgain)
            {
                _runAgain = false;
                _recalculateBackgroundWorker.RunWorkerAsync();
            }
        }


        #region Private Properties

        private FundDetail _fundDetails;
        private FundAccess _fundAccess;
        private ObservableCollection<FundImpact> _impacts;
        private FundImpact _selectedImpact;
        private ObservableCollection<DropdownItem> _types;
        private DropdownItem _selectedType;
        private ObservableCollection<DropdownItem> _subTypes;
        private DropdownItem _selectedSubType;
        private ObservableCollection<DropdownItem> _customerLevels;
        private DropdownItem _selectedCustomerLevel;
        private MultiSelectViewModel _customers = new MultiSelectViewModel();
        private ObservableCollection<DropdownItem> _productLevels;
        private DropdownItem _selectedProductLevel;
        private MultiSelectViewModel _products;
        private List<ComboboxItem> _staticProducts;
        private SingleSelectViewModel _statusVM;
        private Status _selectedStatus;
        private ObservableCollection<Note> _notes;
        private string _newComment;
        private string _newImpactValue;
        private string _fundIdx;
        private List<string> _watchList = new List<string> { "SelectedSubType", "Date_Start", "Date_End", "SelectedItems", "IsParent" };
        private List<List<Property>> _unsavedRows = new List<List<Property>>();
        //private Visibility _linkedEventsTabVis;
        //private Visibility _transferTabVis;
        private bool _linkedEventsTabSelected;
        private bool _transferTabSelected ;
        private string _pageTitle;
    #endregion

    #region Private Void

    private void LoadFundValues()
        {
            var arguments = CommonXml.GetBaseArguments("GetValues");
            if (!String.IsNullOrWhiteSpace(FundIdx))
                arguments.AddElement("Fund_Idx", FundIdx);

            if (CanLoadImpacts() && !IsLoading)
            {
                arguments.AddElement("UpdateValues", 1);
                arguments.AddElement("SubType_Idx", SelectedSubType.ID);
                arguments.AddElement("Start", InputConverter.ToIsoFormat(FundDetails.Date_Start));
                arguments.AddElement("End", InputConverter.ToIsoFormat(FundDetails.Date_End));
                arguments.Add(InputConverter.ToCustomers(SelectedCustomerIdxs));
                arguments.Add(InputConverter.ToProducts(SelectedProducts.Select(p => p.Idx)));

                GetValues(_fundAccess.GetDynamicGridData("GetValues", arguments));
            }
            else
            {
                Values = new RecordViewModel(true);
            }

         

        }

        private void LoadDynamicLinks()
        {
            // [app].[Procast_SP_FUND_GetLinkedEvents]
            _fundAccess.GetDynamicGridDataAsync("GetLinkedEvents", FundIdx).ContinueWith(h =>
            {
                GetLinkedEvents(h.Result);
                //CheckAndUpdateSummary();
            });
        }

        private void LoadStatuses()
        {
            var args = CommonXml.GetBaseArguments("GetWorkflowStatuses");
            args.Add(new XElement("Fund_Idx", FundIdx));
            DynamicDataAccess.GetGenericItemAsync<SingleSelectViewModel>(
                _fundAccess.GetStatusProc(), args).ContinueWith(t =>
                {
                    StatusVM = t.Result;
                });
        }

        private void LoadOtherInfo()
        {
            // [app].[Procast_SP_FUND_GetFundInfo]
            _fundAccess.GetDynamicGridDataAsync("GetFundInfo", FundIdx).ContinueWith(h =>
            {
                GetFundInfo(h.Result);
            });
        }

        private void LoadSummaryTable()
        {
            // [app].[Procast_SP_FUND_GetFundBalances]
            _fundAccess.GetDynamicGridDataAsync("GetFundBalances", FundIdx).ContinueWith(h =>
            {
                GetFundBalances(h.Result);
                CheckAndUpdateSummary();
            });
        }

        private void LoadNotes()
        {
            // [app].[Procast_SP_FUND_GetComments]
            _fundAccess.GetNotes(FundIdx).ContinueWith(
                t =>
                {
                    Notes = new ObservableCollection<Note>(t.Result);
                });
        }

        private void LoadProductLevels()
        {
            // [app].[Procast_SP_FUND_GetProdLevels]
            // [app].[Procast_SP_FUND_GetProdSelection]
            _fundAccess.GetProductLevels(FundIdx).ContinueWith(
                t =>
                {
                    ProductLevels = new ObservableCollection<DropdownItem>(t.Result);
                });
        }

        private void LoadCustomerLevels()
        {
            // [app].[Procast_SP_FUND_GetCustLevels]
            // [app].[Procast_SP_FUND_GetCustSelection]
            _fundAccess.GetCustomerLevels(FundIdx).ContinueWith(
                t =>
                {
                    CustomerLevels = new ObservableCollection<DropdownItem>(t.Result);
                });
        }

        private void LoadTypes()
        {
            //load type
            // [app].[Procast_SP_FUND_GetTypes]
            Types = new ObservableCollection<DropdownItem>(_fundAccess.GetTypes(FundIdx).Result);



            //load subtype
            // [app].[Procast_SP_FUND_GetSubTypes]
            SubTypes = new ObservableCollection<DropdownItem>(_fundAccess.GetSubTypes(SelectedType.ID, FundIdx).Result);


        }

        private void LoadFund()
        {
            // [app].[Procast_SP_FUND_GetFund]
            FundDetails = _fundAccess.GetFund(FundIdx);
            FundIdx = FundDetails.ID;
            PageTitle = String.IsNullOrEmpty(FundDetails.Code) ? "" : "Fund (" + FundDetails.Code + ")";
        }

        private void LoadTransferLog()
        {
            _fundAccess.GetTransferLogGrid("GetValues", FundIdx).ContinueWith(t => GetTransferLog(t.Result));
        }

        private void LoadImpacts(bool usingIdx = false)
        {
            var arguments = CommonXml.GetBaseArguments("GetValues");

            if (usingIdx)
            {
                if (!String.IsNullOrWhiteSpace(FundIdx))
                    arguments.AddElement("Fund_Idx", FundIdx);
                else return;
            }
            else if (CanLoadImpacts())
            {
                if (!String.IsNullOrWhiteSpace(FundIdx))
                    arguments.AddElement("Fund_Idx", FundIdx);

                arguments.AddElement("IsParent", FundDetails.IsParent);
                arguments.AddElement("UpdateValues", 1);
                arguments.AddElement("SubType_Idx", SelectedSubType.ID);
                arguments.AddElement("Start", InputConverter.ToIsoFormat(FundDetails.Date_Start));
                arguments.AddElement("End", InputConverter.ToIsoFormat(FundDetails.Date_End));
                arguments.Add(InputConverter.ToCustomers(SelectedCustomerIdxs));
                arguments.Add(InputConverter.ToProducts(SelectedProducts.Select(p => p.Idx)));
            }
            else
            {
                Impacts = new ObservableCollection<FundImpact>();
                return;
            }

            Impacts = new ObservableCollection<FundImpact>(_fundAccess.GetImpacts(arguments));

            //Impacts = new ObservableCollection<FundImpact>(_fundAccess.GetImpacts(arguments).Result);
        }

        private bool CanLoadImpacts()
        {
            return FundDetails != null
                   && FundDetails.Date_Start != null
                   && FundDetails.Date_End != null
                   && SelectedCustomerLevel != null
                   && Customers != null 
                   && SelectedCustomers != null
                   && SelectedCustomers.Any()
                   && Products != null
                   && SelectedProducts != null
                   && SelectedProducts.Any();
        }

        #endregion

        #region Public
        public bool IsParentCanBeEdited(object param = null)
        {
            var b = false;
            if (LinkedEvents != null && LinkedEvents.HasRecords)
            {
               b = LinkedEvents.Records.Any(t => t.Properties[0].Value != "0");
            }
             
            return (IsEditable && !b); 
        }
        
        public bool IsEditable
        {
            get { return (FundDetails != null && FundDetails.Amendable); }
        }

        public bool IsReadOnly
        {
            get { return !IsEditable; }
        }

        public string FundIdx
        {
            get { return _fundIdx; }
            set
            {
                _fundIdx = value;
                NotifyPropertyChanged(this, vm => vm.FundIdx);
            }
        }

        public bool IsLoading { get; set; }
        
        public string PageTitle 
        {
            get { return _pageTitle; }
            set 
            {
               _pageTitle = value;
               NotifyPropertyChanged(this, vm => vm.PageTitle);
            } 
        }

        public ObservableCollection<FundImpact> Impacts
        {
            get { return _impacts; }
            set
            {
                _impacts = value;
                NotifyPropertyChanged(this, vm => vm.Impacts);
            }
        }

        public FundImpact SelectedImpact
        {
            get { return _selectedImpact; }
            set
            {
                _selectedImpact = value;
                NotifyPropertyChanged(this, vm => vm.SelectedImpact);
            }
        }

        public ObservableCollection<DropdownItem> Types
        {
            get { return _types; }
            set
            {
                _types = value;
                SelectedType = Types.FirstOrDefault(r => r.IsSelected) ?? Types.First();
                NotifyPropertyChanged(this, vm => vm.Types);
            }
        }

        public DropdownItem SelectedType
        {
            get { return _selectedType; }
            set
            {
                _selectedType = value;

                SubTypes = new ObservableCollection<DropdownItem>(_fundAccess.GetSubTypes(_selectedType.ID, FundIdx).Result);

                NotifyPropertyChanged(this, vm => vm.SelectedType);
            }
        }

        public DropdownItem SelectedSubType
        {
            get { return _selectedSubType; }
            set
            {
                _selectedSubType = value;
                _unsavedRows = new List<List<Property>>();
                NotifyPropertyChanged(this, vm => vm.SelectedSubType);
            }
        }

        public ObservableCollection<DropdownItem> SubTypes
        {
            get { return _subTypes; }
            set
            {
                _subTypes = value;
                SelectedSubType = SubTypes.FirstOrDefault(y => y.IsSelected);
                NotifyPropertyChanged(this, vm => vm.SubTypes);
            }
        }

        public FundDetail FundDetails
        {
            get { return _fundDetails; }
            set
            {
                _fundDetails = value;
                if (_fundDetails.Date_End == null)
                    _fundDetails.Date_End = DateTime.Now;

                if (_fundDetails.Date_Start == null)
                    _fundDetails.Date_Start = DateTime.Now;

                FundDetails.PropertyChanged += OnPropertyChanged;
                NotifyPropertyChanged(this, vm => vm.FundDetails);
            }
        }

        public ObservableCollection<DropdownItem> CustomerLevels
        {
            get { return _customerLevels; }
            set
            {
                _customerLevels = value;
                SelectedCustomerLevel = CustomerLevels.FirstOrDefault(r => r.IsSelected) ?? CustomerLevels.First();
                NotifyPropertyChanged(this, vm => vm.CustomerLevels);
            }
        }

        public DropdownItem SelectedCustomerLevel
        {
            get { return _selectedCustomerLevel; }
            set
            {
                _selectedCustomerLevel = value;

                _fundAccess.GetCustomers(SelectedCustomerLevel.ID, FundIdx).ContinueWith(t =>
                {
                    Customers.SetItems(t.Result);
                    LoadProducts();
                });

                NotifyPropertyChanged(this, vm => vm.SelectedCustomerLevel);
            }
        }

        public MultiSelectViewModel Customers
        {
            get { return _customers; }
            set
            {
                _customers = value;
                NotifyPropertyChanged(this, vm => vm.Customers);
            }
        }

        private void CustomersOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "SelectedItems")
            {
                LoadProducts();
                NotifyPropertyChanged(this, vm => vm.SelectedCustomers);
            }
        }

        public List<ComboboxItem> SelectedCustomers
        {
            get { return Customers.SelectedItems.ToList(); }
        }

        public List<string> SelectedCustomerIdxs
        {
            get { return SelectedCustomers.Select(c => c.Idx).ToList(); }
        }

        public ObservableCollection<DropdownItem> ProductLevels
        {
            get { return _productLevels; }
            set
            {
                _productLevels = value;
                SelectedProductLevel = ProductLevels.FirstOrDefault(r => r.IsSelected) ?? ProductLevels.First();
                NotifyPropertyChanged(this, vm => vm.ProductLevels);
            }
        }

        public DropdownItem SelectedProductLevel
        {
            get { return _selectedProductLevel; }
            set
            {
                _selectedProductLevel = value;
               
                LoadProducts();
                
                NotifyPropertyChanged(this, vm => vm.SelectedProductLevel);
            }
        }

        private void LoadProducts()
        {
            if (SelectedProductLevel == null || Customers == null || SelectedCustomers == null) return;

            // If none customer is selected we will clear the product selection.
            if (!SelectedCustomers.Any())
            {
                Products.Items.Clear();
                StaticProducts = new List<ComboboxItem>();
            }
            else _fundAccess.GetProducts(_selectedProductLevel.ID, SelectedCustomerIdxs, FundIdx).ContinueWith(
                t =>
                {
                    // remembering the selected products to reselect them after the products reload
                    var selectedProductsIds = SelectedProductIdxs ?? new List<string>();

                    //Products = new MultiSelectViewModel(t.Result);
                    StaticProducts = new List<ComboboxItem>(t.Result);

                    // reselecting previously selected products
                    foreach (var idx in selectedProductsIds)
                    {
                        var product = StaticProducts.FirstOrDefault(prod => prod.Idx == idx);
                        if (product != null) product.IsSelected = true;
                    }

                    DelistProducts();
                });
        }

        private void DelistProducts()
        {
            if (StaticProducts != null && StaticProducts.Any())
            {
                List<ComboboxItem> copyOfProducts = new List<ComboboxItem>(StaticProducts);
                
                copyOfProducts.RemoveAll(a => a.DelistingsDate != null && a.DelistingsDate < FundDetails.Date_Start);

                Products.SetItems(copyOfProducts);
            }
        }

        public MultiSelectViewModel Products
        {
            get { return _products ?? (_products = new MultiSelectViewModel());}
            set
            {
                _products = value;
                Products.PropertyChanged += ProductsOnPropertyChanged;
                NotifyPropertyChanged(this, vm => vm.Products);
                NotifyPropertyChanged(this, vm => vm.SelectedProducts);
            }
        }

        private void ProductsOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "SelectedItems")
            {
                NotifyPropertyChanged(this, vm => vm.SelectedProducts);
            }
        }

        public List<ComboboxItem> StaticProducts
        {
            get { return _staticProducts; }
            set
            {
                _staticProducts = value;
                NotifyPropertyChanged(this, vm => vm.StaticProducts);
            }
        }

        public List<ComboboxItem> SelectedProducts
        {
            get { return Products == null || Products.SelectedItems == null ? null : Products.SelectedItems.Where(p => p.IsSelected).ToList(); }
        }

        public List<string> SelectedProductIdxs
        {
            get { return SelectedProducts == null ? null : SelectedProducts.Select(p => p.Idx).ToList(); }
        }

        public SingleSelectViewModel StatusVM
        {
            get { return _statusVM; }
            set
            {
                _statusVM = value;
                NotifyPropertyChanged(this, vm => vm.StatusVM);
            }
        }


        public ObservableCollection<Note> Notes
        {
            get { return _notes; }
            set
            {
                _notes = value;
                NotifyPropertyChanged(this, vm => vm.Notes);
            }
        }

        public string NewComment
        {
            get
            {
                return _newComment;

            }
            set
            {
                _newComment = value;
                NotifyPropertyChanged(this, vm => vm.NewComment);
            }
        }

        public string NewImpactValue
        {
            get
            {
                return _newImpactValue;

            }
            set
            {
                _newImpactValue = value;
                NotifyPropertyChanged(this, vm => vm.NewImpactValue);
            }
        }

        //public Visibility LinkedEventsTabVis
        //{
        //    get { return _linkedEventsTabVis; }
        //    set
        //    {
        //        _linkedEventsTabVis = value;
        //        NotifyPropertyChanged(this, vm => vm.LinkedEventsTabVis);
        //    }
        //}

        //public Visibility TransferTabVis
        //{
        //    get { return _transferTabVis; }
        //    set
        //    {
        //        _transferTabVis = value;
        //        NotifyPropertyChanged(this, vm => vm.TransferTabVis);
        //    }
        //}

        public bool LinkedEventsTabSelected
        {
            get { return _linkedEventsTabSelected; }
            set
            {
                _linkedEventsTabSelected = value;
                NotifyPropertyChanged(this, vm => vm.LinkedEventsTabSelected);
            }
        }

        public bool TransferTabSelected
        {
            get { return _transferTabSelected; }
            set
            {
                _transferTabSelected = value;
                NotifyPropertyChanged(this, vm => vm.TransferTabSelected);
            }
        }

        #endregion

        #region Commands

        public ICommand ParentCheckedCommand
        {
            get { return new ViewCommand(IsParentCanBeEdited, TabVisibilityFromIsParent); }
        }

        public ICommand AddCommentCommand
        {
            get { return new ViewCommand(CanAddComment, AddComment); }
        }

        public ICommand SaveCommand
        {
            get { return new ViewCommand(CanSave, Save); }
        }

        public ICommand SaveCloseCommand
        {
            get { return new ViewCommand(CanSave, SaveClose); }
        }

        public ICommand ReloadCommand { get; set; }

        private void Reload(object obj)
        {
             RedirectMe.Goto("fund", FundIdx);
        }

        public ICommand CancelCommand { get; set; }

        private void Cancel(object obj)
        {
            RedirectMe.ListScreen("Fund");            
        }

        public ICommand AddNewImpactCommand
        {
            get { return new ViewCommand(CanAddImpact, AddNewFundValue); }
        }

        private bool CanAddImpact(object obj)
        {
            return FundDetails.Amendable
                   && SelectedImpact != null
                   && SelectedImpact.Idx != null
                   && !string.IsNullOrEmpty(NewImpactValue);
        }

        private void AddNewFundValue(object obj)
        {
            var properties = Values.Records[0].Properties.Select(p => new Property
            {
                ColumnCode = p.ColumnCode,
                IsDisplayed = p.IsDisplayed,
                ControlType = p.ControlType,
                ForeColour = "Red",
                StringFormat = p.StringFormat,
                IsLoaded = true
            }).ToList();

            var newItemIdx = (Values.Records.Select(r => Convert.ToInt32(r.Item_Idx)).Max() + 1).ToString();
            var rand = new Random();
            var newId = rand.Next(-10, -1);

            foreach (var p in properties)
            {
                switch (p.ColumnCode)
                {
                    case "RowID":
                        p.Value = newId.ToString();
                        break;
                    case "User_Name":
                        p.Value = User.CurrentUser.DisplayName;
                        break;
                    case "Date_Created":
                        p.Value = DateTime.Now.ToString("yyyy-MM-dd");
                        break;
                    case "Fund_Impact_Idx":
                        p.Value = SelectedImpact.Idx;
                        break;
                    case "Fund_Impact_Name":
                        p.Value = SelectedImpact.Name;
                        break;
                    case "Amount_Entered":
                        p.StringFormat = SelectedImpact.Format;
                        p.Value = NewImpactValue;
                        break;
                    case "Value":
                        var val = Convert.ToDecimal(NewImpactValue) * SelectedImpact.MultiplicationFactor;

                        if (SelectedImpact != null && SelectedImpact.Format != null && SelectedImpact.Format.ToLower().Contains("p"))
                            val = val * 100;

                        p.Value = String.Format("{0:c}", val);
                        break;
                }
            }

            _unsavedRows.Add(properties);
            Values.AddRecord(properties, "Fund_Values", Values.Records.Count, newItemIdx);

            NotifyPropertyChanged(this, vm => vm.Values);
            Values.CalulateRecordColumns();
            NewImpactValue = "";

        }

        public void TabVisibilityFromIsParent(object obj = null)
        {
            if (FundDetails.IsParent)
            {
               // LinkedEventsTabVis = Visibility.Collapsed;
                //TransferTabVis = Visibility.Visible;
                TransferTabSelected = true;
                LinkedEventsTabSelected = false;
            }
            else
            {
                //LinkedEventsTabVis = Visibility.Visible;
                //TransferTabVis = Visibility.Collapsed;
                TransferTabSelected = false;
                LinkedEventsTabSelected = true;
            }
            LoadImpacts();
        }

        private void ReApplyUnsavedRows()
        {
            if (!Impacts.Any()) return;

            foreach (var row in _unsavedRows)
            {
                decimal impactValueDecimal;

                var impactIdx = row.First(r => r.ColumnCode.Equals("Fund_Impact_Idx")).Value;
                if (!Impacts.Select(i => i.Idx).Contains(impactIdx)) continue;
                var impactValueString = row.First(r => r.ColumnCode.Equals("Amount_Entered")).Value;
                var newMultiplicationFactor = Impacts.First(i => i.Idx.Equals(impactIdx)).MultiplicationFactor;
                var newFormat = Impacts.First(i => i.Idx.Equals(impactIdx)).Format;

                if (newFormat.ToLowerInvariant().Contains("p"))
                    impactValueDecimal = Convert.ToDecimal(impactValueString.Replace(CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, ""));
                else
                    decimal.TryParse(impactValueString, NumberStyles.Currency, CultureInfo.CurrentCulture.NumberFormat, out impactValueDecimal);

                row.First(r => r.ColumnCode == "Value").Value = String.Format("{0:c}", impactValueDecimal * newMultiplicationFactor);
                row.First(r => r.ColumnCode == "Value").ForeColour = "Red";

                var newItemIdx = (Values.Records.Select(r => Convert.ToInt32(r.Item_Idx)).Max() + 1).ToString();
                Values.AddRecord(row, "Fund_Values", Values.Records.Count, newItemIdx);
            }

        }

        public bool IsValid
        {
            get
            {
                var startDate = Convert.ToDateTime(FundDetails.Date_Start, new CultureInfo(App.CurrentLang.LanguageCode));
                var endDate = Convert.ToDateTime(FundDetails.Date_End, new CultureInfo(App.CurrentLang.LanguageCode));

                //&& !string.IsNullOrEmpty(FundDetails.Date_End)
                //       && !string.IsNullOrEmpty(FundDetails.Date_Start)

                return (!string.IsNullOrEmpty(FundDetails.Name)
                        && startDate != new DateTime()
                        && endDate != new DateTime()
                        && startDate <= endDate
                        && SelectedSubType != null
                        && StatusVM != null
                        && StatusVM.SelectedItem != null
                        && SelectedCustomerLevel != null
                        && Customers != null
                        && SelectedCustomers != null
                        && SelectedProductLevel != null
                        && Products != null
                        && SelectedProducts != null && SelectedProducts.Any());

            }
        }

        #endregion

        #region Grids

        private RecordViewModel _fundBalances;

        public RecordViewModel FundBalances
        {
            get { return _fundBalances; }
            set
            {
                if (_fundBalances != value)
                {
                    _fundBalances = value;
                    NotifyPropertyChanged(this, vm => vm.FundBalances);
                }
            }
        }

        private RecordViewModel _linkedEvents;

        public RecordViewModel LinkedEvents
        {
            get { return _linkedEvents; }
            set
            {
                if (_linkedEvents != value)
                {
                    _linkedEvents = value;
                    NotifyPropertyChanged(this, vm => vm.LinkedEvents);
                }
            }
        }

        private RecordViewModel _fundInfo;
        public RecordViewModel FundInfo
        {
            get { return _fundInfo; }
            set
            {
                if (_fundInfo != value)
                {
                    _fundInfo = value;
                    NotifyPropertyChanged(this, vm => vm.FundInfo);
                }
            }
        }

        private RecordViewModel _values;

        public RecordViewModel Values
        {
            get { return _values; }
            set
            {
                if (_values != value)
                {
                    _values = value;
                    NotifyPropertyChanged(this, vm => vm.Values);
                }
            }
        }

        private RecordViewModel _transferLog;

        public RecordViewModel TransferLog
        {
            get { return _transferLog; }
            set
            {
                _transferLog = value;
                NotifyPropertyChanged(this, vm => vm.TransferLog);
            }
        }

        private void GetFundBalances(XElement r)
        {

            if (r == null)
            {
                FundBalances = new RecordViewModel(true);
            }
            else
            {
                FundBalances = new RecordViewModel(r);
            }
        }

        private void GetLinkedEvents(XElement r)
        {

            if (r == null)
            {
                LinkedEvents = new RecordViewModel(true);
            }
            else
            {
                LinkedEvents = new RecordViewModel(r);
            }
        }

        public bool IsParentCheckboxEnabled { get; set; }
        private void GetFundInfo(XElement r)
        {                  
            if (r == null)
            {
                FundInfo = new RecordViewModel(true);
            }
            else
            {
                FundInfo = new RecordViewModel(r);
                IsParentCheckboxEnabled = r.Element("IsParentCheckboxEnabled").MaybeValue() != "0";
                NotifyPropertyChanged(this, vm => vm.IsParentCheckboxEnabled);
            }
        }

        private void GetValues(XElement r)
        {

            if (r == null)
            {
                Values = new RecordViewModel(true);
            }
            else
            {
                Values = new RecordViewModel(r);
            }
        }

        private void GetTransferLog(XElement r)
        {
            if (r == null)
            {
                TransferLog = new RecordViewModel(true);
            }
            else
            {
                TransferLog = new RecordViewModel(r);
            }
        }

        public void CheckAndUpdateSummary()
        {
            // if (LinkedEvents != null && Values != null)
            if (FundBalances != null)
                GetSummaryTableData();
        }


        private void GetSummaryTableData()
        {
            // find all SUMIF column in G2
            foreach (var record in FundBalances.Records)
            {
                //    <Value>=SUMIFS(FinancialScreenPlanningSkuMeasure$TestColumn, FinancialScreenPlanningSkuMeasure$ParentSkuIdx, ParentSkuIdx)</Value>

                //check each SUMIF column and update parent grid based on values
                var sums = record.Properties.Where(p => p.Calculation != null).Where(t => t.Calculation.Contains("=SUM"));

                //NEW SUM has only 2 options, where we get the value of a summed column in a 2nd grid
                //  <Value>=SUM(Fund_Linked_Events$Net_Value)</Value>

                foreach (var ec in sums)
                {
                    var calc = ec.Calculation.Replace("=SUM", "").Replace("(", "").Replace(")", "").Replace(" ", "");

                    var options = calc.Split('$');
                    var grid = options[0];
                    var column = options[1];


                    if (grid == "Fund_Linked_Events" && LinkedEvents != null)
                    {
                        Decimal sum = (from cta in LinkedEvents.Records let col = cta.Properties.FirstOrDefault(prop => prop.ColumnCode == column) where col != null select cta.Properties.FirstOrDefault(p => p.ColumnCode == column) into x where x != null select fixDecimal(x.Value)).Sum();
                        //Get the Value for each item in the correct column where the parentID is correct

                        ec.Value = sum.ToString(CultureInfo.CurrentCulture);
                        NotifyPropertyChanged(this, vm => vm.FundBalances);
                    }

                    if (grid == "Fund_Values" && Values != null && Values.Records != null)
                    {
                        Decimal sum = (from cta in Values.Records let col = cta.Properties.FirstOrDefault(prop => prop.ColumnCode == column) where col != null select cta.Properties.FirstOrDefault(p => p.ColumnCode == column) into x where x != null select fixDecimal(x.Value)).Sum();
                        //Get the Value for each item in the correct column where the parentID is correct

                        ec.Value = sum.ToString(CultureInfo.CurrentCulture);
                        NotifyPropertyChanged(this, vm => vm.FundBalances);
                    }
                }

            }

            FundBalances.CalulateRecordColumns();

        }

        private decimal fixDecimal(string val)
        {
            decimal d;
            //rip out the localised %
            val = val.Replace(CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");

            var isNum = decimal.TryParse(val, NumberStyles.Currency, CultureInfo.CurrentCulture.NumberFormat, out d);

            return isNum ? d : 0;
        }

        #endregion

        #region Comments



        public bool CanAddComment(object param = null)
        {
            CanAddCommentBool = HasID();
            return CanAddCommentBool;
        }

        private bool _canAddCommentBool;
        public bool CanAddCommentBool
        {
            get { return _canAddCommentBool; }
            set
            {
                _canAddCommentBool = value;
                NotifyPropertyChanged(this, vm => vm.CanAddCommentBool);
            }
        }

        public void InitCommentList()
        {
            _fundAccess.GetNotes(FundDetails.ID).ContinueWith(y =>
            {
                Notes = new ObservableCollection<Note>(y.Result);
            });
        }

        public void AddComment(object param)
        {
            if (NewComment.Trim() == "")
            {
                MessageBoxShow("Please enter a value.", "Warning");
                return;
            }

            if (HasID() == false)
            {
                MessageBoxShow("Comments can not be added until this promotion has been saved", "Warning");
                return;
            }

            try
            {
                string res = _fundAccess.AddNote(FundDetails.ID, NewComment);
                //MessageBoxShow(res); 
                NewComment = "";

                InitCommentList();

            }
            catch (ExceedraDataException ex)
            {
                MessageBoxShow(ex.Message, "Error");
            }
        }

        private bool HasID()
        {
            return (!String.IsNullOrWhiteSpace(FundDetails.ID) && FundDetails.ID != "0");
        }

        #endregion

        #region Save

        //save fund
        // [app].[Procast_SP_FUND_SaveFund]
        private void Save(object obj)
        {
            SaveFund(false);
        }

        private void SaveClose(object obj)
        {
            SaveFund(true);
        }

        private void SaveFund(bool close)
        {
            XElement xValues = null;

            if (Values != null)
            {
                XDocument xdoc = new XDocument(new XElement("Fund_Values"));
                if (xdoc.Root != null) xdoc.Root.Add(XElement.Parse(Values.ToXml().ToString()));

                xValues = XElement.Parse(xdoc.ToString());
            }


            if (IsValid)
            {
                var res = _fundAccess.SaveFund(FundDetails, NewComment, SelectedSubType, xValues, StatusVM.SelectedItem.Idx, SelectedCustomerLevel, SelectedCustomerIdxs,
                           SelectedProductLevel, SelectedProductIdxs);

                var fund = res.Element("Fund");

                FundIdx = FundDetails.ID = fund.Element("Fund_Idx").Value;
                FundDetails.Amendable = fund.Element("Amendable").Value == "1";

                NotifyPropertyChanged(this, t => t.IsEditable);
                NotifyPropertyChanged(this, t => t.IsReadOnly);

                var ok = fund.Value.ToLower().Contains("success");
                CustomMessageBox.Show(fund.Element("Message").Value, (ok ? "Success" : "Error"), MessageBoxButton.OK, (ok ? MessageBoxImage.Information : MessageBoxImage.Error));

                if (ok)
                {

                    if (close)
                    {
                       CancelCommand.Execute(null);
                       return;
                    } 

                    LoadFundValues();
                    LoadNotes();
                    LoadStatuses();
                    _unsavedRows = new List<List<Property>>();
                    
                }

            }
            else
            {
                CustomMessageBox.Show("Not all required fileds are set");
            }

        }

        private string FixValue(string val)
        {
            var isNum = false;
            decimal d;
            //rip out the localised %
            val = val.Replace(CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "")
                .Replace(CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol, "");

            if (CultureInfo.CurrentCulture.IetfLanguageTag != ("en-GB"))
            {
                isNum = decimal.TryParse(val, NumberStyles.Number, CultureInfo.CurrentCulture, out d);
            }
            else
            {
                isNum = decimal.TryParse(val, NumberStyles.Any, CultureInfo.GetCultureInfoByIetfLanguageTag("en-GB"), out d);
            }

            try
            {
                var dt = Convert.ToDateTime(val);

                return dt.ToString("yyyy-MM-dd");
            }
            catch { }


            if (isNum)
            {
                var x = d.ToString(CultureInfo.GetCultureInfoByIetfLanguageTag("en-GB"));
                return x;
            }
            return val;
        }

        private bool CanSave(object obj)
        {

            return IsValid;

        }

        #endregion

        public static Task<T> FromResult<T>(T value)
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetResult(value);
            return tcs.Task;
        }

    }

}
