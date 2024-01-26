using BEntities.Filters;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using BE = BEntities;
using BEP = BEntities.Product;

namespace DALayer.Product
{
    public partial class DocumentReceiver
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        public IEnumerable<BEP.DocumentReceiver> ListNotBlackListed(List<Field> FilterList, string SortingBy, params Enum[] Relations)
        {
            string query, filter = FilterList?.Count > 0 ? GetFilterString(FilterList.ToArray()) : "1 = 1";
            query = $@"SELECT	*
					   FROM		Product.DocumentReceiver dr
					   WHERE	NOT EXISTS ( SELECT * FROM Security.MailBlacklist mb WHERE LTRIM(RTRIM(mb.EMail)) = LTRIM(RTRIM(dr.EMail)) ) 
                                AND {filter}
                       ORDER BY {SortingBy} ";

            IEnumerable<BEP.DocumentReceiver> Items = SQLList(query, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        #endregion

    }
}