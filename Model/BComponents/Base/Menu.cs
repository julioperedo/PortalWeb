using System;
using System.Collections.Generic;
using System.Transactions;
using BE = BEntities;
using BED = BEntities.AppData;
using BEB = BEntities.Base;
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

using DAL = DALayer.Base;
using System.Linq;

namespace BComponents.Base {
	public partial class Menu {
        #region Save Methods 

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        public List<BEB.Menu> ListByProfile(long IdProfile, string Order, params Enum[] Relations) {
            try {
                List<BEB.Menu> Items;
                using(DAL.Menu dal = new DAL.Menu()) {
                    Items = dal.ListByProfile(IdProfile, Order, Relations);
                }
                bool boValidate = true;
                while(boValidate) {
                    boValidate = false;
                    foreach(var item in (from i in Items where i.IdPage.HasValue == false select i).ToList()) {
                        if((from i in Items where i.IdParent.HasValue select i.IdParent.Value).Contains(item.Id) == false) {
                            Items.Remove(item);
                            boValidate = true;
                        }
                    }
                }
                return Items;
            } catch(Exception ex) {
                ErrorHandler(ex);
                return null;
            }
        }

        #endregion

        #region Search Methods 

        #endregion
    }
}