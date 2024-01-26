using System;
using System.Collections.Generic;
using System.Data;
using BE = BEntities;
using BEP = BEntities.Product;

namespace DALayer.Product
{
    public partial class PromoBanner
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        #endregion

        #region Search Methods

        public BEP.PromoBanner SearchSuggested(long IdUser, params Enum[] Relations)
        {
            string query = $@"SELECT  TOP 1 pb.*
FROM    Online.Sale s
        INNER JOIN Online.SaleDetail sd ON s.Id = sd.IdSale
        INNER JOIN Product.Product p ON sd.IdProduct = p.Id
        INNER JOIN Product.PromoBannerTrigger pbt ON sd.IdProduct = pbt.IdProduct OR LOWER(p.Category) = LOWER(pbt.Category) OR ( pbt.IdProduct IS NULL AND ISNULL(pbt.Category, '') = '' )
        INNER JOIN Product.PromoBanner pb ON pbt.IdPromo = pb.Id AND pb.Enabled = 1 AND CAST(GETDATE() AS DATE) BETWEEN pb.InitialDate AND ISNULL(pb.FinalDate, GETDATE())           
WHERE   s.IdUser = {IdUser} AND s.StateIdc = 25
ORDER BY NEWID() ";
            BEP.PromoBanner Item = SQLSearch(query, Relations);
            return Item;
        }

        #endregion

    }
}