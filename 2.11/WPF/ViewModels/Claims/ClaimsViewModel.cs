
#region using
using System.Data;

using Model;
using Model.DTOs;
using Model.DataAccess;
using Model.Entity;
using Exceedra.Common.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ViewHelper;
using ViewModels;
using WPF.ViewModels.Scenarios;
using WPF.Pages;
using WPF.ViewModels.Claims.EventStatusV2;
using System.Xml.Linq;
using Coder.UI.WPF;
using Coder.WPF.UI;
using Exceedra.Common;
using Exceedra.Controls.DynamicGrid.ViewModels;

using Exceedra.DynamicGrid.Models;
using Exceedra.Controls.DynamicGrid.Models;
using Exceedra.Controls.Messages;
using Model.DataAccess.Generic;
using Model.DataAccess.Listings;
using Model.Entity.Listings;
using WPF.UserControls.Listings;
using WPF.UserControls.Trees.DataAccess;
using WPF.UserControls.Trees.ViewModels;
using CacheObject = WPF.ViewModels.Generic.CacheObject;

#endregion

namespace WPF.ViewModels.Claims
{
    public class ClaimsViewModel : ViewModelBase
    {
        #region private fields
        public bool _reset { get; set; }
        private readonly ClaimsAccess _claimsAccess = new ClaimsAccess();
        private static IDataTreeViewNodeEventsConsumer _dataTreeEventsConsumer;
        private bool _canMatchManually;
        private bool _canMatchAutomatically;
        private bool _isDataLoading;
        private bool _isSearchClaimDateSelected;
        private bool _isSearchEnteredDateSelected;
        private bool _isClaimsSelectAllChecked = false;
        private bool _isEventsSelectAllChecked = false;
        private bool _canAddClaimEntries = false;

        private DateTime? _claimDateFrom;
        private DateTime? _claimDateTo;
        private DateTime? _eventDateFrom;
        private DateTime? _eventDateTo;

        private RecordViewModel _eventsDynamicGrid = new RecordViewModel(false);
        private RecordViewModel _claimsDynamicGrid = new RecordViewModel(false);


        private readonly ViewCommand _automaticallyMatchCommand;
        private readonly ViewCommand _manualMatchCommand;
        private readonly ViewCommand _saveCommand;
        private readonly ViewCommand _cancelCommand;
        private readonly ViewCommand _editClaimCommand;
        private readonly ViewCommand _editEventCommand;
        private readonly ViewCommand _settleEventsCommand;
        private readonly ViewCommand _approvePaymentsCommand;
        private readonly ViewCommand _saveReviewMatchesCommand;
        private readonly ViewCommand _cancelReviewMatchesCommand;
        private readonly ViewCommand _addClaimEntriesCommand;
        private readonly ViewCommand _addNewClaimRowCommand;
        private SaveMatchesDTO _storedMatches = new SaveMatchesDTO();
        private SalesOrgDataViewModel _selectedSalesOrg;
        private VisibleMatchDataViewModel _selectedVisibleMatch;
        private ISearchableTreeViewNodeEventsConsumer _eventsConsumer;
        private ObservableCollection<ClaimMatchingStatusViewModel> _claimMatchStatusList = new ObservableCollection<ClaimMatchingStatusViewModel>();

        private ObservableCollection<ClaimStatusViewModel> _claimStatusList = new ObservableCollection<ClaimStatusViewModel>();
        private ObservableCollection<EventStatusViewModel> _eventStatusList = new ObservableCollection<EventStatusViewModel>();
        private ObservableCollection<EventTypeViewModel> _eventTypeList = new ObservableCollection<EventTypeViewModel>();
        private ObservableCollection<ClaimEntryEventViewModel> _claimEntryEventList = new ObservableCollection<ClaimEntryEventViewModel>();
        private BulkObservableCollection<ClaimReturnedMatchesViewModel> _claimReturnedMatchesViewModel = new BulkObservableCollection<ClaimReturnedMatchesViewModel>();
        private BulkObservableCollection<ClaimReturnedMatchesViewModel> _claimReturnedMatchesViewModelMaster = new BulkObservableCollection<ClaimReturnedMatchesViewModel>();
        private ObservableCollection<EventGridItemViewModel> _appliedEventList = new ObservableCollection<EventGridItemViewModel>();

        //private int _claimEntryIdsCount = 0;

        private string _userSetDefaultsSalesOrg_Idx;
        private string _claimReferenceFilter;
        private string _claimLineDetailFilter;
        private string _eventNameFilter;
        // private bool _canSaveMatches = true;
        private bool _loadMatchesTab = false;

        #endregion

        #region properties

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

        public string UserSetDefaultSalesOrg_Idx
        {
            get { return _userSetDefaultsSalesOrg_Idx; }
            set
            {
                _userSetDefaultsSalesOrg_Idx = value;
                NotifyPropertyChanged(this, vm => vm.UserSetDefaultSalesOrg_Idx);
            }
        }

        public string ClaimReferenceFilter
        {
            get { return _claimReferenceFilter; }
            set
            {
                _claimReferenceFilter = value;
                NotifyPropertyChanged(this, vm => vm.ClaimReferenceFilter);
            }
        }

        public string ClaimLineDetailFilter
        {
            get { return _claimLineDetailFilter; }
            set
            {
                _claimLineDetailFilter = value;
                NotifyPropertyChanged(this, vm => vm.ClaimLineDetailFilter);
            }
        }

        public string EventNameFilter
        {
            get { return _eventNameFilter; }
            set
            {
                _eventNameFilter = value;
                NotifyPropertyChanged(this, vm => vm.EventNameFilter);
            }
        }

        public bool LoadMatchesTab
        {
            get { return _loadMatchesTab; }
            set
            {
                _loadMatchesTab = value;
                NotifyPropertyChanged(this, vm => vm.LoadMatchesTab);
            }
        }

        public bool CanMatchManually
        {
            get { return _canMatchManually; }
            set
            {
                _canMatchManually = value;
                NotifyPropertyChanged(this, vm => vm.CanMatchManually);
            }
        }

        public bool CanMatchAutomatically
        {
            get { return _canMatchAutomatically; }
            set
            {
                _canMatchAutomatically = value;
                NotifyPropertyChanged(this, vm => vm.CanMatchAutomatically);
            }
        }

        public bool IsDataLoading
        {
            get { return _isDataLoading; }
            set
            {
                _isDataLoading = value;
                NotifyPropertyChanged(this, vm => vm.IsDataLoading);
            }
        }

        public RecordViewModel ClaimsDynamicGrid
        {
            get { return _claimsDynamicGrid; }
            set
            {
                _claimsDynamicGrid = value;
                NotifyPropertyChanged(this, vm => vm.ClaimsDynamicGrid);
            }
        }

        public RecordViewModel EventsDynamicGrid
        {
            get { return _eventsDynamicGrid; }
            set
            {
                _eventsDynamicGrid = value;
                NotifyPropertyChanged(this, vm => vm.EventsDynamicGrid);
            }
        }

        private RecordViewModel _claimsEntryDynamicGrid;
        public RecordViewModel ClaimsEntryDynamicGrid
        {
            get { return _claimsEntryDynamicGrid; }
            set
            {
                _claimsEntryDynamicGrid = value;
                NotifyPropertyChanged(this, vm => vm.ClaimsEntryDynamicGrid);
            }
        }

        public static int Loaded;

        public bool CheckCanApplyFilters(object sender = null)
        {
            return
                StatusTree.GetSelectedNodes().Any()
                && Convert.ToInt32(ClaimsUpperVlaue) >= Convert.ToInt32(ClaimsLowerValue)
                && ListingsVM != null
                && ListingsVM.FilterCheckAndUpdate()
                && ClaimDateTo >= ClaimDateFrom
                && EventDateTo >= EventDateFrom
                ;

        }

        private readonly TreeAccess _treeAccess = new TreeAccess();
        private TreeViewModel _statusTree;
        public TreeViewModel StatusTree
        {
            get { return _statusTree; }
            set
            {
                _statusTree = value;
                NotifyPropertyChanged(this, vm => vm.StatusTree);
            }
        }



        private List<TreeViewHierarchy> _staticStatuses;

        public List<TreeViewHierarchy> StaticStatuses
        {
            get { return _staticStatuses; }
            set
            {
                _staticStatuses = value;
                NotifyPropertyChanged(this, vm => vm.StaticStatuses);
            }
        }
        public ObservableCollection<ClaimMatchingStatusViewModel> ClaimMatchStatusList
        {
            get { return _claimMatchStatusList; }
            set
            {
                _claimMatchStatusList = value;
                NotifyPropertyChanged(this, vm => vm.ClaimMatchStatusList);
            }
        }

        public ObservableCollection<ClaimStatusViewModel> ClaimStatusList
        {
            get { return _claimStatusList; }
            set
            {
                _claimStatusList = value;
                NotifyPropertyChanged(this, vm => vm.ClaimStatusList);
            }
        }


        public ObservableCollection<EventStatusViewModel> EventStatusList
        {
            get { return _eventStatusList; }
            set
            {
                _eventStatusList = value;
                NotifyPropertyChanged(this, vm => vm.EventStatusList);
            }
        }

        public ObservableCollection<EventTypeViewModel> EventTypeList
        {
            get { return _eventTypeList; }
            set
            {
                _eventTypeList = value;
                NotifyPropertyChanged(this, vm => vm.EventTypeList);
            }
        }

        public ObservableCollection<ClaimEntryEventViewModel> ClaimEntryEventList
        {
            get { return _claimEntryEventList; }
            set
            {
                _claimEntryEventList = value;
                NotifyPropertyChanged(this, vm => vm.ClaimEntryEventList);
            }
        }

        public DateTime? ClaimDateFrom
        {
            get { return _claimDateFrom; }
            set
            {
                _claimDateFrom = value;
                NotifyPropertyChanged(this, vm => vm.ClaimDateFrom);
                UpdateDelistingsDate();
            }
        }

        public DateTime? ClaimDateTo
        {
            get { return _claimDateTo; }
            set
            {
                _claimDateTo = value;
                NotifyPropertyChanged(this, vm => vm.ClaimDateTo);
            }
        }

        public DateTime? EventDateFrom
        {
            get { return _eventDateFrom; }
            set
            {
                _eventDateFrom = value;
                NotifyPropertyChanged(this, vm => vm.EventDateFrom);
                UpdateDelistingsDate();
            }
        }

        public DateTime? EventDateTo
        {
            get { return _eventDateTo; }
            set
            {
                _eventDateTo = value;
                NotifyPropertyChanged(this, vm => vm.EventDateTo);
            }
        }

        public bool IsSearchClaimDateSelected
        {
            get { return _isSearchClaimDateSelected; }
            set
            {
                _isSearchClaimDateSelected = value;
                NotifyPropertyChanged(this, vm => vm.IsSearchClaimDateSelected);
            }
        }

        public bool IsSearchEnteredDateSelected
        {
            get { return _isSearchEnteredDateSelected; }
            set
            {
                _isSearchEnteredDateSelected = value;
                NotifyPropertyChanged(this, vm => vm.IsSearchEnteredDateSelected);
            }
        }


        public BulkObservableCollection<ClaimReturnedMatchesViewModel> ClaimsReturnedMatches
        {
            get { return _claimReturnedMatchesViewModel; }
            set
            {
                if (_claimReturnedMatchesViewModel != value)
                {
                    _claimReturnedMatchesViewModel = value;
                    NotifyPropertyChanged(this, vm => vm.ClaimsReturnedMatches);
                }
            }
        }

        public BulkObservableCollection<ClaimReturnedMatchesViewModel> ClaimsReturnedMatchesMaster
        {
            get { return _claimReturnedMatchesViewModelMaster; }
            set
            {
                if (_claimReturnedMatchesViewModelMaster != value)
                {
                    _claimReturnedMatchesViewModelMaster = value;
                    NotifyPropertyChanged(this, vm => vm.ClaimsReturnedMatchesMaster);
                }
            }
        }

        private bool _saveReviewMatches;
        public bool SaveReviewMatches
        {
            get { return _saveReviewMatches; }
            set
            {
                _saveReviewMatches = value;
                NotifyPropertyChanged(this, vm => vm.SaveReviewMatches);
            }
        }


        public ICommand SetUserDefaultCommand
        {
            get
            {
                return new ViewCommand(CheckCanApplyFilters, SetUserDefault);
            }
        }

        public ICommand ApplyFiltersCommand
        {
            get
            {
                return new ViewCommand(CheckCanApplyFilters, ApplyFilters);
            }
        }

        public ICommand AutomaticallyMatchCommand
        {
            get { return _automaticallyMatchCommand; }
        }

        public ICommand ManualMatchCommand
        {
            get { return _manualMatchCommand; }
        }

        public ICommand SaveCommand
        {
            get { return _saveCommand; }
        }

        public ICommand CancelCommand
        {
            get { return _cancelCommand; }
        }

        public ICommand EditClaimCommand
        {
            get { return _editClaimCommand; }
        }

        public ICommand EditEventCommand
        {
            get { return _editEventCommand; }
        }

        /// <summary>
        /// Should be calling applyfilterscommand as well
        /// </summary>
        public ICommand SettleEventsCommand
        {
            get { return _settleEventsCommand; }
        }

        /// <summary>
        /// Should be calling applyfilterscommand as well
        /// </summary>
        public ICommand ApprovePaymentsCommand
        {
            get { return _approvePaymentsCommand; }
        }

        public ICommand SaveReviewMatchesCommand
        {
            get { return _saveReviewMatchesCommand; }
        }

        public ICommand CancelReviewMatchesCommand
        {
            get { return _cancelReviewMatchesCommand; }
        }

        public ICommand AddClaimEntriesCommand
        {
            get { return _addClaimEntriesCommand; }
        }

        public ICommand AddNewClaimRowCommand
        {
            get { return _addNewClaimRowCommand; }
        }

        private string _displayCustomerCount;
        public string DisplayCustomerCount
        {
            get
            { return _displayCustomerCount; }

            set
            {
                _displayCustomerCount = value;
                NotifyPropertyChanged(this, vm => vm.DisplayCustomerCount);

            }
        }

        private string _displayProductCount;

        public string DisplayProductCount
        {
            get { return _displayProductCount; }
            set
            {
                _displayProductCount = value;
                NotifyPropertyChanged(this, vm => vm.DisplayProductCount);
            }
        }

        public IList<string> SelectedMatchingStatusIds
        {
            get { return ClaimMatchStatusList.Where(m => m.IsSelected && m.Data.Id != ClaimMatchStatus.SelectAllId).Select(mt => mt.Data.Id).ToList(); }
        }

        public IList<string> SelectedClaimStatusIds
        {
            get { return ClaimStatusList.Where(c => c.IsSelected && c.Data.Id != ClaimStatus.SelectAllId).Select(cl => cl.Data.Id).ToList(); }
        }

        public IList<string> SelectedEventIds
        {
            get { return EventTypeList.Where(e => e.IsSelected && e.Data.Id != EventType.SelectAllId).Select(ev => ev.Data.Id).ToList(); }
        }


        public IList<string> SelectedEventStatusIds { get { return EventStatusList.Where(c => c.IsSelected && c.Data.Id != EventStatus.SelectAllId).Select(cl => cl.Data.Id).ToList(); } }

        public bool IsClaimsSelectAllChecked
        {
            get { return _isClaimsSelectAllChecked; }
            set
            {
                _isClaimsSelectAllChecked = value;
                NotifyPropertyChanged(this, vm => vm.IsClaimsSelectAllChecked);
            }
        }

        public bool IsEventsSelectAllChecked
        {
            get { return _isEventsSelectAllChecked; }
            set
            {
                _isEventsSelectAllChecked = value;
                NotifyPropertyChanged(this, vm => vm.IsEventsSelectAllChecked);
            }
        }

        public bool CanAddClaimEntries
        {
            get
            {
                return _canAddClaimEntries;
            }
            set
            {
                _canAddClaimEntries = value;
                NotifyPropertyChanged(this, vm => vm.CanAddClaimEntries);
            }
        }

        public ReturnedMatchDTO AllReturnedMatchDTO { get; set; }

        #endregion

        #region events
        public delegate void AddClaimEntriesError(string errorMessage);
        public event AddClaimEntriesError OnAddClaimEntriesError;
        #endregion

        #region ctors
        private ClaimsViewModel()
        {
            PageTitle = App.Configuration.GetScreens().Single(f => f.Key == ScreenKeys.CLAIM.ToString()).Label;

            LoadClaimStatusList();


            _manualMatchCommand = new ViewCommand(CanMatch, MatchManually);
            _automaticallyMatchCommand = new ViewCommand(MatchAutomatically);
            _cancelCommand = new ViewCommand(Cancel);
            _editClaimCommand = new ViewCommand(EditClaim);
            _editEventCommand = new ViewCommand(EditEvent);
            _settleEventsCommand = new ViewCommand(CanSettleEvents, ReturnedMatchSettleEvent);
            _approvePaymentsCommand = new ViewCommand(CanApprovePayments, ReturnedMatchApprovePayments);
            _saveReviewMatchesCommand = new ViewCommand(CanSaveReturnedMatch, ReturnedMatchSave);
            _cancelReviewMatchesCommand = new ViewCommand(ReturnedMatchCancel);
            _addClaimEntriesCommand = new ViewCommand(IsAllClaimEntriesValid, ManuallyAddClaimEntries);
          //  _addNewClaimRowCommand = new ViewCommand(CanAddClaimRows, LoadClaimsEntry);

            LoadListings();

            SetClaimsRanges();
        }

        //private void LoadClaimsEntry(object obj)
        //{
        //    LoadClaimsEntry2();            
        //}

        private void LoadClaimStatusList()
        {
            StatusTree = DynamicDataAccess.GetGenericItem<TreeViewModel>(StoredProcedure.Claims.GetFilterStatuses, CommonXml.GetBaseArguments("GetStatuses"));
        }

        public static ClaimsViewModel New()
        {
            var instance = new ClaimsViewModel();

            instance.Init();
            return instance;
        }

        #endregion

        #region private methods

        #region Init

        private void Init()
        {
            IsDataLoading = true;

            userDefaultsHaveBeenInitiated = false;
            salesOrganisationsHaveBeenInitiated = false;
            claimMatchStatusHaveBeenInitiated = false;
            claimStatusHaveBeenInitiated = false;
            valueRangesHaveBeenInitiated = false;
            eventTypesHaveBeenInitiated = false;

            List<Task> tasks = new List<Task>();

            LoadFromDefault();

            if (App.AppCache.GetItem("Claims_GetUserDefaults") == null)
            {
                tasks.Add(_claimsAccess.GetUserDefaults().ContinueWith(GetUserDefaultsContinuation, App.Scheduler));
            }
            else
            {
                GetUserDefaultContinuationResult(App.AppCache.GetItem("Claims_GetUserDefaults").obj as ClaimUserDefaults);
            }

            GetSalesOrgContinuationResult();
        }



        public bool userDefaultsHaveBeenInitiated = false;
        public bool salesOrganisationsHaveBeenInitiated = false;
        public bool claimMatchStatusHaveBeenInitiated = false;
        public bool claimStatusHaveBeenInitiated = false;
        public bool valueRangesHaveBeenInitiated = false;
        public bool eventTypesHaveBeenInitiated = false;


        #endregion

        #region MatchAutomatically

        private void MatchAutomatically(object parameter)
        {
            CanMatchAutomatically = false;
            CanMatchManually = false;
            AutomaticMatchDTO automaticMatchDTO = new AutomaticMatchDTO();
            var items = ClaimsDynamicGrid.Records.Select(r => r.SelectedItems).ToList();
            automaticMatchDTO.ClaimIds = items.Where(r => r.HasValue() == true).ToList();
            automaticMatchDTO.EventIds = EventsDynamicGrid.Records.Select(r => r.SelectedItems).Where(r => r.HasValue()).ToList();
            var tasks = new[]
            {
                _claimsAccess.GetAutomaticMatches(automaticMatchDTO).ContinueWith(MatchAutomaticallyContinuation, App.Scheduler)
            };
        }


        private void MatchAutomaticallyContinuation(Task<SprocResult> task)
        {
            if (task.Status == TaskStatus.Canceled || task.Status == TaskStatus.Faulted || task.Result == null)
            {
                return;
            }

            var result = task.Result;
            if (result.Success)
            {
                _claimsAccess.ReturnMatches(GetClaimMatchDTO(ReturnMatchType.AutomaticMatch)).ContinueWith(t =>
                {
                    GetReturnedMatchesContinuation(t.Result);
                    ResetAfterMatch();
                    CustomMessageBox.Show(result.Message, "Automatic Matching", MessageBoxButton.OK, MessageBoxImage.Information);
                }, App.Scheduler);
            }
            else
            {
                CustomMessageBox.Show(result.Message, "Automatic Matching", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        #endregion

        #region Reset After Match

        private void ResetAfterMatch()
        {
            ClearSelectedClaimsAndEvents();
            CanMatchAutomatically = false;
            CanMatchManually = false;
        }

        #endregion

        #region MatchManually

        private DateTime _lastManualMatch;
        private bool _isMatchingManually;
        private void MatchManually(object parameter)
        {
            // JS: Even though CanMatch() was checking for _isMatchingManually to be false in order to disable the button,
            // disabling wasn't happening instantly so it was possible to click on the button twice before it actually went disabled.
            // The only one solution for that issue I've found was to use a datetime variable.
            var a = (DateTime.Now.Subtract(_lastManualMatch)).TotalMilliseconds;
            if (a <= 500)
                return;

            try
            {
                _isMatchingManually = true;
                _lastManualMatch = DateTime.Now;

                CanMatchAutomatically = false;
                CanMatchManually = false;

                var task = new Task(() => UpdateMatchesAfterManulMatch());
                task.Start();
                task.ContinueWith(t1 =>
                {
                    _claimsAccess.SaveMatches(_storedMatches).ContinueWith(SaveContinuation, App.Scheduler);

                });
            }
            finally
            {
                _isMatchingManually = false;
            }
        }

        private void UpdateMatchesAfterManulMatch()
        {
            var items = ClaimsDynamicGrid.Records.Select(r => r.SelectedItems).ToList();
            var selectedClaims = items.Where(r => r.HasValue() == true).ToList();

            var selectedEvents = EventsDynamicGrid.Records.Select(r => r.SelectedItems).Where(r => r.HasValue()).ToList();

            foreach (var claimItem in selectedClaims)
            {
                bool claimExists = _storedMatches.ClaimIds.Contains(claimItem);
                if (claimExists == false)
                {
                    _storedMatches.ClaimIds.Add(claimItem);
                }

                foreach (var eventItem in selectedEvents)
                {
                    bool matchExists = _storedMatches.Matches.Any(m => m.EventId == eventItem && m.ClaimId == claimItem);
                    bool eventExists = _storedMatches.EventIds.Contains(eventItem);
                    if (matchExists == false)
                    {
                        _storedMatches.Matches.Add(new ClaimEventMatch() { ClaimId = claimItem, EventId = eventItem });
                    }
                    if (eventExists == false)
                    {
                        _storedMatches.EventIds.Add(eventItem);
                    }
                }
            }
        }

        #endregion

        #region Navigate

        private void EditClaim(object currentClaim)
        {
            var claim = (ClaimGridItemViewModel)currentClaim;
            var page = new ClaimPage(claim.Data.Id);
            MessageBus.Instance.Publish(new NavigateMessage(page));
        }

        private void EditEvent(object currentEvent)
        {
            var eventItem = (EventGridItemViewModel)currentEvent;
            var page = new EventEditorPage(eventItem.Data.Id);
            MessageBus.Instance.Publish(new NavigateMessage(page));
        }

        #endregion

        #region Save

        private void SaveContinuation(Task<SaveMatchesResult> task)
        {
            if (task.Status == TaskStatus.Canceled || task.Status == TaskStatus.Faulted || task.Result == null)
                return;

            var tasks = new[]
            {
                _claimsAccess.ReturnMatches(GetClaimMatchDTO(ReturnMatchType.ManualMatch)).ContinueWith(t=>GetReturnedMatchesContinuation(t.Result), App.Scheduler)
            };

            Task.Factory.ContinueWhenAll(tasks, ts =>
            {
                _storedMatches = new SaveMatchesDTO();
                ResetAfterMatch();
                CustomMessageBox.Show(task.Result.Message, "Saving Matches", MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }

        #endregion

        #region Matching StaticStatuses
        private void GetMatchingStatusesContinuation(Task<IList<ClaimMatchStatus>> task)
        {
            task.Result.Insert(0, new SelectAllClaimMatchingStatus(SelectAllClaimMatchingStatuses, DeselectAllClaimMatchingStatuses) { IsSelected = task.Result.All(s => s.IsSelected) });

            GetMatchingStatusesContinuationResult(task.Result);

        }

        private void GetMatchingStatusesContinuationResult(IList<ClaimMatchStatus> result)
        {
            if (result.Count(r => r.Id == "[ALL]") != 0)
            {
                result.RemoveAt(0);
            }

            result.Insert(0, new SelectAllClaimMatchingStatus(SelectAllClaimMatchingStatuses, DeselectAllClaimMatchingStatuses) { IsSelected = result.All(s => s.IsSelected) });

            ClaimMatchStatusList = new ObservableCollection<ClaimMatchingStatusViewModel>(result.Select(cs => new ClaimMatchingStatusViewModel(cs)));

            foreach (var claimMatchStatusItem in ClaimMatchStatusList)
            {
                claimMatchStatusItem.SelectedClaimMatchingStatusChanged += UpdateMatchingStatusesSelection;
            }
            if (StatusTree.GetSelectedNodes().Any())
            {
                claimMatchStatusHaveBeenInitiated = true;
            }

            App.AppCache.Upsert("Claims_GetClaimMatchStatuses", result);

            if (CheckCanApplyFilters() && Loaded == 0) AppApplyFilters(new object());
        }


        private void UpdateMatchingStatusesSelection(object sender, EventArgs e)
        {
            var selectAllItem = ClaimMatchStatusList.SingleOrDefault(c => (c.Data.Id == SelectAllClaimMatchingStatus.IDValue));
            if (selectAllItem != null)
            {
                bool anyUnticked = ClaimMatchStatusList.Where(c => (c.Data.Id != SelectAllClaimMatchingStatus.IDValue)).Any(c => c.IsSelected == false);
                selectAllItem.SetSelected(!anyUnticked);
            }
            NotifyPropertyChanged(this, vm => vm.ClaimMatchStatusList);

        }
        #endregion

        #region Cancel

        private void Cancel(object parameter)
        {
            if (CustomMessageBox.Show("Do you wish to cancel this operation?", "Cancel", MessageBoxButton.YesNo,
                MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                var page = new Pages.Claims();
                MessageBus.Instance.Publish(new NavigateMessage(page));
            }
        }

        #endregion


        #region Get User Default

        private void GetUserDefaultsContinuation(Task<ClaimUserDefaults> task)
        {
            if (task.Status == TaskStatus.Canceled || task.Status == TaskStatus.Faulted || task.Result == null)
                return;
            GetUserDefaultContinuationResult(task.Result);
        }

        private void GetUserDefaultContinuationResult(ClaimUserDefaults userDefault)
        {
            if (userDefault.ClaimStartDate != DateTime.MinValue)
            {
                ClaimDateFrom = userDefault.ClaimStartDate;
            }

            if (userDefault.ClaimEndDate != DateTime.MinValue)
            {
                ClaimDateTo = userDefault.ClaimEndDate;
            }

            if (userDefault.EventStartDate != DateTime.MinValue)
            {
                EventDateFrom = userDefault.EventStartDate;
            }

            if (userDefault.EventEndDate != DateTime.MinValue)
            {
                EventDateTo = userDefault.EventEndDate;
            }

            UserSetDefaultSalesOrg_Idx = userDefault.SalesOrg_Idx;
            userDefaultsHaveBeenInitiated = true;

            IsSearchClaimDateSelected = string.Equals(userDefault.DateSearchPreference.Trim(), "0");
            IsSearchEnteredDateSelected = !IsSearchClaimDateSelected;

            App.AppCache.Upsert("Claims_GetUserDefaults", userDefault);
        }

        #endregion

        #region Set User Default

        private void SetUserDefault(object parameter)
        {
            if (!CheckCanApplyFilters())
            {
                return;
            }
            SetDefaultFiltersDTO setDefaulFiltersDto = new SetDefaultFiltersDTO();
            setDefaulFiltersDto.ClaimFilterMax = ClaimsUpperVlaue;
            setDefaulFiltersDto.ClaimFilterMin = ClaimsLowerValue;
            setDefaulFiltersDto.ClaimMatchingStatusIds = SelectedMatchingStatusIds;
            setDefaulFiltersDto.Statuses = StatusTree.GetSelectedIdxs();
            setDefaulFiltersDto.CustomerIds = ListingsVM.CustomerIDsList;
            setDefaulFiltersDto.ProductIds = ListingsVM.ProductIDsList;
            setDefaulFiltersDto.SalesOrgId = Model.User.CurrentUser.SalesOrganisationID.ToString();
            setDefaulFiltersDto.ListingsGroupIdx = ListingsVM.ListingGroups.SelectedItem.Idx;

            if (ClaimDateTo.HasValue)
            {
                setDefaulFiltersDto.ClaimEndDate = ClaimDateTo.Value;
            }

            if (ClaimDateFrom.HasValue)
            {
                setDefaulFiltersDto.ClaimStartDate = ClaimDateFrom.Value;
            }
            setDefaulFiltersDto.DateSearchPreference = IsSearchClaimDateSelected ? "0" : "1";
            if (EventDateFrom.HasValue)
            {
                setDefaulFiltersDto.EventStartDate = EventDateFrom.Value;
            }

            if (EventDateTo.HasValue)
            {
                setDefaulFiltersDto.EventEndDate = EventDateTo.Value;
            }

            setDefaulFiltersDto.EventTypeIds = SelectedEventIds;

            _claimsAccess.SetDefaultFilters(setDefaulFiltersDto).ContinueWith(SetUserDefaultsContinuation, App.Scheduler);

        }

        private void SetUserDefaultsContinuation(Task<SprocResult> task)
        {
            if (task.Result.Success == false)
            {
                MessageBoxShow(string.Format("Attempt to set user defaults failed.{0}{1}", Environment.NewLine, task.Result.Message),
                                              "Set User Defaults", MessageBoxButton.OK, MessageBoxImage.Warning);

            }
            else
            {
                MessageBoxShow(task.Result.Message, "Set User Defaults", MessageBoxButton.OK);
            }

            //Reset the listings, status and value range caches
            _listingsAccess.PopulateGeneric();
            _treeAccess.GetClaimsStatuses(true);

            SetClaimsRanges(true);

            //reload the user defaults to appcache
            _claimsAccess.GetUserDefaults().ContinueWith(t =>
            {
                App.AppCache.Upsert("Claims_GetUserDefaults", t.Result);
            }, App.Scheduler);

        }

        #endregion

        #region Clear Selected Claims/Events

        private void ClearSelectedClaimsAndEvents()
        {
            ClaimsDynamicGrid.Records.ToList().ForEach(c => c.DeselectAll());
            EventsDynamicGrid.Records.Do(r => r.DeselectAll());

            IsClaimsSelectAllChecked = false;
            IsEventsSelectAllChecked = false;
            NotifyPropertyChanged(this, vm => vm.EventsDynamicGrid, vm => vm.ClaimsDynamicGrid, vm => vm.IsClaimsSelectAllChecked, vm => vm.IsEventsSelectAllChecked);
        }

        #endregion

        #region De/Select Matching StaticStatuses

        private void SelectAllClaimMatchingStatuses()
        {
            foreach (var status in ClaimMatchStatusList.Where(cs => !(cs.Data is SelectAllClaimMatchingStatus)))
                status.IsSelected = true;
        }

        private void DeselectAllClaimMatchingStatuses()
        {
            foreach (var status in ClaimMatchStatusList.Where(cs => !(cs.Data is SelectAllClaimMatchingStatus)))
                status.IsSelected = false;
        }

        #endregion

        #region Get Returned Matches

        private void GetReturnedMatchesContinuation(IList<ReturnedMatches> returnedMatches)
        {
            AllReturnedMatchDTO = new ReturnedMatchDTO
            {
                ClaimIds = returnedMatches.FirstOrDefault().Claims.Select(claim => claim.GetClaimId()).ToList(),
                EventIds = returnedMatches.FirstOrDefault().Events.Select(eve => eve.GetEventId()).ToList(),
                Matches = returnedMatches.FirstOrDefault().Matches.ToList().Select(x => new ClaimEventMatch { ClaimId = x.ClaimId, EventId = x.EventId }).ToList()
            };

            foreach (var returnedMatch in returnedMatches)
            {
                if (returnedMatch.Matches != null)
                {
                    List<MatchItemEvent> events = returnedMatch.Events.Where(x => returnedMatch.Matches.Any(y => y.EventId == x.GetEventId())).ToList();
                    foreach (MatchItemEvent eventItem in events)
                    {
                        DataTable eventTable = ClaimReturnedMatchesViewModel.GetDetailedTableFromEvent(eventItem);
                        ReturnedMatches firstMatch = returnedMatches.FirstOrDefault();
                        ClaimReturnedMatchesViewModel claimReturnedMatchesViewModel = new ClaimReturnedMatchesViewModel(_dataTreeEventsConsumer, null, eventTable, eventItem, firstMatch, false, false);
                        bool eventExists = ClaimsReturnedMatchesMaster.Any(ev => ev.MatchEvent.GetEventId() == eventItem.GetEventId());
                        if (eventExists == false)
                        {
                            ClaimsReturnedMatchesMaster.Add(claimReturnedMatchesViewModel);
                        }
                        else
                        {

                            var curentClaimReturnedMatchesViewModel = ClaimsReturnedMatchesMaster.Where(ev => ev.MatchEvent.GetEventId() == eventItem.GetEventId()).SingleOrDefault();
                            curentClaimReturnedMatchesViewModel.Children.FirstOrDefault().DataTreeNodeSource.Merge(claimReturnedMatchesViewModel.Children.FirstOrDefault().DataTreeNodeSource);
                            var UniqueRows = curentClaimReturnedMatchesViewModel.Children.FirstOrDefault().DataTreeNodeSource.AsEnumerable().Distinct(DataRowComparer.Default);
                            curentClaimReturnedMatchesViewModel.Children.FirstOrDefault().DataTreeNodeSource = UniqueRows.CopyToDataTable();
                        }
                    }
                }
            }

            ApplyVisibilityMatchesFilter();
        }
        #endregion

        #region GetSalesOrg



        private void GetSalesOrgContinuationResult()
        {

            try
            {
                LoadClaimsEntry2(5);

                LoadGridDependents();
            }
            catch (Exception ex)
            {
                ClaimsEntryDynamicGrid = null;
            }



            if (CheckCanApplyFilters() && Loaded == 0) AppApplyFilters(new object());
        }

        private void LoadGridDependents()
        {
            if (ClaimsEntryDynamicGrid.Records != null)
            {
                // list of columns (codes) which will not have loaded options initially
                // because they are dependent from others
                var dependentColumns =
                    ClaimsEntryDynamicGrid.Records.SelectMany(x => x.Properties)
                        .Select(y => y.DependentColumn)
                        .Distinct()
                        .ToArray();
                ;

                // loading options for properties that are dropdowns
                // and are not dependent from the other columns
                foreach (var col in ClaimsEntryDynamicGrid.Records)
                    foreach (var p in col.Properties.Where(r =>
                        r.ControlType.Contains("down") &&
                        !dependentColumns.Contains(r.ColumnCode)
                        ).ToList())
                        col.InitialDropdownLoad(p, true);

                ClaimsEntryDynamicGrid.IsDataLoading = false;
            }
        }

        #endregion

        private   RecordViewModel _stored
        {
            get
            {
                return new RecordViewModel(_claimsAccess.AddDynamicClaims(Model.User.CurrentUser.SalesOrganisationID.ToString()));                
            }
        }

        private void LoadClaimsEntry2(int rows)
        {
            ClaimsEntryDynamicGrid = _stored;
            for (int i = 0; i < rows; i++)
            {
                var res = _stored.Records;
                res[0].Item_Idx = (ClaimsEntryDynamicGrid.Records.Count + 1).ToString();

                ClaimsEntryDynamicGrid.Records.AddRange(res);
            }
            //if (ClaimsEntryDynamicGrid == null)
            //{
            //    ClaimsEntryDynamicGrid = _stored;                
            //}
            //else
            //{
            //    var res = _stored.Records;
            //    res[0].Item_Idx = (ClaimsEntryDynamicGrid.Records.Count + 1).ToString();

            //    ClaimsEntryDynamicGrid.Records.AddRange(res);
            //    NotifyPropertyChanged(this, vm => vm.ClaimsEntryDynamicGrid);
            //}

        }

        public bool CanMatch(object returnedMatch)
        {
            if (_isMatchingManually)
                return false;

            bool eventSelected = EventsDynamicGrid != null && EventsDynamicGrid.Records != null && EventsDynamicGrid.Records.Any(r => r.SelectedItems.HasValue());
            bool isEnabled = HasSelectedClaims && eventSelected;
            this.CanMatchAutomatically = isEnabled;
            this.CanMatchManually = isEnabled;
            return isEnabled;


        }

        #region Apply Events Continuation



        private void UpdateEventSelection(object sender, EventArgs e)
        {
            IsEventsSelectAllChecked = !EventsDynamicGrid.Records.All(r => r.SelectedItems.HasValue());
            NotifyPropertyChanged(this, vm => vm.IsEventsSelectAllChecked);
        }

        #endregion

        #region AppApplyFilters

        private void ApplyFilters(object obj = null)
        {
            _reset = true;
            AppApplyFilters(null);
        }

        private void AppApplyFilters(object obj)
        {
            if (CheckCanApplyFilters())
            {

                var claims = App.AppCache.GetItem("ClaimsList_Claims");
                var events = App.AppCache.GetItem("ClaimsList_Events");

                if (_reset || (claims == null || events == null))
                {
                    var tasks = new[]
                    {
                        _claimsAccess.ReturnEvents(GetApplyEventsDTO()).ContinueWith(ApplyEventsContinuation, App.Scheduler),
                        _claimsAccess.GetClaims(GetApplyClaimsDTO()).ContinueWith(ApplyClaims, App.Scheduler)

                    };
                    _reset = false;
                    Task.Factory.ContinueWhenAll(tasks, ts => ApplyFiltersComplete());
                }
                else
                {
                    LoadFromCache(claims, events);
                    ApplyFiltersComplete();
                }
            }
        }

        private void LoadFromDefault()
        {
            var defaultClaimXml = CommonXml.GetBaseArguments("GetClaims");
            var defaultEventXml = CommonXml.GetBaseArguments("GetEvents");
            defaultClaimXml.AddElement("LoadFromDefaults", "1");
            defaultEventXml.AddElement("LoadFromDefaults", "1");

            var tasks = new[]
            {
                _claimsAccess.ReturnEvents(defaultEventXml).ContinueWith(ApplyEventsContinuation, App.Scheduler),
                _claimsAccess.GetClaims(defaultClaimXml).ContinueWith(ApplyClaims, App.Scheduler)
            };
            _reset = false;

            Task.Factory.ContinueWhenAll(tasks, ts => ApplyFiltersComplete());
        }

        private void LoadFromCache(CacheObject claims, CacheObject events)
        {
            if (claims != null)
            {
                try
                {
                    ClaimsDynamicGrid = claims.obj as RecordViewModel;
                    claims.dt = DateTime.Now;
                }
                catch (Exception ex)
                {
                    CustomMessageBox.Show(ex.Message);
                    ClaimsDynamicGrid = new RecordViewModel();
                }
            }

            if (events != null)
            {
                EventsDynamicGrid = events.obj as RecordViewModel;
            }

        }

        private void ReloadClaims()
        {
            ClaimsDynamicGrid = new RecordViewModel();
            _claimsAccess.GetClaims(GetApplyClaimsDTO()).ContinueWith(ApplyClaims, App.Scheduler);
        }

        private void ApplyClaims(Task<XElement> obj)
        {
            App.AppCache.Upsert("ClaimsList_Claims", new RecordViewModel(obj.Result));
            var ps = App.AppCache.GetItem("ClaimsList_Claims");
            LoadFromCache(ps, null);
        }

        private void ApplyEventsContinuation(Task<XElement> task)
        {
            App.AppCache.Upsert("ClaimsList_Events", new RecordViewModel(task.Result));
            var ps = App.AppCache.GetItem("ClaimsList_Events");
            LoadFromCache(null, ps);
        }

        private void ApplyFiltersComplete()
        {

            _claimsAccess.ReturnMatches(GetClaimMatchDTO(ReturnMatchType.WithoutMatch)).ContinueWith(t =>
            {
                ResetReturnedMatches();
                GetReturnedMatchesContinuation(t.Result);
            }, App.Scheduler);

            _storedMatches = new SaveMatchesDTO();
            this.ClaimLineDetailFilter = string.Empty;
            this.ClaimReferenceFilter = string.Empty;
            this.EventNameFilter = string.Empty;

            Loaded = 1;

        }

        #endregion

        #region ApplyClaimsDTO

        private ReturnClaimsDTO GetApplyClaimsDTO()
        {
            ReturnClaimsDTO applyClaimsDTO = new ReturnClaimsDTO();
            applyClaimsDTO.ClaimFilterMax = ClaimsUpperVlaue;
            applyClaimsDTO.ClaimFilterMin = ClaimsLowerValue;
            applyClaimsDTO.ClaimMatchingStatusIds = SelectedMatchingStatusIds;
            applyClaimsDTO.Statuses = StatusTree.GetSelectedIdxs();
            applyClaimsDTO.CustomerIds = ListingsVM.CustomerIDsList;
            applyClaimsDTO.ProductIds = ListingsVM.ProductIDsList;

            applyClaimsDTO.DateSearchPreference = IsSearchClaimDateSelected ? "0" : "1";
            applyClaimsDTO.EndDate = ClaimDateTo.Value;
            applyClaimsDTO.StartDate = ClaimDateFrom.Value;
            return applyClaimsDTO;
        }

        #endregion

        #region ApplyEventsDTO

        private ReturnEventsDTO GetApplyEventsDTO()
        {
            ReturnEventsDTO applyEventsDTO = new ReturnEventsDTO();
            applyEventsDTO.CustomerIds = ListingsVM.CustomerIDsList;
            applyEventsDTO.ProductIds = ListingsVM.ProductIDsList;
            applyEventsDTO.EndDate = EventDateTo.Value;
            applyEventsDTO.StartDate = EventDateFrom.Value;
            applyEventsDTO.EventTypeIds = SelectedEventIds;
            applyEventsDTO.EventStatusIds = StatusTree.GetSelectedIdxs();
            return applyEventsDTO;
        }

        #endregion

        #region GetEventsDTO

        private GetEventsDTO GetEventsDTO()
        {
            GetEventsDTO getEventsDTO = new GetEventsDTO();
            getEventsDTO.EndDate = EventDateTo.Value;
            getEventsDTO.StartDate = EventDateFrom.Value;
            getEventsDTO.EventTypeIds = SelectedEventIds;
            getEventsDTO.CustomerIds = ListingsVM.CustomerIDsList;
            return getEventsDTO;
        }

        #endregion

        #region Value Ranges

        public void SetClaimsRanges(bool forceReload = false)
        {

            var cache = App.AppCache.GetItem("Claims_Ranges");
            XElement ranges;
            if (cache == null || forceReload)
            {
                _claimsAccess.GetFilterClaimValues().ContinueWith(t =>
                {
                    ranges = t.Result;
                    App.AppCache.Upsert("Claims_Ranges", ranges);
                    SetClaimsRangeValues(ranges);
                });
            }
            else
            {
                ranges = (XElement)cache.obj;
                SetClaimsRangeValues(ranges);
            }



        }

        private void SetClaimsRangeValues(XElement ranges)
        {
            var claimsMin = ranges.Element("Min_Allowed").MaybeValue();
            ClaimsFilterMinimum = ParseOrDefault(claimsMin);

            var claimsMax = ranges.Element("Max_Allowed").MaybeValue();
            ClaimsFilterMax = ParseOrDefault(claimsMax);

            ClaimsLowerValue = ranges.Element("Min_Value").MaybeValue();
            ClaimsUpperVlaue = ranges.Element("Max_Value").MaybeValue();
        }

        static Int32 ParseOrDefault(string text)
        {
            Int32 tmp;
            Int32.TryParse(text, out tmp);
            return tmp;
        }

        public int ClaimsFilterMax { get; set; }

        public int ClaimsFilterMinimum { get; set; }

        private string _claimsLowerVlaue;
        public string ClaimsLowerValue
        {
            get { return _claimsLowerVlaue; }
            set
            {


                if (CheckStringIsInt(value))
                {
                    if (value == "")
                    {
                        _claimsLowerVlaue = "0";
                        _currentLowerValue = "0";
                    }
                    else
                    {
                        if (ClaimsFilterMinimum <= Convert.ToInt32(value))
                        {
                            _claimsLowerVlaue = value;
                            _currentLowerValue = value;
                        }
                        else
                        {
                            _claimsLowerVlaue = ClaimsFilterMinimum.ToString();
                            _currentLowerValue = ClaimsFilterMinimum.ToString();
                        }
                    }

                    NotifyPropertyChanged(this, vm => vm.ClaimsLowerValue);
                }
                else
                {
                    _claimsLowerVlaue = _currentLowerValue;
                    NotifyPropertyChanged(this, vm => vm.ClaimsLowerValue);
                }
            }
        }

        private string _currentLowerValue { get; set; }

        private string _claimsUpperValue;

        public string ClaimsUpperVlaue
        {
            get { return _claimsUpperValue; }

            set
            {
                if (CheckStringIsInt(value))
                {
                    if (value == "")
                    {
                        _claimsUpperValue = "1";
                        _currentUpperValue = "1";
                    }
                    else
                    {
                        if (ClaimsFilterMax >= Convert.ToInt32(value))
                        {
                            _claimsUpperValue = value;
                            _currentUpperValue = value;
                        }
                        else
                        {
                            _claimsUpperValue = ClaimsFilterMax.ToString();
                            _currentUpperValue = ClaimsFilterMax.ToString();
                        }
                    }
                    NotifyPropertyChanged(this, vm => vm.ClaimsUpperVlaue);
                }
                else
                {
                    {
                        _claimsUpperValue = _currentUpperValue;
                        NotifyPropertyChanged(this, vm => vm.ClaimsUpperVlaue);
                    }
                }
            }
        }

        private string _currentUpperValue { get; set; }

        private bool CheckStringIsInt(string input)
        {
            if (input == "")
                return true;

            int? thisInt = null;
            try
            {
                thisInt = Convert.ToInt32(input);
            }
            catch (Exception)
            {

            }


            return thisInt != null;
        }

        #endregion



        #region ReviewMatches Methods
        private void ReturnedMatchSettleEvent(object obj)
        {
            _claimsAccess.SettleEvents(GetSettleEventsDTO()).ContinueWith(SettleEventsContinuation, App.Scheduler);
        }

        private void SettleEventsContinuation(Task<SprocResult> result)
        {
            var tasks = new[]
            {
                _claimsAccess.ReturnMatches(GetClaimMatchDTO(ReturnMatchType.ManualMatch)).ContinueWith(t=>GetReturnedMatchesContinuation(t.Result), App.Scheduler)
            };
            if (CheckCanApplyFilters() && Loaded == 0) AppApplyFilters(new object());
        }

        private SettleEventsDTO GetSettleEventsDTO()
        {
            SettleEventsDTO settleEventsDTO = new SettleEventsDTO();
            settleEventsDTO.EventIds = ClaimsReturnedMatches.Where(x => x.DataTreeNodeSource.AsEnumerable().Any(dr => bool.Parse(dr[DataTreeNode.IsSelected].ToString()))).Select(e => e.NodeId.ToString()).ToList();
            return settleEventsDTO;
        }

        private void ReturnedMatchApprovePayments(object obj)
        {
            _claimsAccess.ApprovePayments(GetApprovePaymentsDTO()).ContinueWith(ApprovePaymentsContinuation, App.Scheduler);

        }

        private void ApprovePaymentsContinuation(Task<SprocResult> result)
        {
            var tasks = new[]
            {
                _claimsAccess.ReturnMatches(GetClaimMatchDTO(ReturnMatchType.ManualMatch)).ContinueWith(t=>GetReturnedMatchesContinuation(t.Result), App.Scheduler)
            };

            Task.Factory.ContinueWhenAll(tasks, ts => CustomMessageBox.Show(result.Result.Message, "Approve Payments", MessageBoxButton.OK, MessageBoxImage.Information));
            if (CheckCanApplyFilters() && Loaded == 0) AppApplyFilters(new object());
        }

        private ApprovePaymentsDTO GetApprovePaymentsDTO()
        {
            ApprovePaymentsDTO approvePaymentsDTO = new ApprovePaymentsDTO();
            approvePaymentsDTO.ClaimIds = new List<string>();
            ClaimsReturnedMatches
            .Where(child => child.Children != null).ToList().ForEach(x =>
                x.Children.FirstOrDefault().DataTreeNodeSource.AsEnumerable().Where(dr => (bool.Parse(dr[DataTreeNode.IsSelected].ToString()))).Do(dr => approvePaymentsDTO.ClaimIds.Add(dr[DataTreeNode.DataTreeNodeId].ToString())));
            return approvePaymentsDTO;
        }

        private void ReturnedMatchSave(object obj)
        {
            CanMatchAutomatically = false;
            CanMatchManually = false;
            SaveReviewMatches = false;
            var saveMatches = new SaveMatchesDTO();
            IList<ClaimEventMatch> newClaimEventMatches = new List<ClaimEventMatch>();

            saveMatches.EventIds = EventsDynamicGrid.Records.Select(i => i.Item_Idx).ToList();
            saveMatches.ClaimIds = ClaimsDynamicGrid.Records.Select(i => i.Item_Idx).ToList();

            foreach (var claimReturnedMatch in ClaimsReturnedMatches)
            {
                if (claimReturnedMatch.Children != null)
                {
                    var childrenDataTreeNodes = claimReturnedMatch.Children;
                    if (childrenDataTreeNodes != null && childrenDataTreeNodes.Any())
                    {
                        foreach (var drClaimsRow in childrenDataTreeNodes.FirstOrDefault().DataTreeNodeSource.AsEnumerable())
                        {
                            newClaimEventMatches.Add(new ClaimEventMatch
                            {
                                ClaimId = drClaimsRow[DataTreeNode.DataTreeNodeId].ToString(),
                                EventId = drClaimsRow[DataTreeNode.DataTreeParentNodeId].ToString()
                            });
                        }
                    }
                }
            }
            saveMatches.Matches = newClaimEventMatches;

            _claimsAccess.SaveMatches(saveMatches)
                .ContinueWith(SaveReturnedMatchContinuation, App.Scheduler);
        }

        private void SaveReturnedMatchContinuation(Task<SaveMatchesResult> task)
        {
            foreach (var returnedMatch in ClaimsReturnedMatches)
            {
                if (returnedMatch.DataTreeNodeSource.AsEnumerable().Any(dr => bool.Parse(dr[DataTreeNode.IsSelected].ToString())))
                {
                    DataRow eventRow = returnedMatch.DataTreeNodeSource.AsEnumerable().FirstOrDefault();
                    returnedMatch.IsItemSaved = true;
                }
            }

            ClaimsReturnedMatchesMaster.Clear();
            var claimReturnedMatchesViewModel = new BulkObservableCollection<ClaimReturnedMatchesViewModel>();
            claimReturnedMatchesViewModel.AddRange(ClaimsReturnedMatches);
            ClaimsReturnedMatchesMaster = claimReturnedMatchesViewModel;

            if (task.Status == TaskStatus.Canceled || task.Status == TaskStatus.Faulted || task.Result == null)
                return;

            CustomMessageBox.Show(task.Result.Message, "Saving Matches", MessageBoxButton.OK, MessageBoxImage.Information);

            if (CheckCanApplyFilters())
                ReloadClaims();
        }

        private void ReturnedMatchCancel(object returnedMatch)
        {
            if (CustomMessageBox.Show("Do you wish to cancel this operation?", "Cancel", MessageBoxButton.YesNo,
               MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                var page = new Pages.Claims();
                MessageBus.Instance.Publish(new NavigateMessage(page));
            }
        }

        private void ApplyVisibilityMatchesFilter()
        {
            ClaimsReturnedMatches.Clear();
            var claimReturnedMatchesViewModel = new BulkObservableCollection<ClaimReturnedMatchesViewModel>();
            claimReturnedMatchesViewModel.AddRange(ClaimsReturnedMatchesMaster);
            ClaimsReturnedMatches = claimReturnedMatchesViewModel;
        }

        private bool CanApprovePayments(object returnedMatch)
        {
            return ClaimsReturnedMatches != null & ClaimsReturnedMatches.Any(x => x.Children != null && x.Children.Any() && x.Children.FirstOrDefault().DataTreeNodeSource.AsEnumerable().Any(dr => bool.Parse(dr[DataTreeNode.IsSelected].ToString())));
        }

        private bool CanSaveReturnedMatch(object returnedMatch)
        {
            return CanApprovePayments(returnedMatch) || CanSettleEvents(returnedMatch) || SaveReviewMatches;
        }

        private bool CanSettleEvents(object returnedMatch)
        {
            return ClaimsReturnedMatches != null & ClaimsReturnedMatches.Any(x => x.DataTreeNodeSource.AsEnumerable().Any(dr => bool.Parse(dr[DataTreeNode.IsSelected].ToString())));
        }

        private void ResetReturnedMatches()
        {
            ClaimsReturnedMatches.Clear();
            ClaimsReturnedMatchesMaster.Clear();
            ClaimsReturnedMatches = new BulkObservableCollection<ClaimReturnedMatchesViewModel>();
            ClaimsReturnedMatchesMaster = new BulkObservableCollection<ClaimReturnedMatchesViewModel>();
        }

        #endregion

        #region Claim Entries

        private bool _isAddingEntry;
        public bool IsAddingEntry
        {
            get
            {
                return !_isAddingEntry;
            }

            set
            {
                _isAddingEntry = value;
                NotifyPropertyChanged(this, vm => vm.IsAddingEntry);
            }
        }

        private void ManuallyAddClaimEntries(object obj)
        {
            IsAddingEntry = true;
            var recs = new List<XElement>();
            var countToSave = ClaimsEntryDynamicGrid.Records.Where(t => t.IsValid).ToList();
            var checkcount = 0;

            foreach (var id in  countToSave)
            {
                recs.Add(AddClaimX(id.Item_Idx));
            }

            var result = _claimsAccess.ManuallyAddClaim(recs);
              Application.Current.Dispatcher.Invoke((Action) delegate
            {
                var s = countToSave.Where(t => result.Contains(t.Item_Idx)).ToList();
                if (result.Count() == countToSave.Count())
                {  
                    if (CustomMessageBox.ShowYesNo("Would you like to retain this manually entered data?", "All claims saved correctly", "Yes", "No") == MessageBoxResult.Yes)
                    {
                        AddNewClaimeEntryRowDeleteOld(s);
                    }         
                }
                else
                { 
                    if (CustomMessageBox.ShowYesNo("Would you like to remove the claims that saved correctly?", "Not all claims saved correctly", "Yes", "No") == MessageBoxResult.Yes)
                    {
                        AddNewClaimeEntryRowDeleteOld(s);
                    }                     
                } 

                ClaimsEntryDynamicGrid.NotifyRecordsChanged();
                NotifyPropertyChanged(this, vm => vm.ClaimsEntryDynamicGrid);
            });

            ApplyFilters();
                 
        }

        private void AddNewClaimeEntryRowDeleteOld(List<Record> rows)
        {
            for (int i = 0; i < rows.Count(); i++)
            {
                var res = _stored.Records;
                res[0].Item_Idx = (ClaimsEntryDynamicGrid.Records.Count + 1).ToString();

                var dependentColumns =
                    res.SelectMany(x => x.Properties)
                        .Select(y => y.DependentColumn)
                        .Distinct()
                        .ToArray();

                foreach (var col in res)
                    foreach (var p in col.Properties.Where(r =>
                        r.ControlType.Contains("down") &&
                        !dependentColumns.Contains(r.ColumnCode)
                        ).ToList())
                        col.InitialDropdownLoad(p, true);

                ClaimsEntryDynamicGrid.Records.AddRange(res);
            }

            ClaimsEntryDynamicGrid.Records.Remove(rows);
        }

        private bool IsAllClaimEntriesValid(object obj)
        {
            if (ClaimsEntryDynamicGrid == null || ClaimsEntryDynamicGrid.Records == null) return false;

            var firstClaimEntry = ClaimsEntryDynamicGrid.Records.FirstOrDefault();
            return firstClaimEntry != null && firstClaimEntry.IsValid;
        }

        private bool CanAddClaimRows(object obj)
        {
            return true;
        }

        private void AddClaimContinuation(Task<SprocResult> task)
        {
            if (task.Result.Success)
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    CustomMessageBox.Show(task.Result.Message, "Claim entry", MessageBoxButton.OK, MessageBoxImage.Information);

                    if (CustomMessageBox.ShowYesNo("Would you like to retain this manually entered data?", "Claim Entry", "Yes", "No") == MessageBoxResult.No)
                    {
                        ClearClaimEntry();
                    }


                    ApplyFilters();
                });


            }


            IsAddingEntry = false;
        }
        public XElement AddClaimX(string Id)
        {
            //fuck me this is going to be fun....
            //  /Results/RootItem/Attributes/Attribute/
            if (ClaimsEntryDynamicGrid == null)
                return null;

            var u = new XElement("User_Idx", Model.User.CurrentUser.ID);
            var s = new XElement("SalesOrg_Idx", Model.User.CurrentUser.SalesOrganisationID);

            XDocument events = new XDocument(new XDocument(
                from r in ClaimsEntryDynamicGrid.Records.Where(t=>t.Item_Idx == Id)
                select new XElement("RootItem",
                    new XElement("Item_Type", r.Item_Type),
                    new XElement("Item_Idx", r.Item_Idx),
                    ReturnValueForControlType(r)
                        //new XElement("Attributes",
                        //    from p in r.Properties
                        //    select new XElement("Attribute",
                        //        new XElement("ColumnCode", p.ColumnCode),
                        //        new XElement("Value", p.Value)
                        //        )
                        //        )
                        )
                        )
                    );


            XDocument xdoc = new XDocument(new XElement("AddClaims",
                XElement.Parse(events.ToString())
                ));
            xdoc.Root.Add(u);
            xdoc.Root.Add(s);
            var xml = XElement.Parse(xdoc.ToString());

            return xml;
        }

        public void ClearClaimEntry()
        {
            GetSalesOrgContinuationResult();
        }

        private XElement ReturnValueForControlType(Record record)
        {
            XElement Attributes = new XElement("Attributes");


            Attributes.Add(
                from p in record.Properties.Where(a => a.ControlType.ToLower() == "dropdown")
                select new XElement("Attribute",
                    new XElement("ColumnCode", p.ColumnCode),
                    checkPValues(p.Values)
                    )
                    );

            Attributes.Add(
               from p in record.Properties.Where(a => a.ControlType.ToLower() == "datepicker")
               select new XElement("Attribute",
                   new XElement("ColumnCode", p.ColumnCode),
                   GetCorrectFormatDateValue(p)
                   )
                   );

            Attributes.Add(
                from p in record.Properties.Where(a => a.ControlType.ToLower() != "dropdown" && a.ControlType.ToLower() != "datepicker")
                select new XElement("Attribute",
                    new XElement("ColumnCode", p.ColumnCode),
                    new XElement("Value", p.Value)
                    )
                    );


            return Attributes;
        }

        public XElement GetCorrectFormatDateValue(Property p)
        {
            //change to yyyy-mm-ddd
            DateTime dt;
            DateTime.TryParse(p.Value, out dt);

            var v = dt.ToString("yyyy-MM-dd");

            return new XElement("Value", v);
        }

        private XElement checkPValues(ObservableCollection<Option> values)
        {
            XElement value = new XElement("Values");
            if (values != null)
            {
                foreach (var v in values.Where(a => a.IsSelected))
                {
                    value.Add(new XElement("Value", v.Item_Idx));
                }
            }

            return value;
        }
        public void UpDateManualEntryClaims(Record rec, string col)
        {
            if (ClaimsEntryDynamicGrid != null)
            {
                //find the property and do the magic
                if (rec != null)
                {
                    var prop = rec.Properties.FirstOrDefault(t => t.ColumnCode == col);
                    if (prop != null) { rec.LoadDependentDrops(prop, true); }
                }

                _addClaimEntriesCommand.RaiseCanExecuteChanged();
            }
        }

        public void CheckIfClaimCanBeAdded()
        {
            _addClaimEntriesCommand.RaiseCanExecuteChanged();
        }
        #endregion

        #endregion

        #region protected methods

        protected ReturnedMatchDTO GetClaimMatchDTO(ReturnMatchType returnMatchType)
        {
            if (returnMatchType == ReturnMatchType.WithoutMatch)
            {
                AllReturnedMatchDTO = new ReturnedMatchDTO();
                AllReturnedMatchDTO.ClaimIds = ClaimsDynamicGrid.Records.Select(r => r.SelectedItems).ToList();
                AllReturnedMatchDTO.ClaimIds = ClaimsDynamicGrid.Records.Select(a => a.Item_Idx).ToList();
                AllReturnedMatchDTO.EventIds = EventsDynamicGrid.Records.Select(a => a.Item_Idx).ToList();
            }
            else
            {
                AllReturnedMatchDTO = new ReturnedMatchDTO();
                AllReturnedMatchDTO.ClaimIds = _storedMatches.ClaimIds;
                AllReturnedMatchDTO.EventIds = _storedMatches.EventIds;
                AllReturnedMatchDTO.Matches = _storedMatches.Matches;
            }
            return AllReturnedMatchDTO;
        }

        #endregion

        #region enum

        public enum ReturnMatchType
        {
            WithoutMatch,
            ManualMatch,
            AutomaticMatch
        }

        #endregion

        private readonly ListingsAccess _listingsAccess = new ListingsAccess();
        private ListingsViewModel _listingsVM;
        public ListingsViewModel ListingsVM
        {
            get { return _listingsVM; }
            set
            {
                _listingsVM = value;
                NotifyPropertyChanged(this, vm => vm.ListingsVM);
            }
        }

        private void LoadListings()
        {
            _listingsBackgroundWorker.DoWork += _listingsBackgroundWorker_DoWork;
            _listingsBackgroundWorker.RunWorkerCompleted += _listingsBackgroundWorker_RunWorkerCompleted;
            _listingsBackgroundWorker.RunWorkerAsync();
        }

        private ListingsViewModel lvm;
        BackgroundWorker _listingsBackgroundWorker = new BackgroundWorker();

        public List<string> SelectedCustomerIDs { get; set; }
        public List<string> SelectedProductIDs { get; set; }

        void _listingsBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var claimsScreen = App.Configuration.GetScreen(ScreenKeys.CLAIM);

            lvm = new ListingsViewModel(
                ListingsAccess.GetFilterCustomers(false, false, claimsScreen).Result,
                ListingsAccess.GetFilterProducts(false, false, claimsScreen).Result,
                claimsScreen.Key);
        }

        void _listingsBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ListingsVM = lvm;
        }

        private void UpdateDelistingsDate()
        {
            if (ListingsVM != null && (ClaimDateFrom != null || EventDateFrom != null))
            {
                if (ClaimDateFrom < EventDateFrom)
                {
                    if (ClaimDateFrom != null)
                        ListingsVM.DateTimeFromParent = ClaimDateFrom;
                }
                else
                {
                    if (EventDateFrom != null)
                        ListingsVM.DateTimeFromParent = EventDateFrom;
                }
            }
        }

        public void SetAllCLaimschecked()
        {

            if (IsClaimsSelectAllChecked)
            {
                ClaimsDynamicGrid.Records.ToList().ForEach(c => c.SelectAll());
            }
            else
            {
                ClaimsDynamicGrid.Records.ToList().ForEach(c => c.DeselectAll());
            }


            NotifyPropertyChanged(this, vm => vm.ClaimsDynamicGrid);
        }

        public void SetAllEventsChecked()
        {

            if (IsEventsSelectAllChecked)
            {
                EventsDynamicGrid.Records.ToList().ForEach(c => c.SelectAll());
            }
            else
            {
                EventsDynamicGrid.Records.ToList().ForEach(c => c.DeselectAll());
            }

            NotifyPropertyChanged(this, vm => vm.EventsDynamicGrid);
        }


        private bool _hasSelectedClaims;
        public bool HasSelectedClaims
        {
            get
            {
                if (ClaimsDynamicGrid != null && ClaimsDynamicGrid.Records != null && ClaimsDynamicGrid.Records.Any())
                {
                    var s = ClaimsDynamicGrid.Records.Select(r => r.SelectedItems).ToList();
                    _hasSelectedClaims = (s.Any(r => r.HasValue()));
                }
                else
                { _hasSelectedClaims = false; }

                return _hasSelectedClaims;
            }

        }

        public Visibility CreateClaimVisibility
        {
            get { return App.Configuration.IsCreateClaimsActive ? Visibility.Visible : Visibility.Collapsed; }
        }

        public bool CanAutoMatch
        {
            get { return App.Configuration.ClaimsCanAutoMatch; }
        }
    }
}
