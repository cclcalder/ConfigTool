using System.ComponentModel;
using Model.Entity.ROBs;
using System.Linq; 

namespace ViewModels
{ 
    using System.Collections.Generic;
    using System.Globalization;

    public class ImpactViewModel : BaseViewModel
    {
        private static readonly Impact EmptyImpact = new Impact();
        private readonly Impact _impact;
        private ImpactOption _selectedOption;
        private decimal _decimalAmount;

        public ImpactViewModel(Impact impact)
        {
            _impact = impact ?? EmptyImpact;

            SelectedOption = _impact.Options.FirstOrDefault(o => o.IsSelected) ??  _impact.Options.FirstOrDefault();

            //SelectedOption.PropertyChanged += SelectedOption_ProprtyChanged;

            if (SelectedOption != null)
                Format = SelectedOption.Format;

            if (impact != null && impact.Amount != null)
            {
                /* The db saves %'s in decimal format so we need to alter it on load to look correct */
                if (impact.Format != null && impact.Format.Contains("P"))
                {
                    decimal temp;
                    decimal.TryParse(impact.Amount, NumberStyles.Any, CultureInfo.CurrentUICulture, out temp);
                    Amount = (temp*100).ToString();
                }
                else
                {
                    Amount = impact.Amount;
                }
            }            
        }

        public string ID
        {
            get { return _impact.ID; }
            set { _impact.ID = value; }
        }

        public string Name
        {
            get { return _impact.Name; }
            set { _impact.Name = value; }
        }

        public string Format
        {
            get { return _impact.Format; }
            set { _impact.Format = value; }
        }

        public bool Visible
        {
            get { return _impact.ID != null; }
        }

        public IList<ImpactOption> Options
        {
            get { return _impact.Options; }
        }

        public ImpactOption SelectedOption
        {
            get { return _selectedOption; }
            set
            {
                //_selectedOption = value;

                Set(ref _selectedOption, value, "SelectedOption");
                Amount = Amount;
            }
        }


        private void SelectedOption_ProprtyChanged(object sender, PropertyChangedEventArgs e)
        {
            _impact.Format = SelectedOption.Format;
            Amount = _decimalAmount.ToString(Format);
        }

        public void UpdateFormatAndAmountFromSelection(ImpactOption selectedImpactOption)
        {
            Format = selectedImpactOption.Format;
            Amount = Amount;
        }

        public string Amount
        {
            get
            {
                return _amount;
            }

            set
            {
                if (value == null) return;

                var tempValue = value.Replace("%", "");
                if (SelectedOption != null && SelectedOption.Format.Contains("P"))
                {
                    decimal.TryParse(tempValue, NumberStyles.Any, CultureInfo.CurrentUICulture, out _decimalAmount);
                    _decimalAmount = _decimalAmount / 100;
                }
                else
                {
                    decimal.TryParse(tempValue, NumberStyles.Any, CultureInfo.CurrentUICulture, out _decimalAmount);
                }

                if (SelectedOption != null)
                {
                    _amount = _decimalAmount.ToString(SelectedOption.Format);
                }
                else
                {
                    _amount = ((double)_decimalAmount).ToString("G");
                }

                NotifyPropertyChanged(this, vm => vm.Amount);
            }
        }

        private string _amount;

        public decimal DecimalDecimalAmount
        {
            get { return _decimalAmount; }
        }
    }
}