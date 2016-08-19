using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model.Entity
{
    public class EventType
    {
        public static readonly string SelectAllId = "[ALL]";
        public string Id { get; set; }

        public string Name { get; set; }

        public virtual bool IsSelected { get; set; }

        public virtual void SetSelected(bool isSelected)
        {

        }

        public static EventType FromXml(XElement element)
        {
            const string idElement = "Event_Type_Idx";
            const string nameElement = "Event_Type_Name";
            const string isSelectedElement = "IsSelected";

            return new EventType
            {
                Id = element.GetValue<string>(idElement),
                Name = element.GetValue<string>(nameElement),
                IsSelected = element.GetValue<int>(isSelectedElement) == 1
            };
        }
    }
}
