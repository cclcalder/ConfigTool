using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using Model;
namespace WPF.Test
{
    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void ConditionName()
        {
            XElement clientConfigXml = XElement.Parse("<Results><ConfigItem><Section>Configuration</Section><Key>Conditions_Screen_Name</Key><Value>Some Test Value</Value> </ConfigItem></Results>");
            var res = "Conditions";
            var q = from element in clientConfigXml.Elements()
                    where element.Element("Section").MaybeValue() == "Configuration"
                        && element.Element("Key").MaybeValue() == "Conditions_Screen_Name"
                    select element.Element("Value").MaybeValue();

              res = q.First().ToString();

           Assert.IsTrue("Some Test Value" == res);
        }
    }
}
