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

        private const Font.FontFamily GeneralFont = Font.FontFamily.TIMES_ROMAN;
        private const UInt32 GeneralFontSize = 10; //Default font used for most text on invoice pdf

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

        //Header related methods
        //-------------------------------------------------------------------

        /// <summary>
        /// Adds invoice header to pdf document
        /// </summary>
        private void AddInvoiceHeader(CompanySettings company, Invoice inv, Document doc)
        {
            if (null != company && null != inv && null != doc)
            {


            }
        }

        private void AddInvoiceNumber(Invoice inv, Document doc)
        {
            if (null != inv && null != doc)
            {
                string headerText = "Faktura nr. " + inv.InvoiceNumber;
                Paragraph header = new Paragraph(headerText);
                header.Font = new Font(GeneralFont);
                header.Alignment = Element.ALIGN_CENTER;
                doc.Add(header);
            }
        }

        private void AddIssuerInfo(Invoice inv, Document doc)
        {
            if (null != inv && null != doc)
            {
                //string issuerInfoText = "Wystawiający:\n"
                //    + inv
            }

            //-------------------------------------------------------------------
        }
    }
}
