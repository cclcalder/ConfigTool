using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.DTOs
{
    public class ReturnEventsDTO
    {
        private const string DateFormatNoHyphens = "yyyyMMdd";
        public IList<string> CustomerIds { get; set; }
        public IList<string> ProductIds { get; set; } 
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IList<string> EventTypeIds { get; set; }
        public IList<string> EventStatusIds { get; set; }
        public string StartDateInputValue
        {
            get
            {
                return this.StartDate.ToString(DateFormatNoHyphens);
            }
        }
        public string EndDateInputValue
        {
            get
            {
                return this.EndDate.ToString(DateFormatNoHyphens);
            }
        }
    }
}
