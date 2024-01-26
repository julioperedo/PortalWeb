using DALayer.Base;

using DALayer.Security;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using BE = BEntities;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BEP = BEntities.Product;
using BES = BEntities.Security;

namespace DALayer.Product
{
    public partial class VolumePricing
    {

        #region Save Methods

        public void Save2(ref List<BEP.VolumePricing> Items)
        {
            string strQuery = "";

            foreach (BEP.VolumePricing Item in Items)
            {
                if (Item.StatusType == BE.StatusType.Insert)
                {
                    strQuery = "INSERT INTO [Product].[VolumePricing](IdProduct, IdSubsidiary, Quantity, Price, Observations, LogUser, LogDate) VALUES(@IdProduct, @IdSubsidiary, @Quantity, @Price, @Observations, @LogUser, @LogDate) SELECT @@IDENTITY";
                }
                else if (Item.StatusType == BE.StatusType.Update)
                {
                    strQuery = "UPDATE [Product].[VolumePricing] SET IdProduct = @IdProduct, IdSubsidiary = @IdSubsidiary, Quantity = @Quantity, Price = @Price, Observations = @Observations, LogUser = @LogUser, LogDate = @LogDate WHERE Id = @Id";
                }
                else if (Item.StatusType == BE.StatusType.Delete)
                {
                    strQuery = $"DECLARE @USERID BINARY(128);SET @USERID = CAST({Item.LogUser} AS BINARY(128));SET CONTEXT_INFO @USERID; DELETE FROM [Product].[VolumePricing] WHERE Id = @Id";
                }

                if (Item.StatusType != BE.StatusType.NoAction)
                {
                    var parameters = new { @Id = Item.Id, @IdProduct = Item.IdProduct, @IdSubsidiary = Item.IdSubsidiary, @Quantity = Item.Quantity, @Price = Item.Price, @Observations = Item.Observations, @LogUser = Item.LogUser, @LogDate = Item.LogDate };
                    if (Item.StatusType == BE.StatusType.Insert)
                    {
                        Item.Id = Convert.ToInt64(Connection.ExecuteScalar(strQuery, parameters));
                    }
                    else
                    {
                        Connection.Execute(strQuery, parameters);
                    }
                    Item.StatusType = BE.StatusType.NoAction;
                }

            }
        }

        public async Task<long> SaveAsync(BEP.VolumePricing Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Product].[VolumePricing]([IdProduct], [IdSubsidiary], [Quantity], [Price], [Observations], [LogUser], [LogDate]) VALUES(@IdProduct, @IdSubsidiary, @Quantity, @Price, @Observations, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Product].[VolumePricing] SET [IdProduct] = @IdProduct, [IdSubsidiary] = @IdSubsidiary, [Quantity] = @Quantity, [Price] = @Price, [Observations] = @Observations, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Product].[VolumePricing] WHERE [Id] = @Id";
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

        #region Methods

        #endregion

        #region List Methods

        #endregion

        #region Search Methods

        #endregion

    }
}