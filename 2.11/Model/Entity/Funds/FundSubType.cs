using System;
using System.Xml.Linq;

namespace Model.Entity.Funds
{
    public class FundSubType : IEquatable<FundSubType>
    {
        public bool Equals(FundSubType other)
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
            return Equals((FundSubType) obj);
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public string ID { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }

        public static FundSubType FromXml(XElement xml)
        {
            return new FundSubType
                {
                    ID = xml.Element("ID").MaybeValue(),
                    Name = xml.Element("Name").MaybeValue(),
                    IsSelected = xml.Element("IsSelected").MaybeValue() == "1"
                };
        }
    }
}