using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using Exceedra.Common;
using Exceedra.Common.Xml;
using Exceedra.Controls.DynamicGrid.ViewModels;
using Exceedra.Controls.DynamicRow.ViewModels;
using Exceedra.Controls.Messages;
using Model;
using Model.DataAccess;
using Model.DataAccess.Converters;
using Model.DataAccess.Generic;
using Model.Entity;
using Model.Entity.Generic;
using Telerik.Windows.Controls;
using ViewHelper;
using WPF.Navigation;
using WPF.UserControls.Filters.ViewModels;
using WPF.UserControls.Tabs.Models;
using WPF.UserControls.Tabs.ViewModels;
using ViewModelBase = ViewModels.ViewModelBase;
using Model.Enumerators;

namespace WPF.ViewModels.Funds
{
    public class FundListViewModel : ViewModelBase
    {
        public static DateTime? StaticDateFrom;
        private readonly FundAccess _access = new FundAccess();

        private FilterViewModel _filtersVM;

        public List<string> InitallyExpandedDeatilsList = new List<string>();
        public RadTabItem SenderTab;

        public FilterViewModel FiltersVM
        {
            get { return _filtersVM; }
            set
            {
                _filtersVM = value;
                NotifyPropertyChanged(this, vm => vm.FiltersVM);
            }
        }

        public bool CreateType { get; set; }
        public bool IsRecipientActive { get; set; }





        #region Commands

        public ICommand SaveTransferCommand
        {
            get { return new ViewCommand(SaveFundTransfer); }
        }

        public ICommand OpenFundsTransferCommand
        {
            get { return new ViewCommand(CanOpenFundsTransfer, OpenFundsTransfer); }
        }

        public ICommand CancelTansferFund
        {
            get { return new ViewCommand(CancelSaveTransfer); }
        }
        
        public ICommand AddFundCommand
        {
            get { return new ActionCommand(AddFund); }
        }

        public ICommand AddParentFundCommand
        {
            get { return new ActionCommand(AddParentFund); }
        }

        public ICommand CopyFundCommand
        {
            get { return new ViewCommand(CanCopy, CopyFund); }
        }

        public ICommand RemoveFundCommand
        {
            get { return new ViewCommand(CanRemove, RemoveFund); }
        }

        public ICommand UpdateMultipleStatusCommand
        {
            get { return new ViewCommand(CanUpdateStatus, UpdateMultipleStatus); }
        }

        public bool CanRemove(object o)
        {
            var isParent = TabData.SelectedTab.TabName == "ParentFunds";
            if (isParent)
                return (CanCopyCloseDelete(o) && DeleteParentFundsVisibility == Visibility.Visible);

            return CanCopyCloseDelete(o);
        }

        public bool CanCopy(object o)
        {
            var isParent = TabData.SelectedTab.TabName == "ParentFunds";
            if (isParent)
                return (CanCopyCloseDelete(o) && CopyParentFundsVisibility == Visibility.Visible);

            return CanCopyCloseDelete(o);
        }

        public bool CanCopyCloseDelete(object obj)
        {

            return TabData.SelectedTab.AreItemsSelected(null);
        }

        public bool CanUpdateStatus(object obj)
        {
            return (CanCopyCloseDelete(null) && TabData.GetDropdown("UpdateStatusDropdown").Items.SelectedItem != null);
        }

        #endregion

        private void LoadFilters()
        {
            SetupTabs();

            FiltersVM = new FilterViewModel
            {
                ApplyFilter = TestApply,
                //SaveAsDefaultProc = _access.GetSaveAsDefaultsProc(),
                CurrentScreenKey = ScreenKeys.FUND,
                SingleTreeArguments = CommonXml.GetBaseArguments(XMLNode.Nodes.GetStatuses.ToString()),
                StatusTreeProc = StoredProcedure.Fund.GetFilterStatuses,
                //ListingsExtraArguments = new XElement(XMLNode.Nodes.Screen_Code.ToString(), ScreenKeys.FUND.ToString()),
                //ListingsProcs =
                //    new Tuple<string, string>("app.Procast_SP_FUND_GetFilterCustomers",
                //        "app.Procast_SP_FUND_GetFilterProducts"),
                DateArguments = CommonXml.GetBaseArguments(XMLNode.Nodes.GetFilterDates.ToString()),
                DatesProc = StoredProcedure.Fund.GetFilterDates,
                //SaveDefaultsRootNode = "SaveUserPrefs"
            };
             
            FiltersVM.Load();
        }

        private void AddFund()
        {
            RedirectMe.Goto("fund", "", "", "", TabData.SelectedTab.TabName);
        }

        private void AddParentFund()
        {
            RedirectMe.Goto("parentfund", "", "", "", TabData.SelectedTab.TabName);
        }

        private void CopyFund(object obj)
        {
            _access.CopyFund(TabData.SelectedTab.GetSelectedItems())
                .ContinueWith(t =>
                {
                    MessageConverter.DisplayMessage(t.Result);
                    ApplyFilter();
                }, App.Scheduler);
        }

        private void RemoveFund(object obj)
        {
            RemovedSelectedFunds(TabData.SelectedTab.GetSelectedItems());
        }

        private void RemovedSelectedFunds(List<string> selectedIDs)
        {
            _access.RemoveFund(selectedIDs)
                .ContinueWith(t =>
                {
                    MessageConverter.DisplayMessage(t.Result);
                    ApplyFilter();
                }, App.Scheduler);
        }

        public void ApplyFilter()
        {
            //if (FiltersVM.CanApplyFilter(null))
                FiltersVM.ApplyFilter();
        }

        public void SaveFundTransfer(object param)
        {
            var fundsDetailRVMElement = new XElement("Parent",
                new XElement("Results", new XElement(FundDetailRVM.ToCoreXml().Root)));

            var fundsVMElement = new XElement("Children", new XElement(SelectedFundsVM.ToXml().Root));

            var res = _access.SaveFundstransfer(fundsVMElement, fundsDetailRVMElement);

            if (res.ToLowerInvariant().Contains("success"))
            {
                FundsPopUpVisibility = Visibility.Collapsed;
                ApplyFilter();
            }
        }

        public void CancelSaveTransfer(object obj)
        {
            FundsPopUpVisibility = Visibility.Collapsed;
        }

        public void OpenFundsTransfer(object obj)
        {
            UpdatedFundDetail();
            UpdateSelectedFunds();
            FundsPopUpVisibility = Visibility.Visible;
        }

        public bool CanOpenFundsTransfer(object obj)
        {
            return (TabData.SelectedTab.AreItemsSelected(null)
                    && TabData.SelectedTab.TabName == "Funds"
                    && TabData.GetDropdown("TransferFundDropdown").Items.SelectedItem != null
                );
        }

        #region "Multiple status updates"

        private void UpdateMultipleStatus(object obj)
        {
            UpdateMultipleFundsStatuses(TabData.SelectedTab.GetSelectedItems(), TabData.GetDropdown("UpdateStatusDropdown").Items.SelectedItem);
        }

        /// <summary>
        ///     Same proc used in DB for both funds and parent funds, just the selected Ids come from different grids and new
        ///     status from a different dropdown
        /// </summary>
        /// <param name="anyFunds"></param>
        /// <param name="selectedIds"></param>
        /// <param name="status"></param>
        private void UpdateMultipleFundsStatuses(List<string> selectedIds, ComboboxItem status)
        {
            var message = string.Format("Are you sure you want to set selected to {0} status?",
                status.Name);

            if (CustomMessageBox.Show(message, "Warning!", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            try
            {
                    var res = _access.UpdateStatusParentFunds(selectedIds.Where(y => y != null).ToArray(), status.Idx);

                    MessageBoxShow(res);

                    App.AppCache.Remove("Funds_DataList");

                    ApplyFilter();
                
            }
            catch (ExceedraDataException ex)
            {
                MessageBoxShow(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        #endregion

        #region "Constructors"

        private FundListViewModel()
        {
            FundsPopUpVisibility = Visibility.Collapsed;
        }

        public static FundListViewModel New()
        {
            var instance = new FundListViewModel();
            instance.Init();

            return instance;
        }

        public void Init()
        {
            LoadFilters();
            FundsPopUpVisibility = Visibility.Collapsed;
        }

        #endregion

        #region method to prevent a db change

        private XElement AlterDateXml(XElement xml)
        {
            xml.Descendants().First(n => n.Name == "Start").Name = "Start_Date";
            xml.Descendants().First(n => n.Name == "End").Name = "End_Date";

            return xml;
        }

        #endregion












        #region Transfer Model Dialog

        private RowViewModel _fundDetailRVM;
        private Visibility _fundsPopUpVisibility;
        private RecordViewModel _selectedFundsVM;

        public RowViewModel FundDetailRVM
        {
            get { return _fundDetailRVM; }
            set
            {
                _fundDetailRVM = value;
                NotifyPropertyChanged(this, vm => vm.FundDetailRVM);
            }
        }

        public RecordViewModel SelectedFundsVM
        {
            get { return _selectedFundsVM; }
            set
            {
                _selectedFundsVM = value;
                NotifyPropertyChanged(this, vm => vm.SelectedFundsVM);
            }
        }

        public void UpdateSelectedFunds()
        {
            var s = TabData.Tabs.First(t => t.TabName == "Funds").GetSelectedItems();
            var p = TabData.GetDropdown("TransferFundDropdown").Items.SelectedItem.Idx;
            var x = _access.GetTransferChildFunds(s,p);
            SelectedFundsVM = new RecordViewModel(x);

            NotifyPropertyChanged(this, vm => vm.SelectedFundsVM);
        }

        public void UpdatedFundDetail()
        {
            var x = _access.GetTransferDetails(TabData.GetDropdown("TransferFundDropdown").Items.SelectedItem.Idx);
            FundDetailRVM = new RowViewModel(x);
        }

        #endregion


        #region Visibility Configs

        public Visibility AddFundsVisibility
        {
            get { return App.Configuration.IsCreateFundsActive ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility CopyFundsVisibility
        {
            get { return App.Configuration.IsCopyFundsActive ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility DeleteFundsVisibility
        {
            get { return App.Configuration.IsDeleteFundsActive ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility AddParentFundsVisibility
        {
            get { return App.Configuration.IsParentFundsActive ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility CopyParentFundsVisibility
        {
            get { return App.Configuration.IsCopyParentFundsActive ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility DeleteParentFundsVisibility
        {
            get { return App.Configuration.IsDeleteParentFundsActive ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility FundsTransferVisibility
        {
            get { return App.Configuration.IsFundsTransferActive ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility ScheduleTabVisibility
        {
            get { return App.Configuration.IsScheduleTermsActive ? Visibility.Visible : Visibility.Collapsed; }
        }

        //public Visibility ParentFundTabVisibility
        //{
        //    get { return ClientConfiguration.IsParentFundsActive ? Visibility.Visible : Visibility.Collapsed; }
        //}

        public Visibility FundsPopUpVisibility
        {
            get { return _fundsPopUpVisibility; }
            set
            {
                _fundsPopUpVisibility = value;
                NotifyPropertyChanged(this, vm => vm.FundsPopUpVisibility);
            }
        }

        #endregion




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
                TabName = "Funds",
                TabTitle = App.CurrentLang.GetValue("Label_FundsTabTitle", "Funds"),
                TabType = "HorizontalGrid",
                TabMainContentProc = StoredProcedure.Fund.GetFundList,
                AdditionalInputXml = new XElement("IsParent", 0),
                ApplyRootXml = "GetFunds"
            };

            if (TabList == null)
            {
                TabList = new List<Tab>();
            }

            TabList = new List<Tab> { fstTab };

            if (AddParentFundsVisibility == Visibility.Visible)
            {
                Tab sndTab = new Tab
                {
                    TabName = "ParentFunds",
                    TabTitle = App.CurrentLang.GetValue("Label_FundsParentTabTitle", "Parent Funds"),
                    TabType = "HorizontalGrid",
                    TabMainContentProc = StoredProcedure.Fund.GetFundList,
                    AdditionalInputXml = new XElement("IsParent", 1)
                };

 
                TabList.Add(sndTab);
            }

            if (ScheduleTabVisibility == Visibility.Visible)
            {
                Tab trdTab = new Tab
                {
                    TabName = "FundsSchedule",
                    TabTitle = App.CurrentLang.GetValue("Label_FundsScheduleTabTitle", "Funds Schedule"),
                    TabType = "ScheduleGrid",
                    TabMainContentProc = StoredProcedure.Fund.GetFundList,
                    AdditionalInputXml = new XElement("IsParent", 0)
                };
 
                TabList.Add(trdTab);
            }

            var dropdowns = new List<DropdownCollection>
            {
                new DropdownCollection {DropdownProc = StoredProcedure.Fund.DropDownStatuses, Name = "UpdateStatusDropdown", Label = App.CurrentLang.GetValue("Label_UpdateStatuses", "Update Statuses"), DropdownCommand = UpdateMultipleStatusCommand},
                new DropdownCollection {DropdownProc = StoredProcedure.Fund.GetParentFundsList, Name = "TransferFundDropdown", DoNotCache = true, Label = App.CurrentLang.GetValue("Label_TransferFunds", "Transfer Funds"), DropdownCommand = OpenFundsTransferCommand, UseFilters = true, IsVisible = AddParentFundsVisibility}
            };

            var buttons = new List<ButtonCollection>
            {
                new ButtonCollection {Label = App.CurrentLang.GetValue("Button_New", "New"), ButtonStyle = StyleType.Success, ButtonCommand = AddFundCommand, IsVisible = AddFundsVisibility},
                new ButtonCollection {Label = App.CurrentLang.GetValue("Button_NewParent", "New Parent"), ButtonStyle = StyleType.Success, ButtonCommand = AddParentFundCommand, IsVisible = AddParentFundsVisibility},
                new ButtonCollection {Label = App.CurrentLang.GetValue("Button_Copy", "Copy"), ButtonStyle = StyleType.Warning, ButtonCommand = CopyFundCommand, IsVisible = CopyFundsVisibility},
                new ButtonCollection {Label = App.CurrentLang.GetValue("Button_Remove", "Remove"), ButtonStyle = StyleType.Danger, ButtonCommand = RemoveFundCommand, IsVisible = DeleteFundsVisibility},
            };

            if(TabList.Any())
                TabData = new TabViewModel(TabList)
                {
                    Dropdowns = dropdowns,
                    Buttons = buttons,
                    GetFilterXml = GetFilterXml
                };

            TabData.LoadContent();
        }
        
        private void TestApply()
        {
            //var fundXml = AlterDateXml(FiltersVM.GetFiltersAsXml("GetFunds"));
            TabData.LoadContent();
        }

        //public XElement GetFilterXml(string rootNode)
        //{
        //    var root = String.IsNullOrWhiteSpace(rootNode) ? "GetFunds" : rootNode;
        //    return AlterDateXml(FiltersVM.GetFiltersAsXml(root));
        //}

        public XElement GetFilterXml(string rootNode)
        {
            var root = String.IsNullOrWhiteSpace(rootNode) ? "GetFunds" : rootNode;
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
    }
}