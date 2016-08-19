using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model.Entity
{
    public class Claim
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string ClaimReference { get; set; }
        public string ClaimLineDetail { get; set; }
        public DateTime ClaimDate { get; set; }
        public string ClaimDateDisplay { get { return ClaimDate.ToString("dd/MM/yyyy"); } }
        public DateTime EnteredDate { get; set; }
        public string EnteredDateDisplay { get { return EnteredDate.ToString("dd/MM/yyyy"); } }
        public string ClaimValue { get; set; }
        public string ClaimMatchingStatusName { get; set; }
        public string ClaimStatusName { get; set; }
        public string ClaimScanLocation { get; set; }
        public bool IsSelected { get; set; }

        public static Claim FromXml(XElement element)
        {
            const string dateFormat = "yyyy-MM-dd";
            const string idElement = "Claim_Idx";
            const string custIdElement = "Cust_Idx";
            const string custNameElement = "Customer_Name";
            const string claimReferenceElement = "Claim_Reference";
            const string claimLineDetailElement = "Claim_Line_Detail";
            const string claimDateElement = "Claim_Date";
            const string enteredDateElement = "Entered_Date";
            const string claimValueElement = "Claim_Value";
            const string claimMatchingStatusNameElement = "Claim_Matching_Status_Name";
            const string claimStatusNameElement = "Claim_Status_Name";
            const string claimScanLocationElement = "Claim_Scan_Location";
            return new Claim
            {
                Id = element.GetValue<string>(idElement),
                CustomerId = element.GetValue<string>(custIdElement),
                CustomerName = element.GetValue<string>(custNameElement),
                ClaimReference = element.GetValue<string>(claimReferenceElement),
                ClaimLineDetail = element.GetValue<string>(claimLineDetailElement),
                ClaimDate = DateTime.ParseExact(element.GetValue<string>(claimDateElement), dateFormat, null),
                EnteredDate = DateTime.ParseExact(element.GetValue<string>(enteredDateElement), dateFormat, null),
                ClaimValue = element.GetValue<string>(claimValueElement),
                ClaimMatchingStatusName = element.GetValue<string>(claimMatchingStatusNameElement),
                ClaimStatusName = element.GetValue<string>(claimStatusNameElement),
                ClaimScanLocation = element.GetValue<string>(claimScanLocationElement)
            };
        }
    }
}
