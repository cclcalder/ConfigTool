using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Linq;
using Exceedra.Common;
using Model.Entity.Demand;

namespace Model.Entity.Generic
{
    [Serializable]
    public class FilterObject
    {
        public List<string> CustomerIdxs { get; set; }
        public List<string> ProductIdxs { get; set; }
        public List<string> StatusIdxs { get; set; } 
        public FilterDateRange DateRange { get; set; }

        public static T DeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }

        public XElement ToXml()
        {
            var xml = new XElement("Filters");

            var custIdxs = new XElement("Customers");
            foreach (var idx in CustomerIdxs)
            {
                custIdxs.AddElement("Idx", idx);
            }
            xml.Add(custIdxs);

            var prodIdxs = new XElement("Products");
            foreach (var idx in ProductIdxs)
            {
                prodIdxs.AddElement("Idx", idx);
            }
            xml.Add(prodIdxs);

            var statusIdxs = new XElement("Statuses");
            foreach (var idx in StatusIdxs)
            {
                statusIdxs.AddElement("Idx", idx);
            }
            xml.Add(statusIdxs);

            xml.AddElement("Start", DateRange.StartDate.ToString("yyyy-MM-dd"));
            xml.AddElement("End", DateRange.EndDate.ToString("yyyy-MM-dd"));

            return xml;
        }
    }
}