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
	public partial class UserRequestDetail {
        #region Save Methods 

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        public List<BES.UserRequestDetail> List(long IdRequest, string Order) {
            try {
                List<BES.UserRequestDetail> BECollection;
                using(DAL.UserRequestDetail DALObject = new DAL.UserRequestDetail()) {
                    BECollection = DALObject.List(IdRequest, Order);
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