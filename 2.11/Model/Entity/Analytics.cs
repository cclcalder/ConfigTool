using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Animation;
using System.Xml.Linq;
using Exceedra.Common;

namespace Model
{
    public class AnalyticsGroup
    {
        //<Results>
        //  <AnalyticsReports>
        //    <Report_Name>Test</Report_Name>
        //  </AnalyticsReports>
        //</Results>

        public int ReportID { get; set; }
        public string ReportCube { get; set; }
        public string ReportName { get; set; }
        public string ReportCatalog { get; set; }

        public int GroupID { get; set; }
        public List<AnalyticReport> Reports { get; set; }

        public AnalyticsGroup()
        {

        }
        /// <summary>
        /// Creates a new Product instance.
        /// </summary>


        public static AnalyticsGroup FromXml(XElement element)
        {

            const string name = "Name";
            const string id = "Analytics_Report_Idx";
            const string cube = "Cube";
            const string catalog = "Catalog";
            const string groupId ="AnalyticsReportGroup_Idx";

            var reports = new List<AnalyticReport>();

            if (element.Element("Reports") != null)
            {
                var el = element.Element("Reports");
                if (el.Element("Report") != null)
                {
                    var nodes = el.Elements("Report");

                    reports.AddRange(nodes.Select(n => new AnalyticReport(n)));

                }
            }



            return new AnalyticsGroup
            {
                ReportID = element.GetValue<int>(id),
                ReportCube = element.GetValue<string>(cube),
                ReportName = element.GetValue<string>(name),
                ReportCatalog = element.GetValue<string>(catalog),
                GroupID = element.GetValue<int>(groupId),
                Reports = reports
            };
        }
    }

    public class AnalyticReport
    {
        public AnalyticReport()
        {
        }

        public string ID { get; set; }
        public string GroupIdx { get; set; }
        public string Name { get; set; }
        public string Cube { get; set; }
        public string Catalog { get; set; }
        public string LayoutXML { get; set; }

        public bool IsDefault { get; set; }

        private List<string> _selectedUsers;
        public List<string> SelectedUsers
        {
            get
            {
                return _selectedUsers;
            }
            set
            {
                if (_selectedUsers != value)
                    _selectedUsers = value.ToList();
            }
        }

        public bool IsColumnGrandTotalChecked { get; set; }
        public bool IsColumnSubTotalChecked { get; set; }
        public bool IsRowGrandTotalChecked { get; set; }
        public bool IsRowSubTotalChecked { get; set; }

        public bool IsReadOnlyChecked { get; set; }
        public bool IsReadOnlyForUser { get; set; }

        //public static int counter = 0;

        public AnalyticReport(XElement el)
        {
            ID = el.GetValue<string>("Idx");
            Cube = el.GetValue<string>("Group_Cube");
            GroupIdx = el.GetValue<string>("Group_Idx");
            Catalog = el.GetValue<string>("Group_Catalog");
            Name = el.GetElement("Report_LayoutXML").GetElement("DataServiceProvider").GetValue<string>("ReportName");
            LayoutXML = el.GetElement("Report_LayoutXML").GetElement("DataServiceProvider").ToString();
            IsDefault = el.GetValue<int>("IsDefault") == 1 ? true : false;
            IsReadOnlyForUser = el.GetValue<string>("IsReadOnlyForUser") == "1";
            IsReadOnlyChecked = IsReadOnlyForUser || (el.GetValue<string>("IsReadOnlyChecked") == "1");

            //IsReadOnlyChecked = (counter % 2) == 0;
            //IsReadOnlyForUser = IsReadOnlyChecked && (counter % 4) == 0;
            //counter++;

            if (el.Element("Report_Users") != null)
            {
                var u = new List<string>();

                foreach (var user in el.Elements("Report_Users").Elements("User_Idx"))
                {
                    u.Add(user.Value);
                }

                SelectedUsers = u;
            }

            var xTotalSettings = el.Element("Report_TotalsSettings");
            if (xTotalSettings != null)
            {
                IsColumnGrandTotalChecked = xTotalSettings.Element("ColumnGrandTotal").IsTrue();
                IsColumnSubTotalChecked = xTotalSettings.Element("ColumnSubTotal").IsTrue();
                IsRowGrandTotalChecked = xTotalSettings.Element("RowGrandTotal").IsTrue();
                IsRowSubTotalChecked = xTotalSettings.Element("RowSubTotal").IsTrue();
            }
        }
    }

    public class AnalyticsCube
    {
        //<Results>
        //  <AnalyticsReports>
        //    <Report_Name>Test</Report_Name>
        //  </AnalyticsReports>
        //</Results>

        public string Name { get; set; }
        public string Code { get; set; }
        public string Catalog { get; set; }
        public string Cube { get; set; }

        public int GroupID{get;set;}

        public AnalyticsCube()
        {

        }




        public static AnalyticsCube FromXml(XElement element)
        {

            const string nameElement = "AnalyticsReportGroup_Name";
            const string codeElement = "AnalyticsReportGroup_Code";
            const string cubeElement = "AnalyticsReportGroup_Cube";
            const string catalogElement = "AnalyticsReportGroup_Catalog";


            return new AnalyticsCube
            {
                Name = element.GetValue<string>(nameElement),
                Code = element.GetValue<string>(codeElement),
                Cube = element.GetValue<string>(cubeElement),
                Catalog = element.GetValue<string>(catalogElement)
            };
        }

        public AnalyticsCube(string name, string code, string cube, string catalog, int groupID )
        {
            this.Name = name;
            this.Code = code;
            this.Cube = cube;
            this.Catalog = catalog;
            this.GroupID = groupID;
        }

    }
}
