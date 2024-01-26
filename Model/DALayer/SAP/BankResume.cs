using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using BE = BEntities;
using BEA = BEntities.SAP;

namespace DALayer.SAP.Hana
{
	[Serializable()]
	public class BankResume : DALEntity<BEA.BankResume>
	{

		#region Methods

		protected override void LoadRelations(ref BEA.BankResume Item, params Enum[] Relations) { }

		protected override void LoadRelations(ref IEnumerable<BEA.BankResume> Items, params Enum[] Relations) { }

		#endregion

		#region List Methods

		public IEnumerable<BEA.BankResume> List(List<Field> FilterList, string SortingBy)
		{
			string filter = FilterList?.Count > 0 ? GetFilter(FilterList.ToArray()) : "1 = 1", query;
			query = $@"SELECT  TA.""Subsidiary"", TA.""Account"", TA.""SAPUsd"", TA.""SAPBs"", TA.""SAPCLP"", TA.""ExtractUsd"", TA.""EXTRACTO_Bs"" AS ""ExtractBs"", TA.""ExtractCLP"", TA.""Checks"" 
FROM    ( SELECT  1 AS ""ORDEN"", T2.""ActId"", 'DMC_SA' AS ""Subsidiary"", T2.""AcctName"" AS ""Account""
				  , (CASE WHEN (T2.""AcctCode"" LIKE '113%' OR T2.""AcctCode"" LIKE '11240%' OR T2.""AcctCode"" LIKE '1125%' OR T2.""AcctCode"" LIKE '1126%' OR T2.""AcctCode"" LIKE '1127%'	OR T2.""AcctCode"" LIKE '1128%') AND (T2.""AcctName"" LIKE '%DOLARES%' OR T2.""AcctName"" LIKE '%IQQ%' OR T2.""AcctName"" LIKE '%LA%') 
						  THEN ( SELECT IFNULL((SUM(TA.""Credit"") - SUM(TA.""Debit"")) * -1, 0) 
								 FROM   {DBSA}.JDT1 TA 
										INNER JOIN {DBSA}.OJDT TB ON TA.""TransId"" = TB.""TransId"" 
								 WHERE  TA.""Account"" = T2.""AcctCode"" AND TB.""RefDate"" BETWEEN '20070101' AND NOW() ) 
						  ELSE 0 END) AS ""SAPUsd""
				  , (CASE WHEN T2.""ActCurr"" = 'Bs' 
						  THEN ( SELECT IFNULL((SUM(TA.""FCCredit"") - SUM(TA.""FCDebit"")) * -1, 0) 
								 FROM   {DBSA}.JDT1 TA 
										INNER JOIN {DBSA}.OJDT TB ON TA.""TransId"" = TB.""TransId"" 
								 WHERE  TA.""Account"" = T2.""AcctCode"" AND TA.""RevSource"" = 'N' AND TB.""RefDate"" BETWEEN '20070101' AND NOW() ) 
						  ELSE 0 END) AS ""SAPBs"", 0 AS ""SAPCLP""
				  , (CASE WHEN (T2.""AcctCode"" LIKE '113%' OR T2.""AcctCode"" LIKE '11240%' OR T2.""AcctCode"" LIKE '1125%' OR T2.""AcctCode"" LIKE '1126%' OR T2.""AcctCode"" LIKE '1127%' OR T2.""AcctCode"" LIKE '1128%') AND (T2.""AcctName"" LIKE '%DOLARES%' OR T2.""AcctName"" LIKE '%IQQ%' OR T2.""AcctName"" LIKE '%LA%') 
						  THEN ( SELECT IFNULL((SUM(TA.""CredAmnt"") - SUM(TA.""DebAmount"")), 0) 
								 FROM   {DBSA}.OBNK TA 
								 WHERE  TA.""AcctCode"" = T2.""AcctCode"" AND TA.""DueDate"" BETWEEN '20070101' AND NOW() ) 
						  ELSE 0 END) AS ""ExtractUsd""
				  , (CASE WHEN T2.""ActCurr"" = 'Bs' 
						  THEN ( SELECT IFNULL((SUM(TA.""CredAmnt"") - SUM(TA.""DebAmount"")), 0) 
								 FROM   {DBSA}.OBNK TA 
								 WHERE  TA.""AcctCode"" = T2.""AcctCode"" AND TA.""DueDate"" BETWEEN '20070101' AND NOW() ) 
						  ELSE 0 END) AS ""EXTRACTO_Bs"", 0 AS ""ExtractCLP""
				  , (CASE WHEN (T2.""AcctCode"" LIKE '113%' OR T2.""AcctCode"" LIKE '11240%' OR T2.""AcctCode"" LIKE '1125%' OR T2.""AcctCode"" LIKE '1126%' OR T2.""AcctCode"" LIKE '1127%' OR T2.""AcctCode"" LIKE '1128%') AND (T2.""AcctName"" LIKE '%DOLARES%' OR T2.""AcctName"" LIKE '%IQQ%' OR T2.""AcctName"" LIKE '%LA%') 
						  THEN ( SELECT IFNULL((SUM(TA.""Credit"") - SUM(TA.""Debit"")) * -1,	0) 
								 FROM   {DBSA}.JDT1 TA 
										INNER JOIN {DBSA}.OJDT TB ON TA.""TransId"" = TB.""TransId"" 
								 WHERE  TA.""Account"" = T2.""AcctCode"" AND TA.""Credit"" > 0 AND TA.""SourceID"" IS NOT NULL AND TB.""TransType"" = 46 AND TA.""ExtrMatch"" = 0 AND TB.""RefDate"" BETWEEN '20070101'	AND NOW() ) 
						  WHEN T2.""ActCurr"" = 'Bs' 
						  THEN ( SELECT IFNULL((SUM(TA.""SYSCred"") - SUM(TA.""SYSDeb"")) * -1, 0) 
								 FROM   {DBSA}.JDT1 TA 
										INNER JOIN {DBSA}.OJDT TB ON TA.""TransId"" = TB.""TransId"" 
								 WHERE  TA.""Account"" = T2.""AcctCode"" AND TA.""SYSCred"" > 0 AND TA.""SourceID"" IS NOT NULL AND TB.""TransType"" = 46 AND (TA.""ExtrMatch"" = 0) AND TB.""RefDate"" BETWEEN '20070101'	AND NOW() ) 
						  ELSE 0 END) AS ""Checks"" 
		  FROM   {DBSA}.OACT T2 
				 LEFT OUTER JOIN {DBSA}.JDT1 T1 ON T1.""Account"" = T2.""AcctCode"" 
				 LEFT OUTER JOIN {DBSA}.OJDT T0 ON T0.""TransId"" = T1.""TransId"" 
		  WHERE  ( T2.""AcctCode"" LIKE '113%' OR T2.""AcctCode"" LIKE '11240%' OR T2.""AcctCode"" LIKE '1125%' OR T2.""AcctCode"" LIKE '1126%' OR T2.""AcctCode"" LIKE '1127%' OR T2.""AcctCode"" LIKE '1128%' ) 
				 AND (T2.""AcctCode"" <> '11199') AND T2.""FrozenFor"" = 'N' 
		  GROUP BY T2.""AcctCode"", T2.""AcctName"", T2.""ActCurr"", T2.""ActId"" 
		  UNION ALL 
		  SELECT 2 AS ""ORDEN"", T2.""ActId"", 'DMC_IQQ' AS ""BD"", T2.""AcctName"" AS ""CUENTA""
				 , (CASE WHEN ( T2.""AcctCode"" LIKE '113%' OR T2.""AcctCode"" LIKE '1123%' ) AND T2.""AcctName"" LIKE '%DOLARES%' 
						 THEN ( SELECT IFNULL((SUM(TA.""Credit"") - SUM(TA.""Debit"")) * -1, 0) 
								FROM   {DBIQ}.JDT1 TA 
									   INNER JOIN {DBIQ}.OJDT TB ON TA.""TransId"" = TB.""TransId"" 
								WHERE  TA.""Account"" = T2.""AcctCode"" AND TB.""RefDate"" BETWEEN '20070101' AND NOW() ) 
						 ELSE 0 END) AS ""SALDO USD""
				 , (CASE WHEN T2.""ActCurr"" = 'Bs' 
						 THEN ( SELECT IFNULL((SUM(TA.""FCCredit"") - SUM(TA.""FCDebit"")) * -1, 0) 
								FROM   {DBIQ}.JDT1 TA 
									   INNER JOIN {DBIQ}.OJDT TB ON TA.""TransId"" = TB.""TransId"" 
								WHERE  TA.""Account"" = T2.""AcctCode"" AND TA.""RevSource"" = 'N' AND TB.""RefDate"" BETWEEN '20070101' AND NOW() ) 
						 ELSE 0 END) AS ""SALDO Bs""
				 , (CASE WHEN T2.""ActCurr"" = 'CLP' 
						 THEN ( SELECT IFNULL((SUM(TA.""SYSCred"") - SUM(TA.""SYSDeb"")) * -1, 0) 
								FROM   {DBIQ}.JDT1 TA 
									   INNER JOIN {DBIQ}.OJDT TB ON TA.""TransId"" = TB.""TransId"" 
								WHERE  TA.""Account"" = T2.""AcctCode"" AND TB.""RefDate"" BETWEEN '20070101' AND NOW() ) 
						 ELSE 0 END) AS ""SALDO CLP""
				 , (CASE WHEN ( T2.""AcctCode"" LIKE '113%' OR T2.""AcctCode"" LIKE '1123%' ) AND T2.""AcctName"" LIKE '%DOLARES%'
						 THEN ( SELECT IFNULL((SUM(TA.""CredAmnt"") - SUM(TA.""DebAmount"")), 0) 
								FROM   {DBIQ}.OBNK TA 
								WHERE  TA.""AcctCode"" = T2.""AcctCode"" AND TA.""DueDate"" BETWEEN '20070101' AND NOW() ) 
						 ELSE 0 END) AS ""EXTRACTO USD""
				 , (CASE WHEN T2.""ActCurr"" = 'Bs' 
						 THEN ( SELECT IFNULL((SUM(TA.""CredAmnt"") - SUM(TA.""DebAmount"")), 0) 
								FROM   {DBIQ}.OBNK TA 
								WHERE  TA.""AcctCode"" = T2.""AcctCode"" AND TA.""DueDate"" BETWEEN '20070101' AND NOW() ) 
						 ELSE 0 END) AS ""EXTRACTO_Bs""
				 , (CASE WHEN T2.""ActCurr"" = 'CLP' 
						 THEN ( SELECT IFNULL((SUM(TA.""CredAmnt"") - SUM(TA.""DebAmount"")), 0) 
								FROM   {DBIQ}.OBNK TA 
								WHERE  TA.""AcctCode"" = T2.""AcctCode"" AND TA.""DueDate"" BETWEEN '20070101' AND NOW() ) 
						 ELSE 0 END)
				 , (CASE WHEN (T2.""AcctCode"" LIKE '113%' OR T2.""AcctCode"" LIKE '1123%') AND T2.""AcctName"" LIKE '%DOLARES%' 
						 THEN ( SELECT IFNULL((SUM(TA.""Credit"") - SUM(TA.""Debit"")) * -1, 0) 
								FROM   {DBIQ}.JDT1 TA 
									   INNER JOIN {DBIQ}.OJDT TB ON TA.""TransId"" = TB.""TransId"" 
								WHERE  TA.""Account"" = T2.""AcctCode"" AND TA.""Credit"" > 0 AND TA.""SourceID"" IS NOT NULL AND TB.""TransType"" = 46 AND TA.""ExtrMatch"" = 0 AND TB.""RefDate"" BETWEEN '20070101'	AND NOW() ) 
						 WHEN T2.""ActCurr"" = 'CLP' 
						 THEN ( SELECT IFNULL((SUM(TA.""SYSCred"") - SUM(TA.""SYSDeb"")) * -1, 0) 
								FROM   {DBIQ}.JDT1 TA 
									   INNER JOIN {DBIQ}.OJDT TB ON TA.""TransId"" = TB.""TransId"" 
								WHERE  TA.""Account"" = T2.""AcctCode"" AND TA.""SYSCred"" > 0	AND TA.""SourceID"" IS NOT NULL AND TB.""TransType"" = 46 AND TA.""ExtrMatch"" = 0 AND TB.""RefDate"" BETWEEN '20070101' AND NOW() ) 
						 ELSE 0 END) AS ""Cheques"" 
		  FROM   {DBIQ}.OACT T2 
				 LEFT OUTER JOIN {DBIQ}.JDT1 T1 ON T1.""Account"" = T2.""AcctCode"" 
				 LEFT OUTER JOIN {DBIQ}.OJDT T0 ON T0.""TransId"" = T1.""TransId"" 
		  WHERE  ( T2.""AcctCode"" LIKE '113%' OR T2.""AcctCode"" LIKE '1123%' )	AND T2.""AcctCode"" <> '11199' AND T2.""FrozenFor"" = 'N' 
		  GROUP BY T2.""AcctCode"", T2.""AcctName"", T2.""ActCurr"", T2.""ActId""
		  UNION ALL 
		  SELECT 3 AS ""ORDEN"", T2.""ActId"", 'DMC_LA' AS ""BD"", T2.""AcctName"" AS ""CUENTA""
				 , (CASE WHEN T2.""AcctCode"" LIKE '113%' AND (T2.""AcctName"" LIKE '%DOLARES%' OR T2.""AcctName"" LIKE '%WELLS FARGO%') 
						 THEN ( SELECT  IFNULL((SUM(TA.""Credit"") - SUM(TA.""Debit"")) * -1, 0) 
								FROM    {DBLA}.JDT1 TA 
										INNER JOIN {DBLA}.OJDT TB ON TA.""TransId"" = TB.""TransId"" 
								WHERE   TA.""Account"" = T2.""AcctCode"" AND TB.""RefDate"" BETWEEN '20070101' AND NOW() ) 
						 ELSE 0 END) AS ""SALDO USD"", 0 AS ""SALDO Bs"", 0 AS ""SALDO CLP""
				 , (CASE WHEN T2.""AcctCode"" LIKE '113%' AND (T2.""AcctName"" LIKE '%DOLARES%' OR T2.""AcctName"" LIKE '%WELLS FARGO%') 
						 THEN ( SELECT  IFNULL((SUM(TA.""CredAmnt"") - SUM(TA.""DebAmount"")), 0) 
								FROM    {DBLA}.OBNK TA 
								WHERE   TA.""AcctCode"" = T2.""AcctCode"" AND TA.""DueDate"" BETWEEN '20070101' AND NOW() ) 
						 ELSE 0 END) AS ""EXTRACTO USD"", 0 AS ""SALDO Bs"", 0 AS ""SALDO CLP""
				 , (CASE WHEN T2.""AcctCode"" LIKE '113%' AND T2.""AcctName"" LIKE '%DOLARES%'
						 THEN ( SELECT  IFNULL((SUM(TA.""Credit"") - SUM(TA.""Debit"")) * -1, 0) 
								FROM    {DBLA}.JDT1 TA 
										INNER JOIN {DBLA}.OJDT TB ON TA.""TransId"" = TB.""TransId"" 
								WHERE   TA.""Account"" = T2.""AcctCode"" AND TA.""Credit"" > 0 AND TA.""SourceID"" IS NOT NULL AND TB.""TransType"" = 46 AND TA.""ExtrMatch"" = 0 AND TB.""RefDate"" BETWEEN '20070101' AND NOW() ) 
						 ELSE 0 END) AS ""Cheques"" 
		  FROM  {DBLA}.OACT T2 
				LEFT OUTER JOIN {DBLA}.JDT1 T1 ON T1.""Account"" = T2.""AcctCode"" 
				LEFT OUTER JOIN {DBLA}.OJDT T0 ON T0.""TransId"" = T1.""TransId"" 
		  WHERE T2.""AcctCode"" LIKE '113%' AND T2.""AcctCode"" <> '11199' AND T2.""FrozenFor"" = 'N' 
		  GROUP BY T2.""AcctCode"", T2.""ActId"", T2.""AcctName"", T2.""ActCurr"" ) TA 
WHERE    {filter}
ORDER BY {GetOrder(SortingBy)} ";

			IEnumerable<BEA.BankResume> items = SQLList(query);
			return items;
		}

		#endregion

		#region Search Methods

		#endregion

		#region Constructors

		public BankResume() : base() { }

		#endregion
	}
}
