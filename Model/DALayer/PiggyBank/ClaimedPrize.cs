using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using BE = BEntities;
using BEI = BEntities.PiggyBank;

namespace DALayer.PiggyBank
{
    public partial class ClaimedPrize
    {

        #region Save Methods

        public async Task<long> SaveAsync(BEI.ClaimedPrize Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [PiggyBank].[ClaimedPrize]([IdPrize], [IdUser], [ClaimDate], [Quantity], [Points], [LogUser], [LogDate]) VALUES(@IdPrize, @IdUser, @ClaimDate, @Quantity, @Points, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [PiggyBank].[ClaimedPrize] SET [IdPrize] = @IdPrize, [IdUser] = @IdUser, [ClaimDate] = @ClaimDate, [Quantity] = @Quantity, [Points] = @Points, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [PiggyBank].[ClaimedPrize] WHERE [Id] = @Id";
            }

            if (Item.StatusType != BE.StatusType.NoAction)
            {
                if (Item.StatusType == BE.StatusType.Insert)
                    Item.Id = Convert.ToInt64(await Connection.ExecuteScalarAsync(strQuery, Item));
                else
                    await Connection.ExecuteAsync(strQuery, Item);
                Item.StatusType = BE.StatusType.NoAction;
            }
            return Item.Id;
        }

        #endregion

        #region Methods

        #endregion

        #region List Methods

        #endregion

        #region Search Methods

        #endregion

    }
}