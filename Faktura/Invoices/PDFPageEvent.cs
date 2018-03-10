using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Faktura.Invoices
{
    /// <summary>
    /// This is class contaings C# style events for iTextSharp page events.
    /// </summary>
    class PDFPageEvent : PdfPageEventHelper, IPdfPageEvent
    {
        public event PDFPageEventHandler EndPage;
        public event PDFPageEventHandler StartPage;
        public event PDFPageEventHandler CloseDocument;
        public event PDFPageEventHandler OpenDocument;

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            EndPage?.Invoke(document, writer);
            base.OnEndPage(writer, document);
        }

        public override void OnStartPage(PdfWriter writer, Document document)
        {
            StartPage?.Invoke(document, writer);
            base.OnStartPage(writer, document);
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            CloseDocument?.Invoke(document, writer);
            base.OnCloseDocument(writer, document);
        }

        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            OpenDocument?.Invoke(document, writer);
            base.OnOpenDocument(writer, document);
        }
    }

    public delegate void PDFPageEventHandler(Document document, PdfWriter writer);
}
