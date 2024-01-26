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
using System.Threading.Tasks;

namespace BComponents.Security {
    public partial class LoginSession {
        #region Save Methods 

        public async Task<long> SaveAsync(BES.LoginSession Item) {
            long itemId = 0;
            this.ErrorCollection.Clear();
            if(this.Validate(Item)) {
                try {
                    using(DAL.LoginSession dal = new DAL.LoginSession()) {
                        itemId = await dal.SaveAsync(Item);
                    }
                } catch(Exception ex) {
                    base.ErrorHandler(ex);
                }
            } else {
                base.ErrorHandler(new BCException(this.ErrorCollection));
            }
            return itemId;
        }

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        #endregion

        #region Search Methods 

        public async Task<BES.LoginSession> GetSessionAsync(long Id) {
            BES.LoginSession Item = null;
            try {
                using(DAL.LoginSession dal = new DAL.LoginSession()) {
                    Item = await dal.GetSessionAsync(Id);
                }
            } catch(Exception ex) {
                base.ErrorHandler(ex);
            }
            return Item;
        }

        #endregion
    }
}