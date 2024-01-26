using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using BE = BEntities;
using BEN = BEntities.Campaign;

namespace DALayer.Campaign 
{
	public partial class User 
	{

		#region Save Methods

		#endregion

		#region Methods

		#endregion

		#region List Methods

		#endregion

		#region Search Methods

		public async Task<BEN.User> SearchAsync(string EMail, string Password, long IdCampaign, params Enum[] Relations)
		{
			string query = $@"SELECT * FROM Campaign.[User] WHERE EMail = '{EMail}' AND Password = '{Password}' AND IdCampaign = {IdCampaign} ";
			BEN.User Item = await SQLSearchAsync(query, Relations);
			return Item;
		}

		#endregion

	}
}