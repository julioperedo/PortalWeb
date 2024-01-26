using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE = BEntities;
using BEA = BEntities.SAP;

namespace DALayer.SAP.Hana
{
	[Serializable()]
	public class Client : DALEntity<BEA.Client>
	{

		#region Methods

		internal BEA.Client ReturnMaster(string CardCode, params Enum[] Relations)
		{
			return Search(CardCode, Relations);
		}

		protected override void LoadRelations(ref IEnumerable<BEA.Client> Items, params Enum[] Relations) { }

		protected override void LoadRelations(ref BEA.Client item, params Enum[] Relations) { }

		#endregion

		#region List Methods

		public IEnumerable<BEA.Client> List(List<Field> Filters, string Order, params Enum[] Relations)
		{
			string strFilter = Filters?.Count > 0 ? GetFilter(Filters?.ToArray()) : "";
			StringBuilder sb = new();
			sb.AppendLine($@"SELECT   * ");
			sb.AppendLine($@"FROM     ({GetQuery()}) a ");
			if (strFilter != "") sb.AppendLine($@"WHERE    {strFilter} ");
			sb.AppendLine($@"ORDER By {GetOrder(Order)} ");

			IEnumerable<BEA.Client> items = SQLList(sb.ToString(), Relations);
			return items;
		}

		public IEnumerable<BEA.Client> ListBalance(List<Field> FilterList, string Order, params Enum[] Relations)
		{
			var filter = GetFilter(FilterList?.ToArray());
			var sb = new StringBuilder();
			sb.AppendLine($@"SELECT  * ");
			sb.AppendLine($@"FROM    ( SELECT    'SANTA CRUZ' AS ""Subsidiary"", ""CardCode"", ""CardName"", ""Address"", ""MailAddres"", ""Phone1"", ""Phone2"", t0.""Fax"", ""E_Mail"" AS ""EMail"", ""CardFName"", ""LicTradNum"" AS ""NIT"", IFNULL(t1.""SlpName"", '') AS ""SellerCode"" ");
			sb.AppendLine($@"                    , IFNULL(t1.""Memo"", '') AS ""SellerName"", t2.""PymntGroup"" AS ""Terms"", IFNULL(t0.""CreditLine"", 0) AS ""CreditLimit"", t0.""Balance"", t0.""OrdersBal"" AS ""OrdersBalance"" ");
			sb.AppendLine($@"          FROM      {DBSA}.OCRD t0 ");
			sb.AppendLine($@"                    LEFT OUTER JOIN {DBSA}.OSLP t1 ON t0.""SlpCode"" = t1.""SlpCode"" ");
			sb.AppendLine($@"                    INNER JOIN {DBSA}.OCTG t2 ON t0.""GroupNum"" = t2.""GroupNum"" ");
			sb.AppendLine($@"          WHERE     ""CardType"" = 'C' ");
			sb.AppendLine($@"          UNION ");
			sb.AppendLine($@"          SELECT    'MIAMI' AS ""Subsidiary"", ""CardCode"", ""CardName"", ""Address"", ""MailAddres"", ""Phone1"", ""Phone2"", t0.""Fax"", ""E_Mail"", ""CardFName"", ""LicTradNum"", IFNULL(t1.""SlpName"", '') AS ""SellerCode"", IFNULL(t1.""Memo"", '') AS ""Seller"" ");
			sb.AppendLine($@"                    , t2.""PymntGroup"" AS ""Terms"", IFNULL(t0.""CreditLine"", 0) AS ""CreditLimit"", t0.""Balance"", t0.""OrdersBal"" AS ""OrdersBalance"" ");
			sb.AppendLine($@"          FROM      {DBLA}.OCRD t0 ");
			sb.AppendLine($@"                    LEFT OUTER JOIN {DBLA}.OSLP t1 ON t0.""SlpCode"" = t1.""SlpCode"" ");
			sb.AppendLine($@"                    INNER JOIN {DBLA}.OCTG t2 ON t0.""GroupNum"" = t2.""GroupNum"" ");
			sb.AppendLine($@"          WHERE     ""CardType"" = 'C' ");
			sb.AppendLine($@"          UNION ");
			sb.AppendLine($@"          SELECT    'IQUIQUE' AS ""Subsidiary"", ""CardCode"", ""CardName"", ""Address"", ""MailAddres"", ""Phone1"", ""Phone2"", t0.""Fax"", ""E_Mail"", ""CardFName"", ""LicTradNum"", IFNULL(t1.""SlpName"", '') AS ""SellerCode"", IFNULL(t1.""Memo"", '') AS ""Seller"" ");
			sb.AppendLine($@"                    , t2.""PymntGroup"" AS ""Terms"", IFNULL(t0.""CreditLine"", 0) AS ""CreditLimit"", t0.""Balance"", t0.""OrdersBal"" AS ""OrdersBalance"" ");
			sb.AppendLine($@"          FROM      {DBIQ}.OCRD t0 ");
			sb.AppendLine($@"                    LEFT OUTER JOIN {DBIQ}.OSLP t1 ON t0.""SlpCode"" = t1.""SlpCode"" ");
			sb.AppendLine($@"                    INNER JOIN {DBIQ}.OCTG t2 ON t0.""GroupNum"" = t2.""GroupNum"" ");
			sb.AppendLine($@"          WHERE     ""CardType"" = 'C' ) AS a ");
			sb.AppendLine($@"WHERE     {filter} ");
			sb.AppendLine($@"ORDER By {GetOrder(Order)}");

			IEnumerable<BEA.Client> items = SQLList(sb.ToString(), Relations);
			return items;
		}

		public IEnumerable<BEA.Client> ListValidNames(List<string> CardCodes, params Enum[] Relations)
		{
			string strCodes = string.Join(",", CardCodes.Select(x => $"'{x.ToLower()}'"));
			var sb = new StringBuilder();
			sb.AppendLine($@"SELECT  T0.""CardCode"", T0.""CardFName"" ");
			sb.AppendLine($@"FROM    {DBSA}.OCRD T0 ");
			sb.AppendLine($@"WHERE   LOWER(T0.""CardCode"") IN ( {strCodes} ) ");
			sb.AppendLine($@"UNION ");
			sb.AppendLine($@"SELECT  T0.""CardCode"", T1.""Name"" || ' - ' || T1.""Position"" ");
			sb.AppendLine($@"FROM    {DBSA}.OCRD T0 ");
			sb.AppendLine($@"        INNER JOIN {DBSA}.OCPR T1 ON T0.""CardCode"" = T1.""CardCode"" ");
			sb.AppendLine($@"WHERE   LOWER(T0.""CardCode"") IN ( {strCodes} ) ");
			sb.AppendLine($@"UNION ");
			sb.AppendLine($@"SELECT  T0.""CardCode"", T0.""CardFName"" ");
			sb.AppendLine($@"FROM    {DBLA}.OCRD T0 ");
			sb.AppendLine($@"WHERE   LOWER(T0.""CardCode"") IN ( {strCodes} ) ");
			sb.AppendLine($@"UNION ");
			sb.AppendLine($@"SELECT  T0.""CardCode"", T1.""Name"" || ' - ' || T1.""Position"" ");
			sb.AppendLine($@"FROM    {DBLA}.OCRD T0 ");
			sb.AppendLine($@"        INNER JOIN {DBLA}.OCPR T1 ON T0.""CardCode"" = T1.""CardCode"" ");
			sb.AppendLine($@"WHERE   LOWER(T0.""CardCode"") IN ( {strCodes} ) ");
			sb.AppendLine($@"UNION ");
			sb.AppendLine($@"SELECT  T0.""CardCode"", T0.""CardFName"" ");
			sb.AppendLine($@"FROM    {DBIQ}.OCRD T0 ");
			sb.AppendLine($@"WHERE   LOWER(T0.""CardCode"") IN ( {strCodes} ) ");
			sb.AppendLine($@"UNION ");
			sb.AppendLine($@"SELECT  T0.""CardCode"", T1.""Name"" || ' - ' || T1.""Position"" ");
			sb.AppendLine($@"FROM    {DBIQ}.OCRD T0 ");
			sb.AppendLine($@"        INNER JOIN {DBIQ}.OCPR T1 ON T0.""CardCode"" = T1.""CardCode"" ");
			sb.AppendLine($@"WHERE   LOWER(T0.""CardCode"") IN ( {strCodes} ) ");

			IEnumerable<BEA.Client> items = SQLList(sb.ToString(), Relations);
			return items;
		}

		public IEnumerable<BEA.Client> ListValidEMails(List<string> CardCodes, params Enum[] Relations)
		{
			string strCodes = string.Join(",", CardCodes.Select(x => $"'{x.ToLower()}'"));
			var sb = new StringBuilder();
			sb.AppendLine($@"SELECT  T0.""CardCode"", T0.""E_Mail"" AS ""EMail"" ");
			sb.AppendLine($@"FROM    {DBSA}.OCRD T0 ");
			sb.AppendLine($@"WHERE   LOWER(T0.""CardCode"") IN ( {strCodes} ) ");
			sb.AppendLine($@"UNION ");
			sb.AppendLine($@"SELECT  T0.""CardCode"", T1.""E_MailL"" ");
			sb.AppendLine($@"FROM    {DBSA}.OCRD T0 ");
			sb.AppendLine($@"        INNER JOIN {DBSA}.OCPR T1 ON T0.""CardCode"" = T1.""CardCode"" ");
			sb.AppendLine($@"WHERE   LOWER(T0.""CardCode"") IN ( {strCodes} ) ");
			sb.AppendLine($@"UNION ");
			sb.AppendLine($@"SELECT  T0.""CardCode"", T0.""E_Mail"" ");
			sb.AppendLine($@"FROM    {DBLA}.OCRD T0 ");
			sb.AppendLine($@"WHERE   LOWER(T0.""CardCode"") IN ( {strCodes} ) ");
			sb.AppendLine($@"UNION ");
			sb.AppendLine($@"SELECT  T0.""CardCode"", T1.""E_MailL"" ");
			sb.AppendLine($@"FROM    {DBLA}.OCRD T0 ");
			sb.AppendLine($@"        INNER JOIN {DBLA}.OCPR T1 ON T0.""CardCode"" = T1.""CardCode"" ");
			sb.AppendLine($@"WHERE   LOWER(T0.""CardCode"") IN ( {strCodes} ) ");
			sb.AppendLine($@"UNION ");
			sb.AppendLine($@"SELECT  T0.""CardCode"", T0.""E_Mail"" ");
			sb.AppendLine($@"FROM    {DBIQ}.OCRD T0 ");
			sb.AppendLine($@"WHERE   LOWER(T0.""CardCode"") IN ( {strCodes} ) ");
			sb.AppendLine($@"UNION ");
			sb.AppendLine($@"SELECT  T0.""CardCode"", T1.""E_MailL"" ");
			sb.AppendLine($@"FROM    {DBIQ}.OCRD T0 ");
			sb.AppendLine($@"        INNER JOIN {DBIQ}.OCPR T1 ON T0.""CardCode"" = T1.""CardCode"" ");
			sb.AppendLine($@"WHERE   LOWER(T0.""CardCode"") IN ( {strCodes} ) ");

			IEnumerable<BEA.Client> items = SQLList(sb.ToString(), Relations);
			return items;
		}

		public IEnumerable<BEA.Client> ListTopClientsPeriod(DateTime Since, DateTime? Until, decimal MinAmount, string Order, params Enum[] Relations)
		{
			var sb = new StringBuilder();
			sb.AppendLine($@"SELECT      a.""CardCode"", a.""CardName"", ""E_Mail"" AS ""EMail"", SUM(""DocTotal"") AS ""Balance"", ""SellerCode"", ""SellerName"" ");
			sb.AppendLine($@"FROM        ( SELECT    t0.""CardCode"", ""CardName"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", t1.""Quantity"" * t1.""PriceAfVAT"" AS ""DocTotal"", IFNULL(t3.""SlpName"", '') AS ""SellerCode"", IFNULL(t3.""Memo"", '') AS ""SellerName"" ");
			sb.AppendLine($@"              FROM      {DBSA}.OINV t0 ");
			sb.AppendLine($@"                        INNER JOIN {DBSA}.INV1 t1 ON t0.""DocEntry"" = t1.""DocEntry"" ");
			sb.AppendLine($@"                        INNER JOIN {DBSA}.OITM t2 ON t1.""ItemCode"" = t2.""ItemCode"" AND  IFNULL(t2.U_LINEA, '')NOT IN ('VERICASH', 'REPTOS', 'DMC') ");
			sb.AppendLine($@"                        LEFT OUTER JOIN {DBSA}.OSLP t3 ON t0.""SlpCode"" = t3.""SlpCode"" ");
			sb.AppendLine($@"              UNION ALL ");
			sb.AppendLine($@"              SELECT    t0.""CardCode"", ""CardName"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", t1.""Quantity"" * t1.""PriceAfVAT"" AS ""DocTotal"", t3.""SlpName"", t3.""Memo"" ");
			sb.AppendLine($@"              FROM      {DBIQ}.OINV t0 ");
			sb.AppendLine($@"                        INNER JOIN {DBIQ}.INV1 t1 ON t0.""DocEntry"" = t1.""DocEntry"" ");
			sb.AppendLine($@"                        INNER JOIN {DBIQ}.OITM t2 ON t1.""ItemCode"" = t2.""ItemCode"" AND IFNULL(t2.U_LINEA, '')NOT IN ('VERICASH', 'REPTOS', 'DMC') ");
			sb.AppendLine($@"                        LEFT OUTER JOIN {DBIQ}.OSLP t3 ON t0.""SlpCode"" = t3.""SlpCode"" ");
			sb.AppendLine($@"              UNION ALL ");
			sb.AppendLine($@"              SELECT    t0.""CardCode"", ""CardName"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", t1.""Quantity"" * t1.""PriceAfVAT"" AS ""DocTotal"", t3.""SlpName"", t3.""Memo"" ");
			sb.AppendLine($@"              FROM      {DBLA}.OINV t0 ");
			sb.AppendLine($@"                        INNER JOIN {DBLA}.INV1 t1 ON t0.""DocEntry"" = t1.""DocEntry"" ");
			sb.AppendLine($@"                        INNER JOIN {DBLA}.OITM t2 ON t1.""ItemCode"" = t2.""ItemCode"" AND IFNULL(t2.U_LINEA, '')NOT IN ('VERICASH', 'REPTOS', 'DMC') ");
			sb.AppendLine($@"                        LEFT OUTER JOIN {DBLA}.OSLP t3 ON t0.""SlpCode"" = t3.""SlpCode"" ) AS a ");
			sb.AppendLine($@"            INNER JOIN {DBSA}.OCRD b ON a.""CardCode"" = b.""CardCode"" ");
			sb.AppendLine($@"WHERE       a.""CardCode"" NOT IN ('CDMC-002', 'CDMC-003', 'CIMP-008', 'CVAR-001') AND a.""CardCode"" NOT LIKE 'E%' ");
			sb.AppendLine($@"		    AND a.""DocDate"" >= '{Since:yyyy-MM-dd}' ");
			if (Until.HasValue)
			{
				sb.AppendLine($@"		AND a.""DocDate"" <= '{Until.Value:yyyy-MM-dd}' ");
			}
			sb.AppendLine($@"GROUP BY    a.""CardCode"", a.""CardName"", ""E_Mail"", a.""SellerCode"", a.""SellerName"" ");
			sb.AppendLine($@"HAVING SUM(a.""DocTotal"") > {MinAmount:#.00} ");
			sb.AppendLine($@"ORDER BY {GetOrder(Order)}");

			IEnumerable<BEA.Client> items = SQLList(sb.ToString(), Relations);
			return items;
		}

		public IEnumerable<BEA.Client> ListDell(int Year, int Month, params Enum[] Relations)
		{
			var sb = new StringBuilder();
			sb.AppendLine($@"SELECT      DISTINCT a.""CardCode"", a.""CardName"", ""E_Mail"" AS ""MailAddres"" ");
			sb.AppendLine($@"FROM        ( SELECT    CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", t0.""CardCode"", ""CardName"", t1.""ItemCode"", U_LINEA AS ""Line"" ");
			sb.AppendLine($@"              FROM      {DBSA}.OINV t0 ");
			sb.AppendLine($@"                        INNER JOIN {DBSA}.INV1 t1 ON t1.""DocEntry"" = t0.""DocEntry"" ");
			sb.AppendLine($@"                        INNER JOIN {DBSA}.OITM t2 ON t1.""ItemCode"" = t2.""ItemCode"" ");
			sb.AppendLine($@"              WHERE     ( t0.""CardCode"" <> 'CDMC-002' AND  t0.""CardCode"" <> 'CDMC-003' AND t0.""CardCode"" <> 'CIMP-008' ) AND t1.""TargetType"" <> 14 AND   t0.U_CCFACANU = 'N' ");
			sb.AppendLine($@"              UNION ALL ");
			sb.AppendLine($@"              SELECT    CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", t0.""CardCode"", ""CardName"", t1.""ItemCode"", U_LINEA ");
			sb.AppendLine($@"              FROM      {DBLA}.OINV t0 ");
			sb.AppendLine($@"                        INNER JOIN {DBLA}.INV1 t1 ON t1.""DocEntry"" = t0.""DocEntry"" ");
			sb.AppendLine($@"                        INNER JOIN {DBLA}.OITM t2 ON t1.""ItemCode"" = t2.""ItemCode"" ");
			sb.AppendLine($@"              WHERE     ( t0.""CardCode"" <> 'CDMC-002' AND  t0.""CardCode"" <> 'CDMC-003' AND t0.""CardCode"" <> 'CIMP-008' ) AND t1.""TargetType"" <> 14 ");
			sb.AppendLine($@"              UNION ALL ");
			sb.AppendLine($@"              SELECT    CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", t0.""CardCode"", ""CardName"", t1.""ItemCode"", U_LINEA ");
			sb.AppendLine($@"              FROM      {DBIQ}.OINV t0 ");
			sb.AppendLine($@"                        INNER JOIN {DBIQ}.INV1 t1 ON t1.""DocEntry"" = t0.""DocEntry"" ");
			sb.AppendLine($@"                        INNER JOIN {DBIQ}.OITM t2 ON t1.""ItemCode"" = t2.""ItemCode"" ");
			sb.AppendLine($@"              WHERE     ( t0.""CardCode"" <> 'CDMC-002' AND  t0.""CardCode"" <> 'CDMC-003' AND t0.""CardCode"" <> 'CIMP-008' ) AND ( t1.""TargetType"" <> 14 )) AS a ");
			sb.AppendLine($@"            INNER JOIN {DBSA}.OCRD b ON a.""CardCode"" = b.""CardCode"" ");
			sb.AppendLine($@"WHERE       LOWER(""Line"") LIKE '%dell%' AND YEAR(a.""DocDate"") = {Year} AND MONTH(a.""DocDate"") = {Month} AND IFNULL(""E_Mail"", '') <> '' ");
			sb.AppendLine($@"ORDER BY    2 ");

			IEnumerable<BEA.Client> items = SQLList(sb.ToString(), Relations);
			return items;
		}

		public IEnumerable<BEA.Client> ListShort(List<Field> FilterList, string Order)
		{
			string strFilter = FilterList?.Count > 0 ? base.GetFilter(FilterList.ToArray()) : "";

			var sb = new StringBuilder();
			sb.AppendLine($@"SELECT      * ");
			sb.AppendLine($@"FROM        (   SELECT  ""CardCode"", ""CardName"", ""E_Mail"" AS ""EMail"", t1.""SlpName"" AS ""SellerCode"", t1.""Memo"" AS ""Seller"", t0.""SlpCode"" ");
			sb.AppendLine($@"                FROM    {DBSA}.OCRD t0 ");
			sb.AppendLine($@"                        LEFT OUTER JOIN {DBSA}.OSLP t1 ON t0.""SlpCode"" = t1.""SlpCode"" ");
			sb.AppendLine($@"                WHERE   ""CardType"" = 'C' ");
			sb.AppendLine($@"                UNION ");
			sb.AppendLine($@"                SELECT  ""CardCode"", ""CardName"", ""E_Mail"", t1.""SlpName"" AS ""SellerCode"", t1.""Memo"" AS ""Seller"", t0.""SlpCode"" ");
			sb.AppendLine($@"                FROM    {DBIQ}.OCRD t0 ");
			sb.AppendLine($@"                        LEFT OUTER JOIN {DBIQ}.OSLP t1 ON t0.""SlpCode"" = t1.""SlpCode"" ");
			sb.AppendLine($@"                WHERE   ""CardType"" = 'C' ");
			sb.AppendLine($@"                        AND NOT EXISTS ( SELECT * FROM {DBSA}.OCRD WHERE ""CardCode"" = t0.""CardCode"" ) ");
			sb.AppendLine($@"                UNION ");
			sb.AppendLine($@"                SELECT  ""CardCode"", ""CardName"", ""E_Mail"", t1.""SlpName"" AS ""SellerCode"", t1.""Memo"" AS ""Seller"", t0.""SlpCode"" ");
			sb.AppendLine($@"                FROM    {DBLA}.OCRD t0 ");
			sb.AppendLine($@"                        LEFT OUTER JOIN {DBLA}.OSLP t1 ON t0.""SlpCode"" = t1.""SlpCode"" ");
			sb.AppendLine($@"                WHERE   ""CardType"" = 'C' ");
			sb.AppendLine($@"                        AND NOT EXISTS ( SELECT * FROM {DBSA}.OCRD WHERE ""CardCode"" = t0.""CardCode"" ) ");
			sb.AppendLine($@"                        AND NOT EXISTS ( SELECT * FROM {DBIQ}.OCRD WHERE ""CardCode"" = t0.""CardCode"" ) ) a ");
			if (!string.IsNullOrEmpty(strFilter)) sb.AppendLine($@"WHERE     {strFilter} ");
			sb.AppendLine($@"ORDER BY {GetOrder(Order)} ");

			IEnumerable<BEA.Client> Collection = base.SQLList(sb.ToString());
			return Collection;
		}

		public async Task<IEnumerable<BEA.Client>> ListShortAsync(List<Field> FilterList, string Order)
		{
			string strFilter = FilterList?.Count > 0 ? base.GetFilter(FilterList.ToArray()) : "";

			var sb = new StringBuilder();
			sb.AppendLine($@"SELECT      * ");
			sb.AppendLine($@"FROM        (   SELECT  ""CardCode"", ""CardName"", ""E_Mail"" AS ""EMail"", t1.""SlpName"" AS ""SellerCode"", t1.""Memo"" AS ""Seller"", t0.""SlpCode"" ");
			sb.AppendLine($@"                FROM    {DBSA}.OCRD t0 ");
			sb.AppendLine($@"                        LEFT OUTER JOIN {DBSA}.OSLP t1 ON t0.""SlpCode"" = t1.""SlpCode"" ");
			sb.AppendLine($@"                WHERE   ""CardType"" = 'C' ");
			sb.AppendLine($@"                UNION ");
			sb.AppendLine($@"                SELECT  ""CardCode"", ""CardName"", ""E_Mail"", t1.""SlpName"" AS ""SellerCode"", t1.""Memo"" AS ""Seller"", t0.""SlpCode"" ");
			sb.AppendLine($@"                FROM    {DBIQ}.OCRD t0 ");
			sb.AppendLine($@"                        LEFT OUTER JOIN {DBIQ}.OSLP t1 ON t0.""SlpCode"" = t1.""SlpCode"" ");
			sb.AppendLine($@"                WHERE   ""CardType"" = 'C' ");
			sb.AppendLine($@"                        AND NOT EXISTS ( SELECT * FROM {DBSA}.OCRD WHERE ""CardCode"" = t0.""CardCode"" ) ");
			sb.AppendLine($@"                UNION ");
			sb.AppendLine($@"                SELECT  ""CardCode"", ""CardName"", ""E_Mail"", t1.""SlpName"" AS ""SellerCode"", t1.""Memo"" AS ""Seller"", t0.""SlpCode"" ");
			sb.AppendLine($@"                FROM    {DBLA}.OCRD t0 ");
			sb.AppendLine($@"                        LEFT OUTER JOIN {DBLA}.OSLP t1 ON t0.""SlpCode"" = t1.""SlpCode"" ");
			sb.AppendLine($@"                WHERE   ""CardType"" = 'C' ");
			sb.AppendLine($@"                        AND NOT EXISTS ( SELECT * FROM {DBSA}.OCRD WHERE ""CardCode"" = t0.""CardCode"" ) ");
			sb.AppendLine($@"                        AND NOT EXISTS ( SELECT * FROM {DBIQ}.OCRD WHERE ""CardCode"" = t0.""CardCode"" ) ) a ");
			if (!string.IsNullOrEmpty(strFilter)) sb.AppendLine($@"WHERE     {strFilter} ");
			sb.AppendLine($@"ORDER BY {GetOrder(Order)} ");

			IEnumerable<BEA.Client> Collection = await SQLListAsync(sb.ToString());
			return Collection;
		}

		public IEnumerable<BEA.Client> ListShort2(List<Field> FilterList, string Order)
		{
			string strFilter = "";
			if (FilterList != null && FilterList.Count > 0)
			{
				strFilter = base.GetFilter(FilterList.ToArray());
			}

			var sb = new StringBuilder();
			sb.AppendLine($@"SELECT DISTINCT * ");
			sb.AppendLine($@"FROM (SELECT ""CardCode"", ""CardName"", ""E_Mail"" AS ""EMail"", ""CardFName"", t1.""SlpName"" AS ""SellerCode"", t1.""Memo"" AS ""SellerName"", t0.""frozenFor"" AS ""Disabled"" ");
			sb.AppendLine($@"     FROM {DBSA}.OCRD t0 ");
			sb.AppendLine($@"          LEFT OUTER JOIN {DBSA}.OSLP t1 ON t0.""SlpCode"" = t1.""SlpCode"" ");
			sb.AppendLine($@"     WHERE ""CardType"" = 'C' AND  t0.""GroupCode"" NOT IN (106, 112) ");
			sb.AppendLine($@"     UNION ");
			sb.AppendLine($@"     SELECT  ""CardCode"", ""CardName"", ""E_Mail"", ""CardFName"", t1.""SlpName"" AS ""SellerCode"", t1.""Memo"" AS ""Seller"", t0.""frozenFor"" AS ""Disabled"" ");
			sb.AppendLine($@"     FROM {DBIQ}.OCRD t0 ");
			sb.AppendLine($@"          LEFT OUTER JOIN {DBIQ}.OSLP t1 ON t0.""SlpCode"" = t1.""SlpCode"" ");
			sb.AppendLine($@"     WHERE ""CardType"" = 'C' AND  t0.""GroupCode"" NOT IN (106, 112) ");
			sb.AppendLine($@"     UNION ");
			sb.AppendLine($@"     SELECT  ""CardCode"", ""CardName"", ""E_Mail"", ""CardFName"", t1.""SlpName"" AS ""SellerCode"", t1.""Memo"" AS ""Seller"", t0.""frozenFor"" AS ""Disabled"" ");
			sb.AppendLine($@"     FROM {DBLA}.OCRD t0 ");
			sb.AppendLine($@"          LEFT OUTER JOIN {DBLA}.OSLP t1 ON t0.""SlpCode"" = t1.""SlpCode"" ");
			sb.AppendLine($@"     WHERE ""CardType"" = 'C' AND t0.""GroupCode"" NOT IN (106, 112) ) a ");
			if (strFilter != "") sb.AppendLine($@"WHERE     {strFilter} ");
			sb.AppendLine($@"ORDER BY {GetOrder(Order)} ");

			IEnumerable<BEA.Client> Collection = base.SQLList(sb.ToString());
			return Collection;
		}

		public IEnumerable<BEA.Client> ListShort3(List<Field> FilterList, string Order)
		{
			string strFilter = FilterList?.Count > 0 ? base.GetFilter(FilterList.ToArray()) : "";

			var sb = new StringBuilder();
			sb.AppendLine($@"SELECT      * ");
			sb.AppendLine($@"FROM        (   SELECT  ""CardCode"", ""CardName"", ""E_Mail"", t1.""SlpName"" AS ""SellerCode"", t1.""Memo"" AS ""Seller"", t0.""SlpCode"", t0.""LicTradNum"" AS NIT ");
			sb.AppendLine($@"                FROM    {DBSA}.OCRD t0 ");
			sb.AppendLine($@"                        LEFT OUTER JOIN {DBSA}.OSLP t1 ON t0.""SlpCode"" = t1.""SlpCode"" ");
			sb.AppendLine($@"                WHERE   ""CardType"" = 'C' ");
			sb.AppendLine($@"                UNION ");
			sb.AppendLine($@"                SELECT  ""CardCode"", ""CardName"", ""E_Mail"", t1.""SlpName"" AS ""SellerCode"", t1.""Memo"" AS ""Seller"", t0.""SlpCode"", t0.""LicTradNum"" ");
			sb.AppendLine($@"                FROM    {DBIQ}.OCRD t0 ");
			sb.AppendLine($@"                        LEFT OUTER JOIN {DBIQ}.OSLP t1 ON t0.""SlpCode"" = t1.""SlpCode"" ");
			sb.AppendLine($@"                WHERE   ""CardType"" = 'C' ");
			sb.AppendLine($@"                        AND NOT EXISTS ( SELECT * FROM {DBSA}.OCRD WHERE ""CardCode"" = t0.""CardCode"" ) ");
			sb.AppendLine($@"                UNION ");
			sb.AppendLine($@"                SELECT  ""CardCode"", ""CardName"", ""E_Mail"", t1.""SlpName"" AS ""SellerCode"", t1.""Memo"" AS ""Seller"", t0.""SlpCode"", t0.""LicTradNum"" ");
			sb.AppendLine($@"                FROM    {DBLA}.OCRD t0 ");
			sb.AppendLine($@"                        LEFT OUTER JOIN {DBLA}.OSLP t1 ON t0.""SlpCode"" = t1.""SlpCode"" ");
			sb.AppendLine($@"                WHERE   ""CardType"" = 'C' ");
			sb.AppendLine($@"                        AND NOT EXISTS ( SELECT * FROM {DBSA}.OCRD WHERE ""CardCode"" = t0.""CardCode"" ) ");
			sb.AppendLine($@"                        AND NOT EXISTS ( SELECT * FROM {DBIQ}.OCRD WHERE ""CardCode"" = t0.""CardCode"" ) ) a ");
			if (!string.IsNullOrEmpty(strFilter)) sb.AppendLine($@"WHERE     {strFilter} ");
			sb.AppendLine($@"ORDER BY {GetOrder(Order)} ");

			IEnumerable<BEA.Client> Collection = base.SQLList(sb.ToString());
			return Collection;
		}

		public IEnumerable<BEA.ClientExtra> ListExtras(string CardCode)
		{
			var sb = new StringBuilder();
			sb.AppendLine($@"SELECT  DISTINCT * ");
			sb.AppendLine($@"FROM    (   SELECT  'Santa Cruz' AS ""Subsidiary"", ""CardCode"", t0.U_ACRED1 AS ""Acredited1"", t0.U_ACRED2 AS ""Acredited2"" ");
			sb.AppendLine($@"                    , t0.U_ACRED3 AS ""Acredited3"", t0.U_ACRED4 AS ""Acredited4"", t0.U_ACRED5 AS ""Acredited5"", t0.U_ACRED6 AS ""Acredited6"", t0.U_ACRED7 AS ""Acredited7"", t0.U_ACRED8 AS ""Acredited8""  ");
			sb.AppendLine($@"                    , t0.U_CIACRED1 AS ""CIAcredited1"", t0.U_CIACRED2 AS ""CIAcredited2"", t0.U_CIACRED3 AS ""CIAcredited3"", t0.U_CIACRED4 AS ""CIAcredited4"", t0.U_CIACRED5 AS ""CIAcredited5"" ");
			sb.AppendLine($@"                    , t0.U_CIACRED6 AS ""CIAcredited6"", t0.U_CIACRED7 AS ""CIAcredited7"", t0.U_CIACRED8 AS ""ACIcredited8""  ");
			sb.AppendLine($@"            FROM    {DBSA}.OCRD t0 ");
			sb.AppendLine($@"                    LEFT OUTER JOIN {DBSA}.OSLP t1 ON t0.""SlpCode"" = t1.""SlpCode"" ");
			sb.AppendLine($@"                    LEFT OUTER JOIN {DBSA}.OCTG t2 ON t0.""GroupNum"" = t2.""GroupNum"" ");
			sb.AppendLine($@"            WHERE   ""CardType"" = 'C' ");
			sb.AppendLine($@"            UNION ");
			sb.AppendLine($@"            SELECT  'Iquique' AS ""Subsidiary"", ""CardCode"", t0.U_ACRED1 AS ""Acredited1"", t0.U_ACRED2 AS ""Acredited2"" ");
			sb.AppendLine($@"                    , t0.U_ACRED3 AS ""Acredited3"", t0.U_ACRED4 AS ""Acredited4"", t0.U_ACRED5 AS ""Acredited5"", t0.U_ACRED6 AS ""Acredited6"", t0.U_ACRED7 AS ""Acredited7"", t0.U_ACRED8 AS ""Acredited8""  ");
			sb.AppendLine($@"                    , t0.U_CIACRED1 AS ""CIAcredited1"", t0.U_CIACRED2 AS ""CIAcredited2"", t0.U_CIACRED3 AS ""CIAcredited3"", t0.U_CIACRED4 AS ""CIAcredited4"", t0.U_CIACRED5 AS ""CIAcredited5"" ");
			sb.AppendLine($@"                    , t0.U_CIACRED6 AS ""CIAcredited6"", t0.U_CIACRED7 AS ""CIAcredited7"", t0.U_CIACRED8 AS ""ACIcredited8""  ");
			sb.AppendLine($@"            FROM    {DBIQ}.OCRD t0 ");
			sb.AppendLine($@"                    LEFT OUTER JOIN {DBIQ}.OSLP t1 ON t0.""SlpCode"" = t1.""SlpCode"" ");
			sb.AppendLine($@"                    LEFT OUTER JOIN {DBIQ}.OCTG t2 ON t0.""GroupNum"" = t2.""GroupNum"" ");
			sb.AppendLine($@"            WHERE   ""CardType"" = 'C' ");
			sb.AppendLine($@"            UNION ");
			sb.AppendLine($@"            SELECT  'Miami' AS ""Subsidiary"", ""CardCode"", t0.U_ACRED1 AS ""Acredited1"", t0.U_ACRED2 AS ""Acredited2"" ");
			sb.AppendLine($@"                    , t0.U_ACRED3 AS ""Acredited3"", t0.U_ACRED4 AS ""Acredited4"", t0.U_ACRED5 AS ""Acredited5"", t0.U_ACRED6 AS ""Acredited6"", t0.U_ACRED7 AS ""Acredited7"", t0.U_ACRED8 AS ""Acredited8""  ");
			sb.AppendLine($@"                    , t0.U_CIACRED1 AS ""CIAcredited1"", t0.U_CIACRED2 AS ""CIAcredited2"", t0.U_CIACRED3 AS ""CIAcredited3"", t0.U_CIACRED4 AS ""CIAcredited4"", t0.U_CIACRED5 AS ""CIAcredited5"" ");
			sb.AppendLine($@"                    , t0.U_CIACRED6 AS ""CIAcredited6"", t0.U_CIACRED7 AS ""CIAcredited7"", t0.U_CIACRED8 AS ""ACIcredited8""  ");
			sb.AppendLine($@"            FROM    {DBLA}.OCRD t0 ");
			sb.AppendLine($@"                    LEFT OUTER JOIN {DBLA}.OSLP t1 ON t0.""SlpCode"" = t1.""SlpCode"" ");
			sb.AppendLine($@"                    LEFT OUTER JOIN {DBLA}.OCTG t2 ON t0.""GroupNum"" = t2.""GroupNum"" ");
			sb.AppendLine($@"            WHERE   ""CardType"" = 'C' ) a ");
			sb.AppendLine($@"WHERE   ""CardCode"" = '{CardCode}' ");

			IEnumerable<BEA.ClientExtra> items = SQLList<BEA.ClientExtra>(sb.ToString());
			return items;
		}

		public IEnumerable<BEA.ClientResumeDebt> ListDebts(DateTime ToDate, string ClientCodes)
		{
			var query = $@"SELECT *
						FROM ( SELECT T1.""ShortName"" AS ""Code"", T2.""CardName"" AS ""Name"", SUM(T1.""Debit"") AS ""Debit"", SUM(T1.""Credit"") AS ""Credit"", SUM(T1.""Debit"") - SUM(T1.""Credit"") AS ""Difference""              
							   FROM {DBSA}.OJDT T0
									INNER JOIN {DBSA}.JDT1 T1 ON T0.""TransId"" = T1.""TransId""
									INNER JOIN {DBSA}.OCRD T2 ON T2.""CardCode"" = T1.""ShortName""
							   WHERE CAST(T0.""TaxDate"" AS DATE) <= '{ToDate:yyyy-MM-dd}' AND CAST(T0.""DueDate"" AS DATE) <= '{ToDate:yyyy-MM-dd}'
							   GROUP BY T1.""ShortName"", T2.""CardName""
							   UNION ALL
							   SELECT T1.""ShortName"" AS ""Code"", T2.""CardName"" AS ""Name"", SUM(T1.""Debit"") AS ""Debit"", SUM(T1.""Credit"") AS ""Credit"", SUM(T1.""Debit"") - SUM(T1.""Credit"") AS ""Difference""              
							   FROM {DBIQ}.OJDT T0
									INNER JOIN {DBIQ}.JDT1 T1 ON T0.""TransId"" = T1.""TransId""
									INNER JOIN {DBIQ}.OCRD T2 ON T2.""CardCode"" = T1.""ShortName""
							   WHERE CAST(T0.""TaxDate"" AS DATE) <= '{ToDate:yyyy-MM-dd}' AND CAST(T0.""DueDate"" AS DATE) <= '{ToDate:yyyy-MM-dd}'
							   GROUP BY T1.""ShortName"", T2.""CardName""
							   UNION ALL
							   SELECT T1.""ShortName"" AS ""Code"", T2.""CardName"" AS ""Name"", SUM(T1.""Debit"") AS ""Debit"", SUM(T1.""Credit"") AS ""Credit"", SUM(T1.""Debit"") - SUM(T1.""Credit"") AS ""Difference""              
							   FROM {DBLA}.OJDT T0
									INNER JOIN {DBLA}.JDT1 T1 ON T0.""TransId"" = T1.""TransId""
									INNER JOIN {DBLA}.OCRD T2 ON T2.""CardCode"" = T1.""ShortName""
							   WHERE CAST(T0.""TaxDate"" AS DATE) <= '{ToDate:yyyy-MM-dd}' AND CAST(T0.""DueDate"" AS DATE) <= '{ToDate:yyyy-MM-dd}'
							   GROUP BY T1.""ShortName"", T2.""CardName"" ) a
						WHERE ""Difference"" > 0
							  AND ""Code"" IN ( {ClientCodes} ) ";

			IEnumerable<BEA.ClientResumeDebt> items = SQLList<BEA.ClientResumeDebt>(query);
			return items;
		}

		public IEnumerable<BEA.ClientStatics> ListYears(string CardCode)
		{
			//string query = $@"SELECT  ""CardCode"", ""Year"", SUM(""Total"") AS ""Total"", SUM(""Quantity"") AS ""Quantity"", AVG(""Total"") AS ""Average""
			//                  FROM    ( SELECT  ""CardCode"", YEAR(""DocDate"") AS ""Year"", MONTH(""DocDate"") AS ""Month"", SUM(""DocTotal"") AS ""Total"", COUNT(""DocEntry"") AS ""Quantity""
			//                            FROM    ( SELECT  ""DocEntry"", ""CardCode"", CAST(""DocDate"" AS DATE) AS ""DocDate"", ""DocTotal""
			//                                      FROM    {DBSA}.OINV
			//                                      WHERE   CANCELED = 'N'
			//                                      UNION ALL
			//                                      SELECT  ""DocEntry"", ""CardCode"", CAST(""DocDate"" AS DATE) AS ""DocDate"", ""DocTotal""
			//                                      FROM    {DBIQ}.OINV
			//                                      WHERE   CANCELED = 'N'
			//                                      UNION ALL 
			//                                      SELECT  ""DocEntry"", ""CardCode"", CAST(""DocDate"" AS DATE) AS ""DocDate"", ""DocTotal""
			//                                      FROM    {DBLA}.OINV
			//                                      WHERE   CANCELED = 'N' ) a 
			//                            GROUP BY ""CardCode"", YEAR(""DocDate""), MONTH(""DocDate"") ) b
			//                  WHERE   ""CardCode"" = '{CardCode}'          
			//                  GROUP BY ""CardCode"", ""Year"" 
			//                  ORDER BY 2 DESC ";
			string query = $@"SELECT  ""CardCode"", ""Year"", SUM(""Total"") AS ""Total"", SUM(""Quantity"") AS ""Quantity"", AVG(""Total"") AS ""Average"", SUM(""Margin"") AS ""Margin"", SUM(""TaxlessTotal"") AS ""TaxlessTotal""
							  FROM    ( SELECT  ""CardCode"", YEAR(""DocDate"") AS ""Year"", MONTH(""DocDate"") AS ""Month"", SUM(""DocTotal"") AS ""Total"", COUNT(""DocEntry"") AS ""Quantity"", SUM(""Margin"") AS ""Margin"", SUM(""TaxlessTotal"") AS ""TaxlessTotal""
										  FROM    ( SELECT  t0.""DocEntry"", ""CardCode"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", ""DocTotal""
														  , SUM(( CASE WHEN t1.""WhsCode"" = 'ZFSCZ' THEN t1.""Quantity"" * ( t1.""PriceAfVAT"" - t1.""StockPrice"" ) ELSE t1.""Quantity"" * (( 0.84 * t1.""PriceAfVAT"" ) - t1.""StockPrice"" ) END )) AS ""Margin""
														  , SUM(( CASE WHEN t1.""WhsCode"" = 'ZFSCZ' THEN t1.""Quantity"" * t1.""PriceAfVAT"" ELSE t1.""Quantity"" * ( 0.84 * t1.""PriceAfVAT"" ) END )) AS ""TaxlessTotal""
												  FROM    {DBSA}.OINV t0
														  INNER JOIN {DBSA}.INV1 t1 ON t0.""DocEntry"" = t1.""DocEntry""
												  WHERE   t0.CANCELED = 'N'
												  GROUP BY t0.""DocEntry"", ""CardCode"", t0.""DocDate"", ""DocTotal""
												  UNION ALL
												  SELECT  t0.""DocEntry"", ""CardCode"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", ""DocTotal""
														  , SUM( t1.""Quantity"" * ( t1.""PriceAfVAT"" - t1.""StockPrice"" ) ) AS ""Margin""
														  , SUM( t1.""Quantity"" * t1.""PriceAfVAT"" ) AS ""TaxlessTotal""
												  FROM    {DBIQ}.OINV t0
														  INNER JOIN {DBIQ}.INV1 t1 ON t0.""DocEntry"" = t1.""DocEntry""
												  WHERE   t0.CANCELED = 'N'
												  GROUP BY t0.""DocEntry"", ""CardCode"", t0.""DocDate"", ""DocTotal""
												  UNION ALL 
												  SELECT  t0.""DocEntry"", ""CardCode"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", ""DocTotal""
														  , SUM( t1.""Quantity"" * ( t1.""PriceAfVAT"" - t1.""StockPrice"" ) ) AS ""Margin""
														  , SUM( t1.""Quantity"" * t1.""PriceAfVAT"" ) AS ""TaxlessTotal""
												  FROM    {DBLA}.OINV t0
														  INNER JOIN {DBLA}.INV1 t1 ON t0.""DocEntry"" = t1.""DocEntry""
												  WHERE   t0.CANCELED = 'N' 
												  GROUP BY t0.""DocEntry"", ""CardCode"", t0.""DocDate"", ""DocTotal"" ) a 
										  GROUP BY ""CardCode"", YEAR(""DocDate""), MONTH(""DocDate"") ) b
							  WHERE   ""CardCode"" = '{CardCode}'          
							  GROUP BY ""CardCode"", ""Year"" 
							  ORDER BY 2 DESC ";
			IEnumerable<BEA.ClientStatics> items = SQLList<BEA.ClientStatics>(query);
			return items;
		}

		public IEnumerable<BEA.ClientStatics> ListMonths(string CardCode, int Year)
		{
			string query = $@"SELECT  ""CardCode"", YEAR(""DocDate"") AS ""Year"", MONTH(""DocDate"") AS ""Month"", SUM(""DocTotal"") AS ""Total"", COUNT(""DocEntry"") AS ""Quantity"", SUM(""Margin"") AS ""Margin"", SUM(""TaxlessTotal"") AS ""TaxlessTotal""
							  FROM    ( SELECT  t0.""DocEntry"", ""CardCode"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", ""DocTotal""
												  , SUM(( CASE WHEN t1.""WhsCode"" = 'ZFSCZ' THEN t1.""Quantity"" * ( t1.""PriceAfVAT"" - t1.""StockPrice"" ) ELSE t1.""Quantity"" * (( 0.84 * t1.""PriceAfVAT"" ) - t1.""StockPrice"" ) END )) AS ""Margin""
												  , SUM(( CASE WHEN t1.""WhsCode"" = 'ZFSCZ' THEN t1.""Quantity"" * t1.""PriceAfVAT"" ELSE t1.""Quantity"" * ( 0.84 * t1.""PriceAfVAT"" ) END )) AS ""TaxlessTotal""
										  FROM    {DBSA}.OINV t0
												  INNER JOIN {DBSA}.INV1 t1 ON t0.""DocEntry"" = t1.""DocEntry""
										  WHERE   t0.CANCELED = 'N'
										  GROUP BY t0.""DocEntry"", ""CardCode"", t0.""DocDate"", ""DocTotal""
										  UNION ALL
										  SELECT  t0.""DocEntry"", ""CardCode"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", ""DocTotal""
												  , SUM( t1.""Quantity"" * ( t1.""PriceAfVAT"" - t1.""StockPrice"" ) ) AS ""Margin""
												  , SUM( t1.""Quantity"" * t1.""PriceAfVAT"" ) AS ""TaxlessTotal""
										  FROM    {DBIQ}.OINV t0
												  INNER JOIN {DBIQ}.INV1 t1 ON t0.""DocEntry"" = t1.""DocEntry""
										  WHERE   t0.CANCELED = 'N'
										  GROUP BY t0.""DocEntry"", ""CardCode"", t0.""DocDate"", ""DocTotal""
										  UNION ALL 
										  SELECT  t0.""DocEntry"", ""CardCode"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", ""DocTotal""
												  , SUM( t1.""Quantity"" * ( t1.""PriceAfVAT"" - t1.""StockPrice"" ) ) AS ""Margin""
												  , SUM( t1.""Quantity"" * t1.""PriceAfVAT"" ) AS ""TaxlessTotal""
										  FROM    {DBLA}.OINV t0
												  INNER JOIN {DBLA}.INV1 t1 ON t0.""DocEntry"" = t1.""DocEntry""
										  WHERE   CANCELED = 'N'
										  GROUP BY t0.""DocEntry"", ""CardCode"", t0.""DocDate"", ""DocTotal"" ) a
							  WHERE   ""CardCode"" = '{CardCode}' AND YEAR(""DocDate"") = {Year}
							  GROUP BY ""CardCode"", YEAR(""DocDate""), MONTH(""DocDate"")
							  ORDER BY 3 DESC";
			IEnumerable<BEA.ClientStatics> items = SQLList<BEA.ClientStatics>(query);
			return items;
		}

		public IEnumerable<BEA.Client> ListResumeAmounts(string CardCode, DateTime Since, params Enum[] Relations)
		{
			string query = $@"SELECT  ""Subsidiary"", ""CardCode"", SUM(""DocTotal"") AS ""Balance""
							  FROM    ( SELECT    'Santa Cruz' AS ""Subsidiary"", ""CardCode"", CAST(""DocDate"" AS DATE) AS ""DocDate"", ""DocTotal""
										FROM      {DBSA}.OINV
										UNION
										SELECT    'Iquique' AS ""Subsidiary"", ""CardCode"", CAST(""DocDate"" AS DATE) AS ""DocDate"", ""DocTotal""
										FROM      {DBIQ}.OINV
										UNION 
										SELECT    'Miami' AS ""Subsidiary"", ""CardCode"", CAST(""DocDate"" AS DATE) AS ""DocDate"", ""DocTotal""
										FROM      {DBLA}.OINV ) a  
							  WHERE   ""CardCode"" = '{CardCode}' AND ""DocDate"" >= '{Since:yyyy-MM-dd}'
							  GROUP BY ""Subsidiary"", ""CardCode"" ";
			IEnumerable<BEA.Client> items = SQLList(query, Relations);
			return items;
		}

		#endregion

		#region Search Methods

		public BEA.Client Search(string CardCode, params Enum[] Relations)
		{
			StringBuilder sb = new();
			sb.AppendLine($@"SELECT   * ");
			sb.AppendLine($@"FROM     ({GetQuery()}) a ");
			sb.AppendLine($@"WHERE    LOWER(""CardCode"") = '{CardCode.ToLower()}' ");

			BEA.Client item = SQLSearch(sb.ToString(), Relations);
			return item;
		}

		public BEA.ClientStatics SearchStaticts(string CardCode)
		{
			string query = $@"SELECT  ""CardCode"", FIRST_VALUE(""DocDate"" ORDER BY ""DocDate"") AS ""First"", LAST_VALUE(""DocDate"" ORDER BY ""DocDate"") AS ""Last"", SUM(""DocTotal"") AS ""Total"", AVG(""DocTotal"") AS ""Average"", COUNT(*) AS ""Quantity"", SUM(""Margin"") AS ""Margin"", SUM(""TaxlessTotal"") AS ""TaxlessTotal""
							  FROM    ( SELECT  t0.""DocEntry"", ""CardCode"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", ""DocTotal""
												  , SUM(( CASE WHEN t1.""WhsCode"" = 'ZFSCZ' THEN t1.""Quantity"" * ( t1.""PriceAfVAT"" - t1.""StockPrice"" ) ELSE t1.""Quantity"" * (( 0.84 * t1.""PriceAfVAT"" ) - t1.""StockPrice"" ) END )) AS ""Margin""
												  , SUM(( CASE WHEN t1.""WhsCode"" = 'ZFSCZ' THEN t1.""Quantity"" * t1.""PriceAfVAT"" ELSE t1.""Quantity"" * ( 0.84 * t1.""PriceAfVAT"" ) END )) AS ""TaxlessTotal""
										  FROM    {DBSA}.OINV t0
												  INNER JOIN {DBSA}.INV1 t1 ON t0.""DocEntry"" = t1.""DocEntry""
										  WHERE   t0.CANCELED = 'N'
										  GROUP BY t0.""DocEntry"", ""CardCode"", t0.""DocDate"", ""DocTotal""
										  UNION ALL
										  SELECT  t0.""DocEntry"", ""CardCode"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", ""DocTotal""
												  , SUM( t1.""Quantity"" * ( t1.""PriceAfVAT"" - t1.""StockPrice"" ) ) AS ""Margin""
												  , SUM( t1.""Quantity"" * t1.""PriceAfVAT"" ) AS ""TaxlessTotal""
										  FROM    {DBIQ}.OINV t0
												  INNER JOIN {DBIQ}.INV1 t1 ON t0.""DocEntry"" = t1.""DocEntry""
										  WHERE   t0.CANCELED = 'N'
										  GROUP BY t0.""DocEntry"", ""CardCode"", t0.""DocDate"", ""DocTotal""
										  UNION ALL 
										  SELECT  t0.""DocEntry"", ""CardCode"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", ""DocTotal""
												  , SUM( t1.""Quantity"" * ( t1.""PriceAfVAT"" - t1.""StockPrice"" ) ) AS ""Margin""
												  , SUM( t1.""Quantity"" * t1.""PriceAfVAT"" ) AS ""TaxlessTotal""
										  FROM    {DBLA}.OINV t0
												  INNER JOIN {DBLA}.INV1 t1 ON t0.""DocEntry"" = t1.""DocEntry""
										  WHERE   CANCELED = 'N'
										  GROUP BY t0.""DocEntry"", ""CardCode"", t0.""DocDate"", ""DocTotal"" ) a
							  WHERE   ""CardCode"" = '{CardCode}'          
							  GROUP BY ""CardCode"" ";
			BEA.ClientStatics item = SQLSearch<BEA.ClientStatics>(query);
			return item;
		}

		public BEA.Client SearchAmount(string CardCode, DateTime Since, params Enum[] Relations)
		{
			string query = $@"SELECT  ""CardCode"", SUM(""DocTotal"") AS ""Balance""
							  FROM    ( SELECT    ""CardCode"", CAST(""DocDate"" AS DATE) AS ""DocDate"", ""DocTotal""
										FROM      {DBSA}.OINV
										UNION
										SELECT    ""CardCode"", CAST(""DocDate"" AS DATE) AS ""DocDate"", ""DocTotal""
										FROM      {DBIQ}.OINV
										UNION 
										SELECT    ""CardCode"", CAST(""DocDate"" AS DATE) AS ""DocDate"", ""DocTotal""
										FROM      {DBLA}.OINV ) a  
							  WHERE   ""CardCode"" = '{CardCode}' AND ""DocDate"" >= '{Since:yyyy-MM-dd}'
							  GROUP BY ""CardCode"" ";
			BEA.Client item = SQLSearch(query, Relations);
			return item;
		}

		public BEA.ClientHoldInfo SearchHoldInfo(string CardCode)
		{
			string query = $@"SELECT  ""CardCode"", ( CASE U_OHCRED WHEN 1 THEN 1 ELSE 0 END ) AS ""OnHoldCredit"", ( CASE U_OHMORA WHEN 1 THEN 1 ELSE 0 END ) AS ""OnHoldDue""
							FROM    DMC_SA.OCRD
							WHERE   LOWER(""CardCode"") = '{CardCode.ToLower()}' ";
			BEA.ClientHoldInfo item = SQLSearch<BEA.ClientHoldInfo>(query);
			return item;
		}

		#endregion

		#region Private Methods

		private string GetQuery()
		{
			//var sb = new StringBuilder();
			//sb.AppendLine($@"SELECT  ""CardCode"", ""CardName"", ""Address"", ""MailAddres"", ""Phone1"", ""Phone2"", T0.""Cellular"", T0.""Fax"", ""E_Mail"" AS ""EMail"", ""CardFName"", ""LicTradNum"" AS ""NIT"", ""Balance"", ""OrdersBal"" AS ""OrdersBalance"" ");
			//sb.AppendLine($@"        , IFNULL(T1.""SlpName"", '') AS ""SellerCode"", IFNULL(T1.""Memo"", '') AS ""SellerName"", t2.""PymntGroup"" AS ""Terms"", IFNULL(t0.""CreditLine"", 0) AS ""CreditLimit"", CAST(t0.""CreateDate"" AS DATE) AS ""CreateDate"", T0.""City"" ");
			//sb.AppendLine($@"FROM    {DBSA}.OCRD T0 ");
			//sb.AppendLine($@"        LEFT OUTER JOIN {DBSA}.OSLP T1 ON T0.""SlpCode"" = T1.""SlpCode"" ");
			//sb.AppendLine($@"        INNER JOIN {DBSA}.OCTG t2 ON t0.""GroupNum"" = t2.""GroupNum"" ");
			//sb.AppendLine($@"WHERE   ""CardType"" = 'C' ");
			//return sb.ToString();
			string query = $@"SELECT  ""CardCode"", ""CardName"", ""Address"", ""MailAddres"", ""Phone1"", ""Phone2"", T0.""Cellular"", T0.""Fax"", ""E_Mail"" AS ""EMail"", ""CardFName"", ""LicTradNum"" AS ""NIT""
		, ""Balance"", ""OrdersBal"" AS ""OrdersBalance"", IFNULL(T1.""SlpName"", '') AS ""SellerCode"", IFNULL(T1.""Memo"", '') AS ""SellerName"", t2.""PymntGroup"" AS ""Terms""
		, IFNULL(t0.""CreditLine"", 0) AS ""CreditLimit"", CAST(t0.""CreateDate"" AS DATE) AS ""CreateDate"", T0.""City""
FROM    {DBSA}.OCRD T0 
		LEFT OUTER JOIN {DBSA}.OSLP T1 ON T0.""SlpCode"" = T1.""SlpCode"" 
		INNER JOIN {DBSA}.OCTG t2 ON t0.""GroupNum"" = t2.""GroupNum"" 
WHERE   ""CardType"" = 'C' ";
			return query;
		}

		#endregion

		#region Constructors

		public Client() : base() { }

		#endregion
	}
}
