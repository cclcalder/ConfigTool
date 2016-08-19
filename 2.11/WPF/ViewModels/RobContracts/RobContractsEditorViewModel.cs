using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using Exceedra.Common;
using Exceedra.Controls.DynamicGrid.Models;
using Exceedra.Controls.DynamicGrid.ViewModels;
using Exceedra.Controls.DynamicRow.Models;
using Exceedra.Controls.DynamicRow.ViewModels;
using Exceedra.Controls.DynamicTab.ViewModels;
using Exceedra.MultiSelectCombo.ViewModel;
using Model;
using Model.DataAccess;
using Model.DataAccess.Generic;
using Model.Entity.Generic;
using Model.Entity.ROBs;
using Model.Utilities;
using ViewHelper;
using ViewModels;
using WPF.Navigation;
using WPF.UserControls.Listings;
using WPF.UserControls.Tabs.Models;
using WPF.UserControls.Tabs.ViewModels;
using WPF.UserControls.Trees.ViewModels;
using Status = Model.Entity.Generic.Status;
using Exceedra.Common.Utilities;
using Model.DataAccess.Listings;
using Model.Entity;
using Model.Entity.Listings;

namespace WPF.ViewModels.RobContracts
{
    public class RobContractsEditorViewModel : ViewModelBase
    {
        #region fields

        /// <summary>
        /// The id of the rob screen from which
        /// the contract editor was entered
        /// (i.e. "300" for Terms, "400" for TODO: ... etc)
        /// </summary>
        public string _robScreenId;

        /// <summary>
        /// The active tab of the rob screen from which the contract editor was entered.
        /// Used for reactivate this tab after moving back to the rob screen.
        /// </summary>
        private string _robScreenActiveTab;

        /// <summary>
        /// Database intermediary (in the robs context)
        /// </summary>
        private RobAccess _robAccess;

        private readonly GroupEditorAccess _robGroupAccess = new GroupEditorAccess();

        /// <summary>
        /// The id of the contract currently being edited.
        /// If it is a new contract the id is null.
        /// </summary>
        public string _contractId;

        private RowViewModel _contractDetails;
        private bool _isContractAmendable;
        private ListingsViewModel _listings;
        private TreeViewModel _customers;
        private TreeViewModel _products;
        private RowViewModel _newTermDetails;
        private ObservableCollection<Status> _availableContractStatuses;
        private Status _selectedContractStatus;
        private MultiSelectViewModel _availableScenarios;
        private MultiSelectViewModel _availableRecipients;
        private RobRecipient _selectedRecipient;

        /// <summary>
        /// List of ids of the terms deleted by changing the contract's related data (i.e. customers selection).
        /// When a user changes some contract's data that is related to already created terms we can be no longer sure if these terms still match the contract's data.
        /// That's why we immediately remove those terms. The terms that are already saved in the db are removed during the contract's saving using this list.
        /// </summary>
        private readonly List<string> _deletedTermIds = new List<string>();

        #endregion

        #region private methods

        private void Init(RobAccess robAccess, string robScreenId, string contractId, string robScreenActiveTab = null)
        {
            _robScreenId = robScreenId;
            _robScreenActiveTab = robScreenActiveTab;
            _robAccess = robAccess;
            ContractId = contractId;

            GetContractDetails().ContinueWith(t =>
            {
                // The init of customers and products is moved here because we need to know
                // if the trees are read only or not and we get that info in GetContractDetails().

                #region Customers & Products

                var args = CommonXml.GetBaseArguments("GetCustomerTree", _robScreenId);
                args.AddElement("ROBGroup_Idx", ContractId);

                Listings = new ListingsViewModel(DynamicDataAccess.GetGenericItem<TreeViewHierarchy>(StoredProcedure.RobGroup.GetCustomers, args), ListingsAccess.GetFilterProducts().Result);


                // We don't simply assign new TreeViewModel also in the second case (when the Customers != null)
                // because it was causing the customers tree control getting out of sync with the Customers' TreeViewModel -
                // something else was visible on the screen and something else was kept in the TreeViewModel.
                if (Customers == null) Customers = new TreeViewModel(Listings.Customers.Listings);
                else
                {
                    var newCustomers = new TreeViewModel(Listings.Customers.Listings);
                    Customers.Listings = newCustomers.Listings;
                }
                Customers.IsReadOnly = !IsContractAmendable;

                // We don't simply assign new TreeViewModel also in the second case (when the Products != null)
                // because it was causing the products tree control getting out of sync with the Products' TreeViewModel -
                // something else was visible on the screen and something else was kept in the TreeViewModel.
                if (Products == null) Products = new TreeViewModel(Listings.VisibleProducts.Listings);
                else
                {
                    if (Listings.VisibleProducts != null)
                        Products = new TreeViewModel(Listings.VisibleProducts.Listings);
                    else Products = new TreeViewModel();                                                       
                }
                Products.IsReadOnly = !IsContractAmendable;

                // Event fired up whenever the customers selection was changed.
                Customers.SelectionChanged += () =>
                {
                    // Adjusting which products nodes will be visible to a user basing on the selected customer nodes.
                    Listings.SetProductsFromListings(Customers.GetSelectedNodes());

                    if (Listings.VisibleProducts == null) Products.ClearTree();
                    else Products.Listings = Listings.VisibleProducts.Listings;

                    // Loading recipients options basing on the customers selection.
                    GetRecipients();
                };

                #endregion

                // This uses Listings when changing the Start date picker and Listings are initialized above.
                GetAddTermsDetails();

                // This sets recipients based on Customers that are initialized above.
                GetRecipients();

                // All the code below uses Customers when setting the IsCheckSafe property.
                SetupTabs();
                var loaders = TermsTabs.LoadContent().ToArray();

                Task.Factory.ContinueWhenAll(loaders, tt =>
                {
                    SetIsCheckSafe();
                });
            });

            //GetTabGrid().ContinueWith((t) =>
            //{
            //    SetIsCheckSafe();
            //    GetTermTab().Records.CollectionChanged += (sender, args) => SetIsCheckSafe();
            //});

            GetScenarios();
            GetStatuses();
        }

        private Task GetContractDetails()
        {
            return _robAccess.GetContractDetailsVerticalGrid(ContractId).ContinueWith(t =>
            {
                if (t.IsFaulted || t.IsCanceled) return;

                XElement xIsAmendable = t.Result.Element("Amendable");
                if (xIsAmendable != null) IsContractAmendable = xIsAmendable.Value == "1";

                ContractDetails = new RowViewModel(t.Result);
            });
        }

        private void GetAddTermsDetails()
        {
            _robAccess.GetAddNewTermVerticalGrid(ContractId).ContinueWith(t =>
            {
                if (t.IsFaulted || t.IsCanceled) return;
                RowViewModel addTermsDetails = new RowViewModel(t.Result);

                foreach (var record in addTermsDetails.Records)
                {
                    foreach (var property in record.Properties.Where(r => r.ControlType.Contains("down")).ToList())
                    {
                        property.DataSourceInput = property.DataSourceInput.Replace("&gt;", ">").Replace("&lt;", "<");
                        record.InitialDropdownLoad(property);
                    }
                }

                NewTermDetails = addTermsDetails;

                if (NewTermDetails != null)
                    foreach (var record in NewTermDetails.Records)
                    {
                        foreach (var proerty in record.Properties.Where(a => a.ControlType.ToLower() == "datepicker"))
                        {
                            proerty.PropertyChanged += PropertiesRVMPropertyChanged;
                        }
                    }
            });
        }

        private void PropertiesRVMPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (((RowProperty)sender).ControlType.ToLower() == "datepicker" && ((RowProperty)sender).Date != null && ((RowProperty)sender).Date == "Start")
            {
                Listings.DateTimeFromParent = DateTime.Parse(((RowProperty)sender).Value);
                Listings.SetProductsFromListings();
            }
        }

        private void GetStatuses()
        {
            _robGroupAccess.GetWorkflowStatuses(_robScreenId, ContractId)
                .ContinueWith(t =>
                {
                    AvailableContractStatuses = t.Result.ToObservableCollection();
                    SelectedContractStatus = AvailableContractStatuses.FirstOrDefault(r => r.IsSelected);
                }, App.Scheduler);
        }

        private void GetScenarios()
        {
            _robGroupAccess.GetScenarios(_robScreenId, ContractId)
                .ContinueWith(t =>
                {
                    AvailableScenarios.SetItems(t.Result);

                    var noSelectionScenario = AvailableScenarios.Items.FirstOrDefault(sc => sc.Idx == "-1");
                    if (noSelectionScenario != null)
                        AvailableScenarios.Items.First(sc => sc.Idx == "-1").IsSelected = true;
                }, App.Scheduler);
        }

        private void GetRecipients()
        {
            var selectedCustomers = Customers.GetSelectedNodes().Select(cust => cust.Idx).ToList();

            DynamicDataAccess.GetGenericEnumerableAsync<ComboboxItem>(StoredProcedure.RobGroup.GetRecipients, _robGroupAccess.GetRobRecipient(_robScreenId, ContractId, selectedCustomers))
                .ContinueWith(
            t =>
            {
                AvailableRecipients.SetItems(t.Result);
            });
        }

        private bool CanAdd(object obj)
        {
            // checking if the contract is loaded
            if (ContractDetails == null) return false;

            // checking if the term has type, start date, end date
            // (for now dates can have incorrect order - db validates)
            if (NewTermDetails == null || !NewTermDetails.AreRecordsFulfilled()) return false;

            // checking if there is any product selected
            if (Products == null) return false;
            var selectedProductsNodes = Products.GetSelectedNodes();
            if (selectedProductsNodes == null || !selectedProductsNodes.Any()) return false;

            if (ContractDetails != null && !ContractDetails.AreRecordsValid) return false;
            if (NewTermDetails != null && !NewTermDetails.AreRecordsValid) return false;

            return true;
        }

        private void AddTerm(object obj)
        {
            var xContractDetails = ContractDetails.ToCoreXml();
            var xNewTermsDetails = NewTermDetails.ToCoreXml();

            // checking if there is any customer selected
            var selectedCustomers = Customers.GetSelectedNodes().ToList();
            var xContractCustomers = selectedCustomers.Select(cust => cust.Idx);
            var xContractParentCustomers = selectedCustomers.Where(cust => cust.IsParentNode).Select(cust => cust.Idx);

            var selectedProducts = Products.GetSelectedNodes().ToList();
            var xNewTermProducts = selectedProducts.Select(prod => prod.Idx);
            var xNewTermParentProducts = selectedProducts.Where(prod => prod.IsParentNode).Select(prod => prod.Idx);

            // new records are added with a negative IDs so we send the current lowest id
            // to receive a new record with the ID one lower
            var minRecordId = GetTermTabContent().Records.Any() ? GetTermTabContent().Records.Min(rec => int.Parse(rec.Item_Idx)).ToString() : "0";

            _robAccess.AddTerm(ContractId, xContractDetails, xNewTermsDetails, xContractCustomers, xContractParentCustomers, xNewTermProducts, xNewTermParentProducts, minRecordId).ContinueWith(t =>
            {
                if (t.Result == null) return;

                Record term = Record.FromXml(t.Result.Element("RootItem"));
                term.Item_IsDisplayed = true;

                // loading and opening the inside grid of the row
                term.LoadExpandedGrid();
                term.IsDetailsViewModelVisible = true;

                //TermsGrid.AddRecord(term);

                var termTabContent = GetTermTabContent();
                termTabContent.AddRecord(term);
                //TermsTabs.SelectedTab.TabMainContent = new RecordViewModel(((RecordViewModel)TermsTabs.SelectedTab.TabMainContent).Records, ((RecordViewModel)TermsTabs.SelectedTab.TabMainContent).GridTitle);


                // Blank record is in the grid just in case when we don't have any other records (for example, when we create a new contract)
                // so we wouln't be able to figure out the grid scheme (columns, their types etc).
                // As soon as a user adds a new term we would remove the blank record (if it exists)
                var blankRecord = termTabContent.Records.FirstOrDefault(rec => rec.Item_Idx == "0");
                if (blankRecord != null) termTabContent.RemoveRecord(blankRecord);

                GetTermTab().ReloadCount();
                SetIsCheckSafe();
            });
        }

        private void Cancel(object obj)
        {
            IsContractAmendable = true;
            RedirectMe.RobSpecialListScreen(_robScreenId, null);
        }

        private bool CanSave(object obj)
        {
            // checking if the contract has name, start date, end date
            // (for now dates can have incorrect order - db validates)
            if (ContractDetails == null || !ContractDetails.AreRecordsFulfilled()) return false;

            // checking if there is any customer selected
            if (Customers == null) return false;
            var selectedCustomersNodes = Customers.GetSelectedNodes();
            if (selectedCustomersNodes == null || !selectedCustomersNodes.Any()) return false;

            // checking if there is any term created
            var termTabContent = GetTermTabContent();
            if (termTabContent == null || termTabContent.Records == null) return false;
            if (!termTabContent.Records.Any(rec => !string.IsNullOrEmpty(rec.Item_Idx) && rec.Item_Idx != "0")) return false;

            // checking if required terms details are fulfilled
            if (!termTabContent.AreRecordsFulfilled()) return false;

            // checking if all the terms are not marked as "to delete"
            var termsMarkedToDelete = termTabContent.Records.Where(rec => rec.HasChanges == 2).Select(rec => rec.Item_Idx).ToList();
            if (termTabContent.Records.Count == termsMarkedToDelete.Count) return false;

            if (!ContractDetails.AreRecordsValid) return false;

            return true;
        }

        private void SaveAndClose(object obj)
        {
            SaveContract(true); 
        }

        private void Save(object obj)
        {
            SaveContract(false);
        }

        private void SaveContract(bool close)
        {
            var xContractDetails = ContractDetails.ToCoreXml();

            var selectedCustomerNodes = Customers.GetSelectedNodes().ToList();
            var xContractCustomers = selectedCustomerNodes.Select(cust => cust.Idx);
            var xContractParentCustomers = selectedCustomerNodes.Where(cust => cust.IsParentNode).Select(cust => cust.Idx);

            var savedTermsMarkedToDelete = GetSavedTermsMarkedToDeleteIds();

            // ToXml method overloaded:
            // excludeRecordsMarkedToDelete = true
            // excludeRecordsWithoutChanges = true
            var xTerms = GetTermTabContent().ToXml(true, true);

            var statusId = SelectedContractStatus.ID;

            var recipientIds = new List<string>();
            if (AvailableRecipients.SelectedItems.Any())
                recipientIds.AddRange(AvailableRecipients.SelectedItems.Select(a => a.Idx));

            var scenariosIds = AvailableScenarios.SelectedItemIdxs;

            _robAccess.SaveContract(_robScreenId, ContractId, xContractDetails, xContractCustomers,
               xContractParentCustomers, xTerms, savedTermsMarkedToDelete, statusId, recipientIds, scenariosIds).ContinueWith(
                   f =>
                   {
                       if (f.IsFaulted || f.IsCanceled || f.Result == null)
                       {
                           MessageBoxShow("This contract failed to save", "Contract editor");
                           return;
                       }
                       else // if save successful..
                       {
                           var root = f.Result.Element("Message");
                           ContractId = root.Element("ROBGroup_Idx").MaybeValue();
                           _isContractAmendable = root.Element("IsAmendable").MaybeValue() == "1";

                           MessageBox.Show(root.Element("OutcomeMsg").MaybeValue(), "Contract editor", MessageBoxButton.OK, MessageBoxImage.Information);

                           if (close)  
                               GoBack();
                           else
                               Init(_robAccess, _robScreenId, ContractId, _robScreenActiveTab);

                       }


                   });
        }

        private void GoBack()
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                Cancel(null);
            });
        }

        private void Reload(object obj)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            Init(_robAccess, _robScreenId, ContractId, _robScreenActiveTab);
            Mouse.OverrideCursor = null;
        }

        public void SetIsCheckSafe()
        {
            Customers.IsCheckSafe = !GetTermTabContent().HasNonEmptyRecords;
        }

        private List<string> GetSavedTermsMarkedToDeleteIds()
        {
            var savedTermsMarkedToDelete =
                GetTermTabContent().Records
                .Where(rec => rec.HasChanges == 2 && int.Parse(rec.Item_Idx) > 0)
                .Select(term => term.Item_Idx)
                .ToList();

            // we also mark to delete these terms that were previously reported as possible no longer matching the contract's data
            // (and removed from the grid at that time)
            // for more details see the _deletedTermIds description property in the "Terms" region
            savedTermsMarkedToDelete.AddRange(_deletedTermIds);
            return savedTermsMarkedToDelete;
        }

        #endregion

        #region ctors

        public RobContractsEditorViewModel(RobAccess robAccess, string robScreenId, string contractId, string robScreenActiveTabName = "")
        {
            Init(robAccess, robScreenId, contractId, robScreenActiveTabName);
            CancelCommand = new ViewCommand(Cancel);
            SaveAndCloseCommand = new ViewCommand(CanSave, SaveAndClose);
            SaveCommand = new ViewCommand(CanSave, Save);
            ReloadCommand = new ViewCommand(Reload);
            App.Navigator.EnableNavigation(false);
            AddCommentCommand = new ViewCommand(CanAddComment, AddComment);
        }

        #endregion

        #region Comments

        private void LoadNotes()
        {
            // [app].[Procast_SP_Contract_GetComments]
            _robAccess.GetNotes(ContractId).ContinueWith(
                t =>
                {
                    Notes = new ObservableCollection<Note>(t.Result);
                });
        }

        private ObservableCollection<Note> _notes;
        public ObservableCollection<Note> Notes
        {
            get { return _notes; }
            set
            {
                _notes = value;
                NotifyPropertyChanged(this, vm => vm.Notes);
            }
        }

        private string _newComment;
        public string NewComment
        {
            get
            {
                return _newComment;

            }
            set
            {
                _newComment = value;
                NotifyPropertyChanged(this, vm => vm.NewComment);
            }
        }



        public ICommand AddCommentCommand { get; set; }

        public bool CanAddComment(object param = null)
        {
            CanAddCommentBool = HasID();
            return CanAddCommentBool;
        }

        private bool HasID()
        {
            return (!String.IsNullOrWhiteSpace(ContractId) && ContractId != "0");
        }

        private bool _canAddCommentBool;
        public bool CanAddCommentBool
        {
            get { return _canAddCommentBool; }
            set
            {
                _canAddCommentBool = value;
                NotifyPropertyChanged(this, vm => vm.CanAddCommentBool);
            }
        }

        public void AddComment(object param)
        {
            if (NewComment.Trim() == "")
            {
                MessageBoxShow("Please enter a value.", "Warning");
                return;
            }

            if (HasID() == false)
            {
                MessageBoxShow("Comments can not be added until this promotion has been saved", "Warning");
                return;
            }

            try
            {
                string res = _robAccess.AddNote(ContractId, NewComment);
                //MessageBoxShow(res); 
                NewComment = "";
                LoadNotes();

            }
            catch (ExceedraDataException ex)
            {
                MessageBoxShow(ex.Message, "Error");
            }
        }

        #endregion

        #region properties

        // the super and absolutely general section - in charge if the whole screen

        public bool IsContractAmendable
        {
            get { return _isContractAmendable; }
            set
            {
                _isContractAmendable = value;
                NotifyPropertyChanged(this, vm => vm.IsContractAmendable);
            }
        }

        // the very top section

        public ICommand ReloadCommand { get; set; }

        // contract details section

        /// <summary>
        /// The id of the contract currently being edited.
        /// If it is a new contract the id is null.
        /// </summary>
        public string ContractId
        {
            get { return _contractId; }
            set
            {
                _contractId = value;
                NotifyPropertyChanged(this, vm => vm.ContractId);
            }
        }

        /// <summary>
        /// A vertical grid describing contract details
        /// such as its name, start & end dates
        /// </summary>
        public RowViewModel ContractDetails
        {
            get { return _contractDetails; }
            set
            {
                _contractDetails = value;
                NotifyPropertyChanged(this, vm => vm.ContractDetails);
            }
        }

        public Visibility AreRecipientsVisible
        {
            get { return App.Configuration.ROBScreens.FirstOrDefault(a => a.RobAppType == _robScreenId).RobAppRecipient ? Visibility.Visible : Visibility.Collapsed; }
        }

        /// <summary>
        /// Data for the customers & products trees. This data is not directly visible to a user - it's being shown by the Customers & Products TreeViewModels.
        /// It is here only to provide the interaction mechanism between the customers tree and the products tree so that the products tree is amended based on the customers selection.
        /// </summary>
        public ListingsViewModel Listings
        {
            get
            {
                return _listings;
            }
            set
            {
                _listings = value;
                NotifyPropertyChanged(this, vm => vm.Listings);
                NotifyPropertyChanged(this, vm => vm.Customers);
            }
        }

        /// <summary>
        /// Acts like a facade to Listings' customers - exposes its Listings.Customers.
        /// See the initiliaztion in the Init() method.
        /// Provides all the independent logic for the customers tree - selections, toggle planning etc
        /// but not the interaction between the Customers tree and the Products tree!
        /// </summary>
        public TreeViewModel Customers
        {
            get
            {
                return _customers;
            }
            set
            {
                _customers = value;
                NotifyPropertyChanged(this, vm => vm.Customers);
            }
        }

        /// <summary>
        /// Acts like a facade to Listings' products - exposes its Listings.VisibleProducts.
        /// See the initiliaztion in the Init() method.
        /// Provides all the independent logic for the products tree - selections, toggle planning etc
        /// but not the interaction between the Customers tree and the Products tree!
        /// </summary>
        public TreeViewModel Products
        {
            get
            {
                return _products;
            }
            set
            {
                _products = value;
                NotifyPropertyChanged(this, vm => vm.Products);
            }
        }

        // add terms section

        /// <summary>
        /// A vertical grid describing new term details
        /// such as its type (and subtype) and start & end dates
        /// </summary>
        public RowViewModel NewTermDetails
        {
            get { return _newTermDetails; }
            set
            {
                _newTermDetails = value;
                NotifyPropertyChanged(this, vm => vm.NewTermDetails);
            }
        }

        /// <summary>
        /// Adds a new term to the control in each tab of the terms tab grid.
        /// The term is added LOCALLY and will be saved during invoking the SaveAndClose command.
        /// </summary>
        public ICommand AddTermCommand
        {
            get { return new ViewCommand(CanAdd, AddTerm); }
        }

        // tab grid section

        /// <summary>
        /// A tab control containing all of the contract terms (both just created and already existing - created in previous edits)
        /// and presenting them in various formats such as a dynamic grid, a schedule grid etc.)
        /// </summary>
        //public TabbedViewModel ContractTerms
        //{
        //    get { return _contractTerms; }
        //    set
        //    {
        //        _contractTerms = value;
        //        NotifyPropertyChanged(this, vm => vm.ContractTerms);
        //    }
        //}



        private TabViewModel _termsTabs;

        public TabViewModel TermsTabs
        {
            get { return _termsTabs; }
            set
            {
                _termsTabs = value;
                NotifyPropertyChanged(this, vm => vm.TermsTabs);
            }
        }

        private void SetupTabs()
        {
            _firstLoad = "1";

            Tab fstTab = new Tab
            {
                TabName = "Terms",
                TabTitle = App.CurrentLang.GetValue("Label_Terms", "Terms"),
                TabType = "HorizontalGrid",
                TabMainContentProc = StoredProcedure.RobGroup.GetTerms,
            };

            TermsTabs = new TabViewModel(new List<Tab> { fstTab })
            {
                IsExportVisible = true,
                GetFilterXml = GetXml
            };
        }

        private Tab GetTermTab()
        {
            if (TermsTabs == null || TermsTabs.Tabs == null) return null;

            return TermsTabs.GetTab("Terms");
        }

        public RecordViewModel GetTermTabContent()
        {
            var tab = GetTermTab();
            return tab == null ? null : (RecordViewModel)tab.TabMainContent;
        }

        private string _firstLoad = "1";
        private XElement GetXml(string rootTag = "DataSourceInput")
        {
            rootTag = String.IsNullOrWhiteSpace(rootTag) ? "DataSourceInput" : rootTag;
            var args = CommonXml.GetBaseArguments(rootTag);
            args.AddElement("AppType_Idx", _robScreenId);
            args.AddElement("ROBGroup_Idx", ContractId);
            args.AddElement("IsFirstLoad", _firstLoad);

            _firstLoad = "0";
            return args;
        }


        // WARNING: TODO:
        // This should be replaced as soon as it is only possible!
        // In this manner, The RecordViewModel, the RowViewModel etc should
        // have an abstract class above which methods will be used in this view model!
        // Therefore, there would be no need to cast the tabs from the ContractTerms to the RecordViewModel.
        //public RecordViewModel TermsGrid
        //{
        //    get
        //    {
        //        var firstTabControlRecord = ContractTerms.Records.FirstOrDefault();
        //        if (firstTabControlRecord != null)
        //        {
        //            var firstTab = firstTabControlRecord.Properties.FirstOrDefault();
        //            if (firstTab != null)
        //                return firstTab.TabContent as RecordViewModel;
        //        }

        //        return null;
        //    }
        //}

        // bottom section

        /// <summary>
        /// Collection containing all the contract statuses available in the workflow
        /// (even those greyed out that won't be able to be selected)
        /// </summary>
        public ObservableCollection<Status> AvailableContractStatuses
        {
            get { return _availableContractStatuses ?? (_availableContractStatuses = new ObservableCollection<Status>()); }
            set
            {
                _availableContractStatuses = value;
                NotifyPropertyChanged(this, vm => vm.AvailableContractStatuses);
            }
        }

        public Status SelectedContractStatus
        {
            get { return _selectedContractStatus; }
            set
            {
                _selectedContractStatus = value;
                NotifyPropertyChanged(this, vm => vm.SelectedContractStatus);
            }
        }

        public MultiSelectViewModel AvailableScenarios
        {
            get { return _availableScenarios ?? (_availableScenarios = new MultiSelectViewModel()); }
            set
            {
                _availableScenarios = value;
                NotifyPropertyChanged(this, vm => vm.AvailableScenarios);
            }
        }

        public MultiSelectViewModel AvailableRecipients
        {
            get { return _availableRecipients ?? (_availableRecipients = new MultiSelectViewModel()); }
            set
            {
                _availableRecipients = value;
                NotifyPropertyChanged(this, vm => vm.AvailableRecipients);
            }
        }

        public RobRecipient SelectedRecipient
        {
            get { return _selectedRecipient; }
            set
            {
                _selectedRecipient = value;
                NotifyPropertyChanged(this, vm => vm.SelectedRecipient);
            }
        }

        private ViewCommand _cancelCommand;
        public ViewCommand CancelCommand {
            get { return _cancelCommand; }
            set
            {
                _cancelCommand = value;
                NotifyPropertyChanged(this, vm => vm.CancelCommand);
            } 
        }

        /// <summary>
        /// TODO: document it
        /// </summary>
        public ICommand SaveAndCloseCommand { get; set; }
        public ICommand SaveCommand { get; set; }

        #endregion

        #region public methods

        public void PurgeTerms()
        {
            for (int i = 0; i < GetTermTabContent().Records.Count; i++)
            {
                int termId = int.Parse(GetTermTabContent().Records[i].Item_Idx);

                // This term was created before and is now saved in the database so we need to inform the db to delete it when a user will save this contract.
                // Otherwise, we can remove the term immediately without notifying the database.
                if (termId >= 0)
                    _deletedTermIds.Add(GetTermTabContent().Records[i].Item_Idx);

                GetTermTabContent().RemoveRecord(GetTermTabContent().Records[i]);
                i--;
            }
            GetTermTab().ReloadCount();
        }

        #endregion
    }
}