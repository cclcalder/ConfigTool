using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model.Entity
{
    public class EventProductDetail
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ApportionmentValue { get; set; }
        public string ApportionmentType { get; set; }
        public string ClaimLineDetail { get; set; }
        public string EventApportionedAmount { get; set; }
        public string ClaimId { get; set; }

        public static EventProductDetail FromXml(XElement element)
        {
            const string eventProductIdElement = "Product_Idx";
            const string eventProductCodeElement = "Product_Code";
            const string eventProductNameElement = "Product_Name";
            const string claimLineDetailElement = "Claim_Line_Detail";
            const string eventApportionedAmountElement = "Event_Apportioned_Amount";
            const string productApportionmentValueElement = "Product_Apportionment_Value";
            const string productApportionmentTypeElement = "Product_Apportionment_Type";
            const string claimIdElement = "Claim_Idx";

            return new EventProductDetail
            {
                Id = element.GetValue<string>(eventProductIdElement),
                Code = element.GetValue<string>(eventProductCodeElement),
                Name = element.GetValue<string>(eventProductNameElement),
                ApportionmentValue = element.GetValue<string>(productApportionmentValueElement),
                ApportionmentType = element.GetValue<string>(productApportionmentTypeElement),
                ClaimLineDetail = element.GetValue<string>(claimLineDetailElement),
                EventApportionedAmount = element.GetValue<string>(eventApportionedAmountElement),
                ClaimId = element.GetValue<string>(claimIdElement)
            };
        }
    }
}
