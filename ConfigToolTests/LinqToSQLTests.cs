using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConfigTool;

namespace ConfigTool.Tests
{
    [TestClass()]
    public class LinqToSQLTests
    {
        [TestMethod()]
        public void tableAccessTest()
        {
            LinqToSQL.tableAccess();
        }
    }
}