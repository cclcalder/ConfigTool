using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Exceedra.Common;

namespace Model.Entity.ROBs
{
    public class RobRecipient
    {
        public string ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }

        public bool Equals(RobRecipient other)
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
            return Equals((RobRecipient)obj);
        }

        public override int GetHashCode()
        {
            return (ID != null ? ID.GetHashCode() : 0);
        }

        public static RobRecipient FromXml(XElement xml)
        {
            return new RobRecipient
            {
                ID = xml.Element("Idx").MaybeValue(),
                Name = xml.Element("Name").MaybeValue(),
                Code = xml.Element("Code").MaybeValue(),
                IsSelected = xml.Element("IsSelected").MaybeValue() == "1"
            };
        }
    }
}
