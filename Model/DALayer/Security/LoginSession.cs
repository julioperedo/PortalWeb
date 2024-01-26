using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using BE = BEntities;
using BES = BEntities.Security;
using Dapper;

namespace DALayer.Security {
	public partial class LoginSession {

        #region Save Methods

        public async Task<long> SaveAsync(BES.LoginSession Item) {
            long itemId = 0;
            string strQuery = "";
            if(Item.StatusType == BE.StatusType.Insert) {
                strQuery = "INSERT INTO [Security].[LoginSession](UserId, LoginTime, LogoutTime) VALUES(@UserId, @LoginTime, @LogoutTime) SELECT @@IDENTITY";
            } else if(Item.StatusType == BE.StatusType.Update) {
                strQuery = "UPDATE [Security].[LoginSession] SET UserId = @UserId, LoginTime = @LoginTime, LogoutTime = @LogoutTime WHERE Id = @Id";
            } else if(Item.StatusType == BE.StatusType.Delete) {
                strQuery = "DELETE FROM [Security].[LoginSession] WHERE Id = @Id";
            }

            if(Item.StatusType != BE.StatusType.NoAction) {
                var parameters = new { @Id = Item.Id, @UserId = Item.UserId, @LoginTime = Item.LoginTime, @LogoutTime = Item.LogoutTime };

                if(Item.StatusType == BE.StatusType.Insert) {
                    var result = await Connection.ExecuteScalarAsync(strQuery, parameters);
                    itemId = long.Parse(result.ToString());
                } else {
                    await Connection.ExecuteAsync(strQuery, parameters);
                }
                Item.StatusType = BE.StatusType.NoAction;
            }
            return itemId;
        }

        #endregion

        #region Methods

        #endregion

        #region List Methods

        #endregion

        #region Search Methods

        public async Task<BES.LoginSession> GetSessionAsync(long Id) {
            string strQuery = $"SELECT * FROM [Security].[LoginSession] WHERE Id = @Id";
            BES.LoginSession Item = await Connection.QueryFirstOrDefaultAsync<BES.LoginSession>(strQuery, new { @Id = Id });
            return Item;
        }

        #endregion

    }
}