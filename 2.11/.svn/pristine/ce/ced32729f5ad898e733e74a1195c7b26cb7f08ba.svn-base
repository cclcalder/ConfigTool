using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model.Entity
{
    public class ClaimItem
    {
        public string Id { get; set; }
        public string CustomerName { get; set; }
        public string ClaimReference { get; set; }
        public string ClaimLineDetail { get; set; }
        public DateTime ClaimDate { get; set; }
        public string ClaimStatusName { get; set; }
        public string ScanLocation { get; set; }
        public string ClaimValue { get; set; }
        public string ClaimNetValue { get; set; }
        public bool FileFound
        {
            get
            {
                return File.Exists(ScanLocation);
            }
        }
        public static ClaimItem FromXml(XElement element)
        {
            const string idElement = "Claim_Idx";
            const string customerNameElement = "Customer_Name";
            const string claimReferenceElement = "Claim_Reference";
            const string claimLineDetailElement = "Claim_Line_Detail";
            const string claimDateElement = "Claim_Date";
            const string claimStatusNameElement = "Claim_Status_Name";
            const string scanLocationElement = "Claim_Scan_Location";
            const string claimValueElement = "Claim_Value";
            const string claimNetValueElement = "Claim_Net_Value";
            const string dateFormat = "yyyy-MM-dd";
            return new ClaimItem
            {
                Id = element.GetValue<string>(idElement),
                CustomerName = element.GetValue<string>(customerNameElement),
                ClaimReference = element.GetValue<string>(claimReferenceElement),
                ClaimLineDetail = element.GetValue<string>(claimLineDetailElement),
                ClaimDate = DateTime.ParseExact(element.GetValue<string>(claimDateElement), dateFormat, null),
                ClaimStatusName = element.GetValue<string>(claimStatusNameElement),
                ScanLocation = element.GetValue<string>(scanLocationElement),
                ClaimValue = element.GetValue<string>(claimValueElement),
                ClaimNetValue = element.GetValue<string>(claimNetValueElement)
            };
        }
    }
}
