using System;
using System.Collections.Generic;
using BE = BEntities;
using BEntities.AppData;
using BEntities.Base;
using BEntities.Logs;
using BEntities.Marketing;
using BEntities.Online;
using BEntities.Product;
using BEntities.Sales;
using BEntities.SAP;
using BEntities.Security;
using BEntities.Visits;
using BEntities.WebSite;

namespace BEntities.Staff {
    public partial class Replace {

        #region Properties 

        public string SellerName { get; set; }

        public string ReplacementName { get; set; }

        #endregion

        #region Additional Properties 

        #endregion

        #region Contructors 

        #endregion

        #region Override members

        public override string ToString() {
            //TODO: Sobreescribir la propiedad mas utilizada
            return base.ToString();
        }

        #endregion
    }
}