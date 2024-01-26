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

using DAL = DALayer.Security;

namespace BComponents.Security
{
    public partial class ProfileActivity
    {
        #region Save Methods 

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        public List<BES.ProfileActivity> ListByProfile(long IdProfile, string Order, params Enum[] Relations)
        {
            try
            {
                List<BES.ProfileActivity> BECollection;
                using (DAL.ProfileActivity DALObject = new())
                {
                    BECollection = DALObject.ListByProfile(IdProfile, Order, Relations);
                }
                return BECollection;
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
                using DAL.ProfileActivity dal = new();
                permission = dal.GetPermission(UserId, ActivityName);
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
            }
            return permission;
        }

        public int GetPermissionByProfile(long ProfileId, string ActivityName)
        {
            int permission = 0;
            try
            {
                using DAL.ProfileActivity dal = new();
                permission = dal.GetPermissionByProfile(ProfileId, ActivityName);
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