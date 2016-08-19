using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Model.DataAccess;
using Model.Entity;
using Exceedra.Common.Mvvm;
using Exceedra.Common.Utilities;
using ViewHelper;
using WPF.Pages;
using WPF.ViewModels.Cache;
using Exceedra.Common;

namespace WPF.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private readonly ActionCommand _goToSettings = new ActionCommand(GoToSettingsImpl);
        private readonly ActionCommand _goToInsights = new ActionCommand(GoToInsightsImpl);
        private readonly ActionCommand _goToInsightsV2 = new ActionCommand(GoToInsightsV2Impl);
        private readonly ActionCommand _goToAnalytics = new ActionCommand(GoToAnalyticsImpl);
        private readonly ActionCommand _goToAnalyticsV2 = new ActionCommand(GoToAnalyticsV2Impl);
        private readonly ActionCommand _goToPlanning = new ActionCommand(GoToPlanningImpl);
        private readonly ActionCommand _goToPlanningScheduleV2 = new ActionCommand(GoToPlanningScheduleImplV2);
        private readonly ActionCommand _goToPlanningSchedule = new ActionCommand(GoToPlanningScheduleImpl);
        private readonly ActionCommand _goToPricing = new ActionCommand(GoToPricingImpl);
        private readonly ActionCommand _goToPromotions = new ActionCommand(GoToPromotionsImpl);
        private readonly ActionCommand _goToScenarios = new ActionCommand(GoToScenariosListImpl);
        private readonly ActionCommand _goToFundsV2 = new ActionCommand(GoToFundsV2ListImpl);
        private readonly ActionCommand _goToConditions = new ActionCommand(GoToConditionsImpl);
        private readonly ActionCommand _goToNPD = new ActionCommand(GoToNPDImpl);
        private readonly ActionCommand _goToClaims = new ActionCommand(GoToClaimsImpl);
        private readonly ActionCommand _goToAdmin = new ActionCommand(GoToAdminImpl);
        private readonly ActionCommand _showMoreTabs = new ActionCommand(ShowMoreTabsImpl);

        private ClientConfiguration _configuration = ClientConfiguration.Empty;
        private readonly ICommand _buttonClicked;
        private readonly ICommand _goToRob;
        private bool _isEnabled;

        public static bool _diagnostics = false;
        public static bool _diagnosticsQueueVisble = false;
        public bool Diagnostics { get { return _diagnostics; } set { _diagnostics = value; PropertyChanged.Raise(this, "Diagnostics"); } }
        public bool DiagnosticsQueueVisble { get { return _diagnosticsQueueVisble; } set { _diagnosticsQueueVisble = value; PropertyChanged.Raise(this, "DiagnosticsQueueVisble"); } }

        private static bool _buildQueue = false;
        public bool BuildQueue { get { return _buildQueue; } set { _buildQueue = value; PropertyChanged.Raise(this, "BuildQueue"); } }

        private static bool _asyncTaks = false;
        public bool AsyncTasks { get { return _asyncTaks; } set { _asyncTaks = value; PropertyChanged.Raise(this, "AsyncTasks"); } }

        private void AppLanguageChanged(object sender, EventArgs e)
        {
            ReloadScreenLabels();
        }

        private void ReloadScreenLabels()
        {
            var allScreens = _screens.Where(s => s.Children.Any()).Concat(_screens.Flatten(s => s.Children));

            foreach (var screen in allScreens)
            {
                //string labelKey = screen.LabelKey.ToUpper().StartsWith("MENU_") ? screen.LabelKey : "MENU_" + screen.LabelKey;

                var screenLabel = App.CurrentLang.AppLabels.FirstOrDefault(x => x.Code.ToLower() == screen.LabelKey.ToLower());

                if (screenLabel == null)
                {
                    if (string.IsNullOrWhiteSpace(screen.Label))
                        screen.Label = screen.Key;
                }
                else
                    screen.Label = screenLabel.Name;
            }
        }

        public void GoToRob(string appTypeId)
        {
            var screen = App.Configuration.GetScreens().FirstOrDefault(rs => rs.RobAppType == appTypeId);
            // MainPage.CurrentScreen = screen;

            var page = new EventsPage(appTypeId);
            MessageBus.Instance.Publish(new NavigateMessage(page));
        }

        public MainPageViewModel()
        {
            _goToRob = new ActionCommand<string>(GoToRob);
            _buttonClicked = new ActionCommand<string>(ButtonClickedImpl);
            //Configuration = new DesignTimeClientConfigurationAccess().GetClientConfiguration();

            _isEnabled = true;
            Instance = this;

        }

        public MainPageViewModel(IClientConfigurationAccess clientConfigurationAccess)
        {
            _goToRob = new ActionCommand<string>(GoToRob);
            _buttonClicked = new ActionCommand<string>(ButtonClickedImpl);

            if (App.Configuration == null)
            {
                if (clientConfigurationAccess != null)
                {
                    clientConfigurationAccess.GetClientConfigurationAsync()
                        .ContinueWith(t =>
                        {
                            Configuration = t.Result;
                            Diagnostics = Configuration.IsDiagnosticsTab;
                            DiagnosticsQueueVisble = Configuration.IsDiagnosticsAccountPlanQueues;
                        }, App.Scheduler);
                }
            }
            else
            {
                Configuration = App.Configuration;
            }

            Diagnostics = Configuration.IsDiagnosticsTab;
            DiagnosticsQueueVisble = Configuration.IsDiagnosticsAccountPlanQueues;

            _isEnabled = true;
            Instance = this;

            //load to cache 
            Loader.LoadAll(Configuration.Screens);


        }

        private ObservableCollection<Screen> _screens;
        public ObservableCollection<Screen> Screens
        {
            get
            {
                if (_screens == null)
                {
                    _screens = new ObservableCollection<Screen>(Configuration.Screens.OrderBy(t => t.SortOrder));
                    ReloadScreenLabels();
                    TranslationManager.Instance.LanguageChanged += AppLanguageChanged;
                    PropertyChanged.Raise(this, "Screens");
                }

                return _screens;
            }
            set
            {
                _screens = value;
                PropertyChanged.Raise(this, "Screens");
            }
        }

        private ObservableCollection<Screen> _excessedScreens;
        public ObservableCollection<Screen> ExcessedScreens
        {
            get
            {
                if (_excessedScreens == null) _excessedScreens = new ObservableCollection<Screen>();
                return _excessedScreens;
            }
            set
            {
                _excessedScreens = value;
                PropertyChanged.Raise(this, "ExcessedScreens");
                PropertyChanged.Raise(this, "ExcessedScreensVisibility");
            }
        }

        public Visibility ExcessedScreensVisibility
        {
            get
            {
                if (ExcessedScreens.Any()) return Visibility.Visible;
                return Visibility.Collapsed;
            }
        }

        #region ActionCommands

        public ActionCommand GoToSettings
        {
            get { return _goToSettings; }
        }


        public ActionCommand GoToInsights
        {
            get { return _goToInsights; }
        }

        public ActionCommand GoToInsightsV2
        {
            get { return _goToInsightsV2; }
        }

        public ActionCommand GoToAnalytics
        {
            get { return _goToAnalytics; }
        }
        public ActionCommand GoToAnalyticsV2
        {
            get { return _goToAnalyticsV2; }
        }

        public ActionCommand GoToPromotions
        {
            get { return _goToPromotions; }
        }

        public ActionCommand GoToPlanning
        {
            get { return _goToPlanning; }
        }
        public ActionCommand GoToPlanningSchedule
        {
            get { return _goToPlanningSchedule; }
        }

        public ActionCommand GoToPlanningScheduleV2
        {
            get { return _goToPlanningScheduleV2; }
        }

        public ActionCommand GoToPricing
        {
            get { return _goToPricing; }
        }

        public ActionCommand GoToScenarios
        {
            get { return _goToScenarios; }
        }

        public ActionCommand GoToFundsV2
        {
            get { return _goToFundsV2; }
        }


        public ActionCommand GoToConditions
        {
            get { return _goToConditions; }
        }

        public ActionCommand GoToNPD
        {
            get { return _goToNPD; }
        }

        public ActionCommand GoToClaims
        {
            get { return _goToClaims; }
        }

        public ActionCommand GoToAdmin
        {
            get { return _goToAdmin; }
        }

        public ICommand ButtonClicked
        {
            get { return _buttonClicked; }
        }

        public ActionCommand ShowMoreTabs
        {
            get { return _showMoreTabs; }
        }

        #endregion

        public ClientConfiguration Configuration
        {
            get { return _configuration; }
            set
            {
                if (!ReferenceEquals(_configuration, value))
                {
                    _configuration = value;
                    PropertyChanged.Raise(this, "Configuration");
                }
            }
        }

        public ICommand GoToROB
        {
            get { return _goToRob; }
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                PropertyChanged.Raise(this, "IsEnabled");
            }
        }

        public static MainPageViewModel Instance
        {
            get;
            private set;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Navigation

        private static void GoToSettingsImpl()
        {
            MessageBus.Instance.Publish(new NavigateMessage(new Uri("/Pages/UserSettings/Default.xaml", UriKind.Relative)));
        }

        private static void GoToInsightsImpl()
        {
            MessageBus.Instance.Publish(new NavigateMessage(new Uri("InsightsV2/InsightsV2.xaml", UriKind.Relative)));
        }

        private static void GoToInsightsV2Impl()
        {
            MessageBus.Instance.Publish(new NavigateMessage(new Uri("/Pages/InsightsV2/InsightsV2.xaml", UriKind.Relative)));
        }

        private static void GoToAnalyticsImpl()
        {
            MessageBus.Instance.Publish(new NavigateMessage(new Uri("/Pages/Analytics.xaml", UriKind.Relative)));
        }

        private static void GoToAnalyticsV2Impl()
        {
            MessageBus.Instance.Publish(new NavigateMessage(new Uri("/Pages/AnalyticsV2.xaml", UriKind.Relative)));
        }

        private static void GoToPlanningImpl()
        {
            MessageBus.Instance.Publish(new NavigateMessage(new Uri("/Pages/Planning.xaml", UriKind.Relative)));
        }

        private static void GoToPlanningScheduleImpl()
        {
            MessageBus.Instance.Publish(new NavigateMessage(new Uri("/Pages/SchedulePage.xaml", UriKind.Relative)));
        }

        private static void GoToPlanningScheduleImplV2()
        {
            MessageBus.Instance.Publish(new NavigateMessage(new Uri("/Pages/SchedulePageV2.xaml", UriKind.Relative)));
        }

        private static void GoToPricingImpl()
        {
            MessageBus.Instance.Publish(new NavigateMessage(new Uri("/Pages/Pricing.xaml", UriKind.Relative)));
        }

        private static void GoToPromotionsImpl()
        {
            MessageBus.Instance.Publish(new NavigateMessage(new Uri("/Pages/Promotions.xaml", UriKind.Relative)));
        }

        private static void GoToScenariosListImpl()
        {
            MessageBus.Instance.Publish(new NavigateMessage(new Uri("/Pages/ScenariosList.xaml", UriKind.Relative)));
        }

        private static void GoToFundsV2ListImpl()
        {
            MessageBus.Instance.Publish(new NavigateMessage(new Uri("/Pages/Funds/FundsList.xaml", UriKind.Relative)));
        }

        private static void GoToConditionsImpl()
        {
            MessageBus.Instance.Publish(new NavigateMessage(new Uri("/Pages/Conditions.xaml", UriKind.Relative)));
        }

        private static void GoToNPDImpl()
        {
            MessageBus.Instance.Publish(new NavigateMessage(new Uri("/Pages/NPD/NPDList.xaml", UriKind.Relative)));
        }

        private static void GoToClaimsImpl()
        {
            MessageBus.Instance.Publish(new NavigateMessage(new Uri("/Pages/Claims.xaml", UriKind.Relative)));
        }

        private static void GoToAdminImpl()
        {
            MessageBus.Instance.Publish(new NavigateMessage(new Uri("/Pages/Admin/SideMenu.xaml", UriKind.Relative)));
        }

        private void ButtonClickedImpl(object obj)
        {
            string uri = obj.ToString();

            if (uri.StartsWith("RobScreen_"))
            {
                string robAppTypeId = uri.Substring(10);
                GoToRob(robAppTypeId);
            }
            else MessageBus.Instance.Publish(new NavigateMessage(new Uri(uri, UriKind.Relative)));
        }

        private static void ShowMoreTabsImpl()
        {

        }

        #endregion
    }
}