using System.Xml.Linq;

namespace Model
{
    public class ConditionStatus
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public virtual bool IsSelected { get; set; }
        public virtual bool IsEnabled { get; set; }
         public string Colour { get; set; }

        public static ConditionStatus FromXml(XElement element)
        {
            const string idElement = "ID";
            const string nameElement = "Name";
            const string isSelectedElement = "IsSelected";
            const string isEnabled = "IsEnabled";


            var c = new ConditionStatus
            {
                Id = element.GetValue<string>(idElement),
                Name = element.GetValue<string>(nameElement),
                IsSelected = element.GetValue<int>(isSelectedElement) == 1,
                IsEnabled = element.GetValue<int>(isEnabled) == 1,
                Colour = element.GetValue<string>("Colour")
            };

            return c;
        }
    }



}



