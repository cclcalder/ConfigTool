using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Exceedra.Controls.DynamicGrid.ViewModels;
using Exceedra.Controls.DynamicRow.Models;
using Exceedra.PDF;
using GemBox.Spreadsheet;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Model.Entity.CellsGrid;

namespace Exceedra.Documents.PDF
{
    public class PdfDocument  
    {
 
        private Font HeaderFont
        {
            get
            {
                return new Font(Font.FontFamily.HELVETICA, 9f, Font.BOLD, BaseColor.BLACK);
            }
        }

        private Font LabelFont(string colour)
        {
                return new Font(Font.FontFamily.HELVETICA, 8f, Font.NORMAL, new BaseColor(System.Drawing.ColorTranslator.FromHtml(colour)));
        }

        public PdfDocument(string path, string name, string author, object source, DocumentType type)
        {
            Name = name;
            Path = path;
            Author = author;
            DataSource = source;
            DocType = type;
        }
        public void SavePDFDocument(string imagePath)
        {
            using (iTextSharp.text.Document doc = new Document(new RectangleReadOnly(842, 595), 10f, 10f, 60f, 40f))
            { 
                PdfWriter wri = PdfWriter.GetInstance(doc, new FileStream(Path, FileMode.Create));

                TwoColumnHeaderFooter pageEventHandler = new TwoColumnHeaderFooter();
                wri.PageEvent = pageEventHandler;

                pageEventHandler.Title = Name;
                pageEventHandler.Author = Author;
                pageEventHandler.Logo = Image.GetInstance(imagePath);

                doc.Open(); 

                CreatePdf(doc);               
                 
                doc.Close();
            }
        }

 

        public DocumentType DocType { get; set; }
        public string Path { get; set; }
              
        public string Name { get; set; }
        public object DataSource { get; set; }
        public string Author { get; set; }



        public Document CreatePdf(Document doc1)
        { 
        
            switch (DocType)
            {
                case DocumentType.Canvas:
                    CreateCanvasPDF((List<InsightControl>) DataSource, doc1);
                    break;

                case DocumentType.Chart:

                    break;
                case DocumentType.DynamicGrid:
                    CreateGridPDF((RecordViewModel) DataSource, doc1);
                    break;

                case DocumentType.DynamicGrids:
                    CreateGridPDF((List<RecordViewModel>)DataSource, doc1);
                    break;

                default:

                    break;
            }

     
            return doc1;
        }

        private void CreateGridPDF(List<RecordViewModel> dataSource, Document doc)
        {
            foreach (var r in dataSource)
            {
                doc.Add(CreateGridPage(r));
            }
            
        }

        private void CreateGridPDF(RecordViewModel dataSource, Document doc)
        { 
                doc.Add(CreateGridPage(dataSource));     
        }

        public void CreateCanvasPDF(List<InsightControl> ControlsCollection, Document doc)
        {
            var items = ControlsCollection.OrderBy(t => t.ColumnIndex).ThenBy(t=> t.RowIndex).ToList();
             
            foreach (var p in items)
            {
               doc.Add(CreateCanvasPage(p));
            }

        }

        private IElement CreateCanvasPage(InsightControl insightControl)
        {
         
            switch (insightControl.ControlType)
            {
                case "DynamicGrid":
                    return CreateTable((RecordViewModel) insightControl.DataSourceViewModel);

                case "Chart":
                    return null;

                    break;

                default:
                    return null;
            }

        }

        private IElement CreateGridPage(RecordViewModel rvm)
        { 
            return CreateTable(rvm);
        }

        private PdfPTable CreateTable(RecordViewModel rvm)
        {
            var props = rvm.Records[0].Properties.Where(t=>t.IsDisplayed).ToList();
            PdfPTable table = new PdfPTable(props.Count());
            table.WidthPercentage = 100f;

            PdfPCell cell = new PdfPCell(new Phrase(rvm.GridTitle));
            
            cell.Colspan = props.Count;
            cell.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
            table.AddCell(cell);
             
            //header cell
            foreach (var property in props)
            {
                PdfPCell c = new PdfPCell(new Phrase(property.HeaderText, HeaderFont));
                c.HorizontalAlignment = 0;
                c.BackgroundColor = BaseColor.LIGHT_GRAY;                 
                table.AddCell(c);
            }

            foreach (var row in rvm.Records)
            {
                foreach (var col in row.Properties.Where(t => t.IsDisplayed))
                {
                    PdfPCell c = new PdfPCell(new Phrase(col.Value, LabelFont(col.ForeColour)));

                    c.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml(col.BackgroundColour));

                    if (col.BorderColour.ToLower() != "#ffffff")
                    {
                     
                        //c.Border = Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER;

                        //c.Border = Rectangle.LEFT_BORDER;
                        c.BorderWidth = 2;

                        c.BorderColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml(col.BorderColour));
                    }

                    if (col.StringFormat.ToLower().Contains("c"))
                    {
                        c.HorizontalAlignment = 2;
                    }
                    else
                    {
                        c.HorizontalAlignment = 0;
                    }

                    table.AddCell(c);
                }
            }


            return table;
        }

    }

    public enum DocumentType
    {
        Canvas=0,
        DynamicGrid = 1,
        DynamicGrids = 2,
        Chart =3
    }
}
