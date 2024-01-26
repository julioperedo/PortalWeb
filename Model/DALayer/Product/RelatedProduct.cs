using System;
using System.Collections.Generic;
using System.Data;
using BE = BEntities;
using BEP = BEntities.Product;

namespace DALayer.Product
{
    public partial class RelatedProduct
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        public IEnumerable<BEP.RelatedProduct> List(long UserId, string SortingBy, params Enum[] Relations)
        {
            string query = $@"SELECT  DISTINCT rp.*
FROM    Online.Sale s
        INNER JOIN Online.SaleDetail sd ON s.Id = sd.IdSale
        INNER JOIN Product.RelatedProduct rp ON sd.IdProduct = rp.IdProduct
WHERE   s.IdUser = {UserId} AND s.StateIdc = 25 
ORDER BY {SortingBy} ";
            IEnumerable<BEP.RelatedProduct> Items = SQLList(query, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        #endregion

    }
}