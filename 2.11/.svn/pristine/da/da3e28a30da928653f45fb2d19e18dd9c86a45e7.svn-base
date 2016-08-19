using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Model;
using Model.DataAccess;
using ViewHelper;
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

namespace WPF.ViewModels.Conditions
{
    public class ConditionsViewModel : ViewModelBase
    {
        private readonly ConditionAccess _conditionAccess = new ConditionAccess();
        public RadTabItem SenderTab;

        public ConditionsViewModel()
        {
            LoadFilters();
        }

        private void LoadFilters()
        {
            SetupTabs();

            var datesXml = CommonXml.GetBaseArguments("GetFilterDates");
            var statusXml = CommonXml.GetBaseArguments("GetFilterStatuses");

            FiltersVM = new FilterViewModel
            {
                ApplyFilter = Apply,

                //SaveAsDefaultProc = _conditionAccess.GetSaveDefaultsProc(),
                CurrentScreenKey = ScreenKeys.CONDITION,
                
                SingleTreeArguments = statusXml,
                StatusTreeProc = _conditionAccess.GetFilterStatusesProc(),

                //ListingsProcs = new Tuple<string, string>("app.Procast_SP_COND_GetFilterCustomers", "app.Procast_SP_COND_GetFilterProducts"),

                DateArguments = datesXml,
                DatesProc = _conditionAccess.GetFilterDatesProc()
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

            Tab fstTab = new Tab
            {
                TabName = "Conditions",
                TabTitle = App.CurrentLang.GetValue("Label_ConditionsTab", "Conditions"),
                TabType = "HorizontalGrid",
                TabMainContentProc = StoredProcedure.Conditions.GetConditions,
                ApplyRootXml = "GetConditions"
            };
            tabList.Add(fstTab);

            if (ScheduleTabVisibility == Visibility.Visible)
            {
                Tab sndTab = new Tab
                {
                    TabName = "ConditionsSchedule",
                    TabTitle = App.CurrentLang.GetValue("Label_ConditionsScheduleTabTitle", "Conditions Schedule"),
                    TabType = "ScheduleGrid",
                    TabMainContentProc = StoredProcedure.Conditions.GetConditions
                };

                tabList.Add(sndTab);
            }

            var buttons = new List<ButtonCollection>
            {
                new ButtonCollection {Label = App.CurrentLang.GetValue("Button_New", "New"), ButtonStyle = StyleType.Success, ButtonCommand = CreateConditionCommand, IsVisible = CreateConditionsVisibility},
                new ButtonCollection {Label = App.CurrentLang.GetValue("Button_Copy", "Copy"), ButtonStyle = StyleType.Warning, ButtonCommand = CopyConditionCommand, IsVisible = CopyConditionsVisibility},
                new ButtonCollection {Label = App.CurrentLang.GetValue("Button_Remove", "Remove"), ButtonStyle = StyleType.Danger, ButtonCommand = DeleteConditionCommand, IsVisible = DeleteConditionsVisibility},
            };

            TabData = new TabViewModel(tabList)
            {
                Buttons = buttons,
                GetFilterXml = GetFilterXml
            };

            TabData.LoadContent();
        }

        private void Apply()
        {
            //if (!FiltersVM.CanApplyFilter(null)) return;

            TabData.LoadContent();
        }

        //public XElement GetFilterXml(string rootNode)
        //{
        //    var root = String.IsNullOrWhiteSpace(rootNode) ? "GetConditions" : rootNode;
        //    return FiltersVM.GetFiltersAsXml(root);
        //}
        public XElement GetFilterXml(string rootNode)
        {
            var root = String.IsNullOrWhiteSpace(rootNode) ? "GetConditions" : rootNode;
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


        #region Visiblities

        public Visibility ScheduleTabVisibility
        {
            get { return App.Configuration.IsConditionScheduleActive ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility CreateConditionsVisibility
        {
            get { return App.Configuration.IsCreateConditionsActive ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility CopyConditionsVisibility
        {
            get { return App.Configuration.IsCopyConditionsActive ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility DeleteConditionsVisibility
        {
            get { return App.Configuration.IsDeleteConditionsActive ? Visibility.Visible : Visibility.Collapsed; }
        }

        #endregion

        #region Commands

        public ICommand CopyConditionCommand
        {
            get { return new ViewCommand(CanCopyDelete, CopyCondition); }
        }

        public ICommand DeleteConditionCommand
        {
            get { return new ViewCommand(CanCopyDelete, DeleteCondition); }
        }

        public ICommand CreateConditionCommand
        {
            get { return new ViewCommand(CreateCondition); }
        }

        public bool CanCopyDelete(object o)
        {
            return TabData.SelectedTab.AreItemsSelected(null);
        }

        #endregion

        private void CreateCondition(object parameter)
        {
            RedirectMe.Goto("condition", "0");

        }

        private void CopyCondition(object parameter)
        {
                        var selectedConditionIds = TabData.SelectedTab.GetSelectedItems();

                        string message = "Are you sure you want to copy selected conditions?";

                        if (CustomMessageBox.Show(message, "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Warning) ==
                            MessageBoxResult.No)
                            return;

            _conditionAccess.CopyConditions(selectedConditionIds)
                .ContinueWith(_ =>
                {
                    FiltersVM.ApplyFilter();
                });

                        MessageBoxShow("Copied successfully.");

                    
        }

        private void DeleteCondition(object parameter)
        {

            var selectedConditionIds = TabData.SelectedTab.GetSelectedItems();

                        string message = "Are you sure you want to delete selected conditions?";

                        if (CustomMessageBox.Show(message, "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Warning) ==
                            MessageBoxResult.No)
                            return;

                        _conditionAccess.DeleteConditions(selectedConditionIds)
                      .ContinueWith(_ =>
                      {
                          FiltersVM.ApplyFilter();
                      });


                    
        }
    }
}