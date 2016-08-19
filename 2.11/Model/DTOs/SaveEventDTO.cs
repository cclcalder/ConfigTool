using Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.DTOs
{
    public class SaveEventClaimItem
    {
        public string ClaimId { get; set; }
        public string ClaimApportionmentValue { get; set; }
        public string ClaimApportionmentType { get; set; }
    }

    public class SaveEventProductItem
    {
        public string ProductId { get; set; }
        public string ClaimId { get; set; }
        public string ClaimApportionmentValue { get; set; }
        public string ClaimApportionmentType { get; set; }
    }

    public class SaveEventDTO
    {
        public SaveEventDTO()
        {
            this.Claims = new List<SaveEventClaimItem>();
            this.Adjustments = new List<EventDetailAdjustment>();
            this.Products = new List<SaveEventProductItem>();
        }

        public string EventId { get; set; }
        public string EventAdjustment { get; set; }
        public string EventStatusId { get; set; }
        public string ReasonCodeId { get; set; }
        public IList<SaveEventClaimItem> Claims { get; set; }
        public IList<EventDetailAdjustment> Adjustments { get; set; }
        public IList<SaveEventProductItem> Products { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
