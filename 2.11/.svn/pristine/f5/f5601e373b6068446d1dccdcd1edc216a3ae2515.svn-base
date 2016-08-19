using System.Xml.Linq;

namespace Model.Entity.Funds
{
    public class ImpactOption
    {
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
                ID = xml.Element("ID").MaybeValue(),
                Name = xml.Element("Name").MaybeValue(),
                Format = xml.Element("Format").MaybeValue(),
                IsSelected = xml.Element("IsSelected").MaybeValue() == "1"
            };
        } 
    }
}