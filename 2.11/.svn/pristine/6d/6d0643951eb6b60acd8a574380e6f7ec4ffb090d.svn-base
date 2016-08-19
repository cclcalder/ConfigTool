using System.Collections.Specialized;
using Exceedra.CellsGrid;
using Exceedra.Common;
using Exceedra.Common.Logging;
using Exceedra.Common.Utilities;
using Exceedra.Controls.DynamicGrid.Models;
using Model.Entity.DemandPlanning;
using WPF.Navigation;
using WPF.UserControls.Trees.ViewModels;
// ReSharper disable CheckNamespace
using Model.Entity;
using Model.Utilities;

namespace WPF.ViewModels
// ReSharper restore CheckNamespace
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Input;
    using Coder.WPF.UI;
    using Model;
    using Model.DataAccess;
    using Properties;
    using ViewHelper;
    using Wizard;
    using global::ViewModels;

    //using Customer = System.Web.UI.WebControls.Wizard.Customer;
    using System.Threading.Tasks;
    using System.Threading;
    using Exceedra.Controls.DynamicGrid.ViewModels;
    using System.Xml.Linq;
    using System.Windows.Threading;
    using System.Windows.Automation;

    using Exceedra.Controls.DynamicRow.ViewModels;
    using Exceedra.Controls.DynamicRow.Models;
    using Exceedra.Controls.Messages;
    using System.IO;
    using Exceedra.MultiSelectCombo.ViewModel;
    public abstract class PromotionWizardViewModelBase : WizardViewModelBase
    {
        private BackgroundWorker _backgroundWorker = new BackgroundWorker();


        protected const string PostPromoVol = "Post Promo Vol";
        protected const string BasePerWeek = "Base per week";

        protected readonly PromotionAccess DataAccess;

        public WizardPageViewModel AttributesPage;
        public WizardPageViewModel CustomerPage;
        public WizardPageViewModel DatesPage;
        public WizardPageViewModel FinancialsPage;
        public WizardPageViewModel PAndLReviewPage;
        public WizardPageViewModel ProductsPage;
        public WizardPageViewModel VolumesPage;
        public WizardPageViewModel DashboardPage;
        private ICommand _saveCommand;

        private ICommand _reloadCommand;
        private ICommand _reloadCalc;
        

        private ICommand _viewDailyPromotionCommand;
        private ICommand _viewWeeklyPromotionCommand;
        private ViewCommand _uploadSubCustomersViaCsvCommand;
        private ClientConfiguration _configuration;
        private readonly ICommand _applyPromotionVolumeOperationCommand;
        private bool _isEnabled;

        private bool HasCreated = false;

        private PWP PromotionWizardCustomerPage { get; set; }
        private PWP PromotionWizardDatePage { get; set; }
        private PWP PromotionWizardProductPage { get; set; }
        private PWP PromotionWizardAttributePage { get; set; }
        private PWP PromotionWizardVolumePage { get; set; }

        private bool _volumeGridChanged;
        public bool VolumeGridChanged
        {
            get { return _volumeGridChanged; }
            set
            {
                if (_volumeGridChanged != value && !PromotionWizardVolumePage.FirstLoad)
                {
                    SetPageChangedStatus(PromotionWizardVolumePage.Key, true);
                }
                _volumeGridChanged = value;
                NotifyPropertyChanged(this, vm => vm.VolumeGridChanged);
            }
        }
        private PWP PromotionWizardFinancialPage { get; set; }
        private PWP PromotionWizardReviewPage { get; set; }
        protected PromotionWizardViewModelBase(PromotionAccess dataAccess)
            : this(null, null, dataAccess)
        {

        }


        private static bool _showUnitGrid;

        public bool ShowUnitGrid
        {
            get { return _showUnitGrid; }
            set
            {
                _showUnitGrid = value;
                NotifyPropertyChanged(this, vm => vm.ShowUnitGrid);
            }
        }

        private List<PromotionTab> CurrentWizardPages { get; set; }

        public bool ShowDisplay
        {
            get
            {
                var cfg = App.Configuration.Configuration;
                if (cfg != null && cfg.Count() > 0)
                {
                    string res = "0";
                    cfg.TryGetValue("Promotion_Show_Display", out res);
                    return (res == "1" ? true : false);
                }
                else
                {
                    return false;
                }
            }

        }

        public bool ShowFOC
        {
            get
            {
                var cfg = App.Configuration.Configuration;
                if (cfg != null && cfg.Count() > 0)
                {
                    string res = "0";
                    cfg.TryGetValue("Promotion_Show_FOC", out res);
                    return (res == "1" ? true : false);
                }
                else
                {
                    return false;
                }
            }
        }

        private bool StopPromotionDatePeriodsDropdownReset { get; set; }



        protected PromotionWizardViewModelBase(ISearchableTreeViewNodeEventsConsumer eventsConsumer, string promotionId,
            PromotionAccess dataAccess)
        {
            _attributes = "";
            _financials = "";
            _volumes = "";
            PromotionWizardCustomerPage = new PWP("Customer", App.CurrentLang.GetValue("Wizard_Customers", "Customers"));
            PromotionWizardDatePage = new PWP("Dates", App.CurrentLang.GetValue("Wizard_Dates", "Dates"));
            PromotionWizardProductPage = new PWP("Products", App.CurrentLang.GetValue("Wizard_Products", "Products"));
            PromotionWizardAttributePage = new PWP("Attributes", App.CurrentLang.GetValue("Wizard_Attributes", "Attributes"));
            PromotionWizardVolumePage = new PWP("Volumes", App.CurrentLang.GetValue("Wizard_Volumes", "Volumes"));
            PromotionWizardFinancialPage = new PWP("Financials", App.CurrentLang.GetValue("Wizard_Financials", "Financials"));
            PromotionWizardReviewPage = new PWP("Review", App.CurrentLang.GetValue("Wizard_Review", "Review"));

            _backgroundWorker.DoWork += _backgroundWorker_DoWork;

            StopPromotionDatePeriodsDropdownReset = !App.Configuration.StopPromotionDatePeriodsDropdownReset;
            if (dataAccess == null) throw new ArgumentNullException("dataAccess");
            dataAccess.ResetCache();

            DataAccess = dataAccess;

            //PromotionAccessBase.ForceCustomerCacheRefresh();

            _viewDailyPromotionCommand = new ActionCommand(ViewDailyPromotion);
            _viewWeeklyPromotionCommand = new ActionCommand(ViewWeeklyPromotion);
            _uploadSubCustomersViaCsvCommand = new ViewCommand(AnySearchableNodes, LoadSubCustomersFromCsv);
            _applyPromotionVolumeOperationCommand =
                new ActionCommand<PromotionVolumeOperation>(ApplyPromotionVolumeOperation,
                    CanApplyPromotionVolumeOperation);

            EventsConsumer = eventsConsumer;
            if (InitPromotion(promotionId) != true)
            {
                GoToPromotions(true);
                return;
            }

            if (!_backgroundWorker.IsBusy)
                _backgroundWorker.RunWorkerAsync("Init");

            App.Navigator.EnableNavigation(false);

        }

        private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Do something
            if (e.Argument.ToString() == "Init")
            {
                //InitPromotion((e.Argument != null ? e.Argument.ToString() : null)); 
                ShowUnitGrid = false;

                CreatePages();
                UpdateSubMessage("Initialising collections");

                InitializeCollections();

                _isEnabled = true;

                ShowStartUpScreen(InitialiseVisiteds());

                Configuration = new ClientConfigurationAccess().GetClientConfiguration();

            }

        }



        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                NotifyPropertyChanged(this, vm => vm.IsEnabled);
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
                CurrentPromotion.Name = _name;
                if (!PromotionWizardCustomerPage.FirstLoad)
                {
                    SetPageChangedStatus(PromotionWizardCustomerPage.Key, true);
                }
            }
        }

        private void ViewWeeklyPromotion()
        {
            ViewStagedPromotion("Week");
        }

        private void ViewDailyPromotion()
        {
            ViewStagedPromotion("Day");
        }

        private void ViewStagedPromotion(string stageType)
        {
            // SavePromoPhasing("Volumes");
            SavePromotion(PromotionWizardVolumePage.Key, true, false, VolumesPage.Valid);
            var measureIds = CurrentPromotion.Volumes.SelectMany(arg => arg.Measures).Select(arg => arg.ID).Distinct();
            var stagedPromotion = DataAccess.GetStagedPromotion(CurrentPromotion.Id, measureIds, stageType);
            stagedPromotion.IsEditable = CurrentPromotion.IsEditable;
            var volumeScreen = new Pages.StagedPromotion();
            var stagedPromotionVm = new StagedPromotionViewModel(DataAccess, stagedPromotion, volumeScreen);
            volumeScreen.DataContext = stagedPromotionVm;

            IsEnabled = false;
            MainPageViewModel.Instance.IsEnabled = IsEnabled;
            CommandManager.InvalidateRequerySuggested();
            volumeScreen.ShowDialog();
            IsEnabled = true;
            MainPageViewModel.Instance.IsEnabled = IsEnabled;

            //DataAccess.SavePromotion(CurrentPromotion, GetCurrentWizardPageTab(PromotionWizardVolumePage.Key).LastSavedDate, PromotionWizardVolumePage.Key);
            //SavePromoPhasing("Volumes");

            GetCurrentPage(PromotionWizardVolumePage.Key).ForceReload = true;
            LoadVolumes();
        }

        private void SavePromoPhasing(string page)
        {
            if (page == "Volumes")
            {
                XDocument xdoc = new XDocument(
                    new XElement("SaveProductVolumes",
                        new XElement("User_Idx", User.CurrentUser.ID),
                        new XElement("Promo_Idx", CurrentPromotion.Id),
                        new XElement("Results",
                            from r in VolumesRVM.Records
                            select new XElement("RootItem",
                                new XElement("Item_Type", r.Item_Type),
                                new XElement("Item_Idx", r.Item_Idx),
                                new XElement("Attributes",
                                    from p in r.Properties
                                    select new XElement("Attribute",
                                        new XElement("ColumnCode", p.ColumnCode),
                                        new XElement("Value", FixValue(p.Value))
                                        )
                                    )
                                )
                            )
                        )
                    );

                var xml = XElement.Parse(xdoc.ToString());

                var result = DataAccess.SavePromotionPhasing(CurrentPromotion, GetCurrentWizardPageTab(PromotionWizardVolumePage.Key).LastSavedDate, page, xml);
                if (result.ValidationStatus != ValidationStatus.Error)
                {
                 
                    ResetLaterPages(GetCurrentWizardViewModel());
                    RebindNavigation(result);
                }
                else
                {
                    CustomMessageBox.Show(result.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                Application.Current.MainWindow.Cursor = Cursors.Arrow;


            }
        }

        private void ApplyPromotionVolumeOperation(PromotionVolumeOperation operation)
        {
            if (string.IsNullOrWhiteSpace(operation.Value))
            {
                CustomMessageBox.Show("Please enter a valid value", "Invalid Data", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            else
            {
                var newVolumes = DataAccess.ApplyPromotionVolumeOperation(CurrentPromotion.Id, CurrentPromotion.Volumes,
                    operation);
                //PromotionVolumes = new ObservableCollection<PromotionVolume>(newVolumes);
                LoadDynamicVolumeData();
            }
        }

        private bool CanApplyPromotionVolumeOperation(PromotionVolumeOperation arg)
        {
            return CurrentPromotion.IsEditable;
        }

        public ICommand DailyPromotionCommand
        {
            get { return _viewDailyPromotionCommand; }
        }

        public ICommand WeeklyPromotionCommand
        {
            get { return _viewWeeklyPromotionCommand; }
        }

        public ICommand SelectAllSubCustomersCommand
        {
            get { return new ViewCommand(AnySearchableNodes, SelectAllSearchableNodes); }
        }

        public ICommand ClearAllSubCustomersCommand
        {
            get { return new ViewCommand(AnySearchableNodes, ClearAllSearchableNodes); }
        }

        public ICommand UploadSubCustomersViaCsvCommand
        {
            get { return _uploadSubCustomersViaCsvCommand; }
        }

        private PromotionWizardViewModelBase _editingPromoWizardViewModel;

        private void ExecuteSaveCommand(object _)
        {
            SavePromotion(PromotionWizardReviewPage.Key);
            InitPromotionStatuses();
            //GoToPromotions(null);
            // _editingPromoWizardViewModel = new PromotionWizardViewModel(null, CurrentPromotion.Id);
        }


        private void ExecuteReloadCommand(object _)
        {
            var x = GetCurrentWizardViewModel();

            x.ForceReload = true;
            x.BeforeNavigateInTo.Invoke();
            x.BeforeNavigateBackTo.Invoke();
            x.ForceReload = false;
            x.HasChanges = false;

        }

        /// <summary>
        /// call DB to do funky rebuild stuff, then reload screen
        /// </summary>
        /// <param name="obj"></param>
        private void ExecuteReCalc(object obj)
        {
            DataAccess.ReCalc(CurrentPromotion.Id);
            ExecuteReloadCommand(null);
        }

        public ICommand SaveCommand
        {
            get
            {
                return _saveCommand ??
                       (_saveCommand = new ViewCommand(ExecuteSaveCommand));//_ => _statusChanged || _scenariosChanged, 
            }
        }


        public ICommand ReloadCommand
        {
            get
            {
                return _reloadCommand ??
                       (_reloadCommand = new ViewCommand(_ => true, ExecuteReloadCommand));
                //GetCurrentWizardViewModel().HasChanges
            }
        }

        public ICommand ReCalcCommand
        {
            get
            {
                return _reloadCalc ??
                       (_reloadCalc = new ViewCommand(ExecuteReCalc));
                //GetCurrentWizardViewModel().HasChanges
            }
        }

 

        ////public GridLength VolumesProductGridHeight
        ////{
        ////    get
        ////    {
        ////        return App.Configuration.IsCannibalisationActive
        ////            ? new GridLength(4, GridUnitType.Star)
        ////            : new GridLength(100, GridUnitType.Star);
        ////    }
        ////}

        ////public GridLength VolumesCannibalisationGridHeight
        ////{
        ////    get
        ////    {
        ////        return App.Configuration.IsCannibalisationActive
        ////            ? new GridLength(6, GridUnitType.Star)
        ////            : new GridLength(0);
        ////    }
        ////}

        public bool IsCannibalisationActive
        {
            get { return App.Configuration.IsCannibalisationActive; }
        }

        //public bool IsPostPromoActive
        //{
        //    get { return App.Configuration.IsPostPromoActive; }
        //}

        public abstract bool IsSubCustomerActive { get; }

        private void ClearAllSearchableNodes(object o)
        {
            NewTreeSubCustomers.DeselectAll();
        }

        private bool AnySearchableNodes(object o)
        {
            return NewTreeSubCustomers != null && NewTreeSubCustomers.GetFlatTree().Count() > 1;
        }

        private void SelectAllSearchableNodes(object o)
        {
            NewTreeSubCustomers.SelectAll();
        }

        private void LoadSubCustomersFromCsv(object obj)
        {
            IEnumerable<string> fileContent;
            try
            {
                fileContent = IOService.ReadCsvFile();
                if (fileContent == null) return;
            }
            catch (Exception e)
            {
                MessageBoxShow(e.Message, "File Read Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            LoadSubCustomers(fileContent);
        }

        private Action InitialiseVisiteds()
        {
            return () => PageList.ForEach(page => page.Visited = (page.State == ToggleState.On) ? true : false);
        }

        private void CreatePages()
        {


            UpdateSubMessage("Module: " + PromotionWizardCustomerPage.Title);
            PromotionWizardCustomerPage.FirstLoad = true;
            CustomerPage = new WizardPageViewModel(this, PromotionWizardCustomerPage.Key)
            {
                Title = PromotionWizardCustomerPage.Title,
                Visited = true,
                PageViewType = typeof(Wizard.Customer),
                CanAttemptNavigate = IsValidCustomer,
                //  Validate = ValidateCustomerData,
                BeforeNavigateAway = () => SavePromotion(PromotionWizardCustomerPage.Key, true, false, CustomerPage.Valid),
                // AfterNavigateAway = () => SavePromotion(PromotionWizardCustomerPage, true, false, CustomerPage.Valid),              
                BeforeNavigateInTo = () =>
                {
                    if (PromotionWizardCustomerPage.FirstLoad ||
                        GetCurrentPage(PromotionWizardCustomerPage.Key).ForceReload)
                    {
                        InitCustomers(EventsConsumer, CurrentPromotion.Id);
                        CustomerPage.HasChanges = false;
                    }
                },
                BeforeNavigateBackTo = () =>
                {
                    KickOffRebingNavigation(PromotionWizardCustomerPage);
                }
            };
            CustomerPage.HasChanges = false;


            PromotionWizardDatePage.FirstLoad = true;
            UpdateSubMessage("Module: " + PromotionWizardDatePage.Title);
            DatesPage = new WizardPageViewModel(this, PromotionWizardDatePage.Key)
            {
                Title = PromotionWizardDatePage.Title,
                Visited = false,
                PageViewType = typeof(Dates),
                CanAttemptNavigate = () => CanDatesMoveNext(null),
                Validate = IsValidDates,
                BeforeNavigateAway = () => SavePromotion(PromotionWizardDatePage.Key, true, false, DatesPage.Valid),
                BeforeNavigateInTo = () =>
                {
                    if (PromotionWizardDatePage.FirstLoad
                        || GetCurrentPage(PromotionWizardDatePage.Key).ForceReload)
                    {
                        InitDateList();
                        InitPeriods();
                        PromotionWizardDatePage.FirstLoad = false;
                        GetCurrentPage(PromotionWizardDatePage.Key).ForceReload = false;
                        DatesPage.HasChanges = true;
                        _dates = GetSerialisedDate();
                    }
                },
                BeforeNavigateBackTo = () =>
                {
                    KickOffRebingNavigation(PromotionWizardDatePage);
                }
            };



            PromotionWizardProductPage.FirstLoad = true;
            UpdateSubMessage("Module: " + PromotionWizardProductPage.Title);
            ProductsPage = new WizardPageViewModel(this, PromotionWizardProductPage.Key)
            {
                Title = PromotionWizardProductPage.Title,
                Visited = false,
                PageViewType = typeof(Products),
                CanAttemptNavigate = () => CanProductsMoveNext(null),
                // Validate = ValidateProductData,
                BeforeNavigateAway = () => SavePromotion(PromotionWizardProductPage.Key, true, false, ProductsPage.Valid),
                BeforeNavigateInTo = () =>
                {
                    if (PromotionWizardProductPage.FirstLoad
                        || GetCurrentPage(PromotionWizardProductPage.Key).ForceReload)
                    {
                        InitProducts(EventsConsumer);
                        ProductsPage.HasChanges = false;
                        GetCurrentPage(PromotionWizardProductPage.Key).ForceReload = false;
                        _serialisedProducts = GetSerialisedProducts();
                    }
                    GetCurrentPage(PromotionWizardProductPage.Key).ForceReload = false;
                },
                BeforeNavigateBackTo = () =>
                {
                    KickOffRebingNavigation(PromotionWizardProductPage);
                }
            };



            PromotionWizardAttributePage.FirstLoad = true;
            UpdateSubMessage("Module: " + PromotionWizardAttributePage.Title);
            AttributesPage = new WizardPageViewModel(this, PromotionWizardAttributePage.Key)
            {
                Title = PromotionWizardAttributePage.Title,
                Visited = false,
                PageViewType = typeof(Attributes),
                CanAttemptNavigate = IsValidAttributes,
                // Validate = ValidateAttributeData,
                BeforeNavigateAway = () => SavePromotion(PromotionWizardAttributePage.Key, true, false, AttributesPage.Valid),
                BeforeNavigateInTo = () =>
                {
                    if (PromotionWizardAttributePage.FirstLoad
                        || GetCurrentPage(PromotionWizardAttributePage.Key).ForceReload)
                    {
                        InitAttributes();
                        AttributesPage.HasChanges = false;
                        GetCurrentPage(PromotionWizardAttributePage.Key).ForceReload = false;
                    }
                },
                BeforeNavigateBackTo = () =>
                {
                    KickOffRebingNavigation(PromotionWizardAttributePage);
                }
            };



            PromotionWizardVolumePage.FirstLoad = true;
            UpdateSubMessage("Module: " + PromotionWizardVolumePage.Title);
            VolumesPage = new WizardPageViewModel(this, PromotionWizardVolumePage.Key)
            {
                Title = PromotionWizardVolumePage.Title,
                Visited = false,
                PageViewType = typeof(Volumes),
                CanAttemptNavigate = IsValidVolumes,
                Validate = ValidateVolumeData,
                BeforeNavigateInTo = () =>
                {
                    if (PromotionWizardVolumePage.FirstLoad ||
                        GetCurrentPage(PromotionWizardVolumePage.Key).ForceReload)
                    {
                        LoadVolumes();
                        //_volumes = VolumesRVM.Serialize();
                        _volumes = GetSerialisedVolumes();
                        VolumesPage.HasChanges = false;
                    }
                },
                BeforeNavigateAway = () => SavePromotion(PromotionWizardVolumePage.Key, true, false, VolumesPage.Valid),
                BeforeNavigateBackTo = () =>
                {
                    KickOffRebingNavigation(PromotionWizardVolumePage);
                }
            };



            PromotionWizardFinancialPage.FirstLoad = true;
            UpdateSubMessage("Module: " + PromotionWizardFinancialPage.Title);
            FinancialsPage = new WizardPageViewModel(this, PromotionWizardFinancialPage.Key)
            {
                Title = PromotionWizardFinancialPage.Title,
                Visited = false,
                PageViewType = typeof(Financials),
                CanAttemptNavigate = IsValidFinancials,
                // Validate = ValidateFinancialData,
                BeforeNavigateInTo = () =>
                {
                    if (PromotionWizardFinancialPage.FirstLoad ||
                        GetCurrentPage(PromotionWizardFinancialPage.Key).ForceReload)
                    {
                        InitFinancials();
                        //LoadProductFinancialPrices();
                        PromotionWizardFinancialPage.FirstLoad = false;
                        _financials = G1PromoFinancialMeasures.Serialize()
                            + G2ParentProductFinancialMeasures.Serialize()
                            + G3FinancialScreenPlanningSkuMeasure.Serialize();
                        FinancialsPage.HasChanges = false;
                    }
                },
                BeforeNavigateAway = () => SavePromotion(PromotionWizardFinancialPage.Key, true, false, FinancialsPage.Valid),
                Reset = () =>
                {
                    if (CurrentPromotion.FinancialVariables != null)
                        CurrentPromotion.FinancialVariables.Clear();
                    if (CurrentPromotion.FinancialProductVariables != null)
                        CurrentPromotion.FinancialProductVariables.Clear();
                    if (PromotionFinancials != null) PromotionFinancials.Clear();
                    if (ProductFinancialsPricesList != null) ProductFinancialsPricesList.Clear();
                },
                BeforeNavigateBackTo = () =>
                {
                    KickOffRebingNavigation(PromotionWizardFinancialPage);
                }

            };



            PromotionWizardReviewPage.FirstLoad = true;
            UpdateSubMessage("Module: " + PromotionWizardReviewPage.Title);
            PAndLReviewPage = new WizardPageViewModel(this, PromotionWizardReviewPage.Key)
            {
                Title = PromotionWizardReviewPage.Title,
                Visited = false,
                PageViewType = typeof(PLReview),
                //  Validate = ValidatePromotionReview,
                CanAttemptNavigate = () => true,
                Valid = true,
                BeforeNavigateInTo = () =>
                {
                    if (PromotionWizardReviewPage.FirstLoad ||
                        GetCurrentPage(PromotionWizardReviewPage.Key).ForceReload)
                    {
                        if (CurrentPromotion.URL != null)
                            PLReviewUrl = CurrentPromotion.URL;

                        InitApprovals();
                        LoadDashboardRVM();
                        PAndLReviewPage.HasChanges = false;
                    }
                }
            };


            PageList = new List<WizardPageViewModel>
                {
                    //Dashboard,
                    CustomerPage,
                    DatesPage,
                    ProductsPage,
                    AttributesPage,
                    VolumesPage,
                    FinancialsPage,
                    PAndLReviewPage
                };

            RebindPageList();

        }

        private PromotionTab GetCurrentWizardPageTab(string page)
        {
            return CurrentWizardPages.SingleOrDefault(r => r.WizardTabCode == page);
        }

        private WizardPageViewModel GetCurrentWizardViewModel()
        {
            return PageList.SingleOrDefault(r => r.IsCurrent);
        }

        private void KickOffRebingNavigation(PWP current)
        {
            if (CurrentPromotion.Id != null)
            {
                RebindNavigation(DataAccess.GetPromotion(current.Key, GetCurrentWizardPageTab(current.Key).LastSavedDate,
                    CurrentPromotion.Id));
            }
            else
            {
                InitCustomers(EventsConsumer, CurrentPromotion.Id);
                CustomerPage.HasChanges = false;
                Name = "";
            }
        }

        private void ShowStartUpScreen(Action callback)
        {
            UpdateSubMessage("Navigating to startup");

            WizardPageViewModel startupPage = GetCurrentPage(StartUpScreen) ?? CustomerPage;
            int pageIndex = GetPageIndex(startupPage);

            Dispatcher.Invoke(new Action(delegate
            {
                NavigateToPageIndex(pageIndex, callback);
            }), DispatcherPriority.Send);

            Dispatcher.Invoke(new Action(delegate
            {
                PageList.Where(r => r != startupPage).ToList().ForEach(p =>
                {
                    // p.BeforeNavigateInTo();
                    UpdateSubMessage(p.Name);
                });
                NotifyPropertyChanged(this, vm => vm.PageList);

            }), DispatcherPriority.Send);

        }




        /// <summary>
        ///     Initialises the promotion.
        /// </summary>
        /// <param name="promotionId">The promotion id.</param>
        /// <returns>
        ///     <c>true</c> if the promotion should be saved; otherwise, <c>false</c>.
        /// </returns>
        public bool InitPromotion(string promotionId)
        {
            bool save = true;

            if (promotionId == null)
            {
                CurrentPromotion = new Promotion();
                CurrentPromotion.IsEditable = true;
                UpdateSubMessage("Creating blank promotion");

                StartUpScreen = PromotionWizardCustomerPage.Key;
            }
            else
            {
                try
                {
                    RebindNavigation(DataAccess.GetPromotion(PromotionWizardCustomerPage.Key, null, promotionId));


                    UpdateSubMessage("Data: Promotion data");

                    var c = CurrentPromotion.Products;


                    // Set start up screen
                    StartUpScreen = CurrentPromotion.WizarStartScreenName;

                }
                catch (ExceedraDataException ex)
                {
                    //MessageBoxShow(ex.Message + Environment.NewLine + "Couldn't load Promotion.", "Error",
                    //    MessageBoxButton.OK, MessageBoxImage.Warning);

                    return false;
                }
            }

#if LogPerformance
                 Model.Common.LogText("InitPromotion: " + DateTime.Now.Subtract(start).Milliseconds);
#endif
            return save;
        }

        public void GetPromo(Task<Promotion> task)
        {
            CurrentPromotion = task.Result;
        }

        private void InitCustomers(ISearchableTreeViewNodeEventsConsumer eventsConsumer, string promotionId)
        {
            PromotionWizardCustomerPage.FirstLoad = true;

            if ((NewTreeCustomers == null || !NewTreeCustomers.GetFlatTree().Any()) ||
                (GetCurrentPage(PromotionWizardCustomerPage.Key).State != ToggleState.On)
                || (GetCurrentPage(PromotionWizardCustomerPage.Key).ForceReload))
            {
                Name = CurrentPromotion.Name;
                var tt = DataAccess.GetAddPromotionWizardCustomersAsync(promotionId)
                    .ContinueWith(t =>
                    {
                        //CustomerList = new ObservableCollection<PromotionWizardCustomer>();

                        //Populate(this, t, CustomerList, null, vm => vm.CustomerList);

                        NewTreeCustomers = new TreeViewModel(t.Result);
                        NewTreeCustomers.IsReadOnly = !CurrentPromotion.IsEditable;
                        var selectedCust = NewTreeCustomers.GetFlatTree().FirstOrDefault(c => c.IsSelected == "1" && !c.HasChildren);
                        NewTreeCustomers.SelectionChanged += LoadSubCustomers;
                        NewTreeSubCustomers = new TreeViewModel();

                        if (selectedCust != null)
                        {
                            NewTreeCustomers.SetSingleSelection(selectedCust);
                            CurrentPromotion.CustomerIdx = selectedCust.Idx;
                        }

                        PromotionWizardCustomerPage.FirstLoad = false;
                        GetCurrentWizardViewModel().HasChanges = false;

                        _customers = GetSerialisedCustomers();
                    }, App.Scheduler);

            }
            else
            {
                // GetStatusList(promotionId);
                PromotionWizardCustomerPage.FirstLoad = false;
                GetCurrentWizardViewModel().HasChanges = false;
            }
            // return Task.Factory.Completed();
        }

        private void LoadSubCustomers()
        {
            LoadSubCustomers(null);
        }

        private void LoadSubCustomers(IEnumerable<string> csvFile)
        {
            CurrentPromotion.CustomerIdx = NewTreeCustomers.GetSingleSelectedNode().Idx;

            if (csvFile == null)
            {
                var xml = csvFile == null
                    ? DataAccess.GetSubCustomers(CurrentPromotion.Id, User.CurrentUser.ID, CurrentPromotion.CustomerIdx)
                    : DataAccess.GetSubCustomers(CurrentPromotion.Id, User.CurrentUser.ID, CurrentPromotion.CustomerIdx,
                        csvFile);
                NewTreeSubCustomers = new TreeViewModel(xml);
            }
            else
            {
                NewTreeSubCustomers.GetFlatTree().Do(t => t.IsSelected = "0");

                NewTreeSubCustomers.GetFlatTree()
                                  .Where(r => csvFile.Distinct().Contains(r.Code.ToLower())
                                  ).Do(t => t.IsSelected = "1");
            }
           

           
          

            NewTreeSubCustomers.IsReadOnly = !CurrentPromotion.IsEditable;
            NewTreeSubCustomers.SelectionChanged += SubCustomerSelectionChanged;
            CurrentPromotion.SubCustomerIdxs = NewTreeSubCustomers.GetSelectedIdxs();
        }

        private void SubCustomerSelectionChanged()
        {
            CurrentPromotion.SubCustomerIdxs = NewTreeSubCustomers.GetSelectedIdxs();
        }



        public void InitDateList()
        {
            PromotionWizardDatePage.FirstLoad = true;
            UpdateSubMessage("Data: Dates");

            try
            {
                if ((DateList == null || DateList.Count == 0) ||
                    (GetCurrentPage(PromotionWizardDatePage.Key).State != ToggleState.On)
                    || (GetCurrentPage(PromotionWizardDatePage.Key).ForceReload))
                {
                    List<PromotionDate> dates = DataAccess.GetPromotionDates(null, CurrentPromotion.Id, false, false).ToList();
                    DateList = new ObservableCollection<PromotionDateViewModel>(
                       dates.Select(d => new PromotionDateViewModel(d, StopPromotionDatePeriodsDropdownReset)));

                    CurrentPromotion.Dates = dates.ToList();
                }


                // Initialize date values with Today, when the promotion is new
                if (CurrentPromotion.Status == (int)Simple.Common.EditMode.IsAdded)
                {
                    foreach (dynamic d in DateList)
                    {
                        d.StartDate = DateTime.Now;
                        d.EndDate = DateTime.Now;
                    }
                }
            }
            catch (ExceedraDataException ex)
            {
                MessageBoxShow(ex.Message + Environment.NewLine + "Couldn't load Dates.", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            finally
            {
                // PromotionWizardDatePage.FirstLoad = false;
            }
#if LogPerformance
            Model.Common.LogText("InitDateList: " + DateTime.Now.Subtract(start).Milliseconds);
#endif
        }

        public void InitAttributes()
        {

            //if (AttributeList != null) return;
            UpdateSubMessage("Data: Attributes");
            //PromotionWizardAttributePage.FirstLoad = true;

            try
            {

                LoadAttributesRVM();
                //if ((AttributeList == null || AttributeList.Count == 0) 
                //    || (GetCurrentPage(PromotionWizardAttributePage.Key).State != ToggleState.On))
                //{
                //    AttributeList =
                //        new ObservableCollection<PromotionAttribute>(
                //            DataAccess.GetPromotionAttributes(CurrentPromotion, false));
                //}

                //    foreach (PromotionAttribute att in AttributeList)
                //    {
                //        att.SelectedOptions.Clear();
                //        att.SelectedOptions.AddRange(att.Options.Where(o => o.IsSelected));
                //    }

                //CurrentPromotion.Attributes = AttributeList.ToList();
            }
            catch (ExceedraDataException ex)
            {
                MessageBoxShow(ex.Message + Environment.NewLine + "Couldn't load Attributes.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally
            {
                PromotionWizardAttributePage.FirstLoad = false;
            }

#if LogPerformance
            Model.Common.LogText("InitAttributes: " + DateTime.Now.Subtract(start).Milliseconds);
#endif
        }

        public void InitProducts(ISearchableTreeViewNodeEventsConsumer eventsConsumer)
        {

            UpdateSubMessage("Data: Products");
            PromotionWizardProductPage.FirstLoad = true;
            try
            {

                if ((RootProducts == null || RootProducts.Listings == null || RootProducts.Listings.Children == null || !RootProducts.Listings.Children.Any())
                    || (GetCurrentPage(PromotionWizardProductPage.Key).State != ToggleState.On)
                     || (GetCurrentPage(PromotionWizardProductPage.Key).ForceReload))
                {
                    RootProducts = new TreeViewModel(DataAccess.GetAddPromotionProducts(CurrentPromotion.Id, false));
                    LoadProductPrices();
                    RootProducts.SelectionChanged += RootProducts_SelectionChanged; 
                }

                // GetHeirs(RootProducts.FirstOrDefault(r => r.Parent == null));
            }
            catch (ExceedraDataException ex)
            {
                MessageBoxShow(ex.Message + Environment.NewLine + "Couldn't load Products.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally
            {
                PromotionWizardProductPage.FirstLoad = false;
            }



#if LogPerformance
            Model.Common.LogText("InitProducts: " + DateTime.Now.Subtract(start).Milliseconds);
#endif
        }

        private void RootProducts_SelectionChanged()
        {
            LoadProductPrices();
        }

        public void LoadProductPrices()
        {
            // ProductPrices List is in synch, no need to update from database.
            //    if (IsProductPricesInSynch()) return;

            if (RootProducts == null || !RootProducts.GetSelectedNodes().Any())
            {
                CurrentPromotion.ProductPrices = null;
                return;
            }

            try
            {
                IEnumerable<PromotionProductPrice> dataList = DataAccess.GetPromotionProductPrices(CurrentPromotion.Id, RootProducts.GetSelectedIdxs());
                // Get product prices
                ProductPricesList = new ObservableCollection<PromotionProductPrice>(dataList);

                CurrentPromotion.ProductPrices = ProductPricesList != null ? ProductPricesList.ToList() : null;
                NotifyPropertyChanged(this, vm => vm.ProductPricesList);
                _updateProductsDataGrid = false;
            }
            catch (ExceedraDataException ex)
            {
                //MessageBoxShow(ex.Message + Environment.NewLine + "Couldn't load Product Prices.", "Error",
                //    MessageBoxButton.OK, MessageBoxImage.Warning);
            }

#if LogPerformance
            Model.Common.LogText("InitProductPrices: " + DateTime.Now.Subtract(start).Milliseconds);
#endif
        }

        public bool IsProductPricesInSynch()
        {
            // check if every selected product exists in the ProductPricesList, else load them from DB
            // get selected products form tree where they are leaf and dont have children (actual products)

            List<string> selectedProductsIds = RootProducts.GetSelectedIdxs();

            //List<string> selectedProductsIds = new List<string>();
            //foreach (var product in CurrentPromotion.Products)
            //{
            //    if (product != null && product.Children.Count() == 0)
            //    {
            //        selectedProductsIds.Add(product.ID);
            //    }
            //}

            if (!selectedProductsIds.Any())
            {
                ProductPricesList = null;
                return true;
            }

            if (ProductPricesList == null) return false;

            List<string> productPricesIds = ProductPricesList.Select(pp => pp.ID).ToList();

            if (selectedProductsIds.Count() > productPricesIds.Count()) return false;

            if (selectedProductsIds.Count() < productPricesIds.Count())
            {
                List<string> deletedIds = productPricesIds.Except(selectedProductsIds).ToList();

                if (deletedIds.Count > 0)
                {
                    foreach (string deletedId in deletedIds.ToArray())
                    {
                        ObservableCollection<PromotionProductPrice> copy = ProductPricesList;
                        copy.Remove(ProductPricesList.FirstOrDefault(pp => pp.ID == deletedId));
                        ProductPricesList = new ObservableCollection<PromotionProductPrice>(copy);
                    }
                    NotifyPropertyChanged(this, vm => vm.ProductPricesList);
                }
            }

            if (selectedProductsIds.Except(productPricesIds).Any())
                return false;

            return true;
        }

        public void InitFinancials()
        {
            UpdateSubMessage("Data: Financials");
            PromotionWizardFinancialPage.FirstLoad = true;


            try
            {

                //Grid 1
                if ((G1PromoFinancialMeasures == null || G1PromoFinancialMeasures.Records.Count() == 0)
                    || (GetCurrentPage(PromotionWizardFinancialPage.Key).State != ToggleState.On)
                    || (GetCurrentPage(PromotionWizardFinancialPage.Key).ForceReload))
                {
                    G1PromoFinancialMeasures = new RowViewModel(DataAccess.GetPromotionFinancialPromoMeasures(CurrentPromotion.Id));
                    foreach (var col in G1PromoFinancialMeasures.Records.ToList())
                    {

                        foreach (var p in col.Properties)
                        {
                            if (p.ControlType.Contains("down"))
                            {
                                col.InitialDropdownLoad(p);
                            }
                            p.PropertyChanged += G1PromoFinancialMeasures_PropertyChanged;

                        }

                    }


                }

                //Grid 2
                if ((G2ParentProductFinancialMeasures == null || G2ParentProductFinancialMeasures.HasRecords == false)
                    || (GetCurrentPage(PromotionWizardFinancialPage.Key).State != ToggleState.On)
                    || (GetCurrentPage(PromotionWizardFinancialPage.Key).ForceReload))
                {
                    G2ParentProductFinancialMeasures = new RecordViewModel(DataAccess.GetPromotionFinancialParentProductMeasures(CurrentPromotion.Id));
                    G2ParentProductFinancialMeasures.PropertyChanged += G2ParentProductFinancialMeasures_PropertyChanged;
                }

                //Grid 3
                if ((G3FinancialScreenPlanningSkuMeasure == null || G3FinancialScreenPlanningSkuMeasure.HasRecords == false)
                    || (GetCurrentPage(PromotionWizardFinancialPage.Key).State != ToggleState.On)
                    || (GetCurrentPage(PromotionWizardFinancialPage.Key).ForceReload))
                {
                    G3FinancialScreenPlanningSkuMeasure = new RecordViewModel(DataAccess.GetFinancialScreenPlanningSkuMeasure(CurrentPromotion.Id));
                    G3FinancialScreenPlanningSkuMeasure.PropertyChanged += G3FinancialScreenPlanningSkuMeasure_PropertyChanged;
                }

                GetG2ValuesFromG3();

                //When the grids have loaded, fire off the property changed to calculate all the initial values
                G1PromoFinancialMeasures_PropertyChanged(null, new PropertyChangedEventArgs("Value"));
            }
            catch (ExceedraDataException ex)
            {
                MessageBoxShow(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally
            {
                PromotionWizardFinancialPage.FirstLoad = false;
            }

#if LogPerformance
            Model.Common.LogText("InitFinancials: " + DateTime.Now.Subtract(start).Milliseconds);
#endif
        }

        private void G1PromoFinancialMeasures_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                FireG1PromoFinancialMeasuresChanges();
            }
        }

        private void G2ParentProductFinancialMeasures_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Records")
            {
                FireG2ParentProductFinancialMeasuresChanges();
            }
        }



        private void G3FinancialScreenPlanningSkuMeasure_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "HasChanged")
            {
                // FireG3FinancialScreenPlanningSkuMeasureG3();
                GetG2ValuesFromG3();
                //G2ParentProductFinancialMeasures.CalulateRecordColumns();
            }
        }

        //private void FireG3FinancialScreenPlanningSkuMeasureG3()
        //{
        //    throw new NotImplementedException();
        //}


        //        public void LoadProductFinancialPrices()
        //        {
        //            UpdateSubMessage("Financial Pricing");
        //            PromotionWizardFinancialPage.FirstLoad = true;
        //            if (!SelectedProducts.Any())
        //                return;

        //            try
        //            {

        //                //if ((ProductFinancialsPricesList == null || ProductFinancialsPricesList.Count == 0) 
        //                //    || (GetCurrentPage(PromotionWizardFinancialPage.Key).State != ToggleState.On))
        //                //{
        //                //    // Get product financial prices
        //                //    ProductFinancialsPricesList = new ObservableCollection<PromotionProductPrice>(
        //                //        DataAccess.GetPromotionFinancialProductPrices(CurrentPromotion.Id,
        //                //            SelectedProducts.Select(p => p.ID), false));
        //                //}



        //                //CurrentPromotion.FinancialProductVariables = ProductFinancialsPricesList.ToList();
        //            }
        //            catch (ExceedraDataException ex)
        //            {
        //                MessageBoxShow(ex.Message, "Error",
        //                    MessageBoxButton.OK, MessageBoxImage.Warning);
        //            }
        //            finally
        //            {
        //                PromotionWizardFinancialPage.FirstLoad = false;
        //            }

        //#if LogPerformance
        //            Model.Common.LogText("InitProductFinancialPrices: " + DateTime.Now.Subtract(start).Milliseconds);
        //#endif
        //        }

        private RecordViewModel _volumesRvm;

        public RecordViewModel VolumesRVM
        {
            get { return _volumesRvm; }
            set
            {
                if (_volumesRvm != value)
                {
                    _volumesRvm = value;
                    NotifyPropertyChanged(this, vm => vm.VolumesRVM);
                }
            }
        }

        private RecordViewModel _displayVolumesRVM;

        public RecordViewModel DisplayVolumesRVM
        {
            get { return _displayVolumesRVM; }
            set
            {
                if (_displayVolumesRVM != value)
                {
                    _displayVolumesRVM = value;
                    NotifyPropertyChanged(this, vm => vm.DisplayVolumesRVM);
                }
            }
        }

        private UpliftResponse _demandPlanningResponse;

        public UpliftResponse DemandPlanningResponse
        {
            get { return _demandPlanningResponse; }
            set
            {
                if (_demandPlanningResponse != value)
                {
                    _demandPlanningResponse = value;
                    NotifyPropertyChanged(this, vm => vm.DemandPlanningResponse);
                }
            }
        }


        private RecordViewModel _stealVolumesRvm;

        public RecordViewModel StealVolumesRVM
        {
            get { return _stealVolumesRvm; }
            set
            {
                if (_stealVolumesRvm != value)
                {
                    _stealVolumesRvm = value;
                    NotifyPropertyChanged(this, vm => vm.StealVolumesRVM);
                }
            }
        }

        public void LoadVolumes()
        {

            PromotionWizardVolumePage.FirstLoad = true;
            ShowUnitGrid = false;

            try
            {
                if (App.Configuration.UseUpliftForecasting && CurrentPromotion.IsEditable)
                {

                    string url = App.Configuration.UpliftForecastingURL;
                    //"http://localhost:57035/api/uplift/Regression";

                    var promoids = new List<int> { Convert.ToInt32(CurrentPromotion.Id) };

                    var tmp = Model.DataAccess.WebAPIProxy.Run(url, promoids);
                    if (tmp != null)
                    {
                        var results = new UpliftResponse(tmp);

                        if (results.Uplifts != null)
                        {
                            DemandPlanningResponse = results;
                        }
                    }
                }

                LoadDynamicVolumeData();
                UpdateAllowedDisplayunits(false);
            }
            catch (ExceedraDataException ex)
            {
                MessageBoxShow(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally
            {
                PromotionWizardVolumePage.FirstLoad = false;
            }


#if LogPerformance
            Model.Common.LogText("InitVolumes: " + DateTime.Now.Subtract(start).Milliseconds);
#endif
        }

        private void LoadDynamicVolumeData()
        {

            UpdateSubMessage(PromotionWizardVolumePage.Title);

            if ((DisplayVolumesRVM == null || DisplayVolumesRVM.Records == null)
                || (GetCurrentPage(PromotionWizardVolumePage.Key).State != ToggleState.On)
                 || (GetCurrentPage(PromotionWizardVolumePage.Key).ForceReload))
            {

                //load display unit data
                try
                {
                    LoadDisplayVolumesRVM();
                }
                catch (Exception ex)
                {
                    DisplayVolumesRVM = new RecordViewModel();
                    App.LogError("LoadDisplayVolumesRVM:" + ex.Message);
                }
                //finally {

                //}
            }

            ShowUnitGrid = (DisplayVolumesRVM != null && DisplayVolumesRVM.Records != null && DisplayVolumesRVM.Records.Any());


            if ((VolumesRVM == null || VolumesRVM.Records == null) ||
                (GetCurrentPage(PromotionWizardVolumePage.Key).State != ToggleState.On)
                 || (GetCurrentPage(PromotionWizardVolumePage.Key).ForceReload))
            {
                //load volume data
                try
                {
                    LoadVolumesRVM();
                }
                catch (Exception ex)
                {
                    var e = ex;
                    App.LogError("LoadVolumesRVM:" + ex.Message);
                }
            }


            if ((StealVolumesRVM == null || StealVolumesRVM.Records == null) ||
                 (GetCurrentPage(PromotionWizardVolumePage.Key).State != ToggleState.On)
                  || (GetCurrentPage(PromotionWizardVolumePage.Key).ForceReload))
            {
                try
                {
                    LoadStealVolumesRVM();
                }
                catch (Exception ex)
                {
                    var e = ex;
                    App.LogError("LoadStealVolumesRVM:" + ex.Message);
                }

            }

        }

        private void LoadStealVolumesRVM()
        {
            //if (App.Configuration.IsCannibalisationActive && StealVolumesRVM == null)
            //{
            XElement r = DataAccess.GetPromotionStealVolumesX(CurrentPromotion.Id, RootProducts.GetSelectedIdxs());
            if (r == null)
            {
                //  MessageBoxShow("No steal volumes found for this filter");
                StealVolumesRVM = new RecordViewModel();
            }
            else
            {
                StealVolumesRVM = new RecordViewModel(r);
            }
            //}
        }

        private void LoadVolumesRVM()
        {
            //if (VolumesRVM == null)
            //{
            XElement r = DataAccess.GetPromotionVolumesX(CurrentPromotion.Id, null);//SelectedProducts.Where(p => p != null).Select(p => p.ID)
            if (r == null)
            {
                // MessageBoxShow("No volumes found for this filter");
                VolumesRVM = null;
            }
            else
            {
                VolumesRVM = new RecordViewModel(r);

                if (DemandPlanningResponse != null && DemandPlanningResponse.Uplifts != null)
                {

                    foreach (var uplift in DemandPlanningResponse.Uplifts)
                    {

                        var promo = uplift.Promo_Idx;
                        var product = uplift.Sku_Idx;

                        foreach (var measure in uplift.Measures)
                        {
                            var colcode = measure.Code;
                            var val = measure.Value;

                            //find product in volume data
                            var row = VolumesRVM.Records.SingleOrDefault(e => e.Item_Idx == product);

                            //find column 
                            if (row != null)
                            {
                                var prop = row.Properties.SingleOrDefault(c => c.ColumnCode.ToLower() == colcode.ToLower());
                                if (prop != null)
                                {
                                    prop.Value = prop.FormatValue(val);
                                }
                            }
                        }

                    }

                    if (User.CurrentUser != null && User.CurrentUser.Logging)
                    {
                        StorageBase.LogMessageToFile("App Info", "Demand Planning Data Injection",
                            string.Format("Demand Planning Data loaded for {0} products",
                                DemandPlanningResponse.Uplifts.Count),
                            (User.CurrentUser != null ? User.CurrentUser.ID : ""));
                    }
                }

            }
            //}
        }

        public bool UseBomSkuFactor { get; set; }

        private void LoadDisplayVolumesRVM()
        {
            XElement r = DataAccess.GetPromotionDisplayUnitsX(CurrentPromotion.Id, null);

            UseBomSkuFactor = r.Element("UseBOMSkuFactor").MaybeValue() != "0";

            if (r == null)
            {
                // MessageBoxShow("No display volumes found for this filter");
                DisplayVolumesRVM = null;
            }
            else
            {
                DisplayVolumesRVM = new RecordViewModel(r);
            }
            //}
        }


        private void InitPromotionScenarios()
        {
            UpdateSubMessage("Data: Scenarios");

            PromotionWizardReviewPage.FirstLoad = true;

            if ((PromotionScenarios == null || !PromotionScenarios.Items.Any())
                || (GetCurrentPage(PromotionWizardReviewPage.Key).State != ToggleState.On)
                  || (GetCurrentPage(PromotionWizardReviewPage.Key).ForceReload))
            {
                PromotionScenarios.SetItems(DataAccess.GetPromotionScenarios(CurrentPromotion));
            }
            //SelectedScenarios = new ObservableCollection<Scenario>(PromotionScenarios.SelectedItems);
            IsPromotionScenarioVisible = App.Configuration.IsPromotionScenarioActive ? Visibility.Visible : Visibility.Collapsed;

            PromotionWizardReviewPage.FirstLoad = false;
        }

        private void InitPeriods()
        {
            UpdateSubMessage("Periods");

            if ((Periods == null || !Periods.Any())
                || GetCurrentPage(PromotionWizardDatePage.Key).State != ToggleState.On
                 || (GetCurrentPage(PromotionWizardDatePage.Key).ForceReload))
            {
                Periods = DataAccess.GetPromotionDatePeriods(CurrentPromotion, GetCurrentPage(PromotionWizardDatePage.Key).ForceReload);
                CurrentPromotion.DatePeriod = null;
            }
            SelectedPeriod = (CurrentPromotion.DatePeriod ?? Periods.FirstOrDefault(p => p.IsSelected));
        }

        public void InitPromotionStatuses()
        {
            UpdateSubMessage("Data: Statuses");
            PromotionWizardReviewPage.FirstLoad = true;
            try
            {

                if ((PromotionStatuses == null || !PromotionStatuses.Any())
                    || (GetCurrentPage(PromotionWizardReviewPage.Key).State != ToggleState.On)
                    || (GetCurrentPage(PromotionWizardReviewPage.Key).ForceReload))
                {
                    PromotionStatuses = DataAccess.GetPromotionWorkflowStatuses(CurrentPromotion);
                }


                foreach (PromotionStatus s in PromotionStatuses)
                    if (s.ID == CurrentPromotion.Status.ToString(CultureInfo.InvariantCulture))
                        s.IsSelected = true;

                SelectedStatus =
                    PromotionStatuses.SingleOrDefault(
                        s => s.ID == CurrentPromotion.Status.ToString(CultureInfo.InvariantCulture));
            }
            catch (ExceedraDataException ex)
            {
                MessageBoxShow(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally
            {
                PromotionWizardReviewPage.FirstLoad = false;
            }

#if LogPerformance
            Model.Common.LogText("InitPromotionStatuses: " + DateTime.Now.Subtract(start).Milliseconds);
#endif
        }

        public void InitApprovals()
        {
            UpdateSubMessage("Data:  Approvals");
            PromotionWizardReviewPage.FirstLoad = true;
            var ts = new[] {
                DataAccess.GetApprovalLevelsAsync(CurrentPromotion.Id).ContinueWith(InitialiseApprovals, App.Scheduler)           
            };


            Task.Factory.ContinueWhenAll(ts, _ =>
            {
                PromotionWizardReviewPage.FirstLoad = false;
            },
                new CancellationToken(),
                TaskContinuationOptions.None,
                App.Scheduler);

#if LogPerformance
            Model.Common.LogText("InitApprovals: " + DateTime.Now.Subtract(start).Milliseconds);
#endif
        }

        public void InitialiseApprovals(Task<IEnumerable<PromotionApprovalLevel>> task)
        {
            PromotionWizardReviewPage.FirstLoad = true;
            try
            {
                ApproverList = task.Result;

                if (ApproverList != null && ApproverList.Any(x => x.ID != "0"))
                {
                    foreach (PromotionApprovalLevel app in CurrentPromotion.Approvers)
                        ApproverList.Single(a => a.ID == app.ID).Value = app.Value;

                    ApprovalVisibility = Visibility.Visible;
                }
                else ApprovalVisibility = Visibility.Collapsed;
            }
            catch (ExceedraDataException ex)
            {
                ApprovalVisibility = Visibility.Hidden;
                MessageBoxShow(ex.Message, "No Approval Level Found!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally
            {
                PromotionWizardReviewPage.FirstLoad = false;
            }
        }

        public void InitCommentList()
        {
            NewComment = "";

            UpdateSubMessage("Data: Comments");

            try
            {
                CommentList = DataAccess.GetPromotionComments(CurrentPromotion.Id);
            }
            catch (ExceedraDataException ex)
            {
                MessageBoxShow(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

#if LogPerformance
            Model.Common.LogText("InitCommentList: " + DateTime.Now.Subtract(start).Milliseconds);
#endif
        }


        /// <summary>
        ///     Initializes collections of currentParent Promotion
        /// </summary>
        private void InitializeCollections()
        {
            // Application.Current.MainWindow.Cursor = Cursors.Wait;

            InitCommentList();

            UpdateSubMessage("Rendering Interface");

            //   Application.Current.MainWindow.Cursor = Cursors.Arrow;
        }

        #region Properties

        #region CodeAndName

        public string CodeAndName
        {
            get
            {
                return CurrentPromotion == null ? string.Empty : CurrentPromotion.CodeAndName;
            }
        }

        #endregion

        #region EventsConsumer

        /// <summary>
        ///     Gets or sets the SearchableTreeViewNodeEventsConsumer of this PromotionWizardViewModel.
        /// </summary>
        public static ISearchableTreeViewNodeEventsConsumer EventsConsumer { get; set; }

        #endregion

        #region CurrentPromotion

        private Promotion _currentPromotion;

        public Promotion CurrentPromotion
        {
            get { return _currentPromotion; }
            set
            {
                _currentPromotion = value;
                NotifyPropertyChanged(this, vm => vm.CurrentPromotion);
            }
        }

        public bool IsReadOnlyPromo
        {
            get { return !CurrentPromotion.IsEditable; }
        }

        #endregion

        #region Customer

        private TreeViewModel _newTreeSubCustomers;
        public TreeViewModel NewTreeSubCustomers
        {
            get { return _newTreeSubCustomers; }
            set
            {
                _newTreeSubCustomers = value;
                NotifyPropertyChanged(this, vm => vm.NewTreeSubCustomers);
            }
        }

        private TreeViewModel _newTreeCustomers;
        public TreeViewModel NewTreeCustomers
        {
            get { return _newTreeCustomers; }
            set
            {
                _newTreeCustomers = value;
                NotifyPropertyChanged(this, vm => vm.NewTreeCustomers);
            }
        }

        private IEnumerable<PromotionDatePeriod> _periods;

        public IEnumerable<PromotionDatePeriod> Periods
        {
            get { return _periods; }
            set
            {
                _periods = value;
                NotifyPropertyChanged(this, vm => vm.Periods);
            }
        }

        private bool _updatingSelectedPeriod;

        public PromotionDatePeriod SelectedPeriod
        {
            get
            {
                return CurrentPromotion.DatePeriod;
            }
            set
            {
                if (value != CurrentPromotion.DatePeriod)
                {
                    _updatingSelectedPeriod = true;

                    if (!PromotionWizardDatePage.FirstLoad)
                    {
                        if (value != null)
                        {
                            SetPageChangedStatus(PromotionWizardDatePage.Key,
                                (!value.Equals(CurrentPromotion.DatePeriod)));
                        }
                    }

                    CurrentPromotion.DatePeriod = value;
                    if (value != null) //&& StopPromotionDatePeriodsDropdownReset)
                    {
                        UpdateDateList(value);
                    }

                    NotifyPropertyChanged(this, vm => vm.SelectedPeriod);

                    _updatingSelectedPeriod = false;



                }

            }
        }

        private void UpdateDateList(PromotionDatePeriod period)
        {
            // This ordering guarantees that start dates will get set before end dates.
            // The default behaviour is that the end date gets set to the start date plus
            // the offset when the start date gets set.
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

        private Visibility _approvalVisibility;
        public Visibility ApprovalVisibility
        {
            get
            {
                return _approvalVisibility;
            }
            set
            {
                _approvalVisibility = value;
                NotifyPropertyChanged(this, vm => vm.ApprovalVisibility);
            }
        }

        #endregion

        #region Dates

        private ObservableCollection<PromotionDateViewModel> _dateList;

        public ObservableCollection<PromotionDateViewModel> DateList
        {
            get { return _dateList; }
            set
            {
                if (!PromotionWizardDatePage.FirstLoad)
                {
                    SetPageChangedStatus(PromotionWizardDatePage.Key, (!value.Equals(_dateList)));
                }
                if (_dateList != value)
                {
                    _dateList = value;

                    foreach (INotifyPropertyChanged promotionDate in value)
                    {
                        promotionDate.PropertyChanged += PromotionDatePropertyChanged;
                    }

                    //reset pages preceding dates if something has changed


                    NotifyPropertyChanged(this, vm => vm.DateList);
                }
            }
        }

        public bool AreDatesValid { get { return DateList.All(date => date.IsValid); } }

        private void PromotionDatePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_dateList.Any(r => r.HasDateChanges) && (!PromotionWizardDatePage.FirstLoad))
            {
                //ResetLaterPages(DatesPage);
                SetPageChangedStatus(PromotionWizardDatePage.Key, true);
            }

            if (!_updatingSelectedPeriod && StopPromotionDatePeriodsDropdownReset)
                SelectedPeriod = null;

            NotifyPropertyChanged(this, vm => vm.AreDatesValid);
        }

        #endregion

        # region Attributes

        private static string _customers { get; set; }
        private static string _volumes { get; set; }
        private static string _serialisedProducts { get; set; }
        public static string _attributes { get; set; }
        public static string _dates { get; set; }
        public static string _financials { get; set; }
        public static string _customer { get; set; }
        public string AttributeComment
        {
            get { return CurrentPromotion.AttributesComment; }
            set
            {
                if (value != CurrentPromotion.AttributesComment)
                {
                    CurrentPromotion.AttributesComment = value;
                    NotifyPropertyChanged(this, vm => vm.AttributeComment);
                }
            }
        }

        #endregion

        #region Product

        #region Modal Product Editor

        private RecordViewModel m_editProductDataGrid;
        public RecordViewModel EditProductDataGrid
        {
            get { return m_editProductDataGrid; }
            set { m_editProductDataGrid = value; NotifyPropertyChanged(this, vm => vm.EditProductDataGrid); }
        }

        private bool m_editProductGridVis;
        public bool EditProductGridVis
        {
            get { return m_editProductGridVis; }
            set { m_editProductGridVis = value; NotifyPropertyChanged(this, vm => vm.EditProductGridVis); }
        }

        private bool m_modalContenPresenterVis;
        public bool ModalContentPresenterVis
        {
            get { return m_modalContenPresenterVis; }
            set { m_modalContenPresenterVis = value; NotifyPropertyChanged(this, vm => vm.ModalContentPresenterVis); }
        }

        private bool m_areAnyProductsSelected;
        public bool AreAnyProductsSelected
        {
            get { return m_areAnyProductsSelected; }
            set { m_areAnyProductsSelected = value; NotifyPropertyChanged(this, vm => vm.AreAnyProductsSelected); }
        }

        private bool canEnableModalContentButton(object obj)
        {
            return EnableModalContentButton();
        }

        private bool _enableModalContentButton;

        public bool EnableModalContentButton()
        {

            if (RootProducts != null && RootProducts.GetSelectedNodes().Any())
            {
                _enableModalContentButton = true;
            }
            else
            {
                _enableModalContentButton = false;
            }
            return _enableModalContentButton;
        }

        private bool _updateProductsDataGrid = false;

        public ICommand SaveModalProductData
        {
            get { return new ViewCommand(saveModalData); }
        }
        public void saveModalData(object paramter)
        {
            XDocument xdoc = new XDocument(
                new XElement("Results",
                    from r in EditProductDataGrid.Records
                    select new XElement("RootItem",
                        new XElement("Item_Type", r.Item_Type),
                        new XElement("Item_Idx", r.Item_Idx),
                        new XElement("Attributes",
                            from p in r.Properties
                            select new XElement("Attribute",
                                new XElement("ColumnCode", p.ColumnCode),
                                new XElement("Value", FixValue(p.Value))
                                )
                            )
                        )
                        )
                );

            var xml = XElement.Parse(xdoc.ToString());
            XElement returnedString = DataAccess.ModalDynaicGridSave(CurrentPromotion.Id, ModalSelectedProduct.ID, xml);
            string messageString;
            if (returnedString.ToString().Contains("Error") == true)
            {
                messageString = returnedString.GetValue<string>("Error");
                CustomMessageBox.Show(messageString, "Save Unsuccessful", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {

                messageString = returnedString.GetValue<string>("Outcome");
                CustomMessageBox.Show(messageString, "Save Outcome", MessageBoxButton.OK, MessageBoxImage.Information);
                getModalContent();
                _updateProductsDataGrid = true;
            }
        }


        public bool CanAccessModalViewer
        {
            get
            {
                return App.Configuration.IsPromotionProductsRefDataEnabled;
                //var cfg = App.Configuration.IsPromotionProductsRefDataEnabled;
                //if (cfg != null)
                //{
                //    return cfg;
                //}

                //return false;

            }

        }
        public ICommand SetModalVis
        {
            get { return new ViewCommand(canEnableModalContentButton, getModalContent); }
        }
        public void getModalContent(object parmater)
        {

            ModalContentPresenterVis = !ModalContentPresenterVis;
            if (ModalContentPresenterVis == true)
            {
                ModalProductList = new ObservableCollection<PromotionProduct>(from product in (ObservableCollection<PromotionProduct>)DataAccess.GetModalProductDataList(CurrentPromotion.Id, GetSelectedProductIds())
                                                                              where (product.Children == null || product.Children.Count() == 0)
                                                                              select product);

                //OfflineTestList = new ObservableCollection<string>(from product in SelectedProducts
                //                                                                where product.Children == null
                //                                                                select product.ID);

            }
            else
            {
                EditProductGridVis = false;
                if (_updateProductsDataGrid)
                {
                    LoadProductPrices();
                }
            }
        }
        public void getModalContent()
        {
            if (ModalContentPresenterVis == true)
            {
                ModalProductList = new ObservableCollection<PromotionProduct>(from product in (ObservableCollection<PromotionProduct>)DataAccess.GetModalProductDataList(CurrentPromotion.Id, GetSelectedProductIds())
                                                                              where (product.Children == null || product.Children.Count() == 0)
                                                                              select product);

                //OfflineTestList = new ObservableCollection<string>(from product in SelectedProducts
                //                                                                where product.Children == null
                //                                                                select product.ID);

            }
            else
            {
                EditProductGridVis = false;
                if (_updateProductsDataGrid)
                {
                    LoadProductPrices();
                }
            }
        }
        private ObservableCollection<string> m_offlineTestList;
        public ObservableCollection<string> OfflineTestList
        {
            get { return m_offlineTestList; }
            set { m_offlineTestList = value; NotifyPropertyChanged(this, vm => vm.OfflineTestList); }
        }

        public List<string> GetSelectedProductIds()
        {
            ObservableCollection<string> thisList = new ObservableCollection<string>(from product in RootProducts.GetSelectedNodes()
                                                                                     where product.Children == null || !product.Children.Any()
                                                                                     select product.Idx);

            List<string> listToSend = new List<string>();
            foreach (var id in thisList)
            {
                listToSend.Add(id);
            }
            return listToSend;
        }

        public void LoadProductGridData()
        {
            XElement thisXelement = new XElement(DataAccess.GetDynamicProductGrid(CurrentPromotion.Id, ModalSelectedProduct.ID));
            EditProductDataGrid = new RecordViewModel(thisXelement);
            if (m_editProductDataGrid != null)
            {
                EditProductGridVis = true;
            }
        }

        private ObservableCollection<PromotionProduct> m_modalProductList;
        public ObservableCollection<PromotionProduct> ModalProductList
        {
            get { return m_modalProductList; }
            set { m_modalProductList = value; NotifyPropertyChanged(this, vm => vm.ModalProductList); }
        }

        private PromotionProduct m_modalSelectedProduct;
        public PromotionProduct ModalSelectedProduct
        {
            get { return m_modalSelectedProduct; }
            set
            {
                if (value != null)
                    m_modalSelectedProduct = value;

                LoadProductGridData();
                NotifyPropertyChanged(this, vm => vm.ModalSelectedProduct);
            }
        }
        #endregion

        private ObservableCollection<PromotionProduct> _productList;
        private ObservableCollection<PromotionProductPrice> _productPricesList;

        private TreeViewModel _rootProducts;



        public ObservableCollection<PromotionProduct> ProductList
        {
            get { return _productList; }
            set
            {
                if (_productList != value)
                {
                    _productList = value;
                    NotifyPropertyChanged(this, vm => vm.ProductList);
                }
            }
        }

        public TreeViewModel RootProducts
        {
            get { return _rootProducts; }
            set
            {
                _rootProducts = value;
                NotifyPropertyChanged(this, vm => vm.RootProducts);
            }
        }

        private List<PromotionProduct> ConvertToProduct(List<PromotionHierarchy> _selectedProducts)
        {
            return _selectedProducts.Select(p => new PromotionProduct()
            {
                ID = p.ID,
                DisplayName = p.UserName,
                ParentId = p.ParentID,
                IsSelected = p.IsSelected
            }).ToList();
        }


        public void SetChecked(PromotionHierarchy node, bool? IsIt)
        {

            node.IsSelectedBool = IsIt;
            node.IsSelected = (IsIt == true ? "1" : "0");


            if (node.Children != null)
            {
                node.Children.Select(c => { c.IsSelectedBool = IsIt; return c; }).ToList();
                node.Children.Select(c => { c.IsSelected = (IsIt == true ? "1" : "0"); return c; }).ToList();

                foreach (var item in node.Children)
                {
                    SetChecked(item, IsIt);
                }
            }

        }



        public ObservableCollection<PromotionProductPrice> ProductPricesList
        {
            get { return _productPricesList; }
            set
            {
                if (_productPricesList != value)
                {
                    _productPricesList = value;

                    NotifyPropertyChanged(this, vm => vm.ProductPricesList);
                }
            }
        }

        #endregion

        # region Volumes


        public void UpdateAllowedDisplayunits(bool buttonPressed = true, bool rowTotalAllowed = true, bool fixTotal = false)
        {
            //its changed!
            SetPageChangedStatus(PromotionWizardVolumePage.Key, true);

            if (VolumesRVM != null && DisplayVolumesRVM != null && DisplayVolumesRVM.Records != null)
            {
                var totalCheckDisplay = new Dictionary<string, double>();
                //var totalCheckParent = new Dictionary<string, double>();

                foreach (var dvRVM in DisplayVolumesRVM.Records)
                {
                    var columnToSendUpdatesFrom =
                        dvRVM.Properties.Where(p => String.IsNullOrEmpty(p.UpdateToColumn) == false);

                    foreach (var columnToSendUpdateFrom in columnToSendUpdatesFrom)
                    {
                        var updateToColumnCode = columnToSendUpdateFrom.UpdateToColumn ?? "PromoVol_SI";
                        ApplyDisplayOrParentUpdates(dvRVM, totalCheckDisplay, buttonPressed, updateToColumnCode);
                    }
                }

                VolumesRVM.CalulateRecordColumnTotal(VolumesRVM.Records.FirstOrDefault());

                NotifyPropertyChanged(this, vm => vm.VolumesRVM);
            }


            NotifyPropertyChanged(this, vm => vm.DisplayVolumesRVM);
            // throw new NotImplementedException("Not done");
        }

        private void ApplyDisplayOrParentUpdates(Record dvRVM, Dictionary<string, double> totalCheck, bool buttonPressed, string valueColumnCode)
        {
            //get ID of display Item
            var ID = dvRVM.Item_Idx;
            //somewhere to store value
            if (!totalCheck.ContainsKey(ID))
            {
                totalCheck.Add(ID, 0);
            }
            //total from user input

            var displayUnit = dvRVM.Properties.SingleOrDefault(m => m.ColumnCode == valueColumnCode);
            if (displayUnit != null)
            {
                var totalIn = Convert.ToDouble(displayUnit.Value.Replace("%", ""));
                // update the applicable volume data for the display unit

                Record firstRecordWithMatchingParentId = null;

                double bomSkuFactorSum = 0.0;
                foreach (var v in VolumesRVM.Records)
                {
                    var pID = v.Properties.FirstOrDefault(c => c.ColumnCode == "ParentItemIdx").Value;

                    bool isDisplayRecordChild = pID.Replace(",", "").Replace(".", "") == ID;

                    if (buttonPressed && !String.IsNullOrWhiteSpace(ID) && isDisplayRecordChild)
                    {
                        firstRecordWithMatchingParentId = v;

                        double bomSkuFactor = 1;
                        if (UseBomSkuFactor)
                        {
                            try
                            {
                                var v1 = v.Properties.SingleOrDefault(m => m.ColumnCode == "BomSkuFactor").Value;
                                var v2 = FixValue(v1);

                                Double.TryParse(v2, NumberStyles.AllowDecimalPoint, CultureInfo.GetCultureInfoByIetfLanguageTag("en-GB"), out bomSkuFactor);
                                bomSkuFactorSum += bomSkuFactor;
                            }
                            catch (Exception)
                            {

                            }
                        }
                        v.Properties.SingleOrDefault(m => m.ColumnCode == valueColumnCode).Value = (totalIn * bomSkuFactor).ToString();
                        totalCheck[ID] += v.Properties.Where(m => m.ColumnCode == valueColumnCode).Sum(t => Convert.ToDouble(t.Value.Replace("%", "")));

                        VolumesRVM.CalulateRecordColumns(v);
                    }
                }


                // if anyone can make this all work nicer then go for it... CH
                if (bomSkuFactorSum < 1.0 && firstRecordWithMatchingParentId != null)
                {
                    var diff = totalIn - totalCheck.Where(t => t.Key == ID).Sum(r => r.Value);

                    if (diff != 0)
                    {
                        var x = firstRecordWithMatchingParentId.Properties.SingleOrDefault(m => m.ColumnCode == "PromoVol_SI");

                        x.Value =
                            x.FormatValue((Convert.ToDouble(x.Value) + diff).ToString());
                    }


                }
            }
        }

        public List<PromotionVolumeOperation> PromotionVolumeOperations { get; set; }

        private bool _promotionVolumeOperationsCount;
        public bool PromotionVolumeOperationsCount
        {
            get { return _promotionVolumeOperationsCount; }
            set
            {
                _promotionVolumeOperationsCount = value;
                NotifyPropertyChanged(this, vm => vm.PromotionVolumeOperationsCount);
            }

        }

        private bool _keepVolumeOperationsUpdated;
        public bool KeepVolumeOperationsUpdated
        {
            get
            {
                return _keepVolumeOperationsUpdated;
            }
            set
            {
                _keepVolumeOperationsUpdated = value;
                NotifyPropertyChanged(this, vm => vm.KeepVolumeOperationsUpdated);
                CurrentPromotion.KeepVolumeOperation = value;
            }
        }

        #endregion

        #region Financial

        private ObservableCollection<PromotionProductPrice> _productFinancialsPricesList;
        private ObservableCollection<PromotionFinancial> _promotionFinancials;

        public ObservableCollection<PromotionFinancial> PromotionFinancials
        {
            get { return _promotionFinancials; }
            set
            {
                if (!PromotionWizardFinancialPage.FirstLoad)
                {
                    SetPageChangedStatus(PromotionWizardFinancialPage.Key, (!value.Equals(_promotionFinancials)));
                }
                _promotionFinancials = value;

                NotifyPropertyChanged(this, vm => vm.PromotionFinancials);
            }
        }

        private RecordViewModel _mG3FinancialScreenPlanningSkuMeasure;
        public RecordViewModel G3FinancialScreenPlanningSkuMeasure
        {
            get { return _mG3FinancialScreenPlanningSkuMeasure; }
            set { _mG3FinancialScreenPlanningSkuMeasure = value; NotifyPropertyChanged(this, vm => vm.G3FinancialScreenPlanningSkuMeasure); }
        }

        private RecordViewModel _mG2ParentProductFinancialMeasures;
        public RecordViewModel G2ParentProductFinancialMeasures
        {
            get { return _mG2ParentProductFinancialMeasures; }
            set { _mG2ParentProductFinancialMeasures = value; NotifyPropertyChanged(this, vm => vm.G2ParentProductFinancialMeasures); }
        }

        private RowViewModel _mG1PromoFinancialMeasures;
        public RowViewModel G1PromoFinancialMeasures
        {
            get { return _mG1PromoFinancialMeasures; }
            set { _mG1PromoFinancialMeasures = value; NotifyPropertyChanged(this, vm => vm.G1PromoFinancialMeasures); }
        }

        public ObservableCollection<PromotionProductPrice> ProductFinancialsPricesList
        {
            get { return _productFinancialsPricesList; }
            set
            {
                if (!PromotionWizardFinancialPage.FirstLoad)
                {
                    SetPageChangedStatus(PromotionWizardFinancialPage.Key, (!value.Equals(_productFinancialsPricesList)));
                }
                if (_productFinancialsPricesList != value)
                {
                    _productFinancialsPricesList = value;
                    NotifyPropertyChanged(this, vm => vm.ProductFinancialsPricesList);
                }
            }
        }

        #endregion

        #region PLReview

        private string _plReviewUrl;

        public string PLReviewUrl
        {
            get { return _plReviewUrl; }
            set
            {
                _plReviewUrl = !string.IsNullOrWhiteSpace(Settings.Default.ReportServerUrlBase)
                                   ? Regex.Replace(value, "https?://.*?/", Settings.Default.ReportServerUrlBase)
                                   : value;
                CurrentPromotion.URL = _plReviewUrl;
                NotifyPropertyChanged(this, vm => vm.PLReviewUrl);
            }
        }

        #endregion

        # region Wizard Steps Properties

        // --- Edit Mode
        public int EditMode { get; set; }

        // --- Workflow maximum step the user reached in this wizard
        public int StepsTaken { get; set; }

        #endregion

        #region StartUpScreen

        /// <summary>
        ///     Gets or sets the StartUpScreen of this PromotionWizardViewModel.
        /// </summary>
        public string StartUpScreen { get; set; }

        #endregion

        private ObservableCollection<PromotionViewingUser> _viewers;

        public ObservableCollection<PromotionViewingUser> Viewers
        {
            get { return _viewers; }
            set
            {
                _viewers = value;
                NotifyPropertyChanged(this, vm => vm.Viewers);
            }
        }

        # region PromotionStatus

        private IEnumerable<PromotionStatus> _promotionStatuses;

        private PromotionStatus _selectedStatus;
        private bool _statusChanged;
        private bool _scenariosChanged;

        public IEnumerable<PromotionStatus> PromotionStatuses
        {
            get { return _promotionStatuses; }
            set
            {
                _promotionStatuses = value;
                NotifyPropertyChanged(this, vm => vm.PromotionStatuses);
            }
        }

        public PromotionStatus SelectedStatus
        {
            get { return _selectedStatus; }
            set
            {
                if (!PromotionWizardReviewPage.FirstLoad)
                {
                    SetPageChangedStatus(PromotionWizardReviewPage.Key, (!value.Equals(_selectedStatus)));
                }
                if (_selectedStatus != value)
                {
                    bool wasNull = _selectedStatus == null;
                    _selectedStatus = value;
                    NotifyPropertyChanged(this,
                        vm => vm.SelectedStatus,
                        vm => vm.StatusDescription);
                    if (!wasNull)
                    {
                        _statusChanged = true;
                        ((ViewCommand)SaveCommand).RaiseCanExecuteChanged();
                    }
                }
            }
        }

        // --- Status Description
        public string StatusDescription
        {
            get
            {
                //if (SelectedStatus != null)
                //    return SelectedStatus.Name;

                if (PromotionStatuses != null)
                {
                    PromotionStatus current =
                        PromotionStatuses.FirstOrDefault(
                            s => s.ID == CurrentPromotion.Status.ToString(CultureInfo.InvariantCulture));

                    if (current != null)
                        return current.Name;
                }

                const string editingExistingPromotion = "Editing existing Promotion";
                const string addingNewPromotionTemporary = "Adding New Promotion/Temporary";

                return CurrentPromotion.Status == (int)Simple.Common.EditMode.IsEdited
                           ? editingExistingPromotion
                           : addingNewPromotionTemporary;
            }
        }

        public int SelectedApprovalRoleIndex { get; set; }

        #endregion

        #region PromotionScenarios

        private MultiSelectViewModel _promotionScenarios = new MultiSelectViewModel();

        //private IEnumerable<Scenario> _selectedScenarios;

        public MultiSelectViewModel PromotionScenarios
        {
            get { return _promotionScenarios; }
            set
            {
                _promotionScenarios = value;
                NotifyPropertyChanged(this, vm => vm.PromotionScenarios);
            }
        }

        //public IEnumerable<Scenario> SelectedScenarios
        //{
        //    get { return _selectedScenarios; }
        //    set
        //    {
        //        if (!PromotionWizardReviewPage.FirstLoad)
        //        {
        //            SetPageChangedStatus(PromotionWizardReviewPage.Key, (!value.Equals(_selectedScenarios)));
        //        }

        //        if (_selectedScenarios != value)
        //        {
        //            var oldCollectionChangedValue = _selectedScenarios as INotifyCollectionChanged;
        //            if (oldCollectionChangedValue != null)
        //                oldCollectionChangedValue.CollectionChanged -= ScenariosChanged;

        //            var newCollectionChangedValue = value as INotifyCollectionChanged;
        //            if (newCollectionChangedValue != null)
        //                newCollectionChangedValue.CollectionChanged += ScenariosChanged;

        //            _selectedScenarios = value;
        //            NotifyPropertyChanged(this, vm => vm.SelectedScenarios);
        //        }
        //    }
        //}

        public string SelectedScenariosCaption
        {
            get
            {
                const string selected = "{0} Selected";
                return string.Format(selected, PromotionScenarios == null ? 0 : PromotionScenarios.SelectedItems.Count());
            }
            set { }
        }

        public Visibility IsPromotionScenarioVisible { get; set; }

        private void ScenariosChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            _scenariosChanged = (notifyCollectionChangedEventArgs.Action == NotifyCollectionChangedAction.Add
                || notifyCollectionChangedEventArgs.Action == NotifyCollectionChangedAction.Remove);

            SetPageChangedStatus(PromotionWizardReviewPage.Key, _scenariosChanged);

            NotifyPropertyChanged(this, vm => vm.SelectedScenariosCaption);
            ((ViewCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        #endregion

        #region ApprovalLevels

        public IEnumerable<PromotionApprovalLevel> ApproverList { get; set; }

        #endregion

        #region Comments

        public IEnumerable<PromotionComment> CommentList { get; set; }
        public string NewComment { get; set; }

        public bool IsCommentEnabled
        {
            get { return (!string.IsNullOrEmpty(CurrentPromotion.Id)) && CurrentPromotion.IsEditable && HasID(); }
        }

        #endregion

        #endregion

        #region Commands

        # region Volume update commands

        public ICommand ApplyTotalVolumeCommand
        {
            get { return new ViewCommand(ApplyTotalVolume); }
        }

        public ICommand ApplyUpliftVolumeCommand
        {
            get { return new ViewCommand(ApplyUpliftVolume); }
        }

        public ICommand ApplyPostPromoCommand
        {
            get { return new ViewCommand(ApplyPostPromo); }
        }

        #endregion

        # region Navigation Commands

        private ICommand _goToPromotionsCommand;
        public ICommand GoToPromotionsCommand
        {
            get { return _goToPromotionsCommand ?? (_goToPromotionsCommand = new ViewCommand(GoToPromotions)); }
            set { _goToPromotionsCommand = value; }
        }

        #endregion

        #region Step Management Commands

        public ICommand PLReviewNextCommand
        {
            get { return new ViewCommand(PLReviewNextPage); }
        }

        public ICommand FinishSaveCommand
        {
            get { return new ViewCommand(CanFinishSave, FinalSave); }
        }

        #endregion

        public ICommand AddCommentCommand
        {
            get { return new ViewCommand(CanAddComment, AddComment); }
        }

        # endregion

        # region Methods

        #region Volume Updating

        private decimal? _totalVolume;
        private decimal? _volumeUplift;

        public decimal? TotalVolume
        {
            get { return _totalVolume; }
            set
            {
                _totalVolume = value;
                NotifyPropertyChanged(this, vm => vm.TotalVolume);
            }
        }

        public decimal? VolumeUplift
        {
            get { return _volumeUplift; }
            set
            {
                _volumeUplift = value;
                NotifyPropertyChanged(this, vm => vm.VolumeUplift);
            }
        }

        public ICommand ApplyPromotionVolumeOperationCommand
        {
            get { return _applyPromotionVolumeOperationCommand; }
        }

        /// <summary>
        ///     Sets a value to total volume values in PromotionVolumes
        /// </summary>
        /// <param name="parameter"></param>
        public void ApplyTotalVolume(object parameter)
        {
            decimal vol;

            if (!TryConvertToDecimal(parameter, out vol)) return;

            //foreach (PromotionVolume v in PromotionVolumes)
            //{
            //    v.Measures.ElementAt(1).Value = vol.ToString(CultureInfo.InvariantCulture);

            //}

            UpdateAllowedDisplayunits();

            //PromotionVolumes =
            //    new ObservableCollection<PromotionVolume>(PromotionVolumes.OrderBy(pv => pv.Product.DisplayName));

            throw new NotImplementedException("Not done");
        }

        /// <summary>
        /// Find the Value of the product and pass it back
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private Double GetDisplayFactor(string p, string columnCode)
        {
            try
            {

                return Double.Parse(DisplayVolumesRVM.Records.Where(r => r.Item_Idx == p).SingleOrDefault().Properties.Where(r => r.ColumnCode == columnCode).Select(r => r.Value).SingleOrDefault());
            }
            catch
            {
                return 0;
            }

        }

        /// <summary>
        ///     Updates total volume values in PromotionVolumes by adding specified percentage of that value
        /// </summary>
        /// <param name="parameter"></param>
        public void ApplyUpliftVolume(object parameter)
        {
            //decimal percent;
            //if (!TryConvertToDecimal(parameter, out percent)) return;

            //percent += 100M;

            //foreach (PromotionVolume v in PromotionVolumes)
            //{
            //    v.Measures.ElementAt(1).Value =
            //        Math.Round(Model.Common.RemoveFormatForDecimal(v.Measures.First().Value) * percent / 100, 3)
            //            .ToString(CultureInfo.InvariantCulture);
            //}

            //PromotionVolumes =
            //    new ObservableCollection<PromotionVolume>(PromotionVolumes.OrderBy(pv => pv.Product.DisplayName));

            throw new NotImplementedException("Not done");
        }

        private bool TryConvertToDecimal(object parameter, out decimal percent)
        {
            try
            {
                percent = Convert.ToDecimal(parameter.ToString());
            }
            catch (FormatException)
            {
                ShowWarning("Entered value is not a decimal type!");
                percent = 0;
                return false;
            }
            return true;
        }

        public void ApplyPostPromo(object obj)
        {
            ApplyPercentage(obj, PostPromoVol, BasePerWeek);
            //PromotionVolumes =
            //    new ObservableCollection<PromotionVolume>(PromotionVolumes.OrderBy(pv => pv.Product.DisplayName));
            throw new NotImplementedException("Not done");
        }

        private void ApplyPercentage(object obj, string targetName, string sourceName)
        {
            decimal percent;
            if (!TryConvertToDecimal(obj, out percent)) return;
            //foreach (PromotionVolume v in PromotionVolumes)
            //{
            //    decimal baseUnits = Model.Common.RemoveFormatForDecimal(v.GetMeasure(sourceName).Value);
            //    v.GetMeasure(targetName).Value =
            //        Math.Round(baseUnits * (percent / 100), 3).ToString(CultureInfo.InvariantCulture);
            //}

            throw new NotImplementedException("Not done");
        }

        #endregion

        #region LINK Navigation Methods

        protected override void OnCancel()
        {
            XElement removePromoViewer = new XElement("RemovePromotionViewer");
            removePromoViewer.Add(new XElement("User_Idx", User.CurrentUser.ID));
            removePromoViewer.Add(new XElement("Promo_Idx", CurrentPromotion.Id));
            // FireisDirtyAsync();
            WebServiceProxy.Call(StoredProcedure.Promotion.RemovePromotionViewer, removePromoViewer, DisplayErrors.No); //Procast_SP_PROMO_RemovePromotionViewer
            GoToPromotionsCommand.Execute(HasCreated);
        }

        //private void FireisDirty()
        //{
        //    if (_currentPromotion.Id !="0")
        //     DataAccess.IsDirtyResult(Convert.ToInt32(_currentPromotion.Id));
        //}

        //private void FireisDirtyAsync()
        //{
        //    try
        //    {
        //        if (_currentPromotion.Id != "0")
        //         DataAccess.IsDirty(Convert.ToInt32(_currentPromotion.Id));
        //    }
        //    catch { }
        //}

        /// <summary>
        ///     Navigates to the Promotions page
        /// </summary>
        /// <param name="parameter"></param>
        public void GoToPromotions(object parameter)
        {
            RedirectMe.ListScreen("Promo");
        }

        #endregion

        # region Step Management Methods

        private bool IsAnyCustomerSelected()
        {
            // this will work only if the promotion is saved
            if (CurrentPromotion.CustomerIdx != null) { return true; }

            // if the promotion is not saved:
            // _selectedCustomers is an empty array during initial page loading
            // but after selecting any node is populated with some data
            // so first the _selectedCustomers array is searched for any selected node (it's less data to process)
            // and then if not successful every root from root customers and it's children will be searched
            if (NewTreeCustomers != null && NewTreeCustomers.GetSingleSelectedNode() != null) { return true; }

            return false;
        }

        private bool IsValidCustomer()
        {
            // Validation for Customer
            if (string.IsNullOrEmpty(CurrentPromotion.Name))
            {
                return false;
            }

            if (CurrentPromotion.CustomerIdx != null && NewTreeCustomers.GetSingleSelectedNode() == null)
            {
                return false;
            }

            if (!IsAnyCustomerSelected())
            {
                return false;
            }

            return true;
        }


        private bool ValidateCustomerData(bool showMessages)
        {
                    
            return true;
           
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

        public bool CanDatesMoveNext(object parameter)
        {
            // SET DATES
            if (DateList != null)
            {
                CurrentPromotion.Dates = DateList.Select(d => d.GetModel()).ToList();
            }

            // CLIENTSIDE VALIDATION
            return IsValidDates();
        }

        private bool ValidateDatesData()
        {
            return ValidateDatesData(true);
        }

        private bool ValidateDatesData(bool showMessages)
        {
            foreach (var d in DateList)
            {

                if ((d.StartDate > d.EndDate) == true)
                {
                    CustomMessageBox.Show(d.StartDate + " > " + d.EndDate, "Date error");
                    return false;
                }

            }
            return true;

        }

        private bool IsValidProducts()
        {
            // CHAINING VALIDATION
            // if (!IsValidDates()) return false;

            if (RootProducts == null || !RootProducts.GetSelectedNodes().Any()) return false;

            return true;
        }

        public bool CanProductsMoveNext(object parameter)
        {
            // In Products Page
            if (RootProducts == null || !RootProducts.GetSelectedNodes().Any())
            {
                ProductPricesList = null;
                NotifyPropertyChanged(this, vm => vm.ProductPricesList);
                return false;
            }

            if (!IsValidProducts())
                return false;

            // SET PRODUCTS
            //CurrentPromotion.Products = SelectedProducts != null
            //                                ? SelectedProducts.ToList()
            //                                : new List<PromotionProduct>();

            // Load Backup ProductCollection
            //if (!CurrentPromotion.ProductsBackup.Any())
            //{
            //    CurrentPromotion.BackupProducts();
            //}

            //LoadProductPrices();
            CurrentPromotion.ProductPrices = ProductPricesList != null
                                                 ? ProductPricesList.ToList()
                                                 : new List<PromotionProductPrice>();

            return true;
        }


        private bool ValidateProductData()
        {
            return true;// ValidateProductData(true);
        }

        private bool ValidateProductData(bool showMessages)
        { 
            return true;
           
        }

        private bool IsValidAttributes()
        {
            // CHAINING VALIDATION
            // if (!IsValidProducts()) return false;

            if (CurrentPromotion.Attributes != null && CurrentPromotion.Attributes.Any(a => (a.SelectedOption == null)))
                return false;

            if (AttributesRVM == null) return false;

            var nonSelectedDropdowns =
                AttributesRVM.Records.SelectMany(record => record.Properties).Where(prop =>
                    ((prop.SelectedItems == null && prop.IsRequired) || (!prop.SelectedItems.Any()) && prop.IsRequired)
                    && prop.SelectedItem == null
                    && String.IsNullOrWhiteSpace(prop._innerValue));

            if (nonSelectedDropdowns.Any()) return false;

            return true;
        }

        private bool ValidateAttributeData()
        {
            return true;// ValidateAttributeData(true);
        }

        private bool ValidateAttributeData(bool showMessages)
        { 
            return true; 
        }

        private bool IsValidVolumes()
        {
            // CHAINING VALIDATION
            //if (!IsValidAttributes()) return false;

            return true;
        }

        //Proc doe not check data so this has been removed until such time we start to check data in DB
        private bool ValidateVolumeData()
        {
            return true;// ValidateVolumeData(true);
        }

      
        private bool IsValidFinancials()
        {
            return IsValidVolumes();
        }

        private bool ValidateFinancialData()
        {
            return true;// ValidateFinancialData(true);
        }

        private bool ValidateFinancialData(bool showMessages)
        { 
            return true; 
        }

        private bool IsValidReview()
        {
            if (!IsValidFinancials()) return false;

            if (!_statusChanged && !_scenariosChanged) return false;

            return true;
        }

        private bool ValidatePromotionReview()
        {
            return true;// ValidatePromotionReview(true);
        }

        private bool ValidatePromotionReview(bool showMessages)
        { 
            return true; 
        }

        public bool CanPLReviewMoveNext(object parameter)
        {
            return IsValidReview();
        }

        private void PLReviewNextPage(object parameter)
        {
            FinalSave(null);
        }


        public bool IsValidFinish()
        {
            if (!IsValidReview()) return false;

            return true;
        }

        public bool CanFinishSave(object parameter)
        {
            return true;// IsValidFinish();
        }

        public void FinalSave(object parameter)
        {
            //SetStatus();
            //SetApprovals();

            if (SavePromotion(PromotionWizardReviewPage.Key, true, false))
            {
                XElement removePromoViewer = new XElement("RemovePromotionViewer");
                removePromoViewer.Add(new XElement("User_Idx", User.CurrentUser.ID));
                removePromoViewer.Add(new XElement("Promo_Idx", CurrentPromotion.Id));

                WebServiceProxy.Call(StoredProcedure.Promotion.RemovePromotionViewer, removePromoViewer, DisplayErrors.No);
                //MessageBoxShow(App.CurrentLang.GetValue("Message_Promotion-Saved_Okay","Promotion saved correctly"));
                GoToPromotionsCommand.Execute(true);


            }
        }

        #endregion
 

        #region PROMOTION SAVE

        /// <summary>
        ///     Gets a new ID from data access and sets the CurrentPromotion.ID , then initializes data collections of currentPromotion with
        ///     proper data
        /// </summary>
        /// <returns></returns>
        private void RetrieveAndSetNewPromotionID(DateTime? lastSaveDateTime)
        {

            // Get a new Id from data access for currentParent new promotion
            try
            {
                var result = DataAccess.SavePromotion(CurrentPromotion, lastSaveDateTime, PromotionWizardCustomerPage.Key);
                RebindNavigation(result);

            }
            catch// (Exception ex)
            {
                //MessageBoxShow(ex.Message);
            }


        }

        /// <summary>
        ///     Validates All Data collections against Database
        /// </summary>
        /// <returns></returns>
        ////public bool ValidateAll(bool showMessages)
        ////{
        ////    return
        ////        ValidateCustomerData(showMessages) &&
        ////        ValidateDatesData(showMessages) &&
        ////        ValidateProductData(showMessages) &&
        ////        ValidateAttributeData(showMessages) &&
        ////        ValidateVolumeData() &&
        ////        ValidateFinancialData(showMessages) &&
        ////        ValidatePromotionReview(showMessages);
        ////}

        public void SetStatus()
        {
            if (SelectedStatus != null)
                CurrentPromotion.Status = int.Parse(SelectedStatus.ID);
        }

        public void SetScenarios()
        {
            if (PromotionScenarios != null && PromotionScenarios.SelectedItems.Any())
            {
                CurrentPromotion.Scenarios = PromotionScenarios.SelectedItemIdxs;
            }
        }

        public void SetDatePeriod()
        {
            if (SelectedStatus != null)
                CurrentPromotion.DatePeriod = SelectedPeriod;
        }

        public void SetApprovals()
        {
            if (ApproverList != null)
                CurrentPromotion.Approvers = ApproverList.ToList();
        }

        private bool performAllRelevantChecks(string page, bool showMessages)
        {
            bool returnBool = false;

            if (page == PromotionWizardVolumePage.Key)
            {
                returnBool = ValidateVolumeData();
            }
            if (page == PromotionWizardCustomerPage.Key)
            {
                returnBool = ValidateCustomerData(showMessages);
            }
            if (page == PromotionWizardDatePage.Key)
            {
                returnBool = ValidateDatesData(showMessages);
            }
            if (page == PromotionWizardProductPage.Key)
            {
                returnBool = ValidateProductData(showMessages);
            }
            if (page == PromotionWizardAttributePage.Key)
            {
                returnBool = ValidateAttributeData(showMessages);
            }
            if (page == PromotionWizardFinancialPage.Key)
            {
                returnBool = ValidateFinancialData(showMessages);
            }
            if (page == "Review" || page == "Final")
            {
                returnBool = ValidatePromotionReview(showMessages);
            }

            return returnBool;
        }


        /// <summary>
        /// Set status changed for any page
        /// </summary>
        /// <param name="page"></param>
        /// <param name="status"></param>
        private void SetPageChangedStatus(string page, Boolean status)
        {
            if (page != null)
            {
                try
                {
                    GetCurrentPage(page).HasChanges = status;
                }
                catch (Exception)
                {

                }


            }
        }

        /// <summary>
        /// Where the hell are we?
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        private WizardPageViewModel GetCurrentPage(string page)
        {
            if (page != null)
            {
                try
                {
                    return PageList.FirstOrDefault(r => r.Name == page);
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// Saves currentParent promotion and returns return URL from database
        /// </summary>
        /// <returns></returns>
        private bool SavePromotion(string page, bool showMessages = true, bool onetime = true, bool valid = false)
        {
            //create an empty result with pages set (incase the save fails)
            PromotionSaveResults result = new PromotionSaveResults();
            result.WizardPages = CurrentWizardPages;

            var currentPage = GetCurrentPage(page);
            var currentWizardTab = GetCurrentWizardPageTab(page);


            //manage edge case save logic first
            if (CurrentPromotion.IsReadOnly && page != PromotionWizardReviewPage.Key && _customer == CurrentPromotion.Name)
            {
                RebindNavigation(DataAccess.GetPromotion(page, currentWizardTab.LastSavedDate, CurrentPromotion.Id));
                RebindPageList();

                return true;
            }

            if (currentPage != null)
            {
                //Has this page changed? - if not then dont run the save proc
                if (page == PromotionWizardVolumePage.Key)
                {
                    //var v = _volumesRvm.Serialize();
                    var v = GetSerialisedVolumes();
                    if ((_volumes != v
                        || PromotionWizardVolumePage.Saves == 0)
                        || currentPage.State != ToggleState.On)
                    {
                        currentPage.HasChanges = true;
                        _volumes = GetSerialisedVolumes();
                        PromotionWizardVolumePage.Saves += 1;
                        GetCurrentWizardViewModel().ForceReload = false;
                    }
                    else
                    {
                        DateTime? dt = null;
                        if (currentWizardTab != null)
                        {
                            dt = currentWizardTab.LastSavedDate;
                        }
                        RebindNavigation(DataAccess.GetPromotion(page, dt, CurrentPromotion.Id));
                        RebindPageList();
                        return true;
                    }
                }
                else if (page == PromotionWizardProductPage.Key)
                {
                    var productsNewSerialization = GetSerialisedProducts();

                    // If products changed (selection, parent planning etc)
                    if (_serialisedProducts != productsNewSerialization || currentPage.State != ToggleState.On)
                    {
                        currentPage.HasChanges = true;
                        _serialisedProducts = productsNewSerialization;
                        GetCurrentWizardViewModel().ForceReload = false;
                    }
                    else
                    {
                        DateTime? dt = null;
                        if (currentWizardTab != null)
                        {
                            dt = currentWizardTab.LastSavedDate;
                        }
                        RebindNavigation(DataAccess.GetPromotion(page, dt, CurrentPromotion.Id));
                        RebindPageList();
                        return true;
                    }
                }
                else if (page == PromotionWizardDatePage.Key)
                {
                    var v = GetSerialisedDate();
                    PromotionWizardDatePage.Saves = Convert.ToInt16(_dates == v);

                    if (PromotionWizardDatePage.Saves == 0 || currentPage.State != ToggleState.On)
                    {
                        currentPage.HasChanges = true;
                        _dates = GetSerialisedDate();
                        PromotionWizardDatePage.Saves += 1;
                        GetCurrentWizardViewModel().ForceReload = false;
                    }
                    else
                    {
                        DateTime? dt = null;
                        if (currentWizardTab != null)
                        {
                            dt = currentWizardTab.LastSavedDate;
                        }
                        RebindNavigation(DataAccess.GetPromotion(page, dt, CurrentPromotion.Id));
                        RebindPageList();
                        return true;
                    }
                }
                else if (page == PromotionWizardAttributePage.Key)
                {
                    var v = _attributesRvm.Serialize();
                    if (CurrentPromotion.AttributesComment != null)
                        v = string.Format(v + CurrentPromotion.AttributesComment);

                    PromotionWizardAttributePage.Saves = Convert.ToInt16(_attributes == v);

                    if (PromotionWizardAttributePage.Saves == 0
                       || currentPage.State != ToggleState.On)
                    {
                        currentPage.HasChanges = true;
                        _attributes = _attributesRvm.Serialize();
                        PromotionWizardAttributePage.Saves += 1;
                        GetCurrentWizardViewModel().ForceReload = false;
                    }
                    else
                    {
                        DateTime? dt = null;
                        if (currentWizardTab != null)
                        {
                            dt = currentWizardTab.LastSavedDate;
                        }
                        RebindNavigation(DataAccess.GetPromotion(page, dt, CurrentPromotion.Id));
                        RebindPageList();
                        return true;
                    }
                }
                else if (page == PromotionWizardFinancialPage.Key)
                {
                    var v = GetSerialisedFinance();
                    PromotionWizardFinancialPage.Saves = Convert.ToInt16(_financials == v);

                    if (PromotionWizardFinancialPage.Saves == 0
                       || currentPage.State != ToggleState.On)
                    {
                        currentPage.HasChanges = true;
                        _financials = GetSerialisedFinance();
                        PromotionWizardFinancialPage.Saves += 1;
                        GetCurrentWizardViewModel().ForceReload = false;
                    }
                    else
                    {
                        DateTime? dt = null;
                        if (currentWizardTab != null)
                        {
                            dt = currentWizardTab.LastSavedDate;
                        }
                        RebindNavigation(DataAccess.GetPromotion(page, dt, CurrentPromotion.Id));
                        RebindPageList();
                        return true;
                    }
                }
                else if (page == PromotionWizardCustomerPage.Key)
                {
                    var v = GetSerialisedCustomers();
                    PromotionWizardCustomerPage.Saves = Convert.ToInt16(_customers == v);

                    if (PromotionWizardCustomerPage.Saves == 0
                        || currentPage.State != ToggleState.On)
                    {
                        currentPage.HasChanges = true;
                        _customers = GetSerialisedCustomers();
                        PromotionWizardCustomerPage.Saves += 1;
                        GetCurrentWizardViewModel().ForceReload = false;
                    }
                    else
                    {
                        DateTime? dt = null;
                        if (currentWizardTab != null)
                        {
                            dt = currentWizardTab.LastSavedDate;
                        }
                        RebindNavigation(DataAccess.GetPromotion(page, dt, CurrentPromotion.Id));
                        RebindPageList();
                        return true;
                    }
                }
                else if (page == PromotionWizardReviewPage.Key && !currentPage.HasChanges)
                {
                    //no changes, just skip
                    return true;
                }
                else if (!currentPage.HasChanges)
                {
                    DateTime? dt = null;
                    if (currentWizardTab != null)
                    {
                        dt = currentWizardTab.LastSavedDate;
                    }
                    RebindNavigation(DataAccess.GetPromotion(page, dt, CurrentPromotion.Id));
                    RebindPageList();

                    return true;
                }

                currentPage.CanAttemptNavigate = () => false;
            }


            //all other pages/states need to be saved here
            try
            {
                Application.Current.MainWindow.Cursor = Cursors.Wait;

                SetStatus();
                SetScenarios();
                SetDatePeriod();
                SetApprovals();

                var allReleventChecks = performAllRelevantChecks(page, showMessages);

                if (allReleventChecks)
                {

                    if (page == PromotionWizardVolumePage.Key)
                    {
                        XElement v = null;
                        XElement d = null;
                        XElement s = null;

                        if (VolumesRVM != null)
                        {
                            //TODO: move to Model.DataAccess !!!!
                            v = new XElement(PromotionWizardVolumePage.Key);
                            v.AddFirst(XElement.Parse(VolumesRVM.ToXml().ToString()));
                        }


                        if (DisplayVolumesRVM != null && DisplayVolumesRVM.Records != null)
                        {
                            //Display units
                            d = new XElement("DisplayUnits");
                            d.AddFirst(XElement.Parse(DisplayVolumesRVM.ToXml().ToString()));
                        }

                        if (StealVolumesRVM != null && StealVolumesRVM.Records != null)
                        {
                            //cannibalisation units
                            s = new XElement("StealVolumes");
                            s.AddFirst(XElement.Parse(StealVolumesRVM.ToXml().ToString()));
                        }

                        result = DataAccess.SavePromotion(CurrentPromotion, currentWizardTab.LastSavedDate, page, null, v, d, s);

                    }
                    else if (page == PromotionWizardProductPage.Key)
                    {
                        //save products
                        try
                        {

                            var flat = RootProducts.Listings.FlatTree;
                            var prods = flat.Where(r => r.IsParentNode || r.IsSelectedBool != false)
                                .Select(t => new { t.Idx, t.IsParentNode })
                                    .ToDictionary(t => t.Idx, t => t.IsParentNode);

                            result = DataAccess.SavePromotion(CurrentPromotion, currentWizardTab.LastSavedDate, page,
                            prods);
                        }
                        catch
                        {
                        }
                        _statusChanged = false;

                    }
                    else if (page == PromotionWizardFinancialPage.Key)
                    {
                        XElement g1 = null;
                        XElement g2 = null;
                        XElement g3 = null;

                        if (G1PromoFinancialMeasures != null && G1PromoFinancialMeasures.Records != null)
                        {
                            //TODO: move to Model.DataAccess !!!!
                            g1 = new XElement("FinancialScreenPromotionalMeasure");
                            g1.AddFirst(XElement.Parse(G1PromoFinancialMeasures.ToCoreXml().ToString()));
                        }


                        if (G2ParentProductFinancialMeasures != null && G2ParentProductFinancialMeasures.Records != null)
                        {
                            //Display units
                            g2 = new XElement("FinancialScreenParentSkuMeasure");
                            g2.AddFirst(XElement.Parse(G2ParentProductFinancialMeasures.ToXml().ToString()));
                        }

                        if (G3FinancialScreenPlanningSkuMeasure != null && G3FinancialScreenPlanningSkuMeasure.Records != null)
                        {
                            //cannibalisation units
                            g3 = new XElement("FinancialScreenPlanningSkuMeasure");
                            g3.AddFirst(XElement.Parse(G3FinancialScreenPlanningSkuMeasure.ToXml().ToString()));
                        }

                        result = DataAccess.SavePromotion(CurrentPromotion, currentWizardTab.LastSavedDate, page, null, g1, g2, g3);



                    }
                    else if (page == PromotionWizardAttributePage.Key)
                    {
                        XElement g3 = null;

                        if (AttributesRVM != null && AttributesRVM.Records != null)
                        {
                            //cannibalisation units
                            //g3 = new XElement("Attributes");
                            //g3.AddFirst(XElement.Parse(AttributesRVM.ToAttributeXml().ToString()));
                            g3 = XElement.Parse(AttributesRVM.ToAttributeXml().ToString());
                        }

                        result = DataAccess.SavePromotion(CurrentPromotion, currentWizardTab.LastSavedDate, page, null, g3);

                    }
                    else
                    {
                        DateTime? dt = null;
                        if (currentWizardTab != null)
                        {
                            dt = currentWizardTab.LastSavedDate;
                        }

                        // we may need to save the attachments
                        if (DocumentstRvm != null)
                        {
                            var g = new XElement("PromotionAttachments");
                            g.AddFirst(XElement.Parse(DocumentstRvm.ToXml().ToString()));
                            result = DataAccess.SavePromotion(CurrentPromotion, dt, page, null, null, null, null, g);
                        }
                        else
                        {
                            result = DataAccess.SavePromotion(CurrentPromotion, dt, page);
                        }

                        
                    }

                    // UPDATE PRODUCT BACKUP
                    //CurrentPromotion.BackupProducts();
                    reload(onetime);

                }
                else
                {
                    if (currentPage != null)
                    {
                        currentPage.State = ToggleState.Off;
                    }
                    result.ValidationStatus = ValidationStatus.Error;
                    result.Message = "Save failed validation";
                }
            }
            catch (Exception ex)
            {
                //populate dummy result with info for user
                result.ValidationStatus = ValidationStatus.Error;
                result.Message = ex.Message;
                CustomMessageBox.Show(ex.Message, "Error saving promotion");
            }
            //finally 
            //{
            if (page == PromotionWizardAttributePage.Key) currentPage.CanAttemptNavigate = IsValidAttributes;
            else currentPage.CanAttemptNavigate = () => true;

            CommandManager.InvalidateRequerySuggested();

            currentPage.HasChanges = result.ValidationStatus != ValidationStatus.Error;
            currentPage.State = (result.ValidationStatus != ValidationStatus.Error ? ToggleState.On : ToggleState.Indeterminate);
            currentPage.Valid = result.ValidationStatus != ValidationStatus.Error;

            if (result.ValidationStatus != ValidationStatus.Error)
            { 
                if(!string.IsNullOrEmpty(result.Message))
                    CustomMessageBox.Show(result.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);

                ResetLaterPages(GetCurrentWizardViewModel());
                RebindNavigation(result);
            }
            else
            {
                CustomMessageBox.Show(result.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            Application.Current.MainWindow.Cursor = Cursors.Arrow;

            return result.ValidationStatus != ValidationStatus.Error;
            //}
        }

        private string GetSerialisedCustomers()
        {
            var d = "";

            if (CurrentPromotion.CustomerIdx != null)
            {
                d += CurrentPromotion.CustomerIdx.Serialize();
            }

            if (CurrentPromotion.SubCustomerIdxs != null)
            {
                foreach (var subCust in CurrentPromotion.SubCustomerIdxs)
                {
                    d += subCust.Serialize();
                }
            }

            if (Name != null)
            {
                d += Name.Serialize();
            }

            //if(CustomerPage.LastSavedDate != null)
            //    d += CustomerPage.LastSavedDate.Serialize();

            return d;
        }

        private string GetSerialisedProducts()
        {
            var serialisedProducts = string.Empty;

            // if (CurrentPromotion.Products != null)
            // {
            //     d += ProductList.Where(p => p.IsSelected != "0").Select(p => p.ID).ToList().Serialize();
            //     d += ProductList.Where(p => p.IsSelected != "0").Select(p => p.IsParentNode).ToList().Serialize();
            // }

            // JS: I decided to use RootProducts because that is the list which the tree in the view is bound to.
            // ProductList doesn't maintain changing IsSelected of its items properly.
            //var products = RootProducts.FirstOrDefault().GetFlatTree();

            var selectedProductsIds = RootProducts.GetSelectedIdxs();

            if (selectedProductsIds.Any())
            {
                serialisedProducts += selectedProductsIds.Serialize();

                // Get if the selected products are parent planned nodes.
                serialisedProducts += RootProducts.GetSelectedNodes()
                    .Select(p => p.IsParentNode)
                    .ToList().Serialize();
            }

            if (CurrentPromotion.ProductPrices != null)
            {
                serialisedProducts += CurrentPromotion.ProductPrices.Serialize();
            }

            return serialisedProducts;
        }

        private string GetSerialisedDate()
        {
            var d = "";
            if (DateList != null)
            {
                d += _dateList.Serialize();
            }

            if (SelectedPeriod != null)
            {
                d += SelectedPeriod;
            }

            return d;
        }

        private string GetSerialisedVolumes()
        {
            var d = "";

            if (DisplayVolumesRVM != null)
            {
                d += DisplayVolumesRVM.Serialize();
            }

            if (VolumesRVM != null)
            {
                d += VolumesRVM.Serialize();
            }

            if (StealVolumesRVM != null)
            {
                d += StealVolumesRVM.Serialize();
            }


            return d;
        }


        private string GetSerialisedFinance()
        {
            var d = "";

            if (G1PromoFinancialMeasures != null)
            {
                d += G1PromoFinancialMeasures.Serialize();
            }

            if (G2ParentProductFinancialMeasures != null)
            {
                d += G2ParentProductFinancialMeasures.Serialize();
            }

            if (G3FinancialScreenPlanningSkuMeasure != null)
            {
                d += G3FinancialScreenPlanningSkuMeasure.Serialize();
            }

            return d;
        }

        public IEnumerable<PromotionHierarchy> GetFlattened(PromotionHierarchy parent)
        {
            yield return parent;
            if (parent.Children != null)
            {
                foreach (PromotionHierarchy child in parent.Children) // check null if you must
                    foreach (PromotionHierarchy relative in GetFlattened(child))
                        yield return relative;
            }
        }

        private bool VolumesHaveChanged()
        {
            return (DisplayVolumesRVM != null && DisplayVolumesRVM.HasChanged)
                    || (StealVolumesRVM != null && StealVolumesRVM.HasChanged)
                    || (VolumesRVM != null && VolumesRVM.HasChanged);
        }

        /// <summary>
        /// Set all data from save proc back to UI
        /// </summary>
        /// <param name="result"></param>
        private void RebindNavigation(PromotionSaveResults result)
        {
            if (result.ValidationStatus == ValidationStatus.Error)
            {
                ShowWarning(result.Message);
            }

            CurrentPromotion.CodeAndName = result.CodeAndName;
            NotifyPropertyChanged(this, vm => vm.CodeAndName);

            CurrentPromotion.IsEditable = result.IsAmendable;
            CurrentPromotion.Id = result.PromotionID;
            HasCreated = true;
            CurrentWizardPages = result.WizardPages;
            Viewers = new ObservableCollection<PromotionViewingUser>(result.ViewingUsers);
            RebindPageList();
        }

        /// <summary>
        /// Take returned data and reset all navigation icons
        /// </summary>
        /// <param name="wizardPages"></param>
        private void RebindPageList()
        {
            if (CurrentWizardPages == null)
            {
                CurrentWizardPages = new List<PromotionTab>();
                CurrentWizardPages.Add(new PromotionTab()
                {
                    IsCompleted = PromotionTabStatus.NeedsReview,
                    WizardTabCode = PromotionWizardCustomerPage.Key,
                    LastSavedDate = null
                });

            }

            foreach (var p in PageList
                )
            {
                var tab = CurrentWizardPages.SingleOrDefault(r => r.WizardTabCode == p.Name);
                if (tab != null)
                {
                    p.LastSavedDate = tab.LastSavedDate.HasValue ? tab.WizardTabCode + " " + App.CurrentLang.GetValue("Last_Saved", "last saved") + " @ " + tab.LastSavedDate.Value.ToString(CultureInfo.CurrentCulture) : "";

                    switch (tab.IsCompleted)
                    {
                        case PromotionTabStatus.Complete:
                            p.State = ToggleState.On;
                            break;
                        case PromotionTabStatus.NoStarted:
                            p.State = ToggleState.Off;
                            break;

                        case PromotionTabStatus.NeedsReview:
                            p.State = ToggleState.Indeterminate;
                            break;

                        default:
                            throw new Exception("No Promotion tab status found for " + tab.IsCompleted.ToString());
                    }
                }
                else if (p.Name == PromotionWizardReviewPage.Key)
                {
                    p.State = ToggleState.On;
                    p.Visited = true;
                }
            }
        }

        /// <summary>
        /// Get promotion data and set minimum required data
        /// </summary>
        /// <param name="result"></param>
        private void RebindNavigation(PromotionGetResults result)
        {
            if (CurrentPromotion == null)
            {
                CurrentPromotion = new Promotion();
            }

            CurrentPromotion.Name = result.Name;

            CurrentPromotion.CodeAndName = result.CodeAndName;
            NotifyPropertyChanged(this, vm => vm.CodeAndName);

            _customer = CurrentPromotion.Name;
            CurrentPromotion.IsEditable = result.IsAmendable;
            CurrentPromotion.Id = result.PromotionID;
            HasCreated = true;

            if (CurrentPromotion.IsEditable || PromotionWizardReviewPage.Saves == 0)
            {
                PLReviewUrl = result.URL;
                PromotionWizardReviewPage.Saves += 1;
            }


            CurrentPromotion.WizarStartScreenName = result.WizardStartScreenName;
            CurrentPromotion.Status = result.StatusID;
            CurrentWizardPages = result.WizardPages;

            Viewers = new ObservableCollection<PromotionViewingUser>(result.ViewingUsers);

            // RebindPageList();
        }


        private string FixValue(string val)
        {
            var isNum = false;
            decimal d;
            //rip out the localised %
            val = val.Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "")
                .Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol, "");

            if (CultureInfo.CurrentCulture.IetfLanguageTag != ("en-GB"))
            {
                isNum = decimal.TryParse(val, NumberStyles.Number, CultureInfo.CurrentCulture, out d);
            }
            else
            {
                isNum = decimal.TryParse(val, NumberStyles.Any, CultureInfo.GetCultureInfoByIetfLanguageTag("en-GB"), out d);
            }

            if (isNum == true)
            {
                var x = d.ToString(CultureInfo.GetCultureInfoByIetfLanguageTag("en-GB"));
                return x;
            }
            else
            {
                return val;
            }

        }


        private decimal FixDecimal(string val)
        {
            var isNum = false;
            decimal d;
            //rip out the localised %
            val = val.Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");

            isNum = decimal.TryParse(val, NumberStyles.Any, CultureInfo.CurrentCulture, out d);

            if (isNum == true)
            {
                return d;
            }
            else
            {
                return 0;
            }

        }



        public void reload(bool onetime)
        {
            if (onetime)
            {
                _editingPromoWizardViewModel = new PromotionWizardViewModel(null, CurrentPromotion.Id);
            }
        }

        #endregion

        #region General Methods

        /// <summary>
        ///     Shows a warning message with warning style.
        /// </summary>
        public void ShowWarning(string text)
        {
            MessageBoxShow(text, "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public bool CanAddComment(object param)
        {
            //return NewComment != "";
            return true;
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
                string res = DataAccess.AddPromotionComment(CurrentPromotion.Id, NewComment);
                MessageBoxShow(res);

                NewComment = "";
                NotifyPropertyChanged(this, vm => vm.NewComment);

                InitCommentList();
                NotifyPropertyChanged(this, vm => vm.CommentList);
            }
            catch (ExceedraDataException ex)
            {
                MessageBoxShow(ex.Message, "Error");
            }
        }

        #endregion

        //private Visibility _PromotionHasID { get; set; }
        //public Visibility PromotionHasID
        //{
        //    get {
        //        return _PromotionHasID;
        //    }
        //    set { _PromotionHasID = value;
        //    NotifyPropertyChanged(this, vm => vm.PromotionHasID);
        //    }
        //}

        private bool HasID()
        {
            return (!String.IsNullOrWhiteSpace(CurrentPromotion.Id));
        }

        #endregion

        public ClientConfiguration Configuration
        {
            get { return _configuration; }
            private set
            {
                _configuration = value;
                NotifyPropertyChanged(this, vm => vm.Configuration);
            }
        }



        #region View Product Data

        //public ICommand ;

        #endregion

        private RecordViewModel _dashboardRvm = new RecordViewModel();
        public RecordViewModel DashboardRVM
        {
            get
            {
                return _dashboardRvm;
            }
            set
            {
                if (_dashboardRvm != value)
                {
                    _dashboardRvm = value;
                    NotifyPropertyChanged(this, vm => vm.DashboardRVM);
                }
            }
        }

        private RecordViewModel _dashboardRvm2 = new RecordViewModel();
        public RecordViewModel DashboardRVM2
        {
            get
            {
                return _dashboardRvm2;
            }
            set
            {
                if (_dashboardRvm2 != value)
                {
                    _dashboardRvm2 = value;
                    NotifyPropertyChanged(this, vm => vm.DashboardRVM2);
                }
            }
        }

        private RecordViewModel _dashboardRvm3 = new RecordViewModel();
        public RecordViewModel DashboardRVM3
        {
            get
            {
                return _dashboardRvm3;
            }
            set
            {
                if (_dashboardRvm3 != value)
                {
                    _dashboardRvm3 = value;
                    NotifyPropertyChanged(this, vm => vm.DashboardRVM3);
                }
            }
        }

        private RecordViewModel _dashboardRvm4 = new RecordViewModel();
        public RecordViewModel DashboardRVM4
        {
            get
            {
                return _dashboardRvm4;
            }
            set
            {
                if (_dashboardRvm4 != value)
                {
                    _dashboardRvm4 = value;
                    NotifyPropertyChanged(this, vm => vm.DashboardRVM4);
                }
            }
        }

        private void LoadDashboardRVM()
        {

            UpdateSubMessage("Dashboard");

            DashboardRVM.IsDataLoading = true;
            DashboardRVM2.IsDataLoading = true;
            DashboardRVM3.IsDataLoading = true;
            DashboardRVM4.IsDataLoading = true;

            // FireisDirtyAsync();
            //if ((DashboardRVM == null || DashboardRVM.HasRecords) ||
            //     (GetCurrentPage(PromotionWizardFinancialPage.Key).State != ToggleState.On))
            // { 
            DataAccess.GetPromotionDashboardXAsync(CurrentPromotion.Id, StoredProcedure.Promotion.GetPanLGridFirst) //Procast_SP_PROMO_GetPandLGrid_First
                .ContinueWith(Dash1, App.Scheduler);
            // }


            //if ((DashboardRVM2 == null || DashboardRVM2.HasRecords) ||
            //     (GetCurrentPage(PromotionWizardFinancialPage.Key).State != ToggleState.On))
            //{
            DataAccess.GetPromotionDashboardXAsync(CurrentPromotion.Id, StoredProcedure.Promotion.GetPanLGridSecond) //Procast_SP_PROMO_GetPandLGrid_Second
                .ContinueWith(Dash2, App.Scheduler);
            //}
            //if ((DashboardRVM3 == null || DashboardRVM3.HasRecords) ||
            //     (GetCurrentPage(PromotionWizardFinancialPage.Key).State != ToggleState.On))
            //{
            DataAccess.GetPromotionDashboardXAsync(CurrentPromotion.Id, StoredProcedure.Promotion.GetPanLGridThird) //Procast_SP_PROMO_GetPandLGrid_Third
                .ContinueWith(Dash3, App.Scheduler);
            //}

            //if ((DashboardRVM4 == null || DashboardRVM4.HasRecords) ||
            //     (GetCurrentPage(PromotionWizardFinancialPage.Key).State != ToggleState.On))
            //{
            DataAccess.GetPromotionDashboardXAsync(CurrentPromotion.Id, StoredProcedure.Promotion.GetPanLGridFourth) //Procast_SP_PROMO_GetPandLGrid_Fourth
                .ContinueWith(Dash4, App.Scheduler);
            //}

            UpdateSubMessage("Sceanrios");
            InitPromotionScenarios();

            UpdateSubMessage("Statuses");
            InitPromotionStatuses();

            if (CurrentPromotion.URL == null)
            {
                PLReviewUrl = DataAccess.GetPromotion(PromotionWizardReviewPage.Key, null, CurrentPromotion.Id).URL;
            }
            else
            {
                PLReviewUrl = CurrentPromotion.URL;
            }


            if(App.Configuration.IsPromoReportTabVisible)
            LoadReportGrid();


            if (App.Configuration.IsPromoDocumentTabVisible)
                LoadDocumentGrid();

            if (App.Configuration.IsPromoCanvasReportTabVisible)
                LoadCanvasReport();
        }

        private RecordViewModel _rportRvm;
        public RecordViewModel ReportRvm
        {
            get
            {
                return _rportRvm;
            }
            set
            {
                if (_rportRvm != value)
                {
                    _rportRvm = value;
                    NotifyPropertyChanged(this, vm => vm.ReportRvm);
                }
            }
        }

        private RecordViewModel _documentsRvm;
        public RecordViewModel DocumentstRvm
        {
            get
            {
                return _documentsRvm;
            }
            set
            {
                if (_documentsRvm != value)
                {
                    _documentsRvm = value;
                    NotifyPropertyChanged(this, vm => vm.DocumentstRvm);
                }
            }
        }

        private CellsGridViewModel _cellsGrid;
        public CellsGridViewModel CellsGrid
        {
            get { return _cellsGrid; }
            set
            {
                _cellsGrid = value;
                NotifyPropertyChanged(this, vm => vm.CellsGrid);
            }
        }

        public void LoadReportGrid()
        {
            if (App.Configuration.IsPromoReportTabVisible)
            {
                DataAccess.GetPromotionDashboardXAsync(CurrentPromotion.Id,
                    StoredProcedure.Promotion.GetReportGrid) //Procast_SP_PROMO_GetPandLGrid_Fourth
                    .ContinueWith(ReportGrid, App.Scheduler);
            }
        }

        public void LoadDocumentGrid()
        {
            if (App.Configuration.IsPromoDocumentTabVisible)
            {
                DataAccess.GetPromotionDocumentsAsync(CurrentPromotion.Id,
                    StoredProcedure.Promotion.GetDocumentGrid) //Procast_SP_PROMO_GetPandLGrid_Fourth
                    .ContinueWith(DocumentsGrid, App.Scheduler);
            }
        }

        public void LoadCanvasReport()
        {
            CellsGrid = CellsGridViewModel.GetEmptyCellsGrid(User.CurrentUser.ID, User.CurrentUser.SalesOrganisationID,
                App.Configuration.DefaultPromoCanvasReportId, ScreenKeys.PROMOTION.ToString());

            CellsGrid.LoadControlsData(App.Configuration.DefaultPromoCanvasReportId, new XElement("Promo_Idx", CurrentPromotion.Id));
        }

        private void Dash1(Task<XElement> r)
        {

            if (r.Result == null)
            {
                //DashboardRVM = null;
                DashboardRVM = new RecordViewModel(false);
            }
            else
            {
                //DashboardRVM = new RecordViewModel();
                DashboardRVM = new RecordViewModel(r.Result) {GridTitle = Dash1Header};
            }
        }

        private void Dash2(Task<XElement> r)
        {

            if (r.Result == null)
            {
                DashboardRVM2 = new RecordViewModel(false);
            }
            else
            {
                //DashboardRVM3 = new RecordViewModel();
                DashboardRVM2 = new RecordViewModel(r.Result) {GridTitle = Dash2Header};
            }
        }

        private void Dash3(Task<XElement> r)
        {

            if (r.Result == null)
            {
                DashboardRVM3 = new RecordViewModel(false);
            }
            else
            {
                //DashboardRVM3 = new RecordViewModel();
                DashboardRVM3 = new RecordViewModel(r.Result) {GridTitle = Dash3Header};
            }
        }

        private void Dash4(Task<XElement> r)
        {

            if (r.Result == null)
            {
                DashboardRVM4 = new RecordViewModel(false);
            }
            else
            {
                //DashboardRVM4 = new RecordViewModel();
                DashboardRVM4 = new RecordViewModel(r.Result) {GridTitle = Dash4Header};
            }
        }

        private void ReportGrid(Task<XElement> r)
        {

            if (r.Result == null)
            {
                ReportRvm = null;
            }
            else
            {
                ReportRvm = new RecordViewModel(r.Result);
                ReportRvm.GridTitle = ReportRvm.GridTitle;
            }
        }

        private void DocumentsGrid(Task<XElement> r)
        {

            if (r.Result == null)
            {
                DocumentstRvm = null;
            }
            else
            {
                DocumentstRvm = new RecordViewModel(r.Result);
                DocumentstRvm.PropertyChanged += DocumentstRvmParentProductFinancialMeasures_PropertyChanged;
            }
        }

        private void DocumentstRvmParentProductFinancialMeasures_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Records")
            {
                SetPageChangedStatus(PromotionWizardReviewPage.Key, true);
            }
        }



        private void UpdateSubMessage(string t)
        {
            try
            {

                Dispatcher.Invoke(new Action(delegate
                {
                    if (App.WizardNavigator != null)
                        App.WizardNavigator.loadingPanel.SubMessage += ". ";// + DateTime.Now + Environment.NewLine;
                }));

            }
            catch (Exception)
            {


            }

        }

        private string Dash1Header
        {
            get { return App.CurrentLang.GetValue("Label_Dashboard_Grid1", "Grid 1"); }
        }

        private string Dash2Header
        {
            get { return App.CurrentLang.GetValue("Label_Dashboard_Grid2", "Grid 2"); }
        }

        private string Dash3Header
        {
            get { return App.CurrentLang.GetValue("Label_Dashboard_Grid3", "Grid 3"); }
        }

        private string Dash4Header
        {
            get { return App.CurrentLang.GetValue("Label_Dashboard_Grid4", "Grid 4"); }
        }

        public void GetIsParentNode(PromotionHierarchy c)
        {
            if (c.Children != null)
            {
                try
                {
                    var p = ProductList.FirstOrDefault(r => r.ID == c.ID);
                    p.IsParentNode = c.IsSelected2;

                    SetPageChangedStatus(PromotionWizardProductPage.Key, true);
                }
                catch (Exception)
                {

                }

            }
        }



        public void FireG1PromoFinancialMeasuresChanges(bool resetAllowed = true, bool rowTotalAllowed = true)
        {
            //its changed!
            SetPageChangedStatus(PromotionWizardFinancialPage.Key, true);

            //if (G1PromoFinancialMeasures != null && G2ParentProductFinancialMeasures != null && DisplayVolumesRVM.Records != null)
            //{
            ProcessMeasureUpdatingParentProducts();
            //}


            // throw new NotImplementedException("Not done");
        }


        private void FireG2ParentProductFinancialMeasuresChanges()
        {
            //its changed!
            SetPageChangedStatus(PromotionWizardFinancialPage.Key, true);

            ProcessMeasureUpdatingFromG2();
        }

        private void GetG2ValuesFromG3()
        {
            // find all SUMIF column in G2
            foreach (var dvRVM in G2ParentProductFinancialMeasures.Records)
            {
                //    <Value>=SUMIFS(FinancialScreenPlanningSkuMeasure$TestColumn, FinancialScreenPlanningSkuMeasure$ParentSkuIdx, ParentSkuIdx)</Value>

                //check each SUMIF column and update parent grid based on values
                var sumifs = dvRVM.Properties.Where(p => p.Calculation != null).Where(t => t.Calculation.Contains("SUMIFS"));

                // NotifyPropertyChanged(this, vm => vm.G3FinancialScreenPlanningSkuMeasure);

                foreach (var ec in sumifs)
                {
                    Decimal sum = 0;

                    var calc = ec.Calculation.Replace("=SUMIFS", "").Replace("(", "").Replace(")", "").Replace(" ", "").Replace("SUMIFS", "");

                    string[] calcArrary;

                    if (calc.Contains("+"))
                    {
                        calcArrary = calc.Split('+');
                    }
                    else
                    {
                        calcArrary = new string[1];
                        calcArrary[0] = calc;
                    }

                    foreach (var thisCalc in calcArrary)
                    {

                        var options = thisCalc.Split(',');

                        //should be three options
                        if (options.Count() != 3)
                        {
                            return;
                        }

                        var childValueColumn = TableColumn(options[0]);
                        var parentIdColumn = TableColumn(options[1]);
                        var parentIDx = options[2];

                        //find the childValue column based on the childParentID
                        var parentID =
                            dvRVM.Properties.FirstOrDefault(
                                p => p.ColumnCode.ToLowerInvariant() == parentIDx.ToLowerInvariant());


                        if (childValueColumn.Item1 == "G3" && parentID != null)
                        {

                            //Get the Value for each item in the correct column where the parentID is correct
                            foreach (var cta in G3FinancialScreenPlanningSkuMeasure.Records)
                            {
                                //are we in the right Row based on parentID
                                var col =
                                    cta.Properties.FirstOrDefault(
                                        prop =>
                                            prop.ColumnCode.ToLowerInvariant() ==
                                            parentIdColumn.Item2.ToLowerInvariant() && prop.Value == parentID.Value);

                                if (col != null)
                                {
                                    var x =
                                        cta.Properties.FirstOrDefault(
                                            p =>
                                                p.ColumnCode.ToLowerInvariant() ==
                                                childValueColumn.Item2.ToLowerInvariant());
                                    if (x != null) sum += FixDecimal(x.Value);

                                }
                            }

                        }


                        ec.Value = sum.ToString(CultureInfo.CurrentCulture);

                        NotifyPropertyChanged(this, vm => vm.G2ParentProductFinancialMeasures);
                        G2ParentProductFinancialMeasures.CalulateRecordColumns();
                    }


                }
            }

        }

        private Tuple<string, string> TableColumn(string UpdateToColumn)
        {

            var updates = UpdateToColumn.Split('$');
            try
            {
                var table = updates[0];
                var column = updates[1];

                return new Tuple<string, string>(table, column);
            }
            catch (Exception)
            {
                return null;
            }

        }

        private void ProcessMeasureUpdatingParentProducts()
        {
            foreach (var dvRVM in G1PromoFinancialMeasures.Records)
            {

                //check each editable column and update parent grid based on values
                foreach (var ec in dvRVM.Properties.Where(p => p.IsEditable && !String.IsNullOrEmpty(p.UpdateToColumn)))
                {
                    var updates = ec.UpdateToColumn.Split('$');
                    string table = "";
                    string column = "";

                    table = updates[0];
                    column = updates[1];

                    bool fireG2 = false;
                    bool fireG3 = false;

                    if (!string.IsNullOrEmpty(table))
                    {
                        var target = new RecordViewModel();
                        if (table == "G2")
                        {
                            target = G2ParentProductFinancialMeasures;
                            fireG2 = true;
                        }
                        else if (table == "G3")
                        {
                            target = G3FinancialScreenPlanningSkuMeasure;
                            fireG3 = true;
                        }
                        else
                        {

                        }
                        if (target.Records != null)
                        {
                            // GO THROUGH EACH PARENT GRID COLUMN TO FIND THE ONE THAT NEEDS TO BE UPDATED
                            foreach (var cta in target.Records)
                            {
                                Exceedra.Controls.DynamicGrid.Models.Property col =
                                    cta.Properties.FirstOrDefault(p => p.ColumnCode == column);
                                if (col != null)
                                {
                                    // set value based on promo measures grid
                                    col.Value = ec.Value;

                                }
                            }

                            if (fireG2)
                                NotifyPropertyChanged(this, vm => vm.G2ParentProductFinancialMeasures);

                            if (fireG3)
                                NotifyPropertyChanged(this, vm => vm.G3FinancialScreenPlanningSkuMeasure);
                        }
                    }

                    if (fireG2)
                        G2ParentProductFinancialMeasures.CalulateRecordColumns();

                    if (fireG3)
                        G3FinancialScreenPlanningSkuMeasure.CalulateRecordColumns();

                }

            }

        }

        private void ProcessMeasureUpdatingFromG2()
        {
            foreach (var dvRVM in G2ParentProductFinancialMeasures.Records)
            {

                //check each editable column and update parent grid based on values
                foreach (var ec in dvRVM.Properties.Where(p => p.IsEditable == true))
                {
                    var updates = ec.UpdateToColumn.Split('$');
                    string table = "";
                    string column = "";
                    try
                    {
                        table = updates[0];
                        column = updates[1];
                    }
                    catch (Exception)
                    {


                    }

                    bool fireG3 = false;

                    if (!string.IsNullOrEmpty(table))
                    {
                        var target = new RecordViewModel();
                        if (table == "G3")
                        {
                            target = G3FinancialScreenPlanningSkuMeasure;
                            fireG3 = true;
                        }
                        else
                        {

                        }
                        if (target.Records != null)
                        {
                            // GO THROUGH EACH PARENT GRID COLUMN TO FIND THE ONE THAT NEEDS TO BE UPDATED
                            foreach (var cta in target.Records)
                            {

                                var thisOne = cta.Properties.FirstOrDefault(p => p.ColumnCode == "ParentSkuIdx").Value == ec.IDX;

                                if (thisOne)
                                {
                                    var col =
                                        cta.Properties.FirstOrDefault(p => p.ColumnCode == column);
                                    if (col != null)
                                    {
                                        // set value based on promo measures grid
                                        col.Value = ec.Value;

                                    }
                                }
                            }


                            if (fireG3)
                                NotifyPropertyChanged(this, vm => vm.G3FinancialScreenPlanningSkuMeasure);
                        }
                    }


                    if (fireG3)
                        G3FinancialScreenPlanningSkuMeasure.CalulateRecordColumns();

                }

            }

        }


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

        private void LoadAttributesRVM()
        {
            if ((AttributesRVM == null || AttributesRVM.Records == null)
                    || (GetCurrentPage(PromotionWizardAttributePage.Key).State != ToggleState.On)
                    || (GetCurrentPage(PromotionWizardAttributePage.Key).ForceReload))
            {
                DataAccess.GetPromotionAttributesAsync(StoredProcedure.Promotion.GetAttributes, CurrentPromotion.Id)
                    .ContinueWith(AttributesContinuation, App.Scheduler);
            }


        }


        private void AttributesContinuation(Task<XElement> res)
        {

            if (res.Result == null)
            {
                AttributesRVM = null;
            }
            else
            {
                AttributesRVM = RowViewModel.LoadWithData(res.Result);
                AttributeComment = res.Result.GetValue<string>("AttributeComment");
            }

            //check there are no dropdowns in the mix

            foreach (var col in AttributesRVM.Records.ToList())
            {

                foreach (var p in col.Properties.Where(r => r.ControlType.Contains("down")).ToList())
                {
                    col.InitialDropdownLoad(p);
                }

            }

            _attributes = AttributesRVM.Serialize();
            if (CurrentPromotion.AttributesComment != null)
                _attributes = string.Format(_attributes + CurrentPromotion.AttributesComment);



            //  }


        }

    }
}
