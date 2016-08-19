using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model.Entity
{
    public class EventBase
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public static EventBase FromXml(XElement element)
        {          
            const string idElement = "Event_Idx";
            const string nameElement = "Event_Name";

            return new EventBase
            {
                Id = element.GetValue<string>(idElement),
                Name = element.GetValue<string>(nameElement)               
            };
        }
    }
}
