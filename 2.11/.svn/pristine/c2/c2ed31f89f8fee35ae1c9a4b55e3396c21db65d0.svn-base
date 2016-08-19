using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model.Entity
{
    public class ApportionmentItem
    {
        public string ClaimId { get; set; }
        public string ClaimApprotionmentValue { get; set; }
        public string ClaimApprotionmentType { get; set; }

        public static ApportionmentItem FromXml(XElement element)
        {
            const string claimIdElement = "Claim_Idx";
            const string claimApprotionmentElement = "Claim_Apportionment";
            const string claimApprotionmentTypeElement = "Claim_Apportionment_Type";

            return new ApportionmentItem
            {
                ClaimId = element.GetValue<string>(claimIdElement),
                ClaimApprotionmentValue = element.GetValue<string>(claimApprotionmentElement),
                ClaimApprotionmentType = element.GetValue<string>(claimApprotionmentTypeElement)
            };
        }
    }
}
