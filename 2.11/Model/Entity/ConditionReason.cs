using System.Xml.Linq;

namespace Model
{
    public class ConditionReason
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }

        public static ConditionReason FromXml(XElement element)
        {
            const string idElement = "ConditionReason";
            const string nameElement = "ConditionName";
            const string isSelectedElement = "IsSelected";

            return new ConditionReason 
            {
                Id = element.GetValue<string>(idElement),
                Name = element.GetValue<string>(nameElement),
                IsSelected = element.GetValue<string>(isSelectedElement) == "1"
            };
        }
    }
}