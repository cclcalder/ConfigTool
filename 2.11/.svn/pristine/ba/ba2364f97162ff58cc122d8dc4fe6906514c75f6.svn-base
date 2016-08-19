using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model.Entity
{
    public class NpdProductAttribute
    {
        public static NpdProductAttribute FromXml(XElement node)
        {
            const string idElement = "ID";
            const string nameElement = "Name";
            const string valueElement = "Value";
            const string typeElement = "Type";
            const string formatElement = "Format";
            return new NpdProductAttribute
            {
                ID = node.GetValue<string>(idElement),
                Name = node.GetValue<string>(nameElement),
                Type = node.GetValue<string>(typeElement),
                Format = node.GetValue<string>(formatElement),
                Value = node.GetValue<string>(valueElement)
            };
        }

        public string ID { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string Format { get; set; }

        private string _value;
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
            var result = value.ToString(Format, CultureInfo.CurrentCulture.NumberFormat);
            return result;
        }
    }
}
