using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BEA = BEntities.SAP;
using BEL = BEntities.Sales;

namespace MobileService.Models
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
        public bool Adjust { get; set; }

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

    public class ResumeSales
    {
        public string Subsidiary { get; set; }
        public string Warehouse { get; set; }
        public decimal Today { get; set; }
        public decimal Period { get; set; }
        public decimal Percentage { get; set; }
        public decimal Margin { get; set; }
        public decimal MarginTotal { get; set; }
        public decimal TaxeslessTotal { get; set; }
        public decimal Authorized { get; set; }
        public decimal Open { get; set; }
        public decimal Stock { get; set; }
    }

    public class SalesProjection
    {
        public long Id { get; set; }

        public string Subsidiary { get; set; }
        public string Division { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }

        public decimal Amount { get; set; }

        #region Constructors

        public SalesProjection() { }
        public SalesProjection(BEL.Projection Item)
        {
            Id = Item.Id;
            Subsidiary = Item.Subsidiary;
            Division = Item.Division;
            Year = Item.Year;
            Month = Item.Month;
            Amount = Item.Amount;
        }
        public SalesProjection(long Id, string Subsidiary, string Division, int Year, int Month, decimal Amount)
        {
            this.Id = Id;
            this.Subsidiary = Subsidiary;
            this.Division = Division;
            this.Year = Year;
            this.Month = Month;
            this.Amount = Amount;
        }

        #endregion
    }
}