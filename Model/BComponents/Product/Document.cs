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

using DAL = DALayer.Product;
using BEntities.Filters;

namespace BComponents.Product
{
    public partial class Document
    {
        #region Save Methods 

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        public IEnumerable<BEP.Document> ListExtended(List<Field> FilterList, string SortingBy, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEP.Document> Items = default;
                using (DAL.Document dal = new())
                {
                    Items = dal.ListExtended(FilterList, SortingBy, Relations);
                }
                return Items;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEP.Product> ListProducts(string SortingBy)
        {
            try
            {
                IEnumerable<BEP.Product> Items = default;
                using (DAL.Product dal = new())
                {
                    Items = dal.ListProducts(SortingBy);
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