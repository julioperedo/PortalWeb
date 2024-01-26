using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using BE = BEntities;
using BES = BEntities.Security;

namespace DALayer.Security
{
    public partial class ProfileActivity
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        public List<BES.ProfileActivity> ListByProfile(long IdProfile, string Order, params Enum[] Relations)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($@"SELECT	m.Name AS ModuleName, a.Name AS ActivityName, ISNULL(pa.Id, 0) AS Id, CAST({IdProfile} AS BIGINT) AS IdProfile, a.Id AS IdActivity, ISNULL(pa.Permission, 0) AS Permission ");
            sb.AppendLine(@"		, CAST(ISNULL(pa.LogUser, 0) AS BIGINT) AS LogUser, ISNULL(pa.LogDate, GETDATE()) AS LogDate ");
            sb.AppendLine(@"FROM	Base.Activity a ");
            sb.AppendLine(@"		INNER JOIN Base.SModule m ON a.IdSModule = m.Id ");
            sb.AppendLine($@"		LEFT OUTER JOIN Security.ProfileActivity pa ON a.Id = pa.IdActivity AND IdProfile = {IdProfile} ");
            sb.AppendLine($@"ORDER BY {Order} ");

            List<BES.ProfileActivity> Collection = SQLList(sb.ToString(), Relations).AsList();
            return Collection;
        }


        #endregion

        #region Search Methods

        public int GetPermission(long UserId, string ActivityName)
        {
            string query = $@"SELECT  CAST(pa.Permission AS INT)
                              FROM    Security.ProfileActivity pa
                                      INNER JOIN Security.[User] u ON pa.IdProfile = u.IdProfile
                                      INNER JOIN Base.Activity a ON pa.IdActivity = a.Id
                              WHERE   u.Id = {UserId} AND a.Name = '{ActivityName}'";
            var permission = SQLScalar<int>(query); //Connection.ExecuteScalar<int>(query);
            return permission;
        }

        public int GetPermissionByProfile(long ProfileId, string ActivityName)
        {
            string query = $@"SELECT  CAST(pa.Permission AS INT)
                              FROM    Security.ProfileActivity pa
                                      INNER JOIN Base.Activity a ON pa.IdActivity = a.Id
                              WHERE   pa.IdProfile = {ProfileId} AND a.Name = '{ActivityName}'";
            var permission = SQLScalar<int>(query); //Connection.ExecuteScalar<int>(query);
            return permission;
        }

        #endregion

    }
}