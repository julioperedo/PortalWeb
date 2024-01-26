using BEntities.Filters;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using BE = BEntities;
using BEI = BEntities.PiggyBank;

namespace DALayer.PiggyBank
{
    public partial class Message
    {

        #region Save Methods

        public async Task<long> SaveAsync(BEI.Message Item)
        {
            long Id = Item.Id;
            string query = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                query = "INSERT INTO [PiggyBank].[Message]([Title], [Body], [Date], [ImageUrl], [LogUser], [LogDate]) VALUES(@Title, @Body, @Date, @ImageUrl, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                query = "UPDATE [PiggyBank].[Message] SET [Title] = @Title, [Body] = @Body, [Date] = @Date, [ImageUrl] = @ImageUrl, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                query = "DELETE FROM [PiggyBank].[Message] WHERE [Id] = @Id";
            }

            if (Item.StatusType != BE.StatusType.NoAction)
            {
                if (Item.StatusType == BE.StatusType.Insert)
                    Id = Convert.ToInt64(await Connection.ExecuteScalarAsync(query, Item));
                else
                    await Connection.ExecuteAsync(query, Item);
            }
            return Id;
        }

        #endregion

        #region Methods

        #endregion

        #region List Methods

        public async Task<IEnumerable<BEI.Message>> ListAsync(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [PiggyBank].[Message] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEI.Message> Items = await SQLListAsync(sbQuery.ToString(), Relations);
            return Items;
        }

        public async Task<IEnumerable<BEI.Message>> ListByUserAsync(long UserId, string SortingBy, params Enum[] Relations)
        {
            string query = $@"SELECT  *
FROM    PiggyBank.Message m
WHERE   m.RecipientsType = 'A' 
        OR EXISTS ( SELECT * FROM PiggyBank.MessageRecipients mr WHERE mr.IdMessage = m.Id AND mr.Recipient = {UserId} ) 
ORDER BY {SortingBy} ";

            IEnumerable<BEI.Message> Items = await SQLListAsync(query, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        public async Task<BEI.Message> SearchAsync(long Id, params Enum[] Relations)
        {
            string query = $"SELECT * FROM [PiggyBank].[Message] WHERE [Id] = {Id} ";
            BEI.Message Item = await SQLSearchAsync(query, Relations);
            return Item;
        }

        #endregion

    }
}