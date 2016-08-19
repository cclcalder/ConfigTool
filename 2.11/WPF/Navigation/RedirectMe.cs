using Exceedra.Common.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Exceedra.Common;
using Model;
using Model.DataAccess;
using Model.Entity.ROBs;
using ViewHelper;
using ViewModels;
using WPF.Pages;
using WPF.Pages.Demand.DPFcMgmt;
using WPF.Pages.EPOS;
using WPF.Pages.MyActionsDashboard;
using WPF.Pages.NPD;
using WPF.Pages.RobContracts;
using WPF.Pages.RobGroups;
using WPF.Pages.Supersession;
using WPF.Pages.Terms;
using WPF.PromoTemplates;
using WPF.UserControls;
using WPF.ViewModels;
using WPF.ViewModels.Claims;
using WPF.ViewModels.Conditions;
using WPF.ViewModels.Demand;
using WPF.ViewModels.NPD;
using WPF.ViewModels.PromoTemplates;
using WPF.ViewModels.RobContracts;
using WPF.ViewModels.ROBGroups;
using WPF.ViewModels.Scenarios;
using WPF.Wizard;
using Application = System.Windows.Application;

namespace WPF.Navigation
{
    
    public class RedirectMe
    {
        public static string LastOpenTab { get; set; }

        public static void Goto(string path, string idx = "", string content = "", string param = "", string listScreenTabName = "", bool popout = false)
        {
            LastOpenTab = listScreenTabName;
            if (popout) //App.Configuration.IsPopupWindowsActive
            {
                Popup(path, idx, content, param);
            }
            else
            {
                EntryPoint(path, idx, param);
            }
        }

        #region Navigation To Editors 

        public static void EntryPoint(string path, string idx = "", string param = "", string listScreenTabName = "")
        {
            bool lockNav = false;
            switch (path.ToLowerInvariant())
            {
                case "condition":
                    EditCondition(idx);
                    break;

                case "canvas":
                    ViewCanvas(idx);
                    lockNav = true;
                    break;

                case "scenario":
                    EditScenario(idx);
                    break;

                case "fund":
                case "funds":
                case "child_funds":
                case "transfer_grid":
                    EditFunds(idx);
                    break;

                case "parentfund":
                    EditParentFund(idx);                    
                    break;

                case "npd":
                case "npd list":
                    EditNpd(idx);
                    break;

                case "contract":
                case "contractgrid":
                    EditContract(idx, param);
                    break;

                case "rob":
                case "robgrid":
                    EditRob(idx, param);
                    break;

                case "materialrobgrid":
                case "robgroup":
                    EditMaterialRob(idx, param);
                    break;

                case "robgroupcreator":
                    ShowRobGroupCreator(param);
                    break;

                case "groups":
                    EditGroups(idx, param);
                    break;
                    
                case "promotion":
                    App.Navigator.NavigateTo(new WizardFrame(String.IsNullOrEmpty(idx) ? null : idx));
                     
                    break;
                case "template":
                    App.Navigator.NavigateTo(new TemplateFrame(String.IsNullOrEmpty(idx) ? null : idx));
                    break;

                case "claim":
                    EditClaim(idx);
                    break;

                case "event":
                    EditEvent(idx);
                    break;

                case "fcmgmt":
                case "forecasts":
                    EditFcMgmt(idx);
                    break;

                default: /* Special case for when naviation from a schedule. Some times thier path is not Rob but Term, Mgmt etc. 
                          * but that is client specific so we cannot navigate based on that name.
                          * So we check if we have a param, aka AppTypeIdx.
                          */
                    if(!String.IsNullOrWhiteSpace(param) && param != "0")
                        if (path.ToLower().Contains("contract"))
                            EditContract(idx, param);
                        else
                            EditRob(idx, param);
                    break;

            }

            App.Navigator.EnableNavigation(lockNav);
        }

        private static void ViewCanvas(string idx)
        {
           var page = new MyActionsDashboard(idx);
            MessageBus.Instance.Publish(new NavigateMessage(page));
        }

        private static void EditGroups(string idx, string param)
        {
            var page = new GroupEditor(param, idx);
            MessageBus.Instance.Publish(new NavigateMessage(page));
        }

        private static void EditFunds(string idx)
        {
            var page = new FundPage(idx);
            MessageBus.Instance.Publish(new NavigateMessage(page));
        }

        private static void EditParentFund(string idx)
        {
            var page = new FundPage(idx, true);
            MessageBus.Instance.Publish(new NavigateMessage(page));
        }

        private static void EditNpd(string idx)
        {
            var page = new NPDPageV2(idx);
            MessageBus.Instance.Publish(new NavigateMessage(page));
        }

        private static void EditScenario(string idx)
        {
            var page = new ScenarioPage( String.IsNullOrWhiteSpace(idx) ? "0" : idx );
            MessageBus.Instance.Publish(new NavigateMessage(page));
        }

        private static void EditCondition(string idx)
        {
            var page = new ConditionPage(idx ?? "0");
            MessageBus.Instance.Publish(new NavigateMessage(page));
        }
        

        private static void EditClaim(string idx)
        {
            var page = new ClaimPage(idx);
            MessageBus.Instance.Publish(new NavigateMessage(page));
        }

        private static void EditContract(string idx, string appType)
        {
            RobAccess a = new RobAccess(appType);
            var viewModel = new RobContractsEditorViewModel(a, appType, idx);
            var page = new RobContractsEditorView(viewModel);
            MessageBus.Instance.Publish(new NavigateMessage(page));
        }

        private static void EditEvent(string idx)
        {
            var page = new EventEditorPage(idx);
            MessageBus.Instance.Publish(new NavigateMessage(page));
        }

        private static void EditFcMgmt(string idx)
        {
            var page = new DPFcMgmtEditor(idx);
            MessageBus.Instance.Publish(new NavigateMessage(page));
        }

        private static void ShowRobGroupCreator(string appType)
        {
            var page = new GroupCreator(appType);
            MessageBus.Instance.Publish(new NavigateMessage(page));
        }
        
        private static void EditRob(string idx, string appType)
        {
            if (!String.IsNullOrWhiteSpace(idx))
            {
                RobAccess a = new RobAccess(appType);
                a.GetRob(appType, idx)
                    .ContinueWith(t => EditRobContinuation(t, appType), App.Scheduler);
            }
            else
            {
                var eventVM = new EventViewModel();
                var thisEventViewModel = eventVM.NewEventViewModel(appType);
                var page = new EventPage(thisEventViewModel);
                MessageBus.Instance.Publish(new NavigateMessage(page));
            }
        }

        private static void EditRobContinuation(Task<Rob> task, string appType)
        {
            if (task.IsFaulted)
            {
                Messages.Instance.PutError(task.Exception.AggregateMessages());
                return;
            }
            if ((!task.IsCanceled) && task.Result != null)
            {
                var screen = App.Configuration.GetScreens().FirstOrDefault(rs => rs.RobAppType == appType);
                string pageTitle = screen != null ? screen.Label : "ROB";

                var page = new EventPage(EventViewModel.FromRob(appType, pageTitle, task.Result));
                MessageBus.Instance.Publish(new NavigateMessage(page));
            }
        }

        private static void EditMaterialRob(string idx, string appType)
        {
            var groupCreator = new GroupCreator(appType, idx);
            MessageBus.Instance.Publish(new NavigateMessage(groupCreator));
        }

        #endregion

        #region Navigation to Popups

        public static void Popup(string path, string idx = "", string content = "", string param = "")
        {
            App.Navigator.EnableNavigation(false);

            var tag = path.ToLowerInvariant() + ":" + param + ":" + idx;
            if (CheckForExistingWindows(tag)) return;

            var newWindow = ShowWindow(content);
            newWindow.Tag = tag;
            newWindow.Closed += CheckNavigation;
             
            //path = "";
            //PopupDummy("", newWindow);
            //PopupEposMatching("", newWindow);

            switch (path.ToLowerInvariant())
            {
                case "condition":
                    PopupCondition(idx, newWindow);
                    break;

                case "canvas":
                    PopupCanvas(idx, newWindow);
                    break;

                case "scenario":
                    PopupScenario(idx, newWindow);
                    break;

                case "fund":
                case "funds":
                case "child_funds":
                    PopupFund(idx, newWindow);
                    break;

                case "parentfund":
                    PopupParentFund(idx, newWindow);
                    break;

                case "npd":
                case "npd list":
                    PopupNPD(idx, newWindow);
                    break;

                case "rob":
                case "robgrid":
                    PopupRob(idx, param, newWindow);
                    break;

                case "materialrob":
                case "robgroup":
                    PopupMaterialRob(idx, param, newWindow);
                    break;

                case "robgroupcreator":
                    PopupRobGroup(param, newWindow);
                    break;

                case "contractgrid":
                case "contract":
                    PopupContract(idx, param, newWindow);
                    break;

                case "promotion":
                    //PopupDummy(idx, newWindow);
                    PopupPromotion(idx, newWindow);
                    break;

                case "template":
                    PopupTemplate(idx, newWindow);                    
                    break;

                case "claim":
                    PopupClaim(idx, newWindow);
                    break;

                case "event":
                    PopupEvent(idx, newWindow);
                    break;

                case "fcmgmt":
                case "forecasts":
                    PopupFcMgmt(idx, newWindow);
                    break;
            }
        }

        private static void PopupCanvas(string idx, Window window)
        {
            EditorHandler(window, idx, typeof(MyActionsDashboard));
        }

        private static void PopupPromotion(string idx, Window window)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                var page = new PLReview(new PromotionWizardViewModel(null, String.IsNullOrEmpty(idx) ? null : idx));
                    ((PromotionWizardViewModel) (page.DataContext)).GoToPromotionsCommand =
                        new ViewCommand(CancelOverride, window);
                    window.Content = page;
            }));
        }

        private static void PopupTemplate(string idx, Window window)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                var page = new TemplatePLReview(new PromotionTemplateViewModel(null, String.IsNullOrEmpty(idx) ? null : idx));
                    ((PromotionTemplateViewModel) (page.DataContext)).GoToPromotionsCommand =
                        new ViewCommand(CancelOverride, window);
                    window.Content = page;                
            }));
        }

        private static void PopupNPD(string idx, Window window)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                var page = new NPDPageV2(idx);
                ((NPDViewModel)(page.DataContext)).CancelCommand = new ViewCommand(CancelOverride, window);
                window.Content = page;
            }));
        }

        private static void PopupFcMgmt(string idx, Window window)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                var page = new DPFcMgmtEditor(idx);
                ((DPFcMgmtEditorViewModel)(page.DataContext)).CancelCommand = new ViewCommand(CancelOverride, window);
                window.Content = page;
            }));
        }

        private static void PopupScenario(string idx, Window window)
        {
            //Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            //{
            //    var page = new ScenarioPage(Convert.ToInt32(String.IsNullOrWhiteSpace(idx) ? "0" : idx));
            //    ((ScenarioDetailsViewModel)(page.DataContext)).CancelCommand = new ViewCommand(CancelOverride, window);
            //    window.Content = page;
            //}));            

            EditorHandler(window, idx, typeof(ScenarioPage));
        }

        private static void PopupCondition(string idx, Window window)
        {
            EditorHandler(window, idx,typeof(ConditionPage));            
        }

        private static void PopupEposMatching(string idx, Window window)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                var page = new Home();
                //((ConditionDetailsViewModel)(page.DataContext)).CancelCommand = new ViewCommand(CancelOverride, window);
                window.Content = page;
            }));
        }

        private static void PopupDummy(string idx, Window window)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                var page = new Supersession();
                //var page = new dummy();
                //((ConditionDetailsViewModel)(page.DataContext)).CancelCommand = new ViewCommand(CancelOverride, window);
                window.Content = page;
            }));
        }

        private static void PopupFund(string idx, Window window)
        {
            EditorHandler(window,idx, typeof (FundPage));
        }

        private static void PopupParentFund(string idx, Window window)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                var page = new FundPage(idx, true);
                ((FundViewModel)(page.DataContext)).CancelCommand = new ViewCommand(CancelOverride, window);
                window.Content = page;
            }));
        }

        private static void PopupRobGroup(string appType, Window window)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                var page = new GroupCreator(appType);
                ((GroupCreatorViewModel)(page.DataContext)).CloseCommand = new ViewCommand(CancelOverride, window);
                window.Content = page;
            }));      
        }

        private static void PopupRob(string idx, string appType, Window window)
        {
            if (!String.IsNullOrWhiteSpace(idx))
            {
                RobAccess a = new RobAccess(appType);
                a.GetRob(appType, idx)
                    .ContinueWith(t => PopupRobContinuation(t, appType, window), App.Scheduler);
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    var screen = App.Configuration.GetScreens().FirstOrDefault(rs => rs.RobAppType == appType);
                    string pageTitle = screen != null ? screen.Label : "ROB";

                    var eventVM = new EventViewModel();
                    var thisEventViewModel = eventVM.NewEventViewModel(appType);
                    thisEventViewModel.PageTitle = pageTitle;
                    var page = new EventPage(thisEventViewModel);
                    ((EventViewModel)(page.DataContext)).CancelCommand = new ViewCommand(CancelOverride, window);
                    window.Content = page;
                }));

            }
        }

        private static void PopupMaterialRob(string idx, string appType, Window window)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                var page = new GroupCreator(appType, idx);
                window.Content = page;
            }));
        }

        private static void PopupRobContinuation(Task<Rob> task, string appType, Window window)
        {
            if (task.IsFaulted)
            {
                Messages.Instance.PutError(task.Exception.AggregateMessages());
                return;
            }
            if ((!task.IsCanceled) && task.Result != null)
            {
                var screen = App.Configuration.GetScreens().FirstOrDefault(rs => rs.RobAppType == appType);
                string pageTitle = screen != null ? screen.Label : "ROB";

                //Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                //{
                    var page = new EventPage(EventViewModel.FromRob(appType, pageTitle, task.Result));
                //    ((EventViewModel)(page.DataContext)).CancelCommand = new ViewCommand(CancelOverride, window);
                //    window.Content = page;
                //}));     

                EditorHandler(window, page);
            }
        }

      
        private static void PopupContract(string idx, string appType, Window window)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                RobAccess a = new RobAccess(appType);
                var viewModel = new RobContractsEditorViewModel(a, appType, idx);
                var page = new RobContractsEditorView(viewModel);               

                EditorHandler(window, page);

            }));  
        }

        private static void PopupClaim(string idx, Window window)
        {
            EditorHandler(window,idx, typeof(ClaimPage));          
        }

        private static void PopupEvent(string idx, Window window)
        {
            //Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            //{
            //    var page = new EventEditorPage(idx);
            //    ((EventEditorPageViewModel)(page.DataContext)).CancelCommand = new ViewCommand(CancelOverride, window);
            //    window.Content = page;
            //}));
            EditorHandler(window, idx, typeof(EventEditorPage));
        }

        private static Window ShowWindow(string title)
        {
            Window w = new Window { Title = title };

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                var loader = new LoadingPanel { IsLoading = true };
                w.Content = loader;
                w.Show();
            }));

            w.Owner = Application.Current.MainWindow;
            return w;
        }

        private static bool CheckForExistingWindows(string newTag)
        {
            var matchedWindows = Application.Current.Windows.OfType<Window>().Where(x => (string)x.Tag == newTag).ToList();
            if (matchedWindows.Any())
            {
                matchedWindows.Skip(1).Do(x => x.Close());
                matchedWindows[0].Activate();
                matchedWindows[0].WindowState = WindowState.Normal;
                //MessageBox.Show("This is open in another window!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return true;
            }
            return false;
        }

        private static void CancelOverride(object o)
        {
            Application.Current.Dispatcher.Invoke((Action) delegate
            {
                ((Window) o).Close();
                CheckNavigation(null, null);                
            });
        }

      

        private static void CheckNavigation(object sender, EventArgs e)
        {
            if (Application.Current.Windows.Count <= 1)
            {
                App.Navigator.EnableNavigation(true);
            } 
        }

        #endregion

        #region "Popup Editor handlers"
        private static void ReloadOverrideIdx(object o)
        {
            var w = (Window) o;
            dynamic content = w.Content;
            dynamic type = content.GetType();
            dynamic dataContext = content.DataContext;            
            dynamic idx = dataContext.MyIdx;


            EditorHandler(w, idx, type); 
        }

        private static void ReloadOverrideRob(object o)
        {
            var w = (Window)o;
            dynamic content = w.Content;
            EventViewModel dataContext = content.DataContext;

            if (!String.IsNullOrWhiteSpace(dataContext._originalRob.ID))
            {
                RobAccess a = new RobAccess(dataContext.AppTypeID);
                a.GetRob(dataContext.AppTypeID, dataContext._originalRob.ID)
                    .ContinueWith(t => PopupRobContinuation(t, dataContext.AppTypeID, w), App.Scheduler);
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    var screen = App.Configuration.GetScreens().FirstOrDefault(rs => rs.RobAppType == dataContext.AppTypeID);
                    string pageTitle = screen != null ? screen.Label : "ROB";

                    var eventVM = new EventViewModel();
                    var thisEventViewModel = eventVM.NewEventViewModel(dataContext.AppTypeID);
                    thisEventViewModel.PageTitle = pageTitle;
                    var page = new EventPage(thisEventViewModel);

                    EditorHandler(w,page);

                }));

            }

            //var w = (Window)o;
            //dynamic content = w.Content; 
            //EventViewModel dataContext = content.DataContext;
            //var appType = dataContext.AppTypeID;

            //var screen = App.Configuration.Screens.FirstOrDefault(rs => rs.RobAppType == appType);
            //string pageTitle = screen != null ? screen.Label : "ROB";

            //var eventVM = new EventViewModel();
            //var thisEventViewModel = eventVM.NewEventViewModel(appType);
            //thisEventViewModel.PageTitle = pageTitle;
            //var page = new EventPage(thisEventViewModel);

            //EditorHandler(w, page);
        }

        private static void ReloadOverrideContract(object o)
        {
            var w = (Window)o;
            dynamic content = w.Content;
            RobContractsEditorViewModel dataContext = content.DataContext;

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                RobAccess a = new RobAccess(dataContext.ContractId);
                var viewModel = new RobContractsEditorViewModel(a, dataContext._robScreenId, dataContext.ContractId);
                var page = new RobContractsEditorView(viewModel);

                EditorHandler(w, page);

            }));
        }

        private static void EditorHandler(Window o, string idx, Type type)
        {
            var cc = new ViewCommand(CancelOverride, o);
            var rc = new ViewCommand(ReloadOverrideIdx, o);

            dynamic page = Activator.CreateInstance(type, idx);
            try
            {
                page.DataContext.CancelCommand = cc;
                page.DataContext.ReloadCommand = rc;
            }
            catch
            {
            }

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            { 
                o.Content = page;
            }));
        }

        private static void EditorHandler(Window o, EventPage page)
        {
            var cc = new ViewCommand(CancelOverride, o);
            var rc = new ViewCommand(ReloadOverrideRob, o);


            ((EventViewModel)(page.DataContext)).CancelCommand =cc;
            ((EventViewModel)(page.DataContext)).ReloadCommand = rc;

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                o.Content = page;
            }));
        }

        private static void EditorHandler(Window o, RobContractsEditorView page)
        {
            var cc = new ViewCommand(CancelOverride, o);
            var rc = new ViewCommand(ReloadOverrideContract, o);


            ((RobContractsEditorViewModel)(page.ViewModel)).CancelCommand = cc;
            ((RobContractsEditorViewModel)(page.ViewModel)).ReloadCommand = rc;

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                o.Content = page;
            }));
        }


        #endregion

        //TODO
        #region Navigation To List Screens

        public static void ListScreen(string path, string param = "")
        {
            switch (path.ToLowerInvariant())
            {
                case "npd":
                    NPDList();
                    break;
                case "fund":
                    FundList();
                    break;
                case "claims":
                    ClaimsList();
                    break;
                case "scenarios":
                    ScenariosList();
                    break;
                case "promo":
                    PromoList();
                    break;
                case "templates":
                    TemplatesList();
                    break;
                case "conditions":
                    ConditionsList();
                    break;
                case "fcmgmt":
                    FcMgmtList();
                    break;
            }

            CheckNavigation(null, null);
        }

        private static void TemplatesList()
        {
            var page = new Promotions(true);
            //page.Tabs.TabDataSource.SelectedTabIndex = 2;
            MessageBus.Instance.Publish(new NavigateMessage(page));
        }

        private static void PromoList()
        {
            var page = new Promotions(true);
            MessageBus.Instance.Publish(new NavigateMessage(page));
        }

        private static void RobsList(string param)
        {
            var page = new EventsPage(param);
            MessageBus.Instance.Publish(new NavigateMessage(page));
        }

        private static void ScenariosList()
        {
            var page = new ScenariosList();
            MessageBus.Instance.Publish(new NavigateMessage(page));
        }

        private static void ClaimsList()
        {
            var page = new Claims();
            MessageBus.Instance.Publish(new NavigateMessage(page));
        }

        private static void FundList()
        {
            var page = new FundsList();
            MessageBus.Instance.Publish(new NavigateMessage(page));
        }

        private static void NPDList()
        {
            var page = new NPDList();
            MessageBus.Instance.Publish(new NavigateMessage(page));
        }

        private static void ConditionsList()
        {
            var page = new Conditions();
            MessageBus.Instance.Publish(new NavigateMessage(page));
        }

        private static void FcMgmtList()
        {
            var page = new DPFCMgmtList();
            MessageBus.Instance.Publish(new NavigateMessage(page));
        }

        #endregion

        //Special case method for Robs as they have more complex naviation.
        public static void RobSpecialListScreen(string appTypeId, List<string> initallyExpandedDeatilsList)
        {
            var page = new EventsPage(appTypeId, initallyExpandedDeatilsList);
            MessageBus.Instance.Publish(new NavigateMessage(page));
            CheckNavigation(null, null);
        }
    }
}
