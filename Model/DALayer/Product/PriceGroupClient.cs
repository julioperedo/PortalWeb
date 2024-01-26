using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BE = BEntities;

namespace DALayer.Product
{
	public partial class PriceGroupClient
	{

		#region Save Methods

		#endregion

		#region Methods

		#endregion

		#region List Methods

		public async Task<IEnumerable<string>> ListClientsAsync(string ListName, long LineId, params Enum[] Relations)
		{
			string query = $@"SELECT  pgc.CardCode
FROM    Product.PriceGroupClient pgc
		INNER JOIN Product.PriceGroup pg ON pgc.IdGroup = pg.Id
WHERE   LOWER(pg.Name) = '{ListName.ToLower()}'
UNION 
SELECT  pglc.CardCode
FROM    Product.PriceGroupLineClient pglc
		INNER JOIN Product.PriceGroupLine pgl ON pglc.IdGroupLine = pgl.Id
		INNER JOIN Product.PriceGroup pg ON pgl.IdGroup = pg.Id
WHERE   pgl.IdLine = {LineId} AND LOWER(pg.Name) = '{ListName.ToLower()}' ";
			IEnumerable<BE.Product.PriceGroupClient> Items = await SQLListAsync(query, Relations);
			return Items.Select(x => x.CardCode);
		}

		#endregion

		#region Search Methods

		#endregion

	}
}