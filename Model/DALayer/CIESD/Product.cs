using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BE = BEntities;
using BEC = BEntities.CIESD;

namespace DALayer.CIESD
{
    public partial class Product 
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        public IEnumerable<BEC.Product> ListAvailable(string SortBy, params Enum[] Relations)
        {
            string query = $@"SELECT	*
                              FROM		CIESD.Product p
                              WHERE		ISNULL(p.ItemCode, '') <> ''
                                        AND p.Enabled = 1
			                            AND EXISTS ( SELECT	*
							                         FROM	Product.Product p1
							                         WHERE	p.ItemCode = p1.ItemCode
							  			                    AND EXISTS ( SELECT * FROM Product.Price p2 WHERE p1.Id = p2.IdProduct AND p2.IdSudsidiary = 1 AND p2.Regular > 0 ) ) 
                              ORDER BY {SortBy} ";
            IEnumerable<BEC.Product> Items = SQLList(query, Relations);
            return Items;
        }

        public async Task<IEnumerable<BEC.Product>> ListAvailableAsync(string SortBy, params Enum[] Relations)
        {
            string query = $@"SELECT	*
                              FROM		CIESD.Product p
                              WHERE		ISNULL(p.ItemCode, '') <> ''
                                        AND p.Enabled = 1
			                            AND EXISTS ( SELECT	*
							                         FROM	Product.Product p1
							                         WHERE	p.ItemCode = p1.ItemCode
							  			                    AND EXISTS ( SELECT * FROM Product.Price p2 WHERE p1.Id = p2.IdProduct AND p2.Regular > 0 ) ) 
                              ORDER BY {SortBy} ";
            IEnumerable<BEC.Product> Items = await SQLListAsync(query, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        #endregion

    }
}