using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BEL = BEntities.Sales;

namespace Portal.Areas.Commercial.Models
{
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
