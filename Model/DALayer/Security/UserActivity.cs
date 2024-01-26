using System;
using System.Collections.Generic;
using System.Data;
using BE = BEntities;
using BES = BEntities.Security;

namespace DALayer.Security 
{
	public partial class UserActivity 
	{

		#region Save Methods

		#endregion

		#region Methods

		#endregion

		#region List Methods

		public IEnumerable<BES.UserActivity> List(long IdUser, string SortingBy, params Enum[] Relations)
		{
			string query = $@"SELECT  s.Name AS ModuleName, a.Name AS ActivityName, ISNULL(ua.Id, 0) AS Id, CAST({IdUser} AS BIGINT) AS IdUser, a.Id AS IdActivity, ISNULL(ua.Permission, 0) AS Permission 
									, CAST(ISNULL(ua.LogUser, 0) AS BIGINT) AS LogUser, ISNULL(ua.LogDate, GETDATE()) AS LogDate 
							FROM    Base.Activity a
									INNER JOIN Base.SModule s ON a.IdSModule = s.Id
									LEFT OUTER JOIN Security.UserActivity ua ON a.Id = ua.IdActivity AND ua.IdUser = {IdUser}
							ORDER BY {SortingBy}";

			IEnumerable<BES.UserActivity> items = SQLList(query, Relations);
			return items;
		}

		#endregion

		#region Search Methods

		public int GetPermission(long UserId, string ActivityName)
		{
			string query = $@"SELECT  CAST(ua.Permission AS INT)
						      FROM    Security.UserActivity ua
						      		  INNER JOIN Base.Activity a ON ua.IdActivity = a.Id
						      WHERE   ua.IdUser = {UserId} AND a.Name = '{ActivityName}' ";
			var permission = SQLScalar<int>(query); 
			return permission;
		}

		#endregion

	}
}