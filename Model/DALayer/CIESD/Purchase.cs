using BEntities.Filters;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BE = BEntities;
using BEC = BEntities.CIESD;

namespace DALayer.CIESD
{
    public partial class Purchase
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        public IEnumerable<BEC.Purchase> List2(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            string filter = FilterList?.Count > 0 ? GetFilterString(FilterList.ToArray()) : "1 = 1", query;
            query = $@"SELECT	p.*
					   FROM		CIESD.Purchase p
								INNER JOIN CIESD.Product p1 ON p.IdProduct = p1.Id
					   WHERE	{filter} 
                       ORDER BY {Order} ";
            IEnumerable<BEC.Purchase> Items = SQLList(query, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        #endregion

    }
}