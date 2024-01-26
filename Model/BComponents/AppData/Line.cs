using BComponents.Base;
using BComponents.Product;
using BComponents.Sales;
using BComponents.SAP;
using BComponents.Security;
using BComponents.Staff;
using System;
using System.Collections.Generic;
using System.Transactions;
using BE = BEntities;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BED = BEntities.AppData;
using BEF = BEntities.Staff;
using BEL = BEntities.Sales;
using BEP = BEntities.Product;
using BES = BEntities.Security;
using DAL = DALayer.AppData;

namespace BComponents.AppData {
    public partial class Line {
        #region Save Methods 

        public void DeleteAll() {
            this.ErrorCollection.Clear();
            try {
                using(TransactionScope BusinessTransaction = base.GenerateBusinessTransaction()) {
                    using(DAL.Line DALObject = new DAL.Line()) {
                        DALObject.DeleteAll();
                    }
                    BusinessTransaction.Complete();
                }
            } catch(Exception ex) {
                base.ErrorHandler(ex);
            }
        }

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        #endregion

        #region Search Methods 

        #endregion
    }
}