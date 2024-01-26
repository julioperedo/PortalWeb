using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using BE = BEntities;
using BES = BEntities.Security;

namespace DALayer.Security {
    public partial class UserPivotConfig {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        #endregion

        #region Search Methods

        public BES.UserPivotConfig SearchByUser(long IdUser, params Enum[] Relations) {
            string strQuery = $"SELECT * FROM [Security].[UserPivotConfig] WHERE IdUser = {IdUser} ";
            BES.UserPivotConfig Item = Connection.QueryFirstOrDefault<BES.UserPivotConfig>(strQuery);

            if(Item != null) LoadRelations(ref Item, Relations);
            return Item;
        }

        #endregion

    }
}