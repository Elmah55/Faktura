using System;
using System.Collections.Generic;
using Faktura.Companies;

namespace Faktura.Invoices
{
    class Invoice
    {
        public Company Issuer { get; private set; }
        public Company Receiver { get; private set; }
        public DateTime IssueDate { get; private set; }
        public UInt32 PaymentDays { get; private set; }
        public PaymentType PaymentType { get; private set; }
        public string Comment { get; private set; }
        public string InvoiceNumber { get; private set; }
        public ICollection<InvoiceItem> Items { get; private set; }
        public Currency CurrencyType { get; private set; }

        /// <summary>
        /// Total brutto value of all items in invoice
        /// </summary>
        public double BruttoValue
        {
            get
            {
                double totalBruttoValue = 0.0d;

                foreach (InvoiceItem item in this.Items)
                {
                    totalBruttoValue += item.BruttoPrice;
                }

                return totalBruttoValue;
            }
        }

        /// <summary>
        /// Total netto value of all items in invoice
        /// </summary>
        public double NettoValue
        {
            get
            {
                double totalNettoValue = 0.0d;

                foreach (InvoiceItem item in this.Items)
                {
                    totalNettoValue += item.NettoPrice;
                }

                return totalNettoValue;
            }
        }

        public Invoice()
        {

        }

        public Invoice(DateTime issueDate, UInt32 paymentDate, string invoiceNumber, ICollection<InvoiceItem> items
            , PaymentType paymentType, Currency currencyType, string comment = "")
        {
            if (null != invoiceNumber && null != items && null != comment)
            {
                Items = items;
                this.Items = items;
                this.IssueDate = issueDate;
                this.PaymentDays = paymentDate;
                this.InvoiceNumber = invoiceNumber;
                this.PaymentType = paymentType;
                this.CurrencyType = currencyType;
                this.Comment = comment;
            }
            else
            {
                throw new ArgumentNullException("Null argument");
            }
        }
    }

    public enum PaymentType
    {
        Card,
        Cash,
        Transfer
    }

    public enum Currency
    {
        PLN,
        USD,
        EUR
    }
}
