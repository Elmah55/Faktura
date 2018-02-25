using System;

namespace Faktura.Companies
{
    [Serializable]
    public class Company
    {
        private string _CompanyName;
        public string CompanyName
        {
            get
            {
                return _CompanyName;
            }

            set
            {
                _CompanyName = value;
            }
        }

        private UInt64 _NIP;
        public UInt64 NIP
        {
            get
            {
                return _NIP;
            }

            set
            {
                _NIP = value;
            }
        }

        private UInt64 _REGON;
        public UInt64 REGON
        {
            get
            {
                return _REGON;
            }

            set
            {
                _REGON = value;
            }
        }

        //Address
        private string _Street;
        public string Street
        {
            get
            {
                return _Street;
            }

            set
            {
                _Street = value;
            }
        }

        private UInt16 _HouseNumber;
        public UInt16 HouseNumber
        {
            get
            {
                return _HouseNumber;
            }

            set
            {
                _HouseNumber = value;
            }
        }

        private string _City;
        public string City
        {
            get
            {
                return _City;
            }

            set
            {
                _City = value;
            }
        }

        private UInt32 _PostalCode;
        public UInt32 PostalCode
        {
            get
            {
                return _PostalCode;
            }

            set
            {
                _PostalCode = value;
            }
        }

        public Company(string companyName, UInt64 nip, UInt64 regon, string street, UInt16 houseNumber,
        string city, UInt32 postalCode)
        {
            if (null != street && null != companyName && null != city)
            {
                this.CompanyName = companyName;
                this.NIP = nip;
                this.REGON = regon;
                this.Street = street;
                this.HouseNumber = houseNumber;
                this.City = city;
                this.PostalCode = postalCode;
            }
            else
            {
                throw new ArgumentNullException();
            }
        }
    }
}
