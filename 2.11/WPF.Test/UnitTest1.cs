using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

namespace WPF.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void JoinString()
        {

            //string customers = string.Join(",", new List<string>(customerIds).ToArray());
            var customersIDs = new List<string>();
            customersIDs.Add("id1");
            customersIDs.Add("id2");
            string joined = string.Join("", (from c in customersIDs select new XElement("CustomerID", c) ));
            Console.WriteLine(joined.ToString());
        }
    }
}
