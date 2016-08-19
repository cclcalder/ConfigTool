using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using Exceedra.Common;
using Exceedra.Controls.DynamicGrid.ViewModels;
using Exceedra.Controls.Messages;
using Model;
using Model.DataAccess;
using Model.DataAccess.Converters;
using Model.DataAccess.Generic;
using ViewHelper;
using ViewModels;
using WPF.Navigation;
using WPF.UserControls.Filters.ViewModels;
using WPF.UserControls.Tabs.Models;
using WPF.UserControls.Tabs.ViewModels;
using Model.Entity.Listings;

namespace WPF.ViewModels.Demand
{
    public class DPFcMgmtEditorViewModel : ViewModelBase
    {
        private readonly FcMgmtAccess _access = new FcMgmtAccess();
        private string ForecastIdx { get; set; }
        private XElement ForecastIdxElement { get { return new XElement("Forecast_Idx", ForecastIdx); } }

        public DPFcMgmtEditorViewModel(string forecastIdx)
        {
            CancelCommand = new ViewCommand(Cancel);
            ForecastIdx = forecastIdx;
            LoadDesignProperties().Result.ContinueWith(t =>
            {
                SetSerializedScreen();
            });

            SetupTabs();
        }


        private XElement GetBaseArguments(string rootNode = null)
        {
            rootNode = string.IsNullOrEmpty(rootNode) ? "GetData" : rootNode;
            var args = CommonXml.GetBaseArguments(rootNode);
            args.Add(ForecastIdxElement);
            return args;
        }

        #region Loaders

        private Task<Task> LoadDesignProperties()
        {
            DesignVM = new FilterViewModel
            {
                SingleTreeSaveRootNode = "Users",
                SingleTreeArguments = GetBaseArguments(),
                StatusTreeProc = _access.GetUsersProc(),
                OtherFiltersProc = _access.GetFilterOtherProc(),
                OtherArguments = GetBaseArguments(),
                ListingsSelectionProc = StoredProcedure.DemandMgmt.GetCustSku,
                ListingsSelectionArgs = GetBaseArguments(),
                ReloadListingsSelection = true,
                ReloadOthers = true,
                ReloadSingleTree = true,
                UseListingsGroups = false,                                               
            };
            DesignVM.DataLoaded += DesignVM_DataLoaded;

            return DesignVM.Load();
        }

        //Load and set the listings
        private void DesignVM_DataLoaded()
        {
            var defaults = DynamicDataAccess.GetGenericItem<UserSelectedDefaults>(DesignVM.ListingsSelectionProc, DesignVM.ListingsSelectionArgs);
            DesignVM.ListingsVM.SetSelections(defaults);            
        }

        #endregion

        #region Properties

        private FilterViewModel _designVM;
        public FilterViewModel DesignVM
        {
            get { return _designVM; }
            set
            {
                _designVM = value;
                NotifyPropertyChanged(this, vm => vm.DesignVM);
            }
        }

        private RecordViewModel _fcHistoryVM = new RecordViewModel(false);
        public RecordViewModel FcHistoryVM
        {
            get { return _fcHistoryVM; }
            set
            {
                _fcHistoryVM = value;
                NotifyPropertyChanged(this, vm => vm.FcHistoryVM);
            }
        }

        #endregion

        #region Commands

        public ICommand SaveCommand
        {
            get { return new ViewCommand(CanSave, Save); }
        }

        public ICommand SaveAndCloseCommand
        {
            get { return new ViewCommand(CanSave, SaveClose); }
        }

        public ICommand CancelCommand { get; set; }

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

        public List<Tab> TabList { get; set; }

        private void SetupTabs()
        {
            Tab fstTab = new Tab
            {
                TabName = "History",
                TabTitle = App.CurrentLang.GetValue("Label_ForecastHistory", "Forecast History"),
                TabType = "HorizontalGrid",
                TabMainContentProc = StoredProcedure.DemandMgmt.GetFcGrid,
                AdditionalInputXml = ForecastIdxElement,
                ApplyRootXml = "GetData"
            };

            Tab sndTab = new Tab
            {
                TabName = "Comments",
                TabTitle = App.CurrentLang.GetValue("Label_ForecastComments", "Forecast Comments"),
                TabType = "HorizontalGrid",
                TabMainContentProc = StoredProcedure.DemandMgmt.GetComments,
                ApplyRootXml = "GetData",
                AddCommentProc = StoredProcedure.DemandMgmt.AddComments,
                AddCommentArguments = GetBaseArguments("SaveData"),
            };

            if (TabList == null)
            {
                TabList = new List<Tab>();
            }

            TabList = new List<Tab> { fstTab, sndTab };

            if (TabList.Any())
                TabData = new TabViewModel(TabList);

            TabData.GetFilterXml = GetLoaderXml;

            TabData.LoadContent();
        }

        public XElement GetLoaderXml(string rootNode)
        {
            return GetBaseArguments(rootNode);
        }

        private void Save(object o)
        {
            Save();
        }

        private bool Save(bool close = false)
        {
            var res = DynamicDataAccess.GetDynamicData(StoredProcedure.DemandMgmt.Save, GetSerializedScreen());
            var success = MessageConverter.DisplayMessage(res);

            if(success && close)
            {
                SetSerializedScreen();
                Cancel(null);                
            }
            else if (success)
            {
                ForecastIdx = res.Element("Forecast_Idx").MaybeValue();
                DesignVM.OtherArguments = GetBaseArguments();
                DesignVM.SingleTreeArguments = GetBaseArguments();
                DesignVM.ListingsSelectionArgs = GetBaseArguments();
                DesignVM.LoadOtherFilters();
                SetSerializedScreen();
                TabData.LoadContent();
            }

            return success;

        }

        private void SaveClose(object o)
        {
            Save(true);
        }

        private bool CanSave(object o)
        {
            return DesignVM.CanApplyFilter(null);
        }

        private void Cancel(object o)
        {
            if (ScreenChanged())
                if (CustomMessageBox.Show("There are unsaved changes. Are you sure you want to close?", "Unsaved Changes", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                    return;

            RedirectMe.ListScreen("FCMGMT");
        }

        #endregion

        #region Serialization

        private XElement SerializedScreen { get; set; }

        private void SetSerializedScreen()
        {
            SerializedScreen = GetSerializedScreen();
        }

        private XElement GetSerializedScreen()
        {
            var args = DesignVM.GetFiltersAsXml("SaveData");
            args.AddElement("Forecast_Idx", ForecastIdx);

            return args;
        }

        private bool ScreenChanged()
        {
            return !XNode.DeepEquals(GetSerializedScreen(), SerializedScreen);            
        }

        #endregion
    }
}