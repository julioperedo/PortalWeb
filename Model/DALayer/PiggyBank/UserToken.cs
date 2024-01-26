using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using BE = BEntities;
using BEI = BEntities.PiggyBank;

namespace DALayer.PiggyBank
{
    public partial class UserToken
    {

        #region Save Methods

        public async Task<long> SaveAsync(BEI.UserToken Item)
        {
            long Id = Item.Id;
            string query = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                query = "INSERT INTO [PiggyBank].[UserToken]([IdUser], [Token], [LogUser], [LogDate]) VALUES(@IdUser, @Token, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                query = "UPDATE [PiggyBank].[UserToken] SET [IdUser] = @IdUser, [Token] = @Token, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                query = "DELETE FROM [PiggyBank].[UserToken] WHERE [Id] = @Id";
            }

            if (Item.StatusType != BE.StatusType.NoAction)
            {
                if (Item.StatusType == BE.StatusType.Insert)
                    Id = Convert.ToInt64(await Connection.ExecuteScalarAsync(query, Item));
                else
                    await Connection.ExecuteAsync(query, Item);
                Item.StatusType = BE.StatusType.NoAction;
            }
            return Id;
        }

        #endregion

        #region Methods

        #endregion

        #region List Methods

        #endregion

        #region Search Methods

        public async Task<BEI.UserToken> SearchAsync(long UserId, params Enum[] Relations)
        {
            string query = $"SELECT * FROM [PiggyBank].[UserToken] WHERE [IdUser] = {UserId}";
            BEI.UserToken Item = await SQLSearchAsync(query, Relations);
            return Item;
        }

        #endregion

    }
}