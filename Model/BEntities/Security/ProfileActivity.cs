using BEntities.Base;
using System;
using System.Collections.Generic;
using BE = BEntities;

namespace BEntities.Security
{
    public partial class ProfileActivity
    {

        #region Properties 

        public string Name { get; set; }
        public bool Insert { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }
        public string ModuleName { get; set; }
        public string ActivityName { get; set; }

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