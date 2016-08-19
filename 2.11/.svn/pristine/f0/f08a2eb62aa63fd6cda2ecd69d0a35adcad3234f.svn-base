using Coder.UI.WPF;
using Model.DataAccess;
using Model.Entity.ROBs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ViewHelper;
using ViewModels;

namespace WPF.ViewModels.Scenarios
{
    public class ApplicableROBViewModel : ViewModelBase
    {
        private readonly ScenarioAccess _scenarioAccess;
        private string _idSalesOrg;
        public bool HasChanged;

        private string _appTypeId;

        public string AppTypeId
        {
            get { return _appTypeId; }
            set
            {
                _appTypeId = value;
                NotifyPropertyChanged(this, vm => vm.AppTypeId);
            }
        }

        private string _appName;

        public string AppName
        {
            get { return _appName; }
            set
            {
                _appName = value;
                NotifyPropertyChanged(this, vm => vm.AppName);
            }
        }

        private int _scenarioId;

        public int ScenarioId
        {
            get { return _scenarioId; }
            set
            {
                _scenarioId = value;
                NotifyPropertyChanged(this, vm => vm.ScenarioId);
            }
        }

        private bool _isPromoDataLoading;
        //private bool _isApplyingBase; // not needed?
        //private bool _isSaving; // not needed?
        private DateTime? _selectedStartDate;
        private DateTime? _selectedEndDate;

        private int _results;
        public int Results
        {
            get { return _results; }
            set
            {
                _results = value;

                NotifyPropertyChanged(this, vm => vm.Results);
                ShowGrid = (_results == 0 ? Visibility.Hidden : Visibility.Visible);
                ShowEmpty = (_results == 0 ? Visibility.Visible : Visibility.Hidden);



            }
        }


        public Visibility _showGrid;
        public Visibility _showEmpty;
        public Visibility ShowGrid { get { return _showGrid; } set { _showGrid = value; NotifyPropertyChanged(this, vm => vm.ShowGrid); } }
        public Visibility ShowEmpty { get { return _showEmpty; } set { _showEmpty = value; NotifyPropertyChanged(this, vm => vm.ShowEmpty); } }

        public DateTime? SelectedStartDate
        {
            get
            {
                return _selectedStartDate;
            }
            set
            {
                _selectedStartDate = value;
                NotifyPropertyChanged(this, vm => vm.SelectedStartDate);
            }
        }

        public DateTime? SelectedEndDate
        {
            get
            {
                return _selectedEndDate;
            }
            set
            {
                _selectedEndDate = value;
                NotifyPropertyChanged(this, vm => vm.SelectedEndDate);
                //   SetEndDateChange();
            }
        }

        private ObservableCollection<Rob> _dataList = new ObservableCollection<Rob>();
        public ObservableCollection<Rob> DataList
        {
            get
            {
                return _dataList;
            }
            set
            {
                _dataList = new ObservableCollection<Rob>(value);
                NotifyPropertyChanged(this, vm => vm.DataList);


            }
        }

        private ObservableCollection<string> _customerIDs = new ObservableCollection<string>();
        public ObservableCollection<string> CustomerIDs
        {
            get
            {
                return _customerIDs;
            }
            set
            {
                _customerIDs = new ObservableCollection<string>(value);
                NotifyPropertyChanged(this, vm => vm.CustomerIDs);
            }
        }


        private ObservableCollection<string> _productIDs = new ObservableCollection<string>();
        public ObservableCollection<string> ProductIDs
        {
            get
            {
                return _productIDs;
            }
            set
            {
                _productIDs = new ObservableCollection<string>(value);
                NotifyPropertyChanged(this, vm => vm.ProductIDs);
            }
        }
        private ObservableCollection<int> _selectedPromotions = new ObservableCollection<int>();
        public ObservableCollection<int> SelectedPromotions
        {
            get
            {
                return _selectedPromotions;
            }
            set
            {
                _selectedPromotions = new ObservableCollection<int>(value);
                NotifyPropertyChanged(this, vm => vm.SelectedPromotions);
            }
        }

        private bool _isSelectAllEnabled;
        public bool IsSelectAllEnabled
        {
            get { return _isSelectAllEnabled; }
            set
            {
                _isSelectAllEnabled = value;
                NotifyPropertyChanged(this, vm => vm.IsSelectAllEnabled);
            }
        }

        private bool _isSelectAllChecked;
        public bool IsSelectAllChecked
        {
            get { return _isSelectAllChecked; }
            set
            {
                _isSelectAllChecked = value;
                NotifyPropertyChanged(this, vm => vm.IsSelectAllChecked);

                if (!_isPromoDataLoading)
                    DataList.Select(c => { c.IsSelected = _isSelectAllChecked; return c; }).ToList();
                 //&&
                 //   !_isApplyingBase &&
                 //   !_isSaving

                NotifyPropertyChanged(this, vm => vm.SelectedPromotions);
                NotifyPropertyChanged(this, vm => vm.DataList);
                HasChanged = true;
            }
        }

        private BulkObservableCollection<Status> _robStatuses = new BulkObservableCollection<Status>();
        public BulkObservableCollection<Status> RobStatuses
        {
            get
            {
                return _robStatuses;
            }
            set
            {
                _robStatuses = value;
                NotifyPropertyChanged(this, vm => vm.RobStatuses);

            }
        }
        private ObservableCollection<Status> _selectedStatus = new ObservableCollection<Status>();
        public ObservableCollection<Status> SelectedStatus
        {
            get { return _selectedStatus; }
            set
            {
                _selectedStatus = value;
                NotifyPropertyChanged(this, vm => vm.SelectedStatus);

            }
        }

        public ICommand ApplyFilterCommand
        {
            get { return _applyfilterCommand; }
        }

        private readonly ViewCommand _applyfilterCommand;
        private bool CanApplyFilter(object obj)
        {
            var x = CustomerIDs.Any() && ProductIDs.Any();
            var y = ( !_isPromoDataLoading); // || !_isSaving !_isApplyingBase ||
            return x && y;

            //if ((SelectedStartDate != null && SelectedEndDate != null) && SelectedCustomers.Count > 0 && SelectedProducts.Count > 0)
            //{
            //}
            //return false;
        }

        public bool IsPromoDataLoading
        {
            get { return _isPromoDataLoading; }
            set
            {
                _isPromoDataLoading = value;
                NotifyPropertyChanged(this, vm => vm.IsPromoDataLoading);
            }
        }

        public ApplicableROBViewModel(int scenarioId, string idSalesOrg,
            ScenarioAccess scenarioAccess, string appID, string appName,
            List<string> customerIDs,
             List<string> productsIDs,
            DateTime? startDate, DateTime? endDate,
            bool load = true)
        {
            _scenarioId = scenarioId;
            _idSalesOrg = idSalesOrg;
            _scenarioAccess = scenarioAccess;
            _appTypeId = appID;
            _appName = appName;
            _customerIDs = new ObservableCollection<string>(customerIDs);
            _productIDs = new ObservableCollection<string>(productsIDs);
            _selectedStartDate = startDate;
            _selectedEndDate = endDate;
            // _selectedPromotions = new ObservableCollection<int>(promotionIDs);

            _applyfilterCommand = new ViewCommand(CanApplyFilter, ApplyFilter);

            Results = _dataList.Count();
            if (load) { InitData(); } else { }

        }

        private void InitData()
        {
            if (ScenarioId > 0)
            {
                IsPromoDataLoading = true;
            }

            var independentTasks = new[]
                {
                    _scenarioAccess.GetFundingStatuses(AppTypeId)
                                   .ContinueWith(t =>
                                       {
                                           Populate(this, t, _robStatuses, null, vm => vm.RobStatuses);
                                           SelectedStatus =
                                               new ObservableCollection<Status>(
                                                    RobStatuses.Where(x => x.IsSelected).Select(s => s));
                                       }, App.Scheduler)                 
                };
            Task.Factory.ContinueWhenAll(independentTasks, _ => InitSalesOrgDepedentData(),
                new CancellationToken(),
                TaskContinuationOptions.None,
                App.Scheduler);


        }

        private void InitSalesOrgDepedentData()
        {
            var CollectionsOK = true;
            if (_selectedStartDate.HasValue == false || _selectedEndDate.HasValue == false || _productIDs.Count == 0)
            {
                CollectionsOK = false;
                IsPromoDataLoading = false;
            }

            if (CollectionsOK)
                ApplyFilter(new object());
        }

        private const string Check = "check";

        public void LoadData()
        {
            ApplyFilter(new object());
        }

        private void ApplyFilter(object obj)
        {
          
            if (!CanApplyFilter(null))
                return;

            IsPromoDataLoading = true;
            var statusIDs = SelectedStatus.Select(q => q.ID).ToList();//.PromotionStatus.Where(q => q.IsSelected)

            SelectedPromotions = new ObservableCollection<int>(_dataList.Where(p => p.IsSelected).Select(p => Convert.ToInt32(p.ID)).ToList());

            

            DataList.Clear();

            if (AppTypeId == "1")
            {
                _scenarioAccess.GetPromotionData(statusIDs, CustomerIDs, ProductIDs, _selectedStartDate.GetValueOrDefault(), _selectedEndDate.GetValueOrDefault(), _idSalesOrg, AppTypeId, AppName, ScenarioId)
              .ContinueWith(t => ApplyFilterContinuation(t, Equals(obj, Check), SelectedPromotions), App.Scheduler);
            }
            else
            {
                _scenarioAccess.GetRobs(AppTypeId, statusIDs, CustomerIDs, ProductIDs, _selectedStartDate.GetValueOrDefault(), _selectedEndDate.GetValueOrDefault(), _idSalesOrg, AppTypeId, AppName, ScenarioId)
                .ContinueWith(t => ApplyFilterContinuation(t, Equals(obj, Check), SelectedPromotions), App.Scheduler);
            }


        }

        private void ApplyFilterContinuation(Task<IList<Rob>> task, bool check, IList<int> selectedPromotionIds)
        {
            _dataList.AddRange(task.Result);
            NotifyPropertyChanged(this, vm => vm.DataList);
            Results = _dataList.Count();
            NotifyPropertyChanged(this, vm => vm.Results);
            //if (check)
            //{
            //    foreach (var data in _dataList.Where(data =>
            //        SelectedPromotions != null
            //        &&  SelectedPromotions.Contains(int.Parse(data.ID))))
            //        data.IsSelected = true;
            //}
            //else
            //{
            foreach (var data in _dataList.Where(p => selectedPromotionIds.Contains(Convert.ToInt32(p.ID))))
                data.IsSelected = true;
            //}
            IsSelectAllChecked = !(_dataList.Any(x => !x.IsSelected));
            IsSelectAllEnabled = _dataList != null && _dataList.Count > 0;
            IsPromoDataLoading = false;

            //foreach (var rob in _dataList)
            //{
            //    _edit = new ViewCommand(Edit);
            //}
            foreach (var r in _dataList)
            {
                r.Code = AppTypeId + "," + AppName;
            }

            IsPromoDataLoading = false;
        }


        //public ICommand EditCommand
        //{
        //    get { return _edit; }
        //}

        //private   ViewCommand _edit;
        //private void Edit(object o)
        //{
        //    var rob = (Rob)o;
        //    if (AppTypeId != "1")
        //    {

        //        _scenarioAccess.GetRob(AppTypeId, rob.ID)
        //               .ContinueWith(EditRobContinuation, App.Scheduler);
        //    }
        //    else
        //    {
        //        PromotionWizardViewModelBase editingPromoWizardViewModel = new PaymentWizardViewModel(null, rob.ID);
        //    }


        //}
        //private void EditRobContinuation(Task<Rob> task)
        //{
        //    if (task.IsFaulted)
        //    {
        //        Messages.Instance.PutError(task.Exception.AggregateMessages());
        //        return;
        //    }
        //    if ((!task.IsCanceled) && task.Result != null)
        //    {

        //            var page = new EventPage(EventViewModel.FromRob(AppTypeId, AppName, task.Result));
        //            MessageBus.Instance.Publish(new NavigateMessage(page));

        //    }
        //}
    }
}
