using System;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Faktura.Companies;
using iTextSharp.text.pdf.draw;
using System.Windows;

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
        private const UInt32 SignatureTemplateFontSize = 7;
        private const UInt32 SummaryInfoFontSize = 10;
        //-------------------------------------------------------------------

        //New lines spacing
        //-------------------------------------------------------------------
        private const UInt32 NewLinesAfterHeader = 3;
        private const UInt32 NewLinesBeforeSummaryInfo = 3;
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

        //Invoice item's table related fields
        private int TableColumsCount;
        private float[] TableColumnsWidth;
        private PdfPTable InvoiceItemsTable;

        //Pdf document fields

        //Total number of all pages in pdf document.
        //This fields is calculated by incrementing
        //it at every page end event
        private UInt32 DocumentPagesCount;

        //-------------------------------------------------------------------
        #endregion

        public InvoicePDFMaker()
        {
            this.PageEventHelper = new PDFPageEvent();
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

        public void GenerateInvoicePDF(Company company, Invoice inv, string pdfFilePath)
        {
            if (null != company && null != inv && null != pdfFilePath)
            {
                FileStream fStream = CreatePdfFile(pdfFilePath);
                DocumentPagesCount = 1;

                if (null != fStream) //Pdf file created successfully
                {

                    Document doc = new Document();
                    PdfWriter writer = PdfWriter.GetInstance(doc, fStream);
                    doc.Open();
                    SetPageEvents(writer);

                    SetPageEvents(writer);
                    FillDocument(company, inv, doc, writer);

                    doc.Close();
                    doc.Dispose();
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
                PageEventHelper.EndPage += HandlePageEnd;
                PageEventHelper.StartPage += HandlePageStart;
            }
        }

        private void HandlePageStart(Document document, PdfWriter writer)
        {
            //if (0 != document.PageNumber && null != InvoiceItemsTable)
            //{
            //    AddItemsTableColumnsHeaders(InvoiceItemsTable);
            //} TODO Add column headers on every page with invoice item's table
        }

        private void HandlePageEnd(Document document, PdfWriter writer)
        {
            ++DocumentPagesCount;
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
                    MessageBox.Show("Nie można utworzyć dokumentu pdf. Spróbój zamknąć dokument jeśli jest otwarty."
                        , "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

            return fStream;
        }

        private void FillDocument(Company company, Invoice inv, Document doc, PdfWriter writer)
        {
            if (null != company && null != inv && null != doc && null != writer && doc.IsOpen())
            {
                AddInvoiceHeader(company, inv, doc);
                AddNewLines(NewLinesAfterHeader, doc);
                AddInvoiceItems(inv, doc);
                AddNewLines(NewLinesBeforeSummaryInfo, doc);
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
        private void AddInvoiceHeader(Company company, Invoice inv, Document doc)
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

        private void AddIssuerInfo(Company company, Document doc)
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
                InitInvoiceItemsTable();
                AddItemsToTable(inv, InvoiceItemsTable);

                if (null != InvoiceItemsTable)
                {
                    doc.Add(InvoiceItemsTable);
                }
            }
        }

        private void InitInvoiceItemsTable()
        {
            InvoiceItemsTable = new PdfPTable(TableColumnsWidth);
            InvoiceItemsTable.SetTotalWidth(TableColumnsWidth);
            AddItemsTableColumnsHeaders(InvoiceItemsTable);
        }

        private void AddItemsTableColumnsHeaders(PdfPTable table)
        {
            if (null != table)
            {
                Font tableColumnsFont = new Font(DefaultFontFamily, ItemsTableFontSize);

                //Add columns names
                table.AddCell(new PdfPCell(new Phrase(ItemNumberColumnName, tableColumnsFont)));

                foreach (string columnName in InvoiceItem.FieldsName)
                {
                    table.AddCell(new PdfPCell(new Phrase(columnName, tableColumnsFont)));
                }
            }
        }

        private void AddItemsToTable(Invoice inv, PdfPTable table)
        {
            if (null != inv && null != table)
            {
                UInt32 itemNumber = 1;
                Font tableElementsFont = new Font(DefaultFontFamily, ItemsTableFontSize);

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
            if (null != doc && null != writer)
            {
                //Coordinates for pages count footer
                const int pageCountFooterX = 550;
                const int PageCountFooterY = 20;

                UInt32 currentPageNumber = 1;
                Font pagesCountFont = new Font(DefaultFontFamily, FooterFontSize);
                writer.ResetPageCount();
                writer.DirectContentUnder.Reset();

                for (currentPageNumber = 1; currentPageNumber < DocumentPagesCount; currentPageNumber++)
                {
                    string pagesCountStr = "Strona " + currentPageNumber + " z " + DocumentPagesCount;
                    Phrase pagesCountPhrase = new Phrase(pagesCountStr);
                    pagesCountPhrase.Font = pagesCountFont;

                    PdfContentByte content = writer.DirectContentUnder;
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
                string totalBruttoPriceStr = "Razem brutto: " + inv.BruttoValue + " " + inv.CurrencyType + "\n";
                string totalNettoPriceStr = "Razem netto: " + inv.NettoValue + " " + inv.CurrencyType + "\n";
                string paymentAmountStr = "Do zaplaty razem: " + inv.BruttoValue + " " + inv.CurrencyType + "\n";
                string invoiceCommentsStr = "Uwagi: " + inv.Comment + "\n";
                string bankAccountNumberStr = "Numer konta bankowego: " + "\n"; //TODO Add account number to company's class

                Font summaryInfoFont = new Font(DefaultFontFamily, SummaryInfoFontSize);
                Paragraph summaryInfo = new Paragraph();
                summaryInfo.Font = summaryInfoFont;
                summaryInfo.Alignment = Element.ALIGN_LEFT;

                summaryInfo.Add(totalBruttoPriceStr);
                summaryInfo.Add(totalNettoPriceStr);
                summaryInfo.Add(paymentAmountStr);
                summaryInfo.Add(invoiceCommentsStr);

                doc.Add(summaryInfo);
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
                signatureTemplate.Font = signatureTemplateFont;
                //Chunk to separte buyer's and issuer's signature templates
                Chunk separatorChunk = new Chunk(new VerticalPositionMark());

                signatureTemplate.Add(buyerSignatureTemplateStr);
                signatureTemplate.Add(separatorChunk);
                signatureTemplate.Add(issuerSingatureTemplateStr);
                signatureTemplate.Font = signatureTemplateFont;
                doc.Add(signatureTemplate);

                AddNewLines(NewLinesAfterSignatureStr, doc);

                signatureTemplate.Clear();
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
                Paragraph newLines = new Paragraph();

                for (int iterator = 0; iterator < count; iterator++)
                {
                    newLines.Add(new Paragraph(Environment.NewLine));
                }

                pdfDocument.Add(newLines);
            }
        }

        //-------------------------------------------------------------------
        #endregion
    }
}