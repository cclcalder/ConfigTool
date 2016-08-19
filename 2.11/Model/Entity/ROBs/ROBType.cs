using System;
using System.Xml.Linq;
using Exceedra.Common;

namespace Model.Entity.ROBs
{
    public class ROBType : IEquatable<ROBType>
    {
        public bool Equals(ROBType other)
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
            return Equals((ROBType) obj);
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public string ID { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }

        public static ROBType FromXml(XElement xml)
        {
            return new ROBType
                {
                    ID = xml.Element("Idx").MaybeValue(),
                    Name = xml.Element("Name").MaybeValue(),
                    IsSelected = xml.Element("IsSelected").MaybeValue() == "1"
                };
        }
    }
}