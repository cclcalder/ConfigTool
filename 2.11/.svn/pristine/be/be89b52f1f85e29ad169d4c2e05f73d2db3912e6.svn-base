using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model.Entity
{
    public class EventStatus
    {
        public static readonly string SelectAllId = "[ALL]";

        public string Id { get; set; }
        public string Name { get; set; }
        public virtual bool IsSelected { get; set; }

        public virtual void SetSelected(bool isSelected)
        {

        }

        public static EventStatus FromXml(XElement element)
        {
            const string idElement = "Event_Status_Idx";
            const string nameElement = "Event_Status_Name";
            const string isSelectedElement = "IsSelected";

            return new EventStatus
            {
                Id = element.GetValue<string>(idElement),
                Name = element.GetValue<string>(nameElement),
                IsSelected = element.GetValue<int>(isSelectedElement) == 1
            };
        }
    }
}
