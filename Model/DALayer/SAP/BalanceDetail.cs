using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using BE = BEntities;
using BEA = BEntities.SAP;

namespace DALayer.SAP.Hana
{
	public class BalanceDetail : DALEntity<BEA.BalanceDetail>
	{
		#region Methods
		protected override void LoadRelations(ref BEA.BalanceDetail Item, params Enum[] Relations) { }

		protected override void LoadRelations(ref IEnumerable<BEA.BalanceDetail> Items, params Enum[] Relations) { }

		#endregion

		#region List Methods

		public IEnumerable<BEA.BalanceDetail> ListClientsSA(List<Field> FilterList, string Order)
		{
			string filter = FilterList?.Count > 0 ? GetFilter(FilterList.ToArray()) : "1 = 1", query;
			//var sb = new StringBuilder();
			//sb.AppendLine($@"SELECT TA.""Id"", TA.""Name"", TA.""Type"", CAST(TA.""Number"" AS VARCHAR(10)) AS ""Number"", TA.""Date"", TA.""Term"", TA.""Expires"", TA.""Days"", TA.""Balance"" ");
			//sb.AppendLine($@"FROM ( SELECT 'SANTA CRUZ' AS ""Subsidiary"", 'FAC' AS ""Type"", T0.""CardCode"" AS ""Id"", (CASE WHEN T0.""CardCode"" = 'CVAR-001' THEN T0.""CardName"" ELSE T3.""CardFName"" END) AS ""Name"" ");
			//sb.AppendLine($@"              , T0.U_ALMACEN AS ""Warehouse"", T0.""DocNum"" AS ""Number"", T1.""BaseRef"" AS ""DocBase"" ");
			//sb.AppendLine($@"              , CAST(T0.""DocDate"" AS DATE) AS ""Date"", T2.""PymntGroup"" AS ""Term"", CAST(T0.""DocDueDate"" AS DATE) AS ""Expires"", ");
			//sb.AppendLine($@"              (T0.""DocTotal"" - T0.""PaidToDate"") AS ""Balance"", DAYS_BETWEEN(T0.""DocDueDate"", NOW()) AS ""Days"" ");
			//sb.AppendLine($@"              , (CASE WHEN ((DAYS_BETWEEN(T0.""DocDueDate"", NOW()) > 0) AND (T0.""DocTotal"" - T0.""PaidToDate"") > 0) THEN 'HOLD' ELSE ' ' END) AS ""HoldVen"" ");
			//sb.AppendLine($@"       FROM {DBSA}.OINV T0 ");
			//sb.AppendLine($@"            INNER JOIN {DBSA}.INV1 T1 ON T0.""DocEntry"" = T1.""DocEntry"" AND (T0.""DocTotal"" - T0.""PaidToDate"") <> 0 ");
			//sb.AppendLine($@"            INNER JOIN {DBSA}.OCTG T2 ON T0.""GroupNum"" = T2.""GroupNum"" ");
			//sb.AppendLine($@"            INNER JOIN {DBSA}.OCRD T3 ON T3.""CardCode"" = T0.""CardCode"" ");
			//sb.AppendLine($@"            LEFT OUTER JOIN {DBSA}.OWHS T4 ON T4.""WhsCode"" = T1.""WhsCode"" ");
			//sb.AppendLine($@"       WHERE T0.""DocSubType"" <> 'DN' ");
			//sb.AppendLine($@"       GROUP BY T0.""DocNum"", T0.""DocDate"", T0.""DocDueDate"", DAYS_BETWEEN(T0.""DocDueDate"", NOW()), T0.""CardCode"", T3.""CardFName"", T0.U_ALMACEN, T1.""BaseRef"", T2.""PymntGroup"", (T0.""DocTotal"" - T0.""PaidToDate""), T0.""CardName"" ");
			//sb.AppendLine($@"       UNION ALL ");
			//sb.AppendLine($@"       SELECT 'SANTA CRUZ' AS ""BD"", 'ND', T0.""CardCode"", (CASE WHEN T0.""CardCode"" = 'CVAR-001' THEN T0.""CardName"" ELSE T3.""CardFName"" END) AS ""RAZON SOCIAL"" ");
			//sb.AppendLine($@"              , T0.U_ALMACEN, T0.""DocNum"", T1.""BaseRef"", CAST(T0.""DocDate"" AS DATE), T2.""PymntGroup"" ");
			//sb.AppendLine($@"              , CAST(T0.""DocDueDate"" AS DATE), (T0.""DocTotal"" - T0.""PaidToDate"") AS ""Balance"", DAYS_BETWEEN(T0.""DocDueDate"", NOW()) AS ""Days"" ");
			//sb.AppendLine($@"              , (CASE WHEN ((DAYS_BETWEEN(T0.""DocDueDate"", NOW()) > 0) AND (T0.""DocTotal"" - T0.""PaidToDate"") > 0) THEN 'HOLD' ELSE '' END) AS ""SITUACION"" ");
			//sb.AppendLine($@"       FROM {DBSA}.OINV T0 ");
			//sb.AppendLine($@"            INNER JOIN {DBSA}.INV1 T1 ON T0.""DocEntry"" = T1.""DocEntry"" ");
			//sb.AppendLine($@"            INNER JOIN {DBSA}.OCTG T2 ON T0.""GroupNum"" = T2.""GroupNum"" ");
			//sb.AppendLine($@"            INNER JOIN {DBSA}.OCRD T3 ON T3.""CardCode"" = T0.""CardCode"" ");
			//sb.AppendLine($@"            LEFT OUTER JOIN {DBSA}.OWHS T4 ON T4.""WhsCode"" = T1.""WhsCode"" ");
			//sb.AppendLine($@"       WHERE T0.CANCELED = 'N' ");
			//sb.AppendLine($@"             AND (T1.""TargetType"" <> 14 OR T1.""TargetType"" IS NULL) AND T0.""DocSubType"" = 'DN' AND (T0.""DocTotal"" - T0.""PaidToDate"") <> 0 ");
			//sb.AppendLine($@"       GROUP BY T0.""DocNum"", T0.""DocDate"", T0.""DocDueDate"", DAYS_BETWEEN(T0.""DocDueDate"", NOW()), T0.""CardCode"", T3.""CardFName"", T0.U_ALMACEN, T1.""BaseRef"", T2.""PymntGroup"", (T0.""DocTotal"" - T0.""PaidToDate""), T0.""CardName"" ");
			//sb.AppendLine($@"       UNION ALL ");
			//sb.AppendLine($@"       SELECT 'SANTA CRUZ' AS ""BD"", 'PRC', T0.""CardCode"", (CASE WHEN T0.""CardCode"" = 'CVAR-001' THEN T0.""CardName"" ELSE T2.""CardFName"" END) AS ""RAZON SOCIAL"" ");
			//sb.AppendLine($@"              , '', T0.""DocNum"", T1.""BaseRef"", CAST(T0.""DocDate"" AS DATE), '' ");
			//sb.AppendLine($@"              , CAST(T0.""DocDueDate"" AS DATE), T0.""OpenBal"" * -1, DAYS_BETWEEN(T0.""DocDueDate"", NOW()) ");
			//sb.AppendLine($@"              , (CASE WHEN ((DAYS_BETWEEN(T0.""DocDueDate"", NOW()) > 0) AND (T0.""OpenBal"" * -1) > 0) THEN 'HOLD' ELSE '' END) AS ""SITUACION"" ");
			//sb.AppendLine($@"       FROM {DBSA}.ORCT T0 ");
			//sb.AppendLine($@"            INNER JOIN {DBSA}.JDT1 T1 ON T0.""TransId"" = T1.""TransId"" ");
			//sb.AppendLine($@"            INNER JOIN {DBSA}.OCRD T2 ON T2.""CardCode"" = T0.""CardCode"" ");
			//sb.AppendLine($@"       WHERE T0.""PayNoDoc"" = 'Y' AND T0.""Canceled"" = 'N' AND T0.""DocType"" <> 'A' AND T0.""Canceled"" = 'N' AND T1.""ShortName"" = T0.""CardCode"" AND T1.""IntrnMatch"" = 0 AND T0.""OpenBal"" <> 0 ");
			//sb.AppendLine($@"       UNION ALL ");
			//sb.AppendLine($@"       SELECT 'SANTA CRUZ' AS ""BD"", 'PEC', T0.""CardCode"", (CASE WHEN T0.""CardCode"" = 'CVAR-001' THEN T0.""CardName"" ELSE T1.""CardFName"" END) AS ""RAZON SOCIAL"" ");
			//sb.AppendLine($@"              , '', T0.""DocNum"", T2.""BaseRef"", CAST(T0.""DocDate"" AS DATE), '' ");
			//sb.AppendLine($@"              , CAST(T0.""DocDueDate"" AS DATE), T0.""OpenBal"", DAYS_BETWEEN(NOW(), T0.""DocDueDate"") AS ""Days"" ");
			//sb.AppendLine($@"              , (CASE WHEN (DAYS_BETWEEN(NOW(), T0.""DocDueDate"") > 0 AND T0.""OpenBal"" > 0) THEN 'HOLD' ELSE '' END) AS ""SITUACION"" ");
			//sb.AppendLine($@"       FROM {DBSA}.""OVPM"" T0 ");
			//sb.AppendLine($@"            INNER JOIN {DBSA}.OCRD T1 ON T0.""CardCode"" = T1.""CardCode"" ");
			//sb.AppendLine($@"            INNER JOIN {DBSA}.JDT1 T2 ON T0.""TransId"" = T2.""TransId"" ");
			//sb.AppendLine($@"       WHERE T0.""PayNoDoc"" = 'Y' ");
			//sb.AppendLine($@"             AND T0.""Canceled"" = 'N' ");
			//sb.AppendLine($@"             AND T1.""CardType"" = 'C' ");
			//sb.AppendLine($@"             AND T2.""ShortName"" = T0.""CardCode"" ");
			//sb.AppendLine($@"             AND T2.""IntrnMatch"" = 0 ");
			//sb.AppendLine($@"             AND T0.""OpenBal"" <> 0 ");
			//sb.AppendLine($@"       UNION ");
			//sb.AppendLine($@"       SELECT 'SANTA CRUZ' AS ""BD"", 'RD', T2.""CardCode"", (CASE WHEN T2.""CardCode"" = 'CVAR-001' THEN T2.""CardName"" ELSE T2.""CardFName"" END) AS ""RAZON SOCIAL"" ");
			//sb.AppendLine($@"              , '', T0.""TransId"", '', CAST(T0.""RefDate"" AS DATE), T3.""PymntGroup"" ");
			//sb.AppendLine($@"              , CAST(T1.""DueDate"" AS DATE), T1.""BalDueCred"" * -1 AS ""Balance"", DAYS_BETWEEN(T1.""DueDate"", NOW()) AS ""Days"" ");
			//sb.AppendLine($@"              , (CASE WHEN (DAYS_BETWEEN(T1.""DueDate"", NOW()) > 0 AND (T1.""Debit"" - T1.""Credit"") > 0) THEN 'HOLD' ELSE '' END) AS ""SITUACION"" ");
			//sb.AppendLine($@"       FROM {DBSA}.OJDT T0 ");
			//sb.AppendLine($@"            INNER JOIN {DBSA}.JDT1 T1 ON T0.""TransId"" = T1.""TransId"" ");
			//sb.AppendLine($@"            INNER JOIN {DBSA}.OCRD T2 ON T1.""ShortName"" = T2.""CardCode"" ");
			//sb.AppendLine($@"            INNER JOIN {DBSA}.OCTG T3 ON T3.""GroupNum"" = T2.""GroupNum"" ");
			//sb.AppendLine($@"       WHERE T1.""Account"" <> T1.""ShortName"" ");
			//sb.AppendLine($@"             AND T0.""TransType"" = '30' ");
			//sb.AppendLine($@"             AND T1.""IntrnMatch"" = 0 ");
			//sb.AppendLine($@"             AND T2.""CardType"" = 'C' ");
			//sb.AppendLine($@"             AND (T1.""Debit"" - T1.""Credit"") <> 0 ");
			//sb.AppendLine($@"             AND T1.""BalDueCred"" <> 0 ");
			//sb.AppendLine($@"       UNION ");
			//sb.AppendLine($@"       SELECT 'SANTA CRUZ' AS ""BD"", 'NC', T0.""CardCode"", (CASE WHEN T0.""CardCode"" = 'CVAR-001' THEN T0.""CardName"" ELSE T2.""CardFName"" END) AS ""RAZON SOCIAL"" ");
			//sb.AppendLine($@"              , T0.U_ALMACEN, T0.""DocNum"", T1.""BaseRef"", CAST(T0.""DocDate"" AS DATE), '' ");
			//sb.AppendLine($@"              , CAST(T0.""DocDueDate"" AS DATE), (-T0.""DocTotal"" + T0.""PaidToDate"") AS ""Balance"" ");
			//sb.AppendLine($@"              , DAYS_BETWEEN(T0.""DocDueDate"", NOW()) AS ""Days"", (CASE WHEN (DAYS_BETWEEN(T0.""DocDueDate"", NOW()) > 0 AND (-T0.""DocTotal"" + T0.""PaidToDate"") > 0) THEN 'HOLD' ELSE '' END) AS ""SITUACION"" ");
			//sb.AppendLine($@"       FROM {DBSA}.ORIN T0 ");
			//sb.AppendLine($@"            INNER JOIN {DBSA}.RIN1 T1 ON T0.""DocEntry"" = T1.""DocEntry"" ");
			//sb.AppendLine($@"            INNER JOIN {DBSA}.OCRD T2 ON T2.""CardCode"" = T0.""CardCode"" ");
			//sb.AppendLine($@"            LEFT OUTER JOIN {DBSA}.OWHS T4 ON T4.""WhsCode"" = T1.""WhsCode"" ");
			//sb.AppendLine($@"       WHERE T0.""DocSubType"" = '--' ");
			//sb.AppendLine($@"             AND T0.""DocStatus"" = 'O' ");
			//sb.AppendLine($@"       GROUP BY T0.""DocNum"", T0.""DocDate"", T0.""DocDueDate"", T0.""CardCode"", T0.""CardName"", T2.""CardFName"", T1.""BaseRef"", (-T0.""DocTotal"" + T0.""PaidToDate""), T0.U_ALMACEN, DAYS_BETWEEN(T0.""DocDueDate"", NOW())) AS TA ");
			//if (!string.IsNullOrEmpty(filter)) sb.AppendLine($@"WHERE   {filter} ");
			//sb.AppendLine($@"ORDER BY {GetOrder(Order)} ");
			query = $@"SELECT  TA.""Code"", TA.""Name"", TA.""Type"", CAST(TA.""Number"" AS VARCHAR(10)) AS ""Number"", TA.""Date"", TA.""Term"", TA.""Expires"", TA.""Days"", TA.""Balance"" 
FROM    (SELECT 'Santa Cruz' AS ""Subsidiary"", 'RF' AS ""Type"", T0.""CardCode"" AS ""Code"", T3.""CardFName"" AS ""Name"", T0.U_ALMACEN AS ""Warehouse"", T0.""DocNum"" AS ""Number""
				, FIRST_VALUE(T1.""BaseRef"" ORDER BY T1.""LineNum"") AS ""DocBase"", CAST(T0.""DocDate"" AS DATE) AS ""Date"", T2.""PymntGroup"" AS ""Term"", CAST(T5.""DueDate"" AS DATE) AS ""Expires""
				, (T5.""InsTotal"" - T5.""PaidToDate"") AS ""Balance"", DAYS_BETWEEN(T5.""DueDate"", NOW()) AS ""Days""
				, (CASE WHEN ((DAYS_BETWEEN(T5.""DueDate"", NOW()) > 0) AND (T5.""InsTotal"" - T5.""PaidToDate"") > 0) THEN 'HOLD' ELSE ' ' END) AS ""HoldVen"" 
		 FROM   {DBSA}.OINV T0 
				INNER JOIN {DBSA}.INV1 T1 ON T0.""DocEntry"" = T1.""DocEntry"" AND (T0.""DocTotal"" - T0.""PaidToDate"") <> 0 
				INNER JOIN {DBSA}.OCTG T2 ON T0.""GroupNum"" = T2.""GroupNum"" 
				INNER JOIN {DBSA}.OCRD T3 ON T3.""CardCode"" = t0.""CardCode"" 
				INNER JOIN {DBSA}.INV6 T5 ON T5.""DocEntry"" = T0.""DocEntry"" 
		 WHERE  T0.""DocSubType"" <> 'DN'	AND (T5.""InsTotal"" - T5.""PaidToDate"") > 0 
		 GROUP BY T0.""DocNum"", T0.""DocDate"", T5.""DueDate"", T5.""InsTotal"", DAYS_BETWEEN(T0.""DocDueDate"", NOW()), T0.""CardCode"", T3.""CardFName"", T0.U_ALMACEN, T2.""PymntGroup"", (T5.""InsTotal"" - T5.""PaidToDate"") 
		 UNION ALL 
		 SELECT 'Santa Cruz' AS ""BD"", 'ND', T0.""CardCode"", T3.""CardFName"", T0.U_ALMACEN AS ""ALMACEN"", T0.""DocNum"", T1.""BaseRef"", CAST(T0.""DocDate"" AS DATE), T2.""PymntGroup"", CAST(T5.""DueDate"" AS DATE)
				, (T5.""InsTotal"" - T5.""PaidToDate"") AS ""BALANCE"", DAYS_BETWEEN(T5.""DueDate"", NOW()) AS ""DIAS""
				, (CASE WHEN ((DAYS_BETWEEN(T5.""DueDate"", NOW()) > 0) AND (T5.""InsTotal"" - T5.""PaidToDate"") > 0) THEN 'HOLD' ELSE ' ' END) AS ""HOLD_VEN"" 
		 FROM   {DBSA}.OINV T0 
				INNER JOIN {DBSA}.INV1 T1 ON T0.""DocEntry"" = T1.""DocEntry"" 
				INNER JOIN {DBSA}.OCTG T2 ON T0.""GroupNum"" = T2.""GroupNum"" 
				INNER JOIN {DBSA}.OCRD T3 ON T3.""CardCode"" = t0.""CardCode"" 
				INNER JOIN {DBSA}.INV6 T5 ON T5.""DocEntry"" = T0.""DocEntry"" 
		 WHERE  T0.CANCELED = 'N' AND (T1.""TargetType"" <> 14 OR T1.""TargetType"" IS NULL) AND T0.""DocSubType"" = 'DN' AND (T0.""DocTotal"" - T0.""PaidToDate"") <> 0 
		 GROUP BY T0.""DocNum"", T0.""DocDate"", T5.""DueDate"", T5.""InsTotal"", DAYS_BETWEEN(T0.""DocDueDate"", NOW()), T0.""CardCode"", T3.""CardFName"", T0.U_ALMACEN, T1.""BaseRef"", T2.""PymntGroup""
				, (T5.""InsTotal""- T5.""PaidToDate"") 
		 UNION ALL 
		 SELECT 'Santa Cruz' AS ""BD"", 'PRC', T0.""CardCode"", T2.""CardFName"", '', T0.""DocNum"", T1.""BaseRef"", CAST(T0.""DocDate"" AS DATE), '', CAST(T0.""DocDueDate"" AS DATE), T0.""OpenBal"" * -1
				, DAYS_BETWEEN(T0.""DocDueDate"", NOW()), (CASE WHEN ((DAYS_BETWEEN(T0.""DocDueDate"", NOW()) > 0) AND (T0.""OpenBal"" * -1) > 0) THEN 'HOLD' ELSE '' END) AS ""SITUACION"" 
		 FROM   {DBSA}.ORCT T0 
				INNER JOIN {DBSA}.JDT1 T1 ON T0.""TransId"" = T1.""TransId"" 
				INNER JOIN {DBSA}.OCRD T2 ON T2.""CardCode"" = T0.""CardCode"" 
		 WHERE  T0.""PayNoDoc"" = 'Y' AND T0.""Canceled"" = 'N' AND T0.""DocType"" <> 'A' AND T0.""Canceled"" = 'N' AND T1.""ShortName"" = T0.""CardCode"" AND T1.""IntrnMatch"" = 0 AND T0.""OpenBal"" <> 0
		 UNION 
		 SELECT 'Santa Cruz' AS ""BD"", 'NC', T0.""CardCode"", T2.""CardFName"", T0.U_ALMACEN, T0.""DocNum"", T1.""BaseRef"", CAST(T0.""DocDate"" AS DATE), '', CAST(T0.""DocDueDate"" AS DATE)
				, (-T0.""DocTotal"" + T0.""PaidToDate"") AS ""Balance"", DAYS_BETWEEN(T0.""DocDueDate"", NOW()) AS ""Days""
				, (CASE WHEN (DAYS_BETWEEN(T0.""DocDueDate"", NOW()) > 0 AND (-T0.""DocTotal"" + T0.""PaidToDate"") > 0) THEN 'HOLD' ELSE '' END) AS ""SITUACION"" 
		 FROM   {DBSA}.ORIN T0 
				INNER JOIN {DBSA}.RIN1 T1 ON T0.""DocEntry"" = T1.""DocEntry"" 
				INNER JOIN {DBSA}.OCRD T2 ON T2.""CardCode"" = T0.""CardCode"" 
				LEFT OUTER JOIN {DBSA}.OWHS T4 ON T4.""WhsCode"" = T1.""WhsCode"" 
		 WHERE  T0.""DocSubType"" = '--' AND T0.""DocStatus"" = 'O' 
		 GROUP BY T0.""DocNum"", T0.""DocDate"", T0.""DocDueDate"", T0.""CardCode"", T0.""CardName"", T2.""CardFName"", T1.""BaseRef"", (-T0.""DocTotal"" + T0.""PaidToDate""), T0.U_ALMACEN, DAYS_BETWEEN(T0.""DocDueDate"", NOW()) 
		 UNION ALL 
		 SELECT 'Santa Cruz' AS ""BD"", 'RD', T2.""CardCode"", T2.""CardFName"", '', T0.""TransId"", '', CAST(T1.""RefDate"" AS DATE) AS ""FECHA"", '', CAST(T1.""DueDate"" AS DATE) AS ""VENCE""
				, T1.""BalDueDeb"" - T1.""BalDueCred"" AS ""Balance"", DAYS_BETWEEN(T1.""DueDate"", NOW()) AS ""Days""
				, (CASE WHEN (DAYS_BETWEEN(T1.""DueDate"", NOW()) > 0 AND (T1.""Debit"" - T1.""Credit"") > 0) THEN 'HOLD' ELSE '' END) AS ""SITUACION"" 
		 FROM   {DBSA}.OJDT T0 
				INNER JOIN {DBSA}.JDT1 T1 ON T0.""TransId"" = T1.""TransId"" 
				INNER JOIN {DBSA}.OCRD T2 ON T1.""ShortName"" = T2.""CardCode"" 
				INNER JOIN {DBSA}.OCTG T3 ON T3.""GroupNum"" = T2.""GroupNum"" 
		 WHERE  ( T1.""Account"" <> T1.""ShortName"" AND T0.""TransType"" = '30' AND T1.""IntrnMatch"" = 0 AND T2.""CardType"" = 'C' AND (T1.""Debit"" - T1.""Credit"") <> 0 AND (T1.""BalDueDeb""-T1.""BalDueCred"") <> 0) ) AS TA 
WHERE   {filter}
ORDER BY {GetOrder(Order)} ";

			IEnumerable<BEA.BalanceDetail> items = SQLList(query);
			return items;
		}

		public IEnumerable<BEA.BalanceDetail> ListProvidersSA(List<Field> FilterList, string Order)
		{
			string filter = FilterList?.Count > 0 ? GetFilter(FilterList.ToArray()) : "1 = 1",
				query = $@"SELECT  ""Name"", ""Type"", ""Number"", ""Date"", ""Term"", ""Expires"", ""Days"", ""Balance"" * -1 AS ""Balance"" 
FROM    (SELECT 'FAC' AS ""Type"", T0.""CardCode"" AS ""CODIGO"", T3.""CardFName"" AS ""Name""
				, T0.""DocNum"" AS ""NO_DOC"", T0.""NumAtCard"" AS ""Number"", CAST(T0.""DocDate"" AS DATE) AS ""Date"", T2.""PymntGroup"" AS ""Term"", CAST(T5.""DueDate"" AS DATE) AS ""Expires""
				, (CASE WHEN (T0.""CardCode"" = 'BBAN-005' OR T0.""CardCode"" = 'BBAN-006' OR T0.""CardCode"" = 'BBAN-005' OR T0.""CardCode"" = 'BBAN-007') 
						THEN ((T5.""InsTotalSy"" - T5.""PaidSys"") * -1) 
						ELSE ((T5.""InsTotal"" - T5.""PaidToDate"") * -1) END) AS ""Balance"", DAYS_BETWEEN(CAST(T5.""DueDate"" AS DATE), NOW()) AS ""Days"" 
		 FROM   {DBSA}.OPCH T0 
				INNER JOIN {DBSA}.OCTG T2 ON T0.""GroupNum"" = T2.""GroupNum"" 
				INNER JOIN {DBSA}.OCRD T3 ON T3.""CardCode"" = t0.""CardCode"" 
				INNER JOIN {DBSA}.PCH6 T5 ON T5.""DocEntry"" = T0.""DocEntry"" 
		 WHERE  (T5.""InsTotal"" - T5.""PaidToDate"")*-1 <> 0 AND T0.""DocSubType"" = '--' AND (T0.""CardCode"" LIKE 'B%' OR T0.""CardCode"" LIKE 'M%') 
		 GROUP BY T0.""DocNum"", T0.""CardCode"", CAST(T0.""DocDate"" AS DATE), CAST(T5.""DueDate"" AS DATE), T3.""CardFName"", T2.""PymntGroup"", T5.""InsTotal"", T5.""InsTotalSy"", T5.""PaidToDate"", T5.""PaidSys"", (T5.""InsTotal"" - T5.""PaidToDate"") *-1, T0.""NumAtCard""	 	
		 UNION 
		 SELECT 'ND', T0.""CardCode"", T3.""CardFName""
				, T0.""DocNum"", T0.""NumAtCard"" AS ""FAC_PROV"", CAST(T0.""DocDate"" AS DATE), T2.""PymntGroup"", CAST(T0.""DocDueDate"" AS DATE)
				, (CASE WHEN (T0.""CardCode"" = 'BBAN-005' OR T0.""CardCode"" = 'BBAN-006' OR T0.""CardCode"" = 'BBAN-007') 
						THEN ((T0.""DocTotalSy"" - T0.""PaidSys"") * -1) 
						ELSE ((T0.""DocTotal"" - T0.""PaidToDate"") * -1) END) AS ""BALANCE""
				, DAYS_BETWEEN(T0.""DocDueDate"", NOW()) AS ""Days"" 
		 FROM   {DBSA}.OPCH T0 
				INNER JOIN {DBSA}.OCTG T2 ON T0.""GroupNum"" = T2.""GroupNum"" 
				INNER JOIN {DBSA}.OCRD T3 ON T3.""CardCode"" = t0.""CardCode"" 
		 WHERE  T0.CANCELED = 'N' AND T0.""DocSubType"" = 'DM' AND (T0.""DocTotal"" - T0.""PaidToDate"") * -1 <> 0 AND (T0.""CardCode"" LIKE 'B%' OR T0.""CardCode"" LIKE 'M%')
		 GROUP BY T0.""DocNum"", CAST(T0.""DocDate"" AS DATE), CAST(T0.""DocDueDate"" AS DATE), DAYS_BETWEEN(T0.""DocDueDate"", NOW()), T0.""CardCode"", T3.""CardFName"", T2.""PymntGroup"", T0.""NumAtCard"", ((T0.""DocTotal"" - T0.""PaidToDate"") * -1), ((T0.""DocTotalSy"" - T0.""PaidSys"") * -1) 
		 UNION 
		 SELECT 'PRC', T0.""CardCode"", T2.""CardFName""
				, T0.""DocNum"", '', CAST(T0.""DocDate"" AS DATE), '', CAST(T0.""DocDueDate"" AS DATE)
				, (CASE WHEN (T0.""CardCode"" = 'BBAN-005' OR T0.""CardCode"" = 'BBAN-006' OR T0.""CardCode"" = 'BBAN-007') 
						THEN (T0.""OpenBalSc"" * -1) 
						ELSE T0.""OpenBal"" * -1 END  ), DAYS_BETWEEN(T0.""DocDueDate"", NOW()) 
		 FROM   {DBSA}.ORCT T0 
				INNER JOIN {DBSA}.JDT1 T1 ON T0.""TransId"" = T1.""TransId"" 
				INNER JOIN {DBSA}.OCRD T2 ON T2.""CardCode"" = T0.""CardCode"" 
		 WHERE  T0.""PayNoDoc"" = 'Y' AND T0.""Canceled"" = 'N' AND T0.""DocType"" = 'S' AND T0.""Canceled"" = 'N' AND T1.""ShortName"" = T0.""CardCode"" AND T1.""IntrnMatch"" = 0 AND (T0.""CardCode"" LIKE 'B%' OR T0.""CardCode"" LIKE 'M%') 
		 UNION 
		 SELECT 'PEC', T0.""CardCode"", T1.""CardFName""
				, T0.""DocNum"", '', CAST(T0.""DocDate"" AS DATE), '', CAST(T0.""DocDueDate"" AS DATE)
				, (CASE WHEN (T0.""CardCode"" = 'BBAN-005' OR T0.""CardCode"" = 'BBAN-006' OR T0.""CardCode"" = 'BBAN-007') 
						THEN T0.""OpenBalSc"" 
						ELSE T0.""OpenBal"" END)
				, DAYS_BETWEEN(T0.""DocDueDate"", NOW()) AS ""Days"" 
		 FROM   {DBSA}.OVPM T0 
				INNER JOIN {DBSA}.OCRD T1 ON T0.""CardCode"" = T1.""CardCode"" 
				INNER JOIN {DBSA}.JDT1 T2 ON T0.""TransId"" = T2.""TransId"" 
		 WHERE  T0.""PayNoDoc"" = 'Y' AND T0.""Canceled"" = 'N' AND T1.""CardType"" = 'S' AND T2.""ShortName"" = T0.""CardCode"" AND T2.""IntrnMatch"" = 0 AND T0.""OpenBal"" > 0 AND (T0.""CardCode"" LIKE 'B%' OR T0.""CardCode"" LIKE 'M%') 
		 UNION 
		 SELECT 'RD', T2.""CardCode"", T2.""CardFName""
				, T0.""TransId"", '', CAST(T0.""RefDate"" AS DATE), T3.""PymntGroup"", CAST(T1.""DueDate"" AS DATE)
				, (CASE WHEN (T2.""CardCode"" = 'BBAN-005' OR T2.""CardCode"" = 'BBAN-006' OR T2.""CardCode"" = 'BBAN-007') 
						THEN ((T1.""SYSCred"" - T1.""SYSDeb"") * -1) 
						ELSE ((T1.""Credit"" - T1.""Debit"") * -1) END) AS ""Balance""
				, DAYS_BETWEEN(T1.""DueDate"", NOW()) AS ""Days"" 
		 FROM   {DBSA}.OJDT T0 
				INNER JOIN {DBSA}.JDT1 T1 ON T0.""TransId"" = T1.""TransId"" 
				INNER JOIN {DBSA}.OCRD T2 ON T1.""ShortName"" = T2.""CardCode"" 
				INNER JOIN {DBSA}.OCTG T3 ON T3.""GroupNum"" = T2.""GroupNum"" 
		 WHERE  T1.""Account"" <> T1.""ShortName"" AND T0.""TransType"" = '30' AND T1.""IntrnMatch"" = 0 AND T2.""CardType"" = 'S' AND ((T1.""Credit"" - T1.""Debit"") * -1) <> 0 AND T1.""MthDate"" = '' AND (T2.""CardCode"" LIKE 'B%' OR T2.""CardCode"" LIKE 'M%') 
		 UNION 
		 SELECT 'NC', T0.""CardCode"", T2.""CardFName""
				, T0.""DocNum"", T0.""NumAtCard"", CAST(T0.""DocDate"" AS DATE), '', CAST(T0.""DocDueDate"" AS DATE)
				, (CASE WHEN (T0.""CardCode"" = 'BBAN-005' OR T0.""CardCode"" = 'BBAN-006' OR T0.""CardCode"" = 'BBAN-007')
						THEN ((-T0.""DocTotalSy"" + T0.""PaidSys"") * -1) 
						ELSE ((-T0.""DocTotal"" + T0.""PaidToDate"") * -1) END) AS ""Balance""
				, DAYS_BETWEEN(T0.""DocDueDate"", NOW()) AS ""Days"" 
		 FROM   {DBSA}.ORPC T0 
				INNER JOIN {DBSA}.OCRD T2 ON T2.""CardCode"" = T0.""CardCode"" 
		 WHERE  T0.""DocSubType"" = '--' AND T0.""DocStatus"" = 'O' AND (T0.""CardCode"" LIKE 'B%' OR T0.""CardCode"" LIKE 'M%') 
		 GROUP BY T0.""DocNum"", CAST(T0.""DocDate"" AS DATE), CAST(T0.""DocDueDate"" AS DATE), T0.""CardCode"", ((-T0.""DocTotalSy"" + T0.""PaidSys"") * -1), T2.""CardFName"", T0.""NumAtCard"", (-T0.""DocTotal"" + T0.""PaidToDate""), DAYS_BETWEEN(T0.""DocDueDate"", NOW())) AS TA 
WHERE    {filter}
ORDER BY {GetOrder(Order)} ";

			IEnumerable<BEA.BalanceDetail> items = SQLList(query);
			return items;
		}

		public IEnumerable<BEA.BalanceDetail> ListClientsIQ(List<Field> FilterList, string Order)
		{
			string filter = FilterList?.Count > 0 ? GetFilter(FilterList.ToArray()) : "1 = 1", query;
			//var sb = new StringBuilder();
			//sb.AppendLine($@"SELECT TA.""Id"", TA.""Name"", TA.""Type"", CAST(TA.""Number"" AS VARCHAR(10)) AS ""Number"", TA.""Date"", TA.""Term"", TA.""Expires"", TA.""Days"", TA.""Balance"" ");
			//sb.AppendLine($@"FROM ( SELECT 'IQUIQUE' AS ""Subsidiary"", 'FAC' AS ""Type"", T0.""CardCode"" AS ""Id"", T3.""CardFName"" AS ""Name"" ");
			//sb.AppendLine($@"              , T4.""WhsName"" AS ""Warehouse"", T0.""DocNum"" AS ""Number"", T1.""BaseRef"" AS ""Base"", CAST(T0.""DocDate"" AS DATE) AS ""Date"", T2.""PymntGroup"" AS ""Term"", CAST(T0.""DocDueDate"" AS DATE) AS ""Expires"" ");
			//sb.AppendLine($@"              , (T0.""DocTotal"" - T0.""PaidToDate"") AS ""Balance"", DAYS_BETWEEN(T0.""DocDueDate"", NOW()) AS ""Days"" ");
			//sb.AppendLine($@"              , (CASE WHEN ((DAYS_BETWEEN(T0.""DocDueDate"", NOW()) > 7) AND (T0.""DocTotal"" - T0.""PaidToDate"") > 0) THEN 'HOLD' ELSE ' ' END) AS ""HoldVen"" ");
			//sb.AppendLine($@"       FROM {DBIQ}.OINV T0 ");
			//sb.AppendLine($@"            INNER JOIN {DBIQ}.INV1 T1 ON T0.""DocEntry"" = T1.""DocEntry"" AND (T0.""DocTotal"" - T0.""PaidToDate"") <> 0 ");
			//sb.AppendLine($@"            INNER JOIN {DBIQ}.OCTG T2 ON T0.""GroupNum"" = T2.""GroupNum"" ");
			//sb.AppendLine($@"            INNER JOIN {DBIQ}.OCRD T3 ON T3.""CardCode"" = T0.""CardCode"" ");
			//sb.AppendLine($@"            LEFT OUTER JOIN {DBIQ}.OWHS T4 ON T4.""WhsCode"" = T1.""WhsCode"" ");
			//sb.AppendLine($@"       WHERE T0.""DocSubType"" = '--' ");
			//sb.AppendLine($@"       GROUP BY T0.""DocNum"", T0.""DocDate"", T0.""DocDueDate"", DAYS_BETWEEN(T0.""DocDueDate"", NOW()), T0.""CardCode"", T3.""CardFName"", T4.""WhsName"", T1.""BaseRef"", T2.""PymntGroup"", (T0.""DocTotal"" - T0.""PaidToDate"") ");
			//sb.AppendLine($@"       UNION ALL ");
			//sb.AppendLine($@"       SELECT 'IQUIQUE' AS ""BD"", 'ND', T0.""CardCode"", T3.""CardFName"", T4.""WhsName"" AS ""ALMACEN"", T0.""DocNum"", T1.""BaseRef"", CAST(T0.""DocDate"" AS DATE), T2.""PymntGroup"", CAST(T0.""DocDueDate"" AS DATE) ");
			//sb.AppendLine($@"              , (T0.""DocTotal"" - T0.""PaidToDate"") AS ""Balance"", DAYS_BETWEEN(T0.""DocDueDate"", NOW()) AS ""Days"" ");
			//sb.AppendLine($@"              , (CASE WHEN ((DAYS_BETWEEN(T0.""DocDueDate"", NOW()) > 7) AND (T0.""DocTotal"" - T0.""PaidToDate"") > 0) THEN 'HOLD' ELSE '' END) AS ""SITUACION"" ");
			//sb.AppendLine($@"       FROM {DBIQ}.OINV T0 ");
			//sb.AppendLine($@"            INNER JOIN {DBIQ}.INV1 T1 ON T0.""DocEntry"" = T1.""DocEntry"" ");
			//sb.AppendLine($@"            INNER JOIN {DBIQ}.OCTG T2 ON T0.""GroupNum"" = T2.""GroupNum"" ");
			//sb.AppendLine($@"            INNER JOIN {DBIQ}.OCRD T3 ON T3.""CardCode"" = T0.""CardCode"" ");
			//sb.AppendLine($@"            LEFT OUTER JOIN {DBIQ}.OWHS T4 ON T4.""WhsCode"" = T1.""WhsCode"" ");
			//sb.AppendLine($@"       WHERE T0.CANCELED = 'N' AND (T1.""TargetType"" <> 14 OR T1.""TargetType"" IS NULL) AND T0.""DocSubType"" = 'DN' AND (T0.""DocTotal"" - T0.""PaidToDate"") <> 0 ");
			//sb.AppendLine($@"       GROUP BY T0.""DocNum"", T0.""DocDate"", T0.""DocDueDate"", DAYS_BETWEEN(T0.""DocDueDate"", NOW()), T0.""CardCode"", T3.""CardFName"", T4.""WhsName"", T1.""BaseRef"", T2.""PymntGroup"", (T0.""DocTotal"" - T0.""PaidToDate"") ");
			//sb.AppendLine($@"       UNION ALL ");
			//sb.AppendLine($@"       SELECT 'IQUIQUE' AS ""BD"", 'PRC', T0.""CardCode"", T2.""CardFName"", '', T0.""DocNum"", T1.""BaseRef"", CAST(T0.""DocDate"" AS DATE), '', CAST(T0.""DocDueDate"" AS DATE), T0.""OpenBal"" * -1 ");
			//sb.AppendLine($@"              , DAYS_BETWEEN(T0.""DocDueDate"", NOW()), (CASE WHEN ((DAYS_BETWEEN(T0.""DocDueDate"", NOW()) > 7) AND (T0.""OpenBal"" * -1) > 0) THEN 'HOLD' ELSE '' END) AS ""SITUACION"" ");
			//sb.AppendLine($@"       FROM {DBIQ}.""ORCT"" T0 ");
			//sb.AppendLine($@"            INNER JOIN {DBIQ}.""JDT1"" T1 ON T0.""TransId"" = T1.""TransId"" ");
			//sb.AppendLine($@"            INNER JOIN {DBIQ}.OCRD T2 ON T2.""CardCode"" = T0.""CardCode"" ");
			//sb.AppendLine($@"       WHERE T0.""PayNoDoc"" = 'Y' AND T0.""Canceled"" = 'N' AND T0.""DocType"" <> 'A' AND T0.""Canceled"" = 'N' AND T1.""ShortName"" = T0.""CardCode"" AND T1.""IntrnMatch"" = 0 AND T0.""OpenBal"" <> 0 ");
			//sb.AppendLine($@"       UNION ALL ");
			//sb.AppendLine($@"       SELECT 'IQUIQUE' AS ""BD"", 'PEC', T0.""CardCode"", T1.""CardFName"", '', T0.""DocNum"", T2.""BaseRef"", CAST(T0.""DocDate"" AS DATE), '', CAST(T0.""DocDueDate"" AS DATE), T0.""OpenBal"" ");
			//sb.AppendLine($@"              , DAYS_BETWEEN(NOW(), T0.""DocDueDate"") AS ""Days"", (CASE WHEN (DAYS_BETWEEN(NOW(), T0.""DocDueDate"") > 7 AND T0.""OpenBal"" > 0) THEN 'HOLD' ELSE '' END) AS ""SITUACION"" ");
			//sb.AppendLine($@"       FROM {DBIQ}.""OVPM"" T0 ");
			//sb.AppendLine($@"            INNER JOIN {DBIQ}.OCRD T1 ON T0.""CardCode"" = T1.""CardCode"" ");
			//sb.AppendLine($@"            INNER JOIN {DBIQ}.""JDT1"" T2 ON T0.""TransId"" = T2.""TransId"" ");
			//sb.AppendLine($@"       WHERE T0.""PayNoDoc"" = 'Y' AND T0.""Canceled"" = 'N' AND (T0.""CardCode"" <> 'CDMC-002') AND T1.""CardType"" = 'C' AND T2.""ShortName"" = T0.""CardCode"" AND T2.""IntrnMatch"" = 0 AND T0.""OpenBal"" <> 0 ");
			//sb.AppendLine($@"       UNION ALL ");
			//sb.AppendLine($@"       SELECT 'IQUIQUE', 'RD', T2.""CardCode"", T2.""CardFName"", '', T0.""TransId"", '', CAST(T0.""RefDate"" AS DATE), T3.""PymntGroup"", CAST(T1.""DueDate"" AS DATE), (T1.""Debit"" - T1.""Credit"") AS ""Balance"" ");
			//sb.AppendLine($@"              , DAYS_BETWEEN(T1.""DueDate"", NOW()) AS ""Days"", (CASE WHEN (DAYS_BETWEEN(T1.""DueDate"", NOW()) > 7 AND (T1.""Debit"" - T1.""Credit"") > 0) THEN 'HOLD' ELSE '' END) AS ""SITUACION"" ");
			//sb.AppendLine($@"       FROM {DBIQ}.""OJDT"" T0 ");
			//sb.AppendLine($@"            INNER JOIN {DBIQ}.""JDT1"" T1 ON T0.""TransId"" = T1.""TransId"" ");
			//sb.AppendLine($@"            INNER JOIN {DBIQ}.OCRD T2 ON T1.""ShortName"" = T2.""CardCode"" ");
			//sb.AppendLine($@"            INNER JOIN {DBIQ}.OCTG T3 ON T3.""GroupNum"" = T2.""GroupNum"" ");
			//sb.AppendLine($@"       WHERE T1.""Account"" <> T1.""ShortName"" AND T0.""TransType"" = '30' AND T1.""IntrnMatch"" = 0 AND T2.""CardType"" = 'C' AND (T1.""Debit"" - T1.""Credit"") <> 0 AND T1.""MthDate"" = '' ");
			//sb.AppendLine($@"       UNION ALL ");
			//sb.AppendLine($@"       SELECT 'IQUIQUE' AS ""BD"", 'NC', T0.""CardCode"", T2.""CardFName"", T4.""WhsName"" AS ""ALMACEN"", T0.""DocNum"", T1.""BaseRef"", CAST(T0.""DocDate"" AS DATE), '', CAST(T0.""DocDueDate"" AS DATE) ");
			//sb.AppendLine($@"              , (-T0.""DocTotal"" + T0.""PaidToDate"") AS ""Balance"", DAYS_BETWEEN(T0.""DocDueDate"", NOW()) AS ""Days"" ");
			//sb.AppendLine($@"              , (CASE WHEN (DAYS_BETWEEN(T0.""DocDueDate"", NOW()) > 7 AND (-T0.""DocTotal"" + T0.""PaidToDate"") > 0) THEN 'HOLD' ELSE '' END) AS ""SITUACION"" ");
			//sb.AppendLine($@"       FROM {DBIQ}.""ORIN"" T0 ");
			//sb.AppendLine($@"            INNER JOIN {DBIQ}.""RIN1"" T1 ON T0.""DocEntry"" = T1.""DocEntry"" ");
			//sb.AppendLine($@"            INNER JOIN {DBIQ}.OCRD T2 ON T2.""CardCode"" = T0.""CardCode"" ");
			//sb.AppendLine($@"            LEFT OUTER JOIN {DBIQ}.OWHS T4 ON T4.""WhsCode"" = T1.""WhsCode"" ");
			//sb.AppendLine($@"       WHERE T0.""DocSubType"" = '--' AND T0.""DocStatus"" = 'O' ");
			//sb.AppendLine($@"       GROUP BY T0.""DocNum"", T0.""DocDate"", T0.""DocDueDate"", T0.""CardCode"", T2.""CardFName"", T4.""WhsName"", T1.""BaseRef"", (-T0.""DocTotal"" + T0.""PaidToDate""), DAYS_BETWEEN(T0.""DocDueDate"", NOW())) AS TA ");
			//if (!string.IsNullOrEmpty(filter)) sb.AppendLine($@"WHERE   {filter} ");
			//sb.AppendLine($@"ORDER BY {GetOrder(Order)} ");
			query = $@"SELECT  TA.""Code"", TA.""Name"", TA.""Type"", CAST(TA.""Number"" AS VARCHAR(10)) AS ""Number"", TA.""Date"", TA.""Term"", TA.""Expires"", TA.""Days"", TA.""Balance""
FROM    (SELECT 'Iquique' AS ""Subsidiary"", 'RF' AS ""Type"", T0.""CardCode"" AS ""Code"", T3.""CardFName"" AS ""Name"", T0.U_ALMACEN AS ""Warehouse"", T0.""DocNum"" AS ""Number""
				, T1.""BaseRef"" AS ""DocBase"", CAST(T0.""DocDate"" AS DATE) AS ""Date"", T2.""PymntGroup"" AS ""Term"", CAST(T5.""DueDate"" AS DATE) AS ""Expires"", (T5.""InsTotal"" - T5.""PaidToDate"") AS ""Balance""
				, DAYS_BETWEEN(T5.""DueDate"", NOW()) AS ""Days"", (CASE WHEN ((DAYS_BETWEEN(T5.""DueDate"", NOW()) > 0) AND (T5.""InsTotal"" - T5.""PaidToDate"") > 0) THEN 'HOLD' ELSE ' ' END) AS ""HoldVen""
				, IFNULL((SELECT REPLACE(CAST((SELECT TA.""Header"" FROM {DBIQ}.""ORDR"" TA WHERE TA.""DocEntry"" = T1.""BaseEntry"") AS varchar(5000)), ' ', ' -  ') FROM DUMMY), '') AS ""COM_ADD"" 
		 FROM   {DBIQ}.OINV T0 
				INNER JOIN {DBIQ}.INV1 T1 ON T0.""DocEntry"" = T1.""DocEntry"" AND (T0.""DocTotal"" - T0.""PaidToDate"") <> 0 
				INNER JOIN {DBIQ}.OCTG T2 ON T0.""GroupNum"" = T2.""GroupNum"" 
				INNER JOIN {DBIQ}.OCRD T3 ON T3.""CardCode"" = t0.""CardCode"" 
				INNER JOIN {DBIQ}.INV6 T5 ON T5.""DocEntry"" = T0.""DocEntry""
		 WHERE  (T5.""InsTotal"" - T5.""PaidToDate"") > 0
		 GROUP BY T0.""DocNum"", T0.""DocDate"", T5.""DueDate"", T5.""InsTotal"", DAYS_BETWEEN(T0.""DocDueDate"", NOW()), T0.""CardCode"", T3.""CardFName"", T0.U_ALMACEN, T1.""BaseRef"", T2.""PymntGroup""
				, (T5.""InsTotal"" - T5.""PaidToDate""), T1.""BaseEntry"" 
		 UNION ALL 
		 SELECT 'Iquique' AS ""BD"", 'RD', T2.""CardCode"", T2.""CardFName"", '', T0.""TransId"", '', CAST(T1.""RefDate"" AS DATE) AS ""FECHA"", '', CAST(T1.""DueDate"" AS DATE) AS ""VENCE"", (T1.""BalDueDeb""-T1.""BalDueCred"") AS ""Balance""
				, DAYS_BETWEEN(T1.""DueDate"", NOW()) AS ""Days"", (CASE WHEN (DAYS_BETWEEN(T1.""DueDate"", NOW()) > 0 AND (T1.""Debit"" - T1.""Credit"") > 0) THEN 'HOLD' ELSE '' END) AS ""SITUACION"", '' 
		 FROM   {DBIQ}.OJDT T0 
				INNER JOIN {DBIQ}.JDT1 T1 ON T0.""TransId"" = T1.""TransId"" 
				INNER JOIN {DBIQ}.OCRD T2 ON T1.""ShortName"" = T2.""CardCode"" 
				INNER JOIN {DBIQ}.OCTG T3 ON T3.""GroupNum"" = T2.""GroupNum"" 
		 WHERE  (T1.""Account"" <> T1.""ShortName"" AND T0.""TransType"" = '30' AND T1.""IntrnMatch"" = 0 AND T2.""CardType"" = 'C' AND (T1.""Debit"" - T1.""Credit"") <> 0 AND (T1.""BalDueDeb""-T1.""BalDueCred"") <> 0) 
		 UNION ALL 
		 SELECT 'Iquique' AS ""BD"", 'RD', T2.""CardCode"", T2.""CardFName"", '', T0.""TransId"", '', CAST(T0.""RefDate"" AS DATE), T3.""PymntGroup"", CAST(T1.""DueDate"" AS DATE), (T1.""Debit"" - T1.""Credit"") AS ""Balance""
				, DAYS_BETWEEN(T1.""DueDate"", NOW()) AS ""Days"", (CASE WHEN (DAYS_BETWEEN(T1.""DueDate"", NOW()) > 0 AND (T1.""Debit"" - T1.""Credit"") > 0) THEN 'HOLD' ELSE '' END) AS ""SITUACION"", '' 
		 FROM   {DBIQ}.OJDT T0 
				INNER JOIN {DBIQ}.JDT1 T1 ON T0.""TransId"" = T1.""TransId"" 
				INNER JOIN {DBIQ}.OCRD T2 ON T1.""ShortName"" = T2.""CardCode"" 
				INNER JOIN {DBIQ}.OCTG T3 ON T3.""GroupNum"" = T2.""GroupNum"" 
		 WHERE  T1.""Account"" <> T1.""ShortName"" AND T0.""TransType"" = '30' AND T1.""IntrnMatch"" = 0 AND T2.""CardType"" = 'C' AND (T1.""Debit"" - T1.""Credit"") <> 0 AND T1.""MthDate"" = '' 
		 UNION ALL 
		 SELECT 'Iquique' AS ""BD"", 'PRC', T0.""CardCode"", T2.""CardFName"", '', T0.""DocNum"", T1.""BaseRef"", CAST(T0.""DocDate"" AS DATE), '', CAST(T0.""DocDueDate"" AS DATE), T0.""OpenBal"" * -1
				, DAYS_BETWEEN(T0.""DocDueDate"", NOW()), (CASE WHEN ((DAYS_BETWEEN(T0.""DocDueDate"", NOW()) > 0) AND (T0.""OpenBal"" * -1) > 0) THEN 'HOLD' ELSE '' END) AS ""SITUACION"", '' 
		 FROM   {DBIQ}.ORCT T0 
				INNER JOIN {DBIQ}.JDT1 T1 ON T0.""TransId"" = T1.""TransId"" 
				INNER JOIN {DBIQ}.OCRD T2 ON T2.""CardCode"" = T0.""CardCode"" 
		 WHERE  T0.""PayNoDoc"" = 'Y' AND T0.""Canceled"" = 'N' AND T0.""DocType"" <> 'A' AND T0.""Canceled"" = 'N' AND T1.""ShortName"" = T0.""CardCode"" AND T1.""IntrnMatch"" = 0 AND T0.""OpenBal"" <> 0 
		 UNION 
		 SELECT 'Iquique' AS ""BD"", 'NC', T0.""CardCode"", T2.""CardFName"", '', T0.""DocNum"", T1.""BaseRef"", CAST(T0.""DocDate"" AS DATE), '', CAST(T0.""DocDueDate"" AS DATE), (-T0.""DocTotal"" + T0.""PaidToDate"") AS ""Balance""
				, DAYS_BETWEEN(T0.""DocDueDate"", NOW()) AS ""Days"", (CASE WHEN (DAYS_BETWEEN(T0.""DocDueDate"", NOW()) > 0 AND (-T0.""DocTotal"" + T0.""PaidToDate"") > 0) THEN 'HOLD' ELSE '' END) AS ""SITUACION"", '' 
		 FROM   {DBIQ}.ORIN T0 
				INNER JOIN {DBIQ}.RIN1 T1 ON T0.""DocEntry"" = T1.""DocEntry"" 
				INNER JOIN {DBIQ}.OCRD T2 ON T2.""CardCode"" = T0.""CardCode"" 
				LEFT OUTER JOIN {DBIQ}.OWHS T4 ON T4.""WhsCode"" = T1.""WhsCode"" 
		 WHERE  T0.""DocSubType"" = '--' AND T0.""DocStatus"" = 'O'
		 GROUP BY T0.""DocNum"", T0.""DocDate"", T0.""DocDueDate"", T0.""CardCode"", T0.""CardName"", T2.""CardFName"", T1.""BaseRef"", (-T0.""DocTotal"" + T0.""PaidToDate""), T0.U_ALMACEN, DAYS_BETWEEN(T0.""DocDueDate"", NOW()) ) AS TA 
WHERE   {filter}
ORDER BY {GetOrder(Order)} ";

			IEnumerable<BEA.BalanceDetail> items = SQLList(query);
			return items;
		}

		public IEnumerable<BEA.BalanceDetail> ListProvidersIQ(List<Field> FilterList, string Order)
		{
			string filter = FilterList?.Count > 0 ? GetFilter(FilterList.ToArray()) : "1 = 1", query;
			//var sb = new StringBuilder();
			//sb.AppendLine($@"SELECT ""Name"", ""Type"", ""Base"" AS ""Number"", ""Date"", ""Term"", ""Expires"", ""Days"", ""Balance"" * -1 AS ""Balance"" ");
			//sb.AppendLine($@"FROM ( SELECT 'FAC' AS ""Type"", T0.""CardCode"" AS ""Id"", T3.""CardFName"" AS ""Name"", T0.""DocNum"" AS ""Number"", T0.""NumAtCard"" AS ""Base"" ");
			//sb.AppendLine($@"              , T0.""DocDate"" AS ""Date"", T2.""PymntGroup"" AS ""Term"", T0.""DocDueDate"" AS ""Expires"", ((T0.""DocTotal"" - T0.""PaidToDate"") * -1) AS ""Balance"", DAYS_BETWEEN(T0.""DocDueDate"", NOW()) AS ""Days"" ");
			//sb.AppendLine($@"       FROM ""{DBIQ}"".""OPCH"" T0 ");
			//sb.AppendLine($@"            INNER JOIN ""{DBIQ}"".""OCTG"" T2 ON T0.""GroupNum"" = T2.""GroupNum"" ");
			//sb.AppendLine($@"            INNER JOIN ""{DBIQ}"".""OCRD"" T3 ON T3.""CardCode"" = T0.""CardCode"" ");
			//sb.AppendLine($@"       WHERE ((T0.""DocTotal"" - T0.""PaidToDate"") * -1) <> 0 AND T0.""DocSubType"" = '--' AND (T0.""CardCode"" LIKE 'B%' OR T0.""CardCode"" LIKE 'M%') ");
			//sb.AppendLine($@"       GROUP BY T0.""DocNum"", T0.""CardCode"", T0.""DocDate"", T0.""DocDueDate"", T3.""CardFName"", T2.""PymntGroup"", T0.""NumAtCard"", ((T0.""DocTotal"" - T0.""PaidToDate"") * -1) ");
			//sb.AppendLine($@"       UNION ");
			//sb.AppendLine($@"       SELECT 'ND', T0.""CardCode"", T3.""CardFName"", T0.""DocNum"", T0.""NumAtCard"" AS ""FAC_PROV"", T0.""DocDate"", T2.""PymntGroup"" ");
			//sb.AppendLine($@"              , T0.""DocDueDate"", ((T0.""DocTotal"" - T0.""PaidToDate"") * -1) AS ""Balance"", DAYS_BETWEEN(T0.""DocDueDate"", NOW()) AS ""Days"" ");
			//sb.AppendLine($@"       FROM ""{DBIQ}"".""OPCH"" T0 ");
			//sb.AppendLine($@"            INNER JOIN ""{DBIQ}"".""OCTG"" T2 ON T0.""GroupNum"" = T2.""GroupNum"" ");
			//sb.AppendLine($@"            INNER JOIN ""{DBIQ}"".""OCRD"" T3 ON T3.""CardCode"" = T0.""CardCode"" ");
			//sb.AppendLine($@"       WHERE T0.CANCELED = 'N' AND T0.""DocSubType"" = 'DM' AND (T0.""DocTotal"" - T0.""PaidToDate"") * -1 <> 0 AND (T0.""CardCode"" LIKE 'B%' OR T0.""CardCode"" LIKE 'M%') ");
			//sb.AppendLine($@"       GROUP BY T0.""DocNum"", T0.""DocDate"", T0.""DocDueDate"", DAYS_BETWEEN(T0.""DocDueDate"", NOW()), T0.""CardCode"", T3.""CardFName"", T2.""PymntGroup"", T0.""NumAtCard"", (T0.""DocTotal"" - T0.""PaidToDate"") ");
			//sb.AppendLine($@"       UNION ");
			//sb.AppendLine($@"       SELECT 'PRC', T0.""CardCode"", T2.""CardFName"", T0.""DocNum"", '', T0.""DocDate"", '', T0.""DocDueDate"", T0.""OpenBal"" * -1, DAYS_BETWEEN(T0.""DocDueDate"", NOW()) ");
			//sb.AppendLine($@"       FROM ""{DBIQ}"".""ORCT"" T0 ");
			//sb.AppendLine($@"            INNER JOIN ""{DBIQ}"".""JDT1"" T1 ON T0.""TransId"" = T1.""TransId"" ");
			//sb.AppendLine($@"            INNER JOIN ""{DBIQ}"".""OCRD"" T2 ON T2.""CardCode"" = T0.""CardCode"" ");
			//sb.AppendLine($@"       WHERE T0.""PayNoDoc"" = 'Y' AND T0.""Canceled"" = 'N' AND T0.""DocType"" = 'S' AND T0.""Canceled"" = 'N' AND T1.""ShortName"" = T0.""CardCode"" AND T1.""IntrnMatch"" = 0  ");
			//sb.AppendLine($@"             AND (T0.""CardCode"" LIKE 'B%' OR T0.""CardCode"" LIKE 'M%') ");
			//sb.AppendLine($@"       UNION ");
			//sb.AppendLine($@"       SELECT 'PEC', T0.""CardCode"", T1.""CardFName"", T0.""DocNum"", '', T0.""DocDate"", '', T0.""DocDueDate"", T0.""OpenBal"", DAYS_BETWEEN(T0.""DocDueDate"", NOW()) AS ""Days"" ");
			//sb.AppendLine($@"       FROM ""{DBIQ}"".""OVPM"" T0 ");
			//sb.AppendLine($@"            INNER JOIN ""{DBIQ}"".""OCRD"" T1 ON T0.""CardCode"" = T1.""CardCode"" ");
			//sb.AppendLine($@"            INNER JOIN ""{DBIQ}"".""JDT1"" T2 ON T0.""TransId"" = T2.""TransId"" ");
			//sb.AppendLine($@"       WHERE T0.""PayNoDoc"" = 'Y' AND T0.""Canceled"" = 'N' AND T1.""CardType"" = 'S' AND T2.""ShortName"" = T0.""CardCode"" AND T2.""IntrnMatch"" = 0 AND T0.""OpenBal"" > 0 ");
			//sb.AppendLine($@"             AND (T0.""CardCode"" LIKE 'B%' OR T0.""CardCode"" LIKE 'M%') ");
			//sb.AppendLine($@"       UNION ");
			//sb.AppendLine($@"       SELECT 'RD', T2.""CardCode"", T2.""CardFName"", T0.""TransId"", '', T0.""RefDate"", T3.""PymntGroup"", T1.""DueDate"", ((T1.""Credit"" - T1.""Debit"") * -1) AS ""Balance"", DAYS_BETWEEN(T1.""DueDate"", NOW()) AS ""Days"" ");
			//sb.AppendLine($@"       FROM ""{DBIQ}"".""OJDT"" T0 ");
			//sb.AppendLine($@"            INNER JOIN ""{DBIQ}"".""JDT1"" T1 ON T0.""TransId"" = T1.""TransId"" ");
			//sb.AppendLine($@"            INNER JOIN ""{DBIQ}"".""OCRD"" T2 ON T1.""ShortName"" = T2.""CardCode"" ");
			//sb.AppendLine($@"            INNER JOIN ""{DBIQ}"".""OCTG"" T3 ON T3.""GroupNum"" = T2.""GroupNum"" ");
			//sb.AppendLine($@"       WHERE T1.""Account"" <> T1.""ShortName"" AND T0.""TransType"" = '30' AND T1.""IntrnMatch"" = 0 AND T2.""CardType"" = 'S' AND ((T1.""Credit"" - T1.""Debit"") * -1) <> 0 AND T1.""MthDate"" = '' ");
			//sb.AppendLine($@"             AND (T2.""CardCode"" LIKE 'B%' OR T2.""CardCode"" LIKE 'M%') ");
			//sb.AppendLine($@"       UNION ");
			//sb.AppendLine($@"       SELECT 'NC', T0.""CardCode"", T2.""CardFName"", T0.""DocNum"", T0.""NumAtCard"", T0.""DocDate"", '', T0.""DocDueDate"", ((-T0.""DocTotal"" + T0.""PaidToDate"") * -1) AS ""Balance"", DAYS_BETWEEN(T0.""DocDueDate"", NOW()) AS ""Days"" ");
			//sb.AppendLine($@"       FROM ""{DBIQ}"".""ORPC"" T0 ");
			//sb.AppendLine($@"            INNER JOIN ""{DBIQ}"".""OCRD"" T2 ON T2.""CardCode"" = T0.""CardCode"" ");
			//sb.AppendLine($@"       WHERE T0.""DocSubType"" = '--' AND T0.""DocStatus"" = 'O' AND (T0.""CardCode"" LIKE 'B%' OR T0.""CardCode"" LIKE 'M%') ");
			//sb.AppendLine($@"       GROUP BY T0.""DocNum"", T0.""DocDate"", T0.""DocDueDate"", T0.""CardCode"", T2.""CardFName"", T0.""NumAtCard"", ((-T0.""DocTotal"" + T0.""PaidToDate"") * -1), DAYS_BETWEEN(T0.""DocDueDate"", NOW())) AS TA ");
			//if (!string.IsNullOrEmpty(filter)) sb.AppendLine($@"WHERE   {filter} ");
			//sb.AppendLine($@"ORDER BY {GetOrder(Order)} ");
			query = $@"SELECT  ""Name"", ""Type"", ""Number"", CAST(""Date"" AS DATE) AS ""Date"", ""Term"", CAST(""Expires"" AS DATE) AS ""Expires"", ""Days"", ""Balance"" * -1 AS ""Balance"" 
FROM    (SELECT 'FAC' AS ""Type"", T0.""CardCode"" AS ""CODIGO"", T3.""CardFName"" AS ""Name""
				, T0.""DocNum"" AS ""NO_DOC"", T0.""NumAtCard"" AS ""Number"", T0.""DocDate"" AS ""Date"", T2.""PymntGroup"" AS ""Term"", T0.""DocDueDate"" AS ""Expires""
				, ((T0.""DocTotal"" - T0.""PaidToDate"") * -1) AS ""Balance"", DAYS_BETWEEN(T0.""DocDueDate"", NOW()) AS ""Days"" 
		 FROM   {DBIQ}.OPCH T0 
				INNER JOIN {DBIQ}.OCTG T2 ON T0.""GroupNum"" = T2.""GroupNum"" 
				INNER JOIN {DBIQ}.OCRD T3 ON T3.""CardCode"" = t0.""CardCode"" 
		 WHERE  ((T0.""DocTotal"" - T0.""PaidToDate"") * -1) <> 0 AND T0.""DocSubType"" = '--' AND (T0.""CardCode"" LIKE 'B%' OR T0.""CardCode"" LIKE 'M%') 
		 GROUP BY T0.""DocNum"", T0.""CardCode"", T0.""DocDate"", T0.""DocDueDate"", T3.""CardFName"", T2.""PymntGroup"", T0.""NumAtCard"", ((T0.""DocTotal"" - T0.""PaidToDate"") * -1) 
		 UNION 
		 SELECT 'ND', T0.""CardCode"", T3.""CardFName""
				, T0.""DocNum"", T0.""NumAtCard"" AS ""FAC_PROV"", T0.""DocDate"", T2.""PymntGroup"", T0.""DocDueDate""
				, ((T0.""DocTotal"" - T0.""PaidToDate"") * -1) AS ""Balance"", DAYS_BETWEEN(T0.""DocDueDate"", NOW()) AS ""Days"" 
		 FROM   {DBIQ}.OPCH T0 
				INNER JOIN {DBIQ}.OCTG T2 ON T0.""GroupNum"" = T2.""GroupNum"" 
				INNER JOIN {DBIQ}.OCRD T3 ON T3.""CardCode"" = t0.""CardCode"" 
		 WHERE  T0.CANCELED = 'N' AND T0.""DocSubType"" = 'DM' AND (T0.""DocTotal"" - T0.""PaidToDate"") * -1 <> 0 AND (T0.""CardCode"" LIKE 'B%' OR T0.""CardCode"" LIKE 'M%') 
		 GROUP BY T0.""DocNum"", T0.""DocDate"", T0.""DocDueDate"", DAYS_BETWEEN(T0.""DocDueDate"", NOW()), T0.""CardCode"", T3.""CardFName"", T2.""PymntGroup"", T0.""NumAtCard"", (T0.""DocTotal"" - T0.""PaidToDate"") 
		 UNION 
		 SELECT 'PRC', T0.""CardCode"", T2.""CardFName""
				, T0.""DocNum"", '', T0.""DocDate"", '', T0.""DocDueDate""
				, T0.""OpenBal"" * -1, DAYS_BETWEEN(T0.""DocDueDate"", NOW()) 
		 FROM   {DBIQ}.ORCT T0 
				INNER JOIN {DBIQ}.JDT1 T1 ON T0.""TransId"" = T1.""TransId"" 
				INNER JOIN {DBIQ}.OCRD T2 ON T2.""CardCode"" = T0.""CardCode"" 
		 WHERE  T0.""PayNoDoc"" = 'Y' AND T0.""Canceled"" = 'N' AND T0.""DocType"" = 'S' AND T0.""Canceled"" = 'N' AND T1.""ShortName"" = T0.""CardCode"" AND T1.""IntrnMatch"" = 0 
				AND (T0.""CardCode"" LIKE 'B%' OR T0.""CardCode"" LIKE 'M%') 
		 UNION 
		 SELECT 'PEC', T0.""CardCode"", T1.""CardFName""
				, T0.""DocNum"", '', T0.""DocDate"", '', T0.""DocDueDate"", T0.""OpenBal"", DAYS_BETWEEN(T0.""DocDueDate"", NOW()) AS ""Days"" 
		 FROM   {DBIQ}.OVPM T0 
				INNER JOIN {DBIQ}.OCRD T1 ON T0.""CardCode"" = T1.""CardCode"" 
				INNER JOIN {DBIQ}.JDT1 T2 ON T0.""TransId"" = T2.""TransId"" 
		 WHERE  T0.""PayNoDoc"" = 'Y' AND T0.""Canceled"" = 'N' AND T1.""CardType"" = 'S' AND T2.""ShortName"" = T0.""CardCode"" AND T2.""IntrnMatch"" = 0 AND T0.""OpenBal"" > 0 
				AND (T0.""CardCode"" LIKE 'B%' OR T0.""CardCode"" LIKE 'M%') 
		 UNION 
		 SELECT 'RD', T2.""CardCode"", T2.""CardFName""
				, T0.""TransId"", '', T0.""RefDate"", T3.""PymntGroup"", T1.""DueDate"", ((T1.""Credit"" - T1.""Debit"") * -1) AS ""Balance"", DAYS_BETWEEN(T1.""DueDate"", NOW()) AS ""Days"" 
		 FROM   {DBIQ}.OJDT T0 
				INNER JOIN {DBIQ}.JDT1 T1 ON T0.""TransId"" = T1.""TransId"" 
				INNER JOIN {DBIQ}.OCRD T2 ON T1.""ShortName"" = T2.""CardCode"" 
				INNER JOIN {DBIQ}.OCTG T3 ON T3.""GroupNum"" = T2.""GroupNum"" 
		 WHERE  T1.""Account"" <> T1.""ShortName"" AND T0.""TransType"" = '30' AND T1.""IntrnMatch"" = 0 AND T2.""CardType"" = 'S' AND ((T1.""Credit"" - T1.""Debit"") * -1) <> 0 AND T1.""MthDate"" = '' 
				AND (T2.""CardCode"" LIKE 'B%' OR T2.""CardCode"" LIKE 'M%') 
		 UNION 
		 SELECT 'NC', T0.""CardCode"", T2.""CardFName""
				, T0.""DocNum"", T0.""NumAtCard"", T0.""DocDate"", '', T0.""DocDueDate"", ((-T0.""DocTotal"" + T0.""PaidToDate"") * -1) AS ""Balance"", DAYS_BETWEEN(T0.""DocDueDate"", NOW()) AS ""Days"" 
		 FROM   {DBIQ}.ORPC T0 
				INNER JOIN {DBIQ}.OCRD T2 ON T2.""CardCode"" = T0.""CardCode"" 
		 WHERE  T0.""DocSubType"" = '--' AND T0.""DocStatus"" = 'O' AND (T0.""CardCode"" LIKE 'B%' OR T0.""CardCode"" LIKE 'M%') 
		 GROUP BY T0.""DocNum"", T0.""DocDate"", T0.""DocDueDate"", T0.""CardCode"", T2.""CardFName"", T0.""NumAtCard"", ((-T0.""DocTotal"" + T0.""PaidToDate"") * -1), DAYS_BETWEEN(T0.""DocDueDate"", NOW())) AS TA
WHERE   {filter} 
ORDER BY {GetOrder(Order)} ";

			IEnumerable<BEA.BalanceDetail> items = SQLList(query);
			return items;
		}

		public IEnumerable<BEA.BalanceDetail> ListClientsLA(List<Field> FilterList, string Order)
		{
			string filter = FilterList?.Count > 0 ? GetFilter(FilterList.ToArray()) : "1 = 1", query;
			//var sb = new StringBuilder();
			//sb.AppendLine($@"SELECT ""Id"", ""Name"", ""Type"", CAST(""Number"" AS VARCHAR(10)) AS ""Number"", ""Date"", ""Term"", ""Expires"", ""Days"", ""Balance"" ");
			//sb.AppendLine($@"FROM ( SELECT 'MIAMI' AS ""Subsidiary"", 'FAC' AS ""Type"", T0.""CardCode"" AS ""Id"", T3.""CardFName"" AS ""Name"", T4.""WhsName"" AS ""Warehouse"", T0.""DocNum"" AS ""Number"", T1.""BaseRef"" AS ""Base"" ");
			//sb.AppendLine($@"              , CAST(T0.""DocDate"" AS DATE) AS ""Date"", T2.""PymntGroup"" AS ""Term"", CAST(T0.""DocDueDate"" AS DATE) AS ""Expires"", (T0.""DocTotal"" - T0.""PaidToDate"") AS ""Balance"", DAYS_BETWEEN(T0.""DocDueDate"", NOW()) AS ""Days"" ");
			//sb.AppendLine($@"              , (CASE WHEN ((DAYS_BETWEEN(T0.""DocDueDate"", NOW()) > 7) AND (T0.""DocTotal"" - T0.""PaidToDate"") > 0) THEN 'HOLD' ELSE ' ' END) AS ""HoldVen"" ");
			//sb.AppendLine($@"       FROM {DBLA}.OINV T0 ");
			//sb.AppendLine($@"            INNER JOIN {DBLA}.INV1 T1 ON T0.""DocEntry"" = T1.""DocEntry"" AND (T0.""DocTotal"" - T0.""PaidToDate"") <> 0 ");
			//sb.AppendLine($@"            INNER JOIN {DBLA}.OCTG T2 ON T0.""GroupNum"" = T2.""GroupNum"" ");
			//sb.AppendLine($@"            INNER JOIN {DBLA}.OCRD T3 ON T3.""CardCode"" = T0.""CardCode"" ");
			//sb.AppendLine($@"            LEFT OUTER JOIN {DBLA}.OWHS T4 ON T4.""WhsCode"" = T1.""WhsCode"" ");
			//sb.AppendLine($@"       WHERE T0.""DocSubType"" = '--' ");
			//sb.AppendLine($@"       GROUP BY T0.""DocNum"", T0.""DocDate"", T0.""DocDueDate"", DAYS_BETWEEN(T0.""DocDueDate"", NOW()), T0.""CardCode"", T3.""CardFName"", T4.""WhsName"", T1.""BaseRef"", T2.""PymntGroup"", (T0.""DocTotal"" - T0.""PaidToDate"") ");
			//sb.AppendLine($@"       UNION ALL ");
			//sb.AppendLine($@"       SELECT 'MIAMI' AS ""BD"", 'ND', T0.""CardCode"", T3.""CardFName"", T4.""WhsName"" AS ""ALMACEN"", T0.""DocNum"", T1.""BaseRef"", CAST(T0.""DocDate"" AS DATE), T2.""PymntGroup"", CAST(T0.""DocDueDate"" AS DATE) ");
			//sb.AppendLine($@"              , (T0.""DocTotal"" - T0.""PaidToDate"") AS ""Balance"", DAYS_BETWEEN(T0.""DocDueDate"", NOW()) AS ""Days"" ");
			//sb.AppendLine($@"              , (CASE WHEN ((DAYS_BETWEEN(T0.""DocDueDate"", NOW()) > 7) AND (T0.""DocTotal"" - T0.""PaidToDate"") > 0) THEN 'HOLD' ELSE '' END) AS ""SITUACION"" ");
			//sb.AppendLine($@"       FROM {DBLA}.OINV T0 ");
			//sb.AppendLine($@"            INNER JOIN {DBLA}.INV1 T1 ON T0.""DocEntry"" = T1.""DocEntry"" ");
			//sb.AppendLine($@"            INNER JOIN {DBLA}.OCTG T2 ON T0.""GroupNum"" = T2.""GroupNum"" ");
			//sb.AppendLine($@"            INNER JOIN {DBLA}.OCRD T3 ON T3.""CardCode"" = T0.""CardCode"" ");
			//sb.AppendLine($@"            LEFT OUTER JOIN {DBLA}.OWHS T4 ON T4.""WhsCode"" = T1.""WhsCode"" ");
			//sb.AppendLine($@"       WHERE T0.CANCELED = 'N' AND (T1.""TargetType"" <> 14 OR T1.""TargetType"" IS NULL) AND T0.""DocSubType"" = 'DN' AND (T0.""DocTotal"" - T0.""PaidToDate"") <> 0 ");
			//sb.AppendLine($@"       GROUP BY T0.""DocNum"", T0.""DocDate"", T0.""DocDueDate"", DAYS_BETWEEN(T0.""DocDueDate"", NOW()), T0.""CardCode"", T3.""CardFName"", T4.""WhsName"", T1.""BaseRef"", T2.""PymntGroup"", (T0.""DocTotal"" - T0.""PaidToDate"") ");
			//sb.AppendLine($@"       UNION ALL ");
			//sb.AppendLine($@"       SELECT 'MIAMI' AS ""BD"", 'PRC', T0.""CardCode"", T2.""CardFName"", '', T0.""DocNum"", T1.""BaseRef"", CAST(T0.""DocDate"" AS DATE), '', CAST(T0.""DocDueDate"" AS DATE), T0.""OpenBal"" * -1 ");
			//sb.AppendLine($@"              , DAYS_BETWEEN(T0.""DocDueDate"", NOW()), (CASE WHEN ((DAYS_BETWEEN(T0.""DocDueDate"", NOW()) > 7) AND (T0.""OpenBal"" * -1) > 0) THEN 'HOLD' ELSE '' END) AS ""SITUACION"" ");
			//sb.AppendLine($@"       FROM {DBLA}.ORCT T0 ");
			//sb.AppendLine($@"            INNER JOIN {DBLA}.JDT1 T1 ON T0.""TransId"" = T1.""TransId"" ");
			//sb.AppendLine($@"            INNER JOIN {DBLA}.OCRD T2 ON T2.""CardCode"" = T0.""CardCode"" ");
			//sb.AppendLine($@"       WHERE T0.""PayNoDoc"" = 'Y' AND T0.""Canceled"" = 'N' AND T0.""DocType"" <> 'A' AND T0.""Canceled"" = 'N' AND T1.""ShortName"" = T0.""CardCode"" AND T1.""IntrnMatch"" = 0 AND T0.""OpenBal"" <> 0 ");
			//sb.AppendLine($@"       UNION ALL ");
			//sb.AppendLine($@"       SELECT 'MIAMI' AS ""BD"", 'PEC', T0.""CardCode"", T1.""CardFName"", '', T0.""DocNum"", T2.""BaseRef"", CAST(T0.""DocDate"" AS DATE), '', CAST(T0.""DocDueDate"" AS DATE), T0.""OpenBal"" ");
			//sb.AppendLine($@"              , DAYS_BETWEEN(NOW(), T0.""DocDueDate"") AS ""Days"", (CASE WHEN (DAYS_BETWEEN(NOW(), T0.""DocDueDate"") > 7 AND T0.""OpenBal"" > 0) THEN 'HOLD' ELSE '' END) AS ""SITUACION"" ");
			//sb.AppendLine($@"       FROM {DBLA}.""OVPM"" T0 ");
			//sb.AppendLine($@"            INNER JOIN {DBLA}.OCRD T1 ON T0.""CardCode"" = T1.""CardCode"" ");
			//sb.AppendLine($@"            INNER JOIN {DBLA}.JDT1 T2 ON T0.""TransId"" = T2.""TransId"" ");
			//sb.AppendLine($@"       WHERE T0.""PayNoDoc"" = 'Y' AND T0.""Canceled"" = 'N' AND T1.""CardType"" = 'C' AND T2.""ShortName"" = T0.""CardCode"" AND T2.""IntrnMatch"" = 0 AND T0.""OpenBal"" <> 0 ");
			//sb.AppendLine($@"       UNION ALL ");
			//sb.AppendLine($@"       SELECT 'MIAMI' AS ""BD"", 'RD', T2.""CardCode"", T2.""CardFName"", '', T0.""TransId"", '', CAST(T0.""RefDate"" AS DATE), T3.""PymntGroup"", CAST(T1.""DueDate"" AS DATE), (T1.""Debit"" - T1.""Credit"") AS ""Balance"" ");
			//sb.AppendLine($@"              , DAYS_BETWEEN(T1.""DueDate"", NOW()) AS ""Days"", (CASE WHEN (DAYS_BETWEEN(T1.""DueDate"", NOW()) > 7 AND (T1.""Debit"" - T1.""Credit"") > 0) THEN 'HOLD' ELSE '' END) AS ""SITUACION"" ");
			//sb.AppendLine($@"       FROM {DBLA}.""OJDT"" T0 ");
			//sb.AppendLine($@"            INNER JOIN {DBLA}.JDT1 T1 ON T0.""TransId"" = T1.""TransId"" ");
			//sb.AppendLine($@"            INNER JOIN {DBLA}.OCRD T2 ON T1.""ShortName"" = T2.""CardCode"" ");
			//sb.AppendLine($@"            INNER JOIN {DBLA}.OCTG T3 ON T3.""GroupNum"" = T2.""GroupNum"" ");
			//sb.AppendLine($@"       WHERE T1.""Account"" <> T1.""ShortName"" AND T0.""TransType"" = '30' AND T1.""IntrnMatch"" = 0 AND T2.""CardType"" = 'C' AND (T1.""Debit"" - T1.""Credit"") <> 0 AND T1.""MthDate"" = '' ");
			//sb.AppendLine($@"       UNION ALL ");
			//sb.AppendLine($@"       SELECT 'MIAMI' AS ""BD"", 'NC', T0.""CardCode"", T2.""CardFName"", T4.""WhsName"" AS ""ALMACEN"", T0.""DocNum"", T1.""BaseRef"", CAST(T0.""DocDate"" AS DATE), '', CAST(T0.""DocDueDate"" AS DATE) ");
			//sb.AppendLine($@"              , (-T0.""DocTotal"" + T0.""PaidToDate"") AS ""Balance"", DAYS_BETWEEN(T0.""DocDueDate"", NOW()) AS ""Days"" ");
			//sb.AppendLine($@"              , (CASE WHEN (DAYS_BETWEEN(T0.""DocDueDate"", NOW()) > 7 AND (-T0.""DocTotal"" + T0.""PaidToDate"") > 0) THEN 'HOLD' ELSE '' END) AS ""SITUACION"" ");
			//sb.AppendLine($@"       FROM {DBLA}.""ORIN"" T0 ");
			//sb.AppendLine($@"            INNER JOIN {DBLA}.""RIN1"" T1 ON T0.""DocEntry"" = T1.""DocEntry"" ");
			//sb.AppendLine($@"            INNER JOIN {DBLA}.OCRD T2 ON T2.""CardCode"" = T0.""CardCode"" ");
			//sb.AppendLine($@"            LEFT OUTER JOIN {DBLA}.OWHS T4 ON T4.""WhsCode"" = T1.""WhsCode"" ");
			//sb.AppendLine($@"       WHERE T0.""DocSubType"" = '--' AND T0.""DocStatus"" = 'O' ");
			//sb.AppendLine($@"       GROUP BY T0.""DocNum"", T0.""DocDate"", T0.""DocDueDate"", T0.""CardCode"", T2.""CardFName"", T4.""WhsName"", T1.""BaseRef"", (-T0.""DocTotal"" + T0.""PaidToDate""), DAYS_BETWEEN(T0.""DocDueDate"", NOW())) AS TA ");
			//if (!string.IsNullOrEmpty(filter)) sb.AppendLine($@"WHERE   {filter} ");
			//sb.AppendLine($@"ORDER BY {GetOrder(Order)} ");
			query = $@"SELECT  TA.""Code"", TA.""Name"", TA.""Type"", CAST(TA.""Number"" AS VARCHAR(10)) AS ""Number"", TA.""Date"", TA.""Term"", TA.""Expires"", TA.""Days"", TA.""Balance""
FROM    (SELECT 'Miami' AS ""Subsidiary"", 'RF' AS ""Type"", T0.""CardCode"" AS ""Code"", T3.""CardFName"" AS ""Name"", T0.U_ALMACEN AS ""Warehouse"", T0.""DocNum"" AS ""Number""
				, T1.""BaseRef"" AS ""DocBase"", CAST(T0.""DocDate"" AS DATE) AS ""Date"", T2.""PymntGroup"" AS ""Term"", CAST(T5.""DueDate"" AS DATE) AS ""Expires"", (T5.""InsTotal"" - T5.""PaidToDate"") AS ""Balance""
				, DAYS_BETWEEN(T5.""DueDate"", NOW()) AS ""Days"", (CASE WHEN ((DAYS_BETWEEN(T5.""DueDate"", NOW()) > 0) AND (T5.""InsTotal"" - T5.""PaidToDate"") > 0) THEN 'HOLD' ELSE ' ' END) AS ""HoldVen""
				, IFNULL((SELECT REPLACE(CAST((SELECT TA.""Header"" FROM {DBLA}.""ORDR"" TA WHERE TA.""DocEntry"" = T1.""BaseEntry"") AS varchar(5000)), ' ', ' -  ') FROM DUMMY), '') AS ""COM_ADD"" 
		 FROM   {DBLA}.OINV T0 
				INNER JOIN {DBLA}.INV1 T1 ON T0.""DocEntry"" = T1.""DocEntry"" AND (T0.""DocTotal"" - T0.""PaidToDate"") <> 0 
				INNER JOIN {DBLA}.OCTG T2 ON T0.""GroupNum"" = T2.""GroupNum"" 
				INNER JOIN {DBLA}.OCRD T3 ON T3.""CardCode"" = t0.""CardCode"" 
				INNER JOIN {DBLA}.INV6 T5 ON T5.""DocEntry"" = T0.""DocEntry""
		 WHERE  (T5.""InsTotal"" - T5.""PaidToDate"") > 0
		 GROUP BY T0.""DocNum"", T0.""DocDate"", T5.""DueDate"", T5.""InsTotal"", DAYS_BETWEEN(T0.""DocDueDate"", NOW()), T0.""CardCode"", T3.""CardFName"", T0.U_ALMACEN, T1.""BaseRef"", T2.""PymntGroup""
				, (T5.""InsTotal"" - T5.""PaidToDate""), T1.""BaseEntry"" 
		 UNION ALL 
		 SELECT 'Miami' AS ""BD"", 'RD', T2.""CardCode"", T2.""CardFName"", '', T0.""TransId"", '', CAST(T1.""RefDate"" AS DATE) AS ""FECHA"", '', CAST(T1.""DueDate"" AS DATE) AS ""VENCE"", (T1.""BalDueDeb""-T1.""BalDueCred"") AS ""Balance""
				, DAYS_BETWEEN(T1.""DueDate"", NOW()) AS ""Days"", (CASE WHEN (DAYS_BETWEEN(T1.""DueDate"", NOW()) > 0 AND (T1.""Debit"" - T1.""Credit"") > 0) THEN 'HOLD' ELSE '' END) AS ""SITUACION"", '' 
		 FROM   {DBLA}.OJDT T0 
				INNER JOIN {DBLA}.JDT1 T1 ON T0.""TransId"" = T1.""TransId"" 
				INNER JOIN {DBLA}.OCRD T2 ON T1.""ShortName"" = T2.""CardCode"" 
				INNER JOIN {DBLA}.OCTG T3 ON T3.""GroupNum"" = T2.""GroupNum"" 
		 WHERE  (T1.""Account"" <> T1.""ShortName"" AND T0.""TransType"" = '30' AND T1.""IntrnMatch"" = 0 AND T2.""CardType"" = 'C' AND (T1.""Debit"" - T1.""Credit"") <> 0 AND (T1.""BalDueDeb""-T1.""BalDueCred"") <> 0) 
		 UNION ALL 
		 SELECT 'Miami' AS ""BD"", 'RD', T2.""CardCode"", T2.""CardFName"", '', T0.""TransId"", '', CAST(T0.""RefDate"" AS DATE), T3.""PymntGroup"", CAST(T1.""DueDate"" AS DATE), (T1.""Debit"" - T1.""Credit"") AS ""Balance""
				, DAYS_BETWEEN(T1.""DueDate"", NOW()) AS ""Days"", (CASE WHEN (DAYS_BETWEEN(T1.""DueDate"", NOW()) > 0 AND (T1.""Debit"" - T1.""Credit"") > 0) THEN 'HOLD' ELSE '' END) AS ""SITUACION"", '' 
		 FROM   {DBLA}.OJDT T0 
				INNER JOIN {DBLA}.JDT1 T1 ON T0.""TransId"" = T1.""TransId"" 
				INNER JOIN {DBLA}.OCRD T2 ON T1.""ShortName"" = T2.""CardCode"" 
				INNER JOIN {DBLA}.OCTG T3 ON T3.""GroupNum"" = T2.""GroupNum"" 
		 WHERE  T1.""Account"" <> T1.""ShortName"" AND T0.""TransType"" = '30' AND T1.""IntrnMatch"" = 0 AND T2.""CardType"" = 'C' AND (T1.""Debit"" - T1.""Credit"") <> 0 AND T1.""MthDate"" = '' 
		 UNION ALL 
		 SELECT 'Miami' AS ""BD"", 'PRC', T0.""CardCode"", T2.""CardFName"", '', T0.""DocNum"", T1.""BaseRef"", CAST(T0.""DocDate"" AS DATE), '', CAST(T0.""DocDueDate"" AS DATE), T0.""OpenBal"" * -1
				, DAYS_BETWEEN(T0.""DocDueDate"", NOW()), (CASE WHEN ((DAYS_BETWEEN(T0.""DocDueDate"", NOW()) > 0) AND (T0.""OpenBal"" * -1) > 0) THEN 'HOLD' ELSE '' END) AS ""SITUACION"", '' 
		 FROM   {DBLA}.ORCT T0 
				INNER JOIN {DBLA}.JDT1 T1 ON T0.""TransId"" = T1.""TransId"" 
				INNER JOIN {DBLA}.OCRD T2 ON T2.""CardCode"" = T0.""CardCode"" 
		 WHERE  T0.""PayNoDoc"" = 'Y' AND T0.""Canceled"" = 'N' AND T0.""DocType"" <> 'A' AND T0.""Canceled"" = 'N' AND T1.""ShortName"" = T0.""CardCode"" AND T1.""IntrnMatch"" = 0 AND T0.""OpenBal"" <> 0 
		 UNION 
		 SELECT 'Miami' AS ""BD"", 'NC', T0.""CardCode"", T2.""CardFName"", '', T0.""DocNum"", T1.""BaseRef"", CAST(T0.""DocDate"" AS DATE), '', CAST(T0.""DocDueDate"" AS DATE), (-T0.""DocTotal"" + T0.""PaidToDate"") AS ""Balance""
				, DAYS_BETWEEN(T0.""DocDueDate"", NOW()) AS ""Days"", (CASE WHEN (DAYS_BETWEEN(T0.""DocDueDate"", NOW()) > 0 AND (-T0.""DocTotal"" + T0.""PaidToDate"") > 0) THEN 'HOLD' ELSE '' END) AS ""SITUACION"", '' 
		 FROM   {DBLA}.ORIN T0 
				INNER JOIN {DBLA}.RIN1 T1 ON T0.""DocEntry"" = T1.""DocEntry"" 
				INNER JOIN {DBLA}.OCRD T2 ON T2.""CardCode"" = T0.""CardCode"" 
				LEFT OUTER JOIN {DBLA}.OWHS T4 ON T4.""WhsCode"" = T1.""WhsCode"" 
		 WHERE  T0.""DocSubType"" = '--' AND T0.""DocStatus"" = 'O'
		 GROUP BY T0.""DocNum"", T0.""DocDate"", T0.""DocDueDate"", T0.""CardCode"", T0.""CardName"", T2.""CardFName"", T1.""BaseRef"", (-T0.""DocTotal"" + T0.""PaidToDate""), T0.U_ALMACEN, DAYS_BETWEEN(T0.""DocDueDate"", NOW()) ) AS TA 
WHERE   {filter}
ORDER BY {GetOrder(Order)} ";

			IEnumerable<BEA.BalanceDetail> items = SQLList(query);
			return items;
		}

		public IEnumerable<BEA.BalanceDetail> ListProvidersLA(List<Field> FilterList, string Order)
		{
			string filter = FilterList?.Count > 0 ? GetFilter(FilterList.ToArray()) : "1 = 1", query;
			//var sb = new StringBuilder();
			//sb.AppendLine($@"SELECT ""Name"", ""Type"", ""Base"" AS ""Number"", ""Date"", ""Term"", ""Expires"", ""Days"", ""Balance"" * -1 AS ""Balance""");
			//sb.AppendLine($@"FROM ( SELECT 'FAC' AS ""Type"", T0.""CardCode"" AS ""Id"", T3.""CardFName"" AS ""Name"", T0.""DocNum"" AS ""Number"", T0.""NumAtCard"" AS ""Base"", CAST(T0.""DocDate"" AS DATE) AS ""Date"", T2.""PymntGroup"" AS ""Term""");
			//sb.AppendLine($@"              , CAST(T0.""DocDueDate"" AS DATE) AS ""Expires"", ((T0.""DocTotal"" - T0.""PaidToDate"") * -1) AS ""Balance"", DAYS_BETWEEN(T0.""DocDueDate"", NOW()) AS ""Days""");
			//sb.AppendLine($@"       FROM {DBLA}.OPCH T0");
			//sb.AppendLine($@"            INNER JOIN {DBLA}.OCTG T2 ON T0.""GroupNum"" = T2.""GroupNum""");
			//sb.AppendLine($@"            INNER JOIN {DBLA}.OCRD T3 ON T3.""CardCode"" = T0.""CardCode""");
			//sb.AppendLine($@"       WHERE ((T0.""DocTotal"" - T0.""PaidToDate"") * -1) <> 0 AND T0.""DocSubType"" = '--' AND (T0.""CardCode"" LIKE 'B%' OR T0.""CardCode"" LIKE 'M%') AND (T0.""CardCode"" <> 'MTOS-002' AND T0.""CardCode"" <> 'MBEN-002')");
			//sb.AppendLine($@"       GROUP BY T0.""DocNum"", T0.""CardCode"", T0.""DocDate"", T0.""DocDueDate"", T3.""CardFName"", T2.""PymntGroup"", T0.""NumAtCard"", (T0.""DocTotal"" - T0.""PaidToDate"")");
			//sb.AppendLine($@"       UNION");
			//sb.AppendLine($@"       SELECT 'ND', T0.""CardCode"", T3.""CardFName"", T0.""DocNum"", T0.""NumAtCard"" AS ""FAC_PROV"", CAST(T0.""DocDate"" AS DATE), T2.""PymntGroup"", CAST(T0.""DocDueDate"" AS DATE)");
			//sb.AppendLine($@"              , ((T0.""DocTotal"" - T0.""PaidToDate"") * -1) AS ""Balance"", DAYS_BETWEEN(T0.""DocDueDate"", NOW()) AS ""Days""");
			//sb.AppendLine($@"       FROM {DBLA}.OPCH T0");
			//sb.AppendLine($@"            INNER JOIN {DBLA}.OCTG T2 ON T0.""GroupNum"" = T2.""GroupNum""");
			//sb.AppendLine($@"            INNER JOIN {DBLA}.OCRD T3 ON T3.""CardCode"" = T0.""CardCode""");
			//sb.AppendLine($@"       WHERE T0.CANCELED = 'N' AND T0.""DocSubType"" = 'DM' AND (T0.""DocTotal"" - T0.""PaidToDate"") * -1 <> 0 AND (T0.""CardCode"" LIKE 'B%' OR T0.""CardCode"" LIKE 'M%')");
			//sb.AppendLine($@"             AND (T0.""CardCode"" <> 'MTOS-002' AND T0.""CardCode"" <> 'MBEN-002')");
			//sb.AppendLine($@"       GROUP BY T0.""DocNum"", T0.""DocDate"", T0.""DocDueDate"", DAYS_BETWEEN(T0.""DocDueDate"", NOW()), T0.""CardCode"", T3.""CardFName"", T2.""PymntGroup"", T0.""NumAtCard"", (T0.""DocTotal"" - T0.""PaidToDate"")");
			//sb.AppendLine($@"       UNION");
			//sb.AppendLine($@"       SELECT 'PRC', T0.""CardCode"", T2.""CardFName"", T0.""DocNum"", '', CAST(T0.""DocDate"" AS DATE), '', CAST(T0.""DocDueDate"" AS DATE), T0.""OpenBal"" * -1, DAYS_BETWEEN(T0.""DocDueDate"", NOW())");
			//sb.AppendLine($@"       FROM {DBLA}.ORCT T0");
			//sb.AppendLine($@"            INNER JOIN {DBLA}.JDT1 T1 ON T0.""TransId"" = T1.""TransId""");
			//sb.AppendLine($@"            INNER JOIN {DBLA}.OCRD T2 ON T2.""CardCode"" = T0.""CardCode""");
			//sb.AppendLine($@"       WHERE T0.""PayNoDoc"" = 'Y' AND T0.""Canceled"" = 'N' AND T0.""DocType"" = 'S' AND T0.""Canceled"" = 'N' AND T1.""ShortName"" = T0.""CardCode"" AND T1.""IntrnMatch"" = 0");
			//sb.AppendLine($@"             AND (T0.""CardCode"" LIKE 'B%' OR T0.""CardCode"" LIKE 'M%') AND (T0.""CardCode"" <> 'MTOS-002' AND T0.""CardCode"" <> 'MBEN-002')");
			//sb.AppendLine($@"       UNION");
			//sb.AppendLine($@"       SELECT 'PEC', T0.""CardCode"", T1.""CardFName"", T0.""DocNum"", '', CAST(T0.""DocDate"" AS DATE), '', CAST(T0.""DocDueDate"" AS DATE), T0.""OpenBal"", DAYS_BETWEEN(T0.""DocDueDate"", NOW()) AS ""Days""");
			//sb.AppendLine($@"       FROM {DBLA}.OVPM T0");
			//sb.AppendLine($@"            INNER JOIN {DBLA}.OCRD T1 ON T0.""CardCode"" = T1.""CardCode""");
			//sb.AppendLine($@"            INNER JOIN {DBLA}.JDT1 T2 ON T0.""TransId"" = T2.""TransId""");
			//sb.AppendLine($@"       WHERE T0.""PayNoDoc"" = 'Y' AND T0.""Canceled"" = 'N' AND T1.""CardType"" = 'S' AND T2.""ShortName"" = T0.""CardCode"" AND T2.""IntrnMatch"" = 0 AND T0.""OpenBal"" > 0");
			//sb.AppendLine($@"             AND (T0.""CardCode"" LIKE 'B%' OR T0.""CardCode"" LIKE 'M%') AND (T0.""CardCode"" <> 'MTOS-002' AND T0.""CardCode"" <> 'MBEN-002')");
			//sb.AppendLine($@"       UNION");
			//sb.AppendLine($@"       SELECT 'RD', T2.""CardCode"", T2.""CardFName"", T0.""TransId"", '', CAST(T0.""RefDate"" AS DATE), T3.""PymntGroup"", CAST(T1.""DueDate"" AS DATE), ((T1.""Credit"" - T1.""Debit"") * -1) AS ""Balance"", DAYS_BETWEEN(T1.""DueDate"", NOW()) AS ""Days""");
			//sb.AppendLine($@"       FROM {DBLA}.OJDT T0");
			//sb.AppendLine($@"            INNER JOIN {DBLA}.JDT1 T1 ON T0.""TransId"" = T1.""TransId""");
			//sb.AppendLine($@"            INNER JOIN {DBLA}.OCRD T2 ON T1.""ShortName"" = T2.""CardCode""");
			//sb.AppendLine($@"            INNER JOIN {DBLA}.OCTG T3 ON T3.""GroupNum"" = T2.""GroupNum""");
			//sb.AppendLine($@"       WHERE T1.""Account"" <> T1.""ShortName"" AND T0.""TransType"" = '30' AND T1.""IntrnMatch"" = 0 AND T2.""CardType"" = 'S' AND ((T1.""Credit"" - T1.""Debit"") * -1) <> 0 AND T1.""MthDate"" = ''");
			//sb.AppendLine($@"             AND (T2.""CardCode"" LIKE 'B%' OR T2.""CardCode"" LIKE 'M%') AND (T2.""CardCode"" <> 'MTOS-002' AND T2.""CardCode"" <> 'MBEN-002')");
			//sb.AppendLine($@"       UNION");
			//sb.AppendLine($@"       SELECT 'NC', T0.""CardCode"", T2.""CardFName"", T0.""DocNum"", T0.""NumAtCard"", CAST(T0.""DocDate"" AS DATE), '', CAST(T0.""DocDueDate"" AS DATE), ((-T0.""DocTotal"" + T0.""PaidToDate"") * -1) AS ""Balance"", DAYS_BETWEEN(T0.""DocDueDate"", NOW()) AS ""Days""");
			//sb.AppendLine($@"       FROM {DBLA}.ORPC T0");
			//sb.AppendLine($@"            INNER JOIN {DBLA}.OCRD T2 ON T2.""CardCode"" = T0.""CardCode""");
			//sb.AppendLine($@"       WHERE T0.""DocSubType"" = '--' AND T0.""DocStatus"" = 'O' AND (T0.""CardCode"" LIKE 'B%' OR T0.""CardCode"" LIKE 'M%') AND (T0.""CardCode"" <> 'MTOS-002' AND T0.""CardCode"" <> 'MBEN-002')");
			//sb.AppendLine($@"       GROUP BY T0.""DocNum"", T0.""DocDate"", T0.""DocDueDate"", T0.""CardCode"", T2.""CardFName"", T0.""NumAtCard"", ((-T0.""DocTotal"" + T0.""PaidToDate"") * -1), DAYS_BETWEEN(T0.""DocDueDate"", NOW())) AS TA");
			//if (!string.IsNullOrEmpty(filter)) sb.AppendLine($@"WHERE   {filter} ");
			//sb.AppendLine($@"ORDER BY {GetOrder(Order)} ");
			query = $@"SELECT  ""Name"", ""Type"", ""Number"", CAST(""Date"" AS DATE) AS ""Date"", ""Term"", CAST(""Expires"" AS DATE) AS ""Expires"", ""Days"", ""Balance"" * -1 AS ""Balance"" 
FROM    (SELECT 'FAC' AS ""Type"", T0.""CardCode"" AS ""CODIGO"", T3.""CardFName"" AS ""Name""
				, T0.""DocNum"" AS ""NO_DOC"", T0.""NumAtCard"" AS ""Number"", T0.""DocDate"" AS ""Date"", T2.""PymntGroup"" AS ""Term"", T0.""DocDueDate"" AS ""Expires""
				, ((T0.""DocTotal"" - T0.""PaidToDate"") * -1) AS ""Balance"", DAYS_BETWEEN(T0.""DocDueDate"", NOW()) AS ""Days"" 
		 FROM   {DBLA}.OPCH T0 
				INNER JOIN {DBLA}.OCTG T2 ON T0.""GroupNum"" = T2.""GroupNum"" 
				INNER JOIN {DBLA}.OCRD T3 ON T3.""CardCode"" = t0.""CardCode"" 
		 WHERE  ((T0.""DocTotal"" - T0.""PaidToDate"") * -1) <> 0 AND T0.""DocSubType"" = '--' AND (T0.""CardCode"" LIKE 'B%' OR T0.""CardCode"" LIKE 'M%') AND (T0.""CardCode"" <> 'MTOS-002' AND T0.""CardCode"" <> 'MBEN-002') 
		 GROUP BY T0.""DocNum"", T0.""CardCode"", T0.""DocDate"", T0.""DocDueDate"", T3.""CardFName"", T2.""PymntGroup"", T0.""NumAtCard"", (T0.""DocTotal"" - T0.""PaidToDate"") 
		 UNION 
		 SELECT 'ND', T0.""CardCode"", T3.""CardFName""
				, T0.""DocNum"", T0.""NumAtCard"" AS ""FAC_PROV"", T0.""DocDate"", T2.""PymntGroup"", T0.""DocDueDate""
				, ((T0.""DocTotal"" - T0.""PaidToDate"") * -1) AS ""Balance"", DAYS_BETWEEN(T0.""DocDueDate"", NOW()) AS ""Days"" 
		 FROM   {DBLA}.OPCH T0 
				INNER JOIN {DBLA}.OCTG T2 ON T0.""GroupNum"" = T2.""GroupNum"" 
				INNER JOIN {DBLA}.OCRD T3 ON T3.""CardCode"" = t0.""CardCode"" 
		 WHERE  T0.CANCELED = 'N' AND T0.""DocSubType"" = 'DM' AND (T0.""DocTotal"" - T0.""PaidToDate"") * -1 <> 0 AND (T0.""CardCode"" LIKE 'B%' OR T0.""CardCode"" LIKE 'M%') AND (T0.""CardCode"" <> 'MTOS-002' AND T0.""CardCode"" <> 'MBEN-002') 
		 GROUP BY T0.""DocNum"", T0.""DocDate"", T0.""DocDueDate"", DAYS_BETWEEN(T0.""DocDueDate"", NOW()), T0.""CardCode"", T3.""CardFName"", T2.""PymntGroup"", T0.""NumAtCard"", (T0.""DocTotal"" - T0.""PaidToDate"") 
		 UNION 
		 SELECT 'PRC', T0.""CardCode"", T2.""CardFName""
				, T0.""DocNum"", '', T0.""DocDate"", '', T0.""DocDueDate"", T0.""OpenBal"" * -1, DAYS_BETWEEN(T0.""DocDueDate"", NOW()) 
		 FROM   {DBLA}.ORCT T0 
				INNER JOIN {DBLA}.JDT1 T1 ON T0.""TransId"" = T1.""TransId"" 
				INNER JOIN {DBLA}.OCRD T2 ON T2.""CardCode"" = T0.""CardCode"" 
		 WHERE  T0.""PayNoDoc"" = 'Y' AND T0.""Canceled"" = 'N'	AND (T0.""OpenBal"" * -1) != 0 AND T0.""DocType"" = 'S'	AND T0.""Canceled"" = 'N'	AND T1.""ShortName"" = T0.""CardCode"" 
				AND T1.""IntrnMatch"" = 0	AND (T0.""CardCode"" LIKE 'B%' OR T0.""CardCode"" LIKE 'M%') AND (T0.""CardCode"" <> 'MTOS-002' AND T0.""CardCode"" <> 'MBEN-002') 
		 UNION 
		 SELECT 'PEC', T0.""CardCode"", T1.""CardFName""
				, T0.""DocNum"", '', T0.""DocDate"", '', T0.""DocDueDate"", T0.""OpenBal"", DAYS_BETWEEN(T0.""DocDueDate"", NOW()) AS ""Days"" 
		 FROM   {DBLA}.OVPM T0 
				INNER JOIN {DBLA}.OCRD T1 ON T0.""CardCode"" = T1.""CardCode"" 
				INNER JOIN {DBLA}.JDT1 T2 ON T0.""TransId"" = T2.""TransId"" 
		 WHERE  T0.""PayNoDoc"" = 'Y' AND T0.""Canceled"" = 'N'	AND T1.""CardType"" = 'S'	AND T2.""ShortName"" = T0.""CardCode"" AND T2.""IntrnMatch"" = 0 AND T0.""OpenBal"" > 0	
				AND (T0.""CardCode"" LIKE 'B%' OR T0.""CardCode"" LIKE 'M%') AND (T0.""CardCode"" <> 'MTOS-002' AND T0.""CardCode"" <> 'MBEN-002') 
		 UNION 
		 SELECT 'RD', T2.""CardCode"", T2.""CardFName""
				, T0.""TransId"", '', T0.""RefDate"", T3.""PymntGroup"", T1.""DueDate"", ((T1.""Credit"" - T1.""Debit"") * -1) AS ""Balance"", DAYS_BETWEEN(T1.""DueDate"", NOW()) AS ""Days""	
		 FROM   {DBLA}.OJDT T0 
				INNER JOIN {DBLA}.JDT1 T1 ON T0.""TransId"" = T1.""TransId"" 
				INNER JOIN {DBLA}.OCRD T2 ON T1.""ShortName"" = T2.""CardCode"" 
				INNER JOIN {DBLA}.OCTG T3 ON T3.""GroupNum"" = T2.""GroupNum"" 
		 WHERE  T1.""Account"" <> T1.""ShortName"" AND T0.""TransType"" = '30' AND T1.""IntrnMatch"" = 0 AND T2.""CardType"" = 'S' AND ((T1.""Credit"" - T1.""Debit"") * -1) <> 0  
				AND T1.""MthDate"" = '' AND (T2.""CardCode"" LIKE 'B%' OR T2.""CardCode"" LIKE 'M%') AND (T2.""CardCode"" <> 'MTOS-002' AND T2.""CardCode"" <> 'MBEN-002') 
		 UNION 
		 SELECT 'NC', T0.""CardCode"", T2.""CardFName""
				, T0.""DocNum"", T0.""NumAtCard"", T0.""DocDate"", '', T0.""DocDueDate"", ((-T0.""DocTotal"" + T0.""PaidToDate"") * -1) AS ""Balance"", DAYS_BETWEEN(T0.""DocDueDate"", NOW()) AS ""Days"" 
		 FROM   {DBLA}.ORPC T0 
				INNER JOIN {DBLA}.OCRD T2 ON T2.""CardCode"" = T0.""CardCode"" 
		 WHERE  T0.""DocSubType"" = '--' AND T0.""DocStatus"" = 'O'	AND (T0.""CardCode"" LIKE 'B%' OR T0.""CardCode"" LIKE 'M%') AND (T0.""CardCode"" <> 'MTOS-002' AND T0.""CardCode"" <> 'MBEN-002') 
		 GROUP BY T0.""DocNum"", T0.""DocDate"", T0.""DocDueDate"", T0.""CardCode"", T2.""CardFName"", T0.""NumAtCard"", ((-T0.""DocTotal"" + T0.""PaidToDate"") * -1), DAYS_BETWEEN(T0.""DocDueDate"", NOW())) AS TA 
WHERE   {filter} 
ORDER BY {GetOrder(Order)} ";

			IEnumerable<BEA.BalanceDetail> items = SQLList(query);
			return items;
		}

		#endregion

		#region Search Methods

		#endregion

		#region Constructors

		public BalanceDetail() : base() { }

		#endregion
	}
}
