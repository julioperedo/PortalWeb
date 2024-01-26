using BComponents.Base;
using BComponents.Security;
using BEntities.Filters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Transactions;
using BE = BEntities;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BES = BEntities.Security;
using DALH = DALayer.SAP.Hana;

namespace BComponents.SAP
{
	[Serializable()]
	public class Client : BCEntity
	{
		#region Search Methods

		public BEA.Client Search(string CardCode, params Enum[] Relations)
		{
			BEA.Client BEObject = null;
			try
			{
				using DALH.Client DALObject = new();
				BEObject = DALObject.Search(CardCode, Relations);
			}
			catch (Exception ex)
			{
				ErrorHandler(ex);
			}
			return BEObject;
		}

		public BEA.ClientStatics SearchStaticts(string CardCode)
		{
			BEA.ClientStatics BEObject = null;
			try
			{
				using DALH.Client DALObject = new();
				BEObject = DALObject.SearchStaticts(CardCode);
			}
			catch (Exception ex)
			{
				ErrorHandler(ex);
			}
			return BEObject;
		}

		public BEA.Client SearchAmount(string CardCode, DateTime Since, params Enum[] Relations)
		{
			BEA.Client BEObject = null;
			try
			{
				using DALH.Client DALObject = new();
				BEObject = DALObject.SearchAmount(CardCode, Since, Relations);
			}
			catch (Exception ex)
			{
				ErrorHandler(ex);
			}
			return BEObject;
		}

		public BEA.ClientHoldInfo SearchHoldInfo(string CardCode)
		{
			BEA.ClientHoldInfo item = null;
			try
			{
				using DALH.Client DALObject = new();
				item = DALObject.SearchHoldInfo(CardCode);
			}
			catch (Exception ex)
			{
				ErrorHandler(ex);
			}
			return item;
		}

		#endregion

		#region List Methods

		public IEnumerable<BEA.Client> List(List<Field> FilterList, string Order, params Enum[] Relations)
		{
			try
			{
				IEnumerable<BEA.Client> BECollection = default;
				using (DALH.Client DALObject = new())
				{
					BECollection = DALObject.List(FilterList, Order, Relations);
				}
				return BECollection;
			}
			catch (Exception ex)
			{
				ErrorHandler(ex);
				return null;
			}
		}

		public IEnumerable<BEA.Client> ListBalance(List<Field> FilterList, string Order, params Enum[] Relations)
		{
			try
			{
				IEnumerable<BEA.Client> BECollection = default(IEnumerable<BEA.Client>);
				using (DALH.Client DALObject = new DALH.Client())
				{
					BECollection = DALObject.ListBalance(FilterList, Order, Relations);
				}
				return BECollection;
			}
			catch (Exception ex)
			{
				ErrorHandler(ex);
				return null;
			}
		}

		public IEnumerable<BEA.Client> ListValidNames(List<string> CardCodes, params Enum[] Relations)
		{
			try
			{
				IEnumerable<BEA.Client> BECollection = default(IEnumerable<BEA.Client>);
				using (DALH.Client DALObject = new DALH.Client())
				{
					BECollection = DALObject.ListValidNames(CardCodes, Relations);
				}
				return BECollection;
			}
			catch (Exception ex)
			{
				ErrorHandler(ex);
				return null;
			}
		}

		public IEnumerable<BEA.Client> ListValidEMails(List<string> CardCodes, params Enum[] Relations)
		{
			try
			{
				IEnumerable<BEA.Client> BECollection = default(IEnumerable<BEA.Client>);
				using (DALH.Client DALObject = new DALH.Client())
				{
					BECollection = DALObject.ListValidEMails(CardCodes, Relations);
				}
				return BECollection;
			}
			catch (Exception ex)
			{
				ErrorHandler(ex);
				return null;
			}
		}

		public IEnumerable<BEA.Client> ListTopClientsPeriod(DateTime Since, DateTime? Until, decimal MinAmount, string Order, params Enum[] Relations)
		{
			try
			{
				IEnumerable<BEA.Client> BECollection = default(IEnumerable<BEA.Client>);
				using (DALH.Client DALObject = new DALH.Client())
				{
					BECollection = DALObject.ListTopClientsPeriod(Since, Until, MinAmount, Order, Relations);
				}
				return BECollection;
			}
			catch (Exception ex)
			{
				ErrorHandler(ex);
				return null;
			}
		}

		public IEnumerable<BEA.Client> ListDell(int Year, int Month, params Enum[] Relations)
		{
			try
			{
				IEnumerable<BEA.Client> BECollection = default;
				using (DALH.Client DALObject = new DALH.Client())
				{
					BECollection = DALObject.ListDell(Year, Month, Relations);
				}
				return BECollection;
			}
			catch (Exception ex)
			{
				ErrorHandler(ex);
				return null;
			}
		}

		public IEnumerable<BEA.Client> ListShort(List<Field> FilterList, string Order)
		{
			try
			{
				IEnumerable<BEA.Client> BECollection = default;
				using (DALH.Client DALObject = new())
				{
					BECollection = DALObject.ListShort(FilterList, Order);
				}
				return BECollection;
			}
			catch (Exception ex)
			{
				base.ErrorHandler(ex);
				return null;
			}
		}

		public async Task<IEnumerable<BEA.Client>> ListShortAsync(List<Field> FilterList, string Order)
		{
			try
			{
				IEnumerable<BEA.Client> BECollection = default;
				using (DALH.Client DALObject = new())
				{
					BECollection = await DALObject.ListShortAsync(FilterList, Order);
				}
				return BECollection;
			}
			catch (Exception ex)
			{
				base.ErrorHandler(ex);
				return null;
			}
		}

		public IEnumerable<BEA.Client> ListShort2(List<Field> FilterList, string Order)
		{
			try
			{
				IEnumerable<BEA.Client> BECollection = default;
				using (DALH.Client DALObject = new())
				{
					BECollection = DALObject.ListShort2(FilterList, Order);
				}
				return BECollection;
			}
			catch (Exception ex)
			{
				base.ErrorHandler(ex);
				return null;
			}
		}

		public IEnumerable<BEA.Client> ListShort3(List<Field> FilterList, string Order)
		{
			try
			{
				IEnumerable<BEA.Client> BECollection = default;
				using (DALH.Client DALObject = new())
				{
					BECollection = DALObject.ListShort3(FilterList, Order);
				}
				return BECollection;
			}
			catch (Exception ex)
			{
				base.ErrorHandler(ex);
				return null;
			}
		}

		public IEnumerable<BEA.ClientExtra> ListExtras(string CardCode)
		{
			try
			{
				IEnumerable<BEA.ClientExtra> BECollection = default;
				using (DALH.Client DALObject = new())
				{
					BECollection = DALObject.ListExtras(CardCode);
				}
				return BECollection;
			}
			catch (Exception ex)
			{
				base.ErrorHandler(ex);
				return null;
			}
		}

		public IEnumerable<BEA.ClientResumeDebt> ListDebts(DateTime ToDate, string ClientCode)
		{
			try
			{
				IEnumerable<BEA.ClientResumeDebt> items = default;
				using (DALH.Client DALObject = new())
				{
					items = DALObject.ListDebts(ToDate, ClientCode);
				}
				return items;
			}
			catch (Exception ex)
			{
				base.ErrorHandler(ex);
				return new List<BEA.ClientResumeDebt>();
			}
		}

		public IEnumerable<BEA.ClientStatics> ListYears(string ClientCode)
		{
			try
			{
				IEnumerable<BEA.ClientStatics> items = default;
				using (DALH.Client DALObject = new())
				{
					items = DALObject.ListYears(ClientCode);
				}
				return items;
			}
			catch (Exception ex)
			{
				base.ErrorHandler(ex);
				return new List<BEA.ClientStatics>();
			}
		}

		public IEnumerable<BEA.ClientStatics> ListMonths(string ClientCode, int Year)
		{
			try
			{
				IEnumerable<BEA.ClientStatics> items = default;
				using (DALH.Client DALObject = new DALH.Client())
				{
					items = DALObject.ListMonths(ClientCode, Year);
				}
				return items;
			}
			catch (Exception ex)
			{
				base.ErrorHandler(ex);
				return new List<BEA.ClientStatics>();
			}
		}

		public IEnumerable<BEA.Client> ListResumeAmounts(string CardCode, DateTime Since, params Enum[] Relations)
		{
			try
			{
				IEnumerable<BEA.Client> items = default;
				using (DALH.Client DALObject = new())
				{
					items = DALObject.ListResumeAmounts(CardCode, Since, Relations);
				}
				return items;
			}
			catch (Exception ex)
			{
				base.ErrorHandler(ex);
				return new List<BEA.Client>();
			}
		}

		#endregion

		#region Constructors

		public Client() : base() { }

		#endregion
	}
}