using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Exceedra.Common;

namespace Model.Entity.Listings
{
    public class UserSelectedDefaults
    {
        public string ScreenCode { get; set; }

        public HashSet<DefaultItem> Customers { get; set; }

        public HashSet<DefaultItem> Products { get; set; }

        public UserSelectedDefaults(XElement xml)
        {
            var xElement = xml.Element("Customers");
            Customers = xElement != null ? xElement.Elements().Select(c => new DefaultItem {Idx = c.Attribute("Idx").MaybeValue(), IsSelected = c.Attribute("IsSelected").MaybeValue() == "1"}).ToHashSet() : new HashSet<DefaultItem>();
            
            var element = xml.Element("Products");
            Products = element != null ? element.Elements().Select(c => new DefaultItem { Idx = c.Attribute("Idx").MaybeValue(), IsSelected = c.Attribute("IsSelected").MaybeValue() == "1" }).ToHashSet() : new HashSet<DefaultItem>();

            ScreenCode = xml.Element("Screen_Code").MaybeValue();
        }

        public HashSet<string> GetSelectedCustomerIdxs()
        {
            return Customers.Where(c => c.IsSelected).Select(c => c.Idx).ToHashSet();
        }

        public HashSet<string> GetSelectedSkuIdxs()
        {
            return Products.Where(c => c.IsSelected).Select(c => c.Idx).ToHashSet();
        }

        public HashSet<string> GetCustomerIdxs()
        {
            return Customers.Select(c => c.Idx).ToHashSet();
        }

        public HashSet<string> GetSkuIdxs()
        {
            return Products.Select(c => c.Idx).ToHashSet();
        }

        public UserSelectedDefaults()
        {
        }
    }

    public class DefaultItem
    {
        public string Idx { get; set; }
        public bool IsSelected { get; set; }
    }

}
