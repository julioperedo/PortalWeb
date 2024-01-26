using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using BE = BEntities;
using BEH = BEntities.HumanResources;

namespace DALayer.HumanResources
{
    public partial class Department
    {

        #region Save Methods

        public async Task<long> SaveAsync(BEH.Department Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [HumanResources].[Department]([Name], [Enabled], [LogUser], [LogDate]) VALUES(@Name, @Enabled, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [HumanResources].[Department] SET [Name] = @Name, [Enabled] = @Enabled, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [HumanResources].[Department] WHERE [Id] = @Id";
            }

            if (Item.StatusType != BE.StatusType.NoAction)
            {
                if (Item.StatusType == BE.StatusType.Insert)
                    Item.Id = Convert.ToInt64(await Connection.ExecuteScalarAsync(strQuery, Item));
                else
                    await Connection.ExecuteAsync(strQuery, Item);
            }
            return Item.Id;
        }

        #endregion

        /*#region Methods

        #endregion

        #region List Methods

        #endregion

        #region Search Methods

        #endregion*/

    }
}