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

namespace BEntities.Security
{
    public partial class User
    {

        #region Properties 

        public string Name => FirstName + (LastName != null && LastName.Trim().Length > 0 ? " " + LastName : "");
        //{
        //    get { return FirstName + (LastName != null && LastName.Trim().Length > 0 ? " " + LastName : ""); }
        //}

        public string ProfileName { get; set; }
        public string ClientName { get; set; }

        //public DateTime? LastNoteDate { get; set; }
        public string ValidForEnabling { get; set; }

        public int SessionCount { get; set; }

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