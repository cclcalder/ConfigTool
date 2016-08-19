using System.Xml.Linq;

namespace Model.Entity.UserSettings
{
    public class Procedure
    {
        public static Procedure FromXml(XElement xml)
        {
            Procedure procedure = new Procedure
            {
                Name = xml.Attribute("Name") != null ? xml.Attribute("Name").Value : string.Empty,
                HasClientVersion = xml.Attribute("ClientVersionExists") != null && xml.Attribute("ClientVersionExists").Value == "1",
            };

            return procedure;
        }

        public string Source { get; set; }
        public string Name { get; set; }
        public bool HasClientVersion { get; set; }
        public bool HasCorrespondentInDb { get; set; }
    }
}
