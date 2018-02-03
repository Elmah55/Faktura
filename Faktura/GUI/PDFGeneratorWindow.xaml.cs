using Faktura.Invoices;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Faktura.GUI
{
    /// <summary>
    /// Interaction logic for PDFGeneratorWindow.xaml
    /// </summary>
    public partial class PDFGeneratorWindow : Window
    {
        private MainWindow MainWin;
        private InvoicePDFMaker PDFGenerator;

        public PDFGeneratorWindow(MainWindow mainWin)
        {
            if (null != mainWin)
            {
                InitializeComponent();
                PDFGenerator = new InvoicePDFMaker();
                this.MainWin = mainWin;
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// Generates PDF from Invoice class instance and saves
        /// .pdf file to provided path
        /// </summary>
        private bool GeneratePDF(Invoice inv, string pdfPath)
        {
            bool result = false;

            if (null != inv && null != pdfPath && null != PDFGenerator)
            {
                PDFGenerator.GenerateInvoicePDF(inv, pdfPath);
            }

            return result;
        }

        private void ButtonGeneratePDF_Click(object sender, RoutedEventArgs e)
        {
            List<InvoiceItem> items = new List<InvoiceItem>();
            items.Add(new InvoiceItem("Asss", 22, 4, "AHAAHA"));

            Invoice inv = new Invoice(DateTime.Now, 5, "55/44/33", items);
            GeneratePDF(inv, "test.pdf");
        }
    }
}
