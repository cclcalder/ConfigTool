using GemBox.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;

using Model;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Runtime.ExceptionServices;
using Exceedra.Controls.DynamicGrid.Controls;
using Exceedra.Controls.DynamicGrid.ViewModels;
using Exceedra.Pivot.Controls;
using Exceedra.Schedule.Controls;
using Exceedra.Schedule.ViewModels;
using Telerik.Windows.Controls.Pivot.Export;
using Exceedra.TreeGrid.Models;
using Exceedra.Controls.DynamicGrid.Models;
using Exceedra.TreeGrid.Controls;
using Model.Entity.Calendar;
using Model.Entity.Listings;

namespace Exceedra.Documents.Excel
{

    public class ExcelOutput
    {
        private static List<Period> _periods;

        public static void SaveToExcel(string path, List<DynamicGridControl> grids, List<Image> charts,
            List<ExceedraRadPivotGrid> pivots, List<ScheduleControl> schedules,
            List<TreeGrid.Controls.TreeGrid> treeGrids, List<Period> Periods = null)
        {
            SpreadsheetInfo.SetLicense("ERDC-FN4O-YKYN-4DBM");
            _periods = Periods;
            // Create new empty Excel file.
            var ef = new ExcelFile();

            AddCanvasGridData(ef, grids);
            AddCanvasImageData(ef, charts);
            AddCanvasPivotData(ef, pivots);
            AddCanvasScheduleData(ef, schedules);
            AddTreeGridData(ef, treeGrids);

            ef.Save(path);
        }

        private static void AddCanvasScheduleData(ExcelFile ef, List<ScheduleControl> schedules)
        {
            if (schedules == null) return;
            foreach (var s in schedules)
            {
                var schedule = s.ScheduleSource;
                if (schedule.TimelineItems == null || !schedule.TimelineItems.Any()) continue;
                ForeEachSchedule(ef, schedule);
            }

        }

        private static void AddCanvasPivotData(ExcelFile ef, List<ExceedraRadPivotGrid> pivots)
        {
            if (pivots == null) return;
            foreach (var pivot in pivots)
            {
                var exportModel = pivot.GetPivotGrid();
                ForEachPivotGrid(ef, exportModel);
            }
        }

        private static void AddCanvasImageData(ExcelFile ef, List<Image> images)
        {
            if (images == null) return;
            int chartCounter = 0;
            foreach (var image in images)
            {
                var tabName = GetExcelWorksheetValidTabName(image.Tag.ToString(), ef.Worksheets);

                ExcelWorksheet ws = ef.Worksheets.Add(tabName);

                var ms = new MemoryStream();
                image.Save(ms, ImageFormat.Png);

                ms.Position = 0;

                var col = ws.Columns[0];
                ws.Columns[0].Width = image.Width;
                var row = ws.Rows[0];
                ws.Rows[0].Height = image.Height;

                var a = new AnchorCell(col, row, true);
                ws.Pictures.Add(ms, ExcelPictureFormat.Png, a, Convert.ToDouble(image.Width),
                    Convert.ToDouble(image.Height), LengthUnit.Pixel);
            }
        }

        private static void AddCanvasGridData(ExcelFile ef, List<DynamicGridControl> grids)
        {
            if (grids == null) return;
            foreach (var r in grids)
            {
                var grid = r.ItemDataSource as RecordViewModel;
                if (grid.Records == null || !grid.Records.Any()) continue;
                ForEachGrid(ef, grid);
            }
        }

        private static void AddTreeGridData(ExcelFile ef, List<TreeGrid.Controls.TreeGrid> grids)
        {
            if (grids == null) return;
            foreach (
                var grid in
                    grids.SelectMany(g => g.DataSource.VisibleNodes).SelectMany(g => TreeViewHierarchy.GetFlatTree(g)))
            {
                if (grid == null || grid.Data == null || grid.Data.Records == null || !grid.Data.Records.Any())
                    continue;
                ForEachTreeGrid(ef, grid);
            }

        }

        private static void ForEachGrid(ExcelFile ef, RecordViewModel row)
        {
            // Create new worksheet in the excel
            var tabName = GetExcelWorksheetValidTabName(row.GridTitle, ef.Worksheets);
            ExcelWorksheet ws = ef.Worksheets.Add(tabName);

            // report header
            ws.Rows[0].Cells[0].Value = row.GridTitle;
            ws.Rows[0].Cells[0].Style.Font.Size = 18 * 20;
            ws.Rows[0].Cells[0].Style.Font.Weight = ExcelFont.BoldWeight;

            // sub details
            ws.Rows[1].Cells[0].Value = "Created on: " + DateTime.Now.ToString();
            ws.Rows[1].Cells[0].Style.Font.Size = 9 * 20;
            ws.Rows[2].Cells[0].Value = "By: " + User.CurrentUser.DisplayName;
            ws.Rows[2].Cells[0].Style.Font.Size = 9 * 20;
            ws.Rows[3].Cells[0].Value = "From: " + "ExceedraSP";
            ws.Rows[3].Cells[0].Style.Font.Size = 9 * 20;


            var startRow = 5;

            var visibleColumns = row.Records[0].Properties.Where(v => v.IsDisplayed);

            // set up grid headers
            var cc = 0;
            foreach (var property in visibleColumns)
            {
                var cell = ws.Rows[startRow].Cells[cc];
                cell.Value = property.HeaderText;
                cell.Style.Font.Weight = ExcelFont.BoldWeight;
                cell.Style.Font.Color = System.Drawing.Color.Black;

                //headerStyle.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                //headerStyle.Interior.Pattern = Excel.XlPattern.xlPatternSolid;

                cc += 1;
            }

            //start the magic
            startRow += 1;

            // syscle each row of data
            foreach (var p in row.Records)
            {
                cc = 0;
                //cycle each property in 
                foreach (var property in p.Properties.Where(v => v.IsDisplayed))
                {
                    // get the cell we are in
                    var cell = ws.Rows[startRow].Cells[cc];

                    SetCellFromProperty(property, cell);

                    cc += 1;
                }
                startRow += 1;
            }

        }

        private static string ConvertFormat(string stringFormat, string formatType = null)
        {
            string format = "#,0";
            if (stringFormat.Length == 2)
            {
                var decimalPoints = Convert.ToInt32(stringFormat.Substring(1, 1));
                if (decimalPoints > 0)
                {
                    format += ".";
                    for (int i = 0; i < decimalPoints; i++)
                        format += "0";
                }
            }

            if (formatType != null)
            {
                switch (formatType)
                {
                    case "currency":
                        format = CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol + format;
                        break;
                    case "percent":
                        format += " %";
                        break;
                }
            }

            return format;
        }

        private static void ForEachPivotGrid(ExcelFile ef, PivotExportModel pivotGrid)
        {
            // Create new worksheet in the excel
            var tabName = GetExcelWorksheetValidTabName("Pivot Grid", ef.Worksheets);
            ExcelWorksheet ws = ef.Worksheets.Add(tabName);

            // report header
            ws.Rows[0].Cells[0].Value = "Pivot Grid";
            ws.Rows[0].Cells[0].Style.Font.Size = 18 * 20;
            ws.Rows[0].Cells[0].Style.Font.Weight = ExcelFont.BoldWeight;

            // sub details
            ws.Rows[1].Cells[0].Value = "Created on: " + DateTime.Now;
            ws.Rows[1].Cells[0].Style.Font.Size = 9 * 20;
            ws.Rows[2].Cells[0].Value = "By: " + User.CurrentUser.DisplayName;
            ws.Rows[2].Cells[0].Style.Font.Size = 9 * 20;
            ws.Rows[3].Cells[0].Value = "From: " + "ExceedraSP";
            ws.Rows[3].Cells[0].Style.Font.Size = 9 * 20;


            var startRow = 5;

            foreach (var cellInfo in pivotGrid.Cells)
            {
                int rowStartIndex = startRow + cellInfo.Row;
                int columnStartIndex = cellInfo.Column;

                var cell = ws.Rows[rowStartIndex].Cells[columnStartIndex];

                var value = cellInfo.Value;
                if (value != null)
                {
                    decimal d;
                    if (decimal.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.CurrentCulture, out d))
                    {
                        cell.Value = d;
                        if (value.ToString().Contains(CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol))
                            cell.Style.NumberFormat = CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol + "#,#";
                        else if (value.ToString().Contains("%"))
                            cell.Style.NumberFormat = "#,#%";
                    }
                    else
                        cell.Value = value.ToString();


                    cell.Style.VerticalAlignment = VerticalAlignmentStyle.Center;
                    int indent = cellInfo.Indent;
                    if (indent > 0)
                    {
                        cell.Style.Indent = indent;
                    }
                }
            }
        }

        private static void ForEachTreeGrid(ExcelFile ef, TreeGridNode row)
        {
            var mainGrid = row.Data;
            var measures = row.Measures.Records.Select(r => r.Properties.First()).ToArray();
            var nodeTitle = row.Name;

            // Create new worksheet in the excel
            var tabName = GetExcelWorksheetValidTabName("Timeline Week view", ef.Worksheets);
            ExcelWorksheet ws = ef.Worksheets.Add(tabName);

            // report header
            ws.Rows[0].Cells[0].Value = nodeTitle;
            ws.Rows[0].Cells[0].Style.Font.Size = 18 * 20;
            ws.Rows[0].Cells[0].Style.Font.Weight = ExcelFont.BoldWeight;

            // sub details
            ws.Rows[1].Cells[0].Value = "Created on: " + DateTime.Now.ToString();
            ws.Rows[1].Cells[0].Style.Font.Size = 9 * 20;
            ws.Rows[2].Cells[0].Value = "By: " + User.CurrentUser.DisplayName;
            ws.Rows[2].Cells[0].Style.Font.Size = 9 * 20;
            ws.Rows[3].Cells[0].Value = "From: " + "ExceedraSP";
            ws.Rows[3].Cells[0].Style.Font.Size = 9 * 20;


            var startRow = 5;

            var visibleColumns =
                row.Measures.Records[0].Properties.Concat(mainGrid.Records[0].Properties.Where(v => v.IsDisplayed));

            // set up grid headers
            var cc = 0;
            foreach (var property in visibleColumns)
            {
                var cell = ws.Rows[startRow].Cells[cc];
                cell.Value = property.HeaderText;
                cell.Style.Font.Weight = ExcelFont.BoldWeight;
                cell.Style.Font.Color = System.Drawing.Color.Black;

                //headerStyle.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                //headerStyle.Interior.Pattern = Excel.XlPattern.xlPatternSolid;

                cc += 1;
            }

            //start the magic
            startRow += 1;

            // syscle each row of data
            // We must add the measure value to each row, hence why we use a for instead of foreach
            for (int i = 0; i < mainGrid.Records.Count(); i++)
            {
                cc = 0;

                //Add the measure before adding the main row data
                var thisMeasure = measures[i];
                var cell = ws.Rows[startRow].Cells[cc];
                SetCellFromProperty(thisMeasure, cell);

                cc += 1;

                //cycle each property in 
                foreach (var property in mainGrid.Records[i].Properties.Where(v => v.IsDisplayed))
                {
                    // get the cell we are in
                    cell = ws.Rows[startRow].Cells[cc];

                    SetCellFromProperty(property, cell);

                    cc += 1;
                }
                startRow += 1;
            }

        }

        private static void SetCellFromProperty(Property property, ExcelCell cell)
        {
            decimal d;

            if (property.StringFormat == null)
            {
                cell.Value = property.Value;
            }
            // if its a number format, align center
            else if (property.StringFormat.ToLower().StartsWith("n"))
            {
                decimal.TryParse(property.Value, NumberStyles.Any, CultureInfo.CurrentCulture, out d);
                cell.Value = d;
                cell.Style.VerticalAlignment = VerticalAlignmentStyle.Center;
            }
            else if (property.StringFormat.ToLower().StartsWith("c"))
            {
                decimal.TryParse(property.Value, NumberStyles.Currency, CultureInfo.CurrentCulture, out d);
                cell.Value = d;
                cell.Style.NumberFormat = ConvertFormat(property.StringFormat, "currency");
                cell.Style.VerticalAlignment = VerticalAlignmentStyle.Center;
            }
            else if (property.StringFormat.ToLower().StartsWith("p"))
            {
                var v = property.Value.Replace("%", "").Trim();
                decimal.TryParse(v, NumberStyles.Any, CultureInfo.CurrentCulture, out d);
                cell.Value = d / 100;
                cell.Style.NumberFormat = ConvertFormat(property.StringFormat, "percent");
                cell.Style.VerticalAlignment = VerticalAlignmentStyle.Center;
            }
            else
            {
                cell.Value = property.Value;
            }
        }


        #region Schedule Workers

        private static void ForeEachSchedule(ExcelFile ef, ScheduleViewModel schedule)
        {
            AddPromoListData(ef, schedule);

            AddTimeLineSheet(ef, schedule);

            AddTimeLineWeeklySheet(ef, schedule);
        }

        private static void AddTimeLineWeeklySheet(ExcelFile ef, ScheduleViewModel schedule)
        {
            //new worksheet for gantt - and I use gantt in the loosest form of the word
            ExcelWorksheet tws = ef.Worksheets.Add("Timeline Week view");
            tws.Rows[0].Cells[0].Value = schedule.Title + " Timeline Week View";
            tws.Rows[0].Cells[0].Style.Font.Size = 18 * 20;
            tws.Rows[0].Cells[0].Style.Font.Weight = ExcelFont.BoldWeight;

            tws.Rows[1].Cells[0].Value = "Created on: " + DateTime.Now;
            tws.Rows[1].Cells[0].Style.Font.Size = 9 * 20;
            tws.Rows[2].Cells[0].Value = "By: " + User.CurrentUser.DisplayName;
            tws.Rows[2].Cells[0].Style.Font.Size = 9 * 20;
            tws.Rows[3].Cells[0].Value = "From: " + "ExceedraSP";
            tws.Rows[3].Cells[0].Style.Font.Size = 9 * 20;


            //generate gant- for the love of god this is going to get hairy
            var startRow = 5;

            //customer name columns is wider          
            //ws.Columns[startRow].Width = 0 * 256;
            tws.Rows[startRow].Cells[0].Value = "Customer";
            tws.Rows[startRow].Style.Font.Weight = ExcelFont.BoldWeight;

            //set bounds of grid to be built
            var minDate = schedule.TimelineItems.Min(d => d.StartDate);
            var maxDate = schedule.TimelineItems.Max(d => d.EndDate);

            //create column headers, one for each 'day' of the range  
            var currentDay = 1;
            foreach (DateTime day in EachDay(minDate, maxDate))
            {
                if (day.DayOfWeek == DayOfWeek.Monday)
                {

                    tws.Rows[startRow].Cells[currentDay].Value = day.Date.ToShortDateString();
                    tws.Rows[startRow].Cells[currentDay].Style.Rotation = 90;
                    tws.Rows[startRow].Cells[currentDay].Style.Font.Weight = ExcelFont.BoldWeight;
                    tws.Columns[currentDay].Width = 4 * 256;

                }
                else
                {
                    tws.Columns[currentDay].Width = 1 * 256;
                }

                GetMergedHeaderRow(day, currentDay, tws, startRow);

                currentDay += 1;
            }


            // ready to add the real data? - woohoo
            startRow += 1;

            foreach (var p in schedule.TimelineItems.OrderBy(t => t.Category).ThenBy(t => t.Idx))
            {
                tws.Rows[startRow].Height = 2 * 256;
                tws.Rows[startRow].Cells[0].Value = p.TooltipContent1;


                //build start points a ranges for each date range
                var start = p.StartDate;
                var duration = p.Duration.Days;

                // first colum stores customer name
                var startCell = (start - minDate).Days + 1;

                //highlight cells for promo
                for (int i = 0; i < duration; i++)
                {
                    var statusBgrd = ColorTranslator.FromHtml(p.Colour.ToString());
                    // where are we really starting
                    var col = startCell + i;

                    tws.Rows[startRow].Cells[col].Style.Borders.SetBorders(MultipleBorders.Top, statusBgrd,
                        LineStyle.Medium);
                    tws.Rows[startRow].Cells[col].Style.Borders.SetBorders(MultipleBorders.Bottom, statusBgrd,
                        LineStyle.Medium);

                    //set end borders if we are starting/ending the row
                    if (i == 0)
                    {
                        tws.Rows[startRow].Cells[col].Style.Borders.SetBorders(MultipleBorders.Left, statusBgrd,
                            LineStyle.Thick);
                        tws.Rows[startRow].Cells[col + 1].Value = p.Name + " - Start:" +
                                                                  p.StartDate.ToShortDateString() + "  End:" +
                                                                  p.EndDate.ToShortDateString();
                        tws.Rows[startRow].Cells[0].Value = p.TooltipContent1;
                        tws.Rows[startRow].Cells[col].Style.FillPattern.SetSolid(statusBgrd);
                    }

                    if (i == (duration - 1))
                        tws.Rows[startRow].Cells[col].Style.Borders.SetBorders(MultipleBorders.Right, statusBgrd,
                            LineStyle.Medium);

                    // tws.Rows[startRow].Cells[col].Style.FillPattern.SetSolid(statusBgrd);
                    tws.Rows[startRow].Cells[col].Style.VerticalAlignment = VerticalAlignmentStyle.Center;
                }

                startRow += 1;
            }
        }

        private static void GetMergedHeaderRow(DateTime day, int currentDay, ExcelWorksheet tws, int startRow)
        {
            if (_periods != null)
            {
                var sd = day.Date.ToShortDateString();
                Period startperiod;

                if (currentDay == 1)
                {
                    startperiod =
                        _periods.FirstOrDefault(t => day.Date >= t.StartDate && day.Date < t.EndDate);
                }
                else
                {
                    startperiod = _periods.FirstOrDefault(t => t.StartDate.ToShortDateString() == sd);
                }
                var duration = 0;

                if (startperiod != null)
                {
                    duration = (startperiod.EndDate - startperiod.StartDate).Days;
                    var ofset = (day - startperiod.StartDate).Days;
                    duration -= ofset;

                    try
                    {
                        CellRange mergedRange = tws.Cells.GetSubrangeAbsolute(startRow - 1, currentDay, startRow - 1,
                            currentDay + duration);
                        mergedRange.Merged = true;
                        mergedRange.Value = startperiod.Name;
                        mergedRange.Style = MergedCellStyle;
                    }
                    catch
                    {
                    }
                }
            }
        }

        private static void AddPromoListData(ExcelFile ef, ScheduleViewModel schedule)
        {
            // Create new worksheet
            var tabName = GetExcelWorksheetValidTabName(schedule.Title, ef.Worksheets);
            ExcelWorksheet ws = ef.Worksheets.Add(tabName);

            ws.Rows[0].Cells[0].Value = schedule.Title;
            ws.Rows[0].Cells[0].Style.Font.Size = 18 * 20;
            ws.Rows[0].Cells[0].Style.Font.Weight = ExcelFont.BoldWeight;

            //sub details
            ws.Rows[1].Cells[0].Value = "Created on: " + DateTime.Now;
            ws.Rows[1].Cells[0].Style.Font.Size = 9 * 20;
            ws.Rows[2].Cells[0].Value = "By: " + User.CurrentUser.DisplayName;
            ws.Rows[2].Cells[0].Style.Font.Size = 9 * 20;
            ws.Rows[3].Cells[0].Value = "From: " + "ExceedraSP";
            ws.Rows[3].Cells[0].Style.Font.Size = 9 * 20;

            var startRow = 5;

            ws.Rows[startRow].Cells[0].Value = "Customer";
            ws.Rows[startRow].Style.Font.Weight = ExcelFont.BoldWeight;
            ws.Columns[0].Width = 40 * 256;

            // build table headers
            ws.Rows[startRow].Cells[1].Value = "Name";
            ws.Columns[1].Width = 50 * 256;

            ws.Rows[startRow].Cells[2].Value = "Start Date";
            ws.Columns[2].Width = 10 * 256;

            ws.Rows[startRow].Cells[3].Value = "End Date";
            ws.Columns[3].Width = 10 * 256;

            ws.Rows[startRow].Cells[4].Value = "Status";
            ws.Columns[4].Width = 15 * 256;

            startRow += 1;

            foreach (var p in schedule.TimelineItems.OrderBy(t => t.Category).ThenBy(t => t.Idx))
            {
                ws.Rows[startRow].Cells[0].Value = p.TooltipContent1;
                ws.Rows[startRow].Height = 2 * 256;

                ws.Rows[startRow].Cells[1].Value = p.Name;
                ws.Rows[startRow].Cells[2].Value = p.StartDate.ToShortDateString();
                ws.Rows[startRow].Cells[3].Value = p.EndDate.ToShortDateString();
                ws.Rows[startRow].Cells[4].Value = p.Status;
                var statusBgrd = ColorTranslator.FromHtml(p.Colour.ToString());
                ws.Rows[startRow].Cells[4].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                ws.Rows[startRow].Cells[4].Style.Borders.SetBorders(MultipleBorders.Outside, statusBgrd,
                    LineStyle.Medium);
                ws.Rows[startRow].Cells[4].Style.Borders.SetBorders(MultipleBorders.Left, statusBgrd,
                    LineStyle.Thick);

                if (_periods != null)
                {

                    var startperiod = _periods.FirstOrDefault(t => p.StartDate.Date >= t.StartDate && p.StartDate.Date < t.EndDate);
                    if (startperiod != null)
                    {
                        if (p.StartDate >= startperiod.StartDate && p.StartDate < startperiod.EndDate)
                        {
                            ws.Rows[startRow].Cells[5].Value = startperiod.Name;
                        }
                    }
                }

                startRow += 1;
            }

            foreach (var r in ws.Rows.Where(r => r.Index > 4))
            {
                r.Style.VerticalAlignment = VerticalAlignmentStyle.Center;
            }

        }

        private static void AddTimeLineSheet(ExcelFile ef, ScheduleViewModel schedule)
        {
            //new worksheet for gantt - and I use gantt in the loosest form of the word
            var tabName = GetExcelWorksheetValidTabName(schedule.Title + " Timeline", ef.Worksheets);
            ExcelWorksheet tws = ef.Worksheets.Add(tabName);

            tws.Rows[0].Cells[0].Value = schedule.Title + " Timeline";
            tws.Rows[0].Cells[0].Style.Font.Size = 18 * 20;
            tws.Rows[0].Cells[0].Style.Font.Weight = ExcelFont.BoldWeight;

            tws.Rows[1].Cells[0].Value = "Created on: " + DateTime.Now;
            tws.Rows[1].Cells[0].Style.Font.Size = 9 * 20;
            tws.Rows[2].Cells[0].Value = "By: " + User.CurrentUser.DisplayName;
            tws.Rows[2].Cells[0].Style.Font.Size = 9 * 20;
            tws.Rows[3].Cells[0].Value = "From: " + "ExceedraSP";
            tws.Rows[3].Cells[0].Style.Font.Size = 9 * 20;

            //generate gant- for the love of god this is going to get hairy
            var startRow = 5;

            //customer name columns is wider          
            //ws.Columns[startRow].Width = 0 * 256;
            tws.Rows[startRow].Cells[0].Value = "Customer";
            tws.Rows[startRow].Style.Font.Weight = ExcelFont.BoldWeight;

            //set bounds of grid to be built
            var minDate = schedule.TimelineItems.Min(t => t.StartDate);
            var maxDate = schedule.TimelineItems.Max(t => t.EndDate);

            //create column headers, one for each 'day' of the range  
            var currentDay = 1;
            foreach (DateTime day in EachDay(minDate, maxDate))
            {
                var sd = day.Date.ToShortDateString();
                GetMergedHeaderRow(day, currentDay, tws, startRow);

                //if (_periods != null)
                //{

                //    var startperiod = _periods.FirstOrDefault(t => t.StartDate.ToShortDateString() == sd);
                //    var duration = 0;
                //    if (startperiod != null)
                //    {
                //        duration = (startperiod.EndDate - startperiod.StartDate).Days;

                //        CellRange mergedRange = tws.Cells.GetSubrangeAbsolute(startRow - 1, currentDay, startRow - 1,
                //            currentDay + duration);
                //        mergedRange.Merged = true;
                //        mergedRange.Value = startperiod.Name;
                //        mergedRange.Style = MergedCellStyle;
                //    }

                //}

                tws.Rows[startRow].Cells[currentDay].Value = sd;
                tws.Rows[startRow].Cells[currentDay].Style.Rotation = 90;
                tws.Rows[startRow].Cells[currentDay].Style.Font.Weight = ExcelFont.BoldWeight;
                tws.Columns[currentDay].Width = 4 * 256;

                currentDay += 1;
            }

            // ready to add the real data? - woohoo
            startRow += 1;

            foreach (var p in schedule.TimelineItems.OrderBy(t => t.Category).ThenBy(t => t.Idx))
            {
                tws.Rows[startRow].Height = 2 * 256;
                tws.Rows[startRow].Cells[0].Value = p.TooltipContent1;


                //build start points a ranges for each date range
                var start = p.StartDate;
                var duration = p.Duration.Days;

                // first colum stores customer name
                var startCell = (start - minDate).Days + 1;

                //highlight cells for promo
                for (int i = 0; i < duration; i++)
                {
                    var statusBgrd = ColorTranslator.FromHtml(p.Colour.ToString());
                    // where are we really starting
                    var col = startCell + i;

                    tws.Rows[startRow].Cells[col].Style.Borders.SetBorders(MultipleBorders.Top, statusBgrd,
                        LineStyle.Medium);
                    tws.Rows[startRow].Cells[col].Style.Borders.SetBorders(MultipleBorders.Bottom, statusBgrd,
                        LineStyle.Medium);

                    //set end borders if we are starting/ending the row
                    if (i == 0)
                    {
                        tws.Rows[startRow].Cells[col].Style.Borders.SetBorders(MultipleBorders.Left, statusBgrd,
                            LineStyle.Thick);
                        tws.Rows[startRow].Cells[col + 1].Value = p.Name + " - Start:" +
                                                                  p.StartDate.ToShortDateString() + "  End:" +
                                                                  p.EndDate.ToShortDateString();
                        tws.Rows[startRow].Cells[0].Value = p.TooltipContent1;
                        tws.Rows[startRow].Cells[col].Style.FillPattern.SetSolid(statusBgrd);
                    }

                    if (i == (duration - 1))
                        tws.Rows[startRow].Cells[col].Style.Borders.SetBorders(MultipleBorders.Right, statusBgrd,
                            LineStyle.Medium);

                    // tws.Rows[startRow].Cells[col].Style.FillPattern.SetSolid(statusBgrd);
                    tws.Rows[startRow].Cells[col].Style.VerticalAlignment = VerticalAlignmentStyle.Center;
                }

                startRow += 1;
            }

        }

        public static IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }

        #endregion

        private static CellStyle MergedCellStyle
        {
            get
            {
                CellStyle tmpStyle = new CellStyle();
                tmpStyle.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                tmpStyle.VerticalAlignment = VerticalAlignmentStyle.Center;
                tmpStyle.FillPattern.SetSolid(Color.LightGray);
                tmpStyle.Font.Weight = ExcelFont.BoldWeight;
                tmpStyle.Font.Color = Color.White;
                tmpStyle.WrapText = true;
                tmpStyle.Borders.SetBorders(MultipleBorders.Right | MultipleBorders.Top, Color.Black, LineStyle.Thin);

                return tmpStyle;
            }
        }

        private static string GetExcelWorksheetValidTabName(string tabName, ExcelWorksheetCollection worksheets)
        {
            // And excel tab can't be more than 31 characters long
            // Also, there can't be two tabs of the same name (case insensitive)

            if (string.IsNullOrEmpty(tabName))
                tabName = "Sheet1";

            var validTabName = tabName.Replace(@"\", "and").Replace(@"/", "and");

            if (tabName.Length > 31)
                validTabName = validTabName.Substring(0, 31);

            int i = 2;
            while (worksheets.Any(
                worksheet =>
                    string.Equals(worksheet.Name, validTabName, StringComparison.CurrentCultureIgnoreCase)))
            {
                // Adds a number at the end of the name
                if (validTabName.Length <= 30 - i.ToString().Length)
                    validTabName += i;
                else validTabName = validTabName.Substring(0, validTabName.Length - i.ToString().Length) + i;

                i++;
            }

            return validTabName;
        }
    }

}