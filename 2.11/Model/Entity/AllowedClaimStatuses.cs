using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model.Entity
{
    public class AllowedClaimStatuses
    {
        public string ClaimStatusId { get; set; }
        public string ClaimStatusName { get; set; }
        public bool IsEnabled { get; set; }

        public static AllowedClaimStatuses FromXml(XElement element)
        {
            const string claimStatusIdElement = "Claim_Status_Idx";
            const string claimStatusNameElement = "Claim_Status_Name";
            const string claimIsEnabledElement = "IsEnabled";

            return new AllowedClaimStatuses
            {
                ClaimStatusId = element.GetValue<string>(claimStatusIdElement),
                ClaimStatusName = element.GetValue<string>(claimStatusNameElement),
                IsEnabled = element.GetValue<string>(claimIsEnabledElement)=="1"
            };
        }
    }
}
