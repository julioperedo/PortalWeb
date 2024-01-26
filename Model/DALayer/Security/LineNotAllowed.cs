using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using BE = BEntities;
using BES = BEntities.Security;

namespace DALayer.Security
{
    public partial class LineNotAllowed
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        public List<BES.LineNotAllowed> ListSelected(string CardCode, string Order, params Enum[] Relations)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("SELECT	ISNULL(c.Id, 0) AS Id, '" + CardCode + "' AS CardCode, l.Id AS IdLine, l.Name AS LineName, CAST(ISNULL(c.Id, 0) AS BIT) AS Selected ");
            sb.AppendLine("FROM	    Product.Line l ");
            sb.AppendLine("		    LEFT OUTER JOIN Security.LineNotAllowed c ON l.Id = c.IdLine AND c.CardCode = '" + CardCode + "' ");
            sb.AppendLine("ORDER BY " + Order);

            List<BES.LineNotAllowed> Items = SQLList(sb.ToString(), Relations).AsList();
            return Items;
        }

        public List<BES.LineNotAllowed> ListClients()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine(@"SELECT	DISTINCT CardCode ");
            sb.AppendLine(@"FROM	Security.LineNotAllowed ");

            List<BES.LineNotAllowed> Items = SQLList(sb.ToString()).AsList();
            return Items;
        }

        #endregion

        #region Search Methods

        #endregion

    }
}