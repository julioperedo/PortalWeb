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

using DAL = DALayer.Security;

namespace BComponents.Security {
	public partial class SessionHistory {
        #region Save Methods 

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        public List<BES.SessionHistory> List(string CardCode, DateTime Date, params Enum[] Relations) {
            try {
                List<BES.SessionHistory> BECollection;

                using(DAL.SessionHistory DALObject = new DAL.SessionHistory()) {
                    BECollection = DALObject.List(CardCode, Date, Relations);
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