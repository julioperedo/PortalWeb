using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using BE = BEntities;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BED = BEntities.AppData;
using BEF = BEntities.Staff;
using BEL = BEntities.Sales;
using BEO = BEntities.Online;
using BEP = BEntities.Product;
using BES = BEntities.Security;
using DAL = DALayer.Online;

namespace BComponents.Online
{
    public partial class Sale
    {
        #region Save Methods 

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        #endregion

        #region Search Methods 

        public BEO.Sale SearchCurrent(long IdUser, params Enum[] Relations)
        {
            BEO.Sale BEObject = null;
            try
            {
                using DAL.Sale DALObject = new();
                BEObject = DALObject.SearchCurrent(IdUser, Relations);
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
            }
            return BEObject;
        }

        #endregion
    }
}