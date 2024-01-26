using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using BE = BEntities;
using BEA = BEntities.SAP;

namespace DALayer.SAP.Hana
{
    [Serializable()]
    public class StateAccountDetail : DALEntity<BEA.StateAccountDetail>
    {
        #region Methods

        protected override void LoadRelations(ref BEA.StateAccountDetail Item, params Enum[] Relations) { }

        protected override void LoadRelations(ref IEnumerable<BEA.StateAccountDetail> Items, params Enum[] Relations) { }

        #endregion

        #region List Methods

        public IEnumerable<BEA.StateAccountDetail> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sb = new();
            string strFilter = FilterList?.Count > 0 ? GetFilter(FilterList.ToArray()) : "";
            sb.AppendLine($@"SELECT  * ");
            sb.AppendLine($@"FROM    ({GetQuery()}) a ");
            if (strFilter != "") sb.AppendLine($@"WHERE   {strFilter} ");
            sb.AppendLine($@"ORDER By {GetOrder(Order)} ");

            IEnumerable<BEA.StateAccountDetail> items = SQLList(sb.ToString(), Relations);
            return items;
        }

        public IEnumerable<BEA.StateAccountDetail> List2(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sb = new();
            string strFilter = FilterList?.Count > 0 ? GetFilter(FilterList.ToArray()) : "1 = 1";
            sb.AppendLine(@"SELECT  * ");
            sb.AppendLine(@"FROM	COMMON.PortalView_ClientResume ");
            sb.AppendLine($@"WHERE  {strFilter} ");
            sb.AppendLine($@"ORDER BY {GetOrder(Order)} ");

            IEnumerable<BEA.StateAccountDetail> items = SQLList(sb.ToString(), Relations);
            return items;
        }

        public IEnumerable<BEA.StateAccountDetail> ListResume(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sb = new();
            string strFilter = FilterList?.Count > 0 ? GetFilter(FilterList.ToArray()) : "1 = 1";
            sb.AppendLine(@"SELECT  ""Type"", SUM(""DocTotal"") AS ""DocTotal"" ");
            sb.AppendLine(@"FROM	COMMON.PortalView_ClientResume ");
            sb.AppendLine($@"WHERE  {strFilter} ");
            sb.AppendLine(@"GROUP BY ""Type"" ");
            sb.AppendLine($@"ORDER BY {GetOrder(Order)} ");

            IEnumerable<BEA.StateAccountDetail> items = SQLList(sb.ToString(), Relations);
            return items;
        }

        #endregion

        #region Search Methods

        #endregion

        #region Private Methods

        private string GetQuery()
        {
            var sb = new StringBuilder();
            sb.AppendLine($@"SELECT  'MIAMI' AS ""Subsidiary"", T4.""WhsName"" AS ""Warehouse"", 'Factura' AS ""Type"", T0.""CardCode"", T3.""CardName"", T0.""DocNum"", T1.""BaseRef"" AS ""DocBase"", T0.""NumAtCard"" AS ""ClientOrder"", T0.""DocDate"", T2.""PymntGroup"" AS ""Terms"" ");
            sb.AppendLine($@"        , T0.""DocDueDate"" AS ""DueDate"", T0.""DocTotal"", ( T0.""DocTotal"" - T0.""PaidToDate"" ) AS ""Balance"", DAYS_BETWEEN(T0.""DocDueDate"", NOW()) AS ""Days"", T6.""Memo"" AS ""Seller"", CAST(T5.""Header"" AS VARCHAR(5000)) AS ""Header"" ");
            sb.AppendLine($@"        , CAST(T5.""Footer"" AS VARCHAR(5000)) AS ""Footer"", T1.""ItemCode"", T1.""Dscription"" AS ""ItemName"", T1.""Quantity"", T1.""PriceAfVAT"" AS ""Price"", T1.""StockPrice"", 1 AS ""Factor"" ");
            sb.AppendLine($@"        , ( CASE WHEN DAYS_BETWEEN(T0.""DocDueDate"", NOW()) <= 0 THEN 'Vigente' ELSE 'Mora' END ) AS ""State"" ");
            sb.AppendLine($@"FROM    {DBLA}.OINV T0 ");
            sb.AppendLine($@"        INNER JOIN {DBLA}.INV1 T1 ON T0.""DocEntry"" = T1.""DocEntry"" AND ( T0.""DocTotal"" - T0.""PaidToDate"" ) <> 0 ");
            sb.AppendLine($@"        INNER JOIN {DBLA}.OCTG T2 ON T0.""GroupNum"" = T2.""GroupNum"" ");
            sb.AppendLine($@"        INNER JOIN {DBLA}.OCRD T3 ON T3.""CardCode"" = T0.""CardCode"" ");
            sb.AppendLine($@"        LEFT OUTER JOIN {DBLA}.OWHS T4 ON T4.""WhsCode"" = T1.""WhsCode"" ");
            sb.AppendLine($@"        INNER JOIN {DBLA}.ORDR T5 ON T5.""DocEntry"" = T1.""BaseEntry"" ");
            sb.AppendLine($@"        INNER JOIN {DBLA}.OSLP T6 ON T1.""SlpCode"" = T6.""SlpCode"" ");
            sb.AppendLine($@"WHERE   ( T0.""CardCode"" <> 'CIMP-008' AND   T0.""CardCode"" <> 'CDMC-002' ) ");
            sb.AppendLine($@"UNION ");
            sb.AppendLine($@"SELECT  'IQUIQUE', T4.""WhsName"", 'Factura', T0.""CardCode"", T3.""CardName"", T0.""DocNum"", T1.""BaseRef"", T0.""NumAtCard"", T0.""DocDate"", T2.""PymntGroup"", T0.""DocDueDate"", T0.""DocTotal"", ( T0.""DocTotal"" - T0.""PaidToDate"" ) ");
            sb.AppendLine($@"        , DAYS_BETWEEN(T0.""DocDueDate"", NOW()), T6.""Memo"", CAST(T5.""Header"" AS VARCHAR(5000)), CAST(T5.""Footer"" AS VARCHAR(5000)), T1.""ItemCode"", T1.""Dscription"", T1.""Quantity"", T1.""PriceAfVAT"", T1.""StockPrice"", 1 ");
            sb.AppendLine($@"        , ( CASE WHEN DAYS_BETWEEN(T0.""DocDueDate"", NOW()) <= 0 THEN 'Vigente' ELSE 'Mora' END ) AS ""State"" ");
            sb.AppendLine($@"FROM    {DBIQ}.OINV T0 ");
            sb.AppendLine($@"        INNER JOIN {DBIQ}.INV1 T1 ON T0.""DocEntry"" = T1.""DocEntry"" AND ( T0.""DocTotal"" - T0.""PaidToDate"" ) <> 0 ");
            sb.AppendLine($@"        INNER JOIN {DBIQ}.OCTG T2 ON T0.""GroupNum"" = T2.""GroupNum"" ");
            sb.AppendLine($@"        INNER JOIN {DBIQ}.OCRD T3 ON T3.""CardCode"" = T0.""CardCode"" ");
            sb.AppendLine($@"        LEFT OUTER JOIN {DBIQ}.OWHS T4 ON T4.""WhsCode"" = T1.""WhsCode"" ");
            sb.AppendLine($@"        INNER JOIN {DBIQ}.ORDR T5 ON T5.""DocEntry"" = T1.""BaseEntry"" ");
            sb.AppendLine($@"        INNER JOIN {DBIQ}.OSLP T6 ON T1.""SlpCode"" = T6.""SlpCode"" ");
            sb.AppendLine($@"WHERE   ( T0.""CardCode"" <> 'CDMC-002' ) ");
            sb.AppendLine($@"UNION ");
            sb.AppendLine($@"SELECT  'SANTA CRUZ', T4.""WhsName"", 'Factura', T0.""CardCode"", T3.""CardName"", T0.""DocNum"", T1.""BaseRef"", T0.""NumAtCard"", T0.""DocDate"", T2.""PymntGroup"", T0.""DocDueDate"", T0.""DocTotal"", ( T0.""DocTotal"" - T0.""PaidToDate"" ) ");
            sb.AppendLine($@"        , DAYS_BETWEEN(T0.""DocDueDate"", NOW()), T6.""Memo"", CAST(T5.""Header"" AS VARCHAR(5000)), CAST(T5.""Footer"" AS VARCHAR(5000)), T1.""ItemCode"", T1.""Dscription"", T1.""Quantity"", T1.""PriceAfVAT"", T1.""StockPrice"" ");
            sb.AppendLine($@"        , ( CASE WHEN T1.""WhsCode"" = 'ZFSCZ' THEN 1 ELSE 0.84 END ), ( CASE WHEN DAYS_BETWEEN(T0.""DocDueDate"", NOW()) <= 0 THEN 'Vigente' ELSE 'Mora' END ) AS ""State"" ");
            sb.AppendLine($@"FROM    {DBSA}.OINV T0 ");
            sb.AppendLine($@"        INNER JOIN {DBSA}.INV1 T1 ON T0.""DocEntry"" = T1.""DocEntry"" AND ( T0.""DocTotal"" - T0.""PaidToDate"" ) <> 0 ");
            sb.AppendLine($@"        INNER JOIN {DBSA}.OCTG T2 ON T0.""GroupNum"" = T2.""GroupNum"" ");
            sb.AppendLine($@"        INNER JOIN {DBSA}.OCRD T3 ON T3.""CardCode"" = T0.""CardCode"" ");
            sb.AppendLine($@"        LEFT OUTER JOIN {DBSA}.OWHS T4 ON T4.""WhsCode"" = T1.""WhsCode"" ");
            sb.AppendLine($@"        INNER JOIN {DBSA}.ORDR T5 ON T5.""DocEntry"" = T1.""BaseEntry"" ");
            sb.AppendLine($@"        INNER JOIN {DBSA}.OSLP T6 ON T1.""SlpCode"" = T6.""SlpCode"" ");
            sb.AppendLine($@"UNION ");
            sb.AppendLine($@"SELECT  'MIAMI' AS ""Subsidiary"", T4.""WhsName"" AS ""Warehouse"", 'Nota de Débito' AS ""Type"", T0.""CardCode"", T3.""CardName"", T0.""DocNum"", T1.""BaseRef"" AS ""DocBase"", '' AS ""ClientOrder"", T0.""DocDate"", T2.""PymntGroup"" AS ""Terms"" ");
            sb.AppendLine($@"        , T0.""DocDueDate"" AS ""DueDate"", T0.""DocTotal"", ( T0.""DocTotal"" - T0.""PaidToDate"" ) AS ""Balance"", DAYS_BETWEEN(T0.""DocDueDate"", NOW()) AS ""Days"", T6.""Memo"" AS ""Seller"", '' AS ""Header"", '' AS ""Footer"", T1.""ItemCode"" ");
            sb.AppendLine($@"        , T1.""Dscription"" AS ""ItemName"", T1.""Quantity"", T1.""PriceAfVAT"" AS ""Price"", T1.""StockPrice"", 1, '' ");
            sb.AppendLine($@"FROM    {DBLA}.OINV T0 ");
            sb.AppendLine($@"        INNER JOIN {DBLA}.INV1 T1 ON T0.""DocEntry"" = T1.""DocEntry"" ");
            sb.AppendLine($@"        INNER JOIN {DBLA}.OCTG T2 ON T0.""GroupNum"" = T2.""GroupNum"" ");
            sb.AppendLine($@"        INNER JOIN {DBLA}.OCRD T3 ON T3.""CardCode"" = T0.""CardCode"" ");
            sb.AppendLine($@"        LEFT OUTER JOIN {DBLA}.OWHS T4 ON T4.""WhsCode"" = T1.""WhsCode"" ");
            sb.AppendLine($@"        INNER JOIN {DBLA}.OSLP T6 ON T1.""SlpCode"" = T6.""SlpCode"" ");
            sb.AppendLine($@"WHERE   T0.CANCELED = 'N' AND   ( T0.""CardCode"" <> 'CIMP-008' AND   T0.""CardCode"" <> 'CDMC-002' ) AND   ( T1.""TargetType"" <> 14 OR  T1.""TargetType"" IS NULL ) AND   T0.""DocSubType"" = 'DN' AND  ( T0.""DocTotal"" - T0.""PaidToDate"" ) <> 0 ");
            sb.AppendLine($@"UNION ");
            sb.AppendLine($@"SELECT  'IQUIQUE', T4.""WhsName"", 'Nota de Débito', T0.""CardCode"", T3.""CardFName"", T0.""DocNum"", T1.""BaseRef"", '', T0.""DocDate"", T2.""PymntGroup"", T0.""DocDueDate"", T0.""DocTotal"", ( T0.""DocTotal"" - T0.""PaidToDate"" ) AS ""Balance"" ");
            sb.AppendLine($@"        , DAYS_BETWEEN(T0.""DocDueDate"", NOW()) AS ""Days"", T6.""Memo"", '' AS ""Header"", '' AS ""Footer"", T1.""ItemCode"", T1.""Dscription"" AS ""ItemName"", T1.""Quantity"", T1.""PriceAfVAT"" AS ""Price"", T1.""StockPrice"", 1, '' ");
            sb.AppendLine($@"FROM    {DBIQ}.OINV T0 ");
            sb.AppendLine($@"        INNER JOIN {DBIQ}.INV1 T1 ON T0.""DocEntry"" = T1.""DocEntry"" ");
            sb.AppendLine($@"        INNER JOIN {DBIQ}.OCTG T2 ON T0.""GroupNum"" = T2.""GroupNum"" ");
            sb.AppendLine($@"        INNER JOIN {DBIQ}.OCRD T3 ON T3.""CardCode"" = T0.""CardCode"" ");
            sb.AppendLine($@"        LEFT OUTER JOIN {DBIQ}.OWHS T4 ON T4.""WhsCode"" = T1.""WhsCode"" ");
            sb.AppendLine($@"        INNER JOIN {DBIQ}.OSLP T6 ON T1.""SlpCode"" = T6.""SlpCode"" ");
            sb.AppendLine($@"WHERE   T0.CANCELED = 'N' AND   ( T0.""CardCode"" <> 'CDMC-002' ) AND ( T1.""TargetType"" <> 14 OR  T1.""TargetType"" IS NULL ) AND   T0.""DocSubType"" = 'DN' AND  ( T0.""DocTotal"" - T0.""PaidToDate"" ) <> 0 ");
            sb.AppendLine($@"UNION ");
            sb.AppendLine($@"SELECT  'SANTA CRUZ', 'Nota de Débito', T0.""CardCode"", T3.""CardName"", T4.""WhsName"", T0.""DocNum"", T1.""BaseRef"", '', T0.""DocDate"", T2.""PymntGroup"", T0.""DocDueDate"", T0.""DocTotal"" AS ""TOTAL"", ( T0.""DocTotal"" - T0.""PaidToDate"" ) AS ""Balance"" ");
            sb.AppendLine($@"        , DAYS_BETWEEN(T0.""DocDueDate"", NOW()) AS ""Days"", T6.""Memo"", '' AS ""Header"", '' AS ""Footer"", T1.""ItemCode"", T1.""Dscription"" AS ""ItemName"", T1.""Quantity"", T1.""PriceAfVAT"" AS ""Price"", T1.""StockPrice"" ");
            sb.AppendLine($@"        , ( CASE WHEN T1.""WhsCode"" = 'ZFSCZ' THEN 1 ELSE 0.84 END ), '' ");
            sb.AppendLine($@"FROM    {DBSA}.OINV T0 ");
            sb.AppendLine($@"        INNER JOIN {DBSA}.INV1 T1 ON T0.""DocEntry"" = T1.""DocEntry"" ");
            sb.AppendLine($@"        INNER JOIN {DBSA}.OCTG T2 ON T0.""GroupNum"" = T2.""GroupNum"" ");
            sb.AppendLine($@"        INNER JOIN {DBSA}.OCRD T3 ON T3.""CardCode"" = T0.""CardCode"" ");
            sb.AppendLine($@"        LEFT OUTER JOIN {DBSA}.OWHS T4 ON T4.""WhsCode"" = T1.""WhsCode"" ");
            sb.AppendLine($@"        INNER JOIN {DBSA}.OSLP T6 ON T1.""SlpCode"" = T6.""SlpCode"" ");
            sb.AppendLine($@"WHERE   T0.CANCELED = 'N' AND ( T1.""TargetType"" <> 14 OR  T1.""TargetType"" IS NULL ) AND T0.""DocSubType"" = 'DN' AND ( T0.""DocTotal"" - T0.""PaidToDate"" ) <> 0 ");
            sb.AppendLine($@"UNION ");
            sb.AppendLine($@"SELECT  'MIAMI' AS ""Subsidiary"", NULL, 'Pago Efectuado' AS ""Type"", T0.""CardCode"", T2.""CardName"", T0.""DocNum"", NULL, NULL, T0.""DocDate"", NULL, T0.""DocDueDate"" AS ""DueDate"", T0.""DocTotal"", T0.""OpenBal"" * -1 AS ""Balance"" ");
            sb.AppendLine($@"        , DAYS_BETWEEN(T0.""DocDueDate"", NOW()) AS ""DAYS"", NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 'Abono' ");
            sb.AppendLine($@"FROM    {DBLA}.ORCT T0 ");
            sb.AppendLine($@"        INNER JOIN {DBLA}.JDT1 T1 ON T0.""TransId"" = T1.""TransId"" ");
            sb.AppendLine($@"        INNER JOIN {DBLA}.OCRD T2 ON T2.""CardCode"" = T0.""CardCode"" ");
            sb.AppendLine($@"WHERE   T0.""PayNoDoc"" = 'Y' ");
            sb.AppendLine($@"        AND ( T0.""CardCode"" <> 'CIMP-008' AND   T0.""CardCode"" <> 'CDMC-002' ) ");
            sb.AppendLine($@"        AND T0.""Canceled"" = 'N' ");
            sb.AppendLine($@"        AND T0.""DocType"" <> 'A' ");
            sb.AppendLine($@"        AND T0.""Canceled"" = 'N' ");
            sb.AppendLine($@"        AND T1.""ShortName"" = T0.""CardCode"" ");
            sb.AppendLine($@"        AND T1.""IntrnMatch"" = 0 ");
            sb.AppendLine($@"        AND T0.""OpenBal"" > 0 ");
            sb.AppendLine($@"UNION ");
            sb.AppendLine($@"SELECT  'IQUIQUE', NULL, 'Pago Efectuado', T0.""CardCode"", T2.""CardFName"", T0.""DocNum"", NULL, NULL, T0.""DocDate"", NULL, T0.""DocDueDate"", T0.""DocTotal"" AS ""TOTAL"", T0.""OpenBal"" * -1, DAYS_BETWEEN(T0.""DocDueDate"", NOW()), NULL, NULL, NULL ");
            sb.AppendLine($@"        , NULL, NULL, NULL, NULL, NULL, 1, 'Abono' ");
            sb.AppendLine($@"FROM    {DBIQ}.ORCT T0 ");
            sb.AppendLine($@"        INNER JOIN {DBIQ}.JDT1 T1 ON T0.""TransId"" = T1.""TransId"" ");
            sb.AppendLine($@"        INNER JOIN {DBIQ}.OCRD T2 ON T2.""CardCode"" = T0.""CardCode"" ");
            sb.AppendLine($@"WHERE   T0.""PayNoDoc"" = 'Y' AND T0.""Canceled"" = 'N' AND ( T0.""CardCode"" <> 'CDMC-002' ) AND T0.""DocType"" <> 'A' AND T0.""Canceled"" = 'N' AND T1.""ShortName"" = T0.""CardCode"" AND  T1.""IntrnMatch"" = 0 AND T0.""OpenBal"" > 0 ");
            sb.AppendLine($@"UNION ");
            sb.AppendLine($@"SELECT  'SANTA CRUZ', NULL, 'Pago Efectuado', T0.""CardCode"", T2.""CardFName"", T0.""DocNum"", NULL, NULL, T0.""DocDate"", NULL, T0.""DocDueDate"", T0.""DocTotal"" AS ""TOTAL"", T0.""OpenBal"" * -1, DAYS_BETWEEN(T0.""DocDueDate"", NOW()), NULL, NULL, NULL ");
            sb.AppendLine($@"        , NULL, NULL, NULL, NULL, NULL, 1, 'Abono' ");
            sb.AppendLine($@"FROM    {DBSA}.ORCT T0 ");
            sb.AppendLine($@"        INNER JOIN {DBSA}.JDT1 T1 ON T0.""TransId"" = T1.""TransId"" ");
            sb.AppendLine($@"        INNER JOIN {DBSA}.OCRD T2 ON T2.""CardCode"" = T0.""CardCode"" ");
            sb.AppendLine($@"WHERE   T0.""PayNoDoc"" = 'Y' AND T0.""Canceled"" = 'N' AND T0.""DocType"" <> 'A' AND T0.""Canceled"" = 'N' AND T1.""ShortName"" = T0.""CardCode"" AND  T1.""IntrnMatch"" = 0 AND T0.""OpenBal"" > 0 ");
            sb.AppendLine($@"UNION ");
            sb.AppendLine($@"SELECT  'MIAMI' AS ""Subsidiary"", NULL AS ""Warehouse"", 'Pago' AS ""Type"", T0.""CardCode"", T1.""CardName"", T0.""DocNum"", NULL, NULL, T0.""DocDate"", NULL, T0.""DocDueDate"", T0.""DocTotal"" AS ""TOTAL"", T0.""OpenBal"" ");
            sb.AppendLine($@"        , DAYS_BETWEEN(NOW(), T0.""DocDueDate"") AS ""Days"", NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, '' ");
            sb.AppendLine($@"FROM    {DBLA}.OVPM T0 ");
            sb.AppendLine($@"        INNER JOIN {DBLA}.OCRD T1 ON T0.""CardCode"" = T1.""CardCode"" ");
            sb.AppendLine($@"        INNER JOIN {DBLA}.JDT1 T2 ON T0.""TransId"" = T2.""TransId"" ");
            sb.AppendLine($@"WHERE   ( T0.""CardCode"" <> 'CIMP-008' AND   T0.""CardCode"" <> 'CDMC-002' ) AND   T0.""PayNoDoc"" = 'Y' AND T0.""Canceled"" = 'N' AND T1.""CardType"" = 'C' AND T2.""ShortName"" = T0.""CardCode"" AND  T2.""IntrnMatch"" = 0 ");
            sb.AppendLine($@"UNION ");
            sb.AppendLine($@"SELECT  'IQUIQUE', NULL, 'Pago', T0.""CardCode"", T1.""CardName"", T0.""DocNum"", NULL, NULL, T0.""DocDate"", NULL, T0.""DocDueDate"", T0.""DocTotal"", T0.""OpenBal"", DAYS_BETWEEN(NOW(), T0.""DocDueDate"") AS ""Days"", NULL, NULL, NULL, NULL, NULL, NULL ");
            sb.AppendLine($@"        , NULL, NULL, 1, '' ");
            sb.AppendLine($@"FROM    {DBIQ}.OVPM T0 ");
            sb.AppendLine($@"        INNER JOIN {DBIQ}.OCRD T1 ON T0.""CardCode"" = T1.""CardCode"" ");
            sb.AppendLine($@"        INNER JOIN {DBIQ}.JDT1 T2 ON T0.""TransId"" = T2.""TransId"" ");
            sb.AppendLine($@"WHERE   T0.""PayNoDoc"" = 'Y' AND T0.""Canceled"" = 'N' AND ( T0.""CardCode"" <> 'CDMC-002' ) AND T1.""CardType"" = 'C' AND T2.""ShortName"" = T0.""CardCode"" AND  T2.""IntrnMatch"" = 0 ");
            sb.AppendLine($@"UNION ");
            sb.AppendLine($@"SELECT  'SANTA CRUZ', NULL, 'Pago', T0.""CardCode"", T1.""CardName"", T0.""DocNum"", NULL, NULL, T0.""DocDate"", NULL, T0.""DocDueDate"", T0.""DocTotal"" AS ""TOTAL"", T0.""OpenBal"", DAYS_BETWEEN(NOW(), T0.""DocDueDate"") AS ""Days"", NULL, NULL, NULL, NULL ");
            sb.AppendLine($@"        , NULL, NULL, NULL, NULL, 1, '' ");
            sb.AppendLine($@"FROM    {DBSA}.OVPM T0 ");
            sb.AppendLine($@"        INNER JOIN {DBSA}.OCRD T1 ON T0.""CardCode"" = T1.""CardCode"" ");
            sb.AppendLine($@"        INNER JOIN {DBSA}.JDT1 T2 ON T0.""TransId"" = T2.""TransId"" ");
            sb.AppendLine($@"WHERE   T0.""PayNoDoc"" = 'Y' AND T0.""Canceled"" = 'N' AND T1.""CardType"" = 'C' AND T2.""ShortName"" = T0.""CardCode"" AND  T2.""IntrnMatch"" = 0 ");
            sb.AppendLine($@"UNION ");
            sb.AppendLine($@"SELECT  'MIAMI' AS ""Subsidiary"", NULL AS ""Warehouse"", 'Registro Diario' AS ""Type"", T2.""CardCode"", T2.""CardName"", T0.""TransId"" AS ""DocNum"", '', '', T0.""RefDate"", T3.""PymntGroup"", T1.""DueDate"", 0, ( T1.""Debit"" - T1.""Credit"" ) AS ""Balance"" ");
            sb.AppendLine($@"        , DAYS_BETWEEN(T1.""DueDate"", NOW()) AS ""Days"", NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, '' ");
            sb.AppendLine($@"FROM    {DBLA}.OJDT T0 ");
            sb.AppendLine($@"        INNER JOIN {DBLA}.JDT1 T1 ON T0.""TransId"" = T1.""TransId"" ");
            sb.AppendLine($@"        INNER JOIN {DBLA}.OCRD T2 ON T1.""ShortName"" = T2.""CardCode"" ");
            sb.AppendLine($@"        INNER JOIN {DBLA}.OCTG T3 ON T3.""GroupNum"" = T2.""GroupNum"" ");
            sb.AppendLine($@"WHERE   T1.""Account"" <> T1.""ShortName"" ");
            sb.AppendLine($@"        AND T2.""CardCode"" <> 'CIMP-008' ");
            sb.AppendLine($@"        AND T2.""CardCode"" <> 'CDMC-002' ");
            sb.AppendLine($@"        AND T0.""TransType"" = '30' ");
            sb.AppendLine($@"        AND T1.""IntrnMatch"" = 0 ");
            sb.AppendLine($@"        AND T2.""CardType"" = 'C' ");
            sb.AppendLine($@"        AND ( T1.""Debit"" - T1.""Credit"" ) <> 0 ");
            sb.AppendLine($@"        AND T1.""MthDate"" = '' ");
            sb.AppendLine($@"        AND T1.""BalDueCred"" <> 0 ");
            sb.AppendLine($@"UNION ");
            sb.AppendLine($@"SELECT  'IQUIQUE', NULL, 'Registro Diario', T2.""CardCode"", T2.""CardName"", T0.""TransId"", '', '', T0.""RefDate"", T3.""PymntGroup"", T1.""DueDate"", 0, ( T1.""Debit"" - T1.""Credit"" ) AS ""Balance"", DAYS_BETWEEN(T1.""DueDate"", NOW()) AS ""Days"", NULL ");
            sb.AppendLine($@"        , NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, '' ");
            sb.AppendLine($@"FROM    {DBIQ}.OJDT T0 ");
            sb.AppendLine($@"        INNER JOIN {DBIQ}.JDT1 T1 ON T0.""TransId"" = T1.""TransId"" ");
            sb.AppendLine($@"        INNER JOIN {DBIQ}.OCRD T2 ON T1.""ShortName"" = T2.""CardCode"" ");
            sb.AppendLine($@"        INNER JOIN {DBIQ}.OCTG T3 ON T3.""GroupNum"" = T2.""GroupNum"" ");
            sb.AppendLine($@"WHERE   T1.""Account"" <> T1.""ShortName"" ");
            sb.AppendLine($@"        AND T0.""TransType"" = '30' ");
            sb.AppendLine($@"        AND ( T2.""CardCode"" <> 'CDMC-002' ) ");
            sb.AppendLine($@"        AND T1.""IntrnMatch"" = 0 ");
            sb.AppendLine($@"        AND T2.""CardType"" = 'C' ");
            sb.AppendLine($@"        AND ( T1.""Debit"" - T1.""Credit"" ) <> 0 ");
            sb.AppendLine($@"        AND T1.""MthDate"" = '' ");
            sb.AppendLine($@"        AND T1.""BalDueCred"" <> 0 ");
            sb.AppendLine($@"UNION ");
            sb.AppendLine($@"SELECT  'SANTA CRUZ', NULL, 'Registro Diario', T2.""CardCode"", T2.""CardName"", T0.""TransId"", '', '', T0.""RefDate"", T3.""PymntGroup"", T1.""DueDate"", 0, ( T1.""Debit"" - T1.""Credit"" ) AS ""Balance"", DAYS_BETWEEN(T1.""DueDate"", NOW()) AS ""Days"", NULL ");
            sb.AppendLine($@"        , NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, '' ");
            sb.AppendLine($@"FROM    {DBSA}.OJDT T0 ");
            sb.AppendLine($@"        INNER JOIN {DBSA}.JDT1 T1 ON T0.""TransId"" = T1.""TransId"" ");
            sb.AppendLine($@"        INNER JOIN {DBSA}.OCRD T2 ON T1.""ShortName"" = T2.""CardCode"" ");
            sb.AppendLine($@"        INNER JOIN {DBSA}.OCTG T3 ON T3.""GroupNum"" = T2.""GroupNum"" ");
            sb.AppendLine($@"WHERE   T1.""Account"" <> T1.""ShortName"" AND  T0.""TransType"" = '30' AND   T1.""IntrnMatch"" = 0 AND T2.""CardType"" = 'C' AND ( T1.""Debit"" - T1.""Credit"" ) <> 0 AND   T1.""MthDate"" = '' AND   T1.""BalDueCred"" <> 0 ");
            sb.AppendLine($@"UNION ");
            sb.AppendLine($@"SELECT  'MIAMI' AS ""Subsidiary"", T4.""WhsName"" AS ""Warehouse"", 'Devolucion' AS ""Type"", T0.""CardCode"", T2.""CardName"", T0.""DocNum"", T1.""BaseRef"", '', T0.""DocDate"", '', T0.""DocDueDate"", T0.""DocTotal"" ");
            sb.AppendLine($@"        , ( -T0.""DocTotal"" + T0.""PaidToDate"" ) AS ""Balance"", DAYS_BETWEEN(T0.""DocDueDate"", NOW()) AS ""Days"", NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, '' ");
            sb.AppendLine($@"FROM    {DBLA}.ORIN T0 ");
            sb.AppendLine($@"        INNER JOIN {DBLA}.RIN1 T1 ON T0.""DocEntry"" = T1.""DocEntry"" ");
            sb.AppendLine($@"        INNER JOIN {DBLA}.OCRD T2 ON T2.""CardCode"" = T0.""CardCode"" ");
            sb.AppendLine($@"        LEFT OUTER JOIN {DBLA}.OWHS T4 ON T4.""WhsCode"" = T1.""WhsCode"" ");
            sb.AppendLine($@"WHERE   T0.""DocStatus"" = 'O' AND ( T0.""CardCode"" <> 'CIMP-008' AND  T0.""CardCode"" <> 'CDMC-002' ) ");
            sb.AppendLine($@"UNION ");
            sb.AppendLine($@"SELECT  'IQUIQUE', T4.""WhsName"", 'Devolucion', T0.""CardCode"", T2.""CardName"", T0.""DocNum"", T1.""BaseRef"", '', T0.""DocDate"", '', T0.""DocDueDate"", T0.""DocTotal"", ( -T0.""DocTotal"" + T0.""PaidToDate"" ) AS ""Balance"" ");
            sb.AppendLine($@"        , DAYS_BETWEEN(T0.""DocDueDate"", NOW()) AS ""Days"", NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, '' ");
            sb.AppendLine($@"FROM    {DBIQ}.ORIN T0 ");
            sb.AppendLine($@"        INNER JOIN {DBIQ}.RIN1 T1 ON T0.""DocEntry"" = T1.""DocEntry"" ");
            sb.AppendLine($@"        INNER JOIN {DBIQ}.OCRD T2 ON T2.""CardCode"" = T0.""CardCode"" ");
            sb.AppendLine($@"        LEFT OUTER JOIN {DBIQ}.OWHS T4 ON T4.""WhsCode"" = T1.""WhsCode"" ");
            sb.AppendLine($@"WHERE   T0.""DocStatus"" = 'O' AND ( T0.""CardCode"" <> 'CDMC-002' ) ");
            sb.AppendLine($@"UNION ");
            sb.AppendLine($@"SELECT  'SANTA CRUZ', T4.""WhsName"", 'Devolucion', T0.""CardCode"", T2.""CardFName"", T0.""DocNum"", T1.""BaseRef"", '', T0.""DocDate"", '', T0.""DocDueDate"", T0.""DocTotal"", ( -T0.""DocTotal"" + T0.""PaidToDate"" ) AS ""Balance"" ");
            sb.AppendLine($@"        , DAYS_BETWEEN(T0.""DocDueDate"", NOW()) AS ""Days"", NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, ( CASE WHEN T1.""WhsCode"" = 'ZFSCZ' THEN 1 ELSE 0.84 END ), '' ");
            sb.AppendLine($@"FROM    {DBSA}.ORIN T0 ");
            sb.AppendLine($@"        INNER JOIN {DBSA}.RIN1 T1 ON T0.""DocEntry"" = T1.""DocEntry"" ");
            sb.AppendLine($@"        INNER JOIN {DBSA}.OCRD T2 ON T2.""CardCode"" = T0.""CardCode"" ");
            sb.AppendLine($@"        LEFT OUTER JOIN {DBSA}.OWHS T4 ON T4.""WhsCode"" = T1.""WhsCode"" ");
            sb.AppendLine($@"WHERE   T0.""DocStatus"" = 'O' ");
            return sb.ToString();
        }

        #endregion

        #region Constructors

        public StateAccountDetail() : base() { }

        #endregion
    }
}
