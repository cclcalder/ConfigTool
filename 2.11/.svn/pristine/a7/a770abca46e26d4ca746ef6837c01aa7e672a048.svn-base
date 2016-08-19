 
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using Exceedra.Common;
using Exceedra.Controls.DynamicGrid.Models;
using Exceedra.Controls.DynamicGrid.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GemBox.Spreadsheet;

namespace WPF.Test.ExcelToXML
{
    /// <summary>
    /// Summary description for ToGrid
    /// </summary>
    [TestClass]
    public class ToGrid
    {
        [TestMethod]
        public void TestMethod1()
        {
            var path = @"C:\Users\craig\Desktop\novamex.xls";

            SpreadsheetInfo.SetLicense("ERDC-FN4O-YKYN-4DBM");
            var workbook = ExcelFile.Load(path);

            // Select active worksheet from the file.
            var worksheet = workbook.Worksheets["Sheet1"];

            var records = new List<Record>();

            var workingRows = worksheet.Rows.Skip(2).Take(60);
            var excelRows = workingRows as ExcelRow[] ?? workingRows.ToArray();

            var header = excelRows[0];

//< ColumnCode > Promo_Idx </ ColumnCode >
//< HeaderText > Promo_Idx[H] </ HeaderText >
//< Value > 101 </ Value >
//< Format />
//< ForeColour />
//< BorderColour />
//< IsDisplayed > 0 </ IsDisplayed >
//< IsEditable > 0 </ IsEditable >
//< ControlType > Label </ ControlType >
//< DataSource />
//< DependentColumn />
//< TotalsAggregationMethod > NONE </ TotalsAggregationMethod >
//< ExternalData />
//< ColumnSortOrder > 20 </ ColumnSortOrder >

            var rcount = 0;
            foreach (var rows in excelRows.Skip(1))
            {
                var record = new Record();
                record.Item_Name = "Row" + rcount.ToString();
                record.Item_Idx = rcount.ToString();
                record.Item_IsDisplayed = true;
                var props = new List<Property>();
                var i = 0;
                foreach (var cell in rows.Cells.Take(7))
                {
                    var bg = "#ffffff";
                    var c = cell.Style.FillPattern.PatternBackgroundColor;
                    if(!c.Name.StartsWith("ffff"))
                    {
                        bg = "#cccccc";
                    }

                    var align = "Left";
                    if (cell.Style.HorizontalAlignment == HorizontalAlignmentStyle.Right)
                    {
                        align = "Right";
                    }
                   

                    props.Add(new Property()
                    {
                        ColumnCode="Cell" + i.ToString(),
                        ColumnSortOrder = i,
                        HeaderText = header.Cells[i].Value.ToString(),
                        Value = cell.Value != null ? cell.GetFormattedValue() : "",
                        ControlType = "Label",
                        StringFormat = "",
                        IsDisplayed = true,
                        BackgroundColour = bg,
                        BorderColour = bg,
                        Alignment = align

                    });
                    i += 1;
                }

                record.Properties = new ObservableCollection<Property>(props);
                records.Add(record);
                rcount += 1;
            }

            var rvm = new RecordViewModel();
            rvm.Records = new ObservableCollection<Record>(records);

            var ouput = rvm.ToWebServiceXml();
        }

         
    }

    public static class x
    {
        public static XDocument ToXmlDocument(this DataTable dataTable, string rootName)
        {
            var XmlDocument = new XDocument
            {
                Declaration = new XDeclaration("1.0", "utf-8", "")
            };
            XmlDocument.Add(new XElement(rootName));
            foreach (DataRow row in dataTable.Rows)
            {
                XElement element = null;
                if (dataTable.TableName != null)
                {
                    element = new XElement(dataTable.TableName);
                }
                foreach (DataColumn column in dataTable.Columns)
                {
                    element.Add(new
                XElement(column.ColumnName, row[column].ToString().Trim(' ')));
                }
                if (XmlDocument.Root != null) XmlDocument.Root.Add(element);
            }

            return XmlDocument;
        }
    }
}
