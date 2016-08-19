using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model.Entity
{
    public class ClaimCustomerLevel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public static ClaimCustomerLevel FromXml(XElement element)
        {
            const string idElement = "Cust_Level_Idx";
            const string nameElement = "Cust_Level_Name";

            return new ClaimCustomerLevel
            {
                Id = element.GetValue<string>(idElement),
                Name = element.GetValue<string>(nameElement),
            };
        }
    }
}
