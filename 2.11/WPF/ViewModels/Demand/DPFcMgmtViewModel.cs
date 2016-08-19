using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using Exceedra.Common;
using Model;
using Model.DataAccess.Converters;
using Model.DataAccess.Generic;
using Telerik.Windows.Controls;
using ViewHelper;
using WPF.UserControls.Filters.ViewModels;
using ViewModelBase = ViewModels.ViewModelBase;
using Model.Entity;
using WPF.Navigation;
using WPF.UserControls.Tabs.Models;
using WPF.UserControls.Tabs.ViewModels;
using Model.Enumerators;

namespace WPF.ViewModels.Demand
{
    public class DPFcMgmtViewModel : ViewModelBase
    {
        public RadTabItem SenderTab;

        public DPFcMgmtViewModel()
        {
            LoadFilters();
        }

        #region Loaders

        private void LoadFilters()
        {
            SetupTabs();

            FiltersVM = new FilterViewModel
            {
                ApplyFilter = Apply,
                CurrentScreenKey = ScreenKeys.FCMGMT,
                IsUsingOtherFilters = true,
                NoSingleTree = true
            };

            FiltersVM.Load();
        }


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
                TabName = "TrialForecasts",
                TabTitle = App.CurrentLang.GetValue("Label_TrailForecasts", "Trial Forecasts"),
                TabType = "HorizontalGrid",
                TabMainContentProc = StoredProcedure.DemandMgmt.GetTrialForecasts,
                ApplyRootXml = "GetData",
            };
            tabList.Add(mainTab);


            var dropdowns = new List<DropdownCollection>
            {
                new DropdownCollection {Name = "PublishDropdown", Label = App.CurrentLang.GetValue("Label_Publish", "Publish"), DropdownCommand = PublishCommand, AdditionalLoadXml = XElement.Parse("<ROOT><Screen_Code>FCMGMT</Screen_Code></ROOT>"), DropdownProc = StoredProcedure.DemandMgmt.GetScenarios, DoNotCache = true},
            };

            var buttons = new List<ButtonCollection>
            {
                new ButtonCollection {Label = App.CurrentLang.GetValue("Button_New", "New"), ButtonStyle = StyleType.Success, ButtonCommand = NewCommand, IsVisible = NewVisibility},
                new ButtonCollection {Label = App.CurrentLang.GetValue("Button_Copy", "Copy"), ButtonStyle = StyleType.Warning, ButtonCommand = CopyCommand, IsVisible = CopyVisibility},
                new ButtonCollection {Label = App.CurrentLang.GetValue("Button_Remove", "Remove"), ButtonStyle = StyleType.Danger, ButtonCommand = DeleteCommand, IsVisible = DeleteVisibility},
                new ButtonCollection {Label = App.CurrentLang.GetValue("Button_Bulk", "Start Bulk Forecast"), ButtonStyle = StyleType.Info, ButtonCommand = StartBulkCommand, IsVisible = BulkVisibility}
            };

            TabData = new TabViewModel(tabList)
            {
                Dropdowns = dropdowns,
                Buttons = buttons,
                GetFilterXml = GetFilterXml
            };

            TabData.LoadContent();
        }

        public XElement GetFilterXml(string rootNode)
        {
            var root = String.IsNullOrWhiteSpace(rootNode) ? "GetForecasts" : rootNode;
            if (FiltersVM != null && FiltersVM.ApplyFilterCommand.CanExecute(null))
            {
                return FiltersVM.GetFiltersAsXml(root);
            }
            
            var x = CommonXml.GetBaseArguments(root);
            x.AddElement("LoadFromDefaults", "1");
            return x;           
        }

        private void Apply()
        {
            TabData.LoadContent();
        }

        #endregion

        //#endregion

        #region Properties

        private FilterViewModel _filterVM;
        public FilterViewModel FiltersVM
        {
            get { return _filterVM; }
            set
            {
                _filterVM = value;
                NotifyPropertyChanged(this, vm => vm.FiltersVM);
            }
        }

        #endregion

        #region Commands

        private void NewFc(object o)
        {
            RedirectMe.Goto("FCMGMT");
        }

        private void CopyFc(object o)
        {
            if (MessageConverter.DisplayMessage(DynamicDataAccess.GetDynamicData(StoredProcedure.DemandMgmt.Copy, GetCopyCloseArgs())))
                TabData.LoadContent();
        }

        private void DeleteFc(object o)
        {
            if (MessageConverter.DisplayMessage(DynamicDataAccess.GetDynamicData(StoredProcedure.DemandMgmt.Delete, GetCopyCloseArgs())))
                TabData.LoadContent();
        }

        private void Publish(object o)
        {
            if (MessageConverter.DisplayMessage(DynamicDataAccess.GetDynamicData(StoredProcedure.DemandMgmt.Update, GetPublishArgs())))
                TabData.LoadContent();
        }

        private void BulkFc(object o)
        {
            
        }

        private bool CanCopyDelete(object o)
        {
            return TabData.SelectedTab.AreItemsSelected(null);           
        }

        private XElement GetCopyCloseArgs()
        {
            var args = CommonXml.GetBaseSaveArguments();
            args.Add(InputConverter.ToIdxList("Forecasts", TabData.SelectedTab.GetSelectedItems()));
            return args;
        }

        private XElement GetPublishArgs()
        {
            var args = GetCopyCloseArgs();
            var scenarios = new XElement("Scenarios");
            scenarios.AddElement("Idx", TabData.GetDropdown("PublishDropdown").Items.SelectedItem.Idx);
            args.Add(scenarios);
            return args;
        }

        public bool CanPublish(object obj)
        {
            return (CanCopyDelete(obj) && TabData.GetDropdown("PublishDropdown").Items.SelectedItem != null);
        }

        /* It is not decided how bulk forecasting will be enabled in the app */
        public bool CanRunBulk(object o)
        {
            return (CanCopyDelete(o) && false);
        }

        public ICommand NewCommand
        {
            get { return new ViewCommand(NewFc); }
        }

        public ICommand CopyCommand
        {
            get { return new ViewCommand(CanCopyDelete, CopyFc); }
        }

        public ICommand DeleteCommand
        {
            get { return new ViewCommand(CanCopyDelete, DeleteFc); }
        }

        //TODO: Implement
        public ICommand StartBulkCommand
        {
            get { return new ViewCommand(CanRunBulk, BulkFc); }
        }

        public ICommand PublishCommand
        {
            get { return new ViewCommand(CanPublish, Publish); }
        }

        #endregion

        #region Visibility Flags

        public Visibility NewVisibility { get { return (true || App.Configuration.IsNewFcVisible) ? Visibility.Visible : Visibility.Collapsed; } }

        public Visibility CopyVisibility { get { return (true || App.Configuration.IsCopyFcVisible) ? Visibility.Visible : Visibility.Collapsed; } }

        public Visibility DeleteVisibility { get { return (true || App.Configuration.IsDeleteFcVisible) ? Visibility.Visible : Visibility.Collapsed; } }

        public Visibility BulkVisibility { get { return (true || App.Configuration.IsBulkFcVisible) ? Visibility.Visible : Visibility.Collapsed; } }

        #endregion
    }
}