using Exceedra.Controls.DynamicGrid.ViewModels;
using Model;
using Model.DataAccess;
using Model.DTOs;
using Model.Entity;
using Model.Entity.ROBs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using ViewHelper;
using ViewModels;
using WPF.Navigation;
using WPF.Pages;
using WPF.ViewModels.Shared;

namespace WPF.ViewModels.Claims
{
    public class EventEditorPageViewModel : ViewModelBase, IObserver
    {
        #region private fields

        private readonly ClaimsAccess _claimsAccess = new ClaimsAccess();
        private readonly ViewCommand _saveEventCommand;
        private readonly ViewCommand _saveCloseCommand;
        private readonly ViewCommand _addEventCommentCommand;
        private readonly ViewCommand _editClaimCommand;
        private readonly ViewCommand _openScanLocationCommand;

        private readonly ViewCommand _showZeroCommand;

        private string _eventId;
        private string _selectedClaimID;
        private string _eventAdjustment;
        private string _eventComment;
        private bool _canSave = true;
        private bool _showZeroApportionments;
        private int _adjustAccrualRowHeight;
        private EventDetail _eventDetail;
        private IList<SettlementReasonCode> _reasonCodes;
        private IList<AllowedEventStatuses> _allowedEventStatuses;
        private AllowedEventStatuses _selectedAllowedEventStatus;
        private SettlementReasonCode _selectedReasonCode;
        private IList<ClaimItem> _claimList = new List<ClaimItem>();
        private IList<ApportionmentItem> _apportionments = new List<ApportionmentItem>();
        private ObservableCollection<EventEditorClaimGridItemViewModel> _claimGridItems = new ObservableCollection<EventEditorClaimGridItemViewModel>();
        private ObservableCollection<EventDetailAdjustment> _adjustments = new ObservableCollection<EventDetailAdjustment>();
        private ObservableCollection<EventEditorProductGridItemViewModel> _productGridItems = new ObservableCollection<EventEditorProductGridItemViewModel>();
        private readonly ObservableCollection<CommentViewModel> _eventComments = new ObservableCollection<CommentViewModel>();

        #endregion

        #region ctors
        private RecordViewModel _eventEditor_Claims;
        public RecordViewModel EventEditor_G1
        {
            get { return _eventEditor_Claims; }
            set
            {
                _eventEditor_Claims = value; NotifyPropertyChanged(this, vm => vm.EventEditor_G1);
                EventEditor_G1.CalulateRecordColumns();
                EventEditor_G1.CalulateRecordColumnTotal();
            }
        }

        private RecordViewModel _eventEditorG2;
        public RecordViewModel EventEditor_G2
        {
            get { return _eventEditorG2; }
            set
            {
                _eventEditorG2 = value; NotifyPropertyChanged(this, vm => vm.EventEditor_G2);
                    EventEditor_G2.CalulateRecordColumns();
                    EventEditor_G2.CalulateRecordColumnTotal();
                
            }
        }


        public EventEditorPageViewModel(string eventId)
        {
            _eventId = eventId;
            MyIdx = _eventId;
            ReloadCommand = new ViewCommand(Reload);
            CancelCommand = new ViewCommand(Cancel);
            _editClaimCommand = new ViewCommand(EditClaim);
            _openScanLocationCommand = new ViewCommand(OpenScanLocation);
            _saveEventCommand = new ViewCommand(SaveEvent);
            _saveCloseCommand = new ViewCommand(SaveClose);
            _addEventCommentCommand = new ViewCommand(AddEventComment);
            _showZeroCommand = new ViewCommand(CanZero, ReloadProductGrid);
            _eventId = eventId;
            Init(_eventId);
            //InitEventsEditor(eventId);
            //InitEventEditorProducts(eventId);
        }

        private bool CanZero(object obj)
        {
            return !string.IsNullOrEmpty(_selectedClaimID);
        }

        private void ReloadProductGrid(object obj)
        {
            InitProductsGrid(_selectedClaimID);
        }

        //public void InitEventsEditor(string EventEditor_EventIdx)
        //{

        //}

        //public void InitEventEditorProducts(string EventEditor_EventIdx)
        //{

        //}

        #endregion

        #region properties

        public string EventAdjustment
        {
            get
            {
                return _eventAdjustment;
            }
            set
            {
                _eventAdjustment = value;
                UpdateCanSave();
                NotifyPropertyChanged(this, vm => vm.EventAdjustment, vm => vm.AvailableAccrual, vm => vm.ShowAdjustAccrualValidationMessage);
            }
        }

        private DateTime _startDate;
        private DateTime _endDate;

        public DateTime StartDate
        {
            get { return _startDate; }
            set { _startDate = value; NotifyPropertyChanged(this, vm => vm.StartDate); }
        }
        public DateTime EndDate { get { return _endDate; } set { _endDate = value; NotifyPropertyChanged(this, vm => vm.EndDate); } }

        public string EventAdjustmentValue
        {
            get
            {
                return string.IsNullOrEmpty(EventAdjustment.Trim()) ? "0" : EventAdjustment;
            }
        }

        public EventDetail EventDetail
        {
            get
            {
                return _eventDetail;
            }
            set
            {
                _eventDetail = value;
                NotifyPropertyChanged(this, vm => EventDetail);
                NotifyPropertyChanged(this, vm => SettledValue);
            }
        }

        public string AvailableAccrual
        {
            get
            {
                if (_eventDetail == null) return string.Empty;
                double toatalAccrual, settled, adjustAccrual;
                double.TryParse(_eventDetail.Total_Accrual, out toatalAccrual);
                double.TryParse(_eventDetail.Settled, out settled);
                double.TryParse(EventAdjustment, out adjustAccrual);
                double availableAccrual = toatalAccrual - settled + adjustAccrual;
                return string.Format("{0:0.00}", availableAccrual);
            }
        }

        public string SettledValue
        {
            get
            {
                if (_eventDetail == null) return string.Empty;
                double settled = 0;
                double.TryParse(_eventDetail.Settled, out settled);
                return string.Format("{0:0.00}", settled);
            }
        }

        public ObservableCollection<EventEditorClaimGridItemViewModel> ClaimGridItems
        {
            get { return _claimGridItems; }
            set
            {
                if (_claimGridItems != value)
                {
                    _claimGridItems = value;
                    NotifyPropertyChanged(this, vm => vm.ClaimGridItems);
                }
            }
        }

        public ObservableCollection<EventDetailAdjustment> Adjustments
        {
            get
            {
                return _adjustments;
            }
            set
            {
                _adjustments = value;
                NotifyPropertyChanged(this, vm => vm.Adjustments);
                EventAdjustment = Adjustments.Sum(r => Convert.ToDecimal(r.Adjustment_Value)).ToString();
            }
        }
        public IList<SettlementReasonCode> ReasonCodes
        {
            get { return _reasonCodes; }
            set
            {
                _reasonCodes = value;
                NotifyPropertyChanged(this, vm => vm.ReasonCodes);
            }
        }

        public IList<AllowedEventStatuses> AllowedEventStatuses
        {
            get { return _allowedEventStatuses; }
            set
            {
                _allowedEventStatuses = value;
                NotifyPropertyChanged(this, vm => vm.AllowedEventStatuses);
            }
        }

        public AllowedEventStatuses SelectedAllowedEventStatus
        {
            get
            {
                return _selectedAllowedEventStatus;
            }
            set
            {
                _selectedAllowedEventStatus = value;
                UpdateCanSave();
                NotifyPropertyChanged(this, vm => vm.SelectedAllowedEventStatus);
            }
        }

        public SettlementReasonCode SelectedReasonCode
        {
            get
            {
                return _selectedReasonCode;
            }
            set
            {
                _selectedReasonCode = value;
                NotifyPropertyChanged(this, vm => vm.SelectedReasonCode);
            }
        }

        public ObservableCollection<CommentViewModel> EventComments
        {
            get { return _eventComments; }
        }
        public ICommand ReloadCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public ICommand ShowZeroCommand
        {
            get { return _showZeroCommand; }
        }


        public ICommand EditClaimCommand
        {
            get { return _editClaimCommand; }
        }

        public ICommand OpenScanLocationCommand
        {
            get { return _openScanLocationCommand; }
        }

        public ICommand SaveEventCommand
        {
            get { return _saveEventCommand; }
        }

        public ICommand SaveCloseCommand
        {
            get { return _saveCloseCommand; }
        }

        public ICommand AddEventCommentCommand
        {
            get { return _addEventCommentCommand; }
        }

        public string EventComment
        {
            get { return _eventComment; }
            set
            {
                _eventComment = value;
                NotifyPropertyChanged(this, vm => EventComment, vm => EventCommentIsNotEmpty);
            }
        }

        public bool EventCommentIsNotEmpty
        {
            get { return !string.IsNullOrEmpty(EventComment); }
        }

        public ObservableCollection<EventEditorProductGridItemViewModel> ProductGridItems
        {
            get
            {
                return _productGridItems;
            }
            set
            {
                _productGridItems = value;
                NotifyPropertyChanged(this, vm => vm.ProductGridItems);
            }
        }

        //public ObservableCollection<EventEditorProductGridItemViewModel> FilteredProductGridItems
        //{
        //    get
        //    {
        //        if (ShowZeroApportionments)
        //        {
        //            return ProductGridItems;
        //        }
        //        else
        //        {
        //            ObservableCollection<EventEditorProductGridItemViewModel> filteredProductGridItems = new ObservableCollection<EventEditorProductGridItemViewModel>(ProductGridItems.Where(p => p.IsZeroApportionedAmount == false));
        //            return filteredProductGridItems;
        //        }
        //    }
        //}

        public bool ShowZeroApportionments
        {
            get
            {
                return _showZeroApportionments;
            }
            set
            {
                _showZeroApportionments = value;
                NotifyPropertyChanged(this, vm => vm.ShowZeroApportionments);
            }
        }

        public bool CanSave
        {
            get
            {
                return _canSave;
            }
            set
            {
                _canSave = value;
                NotifyPropertyChanged(this, vm => vm.CanSave);
            }
        }


        private string _reportURL;
        public string ReportURL
        {
            get
            {
                return _reportURL;
            }
            set
            {
                _reportURL = value;
                NotifyPropertyChanged(this, vm => vm.ReportURL);
                NotifyPropertyChanged(this, vm => vm.HasReport);
            }
        }


        public bool HasReport
        {
            get
            {
                return !string.IsNullOrWhiteSpace(ReportURL);
            }
        }

        public int AdjustAccrualRowHeight
        {
            get
            {
                return _adjustAccrualRowHeight;
            }
            set
            {
                _adjustAccrualRowHeight = value;
                NotifyPropertyChanged(this, vm => vm.AdjustAccrualRowHeight);
            }
        }

        public Visibility ShowAdjustAccrualValidationMessage
        {
            get
            {
                if (EventDetail == null)
                {
                    return Visibility.Hidden;
                }
                double availableAccrual;
                double.TryParse(AvailableAccrual, out availableAccrual);
                CanSave = availableAccrual >= 0;
                AdjustAccrualRowHeight = CanSave ? 0 : 20;
                return CanSave ? Visibility.Hidden : Visibility.Visible;
            }
        }

        #endregion

        #region private methods



        private void Init(string eventId)
        {

            var getClaims = _claimsAccess.ClaimEventScreenGetClaims(eventId);

            if (getClaims != null)
            {
                EventEditor_G1 = new RecordViewModel(getClaims);
                EventEditor_G2 = new RecordViewModel(false);
            }
            else
            {
                EventEditor_G1 = new RecordViewModel(false);
                EventEditor_G2 = new RecordViewModel(false);
            }

            if (EventEditor_G1 != null)
                foreach (var col in EventEditor_G1.Records)
                {
                    var c = col.Properties.Where(r => r.UpdateToColumn != "");

                    foreach (var p in c)
                    {
                        p.PropertyChanged += e_PropertyChanged;
                    }
                }


            var tasks = new[]
            {
                //_claimsAccess.GetClaimList(_eventId).ContinueWith(GetClaimListContinuation, App.Scheduler),
                _claimsAccess.GetEventComments(_eventId).ContinueWith(GetEventCommentsContinuation, App.Scheduler),
                _claimsAccess.GetEvent(_eventId).ContinueWith(GetEventContinuation, App.Scheduler)

            };

            Task.Factory.ContinueWhenAll(tasks, ts => CompleteInit());
        }


        public void InitProductsGrid(string idx)
        {
            _selectedClaimID = idx;

            var getProducts = _claimsAccess.ClaimEventScreenGetProducts(_eventId, _selectedClaimID, (_showZeroApportionments ? "1" : "0"));

            EventEditor_G2 = getProducts != null ? new RecordViewModel(getProducts) : null;
            FireG1ChangesToG2Changes();

        }

        private void e_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                FireG1ChangesToG2Changes();
            }
        }

        private void FireG1ChangesToG2Changes()
        {
            bool fireUpdateProop = false;
            NotifyPropertyChanged(this, vm => vm.EventEditor_G1);

            foreach (var record in EventEditor_G1.Records.Where(t => t.Item_Idx == _selectedClaimID))
            {
                //EventEditor_G1.CalulateRecordColumns(record);

                foreach (var ec in record.Properties.Where(r => r.UpdateToColumn != ""))
                {
                    var updates = ec.UpdateToColumn.Split('$');

                    string table = "";
                    string column = "";
                    try
                    {
                        table = updates[0];
                        column = updates[1];
                    }
                    catch (Exception)
                    {


                    }

                    if (!string.IsNullOrEmpty(table))
                    {
                        if (EventEditor_G2.HasRecords)
                        {
                            // GO THROUGH EACH PARENT GRID COLUMN TO FIND THE ONE THAT NEEDS TO BE UPDATED
                            foreach (var cta in EventEditor_G2.Records)
                            {
                                Exceedra.Controls.DynamicGrid.Models.Property col =
                                    cta.Properties.FirstOrDefault(p => p.ColumnCode == column);
                                if (col != null)
                                {
                                    // set value based on promo measures grid
                                    col.Value = ec.Value;
                                    fireUpdateProop = true;
                                }
                            }

                        }
                    }

                }
                if (fireUpdateProop)
                {
                    EventEditor_G2.CalulateRecordColumns();
                    EventEditor_G2.CalulateRecordColumnTotal();
                    NotifyPropertyChanged(this, vm => vm.EventEditor_G2);
                }

            }
        }

        private void OpenScanLocation(object currentClaim)
        {
            var claim = (EventEditorClaimGridItemViewModel)currentClaim;

            if (File.Exists(claim.ScanLocation) || claim.ScanLocation.Contains("//"))
            {
                Process.Start(claim.ScanLocation);
            }
            else
            {
                MessageBoxShow(string.Format("File can not be found\n {0}", claim.ScanLocation), "File Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void EditClaim(object currentClaim)
        {
            var claim = (EventEditorClaimGridItemViewModel)currentClaim;
            RedirectMe.Goto("claim", claim.ClaimId);
        }

        //private void GetClaimListContinuation(Task<IList<ClaimItem>> task)
        //{
        //    if (task.IsFaulted || task.IsCanceled || task.Result == null || task.Result.Count == 0) return;
        //    _claimList = task.Result;
        //    var tasks = new[]
        //    {
        //        _claimsAccess.GetEventApportionments(GetEventApportionmentDTO()).ContinueWith(GetEventApportionmentsContinuation, App.Scheduler)
        //    };

        //    Task.Factory.ContinueWhenAll(tasks, ts =>
        //    {
        //        PopulateClaims(task);
        //        _claimsAccess.GetEventMaterials(new GetEventMaterialsDTO() { EventId = _eventId }).ContinueWith(GetEventMaterialsContinuation, App.Scheduler);
        //    });
        //}

        //private void PopulateClaims(Task<IList<ClaimItem>> task)
        //{
        //    Populate(this, task, _claimGridItems, r => r.Select(cs => new EventEditorClaimGridItemViewModel(cs, GetApportionAmount(cs.Id))).ToList(), null, vm => vm.ClaimGridItems);
        //    foreach (var claimItem in ClaimGridItems)
        //    {
        //        claimItem.SelectedClaimApportionmentChanged += UpdateProductApportionedAmount;
        //    }
        //}

        //private void UpdateProductApportionedAmount(object sender, SelectedClaimArgs e)
        //{
        //    var products = ProductGridItems.Where(p => p.ClaimId.ToLower() == e.ClaimLineID.ToLower());
        //    products.Do(p => p.EventApportionedAmount = e.ApportionedAmount);
        //}

        //private ApportionmentItem GetApportionAmount(string claimId)
        //{
        //    return _apportionments.Where(a => a.ClaimId == claimId).SingleOrDefault();
        //}

        //private GetEventApportionmentsDTO GetEventApportionmentDTO()
        //{
        //    return new GetEventApportionmentsDTO()
        //    {
        //        EventId = _eventId,
        //        ClaimIds = _claimList.Any() ? _claimList.Select(e => e.Id).ToList() : new List<string>()
        //    };
        //}

        //private void GetEventApportionmentsContinuation(Task<IList<ApportionmentItem>> task)
        //{
        //    if (task.IsFaulted || task.IsCanceled || task.Result == null || task.Result.Count == 0) return;
        //    _apportionments = task.Result;
        //}

        private void GetSettlementReasonCodesContinuation(Task<IList<SettlementReasonCode>> task)
        {
            if (task.IsFaulted || task.IsCanceled || task.Result == null || task.Result.Count == 0) return;
            ReasonCodes = task.Result;
            SelectedReasonCode = ReasonCodes.Where(r => r.ReasonCodeId == _eventDetail.Reason_Code_Idx).SingleOrDefault();
        }

        private void GetEventContinuation(Task<EventDetail> task)
        {
            if (task.IsFaulted || task.IsCanceled || task.Result == null) return;

            try
            {
                EventDetail = task.Result;
                EventAdjustment = EventDetail.TotalAdjustment.ToString();
                ReportURL = EventDetail.ReportURL;
                Adjustments = new ObservableCollection<EventDetailAdjustment>(EventDetail.Adjustments);

                var tasks = new[]
                {
                _claimsAccess.GetSettlementReasonCodes().ContinueWith(GetSettlementReasonCodesContinuation, App.Scheduler),
                _claimsAccess.GetAllowedEventStatuses(_eventDetail.Event_Status_Idx).ContinueWith(GetAllowedEventStatusesContinuation, App.Scheduler)
            };

                Task.Factory.ContinueWhenAll(tasks, ts => { });
            }
            catch (Exception ex)
            {
                var r = AllowedEventStatuses;

            }




        }



        private void GetAllowedEventStatusesContinuation(Task<IList<AllowedEventStatuses>> task)
        {
            if (task.IsFaulted || task.IsCanceled || task.Result == null || task.Result.Count == 0) return;
            AllowedEventStatuses = task.Result;
            SelectedAllowedEventStatus = AllowedEventStatuses.Where(r => r.EventStatusId == _eventDetail.Event_Status_Idx).SingleOrDefault();
        }

        private void GetEventCommentsContinuation(Task<IList<Comment>> task)
        {
            if (task.IsFaulted || task.IsCanceled || task.Result == null || task.Result.Count == 0) return;
            _eventComments.Clear();
            foreach (CommentViewModel cvm in task.Result.Select(comment => new CommentViewModel(comment)))
            {
                _eventComments.Add(cvm);
            }

            EventComment = string.Empty;
        }

        private void GetEventMaterialsContinuation(Task<IList<EventProductDetail>> task)
        {
            if (task.IsFaulted || task.IsCanceled || task.Result == null || task.Result.Count == 0) return;

            foreach (var eventProduct in task.Result)
            {
                var claim = ClaimGridItems.Where(c => c.ClaimLineDetail.ToLower() == eventProduct.ClaimLineDetail.ToLower()).FirstOrDefault();
                eventProduct.EventApportionedAmount = claim.ApportionedAmount;
            }

            Populate(this, task, _productGridItems, r => r.Select(cs => new EventEditorProductGridItemViewModel(cs)).ToList(), null, vm => vm.ProductGridItems);

        }

        private void CompleteInit()
        {
            if (EventEditor_G1 != null && EventEditor_G2 != null)
            {
                FireG1ChangesToG2Changes();
            }
        }

        private void Reload(object o)
        {
            RedirectMe.Goto("event", _eventId);
        }

        private void Cancel(object parameter)
        {
            Cancel();
        }

        private void Cancel(bool askForConfirmation = true)
        {
            if (askForConfirmation && MessageBox.Show("Do you wish to cancel this operation?", "Cancel", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                return;

            RedirectMe.ListScreen("Claims");
        }


        private void AddEventComment(object parameter)
        {
            AddEventCommentDTO eventComment = new AddEventCommentDTO()
            {
                EventId = _eventId,
                Comment = EventComment
            };
            _claimsAccess.AddEventComments(eventComment)
                        .ContinueWith(t =>
                        {
                            MessageBoxShow(t.Result.Message, "Add Event Comment");
                            _claimsAccess.GetEventComments(_eventId)
                                .ContinueWith(GetEventCommentsContinuation, App.Scheduler);
                        }, App.Scheduler);
        }

        private void SaveEvent(object parameter)
        {
            Save(false);
        }

        private void SaveClose(object parameter)
        {
            Save(true);
        }

        private void Save(bool close)
        {
            CanSave = false;
            SaveEventDTO saveEventDto = GetSemiDynamicSaveEventDTO(); //GetSaveEventDTO();

            _claimsAccess.ValidateEventsEditor(SaveX(saveEventDto)).ContinueWith(t =>
            {
                if (t.Result.Success)
                {
                    Save(saveEventDto);

                    if (close)
                        Cancel(false);
                }
                else
                {
                    CanSave = true;
                    MessageBoxShow(string.Format("The event is not valid.{0}{1}", Environment.NewLine, t.Result.Message),
                        "Save Event", MessageBoxButton.OK, MessageBoxImage.Warning);

                }
            }, App.Scheduler);
        }

        public XElement SaveX(SaveEventDTO saveEventDTO)
        {
            //fuck me this is going to be fun....
            //  /Results/RootItem/Attributes/Attribute/

            if (EventEditor_G1 == null)
                return null;

            var u = new XElement("User_Idx", Model.User.CurrentUser.ID);

            XDocument claims = new XDocument(new XElement("Claims"));
            claims.Root.Add(XElement.Parse(EventEditor_G1.ToXml().ToString()));

            XDocument xdoc = new XDocument(new XElement("SaveEvent",
                XElement.Parse(claims.ToString())
                ));

            if (EventEditor_G2 != null && EventEditor_G2.Records != null)
            {
                XDocument products = new XDocument(new XElement("Products"));
                products.Root.Add(XElement.Parse(EventEditor_G2.ToXml().ToString()));

                xdoc.Root.Add( XElement.Parse(products.ToString()));
            }


            xdoc.Root.Add(u);

            var xml = XElement.Parse(xdoc.ToString());


            xml.Add(new XElement("User_Idx", User.CurrentUser.ID));
            xml.Add(new XElement("Event_Idx", saveEventDTO.EventId));
            xml.Add(new XElement("AdjustmentStartDate", saveEventDTO.StartDate));
            xml.Add(new XElement("AdjustmentEndDate", saveEventDTO.EndDate));
            xml.Add(new XElement("Event_Adjustment", saveEventDTO.EventAdjustment));
            xml.Add(new XElement("Event_Status_Idx", saveEventDTO.EventStatusId));
            if (string.IsNullOrEmpty(saveEventDTO.ReasonCodeId) == false)
            {
                xml.Add(new XElement("Reason_Code_Idx", saveEventDTO.ReasonCodeId));
            }

            var adj = new XElement("Results");
            foreach (var productItem in saveEventDTO.Adjustments)
            {
                var adjElement = new XElement("Adjustment");
                adjElement.Add(new XElement("Adjustment_Row_Idx", productItem.Adjustment_Row_Idx));
                adjElement.Add(new XElement("Adjustment_Start_Date", productItem.Adjustment_Start_Date));
                adjElement.Add(new XElement("Adjustment_End_Date", productItem.Adjustment_End_Date));
                adjElement.Add(new XElement("Adjustment_Value", productItem.Adjustment_Value));
                adjElement.Add(new XElement("Adjustment_Comment", productItem.Adjustment_Comment));
                adjElement.Add(new XElement("Adjustment_User", productItem.Adjustment_User));
                adj.Add(adjElement);
            }
            xml.Add(adj);

            return xml;
        }

        private void UpdateCanSave()
        {
            bool isValid = false;
            double adjustment;
            if (!string.IsNullOrEmpty(EventAdjustment.Trim()) && SelectedAllowedEventStatus != null)
            {
                isValid = double.TryParse(EventAdjustment, out adjustment);
            }

            CanSave = isValid;
            NotifyPropertyChanged(this, vm => vm.CanSave);
        }

        private SaveEventDTO GetSaveEventDTO()
        {
            SaveEventDTO saveEventDto = new SaveEventDTO();
            saveEventDto.EventId = _eventId;
            saveEventDto.EventAdjustment = EventAdjustmentValue;
            saveEventDto.EventStatusId = SelectedAllowedEventStatus.EventStatusId;
            saveEventDto.StartDate = EventDetail.Event_Start_Date;
            saveEventDto.EndDate = EventDetail.Event_End_Date;

            if (SelectedReasonCode != null)
            {
                saveEventDto.ReasonCodeId = SelectedReasonCode.ReasonCodeId;
            }

            var adj = Adjustments.Where(r => r.Adjustment_Start_Date != null && r.Adjustment_End_Date != null).ToList();
            saveEventDto.Adjustments = adj;

            foreach (var claimItem in ClaimGridItems)
            {
                SaveEventClaimItem saveEventClaimItem = new SaveEventClaimItem();
                saveEventClaimItem.ClaimId = claimItem.ClaimId;
                saveEventClaimItem.ClaimApportionmentType = claimItem.ApportionmentType;
                saveEventClaimItem.ClaimApportionmentValue = claimItem.ClaimApportionmentValue;
                saveEventDto.Claims.Add(saveEventClaimItem);
            }

            foreach (var productItem in ProductGridItems)
            {
                SaveEventProductItem saveEventProductItem = new SaveEventProductItem();
                saveEventProductItem.ClaimId = productItem.ClaimId;
                saveEventProductItem.ProductId = productItem.MaterialId;
                saveEventProductItem.ClaimApportionmentValue = productItem.MaterialApportionmentValue;
                saveEventProductItem.ClaimApportionmentType = productItem.MaterialApportionmentType;
                saveEventDto.Products.Add(saveEventProductItem);
            }

            return saveEventDto;
        }

        private SaveEventDTO GetSemiDynamicSaveEventDTO()
        {
            SaveEventDTO saveEventDto = new SaveEventDTO();
            saveEventDto.EventId = _eventId;
            saveEventDto.EventAdjustment = EventAdjustmentValue;
            saveEventDto.EventStatusId = SelectedAllowedEventStatus.EventStatusId;
            saveEventDto.StartDate = EventDetail.Event_Start_Date;
            saveEventDto.EndDate = EventDetail.Event_End_Date;

            if (SelectedReasonCode != null)
            {
                saveEventDto.ReasonCodeId = SelectedReasonCode.ReasonCodeId;
            }

            var adj = Adjustments.Where(r => r.Adjustment_Start_Date != null && r.Adjustment_End_Date != null).ToList();
            saveEventDto.Adjustments = adj;

            return saveEventDto;
        }

        //private void Save(SaveEventDTO saveEventDto)
        //{
        //    _claimsAccess.SaveEvent(saveEventDto).ContinueWith(t =>
        //    {
        //        if (t.Result.Success)
        //        {
        //            MessageBoxShow("The event saved successfully.",
        //                "Save Event", MessageBoxButton.OK, MessageBoxImage.Information);
        //            _claimsAccess.GetEvent(_eventId).ContinueWith(GetEventContinuation, App.Scheduler);
        //        }
        //        else
        //        {
        //            MessageBoxShow(string.Format("The attempt to save the event failed.{0}{1}", Environment.NewLine, t.Result.Message),
        //                "Save Event", MessageBoxButton.OK, MessageBoxImage.Warning);

        //        }
        //        CanSave = true;
        //    }, App.Scheduler);
        //}

        private void Save(SaveEventDTO saveDTO)
        {
            _claimsAccess.SaveEventsEditor(SaveX(saveDTO)).ContinueWith(t =>
            {
                if (t.Result.Success)
                {
                    MessageBoxShow("The event saved successfully.",
                        "Save Event", MessageBoxButton.OK, MessageBoxImage.Information);
                    _claimsAccess.GetEvent(_eventId).ContinueWith(GetEventContinuation, App.Scheduler);
                }
                else
                {
                    MessageBoxShow(string.Format("The attempt to save the event failed.{0}{1}", Environment.NewLine, t.Result.Message),
                        "Save Event", MessageBoxButton.OK, MessageBoxImage.Warning);

                }
                CanSave = true;
            }, App.Scheduler);
        }

        #endregion

        #region public methods

        public void UpdateState()
        {
            // NotifyPropertyChanged(this, vm => vm.);
        }

        #endregion



        internal void UpdateTotals()
        {
            EventAdjustment = string.Format("{0:0.00}", EventDetail.TotalAdjustment);
        }
    }
}
