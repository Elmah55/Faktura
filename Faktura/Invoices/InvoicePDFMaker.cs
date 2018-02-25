using System;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Faktura.Companies;
using System.Collections.Generic;
using iTextSharp.text.pdf.draw;

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

        //Fonts types
        //-------------------------------------------------------------------
        private const Font.FontFamily DefaultFontFamily = Font.FontFamily.TIMES_ROMAN;
        //-------------------------------------------------------------------

        //Fonts sizes
        //-------------------------------------------------------------------
        //Default font used for most text on invoice pdf
        private const UInt32 DefualtFontSize = 14;
        private const UInt32 IssuerInfoFontSize = 10;
        //Font size for elements in invoice item's table
        private const UInt32 ItemsTableFontSize = 10;
        private const UInt32 FooterFontSize = 5;
        private const UInt32 SignatureTemplateFontSize = 1;
        //-------------------------------------------------------------------

        //New lines spacing
        //-------------------------------------------------------------------
        private const UInt32 NewLinesAfterHeader = 3;
        private const UInt32 NewLinesBeforeSignatureTemplate = 2;
        //Lines separating signature's template text and designated place for signature
        private const UInt32 NewLinesAfterSignatureStr = 3;
        //-------------------------------------------------------------------

        //Misc
        //-------------------------------------------------------------------
        private const string ItemNumberColumnName = "Lp.";
        //-------------------------------------------------------------------

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
                    pdfDocument.Open();

                    SetPageEvents(writer);
                    FillDocument(company, inv, pdfDocument, writer);

                    pdfDocument.Close();
                    pdfDocument.Dispose();
                    fStream.Close();
                    fStream.Dispose();
                    writer.Close();
                    writer.Dispose();
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
            if (null != company && null != inv && null != doc && null != writer && doc.IsOpen())
            {
                AddInvoiceHeader(company, inv, doc);
                AddNewLines(NewLinesAfterHeader, doc);
                AddInvoiceItems(inv, doc);
                AddSummaryInfo(inv, doc);
                AddNewLines(NewLinesBeforeSignatureTemplate, doc);
                AddSignatureTemplates(doc);
                AddPageCountFooters(doc, writer);
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
                Font invoiceNumberFont = new Font(DefaultFontFamily, DefualtFontSize);
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

                Font issuerInfoFont = new Font(DefaultFontFamily, IssuerInfoFontSize);
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
                Font tableElementsFont = new Font(DefaultFontFamily, ItemsTableFontSize);

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

        //Other pdf document's content related methods
        #region
        //-------------------------------------------------------------------

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
                    Font pagesCountFont = new Font(DefaultFontFamily, FooterFontSize);
                    pagesCountPhrase.Font = pagesCountFont;

                    PdfContentByte content = PageCountFooterContents.Dequeue();
                    ColumnText.ShowTextAligned(content, Element.ALIGN_RIGHT,
                    pagesCountPhrase, pageCountFooterX, PageCountFooterY, 0);

                    ++currentPageNumber;
                }
            }
        }

        private void AddSummaryInfo(Invoice inv, Document doc)
        {
            if (null != doc && null != inv)
            {
                Font summaryInfoFont = new Font(DefaultFontFamily, DefualtFontSize);

                string totalBruttoPriceStr = "Razem brutto: " + inv.BruttoValue;
                Paragraph totalBruttoPrice = new Paragraph(totalBruttoPriceStr, summaryInfoFont);
                totalBruttoPrice.Alignment = Element.ALIGN_LEFT;

                string totalNettoPriceStr = "Razem netto: " + inv.NettoValue;
                Paragraph totalNettoPrice = new Paragraph(totalNettoPriceStr, summaryInfoFont);
                totalNettoPrice.Alignment = Element.ALIGN_LEFT;

                string paymentAmountStr = "Do zapłaty: " + inv.BruttoValue;
                Paragraph paymentAmount = new Paragraph(paymentAmountStr, summaryInfoFont);
                paymentAmount.Alignment = Element.ALIGN_LEFT;

                string invoiceCommentsStr = "Uwagi: " + inv.Comment;
                Paragraph invoiceComments = new Paragraph(invoiceCommentsStr, summaryInfoFont);
                invoiceComments.Alignment = Element.ALIGN_LEFT;

                string bankAccountNumberStr = "Numer konta bankowego: "; //TODO Add account number to company's class

                doc.Add(totalBruttoPrice);
                doc.Add(totalNettoPrice);
                doc.Add(paymentAmount);
                doc.Add(invoiceComments);
            }
        }

        /// <summary>
        /// Adds templates for placing buyer's and issuer's signatures and/or stamps
        /// </summary>
        private void AddSignatureTemplates(Document doc)
        {
            if (null != doc)
            {
                const string buyerSignatureTemplateStr = "Osoba odbierajaca fakture";
                const string issuerSingatureTemplateStr = "Osoba wystawiajaca fakture";
                const string signatureTemplateStr = ".............................................";

                Font signatureTemplateFont = new Font(DefaultFontFamily, SignatureTemplateFontSize);
                Paragraph signatureTemplate = new Paragraph();
                //Chunk to separte buyer's and issuer's signature templates
                Chunk separatorChunk = new Chunk(new VerticalPositionMark());

                signatureTemplate.Add(buyerSignatureTemplateStr);
                signatureTemplate.Add(new Chunk(separatorChunk));
                signatureTemplate.Add(issuerSingatureTemplateStr);
                signatureTemplate.Font = signatureTemplateFont;
                doc.Add(signatureTemplate);

                AddNewLines(NewLinesAfterSignatureStr, doc);

                signatureTemplate = new Paragraph();
                signatureTemplate.Add(signatureTemplateStr);
                signatureTemplate.Add(separatorChunk);
                signatureTemplate.Add(signatureTemplateStr);
                signatureTemplate.Font = signatureTemplateFont;
                doc.Add(signatureTemplate);
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

        //-------------------------------------------------------------------
        #endregion
    }
}