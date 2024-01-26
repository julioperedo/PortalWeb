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
using BET = BEntities.PostSale;
using BEP = BEntities.Product;
using BEL = BEntities.Sales;
using BEA = BEntities.SAP;
using BES = BEntities.Security;
using BEF = BEntities.Staff;
using BEV = BEntities.Visits;
using BEW = BEntities.WebSite;
using BEC = BEntities.CIESD;

using DAL = DALayer.CIESD;
using BEntities.Filters;

namespace BComponents.CIESD 
{
	public partial class Purchase 
	{
        #region Save Methods 

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        public IEnumerable<BEC.Purchase> List2(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEC.Purchase> Items = null;
                using (DAL.Purchase dal = new())
                {
                    Items = dal.List2(FilterList, Order, Relations);
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