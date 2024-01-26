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

using DAL = DALayer.Staff;
using BEntities.Filters;

namespace BComponents.Staff
{
    public partial class Replace
    {
        #region Save Methods 

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        public IEnumerable<BEF.Replace> List(string SellerCode, string SortingBy, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEF.Replace> Items = null;
                using (DAL.Replace dal = new())
                {
                    string today = DateTime.Today.ToString("yyyy-MM-dd");
                    List<Field> filters = new() {
                        new Field("SellerCode", SellerCode), new Field("InitialDate", today, Operators.LowerOrEqualThan), new Field("FinalDate", today, Operators.HigherOrEqualThan), 
                        new Field(LogicalOperators.And), new Field(LogicalOperators.And) 
                    };
                    Items = dal.List(filters, SortingBy, Relations);
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