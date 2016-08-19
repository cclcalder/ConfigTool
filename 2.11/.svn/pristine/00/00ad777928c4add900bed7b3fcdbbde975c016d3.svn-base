using System;
using System.Xml.Linq;

namespace Model.Entity.ROBs
{
    public class CustomerLevelItem : IEquatable<CustomerLevelItem>
    {
        public bool Equals(CustomerLevelItem other)
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
            return Equals((CustomerLevelItem)obj);
        }

        public override int GetHashCode()
        {
            return (ID != null ? ID.GetHashCode() : 0);
        }

        public string ID { get; set; }
        public string Name { get; set; }

        public static CustomerLevelItem FromXml(XElement xml)
        {
            return new CustomerLevelItem
                {
                    ID = xml.Element("ID").MaybeValue(),
                    Name = xml.Element("Name").MaybeValue()
                };
        }
    }
}