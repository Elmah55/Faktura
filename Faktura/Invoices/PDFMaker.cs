using System;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace Faktura.Invoices
{
    /// <summary>
    /// This is a class for generating pdf file from invoice object
    /// </summary>
    class PDFMaker
    {
        public static Document GenerateInvoicePDF(Invoice pdfInvoice, string pdfFilePath)
        {
            if (null != pdfInvoice && null != pdfFilePath)
            {
                FileStream fStream = new FileStream(pdfFilePath, FileMode.Create);
                Document pdfDocument = new Document();
                PdfWriter writer = PdfWriter.GetInstance(pdfDocument, fStream);

                pdfDocument.OpenDocument();
                pdfDocument.Add(new Paragraph("PDF CREATION TEST"));
                pdfDocument.CloseDocument();
                writer.Close();
                writer.Dispose();

                return pdfDocument;
            }
            else
            {
                throw new ArgumentNullException("Null argument");
            }
        }
    }
}
