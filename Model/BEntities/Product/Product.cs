using BEntities.AppData;
using BEntities.Base;
using BEntities.Logs;
using BEntities.Marketing;
using BEntities.Online;
using BEntities.Sales;
using BEntities.SAP;
using BEntities.Security;
using BEntities.Staff;
using BEntities.Visits;
using BEntities.WebSite;
using System;
using System.Collections.Generic;
using System.Linq;
using BE = BEntities;

namespace BEntities.Product
{
    public partial class Product
    {

        #region Properties 

        public bool WithPrices { get; set; }
        public decimal? SantaCruz { get; set; }
        public decimal? Iquique { get; set; }
        public decimal? Miami { get; set; }
        public string Subsidiary { get; set; }
        public string ProductManager { get; set; }
        public string StockSantaCruz { get; set; }
        public string StockIquique { get; set; }
        public string StockMiami { get; set; }
        public decimal? TransitSantaCruz { get; set; }
        public decimal? TransitIquique { get; set; }
        public decimal? TransitMiami { get; set; }
        public decimal? PriceSantaCruz { get; set; }
        public decimal? PriceIquique { get; set; }
        public decimal? PriceMiami { get; set; }
        public decimal? ReservedSantaCruz { get; set; }
        public decimal? ReservedIquique { get; set; }
        public decimal? ReservedMiami { get; set; }
        public bool CommentsIquique { get; set; }
        public bool CommentsSantaCruz { get; set; }
        public bool CommentsMiami { get; set; }
        public string RotationIquique { get; set; }
        public string RotationSantaCruz { get; set; }
        public string RotationMiami { get; set; }
        public long IdLine { get; set; }
        public string LineName { get; set; }

        #endregion

        #region Additional Properties 

        public IEnumerable<PriceOffer> CurrentOffers
        {
            get
            {
                var result = new List<PriceOffer>();
                if (ListPriceOffers == null || ListPriceOffers.Count == 0) return result;
                var idSubs = ListPriceOffers.Select(x => x.IdSubsidiary).Distinct();
                foreach (var id in idSubs)
                {
                    var temp = from t in ListPriceOffers 
                               where t.IdSubsidiary == id & t.Enabled & t.Price > 0 & (!t.Since.HasValue || t.Since.Value <= DateTime.Today) & (!t.Until.HasValue || t.Until.Value >= DateTime.Today) 
                               orderby t.Price 
                               select t;
                    if (temp.Any()) result.Add(temp.First());
                }
                return result;
                //return ListPriceOffers?.Where(x => x.Enabled & x.Price > 0 & (!x.Since.HasValue || x.Since.Value <= DateTime.Today) & (!x.Until.HasValue || x.Until.Value >= DateTime.Today))?.OrderByDescending(x => x.Price) ?? Enumerable.Empty<PriceOffer>();
            }
        }

        #endregion

        #region Contructors 

        #endregion

        #region Override members

        public override string ToString()
        {
            //TODO: Sobreescribir la propiedad mas utilizada
            return base.ToString();
        }

        #endregion
    }
}