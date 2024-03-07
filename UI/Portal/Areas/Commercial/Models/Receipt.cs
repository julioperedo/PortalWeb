using Portal.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BEA = BEntities.SAP;

namespace Portal.Areas.Commercial.Models
{
    public class Receipt
    {
        public string Subsidiary { get; set; }
        public int DocNumber { get; set; }
        public DateTime DocDate { get; set; }
        public string State { get; set; }
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
        public decimal OnAccount { get; set; }
        public decimal TotalReceipt { get; set; }
        public decimal NotAppliedTotal { get; set; }
        public string Comments { get; set; }
        public List<Note> Notes { get; set; }
        public bool InDue { get; set; }
        public int TotalDueDays { get; set; }
        public int TotalBilled { get; set; }
        public int MaxDueDays { get; set; }
        public bool Adjust { get; set; }
        public string JournalComments { get; set; }

        public Receipt() { }

        public Receipt(BEA.Payment item)
        {
            DocNumber = item.Id;
            Subsidiary = item.Subsidiary;
            DocDate = item.DocDate;
            State = item.State;
            ClientCode = item.ClientCode;
            ClientName = item.ClientName;
            TotalReceipt = item.TotalReceipt;
            Adjust = true;
            Comments = item.Comments;
            JournalComments = item.JournalComments;
            Notes = new List<Note>();
        }
    }

    public class Note
    {
        public int NoteNumber { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal Total { get; set; }
        public int? Days { get; set; }
        public DateTime? DocDate { get; set; }
        public string Terms { get; set; }
        public string IsDelivery { get; set; }
    }
}
