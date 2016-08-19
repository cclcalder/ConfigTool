using System;
using System.Xml.Linq;
using Exceedra.Common;

namespace Model.Entity
{
    [Serializable()]
    public class PromotionChart
    {
        public string Idx { get; set; }
        public string Name { get; set; }
        public bool IsDefault { get; set; }

        public PromotionChart(XElement e)
        {
            Idx = e.Element("Promotion_Graph_Idx").MaybeValue();
            Name = e.Element("Promotion_Graph_Name").MaybeValue();
            IsDefault = e.Element("IsDefault").MaybeValue() == "1";
        }
    }
}