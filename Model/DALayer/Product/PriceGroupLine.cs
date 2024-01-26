using System;
using System.Collections.Generic;
using System.Data;
using BE = BEntities;
using BEP = BEntities.Product;

namespace DALayer.Product
{
    public partial class PriceGroupLine
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        public IEnumerable<BEP.PriceGroupLine> ListByClient(string CardCode, string SortingBy, params Enum[] Relations)
        {
            string query = $@"SELECT  pgl.*
FROM    Product.PriceGroupLine pgl
        INNER JOIN Product.PriceGroupLineClient pglc ON pgl.Id = pglc.IdGroupLine
WHERE   pglc.CardCode = '{CardCode}'
UNION   
SELECT  *
FROM    Product.PriceGroupLine pgl
WHERE   NOT EXISTS ( SELECT * FROM Product.PriceGroupLineClient pglc WHERE pgl.Id = pglc.IdGroupLine )
        AND EXISTS ( SELECT * 
                     FROM   Product.PriceGroup pg
                            INNER JOIN Product.PriceGroupClient pgc1 ON pg.Id = pgc1.IdGroup  
                     WHERE  pgl.IdGroup = pg.Id AND pgc1.CardCode = '{CardCode}' )
ORDER BY {SortingBy} ";
            IEnumerable<BEP.PriceGroupLine> Items = SQLList(query, Relations);
            return Items;
        }

        public IEnumerable<BEP.PriceGroupLine> ListByLine(long LineId, string SortingBy, params Enum[] Relations)
        {
            string query = $@"SELECT  *
FROM    Product.PriceGroupLine pgl
WHERE   pgl.IdLine = {LineId} 
ORDER BY {SortingBy} ";
            IEnumerable<BEP.PriceGroupLine> Items = SQLList(query, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        public BEP.PriceGroupLine SearchByUser(long UserId, params Enum[] Relations)
        {
            string strQuery = $@"SELECT  pgl.*
FROM    Product.PriceGroupLine pgl
        INNER JOIN Product.PriceGroupLineClient pglc ON pgl.Id = pglc.IdGroupLine
WHERE   pglc.CardCode = ( SELECT CardCode FROM Security.[User] u WHERE u.Id = {UserId} ) ";
            BEP.PriceGroupLine Item = SQLSearch(strQuery, Relations);
            return Item;
        }

        #endregion

    }
}