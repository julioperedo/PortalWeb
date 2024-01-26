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
using BEntities.Staff;
using BEntities.WebSite;

namespace BEntities.Visits 
{
	public partial class Visit 
	{

        #region Properties 

        public string ClientName { get; set; }
        public string ClientPhoto { get; set; }
        public string ClientDocId { get; set; }
        public string ClientDocIdRev { get; set; }
        public bool Editable {
            get {
                return DateTime.ParseExact(InitialDate.ToString("yyyy-MM-dd"), "yyyy-M-d", null) >= DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd"), "yyyy-M-d", null);
            }
        }

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