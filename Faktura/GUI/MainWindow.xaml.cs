using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Faktura.Invoices;
using Faktura.Utils;
using Faktura.GUI;
using Faktura.Companies;
using System.Collections.Generic;

namespace Faktura
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WindowManager WinManager;
        private ObservableCollection<InvoiceItem> InvoiceItems;
        private InvoicePDFMaker InvoicePDF;

        public MainWindow()
        {
            InitializeComponent();

            WinManager = new WindowManager();
            InvoiceItems = new ObservableCollection<InvoiceItem>();
            InvoicePDF = new InvoicePDFMaker();

            DataGridInvoiceItems.ItemsSource = InvoiceItems;
            DataGridInvoiceItems.AutoGenerateColumns = false;
        }

        private void ButtonAddPosition_Click(object sender, RoutedEventArgs e)
        {
            // bool a=DataGridInvoiceItems.CancelEdit();
            BeginAddItemToGridView();
        }

        private void BeginAddItemToGridView()
        {
            UInt32 vatRate = 0;
            double nettoPrice = 0.0f;
            UInt32 quantity = 0;

            ParseFailReason parseFailReason = InputParser.ParseGridInvoiceItem
                (TextBoxItemName.Text, TextBoxVATRate.Text, out vatRate, TextBoxNettoPrice.Text,
                out nettoPrice, TextBoxQuantity.Text, out quantity);

            if (ParseFailReason.None == parseFailReason)
            {
                AddInvoiceItemToGridView(TextBoxItemName.Text, TextBoxComment.Text, nettoPrice, vatRate, quantity);
                DataGridInvoiceItems.Items.Refresh();
            }
            else
            {
                GUIInfoHelper.DisplayInputParseError(parseFailReason);
            }
        }

        private void AddInvoiceItemToGridView(string itemName, string comment, double nettoPrice, UInt32 vatRate, UInt32 quantity)
        {
            if ((null != itemName) && (null != comment))
            {
                InvoiceItem newInvoiceItem = new InvoiceItem(itemName, vatRate, nettoPrice, comment, quantity);
                //Variable indicating whether invoice item equal to created
                // invoice item is alread in item's collection
                bool isAlreadyInCollection = false;

                //Check whether item with same properties already exsists in list of invoice items and increase its count
                foreach (InvoiceItem invoiceItem in InvoiceItems)
                {
                    if (invoiceItem.Equals(newInvoiceItem))
                    {
                        invoiceItem.Count += quantity;
                        isAlreadyInCollection = true;
                        break;
                    }
                }

                if (!isAlreadyInCollection)
                {
                    InvoiceItems.Add(newInvoiceItem);
                }
            }
        }

        /// <summary>
        /// Removes selected items from invoices grid view
        /// </summary>
        /// <returns>True if at least one item has been deleted, false if no items where selected or deletion failed</returns>
        private bool RemoveSelectedItems()
        {
            bool removalResult = false;

            if (0 < DataGridInvoiceItems.SelectedItems.Count)
            {
                removalResult = true;

                while (0 != DataGridInvoiceItems.SelectedItems.Count)
                {
                    InvoiceItems.RemoveAt(DataGridInvoiceItems.SelectedIndex);
                }
            }

            return removalResult;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    BeginAddItemToGridView();
                    break;
                case Key.Delete:
                    RemoveSelectedItems();
                    break;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //Close entire application on main window close
            Environment.Exit(0);
        }

        private void ButtonGenerateInvoice_Click(object sender, RoutedEventArgs e)
        {
            CompanySettings cmpsts = new CompanySettings("FIRMA", 55555555, 4444444, "Wiertnicza 2", 6, "Szczecin", 32421);
            List<InvoiceItem> invitems = new List<InvoiceItem>();
            Invoice inv = new Invoice(DateTime.Now, 3, "K/755/33", invitems);
            InvoicePDF.GenerateInvoicePDF(cmpsts, inv, "test.pdf");
        }

        private void OpenCompanySettingsWindow()
        {
            if (null != WinManager)
            {
                WinManager.CompanySettingsWin.Show();
            }
        }

        private void OpenInvoiceSettingsWindow()
        {
            if (null != WinManager)
            {
                WinManager.InvoiceSettingsWin.Show();
            }
        }

        private void MenuItemSettingsInvoice_Click(object sender, RoutedEventArgs e)
        {
            OpenInvoiceSettingsWindow();
        }

        private void MenuItemSettingsCompany_Click(object sender, RoutedEventArgs e)
        {
            OpenCompanySettingsWindow();
        }
    }
}