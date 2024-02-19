using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Data;
using BE = BEntities;
using BEP = BEntities.Product;

namespace DALayer.Product
{
    public partial class Loan
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        public IEnumerable<BEP.Loan> ListExtended(List<Field> FilterList, string SortingBy, params Enum[] Relations)
        {
            string filter = FilterList?.Count > 0 ? GetFilterString(FilterList?.ToArray()) : "1 = 1",
                query = $@"SELECT  l.*
FROM    Product.Loan l
        INNER JOIN Security.[User] u ON l.IdUser = u.Id
        INNER JOIN Product.Product p ON l.IdProduct = p.Id 
WHERE   {filter} 
ORDER BY {SortingBy} ";        

            IEnumerable<BEP.Loan> Items = SQLList(query, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        #endregion

    }
}