using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;
using System.Linq;
 
using Model.Entity;

namespace Model.Test.Schedule
{
    [TestClass]
    public class Test_Statuses
    {
        const string xml = @"<Results>
                                <Type>
                                  <Type_Idx>0</Type_Idx>
                                  <Type_Name>Promotion</Type_Name>
                                <Statuses>
                                  <Status>
                                    <Status_Idx>1</Status_Idx>
                                    <Status_Name>Draft</Status_Name>
                                    <Status_Colour>#FFFFFF</Status_Colour>
                                    <IsSelected>0</IsSelected>
                                  </Status>
                                  <Status>
                                    <Status_Idx>2</Status_Idx>
                                    <Status_Name>Planned</Status_Name>
                                    <Status_Colour>#8B668B</Status_Colour>
                                    <IsSelected>1</IsSelected>
                                  </Status>
                                </Statuses>
                               </Type>  
                               <Type>
                                  <Type_Idx>100</Type_Idx>
                                  <Type_Name>Rob</Type_Name>
                                <Statuses>
                                  <Status>
                                    <Status_Idx>1</Status_Idx>
                                    <Status_Name>Draft</Status_Name>
                                    <Status_Colour>#FFFFFF</Status_Colour>
                                    <IsSelected>1</IsSelected>
                                  </Status>
                                  <Status>
                                    <Status_Idx>2</Status_Idx>
                                    <Status_Name>Planned</Status_Name>
                                    <Status_Colour>#8B668B</Status_Colour>
                                    <IsSelected>1</IsSelected>
                                  </Status>
                                 </Statuses>                         
                            </Type>
                         
                             </Results>";

        [TestMethod]
        public void GetFromXML()
        {
            var el = XElement.Parse(xml);

            var s = el.Elements("Type").Select(ScheduleStatuses.FromXml).ToList();

            Assert.IsTrue(s.Count == 2);
            Assert.IsTrue(s[0].Statuses.Count == 2);
            Assert.IsTrue(s[0].Statuses[0].ID == "1");
            Assert.IsTrue(s[0].Statuses[0].ID != "3");
            Assert.IsTrue(s[0].Statuses[0].IsSelected == false);

            Assert.IsTrue(s[1].Statuses.Count == 2);
            Assert.IsTrue(s[1].Statuses[0].ID == "1");
            Assert.IsTrue(s[1].Statuses[1].ID != "3");
            Assert.IsTrue(s[1].Statuses[1].IsSelected == true);

        }
    }
}
