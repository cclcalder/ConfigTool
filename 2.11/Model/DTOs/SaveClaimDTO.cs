using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.DTOs
{
    public class SaveClaimEventItem
    {
        public string EventId { get; set; }
        public string ClaimApportionmentValue { get; set; }
        public string ClaimApportionmentType { get; set; }
    }

    public class SaveClaimProductItem
    {
        public string EventId { get; set; }
        public string ProductId { get; set; }
        public string ClaimApportionmentValue { get; set; }
        public string ClaimApportionmentType { get; set; }
    }

    public class SaveClaimDTO
    {
        public SaveClaimDTO()
        {
            this.Events = new List<SaveClaimEventItem>();
            this.Products = new List<SaveClaimProductItem>();
            
        }

        public string ClaimId { get; set; }
        public string ClaimAdjustment { get; set; }
        public string ClaimStatusId { get; set; }
        public string ClaimScanPath { get; set; }
        public string ClaimPaymentDate { get; set; }
        public IList<SaveClaimEventItem> Events { get; set; }
        public IList<SaveClaimProductItem> Products { get; set; }
    }
}
