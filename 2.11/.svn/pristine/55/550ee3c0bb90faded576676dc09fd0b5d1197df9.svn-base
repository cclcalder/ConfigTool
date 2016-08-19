using System.Xml.Linq;

namespace Model.Entity
{
    public class ScenarioCustomer
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public static ScenarioCustomer FromXml(XElement element)
        {
            const string idElement = "ID";
            const string nameElement = "Name";

            return new ScenarioCustomer
            {
                Id = element.GetValue<string>(idElement),
                Name = element.GetValue<string>(nameElement)
            };
        }
    }
}