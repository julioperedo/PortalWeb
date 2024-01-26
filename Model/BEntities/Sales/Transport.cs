using System;
using System.Collections.Generic;
using BE = BEntities;
using BEntities.AppData;
using BEntities.Base;
using BEntities.Logs;
using BEntities.Marketing;
using BEntities.Online;
using BEntities.Product;
using BEntities.SAP;
using BEntities.Security;
using BEntities.Staff;
using BEntities.Visits;
using BEntities.WebSite;

namespace BEntities.Sales
{
    public partial class Transport
    {

        #region Properties 

        public List<string> Values { get; set; }

        public string StringValues { get; set; }

        public bool ValidEmail { get; set; }

        #endregion

        #region Additional Properties 

        #endregion

        #region Contructors 

        #endregion

        #region Override members

        public override string ToString()
        {
            //TODO: Sobreescribir la propiedad mas utilizada
            return base.ToString();
        }

        #endregion
    }
}