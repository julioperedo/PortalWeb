using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BE = BEntities;
using BEI = BEntities.PiggyBank;

namespace DALayer.PiggyBank
{
    public partial class MessageRecipients
    {

        #region Save Methods

        public async Task SaveAsync(IEnumerable<BEI.MessageRecipients> Items)
        {
            string query = "";
            IEnumerable<BEI.MessageRecipients> temp = Items.Where(x => x.StatusType == BE.StatusType.Insert);
            if(temp.Any())
            {
                query = "INSERT INTO [PiggyBank].[MessageRecipients]([IdMessage], [Recipient], [LogUser], [LogDate]) VALUES(@IdMessage, @Recipient, @LogUser, @LogDate) ";
               await Connection.ExecuteAsync(query, temp);
            }
            temp = Items.Where(x => x.StatusType == BE.StatusType.Update);
            if (temp.Any())
            {
                query = "UPDATE [PiggyBank].[MessageRecipients] SET [IdMessage] = @IdMessage, [Recipient] = @Recipient, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
               await Connection.ExecuteAsync(query, temp);
            }
            temp = Items.Where(x => x.StatusType == BE.StatusType.Delete);
            if (temp.Any())
            {
                query = "DELETE FROM [PiggyBank].[MessageRecipients] WHERE [Id] = @Id";
                await Connection.ExecuteAsync(query, temp);
            }
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