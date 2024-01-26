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
using BEH = BEntities.HumanResources;
using BEI = BEntities.PiggyBank;
using BEN = BEntities.Campaign;

using DAL = DALayer.Product;

namespace BComponents.Product
{
    public partial class PriceGroupLineClient
    {
        #region Save Methods 

        public void Delete(long GroupLineId, string CardCode)
        {
            this.ErrorCollection.Clear();
            try
            {
                using TransactionScope transac = base.GenerateBusinessTransaction();
                using (DAL.PriceGroupLineClient dal = new())
                {
                    dal.Delete(GroupLineId, CardCode);
                }
                transac.Complete();
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
            }
        }

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        public IEnumerable<BEP.PriceGroupLineClient> ListByLine(long LineId, string SortingBy, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEP.PriceGroupLineClient> Items;
                using (DAL.PriceGroupLineClient dal = new())
                {
                    Items = dal.ListByLine(LineId, SortingBy, Relations);
                }
                return Items;
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