using System;
using System.Xml.Linq;

namespace Model.Entity.Funds
{
    public class ProductLevel : IEquatable<ProductLevel>
    {
        public bool Equals(ProductLevel other)
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
            return Equals((ProductLevel) obj);
        }

        public override int GetHashCode()
        {
            return (ID != null ? ID.GetHashCode() : 0);
        }

        public string ID { get; set; }
        public string Name { get; set; }

        public static ProductLevel FromXml(XElement xml)
        {
            return new ProductLevel
                {
                    ID = xml.Element("ID").MaybeValue(),
                    Name = xml.Element("Name").MaybeValue()
                };
        }
    }
}