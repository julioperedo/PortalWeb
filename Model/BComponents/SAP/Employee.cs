using BEntities.Filters;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Vml.Office;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEA = BEntities.SAP;
using DALH = DALayer.SAP.Hana;

namespace BComponents.SAP
{
	[Serializable()]
	public class Employee : BCEntity
	{
		#region Search Methods

		public BEA.Employee Search(long Id, params Enum[] Relations)
		{
			BEA.Employee Item = null;
			try
			{
				using DALH.Employee DALObject = new();
				Item = DALObject.Search(Id, Relations);
			}
			catch (Exception ex)
			{
				base.ErrorHandler(ex);
			}
			return Item;
		}


		public int GetLastId()
		{
			int Id = 0;
			try
			{
				using DALH.Employee DALObject = new();
				Id = DALObject.GetLastId();
			}
			catch (Exception ex)
			{
				base.ErrorHandler(ex);
			}
			return Id;
		}

		public BEA.EmployeeResume SearchDaysTaken(long EmployeeId, params Enum[] Relations)
		{
			BEA.EmployeeResume Item = null;
			try
			{
				using DALH.Employee DALObject = new();
				Item = DALObject.SearchDaysTaken(EmployeeId, Relations);
			}
			catch (Exception ex)
			{
				base.ErrorHandler(ex);
			}
			return Item;
		}

		#endregion

		#region List Methods

		public IEnumerable<BEA.Employee> List(List<Field> Filters, string SortingBy, params Enum[] Relations)
		{
			try
			{
				IEnumerable<BEA.Employee> Items = default;
				using DALH.Employee DALObject = new();
				Items = DALObject.List(Filters, SortingBy, Relations);
				return Items;
			}
			catch (Exception ex)
			{
				ErrorHandler(ex);
				return null;
			}
		}

		public IEnumerable<BEA.EmployeeResume> ListResumeDaysTaken(string EmployeeCodes, params Enum[] Relations)
		{
			try
			{
				IEnumerable<BEA.EmployeeResume> Items = default;
				using DALH.Employee DALObject = new();
				Items = DALObject.ListResumeDaysTaken(EmployeeCodes, Relations);
				return Items;
			}
			catch (Exception ex)
			{
				ErrorHandler(ex);
				return null;
			}
		}

		#endregion

		#region Constructors

		public Employee() : base() { }

		#endregion

	}
}
