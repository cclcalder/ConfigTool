using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using Exceedra.Common;
using Exceedra.Controls.DynamicGrid.ViewModels;
using Exceedra.Schedule.ViewModels;
using Exceedra.SingleSelectCombo.ViewModel;
using Model;
using Model.DataAccess.Generic;
using Model.Entity.Generic;
using ViewModels;
using WPF.Navigation;
using WPF.UserControls.Tabs.Models;
using Model.Enumerators;

namespace WPF.UserControls.Tabs.ViewModels
{
    public class TabViewModel : ViewModelBase
    {
        public TabViewModel(List<Tab> tabListNew)
        {
            Tabs = tabListNew;
            SetSelectedTab();

        }

        private List<Tab> _tabs = new List<Tab>();

        public TabViewModel()
        {
            //throw new NotImplementedException();
        }

        public List<Tab> Tabs
        {
            get { return _tabs; }
            set
            {
                _tabs = value;
                Tabs.Do(tab => tab.PropertyChanged += TabOnPropertyChanged);
                NotifyPropertyChanged(this, vm => vm.Tabs);
            }
        }

        private bool _isExportVisible = true;

        public bool IsExportVisible
        {
            get
            {
                return _isExportVisible;
            }
            set
            {
                _isExportVisible = value;
                NotifyPropertyChanged(this, vm => vm.IsExportVisible);
            }
        }

        public Func<string, XElement> GetFilterXml { get; set; }

        public List<Task> LoadContent(bool reload = true)
        {
            LoadDropdowns();

            var tasks = new List<Task>();

            foreach (var tab in Tabs)
            {
                switch (tab.TabType.ToLower())
                {
                    case "horizontalgrid":
                        tasks.Add(LoadGrid(tab));
                        break;
                    case "schedulegrid":
                        tasks.Add(LoadSchedule(tab));
                        break;
                    case "chart":
                        tasks.Add(LoadCharts(tab));
                        break;
                }
            }

            return tasks;
        }

        private XElement GetTabXml(Tab tab)
        {
            var xml = GetFilterXml("");
            if (!String.IsNullOrWhiteSpace(tab.ApplyRootXml))
                xml.Name = tab.ApplyRootXml;

            xml.Add(tab.AdditionalInputXml);
            return xml;
        }

        private Task LoadGrid(Tab tab)
        {
            tab.TabMainContent = new RecordViewModel();
            return DynamicDataAccess.GetGenericItemAsync<RecordViewModel>(tab.TabMainContentProc, GetTabXml(tab), true, "").ContinueWith(t => tab.TabMainContent = t.Result);
        }

        private Task LoadSchedule(Tab tab)
        {
            return Task.Factory.StartNew(() =>
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    var scheduleVm =
                        DynamicDataAccess.GetGenericItem<ScheduleViewModel>(
                            tab.TabMainContentProc, GetTabXml(tab), true, "");
                    scheduleVm.Title = tab.TabTitle;
                    tab.TabMainContent = scheduleVm;
                    scheduleVm.SwitchVisibleItems(1);
                });
            });
        }

        private Task LoadCharts(Tab tab)
        {
            tab.TabMainContent = new Exceedra.Chart.ViewModels.RecordViewModel();

            /* Only load the dropdown if we have a proc and we haven't already populated it */
            if (!String.IsNullOrEmpty(tab.TabChartListProc) && !(tab.TabChartItems.Items != null && tab.TabChartItems.Items.Any()))
            {
                return LoadChartDropdown(tab);
            }

            return LoadChart(tab);
        }

        private Task LoadChart(Tab tab, bool def = false)
        {
            var xml = GetTabXml(tab);

            if (!String.IsNullOrEmpty(tab.SelectedItemIdx))
                xml.AddElement("Promotion_Graph_Idx", tab.SelectedItemIdx);

            if (def)
                xml.Add(new XElement("LoadFromDefaults", "1"));

            return DynamicDataAccess.GetGenericItemAsync<Exceedra.Chart.ViewModels.RecordViewModel>(tab.TabMainContentProc, xml).ContinueWith(t =>
            {
                tab.TabMainContent = t.Result;
            });
        }

        private Task LoadChartDropdown(Tab tab)
        {
            var xml = GetTabXml(tab);
            xml.Name = "GetPromotion_Graphs";

            return (DynamicDataAccess.GetGenericEnumerableAsync<ComboboxItem>(
                tab.TabChartListProc, xml).ContinueWith(
                    t =>
                    {
                        var items = t.Result.ToList();
                        items[0].IsSelected = true;
                        tab.TabChartItems.SetItems(items);
                    }));
        }

        private void TabOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "TabMainContent")
            {
                NotifyPropertyChanged(this, vm => vm.Tabs);
            }

            if (propertyChangedEventArgs.PropertyName == "SelectedItemIdx")
            {
                LoadChart(sender as Tab);
            }

            if (propertyChangedEventArgs.PropertyName == "CommentAdded")
            {
                LoadGrid(sender as Tab);
            }
        }

        public void FilterAllTabs(string filterText)
        {
            foreach (var tab in Tabs)
            {
                var contentType = tab.TabType;
                switch (contentType.ToLower())
                {
                    case "horizontalgrid":
                        ((RecordViewModel)tab.TabMainContent).Filter = filterText;
                        break;
                    case "schedulegrid":
                        ((ScheduleViewModel)tab.TabMainContent).Filter = filterText;
                        break;
                }
            }
        }

        public int SelectedTabIndex { get; set; }

        public Tab SelectedTab { get { return Tabs[SelectedTabIndex]; } }

        #region Dropdowns

        private List<DropdownCollection> _dropdowns = new List<DropdownCollection>();
        public List<DropdownCollection> Dropdowns
        {
            get { return _dropdowns; }
            set
            {
                _dropdowns = value;
                NotifyPropertyChanged(this, vm => vm.Dropdowns);
            }
        }

        private void LoadDropdowns()
        {
            if (Dropdowns != null)
            {
                foreach (var dropdown in Dropdowns)
                {
                    if (dropdown.IsDatePicker)
                        dropdown.Date = LoadDatepicker(dropdown);
                    else
                        dropdown.Items.SetItems(LoadDropdown(dropdown).ToList());
                }
                Dropdowns = new List<DropdownCollection>(Dropdowns);
            }

            Buttons = new List<ButtonCollection>(Buttons);
        }


        /* When using additional xml, add a dummy root node around the desired xml
         * e.g. <ROOT>
         * <IncludeTemplateStatuses>0</IncludeTemplateStatuses>
         * <IncludePromotionStatuses>1</IncludePromotionStatuses>
         * <ReturnAsList>1</ReturnAsList>
         * </ROOT>
         * Here root is used to make it valid but not passed to the proc   
         * 
         * Cache Override
         * When loading the dropdowns we use a cache override.
         * This is due to some status trees and dropdowns sharing the same procs
         * This was we can call it here without overwriting the tree cache
         */
        private IEnumerable<ComboboxItem> LoadDropdown(DropdownCollection dropdown)
        {
            var args = new XElement("args");
            if (dropdown.AppTypeIdx != null)
            {
                args = dropdown.UseFilters
                   ? GetFilterXml(dropdown.RootNode)
                   : CommonXml.GetBaseArguments(dropdown.RootNode, dropdown.AppTypeIdx);
            }
            else
            {
                args = dropdown.UseFilters
                   ? GetFilterXml(dropdown.RootNode)
                   : CommonXml.GetBaseArguments(dropdown.RootNode);
            }


            if (dropdown.AdditionalLoadXml != null)
                args.Add(dropdown.AdditionalLoadXml.Elements());

            return DynamicDataAccess.GetGenericEnumerable<ComboboxItem>(dropdown.DropdownProc, args, dropdown.DoNotCache);
        }

        private DateTime LoadDatepicker(DropdownCollection dropdown)
        {
            return DateTime.Today;
        }

        public DropdownCollection GetDropdown(string name)
        {
            return Dropdowns.First(d => d.Name == name);
        }

        public Tab GetTab(string name)
        {
            return Tabs.First(t => t.TabName == name);
        }

        #endregion

        #region Buttons

        private List<ButtonCollection> _buttons = new List<ButtonCollection>();
        public List<ButtonCollection> Buttons
        {
            get { return _buttons; }
            set
            {
                _buttons = value;
                NotifyPropertyChanged(this, vm => vm.Buttons);
            }
        }

        #endregion

        public void SetSelectedTab()
        {
            var tabIdx = Tabs.FindIndex(t => t.TabName == RedirectMe.LastOpenTab);
            SelectedTabIndex = tabIdx == -1 ? 0 : tabIdx;
        }
    }

    public class DropdownCollection
    {
        public string Name { get; set; }
        public string Label { get; set; }

        private SingleSelectViewModel _items = new SingleSelectViewModel();
        public SingleSelectViewModel Items { get { return _items; } set { _items = value; } }
        public ICommand DropdownCommand { get; set; }
        public string RootNode { get; set; }
        public bool UseFilters { get; set; }

        private string _dropdownProc = "";// StoredProcedure.Shared.GetFilterStatusesAndTypes;
        public string DropdownProc { get { return _dropdownProc; } set { _dropdownProc = value; } }

        public Visibility IsVisible { get; set; }
        public XElement AdditionalLoadXml { get; set; }
        public string AppTypeIdx { get; set; }

        /* Special case for scenarios */
        public DateTime Date { get; set; }
        public bool IsDatePicker { get; set; }

        public bool DoNotCache { get; set; }
    }

    public class ButtonCollection
    {
        public string Label { get; set; }
        public ICommand ButtonCommand { get; set; }
        public Visibility IsVisible { get; set; }
        public StyleType ButtonStyle { get; set; }
    }
}