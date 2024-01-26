using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BE = BEntities;
using BES = BEntities.Security;

namespace DALayer.Security
{
    public partial class SessionHistory
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        public List<BES.SessionHistory> List(string CardCode, DateTime Date, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            sbQuery.AppendLine("SELECT  s.* ");
            sbQuery.AppendLine("FROM    [Security].[SessionHistory] s ");
            sbQuery.AppendLine("        INNER JOIN Security.[User] u ON s.IdUser = u.Id AND u.CardCode = @CardCode ");
            sbQuery.AppendLine("WHERE   CAST(s.LogDate AS DATE) = @Date ");
            sbQuery.AppendLine("ORDER By s.LogDate DESC ");

            List<BES.SessionHistory> items = SQLList(sbQuery.ToString(), new { @CardCode = CardCode, @Date = Date.ToString("yyyy-MM-dd") }, Relations).AsList();
            return items;
        }

        #endregion

        #region Search Methods

        #endregion

    }
}