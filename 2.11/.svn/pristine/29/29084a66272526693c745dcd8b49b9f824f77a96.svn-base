using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model.Entity
{
    public class Event
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string EventType { get; set; }
        public string EventSubType { get; set; }
        public string EventStatus { get; set; }
        public DateTime StartDate { get; set; }
        public string StartDateDisplay { get { return StartDate.ToString("dd/MM/yyyy"); } }
        public DateTime EndDate { get; set; }
        public string EndDateDisplay { get { return EndDate.ToString("dd/MM/yyyy"); } }
        public bool IsSelected { get; set; }
        public static Event FromXml(XElement element)
        {
            const string dateFormat = "yyyy-MM-dd";
            const string idElement = "Event_Idx";
            const string nameElement = "Event_Name";
            const string typeElement = "Event_Type";
            const string subtypeElement = "Event_Sub_Type";
            const string statusElement = "Event_Status";
            const string startDateElement = "Event_Start_Date";
            const string endDateElement = "Event_End_Date";
            return new Event
            {
                Id = element.GetValue<string>(idElement),
                Name = element.GetValue<string>(nameElement),
                EventType = element.GetValue<string>(typeElement),
                EventSubType = element.GetValue<string>(subtypeElement),
                EventStatus = element.GetValue<string>(statusElement),
                StartDate = DateTime.ParseExact(element.GetValue<string>(startDateElement), dateFormat, null),
                EndDate = DateTime.ParseExact(element.GetValue<string>(endDateElement), dateFormat, null)
            };
        }
    }
}
