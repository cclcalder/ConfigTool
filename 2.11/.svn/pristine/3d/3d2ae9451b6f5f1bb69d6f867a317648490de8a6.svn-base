using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Windows;
using System.Windows.Input;

using System.Xml.Linq;
using Exceedra.Common;
using Exceedra.Controls.DynamicRow.ViewModels;
using Exceedra.Controls.Messages;
using Model;
using Model.DataAccess;
using Model.DataAccess.Converters;
using Model.DataAccess.Generic;
using Model.DataAccess.Listings;
using Model.Entity;
using ViewHelper;
using ViewModels;
using WPF.UserControls.Listings;
using Model.DataAccess.Schedule;
using WPF.UserControls.Tabs.Models;
using WPF.UserControls.Tabs.ViewModels;

namespace WPF.ViewModels.ScheduleNewFilters
{
    public class PromotionTimeLineViewModelV3 : ViewModelBase
    {

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

        private string _operation;
        public string Operation
        {
            get { return _operation; }
            set
            {
                _operation = value;
                NotifyPropertyChanged(this, vm => vm.Operation);
            }
        }

        private ObservableCollection<ScheduleStatuses> _allStatuses;
        public ObservableCollection<ScheduleStatuses> AllStatuses
        {
            get
            {
                return _allStatuses;
            }
            set
            {
                _allStatuses = value;
                NotifyPropertyChanged(this, vm => vm.AllStatuses);
            }
        }

        //TODO: What is this?
        public bool ShowLinks
        {
            get
            {
                return App.Configuration.ScheduleLink;
            }
        }

        #region Constructors

        public static PromotionTimeLineViewModelV3 New()
        {
            var instance = new PromotionTimeLineViewModelV3();
            instance.Init();

            return instance;
        }

        private PromotionTimeLineViewModelV3()
        {

        }

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

        private void Init()
        {
            PageTitle = App.Configuration.GetScreens().Single(f => f.Key == ScreenKeys.SCHEDULE.ToString()).Label;
            SetupTabs();
            TabData.LoadContent();

            var a = new PromotionStatuses().GetScheduleStatuses();
            AllStatuses = new ObservableCollection<ScheduleStatuses>(a);

            PromotionDate defaultFilterDates = _promotionDataAccess.GetDefaultScheduleFilterDates();
            DateFrom = defaultFilterDates.StartDate;
            DateTo = defaultFilterDates.EndDate;

            _listingsBackgroundWorker.DoWork += _listingsBackgroundWorker_DoWork;
            _listingsBackgroundWorker.RunWorkerCompleted += _listingsBackgroundWorker_RunWorkerCompleted;
            _listingsBackgroundWorker.RunWorkerAsync();
        }

        readonly BackgroundWorker _listingsBackgroundWorker = new BackgroundWorker();

        public List<string> SelectedCustomerIDs { get; set; }
        public List<string> SelectedProductIDs { get; set; }

        void _listingsBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ListingsVM = new ListingsViewModel(ListingsAccess.GetFilterCustomers().Result, ListingsAccess.GetFilterProducts().Result, ScreenKeys.SCHEDULE.ToString());
            Thread.Sleep(1200);
        }

        void _listingsBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (ApplyFilterCommand.CanExecute(null))
                ApplyFilterCommand.Execute(null);
        }

        # endregion

        #region Statuses

        public IEnumerable<ScheduleStatuses> SelectedStatuses
        {
            get
            {

                var l = new List<ScheduleStatuses>();

                foreach (var group in AllStatuses)
                {
                    var children = group.Statuses.Where(i => i.IsSelected);
                    if (children.Any())
                    {
                        var nl = new ScheduleStatuses();
                        nl.Name = group.Name;
                        nl.ID = group.ID;
                        nl.Statuses = children.ToList();
                        l.Add(nl);
                    }
                }

                return l;
            }
        }

        #endregion

        #region Dates
        private DateTime _dateFrom;
        private DateTime _dateTo;
        public DateTime DateFrom
        {
            get { return _dateFrom; }
            set
            {
                _dateFrom = value;
                NotifyPropertyChanged(this, vm => vm.DateFrom);

                if (ListingsVM != null)
                {
                    ListingsVM.DateTimeFromParent = DateFrom;
                }
            }
        }
        public DateTime DateTo
        {
            get { return _dateTo; }
            set
            {
                _dateTo = value;
                NotifyPropertyChanged(this, vm => vm.DateTo);
            }
        }

        private DateTime _dateStoredFrom;
        private DateTime _dateStoredTo;
        public DateTime DateStoredFrom
        {
            get { return _dateStoredFrom; }
            set
            {
                _dateStoredFrom = value;
                NotifyPropertyChanged(this, vm => vm.DateStoredFrom);
            }
        }
        public DateTime DateStoredTo
        {
            get { return _dateStoredTo; }
            set
            {
                _dateStoredTo = value;
                NotifyPropertyChanged(this, vm => vm.DateStoredTo);
            }
        }

        public DateTime VisibleDateFrom
        {
            get
            {
                try
                {
                    return _dateFrom.AddMonths(-7);
                }
                catch (Exception ex)
                {

                    return DateTime.Today;
                }

            }
        }
        public DateTime VisibleDateTo
        {
            get { return _dateFrom.AddMonths(7); }
        }

        #endregion

        #region Commands

        public ICommand ApplyFilterCommand
        {
            get { return new ViewCommand(CanApplyFilter, ApplyFilter); }
        }

        public ICommand SaveAsDefaultCommand
        {
            get { return new ViewCommand(CanSaveAsDefault, SaveAsDefault); }
        }

        //public ICommand CopySingleItemCommand
        //{
        //    get { return new ViewCommand(CanSingle, CopySingleItem); }
        //}

        private bool CanSingle(object obj)
        {
            if (Operation == "Edit")
            {
                return CanEdit;
            }

            if (Operation == "Copy")
            {
                return true;
            }

            return false;
        }

        private bool CanDeleteSingle(object obj)
        {
            if (Operation == "Edit")
            {
                return CanEdit;
            }

            return false;
        }

        private bool _canEdit;

        public bool CanEdit
        {
            get { return _canEdit; }
            set
            {
                _canEdit = value;
                NotifyPropertyChanged(this, vm => vm.CanEdit);
            }
        }


        public ICommand SaveSingleItemCommand
        {
            get { return new ViewCommand(CanSingle, SaveSingleItem); }
        }

        public ICommand DeleteSingleItemCommand
        {
            get { return new ViewCommand(CanDeleteSingle, DeleteSingleItem); }
        }

        private void DeleteSingleItem(object obj)
        {

            string message = string.Format("Are you sure you want to delete this item?");

            if (CustomMessageBox.Show(message, "Warning!", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            var res = new ScheduleAccess().DeleteSingleTimelineItem(EditableGrid.ToCoreXml().Root);
            MessageBoxShow(res.Elements().FirstOrDefault().Value);
            ApplyFilterCommand.Execute(null);
        }

        #endregion

        private readonly PromotionAccess _promotionDataAccess = new PromotionAccess();

        public bool CanApplyFilter(object parameter)
        {
            IsEndDateBeforeStart = DateFrom.CompareTo(DateTo) > 0;

            return ListingsVM != null
                && ListingsVM.FilterCheckAndUpdate()
                && SelectedStatuses.Any()
                && SelectedStatuses.Any()
                && !IsEndDateBeforeStart;
        }

        public bool CanSaveAsDefault(object param)
        {
            return CanApplyFilter(null);
        }

        public void SaveAsDefault(object param)
        {
            try
            {
                string res = _promotionDataAccess.SaveUserPrefsSchedule(new PromotionPreferenceDTO
                {
                    Customers = ListingsVM.CustomerIDsList,
                    Products = ListingsVM.ProductIDsList,
                    ScheduleStatuses = SelectedStatuses,
                    DateStart = DateFrom.ToString("yyyy-MM-dd"),
                    DateEnd = DateTo.Date.ToString("yyyy-MM-dd"),
                    ListingsGroupIdx = ListingsVM.ListingGroups.SelectedItem.Idx
                });

                MessageBoxShow(res);
            }
            catch (ExceedraDataException ex)
            {
                MessageBoxShow(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        #region Listings

        private ListingsViewModel _listingsVM;
        public ListingsViewModel ListingsVM
        {
            get
            {
                return _listingsVM;
            }
            set
            {
                _listingsVM = value;
                NotifyPropertyChanged(this, vm => vm.ListingsVM);
            }
        }

        #endregion 

        private RowViewModel _editableGrid;

        public RowViewModel EditableGrid
        {
            get { return _editableGrid; }
            set
            {
                _editableGrid = value;
                NotifyPropertyChanged(this, vm => vm.EditableGrid);
            }
        }

        internal void LoadIndividual(string idx, string type, bool canEdit, string operation)
        {
            CanEdit = canEdit;
            Operation = operation;
            new ScheduleAccess().GetSingleTimelineItem(idx, type, operation).ContinueWith(t =>
            {
                try
                {
                    EditableGrid = new RowViewModel(t.Result);

                    foreach (var col in EditableGrid.Records.ToList())
                    {

                        foreach (var p in col.Properties.Where(r => r.ControlType.Contains("down")).ToList())
                        {
                            col.InitialDropdownLoad(p);
                        }

                    }

                }
                catch (Exception ex)
                {
                    var res = ex;
                    EditableGrid = null;
                }
            });

        }

        //private void CopySingleItem(object obj)
        //{
        //    var res = new ScheduleAccess().CopySingleTimelineItem(EditableGrid.ToCoreXml().Root);
        //    MessageBoxShow(res.Elements().FirstOrDefault().Value);
        //    ApplyFilterCommand.Execute(null);

        //}

        private void SaveSingleItem(object obj)
        {
            var x = EditableGrid.ToCoreXml().Root;
            x.Add(new XElement("Operation", Operation));

            var res = new ScheduleAccess().SaveSingleTimelineItem(x);
            MessageBoxShow(res.Elements().FirstOrDefault().Value);
            ApplyFilterCommand.Execute(null);
        }


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
                TabName = "Schedule",
                TabTitle = App.CurrentLang.GetValue("Label_ScheduleTabTitle", "Schedule"),
                TabType = "ScheduleGrid",
                TabMainContentProc = StoredProcedure.Schedule.GetPromotionScheduleData,
                ApplyRootXml = "GetItems",
            };
            tabList.Add(mainTab);

            TabData = new TabViewModel(tabList)
            {
                GetFilterXml = GetFilterXml
            };
        }

        private void ApplyFilter(object o)
        {
            if (CanApplyFilter(o))
                TabData.LoadContent().ToArray();

        }

        public XElement GetFilterXml(string rootNode = "")
        {
            var root = String.IsNullOrWhiteSpace(rootNode) ? "GetItems" : rootNode;
            var filterXml = CommonXml.GetBaseArguments(root);

            if (CanApplyFilter(null))
            {

                var dates = new XElement("Dates");
                dates.AddElement("Start", InputConverter.ToIsoFormat(DateFrom));
                dates.AddElement("End", InputConverter.ToIsoFormat(DateTo));
                filterXml.Add(dates);

                filterXml.Add(InputConverter.ToList("Customers", "Cust_Idx", ListingsVM.CustomerIDsList));
                filterXml.Add(InputConverter.ToIdxList("Products", ListingsVM.ProductIDsList));

                var items = new XElement("Items");
                foreach (var status in SelectedStatuses)
                {
                    var itemType = new XElement("ItemType");
                    itemType.AddElement("Type_Idx", status.ID);
                    itemType.Add(InputConverter.ToList("Statuses", "Status_Idx", status.Statuses.Select(s => s.ID)));
                    items.Add(itemType);
                }
                filterXml.Add(items);

                return filterXml;
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
