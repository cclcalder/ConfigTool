using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigTool;

namespace ConfigToolTests
{
    [TestClass()]
    public class ParserTests
    {
        [TestMethod()]
        public void test1()
        {
            var json = ParseWizard.TextToJson();
        }
    }
}
