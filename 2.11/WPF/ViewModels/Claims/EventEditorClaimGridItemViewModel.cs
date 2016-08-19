using Model.Entity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using ViewModels;

namespace WPF.ViewModels.Claims
{
    public class EventEditorClaimGridItemViewModel : ViewModelBase
    {
        #region private fields

        private string _claimApportionment;

        #endregion

        #region properties

        public string ApportionmentType { get; set; }
        public string ClaimId { get; set; }
        public string CustomerName { get; set; }
        public string ClaimReference { get; set; }
        public string ClaimLineDetail { get; set; }
        public string ClaimDate { get; set; }
        public string Status { get; set; }
        public string ScanLocation { get; set; }
        public string ClaimValue { get; set; }
        public string NetValue { get; set; }
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
                    OnSelectedClaimApportionmentChanged(new SelectedClaimArgs() { ApportionedAmount = this.ApportionedAmount, ClaimLineDetail = this.ClaimLineDetail, ClaimLineID = this.ClaimId });
                }
                NotifyPropertyChanged(this, vm => vm.ClaimApportionment, vm => vm.ApportionedAmount);
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

        public string ClaimApportionmentValue
        {
            get
            {
                double claimApportionment = CalculateClaimApportionmentValue();
				ReturnApportionmentValueWithFormat();
                return string.Format("{0:0.00}", claimApportionment);
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

        public EventEditorClaimGridItemViewModel()
        {

        }

        public EventEditorClaimGridItemViewModel(ClaimItem claimItem, ApportionmentItem apportionment)
        {
            this.ClaimId = claimItem.Id;
            this.CustomerName = claimItem.CustomerName;
            this.ClaimReference = claimItem.ClaimReference;
            this.ClaimLineDetail = claimItem.ClaimLineDetail;
            this.ClaimDate = claimItem.ClaimDate.ToString("dd/MM/yyyy");
            this.Status = claimItem.ClaimStatusName;
            this.ScanLocation = claimItem.ScanLocation;
            this.ClaimValue = claimItem.ClaimValue;
            this.NetValue = claimItem.ClaimNetValue;
            double apportionedAmountRaw;
            double.TryParse(apportionment.ClaimApprotionmentValue, out apportionedAmountRaw);
            this.ApportionmentType = apportionment.ClaimApprotionmentType;
            this.ClaimApportionment = ApportionmentType == "P" ? (apportionedAmountRaw * 100).ToString() + "%" : apportionment.ClaimApprotionmentValue;
            
        }

        #endregion

        #region events

        public event EventHandler<SelectedClaimArgs> SelectedClaimApportionmentChanged;

        #endregion

        #region protected methods

        protected virtual void OnSelectedClaimApportionmentChanged(SelectedClaimArgs e)
        {
            EventHandler<SelectedClaimArgs> handler = SelectedClaimApportionmentChanged;
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
                if (isValid == false)
                {
                    if (!App.Configuration.CanEnterNegativeClaimsAppotionment || rawValue.Contains("%"))
                    {
                        MessageBoxShow("Please specify value greater than or equal to 0.");
                    }
                    else
                    {
                        isValid = true;
                    }
                }
            }
            else
            {
                MessageBoxShow("Please specify valid value.");
            }

            return isValid;
        }


        private void ReturnApportionmentValueWithFormat()
        {
            if (IsPercentage)
            {

                //ClaimApportionment.Replace("%", "");
                ApportionmentType = "P";
            }
            else
            {
                ApportionmentType = "V";
            }
           
        }

        private double CalculateClaimApportionmentValue()
        {
            double claimApportionment  = 0;
            if(IsPercentage)
            {
                if (ClaimApportionment.Contains("%"))
                {
                    double.TryParse(ClaimApportionment.Replace("%", string.Empty), out claimApportionment);
                }

                claimApportionment = claimApportionment / 100;

            }
            else
            {
                double.TryParse(ClaimApportionment, out claimApportionment);
            }

            return claimApportionment;
        }

        private double CalculateApportionedAmount()
        {
            double apportionmentAmount;
            double claimApportionment = CalculateClaimApportionmentValue();
            if(IsPercentage)
            {
                double claimNetValue;
                double.TryParse(NetValue, out claimNetValue);
              
                apportionmentAmount = claimNetValue * claimApportionment;
            }
            else
            {
                apportionmentAmount = claimApportionment;
            }
            ReturnApportionmentValueWithFormat();
            return apportionmentAmount;
        }

        #endregion
    }

    public class SelectedClaimArgs : EventArgs
    {
        public string ClaimLineDetail { get; set; }

        public string ClaimLineID { get; set; }

        public string ApportionedAmount { get; set; }
    }
}
