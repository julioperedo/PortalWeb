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

using DAL = DALayer.Base;
using System.Threading.Tasks;

namespace BComponents.Base
{
    public partial class DivisionConfig
    {
        #region Save Methods 

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        public async Task<List<BEB.DivisionConfig>> ListAsync(string Order, params Enum[] Relations)
        {
            try
            {
                List<BEB.DivisionConfig> Items;
                using (DAL.DivisionConfig dal = new DAL.DivisionConfig())
                {
                    Items = await dal.ListAsync(Order, Relations);
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