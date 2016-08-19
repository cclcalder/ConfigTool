using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;

namespace WPF.Test.DemandPlanning
{
    [TestClass]
    public class UnitTest1
    {

        const string dummy = @"               
             <UpliftResponse >
                    <Parameters />
                     <Uplifts>
                           <UpliftRow>
                               <Measures>
                                   <UpliftMeasure>
                                      <Code>SysUpliftPc</Code>
                                      <Value>150</Value>
                                   </UpliftMeasure>
                                   <UpliftMeasure>
                                        <Code>SysUpliftMin</Code>.
                                        <Value>-233067.80799627936</Value>
                                   </UpliftMeasure>
                                   <UpliftMeasure>
                                       <Code>SysUpliftMax</Code>
                                       <Value>233367.80799627936</Value>
                                   </UpliftMeasure>
                              </Measures>
                              <Promo_Idx>1</Promo_Idx>
                              <Sku_Idx>15095</Sku_Idx>
                           </UpliftRow>                     
                      </Uplifts>
              </UpliftResponse>";


        [TestMethod]
        public void TestXMLParsing()
        {
            var x = new Model.Entity.DemandPlanning.UpliftResponse(XElement.Parse(dummy));
            Assert.IsTrue(x.Uplifts.Count > 0);

            Assert.IsTrue(x.Uplifts[0].Measures.Count == 3);
 
        }
    }
}
