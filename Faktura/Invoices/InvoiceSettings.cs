using System;

namespace Faktura.Invoices
{
    [Serializable]
    public class InvoiceSettings
    {
        //Invoice settings
        public static UInt32 DefaultVATRate
        {
            get
            {
                return 23;
            }
        }
        public static UInt32 DefaultDaysForPayment
        {
            get
            {
                return 7;
            }
        }
        public static DateTime DefaultPaymentDate
        {
            get
            {
                return (DateTime.Now.AddDays(DefaultDaysForPayment));
            }
        }
        public static DateTime DefaultIssueDate
        {
            get
            {
                return DateTime.Now;
            }
        }

        //Invoice settings
        private UInt32 _DaysForPayment;
        public UInt32 DaysForPayment
        {
            get
            {
                return _DaysForPayment;
            }

            set
            {
                _DaysForPayment = value;
            }
        }

        private DateTime _IssueDate;
        public DateTime IssueDate
        {
            get
            {
                return _IssueDate;
            }

            set
            {
                _IssueDate = value;
            }
        }

        private UInt32 _VATRate;
        public UInt32 VATRate
        {
            get
            {
                return _VATRate;
            }

            internal set
            {
                _VATRate = value;
            }
        }

        public InvoiceSettings()
        {
            ResetSettings();
        }

        /// <summary>
        /// Sets this instance of invoice settings to its default
        /// values
        /// </summary>
        public void ResetSettings()
        {
            this._DaysForPayment = DefaultDaysForPayment;
            this._VATRate = DefaultVATRate;
            this._IssueDate = DefaultIssueDate;
        }
    }
}
