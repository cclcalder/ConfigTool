using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.DTOs
{
    public class SetDefaultFiltersDTO
    {
        private const string DateFormatNoHyphens = "yyyyMMdd";
        public string ClaimFilterMax { get; set; }
        public string ClaimFilterMin { get; set; }
        public string DateSearchPreference { get; set; }
        public IList<string> ClaimMatchingStatusIds { get; set; }
        public IList<string> Statuses { get; set; }
        public IList<string> CustomerIds { get; set; }
        public IList<string> ProductIds { get; set; } 
        public IList<string> ClaimValueRangeIds { get; set; }
        public DateTime ClaimStartDate { get; set; }
        public DateTime ClaimEndDate { get; set; }
        public DateTime EventStartDate { get; set; }
        public DateTime EventEndDate { get; set; }
        public IList<string> EventTypeIds { get; set; }
        public string SalesOrgId { get; set; }
        public IList<string> EventStatusId { get; set; }

        public string ClaimStartDateInputValue
        {
            get
            {
                return this.ClaimStartDate.ToString(DateFormatNoHyphens);
            }
        }
        public string ClaimEndDateInputValue
        {
            get
            {
                return this.ClaimEndDate.ToString(DateFormatNoHyphens);
            }
        }
        public string EventStartDateInputValue
        {
            get
            {
                return this.EventStartDate.ToString(DateFormatNoHyphens);
            }
        }
        public string EventEndDateInputValue
        {
            get
            {
                return this.EventEndDate.ToString(DateFormatNoHyphens);
            }
        }

        public string ListingsGroupIdx { get; set; }
    }
}
