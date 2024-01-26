using System;
using System.Collections.Generic;
using BE = BEntities;
using BEntities.Base;

namespace BEntities
{
    namespace Security
    {

        public partial class ProfilePage
        {

            #region Properties 

            public string Name { get; set; }
            public bool Selected { get; set; }
            public string ModuleName { get; set; }
            public string PageName { get; set; }

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