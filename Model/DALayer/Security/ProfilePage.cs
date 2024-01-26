using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using BE = BEntities;
using BES = BEntities.Security;

namespace DALayer.Security
{
    public partial class ProfilePage
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        public List<BES.ProfilePage> ListByProfile(long IdProfile, string Order, params Enum[] Relations)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($@"SELECT	m.Name AS ModuleName, p.Name AS PageName, CAST(ISNULL(pp.Id, 0) AS BIGINT) AS Id, CAST({IdProfile} AS BIGINT) AS IdProfile, p.Id AS IdPage ");
            sb.AppendLine(@"		, CAST(ISNULL(pp.LogUser, 0) AS BIGINT) AS LogUser, ISNULL(pp.LogDate, GETDATE()) AS LogDate ");
            sb.AppendLine(@"FROM	Base.Page p ");
            sb.AppendLine(@"		INNER JOIN Base.SModule m ON p.IdSModule = m.Id  ");
            sb.AppendLine($@"		LEFT OUTER JOIN Security.ProfilePage pp ON p.Id = pp.IdPage AND IdProfile = {IdProfile} ");
            sb.AppendLine($@"ORDER BY {Order} ");

            List<BES.ProfilePage> Items = SQLList(sb.ToString(), Relations).AsList();
            return Items;
        }

        #endregion

        #region Search Methods

        #endregion

    }
}