 
using System.Xml.Linq;

namespace Model.Entity.Funds
{
    public class FundRecipient
    {
        public string ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public bool Equals(FundRecipient other)
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
            return Equals((FundRecipient)obj);
        }

        public override int GetHashCode()
        {
            return (ID != null ? ID.GetHashCode() : 0);
        }

        public static FundRecipient FromXml(XElement xml)
        {
            return new FundRecipient
            {
                ID = xml.Element("ID").MaybeValue(),
                Name = xml.Element("Name").MaybeValue(),
                Code = xml.Element("Code").MaybeValue()
            };
        }
    }
}
