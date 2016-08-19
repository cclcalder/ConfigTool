using System;
using System.Xml.Linq;
using Exceedra.Common;

namespace Model.Entity
{
    public class ScenarioProduct
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime? DelistingsDate { get; set; }

        public static ScenarioProduct FromXml(XElement element)
        {
            const string idElement = "ID";
            const string nameElement = "Name";

            return new ScenarioProduct
            {
                Id = element.GetValue<string>(idElement),
                Name = element.GetValue<string>(nameElement),
                DelistingsDate = GetDelisingsDate(element)
            };
        }

        private static DateTime? GetDelisingsDate(XElement element)
        {
            var date = element.Element("Date_End").MaybeValue();

            DateTime? delistingsDate = null;

            if (date != null)
            {
                delistingsDate = DateTime.Parse(date);
            }

            return delistingsDate;
        }
    }
}