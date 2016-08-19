using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Exceedra.Common.Mvvm;
using Exceedra.Common.Utilities;
using Model.Utilities;

namespace Model
{
    [Serializable()]
    public class PromotionExtraData
    {
        public PromotionExtraData()
        {

        }
        public PromotionExtraData(XElement el)
        {
            try
            {
                Name = el.GetValue<string>("Name");
                _value = el.GetValue<string>("Value");
                Format = el.GetValue<string>("Format");
                Sort = SortValue;
                // formating the value
                _value = this.InvariantFormatValue(el.GetValue<string>("Value"));
            }
            catch (Exception ex)
            {
                var r = ex;

            }
            
        }

        private string FormatValue(string value)
        {
            if (!string.IsNullOrWhiteSpace(Format))
            {
                decimal d;
                if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out d))
                {
                    if (Format.StartsWith("P"))
                    {
                        return (d / 100).ToString(Format);
                    }
                    else if (Format.ToLower().Contains("yyyy") || Format.ToLower().Contains("date"))
                    {
                     //its a date, convert it to user date format and output as string
                     return DateTimeHelper.ConvertDateTimeToDate(value);
                    }
                    else
                    {
                        return d.ToString(Format);
                    }
                }
                DateTime date;
                if (DateTime.TryParse(value, out date))
                {
                    //return date.ToString(Format);
                    //ignore the format and deliver the localised formatted date
                    return DateTimeHelper.ConvertDateTimeToDate(value);
                }
            }

            return value;
        }

        private string FormatValue(decimal value)
        {
            var result = (value).ToString(Format, CultureInfo.CurrentCulture.NumberFormat);
            return result;
        } 

        private string InvariantFormatValue(string value)
        {

            if (value == null)
            {
                //Trace.TraceError(ex.Message);
               
               Messages.Instance.PutError(string.Format("An error was found in your XML formatting, the promotion attributed with '{0}'  may load incorrectly",this.Name));
 
                return null;
            }

            decimal d;
            DateTime dateValue = DateTime.MinValue;
            string returnValue = value;
            if (decimal.TryParse(value.Replace("%", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out d))
            {
                returnValue = FormatValue(d);
            }
            else if (DateTime.TryParse(value,out dateValue))
            {
                returnValue = FormatValue(dateValue.ToString());
            }
            return returnValue;
        }

        public IComparable Sort { get; set; }

        #region Name
        /// <summary>
        /// Gets or sets the Text of this PromotionExtraData.
        /// </summary>
        public string Name { get; set; }
        #endregion

        #region Value

        private string _value;

        /// <summary>
        /// Gets or sets the Value of this PromotionExtraData.
        /// </summary>
        public string Value
        {
            get { return _value; }
            set { _value = FormatValue(value); }
        }

        #endregion

        public IComparable SortValue
        {
            get
            {
                decimal d;
                if (decimal.TryParse(Value, out d))
                {
                    return d;
                }
                DateTime date;
                if (DateTime.TryParse(Value, out date))
                {
                    return date.ToString("yyyyMMddHHmmss");
                }
                if (Value == "Not Set") return null;
                return Value;

            }
        }

        #region Format
        /// <summary>
        /// Gets or sets the Format of this PromotionExtraData.
        /// </summary>
        public string Format { get; set; }
        #endregion

    }
}
