using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Exceedra.Common.Utilities;
using Model.DataAccess;

namespace Model.Entity.CellsGrid
{
    public class CellsGridAccessor
    {
        private const string GetInsightGridString = @"<{0}><User_Idx>{1}</User_Idx><SalesOrg_Idx>{2}</SalesOrg_Idx><MenuItem_Idx>{3}</MenuItem_Idx><Screen_Code>{4}</Screen_Code></{0}>";
        private const string GetInsightControlsDataString = @"<{0}><User_Idx>{1}</User_Idx><SalesOrg_Idx>{2}</SalesOrg_Idx><MenuItem_Idx>{3}</MenuItem_Idx><Canvas_Element_Idx>{4}</Canvas_Element_Idx>{5}</{0}>";

        public Task<CellsGridModel> GetInsightGrid(string userId, int salesOrgId, string menuItemId, string screenCode = null)
        {
            const string getGrid = "GetInsightGrid";
            string arguments = GetInsightGridString.FormatWith(getGrid, userId, salesOrgId, menuItemId, screenCode);

            return WebServiceProxy.CallAsync(StoredProcedure.Insights.GetGridItemList, XElement.Parse(arguments))
                .ContinueWith(t => GetInsightGridContinuation(t));
        }

        public XElement GetInsightControlData(string insightId, XElement filters, InsightControl insightControl)
        {
            const string getGrid = "GetCanvasElement";
            string arguments = GetInsightControlsDataString.FormatWith(getGrid, User.CurrentUser.ID, User.CurrentUser.SalesOrganisationID, insightId, insightControl.Id, ConvertDataSourceInput(insightControl.DataSourceInput, filters));
            return WebServiceProxy.Call(insightControl.DataSource, arguments, DisplayErrors.No);
        }

        private CellsGridModel GetInsightGridContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return null;

            int noHorizontalCells = 10;
            int noVerticalCells = 10;

            var noHorizontalCellsXml = task.Result.Element("Number_Of_Horizontal_Cells");
            if (noHorizontalCellsXml != null)
                noHorizontalCells = int.Parse(noHorizontalCellsXml.Value);

            var noVerticalCellsXml = task.Result.Element("Number_Of_Vertical_Cells");
            if (noVerticalCellsXml != null)
                noVerticalCells = int.Parse(noVerticalCellsXml.Value);

            var gridControlsXml = task.Result.Elements("GridControl");
            var insightControls = gridControlsXml.Select(n => new InsightControl(n)).ToObservableCollection();

            return new CellsGridModel
            {
                NoHorizontalCells = noHorizontalCells,
                NoVerticalCells = noVerticalCells,
                InsightControls = insightControls.ToList()
            };
        }

        private XElement ConvertDataSourceInput(string dataSourceInput, XElement filters)
        {
            if (string.IsNullOrEmpty(dataSourceInput)) return filters;

            var ripProperty = XElement.Parse(dataSourceInput.Replace("&gt;", ">").Replace("&lt;", "<"));
            List<XElement> attributes = filters.Elements("RootItem").Elements("Attributes").Elements("Attribute").ToList();

            foreach (var node in ripProperty.Elements())
            {
                switch (node.Name.ToString())
                {
                    case "SalesOrg":
                    case "SalesOrg_Idx":
                        node.Value = User.CurrentUser.SalesOrganisationID.ToString();
                        break;

                    case "User_Idx":
                        node.Value = User.CurrentUser.ID;
                        break;

                    case "ControlsWithInput":

                        var columns = node.Elements("Columns").Elements("Column");

                        foreach (var c in columns)
                        {
                            var xElement = c.Element("ColumnCode");
                            if (xElement != null)
                            {
                                var code = xElement.Value;

                                if (!string.IsNullOrEmpty(code))
                                {
                                    // get property based on column code
                                    var column = attributes.FirstOrDefault(t =>
                                    {
                                        var xColumnCode = t.Element("ColumnCode");
                                        return xColumnCode != null && xColumnCode.Value == code;
                                    });
                                    if (column == null) continue;

                                    var element = c.Element("Values");
                                    if (element == null) continue;

                                    // find the selected items
                                    var columnValues = column.Element("Values");
                                    if (columnValues != null)
                                        foreach (var i in columnValues.Elements("Value"))
                                            element.Add(i);

                                    // if no selected items were found
                                    // check if there is any data in Value property
                                    if (!element.HasElements && column.Element("Value") != null)
                                        element.Add(column.Element("Value"));
                                }
                            }
                        }
                        break;
                }
            }
            return ripProperty;
        }
    }
}
