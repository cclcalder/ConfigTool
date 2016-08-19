using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model.Entity
{
    public class MatchItemClaim
    {
        private string _claimId;
        private string _customerId;
        private DateTime _enteredDate;
        private DateTime _claimDate;

        public MatchItemClaim(string claimId, string customerId, DateTime enteredDate, DateTime claimdate)
        {
            _claimId = claimId;
            _customerId = customerId;
            _enteredDate = enteredDate;
            _claimDate = claimdate;
        }

        [DisplayName("Customer Name")]
        public string CustomerName { get; set; }

        [DisplayName("Claim Date")]
        public string ClaimDate { get { return _claimDate.ToShortDateString(); } }

        [DisplayName("Entered Date")]
        public string EnteredDate { get { return _enteredDate.ToShortDateString(); } }

        [DisplayName("Claim Reference")]
        public string ClaimReference { get; set; }

        [DisplayName("Claim Line Detail")]
        public string ClaimLineDetail { get; set; }

        [DisplayName("Claim Value")]
        public string ClaimValue { get; set; }

         [DisplayName("Claim Status")]
        public string ClaimStatusName { get; set; }

        public string GetClaimId()
        {
            return _claimId;
        }

        public string GetCustomerId()
        {
            return _customerId;
        }

        public static MatchItemClaim FromXml(XElement element)
        {
            const string dateFormat = "yyyy-MM-dd";
            const string claimIdelement = "Claim_Idx";
            const string customerIdElement = "Cust_Idx";
            const string customerNameElement = "Customer_Name";
            const string claimDateElement = "Claim_Date";
            const string enteredDateElement = "Entered_Date";
            const string claimReferenceElement = "Claim_Reference";
            const string claimLineDetailElement = "Claim_Line_Detail";
            const string claimValueElement = "Claim_Value";
            const string claimStatusElement="Claim_Status_Name";
            
            string claimId=element.GetValue<string>(claimIdelement);
            string customerId=element.GetValue<string>(customerIdElement);
            DateTime claimDate=DateTime.ParseExact(element.GetValue<string>(claimDateElement), dateFormat, null);
            DateTime enteredDate=DateTime.ParseExact(element.GetValue<string>(enteredDateElement), dateFormat, null);
            MatchItemClaim claim = new MatchItemClaim(claimId, customerId, enteredDate, claimDate)
            {
                CustomerName = element.GetValue<string>(customerNameElement),
                ClaimReference = element.GetValue<string>(claimReferenceElement),
                ClaimLineDetail = element.GetValue<string>(claimLineDetailElement),
                ClaimValue = element.GetValue<string>(claimValueElement),
                ClaimStatusName = element.GetValue<string>(claimStatusElement)
            };

            return claim;
        }
    }
    public class MatchItemEvent
    {
        private string _eventId;

        public MatchItemEvent(string eventId)
        {
            _eventId = eventId;
        }
        
        public string GetEventId()
        {
            return _eventId;
        }

        [DisplayName("Event Name")]
        public string EventName { get; set; }

        [DisplayName("Event Type")]
        public string EventType { get; set; }

        [DisplayName("Event Subtype")]
        public string EventSubType { get; set; }

        [DisplayName("Event Status")]
        public string EventStatus { get; set; }

        [DisplayName("Total Accrual")]
        public string TotalAccrual { get; set; }

        [DisplayName("Settled")]
        public string Settled { get; set; }

        [DisplayName("Available Accrual")]
        public string AvailableAccrual { get; set; }

        [DisplayName("Total Outstanding Claims")]
        public string TotalOutstandingClaims { get; set; }

        public static MatchItemEvent FromXml(XElement element)
        {
            const string eventIdelement = "Event_Idx";
            const string eventNameElement = "Event_Name";
            const string eventTypeElement = "Event_Type";
            const string eventSubTypeElement = "Event_Sub_Type";
            const string eventStatusElement = "Event_Status";
            const string totalAccrualElement = "Total_Accrual";
            const string settledElement = "Settled";
            const string available_AccrualElement = "Available_Accrual";
            const string totalOutstandingClaimsElement = "Total_Outstanding_Claims";

            string eventId = element.GetValue<string>(eventIdelement);
            MatchItemEvent claimMatch = new MatchItemEvent(eventId)
            {
                EventName = element.GetValue<string>(eventNameElement),
                EventType = element.GetValue<string>(eventTypeElement),
                EventSubType = element.GetValue<string>(eventSubTypeElement),
                EventStatus = element.GetValue<string>(eventStatusElement),
                TotalAccrual = element.GetValue<string>(totalAccrualElement),
                Settled = element.GetValue<string>(settledElement),
                AvailableAccrual = element.GetValue<string>(available_AccrualElement),
                TotalOutstandingClaims = element.GetValue<string>(totalOutstandingClaimsElement)
            };

            return claimMatch;
        } 
    }

    public class ClaimApportionment
    {
        public string Value { get; set; }
        public string Type { get; set; }
        public static ClaimApportionment FromXml(XElement element)
        {
            const string typeelement = "Type";
            const string valueElement = "Value";
            return new ClaimApportionment
            {
                Type = element.GetValue<string>(typeelement),
                Value = element.GetValue<string>(valueElement),
            };
        }
    }
    public class MatchItem
    {
        public string EventId { get; set; }
        public string ClaimId { get; set; }
        public ClaimApportionment ClaimApportionment { get; set; }
        public string ApportionedAmount { get; set; }
        public static MatchItem FromXml(XElement element)
        {
            const string eventIdelement = "Event_Idx";
            const string claimIdElement = "Claim_Idx";
            const string apportionedAmountElement = "Apportioned_Amount";
            return new MatchItem
            {
                EventId = element.GetValue<string>(eventIdelement),
                ClaimId = element.GetValue<string>(claimIdElement),
                ClaimApportionment = element.Elements("Claim_Apportionment").Count() > 0 ? ClaimApportionment.FromXml(element.Elements("Claim_Apportionment").SingleOrDefault()) : null,
                ApportionedAmount = element.GetValue<string>(apportionedAmountElement)
            };
        }
    }

    public class ReturnedMatches
    {
        public IList<MatchItemEvent> Events { get; set; }
        public IList<MatchItemClaim> Claims { get; set; }
        public IList<MatchItem> Matches { get; set; }
        public static ReturnedMatches FromXml(XElement element)
        {
            List<MatchItemClaim> claims = new List<MatchItemClaim>();
            List<MatchItemEvent> events = new List<MatchItemEvent>();
            List<MatchItem> matches = new List<MatchItem>();
 
            foreach (var claimElement in element.Elements("Claims").Elements())
            {
                claims.Add(MatchItemClaim.FromXml(claimElement));
            }

            foreach (var eventElement in element.Elements("Events").Elements())
            {
                events.Add(MatchItemEvent.FromXml(eventElement));
            }

            foreach (var matchElement in element.Elements("Matches").Elements())
            {
                matches.Add(MatchItem.FromXml(matchElement));
            }
            return new ReturnedMatches
            {
                Events = events,
                Claims = claims,
                Matches = matches
            };
        }
    }
}
