using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model.Entity
{
    public class AutomaticMatch
    {
        public string ClaimId { get; set; }
        public string EventId { get; set; }

        public static AutomaticMatch FromXml(XElement element)
        {
            const string claimIdElement = "Claim_Idx";
            const string eventIdElement = "Event_Idx";
            return new AutomaticMatch() {
                ClaimId = element.GetValue<string>(claimIdElement),
                EventId = element.GetValue<string>(eventIdElement)
            };
        }
    }
}
