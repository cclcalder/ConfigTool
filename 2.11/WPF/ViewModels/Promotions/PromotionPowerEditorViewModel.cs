using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Model;
using Model.DataAccess;
using ViewHelper;
using ViewModels;
using WPF.UserControls.Listings;
using System.Xml.Linq;
using Exceedra.Controls.DynamicGrid.ViewModels;
using Exceedra.Controls.DynamicRow.ViewModels;
using Model.Entity;
using Model.Entity.PowerPromotion;
using WPF.UserControls.Trees.DataAccess;
using WPF.UserControls.Trees.ViewModels;
using ICommand = System.Windows.Input.ICommand;
using System.ComponentModel;
using Exceedra.Common;
using Exceedra.Common.Utilities;
using Model.DataAccess.Listings;
using Model.Entity.Generic;
using Model.Entity.Listings;

namespace WPF.ViewModels.PromotionPowerEditor
{
    public class PromotionPowerEditorViewModel : ViewModelBase
    {
        private readonly PromotionAccess _promoDataAccess = new PromotionAccess();
        private ListingsAccess _listingsAccess = new ListingsAccess();

        private readonly ViewCommand _cancelCommand;

        private string _promoIdx;

        public PromotionPowerEditorViewModel()
        {
            _cancelCommand = new ViewCommand(Cancel);
            _uploadSubCustomersViaCsvCommand = new ViewCommand(AnySubCustomerAvailable, LoadSubCustomersFromCsv);
            SelectedPageIdx = 0;
            InitPage1();
        }

        public PromotionPowerEditorViewModel(string promoIdx)
        {
            _cancelCommand = new ViewCommand(Cancel);
            _uploadSubCustomersViaCsvCommand = new ViewCommand(AnySubCustomerAvailable, LoadSubCustomersFromCsv);
            SelectedPageIdx = 0;
            _promoIdx = promoIdx;

            InitPage1();
        }

        public void InitPage1()
        {
            Task[] firstTaskSet =
            {
                LoadListings(), 
                LoadBaseData()
            };

            Task.Factory.ContinueWhenAll(firstTaskSet, t =>
            {
                Task[] secondTaskSet =
                {
                    LoadDateList(),
                    LoadDatePeriods(),
                    LoadSubCustomers()
                };

                Task.Factory.ContinueWhenAll(secondTaskSet, tt =>
                {
                    Task[] thirdTaskSet =
                    {
                        LoadAttributes()
                    };

                    Task.Factory.ContinueWhenAll(thirdTaskSet, ttt =>
                    {
                        Tab1Serialized = GetTab1Serialization();
                    });

                });

            });

        }

        public void InitPage2()
        {
            if (_promoIdx == null)
            {
                SelectedPageIdx = 0;
                return;
            }

            Task[] page2Tasks =
            {
                LoadFinancials(),
                LoadPAndLGrid(),
                LoadPromotionStatuses(),
                LoadPromotionScenarios()
            };

            SelectedPageIdx = 1;

            Task.Factory.ContinueWhenAll(page2Tasks, t =>
            {
                Dispatcher.Invoke(new Action(FireG1PromoFinancialsChanges));

                Tab2Serialized = GetTab2Serialization();
                if ((FinancialsRowVM != null && FinancialsRowVM.Records.Any()))
                {
                    foreach (var col in FinancialsRowVM.Records.ToList())
                    {

                        foreach (var p in col.Properties)
                        {
                            if (p.ControlType.Contains("down"))
                            {
                                col.InitialDropdownLoad(p);
                            }
                            p.PropertyChanged += G1PromoFinancials_PropertyChanged;

                        }

                    }


                }

                LoadTopRightGrid();
                LoadMiddleLeftGrid();
                LoadMiddleCentreGrid();
                LoadMiddleRightGrid();
                LoadBottomRightGrid();
            });

        }

        #region Global

        #region Title

        private string _code;

        public string Code
        {
            get { return _code; }
            set
            {
                _code = value;
                NotifyPropertyChanged(this, vm => vm.Code);
            }
        }

        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged(this, vm => vm.Name);
            }
        }

        private string _initalStatusIdx;

        private bool IsValidTitle()
        {
            return !String.IsNullOrWhiteSpace(Name);
        }

        public void LoadTitle()
        {
            PromotionGetResults promotionResults = _promoDataAccess.GetPromotion("Customer", LastSaved, _promoIdx);

            Code = promotionResults.CodeAndName;
            Name = promotionResults.Name;
        }

        #endregion

        #region Reload

        public ICommand ReloadCommand
        {
            get { return new ViewCommand(Reload); }
        }

        private void Reload(object sender = null)
        {
            if (SelectedPageIdx == 0)
                InitPage1();
            else
                InitPage2();
        }

        #endregion

        #region Cancel

        public ICommand CancelCommand
        {
            get { return _cancelCommand; }
        }

        private void Cancel(object sender)
        {
            GoToPromotions(true);
        }

        public void GoToPromotions(object parameter)
        {
            var page = new Promotions((bool)parameter);
            App.Navigator.EnableNavigation(true);
            App.Navigator.NavigateTo(page);
        }

        #endregion

        #endregion

        #region Tab 1

        #region UI Data Objects

        #region Listings

        private ListingsViewModel _listingsVM;
        public ListingsViewModel ListingsVM
        {
            get { return _listingsVM; }
            set
            {
                _listingsVM = value;
                NotifyPropertyChanged(this, vm => vm.ListingsVM);
            }
        }

        private Task LoadListings()
        {
            return FromResult(false).ContinueWith(t =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    _listingsAccess = new ListingsAccess(_promoIdx);
                    ListingsVM = new ListingsViewModel(ListingsAccess.GetFilterCustomers().Result, ListingsAccess.GetFilterProducts().Result, ScreenKeys.PROMOTION.ToString());
                    ListingsVM.IsReadOnly = IsReadOnly;
                    ListingsVM.FilterCheckAndUpdate();
                    SelectedCustomerIdx = ListingsVM.SelectedCustomers.First().Idx;
                }));
            });
        }

        private XElement ProductIdxToXml(List<string> idxList)
        {
            XElement products = new XElement("Products");
            foreach (var p in idxList)
            {
                products.AddElement("Product_Idx", p);
            }
            return products;
        }


        #endregion

        #region Dates

        private ObservableCollection<PromotionDateViewModel> _dateList;

        public ObservableCollection<PromotionDateViewModel> DateList
        {
            get { return _dateList; }
            set
            {
                if (_dateList != value)
                {
                    _dateList = value;

                    NotifyPropertyChanged(this, vm => vm.DateList);
                }



            }
        }
        private IEnumerable<PromotionDatePeriod> _periods;

        public IEnumerable<PromotionDatePeriod> Periods
        {
            get { return _periods; }
            set
            {
                _periods = value;
                SelectedPeriod = Periods.FirstOrDefault(p => p.IsSelected);
                NotifyPropertyChanged(this, vm => vm.Periods);
            }
        }

        private bool _updatingSelectedPeriod;

        private PromotionDatePeriod _selectedPeriod;
        public PromotionDatePeriod SelectedPeriod
        {
            get
            {
                return _selectedPeriod;
            }
            set
            {
                if (value == null) return;
                _selectedPeriod = value;
                UpdateDateList(value);
                NotifyPropertyChanged(this, vm => vm.SelectedPeriod);
            }
        }

        private void UpdateDateList(PromotionDatePeriod period)
        {
            // This ordering guarantees that start dates will get set before end dates.
            // The default behaviour is that the end date gets set to the start date plus
            // the offset when the start date gets set.
            if (period == null) return;
            foreach (var periodDate in period.PromoDates.OrderBy(pd => pd.DateGroupType))
            {
                var dateGroupId = periodDate.DateGroupID;
                foreach (var promotionDate in DateList.Where(pd => pd.ID == dateGroupId))
                {
                    switch (periodDate.DateGroupType)
                    {
                        case PromotionDatePeriod.PeriodDate.PeriodDateType.Start:
                            promotionDate.StartDate = periodDate.Value;
                            break;
                        case PromotionDatePeriod.PeriodDate.PeriodDateType.End:
                            promotionDate.EndDate = periodDate.Value;
                            break;
                    }
                }
            }
        }

        public Task LoadDateList()
        {
            return FromResult(false).ContinueWith(t =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    IEnumerable<PromotionDate> promoDates;

                    if (_promoIdx != null)
                    {
                        promoDates =
                            _promoDataAccess.GetPowerPromotionDates(null, _promoIdx).ToList();
                    }
                    else
                    {
                        promoDates =
                            _promoDataAccess.GetPowerPromotionDates(ListingsVM.CustomerIDsList.First()).ToList();
                    }

                    DateList = new ObservableCollection<PromotionDateViewModel>(
                        promoDates.Select(d => new PromotionDateViewModel(d)));




                    // Initialize date values with Today, when the promotion is new
                    if (_promoIdx == null)
                    {
                        foreach (dynamic d in DateList)
                        {
                            d.StartDate = DateTime.Now;
                            d.EndDate = DateTime.Now;
                        }
                    }
                }));
            });
        }

        private Task LoadDatePeriods()
        {
            return FromResult(false).ContinueWith(t =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    Periods = _promoDataAccess.GetPowerPromotionDatePeriods(ListingsVM.CustomerIDsList.First(), _promoIdx);
                }));
            });
        }

        private bool IsValidDates()
        {
            // CHAINING VALIDATION
            //if (!IsValidCustomer()) return false;

            if (DateList == null) return false;

            foreach (var d in DateList)
            {
                if (d.StartDate > d.EndDate)
                {
                    return false;
                }
            }

            return true;
        }

        private XElement DatesToXml(ObservableCollection<PromotionDateViewModel> dateCollection)
        {
            XElement dates = new XElement("Dates");
            foreach (var date in dateCollection)
            {
                XElement dateType = new XElement("DateType");
                dateType.AddElement("DateType_Idx", date.ID);
                dateType.AddElement("StartValue", date.StartDate);
                dateType.AddElement("EndValue", date.EndDate);
                dates.Add(dateType);
            }
            return dates;
        }

        #endregion

        #region Attributes

        private RowViewModel _attributesRvm;
        public RowViewModel AttributesRVM
        {
            get
            {
                return _attributesRvm;
            }
            set
            {
                if (_attributesRvm != value)
                {
                    _attributesRvm = value;
                    NotifyPropertyChanged(this, vm => vm.AttributesRVM);
                }
            }
        }

        private Task LoadAttributes()
        {
            return _promoDataAccess.GetPowerPromotionAttributesAsync(GetPage1AsPowerPromotion(), true).
                ContinueWith(t => AttributesContinuation(t), App.Scheduler);
        }

        private void RefreshAttributes()
        {
            _promoDataAccess.GetPowerPromotionAttributesAsync(GetPage1AsPowerPromotion(), true)
                .ContinueWith(t => AttributesContinuation(t, true), App.Scheduler);
        }

        private XElement AttributesContinuation(Task<XElement> res, bool isRefresh = false)
        {

            if (res.Result == null)
            {
                AttributesRVM = null;
            }
            else
            {
                AttributesRVM = RowViewModel.LoadWithData(res.Result);
            }

            foreach (var col in AttributesRVM.Records.ToList())
            {
                //For every dropdown we need to call a proc for the internal data.
                //We grab the power specific objs here and then run it through the generic loader in the control.
                foreach (var p in col.Properties.Where(r => r.ControlType.Contains("down")).ToList())
                {
                    if (_promoIdx == null || isRefresh)
                    {
                        var ripProperty = XElement.Parse(p.DataSourceInput.Replace("&gt;", ">").Replace("&lt;", "<"));
                        foreach (var node in ripProperty.Elements())
                        {
                            switch (node.Name.ToString())
                            {
                                case "Customer":
                                    node.Value = ListingsVM.CustomerIDsList[0];
                                    break;
                                case "Products":
                                    node.Value = ProductIdxToXml(ListingsVM.ProductIDsList).ToString();
                                    break;
                                case "Dates":
                                    node.Value = DatesToXml(DateList).ToString();
                                    break;
                                case "IsPowerEditorMode":
                                    node.Value = "1";
                                    break;
                            }
                        }
                        p.DataSourceInput = ripProperty.ToString();
                    }
                    else
                    {
                        p.ParentIDx = _promoIdx;
                    }

                    col.InitialDropdownLoad(p);
                }

            }

            Tab1SerializedAttributeInputs = GetTab1SerializedAttributeInputs();
            CommandManager.InvalidateRequerySuggested();

            return new XElement("Empty");
        }

        private bool IsValidAttributes()
        {
            if (AttributesRVM == null) return false;

            var nonSelectedDropdowns =
                AttributesRVM.Records.SelectMany(record => record.Properties).Where(prop =>
                prop.ControlType.ToLower().Contains("dropdown")
                    &&    (prop.SelectedItems == null || !prop.SelectedItems.Any())
                    && prop.SelectedItem == null
                    && String.IsNullOrWhiteSpace(prop._innerValue));

            if (nonSelectedDropdowns.Any()) return false;

            return true;
        }

        private bool _autoRefresh;
        public bool AutoRefresh
        {
            get { return _autoRefresh; }
            set
            {
                _autoRefresh = !IsReadOnly && value;
                NotifyPropertyChanged(this, vm => vm.AutoRefresh);
            }
        }

        #endregion

        #region SubCustomers

        private bool AnySubCustomerAvailable(object obj)
        {
            return VisibleSubCustomers != null && VisibleSubCustomers.Children.Any();
        }

        private List<TreeViewHierarchy> _selectedSubCustomers;
        public List<TreeViewHierarchy> SelectedSubCustomers
        {
            get { return (_selectedSubCustomers ?? new List<TreeViewHierarchy>()); }
            set
            {
                _selectedSubCustomers = value.ToList();
                NotifyPropertyChanged(this, vm => vm.SelectedSubCustomers);
            }
        }

        private Task LoadSubCustomers()
        {
            return FromResult(false).ContinueWith(t =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    if ((ListingsVM.SelectedCustomers != null && ListingsVM.SelectedCustomers.Any()) || _promoIdx != null)
                    {
                        var res = _promoDataAccess.GetPowerEditorSubCustomers(User.CurrentUser.ID,
                            ListingsVM.CustomerIDsList.First(), _promoIdx);

                        AllNode = new TreeViewHierarchy(res);

                        SetVisibleSubCustomers(true);
                    }
                    else
                    {
                        VisibleSubCustomers = null;
                    }
                }));
            });
        }

        public void SetVisibleSubCustomers(bool loadingFromDB)
        {
            if (VisibleSubCustomers == null || !VisibleSubCustomers.Children.Any())
            {
                if (AllNode != null)
                    VisibleSubCustomers = AllNode;
            }

            if (ListingsVM.CustomerIDsList.First() != null && VisibleSubCustomers != null)
            {
                if (!loadingFromDB)
                {
                    #region Summary
                    /* Get the current selected Idxs
                     * Remove any subcustomers
                     * Re-apply the selected customers
                     * Add any subcustomers
                     * Revaluate the trees parent nodes to check correct states.
                     */
                    #endregion

                    var selectedIdxs = SubCustomersTreeInput.GetSelectedIdxs();

                    var childrenToRemove = new List<TreeViewHierarchy>();
                    foreach (var child in VisibleSubCustomers.Children)
                    {
                        if (!AllNode.Children.Select(c => c.Idx).Contains(child.Idx))
                            childrenToRemove.Add(child);
                    }
                    VisibleSubCustomers.Children.Remove(childrenToRemove);

                    SetSelected(VisibleSubCustomers, selectedIdxs);

                    foreach (var child in AllNode.Children)
                    {
                        if (!VisibleSubCustomers.Children.Select(c => c.Idx).Contains(child.Idx))
                            VisibleSubCustomers.Children.Add(child);
                    }

                    CheckAllStates(VisibleSubCustomers);
                }
                else
                {
                    VisibleSubCustomers = AllNode;
                }

                if (VisibleSubCustomers.Children.Any())
                    SubCustomersTreeInput = new TreeViewModel
                    {
                        Listings = VisibleSubCustomers,
                        IsReadOnly = IsReadOnly
                    };
                else
                    SubCustomersTreeInput = new TreeViewModel();

            }
            else
            {
                VisibleSubCustomers = new TreeViewHierarchy();
                SubCustomersTreeInput = new TreeViewModel();
            }
        }

        private void CheckAllStates(TreeViewHierarchy ti)
        {
            if (ti.Children.Any())
            {
                foreach (var c in ti.Children)
                    CheckAllStates(c);

                if ((ti.Children.Any(c => c.IsSelected == "0") && ti.Children.Any(c => c.IsSelected == "1")) || ti.Children.Any(c => c.IsSelected == null))
                    ti.IsSelected = null;
                else if ((ti.Children.All(c => c.IsSelected == "1")))
                    ti.IsSelected = "1";
                else
                    ti.IsSelected = "0";
            }
        }

        private void SetSelected(TreeViewHierarchy ti, List<string> idxs)
        {
            if (idxs.Contains(ti.Idx))
            {
                SetSelfAndChildrenAsSelected(ti);
            }
            else
            {
                foreach (var c in ti.Children)
                    SetSelected(c, idxs);
            }
        }

        public void SetSelfAndChildrenAsSelected(TreeViewHierarchy ti)
        {
            ti.IsSelected = "1";
            if (ti.Children.Any())
                ti.Children.Do(SetSelfAndChildrenAsSelected);
        }

        private TreeViewHierarchy _visibableSubCustomers;
        public TreeViewHierarchy VisibleSubCustomers
        {
            get { return _visibableSubCustomers; }
            set
            {
                _visibableSubCustomers = value;
                NotifyPropertyChanged(this, vm => vm.VisibleSubCustomers);
            }
        }

        private TreeAccess _treeAccess = new TreeAccess();
        private TreeViewModel _subCustomersTreeInput;
        public TreeViewModel SubCustomersTreeInput
        {
            get { return _subCustomersTreeInput; }
            set
            {
                _subCustomersTreeInput = value;
                NotifyPropertyChanged(this, vm => vm.SubCustomersTreeInput);
            }
        }

        private TreeViewHierarchy _allNode;
        public TreeViewHierarchy AllNode
        {
            get { return _allNode; }
            set
            {
                _allNode = value;
                NotifyPropertyChanged(this, vm => vm.AllNode);
            }
        }

        #region Load SubCustomer from CSV

        public ICommand UploadSubCustomersViaCsvCommand
        {
            get { return _uploadSubCustomersViaCsvCommand; }
        }

        private ViewCommand _uploadSubCustomersViaCsvCommand;



        private void LoadSubCustomersFromCsv(object obj)
        {
            IEnumerable<string> fileContent = IOService.ReadCsvFile();
            if (fileContent == null) return;

            try
            {
                //needs to make tree using current cust as top. 
                var res = _promoDataAccess.GetPowerEditorSubCustomers(_promoIdx, User.CurrentUser.ID, ListingsVM.SelectedCustomers.FirstOrDefault().Idx, fileContent);
                AllNode = new TreeViewHierarchy(res);

                SetVisibleSubCustomers(true);
            }
            catch (Exception ex)
            {

            }
        }

        #endregion

        #endregion

        #endregion

        #region Commands

        #region Constructors

        public ICommand GoToPage2Command
        {
            get { return new ViewCommand(CanPage1Navigate, NavigateToPage2); }
        }

        public ICommand GoToPage1Command
        {
            get { return new ViewCommand(CanPage2Navigate, NavigateToPage1); }
        }

        public ICommand SavePage1Command
        {
            get { return new ViewCommand(CanPage1Navigate, SavePage1); }
        }

        public ICommand SavePage2Command
        {
            get { return new ViewCommand(CanPage2Save, SavePage2); }
        }

        public ICommand SaveClosePage2Command
        {
            get { return new ViewCommand(CanPage2Save, SaveClosePage2); }
        }

        public ICommand RefreshAttributesCommand
        {
            get { return new ViewCommand(CanRefreshAttributes, RefreshAttributes); }
        }

        public ICommand ReloadSubCustomersCommand
        {
            get { return new ViewCommand(CanReloadSubcustomers, ReloadSubCustomers); }
        }

        private string SelectedCustomerIdx { get; set; }

        #endregion

        #region CanExecutes

        public bool CanPage1Navigate(object sender)
        {
            var canNav = (ListingsVM.FilterCheckAndUpdate()
                && IsValidTitle()
                && IsValidDates()
                && IsValidAttributes()
                && !CanRefreshAttributes(null));
            return canNav;
        }

        public bool CanPage2Navigate(object sender)
        {
            return true;
        }

        public bool CanPage2Save(object sender)
        {
            var canSave = SelectedStatus != null
                          && PAndLRecordVM != null
                          && FinancialsRowVM != null;

            return canSave;
        }

        private bool CanRefreshAttributes(object obj)
        {
            if (IsReadOnly)
                return false;

            var canRefresh = (Tab1SerializedAttributeInputs != GetTab1SerializedAttributeInputs());
            var canRefresh2 = IsValidAttributeInputs();


            if (AutoRefresh && canRefresh && canRefresh2)
            {
                Tab1SerializedAttributeInputs = GetTab1SerializedAttributeInputs();
                RefreshAttributes(null);
            }

            return canRefresh;
        }

        private bool CanReloadSubcustomers(object obj)
        {
            var canReload = ListingsVM.CustomerIDsList.Any() && (ListingsVM.CustomerIDsList.First() != SelectedCustomerIdx);

            if (canReload)
            {
                SelectedCustomerIdx = ListingsVM.CustomerIDsList.First();
                LoadSubCustomers();
                LoadDatePeriods();
            }

            return canReload;
        }

        private bool IsValidAttributeInputs()
        {
            return (ListingsVM.ProductIDsList.Any()
                    && ListingsVM.CustomerIDsList.Any()
                    && IsValidDates());
        }

        #endregion

        #region Executes

        public void NavigateToPage2(object sender)
        {
            bool saveSuccessful;

            if (_promoIdx == null || Tab1Serialized != GetTab1Serialization())
                saveSuccessful = ParsePage1SaveResponse(SavePage1());
            else if (FinancialsRowVM == null || PAndLRecordVM == null)
            {
                InitPage2();
                return;
            }
            else
            {
                //If page2 is already rendered
                SelectedPageIdx = 1;
                return;
            }

            if (saveSuccessful)
                InitPage2();
        }

        public void NavigateToPage1(object sender)
        {
            SelectedPageIdx = 0;
        }

        public void SavePage1(object sender)
        {
            if (_promoIdx == null || Tab1Serialized != GetTab1Serialization())
                ParsePage1SaveResponse(SavePage1());
        }

        public void SavePage2(object sender)
        {
            if (Tab2Serialized != GetTab2Serialization())
            {
                var saveSuccessful = ParsePage2SaveResponse(SavePage2());

                if (saveSuccessful)
                {
                    LoadBaseData();
                    ReloadCommand.Execute(null);
                }
            }
        }

        public void SaveClosePage2(object sender)
        {
            bool saveSuccessful = false;

            if (Tab2Serialized != GetTab2Serialization())
            {
                saveSuccessful = ParsePage2SaveResponse(SavePage2());
            }
            else
            {
                CancelCommand.Execute(null);
            }


            if (saveSuccessful)
                CancelCommand.Execute(null);
        }

        public XElement SavePage1()
        {
            PowerPromotion thisPromotion = GetPage1AsPowerPromotion();

            return _promoDataAccess.SavePowerEditorPage1Task(thisPromotion, LastSaved);
        }

        public XElement SavePage2()
        {
            PowerPromotion thisPromotion = GetPage2AsPowerPromotion();

            return _promoDataAccess.SavePowerEditorPage2Task(thisPromotion, LastSaved);
        }

        private bool ParsePage1SaveResponse(XElement res)
        {
            var response = new PromoSaveResult(res);


            if (response.Success)
            {
                LastSaved = DateTime.Parse(response.LastSaved);
                _promoIdx = response.PromoIdx;
                Tab1Serialized = GetTab1Serialization();
            }

            return response.Success;
        }

        private bool ParsePage2SaveResponse(XElement res)
        {
            var response = new PromoSaveResult(res);

            if (response.Success)
            {
                LastSaved = DateTime.Parse(response.LastSaved);
                Tab2Serialized = GetTab2Serialization();
            }

            return response.Success;
        }

        private void RefreshAttributes(object obj)
        {
            RefreshAttributes();
        }

        private void ReloadSubCustomers(object obj)
        { }

        #endregion

        #endregion

        private Task LoadBaseData()
        {
            if (_promoIdx == null) return FromResult(false);

            return FromResult(false).ContinueWith(t =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    var basePromoData = _promoDataAccess.GetPowerPromotion(_promoIdx);

                    Name = basePromoData.Name;
                    Code = basePromoData.CodeAndName.TrimEnd(Name);
                    IsReadOnly = !basePromoData.IsAmendable;
                    LastSaved = basePromoData.WizardPages.FirstOrDefault().LastSavedDate;
                    _initalStatusIdx = basePromoData.StatusID.ToString();
                    //Statuses.FirstOrDefault(a => a.Idx == basePromoData.StatusID.ToString()).IsSelected = true;
                }));
            });
        }

        private string GetTab1Serialization()
        {
            var tab1Serialized = ListingsVM.SelectedCustomers.Where(cust => (cust.Children == null || !cust.Children.Any()) && cust.IsSelectedBool == true).Select(cust => cust.Idx).Distinct().OrderBy(s => s).ToList().Serialize();
            tab1Serialized += ListingsVM.SelectedProducts.Where(prod => (prod.Children == null || !prod.Children.Any()) && prod.IsSelectedBool == true).Select(prod => prod.Idx).Distinct().OrderBy(s => s).ToList().Serialize();

            if (SubCustomersTreeInput != null && SubCustomersTreeInput.GetSelectedNodes().Any())
                tab1Serialized += SubCustomersTreeInput.GetSelectedIdxs().Serialize();

            tab1Serialized += DateList.Serialize();
            tab1Serialized += AttributesRVM.Serialize();
            tab1Serialized += Name.Serialize();

            return tab1Serialized;
        }

        private string GetTab1SerializedAttributeInputs()
        {
            //Method binds to CanExecute so may fire before initialisations
            //TryCatch to avoid messy null checking when waiting for load.
            try
            {
                var tab1Serialized = ListingsVM.SelectedCustomers.Where(cust => (cust.Children == null || !cust.Children.Any()) && cust.IsSelectedBool == true).Select(cust => cust.Idx).Distinct().OrderBy(s => s).ToList().Serialize();
                tab1Serialized += ListingsVM.SelectedProducts.Where(prod => (prod.Children == null || !prod.Children.Any()) && prod.IsSelectedBool == true).Select(prod => prod.Idx).Distinct().OrderBy(s => s).ToList().Serialize();
                tab1Serialized += SubCustomersTreeInput.GetSelectedIdxs().Serialize();
                tab1Serialized += DateList.Serialize();

                return tab1Serialized;
            }
            catch
            {
                return "";
            }

        }

        private string Tab1Serialized { get; set; }
        private string Tab1SerializedAttributeInputs { get; set; }

        #endregion

        #region Tab 2

        private RowViewModel _financialRowVM;
        public RowViewModel FinancialsRowVM
        {
            get { return _financialRowVM; }
            set
            {
                _financialRowVM = value;
                NotifyPropertyChanged(this, vm => vm.FinancialsRowVM);
            }
        }

        private RecordViewModel _pandLRecordVM;
        public RecordViewModel PAndLRecordVM
        {
            get { return _pandLRecordVM; }
            set
            {
                _pandLRecordVM = value;
                NotifyPropertyChanged(this, vm => vm.PAndLRecordVM);
            }
        }

        private RecordViewModel _topRightRecordVM;
        public RecordViewModel TopRightRecordVM
        {
            get { return _topRightRecordVM; }
            set
            {
                _topRightRecordVM = value;
                NotifyPropertyChanged(this, vm => vm.TopRightRecordVM);
            }
        }

        private RecordViewModel _middleLeftRecordVM;
        public RecordViewModel MiddleLeftRecordVM
        {
            get { return _middleLeftRecordVM; }
            set
            {
                _middleLeftRecordVM = value;
                NotifyPropertyChanged(this, vm => vm.MiddleLeftRecordVM);
            }
        }

        private RecordViewModel _middleCentreRecordVM;
        public RecordViewModel MiddleCentreRecordVM
        {
            get { return _middleCentreRecordVM; }
            set
            {
                _middleCentreRecordVM = value;
                NotifyPropertyChanged(this, vm => vm.MiddleCentreRecordVM);
            }
        }

        private RecordViewModel _middleRightRecordVM;
        public RecordViewModel MiddleRightRecordVM
        {
            get { return _middleRightRecordVM; }
            set
            {
                _middleRightRecordVM = value;
                NotifyPropertyChanged(this, vm => vm.MiddleRightRecordVM);
            }
        }

        private RecordViewModel _bottomRightRecordVM;
        public RecordViewModel BottomRightRecordVM
        {
            get { return _bottomRightRecordVM; }
            set
            {
                _bottomRightRecordVM = value;
                NotifyPropertyChanged(this, vm => vm.BottomRightRecordVM);
            }
        }

        private Task LoadPAndLGrid()
        {
            return _promoDataAccess.GetPowerPromotionPAndLGrid(_promoIdx).ContinueWith(t =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    PAndLRecordVM = new RecordViewModel(t.Result);
                }));
            });
        }

        private Task LoadFinancials()
        {
            return _promoDataAccess.GetPowerPromotionFinancialPromoMeasures(_promoIdx).ContinueWith(t =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    FinancialsRowVM = new RowViewModel(t.Result);
                }));
            });
        }

        private Task LoadTopRightGrid()
        {
            return _promoDataAccess.GetPowerPromotion_SKULevelMeasures(_promoIdx).ContinueWith(t =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    TopRightRecordVM = new RecordViewModel(t.Result);
                }));
            });   
        }

        private Task LoadMiddleLeftGrid()
        {
            return _promoDataAccess.GetPowerPromotion_BaseVolume(_promoIdx).ContinueWith(t =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    MiddleLeftRecordVM = new RecordViewModel(t.Result);
                }));
            });           
        }
        private Task LoadMiddleCentreGrid()
        {
            return _promoDataAccess.GetPowerPromotion_PromoVolume(_promoIdx).ContinueWith(t =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    MiddleCentreRecordVM = new RecordViewModel(t.Result);
                }));
            }); 
        }

        private Task LoadMiddleRightGrid()
        {
            return _promoDataAccess.GetPowerPromotion_IncrementalVolume(_promoIdx).ContinueWith(t =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    MiddleRightRecordVM = new RecordViewModel(t.Result);
                }));
            });            
        }

        private Task LoadBottomRightGrid()
        {
            return _promoDataAccess.GetPowerPromotion_Totals(_promoIdx).ContinueWith(t =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    BottomRightRecordVM = new RecordViewModel(t.Result);
                }));
            });            
        }

        private List<ComboboxItem> _statuses;
        public List<ComboboxItem> Statuses
        {
            get { return _statuses; }
            set
            {
                _statuses = value;
                NotifyPropertyChanged(this, vm => vm.Statuses);
            }
        }

        private ComboboxItem _selectedStatus;
        public ComboboxItem SelectedStatus
        {
            get { return _selectedStatus; }
            set
            {
                _selectedStatus = value;
                NotifyPropertyChanged(this, vm => vm.SelectedStatus);
            }
        }

        public Task LoadPromotionStatuses()
        {
            return _promoDataAccess.GetPowerPromotionWorkflowStatuses(_promoIdx).ContinueWith(t =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    Statuses = t.Result;

                    if (_initalStatusIdx != null)
                        Statuses.FirstOrDefault(a => a.Idx == _initalStatusIdx).IsSelected = true;

                    if (Statuses.Any(a => a.IsSelected))
                        SelectedStatus = Statuses.FirstOrDefault(status => status.IsSelected);
                }));
            });
        }

        private List<ComboboxItem> _promotionScenarios;
        public List<ComboboxItem> PromotionScenarios
        {
            get { return _promotionScenarios; }
            set
            {
                _promotionScenarios = value;
                SelectedPromotionScenarios = PromotionScenarios.Where(scen => scen.IsSelected).ToList();
                NotifyPropertyChanged(this, vm => vm.PromotionScenarios);
            }
        }

        private List<ComboboxItem> _selectedPromotionScenarios;
        public List<ComboboxItem> SelectedPromotionScenarios
        {
            get { return _selectedPromotionScenarios; }
            set
            {
                _selectedPromotionScenarios = value;
                NotifyPropertyChanged(this, vm => vm.SelectedPromotionScenarios);
            }
        }

        public Task LoadPromotionScenarios()
        {
            return _promoDataAccess.GetPowerPromotionScenarios(_promoIdx).ContinueWith(t =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    PromotionScenarios = t.Result;
                }));
            });
        }



        private string Tab2Serialized { get; set; }

        private string GetTab2Serialization()
        {
            try
            {
                var tab2Serialized = FinancialsRowVM.Serialize();
                tab2Serialized += PAndLRecordVM.Serialize();
                tab2Serialized += SelectedStatus.Idx.Serialize();
                tab2Serialized += SelectedPromotionScenarios.Select(scen => scen.Idx).ToList().Serialize();

                return tab2Serialized;
            }
            catch
            {
                return "";
            }
        }

        private void G1PromoFinancials_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                FireG1PromoFinancialsChanges();
            }
        }

        private void FireG1PromoFinancialsChanges()
        {
            foreach (var dvRVM in FinancialsRowVM.Records)
            {
                bool fireG2 = false;

                foreach (var ec in dvRVM.Properties.Where(p => p.IsEditable))
                {
                    var updates = ec.UpdateToColumn.Split('$');

                    if (updates.Count() != 2) continue;

                    var table = updates[0];
                    var column = updates[1];

                    if (!string.IsNullOrEmpty(table))
                    {
                        RecordViewModel target = null;

                        if (table == "G2")
                        {
                            target = PAndLRecordVM;
                            fireG2 = true;
                        }

                        if (target != null && target.Records != null)
                        {
                            //For efficiency, do single look ups of the column (property) index and use this for setting the values.
                            var property = target.Records[0].Properties.First(prop => prop.ColumnCode.ToLowerInvariant() == column.ToLowerInvariant());
                            var propertyIndex = target.Records[0].Properties.IndexOf(property);

                            target.Records.Do(rec => rec.Properties[propertyIndex].Value = ec.Value);
                        }
                    }
                }

                if (fireG2)
                {
                    PAndLRecordVM.CalulateRecordColumns();
                    NotifyPropertyChanged(this, vm => vm.PAndLRecordVM);
                }
            }
        }


        #endregion

        public DateTime? LastSaved;

        private int _selectedPageIdx;
        public int SelectedPageIdx
        {
            get { return _selectedPageIdx; }
            set
            {
                _selectedPageIdx = value;
                NotifyPropertyChanged(this, vm => vm.SelectedPageIdx);
            }
        }

        public PowerPromotion GetPage1AsPowerPromotion()
        {
            PowerPromotion page1 = new PowerPromotion
            {
                Idx = _promoIdx,
                Code = Code,
                Name = Name,
                DatePeriodIdx = SelectedPeriod.ID,
                SelectedCustomer = ListingsVM.CustomerIDsList.First(),
                SelectedProducts = ListingsVM.ProductIDsList,
                DatesXml = DatesToXml(DateList)
            };

            if (AttributesRVM != null)
                page1.AttributesXml = AttributesRVM.ToAttributeXml().Root;

            if (SubCustomersTreeInput != null && SubCustomersTreeInput.GetSelectedNodes().Any())
            {
                page1.SelectedSubCustomers = SubCustomersTreeInput.GetSelectedIdxs();
            }

            return page1;
        }

        public PowerPromotion GetPage2AsPowerPromotion()
        {
            PowerPromotion page2 = new PowerPromotion
            {
                Idx = _promoIdx,
                Code = Code,
                Name = Name,
                ProductMeasuresXml = FinancialsRowVM.ToCoreXml().Root,
                PAndLXml = PAndLRecordVM.ToXml().Root,
                SelectedScenarios = SelectedPromotionScenarios.Select(scen => scen.Idx).ToList(),
                SelectedStatus = SelectedStatus.Idx
            };

            return page2;
        }

        private bool _isReadOnly;
        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set
            {
                _isReadOnly = value;

                if (ListingsVM != null)
                    ListingsVM.IsReadOnly = _isReadOnly;

                NotifyPropertyChanged(this, vm => vm.IsReadOnly);
            }
        }

        //Empty Task
        public static Task FromResult<T>(T result)
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetResult(result);
            return tcs.Task;
        }
    }

    public class PromoSaveResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string PromoIdx { get; set; }
        public string LastSaved { get; set; }

        public PromoSaveResult(XElement response)
        {
            Success = response.Descendants("ValidationStatus").FirstOrDefault().MaybeValue() == "1";
            ErrorMessage = response.Descendants("Message").FirstOrDefault().MaybeValue();
            PromoIdx = response.Descendants("Promo_Idx").FirstOrDefault().MaybeValue();
            LastSaved = response.Descendants("LastSaveDate").FirstOrDefault().MaybeValue();
        }
    }



}
