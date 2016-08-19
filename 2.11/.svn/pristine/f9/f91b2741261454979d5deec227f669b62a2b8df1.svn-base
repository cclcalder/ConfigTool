using Model.DataAccess;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using Exceedra.Common;
using Xunit;

namespace Model.Test
{
    
    
    /// <summary>
    ///This is a test class for PlanningAccessTest and is intended
    ///to contain all PlanningAccessTest Unit Tests
    ///</summary>
    public class PlanningAccessTest
    {
        /// <summary>
        ///A test for TransformPlanningDataXml
        ///</summary>
        //[Fact]
        //public void TransformPlanningDataXmlTest()
        //{
        //    XElement source = XElement.Parse(Properties.Resources.MoreNestedPlanningDataXml);
        //    Dictionary<string, PlanningItem> productLookup = StubProductDictionary();
            
        //    var transformer = new PlanningAccessTransformer(source, productLookup);
        //    IEnumerable<XElement> actual = transformer.GetPlanningDataXml().ToList();
            
        //    Assert.Equal(1, actual.Count());

        //    var element = actual.ElementAt(0);
        //    CheckNode(element, "Bird Paste", "12517", 3, 13);

        //    var values = element.Elements("Row").SelectMany(
        //        row => row.Elements("Value").Select(v => v.Element("Data").MaybeValue()));

        //    foreach (var value in values)
        //    {
        //        Assert.Equal("2.2", value);
        //    }

        //    var midElement = element.Element("Nodes").MaybeElements("Node").OrderBy(r => r.Attribute("Product").MaybeValue()).ElementAt(0);
        //    CheckNode(midElement, "Poultry Paste", "19014", 3, 13);

        //    var leafElement = midElement.Element("Nodes").MaybeElements("Node").OrderBy(r => r.Attribute("Product").MaybeValue()).ElementAt(0);
        //    CheckNode(leafElement, "Chicken Paste", "32133", 3, 13);

        //    leafElement = midElement.Element("Nodes").MaybeElements("Node").OrderBy(r => r.Attribute("Product").MaybeValue()).ElementAt(1);
        //    CheckNode(leafElement, "Turkey Paste", "32128", 3, 13);

        //    midElement = element.Element("Nodes").MaybeElements("Node").OrderBy(r => r.Attribute("Product").MaybeValue()).ElementAt(1);
        //    CheckNode(midElement, "Waterfowl Paste", "19015", 3, 13);

        //    leafElement = midElement.Element("Nodes").MaybeElements("Node").OrderBy(r => r.Attribute("Product").MaybeValue()).ElementAt(0);
        //    CheckNode(leafElement, "Duck Paste", "32135", 3, 13);

        //    leafElement = midElement.Element("Nodes").MaybeElements("Node").OrderBy(r => r.Attribute("Product").MaybeValue()).ElementAt(1);
        //    CheckNode(leafElement, "Goose Paste", "32136", 3, 13);

        //    AssertValues(element, 1, "4.4");
        //    AssertValues(element, 2, "2.2");
        //    AssertValues(element, 3, "1.1");
        //}

        private void AssertValues(XElement element, int depth, string value)
        {
            foreach (var xElement in element.Descendants("Data").Where(elm => elm.Ancestors("Node").Count() == depth))
            {
                Assert.Equal(value, xElement.Value);
            }
        }

        private void CheckNode(XElement element, string productName, string productId, int rowCount, int valueCount)
        {
            Assert.Equal(productName, element.Attribute("Product").MaybeValue());
            Assert.Equal(productId, element.Attribute("ProductID").MaybeValue());
            Assert.Equal(rowCount, element.Elements("Row").Count());
            Assert.True(element.Elements("Row").All(e => e.Element("Values").MaybeElements("Value").Count() == valueCount));
            Assert.Equal(1, element.Elements("Row").Count(e => e.Attribute("Title").MaybeValue() == "Volatility"));
            Assert.Equal(1, element.Elements("Row").Count(e => e.Attribute("Title").MaybeValue() == "Acidity"));
            Assert.Equal(1, element.Elements("Row").Count(e => e.Attribute("Title").MaybeValue() == "Enthalpy"));
        }

        private static Dictionary<string,PlanningItem> StubProductDictionary()
        {
            return new Dictionary<string, PlanningItem>
                       {
                           {"32128", new PlanningItem {Idx = "32128", DisplayName = "Turkey Paste", ParentIdx = "19014"}},
                           {"32133", new PlanningItem {Idx = "32133", DisplayName = "Chicken Paste", ParentIdx = "19014"}},
                           {"32135", new PlanningItem {Idx = "32135", DisplayName = "Duck Paste", ParentIdx = "19015"}},
                           {"32136", new PlanningItem {Idx = "32136", DisplayName = "Goose Paste", ParentIdx = "19015"}},
                           {"19014", new PlanningItem {Idx = "19014", DisplayName = "Poultry Paste", ParentIdx = "12517"}},
                           {"19015", new PlanningItem {Idx = "19015", DisplayName = "Waterfowl Paste", ParentIdx = "12517"}},
                           {"12517", new PlanningItem {Idx = "12517", DisplayName = "Bird Paste"}},
                       };
        }
    }
}
