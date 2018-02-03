using System;

namespace Faktura
{
    class InvoiceItem
    {
        public string ItemName { get; private set; }
        public UInt32 VATRate { get; private set; }
        public double NettoPrice { get; private set; }
        public double BruttoPrice
        {
            get
            {
                return NettoPrice * VATRate;
            }
        }
        public string Comment { get; private set; }

        public InvoiceItem(string itemName, UInt32 VATRate, double nettoPrice, string comment)
        {
            this.ItemName = itemName;
            this.VATRate = VATRate;
            this.NettoPrice = nettoPrice;
            this.Comment = comment;
        }

        public override int GetHashCode()
        {
            return ((int)(ItemName.ToUpper().GetHashCode() + VATRate + NettoPrice));
        }

        public override bool Equals(object obj)
        {
            bool result = false;
            InvoiceItem other = obj as InvoiceItem;

            if (null != obj && null != other)
            {
                if ((this.BruttoPrice == other.BruttoPrice)
                    && (0 == string.Compare(this.Comment, other.Comment, true))
                    && (0 == string.Compare(this.ItemName, other.ItemName, true))
                    && (this.NettoPrice == other.NettoPrice)
                    && (this.VATRate == other.VATRate))
                {
                    result = true;
                }
            }

            return result;
        }
    }
}
