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

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            EndPage?.Invoke(document, writer);
            base.OnEndPage(writer, document);
        }
    }

    public delegate void PDFPageEventHandler(Document document, PdfWriter writer);
}
