using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model.Entity
{
    public class AllowedEventStatuses
    {
        public string EventStatusId { get; set; }
        public string EventStatusName { get; set; }
        public bool IsEnabled { get; set; }

        public static AllowedEventStatuses FromXml(XElement element)
        {
            const string eventStatusIdElement = "Event_Status_Idx";
            const string eventStatusNameElement = "Event_Status_Name";
            const string eventIsEnabledElement = "IsEnabled";

            return new AllowedEventStatuses
            {
                EventStatusId = element.GetValue<string>(eventStatusIdElement),
                EventStatusName = element.GetValue<string>(eventStatusNameElement),
                IsEnabled = element.GetValue<string>(eventIsEnabledElement)=="1"
            };
        }
    }
}
