using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using BE = BEntities;
using BEL = BEntities.Sales;

namespace DALayer.Sales
{
    public partial class Transport
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        public List<BEL.Transport> List(string OrderCodes, string Order, params Enum[] Relations)
        {
            string query = $@"SELECT	t.*, td.Code AS StringValues
				   		  	  FROM		Sales.Transport t
				   	  	  				INNER JOIN Sales.TransportDetail td ON t.Id = td.TransportId
				   		  	  WHERE		td.Code IN ( {OrderCodes} )
				   			  ORDER BY {Order} ";
            List<BEL.Transport> Items = SQLList(query, Relations).ToList();
            return Items;
        }

        #endregion

        #region Search Methods

        #endregion

    }
}