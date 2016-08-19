using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using Exceedra.Common;

namespace Model.Entity.ROBs
{
    using System;

    public class Impact
    {
        public Impact()
        {
            Options = new List<ImpactOption>();
        }
        public string ID { get; set; }
        public string Name { get; set; }
        public string Format { get; set; }
        public string Amount { get; set; }
        public IList<ImpactOption> Options { get; private set; }
     
        public static Impact FromXml(XElement xml)
        {
            return new Impact
                {
                    ID = xml.Element("Idx").MaybeValue(),
                    Name = xml.Element("Name").MaybeValue(),
                    Format = xml.Element("Format").MaybeValue(),
                    Options = xml.Element("Options").MaybeElements("Option").Select(ImpactOption.FromXml).ToList(),
                    Amount = xml.Element("Value").MaybeValue()
                };
        }
    }

    public class ImpactOption
    {

        public event PropertyChangedEventHandler PropertyChanged;

        public bool Equals(ImpactOption other)
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
            return Equals((ImpactOption)obj);
        }

        public override int GetHashCode()
        {
            return (ID != null ? ID.GetHashCode() : 0);
        }

        public string ID { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
        public string Format { get; set; }

        public static ImpactOption FromXml(XElement xml)
        {
            return new ImpactOption
            {
                ID = xml.Element("Idx").MaybeValue() ?? xml.Element("ID").MaybeValue(),
                Name = xml.Element("Name").MaybeValue(),
                Format = xml.Element("Format").MaybeValue(),
                IsSelected = xml.Element("IsSelected").MaybeValue() == "1"
            };
        } 
    }

    public class Option : IEquatable<Option>
    {
        public bool Equals(Option other)
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
            return Equals((Option) obj);
        }

        public override int GetHashCode()
        {
            return (ID != null ? ID.GetHashCode() : 0);
        }

        public static bool operator ==(Option left, Option right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Option left, Option right)
        {
            return !Equals(left, right);
        }

        public string ControlID { get; set; }
        public string ID { get; set; }
        public decimal Value { get; set; }
    }
}