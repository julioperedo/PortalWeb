using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using BE = BEntities;
using BEG = BEntities.Logs;

namespace DALayer.Logs
{
    public partial class ContactMail
    {

        #region Save Methods

        //public async Task<long> SaveAsync(BEG.ContactMail Item)
        //{
        //    string strQuery = "";
        //    long Id = Item.Id;
        //    if (Item.StatusType == BE.StatusType.Insert)
        //    {
        //        strQuery = "INSERT INTO [Logs].[ContactMail]([Name], [EMail], [Company], [Category], [Position], [City], [Address], [Phone], [NIT], [ClientType], [Message], [LogUser], [LogDate]) VALUES(@Name, @EMail, @Company, @Category, @Position, @City, @Address, @Phone, @NIT, @ClientType, @Message, @LogUser, @LogDate) SELECT @@IDENTITY";
        //    }
        //    else if (Item.StatusType == BE.StatusType.Update)
        //    {
        //        strQuery = "UPDATE [Logs].[ContactMail] SET [Name] = @Name, [EMail] = @EMail, [Company] = @Company, [Category] = @Category, [Position] = @Position, [City] = @City, [Address] = @Address, [Phone] = @Phone, [NIT] = @NIT, [ClientType] = @ClientType, [Message] = @Message, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
        //    }
        //    else if (Item.StatusType == BE.StatusType.Delete)
        //    {
        //        strQuery = "DELETE FROM [Logs].[ContactMail] WHERE [Id] = @Id";
        //    }

        //    if (Item.StatusType != BE.StatusType.NoAction)
        //    {
        //        if (Item.StatusType == BE.StatusType.Insert)
        //        {
        //            var result = await Connection.ExecuteScalarAsync(strQuery, Item);
        //            Id = Convert.ToInt64(result);
        //        }                    
        //        else
        //            await Connection.ExecuteAsync(strQuery, Item);
        //        Item.StatusType = BE.StatusType.NoAction;
        //    }
        //    return Id;
        //}

        #endregion

        #region Methods

        #endregion

        #region List Methods

        #endregion

        #region Search Methods

        #endregion

    }
}