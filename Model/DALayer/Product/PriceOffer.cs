using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using BE = BEntities;
using BEP = BEntities.Product;

namespace DALayer.Product
{
    public partial class PriceOffer
    {

        #region Save Methods

        public async Task SaveAsync(BEP.PriceOffer Item)
        {
            string query = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                query = "INSERT INTO [Product].[PriceOffer]([IdProduct], [IdSubsidiary], [Price], [Description], [Enabled], [Since], [Until], [OnlyWithStock], [LogUser], [LogDate]) VALUES(@IdProduct, @IdSubsidiary, @Price, @Description, @Enabled, @Since, @Until, @OnlyWithStock, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                query = "UPDATE [Product].[PriceOffer] SET [IdProduct] = @IdProduct, [IdSubsidiary] = @IdSubsidiary, [Price] = @Price, [Description] = @Description, [Enabled] = @Enabled, [Since] = @Since, [Until] = @Until, [OnlyWithStock] = @OnlyWithStock, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                query = "DELETE FROM [Product].[PriceOffer] WHERE [Id] = @Id";
            }

            if (Item.StatusType != BE.StatusType.NoAction)
            {
                if (Item.StatusType == BE.StatusType.Insert)
                    Item.Id = Convert.ToInt64(await Connection.ExecuteScalarAsync(query, Item));
                else
                    await Connection.ExecuteAsync(query, Item);
            }
        }

        #endregion

        #region Methods

        #endregion

        #region List Methods

        public IEnumerable<BEP.PriceOffer> ListByLine(long LineId, string SortingBy, params Enum[] Relations)
        {
            string query = $@"SELECT  po.*
FROM    Product.PriceOffer po
        INNER JOIN Product.Product p ON po.IdProduct = p.Id
WHERE   po.Price > 0 AND po.Enabled = 1 AND ( ( po.Since IS NULL OR po.Since <= CAST(GETDATE() AS DATE) ) AND ( po.Until IS NULL OR po.Until >= CAST(GETDATE() AS DATE) ) )
        AND EXISTS ( SELECT * FROM Product.LineDetail ld WHERE ld.IdLine = {LineId} AND ISNULL(p.Line, '') = ld.SAPLine )
ORDER BY {SortingBy} ";
            IEnumerable<BEP.PriceOffer> Items = SQLList(query, Relations);
            return Items;
        }

        public IEnumerable<BEP.PriceOffer> ListEnabled(IEnumerable<long> IdProducts, string SortingBy, params Enum[] Relations)
        {
            string strQuery = $@"SELECT  po.*
FROM    Product.PriceOffer po
        INNER JOIN Product.Product p ON po.IdProduct = p.Id
WHERE   po.Enabled = 1 AND ( ( po.Since IS NULL OR po.Since <= CAST(GETDATE() AS DATE) ) AND ( po.Until IS NULL OR po.Until >= CAST(GETDATE() AS DATE) ) ) AND po.IdProduct IN ( {string.Join(",", IdProducts)} )
ORDER BY {SortingBy} ";
            IEnumerable<BEP.PriceOffer> Collection = SQLList(strQuery, Relations);
            return Collection;
        }

        #endregion

        #region Search Methods

        #endregion

    }
}