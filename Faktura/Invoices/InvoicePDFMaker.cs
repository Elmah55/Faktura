using System;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Faktura.Companies;
using System.Collections.Generic;

namespace Faktura.Invoices
{
    /// <summary>
    /// This is a class for generating pdf file from invoice object
    /// </summary>
    class InvoicePDFMaker
    {
        //Consts
        #region
        //-------------------------------------------------------------------

        private const Font.FontFamily DefaultFont = Font.FontFamily.TIMES_ROMAN;

        private const UInt32 DefualtFontSize = 14; //Default font used for most text on invoice pdf
        private const UInt32 IssuerInfoFontSize = 10;
        private const UInt32 ItemsTableFontSize = 10; //Font size for elements in invoice item's table
        private const UInt32 FooterFontSize = 5;

        private const string ItemNumberColumnName = "Lp.";

        //-------------------------------------------------------------------
        #endregion

        //Fields
        #region

        //-------------------------------------------------------------------
        private PDFPageEvent PageEventHelper;

        //This is queue for storing content posistion for wirter
        //so that page count footer can be added after writer has
        //reached and of document and total number of pdf document pages
        //is known
        private Queue<PdfContentByte> PageCountFooterContents;

        //Invoice item's table related fields
        private int TableColumsCount;
        private float[] TableColumnsWidth;

        //-------------------------------------------------------------------
        #endregion

        public InvoicePDFMaker()
        {
            this.PageEventHelper = new PDFPageEvent();
            this.PageCountFooterContents = new Queue<PdfContentByte>();

            this.TableColumsCount = InvoiceItem.FieldsName.Length + 1; //One column for item number

            //Init columns width array
            this.TableColumnsWidth = new float[TableColumsCount];
            TableColumnsWidth[0] = 0.025f; //Item number
            TableColumnsWidth[1] = 0.1f; //Item name
            TableColumnsWidth[2] = 0.045f; //Vat rate
            TableColumnsWidth[3] = 0.04f; //Netto price
            TableColumnsWidth[4] = 0.04f; //Brutto price
            TableColumnsWidth[5] = 0.06f; //Netto value
            TableColumnsWidth[6] = 0.03f; //Quantity
            TableColumnsWidth[7] = 0.16f; //Comment
        }

        public void GenerateInvoicePDF(CompanySettings company, Invoice inv, string pdfFilePath)
        {
            if (null != company && null != inv && null != pdfFilePath)
            {
                FileStream fStream = CreatePdfFile(pdfFilePath);

                if (null != fStream) //Pdf file created successfully
                {
                    Document pdfDocument = new Document();
                    PdfWriter writer = PdfWriter.GetInstance(pdfDocument, fStream);

                    SetPageEvents(writer);
                    FillDocument(company, inv, pdfDocument, writer);

                    writer.Close();
                    writer.Dispose();
                    pdfDocument.Close();
                    pdfDocument.Dispose();
                }
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// Sets writer's events class and
        /// registers methods to the PageEventHelper class events
        /// </summary>
        private void SetPageEvents(PdfWriter writer)
        {
            if (null != writer && null != PageEventHelper)
            {
                writer.PageEvent = PageEventHelper;
                PageEventHelper.EndPage += AddPageCountFootersContents;
            }
        }

        private void AddPageCountFootersContents(Document document, PdfWriter writer)
        {
            if (null != writer && null != PageCountFooterContents)
            {
                PdfContentByte content = writer.DirectContent.Duplicate;
                PageCountFooterContents.Enqueue(content);
            }
        }

        /// <summary>
        /// Returns stream to file created at given path.
        /// </summary>
        /// <returns>On successful file creation stream to file. If creating file fails returns null</returns>
        private FileStream CreatePdfFile(string filePath)
        {
            FileStream fStream = null;

            if (null != filePath)
            {
                try
                {
                    fStream = new FileStream(filePath, FileMode.Create);
                }
                catch (Exception)
                {
                    fStream = null;
                }
            }

            return fStream;
        }

        private void FillDocument(CompanySettings company, Invoice inv, Document doc, PdfWriter writer)
        {
            if (null != company && null != inv && null != doc && null != writer)
            {
                const UInt32 newLinesAfterHeader = 3; //Numer of lines that should be added after document header

                doc.OpenDocument();

                AddInvoiceHeader(company, inv, doc);
                AddNewLines(newLinesAfterHeader, doc);
                AddInvoiceItems(inv, doc);
                AddPageCountFooters(doc, writer);

                doc.CloseDocument();
            }
        }

        /// <summary>
        /// Adds footer to every page containing information about current page number
        /// as well as count of all pages of pdf document. This method should be called after
        /// whole pdf document was filled so that total number of pages is known
        /// </summary>
        private void AddPageCountFooters(Document doc, PdfWriter writer)
        {
            if (null != doc && null != writer && null != PageCountFooterContents)
            {
                //Coordinates for pages count footer
                const int pageCountFooterX = 300;
                const int PageCountFooterY = 20;

                int totalPagesCount = writer.PageNumber; //Amount of all pages of pdf document
                UInt32 currentPageNumber = 1;

                while (0 != PageCountFooterContents.Count)
                {
                    string pagesCountStr = "Strona " + currentPageNumber + " z " + totalPagesCount;
                    Phrase pagesCountPhrase = new Phrase(pagesCountStr);
                    Font pagesCountFont = new Font(DefaultFont, FooterFontSize);
                    pagesCountPhrase.Font = pagesCountFont;

                    PdfContentByte content = PageCountFooterContents.Dequeue();
                    ColumnText.ShowTextAligned(content, Element.ALIGN_RIGHT,
                    pagesCountPhrase, pageCountFooterX, PageCountFooterY, 0);

                    ++currentPageNumber;
                }
            }
        }

        private void AddNewLines(UInt32 count, Document pdfDocument)
        {
            if (0 != count && null != pdfDocument)
            {
                for (int iterator = 0; iterator < count; iterator++)
                {
                    pdfDocument.Add(new Paragraph(Environment.NewLine));
                }
            }
        }

        //Invoice header related methods
        #region
        //-------------------------------------------------------------------

        /// <summary>
        /// Adds invoice header to pdf document
        /// </summary>
        private void AddInvoiceHeader(CompanySettings company, Invoice inv, Document doc)
        {
            if (null != company && null != inv && null != doc)
            {
                AddInvoiceNumber(inv, doc);
                AddIssuerInfo(company, doc);
            }
        }

        private void AddInvoiceNumber(Invoice inv, Document doc)
        {
            if (null != inv && null != doc)
            {
                string headerText = "Faktura VAT nr. " + inv.InvoiceNumber;
                Font invoiceNumberFont = new Font(DefaultFont, DefualtFontSize);
                Paragraph invoiceNumber = new Paragraph(headerText, invoiceNumberFont);
                invoiceNumber.Alignment = Element.ALIGN_CENTER;
                doc.Add(invoiceNumber);
            }
        }

        private void AddIssuerInfo(CompanySettings company, Document doc)
        {
            if (null != company && null != doc)
            {
                const int postalCode1EndIndex = 2;
                const int postalCode2EndIndex = 3;

                string issuerInfoText = "Wystawiający:\n"
                  + company.CompanyName + "\n"
                  + company.Street + " / " + company.HouseNumber + "\n"
                  + company.PostalCode.ToString().Substring(0, postalCode1EndIndex) + "-"
                  + company.PostalCode.ToString().Substring(postalCode1EndIndex, postalCode2EndIndex)
                  + " " + company.City;

                Font issuerInfoFont = new Font(DefaultFont, IssuerInfoFontSize);
                Paragraph issuerInfo = new Paragraph(issuerInfoText, issuerInfoFont);
                issuerInfo.Alignment = Element.ALIGN_RIGHT;
                doc.Add(issuerInfo);
            }
        }

        //-------------------------------------------------------------------
        #endregion

        //Invoice items table related methods
        #region
        //-------------------------------------------------------------------

        /// <summary>
        /// Adds table with invoice items to the invoice pdf document
        /// </summary>
        private void AddInvoiceItems(Invoice inv, Document doc)
        {
            if (null != inv && null != doc && 0 != inv.Items.Count)
            {
                PdfPTable table = new PdfPTable(TableColumsCount);
                Font tableElementsFont = new Font(DefaultFont, ItemsTableFontSize);

                InitItemsTable(table, tableElementsFont);
                AddItemsToTable(inv, table, tableElementsFont);

                doc.Add(table);
            }
        }

        private void InitItemsTable(PdfPTable table, Font tableElementsFont)
        {
            if (null != table && null != tableElementsFont)
            {
                table.SetTotalWidth(TableColumnsWidth);

                //Add columns names
                table.AddCell(new PdfPCell(new Phrase(ItemNumberColumnName, tableElementsFont)));

                foreach (string columnName in InvoiceItem.FieldsName)
                {
                    table.AddCell(new PdfPCell(new Phrase(columnName, tableElementsFont)));
                }
            }
        }

        private void AddItemsToTable(Invoice inv, PdfPTable table, Font tableElementsFont)
        {
            if (null != inv && null != table && null != tableElementsFont)
            {
                UInt32 itemNumber = 1;

                foreach (InvoiceItem item in inv.Items)
                {
                    table.AddCell(new PdfPCell(new Phrase(itemNumber.ToString(), tableElementsFont)));
                    table.AddCell(new PdfPCell(new Phrase(item.Name, tableElementsFont)));
                    table.AddCell(new PdfPCell(new Phrase(item.VATRate.ToString(), tableElementsFont)));
                    table.AddCell(new PdfPCell(new Phrase(item.NettoPrice.ToString(), tableElementsFont)));
                    table.AddCell(new PdfPCell(new Phrase(item.BruttoPrice.ToString(), tableElementsFont)));
                    table.AddCell(new PdfPCell(new Phrase(item.NettoValue.ToString(), tableElementsFont)));
                    table.AddCell(new PdfPCell(new Phrase(item.Count.ToString(), tableElementsFont)));
                    table.AddCell(new PdfPCell(new Phrase(item.Comment, tableElementsFont)));

                    ++itemNumber;
                }
            }
        }

        //-------------------------------------------------------------------
        #endregion
    }
}