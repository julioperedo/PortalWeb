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

using DAL = DALayer.Product;

namespace BComponents.Product
{
    public partial class PriceGroup
    {
        #region Save Methods 

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        public IEnumerable<BEP.PriceGroup> ListByClient(string CardCode, string SortingBy, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEP.PriceGroup> Items;
                using (DAL.PriceGroup dal = new())
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

        #endregion

        #region Search Methods 

        public BEP.PriceGroup SearchByUser(long UserId, params Enum[] Relations)
        {
            BEP.PriceGroup Item = null;
            try
            {
                using (DAL.PriceGroup dal = new())
                {
                    Item = dal.SearchByUser(UserId, Relations);
                }
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