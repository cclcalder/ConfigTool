using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Exceedra.Common;

namespace Model
{

    //<Measure>
    //   <ID>5</ID>
    //   <Name>Base Sales In  (cases)</Name>
    //   <Format>N0</Format>
    //   <Value>307.0000</Value>
    //   <IsEditable>0</IsEditable>
    //   <IsDisplayed>0</IsDisplayed>
    //   <IncludedInRowTotal>1</IncludedInRowTotal>
    //   <HasColumnTotal>SUM</HasColumnTotal>
    // </Measure>

    [Serializable()]
    public class PromotionMeasure : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        public PromotionMeasure()
        {

        }
        public PromotionMeasure(XElement el)
        {
            ID = el.GetValueOrDefault<string>("ID");
            Name = el.GetValue<string>("Name");
            Format = el.GetValue<string>("Format");
            Value = this.InvariantFormatValue(el.GetValue<string>("Value"));


            IsReadOnly = el.GetValueOrDefault<string>("IsEditable") == "0";

            IsRowCalculated = false;
            if (el.Element("IncludedInRowTotal") != null)
            {
                IsRowCalculated = el.GetValue<int>("IncludedInRowTotal") == 1 ? true : false;
            }
            if (el.Element("HasColumnTotal") != null)
            {
                HasTotal = el.GetValueOrDefault<string>("HasColumnTotal").ToLower();
            }

            IsDisplayed = true;
            if (el.Element("IsDisplayed") != null && el.Element("IsDisplayed").Value == "0")
                IsDisplayed = false;
        }

        public string HasTotal { get; set; }

        /// <summary>
        /// Gets or sets the Id of this PromotionMeasure.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the Name of this PromotionMeasure.
        /// </summary>
        public string Name { get; set; }

        private string _value;

        /// <summary>
        /// Gets or sets the Value of this PromotionMeasure.
        /// </summary>
        public string Value
        {
            get { return _value; }
            set
            {
                _value = FormatValue(value);
                this.OnPropertyChanged("Value");
            }
        }

        private string FormatValue(string value)
        {
            decimal d;
            string returnValue = value;
            if (decimal.TryParse(value.Replace("%", ""), NumberStyles.Any, CultureInfo.CurrentCulture, out d))
            {
                returnValue = FormatValue(d);
            }
  
            return returnValue;
        }

        private string FormatValue(decimal value)
        {
            var result = (value).ToString(Format, CultureInfo.CurrentCulture.NumberFormat);
            return result;
        }

        private string InvariantFormatValue(string value)
        {
            decimal d;
            string returnValue = value;
            if (decimal.TryParse(value.Replace("%", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out d))
            {
                returnValue = FormatValue(d);
            }
            return returnValue;
        }
        /// <summary>
        /// Gets or sets the Format of this PromotionMeasure.
        /// </summary>
        public string Format { get; set; }

        public bool IsReadOnly { get; set; }

        public bool IsRowCalculated { get; set; }
        
        public bool IsDisplayed { get; set; }
    }
}