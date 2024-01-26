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
using BEX = BEntities.CIESD;
using BEH = BEntities.HumanResources;
using BEI = BEntities.PiggyBank;
using BEN = BEntities.Campaign;

using DAL = DALayer.PiggyBank;
using System.Threading.Tasks;

namespace BComponents.PiggyBank
{
    public partial class UserToken
    {
        #region Save Methods 

        public async Task<long> SaveAsync(BEI.UserToken Item)
        {
            this.ErrorCollection.Clear();
            long Id = Item.Id;
            if (this.Validate(Item))
            {
                try
                {
                    using DAL.UserToken dal = new();
                    Id = await dal.SaveAsync(Item);
                }
                catch (Exception ex)
                {
                    base.ErrorHandler(ex);
                }
            }
            else
            {
                base.ErrorHandler(new BCException(this.ErrorCollection));
            }
            return Id;
        }

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        #endregion

        #region Search Methods 

        public async Task<BEI.UserToken> SearchAsync(long Id, params Enum[] Relations)
        {
            BEI.UserToken Item = null;
            try
            {
                using DAL.UserToken dal = new();
                Item = await dal.SearchAsync(Id, Relations);
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