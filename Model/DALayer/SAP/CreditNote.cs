using BEntities.Filters;
using BEntities.SAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BEA = BEntities.SAP;

namespace DALayer.SAP.Hana
{
	[Serializable()]
	public class CreditNote : DALEntity<BEA.CreditNote>
	{
		#region Methods

		protected override void LoadRelations(ref BEA.CreditNote Item, params Enum[] Relations) { }

		protected override void LoadRelations(ref IEnumerable<BEA.CreditNote> Items, params Enum[] Relations) { }

		#endregion

		#region List Methods

		public IEnumerable<BEA.CreditNote> List(List<Field> Filters, string SortingBy)
		{
			string filters = Filters?.Count > 0 ? GetFilter(Filters.ToArray()) : "1 = 1", query = $@"SELECT *
FROM	 ( SELECT 'Santa Cruz' AS ""Subsidiary"", ""DocEntry"" AS ""Id"", ""DocNum"" AS ""DocNumber"", CAST(""DocDate"" AS DATE) AS ""DocDate"", CAST(""DocDueDate"" AS DATE) AS ""DocDueDate""
				  , ""CardCode"" AS ""ClientCode"", ""CardName"" AS ""ClientName"", ""Address"", ""NumAtCard"" AS ""Reference"", t1.""SlpName"" AS ""SellerCode"", ""DocTotal"" AS ""Total"", ""Comments""
				  , ""JrnlMemo"" AS ""Memo"", ""TransId""
		   FROM	  {DBSA}.ORIN t0
				  INNER JOIN {DBSA}.OSLP t1 ON t0.""SlpCode"" = t1.""SlpCode""
		   WHERE  CANCELED = 'N'
		   UNION
		   SELECT 'Iquique' AS ""Subsidiary"", ""DocEntry"" AS ""Id"", ""DocNum"" AS ""DocNumber"", CAST(""DocDate"" AS DATE) AS ""DocDate"", CAST(""DocDueDate"" AS DATE) AS ""DocDueDate""
				  , ""CardCode"" AS ""ClientCode"", ""CardName"" AS ""ClientName"", ""Address"", ""NumAtCard"" AS ""Reference"", t1.""SlpName"" AS ""SellerCode"", ""DocTotal"" AS ""Total"", ""Comments""
				  , ""JrnlMemo"" AS ""Memo"", ""TransId""
		   FROM	  {DBIQ}.ORIN t0
				  INNER JOIN {DBIQ}.OSLP t1 ON t0.""SlpCode"" = t1.""SlpCode""
		   UNION
		   SELECT 'Miami' AS ""Subsidiary"", ""DocEntry"" AS ""Id"", ""DocNum"" AS ""DocNumber"", CAST(""DocDate"" AS DATE) AS ""DocDate"", CAST(""DocDueDate"" AS DATE) AS ""DocDueDate""
				  , ""CardCode"" AS ""ClientCode"", ""CardName"" AS ""ClientName"", ""Address"", ""NumAtCard"" AS ""Reference"", t1.""SlpName"" AS ""SellerCode"", ""DocTotal"" AS ""Total"", ""Comments""
				  , ""JrnlMemo"" AS ""Memo"", ""TransId""
		   FROM	  {DBLA}.ORIN t0
				  INNER JOIN {DBLA}.OSLP t1 ON t0.""SlpCode"" = t1.""SlpCode"" ) a
WHERE    {filters}
ORDER BY {GetOrder(SortingBy)} ";
			IEnumerable<BEA.CreditNote> items = SQLList(query);
			return items;
		}

		public IEnumerable<BEA.CreditNoteItem> ListItems(string Subsidiary, long DocNumber)
		{
			string query = $@"SELECT *
							FROM	 ( SELECT 'Santa Cruz' AS ""Subsidiary"", ""DocNum"" AS ""DocNumber"", ""ItemCode"", ""Dscription"" AS ""ItemName"", CAST(""Quantity"" AS INT) AS ""Quantity"", ""PriceAfVAT"" AS ""Price"", ""GTotal"" AS ""Total"", U_COD_FAB AS ""ManufacterCode"", CAST(IFNULL(""AcctCode"", '0') AS INT) AS ""AccountCode"", U_NOMBRE_CUENTA AS ""AccountName""
									   FROM	  {DBSA}.ORIN t0
											  INNER JOIN {DBSA}.RIN1 t1 ON t0.""DocEntry"" = t1.""DocEntry""
									   UNION ALL
									   SELECT 'Iquique' AS ""Subsidiary"", ""DocNum"" AS ""DocNumber"", ""ItemCode"", ""Dscription"" AS ""ItemName"", CAST(""Quantity"" AS INT) AS ""Quantity"", ""PriceAfVAT"" AS ""Price"", ""GTotal"" AS ""Total"", U_COD_FAB AS ""ManufacterCode"", CAST(IFNULL(""AcctCode"", '0') AS INT) AS ""AccountCode"", U_NOMBRE_CUENTA AS ""AccountName""
									   FROM	  {DBIQ}.ORIN t0
											  INNER JOIN {DBIQ}.RIN1 t1 ON t0.""DocEntry"" = t1.""DocEntry""
									   UNION ALL
									   SELECT 'Miami' AS ""Subsidiary"", ""DocNum"" AS ""DocNumber"", ""ItemCode"", ""Dscription"" AS ""ItemName"", CAST(""Quantity"" AS INT) AS ""Quantity"", ""PriceAfVAT"" AS ""Price"", ""GTotal"" AS ""Total"", U_COD_FAB AS ""ManufacterCode"", CAST(IFNULL(""AcctCode"", '0') AS INT) AS ""AccountCode"", U_NOMBRE_CUENTA AS ""AccountName""
									   FROM	  {DBLA}.ORIN t0
											  INNER JOIN {DBLA}.RIN1 t1 ON t0.""DocEntry"" = t1.""DocEntry"" ) a
							WHERE    LOWER(""Subsidiary"") = '{Subsidiary.ToLower()}'
									 AND ""DocNumber"" = {DocNumber} ";
			IEnumerable<BEA.CreditNoteItem> items = SQLList<BEA.CreditNoteItem>(query);
			return items;
		}

		#endregion

		#region Search Methods

		public BEA.CreditNote Search(string Subsidiary, long DocNumber, params Enum[] Relations)
		{
			string query = $@"SELECT *
							FROM	 (SELECT  'Santa Cruz' AS ""Subsidiary"", t0.""DocEntry"" AS ""Id"", ""DocNum"" AS ""DocNumber"", t0.""DocDate"", ""DocDueDate"", ""CardCode"" AS ""ClientCode"", ""CardName"" AS ""ClientName"", t0.""Address"", ""NumAtCard"" AS ""Reference"", t2.""SlpName"" AS ""SellerCode"", ""DocTotal"" AS ""Total"", ""Comments"", ""JrnlMemo"" AS ""Memo"", ""TransId"", t1.""BaseRef"" AS ""NoteNumber"", t3.""PymntGroup"" AS ""Terms""
									  FROM	  {DBSA}.ORIN t0
											  LEFT OUTER JOIN {DBSA}.RIN1 t1 ON t0.""DocEntry"" = t1.""DocEntry"" AND t1.""LineNum"" = 0
											  INNER JOIN {DBSA}.OSLP t2 ON t0.""SlpCode"" = t2.""SlpCode""
											  LEFT OUTER JOIN {DBSA}.OCTG t3 ON t0.""GroupNum"" = t3.""GroupNum""
									  UNION ALL
									  SELECT  'Iquique' AS ""Subsidiary"", t0.""DocEntry"" AS ""Id"", ""DocNum"" AS ""DocNumber"", t0.""DocDate"", ""DocDueDate"", ""CardCode"" AS ""ClientCode"", ""CardName"" AS ""ClientName"", t0.""Address"", ""NumAtCard"" AS ""Reference"", t2.""SlpName"" AS ""SellerCode"", ""DocTotal"" AS ""Total"", ""Comments"", ""JrnlMemo"" AS ""Memo"", ""TransId"", t1.""BaseRef"" AS ""NoteNumber"", t3.""PymntGroup"" AS ""Terms""
									  FROM	  {DBIQ}.ORIN t0
											  LEFT OUTER JOIN {DBIQ}.RIN1 t1 ON t0.""DocEntry"" = t1.""DocEntry"" AND t1.""LineNum"" = 0
											  INNER JOIN {DBIQ}.OSLP t2 ON t0.""SlpCode"" = t2.""SlpCode""
											  LEFT OUTER JOIN {DBIQ}.OCTG t3 ON t0.""GroupNum"" = t3.""GroupNum""
									  UNION ALL
									  SELECT  'Miami' AS ""Subsidiary"", t0.""DocEntry"" AS ""Id"", ""DocNum"" AS ""DocNumber"", t0.""DocDate"", ""DocDueDate"", ""CardCode"" AS ""ClientCode"", ""CardName"" AS ""ClientName"", t0.""Address"", ""NumAtCard"" AS ""Reference"", t2.""SlpName"" AS ""SellerCode"", ""DocTotal"" AS ""Total"", ""Comments"", ""JrnlMemo"" AS ""Memo"", ""TransId"", t1.""BaseRef"" AS ""NoteNumber"", t3.""PymntGroup"" AS ""Terms""
									  FROM	  {DBLA}.ORIN t0
											  LEFT OUTER JOIN {DBLA}.RIN1 t1 ON t0.""DocEntry"" = t1.""DocEntry"" AND t1.""LineNum"" = 0
											  INNER JOIN {DBLA}.OSLP t2 ON t0.""SlpCode"" = t2.""SlpCode""
											  LEFT OUTER JOIN {DBLA}.OCTG t3 ON t0.""GroupNum"" = t3.""GroupNum"" ) a
							WHERE	 LOWER(""Subsidiary"") = '{Subsidiary.ToLower()}'
									 AND ""DocNumber"" = {DocNumber} ";
			BEA.CreditNote item = SQLSearch(query, Relations);
			return item;
		}

		#endregion

		#region Constructors

		public CreditNote() : base() { }

		#endregion
	}
}
