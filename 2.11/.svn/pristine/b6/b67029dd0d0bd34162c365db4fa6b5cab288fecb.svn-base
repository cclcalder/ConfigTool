using System.Collections.Generic;
using System.Xml.Linq;
using Exceedra.Common;

namespace Model.Entity
{
    public class NpdProduct
    {
        public string Idx { get; set; }
        public string Status { get; set; }

        public IList<string> SelectedUsers { get; set; }
        public IList<string> SelectedCustomers { get; set; }

        public XDocument ProductSkuGrid { get; set; }
        public XDocument ProductSkuCustGrid { get; set; }
        public XDocument DesignGrid { get; set; }
        public XDocument ComponentsGrid { get; set; }

        const string StatusElement = "Status_Idx";

        public static NpdProduct FromXml(XElement node)
        {
            var npdProduct = new NpdProduct
            {
                Status = node.GetValue<string>(StatusElement),
            };

            foreach (var x in node.Descendants("User_Idx"))
            {
                npdProduct.SelectedUsers.Add(x.Value);
            }

            foreach (var x in node.Descendants("Customer_Idx"))
            {
                npdProduct.SelectedCustomers.Add(x.Value);
            }
            

            return npdProduct;
        }

        public XElement ToXml(string rootTag)
        {
            XElement arguments = new XElement(rootTag);
            arguments.AddElement(StatusElement, Status);

            XElement users = new XElement("Users");
            foreach (var u in SelectedUsers)
            {
                users.AddElement("User_Idx", u);
            }
            arguments.Add(users);

            XElement customers = new XElement("Customers");
            foreach (var c in SelectedCustomers)
            {
                customers.AddElement("Customer_Idx", c);
            }
            arguments.Add(customers);

            arguments.AddElement("NPD_Product_Sku_Grid", ProductSkuGrid.Root);
            arguments.AddElement("NPD_Product_Sku_Cust_Grid", ProductSkuCustGrid.Root);
            arguments.AddElement("NPD_Design_Grid", DesignGrid.Root);

            if (ComponentsGrid != null) arguments.AddElement("NPD_Components_Grid", ComponentsGrid.Root);

            return arguments;
        }
    }
}
