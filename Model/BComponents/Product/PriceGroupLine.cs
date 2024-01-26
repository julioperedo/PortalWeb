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
    public partial class PriceGroupLine
    {
        #region Save Methods 

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        public IEnumerable<BEP.PriceGroupLine> ListByClient(string CardCode, string SortingBy, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEP.PriceGroupLine> Items;
                using (DAL.PriceGroupLine dal = new())
                {
                    Items = dal.ListByClient(CardCode, SortingBy, Relations);
                }
                return Items;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEP.PriceGroupLine> ListByLine(long LineId, string SortingBy, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEP.PriceGroupLine> Items;
                using (DAL.PriceGroupLine dal = new())
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

        public BEP.PriceGroupLine SearchByUser(long UserId, params Enum[] Relations)
        {
            BEP.PriceGroupLine Item = null;
            try
            {
                using DAL.PriceGroupLine dal = new();
                Item = dal.SearchByUser(UserId, Relations);
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
            }
            return Item;
        }

        #endregion
    }
}