using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Exceedra.Common;

namespace Model.Test.DataAccess
{
    [TestClass]
    public class PromoSaveTestSchedule
    {

        [TestMethod]
        public void CheckXMLParsing()
        { 
            var p = new Model.Entity.PromotionSaveResults(XElement.Parse(xml));
            Assert.IsTrue(p.ValidationStatus == ValidationStatus.Error);
            Assert.IsTrue(p.WizardPages.Count() == 6);

            Assert.IsTrue(p.WizardPages[0].WizardTabCode == "Customer");
        }



        string xml = @"<Results>
                <Msg>Success or Disaster Warning Message</Msg>  Just display this to the user
          <ValidationStatus>0</ValidationStatus>          0 = everything fine, 1 = error 
          <WizardPages>
            <Tab>
              <WizardTab_Code>Customer</WizardTab_Code>
              <IsCompleted>1</IsCompleted>                     0 = not started, 1 = green tick, 2 = previously saved but now needs review
            </Tab>
            <Tab>
              <WizardTab_Code>Dates</WizardTab_Code>
              <IsCompleted>1</IsCompleted>
            </Tab>
            <Tab>
              <WizardTab_Code>Products</WizardTab_Code>
              <IsCompleted>1</IsCompleted>
            </Tab>
            <Tab>
              <WizardTab_Code>Attributes</WizardTab_Code>
              <IsCompleted>1</IsCompleted>
            </Tab>
            <Tab>
              <WizardTab_Code>Volumes</WizardTab_Code>
              <IsCompleted>1</IsCompleted>
            </Tab>
            <Tab>
              <WizardTab_Code>Financials</WizardTab_Code>
              <IsCompleted>1</IsCompleted>
            </Tab>
          </WizardPages>
        </Results>";
    }
}
