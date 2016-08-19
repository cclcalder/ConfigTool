using Exceedra.Controls.DynamicGrid.ViewModels;
using Exceedra.Controls.DynamicRow.ViewModels;
using Model.DataAccess;
using Model.DTOs;
using Model.Entity;
using Model.Entity.ROBs;
using Exceedra.Common.Mvvm;
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
using Exceedra.Controls.Messages;
using ViewHelper;
using ViewModels;
using WPF.Navigation;
using WPF.Pages;
using WPF.ViewModels.Shared;

namespace WPF.ViewModels.Claims
{
    public class ClaimEditorPageViewModel : ViewModelBase, IObserver
    {
        #region private fields

        private readonly ClaimsAccess _claimsAccess = new ClaimsAccess();
        private readonly ViewCommand _validateSaveClaimCommand;
        private readonly ViewCommand _saveCloseClaimCommand;
        private readonly ViewCommand _saveClaimCommand;
        private readonly ViewCommand _addClaimCommentCommand;
        private readonly ViewCommand _addEventCommentCommand;
        private readonly ViewCommand _editEventCommand;
        private readonly ViewCommand _openScanLocationCommand;
        private string _claimId;
        private string _adjustment;
        private string _eventComment;
        private string _claimComment;
        private ClaimDetail _claimDetail;
        private bool _canSave = true;
        private int _adjustmentRowHeight;
        private readonly ObservableCollection<CommentViewModel> _claimComments = new ObservableCollection<CommentViewModel>();
        private readonly ObservableCollection<CommentViewModel> _eventComments = new ObservableCollection<CommentViewModel>();
        //private IList<AllowedClaimStatuses> _allowedClaimStatuses = new List<AllowedClaimStatuses>();
        //private AllowedClaimStatuses _selectedAllowedClaimStatus;
        private IList<EventItem> _eventList = new List<EventItem>();
        private ObservableCollection<ClaimEditorProductGridItemViewModel> _selectedProductGridItems = new ObservableCollection<ClaimEditorProductGridItemViewModel>();
        private ObservableCollection<ClaimEditorEventGridItemViewModel> _eventGridItems = new ObservableCollection<ClaimEditorEventGridItemViewModel>();
        private IList<Apportionment> _apportionments = new List<Apportionment>();


        private RowViewModel _claimEditor_ClaimGrid;
        public RowViewModel ClaimEditor_ClaimGrid
        {
            get { return _claimEditor_ClaimGrid; }
            set
            {
                _claimEditor_ClaimGrid = value; 
                NotifyPropertyChanged(this, vm => vm.ClaimEditor_ClaimGrid);
            }
        }

        private RecordViewModel _claimEditor_EventsGrid;
        public RecordViewModel ClaimEditor_EventsGrid
        {
            get { return _claimEditor_EventsGrid; }
            set
            {
                _claimEditor_EventsGrid = value; NotifyPropertyChanged(this, vm => vm.ClaimEditor_EventsGrid);
                ClaimEditor_EventsGrid.CalulateRecordColumnTotal();
            }
        }

        private RecordViewModel _claimEditor_ProductGrid;
        public RecordViewModel ClaimEditor_ProductGrid
        {
            get { return _claimEditor_ProductGrid; }
            set
            {
                _claimEditor_ProductGrid = value; NotifyPropertyChanged(this, vm => vm.ClaimEditor_ProductGrid); NotifyPropertyChanged(this, vm => vm.ClaimEditor_ProductGridHasData);
                ClaimEditor_ProductGrid.CalulateRecordColumns();
               
            }
        }

        #endregion

        #region properties

        public ObservableCollection<CommentViewModel> ClaimComments
        {
            get { return _claimComments; }
        }

        public ObservableCollection<CommentViewModel> EventComments
        {
            get { return _eventComments; }
        }

        public ClaimDetail ClaimDetail
        {
            get
            {
                return _claimDetail;
            }
            set
            {
                _claimDetail = value;
                NotifyPropertyChanged(this, vm => vm.ClaimDetail);
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

        public Visibility ShowAdjustmentValidationMessage
        {
            get
            {
                if (ClaimDetail == null)
                {
                    return Visibility.Hidden;
                }
                double claimValue, adjustment;
                double.TryParse(ClaimDetail.ClaimValue, out claimValue);
                double.TryParse(Adjustment, out adjustment);
                CanSave = (claimValue + adjustment) >= 0;
                AdjustmentRowHeight = CanSave ? 0 : 20;
                return CanSave ? Visibility.Hidden : Visibility.Visible;
            }
        }

        public string Adjustment
        {
            get
            {
                if (ClaimDetail == null) return string.Empty;
                return _adjustment;
            }
            set
            {
                _adjustment = value;
                //UpdateEventGridByNewNetClaimValue();
               // UpdateCanSave();
                NotifyPropertyChanged(this, vm => vm.Adjustment, vm => vm.NetClaimValue, vm => vm.ShowAdjustmentValidationMessage);
            }
        }

        public string NetClaimValue
        {
            get
            {
                if (ClaimDetail == null) return string.Empty;
                double claimValue;
                double.TryParse(ClaimDetail.ClaimValue, out claimValue);
                double adjustment;
                double.TryParse(Adjustment, out adjustment);
                return string.Format("{0:0.00}", claimValue + adjustment);
            }
        }

        public ObservableCollection<ClaimEditorProductGridItemViewModel> SelectedProductGridItems
        {
            get { return _selectedProductGridItems; }
            set
            {
                if (_selectedProductGridItems != value)
                {
                    _selectedProductGridItems = value;
                    NotifyPropertyChanged(this, vm => vm.SelectedProductGridItems);
                }
            }
        }

      
        //public IList<AllowedClaimStatuses> AllowedClaimStatuses
        //{
        //    get
        //    {
        //        return _allowedClaimStatuses;
        //    }
        //    set
        //    {
        //        _allowedClaimStatuses = value;
        //        NotifyPropertyChanged(this, vm => vm.AllowedClaimStatuses);
        //    }
        //}

        //public AllowedClaimStatuses SelectedAllowedClaimStatus
        //{
        //    get
        //    {
        //        return _selectedAllowedClaimStatus;
        //    }
        //    set
        //    {
        //        _selectedAllowedClaimStatus = value;
        //        //UpdateCanSave();
        //        NotifyPropertyChanged(this, vm => vm.SelectedAllowedClaimStatus, vm => vm.CanSave);
        //    }
        //}

        public ICommand CancelCommand { get; set; }
        public ICommand ReloadCommand { get; set; }

        public ICommand EditEventCommand
        {
            get { return _editEventCommand; }
        }

        public ICommand OpenScanLocationCommand
        {
            get { return _openScanLocationCommand; }
        }

        public ICommand ValidateSaveClaimCommand
        {
            get { return _validateSaveClaimCommand; }
        }

        public ICommand SaveCloseClaimCommand
        {
            get { return _saveCloseClaimCommand; }
        }

        public ICommand SaveClaimCommand
        {
            get { return _saveClaimCommand; }
        }

        public ICommand AddClaimCommentCommand
        {
            get { return _addClaimCommentCommand; }
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

        public string ClaimComment
        {
            get { return _claimComment; }
            set
            {
                _claimComment = value;
                NotifyPropertyChanged(this, vm => ClaimComment, vm => ClaimCommentIsNotEmpty);
            }
        }

        public bool ClaimCommentIsNotEmpty
        {
            get { return !string.IsNullOrEmpty(ClaimComment); }
        }

        public int AdjustmentRowHeight
        {
            get
            {
                return _adjustmentRowHeight;
            }
            set
            {
                _adjustmentRowHeight = value;
                NotifyPropertyChanged(this, vm => AdjustmentRowHeight);
            }
        }

        #endregion

        #region ctors

        public ClaimEditorPageViewModel(string claimId)
        {
            _claimId = claimId;
            MyIdx = claimId;
            CancelCommand = new ViewCommand(Cancel);
            ReloadCommand = new ViewCommand(Reload);
            _editEventCommand = new ViewCommand(EditEvent);
            _openScanLocationCommand = new ViewCommand(OpenScanLocation);
            //_saveClaimCommand = new ViewCommand(ValidateSave, SaveClaim);
            _validateSaveClaimCommand = new ViewCommand( ValidateSave);
            _saveCloseClaimCommand = new ViewCommand(SaveClose);
            //_saveClaimCommand = new ViewCommand(SaveClaim);
            _addClaimCommentCommand = new ViewCommand(AddClaimComment);
            _addEventCommentCommand = new ViewCommand(AddEventComment);
            Init();
        }

        private void Reload(object obj)
        {
            RedirectMe.Goto("claim", _claimId);
        }

        #endregion

      
        #region private methods

        public bool ClaimEditor_ProductGridHasData
        {
            get {return (ClaimEditor_ProductGrid != null && ClaimEditor_ProductGrid.HasRecords); }

        }

        private void Init()
        {
            LoadClaimGrid();

           var getEventsRes = _claimsAccess.ClaimEditorGetEvents(_claimId);
            if (getEventsRes != null)
            {
                ClaimEditor_EventsGrid = new RecordViewModel(getEventsRes);
                foreach (var col in ClaimEditor_EventsGrid.Records.ToList())
                {

                    foreach (var p in col.Properties)
                    {
                        if (p.ControlType.Contains("down"))
                        {
                            col.InitialDropdownLoad(p);
                        }
                        if (p.ControlType.Contains("textbox") || p.UpdateToColumn !="")
                        {
                            p.PropertyChanged += d_PropertyChanged;
                        }
                    }

                }
            }
            else
            {
                ClaimEditor_EventsGrid = null;
            }
 

            var tasks = new[]
            {
                //_claimsAccess.GetClaim(_claimId).ContinueWith(GetClaimContinuation, App.Scheduler),                
                _claimsAccess.GetEventList(_claimId).ContinueWith(GetEventListContinuation, App.Scheduler),
                _claimsAccess.GetClaimComments(_claimId).ContinueWith(GetClaimCommentsContinuation, App.Scheduler)
            };       

            Task.Factory.ContinueWhenAll(tasks, ts => CompleteInit());
        }

        private void LoadClaimGrid()
        {
            var getClaimsRes = _claimsAccess.ClaimEditorGetClaim(_claimId);
            if (getClaimsRes != null)
            {
                ClaimEditor_ClaimGrid = RowViewModel.LoadWithData(getClaimsRes);
            }
            else
            {
                ClaimEditor_ClaimGrid = null;
            }

            if (ClaimEditor_ClaimGrid != null && ClaimEditor_ClaimGrid.Records != null)
                foreach (var col in ClaimEditor_ClaimGrid.Records.ToList())
                {

                    foreach (var p in col.Properties.Where(r => r.ControlType.Contains("down")).ToList())
                    { 
                        col.InitialDropdownLoad(p);
                    }

                    var c = col.Properties.Where(r => !String.IsNullOrEmpty(r.UpdateToColumn));

                    foreach (var p in c)
                    {
                        p.PropertyChanged += c_PropertyChanged;
                    }

                        
                    
                    //var d = col.Properties.FirstOrDefault(r => r.ColumnCode == "Claim_Adjustment");
                    //if (d != null)
                    //{
                    //    d.PropertyChanged += c_PropertyChanged;
                    //}

                }

        }


        private void c_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                FireG1ChangesToG2Changes();
            }
        }

        private void d_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //if (ClaimEditor_ProductGrid != null)
            //{
            //    //"Record" ?
            if (e.PropertyName == "Value")
            {
                FireG2ChangesToG3Changes();
            }
            //}
        }

        private void FireG1ChangesToG2Changes()
        {
              //ClaimEditor_ClaimGrid.CalulateRecordColumns(record);
              foreach (var ec in ClaimEditor_ClaimGrid.Records[0].Properties.Where(r => !String.IsNullOrEmpty(r.UpdateToColumn)))
              {
                  var updates = ec.UpdateToColumn.Split('$'); 

                  var table = updates[0];
                  var column = updates[1];
                  
                  if (!string.IsNullOrEmpty(table))
                  {
                      if (ClaimEditor_EventsGrid.Records != null)
                      {
                          // GO THROUGH EACH PARENT GRID COLUMN TO FIND THE ONE THAT NEEDS TO BE UPDATED
                          foreach (var cta in ClaimEditor_EventsGrid.Records)
                          {
                              Exceedra.Controls.DynamicGrid.Models.Property col =
                                  cta.Properties.FirstOrDefault(p => p.ColumnCode == column);
                              if (col != null)
                              {
                                  // set value based on promo measures grid
                                  col.Value = ec.Value;

                              }
                          }
                          NotifyPropertyChanged(this, vm => vm.ClaimEditor_EventsGrid);
                      }
                  }
                 
              }
              Dispatcher.Invoke(new Action(() =>
              {
                  ClaimEditor_EventsGrid.CalulateRecordColumns();
              }));
         
          
        }

        private void FireG2ChangesToG3Changes()
        { 
            bool fireUpdateProop = false;
            foreach (var record in ClaimEditor_EventsGrid.Records.Where(t => t.Item_Idx == ClaimEditor_SelectedEventIdx))
            {
                foreach (var ec in record.Properties.Where(r => !String.IsNullOrEmpty(r.UpdateToColumn)))
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
                        if (ClaimEditor_ProductGrid != null && ClaimEditor_ProductGrid.Records != null)
                        {
                            // GO THROUGH EACH PARENT GRID COLUMN TO FIND THE ONE THAT NEEDS TO BE UPDATED
                            foreach (var cta in ClaimEditor_ProductGrid.Records)
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
               
            }

            if (fireUpdateProop)
            {
                ClaimEditor_ProductGrid.CalulateRecordColumns();
                ClaimEditor_ProductGrid.CalulateRecordColumnTotal();
                NotifyPropertyChanged(this, vm => vm.ClaimEditor_ProductGrid);
            }
        }

        public void InitProductsGrid(string ClaimEditor_EventIdx)
        {

            ClaimEditor_SelectedEventIdx = ClaimEditor_EventIdx;
            var getProductRes = _claimsAccess.ClaimsEditorProducts(_claimId, ClaimEditor_EventIdx);

            _claimsAccess.GetEventComments(ClaimEditor_SelectedEventIdx)
                               .ContinueWith(GetEventCommentsContinuation, App.Scheduler);

            if(getProductRes != null)
            {
                ClaimEditor_ProductGrid = new RecordViewModel(getProductRes);
                
                FireG2ChangesToG3Changes();
            }
            else
            {
                ClaimEditor_ProductGrid = null;
            }
        }

        private string _claimEditor_SelectedEventIdx;
        public string ClaimEditor_SelectedEventIdx
        {
            get {return _claimEditor_SelectedEventIdx;}
            set { _claimEditor_SelectedEventIdx = value; NotifyPropertyChanged(this, vm => vm.ClaimEditor_SelectedEventIdx); }

        }

        private void EditEvent(object currentEvent)
        {
            var eventItem = (ClaimEditorEventGridItemViewModel)currentEvent;
            var page = new EventEditorPage(eventItem.Id);
            MessageBus.Instance.Publish(new NavigateMessage(page));
        }

        private void CompleteInit()
        {
            FireG1ChangesToG2Changes();
        }

    //    private void GetClaimContinuation(Task<ClaimDetail> task)
    //    {
    //        if (task.IsFaulted || task.IsCanceled || task.Result == null) return;
    //        ClaimDetail = task.Result;
    //        var tasks = new[]
    //{              
    //    _claimsAccess.GetAllowedClaimStatuses(_claimDetail.ClaimStatusId).ContinueWith(GetAllowedStatusContinuation, App.Scheduler)
    //};

    //        Task.Factory.ContinueWhenAll(tasks, ts => { });

    //        Adjustment = ClaimDetail.ClaimAdjustment;
    //        NotifyPropertyChanged(this, vm => vm.Adjustment, vm => vm.NetClaimValue);
    //    }

        //private void GetAllowedStatusContinuation(Task<IList<AllowedClaimStatuses>> task)
        //{
        //    if (task.IsFaulted || task.IsCanceled || task.Result == null || task.Result.Count == 0) return;
        //    AllowedClaimStatuses = task.Result;
        //    SelectedAllowedClaimStatus = AllowedClaimStatuses.Where(c => ClaimDetail.ClaimStatusId == c.ClaimStatusId).SingleOrDefault();
        //}

        private void GetEventListContinuation(Task<IList<EventItem>> task)
        {
            if (task.IsFaulted || task.IsCanceled || task.Result == null || task.Result.Count == 0) return;
            _eventList = task.Result;
            var tasks = new[]
            {
                _claimsAccess.GetClaimApportionments(GetClaimApportionmentDTO()).ContinueWith(GetClaimApportionmentsContinuation, App.Scheduler)
            };

           // Task.Factory.ContinueWhenAll(tasks, ts => { PopulateEvents(task); });
        }
 

        //private Apportionment GetApportionment(string eventId)
        //{
        //    return _apportionments.Where(a => a.EventId == eventId).SingleOrDefault();//Select(ap => ap.ClaimApprotionmentValue).
        //}

     
        private void GetClaimApportionmentsContinuation(Task<IList<Apportionment>> task)
        {
            if (task.IsFaulted || task.IsCanceled || task.Result == null || task.Result.Count == 0) return;
            _apportionments = task.Result;
        }

        private GetClaimApportionmentsDTO GetClaimApportionmentDTO()
        {
            return new GetClaimApportionmentsDTO()
            {
                ClaimId = _claimId,
                EventIds = _eventList.Any() ? _eventList.Select(e => e.Id).ToList() : new List<string>()
            };
        }

        private void GetClaimCommentsContinuation(Task<IList<Comment>> task)
        {
            if (task.IsFaulted || task.IsCanceled || task.Result == null || task.Result.Count == 0) return;
            _claimComments.Clear();
            foreach (CommentViewModel cvm in task.Result.Select(comment => new CommentViewModel(comment)))
            {
                _claimComments.Add(cvm);
            }

            ClaimComment = string.Empty;
        }

        private void GetEventCommentsContinuation(Task<IList<Comment>> task)
        {
            _eventComments.Clear();

            if (task.IsFaulted || task.IsCanceled || task.Result == null || task.Result.Count == 0) return;
            
            foreach (CommentViewModel cvm in task.Result.Select(comment => new CommentViewModel(comment)))
            {
                _eventComments.Add(cvm);
            }

            EventComment = string.Empty;
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

        public XElement SaveX()
        {
            //fuck me this is going to be fun....
            //  /Results/RootItem/Attributes/Attribute/
            if (ClaimEditor_ClaimGrid == null && ClaimEditor_EventsGrid == null && ClaimEditor_ProductGrid == null)
                return null;

            var u = new XElement("User_Idx", Model.User.CurrentUser.ID);

            XDocument claims = new XDocument(new XElement("Claim"));
            XDocument events = new XDocument(new XElement("Events"));
            XDocument products = new XDocument(new XElement("Products"));

            if (ClaimEditor_ClaimGrid != null)
                claims.Root.Add(XElement.Parse(ClaimEditor_ClaimGrid.ToCoreXml().ToString()));
            

            if(ClaimEditor_EventsGrid != null)
                events.Root.Add(XElement.Parse(ClaimEditor_EventsGrid.ToXml().ToString()));

            if(ClaimEditor_ProductGrid != null)
                products.Root.Add(XElement.Parse(ClaimEditor_ProductGrid.ToXml().ToString()));

            XDocument xdoc = new XDocument(new XElement("SaveClaim", 
                XElement.Parse(claims.ToString()),
                XElement.Parse(events.ToString()),
                XElement.Parse(products.ToString())
                ));
            xdoc.Root.Add(u);

            var xml = XElement.Parse(xdoc.ToString());

            return xml;
        }

      


        private void ValidateSave(object parameter)
        {
            SaveClaim(false);
        }

        private void SaveClose(object parameter)
        {
            SaveClaim(true);
        }

        private void SaveClaim(bool close)
        {
             
            if (SaveX() == null)
                return;
            CanSave = false;

            _claimsAccess.ValidateClaimsEditor(SaveX()).ContinueWith(t =>
            {

                if (t.Result.Success)
                {
                    Application.Current.Dispatcher.Invoke((Action)delegate {

                        _claimsAccess.SaveClaimsEditor(SaveX()).ContinueWith(f =>
                        {
                            if (close)
                                Application.Current.Dispatcher.Invoke((Action)delegate
                                {
                                    Cancel(false);
                                });

                            CanSave = true;
                            LoadClaimGrid();
                            NotifyPropertyChanged(this, vm => vm.ClaimEditor_ClaimGrid);
                        }
                );

                    });
                   

                }
                else
                {
                    CanSave = true;
                }

              

            });

        }


        //private void UpdateCanSave()
        //{
        //    bool isValid = false;
        //    double adjustment;
        //    if (!string.IsNullOrEmpty(Adjustment.Trim()) && SelectedAllowedClaimStatus != null)
        //    {
        //        isValid = double.TryParse(Adjustment, out adjustment);
        //    }

        //    CanSave = isValid;
        //    NotifyPropertyChanged(this, vm => vm.CanSave);
        //}


        private void AddClaimComment(object parameter)
        {
            AddClaimCommentDTO claimComment = new AddClaimCommentDTO()
            {
                ClaimId = _claimId,
                Comment = ClaimComment
            };
            _claimsAccess.AddClaimComments(claimComment)
                        .ContinueWith(t =>
                        {
                            MessageBoxShow(t.Result.Message, "Add Claim Comment");
                            _claimsAccess.GetClaimComments(_claimId)
                                .ContinueWith(GetClaimCommentsContinuation, App.Scheduler);
                        }, App.Scheduler);
        }

        private void AddEventComment(object parameter)
        {
            AddEventCommentDTO eventComment = new AddEventCommentDTO()
            {
                EventId = ClaimEditor_SelectedEventIdx,
                Comment = EventComment
            };
            _claimsAccess.AddEventComments(eventComment)
                        .ContinueWith(t =>
                        {
                            MessageBoxShow(t.Result.Message, "Add Event Comment");
                            _claimsAccess.GetEventComments(ClaimEditor_SelectedEventIdx)
                                .ContinueWith(GetEventCommentsContinuation, App.Scheduler);
                        }, App.Scheduler);
        }

        private void OpenScanLocation(object parameter)
        {
            string scanLocation = (string)parameter;



            if (File.Exists(scanLocation) || scanLocation.Contains("//"))
            {
                Process.Start(scanLocation);
            }
            else
            {
                MessageBoxShow(string.Format("File can not be found\n {0}", scanLocation), "File Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void UpdateState()
        {
            
        }

        #endregion

        #region public methods

       

        #endregion

    }
}
