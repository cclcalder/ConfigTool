using System.Globalization;
using System.Reactive.Linq;
using System.Reflection; 
using System.Threading;
using System.Windows.Markup;
using System.Windows.Navigation;
using Elmah.Everywhere;
using Elmah.Everywhere.Diagnostics;
using Exceedra.Common.Logging;
using Exceedra.Controls.Messages;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Model.Entity.Diagnostics;
using WPF.Setup;
using WPF.UserControls.Listings;
using WPF.ViewModels;
using WPF.Wizard;

namespace WPF
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Threading;
    using Model;
    using Model.DataAccess;
    using Model.Entity;
    using Exceedra.Common.Mvvm;
    using System.IO;
    using Model.Language;
    using System.Collections.Generic;
    using WPF.ViewModels.Generic;
    using System.Diagnostics;
    using WPF.PromoTemplates;
  
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static List<ListingsViewModel> _cachedListingsViewModel;
        public static  List<ListingsViewModel> CachedListingsViewModels
        {
            get
            {
                if (_cachedListingsViewModel == null)
                {
                    _cachedListingsViewModel = new List<ListingsViewModel>();
                }
                return _cachedListingsViewModel;
            }
            set { _cachedListingsViewModel = value; }
        }

        private static TaskScheduler _scheduler = TaskScheduler.Default;
        private static readonly TimeSpan RedisplayMessageLimit = TimeSpan.FromMilliseconds(2500);
        private static string _lastMessage;
        private static DateTime _lastTime = DateTime.Now;
        private static ClientConfiguration _configuration;
        private static string path = AppDomain.CurrentDomain.GetPath() + "Log.txt";
        public static AuthenticationContext AuthContext;
        public static MainPageViewModel MainPV { get; set; }
        public static bool ShowKeys { get; set; }

        public static bool HideHTML { get; set; }

        //public static SalesOrgDataViewModel SelectedSalesOrganisation { get; set; }

        private static LoggingConfigData _LoggingConfigData;

        public static AzureADData AzureADConfigData;

        public static SiteData SiteData;

        public static CurrentLanguageSet CurrentLang
        {
            get
            {
                if (Model.User.CurrentUser != null)
                {

                    if (Model.User.CurrentUser.CurrentLanguage != null)
                    {
                        return Model.User.CurrentUser.CurrentLanguage;
                    }
                    else
                    {
                        CurrentLanguageSet c;
                        LanguageCache.TryGetValue(CultureInfo.CurrentCulture.IetfLanguageTag, out c);
                        Model.User.CurrentUser.CurrentLanguage = c;
                    }

                }
                return null;
            }
        }

        // Dictionary<String, String> DllMapping = new Dictionary<String, String>();


        public static string _appBG;
        public static string appBG
        {
            get
            {
                return (string.IsNullOrEmpty(_appBG) ? "#AAAAAA" : _appBG);//"#cccccd"
            }
            set
            {
                _appBG = value;
            }
        }

        public static string VersionInfo
        {
            get
            {
                Assembly assem = Assembly.GetEntryAssembly();
                AssemblyName assemName = assem.GetName();
                Version ver = assemName.Version;
                return ver.Major.ToString() + "." + ver.Minor.ToString() + "." + ver.Build.ToString();  
            }
        }
         
        public static string VersionMinorInfo
        {
            get
            {
                Assembly assem = Assembly.GetEntryAssembly();
                AssemblyName assemName = assem.GetName();
                Version ver = assemName.Version;
                return ver.Major.ToString() + "." + ver.Minor.ToString() + "." + ver.Build.ToString() + "." + ver.MinorRevision;
            }
        }

        public static ApplicationCache AppCache { get; set; }

       // [SecurityPermission(SecurityAction.Demand, ControlAppDomain = true)]
        public App()
        {
            _LoggingConfigData = new LoggingConfigData();

            AzureADConfigData = new AzureADData();

            SiteData = new SiteData();

            Start.SetupElmahConfig(_LoggingConfigData);
            Start.SetupAzureDBConfig(AzureADConfigData);

            Startup += new StartupEventHandler(App_Startup);
            DispatcherUnhandledException += AppDispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += AppUnobservedException;

            AppCache = new ApplicationCache();
       
            StartupUri = new Uri("/Pages/Login.xaml", UriKind.Relative);
            //StartupUri = new Uri("/Pages/Canvas/Dummy/Pivot1.xaml", UriKind.Relative);

            Messages.Instance.All.OfType<ErrorMessage>().Subscribe(DisplayErrorMessage);
            Messages.Instance.All.OfType<InformationMessage>().Subscribe(DisplayInformationMessage);
            Messages.Instance.All.OfType<WarningMessage>().Subscribe(DisplayWarningMessage);

            Application.Current.Navigating += new NavigatingCancelEventHandler(Current_Navigating);

            try
             {//clear log file from Bin folder
                 File.WriteAllText(path, String.Empty);
             }
             catch { }

             TranslationManager.Instance.TranslationProvider = new TranslationProvider(Assembly.GetExecutingAssembly());
        }

        void Current_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Refresh)
            {
                //put your logic here 
                e.Cancel = true;
            }

            if (e.NavigationMode == NavigationMode.Back)
            {
                //put your logic here 
                e.Cancel = true;
            }
        }
        private void SetupAzureAD()
        {

            if (_LoggingConfigData.IsActive && !string.IsNullOrWhiteSpace(_LoggingConfigData.EndPoint))
            {
                var defaults = new ExceptionDefaults
                {
                    Token = "Test-Token",
                    ApplicationName = "ExceedraSP",
                    Host = string.Format("{0}-{1}-{2}", _LoggingConfigData.Name, _LoggingConfigData.ReleaseLevel, App.VersionInfo),
                    RemoteLogUri = new Uri(_LoggingConfigData.EndPoint)
                };


                var writer = new HttpExceptionWritter
                {
                    RequestUri = new Uri(_LoggingConfigData.EndPoint, UriKind.Absolute)
                };
                ExceptionHandler.Configure(writer, defaults, null);

            }
        }

        public static string GetLoggingConfigHost()
        {
            return string.Format("{0}-{1}-{2}", _LoggingConfigData.Name, _LoggingConfigData.ReleaseLevel,
                App.VersionInfo);
        }


        protected override void OnStartup(StartupEventArgs e)
        {
            ExceptionHandler.Attach(AppDomain.CurrentDomain);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ExceptionHandler.Detach(AppDomain.CurrentDomain);
        }

        void App_Startup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            LogError(exception);
            if (e.IsTerminating)
                CustomMessageBox.Show("Goodbye world!");
        }
 
        void AppUnobservedException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            LogError(e.Exception);
            e.SetObserved(); 
        } 

        public static TaskScheduler Scheduler
        {
            get { return _scheduler; }
            protected internal set { _scheduler = value; }
        }

        public static MainPage Navigator { get; set; }
        public static  WizardFrame WizardNavigator { get; set; }
        public static TemplateFrame TemplateNavigator { get; set; }
        

        public static ClientConfiguration Configuration
        {
            get {
                return _configuration ?? (_configuration = new ClientConfigurationAccess().GetClientConfiguration());
            }
            set { _configuration = value; }
        }

        private static void DisplayErrorMessage(ErrorMessage message)
        {
            if (DateTime.Now - _lastTime > RedisplayMessageLimit || message.Text != _lastMessage)
            {
                Thread th = new Thread(new ThreadStart(delegate
                {
                    CustomMessageBox.Show(message.Text, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }));
#pragma warning disable 618
                th.ApartmentState = ApartmentState.STA;
#pragma warning restore 618
                th.Start();
            }

            _lastTime = DateTime.Now;
            _lastMessage = message.Text;
        }

        private static void DisplayInformationMessage(InformationMessage message)
        {
            if (DateTime.Now - _lastTime > RedisplayMessageLimit || message.Text != _lastMessage)
            {
                Thread th = new Thread(new ThreadStart(delegate
                {
                    CustomMessageBox.Show(message.Text, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }));
#pragma warning disable 618
                th.ApartmentState = ApartmentState.STA;
#pragma warning restore 618
                th.Start();
            }

            _lastTime = DateTime.Now;
            _lastMessage = message.Text;
        }

        private static void DisplayWarningMessage(WarningMessage message)
        {
            if (DateTime.Now - _lastTime > RedisplayMessageLimit || message.Text != _lastMessage)
            {
                Thread th = new Thread(new ThreadStart(delegate
                {
                    CustomMessageBox.Show(message.Text, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }));
#pragma warning disable 618
                th.ApartmentState = ApartmentState.STA;
#pragma warning restore 618
                th.Start();                
            }

            _lastTime = DateTime.Now;
            _lastMessage = message.Text;
        }

        public static void LogError(string msg)
        {
            try
            {
                if (User.CurrentUser != null && User.CurrentUser.Logging)
                    StorageBase.LogMessageToFile("App error", "AppDispatcherUnhandledException", msg, (User.CurrentUser != null ? User.CurrentUser.ID : ""));

                File.AppendAllText(path, msg + Environment.NewLine );
                
            }
            catch { }
        }


        public static void LogError(Exception msg)
        {
            try
            {
                //ErrorSignal.FromCurrentContext().Raise(msg);

                var res = "";
                //if (Configuration.IsVerBoseLogging)
                //{

                res = msg.ToFullMessage() + Environment.NewLine + Environment.NewLine
                    + (msg.InnerException == null ? "" : msg.InnerException.ToFullMessage());

                if (User.CurrentUser != null && User.CurrentUser.Logging)
                {
                    StorageBase.LogMessageToFile("App error", "(v) AppDispatcherUnhandledException", res,
                        (User.CurrentUser != null ? User.CurrentUser.ID : ""));                   
                }

                File.AppendAllText(path, res + Environment.NewLine);
                if (_LoggingConfigData.IsActive && !string.IsNullOrWhiteSpace(_LoggingConfigData.EndPoint))
                {
                    ExceptionHandler.Report(msg);
                }

                //}
                //else
                //{
                //    LogError(msg.ToFullMessage());
                //}

            }
            catch { }
        }
		
		
        private static void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
              e.Handled = true;
              //MessageBox.Show(e.Exception.Message);             
              LogError(e.Exception);
        }

        public static bool LoadConfiguration()
        {
            var res = new ClientConfigurationAccess().GetClientConfiguration(true);

            _configuration = res;

                //.ContinueWith(t => { _configuration = t.IsFaulted ? ClientConfiguration.Everything : t.Result; },
                //              App.Scheduler);

          return  res != null;
        }
 
        public static bool AutomaticLoginSuppressed { get; set; }

        public static void SetDefaultCulture(CultureInfo culture)
        {
            if (CultureInfoHelper.RegionCultureInfo == null)
                CultureInfoHelper.RegionCultureInfo = Thread.CurrentThread.CurrentCulture;

            // http://stackoverflow.com/questions/468791/is-there-a-way-of-setting-culture-for-a-whole-application-all-current-threads-a 
            Thread.CurrentThread.CurrentCulture.GetType()
                .GetProperty("DefaultThreadCurrentCulture")
                .SetValue(Thread.CurrentThread.CurrentCulture, culture, null);
            Thread.CurrentThread.CurrentCulture.GetType()
                .GetProperty("DefaultThreadCurrentUICulture")
                .SetValue(Thread.CurrentThread.CurrentCulture, culture, null);

            Type type = typeof(CultureInfo);
            try
            {

                FrameworkElement.LanguageProperty.OverrideMetadata(typeof (FrameworkElement),
                    new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(culture.IetfLanguageTag)));
                type.InvokeMember("s_userDefaultCulture",
                    BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Static,
                    null,
                    culture,
                    new object[] {culture});
                type.InvokeMember("s_userDefaultUICulture",
                    BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Static,
                    null,
                    culture,
                    new object[] {culture});

            }
            catch
            {
                try
                {
                    type.InvokeMember("m_userDefaultCulture",
                        BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Static,
                        null,
                        culture,
                        new object[] {culture});
                    type.InvokeMember("m_userDefaultUICulture",
                        BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Static,
                        null,
                        culture,
                        new object[] {culture});
                }
                catch
                {
                }
            }
        }

        internal static void LoadLanguage(string p)
        {

            if (LanguageCache == null)
            {
                LanguageCache = new Dictionary<string, CurrentLanguageSet>();
            }

            if (LanguageCache.ContainsKey(p))
            {
                 //do nothing it is already in there                
            }
            else
            {
                var cl = new CurrentLanguageSet(p);
                if (cl == null)
                {
                    cl = new CurrentLanguageSet("en-GB");
                }
                LanguageCache.Add(p, cl);
            }
            CurrentLanguageSet c;
            LanguageCache.TryGetValue(p, out c);

            if (Model.User.CurrentUser!=null)
                Model.User.CurrentUser.CurrentLanguage = c; 
        }

        public static Dictionary<string,  CurrentLanguageSet> LanguageCache { get; set; }
        public static string LoginType { get; set; }

        public static void OpenScanLocation(string scanLocation)
        {
            if (scanLocation == null)
            {
                MessageBox.Show(string.Format("File can not be found\n {0}", " NULL "), "File Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (File.Exists(scanLocation) || scanLocation.Contains("//"))
            {
                Process.Start(scanLocation);
            }
            else
            {
                MessageBox.Show(string.Format("File can not be found\n {0}", scanLocation), "File Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void MessageBoxShow(string p1, string p2, MessageBoxButton messageBoxButton, MessageBoxImage messageBoxImage)
        {
            throw new NotImplementedException();
        }


      
    }
}