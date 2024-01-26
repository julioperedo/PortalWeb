using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using BE = BEntities;
using BET = BEntities.PostSale;

namespace DALayer.PostSale 
{
	public partial class ServiceCallSolution 
	{

		#region Save Methods

		#endregion

		#region Methods

		#endregion

		#region List Methods

		public List<BET.ServiceCallSolution> ListBySAPCode(int Id, string OrderBy, params Enum[] Relations)
		{
			string query = $@"SELECT	scs.*
							  FROM		PostSale.ServiceCallSolution scs
										INNER join PostSale.ServiceCall sc ON scs.IdServiceCall = sc.Id
						      WHERE		sc.SAPCode = {Id}
							  ORDER BY {OrderBy} ";
			List<BET.ServiceCallSolution> Items = SQLList(query, Relations).ToList();
			return Items;
		}

		#endregion

		#region Search Methods

		#endregion

	}
}