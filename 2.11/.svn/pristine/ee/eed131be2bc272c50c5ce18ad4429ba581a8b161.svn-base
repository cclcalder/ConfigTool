using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model.Entity.Funds
{
    public class FundAttribute
    {
        public enum ValueType
        {
            String,
            Number,
            Date
        };

        public string Name { get; set; }
        public object Value { get; set; }
        public string Format { get; set; }
        public ValueType Type { get; set; }

        public static FundAttribute FromXml(XElement xml)
        {
            var format = xml.Element("Format").MaybeValue();
            var valueString = xml.Element("Value").MaybeValue();
            ValueType type;
            object value = null;
            if (string.IsNullOrWhiteSpace(format))
            {
                value = valueString;
                type = ValueType.String;
            }
            else if (FormatHelper.IsNumberFormat(format))
            {
                type = ValueType.Number;
                double number;
                if (double.TryParse(valueString, NumberStyles.Any, CultureInfo.InvariantCulture, out number))
                {
                    value = (double?)number;
                }
            }
            else
            {
                type = ValueType.Date;
                DateTime date;
                if (DateTime.TryParse(valueString, out date))
                {
                    value = (DateTime?)date;
                }
            }
            return new FundAttribute
            {
                Name = xml.Element("Name").MaybeValue(),
                Value = value,
                Format = format,
                Type = type
            };
        }
    }
}
