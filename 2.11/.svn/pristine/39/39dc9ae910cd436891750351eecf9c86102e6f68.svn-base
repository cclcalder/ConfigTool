using Exceedra.Common;

namespace Model.Entity.ROBs
{
    using System;
    using System.Xml.Linq;

    public class RobCustomer : IEquatable<RobCustomer>
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public bool IsSelected { get; set; }

        public bool Equals(RobCustomer other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(ID, other.ID);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((RobCustomer) obj);
        }

        public override int GetHashCode()
        {
            return (ID != null ? ID.GetHashCode() : 0);
        }

        public static bool operator ==(RobCustomer left, RobCustomer right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RobCustomer left, RobCustomer right)
        {
            return !Equals(left, right);
        }

        public static RobCustomer FromXml(XElement xml)
        {
            if (xml == null) return null;
            return new RobCustomer
                       {
                           ID = xml.Element("Idx").MaybeValue(),
                           Name = xml.Element("Name").MaybeValue(),
                           Color = xml.Element("Color").MaybeValue(),
                           IsSelected = xml.Element("IsSelected").MaybeValue() == "1"
                       };
        }
    }
}