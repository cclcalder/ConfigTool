using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.DTOs
{
    public class ReturnClaimsDTO
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
        public IList<string> SalesOrgIds { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string StartDateInputValue { 
            get { 
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
