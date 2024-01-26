using System;
using System.Collections.Generic;
using System.Transactions;
using BE = BEntities;
using BED = BEntities.AppData;
using BEB = BEntities.Base;
using BEK = BEntities.Kbytes;
using BEG = BEntities.Logs;
using BEM = BEntities.Marketing;
using BEO = BEntities.Online;
using BEP = BEntities.Product;
using BEL = BEntities.Sales;
using BEA = BEntities.SAP;
using BES = BEntities.Security;
using BEF = BEntities.Staff;
using BEV = BEntities.Visits;
using BEW = BEntities.WebSite;
using BEX = BEntities.CIESD;

using DAL = DALayer.Product;
using System.Threading.Tasks;

namespace BComponents.Product
{
    public partial class PriceOffer
    {
        #region Save Methods 

        public async Task SaveAsync(BEP.PriceOffer Item)
        {
            this.ErrorCollection.Clear();
            if (this.Validate(Item))
            {
                try
                {
                    using DAL.PriceOffer dal = new();
                    await dal.SaveAsync(Item);
                }
                catch (Exception ex)
                {
                    base.ErrorHandler(ex);
                }
            }
            else
            {
                base.ErrorHandler(new BCException(this.ErrorCollection));
            }
        }

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        public IEnumerable<BEP.PriceOffer> ListByLine(long LineId, string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEP.PriceOffer> Items;
                using (DAL.PriceOffer dal = new())
                {
                    Items = dal.ListByLine(LineId, Order, Relations);
                }
                return Items;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEP.PriceOffer> ListEnabled(IEnumerable<long> IdProducts, string SortingBy, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEP.PriceOffer> BECollection;
                using (DAL.PriceOffer DALObject = new())
                {
                    BECollection = DALObject.ListEnabled(IdProducts, SortingBy, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        #endregion

        #region Search Methods 

        #endregion
    }
}