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

using DAL = DALayer.Security;

namespace BComponents.Security 
{
	public partial class UserActivity 
	{
        #region Save Methods 

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        public IEnumerable<BES.UserActivity> List(long IdUser, string SortingBy, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BES.UserActivity> Items;
                using (DAL.UserActivity dal = new())
                {
                    Items = dal.List(IdUser, SortingBy, Relations);
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

        public int GetPermission(long UserId, string ActivityName)
        {
            int permission = 0;
            try
            {
                using DAL.UserActivity dal = new();
                permission = dal.GetPermission(UserId, ActivityName);
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
            }
            return permission;
        }

        #endregion
    }
}