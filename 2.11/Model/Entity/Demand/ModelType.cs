using System.Xml.Linq;
using Exceedra.Common;
using Model.Entity.Generic;

namespace Model.Entity.Demand
{
    public class ModelType : ComboboxItem
    {
        public XElement Parameters { get; set; }

        public ModelType(XElement xml)
        {
            Name = xml.Element("Name").MaybeValue();
            Idx = xml.Element("Idx").MaybeValue();
            IsSelected = xml.Element("IsSelected").MaybeValue() == "1";
            Parameters = xml.Element("Parameters");
        }
        
    }
}