using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model.Entity
{
    public class ClaimDetail
    {
        public string Id { get; set; }
        public string CustomerName { get; set; }
        public string ClaimReference { get; set; }
        public string ClaimLineDetail { get; set; }
        public DateTime ClaimEnteredDate { get; set; }
        public DateTime ClaimDate { get; set; }
        public DateTime? PaymentClaimDate { get; set; }
        public string ClaimEnteredDateDisplay { get { return ClaimEnteredDate.ToString("dd/MM/yyyy"); } }
        public string ClaimDateDisplay { get { return ClaimDate.ToString("dd/MM/yyyy"); } }
        public string ClaimPaymentDateDisplay { get { return (PaymentClaimDate.HasValue==true ? PaymentClaimDate.ToString("dd/MM/yyyy") : ""); } }
        public string ClaimValue { get; set; }
        public string ClaimAdjustment { get; set; }
        public string ClaimNetValue { get; set; }
        public string ClaimStatusId { get; set; }
        public string ClaimMatchedName { get; set; }
        public string ClaimScanLocation { get; set; }
        public bool FileFound
        {
            get
            {
                return File.Exists(ClaimScanLocation);
            }
        }

        public static ClaimDetail FromXml(XElement element)
        {
            const string dateFormat = "yyyy-MM-dd";
            const string idElement = "Claim_Idx";
            const string custNameElement = "Customer_Name";
            const string claimReferenceElement = "Claim_Reference";
            const string claimLineDetailElement = "Claim_Line_Detail";
            const string claimDateElement = "Claim_Date";
            const string enteredDateElement = "Claim_Entered_Date";
            const string claimValueElement = "Claim_Value";
            const string claimScanLocationElement = "Claim_Scan_Location";
            const string claimAdjustmentElement = "Claim_Adjustment";
            const string claimNetValueElement = "Claim_Net_Value";
            const string claimStatusIdElement = "Claim_Status_Idx";
            const string claimMatchedNameElement = "Claim_Matched_Name";
            const string claimPaymentDateElement = "Claim_Payment_Date";

            var d = element.GetValue<string>(claimPaymentDateElement);

            var res = new ClaimDetail
            {
                Id = element.GetValue<string>(idElement),
                CustomerName = element.GetValue<string>(custNameElement),
                ClaimReference = element.GetValue<string>(claimReferenceElement),
                ClaimLineDetail = element.GetValue<string>(claimLineDetailElement),
                ClaimDate = DateTime.ParseExact(element.GetValue<string>(claimDateElement), dateFormat, null),
                ClaimEnteredDate = DateTime.ParseExact(element.GetValue<string>(enteredDateElement), dateFormat, null),
                ClaimValue = element.GetValue<string>(claimValueElement),
                ClaimScanLocation = element.GetValue<string>(claimScanLocationElement),
                ClaimAdjustment = element.GetValue<string>(claimAdjustmentElement),
                ClaimNetValue = element.GetValue<string>(claimNetValueElement),
                ClaimStatusId = element.GetValue<string>(claimStatusIdElement),
                ClaimMatchedName = element.GetValue<string>(claimMatchedNameElement)
              
            };

            if (d != null)
                res.PaymentClaimDate =  DateTime.ParseExact(d, dateFormat, null);

            return res;
        }
    }
}
