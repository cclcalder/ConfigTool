using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Exceedra.Common;

namespace Model.Entity
{
    public class Listing
    {
        public Listing()
        { }

        public Listing(XElement inputElements)
        {
            TheseListings = new Dictionary<string, ListingsProduct>();

            foreach (var element in inputElements.Elements())
            {
                listing(element);
            }
        }

        private void listing(XElement element)
        {
            string customer = element.GetValue<string>("Cust_Idx");
            string sku = element.GetValue<string>("Sku_Idx");
            var date = element.Element("Date_End").MaybeValue();

            DateTime? delistingsDate = null;

            if (date != null)
            {
                delistingsDate = DateTime.Parse(date);
            }

            ListingsProduct listingsProduct = new ListingsProduct();
            listingsProduct.ProductID = sku;
            listingsProduct.DelistingsDate = delistingsDate;

            string thisString = customer + "@";
            TheseListings.Add((thisString + sku), listingsProduct);
        }

        public Dictionary<string, ListingsProduct> TheseListings;

        public Listing CreateListings(XElement inputElement)
        {
            return new Listing(inputElement);
        }
    }

    public class ListingsProduct
    {
        public string ProductID { get; set; }
        public DateTime? DelistingsDate { get; set; }
    }
}
