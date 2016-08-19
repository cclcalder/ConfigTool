using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.DTOs
{
    public class ClaimEventMatchItem
    {
        public string FakeClaimId { get; set; }
        public string EventId { get; set; }
    }

    public class AddClaimItem {
        private const string DateFormatNoHyphens = "yyyyMMdd";
        public string FakeClaimId { get; set; }
        public string CustomerId { get; set; }
        public string ClaimReference { get; set; }
        public string ClaimLineDetail { get; set; }
        public DateTime ClaimDate { get; set; }

        public string ClaimDateInputValue
        {
            get
            {
                return this.ClaimDate.ToString(DateFormatNoHyphens);
            }
        }
        public string ClaimValue { get; set; }
        public string ClaimScanLocation { get; set; }
    }

    public class AddClaimsDTO
    {
		private IList<AddClaimItem> _claims;
		private IList<ClaimEventMatchItem> _matches;

        public IList<AddClaimItem> Claims 
		{
			get
			{
				return _claims ?? (_claims = new List<AddClaimItem>());
			}
		}
        public IList<ClaimEventMatchItem> Matches 
		{
			get
			{
				return _matches ?? (_matches = new List<ClaimEventMatchItem>());
			}
		}
    }
}
