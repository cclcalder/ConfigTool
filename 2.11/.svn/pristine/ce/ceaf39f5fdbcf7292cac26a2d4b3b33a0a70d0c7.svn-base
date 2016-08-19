
namespace Exceedra.Documents.PDF
{
    using System;
 
    using iTextSharp.text.pdf;
    using iTextSharp.text;
  
        public class TwoColumnHeaderFooter : PdfPageEventHelper
        {
            // This is the contentbyte object of the writer
            PdfContentByte cb;
            // we will put the final number of pages in a template
            PdfTemplate template;
            // this is the BaseFont we are going to use for the header / footer
            BaseFont bf = null;
            // This keeps track of the creation time
            DateTime PrintTime = DateTime.Now;
            #region Properties
            private string _Title;
            public string Title
            {
                get { return _Title; }
                set { _Title = value; }
            }

            private Image _Logo;
            public Image Logo
            {
                get { return _Logo; }
                set { _Logo = value; }
            }

            private string _HeaderLeft;
            public string HeaderLeft
            {
                get { return _HeaderLeft; }
                set { _HeaderLeft = value; }
            }
            private string _HeaderRight;
            public string HeaderRight
            {
                get { return _HeaderRight; }
                set { _HeaderRight = value; }
            }

            private string _Author;
            public string Author
            {
                get { return _Author; }
                set { _Author = value; }
            }

        private Font _HeaderFont;
            public Font HeaderFont
            {
                get { return _HeaderFont; }
                set { _HeaderFont = value; }
            }
            private Font _FooterFont;
            public Font FooterFont
            {
                get { return _FooterFont; }
                set { _FooterFont = value; }
            }
            #endregion
            // we override the onOpenDocument method
            public override void OnOpenDocument(PdfWriter writer, Document document)
            {
                try
                {
                    PrintTime = DateTime.Now;
                    bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                    cb = writer.DirectContent;
                    template = cb.CreateTemplate(50, 50);
                }
                catch (DocumentException de)
                {
                }
                catch (System.IO.IOException ioe)
                {
                }
            }

            public override void OnStartPage(PdfWriter writer, Document document)
            {
                base.OnStartPage(writer, document);
                Rectangle pageSize = document.PageSize;
                if (!string.IsNullOrEmpty(Title))
                {
                    cb.BeginText();
                    cb.SetFontAndSize(bf, 15);
                    //cb.SetRGBColorFill(50, 50, 200);
                    cb.SetTextMatrix(pageSize.GetLeft(10), pageSize.GetTop(30));

                    Logo.ScaleAbsolute(30,30);
                    Logo.SetAbsolutePosition(10, 550);
                    cb.AddImage(Logo);


                    cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, Title, 50, 560, 0);
                    cb.EndText();
                }
                //if (HeaderLeft + HeaderRight != string.Empty)
                //{
                //    PdfPTable HeaderTable = new PdfPTable(2);
                //    HeaderTable.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                //    HeaderTable.TotalWidth = pageSize.Width - 80;
                //    HeaderTable.SetWidthPercentage(new float[] { 45, 45 }, pageSize);

                //    PdfPCell HeaderLeftCell = new PdfPCell(new Phrase(8, HeaderLeft, HeaderFont));
                //    HeaderLeftCell.Padding = 5;
                //    HeaderLeftCell.PaddingBottom = 8;
                //    HeaderLeftCell.BorderWidthRight = 0;
                //    HeaderTable.AddCell(HeaderLeftCell);
                //    PdfPCell HeaderRightCell = new PdfPCell(new Phrase(8, HeaderRight));
                //    HeaderRightCell.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                //    HeaderRightCell.Padding = 5;
                //    HeaderRightCell.PaddingBottom = 8;
                //    HeaderRightCell.BorderWidthLeft = 0;
                //    HeaderTable.AddCell(HeaderRightCell);
                //    cb.SetRGBColorFill(0, 0, 0);
                //    HeaderTable.WriteSelectedRows(0, -1, pageSize.GetLeft(40), pageSize.GetTop(50), cb);
                //}
            }
            public override void OnEndPage(PdfWriter writer, Document document)
            {
                base.OnEndPage(writer, document);
                int pageN = writer.PageNumber;
                String text = "Page " + pageN + " of ";
                float len = bf.GetWidthPoint(text, 6);
                Rectangle pageSize = document.PageSize;
                cb.SetRGBColorFill(100, 100, 100);
                cb.BeginText();
                cb.SetFontAndSize(bf, 6);
                cb.SetTextMatrix(pageSize.GetLeft(40), pageSize.GetBottom(30));
                cb.ShowText(text);
                cb.EndText();
                cb.AddTemplate(template, pageSize.GetLeft(40) + len, pageSize.GetBottom(30));

                cb.BeginText();
                cb.SetFontAndSize(bf, 6);
                cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT,
                "Printed On " + PrintTime  + " by " + Author,
                pageSize.GetRight(40),
                pageSize.GetBottom(30), 0);
                cb.EndText();
            }
            public override void OnCloseDocument(PdfWriter writer, Document document)
            {
                base.OnCloseDocument(writer, document);
                template.BeginText();
                template.SetFontAndSize(bf,6);
                template.SetTextMatrix(0, 0);
                template.ShowText("" + ((writer.PageNumber == 1 ? 1 : writer.PageNumber - 1)));
                template.EndText();
            }
        }
 
    //public class PageEventHelper : PdfPageEventHelper
    //{
    //    PdfContentByte cb;
    //    PdfTemplate template;


    //    public override void OnOpenDocument(PdfWriter writer, Document document)
    //    {
    //        cb = writer.DirectContent;
    //        template = cb.CreateTemplate(50, 50);
    //    }

    //    public override void OnEndPage(PdfWriter writer, Document document)
    //    {
    //        base.OnEndPage(writer, document);

    //        int pageN = writer.PageNumber;
    //        String text = "Page " + pageN.ToString() + " of ";
    //        float len = this.BaseFont.GetWidthPoint(text, this.RunDateFont.Size);

    //        iTextSharp.text.Rectangle pageSize = document.PageSize;

    //        cb.SetRGBColorFill(100, 100, 100);

    //        cb.BeginText();
    //        cb.SetFontAndSize(this.RunDateFont.BaseFont, this.RunDateFont.Size);
    //        cb.SetTextMatrix(document.LeftMargin, pageSize.GetBottom(document.BottomMargin));
    //        cb.ShowText(text);

    //        cb.EndText();

    //        cb.AddTemplate(template, document.LeftMargin + len, pageSize.GetBottom(document.BottomMargin));
    //    }

    //    public override void OnCloseDocument(PdfWriter writer, Document document)
    //    {
    //        base.OnCloseDocument(writer, document);

    //        template.BeginText();
    //        template.SetFontAndSize(this.RunDateFont.BaseFont, this.RunDateFont.Size);
    //        template.SetTextMatrix(0, 0);
    //        template.ShowText("" + (writer.PageNumber - 1));
    //        template.EndText();
    //    }
    //}
    //public class PageNumbers
    //{
    //    public static byte[] AddPageNumbers(byte[] pdf)
    //    {
    //        MemoryStream ms = new MemoryStream();
    //        // we create a reader for a certain document
    //        PdfReader reader = new PdfReader(pdf);
    //        // we retrieve the total number of pages
    //        int n = reader.NumberOfPages;
    //        // we retrieve the size of the first page
    //        Rectangle psize = reader.GetPageSize(1);

    //        // step 1: creation of a document-object
    //        Document document = new Document(psize, 50, 50, 50, 50);
    //        // step 2: we create a writer that listens to the document
    //        PdfWriter writer = PdfWriter.GetInstance(document, ms);
    //        // step 3: we open the document

    //        document.Open();
    //        // step 4: we add content
    //        PdfContentByte cb = writer.DirectContent;

    //        int p = 0;
    //        Console.WriteLine("There are " + n + " pages in the document.");
    //        for (int page = 1; page <= reader.NumberOfPages; page++)
    //        {
    //            document.NewPage();
    //            p++;

    //            PdfImportedPage importedPage = writer.GetImportedPage(reader, page);
    //            cb.AddTemplate(importedPage, 0, 0);

    //            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
    //            cb.BeginText();
    //            cb.SetFontAndSize(bf, 10);
    //            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, +p + "/" + n, 7, 44, 0);
    //            cb.EndText();
    //        }
    //        // step 5: we close the document
    //        document.Close();
    //        return ms.ToArray();
    //    }
    //}
}
