using System.Xml.Linq;
using Model.DataAccess.Generic;
using Model.Entity;
using Telerik.Windows.Controls;
using WPF.Navigation;
using WPF.UserControls.Filters.ViewModels;
using WPF.UserControls.Tabs.Models;
using WPF.UserControls.Tabs.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Exceedra.Common;
using Exceedra.Common.Xml;
using Exceedra.Controls.Messages;
using Model;
using Model.DataAccess;
using ViewHelper;
using WPF;
using Model.Enumerators;

namespace ViewModels
{
    public class EventsViewModel : BaseViewModel
    {
        public Visibility CreateRobVisibility { get; set; }
        public Visibility CopyRobVisibility { get; set; }
        public Visibility DeleteRobVisibility { get; set; }
        public Visibility ContractsVisibility { get; set; }
        public Visibility CreateContractVisibility { get; set; }
        public Visibility CopyContractVisibility { get; set; }
        public Visibility RemoveContractVisibility { get; set; }
        public Visibility SkuDetailTabVisibility { get; set; }
        public Visibility ScheduleTabVisibility { get; set; }
        public Visibility GroupCreatorVisibility { get; set; }

        private readonly RobAccess _access;
        private readonly GroupListAccess _groupAccess = new GroupListAccess();

        public RadTabItem SenderTab;
        public List<string> InitallyExpandedDeatilsList = new List<string>();

        public EventsViewModel()
        {
        }

        private string _thisRobKey;
        public EventsViewModel(string appTypeId, string pageTitle)
        {
            AppTypeIdx = appTypeId;
            PageTitle = pageTitle;
            _access = new RobAccess(AppTypeIdx);
             _thisRobKey = App.Configuration.GetScreens().FirstOrDefault(s => s.RobAppType == AppTypeIdx).Key;

            // todo: ### CONSTRAINTS TO REFACTOR
            switch (_thisRobKey)
            {
                case "ROB_TERMS":
                    {
                        CreateRobVisibility = App.Configuration.IsCreateTermsActive ? Visibility.Visible : Visibility.Collapsed;
                        CopyRobVisibility = App.Configuration.IsCopyTermsActive ? Visibility.Visible : Visibility.Collapsed;
                        DeleteRobVisibility = App.Configuration.IsDeleteTermsActive ? Visibility.Visible : Visibility.Collapsed;
                        ContractsVisibility = App.Configuration.IsTermContractsActive ? Visibility.Visible : Visibility.Collapsed;
                        CreateContractVisibility = App.Configuration.IsCreateTermContractsActive ? Visibility.Visible : Visibility.Collapsed;
                        CopyContractVisibility = App.Configuration.IsCopyTermContractsActive ? Visibility.Visible : Visibility.Collapsed;
                        RemoveContractVisibility = App.Configuration.IsRemoveTermContractsActive ? Visibility.Visible : Visibility.Collapsed;
                        SkuDetailTabVisibility = App.Configuration.IsSkuDetailTermsActive ? Visibility.Visible : Visibility.Collapsed;
                        ScheduleTabVisibility = App.Configuration.IsScheduleTermsActive ? Visibility.Visible : Visibility.Collapsed;
                        DefaultTabName = ClientConfiguration.TermsDefaultTab;
                        GroupCreatorVisibility = App.Configuration.IsGroupCreatorVisibleInTerms ? Visibility.Visible : Visibility.Collapsed;
                        break;
                    }
                case "ROB_RISK_OPS":
                    {
                        CreateRobVisibility = App.Configuration.IsCopyRiskOpsActive ? Visibility.Visible : Visibility.Collapsed;
                        CopyRobVisibility = App.Configuration.IsCopyRiskOpsActive ? Visibility.Visible : Visibility.Collapsed;
                        DeleteRobVisibility = App.Configuration.IsDeleteRiskOpsActive ? Visibility.Visible : Visibility.Collapsed;
                        ContractsVisibility = App.Configuration.IsRiskOpsContractsActive ? Visibility.Visible : Visibility.Collapsed;
                        CreateContractVisibility = App.Configuration.IsCreateRiskOpsContractsActive ? Visibility.Visible : Visibility.Collapsed;
                        CopyContractVisibility = App.Configuration.IsCopyRiskOpsContractsActive ? Visibility.Visible : Visibility.Collapsed;
                        RemoveContractVisibility = App.Configuration.IsRemoveRiskOpsContractsActive ? Visibility.Visible : Visibility.Collapsed;
                        SkuDetailTabVisibility = App.Configuration.IsSkuDetailRiskOpsActive ? Visibility.Visible : Visibility.Collapsed;
                        ScheduleTabVisibility = App.Configuration.IsScheduleRiskOpsActive ? Visibility.Visible : Visibility.Collapsed;
                        DefaultTabName = ClientConfiguration.RiskOpsDefaultTab;
                        GroupCreatorVisibility = App.Configuration.IsGroupCreatorVisibleInRiskOps ? Visibility.Visible : Visibility.Collapsed;
                        break;
                    }
                case "ROB_MARKETING":
                    {
                        CreateRobVisibility = App.Configuration.IsCreateMarketingActive ? Visibility.Visible : Visibility.Collapsed;
                        CopyRobVisibility = App.Configuration.IsCopyMarketingActive ? Visibility.Visible : Visibility.Collapsed;
                        DeleteRobVisibility = App.Configuration.IsDeleteMarketingActive ? Visibility.Visible : Visibility.Collapsed;
                        ContractsVisibility = App.Configuration.IsMarketingContractsActive ? Visibility.Visible : Visibility.Collapsed;
                        CreateContractVisibility = App.Configuration.IsCreateMarketingContractsActive ? Visibility.Visible : Visibility.Collapsed;
                        CopyContractVisibility = App.Configuration.IsCopyMarketingContractsActive ? Visibility.Visible : Visibility.Collapsed;
                        RemoveContractVisibility = App.Configuration.IsRemoveMarketingContractsActive ? Visibility.Visible : Visibility.Collapsed;
                        SkuDetailTabVisibility = App.Configuration.IsSkuDetailMarketingActive ? Visibility.Visible : Visibility.Collapsed;
                        ScheduleTabVisibility = App.Configuration.IsScheduleMarketingActive ? Visibility.Visible : Visibility.Collapsed;
                        DefaultTabName = ClientConfiguration.MarketingDefaultTab;
                        GroupCreatorVisibility = App.Configuration.IsGroupCreatorVisibileInMarketing ? Visibility.Visible : Visibility.Collapsed;
                        break;
                    }
                case "ROB_MANAGEMENTADJUST":
                    {
                        CreateRobVisibility = App.Configuration.IsCreateManagementAdjustActive ? Visibility.Visible : Visibility.Collapsed;
                        CopyRobVisibility = App.Configuration.IsCopyManagementAdjustActive ? Visibility.Visible : Visibility.Collapsed;
                        DeleteRobVisibility = App.Configuration.IsDeleteManagementAdjustActive ? Visibility.Visible : Visibility.Collapsed;
                        ContractsVisibility = App.Configuration.IsManagementAdjustContractsActive ? Visibility.Visible : Visibility.Collapsed;
                        CreateContractVisibility = App.Configuration.IsCreateManagementAdjustContractsActive ? Visibility.Visible : Visibility.Collapsed;
                        CopyContractVisibility = App.Configuration.IsCopyManagementAdjustContractsActive ? Visibility.Visible : Visibility.Collapsed;
                        RemoveContractVisibility = App.Configuration.IsRemoveManagementAdjustContractsActive ? Visibility.Visible : Visibility.Collapsed;
                        SkuDetailTabVisibility = App.Configuration.IsSkuDetailManagementAdjustActive ? Visibility.Visible : Visibility.Collapsed;
                        ScheduleTabVisibility = App.Configuration.IsScheduleManagementAdjustActive ? Visibility.Visible : Visibility.Collapsed;
                        DefaultTabName = ClientConfiguration.ManagementAdjustDefaultTab;
                        GroupCreatorVisibility = App.Configuration.IsGroupCreatorVisibileInManagementAdjut ? Visibility.Visible : Visibility.Collapsed;
                        break;
                    }
                case "ROB_TARGET":
                    {
                        CreateRobVisibility = App.Configuration.IsCreateTargetActive ? Visibility.Visible : Visibility.Collapsed;
                        CopyRobVisibility = App.Configuration.IsCopyTargetActive ? Visibility.Visible : Visibility.Collapsed;
                        DeleteRobVisibility = App.Configuration.IsDeleteTargetActive ? Visibility.Visible : Visibility.Collapsed;
                        ContractsVisibility = App.Configuration.IsTargetContractsActive ? Visibility.Visible : Visibility.Collapsed;
                        CreateContractVisibility = App.Configuration.IsCreateTargetContractsActive ? Visibility.Visible : Visibility.Collapsed;
                        CopyContractVisibility = App.Configuration.IsCopyTargetContractsActive ? Visibility.Visible : Visibility.Collapsed;
                        RemoveContractVisibility = App.Configuration.IsRemoveTargetContractsActive ? Visibility.Visible : Visibility.Collapsed;
                        SkuDetailTabVisibility = App.Configuration.IsSkuDetailTargetActive ? Visibility.Visible : Visibility.Collapsed;
                        ScheduleTabVisibility = App.Configuration.IsScheduleTargetActive ? Visibility.Visible : Visibility.Collapsed;
                        DefaultTabName = ClientConfiguration.TargetDefaultTab;
                        GroupCreatorVisibility = App.Configuration.IsGroupCreatorVisibileInTarget ? Visibility.Visible : Visibility.Collapsed;
                        break;
                    }
            }

            // / ### CONSTRAINTS TO REFACTOR
            LoadFilters();
        }
        
        public string DefaultTabName { get; set; }

        public string PageTitle { get; set; }

        private string AppTypeIdx { get; set; }

        #region Commands

        public ICommand AddRobCommand
        {
            get { return new ViewCommand(AddRob); }
        }

        public ICommand ShowGroupCreatorCommand
        {
            get { return new ViewCommand(GotoGroupCreator); }
        }

        public ICommand AddRobGroupCommand
        {
            get { return new ViewCommand(AddRobGroup); }
        }

        public ICommand CopyRobCommand
        {
            get { return new ViewCommand(ValidateRemoveCopy, CopyRob); }
        }
       
        public ICommand RemoveRobCommand
        {
            get { return new ViewCommand(ValidateRemoveCopy, RemoveRob); }
        }

        public ICommand UpdateMultipleStatusCommand
        {
            get { return new ViewCommand(CanUpdateStatus, UpdateMultipleStatus); }
        }

        public ICommand AddContractCommand
        {
            get { return new ViewCommand(AddContract); }
        }

        private void AddContract(object sender)
        {            
            RedirectMe.Goto("contract", "", "", AppTypeIdx, TabData.SelectedTab.TabName);
        }

        private void AddRob(object sender)
        {
            RedirectMe.Goto("rob", "", "", AppTypeIdx, TabData.SelectedTab.TabName);
        }

        private void AddRobGroup(object sender)
        {
            RedirectMe.Goto("robgroup", "", "", AppTypeIdx, TabData.SelectedTab.TabName);
        }

        private void GotoGroupCreator(object sender)
        {
            RedirectMe.Goto("robgroupcreator", "", "", AppTypeIdx, TabData.SelectedTab.TabName);
        }

        private void CopyRob(object obj)
        {
            if (TabData.SelectedTab.TabName == materialRobTabName)
            {
                if (_groupAccess.CopyGroup(AppTypeIdx, TabData.SelectedTab.GetSelectedItems()))
                    FiltersVM.ApplyFilter();
            }
            else if (TabData.SelectedTab.TabName == contractsTabName)
            {
                if (_access.CopyContract(TabData.SelectedTab.GetSelectedItems()))
                    FiltersVM.ApplyFilter();
            }
            else if (TabData.SelectedTab.TabName == mainTabName)
            {
                if (_access.CopyRob(AppTypeIdx, TabData.SelectedTab.GetSelectedItems()))
                    FiltersVM.ApplyFilter();
            }
            else throw new Exception("Not able to recognise the selected tab name");
        }

        private void RemoveRob(object obj)
        {
            if (TabData.SelectedTab.TabName == materialRobTabName)
            {
                if (_groupAccess.RemoveGroup(AppTypeIdx, TabData.SelectedTab.GetSelectedItems()))
                    FiltersVM.ApplyFilter();

            }
            else if (TabData.SelectedTab.TabName == contractsTabName)
            {
                if (_access.DeleteContract(TabData.SelectedTab.GetSelectedItems()))
                    FiltersVM.ApplyFilter();
            }
            else if (TabData.SelectedTab.TabName == mainTabName)
            {
                if (_access.RemoveRob(TabData.SelectedTab.GetSelectedItems()))
                    FiltersVM.ApplyFilter();
            }
            else throw new Exception("Not able to recognise the selected tab name");
        }

        private void UpdateMultipleStatus(object obj)
        {
            string message = string.Format("Are you sure you want to set selected to {0} status?",
                                                  TabData.GetDropdown("UpdateStatusDropdown").Items.SelectedItem.Name);

            if (CustomMessageBox.Show(message, "Warning!", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            if (TabData.SelectedTab.TabName == contractsTabName || TabData.SelectedTab.TabName == materialRobTabName)
            {
                if (_access.UpdateContractStatuses(TabData.SelectedTab.GetSelectedItems(),
                    TabData.GetDropdown("UpdateStatusDropdown").Items.SelectedItem.Idx))
                    FiltersVM.ApplyFilter();
            }
            else
            {
                if (_access.UpdateStatusRobs(TabData.SelectedTab.GetSelectedItems(),
                    TabData.GetDropdown("UpdateStatusDropdown").Items.SelectedItem.Idx, AppTypeIdx))
                    FiltersVM.ApplyFilter();
            }
        }

        public bool ValidateRemoveCopy(object obj)
        {
            return CanCopyCloseDelete;
        }

        public bool CanUpdateStatus(object obj)
        {
            return (CanCopyCloseDelete && TabData.GetDropdown("UpdateStatusDropdown").Items.SelectedItem != null);
        }

        public bool CanCopyCloseDelete
        {
            get
            {
                return TabData.SelectedTab.AreItemsSelected(null);
            }
        }        

        #endregion
        
        #region Filters

        private void LoadFilters()
        {
            SetupTabs();

            var appTypeIdxElement = XElement.Parse("<AppType_Idx>" + AppTypeIdx + "</AppType_Idx>");
            
            var datesXml = CommonXml.GetBaseArguments("GetFilterDates");
            datesXml.Add(appTypeIdxElement);

            var statusXml = CommonXml.GetBaseArguments("GetFilterStatuses");
            statusXml.Add(appTypeIdxElement);
 

            FiltersVM = new FilterViewModel
            {
                ApplyFilter = TestApply,

                SaveExtraArguments = appTypeIdxElement,
                CurrentScreenKey = (ScreenKeys)Enum.Parse(typeof(ScreenKeys), _thisRobKey),
                SingleTreeArguments = statusXml,
                StatusTreeProc = _access.GetFilterStatusProc(),
                DateArguments = datesXml,
                DatesProc = _access.GetFilterDatesProc()
            };

            FiltersVM.Load();
        }

        private FilterViewModel _filtersVM;
        public FilterViewModel FiltersVM
        {
            get
            {
                return _filtersVM;
            }
            set
            {
                _filtersVM = value;
                NotifyPropertyChanged(this, vm => vm.FiltersVM);
            }
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

        private string mainTabName = "ROBs";
        private string materialRobTabName = "MaterialRobs";
        private string contractsTabName = "Contracts";
        private string skuDetailsTabName = "SkuDetails";
        private string scheduleTabName = "Schedule";

        private void SetupTabs()
        {
            var tabList = new List<Tab>();

            Tab mainTab = new Tab
            {
                TabName = mainTabName,
                TabTitle = PageTitle ?? DefaultTabName,
                TabType = "HorizontalGrid",
                TabMainContentProc = _access.GetRobsProc(),
                ApplyRootXml = "GetRobs",
                AdditionalInputXml = new XElement(XMLNode.Nodes.AppType_Idx.ToString(), AppTypeIdx)
            };
            tabList.Add(mainTab);

            Tab materialRobsTab = new Tab
            {
                TabName = materialRobTabName,
                TabTitle = App.CurrentLang.GetValue("Label_Material" + PageTitle, "Material " + PageTitle),
                TabType = "HorizontalGrid",
                TabMainContentProc = _access.GetMaterialRobsProc(),
                ApplyRootXml = "GetMaterialRobs",
                AdditionalInputXml = new XElement(XMLNode.Nodes.AppType_Idx.ToString(), AppTypeIdx)
            };
            tabList.Add(materialRobsTab);

            if (ContractsVisibility == Visibility.Visible)
            {
                Tab contractsTab = new Tab
                {
                    TabName = contractsTabName,
                    TabTitle = App.CurrentLang.GetValue("Label_Contracts", "Contracts"),
                    TabType = "HorizontalGrid",
                    TabMainContentProc = _access.GetContractsProc(),
                    ApplyRootXml = "GetContracts",
                    AdditionalInputXml = new XElement(XMLNode.Nodes.AppType_Idx.ToString(), AppTypeIdx)
                };
                tabList.Add(contractsTab);
            }

            if (SkuDetailTabVisibility == Visibility.Visible)
            {
                Tab skuDetailsTab = new Tab
                {
                    TabName = skuDetailsTabName,
                    TabTitle = App.CurrentLang.GetValue("Label_SkuDetail", "SKU Detail"),
                    TabType = "HorizontalGrid",
                    TabMainContentProc = _access.GetSkuDetailsProc(),
                    ApplyRootXml = "GetItems",
                    AdditionalInputXml = new XElement(XMLNode.Nodes.AppType_Idx.ToString(), AppTypeIdx)
                };

                tabList.Add(skuDetailsTab);
            }

            if (ScheduleTabVisibility == Visibility.Visible)
            {
                Tab scheduleTab = new Tab
                {
                    TabName = scheduleTabName,
                    TabTitle = PageTitle + " Schedule",
                    TabType = "ScheduleGrid",
                    TabMainContentProc = _access.GetRobsProc(),
                    ApplyRootXml = "GetRobs",
                    AdditionalInputXml = new XElement(XMLNode.Nodes.AppType_Idx.ToString(), AppTypeIdx)
                };

                tabList.Add(scheduleTab);
            }

            var dropdowns = new List<DropdownCollection>
            {
                new DropdownCollection {DropdownProc = _access.DropdownStatusProc(), Name = "UpdateStatusDropdown", 
                    Label = App.CurrentLang.GetValue("Label_UpdateStatuses", "Update Statuses"), DropdownCommand = UpdateMultipleStatusCommand, AppTypeIdx = AppTypeIdx},
            };

            var buttons = new List<ButtonCollection>
            {
                new ButtonCollection {Label = App.CurrentLang.GetValue("Button_NewSingle", "New (Single)"), ButtonStyle = StyleType.Success, ButtonCommand = AddRobCommand, IsVisible = CreateRobVisibility},
                new ButtonCollection {Label = App.CurrentLang.GetValue("Button_NewGroup", "New (Group)"), ButtonStyle = StyleType.Success, ButtonCommand = ShowGroupCreatorCommand, IsVisible = GroupCreatorVisibility},
                new ButtonCollection {Label = App.CurrentLang.GetValue("Button_NewContract", "New Contract"), ButtonStyle = StyleType.Success, ButtonCommand = AddContractCommand, IsVisible = CreateContractVisibility},
                new ButtonCollection {Label = App.CurrentLang.GetValue("Button_Copy", "Copy"), ButtonStyle = StyleType.Warning, ButtonCommand = CopyRobCommand, IsVisible = CopyRobVisibility},
                new ButtonCollection {Label = App.CurrentLang.GetValue("Button_Remove", "Remove"), ButtonStyle = StyleType.Danger, ButtonCommand = RemoveRobCommand, IsVisible = DeleteRobVisibility}
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

            TabData.LoadContent();
        }

        //public XElement GetFilterXml(string rootNode)
        //{
        //    var root = String.IsNullOrWhiteSpace(rootNode) ? "GetRobs" : rootNode;
        //    return FiltersVM.GetFiltersAsXml(root);
        //}

        public XElement GetFilterXml(string rootNode)
        {
            var root = String.IsNullOrWhiteSpace(rootNode) ? "GetRobs" : rootNode;
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

        #endregion


    }





}
