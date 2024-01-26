using System;
using System.Collections.Generic;
using System.Transactions;
using BE = BEntities;
using BED = BEntities.AppData;
using BEB = BEntities.Base;
using BEK = BEntities.Kbytes;
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

using DAL = DALayer.Kbytes;

namespace BComponents.Kbytes
{
    public partial class AcceleratorLot
    {
        #region Save Methods 

        public void UpdateQuantity(long Id, int Quantity)
        {
            this.ErrorCollection.Clear();
            try
            {
                using (DAL.AcceleratorLot dal = new DAL.AcceleratorLot())
                {
                    dal.UpdateQuantity(Id, Quantity);
                }
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
            }
        }

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        #endregion

        #region Search Methods 

        public List<BEK.AcceleratorLot> List(string ProductIds, DateTime CurrentDate, params Enum[] Relations)
        {
            List<BEK.AcceleratorLot> Items = null;
            try
            {
                using (DAL.AcceleratorLot dal = new DAL.AcceleratorLot())
                {
                    Items = dal.List(ProductIds, CurrentDate, Relations);
                }
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
            }
            return Items;
        }

        #endregion
    }
}