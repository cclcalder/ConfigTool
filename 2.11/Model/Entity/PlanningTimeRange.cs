using System.Xml.Linq;
using Exceedra.Common;
using Model.Entity.Generic;

namespace Model
{
    public class PlanningTimeRange : ComboboxItem
    {
        public PlanningTimeRange(XElement xml)
        {
            Idx = xml.Element("Idx").MaybeValue();
            Name = xml.Element("Name").MaybeValue();
            IsSelected = xml.Element("IsSelected").MaybeValue() == "1";
            DateFrom = xml.Element("Start_Date").MaybeValue();
            DateTo = xml.Element("End_Date").MaybeValue();
        }

        public PlanningTimeRange()
        {
            
        }

        public string DateFrom { get; set; }
        public string DateTo { get; set; }
    }
}
