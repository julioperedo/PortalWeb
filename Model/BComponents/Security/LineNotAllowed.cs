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
using DAL = DALayer.Security;

namespace BComponents.Security {
    public partial class LineNotAllowed {
        #region Save Methods 

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        public List<BES.LineNotAllowed> ListSelected(string CardCode, string Order, params Enum[] Relations) {
            try {
                List<BES.LineNotAllowed> BECollection;

                using(DAL.LineNotAllowed DALObject = new DAL.LineNotAllowed()) {
                    BECollection = DALObject.ListSelected(CardCode, Order, Relations);
                }

                return BECollection;
            } catch(Exception ex) {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public List<BES.LineNotAllowed> ListClients() {
            try {
                List<BES.LineNotAllowed> BECollection;

                using(DAL.LineNotAllowed DALObject = new DAL.LineNotAllowed()) {
                    BECollection = DALObject.ListClients();
                }

                return BECollection;
            } catch(Exception ex) {
                base.ErrorHandler(ex);
                return null;
            }
        }

        #endregion

        #region Search Methods 

        #endregion
    }
}