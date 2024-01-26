using System;
using System.Collections.Generic;
using System.Transactions;
using BE = BEntities;
using BED = BEntities.AppData;
using BEB = BEntities.Base;
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

using DAL = DALayer.Product;
using BEntities.Filters;

namespace BComponents.Product
{
    public partial class PriceHistory
    {
        #region Save Methods 

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        public IEnumerable<BEP.PriceHistory> List2(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEP.PriceHistory> BECollection;
                using (DAL.PriceHistory DALObject = new())
                {
                    BECollection = DALObject.List2(FilterList, Order, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEP.PriceHistory> ListFirst(List<long> Ids)
        {
            try
            {
                IEnumerable<BEP.PriceHistory> BECollection;
                using (DAL.PriceHistory DALObject = new())
                {
                    BECollection = DALObject.ListFirst(Ids);
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