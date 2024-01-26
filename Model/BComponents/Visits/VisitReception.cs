using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Transactions;
using BE = BEntities;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BED = BEntities.AppData;
using BEF = BEntities.Staff;
using BEG = BEntities.Logs;
using BEL = BEntities.Sales;
using BEM = BEntities.Marketing;
using BEO = BEntities.Online;
using BEP = BEntities.Product;
using BES = BEntities.Security;
using BEV = BEntities.Visits;
using BEW = BEntities.WebSite;
using DAL = DALayer.Visits;

namespace BComponents.Visits {
    public partial class VisitReception {
        #region Save Methods 

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        public List<BEV.VisitReception> List2(List<Field> FilterList, string Order, params Enum[] Relations) {
            try {
                List<BEV.VisitReception> BECollection;
                using(DAL.VisitReception DALObject = new DAL.VisitReception()) {
                    BECollection = DALObject.List2(FilterList, Order, Relations);
                }
                return BECollection;
            } catch(Exception ex) {
                ErrorHandler(ex);
                return null;
            }
        }

        #endregion

        #region Search Methods 

        #endregion
    }
}