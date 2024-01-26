using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEA = BEntities.SAP;

namespace DALayer.SAP.Hana
{
	[Serializable()]
	public class Employee : DALEntity<BEA.Employee>
	{
		#region Global Variables

		readonly string queryBase;

		#endregion

		#region Methods

		protected override void LoadRelations(ref BEA.Employee Item, params Enum[] Relations) { }

		protected override void LoadRelations(ref IEnumerable<BEA.Employee> Items, params Enum[] Relations) { }

		#endregion

		#region List Methods

		public IEnumerable<BEA.Employee> List(List<Field> Filters, string SortingBy, params Enum[] Relations)
		{
			string filter = Filters?.Count > 0 ? GetFilter(Filters?.ToArray()) : "1 = 1"
				, query = $@"{queryBase}
WHERE	{filter}
ORDER BY {GetOrder(SortingBy)} ";

			IEnumerable<BEA.Employee> items = SQLList(query, Relations);
			return items;
		}

		public IEnumerable<BEA.EmployeeResume> ListResumeDaysTaken(string EmployeeCodes, params Enum[] Relations)
		{
			string query = $@"SELECT  U_EMPID AS ""EmployeeId"", SUM(U_NUMDIAS) AS ""Days""
FROM    {DBSA}.""@PLVCH""
WHERE   U_ESTADO = 'A' AND U_TIPO IN ( 'A', 'V' ) {(string.IsNullOrEmpty(EmployeeCodes) ? "" : $"AND U_EMPID IN ( {EmployeeCodes} ) ")}
GROUP BY U_EMPID ";
			IEnumerable<BEA.EmployeeResume> items = SQLList<BEA.EmployeeResume>(query, Relations);
			return items;
		}

		#endregion

		#region Search Methods

		public BEA.Employee Search(long Id, params Enum[] Relations)
		{
			string query = $@"{queryBase} WHERE   ""empID"" = {Id} ";

			BEA.Employee item = SQLSearch(query, Relations);
			return item;
		}

		public int GetLastId()
		{
			string query = $@"SELECT MAX(U_DOCVAC) FROM {DBSA}.""@PLVCH"" ";
			var result = Value(query);
			int id = int.Parse(result.ToString());
			return id;
		}

		public BEA.EmployeeResume SearchDaysTaken(long EmployeeId, params Enum[] Relations)
		{
			string query = $@"SELECT  SUM(U_NUMDIAS) AS ""Days""
FROM    {DBSA}.""@PLVCH""
WHERE   U_ESTADO = 'A' AND U_TIPO IN ( 'A', 'V' ) AND U_EMPID = {EmployeeId} ";
			BEA.EmployeeResume item = SQLSearch<BEA.EmployeeResume>(query, Relations);
			return item;
		}

		#endregion

		#region Private Methods

		#endregion

		#region Constructors

		public Employee() : base()
		{
			queryBase = $@"SELECT  ""empID"" AS ""Id"", TRIM(""firstName"" || ' ' || IFNULL(""middleName"", '')) AS ""FirstName"", TRIM(""lastName"" || ' ' || IFNULL(""U_ApMaterno"", '')) AS ""LastName"", CAST(""startDate"" AS DATE) AS ""StartDate""
		, CAST(""termDate"" AS DATE) AS ""TermDate"", ( SELECT TOP 1 ""BitmapPath"" FROM DMC_SA.OADP o ) AS ""Path"", ""picture"" AS ""Picture"", o2.""name"" AS ""Position""
		, o3.""SlpName"" AS ""ShortName"", ""officeExt"" AS ""Phone"", ""email"" AS ""Email""
FROM    {DBSA}.OHEM o
		LEFT OUTER JOIN {DBSA}.OHST o2 ON o.""status"" = o2.""statusID"" 
		LEFT OUTER JOIN {DBSA}.OSLP o3 ON o.""salesPrson"" = o3.""SlpCode"" ";
		}

		#endregion
	}
}
