using Exceedra.Common;
using Exceedra.Common.Utilities;
using Model.Entity.Listings;
using WPF.Navigation;
using WPF.UserControls.Trees.DataAccess;
using WPF.UserControls.Trees.ViewModels;
// ReSharper disable CheckNamespace
using Model.Entity;


namespace WPF.ViewModels
// ReSharper restore CheckNamespace
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using Coder.WPF.UI;
    using Model;
    using Model.DataAccess;
    using ViewHelper;
    using global::ViewModels;
    using System.Threading.Tasks;
    using Exceedra.Controls.DynamicGrid.ViewModels;
    using System.Xml.Linq;
    using System.Windows.Threading;
    using System.Windows.Automation;
    using Exceedra.Controls.DynamicRow.ViewModels;
    using WPF.PromoTemplates;
    using Exceedra.Controls.Messages;


    public abstract class PromotionTemplateViewModelBase : TemplateViewModelBase
    {
        private BackgroundWorker _backgroundWorker = new BackgroundWorker();


        protected const string PostPromoVol = "Post Promo Vol";
        protected const string BasePerWeek = "Base per week";

        protected readonly PromotionTemplateAccess DataTemplateAccess;


        public TemplatePageViewModel AttributesPage;
        public TemplatePageViewModel CustomerPage;
        public TemplatePageViewModel DatesPage;
        public TemplatePageViewModel FinancialsPage;
        public TemplatePageViewModel ProductsPage;
        public TemplatePageViewModel DashboardPage;
        public TemplatePageViewModel PAndLReviewPage;

        private ICommand _saveCommand;

        private ICommand _reloadCommand;

        private ICommand _viewDailyPromotionCommand;
        private ICommand _viewWeeklyPromotionCommand;
        private ViewCommand _uploadSubCustomersViaCsvCommand;
        private ClientConfiguration _configuration;
        private bool _isEnabled;

        private bool HasCreated = false;

        private PWP PromotionTemplateCustomerPage { get; set; }
        private PWP PromotionTemplateDatePage { get; set; }
        private PWP PromotionTemplateProductPage { get; set; }
        private PWP PromotionTemplateAttributePage { get; set; }
        private PWP PromotionTemplateFinancialPage { get; set; }
        private PWP PromotionTemplateReviewPage { get; set; }

        protected PromotionTemplateViewModelBase(PromotionTemplateAccess DataTemplateAccess)
            : this(null, null, DataTemplateAccess)
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


        private bool _isReadOnly;
        public bool IsReadOnly
        {
            get
            { return _isReadOnly; }
            set
            {
                _isReadOnly = value;
                NotifyPropertyChanged(this, vm => vm.IsReadOnly);
            }
        }

        private bool _isEditable;

        public bool IsEditable
        {
            get { return _isEditable; }
            set
            {
                _isEditable = value;
                if (SubCustomersTreeInput != null) 
                    SubCustomersTreeInput.IsReadOnly = !IsEditable;

                NotifyPropertyChanged(this, vm => vm.IsEditable);
            }
        }



        protected PromotionTemplateViewModelBase(ISearchableTreeViewNodeEventsConsumer eventsConsumer, string promotionId, PromotionTemplateAccess dataTemplateAccess)
        {
            _attributes = "";
            _financials = "";

            PromotionTemplateCustomerPage = new PWP("Customer", App.CurrentLang.GetValue("Wizard_Customers", "Customers"));
            PromotionTemplateDatePage = new PWP("Dates", App.CurrentLang.GetValue("Wizard_Dates", "Dates"));
            PromotionTemplateProductPage = new PWP("Products", App.CurrentLang.GetValue("Wizard_Products", "Products"));
            PromotionTemplateAttributePage = new PWP("Attributes", App.CurrentLang.GetValue("Wizard_Attributes", "Attributes"));
            PromotionTemplateFinancialPage = new PWP("Financials", App.CurrentLang.GetValue("Wizard_Financials", "Financials"));
            PromotionTemplateReviewPage = new PWP("Review", App.CurrentLang.GetValue("Wizard_Review", "Review"));

            _backgroundWorker.DoWork += _backgroundWorker_DoWork;
            _backgroundWorker.RunWorkerCompleted += _backgroundWorker_RunWorkerCompleted;


            DataTemplateAccess = dataTemplateAccess;
            if (dataTemplateAccess == null) throw new ArgumentNullException("DataTemplateAccess");

            //  TemplateAccessBase.ForceCustomerCacheRefresh();

            _uploadSubCustomersViaCsvCommand = new ViewCommand(AnySubCustomerAvailable, LoadSubCustomersFromCsv);

            EventsConsumer = eventsConsumer;
            if (InitPromotion(promotionId) != true)
            {
                GoToPromotionsCommand.Execute(true);
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

        // Completed Method
        private void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

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
                CurrentTemplate.Name = _name;
                if (!PromotionTemplateCustomerPage.FirstLoad)
                {
                    SetPageChangedStatus(PromotionTemplateCustomerPage.Key, true);
                }
            }
        }

        public ICommand UploadSubCustomersViaCsvCommand
        {
            get { return _uploadSubCustomersViaCsvCommand; }
        }

        private PromotionWizardViewModelBase _editingPromoWizardViewModel;

        private void ExecuteSaveCommand(object _)
        {
            if (SavePromotion(PromotionTemplateReviewPage.Key))
            {
                LoadDashboardRVM();
                PAndLReviewPage.HasChanges = false;
            }
        }

        public ICommand PLReviewNextCommand
        {
            get { return new ViewCommand(PLReviewNextPage); }
        }

        private void PLReviewNextPage(object parameter)
        {
            FinalSave(null);
        }


        private void ExecuteReloadCommand(object _)
        {
            var x = GetCurrentWizardViewModel();
            //if (x.HasChanges)
            //{
            x.ForceReload = true;
            x.BeforeNavigateInTo.Invoke();
            x.BeforeNavigateBackTo.Invoke();
            x.ForceReload = false;
            x.HasChanges = false;
            //}
        }

        public ICommand SaveCommand
        {
            get
            {
                return _saveCommand ??
                       (_saveCommand = new ViewCommand(ExecuteSaveCommand));//_ => _statusChanged || _scenariosChanged, 
            }
        }

        private string LoadedName;

        public ICommand ReloadCommand
        {
            get
            {
                return _reloadCommand ??
                       (_reloadCommand = new ViewCommand(_ => true, ExecuteReloadCommand));
                //GetCurrentWizardViewModel().HasChanges
            }
        }



        public abstract bool IsSubCustomerActive { get; }

        private static bool AnySearchableNodes(IEnumerable<TreeViewHierarchy> nodes)
        {
            return nodes != null && nodes.Any();
        }

        private static void SelectAllSearchableNodes(IEnumerable<TreeViewHierarchy> nodes)
        {
            foreach (TreeViewHierarchy node in nodes)
            {
                node.IsSelectedBool = true;
                if (node.Children != null)
                {
                    SelectAllSearchableNodes(node.Children);
                }
            }
        }

        private void LoadSubCustomersFromCsv(object obj)
        {
            IEnumerable<string> fileContent = IOService.ReadCsvFile();
            if (fileContent == null) return;

            try
            {
                var res = _treeAccess.GetSubCustomers(CurrentTemplate.Id, User.CurrentUser.ID, fileContent);
                AllNode = new TreeViewHierarchy(res);

                SetVisibleSubCustomers(true);
            }
            catch (Exception ex)
            {

            }
        }

        private bool AnySubCustomerAvailable(object obj)
        {
            return VisibleSubCustomers != null && VisibleSubCustomers.Children != null && VisibleSubCustomers.Children.Any();
        }

        private Action InitialiseVisiteds()
        {
            return () => PageList.ForEach(page => page.Visited = (page.State == ToggleState.On));
        }

        private void CreatePages()
        {


            UpdateSubMessage("Module: " + PromotionTemplateCustomerPage.Title);
            PromotionTemplateCustomerPage.FirstLoad = true;
            CustomerPage = new TemplatePageViewModel(this, PromotionTemplateCustomerPage.Key)
            {
                Title = PromotionTemplateCustomerPage.Title,
                Visited = true,
                PageViewType = typeof(TemplateCustomer),
                CanAttemptNavigate = IsValidCustomer,
                //  Validate = ValidateCustomerData,
                BeforeNavigateAway = CustomerPageBeforeNavigateAway,
                // AfterNavigateAway = () => SavePromotion(PromotionTemplateCustomerPage, true, false, CustomerPage.Valid),              
                BeforeNavigateInTo = () =>
                {
                    //if (PromotionTemplateCustomerPage.FirstLoad ||
                    //    GetCurrentPage(PromotionTemplateCustomerPage.Key).ForceReload)
                    //{
                    InitCustomers(CurrentTemplate.Id);
                    _customer = GetSerialisedCustomers();
                    // }
                },
                BeforeNavigateBackTo = () =>
                {
                    KickOffRebingNavigation(PromotionTemplateCustomerPage);
                }
            };
            CustomerPage.HasChanges = false;


            PromotionTemplateDatePage.FirstLoad = true;
            UpdateSubMessage("Module: " + PromotionTemplateDatePage.Title);
            DatesPage = new TemplatePageViewModel(this, PromotionTemplateDatePage.Key)
            {
                Title = PromotionTemplateDatePage.Title,
                Visited = false,
                PageViewType = typeof(TemplateDates),
                CanAttemptNavigate = () => CanDatesMoveNext(null),
                Validate = IsValidDates,
                BeforeNavigateAway = () => SavePromotion(PromotionTemplateDatePage.Key, true, false, DatesPage.Valid),
                BeforeNavigateInTo = () =>
                {
                    if (PromotionTemplateDatePage.FirstLoad
                        || GetCurrentPage(PromotionTemplateDatePage.Key).ForceReload)
                    {
                        InitDateList();

                        PromotionTemplateDatePage.FirstLoad = false;
                        GetCurrentPage(PromotionTemplateDatePage.Key).ForceReload = false;
                        DatesPage.HasChanges = true;
                        _dates = GetSerialisedDate();
                    }
                },
                BeforeNavigateBackTo = () =>
                {
                    KickOffRebingNavigation(PromotionTemplateDatePage);
                }
            };



            PromotionTemplateProductPage.FirstLoad = true;
            UpdateSubMessage("Module: " + PromotionTemplateProductPage.Title);
            ProductsPage = new TemplatePageViewModel(this, PromotionTemplateProductPage.Key)
            {
                Title = PromotionTemplateProductPage.Title,
                Visited = false,
                PageViewType = typeof(TemplateProducts),
                CanAttemptNavigate = () => CanProductsMoveNext(null),
                // Validate = ValidateProductData,
                BeforeNavigateAway = () => SavePromotion(PromotionTemplateProductPage.Key, true, false, ProductsPage.Valid),
                BeforeNavigateInTo = () =>
                {
                    if (PromotionTemplateProductPage.FirstLoad
                        || GetCurrentPage(PromotionTemplateProductPage.Key).ForceReload)
                    {
                        InitProducts();

                        GetCurrentPage(PromotionTemplateProductPage.Key).ForceReload = false;
                    }


                },
                BeforeNavigateBackTo = () =>
                {
                    KickOffRebingNavigation(PromotionTemplateProductPage);
                }
            };



            PromotionTemplateAttributePage.FirstLoad = true;
            UpdateSubMessage("Module: " + PromotionTemplateAttributePage.Title);
            AttributesPage = new TemplatePageViewModel(this, PromotionTemplateAttributePage.Key)
            {
                Title = PromotionTemplateAttributePage.Title,
                Visited = false,
                PageViewType = typeof(TemplateAttributes),
                CanAttemptNavigate = IsValidAttributes,
                // Validate = ValidateAttributeData,
                BeforeNavigateAway = () => SavePromotion(PromotionTemplateAttributePage.Key, true, false, AttributesPage.Valid),
                BeforeNavigateInTo = () =>
                {
                    if (PromotionTemplateAttributePage.FirstLoad
                        || GetCurrentPage(PromotionTemplateAttributePage.Key).ForceReload)
                    {
                        InitAttributes();
                        AttributesPage.HasChanges = false;
                        GetCurrentPage(PromotionTemplateAttributePage.Key).ForceReload = false;
                    }
                },
                BeforeNavigateBackTo = () =>
                {
                    KickOffRebingNavigation(PromotionTemplateAttributePage);
                }
            };


            PromotionTemplateFinancialPage.FirstLoad = true;
            UpdateSubMessage("Module: " + PromotionTemplateFinancialPage.Title);
            FinancialsPage = new TemplatePageViewModel(this, PromotionTemplateFinancialPage.Key)
            {
                Title = PromotionTemplateFinancialPage.Title,
                Visited = false,
                PageViewType = typeof(TemplateFinancials),
                //CanAttemptNavigate = IsValidFinancials,
                // Validate = ValidateFinancialData,
                BeforeNavigateInTo = () =>
                {
                    if (PromotionTemplateFinancialPage.FirstLoad ||
                        GetCurrentPage(PromotionTemplateFinancialPage.Key).ForceReload)
                    {
                        InitFinancials();
                        //LoadProductFinancialPrices();
                        PromotionTemplateFinancialPage.FirstLoad = false;
                        _financials = GetSerialisedFinance();

                        FinancialsPage.HasChanges = false;
                    }
                },
                BeforeNavigateAway = () => SavePromotion(PromotionTemplateFinancialPage.Key, true, false, FinancialsPage.Valid),
                Reset = () =>
                {
                    if (CurrentTemplate.FinancialVariables != null)
                        CurrentTemplate.FinancialVariables.Clear();
                    if (CurrentTemplate.FinancialProductVariables != null)
                        CurrentTemplate.FinancialProductVariables.Clear();
                    if (PromotionFinancials != null) PromotionFinancials.Clear();
                    if (ProductFinancialsPricesList != null) ProductFinancialsPricesList.Clear();
                },
                BeforeNavigateBackTo = () =>
                {
                    KickOffRebingNavigation(PromotionTemplateFinancialPage);
                }

            };


            PromotionTemplateReviewPage.FirstLoad = true;
            UpdateSubMessage("Module: " + PromotionTemplateReviewPage.Title);
            PAndLReviewPage = new TemplatePageViewModel(this, PromotionTemplateReviewPage.Key)
            {
                Title = PromotionTemplateReviewPage.Title,
                Visited = false,
                PageViewType = typeof(TemplatePLReview),
                //  Validate = ValidatePromotionReview,
                CanAttemptNavigate = () => true,
                Valid = true,
                BeforeNavigateInTo = () =>
                {
                    if (PromotionTemplateReviewPage.FirstLoad ||
                        GetCurrentPage(PromotionTemplateReviewPage.Key).ForceReload)
                    {
                        GetCurrentPage(PromotionTemplateReviewPage.Key).ForceReload = true;
                        LoadDashboardRVM();
                        PAndLReviewPage.HasChanges = false;
                    }
                }
            };



            PageList = new List<TemplatePageViewModel>
                {
                    CustomerPage,
                    DatesPage,
                    ProductsPage,
                    AttributesPage,                   
                    FinancialsPage,
                    PAndLReviewPage
                };

            RebindPageList();

        }

        private PromotionTab GetCurrentWizardPageTab(string page)
        {
            return CurrentWizardPages.SingleOrDefault(r => r.WizardTabCode == page);
        }

        private TemplatePageViewModel GetCurrentWizardViewModel()
        {
            return PageList.SingleOrDefault(r => r.IsCurrent);
        }

        private void KickOffRebingNavigation(PWP current)
        {
            if (CurrentTemplate.Id != null)
            {
                RebindNavigation(DataTemplateAccess.GetTemplate(CurrentTemplate.Id, current.Key, GetCurrentWizardPageTab(current.Key).LastSavedDate));
            }
            else
            {
                InitCustomers(CurrentTemplate.Id);
                CustomerPage.HasChanges = false;
                Name = "";
            }
        }

        private void ShowStartUpScreen(Action callback)
        {
            UpdateSubMessage("Navigating to startup");

            TemplatePageViewModel startupPage = GetCurrentPage(StartUpScreen) ?? CustomerPage;
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
                CurrentTemplate = new PromotionTemplate();
                CurrentTemplate.IsEditable = true;
                IsEditable = true;
                UpdateSubMessage("Creating blank tempalte");

                StartUpScreen = PromotionTemplateCustomerPage.Key;
            }
            else
            {
                try
                {
                    RebindNavigation(DataTemplateAccess.GetTemplate(promotionId, PromotionTemplateCustomerPage.Key, null));


                    UpdateSubMessage("Data: Promotion data");

                    var c = CurrentTemplate.Products;
                    PromotionTemplateProductPage.FirstLoad = true;
                    SelectedProducts = new PromotionHierarchy().convertListToListHierarchy(c);
                    PromotionTemplateProductPage.FirstLoad = false;

                    // Set start up screen
                    StartUpScreen = CurrentTemplate.WizarStartScreenName;

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

        public void GetPromo(Task<PromotionTemplate> task)
        {
            CurrentTemplate = task.Result;
        }

        private void InitCustomers(string promotionId)
        {
            PromotionTemplateCustomerPage.FirstLoad = true;

            if ((CustomerList == null || CustomerList.Count == 0) ||
                (GetCurrentPage(PromotionTemplateCustomerPage.Key).State != ToggleState.On)
                || (GetCurrentPage(PromotionTemplateCustomerPage.Key).ForceReload))
            {
                Name = CurrentTemplate.Name;
                LoadedName = Name;
                var tt = DataTemplateAccess.GetAddPromotionTemplateCustomersAsync((promotionId ?? ""))
                    .ContinueWith(t =>
                    {
                        //load all customers from DB
                        CustomerList = new ObservableCollection<Model.Customer>(Model.Customer.FromXml(t.Result).ToList());

                        CustomersTree = new TreeViewModel(new TreeViewHierarchy(t.Result));

                        var cust = _customerList.Where(r => r.IsSelected).Where(s => s.Children.Any() == false).ToList();
                        if (cust.Any())
                        {
                            CurrentTemplate.Customers = cust;
                        }

                        //load sub customers
                        LoadSubCustomers(true);


                        PromotionTemplateCustomerPage.FirstLoad = false;

                        NotifyPropertyChanged(this, vm => vm.CustomerList);



                        //VisibleSubCustomers.Where(z => z.ParentID == null).Do(y => y.PerformExpand(App.Configuration.ForcePromoProductsExpand));

                        GetCurrentWizardViewModel().HasChanges = false;
                        _customers = GetSerialisedCustomers();
                    }, App.Scheduler);

            }
            else
            {
                PromotionTemplateCustomerPage.FirstLoad = false;
                GetCurrentWizardViewModel().HasChanges = false;
            }
            // return Task.Factory.Completed();
        }


        //        private void GetCustomers(string pi)
        //        {

        //            try
        //            {

        //              RootCustomers = new ObservableCollection<TreeViewHierarchy>(
        //                      CustomerList.Where(p => p.Parent == null)
        //                          .Select(p => new TreeViewHierarchy(p))
        //                      );

        //              bool forceAll = App.Configuration.ForcePromoProductsExpand;



        //                if (pi != null)
        //                {
        //                    var x = true;
        //                    foreach (var n in RootCustomers.Where(y => y.IsSelectedBool != null && y.IsSelectedBool.Value == true))
        //                    {
        //                        if (x == true)
        //                            FilterSubCustomers();

        //                        x = false;
        //                        n.IsSelected = "1";
        //                        n.IsExpanded = true;
        //                    }
        //                }
        //            }
        //            catch (ExceedraDataException ex)
        //            {
        //                MessageBoxShow(ex.Message + Environment.NewLine + "Couldn't load Customers.", "Error",
        //                    MessageBoxButton.OK, MessageBoxImage.Warning);
        //            }

        //#if LogPerformance
        //            Model.Common.LogText("InitCustomers: " + DateTime.Now.Subtract(start).Milliseconds);
        //#endif
        //        }


        public void InitDateList()
        {
            PromotionTemplateDatePage.FirstLoad = true;
            UpdateSubMessage("Data: Dates");

            try
            {
                if ((DateList == null || DateList.Count == 0) ||
                    (GetCurrentPage(PromotionTemplateDatePage.Key).State != ToggleState.On)
                    || (GetCurrentPage(PromotionTemplateDatePage.Key).ForceReload))
                {
                    List<PromotionDate> dates = DataTemplateAccess.GetPromotionDates(CurrentTemplate.Id, false).ToList();

                    DateList = new ObservableCollection<TemplateDateViewModel>(
                       dates.Select(d => new TemplateDateViewModel(d)));

                    CurrentTemplate.Dates = dates.ToList();
                }


                // Initialize date values with Today, when the promotion is new
                if (CurrentTemplate.Status == (int)Simple.Common.EditMode.IsAdded)
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
                // PromotionTemplateDatePage.FirstLoad = false;
            }
#if LogPerformance
                    Model.Common.LogText("InitDateList: " + DateTime.Now.Subtract(start).Milliseconds);
#endif
        }
        public void InitAttributes()
        {

            //if (AttributeList != null) return;
            UpdateSubMessage("Data: Attributes");
            //PromotionTemplateAttributePage.FirstLoad = true;

            try
            {

                LoadAttributesRVM();

            }
            catch (ExceedraDataException ex)
            {
                MessageBoxShow(ex.Message + Environment.NewLine + "Couldn't load Attributes.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally
            {
                PromotionTemplateAttributePage.FirstLoad = false;
            }

#if LogPerformance
                    Model.Common.LogText("InitAttributes: " + DateTime.Now.Subtract(start).Milliseconds);
#endif
        }
        public void InitProducts()
        {

            UpdateSubMessage("Data: Products");
            PromotionTemplateProductPage.FirstLoad = true;
            try
            {

                if ((ProductList == null || ProductList.Count == 0 || RootProducts == null || RootProducts.Count == 0)
                    || (GetCurrentPage(PromotionTemplateProductPage.Key).State != ToggleState.On)
                     || (GetCurrentPage(PromotionTemplateProductPage.Key).ForceReload))
                {
                    ProductList =
                        new ObservableCollection<PromotionProduct>(
                            DataTemplateAccess.GetAddPromotionProducts(CurrentTemplate.Id, false));


                    RootProducts = new ObservableCollection<PromotionHierarchy>(
                        ProductList.Where(p => p.Parent == null)
                            .Select(p => new PromotionHierarchy(p))
                        );

                    bool forceAll = App.Configuration.ForcePromoProductsExpand;


                    SelectedProducts = new List<PromotionHierarchy>();
                    recProducts(RootProducts[0]);

                    NotifyPropertyChanged(this, vm => vm.SelectedProducts);

                    RootProducts.Where(x => x.ParentID == null).Do(y => y.PerformExpand(App.Configuration.ForcePromoProductsExpand));
                    ProductsPage.HasChanges = false;

                    LoadProductPrices();
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
                PromotionTemplateProductPage.FirstLoad = false;
            }



#if LogPerformance
            Model.Common.LogText("InitProducts: " + DateTime.Now.Subtract(start).Milliseconds);
#endif
        }


        private void recProducts(PromotionHierarchy rp)
        {

            foreach (var p in rp.Children)
            {
                if (p.IsSelectedBool == true)
                {
                    _selectedProducts.Add(p);
                }

                if (p.Children != null)
                {
                    recProducts(p);
                }
            }
        }

        public void LoadProductPrices()
        {
            // ProductPrices List is in synch, no need to update from database.
            //    if (IsProductPricesInSynch()) return;

            if (SelectedProducts == null || !SelectedProducts.Any())
            {
                CurrentTemplate.ProductPrices = null;
                return;
            }

            try
            {
                IEnumerable<PromotionProductPrice> dataList =
                    DataTemplateAccess.GetPromotionProductPrices(CurrentTemplate.Id,
                        SelectedProducts.Where(p => p.IsSelectedBool == true).Select(p => p.ID).ToList()
                        );
                // Get product prices
                ProductPricesList = new ObservableCollection<PromotionProductPrice>(dataList);

                CurrentTemplate.ProductPrices = ProductPricesList != null ? ProductPricesList.ToList() : null;
                NotifyPropertyChanged(this, vm => vm.ProductPricesList);
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

            List<string> selectedProductsIds = new List<string>();
            if (SelectedProducts != null)
            {
                selectedProductsIds =
                    SelectedProducts.Where(p => p.IsSelectedBool == true).Select(p => p.ID).ToList();
            }
            //List<string> selectedProductsIds = new List<string>();
            //foreach (var product in CurrentTemplate.Products)
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
            PromotionTemplateFinancialPage.FirstLoad = true;


            try
            {

                //Grid 1
                if ((G1PromoFinancialMeasures == null || G1PromoFinancialMeasures.Records.Count() == 0)
                    || (GetCurrentPage(PromotionTemplateFinancialPage.Key).State != ToggleState.On)
                    || (GetCurrentPage(PromotionTemplateFinancialPage.Key).ForceReload))
                {
                    G1PromoFinancialMeasures = new RowViewModel(DataTemplateAccess.GetPromotionFinancialPromoMeasures(CurrentTemplate.Id));
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
                    || (GetCurrentPage(PromotionTemplateFinancialPage.Key).State != ToggleState.On)
                    || (GetCurrentPage(PromotionTemplateFinancialPage.Key).ForceReload))
                {
                    var c = DataTemplateAccess.GetPromotionFinancialParentProductMeasures(CurrentTemplate.Id);
                    if (c != null)
                    {
                        G2ParentProductFinancialMeasures = new RecordViewModel(c);
                        G2ParentProductFinancialMeasures.PropertyChanged +=
                            G2ParentProductFinancialMeasures_PropertyChanged;
                    }
                }

                //Grid 3
                if ((G3FinancialScreenPlanningSkuMeasure == null || G3FinancialScreenPlanningSkuMeasure.HasRecords == false)
                    || (GetCurrentPage(PromotionTemplateFinancialPage.Key).State != ToggleState.On)
                    || (GetCurrentPage(PromotionTemplateFinancialPage.Key).ForceReload))
                {
                    var d = DataTemplateAccess.GetFinancialScreenPlanningSkuMeasure(CurrentTemplate.Id);
                    if (d != null)
                    {
                        G3FinancialScreenPlanningSkuMeasure = new RecordViewModel(d);
                        G3FinancialScreenPlanningSkuMeasure.PropertyChanged +=
                            G3FinancialScreenPlanningSkuMeasure_PropertyChanged;
                    }
                }

                if (G3FinancialScreenPlanningSkuMeasure != null)
                    GetG2ValuesFromG3();


                //CurrentTemplate.FinancialVariables = PromotionFinancials.ToList();
            }
            catch (ExceedraDataException ex)
            {
                MessageBoxShow(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally
            {
                PromotionTemplateFinancialPage.FirstLoad = false;
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
            if (e.PropertyName == "Records")
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
        //            PromotionTemplateFinancialPage.FirstLoad = true;
        //            if (!SelectedProducts.Any())
        //                return;

        //            try
        //            {

        //                //if ((ProductFinancialsPricesList == null || ProductFinancialsPricesList.Count == 0) 
        //                //    || (GetCurrentPage(PromotionTemplateFinancialPage.Key).State != ToggleState.On))
        //                //{
        //                //    // Get product financial prices
        //                //    ProductFinancialsPricesList = new ObservableCollection<PromotionProductPrice>(
        //                //        DataTemplateAccess.GetPromotionFinancialProductPrices(CurrentTemplate.Id,
        //                //            SelectedProducts.Select(p => p.ID), false));
        //                //}



        //                //CurrentTemplate.FinancialProductVariables = ProductFinancialsPricesList.ToList();
        //            }
        //            catch (ExceedraDataException ex)
        //            {
        //                MessageBoxShow(ex.Message, "Error",
        //                    MessageBoxButton.OK, MessageBoxImage.Warning);
        //            }
        //            finally
        //            {
        //                PromotionTemplateFinancialPage.FirstLoad = false;
        //            }

        //#if LogPerformance
        //            Model.Common.LogText("InitProductFinancialPrices: " + DateTime.Now.Subtract(start).Milliseconds);
        //#end 






        public void InitPromotionStatuses()
        {
            UpdateSubMessage("Data: Statuses");

            try
            {
                if ((PromotionStatuses == null || !PromotionStatuses.Any()) || GetCurrentPage(PromotionTemplateReviewPage.Key).ForceReload)
                {
                    PromotionStatuses = DataTemplateAccess.GetPromotionWorkflowStatuses(CurrentTemplate);
                }


                foreach (PromotionStatus s in PromotionStatuses)
                    if (s.ID == CurrentTemplate.Status.ToString(CultureInfo.InvariantCulture))
                        s.IsSelected = true;

                SelectedStatus =
                    PromotionStatuses.SingleOrDefault(
                        s => s.ID == CurrentTemplate.Status.ToString(CultureInfo.InvariantCulture));
            }
            catch (ExceedraDataException ex)
            {
                MessageBoxShow(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally
            {

            }

#if LogPerformance
            Model.Common.LogText("InitPromotionStatuses: " + DateTime.Now.Subtract(start).Milliseconds);
#endif
        }


        public void InitCommentList()
        {
            NewComment = "";

            UpdateSubMessage("Data: Comments");

            if (CurrentTemplate.Id == null) return;

            try
            {
                CommentList = DataTemplateAccess.GetPromotionComments(CurrentTemplate.Id);
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
                return CurrentTemplate == null ? string.Empty : CurrentTemplate.CodeAndName;
            }
        }
        #endregion

        #region EventsConsumer

        /// <summary>
        ///     Gets or sets the SearchableTreeViewNodeEventsConsumer of this PromotionWizardViewModel.
        /// </summary>
        public static ISearchableTreeViewNodeEventsConsumer EventsConsumer { get; set; }

        #endregion

        #region CurrentTemplate

        private PromotionTemplate _currentTemplate;

        public PromotionTemplate CurrentTemplate
        {
            get { return _currentTemplate; }
            set
            {
                _currentTemplate = value;
                NotifyPropertyChanged(this, vm => vm.CurrentTemplate);
            }
        }

        public bool IsReadOnlyPromo
        {
            get { return !CurrentTemplate.IsEditable; }
        }

        #endregion

        #region Customer

        private ObservableCollection<Model.Customer> _customerList;


        private List<TreeViewHierarchy> _customerListToSend;
        public List<TreeViewHierarchy> CustomerListToSend
        {
            get { return _customerListToSend; }
            set
            {
                _customerListToSend = value;
                NotifyPropertyChanged(this, vm => vm.CustomerListToSend);
            }
        }

        public ObservableCollection<Model.Customer> CustomerList
        {
            get { return _customerList; }
            set
            {
                if (_customerList != value)
                {
                    _customerList = value;
                    NotifyPropertyChanged(this, vm => vm.CustomerList);



                }
            }
        }

        private TreeViewModel _customersTree;
        public TreeViewModel CustomersTree
        {
            get { return _customersTree; }
            set
            {
                _customersTree = value;
                NotifyPropertyChanged(this, vm => vm.CustomersTree);
            }
        }

        private List<string> SelectedCustomers { get
        {
            return CustomersTree != null ? CustomersTree.GetSelectedIdxs() : new List<string>();
        } }
        //public List<TreeViewHierarchy> SelectedCustomers
        //{
        //    get { return (_selectedCustomers != null ? _selectedCustomers.Distinct(r => r.ID).ToList() : new List<TreeViewHierarchy>()); }
        //    set
        //    {
        //        if (!PromotionTemplateCustomerPage.FirstLoad)
        //        {
        //            SetPageChangedStatus(PromotionTemplateCustomerPage.Key,
        //                _selectedCustomers != null && (!value.Equals(_selectedCustomers)));
        //        }

        //        _selectedCustomers = value;
        //        if (value != null)
        //        {

        //            var sel = _selectedCustomers.Select(r => r.ID).ToList();
        //            CurrentTemplate.Customers = CustomerList.Where(tr => sel.Contains(tr.ID)).Select(t => t).ToList();

        //            LoadSubCustomers();
        //            ResetLaterPages(CustomerPage);
        //        }
        //        else
        //        {
        //            CurrentTemplate.Customers = new List<Model.Customer>();
        //        }




        //    }
        //}



        //public List<TreeViewHierarchy> SelectedSubCustomers
        //{
        //    get { return (_selectedSubCustomers != null ? _selectedSubCustomers.Distinct(r => r.ID).ToList() : new List<TreeViewHierarchy>()); }
        //    // ReSharper disable ValueParameterNotUsed
        //    set
        //    {
        //        if (IsSubCustomerActive)
        //        {
        //            if (!PromotionTemplateCustomerPage.FirstLoad)
        //            {
        //                SetPageChangedStatus(PromotionTemplateCustomerPage.Key, (!value.Equals(_selectedSubCustomers)));                         
        //            }

        //            _selectedSubCustomers = value;
        //            if (value != null)
        //            {
        //                ResetLaterPages(CustomerPage);
        //                var sel = _selectedSubCustomers.Select(r => r.ID).ToList();
        //                CurrentTemplate.SubCustomers =
        //                    TelerikProductList.Where(tr => sel.Contains(tr.ID)).Select(t => t).ToList();
        //            }
        //            else
        //            {
        //                CurrentTemplate.SubCustomers = new List<Model.Customer>();
        //            }

        //        }

        //    }
        //}

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


        /// <summary>
        /// Load all subcustomers to memory
        /// </summary>
        private void LoadSubCustomers(bool force = false)
        {

            var cache = App.AppCache.GetItem("Template_SubCustomersXML");
            bool loadingFromDB = false;

            if (cache == null
                || (GetCurrentPage(PromotionTemplateCustomerPage.Key).ForceReload)
                || force)
            {
                var res = _treeAccess.GetSubCustomers(CurrentTemplate.Id, User.CurrentUser.ID);
                    
                AllNode = new TreeViewHierarchy(res);

                App.AppCache.Upsert("Template_SubCustomersXML", res);
                loadingFromDB = true;
            }
            else
                AllNode = new TreeViewHierarchy((XElement)cache.obj);            

            SetVisibleSubCustomers(loadingFromDB);

        }

        public void SetVisibleSubCustomers(bool loadingFromDB)
        {
            if (VisibleSubCustomers == null || VisibleSubCustomers.Children == null || !VisibleSubCustomers.Children.Any())
            {
                VisibleSubCustomers = AllNode;
                if (!loadingFromDB)
                    DeselectSelfAndChildren(VisibleSubCustomers);
            }
            if (SelectedCustomers.Any())
            {
                if (AllNode != null) AllNode.Children = new MTObservableCollection<TreeViewHierarchy>(AllNode.Children.Where(c => SelectedCustomers.Contains(c.Idx)).ToList());

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
                        {
                            DeselectSelfAndChildren(child);
                            VisibleSubCustomers.Children.Add(child);
                        }
                    }
                    
                    CheckAllStates(VisibleSubCustomers);
                }
                else
                {
                    VisibleSubCustomers = AllNode;
                }

                if (VisibleSubCustomers.Children.Any())
                {
                    VisibleSubCustomers.IsExpanded = true;
                    if (SubCustomersTreeInput == null)
                        SubCustomersTreeInput = new TreeViewModel
                        {
                            Listings = VisibleSubCustomers
                        };
                    else
                        SubCustomersTreeInput.Listings = VisibleSubCustomers;
                }
                else
                    SubCustomersTreeInput = new TreeViewModel();

            }
            else
            {
                VisibleSubCustomers = new TreeViewHierarchy();
                SubCustomersTreeInput = new TreeViewModel();
                DeselectSelfAndChildren(AllNode);
            }
        }

        private void CheckAllStates(TreeViewHierarchy ti)
        {
            if (ti.Children != null && ti.Children.Any())
            {
                foreach (var c in ti.Children)
                    CheckAllStates(c);

                if ((ti.Children.All(c => c.IsSelectedBool == true)))
                    ti.IsSelected = "1";
                else if (ti.Children.Any(c => c.IsSelectedBool == null) || (ti.Children.Any(c => c.IsSelectedBool == false) && ti.Children.Any(c => c.IsSelectedBool == true)))
                    ti.IsSelected = "2";
                else
                    ti.IsSelected = "0";

                ti.IsExpanded = false;
            }
        }

        private void DeselectSelfAndChildren(TreeViewHierarchy tvh)
        {
            tvh.IsSelected = "0";
            foreach (var child in tvh.Children)
            {
                DeselectSelfAndChildren(child);
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
            if(ti.Children.Any())
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
                SubCustomersTreeInput.IsReadOnly = !IsEditable;
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

        #endregion

        #region Dates

        private ObservableCollection<TemplateDateViewModel> _dateList;

        public ObservableCollection<TemplateDateViewModel> DateList
        {
            get { return _dateList; }
            set
            {
                if (!PromotionTemplateDatePage.FirstLoad)
                {
                    SetPageChangedStatus(PromotionTemplateDatePage.Key, (!value.Equals(_dateList)));
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
            NotifyPropertyChanged(this, vm => vm.AreDatesValid);
        }

        #endregion

        # region Attributes

        private static string _customers { get; set; }
        public static string _attributes { get; set; }
        public static string _dates { get; set; }
        public static string _financials { get; set; }
        public static string _customer { get; set; }
        public string AttributeComment
        {
            get { return CurrentTemplate.AttributesComment; }
            set
            {
                if (value != CurrentTemplate.AttributesComment)
                {
                    CurrentTemplate.AttributesComment = value;
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

            if (SelectedProducts != null && SelectedProducts.Any())
            {
                _enableModalContentButton = true;
            }
            else
            {

                _enableModalContentButton = false;
            }
            return _enableModalContentButton;
        }

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
                        new XElement("Item_Idx", r.Item_Idx),
                        new XElement("Item_Type", r.Item_Type),
                        new XElement("Item_RowSortOrder", r.Item_RowSortOrder),
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

            var xml = new ObservableCollection<XElement>(xdoc.Element("Results").Elements("RootItem"));
            string concatedRecords = xml.Aggregate(string.Empty, (current, recordVm) => current + recordVm.ToString());

            //var xml = XElement.Parse(xdoc.ToString());
            XElement returnedString = DataTemplateAccess.ModalDynaicGridSave(CurrentTemplate.Id, concatedRecords);
            string messageString;
            if (returnedString.ToString().Contains("Error") == true)
            {
                messageString = returnedString.GetValue<string>("Error");
                CustomMessageBox.Show(messageString, "Save Unsuccessful", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                messageString = returnedString.GetValue<string>("Message");
                CustomMessageBox.Show(messageString, "Save Outcome", MessageBoxButton.OK, MessageBoxImage.Information);
                getModalContent();
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
                //else
                //{
                //    return false;
                //}
            }

        }
        public ICommand SetModalVis
        {
            get { return new ViewCommand(getModalContent); }
        }
        public void getModalContent(object parmater)
        {
            ModalContentPresenterVis = !ModalContentPresenterVis;
            if (ModalContentPresenterVis == true)
            {
                var previouslySelectedConstraint = ModalConstraintTypeList != null ? ModalConstraintTypeList.FirstOrDefault(constr => constr.IsSelected) : null;
                var previouslySelectedConstraintId = previouslySelectedConstraint != null ? previouslySelectedConstraint.ID : null;

                ModalConstraintTypeList = new ObservableCollection<ConstraintType>(from product in (ObservableCollection<ConstraintType>)DataTemplateAccess.GetModalProductDataList(CurrentTemplate.Id, GetSelectedProductIds())
                                                                                   select product);

                // if none constraint is selected select first one available
                if (previouslySelectedConstraintId == null)
                {
                    var firstConstraint = ModalConstraintTypeList.FirstOrDefault();
                    SelectedSelectedConstraintType = firstConstraint;
                }
                else SelectedSelectedConstraintType = ModalConstraintTypeList.FirstOrDefault(constr => constr.ID == previouslySelectedConstraintId);
            }
            else
            {
                EditProductGridVis = false;
            }
        }
        public void getModalContent()
        {
            if (ModalContentPresenterVis == true)
            {
                var previouslySelectedConstraint = ModalConstraintTypeList != null ? ModalConstraintTypeList.FirstOrDefault(constr => constr.IsSelected) : null;
                var previouslySelectedConstraintId = previouslySelectedConstraint != null ? previouslySelectedConstraint.ID : null;

                ModalConstraintTypeList = new ObservableCollection<ConstraintType>(from product in (ObservableCollection<ConstraintType>)DataTemplateAccess.GetModalProductDataList(CurrentTemplate.Id, GetSelectedProductIds())
                                                                                   select product);
                // if none constraint is selected select first one available
                if (previouslySelectedConstraintId == null)
                {
                    var firstConstraint = ModalConstraintTypeList.FirstOrDefault();
                    SelectedSelectedConstraintType = firstConstraint;
                }
                else SelectedSelectedConstraintType = ModalConstraintTypeList.FirstOrDefault(constr => constr.ID == previouslySelectedConstraintId);
            }
            else
            {
                EditProductGridVis = false;
            }
        }
        //private ObservableCollection<string> m_offlineTestList;
        //public ObservableCollection<string> OfflineTestList
        //{
        //    get { return m_offlineTestList; }
        //    set { m_offlineTestList = value; NotifyPropertyChanged(this, vm => vm.OfflineTestList); }
        //}

        public List<string> GetSelectedProductIds()
        {
            ObservableCollection<string> thisList = new ObservableCollection<string>(from product in SelectedProducts
                                                                                     where product.Children == null
                                                                                     select product.ID);

            List<string> listToSend = new List<string>();
            foreach (var id in thisList)
            {
                listToSend.Add(id);
            }
            return listToSend;
        }

        public void LoadProductGridData()
        {
            XElement thisXelement = new XElement(DataTemplateAccess.GetDynamicConstraintsGrid(CurrentTemplate.Id, SelectedSelectedConstraintType.ID));
            EditProductDataGrid = new RecordViewModel(thisXelement);
            if (m_editProductDataGrid != null)
            {
                EditProductGridVis = true;
            }
        }

        private ObservableCollection<ConstraintType> _modalConstraintTypeList;
        public ObservableCollection<ConstraintType> ModalConstraintTypeList
        {
            get { return _modalConstraintTypeList; }
            set { _modalConstraintTypeList = value; NotifyPropertyChanged(this, vm => vm.ModalConstraintTypeList); }
        }

        private ConstraintType _selectedConstraintType;
        public ConstraintType SelectedSelectedConstraintType
        {
            get { return _selectedConstraintType; }
            set
            {
                if (value != null)
                    _selectedConstraintType = value;

                if (ModalConstraintTypeList != null)
                    foreach (var constraintType in ModalConstraintTypeList)
                        constraintType.IsSelected = false;

                _selectedConstraintType.IsSelected = true;

                LoadProductGridData();
                NotifyPropertyChanged(this, vm => vm.SelectedSelectedConstraintType);
            }
        }
        #endregion

        private ObservableCollection<PromotionProduct> _productList;
        private ObservableCollection<PromotionProductPrice> _productPricesList;

        private ObservableCollection<PromotionHierarchy> _rootProducts;



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

        public ObservableCollection<PromotionHierarchy> RootProducts
        {
            get { return _rootProducts; }
            set
            {
                if (_rootProducts != value)
                {
                    _rootProducts = value;
                    NotifyPropertyChanged(this, vm => vm.RootProducts);
                    // ProductsPage.State = ToggleState.Off;
                }
                else
                {
                    //  ProductsPage.State = ToggleState.On;
                }
            }
        }

        private List<PromotionHierarchy> _selectedProducts;

        public List<PromotionHierarchy> SelectedProducts
        {
            get { return (_selectedProducts != null ? _selectedProducts.Distinct(r => r.ID).ToList() : _selectedProducts); }
            set
            {
                if (!PromotionTemplateProductPage.FirstLoad)
                {
                    SetPageChangedStatus(PromotionTemplateProductPage.Key, !PromotionTemplateProductPage.FirstLoad); //(!value.Equals(_selectedProducts))
                }

                _selectedProducts = value;
                PromotionHierarchy[] list = _selectedProducts.Where(p => p.IsSelectedBool == true).ToArray();
                if (ProductPricesList == null || ProductPricesList.Any() == false)
                {
                    LoadProductPrices();
                    return;
                }

                foreach (PromotionProductPrice productPrice in ProductPricesList.ToArray().Where(productPrice => list.All(sp => sp.ID != productPrice.ID)))
                {
                    ProductPricesList.Remove(productPrice);
                }

                CurrentTemplate.Products = _selectedProducts != null ? ConvertToProduct(_selectedProducts) : null;

                NotifyPropertyChanged(this, vm => vm.SelectedProducts);
                LoadProductPrices();

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

        public void SetChecked(TreeViewHierarchy node, bool? IsIt)
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



        #region Financial

        private ObservableCollection<PromotionProductPrice> _productFinancialsPricesList;
        private ObservableCollection<PromotionFinancial> _promotionFinancials;

        public ObservableCollection<PromotionFinancial> PromotionFinancials
        {
            get { return _promotionFinancials; }
            set
            {
                if (!PromotionTemplateFinancialPage.FirstLoad)
                {
                    SetPageChangedStatus(PromotionTemplateFinancialPage.Key, (!value.Equals(_promotionFinancials)));
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
                if (!PromotionTemplateFinancialPage.FirstLoad)
                {
                    SetPageChangedStatus(PromotionTemplateFinancialPage.Key, (!value.Equals(_productFinancialsPricesList)));
                }
                if (_productFinancialsPricesList != value)
                {
                    _productFinancialsPricesList = value;
                    NotifyPropertyChanged(this, vm => vm.ProductFinancialsPricesList);
                }
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
                if (!PromotionTemplateReviewPage.FirstLoad)
                {
                    SetPageChangedStatus(PromotionTemplateReviewPage.Key, (!value.Equals(_selectedStatus)));
                }

                if (_selectedStatus != value)
                {
                    bool wasNull = _selectedStatus == null;
                    _selectedStatus = value;
                    PAndLReviewPage.HasChanges = true;

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
                            s => s.ID == CurrentTemplate.Status.ToString(CultureInfo.InvariantCulture));

                    if (current != null)
                        return current.Name;
                }

                const string editingExistingPromotion = "Editing existing Promotion";
                const string addingNewPromotionTemporary = "Adding New Promotion/Temporary";

                return CurrentTemplate.Status == (int)Simple.Common.EditMode.IsEdited
                           ? editingExistingPromotion
                           : addingNewPromotionTemporary;
            }
        }

        public int SelectedApprovalRoleIndex { get; set; }

        #endregion

        #region ApprovalLevels

        public IEnumerable<PromotionApprovalLevel> ApproverList { get; set; }

        #endregion

        #region Comments

        public IEnumerable<PromotionComment> CommentList { get; set; }
        public string NewComment { get; set; }

        public bool IsCommentEnabled
        {
            get { return (!string.IsNullOrEmpty(CurrentTemplate.Id)) && CurrentTemplate.IsEditable && HasID(); }
        }

        #endregion

        #endregion

        #region Commands


        # region Navigation Commands

        private ICommand _goToPromotionsCommand;
        public ICommand GoToPromotionsCommand
        {
            get { return _goToPromotionsCommand ?? (_goToPromotionsCommand = new ViewCommand(GoToPromotions)); }
            set { _goToPromotionsCommand = value; }
        }

        #endregion

        #region Step Management Commands


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



        #region LINK Navigation Methods

        protected override void OnCancel()
        {
            XElement removePromoViewer = new XElement("RemovePromotionViewer");
            removePromoViewer.Add(new XElement("User_Idx", User.CurrentUser.ID));
            removePromoViewer.Add(new XElement("Promo_Idx", CurrentTemplate.Id));
            // FireisDirtyAsync();
            WebServiceProxy.Call(StoredProcedure.Promotion.RemovePromotionViewer, removePromoViewer, DisplayErrors.No); //Procast_SP_PROMO_RemovePromotionViewer
            PageList = null;
            GoToPromotionsCommand.Execute(HasCreated);
        }


        /// <summary>
        ///     Navigates to the Promotions page
        /// </summary>
        /// <param name="parameter"></param>
        public void GoToPromotions(object parameter)
        {
            RedirectMe.ListScreen("Templates");
        }

        #endregion

        # region Step Management Methods

        private List<string> _lastSelectedCustomers = new List<string>(); 
        private bool IsValidCustomer()
        {
            if (!_lastSelectedCustomers.SequenceEqual(SelectedCustomers) && CustomersTree != null)
            {
                _lastSelectedCustomers = SelectedCustomers;
                GetSelectedCustomers(CustomersTree.Listings);
                
            }

            if (SelectedCustomers == null || !SelectedCustomers.Any())
            {
                return false;
            }

            if (string.IsNullOrEmpty(CurrentTemplate.Name))
            {
                return false;
            }

            return true;
        }


        private bool ValidateCustomerData(bool showMessages)
        {
            return true;
        }


        public void CustomerPageBeforeNavigateAway()
        {
            var selectedSubCustIdxs = SubCustomersTreeInput.GetSelectedIdxs();
            CurrentTemplate.SubCustomers = new List<Model.Customer>();
            foreach (var s in selectedSubCustIdxs)
            {
                CurrentTemplate.SubCustomers.Add(new Model.Customer { ID = s });
            }

            CurrentTemplate.Customers = CustomerList.Where(tr => SelectedCustomers.Contains(tr.ID)).Select(t => t).ToList();
            var parentNodes = CustomersTree.GetFlatTree().Where(node => node.IsParentNode).Select(node => node.Idx);
            CurrentTemplate.Customers.Do(cust => cust.IsParentNode = false);
            CurrentTemplate.Customers.Where(cust => parentNodes.Contains(cust.ID)).Do(cust => cust.IsParentNode = true);

            var v = GetSerialisedCustomers();
            
            if ((LoadedName == Name && HasCreated == false) || CurrentTemplate.Id == null)
            {
                RetrieveAndSetNewPromotionID(null);
                HasCreated = true;
            }
            else if (v != _customers)
            {
                SavePromotion(PromotionTemplateCustomerPage.Key, true, false, CustomerPage.Valid);
            }
            else
            {
                LoadedName = Name;
                CustomerPage.Valid = true;
            }

            _customers = GetSerialisedCustomers();
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
                CurrentTemplate.Dates = DateList.Select(d => d.GetModel()).ToList();
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

            if (SelectedProducts == null || !SelectedProducts.Any()) return false;

            return true;
        }

        public bool CanProductsMoveNext(object parameter)
        {
            // In Products Page
            if (SelectedProducts == null || !SelectedProducts.Any())
            {
                ProductPricesList = null;
                NotifyPropertyChanged(this, vm => vm.ProductPricesList);
                return false;
            }

            if (!IsValidProducts())
                return false;

            // SET PRODUCTS
            //CurrentTemplate.Products = SelectedProducts != null
            //                                ? SelectedProducts.ToList()
            //                                : new List<PromotionProduct>();

            // Load Backup ProductCollection
            if (!CurrentTemplate.ProductsBackup.Any())
            {
                CurrentTemplate.BackupProducts();
            }

            //LoadProductPrices();
            CurrentTemplate.ProductPrices = ProductPricesList != null
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

            if (CurrentTemplate.Attributes != null && CurrentTemplate.Attributes.Any(a => a.SelectedOption == null))
                return false;

            if (AttributesRVM == null) return false;

            var nonSelectedDropdowns =
                AttributesRVM.Records.SelectMany(record => record.Properties).Where(prop =>
                    ((prop.SelectedItems == null && prop.IsRequired) || (!prop.SelectedItems.Any()) && prop.IsRequired)
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


        private bool ValidateFinancialData()
        {
            return true;// ValidateFinancialData(true);
        }

        private bool ValidateFinancialData(bool showMessages)
        {
            return true;
        }


        public bool CanFinishSave(object parameter)
        {
            return true;// IsValidFinish();
        }

        public void FinalSave(object parameter)
        {
            SavePromotion(PromotionTemplateReviewPage.Key);

            XElement removePromoViewer = new XElement("RemovePromotionViewer");
            removePromoViewer.Add(new XElement("User_Idx", User.CurrentUser.ID));
            removePromoViewer.Add(new XElement("Promo_Idx", CurrentTemplate.Id));

            GoToPromotionsCommand.Execute(true);
        }

        # endregion

        #region PROMOTION SAVE

        /// <summary>
        ///     Gets a new ID from data access and sets the CurrentTemplate.ID , then initializes data collections of currentPromotion with
        ///     proper data
        /// </summary>
        /// <returns></returns>
        private void RetrieveAndSetNewPromotionID(DateTime? lastSaveDateTime)
        {

            // Get a new Id from data access for currentParent new promotion
            try
            {
                var result = DataTemplateAccess.SavePromotion(CurrentTemplate, lastSaveDateTime, PromotionTemplateCustomerPage.Key);
                RebindNavigation(result);

            }
            catch// (Exception ex)
            {
                //MessageBoxShow(ex.Message);
            }


        }


        public void SetStatus()
        {
            if (SelectedStatus != null)
                CurrentTemplate.Status = int.Parse(SelectedStatus.ID);
        }



        public void SetApprovals()
        {
            if (ApproverList != null)
                CurrentTemplate.Approvers = ApproverList.ToList();
        }

        private bool performAllRelevantChecks(string page, bool showMessages)
        {
            bool returnBool = false;

            if (page == PromotionTemplateCustomerPage.Key)
            {
                returnBool = ValidateCustomerData(showMessages);
            }
            if (page == PromotionTemplateDatePage.Key)
            {
                returnBool = ValidateDatesData(showMessages);
            }
            if (page == PromotionTemplateProductPage.Key)
            {
                returnBool = ValidateProductData(showMessages);
            }
            if (page == PromotionTemplateAttributePage.Key)
            {
                returnBool = ValidateAttributeData(showMessages);
            }
            if (page == PromotionTemplateFinancialPage.Key)
            {
                returnBool = ValidateFinancialData(showMessages);
            }
            if (page == "Review" || page == "Final")
            {
                returnBool = true;
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
        private TemplatePageViewModel GetCurrentPage(string page)
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
            if (CurrentTemplate.IsReadOnly && page != PromotionTemplateReviewPage.Key) //  && _customer == CurrentTemplate.Name)
            {
                if (currentWizardTab != null)
                {
                    RebindNavigation(DataTemplateAccess.GetTemplate(CurrentTemplate.Id, page,
                        currentWizardTab.LastSavedDate));
                }
                else
                {
                    RebindNavigation(DataTemplateAccess.GetTemplate(CurrentTemplate.Id, page,
                        null));
                }
                RebindPageList();

                return true;
            }

            if (currentPage != null)
            {
                if (page == PromotionTemplateCustomerPage.Key)
                {
                    var v = GetSerialisedCustomers();
                    PromotionTemplateCustomerPage.Saves = Convert.ToInt16(_customer == v);

                    if (PromotionTemplateCustomerPage.Saves == 0 || currentPage.State != ToggleState.On)
                    {
                        currentPage.HasChanges = true;
                        _customer = GetSerialisedCustomers();
                        PromotionTemplateCustomerPage.Saves += 1;
                        GetCurrentWizardViewModel().ForceReload = false;
                    }
                    else
                    {
                        DateTime? dt = null;
                        if (currentWizardTab != null)
                        {
                            dt = currentWizardTab.LastSavedDate;
                        }


                        RebindNavigation(DataTemplateAccess.GetTemplate(CurrentTemplate.Id, page, dt));
                        RebindPageList();
                        return true;
                    }

                }
                else if (page == PromotionTemplateDatePage.Key)
                {
                    var v = GetSerialisedDate();
                    PromotionTemplateDatePage.Saves = Convert.ToInt16(_dates == v);

                    if (PromotionTemplateDatePage.Saves == 0 || currentPage.State != ToggleState.On)
                    {
                        currentPage.HasChanges = true;
                        _dates = GetSerialisedDate();
                        PromotionTemplateDatePage.Saves += 1;
                        GetCurrentWizardViewModel().ForceReload = false;
                        ResetLaterPages(DatesPage);
                        SetPageChangedStatus(PromotionTemplateDatePage.Key, true);
                    }
                    else
                    {
                        DateTime? dt = null;
                        if (currentWizardTab != null)
                        {
                            dt = currentWizardTab.LastSavedDate;
                        }
                        RebindNavigation(DataTemplateAccess.GetTemplate(CurrentTemplate.Id, page, dt));
                        RebindPageList();
                        return true;
                    }
                }
                else if (page == PromotionTemplateAttributePage.Key)
                {
                    var v = _attributesRvm.Serialize();
                    if (CurrentTemplate.AttributesComment != null)
                        v = string.Format(v + CurrentTemplate.AttributesComment);

                    PromotionTemplateAttributePage.Saves = Convert.ToInt16(_attributes == v);

                    if (PromotionTemplateAttributePage.Saves == 0
                       || currentPage.State != ToggleState.On)
                    {
                        currentPage.HasChanges = true;
                        _attributes = _attributesRvm.Serialize();
                        PromotionTemplateAttributePage.Saves += 1;
                        GetCurrentWizardViewModel().ForceReload = false;
                    }
                    else
                    {
                        DateTime? dt = null;
                        if (currentWizardTab != null)
                        {
                            dt = currentWizardTab.LastSavedDate;
                        }
                        RebindNavigation(DataTemplateAccess.GetTemplate(CurrentTemplate.Id, page, dt));
                        RebindPageList();
                        return true;
                    }
                }
                else if (page == PromotionTemplateFinancialPage.Key)
                {
                    var v = GetSerialisedFinance();
                    PromotionTemplateFinancialPage.Saves = Convert.ToInt16(_financials == v);

                    if (PromotionTemplateFinancialPage.Saves == 0
                       || currentPage.State != ToggleState.On)
                    {
                        currentPage.HasChanges = true;
                        _financials = GetSerialisedFinance();
                        PromotionTemplateFinancialPage.Saves += 1;
                        GetCurrentWizardViewModel().ForceReload = false;
                    }
                    else
                    {
                        DateTime? dt = null;
                        if (currentWizardTab != null)
                        {
                            dt = currentWizardTab.LastSavedDate;
                        }
                        RebindNavigation(DataTemplateAccess.GetTemplate(CurrentTemplate.Id, page, dt));
                        RebindPageList();
                        return true;
                    }
                }
                else if (!currentPage.HasChanges)
                {
                    DateTime? dt = null;
                    if (currentWizardTab != null)
                    {
                        dt = currentWizardTab.LastSavedDate;
                    }
                    RebindNavigation(DataTemplateAccess.GetTemplate(CurrentTemplate.Id, page, dt));
                    RebindPageList();

                    return true;
                }

                //currentPage.CanAttemptNavigate = () => false;
            }


            //all other pages/states need to be saved here
            try
            {
                Application.Current.MainWindow.Cursor = Cursors.Wait;

                SetStatus();

                SetApprovals();

                var allReleventChecks = performAllRelevantChecks(page, showMessages);

                if (allReleventChecks)
                {

                    if (page == PromotionTemplateProductPage.Key)
                    {
                        //save products
                        try
                        {

                            var flat = GetFlattened(RootProducts.FirstOrDefault());
                            var prods = flat.Where(r => r.IsSelected2 || r.IsSelectedBool != false)
                                .Select(t => new { t.ID, t.IsSelected2 })
                                    .ToDictionary(t => t.ID, t => t.IsSelected2);

                            result = DataTemplateAccess.SavePromotion(CurrentTemplate, currentWizardTab.LastSavedDate, page,
                            prods);
                        }
                        catch
                        {
                        }
                        _statusChanged = false;

                    }
                    else if (page == PromotionTemplateFinancialPage.Key)
                    {
                        XElement g1 = null;
                        XElement g2 = null;
                        XElement g3 = null;

                        if (G1PromoFinancialMeasures != null && G1PromoFinancialMeasures.Records != null && G1PromoFinancialMeasures.Records.Any())
                        {
                            //TODO: move to Model.DataTemplateAccess !!!!
                            g1 = new XElement("FinancialScreenPromotionalMeasure");
                            g1.AddFirst(XElement.Parse(G1PromoFinancialMeasures.ToCoreXml().ToString()));
                        }


                        if (G2ParentProductFinancialMeasures != null && G2ParentProductFinancialMeasures.Records != null && G2ParentProductFinancialMeasures.Records.Any())
                        {
                            //Display units
                            g2 = new XElement("FinancialScreenParentSkuMeasure");
                            g2.AddFirst(XElement.Parse(G2ParentProductFinancialMeasures.ToXml().ToString()));
                        }

                        if (G3FinancialScreenPlanningSkuMeasure != null && G3FinancialScreenPlanningSkuMeasure.Records != null && G3FinancialScreenPlanningSkuMeasure.Records.Any())
                        {
                            //cannibalisation units
                            g3 = new XElement("FinancialScreenPlanningSkuMeasure");
                            g3.AddFirst(XElement.Parse(G3FinancialScreenPlanningSkuMeasure.ToXml().ToString()));
                        }

                        result = DataTemplateAccess.SavePromotion(CurrentTemplate, currentWizardTab.LastSavedDate, page, null, g1, g2, g3);



                    }
                    else if (page == PromotionTemplateAttributePage.Key)
                    {
                        XElement g3 = null;

                        if (AttributesRVM != null && AttributesRVM.Records != null)
                        {
                            //cannibalisation units
                            g3 = XElement.Parse(AttributesRVM.ToAttributeXml().ToString());
                        }

                        result = DataTemplateAccess.SavePromotion(CurrentTemplate, currentWizardTab.LastSavedDate, page, null, g3);

                    }
                    else
                    {
                        DateTime? dt = null;
                        if (currentWizardTab != null)
                        {
                            dt = currentWizardTab.LastSavedDate;
                        }

                        result = DataTemplateAccess.SavePromotion(CurrentTemplate, dt, page);
                        CurrentTemplate.IsEditable = result.IsAmendable;
                        IsReadOnly = !CurrentTemplate.IsEditable;
                        IsEditable = CurrentTemplate.IsEditable;
                    }

                    // UPDATE PRODUCT BACKUP
                    CurrentTemplate.BackupProducts();
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
            }
            //finally 
            //{
            if (page == PromotionTemplateAttributePage.Key) currentPage.CanAttemptNavigate = IsValidAttributes;
            //else currentPage.CanAttemptNavigate = () => true;
            CommandManager.InvalidateRequerySuggested();

            currentPage.HasChanges = result.ValidationStatus != ValidationStatus.Error;
            currentPage.State = (result.ValidationStatus != ValidationStatus.Error ? ToggleState.On : ToggleState.Indeterminate);
            currentPage.Valid = result.ValidationStatus != ValidationStatus.Error;

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

            return result.ValidationStatus != ValidationStatus.Error;
            //}
        }

        private string GetSerialisedCustomers()
        {
            var d = "";

            if (CurrentTemplate.Customers != null)
            {
                var selectedCustomers = CustomersTree.GetAsHierarchyDictionary();
                //Idx's
                d += selectedCustomers.Keys.ToList().Serialize();
                //IsParentNodes's
                d += selectedCustomers.Values.ToList().Serialize();
            }

            if (CurrentTemplate.SubCustomers != null)
            {
                //foreach (var subCust in CurrentTemplate.SubCustomers)
                //{
                //d += FlattenCustomers(VisibleSubCustomers).Where(tr => tr.IsSelectedBool == true).Select(tr => tr.ID).ToList().Serialize();
                d += SubCustomersTreeInput.GetSelectedIdxs().Serialize();
                //}

                //if (CurrentTemplate.SubCustomers.FirstOrDefault().Children != null)
                //{
                //    foreach (var child in CurrentTemplate.SubCustomers.FirstOrDefault().Children)
                //    {
                //        d += child.Serialize();
                //    }
                //}
            }

            if (Name != null)
            {
                d += Name.Serialize();
            }

            //if(CustomerPage.LastSavedDate != null)
            //    d += CustomerPage.LastSavedDate.Serialize();

            return d;
        }

        private string GetSerialisedDate()
        {
            var d = "";
            if (DateList != null)
            {
                d += _dateList.Serialize();
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

            CurrentTemplate.CodeAndName = result.CodeAndName;
            NotifyPropertyChanged(this, vm => vm.CodeAndName);

            CurrentTemplate.IsEditable = result.IsAmendable;
            IsReadOnly = !CurrentTemplate.IsEditable;
            IsEditable = CurrentTemplate.IsEditable;
            CurrentTemplate.Id = result.PromotionID;
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
                    WizardTabCode = PromotionTemplateCustomerPage.Key,
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
                else if (p.Name == PromotionTemplateReviewPage.Key)
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
            if (CurrentTemplate == null)
            {
                CurrentTemplate = new PromotionTemplate();
            }

            CurrentTemplate.Name = result.Name;

            CurrentTemplate.CodeAndName = result.CodeAndName;
            NotifyPropertyChanged(this, vm => vm.CodeAndName);

            _customer = CurrentTemplate.Name;
            CurrentTemplate.IsEditable = result.IsAmendable;
            IsReadOnly = !CurrentTemplate.IsEditable;
            IsEditable = CurrentTemplate.IsEditable;
            CurrentTemplate.Id = result.PromotionID;
            HasCreated = true;

            if (CurrentTemplate.IsEditable)
            {
            }

            CurrentTemplate.WizarStartScreenName = result.WizardStartScreenName;
            CurrentTemplate.Status = result.StatusID;
            CurrentWizardPages = result.WizardPages;

            if (result.ViewingUsers != null)
                Viewers = new ObservableCollection<PromotionViewingUser>(result.ViewingUsers);

            // RebindPageList();
        }


        private string FixValue(string val)
        {
            var isNum = false;
            decimal d;
            //rip out the localised %
            val = val.Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");

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
                //  _editingPromoWizardViewModel = new PromotionTeiewModel(null, CurrentTemplate.Id);
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
                string res = DataTemplateAccess.AddPromotionComment(CurrentTemplate.Id, NewComment);
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
            return (!String.IsNullOrWhiteSpace(CurrentTemplate.Id));
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

        public void GetSelected(PromotionHierarchy current, bool loading = false)
        {
            GetHeirs(current, loading);
            GetCurrentPage(PromotionTemplateProductPage.Key).HasChanges = true;
            LoadProductPrices();
        }

        public void GetSelectedCustomers(TreeViewHierarchy current, bool loading = false)
        {
            GetCurrentPage(PromotionTemplateCustomerPage.Key).HasChanges = true;

            CurrentTemplate.Customers = CustomerList.Where(tr => SelectedCustomers.Contains(tr.ID)).Select(t => t).ToList();

            LoadSubCustomers();
        }

        private List<PromotionHierarchy> _sp;

        private void GetHeirs(PromotionHierarchy current, bool loading = false)
        {
            //if (_sp != null) _sp.Clear();
            // if (_sp == null) _sp = new List<PromotionHierarchy>();

            GetHeirs(current.Children, current, loading);

            if (current.IsSelectedBool != false)
            {
                if (!_selectedProducts.Contains(current))
                    _selectedProducts.Add(current);
            }
            else
            {
                if (_selectedProducts.Contains(current))
                    _selectedProducts.Remove(current);

                current.IsSelected2 = false;
            }


        }


        /// <summary>
        /// Get Product tree data
        /// </summary>
        /// <param name="ph"></param>
        /// <param name="currentParent"></param>
        /// <param name="loading"></param>
        private void GetHeirs(ObservableCollection<PromotionHierarchy> ph, PromotionHierarchy currentParent, bool loading = false)
        {
            var t = new List<PromotionHierarchy>();
            if (ph != null)
            {
                foreach (PromotionHierarchy currentChild in ph)
                {
                    //Set all child nodes to the same as their parent
                    if (currentChild.ParentID == currentParent.ID)
                    {
                        currentChild.IsSelectedBool = currentParent.IsSelectedBool;
                        currentChild.IsSelected = (currentParent.IsSelectedBool == true ? "1" : "0");

                        if (currentChild.Children != null && loading == false)
                        {
                            foreach (var item in currentChild.Children)
                            {
                                SetChecked(item, currentParent.IsSelectedBool);
                            }
                        }
                    }

                    if (currentChild.IsSelectedBool != false)
                    {
                        _selectedProducts.Add(currentChild);
                    }
                    else
                    {
                        _selectedProducts.Remove(currentChild);
                        currentChild.IsSelected2 = false;
                    }

                    if (currentChild.Children != null)
                    {
                        GetHeirs(currentChild.Children, currentParent);
                    }
                }

            }

        }

        /// <summary>
        /// Get Customer tree data
        /// </summary>
        /// <param name="ph"></param>
        /// <param name="currentParent"></param>
        /// <param name="loading"></param>
        private void GetCustomerHeirs(ObservableCollection<TreeViewHierarchy> ph, TreeViewHierarchy currentParent, bool loading = false)
        {
            var t = new List<TreeViewHierarchy>();
            if (ph != null)
            {
                foreach (TreeViewHierarchy currentChild in ph)
                {
                    //Set all child nodes to the same as their parent
                    if (currentChild.ParentIdx == currentParent.Idx)
                    {
                        currentChild.IsSelectedBool = currentParent.IsSelectedBool;
                        currentChild.IsSelected = (currentParent.IsSelectedBool == true ? "1" : "0");

                        if (currentChild.Children != null && loading == false)
                        {
                            foreach (var item in currentChild.Children)
                            {
                                SetChecked(item, currentParent.IsSelectedBool);
                            }
                        }
                    }

                    if (currentChild.IsSelectedBool != false)
                    {
                        SelectedCustomers.Add(currentChild.Idx);
                    }
                    else
                    {
                        SelectedCustomers.Remove(currentChild.Idx);
                        currentChild.IsParentNode = false;
                    }

                    if (currentChild.Children != null)
                    {
                        GetCustomerHeirs(currentChild.Children, currentParent);
                    }
                }

            }

        }


        private RecordViewModel _dashboardRvm;
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

        private RecordViewModel _dashboardRvm2;
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

        private RecordViewModel _dashboardRvm3;
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

        private RecordViewModel _dashboardRvm4;
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


        private void UpdateSubMessage(string t)
        {

            Dispatcher.Invoke(new Action(delegate
            {
                if (App.TemplateNavigator != null)
                    App.TemplateNavigator.loadingPanel.SubMessage += ". ";// + DateTime.Now + Environment.NewLine;
            }));

        }

        public void GetIsParentNode(PromotionHierarchy c)
        {
            if (c.Children != null)
            {
                try
                {
                    var p = ProductList.FirstOrDefault(r => r.ID == c.ID);
                    p.IsParentNode = c.IsSelected2;

                    SetPageChangedStatus(PromotionTemplateProductPage.Key, true);
                }
                catch (Exception)
                {

                }

            }
        }

        //public void GetIsParentNode(TreeViewHierarchy c)
        //{
        //    if (c.Children != null)
        //    {
        //        try
        //        {
        //            var p = CustomerList.FirstOrDefault(r => r.ID == c.Idx);
        //            p.IsParentNode = c.IsSelected2;

        //            //GetCurrentPage(PromotionTemplateCustomerPage.Key).HasChanges = true;
        //            SetPageChangedStatus(PromotionTemplateProductPage.Key, true);

        //            GetSelectedCustomers(c);

        //            CurrentTemplate.Customers = CustomerList.Where(tr => SelectedCustomers.Contains(tr.ID)).Select(t => t).ToList();
        //        }
        //        catch (Exception)
        //        {

        //        }

        //    }
        //}

        public void FireG1PromoFinancialMeasuresChanges(bool resetAllowed = true, bool rowTotalAllowed = true)
        {
            //its changed!
            SetPageChangedStatus(PromotionTemplateFinancialPage.Key, true);

            ProcessMeasureUpdatingParentProducts();

        }


        private void FireG2ParentProductFinancialMeasuresChanges()
        {
            //its changed!
            SetPageChangedStatus(PromotionTemplateFinancialPage.Key, true);

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
                    var calc = ec.Calculation.Replace("=SUMIFS", "").Replace("(", "").Replace(")", "").Replace(" ", "");

                    var options = calc.Split(',');

                    //should be three options
                    if (options.Count() != 3)
                    {
                        return;
                    }

                    var childValueColumn = TableColumn(options[0]);
                    var parentIdColumn = TableColumn(options[1]);
                    var parentIDx = options[2];

                    //find the childValue column based on the childParentID
                    var parentID = dvRVM.Properties.FirstOrDefault(p => p.ColumnCode == parentIDx);


                    if (childValueColumn.Item1 == "G3" && parentID != null)
                    {
                        Decimal sum = 0;
                        //Get the Value for each item in the correct column where the parentID is correct
                        foreach (var cta in G3FinancialScreenPlanningSkuMeasure.Records)
                        {
                            //are we in the right Row based on parentID
                            var col =
                                cta.Properties.FirstOrDefault(
                                    prop => prop.ColumnCode == parentIdColumn.Item2 && prop.Value == parentID.Value);

                            if (col != null)
                            {
                                var x = cta.Properties.FirstOrDefault(p => p.ColumnCode == childValueColumn.Item2);
                                if (x != null) sum += FixDecimal(x.Value);

                            }
                        }


                        ec.Value = sum.ToString(CultureInfo.CurrentCulture);

                        NotifyPropertyChanged(this, vm => vm.G2ParentProductFinancialMeasures);


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
                                Exceedra.Controls.DynamicGrid.Models.Property col =
                                    cta.Properties.FirstOrDefault(p => p.ColumnCode == column);
                                if (col != null)
                                {
                                    // set value based on promo measures grid
                                    col.Value = ec.Value;

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
                    || (GetCurrentPage(PromotionTemplateAttributePage.Key).State != ToggleState.On)
                    || (GetCurrentPage(PromotionTemplateAttributePage.Key).ForceReload))
            {
                DataTemplateAccess.GetPromotionAttributesAsync(CurrentTemplate.Id, StoredProcedure.Template.GetAttributes) //Procast_SP_PROMO_GetPromotionAttributes
                    .ContinueWith(AttributesContinuation, App.Scheduler);

                //  AttributesContinuation(null);
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
                string comments = res.Result.GetValue<string>("AttributeComment");
                CurrentTemplate.AttributesComment = comments;
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
            if (CurrentTemplate.AttributesComment != null)
                _attributes = string.Format(_attributes + CurrentTemplate.AttributesComment);



            //  }


        }

        public void GetSelectedSubCustomers(TreeViewHierarchy current, bool loading = false)
        {
            //if (_selectedSubCustomers == null)
            //{
            //    _selectedSubCustomers = new List<string>();
            //}

            GetCurrentPage(PromotionTemplateCustomerPage.Key).HasChanges = true;
            GetSubCustomerHeirs(current.Children, current, loading);

            //if (current.IsSelectedBool != false)
            //{
            //    if (_selectedSubCustomers != null && !_selectedSubCustomers.Contains(current.ID))
            //        _selectedSubCustomers.Add(current.ID);
            //}
            //else
            //{
            //    if (_selectedSubCustomers != null && _selectedSubCustomers.Contains(current.ID))
            //    {
            //        //    _selectedSubCustomers.Remove(current.ID);

            //        var sels = new List<string>();
            //        sels.Add(current.ID);
            //        if (current.Children != null)
            //        {
            //            sels.AddRange(FlattenCustomers(current.Children).Select(y => y.ID));
            //        }

            //        if (_selectedSubCustomers != null)
            //        {
            //            foreach (var v in sels.Distinct())
            //            {
            //                _selectedSubCustomers.RemoveAll(y => y == v);
            //            }
            //        }
            //    }

            //}

            //_selectedSubCustomers = _selectedSubCustomers.Distinct().ToList();

            //  var sel = _selectedSubCustomers.Select(r => r.ID).ToList();
            //   CurrentTemplate.SubCustomers = SubCustomerCache.Where(tr => _selectedSubCustomers.Contains(tr.ID)).Select(t => t).ToList();
        }


        private void GetSubCustomerHeirs(ObservableCollection<TreeViewHierarchy> ph, TreeViewHierarchy currentParent, bool loading = false)
        {
            var t = new List<TreeViewHierarchy>();
            if (ph != null)
            {
                foreach (TreeViewHierarchy currentChild in ph)
                {
                    //Set all child nodes to the same as their parent
                    if (currentChild.ParentIdx == currentParent.Idx)
                    {
                        currentChild.IsSelectedBool = currentParent.IsSelectedBool;
                        currentChild.IsSelected = (currentParent.IsSelectedBool == true ? "1" : "0");

                        if (currentChild.Children != null && loading == false)
                        {
                            foreach (var item in currentChild.Children)
                            {
                                SetChecked(item, currentParent.IsSelectedBool);
                            }
                        }
                    }

                    //if (currentChild.IsSelectedBool != false)
                    //{
                    //    _selectedSubCustomers.Add(currentChild.ID);
                    //}
                    //else
                    //{
                    //    _selectedSubCustomers.Remove(currentChild.ID);


                    //    //var sels = new List<TreeViewHierarchy>();
                    //    //sels.Add(currentChild);
                    //    //if (currentChild.Children != null)
                    //    //{
                    //    //    sels.AddRange(FlattenCustomers(currentChild.Children));
                    //    //}

                    //    //if (_selectedSubCustomers != null)
                    //    //{
                    //    //    _selectedSubCustomers.RemoveAll(r => sels.Contains(r));
                    //    //}


                    //    //currentChild.IsSelected2 = false;
                    //}

                    if (currentChild.Children != null)
                    {
                        GetCustomerHeirs(currentChild.Children, currentParent);
                    }
                }

            }

        }

        private void LoadDashboardRVM()
        {

            UpdateSubMessage("Dashboard");

            // FireisDirtyAsync();
            //if ((DashboardRVM == null || DashboardRVM.HasRecords) ||
            //     (GetCurrentPage(PromotionWizardFinancialPage.Key).State != ToggleState.On))
            // { 
            DataTemplateAccess.GetTemplateDashboardXAsync(CurrentTemplate.Id, StoredProcedure.Template.GetPandLGridFirst) //Procast_SP_PROMO_GetPandLGrid_First
                .ContinueWith(Dash1, App.Scheduler);
            // }

            DataTemplateAccess.GetTemplateDashboardXAsync(CurrentTemplate.Id, StoredProcedure.Template.GetPandLGridSecond,
                "GetTemplateReviewGrid",
                "Template_Idx")
              .ContinueWith(Dash2, App.Scheduler);

            UpdateSubMessage("Statuses");
            InitPromotionStatuses();

        }

        private void Dash1(Task<XElement> r)
        {

            if (r.Result == null)
            {
                DashboardRVM = null;
                DashboardRVM = new RecordViewModel(false);
            }
            else
            {
                DashboardRVM = new RecordViewModel(r.Result);
            }
        }


        private void Dash2(Task<XElement> r)
        {

            if (r.Result == null)
            {
                DashboardRVM2 = null;
                DashboardRVM2 = new RecordViewModel(false);
            }
            else
            {
                DashboardRVM2 = new RecordViewModel(r.Result);
            }
        }

    }

    public class PWP
    {
        public string Title { get; set; }
        public string Key { get; set; }
        public bool FirstLoad { get; set; }
        public int Saves { get; set; }

        public PWP()
        {
        }

        public PWP(string k, string t)
        {
            Key = k;
            Title = t;
        }

    }
}
