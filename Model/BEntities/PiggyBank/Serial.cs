using System;
using System.Collections.Generic;
using BE = BEntities;
using BEntities.AppData;
using BEntities.Base;
using BEntities.Kbytes;
using BEntities.Logs;
using BEntities.Marketing;
using BEntities.Online;
using BEntities.Product;
using BEntities.Sales;
using BEntities.SAP;
using BEntities.Security;
using BEntities.Staff;
using BEntities.Visits;
using BEntities.WebSite;
using BEntities.CIESD;
using BEntities.HumanResources;

namespace BEntities.PiggyBank 
{
    public partial class Serial 
    {

        #region Properties 

        public int SerialsCount { get; set; }
        public int PointsSum { get; set; }
        public string UserName { get; set; }
        public string StoreName { get; set; }
        public string City { get; set; }

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