using System;
using System.Collections.Generic;
using System.Linq;

namespace Portal.Areas.Commercial.Models
{
    public class TokenResponse
    {
        public string Token { get; set; }
        public string Client { get; set; }
    }

    public class ServiceResponse<T>
    {
        public string Message { get; set; }
        public T Item { get; set; }
    }

    public class ReturnLicence
    {
        public string ServiceTransactionId { get; set; }
        public string ClientTransactionId { get; set; }
    }

    public class ProductLicence
    {
        public long Id { get; set; }
        public Guid TransactionId { get; set; }
        public string Sku { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string FulfillmentType { get; set; }
        public IList<Token> Tokens { get; set; }
        public IList<Link> Links { get; set; }
    }

    public class Token
    {
        public string Id { get; set; }
        public string SequenceNumber { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
    }

    public class Link
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Uri { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool ExpirationDateSpecified { get; set; }
    }

}
