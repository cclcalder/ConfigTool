using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Net;

namespace Model.DataAccess
{
    public static class InsightReportsAccess
    {
        public static IEnumerable<Group> GetInsightGroups()
        {
            string arguments = "<GetInsightReports><UserID>{0}</UserID></GetInsightReports>".FormatWith(User.CurrentUser.ID);

            var groupNodes = WebServiceProxy.Call(StoredProcedure.GetInsightReports, XElement.Parse(arguments)).Elements();

            return from node in groupNodes
                   select new Group
                   {
                       Name = node.GetValue<string>("Name"),
                       Reports = (from reportNode in node.Element("Reports").Elements()
                                  select new Report
                                  {
                                      Name = reportNode.GetValue<string>("Name"),
                                      Url = reportNode.GetValue<string>("URL"),
                                      IsDefault = reportNode.GetValue<int>("IsDefault") == 1 ? true : false
                                  }).ToList()
                   };
        }
    }
}
