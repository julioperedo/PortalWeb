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
    public partial class ClientStatus
    {
        #region Save Methods 

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        public List<BEK.ClientStatus> ListCalculated(string CardCode)
        {
            try
            {
                List<BEK.ClientStatus> Items = null;
                using (DAL.ClientStatus dal = new())
                {
                    Items = dal.ListCalculated(CardCode);
                }
                return Items;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public List<BEK.ClientStatus> ListCalculatedByYear(string CardCode)
        {
            try
            {
                List<BEK.ClientStatus> Items = null;
                using (DAL.ClientStatus dal = new())
                {
                    Items = dal.ListCalculatedByYear(CardCode);
                }
                return Items;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public List<BEK.ClientStatus> ListCalculated()
        {
            try
            {
                List<BEK.ClientStatus> Items = null;
                using (DAL.ClientStatus dal = new())
                {
                    Items = dal.ListCalculated();
                }
                return Items;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public List<BEK.ClientStatus> ListCalculated(int Year)
        {
            try
            {
                List<BEK.ClientStatus> Items = null;
                using (DAL.ClientStatus dal = new())
                {
                    Items = dal.ListCalculated(Year);
                }
                return Items;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public List<BEK.ClientStatus> ListAllClientsCalculated()
        {
            try
            {
                List<BEK.ClientStatus> Items = null;
                using (DAL.ClientStatus dal = new())
                {
                    Items = dal.ListAllClientsCalculated();
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