using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Areas.Commercial.Models
{
    public class TransportFilter
    {
        public DateTime? InitialDate { get; set; }
        public DateTime? FinalDate { get; set; }
        public long? TransporterId { get; set; }
        public long? SourceId { get; set; }
        public long? DestinationId { get; set; }
        public string Sent { get; set; }
        public string Filter { get; set; }

        public TransportFilter() { }
    }

    public class Mailer
    {
        public string EMail { get; set; }
        public string Name { get; set; }

        public Mailer() { }
        public Mailer(string Name, string EMail)
        {
            this.Name = Name;
            this.EMail = EMail;
        }
    }

    public class Transport
    {
        public long Id { get; set; }
        public string DocNumber { get; set; }
        public string Date { get; set; }
        public string Transporter { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public string DeliveryTo { get; set; }
        public string Observations { get; set; }
        public string Weight { get; set; }
        public int QuantityPieces { get; set; }
        public string RemainingAmount { get; set; }
        public long TypeId { get; set; }
        public List<Client> Clients { get; set; }

        public Transport(BEntities.Sales.Transport Item)
        {
            Id = Item.Id;
            DocNumber = Item.DocNumber;
            Date = Item.Date.ToString("dd-MM-yyyy");
            Transporter = Item.Transporter?.Name ?? "";
            Source = Item.Source?.Name ?? "";
            Destination = Item.Destination?.Name ?? "";
            DeliveryTo = Item.DeliveryTo ?? "";
            Observations = Item.Observations ?? "";
            Weight = Item.Weight.ToString("N2") ?? "";
            QuantityPieces = Item.QuantityPieces;
            RemainingAmount = Item.RemainingAmount.ToString("N2");
            TypeId = Item.TypeId;
            Clients = new List<Client>();
        }
    }

    public class Client
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Codes { get; set; }
        public List<Mailer> Contacts { get; set; }
        public List<Mailer> Sellers { get; set; }

        public Client()
        {
            Contacts = new List<Mailer>();
            Sellers = new List<Mailer>();
        }
    }
}
