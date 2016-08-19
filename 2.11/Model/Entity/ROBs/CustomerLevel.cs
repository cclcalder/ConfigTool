using System;
using System.Globalization;
using System.Xml.Linq;
using Exceedra.Common;

namespace Model.Entity.ROBs
{
    public class CustomerLevel : IEquatable<CustomerLevel>
    {
        public bool Equals(CustomerLevel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(ID, other.ID);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CustomerLevel) obj);
        }

        public override int GetHashCode()
        {
            return (ID != null ? ID.GetHashCode() : 0);
        }

        public string ID { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }

        public static CustomerLevel FromXml(XElement xml)
        {
            return new CustomerLevel
                {
                    ID = xml.Element("Idx").MaybeValue(),
                    Name = xml.Element("Name").MaybeValue(),
                    IsSelected = xml.Element("IsSelected").MaybeValue() == "1",
                };
        }
    }

    public class RobStatus
    {
        public string Name { get; set; }
        public string Color { get; set; }
        public string ID { get; set; }

        public static RobStatus FromXml(XElement xml)
        {
            if (xml == null) return null;
            return new RobStatus
            {
                ID = xml.Element("ID").MaybeValue(),
                Name = xml.Element("Name").MaybeValue(),
                Color = xml.Element("Color").MaybeValue(),
            };
        }
    }

    public class RobAttribute
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

        public static RobAttribute FromXml(XElement xml)
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
                    value = (DateTime?) date;
                }
            }
            return new RobAttribute
            {
                Name = xml.Element("Name").MaybeValue(),
                Value = value,
                Format = format,
                Type = type
            };
        }
    }
}