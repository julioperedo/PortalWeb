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
    public partial class Visit {
        #region Save Methods 

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        public List<BEV.Visit> List2(List<Field> FilterList, string Order, params Enum[] Relations) {
            try {
                List<BEV.Visit> BECollection;
                using(DAL.Visit DALObject = new DAL.Visit()) {
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

        public BEV.Visit Search2(long Id, params Enum[] Relations) {
            BEV.Visit BEObject = null;
            try {
                using(DAL.Visit DALObject = new DAL.Visit()) {
                    BEObject = DALObject.Search2(Id, Relations);
                }
            } catch(Exception ex) {
                base.ErrorHandler(ex);
            }
            return BEObject;
        }

        #endregion
    }
}