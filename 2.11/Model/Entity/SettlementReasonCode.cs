using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model.Entity
{
    public class SettlementReasonCode
    {
        public string ReasonCodeId { get; set; }
        public string ReasonCodeName { get; set; }

        public static SettlementReasonCode FromXml(XElement element)
        {
            const string reasonCodeIdElement = "Reason_Code_Idx";
            const string reasonCodeNameElement = "Reason_Code_Name";

            return new SettlementReasonCode
            {
                ReasonCodeId = element.GetValue<string>(reasonCodeIdElement),
                ReasonCodeName = element.GetValue<string>(reasonCodeNameElement)                
            };
        }
    }
}
