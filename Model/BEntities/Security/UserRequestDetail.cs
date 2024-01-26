using BEntities.AppData;
using BEntities.Base;
using BEntities.Logs;
using BEntities.Marketing;
using BEntities.Online;
using BEntities.Product;
using BEntities.Sales;
using BEntities.SAP;
using BEntities.Staff;
using BEntities.Visits;
using BEntities.WebSite;
using System;
using System.Collections.Generic;
using BE = BEntities;

namespace BEntities.Security {
    public partial class UserRequestDetail {

        #region Properties 

        public string StateName { get; set; }
        public string UserName { get; set; }

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