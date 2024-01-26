using BEntities.AppData;
using BEntities.Base;
using BEntities.Logs;
using BEntities.Marketing;
using BEntities.Online;
using BEntities.Product;
using BEntities.Sales;
using BEntities.SAP;
using BEntities.Security;
using BEntities.Staff;
using BEntities.WebSite;
using System;
using System.Collections.Generic;
using BE = BEntities;


namespace BEntities.Visits {
    public partial class Person {

        #region Properties 

        public string FullName {
            get {
                return $"{FirstName} {LastName}";
            }
        }

        public string FullName2 {
            get {
                return $"{LastName} {FirstName}";
            }
        }

        public string PhotoURL { get; set; }
        public string DocIdURL { get; set; }
        public string DocIdRevURL { get; set; }
        public bool HasUser { get; set; }

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