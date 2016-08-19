using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Exceedra.Common;
using Model.DataAccess;
using Model.Factories;
using Xunit;

namespace Model.Test
{
    public class PlanningDataProductMappingTest
    {
        static PlanningDataProductMappingTest()
        {
            var productsNodes = XElement.Parse(Properties.Resources.ProductDataXml).Elements();
            PlanningAccess.PlanningProductsCache.AddRange(productsNodes.Select(e => new PlanningItem(e)));

            var measuresNodes = XElement.Parse(Properties.Resources.MeasureDataXml).Elements();
            PlanningAccess.PlanningMeasuresCache.AddRange(measuresNodes.Select(m => new Measure(m)));
        }

        //[Fact]
        //public void CachingWorks()
        //{
        //    var cache = new AutoCache<string, PlanningDataItem>(pdp => pdp.Idx);

        //    var planningData1 = cache.CacheRange(PlanningAccess.ParsePlanningDataXml(XElement.Parse(Properties.Resources.PlanningDataXml))
        //        .Select(PlanningDataItemFactory.CreateFromXElement)).ToList();

        //    var planningData2 =
        //            cache.CacheRange(PlanningAccess.ParsePlanningDataXml(
        //                XElement.Parse(Properties.Resources.PlanningDataXml))
        //                                 .Select(PlanningDataItemFactory.CreateFromXElement))
        //                .ToList();

        //    Assert.Single(planningData1);
        //    Assert.Single(planningData2);
        //    Assert.Same(planningData1[0], planningData2[0]);
        //}

        [Fact]
        public void ChangesAreDetectedAtLeafLevel()
        {
            var parent = GetTopLevelPlanningDataProduct();
            var firstLeaf = parent.ChildItems.First();
            var firstMeasure = firstLeaf.Measures.First();
            var firstValue = firstMeasure.Values.First();

            firstValue.Data += 10;

            Assert.True(firstMeasure.HasChanges());

            foreach (var otherLeaf in parent.ChildItems.Skip(1))
            {
                Assert.False(otherLeaf.Measures.First().HasChanges());
            }
        }

        [Fact]
        public void ReversedChangesAreNotDetectedAtLeafLevel()
        {
            var parent = GetTopLevelPlanningDataProduct();
            var firstLeaf = parent.ChildItems.First();
            var firstMeasure = firstLeaf.Measures.First();
            var firstValue = firstMeasure.Values.First();

            firstValue.Data += 10;
            firstValue.Data -= 10;

            Assert.False(firstMeasure.HasChanges());
        }

        [Fact]
        public void ChangesAreDetectedAtParentLevel()
        {
            var parent = GetTopLevelPlanningDataProduct();
            var firstLeaf = parent.ChildItems.First();
            var firstMeasure = firstLeaf.Measures.First();
            var firstValue = firstMeasure.Values.First();

            firstValue.Data += 10;

            Assert.True(parent.Measures.First().HasChanges());
        }

        [Fact]
        public void ReversedChangesAreNotDetectedAtParentLevel()
        {
            var parent = GetTopLevelPlanningDataProduct();
            var firstLeaf = parent.ChildItems.First();
            var firstMeasure = firstLeaf.Measures.First();
            var firstValue = firstMeasure.Values.First();

            firstValue.Data += 10;
            firstValue.Data -= 10;

            Assert.False(parent.Measures.First().HasChanges());
        }

        private static PlanningDataItem GetTopLevelPlanningDataProduct()
        {
            //var planningData = PlanningAccess.ParsePlanningDataXml(XElement.Parse(Properties.Resources.PlanningDataXml))
            //    .Select(PlanningDataItemFactory.CreateFromXElement).ToList();

            //return planningData.First();
            return null;
        }
    }
}
