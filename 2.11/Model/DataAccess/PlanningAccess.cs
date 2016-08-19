using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Exceedra.Common;
using Exceedra.Common.Xml;
using Model.DataAccess.Converters;
using Model.DataAccess.Generic;
using Model.Entity.Generic;

namespace Model.DataAccess
{
    public static class PlanningAccess
    {
        internal static readonly List<PlanningItem> PlanningProductsCache = new List<PlanningItem>();
        internal static readonly List<Measure> PlanningMeasuresCache = new List<Measure>();

        /// <summary>
        /// Extracts and creates product list returned from web service XML data
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<PlanningItem> GetPlanningProducts()
        {
            if (PlanningProductsCache.Count == 0)
            {
                string arguments = null;

                arguments = "<Products><User_Idx>{0}</User_Idx></Products>".FormatWith(User.CurrentUser.ID);

                var productsNodes = WebServiceProxy.Call(StoredProcedure.GetPlanningProducts, XElement.Parse(arguments)).Elements();

                PlanningProductsCache.Clear();
                PlanningProductsCache.AddRange(productsNodes.Select(p => new PlanningItem(p)));
            }

            return PlanningProductsCache;
        }


        /// <summary>
        /// Extracts and creates planning interval list returned from web service XML data
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ComboboxItem> GetPlanningIntervals()
        {
            var arguments = "<GetPlanningDateIntervals><UserID>{0}</UserID></GetPlanningDateIntervals>".FormatWith(User.CurrentUser.ID);

            return DynamicDataAccess.GetGenericEnumerable<ComboboxItem>(StoredProcedure.GetPlanningDateIntervals, XElement.Parse(arguments));
        }

        public static XElement GetPlanningDataInputXml(PlanningFilterArgDTO filter)
        {
            XElement argument = CommonXml.GetBaseArguments("GetData");
            argument.Add(new XElement("Scenario_Idx", filter.SelectedScenarioIdx));

            argument.Add(InputConverter.ToProducts(filter.ProductIDs));
            argument.Add(InputConverter.ToCustomers(filter.CustomerIDs));
            argument.Add(InputConverter.ToIdxList("Measures", filter.MeasureIDs));

            if (filter.SelectedTimeRangeIdx != string.Empty)
                argument.AddElement("TimeRange_Idx", filter.SelectedTimeRangeIdx);
            else
            {
                argument.AddElement("StartDate", filter.StartDate.Date.ToString("yyyy-MM-dd"));
                argument.AddElement("EndDate", filter.EndDate.Date.ToString("yyyy-MM-dd"));
            }

            argument.AddElement("Interval_Idx", filter.IntervalIdx);

            var chartArgs = new XElement("IsSelectedInChart");

            if (filter.SelectedCharts != null)
            {
                foreach (var m in filter.SelectedCharts)
                {
                    var series = new XElement("Series");
                    series.Add(new XElement("Item_Idx", m.ProductID));
                    series.Add(new XElement("Measure_Idx", m.MeasureID));

                    chartArgs.Add(series);
                }
            }

            argument.Add(chartArgs);

            return argument;
        }


        /// <summary>
        /// Saves user preferences for planning
        /// </summary>
        /// <returns></returns>
        public static bool SaveUserPrefsPlanning(PlanningPreferenceDTO preferenceToSave)
        {
            var argument = new XElement("SaveDefaults");
            argument.Add(new XElement("User_Idx", User.CurrentUser.ID));
            argument.Add(new XElement(XMLNode.Nodes.Screen_Code.ToString(), "Planning"));
            argument.Add(new XElement("SalesOrg_Idx", User.CurrentUser.SalesOrganisationID));

            argument.Add(new XElement("ListingsGroup_Idx", preferenceToSave.ListingsGroupIdx));

            argument.Add(new XElement("Customers"));
            if (preferenceToSave.Customers != null)
            {
                foreach (var cId in preferenceToSave.Customers)
                { 
                    argument.Element("Customers").Add(new XElement("Idx", cId));
                }
            }

            argument.Add(new XElement("Interval"));
            var intvl = new XElement("ID", preferenceToSave.IntervalId);
            argument.Element("Interval").Add(intvl);    
            
            argument.Add(new XElement("Hierarchy_Idx", preferenceToSave.HierarchyIdx));
            argument.Add(new XElement("Scen_Idx", preferenceToSave.ScenarioID));
            argument.Add(new XElement("TimeRangeID", preferenceToSave.TimeRangeID));
            
            argument.Add(new XElement("Products"));
            if (preferenceToSave.Products != null)
            {
                foreach (var pId in preferenceToSave.Products)
                { 
                    argument.Element("Products").Add(new XElement("Idx", pId));
                }
            }

            argument.Add(new XElement("Measures"));
            if (preferenceToSave.Measures != null)
            {
                foreach (var pId in preferenceToSave.Measures)
                {
                    var meas = new XElement("Measure");
                    meas.Add(new XElement("ID", pId));
                    argument.Element("Measures").Add(meas);
                }
            }

            argument.Add(new XElement("Dates"));
            argument.Element("Dates").Add(new XElement("Start", preferenceToSave.DateStart));
            argument.Element("Dates").Add(new XElement("End", preferenceToSave.DateEnd));
             

            return MessageConverter.DisplayMessage(WebServiceProxy.Call(StoredProcedure.SaveUserPrefsPlanning, argument).Elements().FirstOrDefault());


        }

        /// <summary>
        /// Returns list of predefined planning date ranges
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<PlanningTimeRange> GetPlanningTimeRanges()
        {
            string arguments = "<GetPlanningTimeRanges><UserID>{0}</UserID></GetPlanningTimeRanges>".FormatWith(User.CurrentUser.ID);

            return DynamicDataAccess.GetGenericEnumerable<PlanningTimeRange>(StoredProcedure.GetPlanningTimeRanges,
                XElement.Parse(arguments));
        }


        /// <summary>
        /// Returns List of Planning Scenarios from Database
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ComboboxItem> GetPlanningScenarios()
        {
            string arguments = "<GetPlanningScenarios><User_Idx>{0}</User_Idx></GetPlanningScenarios>".FormatWith(User.CurrentUser.ID);

            return DynamicDataAccess.GetGenericEnumerable<ComboboxItem>(StoredProcedure.GetPlanningScenarios,
                XElement.Parse(arguments));
        }
    }
}