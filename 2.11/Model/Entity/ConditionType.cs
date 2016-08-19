using System.Xml.Linq;
using Exceedra.Common;
using Model.Entity.Generic;

namespace Model.Entity
{
    public class ConditionType : ComboboxItem
    {
        public string ConditionTypeIndicator { get; set; }
        public string ConditionTypeFormat { get; set; }


        public static ConditionType FromXml(XElement node)
        {
            const string idElement = "ConditionTypeId";
            const string nameElement = "ConditionTypeName";
            const string conditionTypeIndicator = "ConditionTypeIndicator";
            const string conditionTypeFormat = "ConditionTypeFormat";


            return new ConditionType
            {
                ConditionTypeIndicator = node.Element(conditionTypeIndicator).MaybeValue(),
                Name = node.Element(nameElement).MaybeValue() ?? node.Element("Name").MaybeValue(),
                Idx = node.Element(idElement).MaybeValue() ?? node.Element("Idx").MaybeValue(),
                IsSelected = node.Element("IsSelected").MaybeValue() == "1",
                IsEnabled = (node.Element("IsEnabled").MaybeValue() ?? "1") == "1",
                ConditionTypeFormat = node.Element(conditionTypeFormat).MaybeValue()

            };
        }
    }
}