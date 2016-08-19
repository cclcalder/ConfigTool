using System.Xml.Linq;
using Exceedra.Common;

namespace Model.Entity.ROBs
{
    public class Status
    {
        public bool Equals(Status other)
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
            return Equals((Status)obj);
        }

        public override int GetHashCode()
        {
            return (ID != null ? ID.GetHashCode() : 0);
        }

        public string ID { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsEnabled { get; set; }
        public virtual bool IsSelected { get; set; }

        public string Colour { get; set; }

        public static Status FromXml(XElement xml)
        {
            return new Status
            {
                ID = xml.Element("ID").MaybeValue(),
                Name = xml.Element("Name").MaybeValue(),
                IsActive = xml.Element("IsActive").MaybeValue() == "1",
                IsEnabled = xml.Element("IsEnabled").MaybeValue() == "1",
                IsSelected = xml.Element("IsSelected").MaybeValue() == "1",
                Colour = xml.Element("Colour").MaybeValue()
            };
        }
    }
}