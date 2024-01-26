using System;
using System.Collections.Generic;
using System.Transactions;
using BE = BEntities;
using BED = BEntities.AppData;
using BEB = BEntities.Base;
using BEK = BEntities.Kbytes;
using BEG = BEntities.Logs;
using BEM = BEntities.Marketing;
using BEO = BEntities.Online;
using BEP = BEntities.Product;
using BEL = BEntities.Sales;
using BEA = BEntities.SAP;
using BES = BEntities.Security;
using BEF = BEntities.Staff;
using BEV = BEntities.Visits;
using BEW = BEntities.WebSite;
using BEX = BEntities.CIESD;
using BEH = BEntities.HumanResources;
using BEI = BEntities.PiggyBank;
using BEN = BEntities.Campaign;

using DAL = DALayer.Campaign;
using System.Threading.Tasks;

namespace BComponents.Campaign 
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
			BEN.User Item = null;
			try
			{
				using DAL.User dal = new();
				Item = await dal.SearchAsync(EMail, Password, IdCampaign, Relations);
			}
			catch (Exception ex)
			{
				base.ErrorHandler(ex);
			}
			return Item;
		}

		#endregion
	}
}