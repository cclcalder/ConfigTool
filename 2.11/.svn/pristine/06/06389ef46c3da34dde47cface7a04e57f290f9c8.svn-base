using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;

namespace ExceedraDB.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void LoginUP()
        {
           var res = new Exceedra.DB.Composite.AppUser().Login("ch","1234");
           Assert.IsNotNull(res);            
        }

        private   XElement loginXML = XElement.Parse(@"
                                                      <User>
                                                        <User_Idx>11</User_Idx>
                                                        <Session_ID>23E87FFD-911C-4952-8426-ACC51AAED683</Session_ID>
                                                      </User>");


        [TestMethod]
        public void LoginX()
        {
            var res = new Exceedra.DB.Composite.AppUser().Login(loginXML);
            Assert.IsNotNull(res);
        }
    }
}
