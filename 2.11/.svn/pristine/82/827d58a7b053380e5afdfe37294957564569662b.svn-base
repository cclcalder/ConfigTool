using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using Exceedra.Chart.ViewModels;
using Exceedra.Common;
using Exceedra.Common.Logging;
using Exceedra.Common.Utilities;
using Exceedra.Controls.Helpers;
using Exceedra.LabelControl;
using Exceedra.Pivot.ViewModels;
using Model;
using Model.Entity.CellsGrid;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace Exceedra.CellsGrid
{
    public class CellsGridViewModel : ViewModelBase
    {
        #region ctor

        // constructing only through CellsGridViewModel GetEmptyCellsGrid(string userId, int salesOrgId, string menuItemId)
        private CellsGridViewModel() { }

        #endregion

        #region properties

        private static readonly CellsGridAccessor Accessor = new CellsGridAccessor();

        private int _noHorizontalCells;
        public int NoHorizontalCells
        {
            get { return _noHorizontalCells; }
            set
            {
                _noHorizontalCells = value;
                OnPropertyChanged("NoHorizontalCells");
            }
        }

        private int _noVerticalCells;
        public int NoVerticalCells
        {
            get { return _noVerticalCells; }
            set
            {
                _noVerticalCells = value;
                OnPropertyChanged("NoVerticalCells");
            }
        }

        private ObservableCollection<InsightControl> _controlsCollection;
        public ObservableCollection<InsightControl> ControlsCollection
        {
            get { return _controlsCollection ?? (_controlsCollection = new ObservableCollection<InsightControl>()); }
            set
            {
                _controlsCollection = value;
                OnPropertyChanged("ControlsCollection");
            }
        }

        #endregion

        #region methods

        public static CellsGridViewModel GetEmptyCellsGrid(string userId, int salesOrgId, string menuItemId, string screenCode = null)
        {
            CellsGridModel model = Accessor.GetInsightGrid(userId, salesOrgId, menuItemId, screenCode).Result;

            if (model == null)
            {
                return null;
            }

            return new CellsGridViewModel
            {
                NoHorizontalCells = model.NoHorizontalCells,
                NoVerticalCells = model.NoVerticalCells,
                ControlsCollection = model.InsightControls.ToObservableCollection()
            };
        }

        /// <summary>
        /// Loads the content of all controls.
        /// </summary>
        /// <param name="controlOwnerId">Sometimes the content varies from its owner (i.e. from menu id in canvas) so it is required for the owner id to be passed</param>
        /// <param name="args">An additional xml (i.e. filters in canvas)</param>
        public void LoadControlsData(string controlOwnerId, XElement args)
        {
            var cellOccupied = new InsightControl[NoHorizontalCells, NoVerticalCells];
            List<string> overlappedControlsCodes = new List<string>();
            List<string> outsideCanvasAreaControlsCodes = new List<string>();

            foreach (var control in ControlsCollection)
            {
                var insightControl = control;

                #region checking overlapping elements

                int x1 = int.Parse(insightControl.ColumnIndex);
                int y1 = int.Parse(insightControl.RowIndex);
                int x2 = x1 + int.Parse(insightControl.ColumnSpan) - 1;
                int y2 = y1 + int.Parse(insightControl.RowSpan) - 1;

                for (int i = x1; i <= x2; i++)
                    for (int j = y1; j <= y2; j++)
                    {
                        try
                        {
                            if (cellOccupied[i, j] != null)
                            {
                                if (!overlappedControlsCodes.Contains(insightControl.Code))
                                {
                                    StorageBase.LogMessageToFile("Error", "Canvas", "The control of code \"" + insightControl.Code + "\" is placed (partly) on another control of code \"" + cellOccupied[i, j].Code + "\"." +
                                        "\n\"" + insightControl.Code + "\" control: " +
                                        insightControl.ColumnIndex + "-" + insightControl.ColumnLastIndex + "x" +
                                        insightControl.RowIndex + "-" + insightControl.RowLastIndex +
                                        "\n\"" + cellOccupied[i, j].Code + "\" control: " +
                                        cellOccupied[i, j].ColumnIndex + "-" + cellOccupied[i, j].ColumnLastIndex + "x" +
                                        cellOccupied[i, j].RowIndex + "-" + cellOccupied[i, j].RowLastIndex
                                        , User.CurrentUser.ID);

                                    overlappedControlsCodes.Add(insightControl.Code);
                                }

                                break;
                            }
                            cellOccupied[i, j] = insightControl;
                        }
                        catch (IndexOutOfRangeException ioore)
                        {
                            if (!outsideCanvasAreaControlsCodes.Contains(insightControl.Code))
                            {
                                StorageBase.LogMessageToFile("Error", "Canvas", "The control of code \"" + insightControl.Code + "\" is placed in a cell / cells outside of the canvas area." +
                                    "\nCanvas area: 0-" + (NoHorizontalCells - 1) + "x0-" + (NoVerticalCells - 1) +
                                    "\n\"" + insightControl.Code + "\" control: " +
                                    insightControl.ColumnIndex + "-" + insightControl.ColumnLastIndex + "x" +
                                    insightControl.RowIndex + "-" + insightControl.RowLastIndex
                                    , User.CurrentUser.ID);

                                outsideCanvasAreaControlsCodes.Add(insightControl.Code);
                            }

                            break;
                        }
                    }

                #endregion

                new Task(() =>
                {
                    XElement controlData;

                    try
                    {
                        controlData = insightControl.DataSource != null
                            ? Accessor.GetInsightControlData(controlOwnerId, args, insightControl)
                            : null;

                    }
                    // proc returns empty xml
                    catch (ExceedraDataException)
                    {
                        ReportError(insightControl,
                                                    "The control of code \"" + insightControl.Code + "\" called \"" + insightControl.DataSource +
                                                    "\" but it returned no results.");
                        return;
                    }
                    // proc returns someting but it's different from <Results>...</Results>
                    catch (XmlException)
                    {
                        ReportError(insightControl,
                            "The control of code \"" + insightControl.Code + "\" called \"" + insightControl.DataSource +
                            "\" but it returned xml without <Results> as the root node.");

                        return;
                    }

                    if (controlData == null && insightControl.LinkTo == null) return;

                    #region checking controls' source xmls

                    if (controlData != null)
                    {
                        string xmlType;

                        if (controlData.Element("ChartType") != null ||
                            controlData.Element("xAxisType") != null)
                            xmlType = "Chart";
                        else if (controlData.Element("Grid_Title") != null ||
                            controlData.Elements("RootItem").Any())
                            xmlType = "DynamicGrid";
                        else if (controlData.Element("FieldProperties") != null ||
                                 controlData.Element("Fields") != null)
                            xmlType = "PivotGrid";
                        else if (controlData.Element("Error") != null)
                            xmlType = "Error";
                        else xmlType = "NotRecognised";

                        if (xmlType == "NotRecognised")
                        {
                            ReportError(insightControl,
                                "The control of code \"" + insightControl.Code + "\" called \"" + insightControl.DataSource +
                                "\" but it returned the xml below that the app does not understand:" + "\n\n" + controlData);
                            return;
                        }

                        if (xmlType == "Error")
                        {
                            ReportError(insightControl,
                                "The control of code \"" + insightControl.Code + "\" called \"" + insightControl.DataSource +
                                "\" but it returned the error below:" + "\n\n" + controlData);
                            return;
                        }

                        var isXmlMatchingControlType = string.Equals(insightControl.ControlType, xmlType, StringComparison.CurrentCultureIgnoreCase);
                        if (!isXmlMatchingControlType)
                        {
                            var errorMessage =
                                "Unable to match the xml with data for the control with the control type." +
                                "\n\nControl code: " + insightControl.Code +
                                "\nControl type: " + insightControl.ControlType +
                                "\nXml looks like for: " + xmlType;

                            StorageBase.LogMessageToFile("Error", "Canvas", errorMessage, User.CurrentUser.ID);
                        }
                    }

                    #endregion

                    switch (insightControl.ControlType)
                    {
                        case "Label":
                            {
                                insightControl.DataSourceViewModel = new LabelViewModel { Text = insightControl.Name };
                                break;
                            }
                        case "Navigation":
                            {
                                insightControl.DataSourceViewModel = new NavigationViewModel(insightControl.LinkTo) ;
                                break;
                            }
                        case "Chart":
                            {
                                insightControl.DataSourceViewModel = new RecordViewModel(controlData);
                                break;
                            }
                        case "DynamicGrid":
                            {
                                insightControl.DataSourceViewModel = new Controls.DynamicGrid.ViewModels.RecordViewModel(controlData);
                                break;
                            }
                        case "PivotGrid":
                            {
                                insightControl.DataSourceViewModel = ExceedraRadPivotGridViewModel.LoadWithData(controlData);
                                break;
                            }
                    }

                    var canvasErrorReportable = insightControl.DataSourceViewModel as ICanvasErrorReportable;
                    if (canvasErrorReportable != null) canvasErrorReportable.ErrorReported += ReportError;

                }).Start();
            }
        }

        private void ReportError(string errorText)
        {
            StorageBase.LogMessageToFile("Error", "Canvas", errorText, User.CurrentUser.ID);
        }
        private void ReportError(InsightControl insightControl, string errorText)
        {
            insightControl.ControlType = "Label";
            insightControl.DataSourceViewModel = LabelViewModel.NewCenterText("Unable to load the data for the control");

            StorageBase.LogMessageToFile("Error", "Canvas", errorText, User.CurrentUser.ID);
        }


        #endregion
    }

    public class NavigationViewModel
    {
        public string Item_Idx { get; set; }
        public string What { get; set; }

        public string Name { get; set; }

        public NavigationViewModel(XElement xml)
        {
            Item_Idx= xml.Element("Item_Idx").MaybeValue();
            What = xml.Element("What").MaybeValue();
            Name = xml.Element("Name").MaybeValue();
        }



    }
}
