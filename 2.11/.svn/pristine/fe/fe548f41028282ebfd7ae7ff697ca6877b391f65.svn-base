using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Model;
using Model.DataAccess;
using ViewHelper;
using Exceedra.Controls.DynamicGrid.ViewModels;
using System.Xml.Linq;
using Exceedra.Common;
using Exceedra.Controls.Messages;
using Model.DataAccess.Generic;
using Model.Entity;
using Telerik.Windows.Controls;
using WPF.Navigation;
using WPF.UserControls.Filters.ViewModels;
using WPF.UserControls.Tabs.Models;
using WPF.UserControls.Tabs.ViewModels;
using ViewModelBase = ViewModels.ViewModelBase;
using Model.Enumerators;

namespace WPF.ViewModels.Scenarios
{
    public class ScenarioMainViewModel : ViewModelBase
    {
        private readonly IScenarioListAccess _scenarioDataAccess = new ScenarioAccess();
        public RadTabItem SenderTab;

        private ScenarioMainViewModel()
        {
      
        }

        public static ScenarioMainViewModel New()
        {
            var instance = new ScenarioMainViewModel();
            instance.Init();
            return instance;
        }

        private void Init()
        {
            LoadFilters();
        }

        private void LoadFilters()
        {
            SetupTabs();
            var datesXml = CommonXml.GetBaseArguments("GetPromotionDates");
            FiltersVM = new FilterViewModel
            {
                ApplyFilter = TestApply,
                CurrentScreenKey = ScreenKeys.SCENARIO,
                SingleTreeArguments = CommonXml.GetBaseArguments("GetStatuses"),
                StatusTreeProc = _scenarioDataAccess.FilterStatusProc,
                DateArguments = datesXml,
                DatesProc = _scenarioDataAccess.FilterDatesProc             
            };

            FiltersVM.Load();
        }

        private FilterViewModel _filtersVM;

        public FilterViewModel FiltersVM
        {
            get { return _filtersVM; }
            set
            {
                _filtersVM = value;
                NotifyPropertyChanged(this, vm => vm.FiltersVM);
            }
        }

        public bool CanCopyCloseDelete
        {
            get { return TabData.SelectedTab.AreItemsSelected(null); }
        }

        private bool _canBudgetCloseDelete;
        public bool CanBudgetCloseDelete
        {
            get
            {

                if (GetScenarioTabContent != null && GetScenarioTabContent.Records != null)
                {
                    var s = GetScenarioTabContent.Records.Select(r => r.CheckedItems(ActiveBudgetColumn)).ToList();
                    Dictionary<string, string> checkList = new Dictionary<string, string>();
                    foreach (var thisVar in s.Where(r => r.HasValue() == true))
                    {
                        checkList.Add(thisVar, thisVar);
                    }

                    if (CurrentSelectedBudgets != null && checkList.Count() == CurrentSelectedBudgets.Count())
                    {
                        int checkCount = 0;
                        foreach (var checkListKeyWord in checkList)
                        {
                            if (CurrentSelectedBudgets.ContainsKey(checkListKeyWord.Key) == true)
                            {
                                checkCount = checkCount + 1;
                            }
                        }
                        if (checkCount == checkList.Count())
                        {
                            _canBudgetCloseDelete = false;
                        }
                        else
                        {
                            _canBudgetCloseDelete = true;
                        }
                    }
                    else
                    {
                        _canBudgetCloseDelete = true;
                    }
                }
                else { _canBudgetCloseDelete = false; }

                return _canBudgetCloseDelete;
            }

        }

        public IEnumerable<string> ActiveBudgetScenarios
        {
            get
            {
                return ((RecordViewModel)TabData.GetTab("Scenarios").TabMainContent).Records.Where(r => r.CheckedItem(ActiveBudgetColumn) == true).Select(r => r.Item_Idx).ToList();                
            }
        }

        /// <summary>
        /// Save any checked AS 'Active Budget' scenarios
        /// </summary>
        /// <param name="obj"></param>
        private void SaveActiveBudget(object obj)
        {
            if (ActiveBudgetScenarios == null)
            {
                CustomMessageBox.Show("No active budgets set", "Active budgets", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var lookup = ((RecordViewModel)TabData.GetTab("Scenarios").TabMainContent).Records.ToDictionary(x => Convert.ToInt32(x.Item_Idx), x => x.CheckedItem(ActiveBudgetColumn));
            _scenarioDataAccess.SaveActiveBudgets(lookup, User.CurrentUser.SalesOrganisationID.ToString(), FiltersVM.StartDate, FiltersVM.EndDate, obj)
              .ContinueWith(td =>
              {
                  var result = td.Result;
                  if (result.Contains("Success"))
                  {
                      MessageBoxShow("Active budgets set.");
                      FiltersVM.ApplyFilter();
                  }
                  else
                  {
                      var x = XElement.Parse(result);
                      var msg = x.Value;
                      MessageBoxShow(msg);
                  }


              }, App.Scheduler);
        }

        private void CloseScenarios(object obj)
        {
            _scenarioDataAccess.CloseScenarios(TabData.SelectedTab.GetSelectedItems())
                .ContinueWith(td =>
                {
                    var result = td.Result;
                    if (result.Contains("Results"))
                    {
                        var x = XElement.Parse(result);
                        var msg = x.Value;
                        MessageBoxShow(msg);
                        FiltersVM.ApplyFilter();
                    }
                    else
                    {
                        var x = XElement.Parse(result);
                        var msg = x.Value;
                        MessageBoxShow(msg);
                    }

                }, App.Scheduler);
        }

        private void CopyScenarios(object obj)
        {
            _scenarioDataAccess.CopyScenarios(TabData.SelectedTab.GetSelectedItems())
                .ContinueWith(td =>
                {
                    if (td != null && td.Result != null)
                    {
                        var result = td.Result;
                        var x = XElement.Parse(result);
                        var msg = x.Value;
                        MessageBoxShow(msg);
                        FiltersVM.ApplyFilter();
                    }

                }, App.Scheduler);
        }

        private void LastClosedScenarios(object obj)
        {
            _scenarioDataAccess.LastClosedScenarios(TabData.GetDropdown("SetLastClosedDropdown").Date)
                .ContinueWith(td =>
                {
                    var result = td.Result;
                    var x = XElement.Parse(result);
                    var msg = "Set last date processed: " + x.Value;
                    MessageBoxShow(msg);
                    FiltersVM.ApplyFilter();
                }, App.Scheduler);
        }
        
        private void AddScenario(object obj)
        {
            RedirectMe.Goto("scenario", "", "", "", TabData.SelectedTab.TabName);
        }

        private void RemoveScenarios(object obj)
        {
            const string message = "Are you sure you want to delete the selected scenarios?";

            if (CustomMessageBox.Show(message, "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Warning) ==
                MessageBoxResult.No)
                return;


            _scenarioDataAccess.DeleteScenarios(TabData.SelectedTab.GetSelectedItems())
                .ContinueWith(td =>
                {
                    var result = td.Result;
                    if (result.Contains("Scheduled"))
                    {
                        MessageBoxShow("Selected scenarios scheduled for deletion.");
                        FiltersVM.ApplyFilter();
                    }
                    else
                    {
                        var x = XElement.Parse(result);
                        var msg = x.Value;
                        MessageBoxShow(msg);
                    }

                }, App.Scheduler);
        }

        private void CreateWorkingScenario(object obj)
        {
            _scenarioDataAccess.CreateWorkingScenario(TabData.SelectedTab.GetSelectedItems())
                .ContinueWith(td =>
                {
                    var result = td.Result;
                    var x = XElement.Parse(result);
                    var msg = x.Value;
                    MessageBoxShow(msg);
                    FiltersVM.ApplyFilter();

                }, App.Scheduler);

        }

        private bool GenericActionScenario(object obj)
        {
            return CanCopyCloseDelete;
        }

        private bool BudgetActionScenario(object obj)
        {
            return CanBudgetCloseDelete;
        }

        public Dictionary<string, string> CurrentSelectedBudgets { get; set; }

        private void SetSelectedBudget()
        {
            var budgets = new Dictionary<string, string>();

            if (GetScenarioTabContent.Records != null)
                foreach (var record in GetScenarioTabContent.Records)
            {
                if (record.CheckedItems(ActiveBudgetColumn).IsEmpty() == false)
                {
                    budgets.Add(record.Item_Idx, record.CheckedItems(ActiveBudgetColumn));
                }

                record.CheckedItems("");
            }
            CurrentSelectedBudgets = budgets;
        }

        public string ActiveBudgetColumn { get { return "IsActiveBudget"; } }

        #region Commands

        public Visibility ScheduleTabVisibility
        {
            get { return App.Configuration.IsScenarioScheduleActive ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility CreateScenarioVisibility
        {
            get { return App.Configuration.IsCreateScenarioActive ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility CopyScenarioVisibility
        {
            get { return App.Configuration.IsCopyScenarioActive ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility DeleteScenarioVisibility
        {
            get { return App.Configuration.IsDeleteScenarioActive ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility CloseScenarioVisibility
        {
            get { return App.Configuration.IsCloseScenarioActive ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility ExportScenarioVisibility
        {
            get { return App.Configuration.IsExportScenarioActive ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility UpdateBudgetScenarioVisibility
        {
            get { return App.Configuration.IsUpdateBudgetScenarioActive ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility EditLastClosedScenarioVisibility
        {
            get { return App.Configuration.IsEditLastClosedScenarioActive ? Visibility.Visible : Visibility.Collapsed; }
        }

        public ICommand AddScenarioCommand
        {
            get { return new ViewCommand(AddScenario); }
        }

        public ICommand RemoveScenarioCommand
        {
            get { return new ViewCommand(GenericActionScenario, RemoveScenarios); }
        }

        public ICommand CreateWorkingScenarioCommand
        {
            get { return new ViewCommand(GenericActionScenario, CreateWorkingScenario); }
        }

        public ICommand SaveActiveBudgetCommand
        {
            get { return new ViewCommand(BudgetActionScenario, SaveActiveBudget); }
        }

        public ICommand CloseScenarioCommand
        {
            get { return new ViewCommand(GenericActionScenario, CloseScenarios); }
        }

        public ICommand CopyScenarioCommand
        {
            get { return new ViewCommand(GenericActionScenario, CopyScenarios); }
        }

        public ICommand LastClosedScenarioCommand
        {
            get { return new ViewCommand(LastClosedScenarios); }
        }

        #endregion
        
        #region Tabs

        private TabViewModel _tabData;
        public TabViewModel TabData
        {
            get { return _tabData; }
            set
            {
                _tabData = value;
                NotifyPropertyChanged(this, vm => vm.TabData);
            }
        }

        private void SetupTabs()
        {
            var tabList = new List<Tab>();

            Tab mainTab = new Tab
            {
                TabName = "Scenarios",
                TabTitle = App.CurrentLang.GetValue("Label_ScenariosTabTitle", "Scenarios"),
                TabType = "HorizontalGrid",
                TabMainContentProc = _scenarioDataAccess.GetScenariosProc(),
                ApplyRootXml = "GetScenarios",
            };
            tabList.Add(mainTab);

            if (ScheduleTabVisibility == Visibility.Visible)
            {
                Tab scheduleTab = new Tab
                {
                    TabName = "Schedule",
                    TabTitle = App.CurrentLang.GetValue("Label_ScenariosTabTitle", "Scenarios") + " Schedule",
                    TabType = "ScheduleGrid",
                    TabMainContentProc = _scenarioDataAccess.GetScenariosProc(),
                    ApplyRootXml = "GetScenarios",
                };

                tabList.Add(scheduleTab);
            }

            var buttons = new List<ButtonCollection>
            {
                new ButtonCollection {Label = App.CurrentLang.GetValue("Button_New", "New"), ButtonStyle = StyleType.Success, ButtonCommand = AddScenarioCommand},
                new ButtonCollection {Label = App.CurrentLang.GetValue("Button_Copy","Copy"), ButtonStyle = StyleType.Warning, ButtonCommand = CopyScenarioCommand, IsVisible = CopyScenarioVisibility},
                new ButtonCollection {Label = App.CurrentLang.GetValue("Button_Close","Close"), ButtonStyle = StyleType.Warning, ButtonCommand = CloseScenarioCommand, IsVisible = CloseScenarioVisibility},
                new ButtonCollection {Label = App.CurrentLang.GetValue("Button_Delete","Delete"), ButtonStyle = StyleType.Danger, ButtonCommand = RemoveScenarioCommand, IsVisible = DeleteScenarioVisibility},
                new ButtonCollection {Label = App.CurrentLang.GetValue("Button_CreateWorkingScenario","Create Working Scenario"), ButtonStyle = StyleType.Secondary, ButtonCommand = CreateWorkingScenarioCommand},
                new ButtonCollection {Label = App.CurrentLang.GetValue("Button_UpdateBudgets","Update Budgets"), ButtonStyle = StyleType.Info, ButtonCommand = SaveActiveBudgetCommand, IsVisible = UpdateBudgetScenarioVisibility},
            };

            var dropdowns = new List<DropdownCollection>
            {
                new DropdownCollection {DropdownProc = _scenarioDataAccess.GetLastDateProc(), IsDatePicker = true, Name = "SetLastClosedDropdown", Label = App.CurrentLang.GetValue("Label_SetLastClosed", "Set Last Closed"), IsVisible = EditLastClosedScenarioVisibility, DropdownCommand = LastClosedScenarioCommand},
            };


            TabData = new TabViewModel(tabList)
            {
                Dropdowns = dropdowns,
                Buttons = buttons,
                GetFilterXml = GetFilterXml
            };

            TabData.LoadContent();
        }



        private void TestApply()
        {
            if (!FiltersVM.CanApplyFilter(null)) return;

            var tabTasks = TabData.LoadContent();

            Task.Factory.ContinueWhenAll(tabTasks.ToArray(), t => SetSelectedBudget());
        }

        //public XElement GetFilterXml(string rootNode)
        //{
        //    var root = String.IsNullOrWhiteSpace(rootNode) ? "GetScenarios" : rootNode;
        //    return CommonXml.ConvertToOldStyle(FiltersVM.GetFiltersAsXml(root));
        //}

        public XElement GetFilterXml(string rootNode)
        {
            var root = String.IsNullOrWhiteSpace(rootNode) ? "GetScenarios" : rootNode;
            if (FiltersVM != null && FiltersVM.ApplyFilterCommand.CanExecute(null))
            {
                return FiltersVM.GetFiltersAsXml(root);
            }
            else
            {
                var x = CommonXml.GetBaseArguments(root);
                x.AddElement("LoadFromDefaults", "1");

                return x;
            }

        }

        #region helper methods for getting data

        private RecordViewModel GetScenarioTabContent
        {
            get { return (RecordViewModel) TabData.GetTab("Scenarios").TabMainContent; }
        }

        #endregion


        #endregion

    }
}
