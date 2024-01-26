using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using BE = BEntities;
using BET = BEntities.PostSale;

namespace DALayer.PostSale 
{
	public partial class ServiceCallActivity 
	{

		#region Save Methods

		#endregion

		#region Methods

		#endregion

		#region List Methods

		public List<BET.ServiceCallActivity> ListBySAPCode(int Id, string OrderBy, params Enum[] Relations)
		{
			string query = $@"SELECT	sca.*
							  FROM		PostSale.ServiceCallActivity sca
										INNER join PostSale.ServiceCall sc ON sca.IdServiceCall = sc.Id
						      WHERE		sc.SAPCode = {Id}
							  ORDER BY {OrderBy} ";
			List<BET.ServiceCallActivity> Items = SQLList(query, Relations).AsList(); 
			return Items;
		}

		#endregion

		#region Search Methods

		#endregion

	}
}