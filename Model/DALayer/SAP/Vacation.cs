using BEntities.Filters;
using BEntities.SAP;
using Dapper;
using DocumentFormat.OpenXml.Bibliography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE = BEntities;
using BEA = BEntities.SAP;

namespace DALayer.SAP.Hana
{
	[Serializable()]
	public class Vacation : DALEntity<BEA.Vacation>
	{
		#region Global Variables

		readonly string baseQuery = @"SELECT  ""Code"" AS ""Id"", U_EMPID AS ""EmployeeId""
		, CAST(TO_VARCHAR(U_FECHA, 'yyyy-MM-dd') || ' ' || SUBSTRING(LPAD(TO_VARCHAR(U_HORA), 4, '0'), 1, 2) || ':' || SUBSTRING(LPAD(TO_VARCHAR(U_HORA), 4, '0'), 3, 2) AS TIMESTAMP) AS ""DocDate""
		, CAST(U_FROMDATE AS DATE) AS ""Since"", CAST(U_TODATE AS DATE) AS ""Until"", U_NUMDIAS AS ""Days"", U_TIPO AS ""Type"", CAST(U_ANIO AS INT) AS ""Year"", U_ESTADO AS ""State"", IFNULL(U_COMMENT, '') AS ""Comments""
FROM    ""DMC_SA"".""@PLVCH"" ";

		#endregion

		#region Methods

		protected override void LoadRelations(ref BEA.Vacation Item, params Enum[] Relations) { }

		protected override void LoadRelations(ref IEnumerable<BEA.Vacation> Items, params Enum[] Relations) { }

		#endregion

		#region Search Methods

		public BEA.Vacation Search(string Id, params Enum[] Relations)
		{
			string query = $@"{baseQuery} 
WHERE   ""Code"" = '{Id}' ";

			BEA.Vacation item = SQLSearch(query, Relations);
			return item;
		}

		public BEA.Vacation SearchDaysTakenByEmployee(int EmployeeId, params Enum[] Relations)
		{
			string query = $@"SELECT  {EmployeeId} AS ""EmployeeId"", SUM(""Days"") AS ""Days"" 
FROM    ( {baseQuery} ) a 
WHERE   ""State"" = 'A' AND ""EmployeeId"" = {EmployeeId} ";
			BEA.Vacation item = SQLSearch(query, Relations);
			return item;
		}

		public BEA.Vacation SearchDaysRejectedByEmployee(int EmployeeId, params Enum[] Relations)
		{
			string query = $@"SELECT  {EmployeeId} AS ""EmployeeId"", SUM(""Days"") AS ""Days"" 
FROM    ( {baseQuery} ) a 
WHERE   ""State"" <> 'A' AND ""EmployeeId"" = {EmployeeId} ";
			BEA.Vacation item = SQLSearch(query, Relations);
			return item;
		}

		#endregion

		#region List Methods

		public IEnumerable<BEA.Vacation> List(List<Field> Filters, string SortingBy, params Enum[] Relations)
		{
			string query = $@"{baseQuery}  
WHERE   {(Filters?.Count > 0 ? GetFilter(Filters.ToArray()) : "1 = 1")}
ORDER BY {GetOrder(SortingBy)} ";

			IEnumerable<BEA.Vacation> items = SQLList(query, Relations);
			return items;
		}

		#endregion

		#region Save Methods

		public void Save(BEA.Vacation item)
		{
			var d = new { Code = item.Id, Id = int.Parse(item.Id), item.DocDate, Hour = (short)(item.DocDate.Hour * 100 + item.DocDate.Minute), item.EmployeeId, Author = item.Author.ToString(), item.Since, item.Until, Days = (float)item.Days, item.Type, Year = item.Year.ToString(), item.State, StartDate = item.StartDate.Year * 10000 + item.StartDate.Month * 100 + item.StartDate.Day };
			string query = $@"INSERT INTO {DBSA}.""@PLVCH"" (""Code"", ""Name"", U_DOCVAC, U_FECHA, U_HORA, U_EMPID, U_AUTOR, U_FROMDATE, U_TODATE, U_NUMDIAS, U_TIPO, U_ANIO, U_ESTADO, U_STARTDATE, U_NUMDIAS1, U_NUMDIAS2, U_NUMDIAS3, U_NUMDIAS4, U_COMMENT)
VALUES('{d.Code}', '{d.Code}', {d.Id}, '{d.DocDate:yyyy-MM-dd}', {d.Hour}, {d.EmployeeId}, {d.Author}, '{d.Since:yyyy-MM-dd}', '{d.Until:yyyy-MM-dd}', {d.Days}, '{d.Type}', '{d.Year}', '{d.State}', {d.StartDate}, {d.Days}, 0, 0, 0, '{item.Comments}')";
			Connection.Execute(query);
		}

		#endregion

		#region Constructors

		public Vacation() { }

		#endregion
	}
}
