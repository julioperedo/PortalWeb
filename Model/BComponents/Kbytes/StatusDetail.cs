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
    public partial class StatusDetail
    {
        #region Save Methods 

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        public List<BEK.StatusDetail> List(string CardCode, int Year, string Order, params Enum[] Relations)
        {
            try
            {
                List<BEK.StatusDetail> BECollection;
                using (DAL.StatusDetail DALObject = new DAL.StatusDetail())
                {
                    BECollection = DALObject.List(CardCode, Year, Order, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public List<BEK.StatusDetail> ListNotes(string CardCode, int Year, string Order, params Enum[] Relations)
        {
            try
            {
                List<BEK.StatusDetail> BECollection;
                using (DAL.StatusDetail DALObject = new DAL.StatusDetail())
                {
                    BECollection = DALObject.ListNotes(CardCode, Year, Order, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public List<BEK.StatusDetail> ListNotes(string CardCode, string Order, params Enum[] Relations)
        {
            try
            {
                List<BEK.StatusDetail> BECollection;
                using (DAL.StatusDetail DALObject = new DAL.StatusDetail())
                {
                    BECollection = DALObject.ListNotes(CardCode, Order, Relations);
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

        public BEK.StatusDetail SearchLast(string CardCode, int Year, params Enum[] Relations)
        {
            BEK.StatusDetail BEObject = null;

            try
            {
                using (DAL.StatusDetail DALObject = new DAL.StatusDetail())
                {
                    BEObject = DALObject.SearchLast(CardCode, Year, Relations);
                }
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