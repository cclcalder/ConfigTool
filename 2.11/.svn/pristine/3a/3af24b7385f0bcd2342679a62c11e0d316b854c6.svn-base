using System;
using System.Xml.Linq;
using Exceedra.Common;

namespace Model.Entity.NPD
{
    public class NPDStatus
    {
        public string Name { get; set; }
        public string Idx { get; set; }
        public string Colour { get; set; }
        public string SortIdx { get; set; }
        public bool IsSelected { get; set; }
        public bool IsEnabled { get; set; }

        public NPDStatus(XElement xml)
        {
            Name = xml.Element("Name").MaybeValue();
            Idx = xml.Element("ID").MaybeValue();
            Colour = xml.Element("Colour").MaybeValue();
            SortIdx = xml.Element("Sort").MaybeValue();
            IsSelected = xml.Element("IsSelected").MaybeValue() == "1";
            IsEnabled = xml.Element("IsEnabled").MaybeValue() == "1";
        }

        public NPDStatus()
        {
        }
    }
}