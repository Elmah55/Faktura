using System;

namespace Faktura
{
    class InvoiceItem
    {
        //This is table containing name of this class' fields
        private static string[] _FieldsName = {"Nazwa","Stawka VAT","Cena Netto",
            "Cena Brutto","Wartosc Netto","Ilosc","Komentarz"};
        public static string[] FieldsName
        {
            get
            {
                return _FieldsName;
            }
        }

        //Fields

        public string Name { get; private set; }
        public UInt32 VATRate { get; private set; }

        public double NettoPrice { get; private set; }
        public double BruttoPrice
        {
            get
            {
                return NettoPrice + (NettoPrice * VATRate);
            }
        }
        public double NettoValue
        {
            get
            {
                return (NettoPrice * Count);
            }
        }

        public UInt32 Count { get; set; } //Count of this item copies in one invoice
        public string Comment { get; private set; }

        public InvoiceItem(string itemName, UInt32 VATRate, double nettoPrice, string comment, UInt32 count)
        {
            this.Name = itemName;
            this.VATRate = VATRate;
            this.NettoPrice = nettoPrice;
            this.Comment = comment;
            this.Count = count;
        }

        public override int GetHashCode()
        {
            return ((int)(Name.ToUpper().GetHashCode() + VATRate + NettoPrice));
        }

        public override bool Equals(object obj)
        {
            bool result = false;
            InvoiceItem other = obj as InvoiceItem;

            if (null != obj && null != other)
            {
                result = (this.BruttoPrice == other.BruttoPrice)
                    && (0 == string.Compare(this.Comment, other.Comment, true))
                    && (0 == string.Compare(this.Name, other.Name, true))
                    && (this.NettoPrice == other.NettoPrice)
                    && (this.VATRate == other.VATRate);
            }

            return result;
        }
    }
}
