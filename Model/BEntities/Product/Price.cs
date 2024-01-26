using System;
using System.Collections.Generic;
using BE = BEntities;
using BEntities.AppData;
using BEntities.Base;
using BEntities.Logs;
using BEntities.Marketing;
using BEntities.Online;
using BEntities.Sales;
using BEntities.SAP;
using BEntities.Security;
using BEntities.Staff;
using BEntities.Visits;
using BEntities.WebSite;

namespace BEntities.Product
{
    public partial class Price
    {

        #region Properties 

        #endregion

        #region Methods 

        public bool HasData()
        {
            return Regular > 0 | (ClientSuggested.HasValue && ClientSuggested.Value > 0) | !string.IsNullOrWhiteSpace(Observations) | !string.IsNullOrWhiteSpace(Commentaries);
        }

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