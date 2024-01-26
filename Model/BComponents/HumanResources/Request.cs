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

using DAL = DALayer.HumanResources;
using System.Threading.Tasks;
using BEntities.Filters;

namespace BComponents.HumanResources
{
    public partial class Request
    {
        #region Save Methods 

        public async Task UpdateStateAsync(long Id, long NewStateId, long IdUser, DateTime Date)
        {
            try
            {
                using DAL.Request dal = new();
                await dal.UpdateStateAsync(Id, NewStateId, IdUser, Date);
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

        public async Task<IEnumerable<BEH.Request>> ListAsync(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEH.Request> Items;
                using (DAL.Request dal = new())
                {
                    Items = await dal.ListAsync(FilterList, Order, Relations);
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