using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using Exceedra.Common;
using Exceedra.Common.Utilities;
using Exceedra.Controls.DynamicGrid.ViewModels;
using Exceedra.Controls.DynamicRow.Models;
using Exceedra.Controls.DynamicRow.ViewModels;
using Exceedra.Controls.Messages;
using Exceedra.MultiSelectCombo.ViewModel;
using Model;
using Model.DataAccess;
using Model.DataAccess.Listings;
using Model.DataAccess.ROBGroups;
using Model.Entity.Generic;
using Model.Entity.ROBs;
using ViewHelper;
using ViewModels;
using WPF.Navigation;
using WPF.UserControls.Listings;
using WPF.UserControls.Trees.ViewModels;
using Status = Model.Entity.Generic.Status;
using ViewModelBase = ViewModels.ViewModelBase;

namespace WPF.ViewModels.ROBGroups
{
    public class GroupCreatorViewModel : ViewModelBase
    {
        private readonly ROBCreatorAccess _access;
        private readonly GroupEditorAccess _robGroupAccess;
        private readonly string _appTypeIdx;
        private string _robGroupIdx;
        BackgroundWorker _loadProductsBackgroundWorker;
        readonly BackgroundWorker _listingsBackgroundWorker = new BackgroundWorker();



        public GroupCreatorViewModel(string appTypeIdx, string robGroupIdx = null)
        {
            Customers.PropertyChanged += CustomersOnPropertyChanged;
            CloseCommand = new ViewCommand(Close);
            _access = new ROBCreatorAccess(appTypeIdx);
            _robGroupAccess = new GroupEditorAccess();
            _appTypeIdx = appTypeIdx;
            _robGroupIdx = robGroupIdx;
            _loadProductsBackgroundWorker = CreateLoadProductsWorker();
            _listingsBackgroundWorker.DoWork += _listingsBackgroundWorker_DoWork;

            LoadData();
        }

        private void LoadData()
        {
            LoadRobs();

            _listingsBackgroundWorker.RunWorkerAsync();

            LoadProperties();
            GetScenarios();
            GetStatuses();
        }

        private BackgroundWorker CreateLoadProductsWorker()
        {
            BackgroundWorker newWorker = new BackgroundWorker();
            newWorker.WorkerSupportsCancellation = true;
            newWorker.DoWork += _loadProductsBackgroundWorker_DoWork;
            return newWorker;
        }

        private void LoadProducts()
        {
            //ListingsVM.SetProductsFromListings();
            var tempTVM = new TreeViewModel(ListingsVM.VisibleProducts.Listings);
            //DeselectAllProducts(tempTVM);

            tempTVM.ListTree[0].CollapseAll();

            Products = tempTVM;
            Products.IsReadOnly = IsGroupReadOnly;
        }

        void _listingsBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Products.IsTreeLoading = true;

            //ListingsVM = new ListingsViewModel(thisAccess.GetROBsFilterCustomers, thisAccess.GetROBsFilterProducts);
            ListingsVM = new ListingsViewModel(ListingsAccess.GetFilterCustomers().Result, ListingsAccess.GetFilterProducts(false, true).Result);


            Products.IsTreeLoading = false;

            LoadCustomerLevels();
        }

        private void PropertiesRVMPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (((RowProperty)sender).ControlType.ToLower() == "datepicker" && ((RowProperty)sender).Date != null && ((RowProperty)sender).Date == "Start")
            {
                ListingsVM.DateTimeFromParent = DateTime.Parse(((RowProperty)sender).Value);
                ListingsVM.SetProductsFromListings();
            }
        }

        private void LoadCustomerLevels()
        {
            CustomerLevels = _access.GetCustomerLevels(_robGroupIdx).Result;
        }

        private void LoadCustomers()
        {
            Customers.SetItems(_access.GetCustomers(SelectedCustomerLevel.Idx, _robGroupIdx).Result);
        }

        private void LoadRobs()
        {
            if (!string.IsNullOrEmpty(_robGroupIdx))
            {
                var existingRobGroups = new RecordViewModel(_access.GetROBGroup(new XElement("ROBGroup_Idx", _robGroupIdx)).Result);
                LoadROBGroup(existingRobGroups);
            }
            else
            {
                ROBGroupRVM = new RecordViewModel(false);
                ROBGroupRVM.SerializeState();
            }

        }

        private void LoadProperties()
        {
            _access.GetProperties(_robGroupIdx).ContinueWith(t => GetPropertiesContinuation(t.Result));
        }

        private void GetScenarios()
        {
            _robGroupAccess.GetScenarios(_appTypeIdx, _robGroupIdx)
                .ContinueWith(t =>
                {
                    AvailableScenarios.SetItems(t.Result);

                    //var noSelectionScenario = AvailableScenarios.Items.FirstOrDefault(sc => sc.Idx == "-1");
                    //if (noSelectionScenario != null)
                    //    SelectedScenarios = new ObservableCollection<Scenario> { noSelectionScenario };

                    //else
                    //{
                    //    var selectedScenarios = AvailableScenarios.Where(sc => sc.IsSelected);
                    //    SelectedScenarios = new ObservableCollection<Scenario>(selectedScenarios);
                    //}
                }, App.Scheduler);
        }

        private void GetStatuses()
        {
            _robGroupAccess.GetWorkflowStatuses(_appTypeIdx, _robGroupIdx)
                .ContinueWith(t =>
                {
                    AvailableStatuses = t.Result.ToObservableCollection();
                    SelectedStatus = AvailableStatuses.FirstOrDefault(r => r.IsSelected);
                }, App.Scheduler);
        }

        private void GetPropertiesContinuation(XElement result)
        {
            IsGroupReadOnly = result.Element("Amendable").MaybeValue() == "0";
            PropertiesRVM = new RowViewModel(result);

            foreach (var record in PropertiesRVM.Records)
            {
                foreach (var property in record.Properties.Where(r => r.ControlType.Contains("down")).ToList())
                {
                    property.DataSourceInput = property.DataSourceInput.Replace("&gt;", ">").Replace("&lt;", "<");
                    record.InitialDropdownLoad(property);
                }
            }

            if (PropertiesRVM != null)
                foreach (var record in PropertiesRVM.Records)
                {
                    foreach (var proerty in record.Properties.Where(a => a.ControlType.ToLower() == "datepicker"))
                    {
                        proerty.PropertyChanged += PropertiesRVMPropertyChanged;
                    }
                }

            PropertiesRVM.SerializeState();
        }

        private void LoadROBGroup(RecordViewModel recordViewModel)
        {
            ROBGroupRVM = recordViewModel;

            var dependentColumns = ROBGroupRVM.Records.SelectMany(x => x.Properties).Select(y => y.DependentColumn).Distinct().ToArray();

            foreach (var col in ROBGroupRVM.Records)
                foreach (var p in col.Properties.Where(r =>
                    r.ControlType.Contains("down") &&
                    !dependentColumns.Contains(r.ColumnCode)
                    ).ToList())
                    col.InitialDropdownLoad(p);

            ROBGroupRVM.UpdateHeaders();

            ROBGroupRVM.SerializeState();
        }

        private void SaveROBGroup(bool close)
        {
            XElement[] args =
                            {
                    new XElement("Properties", PropertiesRVM.ToCoreXml().Root),
                    new XElement("CustomerLevel", SelectedCustomerLevel.Idx),

                    new XElement("Customers",
                    from selectedCustomerIdx in Customers.SelectedItemIdxs
                    select new XElement("Customer", selectedCustomerIdx)),

                    new XElement("Products",
                    from selectedProduct in Products.GetSelectedNodes()
                    select new XElement("Product",
                        new XElement("Idx", selectedProduct.Idx),
                        new XElement("IsParentNode", selectedProduct.IsParentNode ? "1" : "0"))
                    ),

                    new XElement("ROBGroup", ROBGroupRVM.ToXml().Root),

                    new XElement("Status_Idx", SelectedStatus.ID),

                    new XElement("Scenarios",
                    from selectedScenario in AvailableScenarios.SelectedItems
                    select new XElement("Idx", selectedScenario.Idx))
                };

            var result = _access.SaveROBGroup(_robGroupIdx, args);
            var isSavedSuccessfully = result.ToString().ToLower().Contains("success");

            if (isSavedSuccessfully)
                if (close)
                    Close();
                else
                {
                    _robGroupIdx = _robGroupIdx ?? result.Element("ROBGroup_Idx").MaybeValue();
                    Reload(null);
                }
        }

        private void LoadImpacts(string selectedIdx)
        {
            Impacts = _access.GetImpacts(selectedIdx).Select(f => new ImpactViewModel(f)).ToList();
        }

        public void UpdateImapctFormat(ImpactOption selectedItem)
        {
            if (selectedItem != null)
                Impacts.FirstOrDefault(a => a.Options.Contains(selectedItem)).UpdateFormatAndAmountFromSelection(selectedItem);
        }

        private XElement GetImpactOptionsAsXml()
        {
            var xml = new XElement("ImpactOptions");
            foreach (var impact in Impacts)
            {
                decimal value;
                var amount = impact.Amount;
                if (amount.Contains("%"))
                    amount = amount.Replace("%", "");

                decimal.TryParse(amount, NumberStyles.Any, CultureInfo.CurrentUICulture, out value);

                var option = new XElement("Option");
                option.AddElement("Idx", impact.SelectedOption.ID);
                option.AddElement("Value", value);

                xml.Add(option);
            }

            return xml;
        }

        private void SetCustomerInListings()
        {
            //ListingsVM.SelectedCustomers = new ObservableCollection<TreeViewHierarchy>();
            ListingsVM.Customers.GetFlatTree()
                .Where(c => c.IsSelectedBool == true).Do(c => c.IsSelectedBool = false);

            var custsToSelect =
                ListingsVM.Customers.GetFlatTree().Where(cust => Customers.SelectedItemIdxs.Contains(cust.Idx));

            custsToSelect.Do(c =>
            {
                c.IsSelectedBool = true;
                ListingsVM.Customers.GetSelected(c);
            });
        }

        private void DeselectAllProducts(TreeViewModel tvm)
        {
            //We are using the rob filters listing so just set to deselected here so we ignore the defaults.
            if (tvm.ListTree[0].ParentIdx == null)
            {
                tvm.ListTree[0].IsSelectedBool = false;
                tvm.GetSelected(tvm.ListTree[0]);
            }
        }

        private void CloseROBGroup()
        {
            if (ROBGroupRVM.Records != null && PropertiesRVM.Records != null)
            {
                if (!XNode.DeepEquals(ROBGroupRVM.ToXml().Root, ROBGroupRVM.Serialization)
                    || !XNode.DeepEquals(PropertiesRVM.ToCoreXml().Root, PropertiesRVM.Serialization))
                {
                    string message = "Are you sure you want to cancel the curent operation?";

                    if (CustomMessageBox.Show(message, "Cancel", MessageBoxButton.YesNo, MessageBoxImage.Question) ==
                        MessageBoxResult.No)
                        return;
                }
            }

            Close();
        }

        private void Close()
        {
            RedirectMe.RobSpecialListScreen(_appTypeIdx, null);
        }

        #region Properties

        private bool _areProductsLoading = false;
        public bool AreProductsLoading
        {
            get { return _areProductsLoading; }
            set
            {
                _areProductsLoading = value;
                NotifyPropertyChanged(this, vm => vm.AreProductsLoading);
            }
        }

        private RowViewModel _propertiesRVM;

        public RowViewModel PropertiesRVM
        {
            get { return _propertiesRVM; }
            set
            {
                _propertiesRVM = value;
                SetPropertyChangedListeners(PropertiesRVM);
                NotifyPropertyChanged(this, vm => vm.PropertiesRVM);
            }
        }

        private bool _isGroupReadOnly;
        public bool IsGroupReadOnly
        {
            get { return _isGroupReadOnly; }
            set
            {
                _isGroupReadOnly = value;
                NotifyPropertyChanged(this, vm => vm.IsGroupReadOnly);
                Products.IsReadOnly = IsGroupReadOnly;
            }
        }

        private List<ComboboxItem> _customerLevels;

        public List<ComboboxItem> CustomerLevels
        {
            get { return _customerLevels; }
            set
            {
                _customerLevels = value;
                SelectedCustomerLevel = CustomerLevels.FirstOrDefault(level => level.IsSelected);
                NotifyPropertyChanged(this, vm => vm.CustomerLevels);
            }
        }

        private ComboboxItem _selectedCustomerLevel;

        public ComboboxItem SelectedCustomerLevel
        {
            get { return _selectedCustomerLevel; }
            set
            {
                _selectedCustomerLevel = value;
                NotifyPropertyChanged(this, vm => vm.SelectedCustomerLevel);

                if (SelectedCustomerLevel != null)
                    LoadCustomers();
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

        private void SetCustomersAndLoadProducts()
        {
            if (_loadProductsBackgroundWorker.IsBusy)
            {
                _loadProductsBackgroundWorker.CancelAsync();
                _loadProductsBackgroundWorker = CreateLoadProductsWorker();
            }

            _loadProductsBackgroundWorker.RunWorkerAsync();
        }


        void _loadProductsBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Products.IsTreeLoading = true;

            if (Customers.SelectedItems != null && Customers.SelectedItems.Any())
            {
                SetCustomerInListings();
                LoadProducts();
            }
            else
            {
                Products = new TreeViewModel();
                Products.IsReadOnly = true;
            }
        }

        private TreeViewModel _products = new TreeViewModel();

        public TreeViewModel Products
        {
            get { return _products; }
            set
            {
                _products = value;
                NotifyPropertyChanged(this, vm => vm.Products);
            }
        }

        private RecordViewModel _robGroupRVM;

        public RecordViewModel ROBGroupRVM
        {
            get { return _robGroupRVM; }
            set
            {
                _robGroupRVM = value;
                NotifyPropertyChanged(this, vm => vm.ROBGroupRVM);
            }
        }

        private List<ImpactViewModel> _impacts;

        public List<ImpactViewModel> Impacts
        {
            get { return _impacts; }
            set
            {
                _impacts = value;
                NotifyPropertyChanged(this, vm => vm.Impacts);
            }
        }

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

        private ObservableCollection<Status> _availableStatuses;
        public ObservableCollection<Status> AvailableStatuses
        {
            get { return _availableStatuses ?? (_availableStatuses = new ObservableCollection<Status>()); }
            set
            {
                _availableStatuses = value;
                NotifyPropertyChanged(this, vm => vm.AvailableStatuses);
            }
        }

        private Status _selectedStatus;
        public Status SelectedStatus
        {
            get { return _selectedStatus; }
            set
            {
                _selectedStatus = value;
                NotifyPropertyChanged(this, vm => vm.SelectedStatus);
            }
        }

        private MultiSelectViewModel _availableScenarios;
        public MultiSelectViewModel AvailableScenarios
        {
            get { return _availableScenarios ?? (_availableScenarios = new MultiSelectViewModel()); }
            set
            {
                _availableScenarios = value;
                NotifyPropertyChanged(this, vm => vm.AvailableScenarios);
            }
        }

        private bool IsValidCustomerLevel
        {
            get { return SelectedCustomerLevel != null; }
        }

        private bool IsValidCustomer
        {
            get { return Customers.SelectedItems != null && Customers.SelectedItems.Any(); }
        }

        private bool IsValidProducts
        {
            get { return Products != null && Products.GetSelectedNodes().Any(); }
        }

        private bool IsValidImpactOptions
        {
            get { return Impacts != null && Impacts.Any() && Impacts.All(i => i.SelectedOption != null); }
        }

        private bool IsValidRobGroup
        {
            get { return ROBGroupRVM != null && ROBGroupRVM.Records != null && ROBGroupRVM.Records.Any(); }
        }

        //private bool _isTreeReadOnly;
        //public bool IsTreeReadOnly
        //{
        //    get { return _isTreeReadOnly; }
        //    set
        //    {
        //        _isTreeReadOnly = value;
        //        NotifyPropertyChanged(this, vm => vm.IsTreeReadOnly);
        //    }
        //}

        #endregion

        #region Commands

        public ICommand ApplyCommand
        {
            get { return new ViewCommand(CanApply, Apply); }
        }

        private bool CanApply(object o)
        {
            return IsValidCustomerLevel
                   && IsValidCustomer
                   && IsValidProducts
                   && IsValidImpactOptions
                   && PropertiesRVM != null
                   && PropertiesRVM.AreRecordsValid
                   ;
        }

        private void Apply(object obj)
        {
            var robGroupsViewModel = new RecordViewModel(_access.GetROBGroup(PropertiesRVM.ToCoreXml().Root, GetImpactOptionsAsXml(), Customers.SelectedItemIdxs, Products.GetAsHierarchyDictionary()).Result);
            LoadROBGroup(robGroupsViewModel);
        }

        public ICommand SaveCommand
        {
            get { return new ViewCommand(CanSave, Save); }
        }

        public ICommand SaveCloseCommand
        {
            get { return new ViewCommand(CanSave, SaveClose); }
        }

        
        public ICommand ReloadCommand
        {
            get { return new ViewCommand(Reload); }
        }

        private void Reload(object o)
        {
            LoadData();
        }

        private bool CanSave(object o)
        {
            return IsValidRobGroup
                   && PropertiesRVM != null
                   && PropertiesRVM.AreRecordsValid;
        }

        private void Save(object obj)
        {
            SaveROBGroup(false);
        }

        private void SaveClose(object o)
        {
            SaveROBGroup(true);
        }

        public ICommand CloseCommand { get; set; }

        private void Close(object obj)
        {
            CloseROBGroup();
        }

        #endregion

        private void SetPropertyChangedListeners(RowViewModel rvm)
        {
            if (rvm != null)
                rvm.Records.Do(rec => rec.Properties.Where(p => !String.IsNullOrWhiteSpace(p.UpdateToColumn)).Do(prop => prop.PropertyChanged += Dropdown_PropertyChanged));
        }

        private void Dropdown_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var cell = (RowProperty)sender;

            if (e.PropertyName == "SelectedItem")
            {
                LoadImpacts(cell.SelectedItem.Item_Idx);
            }

        }

        private void CustomersOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItems")
            {
                SetCustomersAndLoadProducts();
            }
        }
    }
}