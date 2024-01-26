using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using BE = BEntities;
using BES = BEntities.Security;

namespace DALayer.Security
{
    public partial class UserData
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        #endregion

        #region Search Methods

        public BES.UserData SearchByUser(long IdUser, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Security].[UserData] WHERE IdUser = @IdUser";
            BES.UserData Item = Connection.QueryFirstOrDefault<BES.UserData>(strQuery, new { @IdUser = IdUser });
            if (Item != null) LoadRelations(ref Item, Relations);
            return Item;
        }

        public async Task<BES.UserData> SearchByUserAsync(long IdUser, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Security].[UserData] WHERE IdUser = {IdUser} ";
            BES.UserData Item = await Connection.QueryFirstOrDefaultAsync<BES.UserData>(strQuery);
            if (Item != null) LoadRelations(ref Item, Relations);
            return Item;
        }

        #endregion

    }
}