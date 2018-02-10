using System;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Faktura.Companies;

namespace Faktura.Invoices
{
    /// <summary>
    /// This is a class for generating pdf file from invoice object
    /// </summary>
    class InvoicePDFMaker
    {
        //Consts

        private const Font.FontFamily DefaultFont = Font.FontFamily.TIMES_ROMAN;

        private const UInt32 DefualtFontSize = 14; //Default font used for most text on invoice pdf
        private const UInt32 IssuerInfoFontSize = 10;

        public Document GenerateInvoicePDF(CompanySettings company, Invoice inv, string pdfFilePath)
        {
            if (null != company && null != inv && null != pdfFilePath)
            {
                FileStream fStream = new FileStream(pdfFilePath, FileMode.Create);
                Document pdfDocument = new Document();
                PdfWriter writer = PdfWriter.GetInstance(pdfDocument, fStream);

                pdfDocument.OpenDocument();

                AddInvoiceHeader(company, inv, pdfDocument);

                pdfDocument.CloseDocument();
                writer.Close();
                writer.Dispose();

                return pdfDocument;
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        //Invoice header related methods
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
                string issuerInfoText = "Wystawiający:\n"
                  + company.CompanyName + "\n"
                  + company.Street + " / " + company.HouseNumber + "\n"
                  + company.PostalCode.ToString().Substring(0, 2) + "-"
                  + company.PostalCode.ToString().Substring(2, 3)
                  + " " + company.City;

                Font issuerInfoFont = new Font(DefaultFont, IssuerInfoFontSize);
                Paragraph issuerInfo = new Paragraph(issuerInfoText, issuerInfoFont);
                issuerInfo.Alignment = Element.ALIGN_RIGHT;
                doc.Add(issuerInfo);
            }
        }

        //-------------------------------------------------------------------

        /// <summary>
        /// Adds table with invoice items to the invoice pdf document
        /// </summary>
        private void AddInvoiceItems(Invoice inv, Document doc)
        {
            if (null != inv && null != doc && 0 != inv.Items.Count)
            {
                //Table with names of 
                //const string[] itemsTableColumns
            }
        }
    }
}
