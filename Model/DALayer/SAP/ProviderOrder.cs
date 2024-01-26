using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BEA = BEntities.SAP;

namespace DALayer.SAP
{
    [Serializable()]
    public class ProviderOrder : Hana.DALEntity<BEA.ProviderOrder>
    {
        #region Methods

        protected override void LoadRelations(ref BEA.ProviderOrder Item, params Enum[] Relations) { }

        protected override void LoadRelations(ref IEnumerable<BEA.ProviderOrder> Items, params Enum[] Relations) { }

        #endregion

        #region List Methods

        //public List<BEA.Item> List(List<Field> FilterList, string Order, params Enum[] Relations)
        //{
        //    var filter = GetFilter(FilterList?.ToArray());
        //    var sb = new StringBuilder();
        //    sb.AppendLine($@"SELECT  ""Address"" AS ""Name"", ""CardCode"", ""Street"" AS ""Address1"", ""Block"" AS ""Contact"", t0.""Country"", ""City"", t1.""Name"" AS ""State"", ""TaxCode"", ""AdresType"" AS ""Type"", ""Address2"", ""Address3"" ");
        //    sb.AppendLine($@"FROM    {DBSA}.CRD1 t0 ");
        //    sb.AppendLine($@"        LEFT OUTER JOIN {DBSA}.OCST t1 ON t0.""State"" = t1.""Id"" AND t0.""Country"" = t1.""Country"" ");
        //    sb.AppendLine($@"WHERE	{filter} ");
        //    sb.AppendLine($@"ORDER By {GetOrder(Order)}");

        //    List<BEA.Item> items = SQLList<BEA.Item>(sb.ToString());
        //    return items;
        //}

        public IEnumerable<BEA.ProviderOrder> ListFull(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            string strFilter = FilterList?.Count > 0 ? GetFilter(FilterList.ToArray()) : "";
            var sb = new StringBuilder();
            sb.AppendLine($@"SELECT  DISTINCT ""Subsidiary"", ""DocNumber"", ""DocDate"", ""EstimatedDate"", ""State"", ""ProviderCode"", ""ProviderName"", ""ReferenceOrder"", ""Warehouse"", ""SellerCode"", ""Terms"", ""OtherCosts"", ""Total"", ""BillNumber""  ");
            sb.AppendLine($@"        , ""BillDate"", ""Quantity"", ""OpenQuantity"", ""FilePath"", ""FileName""  ");
            sb.AppendLine($@"FROM    ( SELECT    DISTINCT 'Santa Cruz' AS ""Subsidiary"", T1.""DocNum"" AS ""DocNumber"", CAST(T1.""DocDate"" AS DATE) AS ""DocDate"", CAST(T1.""DocDueDate"" AS DATE) AS ""EstimatedDate""  ");
            sb.AppendLine($@"                    , ( CASE WHEN T1.""DocStatus"" = 'C' THEN 'CERRADO' ELSE 'ABIERTO' END ) AS ""State"", T1.""CardCode"" AS ""ProviderCode"", ( CASE IFNULL(T1.""CardName"", '') WHEN '' THEN T1.""CardCode"" ELSE T1.""CardName"" END ) AS ""ProviderName""  ");
            sb.AppendLine($@"                    , T1.""NumAtCard"" AS ""ReferenceOrder"", T2.""WhsName"" AS ""Warehouse"", T3.""SlpName"" AS ""SellerCode"", T5.""PymntGroup"" AS ""Terms"", T1.""TotalExpns"" AS ""OtherCosts"", T1.""DocTotal"" AS ""Total""  ");
            sb.AppendLine($@"                    , ( CASE IFNULL(T7.""BaseType"", 20) WHEN 22 THEN T8.""DocNum"" ELSE T10.""DocNum"" END ) AS ""BillNumber"", CAST(( CASE IFNULL(T7.""BaseType"", 20) WHEN 22 THEN T8.""DocDate"" ELSE T10.""DocDate"" END ) AS date) AS ""BillDate""  ");
            sb.AppendLine($@"                    , ( SELECT  CAST(SUM(""Quantity"") AS INTEGER)FROM   {DBSA}.POR1 WHERE ""DocEntry"" = T1.""DocEntry"" ) AS ""Quantity""  ");
            sb.AppendLine($@"                    , ( SELECT  CAST(SUM(""OpenCreQty"") AS INTEGER)FROM {DBSA}.POR1 WHERE ""DocEntry"" = T1.""DocEntry"" ) AS ""OpenQuantity"" ");
            sb.AppendLine($@"                    , T6.""ItemCode"", T6.""Dscription"" AS ""ItemName"", T12.U_LINEA AS ""ItemLine"", T12.U_CATEGORIA AS ""ItemCategory"", T12.U_SUBCATEG AS ""ItemSubcategory"" ");
            sb.AppendLine($@"                    , ( CASE IFNULL(T7.""BaseType"", 20) WHEN 22 THEN T13.""FilePath"" ELSE T14.""FilePath"" END ) AS ""FilePath"" ");
            sb.AppendLine($@"                    , ( CASE IFNULL(T7.""BaseType"", 20) WHEN 22 THEN T13.""FileName"" ELSE T14.""FileName"" END ) AS ""FileName"" ");
            sb.AppendLine($@"          FROM      {DBSA}.OPOR T1  ");
            sb.AppendLine($@"                    INNER JOIN {DBSA}.OSLP T3 ON T1.""SlpCode"" = T3.""SlpCode""  ");
            sb.AppendLine($@"                    INNER JOIN {DBSA}.OCTG T5 ON T1.""GroupNum"" = T5.""GroupNum""  ");
            sb.AppendLine($@"                    INNER JOIN {DBSA}.POR1 T6 ON T1.""DocEntry"" = T6.""DocEntry""  ");
            sb.AppendLine($@"                    INNER JOIN {DBSA}.OWHS T2 ON T6.""WhsCode"" = T2.""WhsCode""  ");
            sb.AppendLine($@"                    LEFT OUTER JOIN {DBSA}.PCH1 T7 ON T7.""BaseRef"" = CAST(T1.""DocNum"" AS VARCHAR(10))  ");
            sb.AppendLine($@"                    LEFT OUTER JOIN {DBSA}.OPCH T8 ON T8.""DocEntry"" = T7.""DocEntry"" AND T8.CANCELED = 'N' ");
            sb.AppendLine($@"                    LEFT OUTER JOIN {DBSA}.OPDN T11 ON T11.""DocEntry"" = T6.""TrgetEntry""  ");
            sb.AppendLine($@"                    LEFT OUTER JOIN {DBSA}.PCH1 T9 ON T9.""BaseRef"" = CAST(T11.""DocNum"" AS VARCHAR(10)) AND  T9.""BaseType"" = 20  ");
            sb.AppendLine($@"                    LEFT OUTER JOIN {DBSA}.OPCH T10 ON T10.""DocEntry"" = T9.""DocEntry""  ");
            sb.AppendLine($@"                    INNER JOIN {DBSA}.OITM T12 ON T6.""ItemCode"" = T12.""ItemCode""  ");
            sb.AppendLine($@"                    LEFT OUTER JOIN ( SELECT ""AbsEntry"", FIRST_VALUE(CAST(""trgtPath"" AS VARCHAR(5000)) ORDER BY ""Line"") AS ""FilePath"", STRING_AGG(CAST(""FileName"" AS VARCHAR(5000)) || '.' || CAST(""FileExt"" AS VARCHAR(5000)), '; ') AS ""FileName"" ");
            sb.AppendLine($@"                                      FROM   {DBSA}.ATC1 s0 ");
            sb.AppendLine($@"                                      GROUP BY ""AbsEntry"" ) t13 ON T8.""AtcEntry"" = t13.""AbsEntry"" ");
            sb.AppendLine($@"                    LEFT OUTER JOIN ( SELECT ""AbsEntry"", FIRST_VALUE(CAST(""trgtPath"" AS VARCHAR(5000)) ORDER BY ""Line"") AS ""FilePath"", STRING_AGG(CAST(""FileName"" AS VARCHAR(5000)) || '.' || CAST(""FileExt"" AS VARCHAR(5000)), '; ') AS ""FileName"" ");
            sb.AppendLine($@"                                      FROM   {DBSA}.ATC1 s0 ");
            sb.AppendLine($@"                                      GROUP BY ""AbsEntry"" ) t14 ON T10.""AtcEntry"" = t14.""AbsEntry"" ");
            sb.AppendLine($@"          WHERE     T1.CANCELED = 'N'  ");
            sb.AppendLine($@"          UNION  ");
            sb.AppendLine($@"          SELECT    DISTINCT 'Miami', T1.""DocNum"", CAST(T1.""DocDate"" AS DATE), CAST(T1.""DocDueDate"" AS DATE) ");
            sb.AppendLine($@"                    , ( CASE WHEN T1.""DocStatus"" = 'C' THEN 'CERRADO' ELSE 'ABIERTO' END ), T1.""CardCode"", ( CASE IFNULL(T1.""CardName"", '') WHEN '' THEN T1.""CardCode"" ELSE T1.""CardName"" END ) ");
            sb.AppendLine($@"                    , T1.""NumAtCard"", T2.""WhsName"", T3.""SlpName"", T5.""PymntGroup"", T1.""TotalExpns"", T1.""DocTotal"" ");
            sb.AppendLine($@"                    , ( CASE IFNULL(T7.""BaseType"", 20) WHEN 22 THEN T8.""DocNum"" ELSE T10.""DocNum"" END ) AS ""BillNumber"", CAST(( CASE IFNULL(T7.""BaseType"", 20) WHEN 22 THEN T8.""DocDate"" ELSE T10.""DocDate"" END ) AS date) AS ""BillDate""  ");
            sb.AppendLine($@"                    , ( SELECT  CAST(SUM(""Quantity"") AS INTEGER)FROM   {DBLA}.POR1 WHERE ""DocEntry"" = T1.""DocEntry"" ) ");
            sb.AppendLine($@"                    , ( SELECT  CAST(SUM(""OpenCreQty"") AS INTEGER)FROM {DBLA}.POR1 WHERE ""DocEntry"" = T1.""DocEntry"" ) ");
            sb.AppendLine($@"                    , T6.""ItemCode"", T6.""Dscription"", T12.U_LINEA, T12.U_CATEGORIA, T12.U_SUBCATEG ");
            sb.AppendLine($@"                    , ( CASE IFNULL(T7.""BaseType"", 20) WHEN 22 THEN T13.""FilePath"" ELSE T14.""FilePath"" END ) AS ""FilePath"" ");
            sb.AppendLine($@"                    , ( CASE IFNULL(T7.""BaseType"", 20) WHEN 22 THEN T13.""FileName"" ELSE T14.""FileName"" END ) AS ""FileName"" ");
            sb.AppendLine($@"          FROM      {DBLA}.OPOR T1  ");
            sb.AppendLine($@"                    INNER JOIN {DBLA}.OSLP T3 ON T1.""SlpCode"" = T3.""SlpCode""  ");
            sb.AppendLine($@"                    INNER JOIN {DBLA}.OCTG T5 ON T1.""GroupNum"" = T5.""GroupNum""  ");
            sb.AppendLine($@"                    INNER JOIN {DBLA}.POR1 T6 ON T1.""DocEntry"" = T6.""DocEntry""  ");
            sb.AppendLine($@"                    INNER JOIN {DBLA}.OWHS T2 ON T6.""WhsCode"" = T2.""WhsCode""  ");
            sb.AppendLine($@"                    LEFT OUTER JOIN {DBLA}.PCH1 T7 ON T7.""BaseRef"" = CAST(T1.""DocNum"" AS VARCHAR(10))  ");
            sb.AppendLine($@"                    LEFT OUTER JOIN {DBLA}.OPCH T8 ON T8.""DocEntry"" = T7.""DocEntry"" AND T8.CANCELED = 'N' ");
            sb.AppendLine($@"                    LEFT OUTER JOIN {DBLA}.OPDN T11 ON T11.""DocEntry"" = T6.""TrgetEntry""  ");
            sb.AppendLine($@"                    LEFT OUTER JOIN {DBLA}.PCH1 T9 ON T9.""BaseRef"" = CAST(T11.""DocNum"" AS VARCHAR(10)) AND T9.""BaseType"" = 20  ");
            sb.AppendLine($@"                    LEFT OUTER JOIN {DBLA}.OPCH T10 ON T10.""DocEntry"" = T9.""DocEntry""  ");
            sb.AppendLine($@"                    INNER JOIN {DBLA}.OITM T12 ON T6.""ItemCode"" = T12.""ItemCode""  ");
            sb.AppendLine($@"                    LEFT OUTER JOIN ( SELECT ""AbsEntry"", FIRST_VALUE(CAST(""trgtPath"" AS VARCHAR(5000)) ORDER BY ""Line"") AS ""FilePath"", STRING_AGG(CAST(""FileName"" AS VARCHAR(5000)) || '.' || CAST(""FileExt"" AS VARCHAR(5000)), '; ') AS ""FileName"" ");
            sb.AppendLine($@"                                      FROM   {DBLA}.ATC1 s0 ");
            sb.AppendLine($@"                                      GROUP BY ""AbsEntry"" ) t13 ON T8.""AtcEntry"" = t13.""AbsEntry"" ");
            sb.AppendLine($@"                    LEFT OUTER JOIN ( SELECT ""AbsEntry"", FIRST_VALUE(CAST(""trgtPath"" AS VARCHAR(5000)) ORDER BY ""Line"") AS ""FilePath"", STRING_AGG(CAST(""FileName"" AS VARCHAR(5000)) || '.' || CAST(""FileExt"" AS VARCHAR(5000)), '; ') AS ""FileName"" ");
            sb.AppendLine($@"                                      FROM   {DBLA}.ATC1 s0 ");
            sb.AppendLine($@"                                      GROUP BY ""AbsEntry"" ) t14 ON T10.""AtcEntry"" = t14.""AbsEntry"" ");
            sb.AppendLine($@"          WHERE     T1.CANCELED = 'N'  ");
            sb.AppendLine($@"          UNION  ");
            sb.AppendLine($@"          SELECT    DISTINCT 'Iquique', T1.""DocNum"", CAST(T1.""DocDate"" AS DATE), CAST(T1.""DocDueDate"" AS DATE)  ");
            sb.AppendLine($@"                    , ( CASE WHEN T1.""DocStatus"" = 'C' THEN 'CERRADO' ELSE 'ABIERTO' END ), T1.""CardCode"", ( CASE IFNULL(T1.""CardName"", '')WHEN '' THEN T1.""CardCode"" ELSE T1.""CardName"" END )  ");
            sb.AppendLine($@"                    , T1.""NumAtCard"", T2.""WhsName"", T3.""SlpName"", T5.""PymntGroup"", T1.""TotalExpns"", T1.""DocTotal"" ");
            sb.AppendLine($@"                    , ( CASE IFNULL(T7.""BaseType"", 20) WHEN 22 THEN T8.""DocNum"" ELSE T10.""DocNum"" END ) AS ""BillNumber"", CAST(( CASE IFNULL(T7.""BaseType"", 20) WHEN 22 THEN T8.""DocDate"" ELSE T10.""DocDate"" END ) AS date) AS ""BillDate""  ");
            sb.AppendLine($@"                    , ( SELECT  CAST(SUM(""Quantity"") AS INTEGER)FROM {DBIQ}.POR1 WHERE ""DocEntry"" = T1.""DocEntry"" ) ");
            sb.AppendLine($@"                    , ( SELECT    CAST(SUM(""OpenCreQty"") AS INTEGER)FROM {DBIQ}.POR1 WHERE  ""DocEntry"" = T1.""DocEntry"" ) ");
            sb.AppendLine($@"                    , T6.""ItemCode"", T6.""Dscription"", T12.U_LINEA, T12.U_CATEGORIA, T12.U_SUBCATEG ");
            sb.AppendLine($@"                    , ( CASE IFNULL(T7.""BaseType"", 20) WHEN 22 THEN T13.""FilePath"" ELSE T14.""FilePath"" END ) AS ""FilePath"" ");
            sb.AppendLine($@"                    , ( CASE IFNULL(T7.""BaseType"", 20) WHEN 22 THEN T13.""FileName"" ELSE T14.""FileName"" END ) AS ""FileName"" ");
            sb.AppendLine($@"          FROM      {DBIQ}.OPOR T1  ");
            sb.AppendLine($@"                    INNER JOIN {DBIQ}.OSLP T3 ON T1.""SlpCode"" = T3.""SlpCode""  ");
            sb.AppendLine($@"                    INNER JOIN {DBIQ}.OCTG T5 ON T1.""GroupNum"" = T5.""GroupNum""  ");
            sb.AppendLine($@"                    INNER JOIN {DBIQ}.POR1 T6 ON T1.""DocEntry"" = T6.""DocEntry""  ");
            sb.AppendLine($@"                    INNER JOIN {DBIQ}.OWHS T2 ON T6.""WhsCode"" = T2.""WhsCode""  ");
            sb.AppendLine($@"                    LEFT OUTER JOIN {DBIQ}.PCH1 T7 ON T7.""BaseRef"" = CAST(T1.""DocNum"" AS VARCHAR(10))  ");
            sb.AppendLine($@"                    LEFT OUTER JOIN {DBIQ}.OPCH T8 ON T8.""DocEntry"" = T7.""DocEntry"" AND T8.CANCELED = 'N'  ");
            sb.AppendLine($@"                    LEFT OUTER JOIN {DBIQ}.OPDN T11 ON T11.""DocEntry"" = T6.""TrgetEntry""  ");
            sb.AppendLine($@"                    LEFT OUTER JOIN {DBIQ}.PCH1 T9 ON T9.""BaseRef"" = CAST(T11.""DocNum"" AS VARCHAR(10)) AND T9.""BaseType"" = 20  ");
            sb.AppendLine($@"                    LEFT OUTER JOIN {DBIQ}.OPCH T10 ON T10.""DocEntry"" = T9.""DocEntry""  ");
            sb.AppendLine($@"                    INNER JOIN {DBIQ}.OITM T12 ON T6.""ItemCode"" = T12.""ItemCode""  ");
            sb.AppendLine($@"                    LEFT OUTER JOIN ( SELECT ""AbsEntry"", FIRST_VALUE(CAST(""trgtPath"" AS VARCHAR(5000)) ORDER BY ""Line"") AS ""FilePath"", STRING_AGG(CAST(""FileName"" AS VARCHAR(5000)) || '.' || CAST(""FileExt"" AS VARCHAR(5000)), '; ') AS ""FileName"" ");
            sb.AppendLine($@"                                      FROM   {DBIQ}.ATC1 s0 ");
            sb.AppendLine($@"                                      GROUP BY ""AbsEntry"" ) t13 ON T8.""AtcEntry"" = t13.""AbsEntry"" ");
            sb.AppendLine($@"                    LEFT OUTER JOIN ( SELECT ""AbsEntry"", FIRST_VALUE(CAST(""trgtPath"" AS VARCHAR(5000)) ORDER BY ""Line"") AS ""FilePath"", STRING_AGG(CAST(""FileName"" AS VARCHAR(5000)) || '.' || CAST(""FileExt"" AS VARCHAR(5000)), '; ') AS ""FileName"" ");
            sb.AppendLine($@"                                      FROM   {DBIQ}.ATC1 s0 ");
            sb.AppendLine($@"                                      GROUP BY ""AbsEntry"" ) t14 ON T10.""AtcEntry"" = t14.""AbsEntry"" ");
            sb.AppendLine($@"          WHERE     T1.CANCELED = 'N' ) AS a  ");
            if (!string.IsNullOrWhiteSpace(strFilter)) sb.AppendLine($@"WHERE   {strFilter} ");
            sb.AppendLine($@"ORDER By {GetOrder(Order)} ");

            IEnumerable<BEA.ProviderOrder> items = SQLList(sb.ToString(), Relations);
            return items;
        }

        public IEnumerable<BEA.ProviderOrderItem> ListItems(List<Field> FilterList, string Order)
        {
            string strFilter = FilterList?.Count > 0 ? GetFilter(FilterList.ToArray()) : "";
            var sb = new StringBuilder();
            sb.AppendLine($@"SELECT  *  ");
            sb.AppendLine($@"FROM    ( SELECT    DISTINCT 'Santa Cruz' AS ""Subsidiary"", t0.""DocNum"" AS ""DocNumber"", t2.""ItemCode"", t2.""Dscription"" AS ""ItemName"", t3.""SuppCatNum"" AS ""BrandCode"", CAST(t2.""Quantity"" AS INTEGER) AS ""Quantity"" ");
            sb.AppendLine($@"                    , CAST(t2.""DelivrdQty"" AS INTEGER) AS ""DeliveredQuantity"", CAST(t2.""OpenCreQty"" AS INTEGER) AS ""OpenQuantity"", t2.""PriceAfVAT"" AS ""Price"", t2.""PriceAfVAT"" * t2.""Quantity"" AS ""Subtotal""  ");
            sb.AppendLine($@"          FROM      {DBSA}.OPOR t0  ");
            sb.AppendLine($@"                    INNER JOIN {DBSA}.OSLP t1 ON t0.""SlpCode"" = t1.""SlpCode""  ");
            sb.AppendLine($@"                    INNER JOIN {DBSA}.POR1 t2 ON t0.""DocEntry"" = t2.""DocEntry""  ");
            sb.AppendLine($@"                    INNER JOIN {DBSA}.OITM t3 ON t3.""ItemCode"" = t2.""ItemCode""  ");
            sb.AppendLine($@"          WHERE     t0.CANCELED = 'N'  ");
            sb.AppendLine($@"          UNION  ");
            sb.AppendLine($@"          SELECT    DISTINCT 'Miami', t0.""DocNum"", t2.""ItemCode"", t2.""Dscription"", t3.""SuppCatNum"", CAST(t2.""Quantity"" AS INTEGER), CAST(t2.""DelivrdQty"" AS INTEGER), CAST(t2.""OpenCreQty"" AS INTEGER), t2.""PriceAfVAT"", t2.""PriceAfVAT"" * t2.""Quantity"" ");
            sb.AppendLine($@"          FROM      {DBLA}.OPOR t0  ");
            sb.AppendLine($@"                    INNER JOIN {DBLA}.OSLP t1 ON t0.""SlpCode"" = t1.""SlpCode""  ");
            sb.AppendLine($@"                    INNER JOIN {DBLA}.POR1 t2 ON t0.""DocEntry"" = t2.""DocEntry""  ");
            sb.AppendLine($@"                    INNER JOIN {DBLA}.OITM t3 ON t3.""ItemCode"" = t2.""ItemCode""  ");
            sb.AppendLine($@"          WHERE     t0.CANCELED = 'N'  ");
            sb.AppendLine($@"          UNION  ");
            sb.AppendLine($@"          SELECT    DISTINCT 'Iquique', t0.""DocNum"", t2.""ItemCode"", t2.""Dscription"", t3.""SuppCatNum"", CAST(t2.""Quantity"" AS INTEGER), CAST(t2.""DelivrdQty"" AS INTEGER), CAST(t2.""OpenCreQty"" AS INTEGER), t2.""PriceAfVAT"", t2.""PriceAfVAT"" * t2.""Quantity""  ");
            sb.AppendLine($@"          FROM      {DBIQ}.OPOR t0  ");
            sb.AppendLine($@"                    INNER JOIN {DBIQ}.OSLP t1 ON t0.""SlpCode"" = t1.""SlpCode""  ");
            sb.AppendLine($@"                    INNER JOIN {DBIQ}.POR1 t2 ON t0.""DocEntry"" = t2.""DocEntry""  ");
            sb.AppendLine($@"                    INNER JOIN {DBIQ}.OITM t3 ON t3.""ItemCode"" = t2.""ItemCode""  ");
            sb.AppendLine($@"          WHERE     t0.CANCELED = 'N' ) AS a  ");
            if (!string.IsNullOrWhiteSpace(strFilter)) sb.AppendLine($@"WHERE   {strFilter} ");
            sb.AppendLine($@"ORDER By {GetOrder(Order)} ");

            IEnumerable<BEA.ProviderOrderItem> items = base.SQLList<BEA.ProviderOrderItem>(sb.ToString());
            return items;
        }

        public IEnumerable<BEA.ProviderOrder> ListByItemRequested(string ItemCode, string Subsidiary, string WarehouseCode)
        {
            string query = $@"SELECT  *
                            FROM    ( SELECT  'Santa Cruz' AS ""Subsidiary"", t0.""DocNum"" AS ""DocNumber"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", CAST(t0.""DocDueDate"" AS DATE) AS ""EstimatedDate"", t0.""NumAtCard"" AS ""ReferenceOrder""
                                              , t0.""CardCode"" AS ""ProviderCode"", ( CASE IFNULL(t0.""CardName"", '') WHEN '' THEN t0.""CardCode"" ELSE t0.""CardName"" END ) AS ""ProviderName"", t1.""ItemCode"", t3.""WhsName"" AS ""Warehouse""
                                      FROM    {DBSA}.OPOR t0
                                              INNER JOIN {DBSA}.POR1 t1 ON t0.""DocEntry"" = t1.""DocEntry""
                                              INNER JOIN {DBSA}.OWHS t3 ON t3.""WhsCode"" = t1.""WhsCode""
                                      WHERE   t0.""DocStatus"" = 'O'
                                      UNION
                                      SELECT  'Iquique' AS ""Subsidiary"", t0.""DocNum"" AS ""DocNumber"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", CAST(t0.""DocDueDate"" AS DATE) AS ""EstimatedDate"", t0.""NumAtCard"" AS ""ReferenceOrder""
                                              , t0.""CardCode"" AS ""ProviderCode"", ( CASE IFNULL(t0.""CardName"", '') WHEN '' THEN t0.""CardCode"" ELSE t0.""CardName"" END ) AS ""ProviderName"", t1.""ItemCode"", t3.""WhsName"" AS ""Warehouse""
                                      FROM    {DBIQ}.OPOR t0
                                              INNER JOIN {DBIQ}.POR1 t1 ON t0.""DocEntry"" = t1.""DocEntry""
                                              INNER JOIN {DBIQ}.OWHS t3 ON t3.""WhsCode"" = t1.""WhsCode""
                                      WHERE   t0.""DocStatus"" = 'O'
                                      UNION
                                      SELECT  'Miami' AS ""Subsidiary"", t0.""DocNum"" AS ""DocNumber"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", CAST(t0.""DocDueDate"" AS DATE) AS ""EstimatedDate"", t0.""NumAtCard"" AS ""ReferenceOrder""
                                              , t0.""CardCode"" AS ""ProviderCode"", ( CASE IFNULL(t0.""CardName"", '') WHEN '' THEN t0.""CardCode"" ELSE t0.""CardName"" END ) AS ""ProviderName"", t1.""ItemCode"", t3.""WhsName"" AS ""Warehouse""
                                      FROM    {DBLA}.OPOR t0
                                              INNER JOIN {DBLA}.POR1 t1 ON t0.""DocEntry"" = t1.""DocEntry""
                                              INNER JOIN {DBLA}.OWHS t3 ON t3.""WhsCode"" = t1.""WhsCode""
                                      WHERE   t0.""DocStatus"" = 'O' ) a
                            WHERE   LOWER(""ItemCode"") = '{ItemCode.ToLower()}'
                                    AND LOWER(""Subsidiary"") = '{Subsidiary.ToLower()}'
                                    AND LOWER(""Warehouse"") = '{WarehouseCode.ToLower()}' ";

            IEnumerable<BEA.ProviderOrder> items = SQLList(query, Array.Empty<Enum>());
            return items;
        }

        public IEnumerable<BEA.ProviderOrderItem> ListByItemRequestedDetails(string ItemCode, string Subsidiary, string WarehouseCode)
        {
            string query = $@"SELECT  *
                            FROM    ( SELECT  'Santa Cruz' AS ""Subsidiary"", t0.""DocNum"" AS ""DocNumber"", CAST(t1.""Quantity"" AS INTEGER) AS ""Quantity"", CAST(t1.""OpenQty"" AS INTEGER) AS ""OpenQuantity""
                                              , CAST(IFNULL(t1.""DelivrdQty"", 0) AS INTEGER) AS ""DeliveredQuantity"", t1.""PriceAfVAT"" AS ""Price"", t1.""Currency"", t1.""PriceAfVAT"" * t1.""Quantity"" AS ""Subtotal""
                                              , t1.""PriceAfVAT"" * t1.""OpenQty"" AS ""OpenSubtotal"", t1.""ItemCode"", t3.""WhsName"" AS ""Warehouse""
                                      FROM    {DBSA}.OPOR t0
                                              INNER JOIN {DBSA}.POR1 t1 ON t0.""DocEntry"" = t1.""DocEntry""
                                              INNER JOIN {DBSA}.OWHS t3 ON t3.""WhsCode"" = t1.""WhsCode""
                                      WHERE   t0.""DocStatus"" = 'O'
                                      UNION
                                      SELECT  'Iquique' AS ""Subsidiary"", t0.""DocNum"" AS ""DocNumber"", CAST(t1.""Quantity"" AS INTEGER) AS ""Quantity"", CAST(t1.""OpenQty"" AS INTEGER) AS ""OpenQuantity""
                                              , CAST(IFNULL(t1.""DelivrdQty"", 0) AS INTEGER) AS ""DeliveredQuantity"", t1.""PriceAfVAT"" AS ""Price"", t1.""Currency"", t1.""PriceAfVAT"" * t1.""Quantity"" AS ""Subtotal""
                                              , t1.""PriceAfVAT"" * t1.""OpenQty"" AS ""OpenSubtotal"", t1.""ItemCode"", t3.""WhsName"" AS ""Warehouse""
                                      FROM    {DBIQ}.OPOR t0
                                              INNER JOIN {DBIQ}.POR1 t1 ON t0.""DocEntry"" = t1.""DocEntry""
                                              INNER JOIN {DBIQ}.OWHS t3 ON t3.""WhsCode"" = t1.""WhsCode""
                                      WHERE   t0.""DocStatus"" = 'O'
                                      UNION        
                                      SELECT  'Miami' AS ""Subsidiary"", t0.""DocNum"" AS ""DocNumber"", CAST(t1.""Quantity"" AS INTEGER) AS ""Quantity"", CAST(t1.""OpenQty"" AS INTEGER) AS ""OpenQuantity""
                                              , CAST(IFNULL(t1.""DelivrdQty"", 0) AS INTEGER) AS ""DeliveredQuantity"", t1.""PriceAfVAT"" AS ""Price"", t1.""Currency"", t1.""PriceAfVAT"" * t1.""Quantity"" AS ""Subtotal""
                                              , t1.""PriceAfVAT"" * t1.""OpenQty"" AS ""OpenSubtotal"", t1.""ItemCode"", t3.""WhsName"" AS ""Warehouse""
                                      FROM    {DBLA}.OPOR t0
                                              INNER JOIN {DBLA}.POR1 t1 ON t0.""DocEntry"" = t1.""DocEntry""
                                              INNER JOIN {DBLA}.OWHS t3 ON t3.""WhsCode"" = t1.""WhsCode""
                                      WHERE   t0.""DocStatus"" = 'O' ) a
                            WHERE   LOWER(""ItemCode"") = '{ItemCode.ToLower()}'      
                                    AND LOWER(""Subsidiary"") = '{Subsidiary.ToLower()}'
                                    AND LOWER(""Warehouse"") = '{WarehouseCode.ToLower()}' ";
            IEnumerable<BEA.ProviderOrderItem> items = SQLList<BEA.ProviderOrderItem>(query);
            return items;
        }

        public IEnumerable<BEA.Item> ListProviders()
        {
            var sb = new StringBuilder();
            sb.AppendLine($@"SELECT      DISTINCT * ");
            sb.AppendLine($@"FROM        ( SELECT    DISTINCT t0.""CardCode"" AS ""Code"", ( CASE IFNULL(t1.""CardName"", '') WHEN '' THEN t0.""CardCode"" ELSE t1.""CardName"" END ) AS ""Name"" ");
            sb.AppendLine($@"              FROM      {DBSA}.OPOR t0 ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.OCRD t1 ON t0.""CardCode"" = t1.""CardCode"" ");
            sb.AppendLine($@"              WHERE     t0.CANCELED = 'N' ");
            sb.AppendLine($@"              UNION ");
            sb.AppendLine($@"              SELECT    DISTINCT t0.""CardCode"", ( CASE IFNULL(t1.""CardName"", '') WHEN '' THEN t0.""CardCode"" ELSE t1.""CardName"" END ) ");
            sb.AppendLine($@"              FROM      {DBLA}.OPOR t0 ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.OCRD t1 ON t0.""CardCode"" = t1.""CardCode"" ");
            sb.AppendLine($@"              WHERE     t0.CANCELED = 'N' ");
            sb.AppendLine($@"              UNION ");
            sb.AppendLine($@"              SELECT    DISTINCT t0.""CardCode"", ( CASE IFNULL(t1.""CardName"", '') WHEN '' THEN t0.""CardCode"" ELSE t1.""CardName"" END ) ");
            sb.AppendLine($@"              FROM      {DBIQ}.OPOR t0 ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.OCRD t1 ON t0.""CardCode"" = t1.""CardCode"" ");
            sb.AppendLine($@"              WHERE     t0.CANCELED = 'N' ) a ");
            sb.AppendLine($@"ORDER BY    2 ");
            IEnumerable<BEA.Item> items = SQLList<BEA.Item>(sb.ToString());
            return items;
        }

        #endregion

        #region Search Methods

        public BEA.ProviderOrder Search(string Subsidiary, int DocNum, params Enum[] Relations)
        {
            var sb = new StringBuilder();
            sb.AppendLine($@"SELECT      * ");
            sb.AppendLine($@"FROM        ( SELECT    DISTINCT ");
            sb.AppendLine($@"                        'Santa Cruz' AS ""Subsidiary"", T1.""DocNum"", CAST(T1.""DocDate"" AS DATE) AS ""DocDate"", CAST(T1.""DocDueDate"" AS DATE) AS ""EstimatedDate"", T1.""JrnlMemo"" AS ""DailyComments"" ");
            sb.AppendLine($@"                        , ( CASE WHEN T1.""DocStatus"" = 'C' THEN 'CERRADO' ELSE 'ABIERTO' END ) AS ""State"", T1.""CardCode"" AS ""ProviderCode"" ");
            sb.AppendLine($@"                        , ( CASE IFNULL(T7.""CardName"", '') WHEN '' THEN T1.""CardCode"" ELSE T7.""CardName"" END ) AS ""ProviderName"", T1.""Comments"", T1.""Address"" AS ""BillingAddress"" ");
            sb.AppendLine($@"                        , T1.""Address2"" AS ""DeliveryAddress"", T1.""ShipToCode"", T1.""NumAtCard"" AS ""ReferenceOrder"", T2.""WhsName"" AS ""Warehouse"", T3.""SlpName"" AS ""SellerCode"" ");
            sb.AppendLine($@"                        , T4.""TrnspName"" AS ""Transport"", T5.""PymntGroup"" AS ""Terms"", T1.""TotalExpns"" AS ""OtherCosts"", T1.""DocTotal"" AS ""Total"" ");
            sb.AppendLine($@"              FROM      {DBSA}.OPOR T1 ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.OSLP T3 ON T1.""SlpCode"" = T3.""SlpCode"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBSA}.OSHP T4 ON T1.""TrnspCode"" = T4.""TrnspCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.OCTG T5 ON T1.""GroupNum"" = T5.""GroupNum"" ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.POR1 T6 ON T1.""DocEntry"" = T6.""DocEntry"" ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.OWHS T2 ON T6.""WhsCode"" = T2.""WhsCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.OCRD T7 ON T1.""CardCode"" = T7.""CardCode"" ");
            sb.AppendLine($@"              WHERE     T1.CANCELED = 'N' ");
            sb.AppendLine($@"              UNION ");
            sb.AppendLine($@"              SELECT    DISTINCT ");
            sb.AppendLine($@"                        'Miami', T1.""DocNum"", CAST(T1.""DocDate"" AS DATE), CAST(T1.""DocDueDate"" AS DATE), T1.""JrnlMemo"", ( CASE WHEN T1.""DocStatus"" = 'C' THEN 'CERRADO' ELSE 'ABIERTO' END ) ");
            sb.AppendLine($@"                        , T1.""CardCode"", ( CASE IFNULL(T7.""CardName"", '') WHEN '' THEN T1.""CardCode"" ELSE T7.""CardName"" END ), T1.""Comments"", T1.""Address"", T1.""Address2"", T1.""ShipToCode"" ");
            sb.AppendLine($@"                        , T1.""NumAtCard"", T2.""WhsName"", T3.""SlpName"", T4.""TrnspName"", T5.""PymntGroup"", T1.""TotalExpns"", T1.""DocTotal"" ");
            sb.AppendLine($@"              FROM      {DBLA}.OPOR T1 ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.OSLP T3 ON T1.""SlpCode"" = T3.""SlpCode"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBLA}.OSHP T4 ON T1.""TrnspCode"" = T4.""TrnspCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.OCTG T5 ON T1.""GroupNum"" = T5.""GroupNum"" ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.POR1 T6 ON T1.""DocEntry"" = T6.""DocEntry"" ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.OWHS T2 ON T6.""WhsCode"" = T2.""WhsCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.OCRD T7 ON T1.""CardCode"" = T7.""CardCode"" ");
            sb.AppendLine($@"              WHERE     T1.CANCELED = 'N' ");
            sb.AppendLine($@"              UNION ");
            sb.AppendLine($@"              SELECT    DISTINCT ");
            sb.AppendLine($@"                        'Iquique', T1.""DocNum"", CAST(T1.""DocDate"" AS DATE), CAST(T1.""DocDueDate"" AS DATE), T1.""JrnlMemo"", ( CASE WHEN T1.""DocStatus"" = 'C' THEN 'CERRADO' ELSE 'ABIERTO' END ) ");
            sb.AppendLine($@"                        , T1.""CardCode"", ( CASE IFNULL(T7.""CardName"", '') WHEN '' THEN T1.""CardCode"" ELSE T7.""CardName"" END ), T1.""Comments"", T1.""Address"", T1.""Address2"", T1.""ShipToCode"" ");
            sb.AppendLine($@"                        , T1.""NumAtCard"", T2.""WhsName"", T3.""SlpName"", T4.""TrnspName"", T5.""PymntGroup"", T1.""TotalExpns"", T1.""DocTotal"" ");
            sb.AppendLine($@"              FROM      {DBIQ}.OPOR T1 ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.OSLP T3 ON T1.""SlpCode"" = T3.""SlpCode"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBIQ}.OSHP T4 ON T1.""TrnspCode"" = T4.""TrnspCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.OCTG T5 ON T1.""GroupNum"" = T5.""GroupNum"" ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.POR1 T6 ON T1.""DocEntry"" = T6.""DocEntry"" ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.OWHS T2 ON T6.""WhsCode"" = T2.""WhsCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.OCRD T7 ON T1.""CardCode"" = T7.""CardCode"" ");
            sb.AppendLine($@"              WHERE     T1.CANCELED = 'N' ) A ");
            sb.AppendLine($@"WHERE       LOWER(""Subsidiary"") = '{Subsidiary.ToLower()}' AND ""DocNum"" = '{DocNum}' ");

            BEA.ProviderOrder item = SQLSearch(sb.ToString(), Relations);
            return item;
        }

        #endregion

        #region Constructors

        public ProviderOrder() : base() { }

        #endregion
    }
}
