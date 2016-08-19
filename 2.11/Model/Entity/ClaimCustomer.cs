using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model.Entity
{
    public class ClaimCustomer
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public virtual bool IsSelected { get; set; }

        public static ClaimCustomer FromXml(XElement element)
        {
            const string idElement = "Cust_Idx";
            const string nameElement = "Cust_Name";
            const string isSelectedElement = "IsSelected";

            return new ClaimCustomer
            {
                Id = element.GetValue<string>(idElement),
                Name = element.GetValue<string>(nameElement),
                IsSelected = element.GetValue<int>(isSelectedElement) == 1
            };
        }
    }
}
