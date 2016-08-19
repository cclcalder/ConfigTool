using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using Exceedra.Common;
using Exceedra.Common.Xml;
using Exceedra.Controls.DynamicRow.Models;
using Exceedra.Controls.DynamicRow.ViewModels;
using Model.DataAccess.Converters;
using Model.DataAccess.Generic;
using Model.DataAccess.Listings;
using Model.Entity;
using Model.Entity.Demand;
using ViewHelper;
using ViewModels;
using WPF.UserControls.Listings;
using WPF.UserControls.Trees.ViewModels;
using Exceedra.SingleSelectCombo.ViewModel;
using Model.DataAccess;
using Model;
using Model.Entity.Generic;

/* How to use:
 * 1a. If there is a proc that will return each proc we need for loading status, cust, prod and other, just pass it into the constructor.
 * OR
 * 1b. If there is no proc, then make new instance and set each of the proc properties manually.
 * 2. In the pages VM, construct an apply method as normal but then set ApplyFilter to reference that method.
 */


namespace WPF.UserControls.Filters.ViewModels
{
    public class FilterViewModel : ViewModelBase
    {
        /// <summary>
        /// If a screen uses the default listings it will look here to check if there is anything cached
        /// </summary>
        //private static ListingsViewModel _cachedDefaultListingsViewModel;

        /// <summary>
        /// If a screen uses its own customized listings it will look for it cached into this list
        /// </summary>

        private string _pageTitle;
        public string PageTitle
        {
            get { return _pageTitle; }
            set
            {
                _pageTitle = value;
                NotifyPropertyChanged(this, vm => vm.PageTitle);
            }
        }

        public bool UseListingsGroups { get; set; }

        public bool NoSingleTree { get; set; }

        public FilterViewModel()
        {
            UseListingsGroups = true;
            /* Default procs 
             * These can be overwritten when constructing a FilterViewModel, but the aim is that all list screens use these procs
             */
            ListingsSelectionProc = "";//StoredProcedure.Shared.GetDefaults;
            ListingsProcs = new Tuple<string, string>(StoredProcedure.Shared.GetFilterCustomers, StoredProcedure.Shared.GetFilterProducts);
            StatusTreeProc = "";// StoredProcedure.Shared.GetFilterStatusesAndTypes;
            OtherFiltersProc = StoredProcedure.Shared.GetFiltersGrid;
         
        }

        public FilterViewModel(string mainProc)
        {
            DynamicDataAccess.GetDynamicDataAsync(mainProc, BaseScreenArguments).ContinueWith(t =>
            {
                GetProcsFromXml(t.Result);
            });
        }

        private void GetProcsFromXml(XElement xml)
        {
            StatusTreeProc = xml.Element("StatusProc").MaybeValue();
            ListingsProcs = new Tuple<string, string>(StoredProcedure.Shared.GetFilterCustomers, StoredProcedure.Shared.GetFilterProducts);
            OtherFiltersProc = xml.Element("OthersProc").MaybeValue();
            SaveAsDefaultProc = StoredProcedure.Shared.SaveDefaults;//xml.Element("SaveAsDefaultProc").MaybeValue();
            ListingsSelectionProc = "";// StoredProcedure.Shared.GetDefaults;
            Load();
        
        }

        #region Loaders

        private Task LoadStatusTree()
        {
            StatusTreeVM.IsTreeLoading = true;

            return DynamicDataAccess.GetGenericItemAsync<TreeViewModel>(StatusTreeProc, SingleTreeArguments, ReloadSingleTree)
                .ContinueWith(t =>
                {
                    StatusTreeVM = t.Result;
                    StatusTreeVM.IsTreeLoading = false;
                }, App.Scheduler);
        }

        private Task LoadListings()
        {
            return Task.Factory.StartNew(() =>
            {
                ListingsViewModel cachedListingsViewModel = null;
                var currentScreen = App.Configuration.GetScreen(CurrentScreenKey);

                if (currentScreen != null) 
                    cachedListingsViewModel = App.CachedListingsViewModels.FirstOrDefault(listingsViewModel => listingsViewModel.ScreenCode == currentScreen.Key);

                if (cachedListingsViewModel != null)
                {
                    ListingsVM = cachedListingsViewModel;
                    ListingsVM.DateTimeFromParent = StartDate;
                    ListingsVM.ScreenCode = CurrentScreen;
                    ListingsVM.SetProducts(UseListingsGroups);
                }
                else
                {

                        ListingsVM = new ListingsViewModel(
                            ListingsAccess.GetFilterCustomers(false, false, currentScreen).Result,
                            ListingsAccess.GetFilterProducts(false, false, currentScreen).Result,
                            CurrentScreen,
                            UseListingsGroups)
                        { DateTimeFromParent = StartDate };

                    if (string.IsNullOrEmpty(ListingsSelectionProc))
                    {
                        if (currentScreen != null)
                            App.CachedListingsViewModels.Add(ListingsVM);
                    }

                }

                //if (DataLoaded != null) DataLoaded();
            });
        }

        public Task LoadOtherFilters()
        {
            return DynamicDataAccess.GetDynamicDataAsync(OtherFiltersProc, OtherArguments, ReloadOthers)
                .ContinueWith(t =>
                {
                    var xml = CurrentScreenKey.ToString().ToLower().Contains("phasing") ? XElement.Parse("<Results><RootItem><Item_Idx>1</Item_Idx><Item_Type>SEASONALS_GRIDFILTERS</Item_Type><Item_RowSortOrder>1</Item_RowSortOrder><Attributes><Attribute><ColumnCode>START_DATE</ColumnCode><HeaderText>Start Date</HeaderText><Format>ShortDate</Format><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><IsRequired>1</IsRequired><ControlType>DatePicker</ControlType><DataSource /><DataSourceInput /><Value>2016-02-10</Value><Values /><SortOrder>10</SortOrder><Validation><MaxValue>END_DATE</MaxValue></Validation></Attribute><Attribute><ColumnCode>END_DATE</ColumnCode><HeaderText>End Date</HeaderText><Format>ShortDate</Format><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><IsRequired>1</IsRequired><ControlType>DatePicker</ControlType><DataSource /><DataSourceInput /><Value>2017-05-10</Value><Values /><SortOrder>20</SortOrder><Validation><MinValue>START_DATE</MinValue></Validation></Attribute><Attribute><ColumnCode>FORECAST</ColumnCode><HeaderText>Trial Forecast</HeaderText><Format /><ForeColour /><BorderColour /><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><IsRequired>1</IsRequired><ControlType>Dropdown</ControlType><DataSource>app.Procast_SP_SHARED_GetFiltersGrid_PopulateDropdowns</DataSource><DataSourceInput>&lt;User_Idx&gt;11&lt;/User_Idx&gt;&lt;SalesOrg_Idx&gt;1&lt;/SalesOrg_Idx&gt;&lt;Screen_Idx&gt;8&lt;/Screen_Idx&gt;&lt;IsROBAppTypeEntry&gt;0&lt;/IsROBAppTypeEntry&gt;&lt;ColumnCode&gt;FORECAST&lt;/ColumnCode&gt;</DataSourceInput><Values /><SortOrder>30</SortOrder><Validation /></Attribute></Attributes></RootItem></Results>") : t.Result;

                    /* When this vm is used in an editor screen, we use this rowVm to generate and set the amendable state */
                    AreFiltersReadOnly = t.Result.Element("Amendable").MaybeValue() == "0";

                    var otherFilters = new RowViewModel(xml);
                    var start = otherFilters.Records[0].Properties.First(p => p.ColumnCode.ToLower().Contains("start_date")).Value;
                    var end = otherFilters.Records[0].Properties.First(p => p.ColumnCode.ToLower().Contains("end_date")).Value;
                    StartDate = DateTime.Parse(String.IsNullOrEmpty(start) ? DateTime.Today.ToString() : start);
                    EndDate = DateTime.Parse(String.IsNullOrEmpty(end) ? DateTime.Today.ToString() : end);

                    otherFilters.Records[0].Properties.Where(p => p.ColumnCode.ToLower().Contains("date")).Do(p => p.PropertyChanged += OnPropertyChanged);

                    otherFilters.IsLoading = false;
                    /* This really should be inside the RowRecord class!
                     * Why not just load the dropdowns when the class is built, rather than having to use this code everywhere?
                     * EW
                     */
                    foreach (var record in otherFilters.Records)
                    {
                        foreach (var property in record.Properties.Where(r => r.ControlType.Contains("down")).ToList())
                        {
                            property.DataSourceInput = property.DataSourceInput.Replace("&gt;", ">").Replace("&lt;", "<");
                            record.InitialDropdownLoad(property);
                        }
                    }

                    if (IsCustomDropdownForDemo)
                    {
                        //TODO: COMMENT OUT FOR RELEASE!!!!!!

                        otherFilters.Records[0].Properties.Add(new RowProperty
                        {
                            ColumnCode = "PhasingDemo",
                            HeaderText = "Phasing Source",
                            IsDisplayed = true,
                            IsEditable = true,
                            MaxWidth = 400,
                            ControlType = "dropdown",
                            Values = new System.Collections.ObjectModel.ObservableCollection<Exceedra.DynamicGrid.Models.Option>
                            {
                                new Exceedra.DynamicGrid.Models.Option { IsSelected = true, Item_Idx = "1", Item_Name = "Standard" },
                                new Exceedra.DynamicGrid.Models.Option { IsSelected = false, Item_Idx = "2", Item_Name = "Promo (First Week)" },
                                new Exceedra.DynamicGrid.Models.Option { IsSelected = false, Item_Idx = "3", Item_Name = "Promo (Mid Week)" },
                                new Exceedra.DynamicGrid.Models.Option { IsSelected = false, Item_Idx = "4", Item_Name = "Promo (Last Week)" }
                            },
                            Value = "",
                            IsRequired = true,

                        });

                    }
                    OtherFiltersVM = otherFilters;
                });
        }

        public bool IsCustomDropdownForDemo { get; set; }


        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "Value")
            {
                var prop = (RowProperty)sender;
                DateTime date;

                if (prop.ColumnCode.ToLower().Contains("start_date"))
                {
                    if (DateTime.TryParse(prop.Value, out date))
                        StartDate = date;
                }

                else if (prop.ColumnCode.ToLower().Contains("end_date"))
                {
                    if (DateTime.TryParse(prop.Value, out date))
                        EndDate = date;
                }
            }
        }

        private Task LoadDates()
        {
            return DynamicDataAccess.GetGenericItemAsync<FilterDateRange>(DatesProc, DateArguments, false).ContinueWith(t =>
            {
                StartDate = t.Result.StartDate;
                EndDate = t.Result.EndDate;
            }, App.Scheduler);
        }

        #endregion

        #region Properties

        public bool ReloadSingleTree { get; set; }

        public string StatusTreeProc { get; set; }

        public string SingleTreeSaveRootNode { get; set; }

        public Tuple<string, string> ListingsProcs { get; set; }

        public bool ReloadOthers { get; set; }

        private string _otherFiltersProc;
        public string OtherFiltersProc
        {
            get { return _otherFiltersProc; }
            set
            {
                IsUsingOtherFilters = true;
                _otherFiltersProc = value;
            }
        }

        /* Proc to determine how we load "defaults".
         * Shared Proc for all List screens.
         * Unique for editor screens.
         */
        public string ListingsSelectionProc { get; set; }

        private string _datesProc;

        public string DatesProc
        {
            get { return _datesProc; }
            set
            {
                IsUsingOtherFilters = false;
                _datesProc = value;
            }
        }

        public string SaveAsDefaultProc { get; set; }

        private ScreenKeys _currentScreenKey;
        public ScreenKeys CurrentScreenKey
        {
            get { return _currentScreenKey; }
            set
            {
                _currentScreenKey = value;
                NotifyPropertyChanged(this, vm => vm.CurrentScreenKey);
                PageTitle =  App.Configuration.GetScreens().Single(f => f.Key == CurrentScreen).Label;
            }
        }

        public string CurrentScreen
        {
            get { return CurrentScreenKey.ToString(); }
        }

        private TreeViewModel _statusTreeVM = new TreeViewModel();
        public TreeViewModel StatusTreeVM
        {
            get { return _statusTreeVM; }
            set
            {
                _statusTreeVM = value;
                NotifyPropertyChanged(this, vm => vm.StatusTreeVM);
            }
        }

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

        private RowViewModel _otherFiltersVM = new RowViewModel { IsLoading = true };
        public RowViewModel OtherFiltersVM
        {
            get { return _otherFiltersVM; }
            set
            {
                _otherFiltersVM = value;
                NotifyPropertyChanged(this, vm => vm.OtherFiltersVM);
            }
        }

        private DateTime _startDate;
        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                NotifyPropertyChanged(this, vm => vm.StartDate);
                if (ListingsVM != null)
                {
                    ListingsVM.DateTimeFromParent = StartDate;
                }
            }
        }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
                NotifyPropertyChanged(this, vm => vm.EndDate);
            }
        }

        private bool _isEndDateBeforeStartDate;
        public bool IsEndDateBeforeStartDate
        {
            get { return _isEndDateBeforeStartDate; }
            set
            {
                _isEndDateBeforeStartDate = value;
                NotifyPropertyChanged(this, vm => vm.IsEndDateBeforeStartDate);
            }
        }

        private bool _isUsingOtherFilters;
        public bool IsUsingOtherFilters
        {
            get { return _isUsingOtherFilters; }
            set
            {
                _isUsingOtherFilters = value;
                NotifyPropertyChanged(this, vm => vm.IsUsingOtherFilters);
            }
        }

        private bool _areFiltersReadOnly;

        public bool AreFiltersReadOnly
        {
            get
            {
                return _areFiltersReadOnly;
            }
            set
            {
                _areFiltersReadOnly = value;
                NotifyPropertyChanged(this, vm => vm.AreFiltersReadOnly);
            }
        }

        #endregion

        #region Commands

        public Action ApplyFilter;

        public ICommand ApplyFilterCommand
        {
            get { return new ViewCommand(CanApplyFilter, InternalApplyFilter); }
        }

        public ICommand SaveAsDefaultCommand
        {
            get { return new ViewCommand(CanApplyFilter, SaveAsDefault); }
        }

        private void InternalApplyFilter(object o)
        {
            if (ApplyFilter == null)
            {
                return;
            }

            if (CanApplyFilter(null))
                ApplyFilter();
        }

        public bool CanApplyFilter(object parameter)
        {
            IsEndDateBeforeStartDate = StartDate.CompareTo(EndDate) > 0;

            //Custom Planning code check
            var check = false;
            if (IsUsingPlanningFilters)
            {
                check = Intervals.SelectedItem != null && PredefinedTimeRanges.SelectedItem != null && (PredefinedTimeRanges.SelectedItem.Name != "Custom" || DateTo.CompareTo(DateFrom) >= 0);
            }
            else
            {
                check = (IsUsingOtherFilters || !IsEndDateBeforeStartDate)
                     && (!IsUsingOtherFilters || (OtherFiltersVM.AreRecordsValid && OtherFiltersVM.AreRecordsFulfilled()));
            }


            var ok = ((StatusTreeVM != null
                       && StatusTreeVM.GetSelectedNodes().Any()) || NoSingleTree)
                     && ListingsVM != null
                     && ListingsVM.FilterCheckAndUpdate()
                     && check;

            return ok;


            //TODO: Validation for rowViewModel/Other filters.
        }

        private void SaveAsDefault(object obj)
        {
            if (IsUsingPlanningFilters)
            {
                if (SavePlanningDefaults())
                    SetCache();
            }
            else
            {
                var args = GetFiltersAsXml(SaveDefaultsRootNode ?? "SaveDefaults");


                SaveAsDefaultProc = (string.IsNullOrEmpty(SaveAsDefaultProc) ? StoredProcedure.Shared.SaveDefaults : SaveAsDefaultProc);
                if (DynamicDataAccess.SaveDynamicData(SaveAsDefaultProc, args))
                    SetCache();
            }


        }

        private void SetCache()
        {
            if (!NoSingleTree)
                DynamicDataAccess.GetGenericItemAsync<TreeViewModel>(StatusTreeProc, SingleTreeArguments);

            if (IsUsingOtherFilters)
                DynamicDataAccess.GetGenericItemAsync<RowViewModel>(OtherFiltersProc, OtherArguments);
            else
                DynamicDataAccess.GetGenericItemAsync<FilterDateRange>(DatesProc, DateArguments);
        }

        #endregion

        #region tasks

        private Task _loadStatusTreeTask;
        private Task _loadDatesTask;

        public Task<Task> Load()
        {
            if (IsUsingPlanningFilters)
            {
                LoadPlanningData();
                _loadDatesTask = Task.FromResult(true);
            }
            else
            {
                _loadDatesTask = IsUsingOtherFilters ? LoadOtherFilters() : LoadDates();
            }
            _loadStatusTreeTask = NoSingleTree ? null : LoadStatusTree();

            return _loadDatesTask.ContinueWith(t =>
            {
                return LoadListings().ContinueWith(r =>
                {
                    if (DataLoaded != null) DataLoaded();
                });
            });


        }

        public delegate void Loaded();
        public event Loaded DataLoaded;


        #endregion

        #region ConvertTo Xml

        public XElement GetFiltersAsXml(string rootTag = null)
        {
            if (!CanApplyFilter(null))
                return new XElement("FiltersNotLoaded");

            var arguments = CommonXml.GetBaseArguments(rootTag);

            arguments.Add(new XElement(XMLNode.Nodes.Screen_Code.ToString(), CurrentScreen));
            arguments.Add(InputConverter.ToCustomers(ListingsVM.CustomerIDsList));
            arguments.Add(InputConverter.ToProducts(ListingsVM.ProductIDsList));

            if (!NoSingleTree) arguments.Add(InputConverter.ToIdxList(SingleTreeSaveRootNode ?? "Statuses", StatusTreeVM.GetSelectedIdxs()));

            //If we do not have a group, then default to 1 (aka All)
            arguments.AddElement("ListingsGroup_Idx", (ListingsVM.ListingGroups != null && ListingsVM.ListingGroups.SelectedItem != null) ? ListingsVM.ListingGroups.SelectedItem.Idx : "1");

            if (IsUsingOtherFilters)
                arguments.Add(OtherFiltersVM.ToAttributeXml().Root);
            else
            {
                arguments.AddElement("Start", StartDate.ToString("yyyy-MM-dd"));
                arguments.AddElement("End", EndDate.ToString("yyyy-MM-dd"));
            }

            if (IsSaveDefaultsOldStyle) arguments = CommonXml.ConvertToOldStyle(arguments);

            if (SaveExtraArguments != null)
                if (SaveExtraArguments.Name.ToString().ToLowerInvariant() == "extra")
                    foreach (var node in SaveExtraArguments.Nodes())
                        arguments.Add(node);
                else
                    arguments.Add(SaveExtraArguments);

            return arguments;
        }

        #endregion

        #region Arguments

        //public XElement ListingsExtraArguments { get; set; }
        private XElement _singleTreeArguments;
        public XElement SingleTreeArguments
        {
            get
            {
                var args = _singleTreeArguments ?? BaseScreenArguments;
                if (IsUsingPlanningFilters)
                    args.Name = "GetData";
                return args;
            }
            set { _singleTreeArguments = value; }
        }

        private XElement _otherArguments;
        public XElement OtherArguments { get { return _otherArguments ?? BaseScreenArguments; } set { _otherArguments = value; } }

        private XElement _dateArguments;
        public XElement DateArguments { get { return _dateArguments ?? BaseScreenArguments; } set { _dateArguments = value; } }

        public XElement SaveExtraArguments { get; set; }

        public XElement BaseScreenArguments { get { return CommonXml.GetBaseScreenArguments(CurrentScreen); } }

        #endregion

        public XElement ListingsSelectionArgs { get; set; }
        public bool? ReloadListingsSelection { get; set; }


        /* When we move to 2.9 these should be removed.
         * They are here in 2.8 to prevent delivery from needing to make changes.          
         */
        #region properties to prevent db changes

        public string SaveDefaultsRootNode { get; set; }
        public bool IsSaveDefaultsOldStyle { get; set; }

        #endregion



        #region Custom Code for planning 

        private bool _isUsingPlanningFilters;
        public bool IsUsingPlanningFilters
        {
            get { return _isUsingPlanningFilters; }
            set
            {
                _isUsingPlanningFilters = value;
                NotifyPropertyChanged(this, vm => vm.IsUsingPlanningFilters);
            }
        }

        private SingleSelectViewModel _planningScenarioList = new SingleSelectViewModel();
        public SingleSelectViewModel PlanningScenarioList
        {
            get { return _planningScenarioList; }
            set
            {
                NotifyPropertyChanged(this, vm => vm.PlanningScenarioList);
            }
        }

        private SingleSelectViewModel _predefinedTimeRanges = new SingleSelectViewModel();
        public SingleSelectViewModel PredefinedTimeRanges
        {
            get { return _predefinedTimeRanges; }
            set
            {
                NotifyPropertyChanged(this, vm => vm.PredefinedTimeRanges);
            }
        }

        private SingleSelectViewModel _exceptions = new SingleSelectViewModel();
        public SingleSelectViewModel Exceptions
        {
            get { return _exceptions; }
            set
            {
                NotifyPropertyChanged(this, vm => vm.Exceptions);
            }
        }

        private SingleSelectViewModel _intervals = new SingleSelectViewModel();
        public SingleSelectViewModel Intervals
        {
            get { return _intervals; }
            set
            {
                NotifyPropertyChanged(this, vm => vm.Intervals);
            }
        }

        private SingleSelectViewModel _heirarchies = new SingleSelectViewModel();
        public SingleSelectViewModel Heirarchies
        {
            get { return _heirarchies; }
            set
            {
                NotifyPropertyChanged(this, vm => vm.Heirarchies);
            }
        }

        private DateTime m_dateFrom;
        public DateTime DateFrom
        {
            get { return m_dateFrom; }
            set
            {
                m_dateFrom = value;
                NotifyPropertyChanged(this, vm => vm.DateFrom);
                if (ListingsVM != null)
                {
                    ListingsVM.DateTimeFromParent = DateFrom;
                }
            }
        }

        private DateTime m_dateTo;
        public DateTime DateTo
        {
            get { return m_dateTo; }
            set { m_dateTo = value; NotifyPropertyChanged(this, vm => vm.DateTo); }
        }

        private bool _useCustomTimeRange;
        public bool UseCustomTimeRange
        {
            get { return _useCustomTimeRange; }
            set
            {
                _useCustomTimeRange = value;
                NotifyPropertyChanged(this, vm => vm.UseCustomTimeRange);
            }
        }

        private void LoadPlanningData()
        {
            LoadPlanningHierarchies();
            LoadPlanningScenarioList();
            LoadIntervals();
            LoadPredefinedTimeRanges();
            //TODO: UNCOMMENT ON RELEASE! ALSO UNCOMMENT IN XAML!
            LoadExceptions();

            if (SelectedTimeRange == null || String.IsNullOrEmpty(SelectedTimeRange.DateFrom) || String.IsNullOrEmpty(SelectedTimeRange.DateTo))
            {
                // Init Date Pickers
                DateFrom = DateTime.Now.Subtract(new TimeSpan(30, 0, 0, 0));
                DateTo = DateTime.Now;
            }
            else
            {
                DateFrom = DateTime.Parse(SelectedTimeRange.DateFrom, System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.AssumeLocal);
                DateTo = DateTime.Parse(SelectedTimeRange.DateTo, System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.AssumeLocal);
            }
        }

        private void LoadPlanningHierarchies()
        {
            Heirarchies.SetItems(DynamicDataAccess.GetGenericEnumerable<ComboboxItem>(StoredProcedure.GetPlanningHierarchies, CommonXml.GetBaseArguments("GetData")));
        } 

        private void LoadPlanningScenarioList()
        {
            PlanningScenarioList.SetItems(PlanningAccess.GetPlanningScenarios());
        }

        private void LoadIntervals()
        {
            var intervals = PlanningAccess.GetPlanningIntervals().ToList();
            intervals.Do(r => r.IsEnabled = true);
            Intervals.SetItems(intervals);
            if (Intervals.SelectedItem == null)
                Intervals.SelectedItem = Intervals.Items.First();
        }

        private void LoadPredefinedTimeRanges()
        {
            var ranges = PlanningAccess.GetPlanningTimeRanges().ToList();
            ranges.Do(r => r.IsEnabled = true);
            PredefinedTimeRanges.SetItems(ranges);
            UseCustomTimeRange = PredefinedTimeRanges.SelectedItem.Name == "Custom";
            PredefinedTimeRanges.PropertyChanged += PredefinedTimeRanges_PropertyChanged;
        }

        private void LoadExceptions()
        {
            Exceptions.SetItems(new List<ComboboxItem>
            {
                new ComboboxItem { Idx = "0", IsEnabled = true, IsSelected = true, Name = "New Products (Planned)" },
                new ComboboxItem { Idx = "1", IsEnabled = true, IsSelected = false, Name = "New Products (Launched)" },
                new ComboboxItem { Idx = "2", IsEnabled = true, IsSelected = false, Name = "Inflight Promotions" },
                new ComboboxItem { Idx = "3", IsEnabled = true, IsSelected = false, Name = "Forecast Error (+10%)" },
                new ComboboxItem { Idx = "4", IsEnabled = true, IsSelected = false, Name = "Base Error (+10%)" },
                new ComboboxItem { Idx = "5", IsEnabled = true, IsSelected = false, Name = "Promotion Error (+10%)" },
            });
        }

        private void PredefinedTimeRanges_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem")
            {
                UseCustomTimeRange = PredefinedTimeRanges.SelectedItem.Name == "Custom";
            }
        }

        private Model.PlanningTimeRange SelectedTimeRange
        {
            get { return (Model.PlanningTimeRange)PredefinedTimeRanges.SelectedItem; }
        }

        private bool SavePlanningDefaults()
        {
            return PlanningAccess.SaveUserPrefsPlanning(new PlanningPreferenceDTO
            {
                Customers = ListingsVM.CustomerIDsList,
                Products = ListingsVM.ProductIDsList,
                IntervalId = Intervals.SelectedItem.Idx,
                DateStart = DateFrom.ToString("yyyy-MM-dd"),
                DateEnd = DateTo.Date.ToString("yyyy-MM-dd"),
                ScenarioID = PlanningScenarioList.SelectedItem.Idx,
                TimeRangeID = SelectedTimeRange.Idx,
                Measures = StatusTreeVM.GetSelectedIdxs(),
                ListingsGroupIdx = ListingsVM.ListingGroups.SelectedItem.Idx,
                HierarchyIdx = Heirarchies.SelectedItem.Idx
            });
        }

        #endregion
    }
}