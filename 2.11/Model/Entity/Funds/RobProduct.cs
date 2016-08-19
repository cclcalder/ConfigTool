namespace Model.Entity.Funds
{
    using System;
    using System.Xml.Linq;

    public class FundProduct : IEquatable<FundProduct>
    {
        public string ID { get; set; }
        public string Name { get; set; }

        public bool Equals(FundProduct other)
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
            return Equals((FundProduct) obj);
        }

        public override int GetHashCode()
        {
            return (ID != null ? ID.GetHashCode() : 0);
        }

        public static FundProduct FromXml(XElement xml)
        {
            return new FundProduct
                       {
                           ID = xml.Element("ID").MaybeValue(),
                           Name = xml.Element("Name").MaybeValue()
                       };
        }
    }
}