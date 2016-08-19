using Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewModels;

namespace WPF.ViewModels.Claims
{
    public class EventEditorProductGridItemViewModel : ViewModelBase
    {
        #region private fields

        private string _materialApportionment;
        private string _materialApportionmentType;
        private string _eventApportionedAmount;
        #endregion

        #region properties

        public string MaterialId { get; set; }
        public string ClaimId { get; set; }
        public string Code { get; set; }
        public string MaterialName { get; set; }
        public string ClaimLineDetail { get; set; }
        public string EventApportionedAmount 
        { 
            get 
            { 
                return _eventApportionedAmount; 
            } 
            set
            {
                _eventApportionedAmount = value;
                NotifyPropertyChanged(this, vm => vm.EventApportionedAmount, vm => vm.ApportionedAmount);
            }
        }

        public string MaterialApportionmentType
        {
            get
            {
                return _materialApportionmentType;
            }
            set
            {
                _materialApportionmentType = value;
                NotifyPropertyChanged(this, vm => vm.MaterialApportionmentType);
            }
        }

        public string MaterialApportionment
        {
            get
            {
                return _materialApportionment;
            }
            set
            {
                if (ValidateApportionment(value))
                {
                    _materialApportionment = value;
                    MaterialApportionmentType = IsPercentage ? "P" : "V";
                }
                NotifyPropertyChanged(this, vm => vm.MaterialApportionment, vm => vm.ApportionedAmount, vm => vm.IsZeroApportionedAmount);
            }
        }

        public string MaterialApportionmentValue
        {
            get
            {
                double materialApportionmentValue = CalculateMaterialApportionmentValue();
                return string.Format("{0:0.00}", materialApportionmentValue);
            }
        }

        public bool IsZeroApportionedAmount
        {
            get
            {
                double apportionedAmount;
                double.TryParse(ApportionedAmount, out apportionedAmount);
                return apportionedAmount == 0;
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

        public bool IsPercentage
        {
            get
            {
                return MaterialApportionment.Contains("%");
            }
        }

        #endregion

        #region ctors

        public EventEditorProductGridItemViewModel(EventProductDetail eventProductDetail)
        {
            this.MaterialId = eventProductDetail.Id;
            this.ClaimId = eventProductDetail.ClaimId;
            this.Code = eventProductDetail.Code;
            this.MaterialName = eventProductDetail.Name;
            this.ClaimLineDetail = eventProductDetail.ClaimLineDetail;
            this.MaterialApportionmentType = eventProductDetail.ApportionmentType;
            this.EventApportionedAmount = eventProductDetail.EventApportionedAmount;
            double apportionedAmountRaw;
            double.TryParse(eventProductDetail.ApportionmentValue, out apportionedAmountRaw);
            this.MaterialApportionment = this.MaterialApportionmentType == "P" ? (apportionedAmountRaw * 100).ToString() + "%" : eventProductDetail.ApportionmentValue;
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
                    MessageBoxShow("Please specify value greater than or equal to 0.");
                }
            }
            else
            {
                MessageBoxShow("Please specify valid value.");
            }

            return isValid;
        }

        private double CalculateMaterialApportionmentValue()
        {
            double materialApportionment, materialApportionmentValue;
            if (IsPercentage)
            {
                double.TryParse(MaterialApportionment.Replace("%", string.Empty), out materialApportionment);
                materialApportionmentValue = (materialApportionment / 100);
            }
            else
            {
                double.TryParse(MaterialApportionment, out materialApportionment);
                materialApportionmentValue = materialApportionment;
            }

            return materialApportionmentValue;
        }

        private double CalculateApportionedAmount()
        {
            double apportionmentAmount, materialApportionment;
            materialApportionment = CalculateMaterialApportionmentValue();
            if (IsPercentage)
            {
                double eventApportionedAmount;
                double.TryParse(EventApportionedAmount, out eventApportionedAmount);
                apportionmentAmount = eventApportionedAmount * materialApportionment;
            }
            else
            {
                apportionmentAmount = materialApportionment;
            }

            return apportionmentAmount;
        }

        #endregion
    }
}
