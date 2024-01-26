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

using DAL = DALayer.Product;
using System.Threading.Tasks;

namespace BComponents.Product
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
			try
			{
				IEnumerable<string> Items;
				using (DAL.PriceGroupClient dal = new())
				{
					Items = await dal.ListClientsAsync(ListName, LineId, Relations);
				}
				return Items;
			}
			catch (Exception ex)
			{
				base.ErrorHandler(ex);
				return null;
			}
		}

		#endregion

		#region Search Methods 

		#endregion
	}
}