using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using Exceedra.Common;
using Exceedra.Controls.Messages;
using Model;
using Model.DataAccess;
using Model.DataAccess.Converters;
using Model.DataAccess.Generic;
using Model.Entity;
using Model.Entity.Generic;
using Model.Entity.NPD;
using ViewHelper;
using ViewModels;
using WPF.Navigation;
using WPF.UserControls.Tabs.Models;
using WPF.UserControls.Tabs.ViewModels;
using Model.Enumerators;
using Exceedra.MultiSelectCombo.ViewModel;

namespace WPF.ViewModels.NPD
{
    public class NPDListViewModel : ViewModelBase
    {
        private readonly NDPAccess _ndpAccess = new NDPAccess();

        public NPDListViewModel()
        {
            InitData();
        }

        private void InitData()
        {
            PageTitle = App.Configuration.GetScreens().Single(f => f.Key == ScreenKeys.NPD.ToString()).Label;
            SetupTabs();

            var tasks = new[]
            {
                LoadDates(),
                LoadStatuses()
            };

            Task.Factory.ContinueWhenAll(tasks, t =>
            {
                Dispatcher.Invoke(new Action(delegate
                {
                    ApplyFilter(null);
                }));
            });
        }

        #region PageTitle

        private string _pageTitle;
        public string PageTitle
        {
            get { return _pageTitle; }
            set
            {
                _pageTitle = value;
                NotifyPropertyChanged(this, vm => vm.PageTitle);
            }
        }

        #endregion

        #region Loaders

        private Task LoadDates(bool resetCache = false)
        {
            return (_ndpAccess.GetDefaultFilterDates(resetCache).ContinueWith(t => NPDFilterDates = t.Result));
        }

        private Task LoadStatuses(bool resetCache = false)
        {
            return (_ndpAccess.GetFilterStatuses(resetCache).ContinueWith(t =>
            {
                var statuses = t.Result.ToList();
                statuses.Do(i => i.IsEnabled = true);
                NPDFilterStatuses.SetItems(t.Result);
            }));
        }

        #endregion

        #region Statuses




        private MultiSelectViewModel _npdFilterStatuses = new MultiSelectViewModel();
        public MultiSelectViewModel NPDFilterStatuses
        {
            get { return _npdFilterStatuses; }
            set
            {
                _npdFilterStatuses = value;
                NotifyPropertyChanged(this, vm => vm.NPDFilterStatuses);
            }
        }

        #endregion

        #region Dates

        private NPDDate _npdFilterDates;

        public NPDDate NPDFilterDates
        {
            get { return _npdFilterDates; }
            set
            {
                _npdFilterDates = value;
                StartDate = value.StartDate;
                EndDate = value.EndDate;
                NotifyPropertyChanged(this, vm => vm.NPDFilterDates);
            }
        }

        private DateTime _startDate;

        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                NotifyPropertyChanged(this, vm => vm.StartDate);
            }
        }

        private DateTime _endDate;

        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
                NotifyPropertyChanged(this, vm => vm.EndDate);
            }
        }

        #endregion

        #region Replace Products

        private readonly ObservableCollection<MasterProduct> _masterProducts = new ObservableCollection<MasterProduct>();
        public ObservableCollection<MasterProduct> MasterProducts
        {
            get { return _masterProducts; }
            set { }
        }

        private MasterProduct _selectedMasterProduct;
        public MasterProduct SelectedMasterProduct
        {
            get { return _selectedMasterProduct; }
            set
            {
                _selectedMasterProduct = value;
                NotifyPropertyChanged(this, vm => vm.SelectedMasterProduct);
            }
        }

        #endregion

        #region ButtonCommands


        public Visibility CreateNpdVisibility
        {
            get { return App.Configuration.IsCreateNpdActive ? Visibility.Visible : Visibility.Collapsed; }
        }
        public Visibility CopyNpdVisibility
        {
            get { return App.Configuration.IsCopyNpdActive ? Visibility.Visible : Visibility.Collapsed; }
        }
        public Visibility DeleteNpdVisibility
        {
            get { return App.Configuration.IsDeleteNpdActive ? Visibility.Visible : Visibility.Collapsed; }
        }

        public ICommand ApplyFilterCommand
        {
            get { return new ViewCommand(CanApplyFilter, ApplyFilter); }
        }

        public ICommand SaveAsDefaultCommand
        {
            get { return new ViewCommand(CanApplyFilter, SaveAsDefault); }
        }

        public bool CanApplyFilter(object parameter)
        {
            IsEndDateBeforeStart = StartDate > EndDate;

            return (!IsEndDateBeforeStart && NPDFilterStatuses.Items.Any() && NPDFilterStatuses.SelectedItems.Any());
        }

        private void SaveAsDefault(object obj)
        {
            if (_ndpAccess.SaveUserPreferences(GetFilterXml("SaveUserPrefs")))
            {
                LoadDates(true);
                LoadStatuses(true);
            }
        }

        public ICommand AddNPDCommand
        {
            get { return new ViewCommand(AddNPD); }
        }

        public ICommand CopyNPDCommand
        {
            get { return new ViewCommand(ValidateCopyDelete, CopyNPD); }
        }

        public ICommand RemoveNPDCommand
        {
            get { return new ViewCommand(ValidateCopyDelete, RemoveNPD); }
        }

        public ICommand ReplaceNPDCommand
        {
            get { return new ViewCommand(ValidateReplace, ReplaceNPD); }
        }

        private void AddNPD(object obj)
        {
            RedirectMe.Goto("NPD", "", "", "", TabData.SelectedTab.TabName);
        }

        private void CopyNPD(object obj)
        {
            if (CopyNPDConfirmation())
            {
                if (_ndpAccess.CopyNPD(TabData.SelectedTab.GetSelectedItems()))
                    ApplyFilter(null);
            }
        }

        private void RemoveNPD(object obj)
        {
            if (RemoveNPDConfirmation())
            {
                if (_ndpAccess.RemoveNPD(TabData.SelectedTab.GetSelectedItems()))
                    ApplyFilter(null);
            }
        }

        private void ReplaceNPD(object obj)
        {
            if (_ndpAccess.ReplaceNPD(TabData.SelectedTab.GetSelectedItems().First(), TabData.GetDropdown("ReplaceDropdown").Items.SelectedItem.Idx))
                ApplyFilter(null);
        }

        private bool ValidateReplace(object obj)
        {
            return CanReplace;
        }

        public bool CanReplace
        {
            get { return TabData.SelectedTab.GetSelectedItems().Count == 1 && TabData.GetDropdown("ReplaceDropdown").Items.SelectedItem != null; }
        }

        private bool ValidateCopyDelete(object obj)
        {
            return CanCopyDelete;
        }

        public bool CanCopyDelete
        {
            get { return TabData.SelectedTab.AreItemsSelected(null); }
        }


        #endregion

        #region Misc

        private bool _isEndDateBeforeStart;
        public bool IsEndDateBeforeStart
        {
            get { return _isEndDateBeforeStart; }
            set
            {
                _isEndDateBeforeStart = value;
                NotifyPropertyChanged(this, vm => vm.IsEndDateBeforeStart);
            }
        }

        private bool RemoveNPDConfirmation()
        {
            if (CustomMessageBox.Show("Are you sure you want to remove the selected items?", "Remove", MessageBoxButton.YesNo, MessageBoxImage.Question) ==
                MessageBoxResult.Yes) return true;

            return false;
        }

        private bool CopyNPDConfirmation()
        {
            if (CustomMessageBox.Show("Are you sure you want to copy the selected items?", "Copy", MessageBoxButton.YesNo, MessageBoxImage.Question) ==
                MessageBoxResult.Yes) return true;

            return false;
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
                TabName = "NPDs",
                TabTitle = App.CurrentLang.GetValue("Label_NPDs", "New Products"),
                TabType = "HorizontalGrid",
                TabMainContentProc = _ndpAccess.GetNPDsProc(),
                ApplyRootXml = "GetNPDs",
            };
            tabList.Add(mainTab);


            var dropdowns = new List<DropdownCollection>
            {
                new DropdownCollection {DropdownProc = _ndpAccess.GetReplacementProductsProc(), Name = "ReplaceDropdown", DoNotCache = true, Label = App.CurrentLang.GetValue("Label_ReplaceProducts", "Replace With"), DropdownCommand = ReplaceNPDCommand},
            };

            var buttons = new List<ButtonCollection>
            {
                new ButtonCollection {Label = App.CurrentLang.GetValue("Button_New", "New"), ButtonStyle = StyleType.Success, ButtonCommand = AddNPDCommand, IsVisible = CreateNpdVisibility},
                new ButtonCollection {Label = App.CurrentLang.GetValue("Button_Copy", "Copy"), ButtonStyle = StyleType.Warning, ButtonCommand = CopyNPDCommand, IsVisible = CopyNpdVisibility},
                new ButtonCollection {Label = App.CurrentLang.GetValue("Button_Remove", "Remove"), ButtonStyle = StyleType.Danger, ButtonCommand = RemoveNPDCommand, IsVisible = DeleteNpdVisibility}
            };

            TabData = new TabViewModel(tabList)
            {
                Dropdowns = dropdowns,
                Buttons = buttons,
                GetFilterXml = GetFilterXml
            };
        }

        private void ApplyFilter(object o)
        {
            if (CanApplyFilter(o))
                TabData.LoadContent();
        }

        public XElement GetFilterXml(string rootNode = "")
        {
            var root = String.IsNullOrWhiteSpace(rootNode) ? "GetNPDs" : rootNode;
            var filterXml = CommonXml.GetBaseArguments(root);
            filterXml.AddElement("Start_Date", InputConverter.ToIsoFormat(StartDate));
            filterXml.AddElement("End_Date", InputConverter.ToIsoFormat(EndDate));
            filterXml.Add(InputConverter.ToList("Statuses", "Status_Idx", NPDFilterStatuses.SelectedItemIdxs));

            return filterXml;
        }

        #endregion

    }
}