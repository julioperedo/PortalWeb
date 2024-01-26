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
    public partial class SubCategory {
        #region Save Methods 

        public void DeleteAll() {
            this.ErrorCollection.Clear();
            try {
                using(TransactionScope BusinessTransaction = base.GenerateBusinessTransaction()) {
                    using(DAL.SubCategory DALObject = new DAL.SubCategory()) {
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

        public List<BED.SubCategory> ListDistinct(long CategoryId, string Order, params Enum[] Relations)
        {
            try
            {
                List<BED.SubCategory> Items;
                using (DAL.SubCategory dal = new DAL.SubCategory())
                {
                    Items = dal.ListDistinct(CategoryId, Order, Relations);
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