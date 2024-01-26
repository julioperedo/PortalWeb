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
    public partial class ProfilePage {
        #region Save Methods 

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        public List<BES.ProfilePage> ListByProfile(long IdProfile, string Order, params Enum[] Relations) {
            try {
                List<BES.ProfilePage> BECollection;
                using(DAL.ProfilePage DALObject = new DAL.ProfilePage()) {
                    BECollection = DALObject.ListByProfile(IdProfile, Order, Relations);
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