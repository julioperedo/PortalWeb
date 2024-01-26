using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities.SAP
{
    [Serializable()]
    public class Client : BEntity
    {
        #region Properties

        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string CardFName { get; set; }
        public string Address { get; set; }
        public string MailAddres { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Cellular { get; set; }
        public string Fax { get; set; }
        public string EMail { get; set; }
        public string SellerCode { get; set; }
        public string SellerName { get; set; }
        public string NIT { get; set; }
        public string Terms { get; set; }
        public decimal CreditLimit { get; set; }
        public decimal Balance { get; set; }
        public decimal OrdersBalance { get; set; }
        public string Subsidiary { get; set; }
        public string ContactPerson { get; set; }
        public string ACCPAC { get; set; }
        public DateTime CreateDate { get; set; }
        public string City { get; set; }

        #endregion

        #region Contructors

        public Client() { }

        #endregion
    }

    [Serializable()]
    public class ClientStatics : BEntity
    {
        public string CardCode { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Total { get; set; }
        public decimal Margin { get; set; }
        public decimal TaxlessTotal { get; set; }
        public decimal Average { get; set; }
        public long Quantity { get; set; }
        public DateTime? First { get; set; }
        public DateTime? Last { get; set; }

        public ClientStatics() { }
    }

    [Serializable()]
    public class ClientHoldInfo
    {
        public string CardCode { get; set; }
        public int OnHoldCredit { get; set; }
        public int OnHoldDue { get; set; }
    }
}
