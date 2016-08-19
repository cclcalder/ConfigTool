using Model;
using Model.DataAccess;
using Model.DTOs;
using Model.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ViewHelper;
using ViewModels;

namespace WPF.ViewModels.Claims
{
    public class ClaimEntryViewModel : ViewModelBase
    {
        #region private fields

        private readonly ClaimsAccess _claimsAccess = new ClaimsAccess();
        private ClaimStatus _selectedClaimEntryStatus;
        private ObservableCollection<ClaimStatus> _claimEntryStatusList = new ObservableCollection<ClaimStatus>();
        private ObservableCollection<ClaimEntryEventViewModel> _claimEntryEventList = new ObservableCollection<ClaimEntryEventViewModel>();
        private ObservableCollection<ClaimEntryEventViewModel> _selectedClaimEntryEventList = new ObservableCollection<ClaimEntryEventViewModel>();
        private ClaimCustomerLevel _selectedCustomerLevel;
        private ObservableCollection<ClaimCustomerLevel> _customerLevelList = new ObservableCollection<ClaimCustomerLevel>();
        private Customer _selectedCustomer;
        private ObservableCollection<Customer> _customerList = new ObservableCollection<Customer>();
        private string _selectedFilePath = string.Empty;
        private bool _hasData = false;
        private bool _showCopyLineAbove = false;
        private bool _canCopyLineAbove = false;
        private bool _canBeDeleted = false;
        private bool _hasDataComplete = false;
        private string _salesOrganizationId;
        #endregion

        #region events
        public event EventHandler OnEntryRowBeginAddData;
        public delegate void EntryRowAddDataCompleted(ClaimEntryViewModel entry);
        public event EntryRowAddDataCompleted OnEntryRowAddDataCompleted;
        public delegate void Delete(ClaimEntryViewModel entry);
        public event Delete OnDelete;
        public delegate void EntryRowAddDataIncomplete(ClaimEntryViewModel entry);
        public event EntryRowAddDataIncomplete OnEntryRowAddDataIncomplete;
        public delegate void CopyLineAbove(ClaimEntryViewModel entry);
        public event CopyLineAbove OnCopyLineAbove;
        #endregion

        #region properties

        public ObservableCollection<ClaimEntryEventViewModel> ClaimEntryEventList
        {
            get { return _claimEntryEventList; }
            set
            {
                _claimEntryEventList = value;
                NotifyPropertyChanged(this, vm => vm.ClaimEntryEventList);
            }
        }

        public ObservableCollection<ClaimEntryEventViewModel> SelectedClaimEntryEventList
        {
            get { return _selectedClaimEntryEventList; }
            set
            {
                _selectedClaimEntryEventList = value;
                NotifyPropertyChanged(this, vm => vm.SelectedClaimEntryEventList);
            }
        }


        public ObservableCollection<ClaimCustomerLevel> CustomerLevelList
        {
            get
            {
                return _customerLevelList;
            }
        }

        public ClaimCustomerLevel SelectedCustomerLevel
        {
            get { return _selectedCustomerLevel; }
            set
            {
                _selectedCustomerLevel = value;
                NotifyPropertyChanged(this, vm => vm.SelectedCustomerLevel);
                ReloadCustomers();
                if (OnEntryRowBeginAddData != null)
                {
                    NotifyBeginAddData();
                }
            }
        }

        public ObservableCollection<Customer> CustomerList
        {
            get
            {
                return _customerList;
            }
        }

        public Customer SelectedCustomer
        {
            get { return _selectedCustomer; }
            set
            {
                if (_selectedCustomer != value)
                {
                    _selectedCustomer = value;
                    NotifyPropertyChanged(this, vm => vm.SelectedCustomer);

                    if (OnEntryRowBeginAddData != null)
                    {
                        NotifyBeginAddData();
                    }
                }
            }
        }

        public string SelectedFilePath
        {
            get
            {
                return _selectedFilePath;
            }
            set
            {
                _selectedFilePath = value;
                NotifyPropertyChanged(this, vm => vm.SelectedFilePath);

                if (OnEntryRowBeginAddData != null)
                {
                    NotifyBeginAddData();
                }
            }
        }

        public bool HasData
        {
            get
            {
                return _hasData;
            }
            set
            {
                _hasData = value;
                NotifyPropertyChanged(this, vm => vm.HasData);
            }
        }

        public bool ShowCopyLineAbove
        {
            get
            {
                return _showCopyLineAbove;
            }
            set
            {
                _showCopyLineAbove = value;
                NotifyPropertyChanged(this, vm => vm.ShowCopyLineAbove);
            }
        }

        public bool CanCopyLineAbove
        {
            get
            {
                return _canCopyLineAbove;
            }
            set
            {
                _canCopyLineAbove = value;
                NotifyPropertyChanged(this, vm => vm.CanCopyLineAbove);
            }
        }

        public bool CanBeDeleted
        {
            get
            {
                return _canBeDeleted;
            }
            set
            {
                _canBeDeleted = value;
                NotifyPropertyChanged(this, vm => vm.CanBeDeleted);
            }
        }

        public bool HasDataComplete
        {
            get
            {
                return _hasDataComplete;
            }
            set
            {
                _hasDataComplete = value;
                NotifyPropertyChanged(this, vm => vm.HasDataComplete);
            }
        }


        private string _claimReference = string.Empty;
        public string ClaimReference
        {
            get
            {
                return _claimReference;
            }
            set
            {
                _claimReference = value;
                NotifyPropertyChanged(this, vm => vm.ClaimReference);

                if (OnEntryRowBeginAddData != null)
                {
                    NotifyBeginAddData();
                }
            }
        }

        private string _claimLineDetail = string.Empty;
        public string ClaimLineDetail
        {
            get
            {
                return _claimLineDetail;
            }
            set
            {
                _claimLineDetail = value;
                NotifyPropertyChanged(this, vm => vm.ClaimLineDetail);

                if (OnEntryRowBeginAddData != null)
                {
                    NotifyBeginAddData();
                }
            }
        }

        private DateTime? _claimDate;
        public DateTime? ClaimDate
        {
            get
            {
                return _claimDate;
            }
            set
            {
                _claimDate = value;
                NotifyPropertyChanged(this, vm => vm.ClaimDate);

                if (OnEntryRowBeginAddData != null)
                {
                    NotifyBeginAddData();
                }
            }
        }

        private decimal? _claimValue;
        public decimal? ClaimValue
        {
            get
            {
                return _claimValue;
            }
            set
            {
                _claimValue = value;
                NotifyPropertyChanged(this, vm => vm.ClaimValue);

                if (OnEntryRowBeginAddData != null)
                {
                    NotifyBeginAddData();
                }
            }
        }

        private int _id;
        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                NotifyPropertyChanged(this, vm => vm.Id);
            }
        }
        #endregion

        #region commands
        public ICommand DeleteCommand
        {
            get;
            private set;
        }

        public ICommand CopyLineAboveCommand
        {
            get;
            private set;
        }

        #endregion

        #region ctors

        public ClaimEntryViewModel(int id, string salesOrganizationId, bool? showCopyLineAbove = null)
        {
            _salesOrganizationId = salesOrganizationId;
            this.Id = id;
            if (showCopyLineAbove.HasValue)
            {
                this.ShowCopyLineAbove = showCopyLineAbove.Value;
            }
            DeleteCommand = new ActionCommand(ExecuteDelete);
            CopyLineAboveCommand = new ActionCommand(ExecuteCopyLineAbove);
            this.InitData();
        }

        #endregion
        private bool isCopying = false;
        #region public methods
        public void CopyDataFrom(ClaimEntryViewModel anotherEntry)
        {
            isCopying = true;
            UpdateSelectedCustomerLevel(this.CustomerLevelList.SingleOrDefault(cl => cl.Id == anotherEntry.SelectedCustomerLevel.Id));
            this.SetCustomerList(anotherEntry.CustomerList);
            this.SelectedCustomer = anotherEntry.SelectedCustomer; //this.CustomerList.SingleOrDefault(cl => cl.ID == anotherEntry.SelectedCustomer.ID);
            this.ClaimReference = anotherEntry.ClaimReference;
            this.ClaimLineDetail = anotherEntry.ClaimLineDetail;
            this.ClaimDate = anotherEntry.ClaimDate;
            this.ClaimValue = anotherEntry.ClaimValue;
            this.SelectedFilePath = anotherEntry.SelectedFilePath;

            if (anotherEntry.SelectedClaimEntryEventList.Count > 0)
            {
                foreach (var evt in anotherEntry.SelectedClaimEntryEventList)
                {
                    var entryEvent = this.ClaimEntryEventList.SingleOrDefault(e => e.Data.Id == evt.Data.Id);
                    this.SelectedClaimEntryEventList.Add(entryEvent);
                }
            }

            isCopying = false;
        }

        public void UpdateSaleOrganization(string salesOrganizationId)
        {
            _salesOrganizationId = salesOrganizationId;
            ReloadCustomers();
        }

        #endregion

        #region private methods
        private void InitData()
        {
            //_claimsAccess.GetClaimCustomerLevelItems().ContinueWith(GetCustomerLevelItemsContinuation, App.Scheduler);
            ReloadCustomers();
        }

        private void ReloadCustomers()
        {
            if (CustomerLevelList.Any())
            {
                var reloadCustomersDTO = new GetCustomersOnClaimEntryDTO();
                reloadCustomersDTO.CustomerLevelId = SelectedCustomerLevel == null ? CustomerLevelList.First().Id : SelectedCustomerLevel.Id;
                reloadCustomersDTO.SalesOrganizationId = _salesOrganizationId;
                if (isCopying == false)
                {
                    _claimsAccess.GetCustomersOnClaimEntry(reloadCustomersDTO).ContinueWith(GetCustomersContinuation, App.Scheduler);
                }
            }
        }

        private void GetCustomerLevelItemsContinuation(Task<IList<ClaimCustomerLevel>> task)
        {
            Populate(this, task, _customerLevelList, null, vm => vm.CustomerLevelList);
        }

        private void GetCustomersContinuation(Task<IList<Customer>> task)
        {
            if (isCopying == false)
            {
                Populate(this, task, _customerList, null, vm => vm.CustomerList);
            }
        }

        private void NotifyBeginAddData()
        {
            HasData = true;
            CanBeDeleted = true;
            OnEntryRowBeginAddData(this, null);

            var result = CheckIfRowDataIsComplete();
            if (result == true && OnEntryRowAddDataCompleted != null)
            {
                HasDataComplete = true;
                OnEntryRowAddDataCompleted(this);
            }
            else if (OnEntryRowAddDataIncomplete != null)
            {
                HasDataComplete = false;
                OnEntryRowAddDataIncomplete(this);
            }
        }

        private bool CheckIfRowDataIsComplete()
        {
            if (SelectedCustomerLevel == null)
            {
                return false;
            }

            if (SelectedCustomer == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(ClaimReference))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(ClaimLineDetail))
            {
                return false;
            }

            if (ClaimDate.HasValue == false)
            {
                return false;
            }

            if (ClaimValue.HasValue == false)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(SelectedFilePath) && IsValidFilePath())
            {
                return false;
            }

            return true;
        }

        private bool IsValidFilePath()
        {
            return (File.Exists(SelectedFilePath) || SelectedFilePath.Contains("//"));
        }

        private void SetCustomerList(ObservableCollection<Customer> customerList)
        {
            _customerList = customerList;
            NotifyPropertyChanged(this, vm => vm.CustomerList);
        }

        private void ExecuteDelete()
        {
            if (OnDelete != null)
                OnDelete(this);
        }

        private void ExecuteCopyLineAbove()
        {
            if (OnCopyLineAbove != null)
                OnCopyLineAbove(this);
        }


        private void UpdateSelectedCustomerLevel(ClaimCustomerLevel claimCustomerLevel)
        {
            _selectedCustomerLevel = claimCustomerLevel;
            NotifyPropertyChanged(this, vm => vm.SelectedCustomerLevel);
            if (OnEntryRowBeginAddData != null)
            {
                NotifyBeginAddData();
            }
        }
        #endregion
    }
}
