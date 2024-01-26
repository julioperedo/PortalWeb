using System;
using System.Collections.Generic;
using System.Data;
using BE = BEntities;
using BEP = BEntities.Product;

namespace DALayer.Product
{
    public partial class RelatedCategory
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        public IEnumerable<BEP.RelatedCategory> List(long UserId, string SortingBy, params Enum[] Relations)
        {
            string query = $@"SELECT  DISTINCT rc.*
FROM    Online.Sale s
        INNER JOIN Online.SaleDetail sd ON s.Id = sd.IdSale
        INNER JOIN Product.Product p ON sd.IdProduct = p.Id
        INNER JOIN Product.RelatedCategory rc ON p.Category = rc.Category
WHERE   s.IdUser = {UserId} AND s.StateIdc = 25 
ORDER BY {SortingBy} ";
            IEnumerable<BEP.RelatedCategory> Items = SQLList(query, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        #endregion

    }
}