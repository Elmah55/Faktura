using System;

namespace Faktura
{
    /// <summary>
    /// This is class representing invoice item in
    /// invoice items grid
    /// </summary>
    class GridInvoiceItem : InvoiceItem
    {
        public UInt32 Count { get; set; }
        public double NettoValue
        {
            get
            {
                return (base.NettoPrice * Count);
            }
        }

        public GridInvoiceItem(InvoiceItem baseItem)
            : base(baseItem.ItemName, baseItem.VATRate, baseItem.NettoPrice, baseItem.Comment)
        {
            Count = 0;
        }
    }
}
