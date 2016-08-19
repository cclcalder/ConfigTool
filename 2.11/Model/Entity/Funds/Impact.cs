using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Model.Entity.Funds
{
    public class Impact
    {
        public Impact()
        {
            Options = new List<ImpactOption>();
        }
        public string ID { get; set; }
        public string Name { get; set; }
        public string Format { get; set; }
        public IList<ImpactOption> Options { get; private set; }

        public static Impact FromXml(XElement xml)
        {
            return new Impact
            {
                ID = xml.Element("ID").MaybeValue(),
                Name = xml.Element("Name").MaybeValue(),
                Format = xml.Element("Format").MaybeValue(),
                Options = xml.Element("Options").MaybeElements("Option").Select(ImpactOption.FromXml).ToList()
            };
        }
    }
}