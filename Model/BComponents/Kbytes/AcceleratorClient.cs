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
    public partial class AcceleratorClient
    {
        #region Save Methods 

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        public List<BEK.AcceleratorClient> List(string CardCode, DateTime CurrentDate, params Enum[] Relations)
        {
            List<BEK.AcceleratorClient> Items = null;
            try
            {
                using (DAL.AcceleratorClient dal = new DAL.AcceleratorClient())
                {
                    Items = dal.List(CardCode, CurrentDate, Relations);
                }
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
            }
            return Items;
        }

        #endregion

        #region Search Methods 

        #endregion
    }
}