using BComponents.Product;
using BComponents.Sales;
using BComponents.SAP;
using BComponents.Security;
using System;
using System.Collections.Generic;
using System.Transactions;
using BE = BEntities;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BEL = BEntities.Sales;
using BEP = BEntities.Product;
using BES = BEntities.Security;
using DAL = DALayer.Base;

namespace BComponents.Base {
    public partial class Event {

        #region Save Methods 

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        public List<BEB.Event> ListLast(params Enum[] Relations) {
            try {
                List<BEB.Event> BECollection;
                using(DAL.Event DALObject = new DAL.Event()) {
                    BECollection = DALObject.ListLast(Relations);
                }
                return BECollection;
            } catch(Exception ex) {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public List<BEB.Event> List(int InitialIndex, int FinalIndex, string Order, params Enum[] Relations) {
            try {
                List<BEB.Event> BECollection;
                using(DAL.Event DALObject = new DAL.Event()) {
                    BECollection = DALObject.List(InitialIndex, FinalIndex, Order, Relations);
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