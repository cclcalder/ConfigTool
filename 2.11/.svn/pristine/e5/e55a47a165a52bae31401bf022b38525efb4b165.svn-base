using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model.Entity
{
    public class EventProduct
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ApportionmentValue { get; set; }
        public string ApportionmentType { get; set; }
        public string ApportionedAmount { get; set; }

        public static EventProduct FromXml(XElement element)
        {
            const string eventProductIdElement = "Product_Idx";
            const string eventProductCodeElement = "Product_Code";
            const string eventProductNameElement = "Product_Name";
            const string eventProductApportionmentValueElement = "Product_Apportionment_Value";
            const string eventProductApportionmentTypeElement = "Product_Apportionment_Type";
            const string eventProductApportionedAmountElement = "Product_Apportioned_Amount";

            return new EventProduct
            {
                Id = element.GetValue<string>(eventProductIdElement),
                Code = element.GetValue<string>(eventProductCodeElement),
                Name = element.GetValue<string>(eventProductNameElement),
                ApportionmentValue = element.GetValue<string>(eventProductApportionmentValueElement),
                ApportionmentType = element.GetValue<string>(eventProductApportionmentTypeElement),
                ApportionedAmount = element.GetValue<string>(eventProductApportionedAmountElement)
            };
        }
    }
}
