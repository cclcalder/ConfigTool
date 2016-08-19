using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model
{
    [Serializable()]
    public class PromotionFinancial
    {
        public PromotionFinancial() { }
        public PromotionFinancial(XElement el)
            : this(el.GetValue<string>("ID"), el.GetValue<string>("Name"), el.GetValue<string>("Format"), el.GetValue<string>("Value"))
        {

        }

        public PromotionFinancial(string id, string name, string format, string value)
        {
            ID = id;
            Name = name;
            Format = format;
            Value = this.InvariantFormatValue(value);
        }

        #region Name
        /// <summary>
        /// Gets or sets the Name of this PromotionFinancial.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = UpdateCurrencyCulture(value);
            }
        }

        private string UpdateCurrencyCulture(string value)
        {
            return value.Replace("£", System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol);
        }

        #endregion

        #region ID
        /// <summary>
        /// Gets or sets the Id of this PromotionFinancial.
        /// </summary>
        public string ID { get; set; }
        #endregion

        #region Value

        private string _value;
        private string _name;

        /// <summary>
        /// Gets or sets the Value of this PromotionFinancial.
        /// </summary>
        public string Value
        {
            get { return _value; }
            set
            {
                _value = FormatValue(value);
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
            var result = (Format.StartsWith("P", StringComparison.CurrentCulture) ? (value / 100) : value).ToString(Format);
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
        #endregion

        #region Format
        /// <summary>
        /// Gets or sets the Format of this PromotionFinancial.
        /// </summary>
        public string Format { get; set; }
        #endregion
    }
}
