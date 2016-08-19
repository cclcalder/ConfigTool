using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using Exceedra.Common.Logging;
using Exceedra.Controls.Messages;
using Exceedra.SideMenu;
using Telerik.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Docking;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Diagrams.Core;
using WPF.Navigation;
using WPF.Pages.Demand.DPFcMgmt;
using WPF.Pages.NPD;
using WPF.Pages.RobContracts;
using WPF.Pages.RobGroups;
using WPF.Pages.Terms;
using WPF.ViewModels.Generic;
using WPF.Wizard;

namespace WPF
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Navigation;
    using Model;
    using Model.DataAccess;
    using Exceedra.Common.Mvvm;
    using Pages;
    using ViewModels;
    using global::ViewModels;
    using System.IO;
    using System.Reflection;
    using System.Xml.Linq;
    using System.Collections.ObjectModel;
    using Model.Entity;
    using System.Timers;
    using System.ComponentModel;
    using System.Windows.Threading;
    using Model.DataAccess.Generic;
    using ViewHelper;    /// <summary>
                         /// Interaction logic for Page1.xaml
                         /// </summary>
    public partial class MainPage : Page
    {
        private BackgroundWorker _backgroundWorker = new BackgroundWorker();

        //private readonly IDictionary<Uri, Button> _buttonLookup;
        private readonly Stack<Page> _navigationHistory = new Stack<Page>();
        private Page _navigationCurrent;
       // public static Screen CurrentScreen { get; set; }
        bool firstTimeLoaded = false;
        public MainPage()
        {
            SetMainWindowTitle();
            
            thisBuildQueueList = new ObservableCollection<BuildQueueClass>();

            App.Scheduler = TaskScheduler.FromCurrentSynchronizationContext();

            InitializeComponent();
            DataContext = App.MainPV = new MainPageViewModel(new ClientConfigurationAccess());
       
            MessageBus.Instance.Subscribe<NavigateMessage>(Navigate);
            
            frmMain.Navigated += frmMain_Navigated;
            frmMain.Navigating += frmMain_Navigating;

            maingrid.PreviewKeyDown += FrmMainOnPreviewKeyDown;

            txtCurrentUserDescription.DataContext = User.CurrentUser;
            Loaded += Window_Loaded;
            Unloaded += WindowUnloaded;
            App.Navigator = this;
            //_buttonLookup = CreateButtonLookup();

            // turn on top level nav
            EnableNavigation(true);
            // hide right column
            //rightCol.Width =0;

            _backgroundWorker.DoWork += _backgroundWorker_DoWork;
            _backgroundWorker.RunWorkerCompleted += _backgroundWorker_RunWorkerCompleted;

            //listOfTabGrids = new List<Grid>();
            //listOfTabGrids.Add(sysTab);
            //listOfTabGrids.Add(asyncTasksTab);
            //listOfTabGrids.Add(buildQueueTab);
            //foreach (var item in listOfTabGrids)
            //{
            //    item.Visibility = Visibility.Hidden;
            //}

            //listOfTabButtons = new List<Button>();
            //listOfTabButtons.Add(diagnosticButton);
            //listOfTabButtons.Add(account_Builds);
            //listOfTabButtons.Add(asynchronous_Tasks);

            //Docking.PaneStateChange += DockingOnPaneStateChange;

            Menu.ShowMenuIcon = App.Configuration.ShowMenuIcon;
            // pgroup.Loaded += PgroupOnLoaded;

            User.CurrentUser.PropertyChanged += (sender, args) =>
            {
                try
                {
                    if (args.PropertyName == "Accent")
                        Menu.ButtonBackground =
                            (SolidColorBrush) new BrushConverter().ConvertFromString(User.CurrentUser.Accent);
                }
                catch
                {
                    // ignored
                }
            };
        }

        private void PgroupOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                pgroup.UnpinAllPanes();
            }
            catch { }
             
        }

        private void Docking_Unpin(object sender, Telerik.Windows.Controls.Docking.StateChangeEventArgs e)
        {
            var orderredPanes = this.pgroup.UnpinnedPanes.OrderBy(x => x.Tag);

            foreach (RadPane pane in orderredPanes)
            {
                this.pgroup.RemovePane(pane);
                this.pgroup.Items.Add(pane);
            }

            // we need to manually activate a Pane after modifying the collection
            var activePane = this.pgroup.EnumeratePanes().LastOrDefault(p => p.IsPinned);
            if (activePane != null)
            {
                activePane.IsActive = true;
            }
        }
        void selectedPane_MouseUp(object sender, MouseButtonEventArgs e)

        {

            var pane = sender as RadPane;

            if (pane != null)

            {

                pane.IsPinned = true;

                pane.MouseUp -= selectedPane_MouseUp;

            }

        }


        //stop F5 from refreshing app in browser
        private void FrmMainOnPreviewKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            switch (keyEventArgs.Key)
            {
                case Key.F2:
                    keyEventArgs.Handled = true;

                    Menu.Toggle();

                    break;

                case Key.F5:
                    keyEventArgs.Handled = true;
                    break;

                default:
                    keyEventArgs.Handled = false;
                    break;
            }
        }

        private void WindowUnloaded(object sender, RoutedEventArgs e)
        {
            //Process.Start("IEXPLORE.EXE", BrowserInteropHelper.Source.ToString());
            //Environment.Exit(-1);
        }

        private void frmMain_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            //if (e.Uri != null)
            //{

            //        foreach (var tabMenu in tabsMenu.Items)
            //        {
            //            Screen tab = (Screen)tabMenu;
            //            ContentPresenter cp = tabsMenu.ItemContainerGenerator.ContainerFromItem(tabMenu) as ContentPresenter;
            //            Button b = FindVisualChild<Button>(cp);

            //            if (tab.Uri.Length > 0 && tab.Uri.Substring(1) == e.Uri.ToString())
            //            { 
            //                CurrentScreen = tab;
            //            }
                        
            //        }


            //}
        }

        public void SetMainWindowTitle()
        {
            string configValue;
            Application.Current.MainWindow.Title = App.Configuration.Configuration.TryGetValue("IETabName", out configValue) ? configValue : "ESP";
        }

        private void Navigate(NavigateMessage msg)
        {
            if (msg == NavigateMessage.Back)
            {
                if (_navigationHistory.Count == 0) throw new InvalidOperationException("Cannot navigate back.");
                _navigationCurrent = _navigationHistory.Pop();
            }
            else
            {
                if (msg.Page != null)
                {
                    if (_navigationCurrent != null)
                    {
                        _navigationHistory.Push(_navigationCurrent);
                    }
                    _navigationCurrent = msg.Page;
                }
                else
                {
                    _navigationHistory.Clear();
                    _navigationCurrent = null;
                }
            }

            if (_navigationCurrent != null)
            {            

                frmMain.Navigate(_navigationCurrent);
                return;
            }
            frmMain.Navigate(msg.Uri, msg.ViewModel);
        }

        private void frmMain_Navigated(object sender, NavigationEventArgs e)
        {
            if(e.Uri == null)
            {
                var props = e.Content.GetType().GetProperties().FirstOrDefault(p => p.Name == "AppTypeID");
                if (props != null)
                {
                    var prop = props.GetValue(e.Content, null).ToString();
                    var value = prop;
                    Menu.AssertButtonSelection("RobScreen_" + value);
                }
            }
            else
            {
                Menu.AssertButtonSelection(e.Uri != null ? ("/" + e.Uri.OriginalString) : ("RobScreen_" + e.Content.GetType().GetProperties().First(p => p.Name == "AppTypeID").GetValue(e.Content, null).ToString()));
            }

            //if (e.Uri != null)
            //{
            //    ResetButtonTemplates(e.Uri); 
            //}
            //else
            //{
            //    ResetButtonTemplates(e.Content);
            //}

            var navigated = e.ExtraData as Action;
            if (navigated != null) navigated();
            else if (e.ExtraData is ViewModelBase)
            {
                var page = e.Content as Page;
                if (page != null)
                {
                    page.DataContext = e.ExtraData;
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (firstTimeLoaded)
            {
                return;
            }
            string pageUri = null;

            if (!(string.IsNullOrWhiteSpace(App.Configuration.StartScreen)))
            {
                try
                {
                    if (App.Configuration.ROBScreens.Any(s => s.Uri == App.Configuration.StartScreen))
                    {
                        string robAppTypeId = App.Configuration.StartScreen.Substring(10);
                        ((MainPageViewModel)DataContext).GoToRob(robAppTypeId);
                        return;
                    }
                    else
                    {
                        if (App.Configuration.StartScreen == "ScenarioManagement")
                        {
                            pageUri = "/Pages/Scenarios.xaml";
                        }
                        //    else if(App.Configuration.StartScreen =="DemoDashboard")
                        //{
                        //    pageUri = "/Pages/ExecutiveDashboard/MainPage.xaml";
                        //}
                        else
                        {
                            pageUri = App.Configuration.StartScreen;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message);
                }
            }

            firstTimeLoaded = true;

            frmMain.Navigate(new Uri(pageUri ?? "/Pages/Promotions.xaml", UriKind.Relative));

        }

        public static T FindVisualChild<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        return (T)child;
                    }

                    T childItem = FindVisualChild<T>(child);
                    if (childItem != null) return childItem;
                }
            }
            return null;
        }

        //private void ResetButtonTemplates(Uri uri)
        //{
            

        //    // for new menu
        //    foreach (var tabMenu in tabsMenu.Items)
        //    {
        //        Screen tab = (Screen)tabMenu;
        //        ContentPresenter cp = tabsMenu.ItemContainerGenerator.ContainerFromItem(tabMenu) as ContentPresenter;
        //        Button b = FindVisualChild<Button>(cp);

        //        if (!string.IsNullOrEmpty(tab.Uri) && tab.Uri.Substring(1) == uri.ToString())
        //        {
        //            b.Template = (ControlTemplate)FindResource("NavigationButtonSelected");
        //            //CurrentScreen = tab;
        //        }
        //        else b.Template = (ControlTemplate)FindResource("NavigationButton");
        //    }

        //    //foreach (var button in _buttonLookup)
        //    //{
        //    //    if (button.Key.Equals(uri))
        //    //    {
        //    //        button.Value.Template = (ControlTemplate)FindResource("NavigationButtonSelected");
        //    //    }
        //    //    else
        //    //    {
        //    //        button.Value.Template = (ControlTemplate)FindResource("NavigationButton");
        //    //    }
        //    //}

        //    //foreach (var button in _robButtons)
        //    //{
        //    //    button.Template = (ControlTemplate)FindResource("NavigationButton");
        //    //}
        //}

        //private void ResetButtonTemplates(object content)
        //{
        //    var type = content.GetType(); 

        //    foreach (var tabMenu in tabsMenu.Items)
        //    { 
        //        Screen tab = (Screen)tabMenu;
        //        ContentPresenter cp = tabsMenu.ItemContainerGenerator.ContainerFromItem(tabMenu) as ContentPresenter;
        //        Button b = FindVisualChild<Button>(cp);

        //        if (tab.Uri == null) continue;

        //        if (type == typeof(EventPage))
        //        {
        //            var page = content as EventPage;
        //            if (tab.Uri.Length > 10 && tab.Uri.Substring(10) == page.AppTypeID)
        //            {
        //                b.Template = (ControlTemplate) FindResource("NavigationButtonSelected");
        //            }
        //            else
        //            {
        //                b.Template = (ControlTemplate)FindResource("NavigationButton");
        //            }
        //        }
        //        else if (type == typeof(GroupEditor))
        //        {
        //            var page = content as GroupEditor;
        //            if (tab.Uri.Length > 10 && tab.Uri.Substring(10) == page.AppTypeID)
        //            {
        //                b.Template = (ControlTemplate)FindResource("NavigationButtonSelected");             
        //            }
        //            else
        //            {
        //                b.Template = (ControlTemplate)FindResource("NavigationButton");
        //            }
        //        }
        //        else if (type == typeof(GroupCreator))
        //        {
        //            var page = content as GroupCreator;
        //            if (tab.Uri.Length > 10 && tab.Uri.Substring(10) == page.AppTypeID)
        //            {
        //                b.Template = (ControlTemplate)FindResource("NavigationButtonSelected");
        //            }
        //            else
        //            {
        //                b.Template = (ControlTemplate)FindResource("NavigationButton");
        //            }
        //        }
        //        else if (type == typeof(RobContractsEditorView))
        //        {
        //            var page = content as RobContractsEditorView;
        //            if (tab.Uri.ToLower().Contains("contract"))
        //            {
        //                b.Template = (ControlTemplate)FindResource("NavigationButtonSelected");
        //            }
        //            else
        //            {
        //                b.Template = (ControlTemplate)FindResource("NavigationButton");
        //            }
        //        }
        //        else if (type == typeof(EventsPage))
        //        {
        //            var page = content as EventsPage;
        //            if (tab.Uri.Length > 10 && tab.Uri.Substring(10) == page.AppTypeID)
        //            {
        //                b.Template = (ControlTemplate)FindResource("NavigationButtonSelected");
        //                //CurrentScreen = tab;
        //            }
        //            else
        //            {
        //                b.Template = (ControlTemplate)FindResource("NavigationButton");
        //            }
                 
        //        }
        //        else if (type == typeof(ScenarioPage))
        //        {
        //            var page = content as ScenarioPage;
        //            if (tab.Uri.ToLower().Contains("scenario"))
        //            {
        //                b.Template = (ControlTemplate)FindResource("NavigationButtonSelected");
        //            }
        //            else
        //            {
        //                b.Template = (ControlTemplate)FindResource("NavigationButton");
        //            }
        //        }
        //        else if (type == typeof(DPFcMgmtEditor))
        //        {
        //            if (tab.Uri.ToLower().Contains("dpfc"))
        //            {
        //                b.Template = (ControlTemplate)FindResource("NavigationButtonSelected");
        //            }
        //            else
        //            {
        //                b.Template = (ControlTemplate)FindResource("NavigationButton");
        //            }
        //        }
        //        else if (type == typeof(WizardFrame))
        //        {
        //           if (tab.Uri.ToLower().Contains("promotion"))
        //            {
        //                b.Template = (ControlTemplate)FindResource("NavigationButtonSelected");
        //               // CurrentScreen = tab;
        //            }
        //            else
        //            {
        //                b.Template = (ControlTemplate)FindResource("NavigationButton");
        //            }

        //        }
        //        else if (type == typeof(FundPage))
        //        {
        //            if (tab.Uri.ToLower().Contains("funds"))
        //            {
        //                b.Template = (ControlTemplate)FindResource("NavigationButtonSelected");
        //                // CurrentScreen = tab;
        //            }
        //            else
        //            {
        //                b.Template = (ControlTemplate)FindResource("NavigationButton");
        //            }

        //        }
        //        else if (type == typeof(ConditionPage))
        //        {
        //            if (tab.Uri.ToLower().Contains("condtion"))
        //            {
        //                b.Template = (ControlTemplate)FindResource("NavigationButtonSelected");
        //                // CurrentScreen = tab;
        //            }
        //            else
        //            {
        //                b.Template = (ControlTemplate)FindResource("NavigationButton");
        //            }

        //        }
        //        else if (type == typeof(NPDPageV2))
        //        {
        //            if (tab.Uri.ToLower().Contains("npd"))
        //            {
        //                b.Template = (ControlTemplate)FindResource("NavigationButtonSelected");
        //                // CurrentScreen = tab;
        //            }
        //            else
        //            {
        //                b.Template = (ControlTemplate)FindResource("NavigationButton");
        //            }

        //        }
        //        else
        //        {
        //            if (tab.Uri.ToLower().Contains(type.Name.ToLower()))
        //            {
        //                b.Template = (ControlTemplate)FindResource("NavigationButtonSelected");
        //               // CurrentScreen = tab;
        //            }
        //            else
        //            {
        //                b.Template = (ControlTemplate)FindResource("NavigationButton");
        //            }
        //        }
                     
        //    }
             
        //}



        public void NavigateTo(Page view)
        {
            frmMain.Navigate(view);
        }

        public void NavigateTo(Page view, Action navigated)
        {
            frmMain.Navigate(view, navigated);
        }

        private static bool _isNavEnabled;
        public bool isNavEnabled { get { return _isNavEnabled; } set { _isNavEnabled = value; } }

        public void EnableNavigation(bool val)
        {
            Menu.EnableNavigation = val;
            //TopLevelNavigation.IsEnabled = val;
            //btnMore.IsEnabled = val;
            //if (val == true)
            //{
            //    TopLevelNavigation.Opacity = 1;
            //    btnMore.Opacity = 1;
            //}
            //else
            //{
            //    TopLevelNavigation.Opacity = .5;
            //    btnMore.Opacity = .5;
            //}
        }

        public System.Windows.Input.ICommand Logout { get { return new ViewCommand(new Action<object>(_ => LogoutClick(null, null))); } }

        private void LogoutClick(object sender, RoutedEventArgs e)
        {
            // Logout
            User.Logout();
            
            App.AppCache = new ApplicationCache();
            App.CachedListingsViewModels = null;
            App.Configuration.Logout();
            XmlCache.Clear();

            if (App.AuthContext != null)
            {
                App.AuthContext = null;
            }
            if (NavigationService != null)
            {
                App.AutomaticLoginSuppressed = true;
             

                NavigationService.Navigate(new Uri("/Pages/Login.xaml", UriKind.RelativeOrAbsolute));
            }

        }


        //private IDictionary<Uri, Button> CreateButtonLookup()
        //{
        //    return new Dictionary<Uri, Button>
        //        {
        //            {new Uri("Pages/Insights.xaml", UriKind.Relative), btnInsights},
        //            {new Uri("Pages/UserSettings/Default.xaml", UriKind.Relative), menu_middle},
        //            {new Uri("Pages/AnalyticsV2.xaml", UriKind.Relative), btnAnalyticsV2},
        //            {new Uri("Pages/SchedulePage.xaml", UriKind.Relative), btnPlanningSchedule},
        //            {new Uri("Pages/SchedulePageV2.xaml", UriKind.Relative), btnPlanningScheduleV2},
        //            {new Uri("Pages/Planning.xaml", UriKind.Relative), btnPlanning},
        //            {new Uri("Pages/Pricing.xaml", UriKind.Relative), btnPricing},
        //            {new Uri("Pages/Promotions.xaml", UriKind.Relative), btnPromotions},
        //            {new Uri("Pages/ScenariosList.xaml", UriKind.Relative), btnScenarios},
        //            {new Uri("Pages/Conditions.xaml", UriKind.Relative), btnConditions},
        //            {new Uri("Pages/NPD/NPDList.xaml", UriKind.Relative), btnNPD},
        //            {new Uri("Pages/Claims.xaml", UriKind.Relative), btnClaims},
        //            {new Uri("Pages/InsightsV2/InsightsV2.xaml", UriKind.Relative), btnInsightsV2},
        //            {new Uri("Pages/Funds/FundsList.xaml", UriKind.Relative), btnFundsV2},
        //            {new Uri("Pages/Admin/SideMenu.xaml", UriKind.Relative), btnAdmin}
        //        };
        //}

        //private readonly HashSet<Button> _robButtons = new HashSet<Button>();

        //private void ButtonLoaded(object sender, RoutedEventArgs e)
        //{
        //    var button = sender as Button;
        //    if (button != null)
        //    {
        //        if (App.Configuration.ROBScreens.Any(s => s.Key == App.Configuration.StartScreen && button.CommandParameter.ToString() == s.AppTypeID))
        //        {
        //            button.Template = (ControlTemplate)FindResource("NavigationButtonSelected");
        //        }
        //        _robButtons.Add(button);
        //    }
        //}


        //private void Image_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    //this.version.Visibility = (this.version.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed);

        //}

        //private void adminLog_Click(object sender, RoutedEventArgs e)
        //{
        //    sideTabClickMethod(listOfTabGrids.Where(a => a.Name == "sysTab").First(), listOfTabButtons.Where(b => b.Name == "diagnosticButton").First());

        //}

        //private void asynchronous_Tasks_Click(object sender, RoutedEventArgs e)
        //{
        //    sideTabClickMethod(listOfTabGrids.Where(a => a.Name == "asyncTasksTab").First(), listOfTabButtons.Where(b => b.Name == "asynchronous_Tasks").First());

        //}

        //private void account_Builds_Click(object sender, RoutedEventArgs e)
        //{

        //    sideTabClickMethod(listOfTabGrids.Where(a => a.Name == "buildQueueTab").First(), listOfTabButtons.Where(b => b.Name == "account_Builds").First());

        //}

        //public void sideTabClickMethod(Grid thisGrid, Button thisButton)
        //{
        //    if (thisGrid.Visibility == Visibility.Hidden)
        //    {
        //        //rightCol.Width = 300;
        //        listOfTabGrids.Where(a => a == thisGrid).First().Visibility = Visibility.Visible;
        //        foreach (var thisItem in listOfTabGrids.Where(a => a != thisGrid))
        //        {
        //            thisItem.Visibility = Visibility.Hidden;
        //        }
        //        foreach (var thisItem in listOfTabButtons.Where(a => a != thisButton))
        //        {
        //            thisItem.Template = (ControlTemplate)FindResource("NavigationButton");
        //        }
        //        Thickness margin = thisButton.Margin;
        //        margin.Left = 0;
        //        listOfTabButtons.Where(a => a == thisButton).First().Template = (ControlTemplate)FindResource("NavigationButtonSelected");
        //        listOfTabButtons.Where(a => a == thisButton).First().Margin = margin;

        //    }
        //    else
        //    {
        //        //rightCol.Width = 0;
        //        foreach (var thisItem in listOfTabGrids.Where(a => a.Visibility != Visibility.Hidden))
        //        {
        //            thisItem.Visibility = Visibility.Hidden;
        //        }
        //        //foreach (var thisButtonItem in listOfTabButtons)
        //        //{
        //        //    thisButtonItem.Template = (ControlTemplate)FindResource("NavigationButton");
        //        //}
        //    }
        //}

        //List<Grid> listOfTabGrids;
        //List<Button> listOfTabButtons;

        //  static Timer _buildTimer;
        public void _buildTimerElapsed(object sender, ElapsedEventArgs e)
        {

            if (!_backgroundWorker.IsBusy)
                _backgroundWorker.RunWorkerAsync("Init");
        }


        private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Do something
            if (e.Argument.ToString() == "Init")
            {
                UpdateAccountBuildList();
                UpdateAsyncTasks();
            }

        }

        // Completed Method
        private void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //string statusText = "";
            //if (e.Cancelled)
            //{
            //    statusText = "Cancelled";
            //}
            //else if (e.Error != null)
            //{
            //    statusText = "Exception Thrown";
            //}
            //else
            //{
            //    statusText = "Completed";

            //}
        }


        private string path = AppDomain.CurrentDomain.GetPath() + "Log.txt";

        // refresh button
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //output.Text = File.ReadAllText(path);
            outputgrid.ItemsSource = Exceedra.Common.Logging.StorageBase.Items;
        }

        // clear (bin) button
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            File.WriteAllText(path, String.Empty);
            //output.Text = File.ReadAllText(path);
            Exceedra.Common.Logging.StorageBase.Clear();
            outputgrid.ItemsSource = Exceedra.Common.Logging.StorageBase.Items;
        }

        private void RadMenuItem_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            //Clipboard.SetText(output.Text);
        }

        //private void txtCurrentUserDescription_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{

        //}

        //private void btnAdmin_Click(object sender, RoutedEventArgs e)
        //{

        //}

        private void update_Account_Build_List_Click(object sender, RoutedEventArgs e)
        {
            UpdateAccountBuildList();
        }


        ObservableCollection<BuildQueueClass> thisBuildQueueList;
        private void UpdateAccountBuildList()
        {
            string argument = string.Format("<GetInfoAccountPlan><User_Idx>{0}</User_Idx></GetInfoAccountPlan>", User.CurrentUser.ID);
            try
            {
                var nodes = WebServiceProxy.Call(StoredProcedure.Info.AccountPlanBuildQueue, XElement.Parse(argument), DisplayErrors.No).Elements();

                //                var thisString = @"    
                //      <AccountPlanInfo>
                //        <Task_ID>76</Task_ID>
                //        <Description>Account Plan Rebuild for Planning Save</Description>
                //        <Queued_Date>2015-01-12T14:35:40.437</Queued_Date>
                //        <Queued_By>Exceedra, Admin</Queued_By>
                //        <Start_Date>1970-01-01</Start_Date>
                //        <Start_Date_Is_Estimate>1</Start_Date_Is_Estimate>
                //        <Finished_Date>1970-01-01</Finished_Date>
                //        <Finish_Date_Is_Estimate>1</Finish_Date_Is_Estimate>
                //      </AccountPlanInfo>";
                //                var thisOtherString = @"
                //      <AccountPlanInfo>
                //        <Task_ID>75</Task_ID>
                //        <Description>Account Plan Rebuild for Promotion AX-6</Description>
                //        <Queued_Date>2015-01-12T14:28:04.353</Queued_Date>
                //        <Queued_By>Exceedra, Admin</Queued_By>
                //        <Start_Date>1970-01-01</Start_Date>
                //        <Start_Date_Is_Estimate>1</Start_Date_Is_Estimate>
                //        <Finished_Date>1970-01-01</Finished_Date>
                //        <Finish_Date_Is_Estimate>1</Finish_Date_Is_Estimate>
                //      </AccountPlanInfo>
                //    ";
                //                var nodes = new List<XElement>();
                //                nodes.Add(XElement.Parse(thisString));
                //                nodes.Add(XElement.Parse(thisOtherString));

                //foreach (XElement element in nodes)
                //{
                //    thisBuildQueueList.Add(new BuildQueueClass(element).BuildQueueItemToString());
                //}

                //Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.buildQueueOutput.Text = string.Join(Environment.NewLine, thisBuildQueueList)));
                foreach (XElement element in nodes)
                {
                    thisBuildQueueList.Add(new BuildQueueClass(element));
                }
                //OthisBuildQueueList = new ObservableCollection<BuildQueueClass>(from items in thisBuildQueueList
                //                           orderby thisBuildQueueList.OrderByDescending(a => a).Take(10)
                //                           select items);

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.buildQueueOutput.ItemsSource = thisBuildQueueList.OrderBy(a => a.GetTimeStamp()).Take(10)));
            }
            catch (Exception ex)
            {
                // Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.buildQueueOutput.Text = ex.Message));
                if (buildQueueTab.Visibility == Visibility.Visible)
                {
                    CustomMessageBox.Show(ex.Message);
                }
            }
        }

        private void clear_Account_Build_List_Click(object sender, RoutedEventArgs e)
        {
            this.buildQueueOutput.ItemsSource = new ObservableCollection<BuildQueueClass>();
        }

        private void update_Async_Tasks_Click(object sender, RoutedEventArgs e)
        {

            UpdateAsyncTasks();
        }

        private void UpdateAsyncTasks()
        {
            ObservableCollection<BuildQueueClass> thisAsyncList = new ObservableCollection<BuildQueueClass>();
            string argument = "<GetInfoAsyncTasks><User_Idx>{0}</User_Idx></GetInfoAsyncTasks>".FormatWith(1);//User.CurrentUser.ID);
            try
            {

                var nodes = WebServiceProxy.Call(StoredProcedure.Info.AsynchronousTaskQueue, XElement.Parse(argument), DisplayErrors.No).Elements();
                //                var nodes = @"    <Results>
                //      <AsyncTasksInfo>
                //        <Task_ID>1</Task_ID>
                //        <Description>Create Closed Copy of Scenario </Description>
                //        <Queued_Date>2015-01-12T11:30:13.287</Queued_Date>
                //        <Queued_By>Exceedra, Admin</Queued_By>
                //        <Start_Date>1970-01-01</Start_Date>
                //        <Start_Date_Is_Estimate>1</Start_Date_Is_Estimate>
                //        <Finished_Date>2015-01-12T11:30:16.717</Finished_Date>
                //        <Finish_Date_Is_Estimate>0</Finish_Date_Is_Estimate>
                //      </AsyncTasksInfo>
                //    </Results>";

                foreach (XElement element in nodes)
                {
                    thisAsyncList.Add(new BuildQueueClass(element));
                }
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.asyncTasksOutput.ItemsSource = thisAsyncList.OrderBy(a => a.GetTimeStamp()).Take(10)));
                // asyncTasksOutput.Text = nodes.ToString();
            }
            catch (Exception ex)
            {
                //Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.asyncTasksOutput.Text = ex.Message));
                //asyncTasksOutput.Text = ex.Message;
                if (asyncTasksTab.Visibility == Visibility.Visible)
                {
                    //MessageBox.Show(ex.Message);
                    thisAsyncList.Add(new BuildQueueClass()
                    {
                        Description = ex.Message,
                        Finish_Date = DateTime.Now.ToString()
                    });
                }
            }
        }

        private void clear_Async_Tasks_Click(object sender, RoutedEventArgs e)
        {
            this.asyncTasksOutput.ItemsSource = new ObservableCollection<BuildQueueClass>();
        }

        private byte[] _screenshot { get; set; }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var fb = new Exceedra.Common.Logging.FeedbackBase();
            Assembly assem = Assembly.GetEntryAssembly();
            AssemblyName assemName = assem.GetName();
            Version ver = assemName.Version;
              
            fb.SendMail(tfeedback.Text, _screenshot, string.Format("Version {0}", ver.ToString()), Model.User.CurrentUser.DisplayName);
            tfeedback.Text = "";
            ProductDataModalPresenter.Visibility = Visibility.Hidden;
            _screenshot = new byte[0];


           

        }
 
        public string DisplayName { get { return User.CurrentUser.DisplayName.Split(',').Last().Trim(); } }

        public string DisplayInitials { get { return string.Concat(User.CurrentUser.DisplayName.Replace(",", "").Trim().Split(' ').Select(s => s.Trim().First()).Reverse()); } }

        private void ButtonBase_OnClick2(object sender, RoutedEventArgs e)
        {
            _screenshot = this.GetJpgImage(1, 90);
            ProductDataModalPresenter.Visibility = Visibility.Visible;
        }

        private void ButtonBase_OnClick3(object sender, RoutedEventArgs e)
        {
            tfeedback.Text = "";
            ProductDataModalPresenter.Visibility = Visibility.Hidden;
        }

        // converting app trace row into template ready for SSMS and copying it into the clipboard
        private void Button_AppTraceGetSsmsTemplate_RightClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var s = e.OriginalSource as FrameworkElement;

            var parentRow = s.ParentOfType<GridViewRow>();
            if (parentRow != null)
            {
                var logItem = parentRow.Item as LogItem;

                // preparing SSMS template
                string rowToPassToClipboard = @"EXEC ";
                rowToPassToClipboard += logItem.Method;
                rowToPassToClipboard += "\n@XML_In =\n'\n";
                rowToPassToClipboard += logItem.Response;
                rowToPassToClipboard += "\n'";

                Clipboard.SetText(rowToPassToClipboard);
            }

        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var viewModel = DataContext as MainPageViewModel;
            if (viewModel == null) return;

            ObservableCollection<Screen> excessedTabs = new BaseCollection<Screen>();
            for (int i = 0; i < tabsMenu.Items.Count; i++)
            {
                var tab = tabsMenu.ItemContainerGenerator.ContainerFromItem(tabsMenu.Items[i]) as FrameworkElement;
                if (tab == null) continue;

                var isVisible = IsUserVisible(tab, SplitContainer);
                if (!isVisible) excessedTabs.Add(viewModel.Screens[i]);
            }

            viewModel.ExcessedScreens = excessedTabs;
        }

        private bool IsUserVisible(FrameworkElement element, FrameworkElement container)
        {
            if (!element.IsVisible) return false;

            Rect bounds = element.TransformToAncestor(container).TransformBounds(new Rect(0.0, 0.0, element.ActualWidth, element.ActualHeight));
            Rect rect = new Rect(0.0, 0.0, container.ActualWidth, container.ActualHeight);
            //return rect.IntersectsWith(bounds);
            return rect.Contains(bounds);
            //return rect.Contains(bounds.TopLeft) || rect.Contains(bounds.BottomRight);
        }

        private void btnMore_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Button button = sender as Button;
            button.ContextMenu.PlacementTarget = button;
            button.ContextMenu.IsOpen = true;
        }

        public void HyperlinkClicked(string type, string idx, string content, string appTypeIdx, bool popout = false)
        {
            //string appType = null;
            //if (type.ToLowerInvariant().Contains("rob") || type.ToLowerInvariant().Contains("contract"))
            //{
            //    appType = type.Substring(type.Length - 3, 3);
            //    type = type.Replace(appType, "");
            //}

            RedirectMe.Goto(type, idx, content, appTypeIdx,"", popout);

        }

        public void OpenScanLocation(string scanLocation)
        {
            App.OpenScanLocation(scanLocation);
        }

        public void DataPointClicked(string type, string idx)
        {
            RedirectMe.Goto(type, idx);
        }

        private void CloseClick(object sender, MouseButtonEventArgs e)
        {
            Menu.Toggle();
        }

        private void pane1_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!pane1.IsPinned)
            {
                outputgrid.ItemsSource = StorageBase.Items;
                pane1.IsPinned = true;
            }
        }

        private void Menu_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var b = FindVisualChild<WebBrowser>(frmMain);
            if(b!=null)
                   b.Visibility = (Menu.State == MenuState.Hidden) ? Visibility.Visible : Visibility.Hidden;
        }
    }
}