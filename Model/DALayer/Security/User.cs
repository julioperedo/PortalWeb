using BEntities.Filters;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE = BEntities;
using BES = BEntities.Security;

namespace DALayer.Security
{
	public partial class User
	{

		#region Save Methods

		public async Task UnforceToLogOffAsync(long Id)
		{
			string query = $"UPDATE [Security].[User] SET [RequiredLogOff] = 0 WHERE [Id] = {Id} ";
			await Connection.ExecuteAsync(query);
		}

		#endregion

		#region Methods

		#endregion

		#region List Methods

		public List<BES.User> ListWithTokens(string Filter, string Order, params Enum[] Relations)
		{
			DynamicParameters parameters = new();
			var sb = new StringBuilder();
			sb.AppendLine(@"SELECT	* ");
			sb.AppendLine(@"FROM	Security.[User] u ");
			sb.AppendLine(@"WHERE	EXISTS ( SELECT * FROM AppData.UserToken WHERE IdUser = u.Id ) ");
			if (!string.IsNullOrWhiteSpace(Filter))
			{
				sb.AppendLine($@"		AND ( u.FirstName LIKE @Filter OR LastName LIKE @Filter OR CardCode LIKE @Filter ) ");
				parameters.Add("Filter", "%" + Filter.Replace("[", "[[]").Replace("%", "[%]") + "%");
			}
			sb.AppendLine($@"ORDER BY {Order} ");

			List<BES.User> Collection = SQLList(sb.ToString(), parameters, Relations).AsList();
			return Collection;
		}

		public List<BES.User> ListWithCheckpoints(DateTime? Since, DateTime? Until)
		{
			string strFilter = "";
			DynamicParameters parameters = new DynamicParameters();
			if (Since.HasValue)
			{
				strFilter += " AND CAST(CheckDate AS DATE) >= @Since";
				parameters.Add("Since", Since.Value.ToString("yyyy/MM/dd"));
			}
			if (Until.HasValue)
			{
				strFilter += " AND CAST(CheckDate AS DATE) <= @Until";
				parameters.Add("Until", Until.Value.ToString("yyyy/MM/dd"));
			}

			var sb = new StringBuilder();
			sb.AppendLine(@"SELECT	* ");
			sb.AppendLine(@"FROM	Security.[User] u ");
			sb.AppendLine($@"WHERE	EXISTS ( SELECT * FROM AppData.[CheckPoint] WHERE IdGuard = u.Id {strFilter} ) ");
			sb.AppendLine(@"ORDER BY FirstName, LastName ");

			List<BES.User> Items = SQLList(sb.ToString(), parameters).AsList();
			return Items;
		}

		public IEnumerable<BES.User> ListExtended(List<Field> FilterList, string SortingBy, params Enum[] Relations)
		{
			string query, filter = FilterList?.Count > 0 ? GetFilterString(FilterList.ToArray()) : "1 = 1";
			query = $@"SELECT	u.*, ( SELECT COUNT(*) FROM Security.SessionHistory sh WHERE u.Id = sh.IdUser ) AS SessionCount
					   FROM		Security.[User] u
					   WHERE	{filter}
					   ORDER BY {SortingBy} ";

			IEnumerable<BES.User> Items = SQLList(query, Relations);
			return Items;
		}

		#endregion

		#region Search Methods

		public BES.User Search2(string Login, string Pass, params Enum[] Relations)
		{
			string strQuery = $"SELECT * FROM [Security].[User] WHERE [Login] = '{Login}' AND [Password] = '{Pass}' ";
			BES.User BEUser = SQLSearch(strQuery, Relations);
			return BEUser;
		}

		public BES.User Search(string EMail, params Enum[] Relations)
		{
			string strQuery = $"SELECT * FROM [Security].[User] WHERE LOWER([EMail]) = '{EMail.ToLower().Trim()}' ";
			BES.User BEUser = SQLSearch(strQuery, Relations);
			return BEUser;
		}

		public BES.User SearchByLogin(string Login, params Enum[] Relations)
		{
			string strQuery = $"SELECT * FROM [Security].[User] WHERE [Login] = '{Login}' ";
			BES.User BEUser = SQLSearch(strQuery, Relations);
			return BEUser;
		}

		public BES.User SearchSeller(string SellerCode, params Enum[] Relations)
		{
			var sb = new StringBuilder();
			sb.AppendLine("SELECT   u.* ");
			sb.AppendLine("FROM     Security.[User] u ");
			sb.AppendLine("		    INNER JOIN Security.UserData d ON u.Id = d.IdUser ");
			sb.AppendLine($"WHERE	SellerCode = '{SellerCode}' ");

			BES.User Item = SQLSearch(sb.ToString(), Relations);
			return Item;
		}

		public async Task<BES.User> GetUserAsync(long Id)
		{
			string strQuery = $"SELECT * FROM [Security].[User] WHERE Id = {Id} ";
			BES.User Item = await SQLSearchAsync(strQuery);
			return Item;
		}

		public async Task<BES.User> GetUserByUsernameAsync(string Login, string Password, bool GetExtra)
		{
			string strQuery = $"SELECT * FROM [Security].[User] WHERE [Login] = '{Login}' AND [Password] = '{Password}'";
			Enum[] Relations = Array.Empty<Enum>();
			if (GetExtra)
			{
				Relations = new Enum[] { BES.relUser.UserClients, BES.relUser.UserProfiles, BES.relUserProfile.Profile };
			}
			BES.User Item = await SQLSearchAsync(strQuery, Relations);
			//if (Item != null & GetExtra) LoadRelations(ref Item, BES.relUser.UserClients, BES.relUser.UserProfiles, BES.relUserProfile.Profile);
			return Item;
		}

		public BES.User Search(string Login, string Password, params Enum[] Relations)
		{
			string strQuery = $"SELECT * FROM [Security].[User] WHERE [Login] = '{Login}' AND [Password] = '{Password}'";
			BES.User Item = SQLSearch(strQuery, Relations);
			return Item;
		}

		public async Task<BES.User> GetByEmployeeIdAsync(long Id)
		{
			string query = $@"SELECT  u.*
FROM    Security.[User] u
		INNER JOIN Security.UserData ud ON u.Id = ud.IdUser
WHERE   ud.EmployeeId = {Id} ";
			BES.User Item = await SQLSearchAsync(query);
			return Item;
		}

		#endregion

	}
}