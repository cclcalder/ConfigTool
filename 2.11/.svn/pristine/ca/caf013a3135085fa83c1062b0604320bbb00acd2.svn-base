using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model.Entity
{
    public class Apportionment
    {
        public string EventId { get; set; }
        public string ClaimApprotionmentValue { get; set; }
        public string ClaimApprotionmentType { get; set; }

        public static Apportionment FromXml(XElement element)
        {
            const string eventIdElement = "Event_Idx";
            const string claimApprotionmentValueElement = "Claim_Apportionment_Value";
            const string claimApprotionmentTypeElement = "Claim_Apportionment_Type";

            return new Apportionment
            {
                EventId = element.GetValue<string>(eventIdElement),
                ClaimApprotionmentValue = element.GetValue<string>(claimApprotionmentValueElement),
                ClaimApprotionmentType = element.GetValue<string>(claimApprotionmentTypeElement)
            };
        }
    }
}
