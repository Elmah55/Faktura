using System;

namespace Faktura.Utils
{
    class InputParser
    {
        //Const

        //Comapny settings
        private const UInt32 PostalCodeInputLenght = 5;

        /// <summary>
        /// Checks whether all parameters are correct to create invoice item
        /// </summary>
        /// <returns>Exception if</returns>
        public static ParseFailReason ParseGridInvoiceItem(string itemName, string vatRate, out UInt32 oVatRate,
            string nettoPrice, out double oNettoPrice, string quantity, out UInt32 oQuantity)
        {
            ParseFailReason reason = ParseFailReason.None;
            oVatRate = 0;
            oQuantity = 0;
            oNettoPrice = 0.0d;

            if (null != itemName && null != vatRate && null != nettoPrice)
            {
                if (0 == itemName.Length || 0 == vatRate.Length || 0 == nettoPrice.Length || 0 == quantity.Length)
                {
                    reason = ParseFailReason.EmptyInput;
                }
                else
                {
                    try
                    {
                        oVatRate = UInt32.Parse(vatRate);
                        oQuantity = UInt32.Parse(quantity);
                        oNettoPrice = double.Parse(nettoPrice);
                    }
                    catch (Exception ex)
                    {
                        reason = SetFailReason(ex);
                    }
                }
            }
            else
            {
                reason = ParseFailReason.NullArgument;
            }

            return reason;
        }

        public static ParseFailReason ParseInvoiceSettings(string VATRate, out UInt32 oVatRate,
    string paymentDays, out UInt32 oPaymentDays)
        {
            ParseFailReason reason = ParseFailReason.None;
            oVatRate = oPaymentDays = 0;

            if (null != VATRate && null != paymentDays)
            {
                if (0 == VATRate.Length || 0 == paymentDays.Length)
                {
                    reason = ParseFailReason.EmptyInput;
                }
                else
                {
                    try
                    {
                        oVatRate = UInt32.Parse(VATRate);
                        oPaymentDays = UInt32.Parse(paymentDays);
                    }
                    catch (Exception ex)
                    {
                        reason = SetFailReason(ex);
                    }
                }
            }
            else
            {
                reason = ParseFailReason.NullArgument;
            }

            return reason;
        }

        public static ParseFailReason ParseCompanySettigns(string companyName, string NIP, out UInt64 oNIP,
            string REGON, out UInt64 oREGON, string street, string houseNumber, out UInt16 oHouseNumber
            , string city, string postalCode, out UInt32 oPostalCode)
        {
            ParseFailReason reason = ParseFailReason.None;
            oNIP = oREGON = oPostalCode = oHouseNumber = 0;

            if (null == companyName || null == NIP && null == REGON && null == street
                || null == houseNumber || null == city || null == postalCode)
            {
                reason = ParseFailReason.NullArgument;
            }
            else if (0 == companyName.Length || 0 == NIP.Length || 0 == REGON.Length || 0 == street.Length
                || 0 == houseNumber.Length || 0 == city.Length || 0 == postalCode.Length)
            {
                reason = ParseFailReason.EmptyInput;
            }
            else if (PostalCodeInputLenght != postalCode.Length)
            {
                reason = ParseFailReason.InvalidInputLenght;
            }
            else
            {
                try
                {
                    oNIP = UInt64.Parse(NIP);
                    oREGON = UInt64.Parse(REGON);
                    oHouseNumber = UInt16.Parse(houseNumber);
                    oPostalCode = UInt32.Parse(postalCode);
                }
                catch (Exception ex)
                {
                    SetFailReason(ex);
                }
            }

            return reason;
        }

        public static ParseFailReason ParseInvoice(string paymentDays, out UInt32 oPaymentDays)
        {
            ParseFailReason reason = ParseFailReason.None;
            oPaymentDays = 0;

            if (null == paymentDays)
            {
                reason = ParseFailReason.NullArgument;
            }
            else if (0 == paymentDays.Length)
            {
                reason = ParseFailReason.EmptyInput;
            }
            else
            {
                try
                {
                    UInt32.Parse(paymentDays);
                }
                catch (Exception ex)
                {
                    SetFailReason(ex);
                }
            }

            return reason;
        }

        private static ParseFailReason SetFailReason(Exception ex)
        {
            ParseFailReason reason = ParseFailReason.None;

            if (null != ex)
            {
                if (ex is FormatException)
                {
                    reason = ParseFailReason.WrongInput;
                }
                else if (ex is OverflowException)
                {
                    reason = ParseFailReason.InputOverflow;
                }
                else
                {
                    reason = ParseFailReason.Unkown;
                }
            }
            else
            {
                reason = ParseFailReason.Unkown;
            }

            return reason;
        }
    }

    public enum ParseFailReason
    {
        None,
        EmptyInput,
        WrongInput,
        TooBigInput,
        InvalidInputLenght,
        InputOverflow,
        NullArgument,
        Unkown
    }
}