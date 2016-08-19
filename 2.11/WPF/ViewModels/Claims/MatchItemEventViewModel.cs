using System.Collections.Generic;
using Model.Entity;

namespace WPF.ViewModels.Claims
{
    public class MatchItemEventViewModel 
    {
        public string EventId { get; set; }
        public string EventName { get; set; }
        public string EventType { get; set; }
        public string EventSubType { get; set; }
        public string EventStatus { get; set; }
        public string TotalAccrual { get; set; }
        public string Settled { get; set; }
        public string AvailableAccrual { get; set; }
        public string TotalOutstandingClaims { get; set; }
        public IList<MatchItemClaimViewModel> Claims { get; set; }
    }
}