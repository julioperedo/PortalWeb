using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using BE = BEntities;
using BEP = BEntities.Product;

namespace DALayer.Product
{
    public partial class PriceGroup
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        public IEnumerable<BEP.PriceGroup> ListByClient(string CardCode, string SortingBy, params Enum[] Relations)
        {
            string strQuery = $@"SELECT  pg.*
FROM    Product.PriceGroup pg
        INNER JOIN Product.PriceGroupClient pgc ON pg.Id = pgc.IdGroup
WHERE   pgc.CardCode = '{CardCode}'
ORDER BY {SortingBy} ";
            IEnumerable<BEP.PriceGroup> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        public BEP.PriceGroup SearchByUser(long UserId, params Enum[] Relations)
        {
            string strQuery = $@"SELECT  pg.*
FROM    Product.PriceGroup pg
        INNER JOIN Product.PriceGroupClient pgc1 ON pg.Id = pgc1.IdGroup 
WHERE   pgc1.CardCode = ( SELECT CardCode FROM Security.[User] u WHERE u.Id = {UserId} )";
            BEP.PriceGroup Item = SQLSearch(strQuery, Relations);
            return Item;
        }

        #endregion

    }
}