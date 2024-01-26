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
    public partial class UserPivotConfig {

        #region Save Methods 

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        #endregion

        #region Search Methods 

        public BES.UserPivotConfig SearchByUser(long IdUser, params Enum[] Relations) {
            BES.UserPivotConfig Item = null;
            try {
                using(DAL.UserPivotConfig DALObject = new DAL.UserPivotConfig()) {
                    Item = DALObject.SearchByUser(IdUser, Relations);
                }
            } catch(Exception ex) {
                base.ErrorHandler(ex);
            }
            return Item;
        }

        #endregion
    }
}