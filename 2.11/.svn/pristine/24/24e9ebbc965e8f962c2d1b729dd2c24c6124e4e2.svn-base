using Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewModels;

namespace WPF.ViewModels.Claims
{
    public class ClaimEditorProductGridItemViewModel : ViewModelBase
    {
        #region private fields

        private string _eventId;
        private double _eventApportionedAmount;
        private string _apportionmentType;
        #endregion

        #region properties

        public string Code { get; set; }
        public string MaterialName { get; set; }
        private string _materialApportionment;

        public string ProductId { get; set; }

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
                    ApportionmentType = IsPercentage ? "P" : "V";                  
                }
                NotifyPropertyChanged(this, vm => vm.MaterialApportionment, vm => vm.ApportionedAmount, vm => vm.IsPercentage);
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

        #endregion

        #region ctors

        public ClaimEditorProductGridItemViewModel(EventProduct eventProduct, double eventApportionedAmount, string eventId)
        {
            _eventId = eventId;
            _eventApportionedAmount = eventApportionedAmount;
            this.Code = eventProduct.Code;
            this.MaterialName = eventProduct.Name;
            this.ProductId = eventProduct.Id;
            double apportionedAmountRaw;
            double.TryParse(eventProduct.ApportionmentValue, out apportionedAmountRaw);
            this.ApportionmentType = eventProduct.ApportionmentType;
            this.MaterialApportionment = this.ApportionmentType == "P" ? (apportionedAmountRaw * 100).ToString() + "%" : eventProduct.ApportionmentValue;
        }

        #endregion

        #region public methods

        public void UpdateEventApportionedAmount(string apportionedAmount)
        {
            double.TryParse(apportionedAmount, out _eventApportionedAmount);
            NotifyPropertyChanged(this, vm => vm.ApportionedAmount);
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
            double materialApportionmentValue, materialApportionment;
            if (this.IsPercentage)
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
            double materialApportionment, apportionmentAmount;
            materialApportionment = CalculateMaterialApportionmentValue();
            if (this.IsPercentage)
            {
                apportionmentAmount = _eventApportionedAmount * materialApportionment;

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
