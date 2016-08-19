using Exceedra.Common;

namespace Model.Entity.ROBs
{
    using System;
    using System.Xml.Linq;

    public class RobProduct : IEquatable<RobProduct>
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
        public DateTime? DelistingsDate { get; set; }

        public bool Equals(RobProduct other)
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
            return Equals((RobProduct) obj);
        }

        public override int GetHashCode()
        {
            return (ID != null ? ID.GetHashCode() : 0);
        }

        public static RobProduct FromXml(XElement xml)
        {
            return new RobProduct
                       {
                           ID = xml.Element("Idx").MaybeValue(),
                           Name = xml.Element("Name").MaybeValue(),
                           IsSelected = xml.Element("IsSelected").MaybeValue() == "1",
                           DelistingsDate = GetDelisingsDate(xml)
                       };
        }

        private static DateTime? GetDelisingsDate(XElement element)
        {
            var date = element.Element("Date_End").MaybeValue();

            DateTime? delistingsDate = null;

            if (date != null)
            {
                delistingsDate = DateTime.Parse(date);
            }

            return delistingsDate;
        }
    }
}