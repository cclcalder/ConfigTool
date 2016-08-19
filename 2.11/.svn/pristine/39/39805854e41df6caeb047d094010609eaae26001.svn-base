using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Exceedra.Common;

namespace Model.DataAccess.Converters
{
    public static class InputConverter
    {
        public static XElement ToCustomers(IEnumerable<string> customers)
        {
            return ToIdxList("Customers", customers.OrderBy(q => q));
        }

        public static XElement ToCustomers(string customer)
        {
            return ToIdxList("Customers", new List<string> { customer });
        }

        public static XElement ToProducts(IEnumerable<string> products)
        {
            return ToIdxList("Products", products.OrderBy(q => q));
        }

        public static XElement ToProducts(string product)
        {
            return ToIdxList("Products", new List<string> { product });
        }

        public static XElement ToProducts(Dictionary<string, string> products)
        {
            return ToPlanningList("Products", "Product", products);
        }

        public static XElement ToIdxList(string tag, IEnumerable<string> input)
        {
            XElement output = new XElement(tag);
            foreach (var i in input.OrderBy(q => q))
            {
                output.AddElement("Idx", i);
            }

            return output;
        }

        public static XElement ToList(string tag, string innerTag, IEnumerable<string> input)
        {
            XElement output = new XElement(tag);
            foreach (var i in input.OrderBy(q => q))
            {
                output.AddElement(innerTag, i);
            }

            return output;
        }

        private static XElement ToPlanningList(string tag, string innerTag, Dictionary<string, string> input)
        {
            XElement output = new XElement(tag);
            foreach (var i in input)
            {
                var product = new XElement(innerTag);
                product.AddElement("Idx", i.Key);
                product.AddElement("IsParentNode", i.Value);

                output.Add(product);
            }

            return output;
        }

        public static string ToIsoFormat(DateTime? date)
        {
            return date.ToString("yyyy-MM-dd");
        }

        public static string ToIsoFormat(string date)
        {
            try
            {
                return Convert.ToDateTime(date).ToString("yyyy-MM-dd");
            }
            catch
            {
                return "";
            }
        }
    }
}