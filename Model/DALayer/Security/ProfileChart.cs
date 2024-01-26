using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using BE = BEntities;
using BES = BEntities.Security;

namespace DALayer.Security
{
    public partial class ProfileChart
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        public List<BES.ProfileChart> ListByProfile(long IdProfile, string Order, params Enum[] Relations)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($@"SELECT	t.Name AS ChartGroup, c.Name AS ChartName, CAST(ISNULL(pc.Id, 0) AS BIGINT) AS Id, CAST({IdProfile} AS BIGINT) AS IdProfile, c.Id AS IdChart ");
            sb.AppendLine(@"		, CAST(ISNULL(pc.LogUser, 0) AS BIGINT) AS LogUser, ISNULL(pc.LogDate, GETDATE()) AS LogDate ");
            sb.AppendLine(@"FROM	Base.Chart c ");
            sb.AppendLine(@"		INNER JOIN Base.Classifier t ON c.IdcChartType = t.Id ");
            sb.AppendLine($@"		LEFT OUTER JOIN Security.ProfileChart pc ON c.Id = pc.IdChart AND IdProfile = {IdProfile} ");
            sb.AppendLine($@"ORDER BY {Order} ");

            List<BES.ProfileChart> Collection = SQLList(sb.ToString(), Relations).AsList();
            return Collection;
        }

        #endregion

        #region Search Methods

        #endregion

    }
}