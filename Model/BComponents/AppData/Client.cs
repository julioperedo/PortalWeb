using BComponents.Base;
using BComponents.Online;
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
using BEO = BEntities.Online;
using BEP = BEntities.Product;
using BES = BEntities.Security;
using DAL = DALayer.AppData;


namespace BComponents.AppData
{
    public partial class Client
    {
        #region Save Methods 

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        #endregion

        #region Search Methods 

        public BED.Client Search(string Code, params Enum[] Relations)
        {
            BED.Client Item = null;
            try
            {
                using (DAL.Client dal = new DAL.Client())
                {
                    Item = dal.Search(Code, Relations);
                }
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
            }
            return Item;
        }

        #endregion
    }
}