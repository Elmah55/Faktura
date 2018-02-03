using System;
using System.Collections.Generic;

namespace Faktura.Invoices
{
    class Invoice
    {
        public DateTime IssueDate { get; private set; }
        public UInt32 PaymentDays { get; private set; }
        public string InvoiceNumber { get; private set; }
        public ICollection<InvoiceItem> Items { get; private set; }

        public Invoice(DateTime issueDate, UInt32 paymentDate, string invoiceNumber, ICollection<InvoiceItem> items)
        {
            if (null != invoiceNumber || null != items)
            {
                Items = items;
                this.Items = items;
                this.IssueDate = issueDate;
                this.PaymentDays = paymentDate;
                this.InvoiceNumber = invoiceNumber;
            }
            else
            {
                throw new ArgumentNullException("Null argument");
            }
        }
    }
}
