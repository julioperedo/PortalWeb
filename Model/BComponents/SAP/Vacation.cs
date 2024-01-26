using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE = BEntities;
using BEA = BEntities.SAP;
using DALH = DALayer.SAP.Hana;

namespace BComponents.SAP
{
	[Serializable()]
	public class Vacation : BCEntity
	{
		#region Search Methods

		public BEA.Vacation Search(string Id, params Enum[] Relations)
		{
			BEA.Vacation Item = null;
			try
			{
				using DALH.Vacation DALObject = new();
				Item = DALObject.Search(Id, Relations);
			}
			catch (Exception ex)
			{
				ErrorHandler(ex);
			}
			return Item;
		}

		public BEA.Vacation SearchDaysTakenByEmployee(int EmployeeId, params Enum[] Relations)
		{
			BEA.Vacation Item = null;
			try
			{
				using DALH.Vacation DALObject = new();
				Item = DALObject.SearchDaysTakenByEmployee(EmployeeId, Relations);
			}
			catch (Exception ex)
			{
				ErrorHandler(ex);
			}
			return Item;
		}

		public BEA.Vacation SearchDaysRejectedByEmployee(int EmployeeId, params Enum[] Relations)
		{
			BEA.Vacation Item = null;
			try
			{
				using DALH.Vacation DALObject = new();
				Item = DALObject.SearchDaysRejectedByEmployee(EmployeeId, Relations);
			}
			catch (Exception ex)
			{
				ErrorHandler(ex);
			}
			return Item;
		}

		#endregion

		#region List Methods

		public IEnumerable<BEA.Vacation> List(List<Field> FilterList, string Order, params Enum[] Relations)
		{
			try
			{
				IEnumerable<BEA.Vacation> BECollection = default;
				using DALH.Vacation DALObject = new();
				BECollection = DALObject.List(FilterList, Order, Relations);
				return BECollection;
			}
			catch (Exception ex)
			{
				ErrorHandler(ex);
				return null;
			}
		}

		#endregion

		#region Save Methods

		public void Save(BEA.Vacation Item)
		{
			try
			{
				using DALH.Vacation dal = new();
				dal.Save(Item);
			}
			catch (Exception ex)
			{
				base.ErrorHandler(ex);
			}
		}

		#endregion

		#region Constructors

		public Vacation() : base() { }

		#endregion
	}
}
