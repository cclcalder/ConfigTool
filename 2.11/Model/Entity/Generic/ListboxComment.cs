using System;
using System.Xml.Linq;

namespace Model.Entity.Generic
{
    public class ListboxComment
    {
        public ListboxComment() { }

        public ListboxComment(XElement xml)
        {
            TimeStamp = xml.Attribute("TimeStamp").Value;
            UserName = xml.Attribute("UserName").Value;
            Value = xml.Attribute("Value").Value;
        }

        public int Idx { get; set; }

        private string _timeStamp;
        public string TimeStamp
        {
            get { return _timeStamp; }
            set
            {
                DateTime date;
                DateTime.TryParse(value, System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.AssumeLocal, out date);
                
                _timeStamp = date == null ? "" : date.ToShortDateString();
            }
        }

        public string UserName { get; set; }

        public string Value { get; set; }

        public string Header { get { return "[" + TimeStamp + " " + UserName + "]"; } }

        private DateTime Truncate(DateTime dateTime, TimeSpan timeSpan)
        {
            if (timeSpan == TimeSpan.Zero) return dateTime; // Or could throw an ArgumentException
            return dateTime.AddTicks(-(dateTime.Ticks % timeSpan.Ticks));
        }
    }


}
