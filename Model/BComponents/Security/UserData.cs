using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using BE = BEntities;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BED = BEntities.AppData;
using BEF = BEntities.Staff;
using BEG = BEntities.Logs;
using BEL = BEntities.Sales;
using BEM = BEntities.Marketing;
using BEO = BEntities.Online;
using BEP = BEntities.Product;
using BES = BEntities.Security;
using BEV = BEntities.Visits;
using BEW = BEntities.WebSite;
using DAL = DALayer.Security;

namespace BComponents.Security
{
    public partial class UserData
    {
        #region Save Methods 

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        #endregion

        #region Search Methods 

        public BES.UserData SearchByUser(long IdUser, params Enum[] Relations)
        {
            BES.UserData Item = null;
            try
            {
                using (DAL.UserData DALObject = new DAL.UserData())
                {
                    Item = DALObject.SearchByUser(IdUser, Relations);
                }
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
            }
            return Item;
        }

        public async Task<BES.UserData> SearchByUserAsync(long IdUser, params Enum[] Relations)
        {
            BES.UserData Item = null;
            try
            {
                using (DAL.UserData DALObject = new())
                {
                    Item = await DALObject.SearchByUserAsync(IdUser, Relations);
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