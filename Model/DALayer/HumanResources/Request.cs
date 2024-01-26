using BEntities.Filters;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using BE = BEntities;
using BEH = BEntities.HumanResources;

namespace DALayer.HumanResources
{
	public partial class Request
	{

		#region Save Methods

		public async Task UpdateStateAsync(long Id, long NewStateId, long IdUser, DateTime Date)
		{
			string query = $@"UPDATE [HumanResources].[Request] SET [IdState] = {NewStateId}, [LogUser] = {IdUser}, [LogDate] = '{Date:yyyy-MM-dd HH:mm:ss}' WHERE [Id] = {Id}";
			await Connection.ExecuteAsync(query);
		}

		#endregion

		#region Methods

		#endregion

		#region List Methods

		public async Task<IEnumerable<BEH.Request>> ListAsync(List<Field> FilterList, string SortingBy, params Enum[] Relations)
		{
			string filter = FilterList?.Count > 0 ? GetFilterString(FilterList.ToArray()) : "1 = 1",
		query = $@"SELECT  r.*
FROM    HumanResources.Request r
		LEFT OUTER JOIN HumanResources.Vacation v ON r.Id = v.IdRequest
		LEFT OUTER JOIN HumanResources.Travel t ON r.Id = t.IdRequest
		LEFT OUTER JOIN HumanResources.License l ON r.Id = l.IdRequest
		LEFT OUTER JOIN HumanResources.HomeOffice ho ON r.Id = ho.IdRequest
WHERE   {filter}
ORDER BY {SortingBy} ";
			IEnumerable<BEH.Request> Items = await SQLListAsync(query, Relations);
			return Items;
		}

		#endregion

		#region Search Methods

		#endregion

	}
}