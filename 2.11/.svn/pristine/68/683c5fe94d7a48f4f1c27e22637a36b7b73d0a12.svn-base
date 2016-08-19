using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model.Entity
{
    public class EventItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string EventType { get; set; }
        public string EventSubType { get; set; }
        public string EventStatus { get; set; }
        public string Author { get; set; }
        public string EventApprover { get; set; }
        public string TotalAccrual { get; set; }
        public string Settled { get; set; }
        public string AvailableAccrual { get; set; }

        public static EventItem FromXml(XElement element)
        {
            const string idElement = "Event_Idx";
            const string eventNameElement = "Event_Name";
            const string eventTypeElement = "Event_Type";
            const string eventSubTypeElement = "Event_Sub_Type";
            const string eventStatusElement = "Event_Status";
            const string eventAuthorElement = "Event_Author";
            const string eventApproverElement = "Event_Approver";
            const string totalAccrualElement = "Total_Accrual";
            const string settledElement = "Settled";
            const string availableAccrualElement = "Available_Accrual";

            return new EventItem
            {
                Id = element.GetValue<string>(idElement),
                Name = element.GetValue<string>(eventNameElement),
                EventType = element.GetValue<string>(eventTypeElement),
                EventSubType = element.GetValue<string>(eventSubTypeElement),
                EventStatus = element.Element("Status").GetValue<string>(eventStatusElement),
                Author = element.GetValue<string>(eventAuthorElement),
                EventApprover = element.GetValue<string>(eventApproverElement),
                TotalAccrual = element.GetValue<string>(totalAccrualElement),
                Settled = element.GetValue<string>(settledElement),
                AvailableAccrual = element.GetValue<string>(availableAccrualElement)
            };
        }
    }
}
