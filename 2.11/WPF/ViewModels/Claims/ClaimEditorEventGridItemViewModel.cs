using Model.DataAccess;
using Model.DTOs;
using Model.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels;

namespace WPF.ViewModels.Claims
{
    public class ClaimEditorEventGridItemViewModel : ViewModelBase
    {
        #region private fields

        private readonly ClaimsAccess _claimsAccess = new ClaimsAccess();
        private bool _isSelected;
        private ObservableCollection<ClaimEditorProductGridItemViewModel> _productGridItems = new ObservableCollection<ClaimEditorProductGridItemViewModel>();
        private string _apportionmentType;
        private string _claimApportionment;
        #endregion

        #region properties

        public string ClaimId { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string EventType { get; set; }
        public string EventSubType { get; set; }
        public string EventStatus { get; set; }
        public string Author { get; set; }
        public string TotalAccrual { get; set; }
        public string Settled { get; set; }
        public string AvailableAccrual { get; set; }

        public string ApportionmentType
        {
            get
            {
                return _apportionmentType;
            }
            set
            {
                _apportionmentType = value;
                NotifyPropertyChanged(this, vm => vm.ApportionmentType);
            }
        }

        public string ClaimApportionment
        {
            get
            {
                return _claimApportionment;
            }
            set
            {
                if (ValidateApportionment(value))
                {
                    _claimApportionment = value;
                    NotifyPropertyChanged(this, vm => vm.ClaimApportionment, vm => vm.ApportionedAmount, vm => vm.RemainingAccrual, vm => vm.IsPercentage);
                    ApportionmentType = IsPercentage ? "P" : "V";
                    UpdateProductGridItems();
                }
                else
                {
                    NotifyPropertyChanged(this, vm => vm.ClaimApportionment, vm => vm.ApportionedAmount, vm => vm.RemainingAccrual, vm => vm.IsPercentage);
                }
            }
        }

        public string ClaimApportionmentValue
        {
            get
            {
                double claimApportionment = CalculateClaimApportionmentValue();
                return string.Format("{0:0.00}", claimApportionment);
            }
        }

        public string ApportionedAmount
        {
            get
            {
                double apportionmentAmount = CalculateApportionedAmount();
                return string.Format("{0:0.00}", apportionmentAmount);
            }
        }

        public string RemainingAccrual
        {
            get
            {

                double apportionmentAmount = CalculateApportionedAmount();
                double availableAccrual;
                double.TryParse(AvailableAccrual, out availableAccrual);
                return string.Format("{0:0.00}", availableAccrual - apportionmentAmount);
            }
        }

        public string ClaimNetValue { get; set; }

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                if (_isSelected)
                {
                    OnSelectedEventChanged(new SelectedEventArgs() { EventId = this.Id });
                }
                NotifyPropertyChanged(this, vm => vm.IsSelected);
            }
        }

        public ObservableCollection<ClaimEditorProductGridItemViewModel> ProductGridItems
        {
            get { return _productGridItems; }
            set
            {
                if (_productGridItems != value)
                {
                    _productGridItems = value;
                    NotifyPropertyChanged(this, vm => vm.ProductGridItems);
                }
            }
        }

        public bool IsPercentage
        {
            get
            {
                return ClaimApportionment.Contains("%");
            }
        }

        #endregion

        #region ctors

        public ClaimEditorEventGridItemViewModel()
        {

        }

        public ClaimEditorEventGridItemViewModel(EventItem eventItem, string claimNetValue, Apportionment apportionment, string claimId)
        {
            this.Id = eventItem.Id;
            this.Name = eventItem.Name;
            this.EventType = eventItem.EventType;
            this.EventSubType = eventItem.EventSubType;
            this.EventStatus = eventItem.EventStatus;
            this.Author = eventItem.Author;
            this.TotalAccrual = eventItem.TotalAccrual;
            this.Settled = eventItem.Settled;
            double settled, totalAccrual, apportionedAmountRaw;
            double.TryParse(this.Settled, out settled);
            double.TryParse(this.TotalAccrual, out totalAccrual);
            this.AvailableAccrual = string.Format("{0:0.00}", totalAccrual - settled);
            double.TryParse(apportionment.ClaimApprotionmentValue, out apportionedAmountRaw);
            this.ClaimNetValue = claimNetValue;
            this.ClaimId = claimId;
            this.ApportionmentType = apportionment.ClaimApprotionmentType;
            this.ClaimApportionment = ApportionmentType == "P" ? (apportionedAmountRaw * 100).ToString() + "%" : apportionment.ClaimApprotionmentValue;
            GetEventProducts();
        }

        #endregion

        #region public methods

        public void UpdateNetClaimValue(string newValue)
        {
            ClaimNetValue = newValue;
            NotifyPropertyChanged(this, vm => vm.ClaimApportionment, vm => vm.ApportionedAmount, vm => vm.RemainingAccrual);
            UpdateProductGridItems();
        }

        private void UpdateProductGridItems()
        {
            foreach (var productItem in ProductGridItems)
            {
                productItem.UpdateEventApportionedAmount(this.ApportionedAmount);
            }
        }

        #endregion

        #region events

        public event EventHandler<SelectedEventArgs> SelectedEventChanged;

        #endregion

        #region protected methods

        protected virtual void OnSelectedEventChanged(SelectedEventArgs e)
        {
            EventHandler<SelectedEventArgs> handler = SelectedEventChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region private methods

        private bool ValidateApportionment(string rawValue)
        {
            double value;
            bool isValid = double.TryParse(rawValue.Remove("%"), out value);
            if (isValid)
            {
                isValid = value >= 0;
                if(isValid==false)
                {
                    MessageBoxShow("Please specify value greater than or equal to 0.");
                }
            }
            else
            {
                MessageBoxShow("Please specify valid value.");
            }

            return isValid;
        }

        private double CalculateClaimApportionmentValue()
        {
            double claimApportionment;
            if (IsPercentage)
            {
                double.TryParse(ClaimApportionment.Replace("%", string.Empty), out claimApportionment);
                return (claimApportionment / 100);
            }
            else
            {
                double.TryParse(ClaimApportionment, out claimApportionment);
                return claimApportionment;
            }
        }

        private double CalculateApportionedAmount()
        {
            double claimApportionment, apportionmentAmount;
            claimApportionment = CalculateClaimApportionmentValue();
            if (IsPercentage)
            {
                double claimValue;
                double.TryParse(ClaimNetValue, out claimValue);
                apportionmentAmount = claimValue * claimApportionment;
            }
            else
            {
                apportionmentAmount = claimApportionment;
            }

            return apportionmentAmount;
        }

        private void GetEventProducts()
        {
            GetEventProductsDTO getEventProductsDTO = new GetEventProductsDTO() { ClaimId = this.ClaimId, EventId = this.Id };
            _claimsAccess.GetEventProducts(getEventProductsDTO).ContinueWith(GetEventProductsContinuation, App.Scheduler);
        }

        private void GetEventProductsContinuation(Task<IList<EventProduct>> task)
        {
            if (task.IsFaulted || task.IsCanceled || task.Result == null || task.Result.Count == 0) return;
            Populate(this, task, _productGridItems, r => r.Select(cs => new ClaimEditorProductGridItemViewModel(cs, GetEventApportionedAmount(), this.Id)).ToList(), null, vm => vm.ProductGridItems);
        }

        private double GetEventApportionedAmount()
        {
            double apportionedAmount;
            double.TryParse(this.ApportionedAmount, out apportionedAmount);

            return apportionedAmount;
        }

        #endregion
    }

    public class SelectedEventArgs : EventArgs
    {
        public string EventId { get; set; }
    }
}
