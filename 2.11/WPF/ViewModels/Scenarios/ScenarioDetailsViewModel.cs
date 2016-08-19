using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Coder.UI.WPF;
using Exceedra.Common;
using Exceedra.MultiSelectCombo.ViewModel;
using Exceedra.SingleSelectCombo.ViewModel;
using Model;
using Model.DataAccess;
using Model.DataAccess.Generic;
using Model.Entity;
using Model.Entity.ROBs;
using Exceedra.Common.Mvvm;
using Exceedra.Controls.Messages;
using ViewHelper;
using ViewModels;
using WPF.Pages;
using Model.Entity.Generic;
using WPF.Navigation;
using ViewModelBase = ViewModels.ViewModelBase;

namespace WPF.ViewModels.Scenarios
{
    public class ScenarioDetailsViewModel : ViewModelBase
    {
        private const string DateTimeFormatNoDashes = "yyyyMMdd";

        private enum ScenarioEditMode
        {
            All,
            Demand,
            None
        }

        private string _scenarioId;
        private readonly ScenarioAccess _scenarioAccess;
        private readonly IClientConfigurationAccess _clientConfigurationAccess;
        private string _name;
        private bool _hasChanged;
        public bool _firstLoaded = false;
        //private bool _isBusy;
        private ScenarioEditMode _scenarioEditMode;

        private List<ComboboxItem> _staticProducts;


        private SingleSelectViewModel _scenarioTypes = new SingleSelectViewModel();
        private SingleSelectViewModel _scenarioStatus = new SingleSelectViewModel();
        private SingleSelectViewModel _scenarioData = new SingleSelectViewModel();
         
        private BulkObservableCollection<ComboboxItem> _selectedUsers = new BulkObservableCollection<ComboboxItem>();
        private bool _initializing = true;

        private readonly ViewCommand _applyBaseCommand;
        private readonly ViewCommand _applyFilterCommand;
        private readonly ViewCommand _saveCommand;
        private readonly ViewCommand _saveCloseCommand;

        private ClientConfiguration _clientConfiguration;
        private DateTime? _selectedStartDate;
        private DateTime? _selectedEndDate;
        private ObservableCollection<Rob> _dataPromotion = new ObservableCollection<Rob>();
        private ObservableCollection<Rob> _dataFunding = new ObservableCollection<Rob>();

        public ScenarioDetailsViewModel(string scenarioId)
            : this(scenarioId, new ScenarioAccess(), new ClientConfigurationAccess()) { }

        public ScenarioDetailsViewModel(string scenarioId,
            ScenarioAccess scenarioAccess, IClientConfigurationAccess clientConfigurationAccess)
        {
            SetupControlEvents();

            _scenarioId = scenarioId;
            MyIdx = _scenarioId.ToString();
            _scenarioEditMode = scenarioId == "0" ? ScenarioEditMode.All : ScenarioEditMode.None;
            
            _scenarioAccess = scenarioAccess;
            _clientConfigurationAccess = clientConfigurationAccess;

            _applyBaseCommand = new ViewCommand(CanApplyBase, ApplyBase);
            _applyFilterCommand = new ViewCommand(CanApplyRobTabsFilter, ApplyFilter);
            _saveCommand = new ViewCommand(CanSave, Save);
            _saveCloseCommand = new ViewCommand(CanSave, SaveClose);
            CancelCommand = new ViewCommand(Cancel);
            ReloadCommand = new ViewCommand(Reload);

            StaticProducts = new List<ComboboxItem>();
  
            SetupEditableControls();
           
            InitData();
            LoadUserList();
           
        }

        private void Reload(object obj)
        {
            RedirectMe.Goto("scenario", ScenarioId.ToString());
            //InitData();
        }

        /// <summary>
        /// add property changed event handlers to dropdowns
        /// </summary>
        private void SetupControlEvents()
        {
            ScenarioTypes.PropertyChanged += ScenarioTypes_PropertyChanged;
            CustomerLevels.PropertyChanged += CustomerLevels_PropertyChanged;
            Customers.PropertyChanged += CustomersOnPropertyChanged;
            ProductLevels.PropertyChanged += ProductLevels_PropertyChanged;
            Products.PropertyChanged += Products_PropertyChanged;
        }

        private bool CanApplyRobTabsFilter(object obj)
        {
            return (_selectedStartDate.HasValue && _selectedEndDate.HasValue && Products.SelectedItems.Any() && Customers.SelectedItems.Any());
        }

        private void ApplyFilter(object obj)
        {
            UpdateSaveSelection();
            foreach (var t in ROBTabs)
            {
                t.LoadData();
            }

            
        }

        /// <summary>
        /// Set the editable states here as it will need db work to update the proc responses to indicate if combobox items are enabled/disabled
        /// </summary>
        private void SetupEditableControls()
        { 
            ScenarioTypes.IsEditable = AllControlsEditable;
            ScenarioData.IsEditable = DemandControlsEditable;
            CustomerLevels.IsEditable = AllControlsEditable;             
            ProductLevels.IsEditable = AllControlsEditable;
        }

        private void InitData()
        {
            if (ScenarioId > 0)
            {
                IsDataLoading = true;
            }

            var ts = new[] {               
                LoadApplicableRobs()
            };

            if (ScenarioId > 0)
            {
                _scenarioAccess.GetScenarioDetails(ScenarioId).ContinueWith(t =>
                {
                    ApplyScenarioDetails(t);
                    Task.Factory.ContinueWhenAll(ts, _ => LoadScenarioTypes(), new CancellationToken(), TaskContinuationOptions.None, App.Scheduler);
                });
            }
            else
            {
                Task.Factory.ContinueWhenAll(ts, _ => LoadScenarioTypes(), new CancellationToken(), TaskContinuationOptions.None, App.Scheduler);
                SelectedStartDate = DateTime.Today.AddMonths(-1);
                SelectedEndDate = DateTime.Today;
            }

        }

        //private void LoadEditData()
        //{
        //    _initializing = false;

        //    if (ScenarioId > 0)
        //    {
        //        _scenarioAccess.GetScenarioDetails(ScenarioId)
        //            .ContinueWith(LoadEditValues, App.Scheduler);
        //    }
        //    else
        //    {
        //        LoadCustomerLevels();
        //    }
        //}

        public void UpdateFormOnScenarioTypeChange()
        {
            LoadBaseScenarioList();
            LoadScenarioStatuses();
        }

       
        private Task LoadCustomerLevels()
        {
            return _scenarioAccess.GetCustomerLevels(AppTypeId, _idSalesOrg, GetScenarioID()).ContinueWith(t =>
            {
                CustomerLevels.SetItems(t.Result);              
            }, App.Scheduler);
        }

        private Task LoadProductLevels()
        {
            return _scenarioAccess.GetProductLevels(AppTypeId, _idSalesOrg, GetScenarioID()).ContinueWith(t =>
            {
                var tempList = t.Result;
                var comboboxItems = tempList as IList<ComboboxItem> ?? tempList.ToList();
                if (_firstLoaded)
                    comboboxItems.Do(p => p.IsSelected = false);
                 
                ProductLevels.SetItems(comboboxItems);               
            }, App.Scheduler);
        }

        //private Task LoadUsers()
        //{
        //    return _scenarioAccess.GetUsers(User.CurrentUser.ID, ScenarioId, _idSalesOrg, true).ContinueWith(t =>
        //    {
        //        UserData = new MultiSelectViewModel(t.Result);
        //    }, App.Scheduler);
        //}

        private Task LoadApplicableRobs()
        {
            return _scenarioAccess.GetApplicableRobs(Convert.ToInt32(_idSalesOrg)).ContinueWith(x =>
            {
                ApplicableRobResults = new ObservableCollection<ApplicableRobResult>(x.Result);
                //Populate(this, x, _applicableRobResult, null, vm => vm.ApplicableRobResults);
                LoadROBTabs();
            }, App.Scheduler);
        }

        private void ScenarioTypes_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem")
            {
                if (ScenarioTypes.SelectedItem != null)
                    LoadBaseScenarioList();
                    LoadCustomerLevels();
            }
        }

        private void CustomerLevels_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem" && CustomerLevels.SelectedItem != null)
            {
                //if (Customers != null && Customers.SelectedItems != null) 
                //    Customers.SelectedItems.Clear();

                _dataPromotion.Clear();
                _dataFunding.Clear();
                LoadCustomersCombo();
            }
        }

        private void LoadCustomersCombo()
        {
            _scenarioAccess.GetCustomers(AppTypeId, CustomerLevels.SelectedItem.Idx, _idSalesOrg, true, GetScenarioID().ToString())
                .ContinueWith(t =>
                {
                    var tempList = t.Result;
                    var comboboxItems = tempList as IList<ComboboxItem> ?? tempList.ToList();
                    if (_firstLoaded)
                        comboboxItems.Do(p => p.IsSelected = false);

                    Customers.SetItems(comboboxItems);
                }, App.Scheduler);
        }


        private void ProductLevels_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem" && ProductLevels.SelectedItem != null)
            {
                if (ProductLevels.SelectedItem != null)
                {
                    if (StaticProducts != null) StaticProducts.Clear();
                    _dataPromotion.Clear();
                    _dataFunding.Clear();

                    LoadProducts();
                }
                else
                {
                    ProductLevels.Clear();
                }
            }
        }


        private void Products_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItems")
            {
                UpdateSaveSelection();
 
            }
        }

        private ObservableCollection<ApplicableROBViewModel> _robTabs = new ObservableCollection<ApplicableROBViewModel>();

        public ObservableCollection<ApplicableROBViewModel> ROBTabs
        {
            get
            {
                return _robTabs;
            }
            set
            {
                _robTabs = value;
                NotifyPropertyChanged(this, vm => vm.ROBTabs);
            }
        }
         
        private static int _selectedTab { get; set; }
        public int SelectedTab { get { return _selectedTab; } set { _selectedTab = value; NotifyPropertyChanged(this, vm => vm.SelectedTab); } }
         
        private void LoadROBTabs()
        {
            if (Customers == null || Products == null) return;

            ROBTabs.Clear();
           
            IsDataLoading = true;

            var custIDs = Customers.SelectedItems.Select(c => c.Idx).Distinct().ToList();
            var prodIDs = Products.SelectedItems.Select(c => c.Idx).ToList();

            foreach (var res in ApplicableRobResults)
            {
                //load viewmodel for each tab
                var r = new ApplicableROBViewModel(Convert.ToInt32(_scenarioId),
                    _idSalesOrg, _scenarioAccess, res.ID, res.Name, custIDs, prodIDs, SelectedStartDate,
                    SelectedEndDate);
                r.SelectedPromotions.CollectionChanged += SelectedPromotionChanged;
                ROBTabs.Add(r);
            }
            SelectedTab = ROBTabs.Count() - 1;
            IsDataLoading = false;
        }

        private void SelectedPromotionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CustomMessageBox.Show("changed");
        }

        private void LoadScenarioTypes()
        {
            var scenarioId = ScenarioId == 0 ? -1 : ScenarioId;

            var args = _scenarioAccess.GetScenarioTypesArgs(_idSalesOrg, scenarioId);

            DynamicDataAccess.GetGenericEnumerableAsync<ComboboxItem>(StoredProcedure.Scenarios.GetScenarioTypes, args).ContinueWith(
                t =>
                {
                    ScenarioTypes.SetItems(t.Result);

                    if (ScenarioTypes.Items.Count == 1)
                    {
                        ScenarioTypes.SelectedItem = ScenarioTypes.Items.FirstOrDefault();
                    }
                    else
                    {
                        if (ScenarioTypes.SelectedItem == null && ScenarioDetails != null &&
                            ScenarioDetails.ItemTypeIdx != null)
                            ScenarioTypes.SelectedItem =
                                ScenarioTypes.Items.First(i => i.Idx == ScenarioDetails.ItemTypeIdx);
                    }

                    LoadScenarioStatuses();
                });                
        }
  
        private void LoadBaseScenarioList()
        {
            var selectedScenarioTypeId = ScenarioTypes.SelectedItem.Idx;
            var args = _scenarioAccess.GetScenariosArgs(_idSalesOrg, selectedScenarioTypeId, ScenarioId);

            DynamicDataAccess.GetGenericEnumerableAsync<ComboboxItem>(StoredProcedure.Scenarios.GetScenariosShort, args).ContinueWith(
            t =>
            {
                ScenarioData.SetItems(t.Result);
            });
        }

        private void LoadScenarioStatuses()
        {
            var selectedScenarioTypeId = ScenarioTypes.SelectedItem.Idx;
            var args = _scenarioAccess.GetScenarioStatusesArgs(_idSalesOrg, ScenarioId, selectedScenarioTypeId);
            DynamicDataAccess.GetDynamicDataAsync(StoredProcedure.Scenarios.GetScenarioStatuses, args).ContinueWith(
                t =>
                {
                    var items = t.Result.Elements().Select(e => new ComboboxItem(e)).ToList();

                    ScenarioStatus.SetItems(items);

                    if (ScenarioStatus.SelectedItem == null)
                        ScenarioStatus.SelectedItem = ScenarioStatus.Items.First();

                    _clientConfigurationAccess.GetClientConfigurationAsync()
                        .ContinueWith(SetUpAppType, App.Scheduler);
                });
        }

        private void SetUpAppType(Task<ClientConfiguration> task)
        {
            // todo: not sure which of these are correct, am guessing that we should set the client config, but no idea why a return statement was sitting ahead of it in code.
            // have commented the return out 20/01/2016 CH
                //return;

          _clientConfiguration = task.Result;
        }
         
        private void ApplyScenarioDetails(Task<ScenarioDataDetails> task, bool reset = false)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
            {
                IsDataLoading = false;
                FundingDataList.Clear();

                return;
            }

            if (ScenarioDetails == task.Result)
            {
                return;
            }
             
            ScenarioDetails = task.Result;                     
           
            if(reset==false) // on base change, the name should not be reset
                Name = ScenarioDetails.Name.TrimStart();


            SelectedStartDate = ScenarioDetails.StartDate;
            SelectedEndDate = ScenarioDetails.EndDate;
 
            _scenarioEditMode = (ScenarioEditMode)Enum.Parse(typeof(ScenarioEditMode), ScenarioDetails.IsEditable, true);
            NotifyPropertyChanged(this, vm => vm.AllControlsEditable, vm => vm.DemandControlsEditable, vm => vm.SaveButtonEnabled);
            SetupEditableControls();
            IsDataLoading = false;                        
        }

        private string _idSalesOrg
        {
            get { return User.CurrentUser.SalesOrganisationID.ToString(); }
        }

        //CH: changed to include endDate validation 2014-02-13
        //START changes
        private Visibility _endDateIsValid = Visibility.Collapsed;
        public Visibility EndDateIsValid
        {
            get
            {
                return _endDateIsValid;
            }
            set
            {
                _endDateIsValid = value;
                NotifyPropertyChanged(this, vm => vm.EndDateIsValid);

            }
        }

        public void SetEndDateChange()
        {
            if (SelectedStartDate.HasValue && SelectedEndDate.HasValue)
            {
                if (SelectedStartDate.Value > SelectedEndDate.Value)
                {
                    _endDateIsValid = Visibility.Visible;
                }
                else
                {
                    _endDateIsValid = Visibility.Collapsed;
                }
            }
            else
            {
                _endDateIsValid = Visibility.Visible;
            }

            RefreshROBDatesData();

            NotifyPropertyChanged(this, vm => vm.EndDateIsValid);
        }


        //End Changes

        protected string AppTypeId
        {
            get { return "1"; }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name =  value.Trim();
                NotifyPropertyChanged(this, vm => vm.Name);
            }
        }

        #region Current Scenario

        public bool NoControlsEditable { get { return _scenarioEditMode == ScenarioEditMode.None; } }
        public bool AllControlsEditable { get { return _scenarioEditMode == ScenarioEditMode.All; } }
        public bool DemandControlsEditable { get { return _scenarioEditMode != ScenarioEditMode.None; } }

        public int ScenarioId
        {
            get { return Convert.ToInt32(_scenarioId); }
            set
            {
                _scenarioId = value.ToString();
                NotifyPropertyChanged(this, vm => vm.ScenarioId);
            }
        }

        private bool _isActiveBudget;
        public bool IsActiveBudget
        {
            get { return _isActiveBudget; }
            set
            {
                _isActiveBudget = value;
                NotifyPropertyChanged(this, vm => vm.IsActiveBudget);
            }
        }

        public ScenarioDataDetails ScenarioDetails { get; set; }

        #endregion


        public bool HasChanged
        {
            get
            {
                return _hasChanged;
            }
            set
            {
                _hasChanged = value;
                NotifyPropertyChanged(this, vm => vm.HasChanged);
            }
        }

        public bool Initializing
        {
            get { return _initializing; }
        }

        public void UpdateSaveSelection()
        {
            RefreshROBCustomerData();
            RefreshROBProductData();
        }


        private ObservableCollection<SalesOrgDataViewModel> _salesOrgsCollection;
        public ObservableCollection<SalesOrgDataViewModel> SalesOrgsCollection
        {
            get
            {
                return _salesOrgsCollection;
            }
            set
            {
                _salesOrgsCollection = value;
                NotifyPropertyChanged(this, vm => vm.SalesOrgsCollection);
            }
        }

       
        private SingleSelectViewModel _customerLevels = new SingleSelectViewModel();
        public SingleSelectViewModel CustomerLevels
        {
            get { return _customerLevels; }
            set
            {
                _customerLevels = value;
                NotifyPropertyChanged(this, vm => vm.CustomerLevels);
            }
        }

        private MultiSelectViewModel _userData = new MultiSelectViewModel();
        public MultiSelectViewModel UserData
        {
            get { return _userData; }
            set
            {
                _userData = value;
                NotifyPropertyChanged(this, vm => vm.UserData);
            }
        }

        private MultiSelectViewModel _customers = new MultiSelectViewModel();
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
                    LoadProductLevels(); 
            }
        }

        private static bool _loadingComboData = false;
        private void LoadProducts(List<string> existingSelectedProductIds = null)
        {
            if (_loadingComboData == false)
            {
                if (Customers != null && Customers.SelectedItems != null && ProductLevels.SelectedItem != null)
                {
                    _loadingComboData = true;
                    _scenarioAccess.GetProducts(AppTypeId, ProductLevels.SelectedItem.Idx, _idSalesOrg,
                        Customers.SelectedItems.Select(x => x.Idx).Distinct().ToList(), true, GetScenarioID().ToString())
                        .ContinueWith(t =>
                        {
                            //var prods = t.Result.ToList();
                            //if (existingSelectedProductIds != null)
                            //{
                            //    prods.Do(p => p.IsSelected = (existingSelectedProductIds.Contains(p.Idx)));
                            //}

                            var tempList = t.Result;

                            var comboboxItems = tempList as IList<ComboboxItem> ?? tempList.ToList();
                            if (_firstLoaded)
                                comboboxItems.Do(p => p.IsSelected = false);

                            Products.SetItems(comboboxItems);

                            if (_firstLoaded == false)
                                ApplyFilter(null);


                            _firstLoaded = true;
                            _isApplyingBase = false;

                            PopulateStaticProductsAndDelist(t);

                            RefreshROBProductData();

                            _loadingComboData = false;
                        });
                }
            }
        }

        private int GetScenarioID()
        {
            if (_isApplyingBase)
            {
                return BaseScenarioID;
            }
            else
            {
                return ScenarioId;
            }
        }

        private MultiSelectViewModel _products = new MultiSelectViewModel();
        public MultiSelectViewModel Products
        {
            get
            {
                //if (_products == null) _products = new MultiSelectViewModel();
                return _products;
            }
            set
            {
                _products = value;
                NotifyPropertyChanged(this, vm => vm.Products);
            }
        }
         
        private SingleSelectViewModel _productLevels = new SingleSelectViewModel();
        public SingleSelectViewModel ProductLevels
        {
            get { return _productLevels; }
            set
            {
                _productLevels = value;
                NotifyPropertyChanged(this, vm => vm.ProductLevels);
            }
        }


        private ObservableCollection<ApplicableRobResult> _applicableRobResult;
        public ObservableCollection<ApplicableRobResult> ApplicableRobResults
        {
            get { return _applicableRobResult; }
            set
            {
                _applicableRobResult = value;
                NotifyPropertyChanged(this, vm=>vm.ApplicableRobResults);
            }

        }

        private void RefreshROBDatesData()
        {
            foreach (var r in ROBTabs)
            {
                r.SelectedEndDate = SelectedEndDate;
                r.SelectedStartDate = SelectedStartDate;
            }
        }
        private void RefreshROBCustomerData()
        {
            var custIDs = Customers.SelectedItems.Select(c => c.Idx).Distinct().ToList();

            foreach (var r in ROBTabs)
            {
                r.CustomerIDs = new ObservableCollection<string>(custIDs);
            }
        }


        /// <summary>
        /// Sets the products available to the grids, user needs to hit Apply to reload each grid
        /// </summary>
        private void RefreshROBProductData()
        {
            var prodIDs = Products.SelectedItems.Select(c => c.Idx).ToList();

            foreach (var r in ROBTabs)
            {
                r.ProductIDs = new ObservableCollection<string>(prodIDs);
            }
        }

        private Task LoadUserList()
        {
            if (_idSalesOrg != "")
            {
             
                return _scenarioAccess.GetUsers(User.CurrentUser.ID, Convert.ToInt32(_scenarioId), _idSalesOrg, true).ContinueWith(t =>
                {
                    UserData.SetItems(t.Result);

                    if (_scenarioId == "0")
                        LoadCleanUserList();
                });
            }

            return Task.Factory.Completed();
        }

        private void LoadCleanUserList()
        {
            _selectedUsers.AddRange(UserData.Items.Where(userData => userData.Idx == User.CurrentUser.ID));
            NotifyPropertyChanged(this, vm => vm.SelectedUsers);
            foreach (var userData in UserData.Items.Where(userData => userData.Idx == User.CurrentUser.ID))
                userData.IsSelected = true;
        }

        private void PopulateSelectedUsers()
        {
            if (UserData == null) 
                return;
 
            foreach (var userData in UserData.Items.Where(u => u.IsSelected))         
            {
                _selectedUsers.Add(userData);
            }

            NotifyPropertyChanged(this, vm => vm.SelectedUsers);
        }


        public SingleSelectViewModel ScenarioStatus
        {
            get { return _scenarioStatus; }
            set { _scenarioStatus = value; }
        }
         
        public SingleSelectViewModel ScenarioData
        {
            get { return _scenarioData; }
            set { _scenarioData = value; }
        }

        public BulkObservableCollection<ComboboxItem> SelectedUsers
        {
            get
            {
                return _selectedUsers;
            }
            set
            {
                _selectedUsers = value;
                NotifyPropertyChanged(this, vm => vm.SelectedUsers);
                if (SelectedUsers.Count() > 0) { CanSelectedUsersSave = true; } else { CanSelectedUsersSave = false; }
            }
        }
        private bool m_canSelectedUsersSave;
        public bool CanSelectedUsersSave
        {
            get { return m_canSelectedUsersSave; }
            set { m_canSelectedUsersSave = value; NotifyPropertyChanged(this, vm => vm.CanSelectedUsersSave); }
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

        private void PopulateStaticProductsAndDelist(Task<IEnumerable<ComboboxItem>> task)
        {
            StaticProducts.AddRange(task.Result);

        }

        private void DelistProducts()
        {
            if (StaticProducts != null)
            {              
                Products.SetItems(StaticProducts.Where(a => a.DelistingsDate == null || a.DelistingsDate >= SelectedEndDate));
            }
        }

        public void UpdateProductSelectedForCustomers()
        {
            List<string> existingSelectedProductIds = null;
            if (Products.SelectedItems.Count > 0)
            {
                existingSelectedProductIds = Products.SelectedItems.Select(prd => prd.Idx).ToList();
            }

            LoadProducts(existingSelectedProductIds);
        }

        private void PopulateSelectedProductsCollection()
        {
            if (Products.SelectedItems != null)
                Products.SelectedItems.Clear();
        }

        public SingleSelectViewModel ScenarioTypes
        {
            get { return _scenarioTypes; }
            set { _scenarioTypes = value; }
        }

        public bool SaveButtonEnabled
        {
            get
            {
                return (this.AllControlsEditable || this.DemandControlsEditable);
            }
        }

        //public BulkObservableCollection<Status> FundingStatus
        //{
        //    get
        //    {
        //        return _fundingStatus;
        //    }
        //}

        //public BulkObservableCollection<Status> PromotionStatus
        //{
        //    get
        //    {
        //        return _promotionStatus;
        //    }
        //}


        //public ObservableCollection<Status> SelectedPromotionStatus
        //{
        //    get { return _selectedPromotionStatus; }
        //    set
        //    {
        //        _selectedPromotionStatus = value;
        //        NotifyPropertyChanged(this, vm => vm.SelectedPromotionStatus);
        //    }
        //}
        //public ObservableCollection<Status> SelectedFundingStatus
        //{
        //    get { return _selectedFundingStatus; }
        //    set
        //    {
        //        _selectedFundingStatus = value;
        //        NotifyPropertyChanged(this, vm => vm.SelectedFundingStatus);
        //    }
        //}

        private PromotionDataViewModel _currentPromotion;

        private static bool _isDataLoading;
        public bool IsDataLoading
        {
            get { return _isDataLoading; }
            set
            {
                _isDataLoading = value;
                NotifyPropertyChanged(this, vm => vm.IsDataLoading);
            }
        }

        public bool IsDataLoaded { get { return !IsDataLoading; } }

        public PromotionDataViewModel CurrentPromotion
        {
            get { return _currentPromotion; }
            set
            {
                _currentPromotion = value;
                NotifyPropertyChanged(this, vm => vm.CurrentPromotion);
            }
        }

        public DateTime? SelectedStartDate
        {
            get
            {
                return _selectedStartDate;
            }
            set
            {
                _selectedStartDate = value;
                NotifyPropertyChanged(this, vm => vm.SelectedStartDate);
                if (SelectedStartDate != null) { CanSelectedStartDateSave = true; } else { CanSelectedStartDateSave = false; }
                RefreshROBDatesData();
            }
        }
        private bool m_canSelectedStartDateSave;
        public bool CanSelectedStartDateSave
        {
            get { return m_canSelectedStartDateSave; }
            set { m_canSelectedStartDateSave = value; NotifyPropertyChanged(this, vm => vm.CanSelectedStartDateSave); }
        }

        public DateTime? SelectedEndDate
        {
            get
            {
                return _selectedEndDate;
            }
            set
            {
                _selectedEndDate = value;
                NotifyPropertyChanged(this, vm => vm.SelectedEndDate);
                SetEndDateChange();
                //DelistProducts();
            }
        }

        public ICommand ApplyBaseCommand
        {
            get { return _applyBaseCommand; }
        }

        public ICommand ApplyFilterCommand
        {
            get { return _applyFilterCommand; }
        }
        //public ICommand ApplyFilterPromotionsCommand
        //{
        //    get { return _applyfilterPromotionsCommand; }
        //}

        public ICommand SaveCommand
        {
            get { return _saveCommand; }
        }

        public ICommand SaveCloseCommand
        {
            get { return _saveCloseCommand; }
        }


        public ICommand CancelCommand { get; set; }
        public ICommand ReloadCommand { get; set; }


        //public ICommand ApplyFilterFundingCommand
        //{
        //    get { return _applyfilterFundingCommand; }
        //}

        //public ICommand EditPromotionCommand
        //{
        //    get { return new ViewCommand(EditPromotion); }
        //}

        //public ICommand EditFundingCommand
        //{
        //    get { return new ViewCommand(EditFunding); }
        //}

        private void EditFunding(object o)
        {


            var rob = (Rob)o;
            var code = rob.Code.Split(',');
            if (code[0] != "1")
            {
                _scenarioAccess.GetRob(code[0], rob.ID)
                .ContinueWith(EditRobContinuation, App.Scheduler);
            }
            else
            {
                PromotionWizardViewModelBase editingPromoWizardViewModel = new PromotionWizardViewModel(null, rob.ID);
            }


        }

        private void EditRobContinuation(Task<Rob> task)
        {
            if (task.IsFaulted)
            {
                Messages.Instance.PutError(task.Exception.AggregateMessages());
                return;
            }
            if ((!task.IsCanceled) && task.Result != null)
            {
                var page = new EventPage(EventViewModel.FromRob(_clientConfiguration.FundingAppTypeId, "Funding", task.Result));
                MessageBus.Instance.Publish(new NavigateMessage(page));
            }
        }

        //public ObservableCollection<Rob> PromotionDataList
        //{
        //    get
        //    {
        //        return _dataPromotion;
        //    }
        //    set
        //    {
        //        _dataPromotion = new ObservableCollection<Rob>(value);
        //        NotifyPropertyChanged(this, vm => vm.PromotionDataList);
        //    }
        //}

        public ObservableCollection<Rob> FundingDataList
        {
            get
            {
                return _dataFunding;
            }
            set
            {
                _dataFunding = new ObservableCollection<Rob>(value);
                NotifyPropertyChanged(this, vm => vm.FundingDataList);
            }
        }
        public bool IsFundingSelectAllEnabled
        {
            get { return _isFundingSelectAllEnabled; }
            set
            {
                _isFundingSelectAllEnabled = value;
                NotifyPropertyChanged(this, vm => vm.IsFundingSelectAllEnabled);
            }
        }

        public bool IsFundingSelectAllChecked
        {
            get { return _isFundingSelectAllChecked; }
            set
            {
                _isFundingSelectAllChecked = value;
                NotifyPropertyChanged(this, vm => vm.IsFundingSelectAllChecked);
            }
        }

        public bool IsPromotionSelectAllEnabled
        {
            get { return _isPromotionSelectAllEnabled; }
            set
            {
                _isPromotionSelectAllEnabled = value;
                NotifyPropertyChanged(this, vm => vm.IsPromotionSelectAllEnabled);
            }
        }
        public bool IsPromotionSelectAllChecked { get; set; }

        private bool _isApplyingBase;
        private void ApplyBase(object obj)
        {

            _isApplyingBase = true;
            const string message = "Are you sure you want to apply these settings?";

            if (MessageBox.Show(message, "Cancel", MessageBoxButton.YesNo, MessageBoxImage.Question) ==
                MessageBoxResult.No)
            {
                _isApplyingBase = false;
                return;
            }

            IsDataLoading = true;

            BaseScenarioID = int.Parse(ScenarioData.SelectedItem.Idx);
            _scenarioAccess.GetScenarioDetails(BaseScenarioID)
                .ContinueWith(t =>
                {
                
                    ApplyScenarioDetails(t, true);                    
                    //_isApplyingBase = false;
                    _scenarioEditMode = ScenarioEditMode.All;
                    NotifyPropertyChanged(this, vm => vm.AllControlsEditable, vm => vm.DemandControlsEditable, vm => vm.SaveButtonEnabled);  
                    IsDataLoading = false;


                    _firstLoaded = false;
                    LoadCustomerLevels().ContinueWith(_ =>
                    {
                       
                    });

                }, App.Scheduler);
              
        }

        //private void ScenarioEnabledControlsContinuation(Task<string> task)
        //{
        //    _scenarioEditMode = (ScenarioEditMode)Enum.Parse(typeof(ScenarioEditMode), task.Result, true);
        //    NotifyPropertyChanged(this, vm => vm.AllControlsEditable, vm => vm.DemandControlsEditable, vm => vm.SaveButtonEnabled);
        //}

        private bool CanApplyBase(object obj)
        {
            if (_isApplyingBase || _isDataLoading || _isSaving )//|| string.IsNullOrEmpty(Name)
                return false;
            return ScenarioData.SelectedItem != null;
        }

        private bool _isSaving;
        private bool _isFundingSelectAllChecked;
        private bool _isFundingSelectAllEnabled;
        private bool _isPromotionSelectAllEnabled;

        private static int _baseScenarioID;
        public int BaseScenarioID
        {
            get { return _baseScenarioID; }
            set { _baseScenarioID = value; }
        }

        private void Save(object obj)
        {
           SaveScenario(false);
          
        }

        private void SaveClose(object obj)
        {
            SaveScenario(true);
        }

        private void SaveScenario(bool close)
        {
            List<string> FundingDataList = new List<string>();
            List<string> PromotionDataList = new List<string>();
            foreach (var rob in ROBTabs)
            { //add each child selected rob to the mix
                if (rob.AppTypeId != "1")
                { FundingDataList.AddRange(rob.DataList.Where(r => r.IsSelected).Select(e => e.ID).ToList()); }
                else
                { PromotionDataList.AddRange(rob.DataList.Where(r => r.IsSelected).Select(e => e.ID).ToList()); }

            }

            _isSaving = true;

            try
            {
                var Result = _scenarioAccess.SaveScenario(_scenarioId.ToString(CultureInfo.InvariantCulture), _idSalesOrg, Name, ScenarioTypes.SelectedItem.Idx,
               ScenarioStatus.SelectedItem.Idx,
               _selectedStartDate.GetValueOrDefault().ToString(DateTimeFormatNoDashes),
               _selectedEndDate.GetValueOrDefault().ToString(DateTimeFormatNoDashes),
               (CustomerLevels.SelectedItem == null ? CustomerLevels.Items.FirstOrDefault(t => t.IsSelected).Idx : CustomerLevels.SelectedItem.Idx),
               ProductLevels.SelectedItem.Idx,
               Customers.SelectedItems.Select(r => r.Idx).Distinct().ToList(),
               Products.SelectedItems.Select(r => r.Idx).ToList(),
               PromotionDataList,
               FundingDataList,
               UserData.SelectedItems.Select(q => q.Idx).Distinct().ToList(),
               BaseScenarioID, IsActiveBudget);

                //.ContinueWith(t =>
                //{
                _isSaving = false;
                try
                {
                    List<ScenarioResult> resSaveScenario = Result.ToList();
                    if (resSaveScenario[0].Outcome == "Save Successful" ||
                        resSaveScenario[0].Outcome == "New Scenario Created")
                    {
                        MessageBoxShow("This Scenario has been saved correctly", "New Scenario");
                        ScenarioId = int.Parse(resSaveScenario[0].ID);
                    }
                    else
                    {
                        MessageBoxShow(resSaveScenario[0].Outcome.ToString(CultureInfo.InvariantCulture),
                            "New Scenario");
                    }

                    if(close) Close();
 
                }
                catch (Exception ex)
                {

                    _isSaving = false;
                }
                 
                // }, App.Scheduler);
            }
            catch (Exception ex)
            {
                MessageBoxShow(string.Format("This Scenario has not been saved \n {0}", ex.Message), "New Scenario");
                _isSaving = false;
            }

        }

        private void Cancel(object o)
        {
            string message = "Are you sure you want to cancel the curent operation?";

            if (CustomMessageBox.Show(message, "Cancel", MessageBoxButton.YesNo, MessageBoxImage.Question) ==
                MessageBoxResult.No)
                return;

            Close();
        }

        private void Close()
        {
            RedirectMe.ListScreen("Scenarios");
        }

        private bool CanSave(object obj)
        {
            if (_isApplyingBase || _isDataLoading || _isSaving)// || !_hasChanged)
                return false;

            //CH: changed to include date validation 2014-02-13

            if (!string.IsNullOrEmpty(Name)
                && ScenarioTypes.SelectedItem != null
                && ScenarioStatus.SelectedItem != null
                && UserData.SelectedItems.Count > 0
                && Customers != null && Customers.SelectedItems.Count > 0
                && Products != null && Products.SelectedItems.Count > 0
                && SelectedStartDate != null
                && SelectedEndDate != null
                && EndDateIsValid == Visibility.Collapsed) return true;
            
            return false;            
        }

        #region "Comments"
        private void LoadNotes()
        {
            // [app].[Procast_SP_FUND_GetComments]
            _scenarioAccess.GetNotes(ScenarioDetails.Id).ContinueWith(
                t =>
                {
                    Notes = new ObservableCollection<Note>(t.Result);
                });
        }

        private bool HasID()
        {
            if (ScenarioDetails == null)
            {
                return false;
            }

            return (!String.IsNullOrWhiteSpace(ScenarioDetails.Id) && ScenarioDetails.Id != "0");
        }

        private ObservableCollection<Note> _notes;
        public ObservableCollection<Note> Notes
        {
            get { return _notes; }
            set
            {
                _notes = value;
                NotifyPropertyChanged(this, vm => vm.Notes);
            }
        }

        private string _newComment;
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

        public ICommand AddCommentCommand
        {
            get { return new ViewCommand(CanAddComment, AddComment); }
        }

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
            _scenarioAccess.GetNotes(ScenarioDetails.Id).ContinueWith(y =>
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
                string res = _scenarioAccess.AddNote(ScenarioDetails.Id, NewComment);
                //MessageBoxShow(res); 
                NewComment = "";

                InitCommentList();

            }
            catch (ExceedraDataException ex)
            {
                MessageBoxShow(ex.Message, "Error");
            }
        }


        #endregion

    }
}


