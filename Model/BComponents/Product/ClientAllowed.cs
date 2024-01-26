using System;
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
using DAL = DALayer.Product;

namespace BComponents.Product
{
    public partial class ClientAllowed
    {
        //private BE.Settings.SAPSettings config;
        //public ClientAllowed(IOptions<BE.Settings.SAPSettings> configuration) : base()
        //{
        //    config = configuration.Value;
        //    var xxxx = config.DBSA;
        //}

        #region Save Methods 

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        #endregion

        #region Search Methods 

        public bool IsAllowed(string CardCode)
        {
            bool boAllowed = false;
            try
            {
                using DAL.ClientAllowed DALObject = new();
                boAllowed = DALObject.IsAllowed(CardCode);
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
            }
            return boAllowed;
        }

        #endregion
    }
}