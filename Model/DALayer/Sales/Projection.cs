using BEntities.Filters;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BE = BEntities;
using BEL = BEntities.Sales;

namespace DALayer.Sales
{
    public partial class Projection
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        public List<BEL.Projection> List(int InitYear, int InitMonth, int FinalYear, int FinalMonth, string Order, params Enum[] Relations)
        {
            string query = $@"SELECT	*
                              FROM		Sales.Projection p
                              WHERE		DATEFROMPARTS(p.[Year], p.[Month], 15) BETWEEN DATEFROMPARTS({InitYear}, {InitMonth}, 1) AND DATEFROMPARTS({FinalYear}, {FinalMonth}, 28)
                              ORDER BY  {Order} ";
            List<BEL.Projection> Items = SQLList(query, Relations).ToList();
            return Items;
        }

        #endregion

        #region Search Methods

        #endregion

    }
}