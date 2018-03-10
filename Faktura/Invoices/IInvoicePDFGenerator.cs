using Faktura.Companies;

namespace Faktura.Invoices
{
    interface IInvoicePDFGenerator
    {
        void GenerateInvoicePDF(Company company, Invoice inv, string pdfFilePath);
    }
}
