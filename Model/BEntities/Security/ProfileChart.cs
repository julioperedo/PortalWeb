using BEntities.Base;
using System;
using System.Collections.Generic;
using BE = BEntities;
//using BEntities.Product;
//using BEntities.SAP;


namespace BEntities.Security {
    public partial class ProfileChart {

        #region Properties 

        public string Name { get; set; }
        public bool Selected { get; set; }
        public string ChartGroup { get; set; }
        public string ChartName { get; set; }

        #endregion

        #region Additional properties 

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