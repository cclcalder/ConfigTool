// ReSharper disable CheckNamespace

using System.ComponentModel;
using System.Windows;
using System.Xml.Linq;
using Coder.UI.WPF;
using Exceedra.Controls.DynamicGrid.ViewModels;
using Exceedra.MultiSelectCombo.ViewModel;
using Exceedra.SingleSelectCombo.ViewModel;
using Model.DataAccess.Generic;
using Model.Entity;
using Model.Entity.Generic;
using WPF.Navigation;
using WPF.UserControls;

namespace ViewModels
// ReSharper restore CheckNamespace
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Model;
    using Model.DataAccess;
    using Model.Entity.ROBs;
    using ViewHelper;
    using WPF;
    using WPF.ViewModels.Shared;
    using Exceedra.Common.Mvvm;
    public class EventViewModel : BaseViewModel
    {
        private static readonly ImpactViewModel[] EmptyImpactViewModels =
            Enumerable.Repeat(new ImpactViewModel(null), 3).ToArray();

        private readonly RobAccess _access;
        private readonly string _appTypeId;

        private readonly CommentsViewModel _commentsViewModel = new CommentsViewModel();

        public Rob _originalRob;

        private List<ComboboxItem> _staticProducts = new List<ComboboxItem>();

        private List<string> _initallyExpandedDeatilsList = new List<string>();

        private DateTime _end = DateTime.Now.AddDays(365);
        private ImpactViewModel[] _impacts = EmptyImpactViewModels;
        private string _name;

        private DateTime _start = DateTime.Today;
        private ClientConfiguration _clientConfiguration;

        private string _fileLocation;
        private Visibility _fileSelectorVisibility;
        private bool _isReadOnly;

        private static bool _canEditOptions;
        public bool CanEditOptions
        {
            get
            {
                return _canEditOptions;
            }
            set
            {
                _canEditOptions = value;
                OnPropertyChanged("CanEditOptions");
            }
        }

        private RecordViewModel _robInformation;

        public RecordViewModel RobInformation
        {
            get { return _robInformation; }
            set
            {
                if (value == null || _robInformation == value) return;

                _robInformation = value;
                NotifyPropertyChanged(this, vm => vm.RobInformation);
                NotifyPropertyChanged(this, vm => vm.HasRobInformation);
            }
        }

        public bool HasRobInformation
        {
            get { return RobInformation != null && RobInformation.HasRecords; }
        }
        
        private EventViewModel(string appTypeId, RobAccess access)
        {
            App.Navigator.EnableNavigation(false);
            _appTypeId = appTypeId;
            _access = access;
            CancelCommand = new ActionCommand(Cancel);
            ReloadCommand = new ViewCommand(Reload);

            _clientConfiguration = new ClientConfigurationAccess().GetClientConfiguration();
            CommentEventHandler();

            CanEditOptions = false;
        }

        public EventViewModel()
        { }

        private EventViewModel(string appTypeId, List<string> initallyExpandedDeatilsList = null)
            : this(appTypeId, new RobAccess(appTypeId))
        {
            _originalRob = new Rob();
            _originalRob.IsEditable = true;

            _commentsViewModel.DisableAdd = true;

            _initallyExpandedDeatilsList = initallyExpandedDeatilsList;
        }

        private EventViewModel(string appTypeId, Rob rob, List<string> initallyExpandedDeatilsList = null)
            : this(appTypeId, new RobAccess(appTypeId))
        {
            _originalRob = rob;
            MyIdx = rob.ID;
            _commentsViewModel.FilterEnabled = (rob.ID != null);

            FileLocation = _originalRob.FileLocation;
            IsReadOnly = !_originalRob.IsEditable;

            _initallyExpandedDeatilsList = initallyExpandedDeatilsList;
        }

        public string PageTitle { get; set; }

        public bool CreateType
        {
            get
            {
                return
                    _clientConfiguration.ROBScreens.Where(x => x.RobAppType == AppTypeID)
                                        .Select(x => x.NewButton)
                                        .FirstOrDefault();
            }
        }

        public bool IsRecipientActive
        {
            get
            {
                return
                  _clientConfiguration.Screens.Where(x => x.RobAppType == AppTypeID)
                                      .Select(x => x.RobAppRecipient)
                                      .FirstOrDefault();
            }
        }

        public CommentsViewModel CommentsViewModel
        {
            get { return _commentsViewModel; }
        }

        public void UpdateSaveSelection()
        {
            OnPropertyChanged("SelectedCustomers");
            OnPropertyChanged("SelectedProducts");
            OnPropertyChanged("SelectedScenarios");
        }


        private MultiSelectViewModel _customers = new MultiSelectViewModel();
        public MultiSelectViewModel Customers
        {
            get { return _customers; }
            set
            {
                _customers = value;
                NotifyPropertyChanged(this, vm => vm.Customers);
            }
        }

        private MultiSelectViewModel _products = new MultiSelectViewModel();
        public MultiSelectViewModel Products
        {
            get { return _products; }
            set
            {
                _products = value;
                NotifyPropertyChanged(this, vm => vm.Customers);
            }
        }

        public List<ComboboxItem> StaticProducts
        {
            get { return _staticProducts; }
            set
            {
                _staticProducts = value;
                NotifyPropertyChanged(this, vm => vm.StaticProducts);
            }
        }

        private SingleSelectViewModel _customerLevels = new SingleSelectViewModel();
        public SingleSelectViewModel CustomerLevels
        {
            get { return _customerLevels; }
            set
            {
                _customerLevels = value;
                NotifyPropertyChanged(this, vm => vm.CustomerLevels);
            }
        }

        private SingleSelectViewModel _productLevels = new SingleSelectViewModel();
        public SingleSelectViewModel ProductLevels
        {
            get { return _productLevels; }
            set
            {
                _productLevels = value;
                NotifyPropertyChanged(this, vm => vm.ProductLevels);
            }
        }

        private SingleSelectViewModel _eventSubTypes = new SingleSelectViewModel();
        public SingleSelectViewModel EventSubTypes
        {
            get { return _eventSubTypes; }
            set
            {
                _eventSubTypes = value;
                NotifyPropertyChanged(this, vm => vm.EventSubTypes);
            }
        }

        private SingleSelectViewModel _statuses = new SingleSelectViewModel();
        public SingleSelectViewModel Statuses
        {
            get { return _statuses; }
            set
            {
                _statuses = value;
                NotifyPropertyChanged(this, vm => vm.Statuses);
            }
        }

        private SingleSelectViewModel _eventTypes = new SingleSelectViewModel();
        public SingleSelectViewModel EventTypes
        {
            get { return _eventTypes; }
            set
            {
                _eventTypes = value;
                NotifyPropertyChanged(this, vm => vm.EventTypes);
            }
        }

        private MultiSelectViewModel _recipients = new MultiSelectViewModel();
        public MultiSelectViewModel Recipients
        {
            get { return _recipients; }
            set
            {
                _recipients = value;
                NotifyPropertyChanged(this, vm => vm.Recipients);
            }
        }

        public DateTime Start
        {
            get { return _start; }
            set
            {
                Set(ref _start, value, "Start");
                Delistproducts();
            }
        }

        public DateTime End
        {
            get { return _end; }
            set
            {
                Set(ref _end, value, "End");
            }
        }


        public bool IsEndDateBeforeStart
        {
            get { return End < Start; }
        }


        public bool IsFilteredByListings
        {
            get
            {
                return
                    _clientConfiguration.Screens.Where(x => x.RobAppType == AppTypeID)
                      .Select(x => x.IsFilteredByListings)
                      .FirstOrDefault();
            }
        }



        public ImpactViewModel[] Impacts
        {
            get { return _impacts; }
            set
            {
                if (Set(ref _impacts, value, "Impacts"))
                {
                    OnPropertyChanged("Impacts[0]");
                }

                CanEditOptions = (Impacts.Any());
            }
        }

        public ICommand CancelCommand { get; set; }

        public ICommand SaveCommand
        {
            get { return new ViewCommand(CanSave, Save); }
        }

        public ICommand SaveCloseCommand
        {
            get { return new ViewCommand(CanSave, SaveClose); }
        }

        public ICommand ReloadCommand { get; set; }

        private void Reload(object obj)
        {
            RedirectMe.Goto("rob", _originalRob.ID ?? "", _originalRob.Name, AppTypeID);
        }

        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value, "Name"); }
        }

        private MultiSelectViewModel _scenarios = new MultiSelectViewModel();
        public MultiSelectViewModel Scenarios
        {
            get { return _scenarios; }
            set
            {
                _scenarios = value;
                NotifyPropertyChanged(this, vm => vm.Scenarios);
            }
        }




        public string AppTypeID
        {
            get { return _appTypeId; }
        }

        public bool IsReadOnly
        {
            get
            {
                return _isReadOnly;
            }
            set
            {
                _isReadOnly = value;

                if(EventTypes.Items != null) EventTypes.Items.Do(i => i.IsEnabled = !IsReadOnly);
                if (EventSubTypes.Items != null) EventSubTypes.Items.Do(i => i.IsEnabled = !IsReadOnly);
                if (CustomerLevels.Items != null) CustomerLevels.Items.Do(i => i.IsEnabled = !IsReadOnly);
                if (ProductLevels.Items != null) ProductLevels.Items.Do(i => i.IsEnabled = !IsReadOnly);

                NotifyPropertyChanged(this, vm => vm.IsReadOnly);
            }
        }

        public bool IsEditable
        {
            get
            {
                if (_originalRob == null) return true;
                return _originalRob.IsEditable;
            }
        }

        public string FileLocation
        {
            get { return _fileLocation; }
            set
            {
                _fileLocation = value;
                NotifyPropertyChanged(this, vm => vm.FileLocation);
            }
        }

        public Visibility FileSelectorVisibility
        {
            get { return _fileSelectorVisibility; }
            set
            {
                _fileSelectorVisibility = value;
                NotifyPropertyChanged(this, vm => vm.FileSelectorVisibility);
            }
        }

        private bool CanSave(object o)
        {
            var result = !IsEndDateBeforeStart
                         && Customers.SelectedItems != null
                         && Customers.SelectedItems.Any()
                         && (!string.IsNullOrWhiteSpace(Name))
                         && Statuses != null
                         && Statuses.SelectedItem != null
                         && EventSubTypes != null
                         && EventSubTypes.SelectedItem != null
                         && CustomerLevels.SelectedItem != null
                         && ProductLevels.SelectedItem != null
                         && Products.SelectedItems != null
                         && Products.SelectedItems.Any()
                         && Scenarios.SelectedItems != null
                         && Scenarios.SelectedItems.Any()
                         && Impacts.Any(i => i.SelectedOption != null && !string.IsNullOrWhiteSpace(i.SelectedOption.ID));

            return result;
        }


        private void Save(object o)
        {
            var r = Save();
            if (r != null)
            {
                MessageBoxShow("Saved successfully", "Information");

                ReloadCommand.Execute(null);
                //_originalRob = r;

                //LoadInfoGrid(r);
                //LoadStatuses();                
            }
            else
            {
                MessageBoxShow("Save Failed", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void SaveClose(object o)
        {
            var r = Save();
            if (r != null)
            {
                MessageBoxShow("Saved successfully", "Information");
                OnSaved();
            }
            else
            {
                MessageBoxShow("Save Failed", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }


        private Rob Save()
        {
            Rob rob = _originalRob ?? new Rob();
            rob.Customers = Customers.SelectedItems.Distinct().ToList();
            rob.Name = Name;
            rob.StatusIdx = Statuses.SelectedItem.Idx;
            rob.ItemType = EventSubTypes.SelectedItem.Idx;
            rob.Start = _start;
            rob.End = _end;
            rob.CustomerLevelIdx = CustomerLevels.SelectedItem.Idx;
            rob.ProductLevelIdx = ProductLevels.SelectedItem.Idx;
            rob.SelectedProductIdxs = Products.SelectedItems.Select(p => p.Idx).Distinct().ToList();
            rob.SelectedScenarioIdxs = Scenarios.SelectedItems.Select(s => s.Idx).ToList();
            rob.FileLocation = FileLocation;

            var io = Impacts.Where(i => i.SelectedOption != null && !string.IsNullOrWhiteSpace(i.SelectedOption.ID))
                .Select(i => new Option { ID = i.SelectedOption.ID, Value = i.DecimalDecimalAmount })
                .Distinct()
                .ToList();
            rob.ImpactOptions = io;

            if (io.Count < 1)
            {
                Messages.Instance.PutError("At least one impact needs to have a selected option");
            }
            else
            {
                rob.Recipients = Recipients.SelectedItemIdxs.Any() ? new List<string>(Recipients.SelectedItemIdxs) : null;

                IList<Comment> comments = _originalRob == null
                    ? CommentsViewModel.Comments.Select(c => c.ToEntity()).ToList()
                    : new List<Comment>();


                var result = _access.SaveRob(rob, comments);

                if (result.Value.Contains("success"))
                {
                    var robIdx = result.Element("ROB_Idx");
                    if (robIdx != null)
                        rob.ID = robIdx.Value;

                    return rob;
                }
            }

            return null;
        }

        private void Cancel()
        {
            RedirectMe.RobSpecialListScreen(_appTypeId, _initallyExpandedDeatilsList);
        }

        private Task Init(string appTypeId)
        {
            /* Set the editable states here as it will need db work to update the proc responses to indicate if combobox items are enabled/disabled */
            EventTypes.IsEditable = IsEditable;
            EventSubTypes.IsEditable = IsEditable;
            CustomerLevels.IsEditable = IsEditable;
            ProductLevels.IsEditable = IsEditable;

            EventTypes.PropertyChanged += EventTypes_PropertyChanged;
            EventSubTypes.PropertyChanged += EventSubTypes_PropertyChanged;
            CustomerLevels.PropertyChanged += CustomerLevels_PropertyChanged;
            ProductLevels.PropertyChanged += ProductLevels_PropertyChanged;
            Customers.PropertyChanged += Customers_PropertyChanged;

            GetRobComments();
            LoadConfig(appTypeId);

            var args = CommonLoadArguments();

            LoadTypes(args);
            LoadCustomerLevels(args);
            LoadProductLevels(args);
            LoadStatuses();
            LoadScenarios();

            var tasks = new[]
            {
                _access.GetCommentTypes().ContinueWith(GetCommentsTypeContinuation, App.Scheduler),
                _access.GetFilterCommentTypes().ContinueWith(GetFilterCommentsTypeContinuation, App.Scheduler)
            };

            return Task.Factory.ContinueWhenAll(tasks, _ => { });
        }

        private XElement CommonLoadArguments()
        {
            var args = CommonXml.GetBaseArguments("DataSourceInput", AppTypeID);
            var idx = _originalRob == null ? "0" : _originalRob.ID;
            if (idx != "0")
            {
                args.Add(new XElement("ROB_Idx", idx));
            }

            return args;
        }

        #region PropertyChanged events

        private void CustomerLevels_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem")
            {
                if (CustomerLevels.SelectedItem != null)
                    LoadCustomerLevelItems();
            }
        }

        private void ProductLevels_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem")
            {
                if (ProductLevels.SelectedItem != null)
                    LoadProductLevelItems();
            }
        }

        private void EventTypes_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem")
            {
                if (EventTypes.SelectedItem != null)
                    LoadSubTypes();
            }
        }

        private void EventSubTypes_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem")
            {
                if (EventSubTypes.SelectedItem != null)
                    LoadImpacts();
            }
        }

        private void Customers_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItems")
            {
                if (Customers.SelectedItems.Any())
                {
                    LoadProductLevelItems();
                    LoadRecipients();
                }
            }
        }

        #endregion

        #region Load Methods

        private void LoadTypes(XElement args)
        {
            DynamicDataAccess.GetGenericEnumerableAsync<ComboboxItem>(StoredProcedure.ROB.GetTypes, args).ContinueWith(
                t =>
                {
                    EventTypes.SetItems(t.Result);
                    EventTypes.Items.Do(i => i.IsEnabled = !IsReadOnly);
                });
        }

        private void LoadSubTypes()
        {
            var args = CommonXml.GetBaseArguments("DataSourceInput", AppTypeID);
            args.Add(new XElement("Type_Idx", EventTypes.SelectedItem.Idx));
            var idx = _originalRob == null ? "0" : _originalRob.ID;
            if (idx != "0")
            {
                args.Add(new XElement("ROB_Idx", idx));
            }

            DynamicDataAccess.GetGenericEnumerableAsync<ComboboxItem>(StoredProcedure.ROB.GetSubTypes, args).ContinueWith(
                t =>
                {
                    EventSubTypes.SetItems(t.Result);
                    EventSubTypes.Items.Do(i => i.IsEnabled = !IsReadOnly);
                });
        }

        private void LoadCustomerLevels(XElement args)
        {
            DynamicDataAccess.GetGenericEnumerableAsync<ComboboxItem>(StoredProcedure.ROB.GetCustomerLevels, args).ContinueWith(
                t =>
                {
                    CustomerLevels.SetItems(t.Result);
                    CustomerLevels.Items.Do(i => i.IsEnabled = !IsReadOnly);
                });
        }

        private void LoadProductLevels(XElement args)
        {
            DynamicDataAccess.GetGenericEnumerableAsync<ComboboxItem>(StoredProcedure.ROB.GetProductLevels, args).ContinueWith(
                t =>
                {
                    ProductLevels.SetItems(t.Result);
                    ProductLevels.Items.Do(i => i.IsEnabled = !IsReadOnly);
                });
        }

        private void LoadStatuses()
        {
            var args = CommonXml.GetBaseArguments("GetWorkflowStatuses", AppTypeID);
            var idx = _originalRob == null ? "0" : _originalRob.ID;
            if (idx != "0")
            {
                args.Add(new XElement("RobID", idx));
            }

            DynamicDataAccess.GetGenericEnumerableAsync<ComboboxItem>(StoredProcedure.ROB.GetWorkflowStatuses, args).ContinueWith(
                t =>
                {
                    Statuses.SetItems(t.Result);
                });
        }

        private void LoadScenarios()
        {
            var args = _access.GetScenariosArgs(_originalRob.ID);

            DynamicDataAccess.GetGenericEnumerableAsync<ComboboxItem>(StoredProcedure.ROB.GetScenarios, args).ContinueWith(
                t =>
                {
                    Scenarios.SetItems(t.Result);
                });
        }

        private void LoadRecipients()
        {
            if (CustomerLevels.SelectedItem != null && Customers != null && Customers.SelectedItems.Any())
            {
                var args = _access.GetRobRecipientArgs(_originalRob.ID, CustomerLevels.SelectedItem.Idx,
                    Customers.SelectedItems.Select(x => x.Idx).ToList());

                DynamicDataAccess.GetGenericEnumerableAsync<ComboboxItem>(StoredProcedure.ROB.GetRobRecipients, args).ContinueWith(
                t =>
                {
                    Recipients.SetItems(t.Result);
                });
            }
        }

        private void LoadCustomerLevelItems()
        {
            if (CustomerLevels.SelectedItem != null)
            {
                var args = _access.GetCustomersArgs(CustomerLevels.SelectedItem.Idx,
                    _originalRob.ID == "" ? "0" : _originalRob.ID);

                DynamicDataAccess.GetGenericEnumerableAsync<ComboboxItem>(StoredProcedure.ROB.GetCustomerLevelItems, args).ContinueWith(
                    t =>
                    {
                        Customers.SetItems(t.Result);
                    });
            }
        }

        private void LoadConfig(string appTypeId)
        {

            var thisRobKey = App.Configuration.GetScreens().FirstOrDefault(s => s.RobAppType == appTypeId).Key;

            // todo: ### CONSTRAINTS TO REFACTOR
            switch (thisRobKey)
            {
                case "ROB_TERMS":
                    {
                        FileSelectorVisibility = App.Configuration.TermsEditorFolderSelector ? Visibility.Visible : Visibility.Collapsed;
                        break;
                    }
                case "ROB_RISK_OPS":
                    {
                        FileSelectorVisibility = App.Configuration.RiskOpsEditorFolderSelector
                            ? Visibility.Visible
                            : Visibility.Collapsed;
                        break;
                    }
                case "ROB_MARKETING":
                    {
                        FileSelectorVisibility = App.Configuration.MarketingEditorFileSelector ? Visibility.Visible : Visibility.Collapsed;
                        break;
                    }
                case "ROB_MANAGEMENTADJUST":
                    {
                        FileSelectorVisibility = App.Configuration.ManagementAdjustEditorFileSelector ? Visibility.Visible : Visibility.Collapsed;
                        break;
                    }
                case "ROB_TARGET":
                    {
                        FileSelectorVisibility = App.Configuration.TargetEditorFolderSelector ? Visibility.Visible : Visibility.Collapsed;
                        break;
                    }
            }
        }

        public void LoadProductLevelItems()
        {
            if (ProductLevels.SelectedItem != null && CustomerLevels.SelectedItem != null && Customers.SelectedItems != null && Customers.SelectedItems.Any())
            {
                var args = _access.GetProductArgs(ProductLevels.SelectedItem.Idx, _originalRob.ID == "" ? "0" : _originalRob.ID,
                    IsFilteredByListings, CustomerLevels.SelectedItem.Idx, Customers.SelectedItems.Select(sc => sc.Idx));

                DynamicDataAccess.GetGenericEnumerableAsync<ComboboxItem>(StoredProcedure.ROB.GetProductLevelItems, args).ContinueWith(
                    t =>
                    {
                        StaticProducts.Clear();
                        StaticProducts.AddRange(t.Result);
                        Delistproducts();
                    });
            }
        }

        private void LoadImpacts()
        {
            if (EventSubTypes.SelectedItem != null)
            {
                _access.GetImpacts(EventSubTypes.SelectedItem.Idx, _originalRob.ID == "" ? "0" : _originalRob.ID)
                       .ContinueWith(LoadImpactsContinuation, App.Scheduler);
            }
            else
            {
                Impacts = EmptyImpactViewModels;
            }
        }

        private void LoadImpactsContinuation(Task<IList<Impact>> task)
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Impacts = EmptyImpactViewModels;
                return;
            }

            Impacts = task.Result.Select(f => new ImpactViewModel(f)).ToArray();
        }

        #endregion

        public void UpdateImapctFormat(ImpactOption selectedItem)
        {
            if (selectedItem != null)
                Impacts.FirstOrDefault(a => a.Options.Contains(selectedItem)).UpdateFormatAndAmountFromSelection(selectedItem);
        }

        private void Delistproducts()
        {
            var nonDelistedProducts = StaticProducts.Where(a => a.DelistingsDate == null || a.DelistingsDate >= Start);

            Products.SetItems(nonDelistedProducts);
        }

        public EventViewModel NewEventViewModel(string appTypeId, List<string> initallyExpandedDeatilsList = null)
        {
            var instance = new EventViewModel(appTypeId, initallyExpandedDeatilsList);
            instance.Init(appTypeId);
            return instance;
        }

        public static EventViewModel FromRob(string appTypeId, string title, Rob rob, List<string> initalyExpandedItems = null)
        {
            var instance = new EventViewModel(appTypeId, rob)
            {
                PageTitle = title + (string.IsNullOrWhiteSpace(rob.Code) ? "" : " (" + rob.Code + ")")
            };
            instance.Init(appTypeId).ContinueWith(_ => instance.InitFromRobContinuation(instance, rob));
            return instance;
        }

        private void InitFromRobContinuation(EventViewModel viewModel, Rob rob)
        {
            viewModel.Name = rob.Name;

            Start = rob.Start;
            End = rob.End;

            TryLoadInfoGrid(rob);

        }

        private void TryLoadInfoGrid(Rob rob)
        {
            if (rob.ShowInfoGrid)
            {
                _access.LoadInformationGrid(rob.ID).ContinueWith(t =>
                {
                    if (t.Result == null) return;
                    RobInformation = new RecordViewModel(t.Result);
                });
            }
        }

        private void LoadInfoGrid(Rob rob)
        {
            if (rob.ShowInfoGrid)
            {
                _access.LoadInformationGrid(rob.ID).ContinueWith(t =>
                {
                    if (t.Result == null) return;
                    RobInformation = new RecordViewModel(t.Result);
                });
            }
        }

        public void OnSaved(EventArgs e = null)
        {
            CancelCommand.Execute(null);
        }

        #region Comments

        private void CommentEventHandler()
        {
            CommentsViewModel.OnExecuteAddMethod += new EventHandler(AddComment);
            CommentsViewModel.OnExecuteDeleteMethod += new EventHandler(DeleteComment);
        }

        private void DeleteComment(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(CommentsViewModel.SelectedComment.ID))
            {
                if (_originalRob != null && _originalRob.ID != "0")
                {
                    _access.DeleteCommentAsync(_originalRob.ID, CommentsViewModel.SelectedComment.ID)
                        .ContinueWith(t => { GetRobComments(); });


                }
            }
        }

        private void AddComment(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(CommentsViewModel.NewCommentText))
            {
                if (_originalRob != null && _originalRob.ID != "0")
                {
                    _access.AddCommentAsync(_originalRob.ID, CommentsViewModel.CurrentComment.ToEntity())
                        .ContinueWith(t =>
                        {
                            GetRobComments();
                        });

                }
            }
        }

        private void GetRobComments()
        {
            if (_originalRob != null)
            {
                _access.GetComments(_originalRob == null ? "0" : _originalRob.ID)
                       .ContinueWith(GetCommentsContinuation, App.Scheduler);
            }
        }

        private void GetCommentsContinuation(Task<IList<Comment>> task)
        {
            if (task.IsFaulted || task.IsCanceled) return;

            if (task.Result == null || task.Result.Count == 0)
            {
                CommentsViewModel.Comments = new ObservableCollection<CommentViewModel>();
                return;
            }

            var comments = new List<CommentViewModel>();
            foreach (CommentViewModel cvm in task.Result.Select(comment => new CommentViewModel(comment)))
            {
                comments.Add(cvm);
            }
            CommentsViewModel.Comments = new ObservableCollection<CommentViewModel>(comments);
        }

        private void GetCommentsTypeContinuation(Task<IList<CommentType>> task)
        {
            if (task.IsFaulted || task.IsCanceled || task.Result == null || task.Result.Count == 0) return;

            var commentTypes = new List<CommentTypeViewModel>();
            foreach (CommentTypeViewModel cvm in task.Result.Select(commentType => new CommentTypeViewModel(commentType)))
            {
                commentTypes.Add(cvm);
            }
            CommentsViewModel.CommentTypes = new ObservableCollection<CommentTypeViewModel>(commentTypes);

            //get the first selected comment type or, take the first one in the list - PBI:3686
            var com = commentTypes.Where(r => r.IsSelected == true).FirstOrDefault();
            if (com == null)
            {
                commentTypes.FirstOrDefault().IsSelected = true;
                com = commentTypes.FirstOrDefault();
            }

            CommentsViewModel.SelectedCommentType = (CommentTypeViewModel)com;
        }


        private void GetFilterCommentsTypeContinuation(Task<IEnumerable<CommentComboboxItem>> task)
        {
            if (task.IsFaulted || task.IsCanceled || task.Result == null || task.Result.Count() == 0) return;

            CommentsViewModel.FilterCommentTypes.SetItems(task.Result);
        }

        #endregion
    }
}