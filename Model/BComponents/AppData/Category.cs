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
    public partial class Category {
        #region Save Methods 

        public void DeleteAll() {
            this.ErrorCollection.Clear();
            try {
                using(TransactionScope BusinessTransaction = base.GenerateBusinessTransaction()) {
                    using(DAL.Category DALObject = new DAL.Category()) {
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

        public List<BED.Category> ListDistinct(string Order, params Enum[] Relations)
        {
            try
            {
                List<BED.Category> Items;
                using (DAL.Category dal = new DAL.Category())
                {
                    Items = dal.ListDistinct(Order, Relations);
                }
                return Items;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        #endregion

        #region Search Methods 

        #endregion
    }
}