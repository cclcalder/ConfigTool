using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using Exceedra.Common.Utilities;
using Exceedra.DynamicGrid.Models.Validation;
using Model.Annotations;

namespace Exceedra.DynamicGrid.Models
{
    [Serializable]
    public abstract class PropertyBase : INotifyPropertyChanged
    {

        #region fields

        private string _dataSourceInput;
        private string _errorMessage;
        private List<ValidationRule> _validations;
        private string _value;

        #endregion

        #region properties

        public string BackgroundColour { get; set; }

        private string _borderColour = "transparent";
        public string BorderColour
        {
            get { return _borderColour; }
            set
            {
                _borderColour = value;
                PropertyChanged.Raise(this, "BorderColour");
            }
        }

        public string Calculation { get; set; }

        public string ColumnCode { get; set; }

        public string ControlType { get; set; }

        public string DataSource { get; set; }

        public string DataSourceInput
        {
            get
            {
                return _dataSourceInput;
            }
            set
            {
                if (String.IsNullOrEmpty(value)) _dataSourceInput = null;
                else
                {
                    // this try catch is here because in some parts of the app
                    // we instantiate records using serializer which gets the value
                    // of the DataSourceInput property without embedding it into the <DataSourceInput> node
                    // so we have to check if it's embedded and if not we have to embed it manually
                    try
                    {
                        XElement xDataSourceInput = XElement.Parse(value);
                        _dataSourceInput = xDataSourceInput.ToString();
                    }
                    catch (Exception)
                    {
                        _dataSourceInput = new XElement("DataSourceInput", value).ToString();
                    }

                }

                PropertyChanged.Raise(this, "DataSourceInput");

                //load the options when the datasourceinput is changed, we can now set this from the calling control
                //if (!string.IsNullOrWhiteSpace(_dataSourceInput))
                //{
                //    try
                //    {
                //        this.Values = Option.GetFromXML(_dataSourceInput, this.DataSource);
                //        if (!string.IsNullOrWhiteSpace(this.Value))
                //        {
                //            this.SelectedItem = Values.Where(r => r.Item_Idx == this.Value).FirstOrDefault();
                //        }
                //    }

                //    catch { }
                //}
            }
        }

        /// <summary>
        /// Generated based on the Validations collection.
        /// Empty (or null) if the proprerty is valid.
        /// </summary>
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                PropertyChanged.Raise(this, "ErrorMessage");
                PropertyChanged.Raise(this, "IsValid");
            }
        }

        private string _forecolour = "#000000";
        public string ForeColour
        {
            get
            {
                return _forecolour;
            }
            set
            {
                _forecolour = value;
                PropertyChanged.Raise(this, "ForeColour");
            }
        }

        public virtual bool HasSelectedItems
        {
            get
            {
                return SelectedItem != null ||
                       (SelectedItems != null && SelectedItems.Any());
            }
        }

        public string HeaderText { get; set; }

        public bool IsDisplayed { get; set; }

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
                PropertyChanged.Raise(this, "IsEditable");
            }
        }

        private bool _isExpandable;
        public bool IsExpandable
        {
            get
            {
                return _isExpandable;
            }
            set
            {
                _isExpandable = value;
                PropertyChanged.Raise(this, "IsExpandable");
            }
        }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }
            set
            {
                _isExpanded = value;
                PropertyChanged.Raise(this, "IsExpanded");
            }
        }

        public bool IsLoaded { get; set; }

        public virtual bool IsMultiSelectable
        {
            get { return ControlType.ToLower() == "multidropdown"; }
        }

        public bool IsRequired { get; set; }

        /// <summary>
        /// True if there is no error message for the property.
        /// Otherwise, false.
        /// </summary>
        public bool IsValid
        {
            get { return string.IsNullOrEmpty(ErrorMessage); }
        }

        public bool IsChecked
        {
            get
            {

                if (ControlType != null && ControlType.ToLower() == "checkbox")
                {
                    return Value == "1";
                }
                return false;
            }
            set
            {
                if (ControlType.ToLower() == "checkbox")
                {
                    Value = (value ? "1" : "0");
                }
            }
        }

        protected bool Loaded { get; set; }

        public abstract Option SelectedItem { get; set; }

        public abstract ObservableCollection<Option> SelectedItems { get; set; }

        [XmlElement("Format")]
        public string StringFormat { get; set; }

        public string TotalsAggregationMethod { get; set; }

        public string UpdateToColumn { get; set; }

        /// <summary>
        /// A collection of ValidationRules used to validate the Value of this property against other properties' Values in the same RowViewModel.
        /// </summary>
        public List<ValidationRule> Validations
        {
            get
            {
                if (_validations == null) _validations = new List<ValidationRule>();
                return _validations;
            }
            set { _validations = value; }
        }

        public abstract string Value { get; set; }

        /// <summary>
        /// Used when sorting rows by columns - we couldn't just use Value because it's of type string so everything would be sorted alphabetically.
        /// Here, we return a value of the type that suites what the Value property contains.
        /// </summary>
        public object ValueToSort
        {
            get
            {
                string valueToSort = Value;

                // Sorting uses the DataGridTemplateColumn sorting logic which breaks when trying to sort data of different type (i.e. string and datetime).
                // <Not Set> could mean that a string is not set, or date is not set, etc.
                // Therefore, in order not to break the sorting logic, we must handle this case and return NULL (which is fine for both string and datetime in our example).
                if (valueToSort == "<Not Set>")
                    return null;

                if (!StringFormat.IsEmpty())
                {
                    //okay its a number of some kind
                    if (StringFormat.ToLower().StartsWith("c")
                        || StringFormat.ToLower().StartsWith("p")
                        || StringFormat.ToLower().StartsWith("n"))
                    {
                        // removing currency and percent symbols
                        valueToSort = valueToSort.Replace(CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol, "");
                        valueToSort = valueToSort.Replace(CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");

                        //its a negative number
                        if (valueToSort.StartsWith("(") && valueToSort.EndsWith(")"))
                        {
                            valueToSort = valueToSort.Replace("(", "");
                            valueToSort = valueToSort.Replace(")", "");
                            valueToSort = "-" + valueToSort;
                        }

                        // is it really a number?
                        double valueAsDouble;
                        bool isDouble = double.TryParse(valueToSort, out valueAsDouble);

                        if (isDouble)
                            return valueAsDouble;
                    }


                    // maybe it's a date?
                    DateTime valueAsDate;
                    bool isDate = DateTime.TryParse(valueToSort, out valueAsDate);

                    if (isDate)
                        return valueAsDate;

                }

                // otherwise it's a string
                return valueToSort.Trim();
            }
        }

        [XmlArrayItem("Value")]
        public abstract ObservableCollection<Option> Values { get; set; }

        public string FitWidth { get; set; }

        public string Alignment { get; set; }

        public string Width { get; set; }

        public string Date { get; set; }

        #endregion

        #region public methods

        public bool CheckValidEntry(string value)
        {
            value = value.Replace(CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");

            decimal d;
            bool isNum = decimal.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out d);
            bool formatIsNum = StringFormat.StartsWith("C") || StringFormat.StartsWith("P") || StringFormat.StartsWith("N");

            if (!isNum && formatIsNum)
                ForeColour = "#FF0000";
            else
                ForeColour = "#000000";

            return isNum;
        }

        public string FormatValue(string value)
        {
            if (value == null) return null;

            if (!string.IsNullOrWhiteSpace(StringFormat))
            {   // allow calculations to bypass formatting
                if (value.StartsWith("="))
                    return value;

                decimal d;

                if (Loaded)
                {
                    value = value.Replace(CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");

                    DateTime date;
                    var isDate = DateTime.TryParseExact(value,
                    // allowed formats
                    new[]
                    {
                         "yyyy-MM-dd",
                         CultureInfo.CurrentCulture.DateTimeFormat.LongDatePattern, CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern,
                         CultureInfo.CurrentCulture.DateTimeFormat.FullDateTimePattern, CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern, CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern
                    },
                    CultureInfo.CurrentCulture, DateTimeStyles.None, out date);

                    bool isNum = decimal.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out d);

                    bool formatIsNum = StringFormat.StartsWith("C") || StringFormat.StartsWith("P") ||
                                       StringFormat.StartsWith("N");

                    if (!isNum && formatIsNum)
                    {
                        ForeColour = "#FF0000";
                        ErrorMessage = "Value is not numeric";
                    }
                    else
                    {
                        ForeColour = "#000000";
                        ErrorMessage = "";
                    }

                    // JS: In some cultures (i.e. de-DE) a date can also be interpreted as a number so I have added an additional check+
                    // LB : Currencies are numbers too. 
                    if ((isDate || !isNum) && !StringFormat.ToLower().Contains("c"))
                    {
                        return value;
                    }

                }
                else
                {
                    value = value.Replace(CultureInfo.GetCultureInfoByIetfLanguageTag("en-GB").NumberFormat.PercentSymbol, "");
                    bool isNum = decimal.TryParse(value, NumberStyles.Any, CultureInfo.GetCultureInfoByIetfLanguageTag("en-GB"), out d);
                    Loaded = true;

                    if (isNum == false)
                    {

                        DateTime date;
                        var isDate = DateTime.TryParse(value, out date);

                        if (isDate && (StringFormat == "LongDate" || StringFormat == "ShortDate"))
                        {

                            if (StringFormat == "LongDate")
                            {
                                return date.ToLongDateString();
                            }
                            if (StringFormat == "ShortDate")
                            {
                                return date.ToShortDateString();
                            }

                            return date.ToString(StringFormat);

                        }
                        else
                        {
                            return value;
                        }

                    }
                    //I am still number

                }

                if (StringFormat.StartsWith("P"))
                {
                    return (d / 100).ToString(StringFormat);
                }
                else
                {
                    return d.ToString(StringFormat, CultureInfo.CurrentCulture);
                }

            }

            return value;
        }

        public abstract bool HasValue();

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
