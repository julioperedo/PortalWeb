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
using DAL = DALayer.Product;

namespace BComponents.Product {
    public partial class PriceExternalSite {
        #region Save Methods 

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        #endregion

        #region Search Methods 

        public BEP.PriceExternalSite SearchByProduct(long IdProduct, params Enum[] Relations) {
            BEP.PriceExternalSite BEObject = null;

            try {
                using(DAL.PriceExternalSite DALObject = new DAL.PriceExternalSite()) {
                    BEObject = DALObject.SearchByProduct(IdProduct, Relations);
                }
            } catch(Exception ex) {
                base.ErrorHandler(ex);
            }
            return BEObject;
        }

        #endregion
    }
}