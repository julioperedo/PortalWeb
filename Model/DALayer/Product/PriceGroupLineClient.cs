using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using BE = BEntities;
using BEP = BEntities.Product;

namespace DALayer.Product
{
    public partial class PriceGroupLineClient
    {

        #region Save Methods

        public void Delete(long GroupLineId, string CardCode)
        {
            string query = $"DELETE FROM [Product].[PriceGroupLineClient] WHERE [CardCode] = '{CardCode}' AND [IdGroupLine] = {GroupLineId}";
            Connection.Execute(query);
        }

        #endregion

        #region Methods

        #endregion

        #region List Methods

        public IEnumerable<BEP.PriceGroupLineClient> ListByLine(long LineId, string SortingBy, params Enum[] Relations)
        {
            string query = $@"SELECT  pglc.*
FROM    Product.PriceGroupLineClient pglc
        INNER JOIN Product.PriceGroupLine pgl ON pglc.IdGroupLine = pgl.Id
WHERE   pgl.IdLine = {LineId}
ORDER BY {SortingBy} ";
            IEnumerable<BEP.PriceGroupLineClient> Items = SQLList(query, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        #endregion

    }
}