using System;
using System.Collections.Generic;
using System.Transactions;
using BE = BEntities;
using BED = BEntities.AppData;
using BEB = BEntities.Base;
using BEK = BEntities.Kbytes;
using BEG = BEntities.Logs;
using BEI = BEntities.PiggyBank;
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

using DAL = DALayer.PiggyBank;
using System.Threading.Tasks;

namespace BComponents.PiggyBank
{
    public partial class User
    {
        #region Save Methods 

        #endregion

        #region Methods 

        //public async void DeleteUser(string eMail)
        //{
        //    this.ErrorCollection.Clear();
        //    try
        //    {
        //        using TransactionScope BusinessTransaction = base.GenerateBusinessTransaction();
        //        using (DAL.User dal = new())
        //        {
        //            await dal.DeleteUserAsync(eMail);
        //        }
        //        BusinessTransaction.Complete();
        //    }
        //    catch (Exception ex)
        //    {
        //        base.ErrorHandler(ex);
        //    }
        //}

        public async Task Disable(long Id)
        {
            try
            {
                using DAL.User dal = new();
                await dal.Disable(Id);
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
            }
        }

        #endregion

        #region List Methods 

        public IEnumerable<BEI.User> ListResume(string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEI.User> Items;
                using (DAL.User dal = new())
                {
                    Items = dal.ListResume(Order, Relations);
                }
                return Items;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public async Task<IEnumerable<BEI.User>> ListWithTokensAsync(string Filter, string SortingBy, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEI.User> Items;
                using (DAL.User dal = new())
                {
                    Items = await dal.ListWithTokensAsync(Filter, SortingBy, Relations);
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

        public async Task<BEI.User> SearchAsync(string EMail, string Password, params Enum[] Relations)
        {
            BEI.User Item = null;
            try
            {
                using DAL.User dal = new();
                Item = await dal.SearchAsync(EMail, Password, Relations);
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
            }
            return Item;
        }

        public async Task<BEI.User> SearchAsync(string EMail, params Enum[] Relations)
        {
            BEI.User Item = null;
            try
            {
                using DAL.User dal = new();
                Item = await dal.SearchAsync(EMail, Relations);
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