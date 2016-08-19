using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Exceedra.Common;

namespace Model.Entity.GroupEditor
{
    public class GroupBase
    {
       // private string p1;

        public GroupBase() { }

        public GroupBase(string p1, string p2, bool p3)
        { 
            this.ID = p1;
            this.Name = p2;
            this.IsAmendable = p3;
        }

        public string ID { get; set; } 
        public string Name { get; set; } // ROBGroup_Name
        public bool IsAmendable { get; set; } //IsAmendable   



        public static GroupBase FromXml(XElement xml)
        {
            return new GroupBase(xml.Element("ID").MaybeValue(), 
                xml.Element("ROBGroup_Name").MaybeValue(), 
                xml.Element("IsAmendable").MaybeValue() == "1");
        }
    }

 
}
