using System;
using System.Collections.Generic;
using BE = BEntities;
//using BEntities.Product;
//using BEntities.SAP;
using BEntities.Security;


namespace BEntities
{
    namespace Base
    {

        public partial class SModule
        {

            #region Properties 

            public List<ProfileActivity> CollectionSelectedActivities { get; set; }

            #endregion

            #region Additional properties 

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
}