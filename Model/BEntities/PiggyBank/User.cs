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
using System.Net.Security;

namespace BEntities.PiggyBank
{
    public partial class User
    {

        #region Properties 

        public int Points { get; set; }

        public int SerialsCount { get; set; }

        #endregion

        #region Additional Properties 

        #endregion

        #region Contructors 

        public User(string Name, string StoreName, string EMail, string Password, string City, string Address, string Phone, bool ForInserting = false)
        {
            this.Name = Name;
            this.StoreName = StoreName;
            this.EMail = EMail;
            this.Password = Password;
            this.City = City;
            this.Address = Address;
            this.Phone = Phone;
            this.Enabled = true;
            this.LogDate = DateTime.Now;
            if (ForInserting) this.StatusType = StatusType.Insert;
        }

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