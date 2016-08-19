using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Coder.UI.WPF;
using Exceedra.Common;
using Exceedra.MultiSelectCombo.ViewModel;
using Exceedra.SingleSelectCombo.ViewModel;
using Model;
using Model.DataAccess;
using Model.Entity;
using Model.Entity.Generic;
using Exceedra.Common.Mvvm;
using ViewHelper;
using ViewModels;
using WPF.Navigation;
using WPF.ViewModels.Shared;
using WPF.ViewModels.Scenarios;
using Comment = Model.Entity.ROBs.Comment;
using TaskFactoryEx = Exceedra.Common.TaskFactoryEx;

namespace WPF.ViewModels.Conditions
{
    public class ConditionDetailsViewModel : ViewModelBase, IObserver
    {
        private string _salesOrgId
        {
            get
            {
                return User.CurrentUser.SalesOrganisationID.ToString();
            }
        }


        private readonly ConditionAccess _conditionAccess;

        private static ConditionDetail CurrentCondtion { get; set; }
        public ConditionDetailsViewModel(string conditionId)
            : this(conditionId,  new ConditionAccess()) { }

        private ConditionDetailsViewModel(string conditionId,
            ConditionAccess conditionAccess)
        {
            Scenarios.PropertyChanged += ScenariosOnPropertyChanged;
            ConditionTypes.PropertyChanged += ConditionTypesOnPropertyChanged;
            CustomerLevels.PropertyChanged += CustomerLevelsOnPropertyChanged;
            Customers.PropertyChanged += CustomersOnPropertyChanged;
            ProductLevels.PropertyChanged += ProductLevelsOnPropertyChanged;

            ConditionId = conditionId;
            MyIdx = ConditionId;
               //            Name = name;
            _conditionAccess = conditionAccess;

            InitData();

            CanLoadGrid = true;
            CanSaveCondition = true;
            IsFiltering = "Apply";
            _hasSelectionChanged = false;

            _applyChangesCommand = new ViewCommand(CanApplyChanges, ApplyChanges);
            _setDefaultPricingCommand = new ViewCommand(SetDefaultPricing);
            _saveCommentCommand = new ViewCommand(SaveComment);
            _submitCommand = new ViewCommand(CanSubmit, Submit);
            _saveCloseCommand = new ViewCommand(CanSubmit, SaveClose);
            CancelCommand = new ViewCommand(Cancel);
            ReloadCommand = new ViewCommand(Reload);
        }

        private void SaveClose(object obj)
        {
            ValidateSave(true);
        }

        private void Reload(object obj)
        {
            RedirectMe.Goto("condition",ConditionId);    
        }

        private void ProductLevelsOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem")
            {
                LoadProducts();
            }
        }

        private void CustomersOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItems")
            {
                LoadProductLevels();
            }
        }

        private void CustomerLevelsOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem")
            {
                LoadCustomers();
            }
        }

        private void ScenariosOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem")
            {
                LoadStatuses();
            }
        }

        private bool _hasSelectionChanged;

        private bool _setMissingPriceEnabled;
        public bool IsSetMissingPricingEnabled
        {
            get { return _setMissingPriceEnabled; }
            set
            {
                _setMissingPriceEnabled = value;
                NotifyPropertyChanged(this, vm => vm.IsSetMissingPricingEnabled);
            }
            //return int.TryParse(DefaultPrice, out validPrice);
        }

        public bool CanLoad(object o)
        {
            NotifyPropertyChanged(this, vm => vm.IsEndDateBeforeStart);

            bool custNotNeeded;
            if (IsProductOnlyMeasure)
            {
                custNotNeeded = true;
            }
            else
            {
                custNotNeeded = CustomerLevels.SelectedItem != null
                                && Customers != null && Customers.SelectedItems.Any()
                                && ProductLevels.SelectedItem != null;
            }

            return  custNotNeeded
                    && Products.Items != null && Products.SelectedItems.Any()
                    && Start != null
                    && End != null
                    && !IsEndDateBeforeStart;
        }

        public bool IsEndDateBeforeStart
        {
            get { return End < Start; }
        }

        private void InitData()
        {
            IsSetMissingPricingEnabled = false;
            var today = DateTime.Today;
            Start = today.AddMonths(-3);
            End = today;

            InitialiseSalesOrg();

            if (ConditionId != "0")
            {
                IsDataLoading = true;
                LoadEditData();
                CanSaveCondition = true;
                CanLoadGrid = true;
            }
            else
            {
                var tasks = LoadCoreData();

                Task.Factory.ContinueWhenAll(tasks, _ =>
                {
                    CanSaveCondition = true;
                    CanLoadGrid = true;
                });
            }
        }

        private Task[] LoadCoreData()
        {
            var tasks = new[]
                {
                    LoadConditionTypes(),
                    LoadScenario(),
                    LoadReasons()
                };

            return tasks;
        }

        private Task LoadCustomerLevels()
        {
            return _conditionAccess.GetCustomerLevels(ConditionId).ContinueWith(t =>
            {
                var tempList = t.Result;

                if (CurrentCondtion != null && FirstLoad)
                    tempList.Where(i => i.Idx.Equals(CurrentCondtion.CustomerLevel1Id)).Do(i => i.IsSelected = true);
                else
                    FirstLoad = false;

                CustomerLevels.SetItems(tempList);
            });
        }

        private void PopulateProductLevels(Task<IList<ComboboxItem>> task)
        {
            var tempList = task.Result;
           // tempList.Do(i => i.IsSelected = false);

            if (CurrentCondtion != null && FirstLoad)
                tempList.Where(i => i.Idx.Equals(CurrentCondtion.ProductLevel1Id)).Do(i => i.IsSelected = true);                
            else
                FirstLoad = false;
                
            ProductLevels.SetItems(tempList);           
        }

        private void PopulateCustomers(Task<IList<ComboboxItem>> task)
        {
            var tempList = task.Result;

            if (CurrentCondtion != null && FirstLoad)
                tempList.Where(i => CurrentCondtion.CustomerLevel2Ids.Contains(i.Idx)).Do(i => i.IsSelected = true);
            else
                FirstLoad = false;

            Customers.SetItems(tempList);
        }

        private void DelistProducts()
        {
            if (StaticProducts != null)
            {
                ObservableCollection<ComboboxItem> thisProductCollection = new ObservableCollection<ComboboxItem>();

                thisProductCollection.AddRange(StaticProducts);
                
                var delistedProducts = thisProductCollection.Where(a => a.DelistingsDate != null && a.DelistingsDate >= End);

                Products.SetItems(delistedProducts);
            }
        }

        private void InitialiseSalesOrg()
        {
            var sodl = (List<SalesOrgDataViewModel>)App.AppCache.GetItem("SalesOrganisations").obj;

            var pointlessComboItem = new ComboboxItem
            {
                Idx = User.CurrentUser.SalesOrganisationID.ToString(),
                Name = sodl.First(s => s.SalesOrgData.ID == User.CurrentUser.SalesOrganisationID.ToString()).SalesOrgData.Name,
                IsSelected = true
            };

            SalesOrgs.SetItems(new List<ComboboxItem> { pointlessComboItem });
        }



        public enum ConditionMode
        {
            New,
            Edit,
            Status
        }

        private ConditionMode _conditionMode;
        public ConditionMode SelectedConditionMode
        {
            get { return _conditionMode; }
            set
            {
                _conditionMode = value;
                NotifyPropertyChanged(this, vm => vm.SelectedConditionMode);
                NotifyPropertyChanged(this, vm => IsAllControlsEnabled);
            }
        }

        public bool IsAllControlsEnabled
        {
            get { return SelectedConditionMode == ConditionMode.New || SelectedConditionMode == ConditionMode.Edit || IsEditable; }
        }

        public bool IsLoadEnabled
        {
            get
            {
                return CanLoadGrid && IsAllControlsEnabled
                && (SelectedConditionType != null)
                && (Scenarios.SelectedItem != null);
            }
        }

        public bool DisableCustomerOptions
        {
            get { return !IsProductOnlyMeasure; }
        }

        public bool IsProductOnlyMeasure
        {
            get
            {
                if (SelectedConditionType != null)
                {
                    return SelectedConditionType.ConditionTypeIndicator == "P";
                }
                else
                {
                    return true;
                }
            }
        }

        private Visibility _isWorkFlowEnabled = Visibility.Visible;
        public Visibility IsWorkFlowEnabled
        {
            get { return _isWorkFlowEnabled; }
            set
            {
                _isWorkFlowEnabled = value;
                NotifyPropertyChanged(this, vm => vm.IsWorkFlowEnabled);
            }
        }




        private bool _isEditable;
        public bool IsEditable
        {
            get
            {
                return _isEditable;
            }
            set
            {
                _isEditable = value;
                NotifyPropertyChanged(this, vm => vm.IsEditable);
                NotifyPropertyChanged(this, vm => vm.IsAllControlsEnabled);
            }
        }

        private void LoadEditData()
        {
            IsSetMissingPricingEnabled = false;
            if (ConditionId != "0")
            {
                IsSetMissingPricingEnabled = true;
                _conditionAccess.GetCondition(ConditionId)
                    .ContinueWith(GetConditionContinuation, App.Scheduler);
                _conditionAccess.GetConditionComments(ConditionId)
                    .ContinueWith(GetConditionCommentsContinuation, App.Scheduler);
            }
        }

        private void GetConditionModeContinuation(Task<bool> task)
        {
            SelectedConditionMode = task.Result ? ConditionMode.Edit : ConditionMode.Status;
        }

        private void GetConditionCommentsContinuation(Task<IList<Comment>> task)
        {

            if (task.IsFaulted || task.IsCanceled || task.Result == null || task.Result.Count == 0) return;
            _comments.Clear();
            foreach (CommentViewModel cvm in task.Result.Select(comment => new CommentViewModel(comment)))
            {
                _comments.Add(cvm);
            }

        }

        private Task LoadConditionTypes()
        {
            return _conditionAccess.GetConditionTypes().ContinueWith(v =>
            {
                TypeList = new List<ConditionType>(v.Result.Element("ConditionTypes").Elements().Select(ConditionType.FromXml));

                if (CurrentCondtion != null)
                    TypeList.Where(
                        t =>
                            t.Idx == CurrentCondtion.ConditionTypeId &&
                            t.ConditionTypeIndicator == CurrentCondtion.ConditionTypeIndicator)
                            .Do(i => i.IsSelected = true);

                ConditionTypes.SetItems(TypeList);

            }, App.Scheduler);
        }

        public List<ConditionType> TypeList { get; set; } 
        private void ConditionTypesOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem" && ConditionTypes.SelectedItem != null)
            {
                var selectedConditionType = ConditionTypes.SelectedItem as ConditionType;
                if (selectedConditionType == null) return;

                SelectedConditionType = TypeList.First(t => t.Idx == selectedConditionType.Idx && t.ConditionTypeIndicator == selectedConditionType.ConditionTypeIndicator);
            }
        }

        private void GetConditionContinuation(Task<ConditionDetail> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return;

            CurrentCondtion = task.Result;

            IsEditable = CurrentCondtion.isEditable;
            ShowChildSelection = CurrentCondtion.ShowChildSelection;
            NotifyPropertyChanged(this, vm => vm.ShowChildSelection);

            LoadControlsEnablings();

         

            LoadCoreData();
            
            Name = CurrentCondtion.Name;
            Start = CurrentCondtion.StartDate;
            End = CurrentCondtion.EndDate;
            PercentChange = CurrentCondtion.PercentChange;
            AbsoluteChange = CurrentCondtion.AbsoluteChange;
            SetTheValue = CurrentCondtion.ChangeFixed;

            CanLoadGrid = true;
            CanSaveCondition = true;
            FirstLoad = true;
        }

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
        private Task LoadControlsEnablings()
        {
            return _conditionAccess.GetConditionControlsEnabled(_salesOrgId, ConditionId == "0" ? "-1" : ConditionId)
                   .ContinueWith(GetConditionModeContinuation, App.Scheduler);
        }

        private Task LoadReasons()
        {
            return _conditionAccess.GetReasons(ConditionId).ContinueWith(t =>
            {
                Reasons.SetItems(t.Result);
            }, App.Scheduler);
        }

        private Task LoadStatuses()
        {
            return _conditionAccess.GetConditionStatuses(ConditionId, Scenarios.SelectedItem.Idx)
                .ContinueWith(t5 =>
                {
                    var tempItems = t5.Result;

                    tempItems = tempItems.OrderBy(a => a.SortOrder).ToList();

                    Statuses.SetItems(tempItems);

                }, App.Scheduler);
        }


        private bool _firstLoad;
        public bool FirstLoad
        {
            get { return _firstLoad; }
            set { _firstLoad = value; }
        }


        private void LoadProducts()
        {
            if (ProductLevels.SelectedItem != null)
                _conditionAccess.GetProductLevelItems(ProductLevels.SelectedItem.Idx,
                    Customers.SelectedItems.Select(x => x.Idx), _salesOrgId, ConditionId, Scenarios.SelectedItem.Idx)
                    .ContinueWith(v =>
                    {
                        var tempList = v.Result;
                        if (CurrentCondtion != null && FirstLoad)
                            tempList.Where(p => CurrentCondtion.ProductLevel2Ids.Any(id => p.Idx == id))
                                .Do(p => p.IsSelected = true);

                        Products.SetItems(tempList);

                        if(FirstLoad && SelectCommand.CanExecute(null))
                            SelectCommand.Execute(null);

                        FirstLoad = false;

                    }, App.Scheduler);
            else
            {
                Products.Clear();
                FirstLoad = false;
            }

                            
        }

        public bool ShowChildSelection { get; set; }

        public string ConditionId { get; set; }

        private void LoadProductLevels()
        {
            if (Customers.SelectedItems.Any() || IsProductOnlyMeasure)
            {
                _conditionAccess.GetProductLevels(ConditionId)
                           .ContinueWith(PopulateProductLevels, App.Scheduler);
            }
            else
            {
                FirstLoad = false;
                ProductLevels.Clear();
            }
        }


        private Task LoadScenario()
        {
            return _conditionAccess.GetScenarios(_salesOrgId, ConditionId).ContinueWith(t =>
            {
                var tempList = t.Result.ToList();
                if (!tempList.Any(i => i.IsSelected))
                    tempList[0].IsSelected = true;
                Scenarios.SetItems(tempList);
            }, App.Scheduler);

        }

        /// <summary>
        /// Load customer levels based on salesOrgID
        /// </summary>
        private void LoadCustomers()
        {
            if (CustomerLevels.SelectedItem != null)
            {
                _conditionAccess.GetCustomerLevelItems(CustomerLevels.SelectedItem.Idx, _salesOrgId, ConditionId)
                    .ContinueWith(PopulateCustomers, App.Scheduler);
            }
            else
            {
                Customers.Clear();
                FirstLoad = false;
            }

        }



        private ConditionType _selectedConditionType;
        public ConditionType SelectedConditionType
        {
            get { return _selectedConditionType; }
            set
            {
                _selectedConditionType = value;
                NotifyPropertyChanged(this, vm => vm.SelectedConditionType);
                NotifyPropertyChanged(this, vm => vm.IsLoadEnabled);
                NotifyPropertyChanged(this, vm => vm.IsProductOnlyMeasure);
                NotifyPropertyChanged(this, vm => vm.DisableCustomerOptions);
                NotifyPropertyChanged(this, vm => vm.AreCustomersEnabled);

                if (IsProductOnlyMeasure == false)
                {

                    LoadCustomerLevels();
                    
                    if (ProductLevels.Items != null)
                    {
                        ProductLevels.Clear();
                    }

                    if (Products.Items != null)
                    {
                        Products.Clear();
                    }
                }
                else
                {
                    if (CustomerLevels.Items != null)
                    {
                        CustomerLevels.Clear();
                    }
                    if (Customers.Items != null)
                    {
                        Customers.Clear();
                    }

                    LoadProductLevels();
                }

            }
        }

        public bool AreCustomersEnabled
        {
            get { return !IsProductOnlyMeasure && IsAllControlsEnabled; } 
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

        private SingleSelectViewModel _salesOrgs = new SingleSelectViewModel();
        public SingleSelectViewModel SalesOrgs
        {
            get { return _salesOrgs; }
            set
            {
                _salesOrgs = value;
                NotifyPropertyChanged(this, vm => vm.SalesOrgs);
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

        private SingleSelectViewModel _conditionTypes = new SingleSelectViewModel();
        public SingleSelectViewModel ConditionTypes
        {
            get { return _conditionTypes; }
            set
            {
                _conditionTypes = value;
                NotifyPropertyChanged(this, vm => vm.ConditionTypes);
            }
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
                NotifyPropertyChanged(this, vm => vm.Products);
            }
        }

        private List<ComboboxItem> _staticProducts;
        public List<ComboboxItem> StaticProducts
        {
            get { return _staticProducts; }
            set
            {
                _staticProducts = value;
                NotifyPropertyChanged(this, vm => vm.StaticProducts);
            }
        }

        private readonly BulkObservableCollection<ConditionMeasureViewModel> _conditionMeasures = new BulkObservableCollection<ConditionMeasureViewModel>();
        public BulkObservableCollection<ConditionMeasureViewModel> ConditionMeasures
        {
            get { return _conditionMeasures; }
        }

        public bool IsAnyConditionMeasureExist
        {
            get { return _conditionMeasures != null && _conditionMeasures.Any(); }
        }

        public void ChangeSelected()
        {

            if (IsConditionMeasureSelectAllChecked)
            {
                ConditionMeasures.ToList().ForEach(x => x.IsSelected = true);
            }
            else
            {
                ConditionMeasures.ToList().ForEach(x => x.IsSelected = false);
            }

            NotifyPropertyChanged(this, vm => vm.ConditionMeasures);
            NotifyPropertyChanged(this, vm => vm.IsAnyConditionMeasureExist);
        }

        private SingleSelectViewModel _scenarios = new SingleSelectViewModel();
        public SingleSelectViewModel Scenarios
        {
            get { return _scenarios; }
            set
            {
                _scenarios = value;
                NotifyPropertyChanged(this, vm => vm.Scenarios);
            }
        }

        private SingleSelectViewModel _reasons = new SingleSelectViewModel();
        public SingleSelectViewModel Reasons
        {
            get { return _reasons; }
            set
            {
                _reasons = value;
                NotifyPropertyChanged(this, vm => vm.Reasons);
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

        private bool _isConditionMeasureSelectAllEnabled;
        public bool IsConditionMeasureSelectAllEnabled
        {
            get { return _isConditionMeasureSelectAllEnabled; }
            set
            {
                _isConditionMeasureSelectAllEnabled = value;
                NotifyPropertyChanged(this, vm => vm.IsConditionMeasureSelectAllEnabled);
            }
        }

        private bool _isConditionMeasureSelectAllChecked = false;
        public bool IsConditionMeasureSelectAllChecked
        {
            get { return _isConditionMeasureSelectAllChecked; }
            set
            {
                _isConditionMeasureSelectAllChecked = value;
                NotifyPropertyChanged(this, vm => vm.IsConditionMeasureSelectAllChecked);
            }
        }

        public bool CanSaveComment
        {
            get
            {
                return IsAllControlsEnabled;
            }
        }

        public bool CanSaveCondition
        {
            get { return (_canSaveCondition && !_hasSelectionChanged); }
            set
            {
                _canSaveCondition = value;
                App.LogError("CanSaveCondition " + value.ToString());
                NotifyPropertyChanged(this, vm => vm.CanSaveCondition);
            }
        }


        public bool CanLoadGrid
        {
            get { return _canLoadGrid; }
            set
            {
                _canLoadGrid = value;
                App.LogError("CanLoadGrid = " + value.ToString());
                NotifyPropertyChanged(this, vm => vm.CanLoadGrid);
                NotifyPropertyChanged(this, vm => vm.IsLoadEnabled);

            }
        }

        public bool CommentIsNotEmpty
        {
            get
            {
                return !string.IsNullOrEmpty(Comment);// && IsAllControlsEnabled; 
            }
        }

        public string Comment
        {
            get { return _comment; }
            set
            {
                _comment = value;
                NotifyPropertyChanged(this, vm => Comment, vm => CommentIsNotEmpty);
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged(this, vm => vm.Name);
                PageTitle = String.IsNullOrEmpty(Name) ? "" : ScreenKeys.CONDITION.ToString().FirstCharToUpper() + ": " + Name + " (" + ConditionId + ")";
            }
        }

        public ICommand SaveCommentCommand
        {
            get { return _saveCommentCommand; }
        }

        public ICommand SubmitCommand
        {
            get { return _submitCommand; }
        }

        public ICommand SaveCloseCommand
        {
            get { return _saveCloseCommand; }
        }

        public ViewCommand CancelCommand { get; set; }
        public ViewCommand ReloadCommand { get; set; }

        private static readonly object SaveSyncObject = new object();

        private bool CanSubmit(object o)
        {
            var canSubmit = CanSaveCondition
                && !String.IsNullOrWhiteSpace(Name)

                && Products.SelectedItems != null && Products.SelectedItems.Any()
                && Start <= End;

            if (!IsProductOnlyMeasure)
            {
                return canSubmit && Customers.SelectedItems != null && Customers.SelectedItems.Any();
            }
            else
            {
                return canSubmit;
            }

        }

        private void Submit(object obj)
        {
            ValidateSave(false);
        }

        private void ValidateSave(bool close)
        {
            if (CanSaveCondition)
                lock (SaveSyncObject)
                    if (CanSaveCondition)
                    {
                        CanSaveCondition = false;
                        if (!ValidateSubmit())
                        {
                            CanSaveCondition = true;
                            return;
                        }


                        List<ConditionMeasure> conditionMesaures = ConditionMeasures.Where(d => d.IsSelected == true).Select(b => b.Data).ToList();
                        var comments = this.Comments.Select(c => c.Value);
                        _conditionAccess.Validate(ConditionId, (CustomerLevels.SelectedItem != null ? CustomerLevels.SelectedItem.Idx : ""), ProductLevels.SelectedItem.Idx,
                            _salesOrgId, Statuses.SelectedItem.Idx, Reasons.SelectedItem == null ? null : Reasons.SelectedItem.Idx,
                            SelectedConditionType.Idx, Name, Start.GetValueOrDefault(), End.GetValueOrDefault(),
                            Customers.SelectedItems.Select(c => c.Idx), Products.SelectedItems.Select(p => p.Idx),
                             Scenarios.SelectedItem.Idx, conditionMesaures, PercentChange, AbsoluteChange,
                            SetTheValue, ShowChildSelection, comments, SelectedConditionType.ConditionTypeIndicator)
                            .ContinueWith(t =>
                            {
                                if (t.Result.Status == ValidationStatus.Success)
                                    Save(close);
                                else
                                {
                                    MessageBoxShow(string.Format("This condition could not be saved.{0}{1}", Environment.NewLine, t.Result.Message),
                                        "Submit", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    CanSaveCondition = true;
                                }
                            }, App.Scheduler);
                    }
        }

        private void Save(bool close)
        {
            var comments = this.Comments.Select(c => c.Value);
            //var conditionMesaures = ConditionMeasures.Select(b => b.Data);
            List<ConditionMeasure> conditionMesaures = ConditionMeasures.Where(d => d.IsSelected == true).Select(b => b.Data).ToList();
            _conditionAccess.Save(ConditionId, (CustomerLevels.SelectedItem != null ? CustomerLevels.SelectedItem.Idx : ""), ProductLevels.SelectedItem.Idx,
                _salesOrgId, Statuses.SelectedItem.Idx, Reasons.SelectedItem == null ? null : Reasons.SelectedItem.Idx,
                SelectedConditionType.Idx, Name, Start.GetValueOrDefault(), End.GetValueOrDefault(),
                Customers.SelectedItems.Select(c => c.Idx), Products.SelectedItems.Select(p => p.Idx),
                 Scenarios.SelectedItem.Idx, conditionMesaures, PercentChange, AbsoluteChange,
                SetTheValue, ShowChildSelection, comments, SelectedConditionType.ConditionTypeIndicator)
                .ContinueWith(t =>
                {
                    if (t.Status != TaskStatus.Faulted && t.Result != "-1")
                    {
                        MessageBoxShow("The condition was saved correctly.", "Submit");
                        ConditionId = t.Result;
                        NotifyPropertyChanged(this, vm => vm.CanSaveComment);

                        //_conditionAccess.GetConditionComments(ConditionId).ContinueWith(GetConditionCommentsContinuation, App.Scheduler);

                        if (close)
                            CancelCommand.Execute(null);
                        else
                            ReloadCommand.Execute(null);
                    }
                    else
                        MessageBoxShow("This condition could not be saved.", "Submit", MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                    lock (SaveSyncObject)
                        CanSaveCondition = true;
                }, App.Scheduler);
        }

        private void Cancel(object obj)
        { 
            RedirectMe.ListScreen("Conditions");
        }

        private void SaveComment(object obj)
        {
            if (!string.IsNullOrWhiteSpace(Comment))
            {
                if (ConditionId != "0" || ConditionId != "-1")
                {
                    _conditionAccess.AddComment(ConditionId, Comment)
                        .ContinueWith(t =>
                        {
                            MessageBoxShow(t.Result, "Add Comment");
                            _conditionAccess.GetConditionComments(ConditionId)
                                .ContinueWith(GetConditionCommentsContinuation, App.Scheduler);
                        }, App.Scheduler);
                    Comment = null;
                }
                else
                {
                    var comment = new Comment
                    {
                        TimeStamp = DateTime.Now,
                        UserName = User.CurrentUser.DisplayName,
                        Value = Comment
                    };
                    Comments.Add(new CommentViewModel(comment));
                    Comment = null;
                }

            }

        }

        private PropertyChangedEventHandler PropetyChangedEventHandler;

        private readonly ViewCommand _applyChangesCommand;
        private readonly ViewCommand _setDefaultPricingCommand;
        private readonly ViewCommand _saveCommentCommand;
        private readonly ViewCommand _submitCommand;
        private readonly ViewCommand _saveCloseCommand;
        private DateTime? _end;
        private DateTime? _start;
        private string _percentChange;
        private string _absoluteChange;
        private string _defaultprice;
        private string _setTheValue;
        private string _comment;
        private string _name;
        private readonly ObservableCollection<CommentViewModel> _comments = new ObservableCollection<CommentViewModel>();
        private bool _canSaveCondition;
        private bool _canLoadGrid;
        private bool _isDataLoading;
        private string _isFiltering;

        public DateTime? Start
        {
            get { return _start; }
            set
            {
                _start = value;
                NotifyPropertyChanged(this, vm => vm.Start);
            }
        }

        public DateTime? End
        {
            get { return _end; }
            set
            {
                _end = value;
                NotifyPropertyChanged(this, vm => vm.End);
                //DelistProducts();
            }
        }

        public ObservableCollection<CommentViewModel> Comments
        {
            get { return _comments; }
        }

        public string PercentChange
        {
            get { return _percentChange; }
            set
            {
                _percentChange = value;
                NotifyPropertyChanged(this, vm => vm.PercentChange);
            }
        }

        public string AbsoluteChange
        {
            get { return _absoluteChange; }
            set
            {
                _absoluteChange = value;
                NotifyPropertyChanged(this, vm => vm.AbsoluteChange);
            }
        }

        private string m_savedDefaultPrice;
        public string SavedDefaultprice
        {
            get { return m_savedDefaultPrice; }
            set
            {
                m_savedDefaultPrice = value;
                NotifyPropertyChanged(this, vm => vm.SavedDefaultprice);
            }
        }

        public string DefaultPrice
        {
            get { return _defaultprice; }
            set
            {
                CanSaveDefaultValue(value);
                _defaultprice = value;

                NotifyPropertyChanged(this, vm => vm.DefaultPrice);
            }
        }

        public bool CanSaveDefaultValue(string value)
        {
            decimal testDecimal;

            if (value == SavedDefaultprice || value.Count() == 0 || !decimal.TryParse(value, NumberStyles.Float, CultureInfo.CurrentCulture, out testDecimal))
            { IsSetMissingPricingEnabled = false; }
            else
            { IsSetMissingPricingEnabled = true; }

            return IsSetMissingPricingEnabled;
        }

        public string SetTheValue
        {
            get { return _setTheValue; }
            set
            {
                _setTheValue = value;
                NotifyPropertyChanged(this, vm => vm.SetTheValue);
            }
        }

        public ICommand ApplyChangesCommand
        {
            get { return _applyChangesCommand; }
        }

        public ICommand SelectCommand
        {
            get { return new ViewCommand(CanLoad, LoadGrid); }
        }

        public ICommand SetDefaultPricingCommand
        {
            get { return _setDefaultPricingCommand; }
        }

        private void ApplyChanges(object parameters)
        {
            double percentChange;
            var changeIsPercentage = double.TryParse(PercentChange, out percentChange);
            double absoluteChange;
            var changeIsAbsolute = double.TryParse(AbsoluteChange, out absoluteChange);
            decimal value;
            var valueIsSet = decimal.TryParse(SetTheValue, out value);
            const string applyChanges = "Apply Changes";
            if (changeIsPercentage && valueIsSet)
            {
                MessageBoxShow("It is not permitted to apply both a percentage change and an exact value.",
                    applyChanges, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (changeIsAbsolute && valueIsSet)
            {
                MessageBoxShow("It is not permitted to apply both an absolute change and an exact value.",
                    applyChanges, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (changeIsAbsolute && changeIsPercentage)
            {
                MessageBoxShow("It is not permitted to apply both an absolute change and a percentage change.",
                    applyChanges, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!changeIsPercentage && !valueIsSet && !changeIsAbsolute)
            {
                MessageBoxShow("It is necessary to apply a percentage, an absolute change, or an exact value.",
                    applyChanges, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (valueIsSet)
                foreach (var conditionMeasure in SelectedConditionMeasures)
                    conditionMeasure.SetNewValue(value, SelectedConditionType.ConditionTypeFormat);
            if (changeIsPercentage)
                foreach (var conditionMeasure in SelectedConditionMeasures)
                    conditionMeasure.SetNewValue(conditionMeasure.Data.OldValue * (decimal)(1.0 + percentChange / 100), SelectedConditionType.ConditionTypeFormat);
            if (changeIsAbsolute)
                foreach (var conditionMeasure in SelectedConditionMeasures)
                    conditionMeasure.SetNewValue(conditionMeasure.Data.OldValue + (decimal)absoluteChange, SelectedConditionType.ConditionTypeFormat);

            NotifyPropertyChanged(this, vm => vm.ConditionMeasures);
            NotifyPropertyChanged(this, vm => vm.IsAnyConditionMeasureExist);
        }

        private bool CanApplyChanges(object o)
        {
            return !String.IsNullOrWhiteSpace(PercentChange)
                   || !String.IsNullOrWhiteSpace(AbsoluteChange)
                   || !String.IsNullOrWhiteSpace(SetTheValue);
        }

        private void SetDefaultPricing(object obj)
        {
            if (IsSetMissingPricingEnabled)
            {
                _conditionAccess.GetConditionMissingPrices(SelectedConditionType, Start.GetValueOrDefault(), End.GetValueOrDefault(), CustomerLevels.SelectedItem == null ? null : CustomerLevels.SelectedItem.Idx, ProductLevels.SelectedItem.Idx,
                            Customers.SelectedItems.Select(c => c.Idx), Products.SelectedItems.Select(p => p.Idx), ConditionId != "0" || ConditionId != "-1" ? ConditionId.ToString(CultureInfo.InvariantCulture) : null, DefaultPrice,
                            ShowChildSelection,
                            Convert.ToInt32(Scenarios.SelectedItem.Idx),
                            Convert.ToInt32(_salesOrgId))
                        .ContinueWith(GetConditionMissingPriceSet, App.Scheduler);
            }
            else
            {
                MessageBoxShow("Please enter valid missing price value");
            }
            IsSetMissingPricingEnabled = false;
        }

        private void GetConditionMissingPriceSet(Task<IList<ConditionMeasure>> task)
        {
            if (task.IsCanceled || task.IsFaulted)
                return;

            var conditionalMeasures = task.Result;
            conditionalMeasures.ToList().ForEach(r => _conditionMeasures.Add(new ConditionMeasureViewModel(r, true, this)));

            IsConditionMeasureSelectAllChecked = !(_conditionMeasures.Any(x => !x.IsSelected));
            IsConditionMeasureSelectAllEnabled = _conditionMeasures != null && _conditionMeasures.Count > 0;
            NotifyPropertyChanged(this, vm => vm.IsAnyConditionMeasureExist);

            if (task.Result.Any())
                DefaultPrice = "";
            CanLoadGrid = true;
            IsDataLoading = false;
        }
        private IEnumerable<ConditionMeasureViewModel> SelectedConditionMeasures
        {
            get
            {
                return _conditionMeasures.Where(cm => cm.IsSelected);
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


        public string IsFiltering
        {
            get { return _isFiltering; }
            set
            {
                _isFiltering = value;
                NotifyPropertyChanged(this, vm => vm.IsFiltering);
            }
        }



        private static readonly object LoadGridSyncObject = new object();
        private void LoadGrid(object parameter)
        {
            IsSetMissingPricingEnabled = false;
            SavedDefaultprice = null;
            CanSaveDefaultValue(DefaultPrice);
            App.LogError("Try Load Grid @" + DateTime.Now);
            if (CanLoadGrid)
                App.LogError("Can Load Grid 1 @" + DateTime.Now);
            lock (LoadGridSyncObject)
                if (CanLoadGrid)
                {
                    App.LogError("Can Load Grid 2 @" + DateTime.Now);
                    CanLoadGrid = false;
                    IsDataLoading = true;
                    ConditionMeasures.Clear();
                    const string operation = "Select";
                    App.LogError("Try Validate select @" + DateTime.Now);
                    if (!ValidateSelect(operation))
                    {
                        App.LogError("Failed validate select @" + DateTime.Now);
                        CanLoadGrid = true;
                        IsDataLoading = false;
                        return;
                    }
                    NotifyPropertyChanged(this, vm => vm.DisableCustomerOptions);
                    App.LogError("Try call webservice @" + DateTime.Now);
                    _conditionAccess.GetMeasures(SelectedConditionType, Start.GetValueOrDefault(), End.GetValueOrDefault(),
                        Customers.SelectedItems, Products.SelectedItems, _salesOrgId, ShowChildSelection, ConditionId != "0" || ConditionId != "-1" ? ConditionId.ToString(CultureInfo.InvariantCulture) : null,  Scenarios.SelectedItem.Idx)
                        .ContinueWith(t =>
                        {
                            App.LogError("Webservice returned data @" + DateTime.Now);
                            Populate(this, TaskFactoryEx.FromResult(Task.Factory, t.Result.Measures), _conditionMeasures,
                                r => r.Select(cm => new ConditionMeasureViewModel(cm, true, this)).ToList(), null,
                                vm => vm.ConditionMeasures);
                            App.LogError("Data populate complete @" + DateTime.Now);
                            //Todo XML may need to return selected measures
                            IsConditionMeasureSelectAllChecked = !(_conditionMeasures.Any(x => !x.IsSelected));
                            IsConditionMeasureSelectAllEnabled = _conditionMeasures != null && _conditionMeasures.Count > 0;
                            NotifyPropertyChanged(this, vm => vm.IsAnyConditionMeasureExist);
                            lock (LoadGridSyncObject)
                                CanLoadGrid = true;

                            IsDataLoading = false;
                        },
                        App.Scheduler);
                    App.LogError("Load complete with data @" + DateTime.Now);
                }
            _hasSelectionChanged = false;
            NotifyPropertyChanged(this, vm => vm.CanSaveCondition);

            IsDataLoading = false;

        }

        private bool ValidateSubmit()
        {

            const string operation = "Submit";
            if (!ValidateSelect(operation))
                return false;

            ConditionMeasures.Select(c => { c.RowError = false; return c; }).ToList();

            var endDateError = ConditionMeasures.Where(d => d.Data.EndDate > End).Select(c => { c.RowError = true; return c; }).ToList();
            var startDateError = ConditionMeasures.Where(d => d.Data.StartDate < Start).Select(c => { c.RowError = true; return c; }).ToList();

            var numOfEndDateError = endDateError.Count();
            var numOfStartDateError = startDateError.Count();

            if (numOfEndDateError > 0 || numOfStartDateError > 0)
            {
                var errorMessage = "";
                var errorLimit = 3;
                var errorCount = 0;
                for (int i = 0; i < errorLimit && i < numOfEndDateError; i++, errorCount++)
                {
                    errorMessage += endDateError[i].Data.CustomerId + " - " + endDateError[i].Data.ProductId + ": End Date is beyond the conditions End Date" + Environment.NewLine;
                }

                for (int i = 0; i < errorLimit && i < numOfStartDateError; i++, errorCount++)
                {
                    errorMessage += startDateError[i].Data.CustomerId + " - " + startDateError[i].Data.ProductId + ": Start Date is before the conditions Start Date" + Environment.NewLine;
                }

                var numOfRemainingErrors = (numOfEndDateError + numOfStartDateError) - errorCount;

                if (numOfRemainingErrors > 0)
                    errorMessage += "(" + numOfRemainingErrors + " additional Date errors)";

                MessageBoxShow(errorMessage, operation, MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }


            if (string.IsNullOrEmpty(Name))
            {
                MessageBoxShow("A name must be chosen.", operation, MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!IsProductOnlyMeasure && CustomerLevels.SelectedItem == null)
            {
                MessageBoxShow("A customer must be selected at Level 1.", operation, MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }
            if (ProductLevels.SelectedItem == null)
            {
                MessageBoxShow("A product must be selected at Level 1.", operation, MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }

            if (Reasons.SelectedItem == null && _conditionMeasures.Any(cm => cm.HasChanged))
            {
                MessageBoxShow("A reason must be selected.", operation, MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }
            if (Statuses.SelectedItem == null)
            {
                MessageBoxShow("A status must be selected.", operation, MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private bool ValidateSelect(string operation)
        {
            if (!IsProductOnlyMeasure && (Customers.SelectedItems == null || !Customers.SelectedItems.Any()))
            {
                MessageBoxShow("At least one customer selection must be made.", operation, MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (Products.SelectedItems == null || !Products.SelectedItems.Any())
            {
                MessageBoxShow("At least one product selection must be made.", operation, MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }
            if (SelectedConditionType == null)
            {
                MessageBoxShow("A condition type must be selected.", operation, MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (Start == null)
            {
                MessageBoxShow("A start date must be specified.", operation, MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (End == null)
            {
                MessageBoxShow("An end date must be specified.", operation, MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        public IEnumerable<ConditionMeasure> SelectedConditionMeaures
        {
            get
            {
                return ConditionMeasures != null
                    ? ConditionMeasures.Where(a => a.IsSelected).Select(b => b.Data)
                    : Enumerable.Empty<ConditionMeasure>();
            }
        }

        public void UpdateState()
        {
            NotifyPropertyChanged(this, vm => vm.ConditionMeasures);
            NotifyPropertyChanged(this, vm => vm.IsAnyConditionMeasureExist);
        }

        public IList<string> ExistingSelectedProductIds { get; set; }

        public void UpdateProductSelectedForCustomers()
        {
            IsFiltering = "Validating \n Products...";
            IsEditable = false;
            if (Products.SelectedItems.Any())
            {
                ExistingSelectedProductIds = new List<string>();
                ExistingSelectedProductIds = Products.SelectedItems.Select(prd => prd.Idx).ToList();
            }
            Products.Clear();

            if (ProductLevels.SelectedItem != null && Customers.SelectedItems.Any())
            {
                _conditionAccess.GetProductLevelItems(ProductLevels.SelectedItem.Idx, Customers.SelectedItems.Select(x => x.Idx).AsEnumerable<string>(), _salesOrgId, ConditionId, Scenarios.SelectedItem.Idx)
                    .ContinueWith(t =>
                    {
                        Products.SetItems(t.Result);
                        UpdateCustomerSelectedProducts();
                        IsFiltering = "Apply";
                        IsEditable = true;
                    }, App.Scheduler);
            }
            else
            {
                IsFiltering = "Apply";
                IsEditable = true;
            }
        }

        private void UpdateCustomerSelectedProducts()
        {
            Products.Clear();
            if (CurrentCondtion != null && (ExistingSelectedProductIds != null || CurrentCondtion.ProductLevel2Ids.Any()))
            {
                var prods = CurrentCondtion.ProductLevel2Ids;
                Products.Items.Where(p => prods.Any(p2 => p.Idx.Equals(p2))).Do(p => p.IsSelected = true);

            }
        }




    }
}

