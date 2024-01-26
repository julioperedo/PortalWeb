using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using BE = BEntities;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BED = BEntities.AppData;
using BEF = BEntities.Staff;
using BEG = BEntities.Logs;
using BEL = BEntities.Sales;
using BEM = BEntities.Marketing;
using BEO = BEntities.Online;
using BEP = BEntities.Product;
using BES = BEntities.Security;
using BEV = BEntities.Visits;
using BEW = BEntities.WebSite;
using DAL = DALayer.Security;

namespace BComponents.Security
{
	public partial class User
	{
		#region Save Methods 

		public async Task UnforceToLogOffAsync(long Id)
		{
			try
			{
				using DAL.User dal = new();
				await dal.UnforceToLogOffAsync(Id);
			}
			catch (Exception ex)
			{
				base.ErrorHandler(ex);
			}
		}

		#endregion

		#region Methods 

		#endregion

		#region List Methods 

		public List<BES.User> List(string CardCode, string Order, params Enum[] Relations)
		{
			try
			{
				List<BES.User> BECollection;
				List<Field> lstFilter = new() { new Field { Name = "CardCode", Value = CardCode } };

				using (DAL.User DALObject = new())
				{
					BECollection = DALObject.List(lstFilter, Order, Relations).ToList();
				}
				return BECollection;
			}
			catch (Exception ex)
			{
				ErrorHandler(ex);
				return null;
			}
		}

		public Task<List<BES.User>> ListAsync(List<Field> FilterList, string Order, params Enum[] Relations)
		{
			try
			{
				List<BES.User> BECollection;

				using (DAL.User DALObject = new())
				{
					BECollection = DALObject.List(FilterList, Order, Relations).ToList();
				}

				return Task.FromResult<List<BES.User>>(BECollection);
			}
			catch (Exception ex)
			{
				base.ErrorHandler(ex);
				return null;
			}
		}

		public List<BES.User> ListWithTokens(string Filter, string Order, params Enum[] Relations)
		{
			try
			{
				List<BES.User> BECollection;
				using (DAL.User DALObject = new())
				{
					BECollection = DALObject.ListWithTokens(Filter, Order, Relations);
				}

				return BECollection;
			}
			catch (Exception ex)
			{
				base.ErrorHandler(ex);
				return null;
			}
		}

		public List<BES.User> ListWithCheckpoints(DateTime? Since, DateTime? Until)
		{
			try
			{
				List<BES.User> BECollection;
				using (DAL.User DALObject = new())
				{
					BECollection = DALObject.ListWithCheckpoints(Since, Until);
				}
				return BECollection;
			}
			catch (Exception ex)
			{
				base.ErrorHandler(ex);
				return null;
			}
		}

		public IEnumerable<BES.User> ListExtended(List<Field> Filter, string Order, params Enum[] Relations)
		{
			try
			{
				IEnumerable<BES.User> items;
				using (DAL.User DALObject = new())
				{
					items = DALObject.ListExtended(Filter, Order, Relations);
				}
				return items;
			}
			catch (Exception ex)
			{
				base.ErrorHandler(ex);
				return null;
			}
		}

		#endregion

		#region Search Methods 

		public BES.User Search2(string Login, string Pass, params Enum[] Relations)
		{
			BES.User BEObject = null;
			try
			{
				using DAL.User DALObject = new();
				BEObject = DALObject.Search2(Login, Pass, Relations);
			}
			catch (Exception ex)
			{
				ErrorHandler(ex);
			}
			return BEObject;
		}

		public BES.User Search(string EMail, params Enum[] Relations)
		{
			BES.User BEObject = null;

			try
			{
				using DAL.User DALObject = new();
				BEObject = DALObject.Search(EMail, Relations);
			}
			catch (Exception ex)
			{
				ErrorHandler(ex);
			}
			return BEObject;
		}

		public BES.User SearchSeller(string SellerCode, params Enum[] Relations)
		{
			BES.User BEObject = null;

			try
			{
				using DAL.User DALObject = new();
				BEObject = DALObject.SearchSeller(SellerCode, Relations);
			}
			catch (Exception ex)
			{
				ErrorHandler(ex);
			}
			return BEObject;
		}

		public Task<BES.User> SearchAsync(string EMail, params Enum[] Relations)
		{
			BES.User BEObject = null;
			try
			{
				using DAL.User DALObject = new();
				BEObject = DALObject.Search(EMail, Relations);
			}
			catch (Exception ex)
			{
				ErrorHandler(ex);
			}
			return Task.FromResult(BEObject);
		}

		public Task<BES.User> SearchByLoginAsync(string Login, params Enum[] Relations)
		{
			BES.User BEObject = null;
			try
			{
				using DAL.User DALObject = new();
				BEObject = DALObject.SearchByLogin(Login, Relations);
			}
			catch (Exception ex)
			{
				ErrorHandler(ex);
			}
			return Task.FromResult(BEObject);
		}

		public async Task<BES.User> GetUserAsync(long Id)
		{
			BES.User Item = null;
			try
			{
				using DAL.User dal = new();
				Item = await dal.GetUserAsync(Id);
			}
			catch (Exception ex)
			{
				ErrorHandler(ex);
			}
			return Item;
		}

		public async Task<BES.User> GetUserByUsernameAsync(string Login, string Password, bool GetExtra)
		{
			BES.User Item = null;
			try
			{
				using DAL.User dal = new();
				Item = await dal.GetUserByUsernameAsync(Login, Password, GetExtra);
			}
			catch (Exception ex)
			{
				ErrorHandler(ex);
			}
			return Item;
		}

		public BES.User Search(string Login, string Password, params Enum[] Relations)
		{
			BES.User Item = null;
			try
			{
				using DAL.User dal = new();
				Item = dal.Search(Login, Password, Relations);
			}
			catch (Exception ex)
			{
				base.ErrorHandler(ex);
			}
			return Item;
		}

		public async Task<BES.User> GetByEmployeeIdAsync(long Id)
		{
			BES.User Item = null;
			try
			{
				using DAL.User dal = new();
				Item = await dal.GetByEmployeeIdAsync(Id);
			}
			catch (Exception ex)
			{
				ErrorHandler(ex);
			}
			return Item;
		}

		#endregion
	}
}