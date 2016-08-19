using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Model.Test
{
    [TestClass]
    public class PromotionFinancialTest
    {
        [TestMethod]
        public void PercentValueFormatsCorrectlyFromConstructor()
        {
            var target = new PromotionFinancial("1", "Foo", "P", "100");
            Assert.AreEqual("100.00%", target.Value);
        }

        [TestMethod]
        public void PercentValueFormatsCorrectlyFromProperty()
        {
            var target = new PromotionFinancial("1", "Foo", "P", "100");
            target.Value = "200";
            Assert.AreEqual("200.00%", target.Value);
        }
    }
}
