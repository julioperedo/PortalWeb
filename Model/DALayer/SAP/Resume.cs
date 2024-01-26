using BEntities.Filters;
using DALayer;
using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE = BEntities;
using BEA = BEntities.SAP;

namespace DALayer.SAP.Hana
{
    [Serializable()]
    public class Resume : DALEntity<BEA.Resume>
    {

        #region Methods

        protected override void LoadRelations(ref IEnumerable<BEA.Resume> Items, params Enum[] Relations) { }

        protected override void LoadRelations(ref BEA.Resume item, params Enum[] Relations) { }

        #endregion

        #region List Methods

        public IEnumerable<BEA.Resume> ResumeSaleByPeriod(DateTime InitialDate, DateTime FinalDate, List<string> Lines, List<string> Sellers, params Enum[] Relations)
        {
            string initial = InitialDate.ToString("yyyy-MM-dd"), final = FinalDate.ToString("yyyy-MM-dd"), lines = string.Join(",", Lines.Select(x => $"'{x.ToLower()}'")), sellers = string.Join(",", Sellers.Select(x => $"'{x.ToLower()}'"));
            StringBuilder sb = new();
            sb.AppendLine($@"SELECT      a.""Subsidiary"", a.""Warehouse"", a.""Division"", SUM(a.""Total"") AS ""Total"", SUM(a.""TaxlessTotal"") AS ""TaxlessTotal"", SUM(a.""Margin"") AS ""Margin"" ");
            sb.AppendLine($@"FROM        ( SELECT    'Santa Cruz' AS ""Subsidiary"", t2.""WhsName"" AS ""Warehouse"" ");
            sb.AppendLine($@"                        , ( CASE WHEN LOWER(IFNULL(t3.U_LINEA, '')) IN ({lines}) THEN 'M' ELSE ( CASE WHEN LOWER(t4.""SlpName"") IN ({sellers}) THEN 'E' ELSE '' END ) END ) AS ""Division"", t1.""Quantity"" * t1.""PriceAfVAT"" AS ""Total"" ");
            sb.AppendLine($@"                        , ( CASE WHEN t1.""WhsCode"" = 'ZFSCZ' THEN t1.""Quantity"" * t1.""PriceAfVAT"" ELSE t1.""Quantity"" * 0.84 * t1.""PriceAfVAT"" END ) AS ""TaxlessTotal"" ");
            sb.AppendLine($@"                        , ( CASE WHEN t1.""WhsCode"" = 'ZFSCZ' THEN t1.""Quantity"" * ( t1.""PriceAfVAT"" - t1.""StockPrice"" ) ELSE t1.""Quantity"" * (( 0.84 * t1.""PriceAfVAT"" ) - t1.""StockPrice"" ) END ) AS ""Margin"" ");
            sb.AppendLine($@"              FROM      {DBSA}.OINV t0 ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.INV1 t1 ON t1.""DocEntry"" = t0.""DocEntry"" ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.OWHS t2 ON t2.""WhsCode"" = t1.""WhsCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.OITM t3 ON t3.""ItemCode"" = t1.""ItemCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.OSLP t4 ON t4.""SlpCode"" = t0.""SlpCode"" ");
            sb.AppendLine($@"              WHERE     t0.""CardCode"" NOT IN ('CDMC-002', 'CDMC-003', 'CIMP-008') AND  t0.U_CCFACANU = 'N' AND CAST(t0.""DocDate"" AS DATE) BETWEEN '{initial}' AND '{final}' ");
            sb.AppendLine($@"              UNION ALL ");
            sb.AppendLine($@"              SELECT    'Iquique' AS ""Subsidiary"", t2.""WhsName"" AS ""Warehouse"" ");
            sb.AppendLine($@"                        , ( CASE WHEN LOWER(IFNULL(t3.U_LINEA, '')) IN ({lines}) THEN 'M' ELSE ( CASE WHEN LOWER(t4.""SlpName"") IN ({sellers}) THEN 'E' ELSE '' END ) END ) AS ""Division"", t1.""Quantity"" * t1.""PriceAfVAT"" AS ""Total"" ");
            sb.AppendLine($@"                        , t1.""Quantity"" * t1.""PriceAfVAT"" AS ""TaxlessTotal"", t1.""Quantity"" * ( t1.""PriceAfVAT"" - t1.""StockPrice"" ) AS ""Margin"" ");
            sb.AppendLine($@"              FROM      {DBIQ}.OINV t0 ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.INV1 t1 ON t1.""DocEntry"" = t0.""DocEntry"" ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.OWHS t2 ON t2.""WhsCode"" = t1.""WhsCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.OITM t3 ON t3.""ItemCode"" = t1.""ItemCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.OSLP t4 ON t4.""SlpCode"" = t0.""SlpCode"" ");
            sb.AppendLine($@"              WHERE     t0.""CardCode"" NOT IN ('CDMC-002', 'CDMC-003', 'CIMP-008') AND  t0.U_CCFACANU = 'N' AND CAST(t0.""DocDate"" AS DATE) BETWEEN '{initial}' AND '{final}' ");
            sb.AppendLine($@"              UNION ALL ");
            sb.AppendLine($@"              SELECT    'Miami' AS ""Subsidiary"", t2.""WhsName"" AS ""Warehouse"" ");
            sb.AppendLine($@"                        , ( CASE WHEN LOWER(IFNULL(t3.U_LINEA, '')) IN ({lines}) THEN 'M' ELSE ( CASE WHEN LOWER(t4.""SlpName"") IN ({sellers}) THEN 'E' ELSE '' END ) END ) AS ""Division"", t1.""Quantity"" * t1.""PriceAfVAT"" AS ""Total"" ");
            sb.AppendLine($@"                        , t1.""Quantity"" * t1.""PriceAfVAT"" AS ""TaxlessTotal"", t1.""Quantity"" * ( t1.""PriceAfVAT"" - t1.""StockPrice"" ) AS ""Margin"" ");
            sb.AppendLine($@"              FROM      {DBLA}.OINV t0 ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.INV1 t1 ON t1.""DocEntry"" = t0.""DocEntry"" ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.OWHS t2 ON t2.""WhsCode"" = t1.""WhsCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.OITM t3 ON t3.""ItemCode"" = t1.""ItemCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.OSLP t4 ON t4.""SlpCode"" = t0.""SlpCode"" ");
            sb.AppendLine($@"              WHERE     t0.""CardCode"" NOT IN ('CDMC-002', 'CDMC-003', 'CIMP-008') AND  t0.U_CCFACANU = 'N' AND CAST(t0.""DocDate"" AS DATE) BETWEEN '{initial}' AND '{final}' ");
            sb.AppendLine($@"              UNION ALL ");
            sb.AppendLine($@"              SELECT    'Iquique' AS ""Subsidiary"", t3.""WhsName"" AS ""Warehouse"" ");
            sb.AppendLine($@"                        , ( CASE WHEN LOWER(IFNULL(t4.U_LINEA, '')) IN ({lines}) THEN 'M' ELSE ( CASE WHEN LOWER(t2.""SlpName"") IN ({sellers}) THEN 'E' ELSE '' END ) END ) AS ""Division"" ");
            sb.AppendLine($@"                        , - t1.""Quantity"" * t1.""PriceAfVAT"" AS ""Total"", - t1.""Quantity"" * t1.""PriceAfVAT"" AS ""TaxlessTotal"", - t1.""Quantity"" * ( t1.""PriceAfVAT"" - t1.""StockPrice"" ) AS ""Margin"" ");
            sb.AppendLine($@"              FROM	     {DBIQ}.ORIN t0 ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.RIN1 t1 ON t0.""DocEntry"" = t1.""DocEntry"" ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.OSLP t2 ON t0.""SlpCode"" = t2.""SlpCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.OWHS t3 ON t1.""WhsCode"" = t3.""WhsCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.OITM t4 ON t4.""ItemCode"" = t1.""ItemCode"" ");
            sb.AppendLine($@"              WHERE     t0.""CardCode"" NOT IN ('CDMC-002', 'CDMC-003', 'CIMP-008') AND CANCELED = 'N' AND t0.""DocType"" = 'I' AND CAST(t0.""DocDate"" AS DATE) BETWEEN '{initial}' AND '{final}' ");
            sb.AppendLine($@"              UNION ALL ");
            sb.AppendLine($@"              SELECT    'Miami' AS ""Subsidiary"", t3.""WhsName"" AS ""Warehouse"" ");
            sb.AppendLine($@"                        , ( CASE WHEN LOWER(IFNULL(t4.U_LINEA, '')) IN ({lines}) THEN 'M' ELSE ( CASE WHEN LOWER(t2.""SlpName"") IN ({sellers}) THEN 'E' ELSE '' END ) END ) AS ""Division"" ");
            sb.AppendLine($@"                        , - t1.""Quantity"" * t1.""PriceAfVAT"" AS ""Total"", - t1.""Quantity"" * t1.""PriceAfVAT"" AS ""TaxlessTotal"", - t1.""Quantity"" * ( t1.""PriceAfVAT"" - t1.""StockPrice"" ) AS ""Margin"" ");
            sb.AppendLine($@"              FROM	     {DBLA}.ORIN t0 ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.RIN1 t1 ON t0.""DocEntry"" = t1.""DocEntry"" ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.OSLP t2 ON t0.""SlpCode"" = t2.""SlpCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.OWHS t3 ON t1.""WhsCode"" = t3.""WhsCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.OITM t4 ON t4.""ItemCode"" = t1.""ItemCode"" ");
            sb.AppendLine($@"              WHERE     t0.""CardCode"" NOT IN ('CDMC-002', 'CDMC-003', 'CIMP-008') AND CANCELED = 'N' AND t0.""DocType"" = 'I' AND CAST(t0.""DocDate"" AS DATE) BETWEEN '{initial}' AND '{final}' ");
            sb.AppendLine($@"              UNION ALL ");
            sb.AppendLine($@"              SELECT    'Santa Cruz' AS ""Subsidiary"", t3.""WhsName"" AS ""Warehouse"" ");
            sb.AppendLine($@"                        , ( CASE WHEN LOWER(IFNULL(t4.U_LINEA, '')) IN ({lines}) THEN 'M' ELSE ( CASE WHEN LOWER(t2.""SlpName"") IN ({sellers}) THEN 'E' ELSE '' END ) END ) AS ""Division"" ");
            sb.AppendLine($@"                        , - t1.""Quantity"" * t1.""PriceAfVAT"" AS ""Total"", ( CASE WHEN t1.""WhsCode"" = 'ZFSCZ' THEN - t1.""Quantity"" * t1.""PriceAfVAT"" ELSE - t1.""Quantity"" * 0.84 * t1.""PriceAfVAT"" END ) AS ""TaxlessTotal"" ");
            sb.AppendLine($@"                        , ( CASE WHEN t1.""WhsCode"" = 'ZFSCZ' THEN - t1.""Quantity"" * ( t1.""PriceAfVAT"" - t1.""StockPrice"" ) ELSE - t1.""Quantity"" * (( 0.84 * t1.""PriceAfVAT"" ) - t1.""StockPrice"" ) END ) AS ""Margin"" ");
            sb.AppendLine($@"              FROM	     {DBSA}.ORIN t0 ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.RIN1 t1 ON t0.""DocEntry"" = t1.""DocEntry"" ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.OSLP t2 ON t0.""SlpCode"" = t2.""SlpCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.OWHS t3 ON t1.""WhsCode"" = t3.""WhsCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.OITM t4 ON t4.""ItemCode"" = t1.""ItemCode"" ");
            sb.AppendLine($@"              WHERE     t0.""CardCode"" NOT IN ('CDMC-002', 'CDMC-003', 'CIMP-008') AND CANCELED = 'N' AND t0.""DocType"" = 'I' AND CAST(t0.""DocDate"" AS DATE) BETWEEN '{initial}' AND '{final}' ) AS a ");
            sb.AppendLine($@"GROUP BY    a.""Subsidiary"", a.""Warehouse"", a.""Division"" ");

            IEnumerable<BEA.Resume> items = SQLList(sb.ToString(), Relations);
            return items;
        }

        public IEnumerable<BEA.ResumePeriod> ResumeSaleByPeriod2(DateTime InitialDate, DateTime FinalDate, List<string> Lines, List<string> Sellers)
        {
            string initial = InitialDate.ToString("yyyy-MM-dd"), final = FinalDate.ToString("yyyy-MM-dd"), lines = string.Join(",", Lines.Select(x => $"'{x.ToLower()}'")), sellers = string.Join(",", Sellers.Select(x => $"'{x.ToLower()}'"));
            string query = $@"SELECT      a.""Subsidiary"", a.""Division"", a.""Year"", a.""Month"", SUM(a.""Total"") AS ""Total""
                             FROM        ( SELECT    'Santa Cruz' AS ""Subsidiary"", t2.""WhsName"" AS ""Warehouse"" 
                                                     , ( CASE WHEN LOWER(IFNULL(t3.U_LINEA, '')) IN ({lines}) THEN 'M' ELSE ( CASE WHEN LOWER(t4.""SlpName"") IN ({sellers}) THEN 'E' ELSE '' END ) END ) AS ""Division"", t1.""Quantity"" * t1.""PriceAfVAT"" AS ""Total"" 
                                                     , ( CASE WHEN t1.""WhsCode"" = 'ZFSCZ' THEN t1.""Quantity"" * t1.""PriceAfVAT"" ELSE t1.""Quantity"" * 0.84 * t1.""PriceAfVAT"" END ) AS ""TaxlessTotal"" 
                                                     , ( CASE WHEN t1.""WhsCode"" = 'ZFSCZ' THEN t1.""Quantity"" * ( t1.""PriceAfVAT"" - t1.""StockPrice"" ) ELSE t1.""Quantity"" * (( 0.84 * t1.""PriceAfVAT"" ) - t1.""StockPrice"" ) END ) AS ""Margin"" 
                                                     , YEAR(t0.""DocDate"") AS ""Year"", MONTH(t0.""DocDate"") AS ""Month""
                                           FROM      {DBSA}.OINV t0 
                                                     INNER JOIN {DBSA}.INV1 t1 ON t1.""DocEntry"" = t0.""DocEntry"" 
                                                     INNER JOIN {DBSA}.OWHS t2 ON t2.""WhsCode"" = t1.""WhsCode"" 
                                                     INNER JOIN {DBSA}.OITM t3 ON t3.""ItemCode"" = t1.""ItemCode"" 
                                                     INNER JOIN {DBSA}.OSLP t4 ON t4.""SlpCode"" = t0.""SlpCode"" 
                                           WHERE     t0.""CardCode"" NOT IN ('CDMC-002', 'CDMC-003', 'CIMP-008') AND  t0.U_CCFACANU = 'N' AND t1.""TargetType"" <> 14 AND CAST(t0.""DocDate"" AS DATE) BETWEEN '{initial}' AND '{final}' 
                                           UNION ALL 
                                           SELECT    'Iquique' AS ""Subsidiary"", t2.""WhsName"" AS ""Warehouse"" 
                                                     , ( CASE WHEN LOWER(IFNULL(t3.U_LINEA, '')) IN ({lines}) THEN 'M' ELSE ( CASE WHEN LOWER(t4.""SlpName"") IN ({sellers}) THEN 'E' ELSE '' END ) END ) AS ""Division"", t1.""Quantity"" * t1.""PriceAfVAT"" AS ""Total"" 
                                                     , t1.""Quantity"" * t1.""PriceAfVAT"" AS ""TaxlessTotal"", t1.""Quantity"" * ( t1.""PriceAfVAT"" - t1.""StockPrice"" ) AS ""Margin"" 
                                                     , YEAR(t0.""DocDate"") AS ""Year"", MONTH(t0.""DocDate"") AS ""Month""
                                           FROM      {DBIQ}.OINV t0 
                                                     INNER JOIN {DBIQ}.INV1 t1 ON t1.""DocEntry"" = t0.""DocEntry"" 
                                                     INNER JOIN {DBIQ}.OWHS t2 ON t2.""WhsCode"" = t1.""WhsCode"" 
                                                     INNER JOIN {DBIQ}.OITM t3 ON t3.""ItemCode"" = t1.""ItemCode"" 
                                                     INNER JOIN {DBIQ}.OSLP t4 ON t4.""SlpCode"" = t0.""SlpCode"" 
                                           WHERE     t0.""CardCode"" NOT IN ('CDMC-002', 'CDMC-003', 'CIMP-008') AND  t0.U_CCFACANU = 'N' AND t1.""TargetType"" <> 14 AND CAST(t0.""DocDate"" AS DATE) BETWEEN '{initial}' AND '{final}' 
                                           UNION ALL 
                                           SELECT    'Miami' AS ""Subsidiary"", t2.""WhsName"" AS ""Warehouse"" 
                                                     , ( CASE WHEN LOWER(IFNULL(t3.U_LINEA, '')) IN ({lines}) THEN 'M' ELSE ( CASE WHEN LOWER(t4.""SlpName"") IN ({sellers}) THEN 'E' ELSE '' END ) END ) AS ""Division"", t1.""Quantity"" * t1.""PriceAfVAT"" AS ""Total"" 
                                                     , t1.""Quantity"" * t1.""PriceAfVAT"" AS ""TaxlessTotal"", t1.""Quantity"" * ( t1.""PriceAfVAT"" - t1.""StockPrice"" ) AS ""Margin"" 
                                                     , YEAR(t0.""DocDate"") AS ""Year"", MONTH(t0.""DocDate"") AS ""Month""
                                           FROM      {DBLA}.OINV t0 
                                                     INNER JOIN {DBLA}.INV1 t1 ON t1.""DocEntry"" = t0.""DocEntry"" 
                                                     INNER JOIN {DBLA}.OWHS t2 ON t2.""WhsCode"" = t1.""WhsCode"" 
                                                     INNER JOIN {DBLA}.OITM t3 ON t3.""ItemCode"" = t1.""ItemCode"" 
                                                     INNER JOIN {DBLA}.OSLP t4 ON t4.""SlpCode"" = t0.""SlpCode"" 
                                           WHERE     t0.""CardCode"" NOT IN ('CDMC-002', 'CDMC-003', 'CIMP-008') AND  t0.U_CCFACANU = 'N' AND t1.""TargetType"" <> 14 AND CAST(t0.""DocDate"" AS DATE) BETWEEN '{initial}' AND '{final}' ) AS a 
                             GROUP BY    a.""Subsidiary"", a.""Division"", a.""Year"", a.""Month"" ";

            IEnumerable<BEA.ResumePeriod> items = SQLList<BEA.ResumePeriod>(query);
            return items;
        }

        public IEnumerable<BEA.ResumeBySeller> ResumeSaleBySeller(DateTime InitialDate, DateTime FinalDate)
        {
            string initial = InitialDate.ToString("yyyy-MM-dd"), final = FinalDate.ToString("yyyy-MM-dd");
            StringBuilder sb = new();
            sb.AppendLine($@"SELECT      a.""SellerName"", SUM(a.""Total"") AS ""Total"", SUM(a.""TaxlessTotal"") AS ""TaxlessTotal"", SUM(a.""Margin"") AS ""Margin"" ");
            sb.AppendLine($@"FROM        ( SELECT    t4.""Memo"" AS ""SellerName"", t1.""Quantity"" * t1.""PriceAfVAT"" AS ""Total"" ");
            sb.AppendLine($@"                        , ( CASE WHEN t1.""WhsCode"" = 'ZFSCZ' THEN t1.""Quantity"" * t1.""PriceAfVAT"" ELSE t1.""Quantity"" * 0.84 * t1.""PriceAfVAT"" END ) AS ""TaxlessTotal"" ");
            sb.AppendLine($@"                        , ( CASE WHEN t1.""WhsCode"" = 'ZFSCZ' THEN t1.""Quantity"" * ( t1.""PriceAfVAT"" - t1.""StockPrice"" ) ELSE t1.""Quantity"" * (( 0.84 * t1.""PriceAfVAT"" ) - t1.""StockPrice"" ) END ) AS ""Margin"" ");
            sb.AppendLine($@"              FROM      {DBSA}.OINV t0 ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.INV1 t1 ON t1.""DocEntry"" = t0.""DocEntry"" ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.OWHS t2 ON t2.""WhsCode"" = t1.""WhsCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.OITM t3 ON t3.""ItemCode"" = t1.""ItemCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.OSLP t4 ON t4.""SlpCode"" = t0.""SlpCode"" ");
            sb.AppendLine($@"              WHERE     t0.""CardCode"" NOT IN ('CDMC-002', 'CDMC-003', 'CIMP-008') AND  t0.U_CCFACANU = 'N' AND t1.""TargetType"" <> 14 AND CAST(t0.""DocDate"" AS DATE) BETWEEN '{initial}' AND '{final}' ");
            sb.AppendLine($@"              UNION ALL ");
            sb.AppendLine($@"              SELECT    t4.""Memo"" AS ""SellerName"", t1.""Quantity"" * t1.""PriceAfVAT"" AS ""Total"" ");
            sb.AppendLine($@"                        , t1.""Quantity"" * t1.""PriceAfVAT"" AS ""TaxlessTotal"", t1.""Quantity"" * ( t1.""PriceAfVAT"" - t1.""StockPrice"" ) AS ""Margin"" ");
            sb.AppendLine($@"              FROM      {DBIQ}.OINV t0 ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.INV1 t1 ON t1.""DocEntry"" = t0.""DocEntry"" ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.OWHS t2 ON t2.""WhsCode"" = t1.""WhsCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.OITM t3 ON t3.""ItemCode"" = t1.""ItemCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.OSLP t4 ON t4.""SlpCode"" = t0.""SlpCode"" ");
            sb.AppendLine($@"              WHERE     t0.""CardCode"" NOT IN ('CDMC-002', 'CDMC-003', 'CIMP-008') AND  t0.U_CCFACANU = 'N' AND t1.""TargetType"" <> 14 AND CAST(t0.""DocDate"" AS DATE) BETWEEN '{initial}' AND '{final}' ");
            sb.AppendLine($@"              UNION ALL ");
            sb.AppendLine($@"              SELECT    t4.""Memo"" AS ""SellerName"", t1.""Quantity"" * t1.""PriceAfVAT"" AS ""Total"" ");
            sb.AppendLine($@"                        , t1.""Quantity"" * t1.""PriceAfVAT"" AS ""TaxlessTotal"", t1.""Quantity"" * ( t1.""PriceAfVAT"" - t1.""StockPrice"" ) AS ""Margin"" ");
            sb.AppendLine($@"              FROM      {DBLA}.OINV t0 ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.INV1 t1 ON t1.""DocEntry"" = t0.""DocEntry"" ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.OWHS t2 ON t2.""WhsCode"" = t1.""WhsCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.OITM t3 ON t3.""ItemCode"" = t1.""ItemCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.OSLP t4 ON t4.""SlpCode"" = t0.""SlpCode"" ");
            sb.AppendLine($@"              WHERE     t0.""CardCode"" NOT IN ('CDMC-002', 'CDMC-003', 'CIMP-008') AND  t0.U_CCFACANU = 'N' AND t1.""TargetType"" <> 14 AND CAST(t0.""DocDate"" AS DATE) BETWEEN '{initial}' AND '{final}' ) AS a ");
            sb.AppendLine($@"GROUP BY    a.""SellerName"" ");

            IEnumerable<BEA.ResumeBySeller> items = SQLList<BEA.ResumeBySeller>(sb.ToString());
            return items;
        }

        public IEnumerable<BEA.ResumeBySeller> ResumeSaleBySellerByMonth(DateTime? InitialDate, DateTime? FinalDate)
        {
            string strFilter = "";
            if (InitialDate.HasValue) strFilter += $@" AND CAST(T0.""DocDate"" AS DATE) >= '{InitialDate.Value:yyyy-MM-dd}'";
            if (FinalDate.HasValue) strFilter += $@" AND CAST(T0.""DocDate"" AS DATE) <= '{FinalDate.Value:yyyy-MM-dd}'";
            var sb = new StringBuilder();
            sb.AppendLine($@"SELECT  ""Seller"" AS ""SellerName"", ""Year"", ""Month"", SUM(""Total"") AS ""Total"" ");
            sb.AppendLine($@"FROM    (   SELECT  IFNULL(t1.""Memo"", 'NINGUNO') AS ""Seller"", YEAR(T0.""DocDate"") AS ""Year"", MONTH(T0.""DocDate"") AS ""Month"" ");
            sb.AppendLine($@"                    , (CASE WHEN (SELECT SUM(TA.""Quantity"") FROM {DBSA}.RIN1 TA WHERE TA.""DocEntry"" = TX.""DocEntry"" AND TA.""ItemCode"" = TX.""ItemCode"") > 0 ");
            sb.AppendLine($@"                            THEN (T2.""PriceAfVAT"" * T2.""Quantity"") - (TX.""PriceAfVAT"" * TX.""Quantity"") ");
            sb.AppendLine($@"                            ELSE (T2.""PriceAfVAT"" * T2.""Quantity"") END) AS ""Total"" ");
            sb.AppendLine($@"            FROM    {DBSA}.OINV T0 ");
            sb.AppendLine($@"                    INNER JOIN {DBSA}.INV1 T2 ON T0.""DocEntry"" = T2.""DocEntry"" ");
            sb.AppendLine($@"                    LEFT OUTER JOIN {DBSA}.RIN1 TX ON TX.""BaseRef"" = CAST(T0.""DocNum"" AS VARCHAR(20)) AND TX.""BaseType"" = 13 AND TX.""ItemCode"" = T2.""ItemCode"" ");
            sb.AppendLine($@"                    INNER JOIN {DBSA}.OITM T3 ON T2.""ItemCode"" = T3.""ItemCode"" ");
            sb.AppendLine($@"                    INNER JOIN {DBSA}.OSLP t1 ON t1.""SlpCode"" = T0.""SlpCode"" ");
            sb.AppendLine($@"            WHERE   T0.""CardCode"" NOT IN ('CDMC-002','CIMP-008') AND T0.CANCELED = 'N' {strFilter} ");
            sb.AppendLine($@"            UNION ALL ");
            sb.AppendLine($@"            SELECT  IFNULL(t1.""Memo"", 'NINGUNO') AS ""Seller"", YEAR(T0.""DocDate"") AS ""Year"", MONTH(T0.""DocDate"") AS ""Month"" ");
            sb.AppendLine($@"                    , (CASE WHEN (SELECT SUM(TA.""Quantity"") FROM {DBLA}.RIN1 TA WHERE TA.""DocEntry"" = TX.""DocEntry"" AND TA.""ItemCode"" = TX.""ItemCode"") > 0 ");
            sb.AppendLine($@"                            THEN (T2.""PriceAfVAT"" * T2.""Quantity"") - (TX.""PriceAfVAT"" * TX.""Quantity"") ELSE (T2.""PriceAfVAT"" * T2.""Quantity"") END) AS ""Total"" ");
            sb.AppendLine($@"            FROM    {DBLA}.OINV T0 ");
            sb.AppendLine($@"                    INNER JOIN {DBLA}.INV1 T2 ON T0.""DocEntry"" = T2.""DocEntry"" ");
            sb.AppendLine($@"                    LEFT OUTER JOIN {DBLA}.RIN1 TX ON TX.""BaseRef"" = CAST(T0.""DocNum"" AS VARCHAR(20)) AND TX.""BaseType"" = 13 AND TX.""ItemCode"" = T2.""ItemCode"" ");
            sb.AppendLine($@"                    INNER JOIN {DBLA}.OITM T3 ON T2.""ItemCode"" = T3.""ItemCode"" ");
            sb.AppendLine($@"                    INNER JOIN {DBLA}.OSLP t1 ON t1.""SlpCode"" = T0.""SlpCode"" ");
            sb.AppendLine($@"            WHERE   T0.""CardCode"" NOT IN ('CDMC-002','CIMP-008') AND T0.CANCELED = 'N' {strFilter} ");
            sb.AppendLine($@"            UNION ALL ");
            sb.AppendLine($@"            SELECT  IFNULL(t1.""Memo"", 'NINGUNO') AS ""Seller"", YEAR(T0.""DocDate"") AS ""Year"", MONTH(T0.""DocDate"") AS ""Month"" ");
            sb.AppendLine($@"                    , (CASE WHEN (SELECT SUM(TA.""Quantity"") FROM {DBIQ}.RIN1 TA WHERE TA.""DocEntry"" = TX.""DocEntry"" AND TA.""ItemCode"" = TX.""ItemCode"") > 0 ");
            sb.AppendLine($@"                            THEN (T2.""PriceAfVAT"" * T2.""Quantity"") - (TX.""PriceAfVAT"" * TX.""Quantity"") ");
            sb.AppendLine($@"                            ELSE (T2.""PriceAfVAT"" * T2.""Quantity"") END) AS ""Total"" ");
            sb.AppendLine($@"            FROM    {DBIQ}.OINV T0 ");
            sb.AppendLine($@"                    INNER JOIN {DBIQ}.INV1 T2 ON T0.""DocEntry"" = T2.""DocEntry"" ");
            sb.AppendLine($@"                    LEFT OUTER JOIN {DBIQ}.RIN1 TX ON TX.""BaseRef"" = CAST(T0.""DocNum"" AS VARCHAR(20)) AND TX.""BaseType"" = 13 AND TX.""ItemCode"" = T2.""ItemCode"" ");
            sb.AppendLine($@"                    INNER JOIN {DBIQ}.OITM T3 ON T2.""ItemCode"" = T3.""ItemCode"" ");
            sb.AppendLine($@"                    INNER JOIN {DBIQ}.OSLP t1 ON t1.""SlpCode"" = T0.""SlpCode"" ");
            sb.AppendLine($@"            WHERE   T0.""CardCode"" NOT IN ('CDMC-002','CIMP-008') AND T0.CANCELED = 'N' {strFilter} ");
            sb.AppendLine($@"        ) AS A ");
            sb.AppendLine($@"GROUP BY ""Seller"", ""Year"", ""Month"" ");
            sb.AppendLine($@"ORDER BY 2 DESC, 3 DESC, 4 DESC ");

            IEnumerable<BEA.ResumeBySeller> items = SQLList<BEA.ResumeBySeller>(sb.ToString());
            return items;
        }

        public IEnumerable<BEA.ResumeItem> ResumeByPeriod(List<Field> Filters, params Enum[] Relations)
        {
            string strFilter = Filters?.Count > 0 ? GetFilter(Filters.ToArray()) : "";
            var sb = new StringBuilder();
            sb.AppendLine($@"SELECT      ""Subsidiary"", ""Warehouse"", ""Year"", ""Month"", ""Day"", ""Date"", ""BillNumber"", ""ClientName"", ""ItemCode"", ""ItemName"", ""Line"", ""Category"", ""Subcategory"", ""Seller"", ""ProductManager"", SUM(""Quantity"") AS ""Quantity"", SUM(""Total"") AS ""Total"" ");
            sb.AppendLine($@"            , SUM(""Margin"") AS ""Margin"", SUM(""TaxlessTotal"") AS ""TaxlessTotal"" ");
            sb.AppendLine($@"FROM        ( SELECT    'SANTA CRUZ' AS ""Subsidiary"", T1.""WhsName"" AS ""Warehouse"", YEAR(T0.""DocDate"") AS ""Year"", MONTH(T0.""DocDate"") AS ""Month"", DAYOFMONTH(T0.""DocDate"") AS ""Day"", CAST(T0.""DocDate"" AS DATE) AS ""Date"", T0.""DocNum"" AS ""BillNumber"" ");
            sb.AppendLine($@"                        , T0.""CardCode"" AS ""CodCliente"", T0.""CardName"" AS ""ClientName"", TA.""ItemCode"", TA.""Dscription"" AS ""ItemName"", IFNULL(T2.U_LINEA, 'Ninguna') AS ""Line"", IFNULL(T2.U_CATEGORIA, 'Ninguna') AS ""Category"" ");
            sb.AppendLine($@"                        , IFNULL(T2.U_SUBCATEG, 'Ninguna') AS ""Subcategory"", IFNULL(T3.""Memo"", 'Ninguno') AS ""Seller"", IFNULL(T4.""Memo"", 'Ninguno') AS ""ProductManager"", TA.""Quantity"" AS ""Quantity"", TA.""Quantity"" * TA.""PriceAfVAT"" AS ""Total"" ");
            sb.AppendLine($@"                        , ( CASE WHEN T1.""WhsCode"" = 'ZFSCZ' THEN TA.""Quantity"" * ( TA.""PriceAfVAT"" - TA.""StockPrice"" ) ELSE TA.""Quantity"" * (( 0.84 * TA.""PriceAfVAT"" ) - TA.""StockPrice"" ) END ) AS ""Margin"" ");
            sb.AppendLine($@"                        , ( CASE WHEN T1.""WhsCode"" = 'ZFSCZ' THEN TA.""Quantity"" * TA.""PriceAfVAT"" ELSE TA.""Quantity"" * 0.84 * TA.""PriceAfVAT"" END ) AS ""TaxlessTotal"" ");
            sb.AppendLine($@"              FROM      {DBSA}.OINV T0 ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.INV1 TA ON TA.""DocEntry"" = T0.""DocEntry"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBSA}.OWHS T1 ON TA.""WhsCode"" = T1.""WhsCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.OITM T2 ON TA.""ItemCode"" = T2.""ItemCode"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBSA}.OSLP T3 ON T0.""SlpCode"" = T3.""SlpCode"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBSA}.OSLP T4 ON T2.U_CODGRT = T4.""SlpName"" ");
            sb.AppendLine($@"              WHERE     ( T0.""CardCode"" <> 'CDMC-003' AND  T0.""CardCode"" <> 'CIMP-008' ) AND   T0.U_CCFACANU = 'N' ");
            sb.AppendLine($@"              UNION ");
            sb.AppendLine($@"              SELECT    'SANTA CRUZ' AS ""Subsidiary"", T6.""WhsName"" AS ""Warehouse"", YEAR(T0.""DocDate"") AS ""Year"", MONTH(T0.""DocDate"") AS ""Month"", DAYOFMONTH(T0.""DocDate"") AS ""Day"", CAST(T0.""DocDate"" AS DATE) AS ""Date"", T0.""DocNum"" ");
            sb.AppendLine($@"                        , T0.""CardCode"" AS ""CodCliente"", T0.""CardName"" AS ""ClientName"", Ta.""ItemCode"", T2.""ItemName"" AS ""ItemName"", IFNULL(T2.U_LINEA, '') AS ""Line"", IFNULL(T2.U_CATEGORIA, '') AS ""Category"" ");
            sb.AppendLine($@"                        , IFNULL(T2.U_SUBCATEG, '') AS ""Subcategory"", T5.""Memo"" AS ""Seller"", T4.""Memo"" AS ""ProductManager"", -Ta.""InQty"" AS ""Quantity"", -T1.""PriceAfVAT"" * Ta.""InQty"" AS ""Total"" ");
            sb.AppendLine($@"                        , - ( CASE WHEN Ta.""Warehouse"" = 'ZFSCZ' THEN Ta.""InQty"" * ( T1.""PriceAfVAT"" - Ta.""CalcPrice"" ) ELSE Ta.""InQty"" * (( 0.84 * T1.""PriceAfVAT"" ) - Ta.""CalcPrice"" ) END ) AS ""Margin"" ");
            sb.AppendLine($@"                        , - ( CASE WHEN Ta.""Warehouse"" = 'ZFSCZ' THEN Ta.""InQty"" * T1.""PriceAfVAT"" ELSE Ta.""InQty"" * 0.84 * T1.""PriceAfVAT"" END ) AS ""TaxlessTotal"" ");
            sb.AppendLine($@"              FROM      {DBSA}.OINM Ta ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.ORIN T0 ON T0.""DocNum"" = Ta.BASE_REF ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.RIN1 T1 ON T1.""LineNum"" = Ta.""DocLineNum"" AND Ta.""ItemCode"" = T1.""ItemCode"" AND   T0.""DocEntry"" = T1.""DocEntry"" ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.OITM T2 ON Ta.""ItemCode"" = T2.""ItemCode"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBSA}.OSLP T4 ON T4.""SlpName"" = T2.U_CODGRT ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBSA}.OSLP T5 ON T5.""SlpCode"" = T0.""SlpCode"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBSA}.OWHS T6 ON T6.""WhsCode"" = Ta.""Warehouse"" ");
            sb.AppendLine($@"              WHERE     T0.""CardCode"" <> 'CIMP-008' AND T0.""CardCode"" <> 'CDMC-003' AND ( Ta.""TransType"" = 14 OR   Ta.""TransType"" = 16 ) ");
            sb.AppendLine($@"              UNION ");
            sb.AppendLine($@"              SELECT    'SANTA CRUZ' AS ""Subsidiary"", T6.""WhsName"", YEAR(T0.""DocDate"") AS ""Year"", MONTH(T0.""DocDate"") AS ""Month"", DAYOFMONTH(T0.""DocDate"") AS ""Day"", CAST(T0.""DocDate"" AS DATE) AS ""Date"", T0.""DocNum"", T0.""CardCode"", T0.""CardName"" ");
            sb.AppendLine($@"                        , T1.""ItemCode"", T1.""Dscription"", IFNULL(T2.U_LINEA, ''), IFNULL(T2.U_CATEGORIA, ''), IFNULL(T2.U_SUBCATEG, ''), T5.""SlpName"", T4.""Memo"", -T1.""Quantity"", -T1.""PriceAfVAT"" * T1.""Quantity"" ");
            sb.AppendLine($@"                        , - ( CASE WHEN T1.""WhsCode"" = 'ZFSCZ' THEN T1.""Quantity"" * ( T1.""PriceAfVAT"" - T1.""StockPrice"" ) ELSE T1.""Quantity"" * (( 0.84 * T1.""PriceAfVAT"" ) - T1.""StockPrice"" ) END ) AS ""Margin"" ");
            sb.AppendLine($@"                        , - ( CASE WHEN T1.""WhsCode"" = 'ZFSCZ' THEN T1.""Quantity"" * T1.""PriceAfVAT"" ELSE T1.""Quantity"" * 0.84 * T1.""PriceAfVAT"" END ) AS ""TaxlessTotal"" ");
            sb.AppendLine($@"              FROM      {DBSA}.ORIN T0 ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.RIN1 T1 ON T0.""DocEntry"" = T1.""DocEntry"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBSA}.OINV TA ON CAST(TA.""DocNum"" AS VARCHAR(10)) = T1.""BaseRef"" ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.OITM T2 ON T1.""ItemCode"" = T2.""ItemCode"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBSA}.OSLP T4 ON T4.""SlpName"" = T2.U_CODGRT ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBSA}.OSLP T5 ON T0.""SlpCode"" = T5.""SlpCode"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBSA}.OWHS T6 ON T6.""WhsCode"" = T1.""WhsCode"" ");
            sb.AppendLine($@"              WHERE     T0.""DocType"" = 'I' AND T2.""InvntItem"" = 'N' AND T0.""CardCode"" <> 'CIMP-008' AND T0.""CardCode"" <> 'CDMC-003' AND (( T1.""BaseType"" <> 13 ) OR ( T1.""BaseType"" = 13 AND   TA.""isIns"" = 'N' )) ");
            sb.AppendLine($@"              UNION ");
            sb.AppendLine($@"              SELECT    'SANTA CRUZ' AS ""Subsidiary"", T3.""WhsName"" AS ""Warehouse"", YEAR(T0.""DocDate"") AS ""Year"", MONTH(T0.""DocDate"") AS ""Month"", DAYOFMONTH(T0.""DocDate"") AS ""Day"", CAST(T0.""DocDate"" AS DATE) AS ""Date"", T0.""DocNum"" ");
            sb.AppendLine($@"                        , T0.""CardCode"" AS ""CodCliente"", T0.""CardName"" AS ""ClientName"", Ta.""ItemCode"", T2.""ItemName"" AS ""ItemName"", IFNULL(T2.U_LINEA, 'Ninguna') AS ""Line"", IFNULL(T2.U_CATEGORIA, 'Ninguna') AS ""Category"" ");
            sb.AppendLine($@"                        , IFNULL(T2.U_SUBCATEG, 'Ninguna') AS ""Subcategory"", T4.""Memo"" AS ""Seller"", T2.U_GPRODUCT AS ""ProductManager"", Ta.""InQty"" * -1 AS ""Quantity"", ( T1.""PriceAfVAT"" * Ta.""InQty"" ) * ( -1 ) AS ""Total"" ");
            sb.AppendLine($@"                        , - ( CASE WHEN T1.""WhsCode"" = 'ZFSCZ' THEN Ta.""InQty"" * ( T1.""PriceAfVAT"" - Ta.""CalcPrice"" ) ELSE Ta.""InQty"" * (( 0.84 * T1.""PriceAfVAT"" ) - Ta.""CalcPrice"" ) END ) AS ""Margin"" ");
            sb.AppendLine($@"                        , - ( CASE WHEN T1.""WhsCode"" = 'ZFSCZ' THEN Ta.""InQty"" * T1.""PriceAfVAT"" ELSE Ta.""InQty"" * 0.84 * T1.""PriceAfVAT"" END ) AS ""TaxlessTotal"" ");
            sb.AppendLine($@"              FROM      {DBSA}.OINM Ta ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.ORDN T0 ON T0.""DocNum"" = Ta.BASE_REF ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.RDN1 T1 ON T1.""LineNum"" = Ta.""DocLineNum"" AND Ta.""ItemCode"" = T1.""ItemCode"" AND   T0.""DocEntry"" = T1.""DocEntry"" ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.OITM T2 ON Ta.""ItemCode"" = T2.""ItemCode"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBSA}.OWHS T3 ON T3.""WhsCode"" = Ta.""Warehouse"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBSA}.OSLP T4 ON T4.""SlpCode"" = T0.""SlpCode"" ");
            sb.AppendLine($@"              WHERE     Ta.""TransType"" = 16 AND T0.""CardCode"" <> 'CIMP-008' AND T0.""CardCode"" <> 'CDMC-003' ");
            sb.AppendLine($@"              UNION ");
            sb.AppendLine($@"              SELECT    'SANTA CRUZ' AS ""Subsidiary"", T3.""WhsName"" AS ""Warehouse"", YEAR(T0.""DocDate"") AS ""Year"", MONTH(T0.""DocDate"") AS ""Month"", DAYOFMONTH(T0.""DocDate"") AS ""Day"", CAST(T0.""DocDate"" AS DATE) AS ""Date"", T0.""DocNum"" ");
            sb.AppendLine($@"                        , T0.""CardCode"" AS ""CodCliente"", T0.""CardName"" AS ""ClientName"", T1.""ItemCode"", T1.""Dscription"", IFNULL(T2.U_LINEA, 'Ninguna'), IFNULL(T2.U_CATEGORIA, 'Ninguna'), IFNULL(T2.U_SUBCATEG, 'Ninguna'), T4.""Memo"", T2.U_GPRODUCT ");
            sb.AppendLine($@"                        , T1.""Quantity"" * -1, T1.""PriceAfVAT"" * T1.""Quantity"" * -1 ");
            sb.AppendLine($@"                        , - ( CASE WHEN T1.""WhsCode"" = 'ZFSCZ' THEN T1.""Quantity"" * ( T1.""PriceAfVAT"" - T1.""StockPrice"" ) ELSE T1.""Quantity"" * (( 0.84 * T1.""PriceAfVAT"" ) - T1.""StockPrice"" ) END ) AS ""Margin"" ");
            sb.AppendLine($@"                        , - ( CASE WHEN T1.""WhsCode"" = 'ZFSCZ' THEN T1.""Quantity"" * T1.""PriceAfVAT"" ELSE T1.""Quantity"" * 0.84 * T1.""PriceAfVAT"" END ) AS ""TaxlessTotal"" ");
            sb.AppendLine($@"              FROM      {DBSA}.ORDN T0 ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.RDN1 T1 ON T0.""DocEntry"" = T1.""DocEntry"" ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.OITM T2 ON T1.""ItemCode"" = T2.""ItemCode"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBSA}.OWHS T3 ON T3.""WhsCode"" = T1.""WhsCode"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBSA}.OSLP T4 ON T4.""SlpCode"" = T0.""SlpCode"" ");
            sb.AppendLine($@"              WHERE     T0.""DocType"" = 'I' AND T2.""InvntItem"" = 'N' AND T0.""CardCode"" <> 'CIMP-008' AND T0.""CardCode"" <> 'CDMC-003' ");
            sb.AppendLine($@"              UNION ");
            sb.AppendLine($@"              SELECT    'MIAMI' AS ""Subsidiary"", T1.""WhsName"" AS ""Warehouse"", YEAR(T0.""DocDate"") AS ""Year"", MONTH(T0.""DocDate"") AS ""Month"", DAYOFMONTH(T0.""DocDate"") AS ""Day"", CAST(T0.""DocDate"" AS DATE) AS ""Date"", T0.""DocNum"" AS ""BillNumber"" ");
            sb.AppendLine($@"                        , T0.""CardCode"" AS ""CodCliente"", T0.""CardName"" AS ""ClientName"", TA.""ItemCode"", TA.""Dscription"", IFNULL(T2.U_LINEA, 'Ninguna'), IFNULL(T2.U_CATEGORIA, 'Ninguna'), IFNULL(T2.U_SUBCATEG, 'Ninguna') ");
            sb.AppendLine($@"                        , IFNULL(T3.""Memo"", 'Ninguno'), IFNULL(T4.""Memo"", 'Ninguno') AS ""ProductManager"", TA.""Quantity"" AS ""Quantity"", TA.""Quantity"" * TA.""PriceAfVAT"" AS ""Total"" ");
            sb.AppendLine($@"                        , ( CASE T0.""DocTotal"" WHEN 0 THEN 0 ELSE TA.""Quantity"" * ( TA.""PriceAfVAT"" - TA.""StockPrice"" ) END ) AS ""Margin"", ( CASE T0.""DocTotal"" WHEN 0 THEN 0 ELSE TA.""Quantity"" * TA.""PriceAfVAT"" END ) AS ""TaxlessTotal"" ");
            sb.AppendLine($@"              FROM      {DBLA}.OINV T0 ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.INV1 TA ON TA.""DocEntry"" = T0.""DocEntry"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBLA}.OWHS T1 ON TA.""WhsCode"" = T1.""WhsCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.OITM T2 ON TA.""ItemCode"" = T2.""ItemCode"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBLA}.OSLP T3 ON T0.""SlpCode"" = T3.""SlpCode"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBLA}.OSLP T4 ON T2.U_CODGRT = T4.""SlpName"" ");
            sb.AppendLine($@"              WHERE     ( T0.""CardCode"" <> 'CDMC-002' AND  T0.""CardCode"" <> 'CIMP-008' ) ");
            sb.AppendLine($@"              UNION ");
            sb.AppendLine($@"              SELECT    'MIAMI' AS ""Subsidiary"", T3.""WhsName"" AS ""Warehouse"", YEAR(T0.""DocDate"") AS ""Year"", MONTH(T0.""DocDate"") AS ""Month"", DAYOFMONTH(T0.""DocDate"") AS ""Day"", CAST(T0.""DocDate"" AS DATE) AS ""Date"", T0.""DocNum"" AS ""BillNumber"" ");
            sb.AppendLine($@"                        , T0.""CardCode"" AS ""CodCliente"", T0.""CardName"" AS ""ClientName"", Ta.""ItemCode"", T2.""ItemName"", T2.U_LINEA AS ""Line"", T2.U_CATEGORIA AS ""Category"", T2.U_SUBCATEG AS ""Subcategory"", T5.""Memo"" AS ""Seller"" ");
            sb.AppendLine($@"                        , T4.""Memo"" AS ""ProductManager"", -Ta.""InQty"" AS ""Quantity"", -T1.""PriceAfVAT"" * Ta.""InQty"" AS ""Total"", - ( CASE T0.""DocTotal"" WHEN 0 THEN 0 ELSE Ta.""InQty"" * ( T1.""PriceAfVAT"" - Ta.""CalcPrice"" ) END ) AS ""Margin"" ");
            sb.AppendLine($@"                        , - ( CASE T0.""DocTotal"" WHEN 0 THEN 0 ELSE Ta.""InQty"" * T1.""PriceAfVAT"" END ) AS ""TaxlessTotal"" ");
            sb.AppendLine($@"              FROM      {DBLA}.OINM Ta ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.ORIN T0 ON T0.""DocNum"" = Ta.BASE_REF ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.RIN1 T1 ON T1.""LineNum"" = Ta.""DocLineNum"" AND   Ta.""ItemCode"" = T1.""ItemCode"" AND   T0.""DocEntry"" = T1.""DocEntry"" ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.OITM T2 ON Ta.""ItemCode"" = T2.""ItemCode"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBLA}.OWHS T3 ON T3.""WhsCode"" = Ta.""Warehouse"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBLA}.OSLP T4 ON T4.""SlpName"" = T2.U_CODGRT ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBLA}.OSLP T5 ON T5.""SlpCode"" = T0.""SlpCode"" ");
            sb.AppendLine($@"              WHERE     Ta.""TransType"" = 14 AND T0.""CardCode"" <> 'CDMC-002' AND T0.""CardCode"" <> 'CIMP-008' ");
            sb.AppendLine($@"              UNION ");
            sb.AppendLine($@"              SELECT    'MIAMI' AS ""Subsidiary"", T3.""WhsName"", YEAR(T0.""DocDate"") AS ""Year"", MONTH(T0.""DocDate"") AS ""Month"", DAYOFMONTH(T0.""DocDate"") AS ""Day"", CAST(T0.""DocDate"" AS DATE), T0.""DocNum"", T0.""CardCode"" AS ""Codigo_SN"" ");
            sb.AppendLine($@"                        , T0.""CardName"" AS ""Nombre_Comercial"", T1.""ItemCode"", T1.""Dscription"", T2.U_LINEA AS ""Line"", T2.U_CATEGORIA AS ""Category"", T2.U_SUBCATEG AS ""Subcategory"", T5.""Memo"", T4.""Memo"", -T1.""Quantity"" ");
            sb.AppendLine($@"                        , -T1.""PriceAfVAT"" * T1.""Quantity"", - ( CASE T0.""DocTotal"" WHEN 0 THEN 0 ELSE T1.""Quantity"" * ( T1.""PriceAfVAT"" - T1.""StockPrice"" ) END ) AS ""Margin"" ");
            sb.AppendLine($@"                        , - ( CASE T0.""DocTotal"" WHEN 0 THEN 0 ELSE T1.""Quantity"" * T1.""PriceAfVAT"" END ) AS ""TaxlessTotal"" ");
            sb.AppendLine($@"              FROM      {DBLA}.ORIN T0 ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.RIN1 T1 ON T0.""DocEntry"" = T1.""DocEntry"" ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.OITM T2 ON T1.""ItemCode"" = T2.""ItemCode"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBLA}.OWHS T3 ON T3.""WhsCode"" = T1.""WhsCode"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBLA}.OSLP T4 ON T4.""SlpName"" = T2.U_CODGRT ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBLA}.OSLP T5 ON T0.""SlpCode"" = T5.""SlpCode"" ");
            sb.AppendLine($@"              WHERE     T0.""DocType"" = 'I' AND T2.""InvntItem"" = 'N' AND T0.""CardCode"" <> 'CDMC-002' AND T0.""CardCode"" <> 'CIMP-008' ");
            sb.AppendLine($@"              UNION ");
            sb.AppendLine($@"              SELECT    'MIAMI' AS ""Subsidiary"", T3.""WhsName"", YEAR(T0.""DocDate"") AS ""Year"", MONTH(T0.""DocDate"") AS ""Month"", DAYOFMONTH(T0.""DocDate"") AS ""Day"", CAST(T0.""DocDate"" AS DATE), T0.""DocNum"", T0.""CardCode"" AS ""Codigo_SN"" ");
            sb.AppendLine($@"                        , T0.""CardName"" AS ""Nombre_Comercial"", Ta.""ItemCode"", T2.""ItemName"", T2.U_LINEA AS ""Line"", T2.U_CATEGORIA AS ""Category"", T2.U_SUBCATEG AS ""Subcategory"", T5.""Memo"", T4.""Memo"", -Ta.""InQty"", -T1.""PriceAfVAT"" * Ta.""InQty"" ");
            sb.AppendLine($@"                        , - ( CASE T0.""DocTotal"" WHEN 0 THEN 0 ELSE Ta.""InQty"" * ( T1.""PriceAfVAT"" - Ta.""CalcPrice"" ) END ) AS ""Margin"", - ( CASE T0.""DocTotal"" WHEN 0 THEN 0 ELSE Ta.""InQty"" * T1.""PriceAfVAT"" END ) AS ""TaxlessTotal"" ");
            sb.AppendLine($@"              FROM      {DBLA}.OINM Ta ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.ORDN T0 ON T0.""DocNum"" = Ta.BASE_REF ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.RDN1 T1 ON T1.""LineNum"" = Ta.""DocLineNum"" AND   Ta.""ItemCode"" = T1.""ItemCode"" AND   T0.""DocEntry"" = T1.""DocEntry"" ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.OITM T2 ON Ta.""ItemCode"" = T2.""ItemCode"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBLA}.OWHS T3 ON T3.""WhsCode"" = Ta.""Warehouse"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBLA}.OSLP T4 ON T4.""SlpName"" = T2.U_CODGRT ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBLA}.OSLP T5 ON T5.""SlpCode"" = T0.""SlpCode"" ");
            sb.AppendLine($@"              WHERE     Ta.""TransType"" = 16 AND T0.""CardCode"" <> 'CDMC-002' AND T0.""CardCode"" <> 'CIMP-008' ");
            sb.AppendLine($@"              UNION ");
            sb.AppendLine($@"              SELECT    'IQUIQUE' AS ""Subsidiary"", T1.""WhsName"" AS ""Warehouse"", YEAR(T0.""DocDate"") AS ""Year"", MONTH(T0.""DocDate"") AS ""Month"", DAYOFMONTH(T0.""DocDate"") AS ""Day"", CAST(T0.""DocDate"" AS DATE) AS ""Date"", T0.""DocNum"" AS ""BillNumber"" ");
            sb.AppendLine($@"                        , T0.""CardCode"" AS ""CodCliente"", T0.""CardName"" AS ""ClientName"", TA.""ItemCode"", TA.""Dscription"", IFNULL(T2.U_LINEA, 'Ninguna'), IFNULL(T2.U_CATEGORIA, 'Ninguna'), IFNULL(T2.U_SUBCATEG, 'Ninguna') ");
            sb.AppendLine($@"                        , IFNULL(T3.""Memo"", 'Ninguno'), IFNULL(T4.""Memo"", 'Ninguno') AS ""ProductManager"", TA.""Quantity"" AS ""Quantity"", TA.""Quantity"" * TA.""PriceAfVAT"" AS ""Total"" ");
            sb.AppendLine($@"                        , ( CASE T0.""DocTotal"" WHEN 0 THEN 0 ELSE TA.""Quantity"" * ( TA.""PriceAfVAT"" - TA.""StockPrice"" ) END ) AS ""Margin"", ( CASE T0.""DocTotal"" WHEN 0 THEN 0 ELSE TA.""Quantity"" * TA.""PriceAfVAT"" END ) AS ""TaxlessTotal"" ");
            sb.AppendLine($@"              FROM      {DBIQ}.OINV T0 ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.INV1 TA ON TA.""DocEntry"" = T0.""DocEntry"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBIQ}.OWHS T1 ON TA.""WhsCode"" = T1.""WhsCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.OITM T2 ON TA.""ItemCode"" = T2.""ItemCode"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBIQ}.OSLP T3 ON T0.""SlpCode"" = T3.""SlpCode"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBIQ}.OSLP T4 ON T2.U_CODGRT = T4.""SlpName"" ");
            sb.AppendLine($@"              WHERE     ( T0.""CardCode"" <> 'CDMC-002' AND  T0.""CardCode"" <> 'CDMC-003' ) ");
            sb.AppendLine($@"              UNION ");
            sb.AppendLine($@"              SELECT    'IQUIQUE' AS ""Subsidiary"", T3.""WhsName"" AS ""Warehouse"", YEAR(Ta.""DocDate"") AS ""Year"", MONTH(Ta.""DocDate"") AS ""Month"", DAYOFMONTH(Ta.""DocDate"") AS ""Day"", CAST(Ta.""DocDate"" AS DATE) AS ""Date"", T0.""DocNum"" AS ""BillNumber"" ");
            sb.AppendLine($@"                        , T0.""CardCode"" AS ""CodCliente"", T0.""CardName"" AS ""ClientName"", Ta.""ItemCode"", T2.""ItemName"", T2.U_LINEA AS ""Line"", T2.U_CATEGORIA AS ""Category"", T2.U_SUBCATEG AS ""Subcategory"", T5.""Memo"" AS ""Seller"" ");
            sb.AppendLine($@"                        , T4.""Memo"" AS ""ProductManager"", -Ta.""InQty"" AS ""Quantity"", - ( T1.""PriceAfVAT"" * Ta.""InQty"" ) AS ""Total"", - ( CASE T0.""DocTotal"" WHEN 0 THEN 0 ELSE Ta.""InQty"" * ( T1.""PriceAfVAT"" - Ta.""CalcPrice"" ) END ) AS ""Margin"" ");
            sb.AppendLine($@"                        , - ( CASE T0.""DocTotal"" WHEN 0 THEN 0 ELSE Ta.""InQty"" * T1.""PriceAfVAT"" END ) AS ""TaxlessTotal"" ");
            sb.AppendLine($@"              FROM      {DBIQ}.OINM Ta ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.ORIN T0 ON T0.""DocNum"" = Ta.BASE_REF ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.OITM T2 ON Ta.""ItemCode"" = T2.""ItemCode"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBIQ}.OWHS T3 ON T3.""WhsCode"" = Ta.""Warehouse"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBIQ}.OSLP T4 ON T4.""SlpName"" = T2.U_CODGRT ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBIQ}.OSLP T5 ON T5.""SlpCode"" = T0.""SlpCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.RIN1 T1 ON T1.""LineNum"" = Ta.""DocLineNum"" AND Ta.""ItemCode"" = T1.""ItemCode"" AND  T0.""DocEntry"" = T1.""DocEntry"" ");
            sb.AppendLine($@"              WHERE     Ta.""TransType"" = 14 AND T0.""CardCode"" <> 'CDMC-002' AND T0.""CardCode"" <> 'CDMC-003' ");
            sb.AppendLine($@"              UNION ");
            sb.AppendLine($@"              SELECT    'IQUIQUE' AS ""Subsidiary"", T3.""WhsName"", YEAR(T0.""DocDate"") AS ""Year"", MONTH(T0.""DocDate"") AS ""Month"", DAYOFMONTH(T0.""DocDate"") AS ""Day"", CAST(T0.""DocDate"" AS DATE) AS ""Date"", T0.""DocNum"" AS ""No_Documento"" ");
            sb.AppendLine($@"                        , T0.""CardCode"" AS ""Codigo_SN"", T0.""CardName"" AS ""Nombre_Comercial"", T1.""ItemCode"", T1.""Dscription"", T2.U_LINEA AS ""Line"", T2.U_CATEGORIA AS ""Category"", T2.U_SUBCATEG AS ""Subcategory"", T5.""Memo"" AS ""Seller"", T4.""Memo"" ");
            sb.AppendLine($@"                        , -T1.""Quantity"" AS ""Quantity"", - ( T1.""PriceAfVAT"" * T1.""Quantity"" ) AS ""Total"", - ( CASE T0.""DocTotal"" WHEN 0 THEN 0 ELSE T1.""Quantity"" * ( T1.""PriceAfVAT"" - T1.""StockPrice"" ) END ) AS ""Margin"" ");
            sb.AppendLine($@"                        , - ( CASE T0.""DocTotal"" WHEN 0 THEN 0 ELSE T1.""Quantity"" * T1.""PriceAfVAT"" END ) AS ""TaxlessTotal"" ");
            sb.AppendLine($@"              FROM      {DBIQ}.ORIN T0 ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.RIN1 T1 ON T0.""DocEntry"" = T1.""DocEntry"" ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.OITM T2 ON T1.""ItemCode"" = T2.""ItemCode"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBIQ}.OWHS T3 ON T3.""WhsCode"" = T1.""WhsCode"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBIQ}.OSLP T4 ON T4.""SlpName"" = T2.U_CODGRT ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBIQ}.OSLP T5 ON T0.""SlpCode"" = T5.""SlpCode"" ");
            sb.AppendLine($@"              WHERE     T0.""DocType"" = 'I' AND T2.""InvntItem"" = 'N' AND T0.""CardCode"" <> 'CDMC-002' AND T0.""CardCode"" <> 'CDMC-003' ");
            sb.AppendLine($@"              UNION ");
            sb.AppendLine($@"              SELECT    'IQUIQUE' AS ""Subsidiary"", T3.""WhsName"", YEAR(T0.""DocDate"") AS ""Year"", MONTH(T0.""DocDate"") AS ""Month"", DAYOFMONTH(T0.""DocDate"") AS ""Day"", CAST(T0.""DocDate"" AS DATE) AS ""Date"", T0.""DocNum"" AS ""No_Documento"" ");
            sb.AppendLine($@"                        , T0.""CardCode"" AS ""Codigo_SN"", T0.""CardName"" AS ""Nombre_Comercial"", Ta.""ItemCode"", T2.""ItemName"", T2.U_LINEA AS ""Line"", T2.U_CATEGORIA AS ""Category"", T2.U_SUBCATEG AS ""Subcategory"", T5.""Memo"" AS ""Seller"", T4.""Memo"" ");
            sb.AppendLine($@"                        , -Ta.""InQty"" AS ""Quantity"", - ( T1.""PriceAfVAT"" * Ta.""InQty"" ) AS ""Total"", - ( CASE T0.""DocTotal"" WHEN 0 THEN 0 ELSE Ta.""InQty"" * ( T1.""PriceAfVAT"" - Ta.""CalcPrice"" ) END ) AS ""Margin"" ");
            sb.AppendLine($@"                        , - ( CASE T0.""DocTotal"" WHEN 0 THEN 0 ELSE Ta.""InQty"" * T1.""PriceAfVAT"" END ) AS ""TaxlessTotal"" ");
            sb.AppendLine($@"              FROM      {DBIQ}.OINM Ta ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.ORDN T0 ON T0.""DocNum"" = Ta.BASE_REF ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.OITM T2 ON Ta.""ItemCode"" = T2.""ItemCode"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBIQ}.OWHS T3 ON T3.""WhsCode"" = Ta.""Warehouse"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBIQ}.OSLP T4 ON T4.""SlpName"" = T2.U_CODGRT ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBIQ}.OSLP T5 ON T5.""SlpCode"" = T0.""SlpCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.RDN1 T1 ON T1.""LineNum"" = Ta.""DocLineNum"" AND Ta.""ItemCode"" = T1.""ItemCode"" AND  T0.""DocEntry"" = T1.""DocEntry"" ");
            sb.AppendLine($@"              WHERE     Ta.""TransType"" = 16 AND T0.""CardCode"" <> 'CDMC-002' AND T0.""CardCode"" <> 'CDMC-003' ) AS a ");
            if (!string.IsNullOrEmpty(strFilter)) sb.AppendLine($@"WHERE  {strFilter} ");
            sb.AppendLine($@"GROUP BY    ""Subsidiary"", ""Warehouse"", ""Year"", ""Month"", ""Date"", ""BillNumber"", ""Day"", ""ClientName"", ""ItemCode"", ""ItemName"", ""Line"", ""Category"", ""Subcategory"", ""Seller"", ""ProductManager""");

            IEnumerable<BEA.ResumeItem> items = SQLList<BEA.ResumeItem>(sb.ToString());
            return items;
        }

        public IEnumerable<BEA.Resume> ResumeStock(List<string> Lines, params Enum[] Relations)
        {
            string lines = string.Join(",", Lines.Select(x => $"'{x.ToLower()}'"));
            var sb = new StringBuilder();
            sb.AppendLine($@"SELECT      'Santa Cruz' AS ""Subsidiary"", a.""Warehouse"", a.""Division"", SUM(a.""OpenValue"") AS ""Total"" ");
            sb.AppendLine($@"FROM        ( SELECT    t3.""WhsName"" AS ""Warehouse"", ( CASE WHEN LOWER(IFNULL(t5.U_LINEA, '')) IN ({lines}) THEN 'M' ELSE '' END ) AS ""Division"", COALESCE(t2.""OpenValue"", t0.""OpenStock"") AS ""OpenValue"" ");
            sb.AppendLine($@"              FROM      {DBSA}.OIVL t0 ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.IVL1 t1 ON t0.""TransSeq"" = t1.""TransSeq"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBSA}.OIVQ t2 ON t1.""TransSeq"" = t2.""TransSeq"" AND t1.""LayerID"" = t2.""LayerID"" ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.OWHS t3 ON t3.""WhsCode"" = t0.""LocCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.OILM t4 ON t0.""MessageID"" = t4.""MessageID"" ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.OITM t5 ON t5.""ItemCode"" = t0.""ItemCode"" ");
            sb.AppendLine($@"              WHERE     ( t0.""TransType"" NOT IN (18, 19) OR ( t0.""TransType"" IN (18, 19) AND   t4.""EffectQty"" <> 0 AND ( t0.""TransType"" = 18 OR ( t0.""TransType"" = 19 AND  t4.""ActionType"" NOT IN (6, 11))) AND t4.""AccumType"" <> 3 )) ");
            sb.AppendLine($@"                        AND t0.""TransType"" NOT IN (162, 69, 163, 164) ");
            sb.AppendLine($@"                        AND   ( t0.""TransType"" <> 13 OR   t0.""TransType"" = 13 AND t4.""AccumType"" <> 4 ) ");
            sb.AppendLine($@"                        AND   COALESCE(t2.""OpenQty"", 0) > 0 ");
            sb.AppendLine($@"                        AND   COALESCE(t2.""OpenValue"", t0.""OpenStock"") > 0 ");
            sb.AppendLine($@"                        AND t0.""DocDate"" <= NOW() ");
            sb.AppendLine($@"              UNION ALL ");
            sb.AppendLine($@"              SELECT    t3.""WhsName"" AS ""Warehouse"", ( CASE WHEN LOWER(IFNULL(t5.U_LINEA, '')) IN ({lines}) THEN 'M' ELSE '' END ) AS ""Division"", COALESCE(T2.""OpenValue"", T0.""OpenStock"") AS ""OpenValue"" ");
            sb.AppendLine($@"              FROM      {DBSA}.OIVL T0 ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.IVL1 T1 ON T0.""TransSeq"" = T1.""TransSeq"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBSA}.OIVQ T2 ON T1.""TransSeq"" = T2.""TransSeq"" AND T1.""LayerID"" = T2.""LayerID"" ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.OWHS t3 ON t3.""WhsCode"" = T0.""LocCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.OILM T4 ON T0.""MessageID"" = T4.""MessageID"" ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.OITM t5 ON t5.""ItemCode"" = T0.""ItemCode"" ");
            sb.AppendLine($@"              WHERE     ( T0.""TransType"" = 18 OR   T0.""TransType"" = 19 ) ");
            sb.AppendLine($@"                        AND   T4.""EffectQty"" = 0 ");
            sb.AppendLine($@"                        AND  ( T0.""TransType"" = 18 OR ( T0.""TransType"" = 19 AND  T4.""ActionType"" <> 6 AND T4.""ActionType"" <> 11 )) ");
            sb.AppendLine($@"                        AND   T4.""AccumType"" <> 3 ");
            sb.AppendLine($@"                        AND COALESCE(T2.""OpenQty"", 0) > 0 ");
            sb.AppendLine($@"                        AND   COALESCE(T2.""OpenValue"", T0.""OpenStock"") > 0 ");
            sb.AppendLine($@"                        AND T0.""DocDate"" <= NOW() ");
            sb.AppendLine($@"              UNION ALL ");
            sb.AppendLine($@"              SELECT    t3.""WhsName"" AS ""Warehouse"", ( CASE WHEN LOWER(IFNULL(t5.U_LINEA, '')) IN ({lines}) THEN 'M' ELSE '' END ) AS ""Division"", COALESCE(T2.""OpenValue"", T0.""OpenStock"") AS ""OpenValue"" ");
            sb.AppendLine($@"              FROM      {DBSA}.OIVL T0 ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.IVL1 T1 ON T0.""TransSeq"" = T1.""TransSeq"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBSA}.OIVQ T2 ON T1.""TransSeq"" = T2.""TransSeq"" AND T1.""LayerID"" = T2.""LayerID"" ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.OWHS t3 ON t3.""WhsCode"" = T0.""LocCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.OITM t5 ON t5.""ItemCode"" = T0.""ItemCode"" ");
            sb.AppendLine($@"              WHERE     T0.""TransType"" = 162 AND   COALESCE(T2.""OpenQty"", 0) > 0 AND   COALESCE(T2.""OpenValue"", T0.""OpenStock"") > 0 AND T0.""DocDate"" <= NOW() ");
            sb.AppendLine($@"              UNION ALL ");
            sb.AppendLine($@"              SELECT    t3.""WhsName"" AS ""Warehouse"", ( CASE WHEN LOWER(IFNULL(t5.U_LINEA, '')) IN ({lines}) THEN 'M' ELSE '' END ) AS ""Division"", COALESCE(T2.""OpenValue"", T0.""OpenStock"") AS ""OpenValue"" ");
            sb.AppendLine($@"              FROM      {DBSA}.OIVL T0 ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.IVL1 T1 ON T0.""TransSeq"" = T1.""TransSeq"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBSA}.OIVQ T2 ON T1.""TransSeq"" = T2.""TransSeq"" AND T1.""LayerID"" = T2.""LayerID"" ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.OWHS t3 ON t3.""WhsCode"" = T0.""LocCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.OITM t5 ON t5.""ItemCode"" = T0.""ItemCode"" ");
            sb.AppendLine($@"              WHERE     T0.""TransType"" = 69 AND COALESCE(T2.""OpenQty"", 0) > 0 AND  COALESCE(T2.""OpenValue"", T0.""OpenStock"") > 0 AND T0.""DocDate"" <= NOW() ");
            sb.AppendLine($@"              UNION ALL ");
            sb.AppendLine($@"              SELECT    t3.""WhsName"" AS ""Warehouse"", ( CASE WHEN LOWER(IFNULL(t5.U_LINEA, '')) IN ({lines}) THEN 'M' ELSE '' END ) AS ""Division"", COALESCE(T2.""OpenValue"", T0.""OpenStock"") AS ""OpenValue"" ");
            sb.AppendLine($@"              FROM      {DBSA}.OIVL T0 ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.IVL1 T1 ON T0.""TransSeq"" = T1.""TransSeq"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBSA}.OIVQ T2 ON T1.""TransSeq"" = T2.""TransSeq"" AND T1.""LayerID"" = T2.""LayerID"" ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.OWHS t3 ON t3.""WhsCode"" = T0.""LocCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.OILM T4 ON T0.""MessageID"" = T4.""MessageID"" ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.OITM t5 ON t5.""ItemCode"" = T0.""ItemCode"" ");
            sb.AppendLine($@"              WHERE     ( T0.""TransType"" = 63 OR   T0.""TransType"" = 64 ) AND   T4.""EffectQty"" <> 0 AND COALESCE(T2.""OpenQty"", 0) > 0 AND   COALESCE(T2.""OpenValue"", T0.""OpenStock"") > 0 AND T0.""DocDate"" <= NOW() ");
            sb.AppendLine($@"              UNION ALL ");
            sb.AppendLine($@"              SELECT    t3.""WhsName"" AS ""Warehouse"", ( CASE WHEN LOWER(IFNULL(t5.U_LINEA, '')) IN ({lines}) THEN 'M' ELSE '' END ) AS ""Division"", COALESCE(T2.""OpenValue"", T0.""OpenStock"") AS ""OpenValue"" ");
            sb.AppendLine($@"              FROM      {DBSA}.OIVL T0 ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.IVL1 T1 ON T0.""TransSeq"" = T1.""TransSeq"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBSA}.OIVQ T2 ON T1.""TransSeq"" = T2.""TransSeq"" AND T1.""LayerID"" = T2.""LayerID"" ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.OWHS t3 ON t3.""WhsCode"" = T0.""LocCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.OILM T4 ON T0.""MessageID"" = T4.""MessageID"" ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.OITM t5 ON t5.""ItemCode"" = T0.""ItemCode"" ");
            sb.AppendLine($@"              WHERE     ( T0.""TransType"" = 63 OR   T0.""TransType"" = 64 ) AND   T4.""EffectQty"" = 0 AND  COALESCE(T2.""OpenQty"", 0) > 0 AND   COALESCE(T2.""OpenValue"", T0.""OpenStock"") > 0 AND T0.""DocDate"" <= NOW()) AS a ");
            sb.AppendLine($@"GROUP BY    a.""Warehouse"", a.""Division"" ");
            sb.AppendLine($@"UNION ALL ");
            sb.AppendLine($@"SELECT      'Iquique' AS ""Subsidiary"", a.""Warehouse"", a.""Division"", SUM(a.""OpenValue"") AS ""Total"" ");
            sb.AppendLine($@"FROM        ( SELECT    t3.""WhsName"" AS ""Warehouse"", ( CASE WHEN LOWER(IFNULL(t5.U_LINEA, '')) IN ({lines}) THEN 'M' ELSE '' END ) AS ""Division"", COALESCE(T2.""OpenValue"", T0.""OpenStock"") AS ""OpenValue"" ");
            sb.AppendLine($@"              FROM      {DBIQ}.OIVL T0 ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.IVL1 T1 ON T0.""TransSeq"" = T1.""TransSeq"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBIQ}.OIVQ T2 ON T1.""TransSeq"" = T2.""TransSeq"" AND T1.""LayerID"" = T2.""LayerID"" ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.OWHS t3 ON t3.""WhsCode"" = T0.""LocCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.OILM T4 ON T0.""MessageID"" = T4.""MessageID"" ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.OITM t5 ON t5.""ItemCode"" = T0.""ItemCode"" ");
            sb.AppendLine($@"              WHERE     (( T0.""TransType"" <> 18 AND T0.""TransType"" <> 19 ) ");
            sb.AppendLine($@"                         OR  (( T0.""TransType"" = 18 OR   T0.""TransType"" = 19 ) AND   T4.""EffectQty"" <> 0 AND ( T0.""TransType"" = 18 OR ( T0.""TransType"" = 19 AND  T4.""ActionType"" <> 6 AND T4.""ActionType"" <> 11 )) AND   T4.""AccumType"" <> 3 )) ");
            sb.AppendLine($@"                        AND  T0.""TransType"" <> 162 ");
            sb.AppendLine($@"                        AND   T0.""TransType"" <> 69 ");
            sb.AppendLine($@"                        AND T0.""TransType"" <> 163 ");
            sb.AppendLine($@"                        AND  T0.""TransType"" <> 164 ");
            sb.AppendLine($@"                        AND   ( T0.""TransType"" <> 13 OR   T0.""TransType"" = 13 AND T4.""AccumType"" <> 4 ) ");
            sb.AppendLine($@"                        AND   COALESCE(T2.""OpenQty"", 0) > 0 ");
            sb.AppendLine($@"                        AND   COALESCE(T2.""OpenValue"", T0.""OpenStock"") > 0 ");
            sb.AppendLine($@"                        AND T0.""DocDate"" <= NOW() ");
            sb.AppendLine($@"              UNION ALL ");
            sb.AppendLine($@"              SELECT    t3.""WhsName"" AS ""Warehouse"", ( CASE WHEN LOWER(IFNULL(t5.U_LINEA, '')) IN ({lines}) THEN 'M' ELSE '' END ) AS ""Division"", COALESCE(T2.""OpenValue"", T0.""OpenStock"") AS ""OpenValue"" ");
            sb.AppendLine($@"              FROM      {DBIQ}.OIVL T0 ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.IVL1 T1 ON T0.""TransSeq"" = T1.""TransSeq"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBIQ}.OIVQ T2 ON T1.""TransSeq"" = T2.""TransSeq"" AND T1.""LayerID"" = T2.""LayerID"" ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.OWHS t3 ON t3.""WhsCode"" = T0.""LocCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.OILM T4 ON T0.""MessageID"" = T4.""MessageID"" ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.OITM t5 ON t5.""ItemCode"" = T0.""ItemCode"" ");
            sb.AppendLine($@"              WHERE     ( T0.""TransType"" = 18 OR   T0.""TransType"" = 19 ) ");
            sb.AppendLine($@"                        AND   T4.""EffectQty"" = 0 ");
            sb.AppendLine($@"                        AND  ( T0.""TransType"" = 18 OR ( T0.""TransType"" = 19 AND  T4.""ActionType"" <> 6 AND T4.""ActionType"" <> 11 )) ");
            sb.AppendLine($@"                        AND   T4.""AccumType"" <> 3 ");
            sb.AppendLine($@"                        AND COALESCE(T2.""OpenQty"", 0) > 0 ");
            sb.AppendLine($@"                        AND   COALESCE(T2.""OpenValue"", T0.""OpenStock"") > 0 ");
            sb.AppendLine($@"                        AND T0.""DocDate"" <= NOW() ");
            sb.AppendLine($@"              UNION ALL ");
            sb.AppendLine($@"              SELECT    t3.""WhsName"" AS ""Warehouse"", ( CASE WHEN LOWER(IFNULL(t5.U_LINEA, '')) IN ({lines}) THEN 'M' ELSE '' END ) AS ""Division"", COALESCE(T2.""OpenValue"", T0.""OpenStock"") AS ""OpenValue"" ");
            sb.AppendLine($@"              FROM      {DBIQ}.OIVL T0 ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.IVL1 T1 ON T0.""TransSeq"" = T1.""TransSeq"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBIQ}.OIVQ T2 ON T1.""TransSeq"" = T2.""TransSeq"" AND T1.""LayerID"" = T2.""LayerID"" ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.OWHS t3 ON t3.""WhsCode"" = T0.""LocCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.OITM t5 ON t5.""ItemCode"" = T0.""ItemCode"" ");
            sb.AppendLine($@"              WHERE     T0.""TransType"" = 162 AND   COALESCE(T2.""OpenQty"", 0) > 0 AND   COALESCE(T2.""OpenValue"", T0.""OpenStock"") > 0 AND T0.""DocDate"" <= NOW() ");
            sb.AppendLine($@"              UNION ALL ");
            sb.AppendLine($@"              SELECT    t3.""WhsName"" AS ""Warehouse"", ( CASE WHEN LOWER(IFNULL(t5.U_LINEA, '')) IN ({lines}) THEN 'M' ELSE '' END ) AS ""Division"", COALESCE(T2.""OpenValue"", T0.""OpenStock"") AS ""OpenValue"" ");
            sb.AppendLine($@"              FROM      {DBIQ}.OIVL T0 ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.IVL1 T1 ON T0.""TransSeq"" = T1.""TransSeq"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBIQ}.OIVQ T2 ON T1.""TransSeq"" = T2.""TransSeq"" AND T1.""LayerID"" = T2.""LayerID"" ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.OWHS t3 ON t3.""WhsCode"" = T0.""LocCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.OITM t5 ON t5.""ItemCode"" = T0.""ItemCode"" ");
            sb.AppendLine($@"              WHERE     T0.""TransType"" = 69 AND COALESCE(T2.""OpenQty"", 0) > 0 AND  COALESCE(T2.""OpenValue"", T0.""OpenStock"") > 0 AND T0.""DocDate"" <= NOW() ");
            sb.AppendLine($@"              UNION ALL ");
            sb.AppendLine($@"              SELECT    t3.""WhsName"" AS ""Warehouse"", ( CASE WHEN LOWER(IFNULL(t5.U_LINEA, '')) IN ({lines}) THEN 'M' ELSE '' END ) AS ""Division"", COALESCE(T2.""OpenValue"", T0.""OpenStock"") AS ""OpenValue"" ");
            sb.AppendLine($@"              FROM      {DBIQ}.OIVL T0 ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.IVL1 T1 ON T0.""TransSeq"" = T1.""TransSeq"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBIQ}.OIVQ T2 ON T1.""TransSeq"" = T2.""TransSeq"" AND T1.""LayerID"" = T2.""LayerID"" ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.OWHS t3 ON t3.""WhsCode"" = T0.""LocCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.OILM T4 ON T0.""MessageID"" = T4.""MessageID"" ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.OITM t5 ON t5.""ItemCode"" = T0.""ItemCode"" ");
            sb.AppendLine($@"              WHERE     ( T0.""TransType"" = 63 OR   T0.""TransType"" = 64 ) AND   T4.""EffectQty"" <> 0 AND COALESCE(T2.""OpenQty"", 0) > 0 AND   COALESCE(T2.""OpenValue"", T0.""OpenStock"") > 0 AND T0.""DocDate"" <= NOW() ");
            sb.AppendLine($@"              UNION ALL ");
            sb.AppendLine($@"              SELECT    t3.""WhsName"" AS ""Warehouse"", ( CASE WHEN LOWER(IFNULL(t5.U_LINEA, '')) IN ({lines}) THEN 'M' ELSE '' END ) AS ""Division"", COALESCE(T2.""OpenValue"", T0.""OpenStock"") AS ""OpenValue"" ");
            sb.AppendLine($@"              FROM      {DBIQ}.OIVL T0 ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.IVL1 T1 ON T0.""TransSeq"" = T1.""TransSeq"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBIQ}.OIVQ T2 ON T1.""TransSeq"" = T2.""TransSeq"" AND T1.""LayerID"" = T2.""LayerID"" ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.OWHS t3 ON t3.""WhsCode"" = T0.""LocCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.OILM T4 ON T0.""MessageID"" = T4.""MessageID"" ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.OITM t5 ON t5.""ItemCode"" = T0.""ItemCode"" ");
            sb.AppendLine($@"              WHERE     ( T0.""TransType"" = 63 OR   T0.""TransType"" = 64 ) AND   T4.""EffectQty"" = 0 AND  COALESCE(T2.""OpenQty"", 0) > 0 AND   COALESCE(T2.""OpenValue"", T0.""OpenStock"") > 0 AND T0.""DocDate"" <= NOW()) AS a ");
            sb.AppendLine($@"GROUP BY    a.""Warehouse"", a.""Division"" ");
            sb.AppendLine($@"UNION ALL ");
            sb.AppendLine($@"SELECT      'Miami' AS ""Subsidiary"", a.""Warehouse"", a.""Division"", SUM(a.""OpenValue"") AS ""Total"" ");
            sb.AppendLine($@"FROM        ( SELECT    t3.""WhsName"" AS ""Warehouse"", ( CASE WHEN LOWER(IFNULL(t5.U_LINEA, '')) IN ({lines}) THEN 'M' ELSE '' END ) AS ""Division"", COALESCE(T2.""OpenValue"", T0.""OpenStock"") AS ""OpenValue"" ");
            sb.AppendLine($@"              FROM      {DBLA}.OIVL T0 ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.IVL1 T1 ON T0.""TransSeq"" = T1.""TransSeq"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBLA}.OIVQ T2 ON T1.""TransSeq"" = T2.""TransSeq"" AND   T1.""LayerID"" = T2.""LayerID"" ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.OWHS t3 ON t3.""WhsCode"" = T0.""LocCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.OILM T4 ON T0.""MessageID"" = T4.""MessageID"" ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.OITM t5 ON t5.""ItemCode"" = T0.""ItemCode"" ");
            sb.AppendLine($@"              WHERE     (( T0.""TransType"" <> 18 AND T0.""TransType"" <> 19 ) ");
            sb.AppendLine($@"                         OR  (( T0.""TransType"" = 18 OR   T0.""TransType"" = 19 ) AND   T4.""EffectQty"" <> 0 AND ( T0.""TransType"" = 18 OR ( T0.""TransType"" = 19 AND  T4.""ActionType"" <> 6 AND T4.""ActionType"" <> 11 )) AND   T4.""AccumType"" <> 3 )) ");
            sb.AppendLine($@"                        AND  T0.""TransType"" <> 162 ");
            sb.AppendLine($@"                        AND   T0.""TransType"" <> 69 ");
            sb.AppendLine($@"                        AND T0.""TransType"" <> 163 ");
            sb.AppendLine($@"                        AND  T0.""TransType"" <> 164 ");
            sb.AppendLine($@"                        AND   ( T0.""TransType"" <> 13 OR   T0.""TransType"" = 13 AND T4.""AccumType"" <> 4 ) ");
            sb.AppendLine($@"                        AND   COALESCE(T2.""OpenQty"", 0) > 0 ");
            sb.AppendLine($@"                        AND   COALESCE(T2.""OpenValue"", T0.""OpenStock"") > 0 ");
            sb.AppendLine($@"                        AND T0.""DocDate"" <= NOW() ");
            sb.AppendLine($@"              UNION ALL ");
            sb.AppendLine($@"              SELECT    t3.""WhsName"" AS ""Warehouse"", ( CASE WHEN LOWER(IFNULL(t5.U_LINEA, '')) IN ({lines}) THEN 'M' ELSE '' END ) AS ""Division"", COALESCE(T2.""OpenValue"", T0.""OpenStock"") AS ""OpenValue"" ");
            sb.AppendLine($@"              FROM      {DBLA}.OIVL T0 ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.IVL1 T1 ON T0.""TransSeq"" = T1.""TransSeq"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBLA}.OIVQ T2 ON T1.""TransSeq"" = T2.""TransSeq"" AND   T1.""LayerID"" = T2.""LayerID"" ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.OWHS t3 ON t3.""WhsCode"" = T0.""LocCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.OILM T4 ON T0.""MessageID"" = T4.""MessageID"" ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.OITM t5 ON t5.""ItemCode"" = T0.""ItemCode"" ");
            sb.AppendLine($@"              WHERE     ( T0.""TransType"" = 18 OR   T0.""TransType"" = 19 ) ");
            sb.AppendLine($@"                        AND   T4.""EffectQty"" = 0 ");
            sb.AppendLine($@"                        AND  ( T0.""TransType"" = 18 OR ( T0.""TransType"" = 19 AND  T4.""ActionType"" <> 6 AND T4.""ActionType"" <> 11 )) ");
            sb.AppendLine($@"                        AND   T4.""AccumType"" <> 3 ");
            sb.AppendLine($@"                        AND COALESCE(T2.""OpenQty"", 0) > 0 ");
            sb.AppendLine($@"                        AND   COALESCE(T2.""OpenValue"", T0.""OpenStock"") > 0 ");
            sb.AppendLine($@"                        AND T0.""DocDate"" <= NOW() ");
            sb.AppendLine($@"              UNION ALL ");
            sb.AppendLine($@"              SELECT    t3.""WhsName"" AS ""Warehouse"", ( CASE WHEN LOWER(IFNULL(t5.U_LINEA, '')) IN ({lines}) THEN 'M' ELSE '' END ) AS ""Division"", COALESCE(T2.""OpenValue"", T0.""OpenStock"") AS ""OpenValue"" ");
            sb.AppendLine($@"              FROM      {DBLA}.OIVL T0 ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.IVL1 T1 ON T0.""TransSeq"" = T1.""TransSeq"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBLA}.OIVQ T2 ON T1.""TransSeq"" = T2.""TransSeq"" AND   T1.""LayerID"" = T2.""LayerID"" ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.OWHS t3 ON t3.""WhsCode"" = T0.""LocCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.OITM t5 ON t5.""ItemCode"" = T0.""ItemCode"" ");
            sb.AppendLine($@"              WHERE     T0.""TransType"" = 162 AND   COALESCE(T2.""OpenQty"", 0) > 0 AND   COALESCE(T2.""OpenValue"", T0.""OpenStock"") > 0 AND T0.""DocDate"" <= NOW() ");
            sb.AppendLine($@"              UNION ALL ");
            sb.AppendLine($@"              SELECT    t3.""WhsName"" AS ""Warehouse"", ( CASE WHEN LOWER(IFNULL(t5.U_LINEA, '')) IN ({lines}) THEN 'M' ELSE '' END ) AS ""Division"", COALESCE(T2.""OpenValue"", T0.""OpenStock"") AS ""OpenValue"" ");
            sb.AppendLine($@"              FROM      {DBLA}.OIVL T0 ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.IVL1 T1 ON T0.""TransSeq"" = T1.""TransSeq"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBLA}.OIVQ T2 ON T1.""TransSeq"" = T2.""TransSeq"" AND   T1.""LayerID"" = T2.""LayerID"" ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.OWHS t3 ON t3.""WhsCode"" = T0.""LocCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.OITM t5 ON t5.""ItemCode"" = T0.""ItemCode"" ");
            sb.AppendLine($@"              WHERE     T0.""TransType"" = 69 AND COALESCE(T2.""OpenQty"", 0) > 0 AND  COALESCE(T2.""OpenValue"", T0.""OpenStock"") > 0 AND T0.""DocDate"" <= NOW() ");
            sb.AppendLine($@"              UNION ALL ");
            sb.AppendLine($@"              SELECT    t3.""WhsName"" AS ""Warehouse"", ( CASE WHEN LOWER(IFNULL(t5.U_LINEA, '')) IN ({lines}) THEN 'M' ELSE '' END ) AS ""Division"", COALESCE(T2.""OpenValue"", T0.""OpenStock"") AS ""OpenValue"" ");
            sb.AppendLine($@"              FROM      {DBLA}.OIVL T0 ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.IVL1 T1 ON T0.""TransSeq"" = T1.""TransSeq"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBLA}.OIVQ T2 ON T1.""TransSeq"" = T2.""TransSeq"" AND   T1.""LayerID"" = T2.""LayerID"" ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.OWHS t3 ON t3.""WhsCode"" = T0.""LocCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.OILM T4 ON T0.""MessageID"" = T4.""MessageID"" ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.OITM t5 ON t5.""ItemCode"" = T0.""ItemCode"" ");
            sb.AppendLine($@"              WHERE     ( T0.""TransType"" = 63 OR   T0.""TransType"" = 64 ) AND   T4.""EffectQty"" <> 0 AND COALESCE(T2.""OpenQty"", 0) > 0 AND   COALESCE(T2.""OpenValue"", T0.""OpenStock"") > 0 AND T0.""DocDate"" <= NOW() ");
            sb.AppendLine($@"              UNION ALL ");
            sb.AppendLine($@"              SELECT    t3.""WhsName"" AS ""Warehouse"", ( CASE WHEN LOWER(IFNULL(t5.U_LINEA, '')) IN ({lines}) THEN 'M' ELSE '' END ) AS ""Division"", COALESCE(T2.""OpenValue"", T0.""OpenStock"") AS ""OpenValue"" ");
            sb.AppendLine($@"              FROM      {DBLA}.OIVL T0 ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.IVL1 T1 ON T0.""TransSeq"" = T1.""TransSeq"" ");
            sb.AppendLine($@"                        LEFT OUTER JOIN {DBLA}.OIVQ T2 ON T1.""TransSeq"" = T2.""TransSeq"" AND   T1.""LayerID"" = T2.""LayerID"" ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.OWHS t3 ON t3.""WhsCode"" = T0.""LocCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.OILM T4 ON T0.""MessageID"" = T4.""MessageID"" ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.OITM t5 ON t5.""ItemCode"" = T0.""ItemCode"" ");
            sb.AppendLine($@"              WHERE     ( T0.""TransType"" = 63 OR   T0.""TransType"" = 64 ) AND   T4.""EffectQty"" = 0 AND  COALESCE(T2.""OpenQty"", 0) > 0 AND   COALESCE(T2.""OpenValue"", T0.""OpenStock"") > 0 AND T0.""DocDate"" <= NOW()) AS a ");
            sb.AppendLine($@"GROUP BY    a.""Warehouse"", a.""Division"" ");
            sb.AppendLine($@"ORDER BY    1, 2 ");

            IEnumerable<BEA.Resume> items = SQLList(sb.ToString(), Relations);
            return items;
        }

        public IEnumerable<BEA.Resume> ResumeStock(string Subsidiary, string Warehouse, string Division, params Enum[] Relations)
        {
            Base.DivisionConfig dalDivision = new();
            IEnumerable<BE.Base.DivisionConfig> lstConfigItems = dalDivision.List("Type, Name");
            var Lines = (from i in lstConfigItems where i.Type == "M" select i.Name).ToList();
            string db, query, lines = string.Join(",", Lines.Select(x => $"'{x.ToLower()}'")),
                filter = Division == "Mobile" ? $@"AND LOWER(""Line"") IN ( {lines} )" : "";
            db = Subsidiary.ToLower() switch { "santa cruz" => DBSA, "iquique" => DBIQ, _ => DBLA };
            query = $@"SELECT      ""Line"", SUM(a.""OpenValue"") AS ""Total"" 
FROM        ( SELECT    t3.""WhsName"" AS ""Warehouse"", t5.U_LINEA AS ""Line"", COALESCE(t2.""OpenValue"", t0.""OpenStock"") AS ""OpenValue"" 
              FROM      {db}.OIVL t0 
                        INNER JOIN {db}.IVL1 t1 ON t0.""TransSeq"" = t1.""TransSeq"" 
                        LEFT OUTER JOIN {db}.OIVQ t2 ON t1.""TransSeq"" = t2.""TransSeq"" AND t1.""LayerID"" = t2.""LayerID"" 
                        INNER JOIN {db}.OWHS t3 ON t3.""WhsCode"" = t0.""LocCode"" 
                        INNER JOIN {db}.OILM t4 ON t0.""MessageID"" = t4.""MessageID"" 
                        INNER JOIN {db}.OITM t5 ON t5.""ItemCode"" = t0.""ItemCode"" 
              WHERE     ( t0.""TransType"" NOT IN (18, 19) OR ( t0.""TransType"" IN (18, 19) AND t4.""EffectQty"" <> 0 AND ( t0.""TransType"" = 18 OR ( t0.""TransType"" = 19 AND  t4.""ActionType"" NOT IN (6, 11))) AND t4.""AccumType"" <> 3 ) ) 
                        AND t0.""TransType"" NOT IN (162, 69, 163, 164) 
                        AND ( t0.""TransType"" <> 13 OR ( t0.""TransType"" = 13 AND t4.""AccumType"" <> 4 ) ) 
                        AND COALESCE(t2.""OpenQty"", 0) > 0 
                        AND COALESCE(t2.""OpenValue"", t0.""OpenStock"") > 0 
                        AND t0.""DocDate"" <= NOW() 
              UNION ALL 
              SELECT    t3.""WhsName"" AS ""Warehouse"", t5.U_LINEA, COALESCE(T2.""OpenValue"", T0.""OpenStock"") AS ""OpenValue"" 
              FROM      {db}.OIVL T0 
                        INNER JOIN {db}.IVL1 T1 ON T0.""TransSeq"" = T1.""TransSeq"" 
                        LEFT OUTER JOIN {db}.OIVQ T2 ON T1.""TransSeq"" = T2.""TransSeq"" AND T1.""LayerID"" = T2.""LayerID"" 
                        INNER JOIN {db}.OWHS t3 ON t3.""WhsCode"" = T0.""LocCode"" 
                        INNER JOIN {db}.OILM T4 ON T0.""MessageID"" = T4.""MessageID"" 
                        INNER JOIN {db}.OITM t5 ON t5.""ItemCode"" = T0.""ItemCode"" 
              WHERE     t0.""TransType"" NOT IN (18, 19)
                        AND T4.""EffectQty"" = 0 
                        AND ( T0.""TransType"" = 18 OR ( T0.""TransType"" = 19 AND  T4.""ActionType"" <> 6 AND T4.""ActionType"" <> 11 )) 
                        AND T4.""AccumType"" <> 3 
                        AND COALESCE(T2.""OpenQty"", 0) > 0 
                        AND COALESCE(T2.""OpenValue"", T0.""OpenStock"") > 0 
                        AND T0.""DocDate"" <= NOW() 
              UNION ALL 
              SELECT    t3.""WhsName"" AS ""Warehouse"", t5.U_LINEA, COALESCE(T2.""OpenValue"", T0.""OpenStock"") AS ""OpenValue"" 
              FROM      {db}.OIVL T0 
                        INNER JOIN {db}.IVL1 T1 ON T0.""TransSeq"" = T1.""TransSeq"" 
                        LEFT OUTER JOIN {db}.OIVQ T2 ON T1.""TransSeq"" = T2.""TransSeq"" AND T1.""LayerID"" = T2.""LayerID"" 
                        INNER JOIN {db}.OWHS t3 ON t3.""WhsCode"" = T0.""LocCode"" 
                        INNER JOIN {db}.OITM t5 ON t5.""ItemCode"" = T0.""ItemCode"" 
              WHERE     T0.""TransType"" = 162 AND COALESCE(T2.""OpenQty"", 0) > 0 AND COALESCE(T2.""OpenValue"", T0.""OpenStock"") > 0 AND T0.""DocDate"" <= NOW() 
              UNION ALL 
              SELECT    t3.""WhsName"" AS ""Warehouse"", t5.U_LINEA, COALESCE(T2.""OpenValue"", T0.""OpenStock"") AS ""OpenValue"" 
              FROM      {db}.OIVL T0 
                        INNER JOIN {db}.IVL1 T1 ON T0.""TransSeq"" = T1.""TransSeq"" 
                        LEFT OUTER JOIN {db}.OIVQ T2 ON T1.""TransSeq"" = T2.""TransSeq"" AND T1.""LayerID"" = T2.""LayerID"" 
                        INNER JOIN {db}.OWHS t3 ON t3.""WhsCode"" = T0.""LocCode"" 
                        INNER JOIN {db}.OITM t5 ON t5.""ItemCode"" = T0.""ItemCode"" 
              WHERE     T0.""TransType"" = 69 AND COALESCE(T2.""OpenQty"", 0) > 0 AND  COALESCE(T2.""OpenValue"", T0.""OpenStock"") > 0 AND T0.""DocDate"" <= NOW() 
              UNION ALL 
              SELECT    t3.""WhsName"" AS ""Warehouse"", t5.U_LINEA, COALESCE(T2.""OpenValue"", T0.""OpenStock"") AS ""OpenValue"" 
              FROM      {db}.OIVL T0 
                        INNER JOIN {db}.IVL1 T1 ON T0.""TransSeq"" = T1.""TransSeq"" 
                        LEFT OUTER JOIN {db}.OIVQ T2 ON T1.""TransSeq"" = T2.""TransSeq"" AND T1.""LayerID"" = T2.""LayerID"" 
                        INNER JOIN {db}.OWHS t3 ON t3.""WhsCode"" = T0.""LocCode"" 
                        INNER JOIN {db}.OILM T4 ON T0.""MessageID"" = T4.""MessageID"" 
                        INNER JOIN {db}.OITM t5 ON t5.""ItemCode"" = T0.""ItemCode"" 
              WHERE     ( T0.""TransType"" = 63 OR   T0.""TransType"" = 64 ) AND   T4.""EffectQty"" <> 0 AND COALESCE(T2.""OpenQty"", 0) > 0 AND   COALESCE(T2.""OpenValue"", T0.""OpenStock"") > 0 AND T0.""DocDate"" <= NOW() 
              UNION ALL 
              SELECT    t3.""WhsName"" AS ""Warehouse"", t5.U_LINEA, COALESCE(T2.""OpenValue"", T0.""OpenStock"") AS ""OpenValue"" 
              FROM      {db}.OIVL T0 
                        INNER JOIN {db}.IVL1 T1 ON T0.""TransSeq"" = T1.""TransSeq"" 
                        LEFT OUTER JOIN {db}.OIVQ T2 ON T1.""TransSeq"" = T2.""TransSeq"" AND T1.""LayerID"" = T2.""LayerID"" 
                        INNER JOIN {db}.OWHS t3 ON t3.""WhsCode"" = T0.""LocCode"" 
                        INNER JOIN {db}.OILM T4 ON T0.""MessageID"" = T4.""MessageID"" 
                        INNER JOIN {db}.OITM t5 ON t5.""ItemCode"" = T0.""ItemCode"" 
              WHERE     ( T0.""TransType"" = 63 OR T0.""TransType"" = 64 ) AND T4.""EffectQty"" = 0 AND  COALESCE(T2.""OpenQty"", 0) > 0 AND COALESCE(T2.""OpenValue"", T0.""OpenStock"") > 0 AND T0.""DocDate"" <= NOW() ) AS a 
WHERE       LOWER(""Warehouse"") = '{Warehouse.ToLower()}' {filter}             
GROUP BY    ""Line""
ORDER BY 2 DESC ";
            IEnumerable<BEA.Resume> items = SQLList(query, Relations);
            return items;
        }


        public IEnumerable<BEA.Resume> AuthorizedOrders(List<string> Lines, List<string> Sellers, params Enum[] Relations)
        {
            StringBuilder sb = new();
            string lines = string.Join(",", Lines.Select(x => $"'{x.ToLower()}'")), sellers = string.Join(",", Sellers.Select(x => $"'{x.ToLower()}'"));
            sb.AppendLine($@"SELECT      'Santa Cruz' AS ""Subsidiary"", t2.""WhsName"" AS ""Warehouse"", ( CASE WHEN LOWER(IFNULL(t3.U_LINEA, '')) IN ({lines}) THEN 'M' ELSE ( CASE WHEN LOWER(t4.""SlpName"") IN ({sellers}) THEN 'E' ELSE '' END ) END ) AS ""Division"", SUM(IFNULL(t1.""PriceAfVAT"" * t1.""OpenQty"", 0)) AS ""Total"" ");
            sb.AppendLine($@"FROM        {DBSA}.ORDR t0 ");
            sb.AppendLine($@"            INNER JOIN {DBSA}.RDR1 t1 ON t1.""DocEntry"" = t0.""DocEntry"" ");
            sb.AppendLine($@"            INNER JOIN {DBSA}.OWHS t2 ON t2.""WhsCode"" = t1.""WhsCode"" ");
            sb.AppendLine($@"            INNER JOIN {DBSA}.OITM t3 ON t3.""ItemCode"" = t1.""ItemCode"" ");
            sb.AppendLine($@"            INNER JOIN {DBSA}.OSLP t4 ON t4.""SlpCode"" = t0.""SlpCode"" ");
            sb.AppendLine($@"WHERE       t0.""DocStatus"" = 'O' AND t0.""Confirmed"" = 'Y' AND t1.""OpenQty"" > 0 AND t0.""CardCode"" NOT IN ('CDMC-002', 'CDMC-003', 'CIMP-008') AND  IFNULL(t0.U_CORRELATIVO, '') NOT LIKE '%NO PROCESAR%' AND t0.CANCELED = 'N' ");
            sb.AppendLine($@"GROUP BY    t2.""WhsName"", ( CASE WHEN LOWER(IFNULL(t3.U_LINEA, '')) IN ({lines}) THEN 'M' ELSE ( CASE WHEN LOWER(t4.""SlpName"") IN ({sellers}) THEN 'E' ELSE '' END ) END ) ");
            sb.AppendLine($@"UNION ALL ");
            sb.AppendLine($@"SELECT      'Iquique' AS ""Subsidiary"", t2.""WhsName"" AS ""Warehouse"", ( CASE WHEN LOWER(IFNULL(t3.U_LINEA, '')) IN ({lines}) THEN 'M' ELSE ( CASE WHEN LOWER(t4.""SlpName"") IN ({sellers}) THEN 'E' ELSE '' END ) END ) AS ""Division"", SUM(IFNULL(t1.""PriceAfVAT"" * t1.""OpenQty"", 0)) AS ""Total"" ");
            sb.AppendLine($@"FROM        {DBIQ}.ORDR t0 ");
            sb.AppendLine($@"            INNER JOIN {DBIQ}.RDR1 t1 ON t1.""DocEntry"" = t0.""DocEntry"" ");
            sb.AppendLine($@"            INNER JOIN {DBIQ}.OWHS t2 ON t2.""WhsCode"" = t1.""WhsCode"" ");
            sb.AppendLine($@"            INNER JOIN {DBIQ}.OITM t3 ON t3.""ItemCode"" = t1.""ItemCode"" ");
            sb.AppendLine($@"            INNER JOIN {DBIQ}.OSLP t4 ON t4.""SlpCode"" = t0.""SlpCode"" ");
            sb.AppendLine($@"WHERE       t0.""DocStatus"" = 'O' AND t0.""Confirmed"" = 'Y' AND   t1.""OpenQty"" > 0 AND t0.""CardCode"" NOT IN ('CDMC-002', 'CDMC-003', 'CIMP-008') AND  IFNULL(t0.U_CORRELATIVO, '') NOT LIKE '%NO PROCESAR%' AND t0.CANCELED = 'N' ");
            sb.AppendLine($@"GROUP BY    t2.""WhsName"", ( CASE WHEN LOWER(IFNULL(t3.U_LINEA, '')) IN ({lines}) THEN 'M' ELSE ( CASE WHEN LOWER(t4.""SlpName"") IN ({sellers}) THEN 'E' ELSE '' END ) END ) ");
            sb.AppendLine($@"UNION ALL ");
            sb.AppendLine($@"SELECT      'Miami' AS ""Subsidiary"", t2.""WhsName"" AS ""Warehouse"", ( CASE WHEN LOWER(IFNULL(t3.U_LINEA, '')) IN ({lines}) THEN 'M' ELSE ( CASE WHEN LOWER(t4.""SlpName"") IN ({sellers}) THEN 'E' ELSE '' END ) END ) AS ""Division"", SUM(IFNULL(t1.""PriceAfVAT"" * t1.""OpenQty"", 0)) AS ""Total"" ");
            sb.AppendLine($@"FROM        {DBLA}.ORDR t0 ");
            sb.AppendLine($@"            INNER JOIN {DBLA}.RDR1 t1 ON t1.""DocEntry"" = t0.""DocEntry"" ");
            sb.AppendLine($@"            INNER JOIN {DBLA}.OWHS t2 ON t2.""WhsCode"" = t1.""WhsCode"" ");
            sb.AppendLine($@"            INNER JOIN {DBLA}.OITM t3 ON t3.""ItemCode"" = t1.""ItemCode"" ");
            sb.AppendLine($@"            INNER JOIN {DBLA}.OSLP t4 ON t4.""SlpCode"" = t0.""SlpCode"" ");
            sb.AppendLine($@"WHERE       t0.""DocStatus"" = 'O' AND t0.""Confirmed"" = 'Y' AND   t1.""OpenQty"" > 0 AND t0.""CardCode"" NOT IN ('CDMC-002', 'CDMC-003', 'CIMP-008') AND  IFNULL(t0.U_CORRELATIVO, '') NOT LIKE '%NO PROCESAR%' AND t0.CANCELED = 'N' ");
            sb.AppendLine($@"GROUP BY    t2.""WhsName"", ( CASE WHEN LOWER(IFNULL(t3.U_LINEA, '')) IN ({lines}) THEN 'M' ELSE ( CASE WHEN LOWER(t4.""SlpName"") IN ({sellers}) THEN 'E' ELSE '' END ) END ) ");

            IEnumerable<BEA.Resume> items = SQLList(sb.ToString(), Relations);
            return items;
        }

        public IEnumerable<BEA.Resume> OpenAmounts(List<string> Lines, List<string> Sellers, params Enum[] Relations)
        {
            StringBuilder sb = new();
            string lines = string.Join(",", Lines.Select(x => $"'{x.ToLower()}'")), sellers = string.Join(",", Sellers.Select(x => $"'{x.ToLower()}'"));
            sb.AppendLine($@"SELECT      'Santa Cruz' AS ""Subsidiary"", t2.""WhsName"" AS ""Warehouse"", ( CASE WHEN LOWER(IFNULL(t3.U_LINEA, '')) IN ({lines}) THEN 'M' ELSE ( CASE WHEN LOWER(t4.""SlpName"") IN ({sellers}) THEN 'E' ELSE '' END ) END ) AS ""Division"" ");
            sb.AppendLine($@"            , SUM(IFNULL(t1.""PriceAfVAT"" * t1.""OpenQty"", 0)) AS ""Total"" ");
            sb.AppendLine($@"FROM        {DBSA}.ORDR t0 ");
            sb.AppendLine($@"            INNER JOIN {DBSA}.RDR1 t1 ON t1.""DocEntry"" = t0.""DocEntry"" ");
            sb.AppendLine($@"            INNER JOIN {DBSA}.OWHS t2 ON t2.""WhsCode"" = t1.""WhsCode"" ");
            sb.AppendLine($@"            INNER JOIN {DBSA}.OITM t3 ON t3.""ItemCode"" = t1.""ItemCode"" ");
            sb.AppendLine($@"            INNER JOIN {DBSA}.OSLP t4 ON t4.""SlpCode"" = t0.""SlpCode"" ");
            sb.AppendLine($@"WHERE       t1.""OpenQty"" > 0 AND t0.""CardCode"" NOT IN ('CDMC-002', 'CDMC-003', 'CIMP-008') AND t0.CANCELED = 'N' ");
            sb.AppendLine($@"GROUP BY    t2.""WhsName"", ( CASE WHEN LOWER(IFNULL(t3.U_LINEA, '')) IN ({lines}) THEN 'M' ELSE ( CASE WHEN LOWER(t4.""SlpName"") IN ({sellers}) THEN 'E' ELSE '' END ) END ) ");
            sb.AppendLine($@"UNION ALL ");
            sb.AppendLine($@"SELECT      'Iquique' AS ""Subsidiary"", t2.""WhsName"" AS ""Warehouse"", ( CASE WHEN LOWER(IFNULL(t3.U_LINEA, '')) IN ({lines}) THEN 'M' ELSE ( CASE WHEN LOWER(t4.""SlpName"") IN ({sellers}) THEN 'E' ELSE '' END ) END ) AS ""Division"" ");
            sb.AppendLine($@"            , SUM(IFNULL(t1.""PriceAfVAT"" * t1.""OpenQty"", 0)) AS ""Total"" ");
            sb.AppendLine($@"FROM        {DBIQ}.ORDR t0 ");
            sb.AppendLine($@"            INNER JOIN {DBIQ}.RDR1 t1 ON t1.""DocEntry"" = t0.""DocEntry"" ");
            sb.AppendLine($@"            INNER JOIN {DBIQ}.OWHS t2 ON t2.""WhsCode"" = t1.""WhsCode"" ");
            sb.AppendLine($@"            INNER JOIN {DBIQ}.OITM t3 ON t3.""ItemCode"" = t1.""ItemCode"" ");
            sb.AppendLine($@"            INNER JOIN {DBIQ}.OSLP t4 ON t4.""SlpCode"" = t0.""SlpCode"" ");
            sb.AppendLine($@"WHERE       t1.""OpenQty"" > 0 AND t0.""CardCode"" NOT IN ('CDMC-002', 'CDMC-003', 'CIMP-008') AND t0.CANCELED = 'N' ");
            sb.AppendLine($@"GROUP BY    t2.""WhsName"", ( CASE WHEN LOWER(IFNULL(t3.U_LINEA, '')) IN ({lines}) THEN 'M' ELSE ( CASE WHEN LOWER(t4.""SlpName"") IN ({sellers}) THEN 'E' ELSE '' END ) END ) ");
            sb.AppendLine($@"UNION ALL ");
            sb.AppendLine($@"SELECT      'Miami' AS ""Subsidiary"", t2.""WhsName"" AS ""Warehouse"", ( CASE WHEN LOWER(IFNULL(t3.U_LINEA, '')) IN ({lines}) THEN 'M' ELSE ( CASE WHEN LOWER(t4.""SlpName"") IN ({sellers}) THEN 'E' ELSE '' END ) END ) AS ""Division"" ");
            sb.AppendLine($@"            , SUM(IFNULL(t1.""PriceAfVAT"" * t1.""OpenQty"", 0)) AS ""Total"" ");
            sb.AppendLine($@"FROM        {DBLA}.ORDR t0 ");
            sb.AppendLine($@"            INNER JOIN {DBLA}.RDR1 t1 ON t1.""DocEntry"" = t0.""DocEntry"" ");
            sb.AppendLine($@"            INNER JOIN {DBLA}.OWHS t2 ON t2.""WhsCode"" = t1.""WhsCode"" ");
            sb.AppendLine($@"            INNER JOIN {DBLA}.OITM t3 ON t3.""ItemCode"" = t1.""ItemCode"" ");
            sb.AppendLine($@"            INNER JOIN {DBLA}.OSLP t4 ON t4.""SlpCode"" = t0.""SlpCode"" ");
            sb.AppendLine($@"WHERE       t1.""OpenQty"" > 0 AND t0.""CardCode"" NOT IN ('CDMC-002', 'CDMC-003', 'CIMP-008') AND t0.CANCELED = 'N' ");
            sb.AppendLine($@"GROUP BY    t2.""WhsName"", ( CASE WHEN LOWER(IFNULL(t3.U_LINEA, '')) IN ({lines}) THEN 'M' ELSE ( CASE WHEN LOWER(t4.""SlpName"") IN ({sellers}) THEN 'E' ELSE '' END ) END ) ");

            IEnumerable<BEA.Resume> items = SQLList(sb.ToString(), Relations);
            return items;
        }

        public IEnumerable<BEA.ResumeByClient> ResumeClientByPeriod(DateTime InitialDate, DateTime FinalDate, List<string> Lines, List<string> Sellers)
        {
            string initial = InitialDate.ToString("yyyy-MM-dd"), final = FinalDate.ToString("yyyy-MM-dd"), lines = string.Join(",", Lines.Select(x => $"'{x.ToLower()}'")),
                sellers = string.Join(",", Sellers.Select(x => $"'{x.ToLower()}'"));
            StringBuilder sb = new();
            sb.AppendLine($@"SELECT      a.""Subsidiary"", a.""CardCode"", b.""CardName"", a.""Division"", SUM(a.""Total"") AS ""Total"", SUM(a.""TaxlessTotal"") AS ""TaxlessTotal"", SUM(a.""Margin"") AS ""Margin"" ");
            sb.AppendLine($@"FROM        ( SELECT    'Santa Cruz' AS ""Subsidiary"", t0.""CardCode"", t1.""Quantity"" * t1.""PriceAfVAT"" AS ""Total"" ");
            sb.AppendLine($@"                        , ( CASE t1.""WhsCode"" WHEN 'ZFSCZ' THEN t1.""Quantity"" * t1.""PriceAfVAT"" ELSE t1.""Quantity"" * t1.""PriceAfVAT"" * 0.84 END ) AS ""TaxlessTotal"" ");
            sb.AppendLine($@"                        , ( CASE t1.""WhsCode"" WHEN 'ZFSCZ' THEN t1.""Quantity"" * ( t1.""PriceAfVAT"" - t1.""StockPrice"" ) ELSE t1.""Quantity"" * (( t1.""PriceAfVAT"" * 0.84 ) - t1.""StockPrice"" ) END ) AS ""Margin"" ");
            sb.AppendLine($@"                        , ( CASE WHEN LOWER(IFNULL(t4.U_LINEA, '')) IN ({lines}) THEN 'M' ELSE ( CASE WHEN LOWER(t3.""SlpName"") IN ({sellers}) THEN 'E' ELSE '' END ) END ) AS ""Division"" ");
            sb.AppendLine($@"              FROM      {DBSA}.OINV t0 ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.INV1 t1 ON t1.""DocEntry"" = t0.""DocEntry"" ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.OSLP t3 ON t3.""SlpCode"" = t0.""SlpCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.OITM t4 ON t4.""ItemCode"" = t1.""ItemCode"" ");
            sb.AppendLine($@"              WHERE     t0.""CardCode"" NOT IN ('CDMC-002', 'CDMC-003', 'CIMP-008') AND  t0.U_CCFACANU = 'N' AND t1.""TargetType"" <> 14 AND CAST(t0.""DocDate"" AS DATE) >= '{initial}' AND  CAST(t0.""DocDate"" AS DATE) <= '{final}' ) AS a ");
            sb.AppendLine($@"            INNER JOIN {DBSA}.OCRD b ON b.""CardCode"" = a.""CardCode"" ");
            sb.AppendLine($@"GROUP BY    a.""Subsidiary"", a.""CardCode"", b.""CardName"", a.""Division"" ");
            sb.AppendLine($@"UNION ALL ");
            sb.AppendLine($@"SELECT      a.""Subsidiary"", a.""CardCode"", b.""CardName"", a.""Division"", SUM(a.""Total"") AS ""Total"", SUM(a.""TaxlessTotal"") AS ""TaxlessTotal"", SUM(a.""Margin"") AS ""Margin"" ");
            sb.AppendLine($@"FROM        ( SELECT    'Iquique' AS ""Subsidiary"", t0.""CardCode"", t1.""Quantity"" * t1.""PriceAfVAT"" AS ""Total"", t1.""Quantity"" * t1.""PriceAfVAT"" AS ""TaxlessTotal"", t1.""Quantity"" * ( t1.""PriceAfVAT"" - t1.""StockPrice"" ) AS ""Margin"" ");
            sb.AppendLine($@"                        , ( CASE WHEN LOWER(IFNULL(t4.U_LINEA, '')) IN ({lines}) THEN 'M' ELSE ( CASE WHEN LOWER(t3.""SlpName"") IN ({sellers}) THEN 'E' ELSE '' END ) END ) AS ""Division"" ");
            sb.AppendLine($@"              FROM      {DBIQ}.OINV t0 ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.INV1 t1 ON t1.""DocEntry"" = t0.""DocEntry"" ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.OSLP t3 ON t3.""SlpCode"" = t0.""SlpCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.OITM t4 ON t4.""ItemCode"" = t1.""ItemCode"" ");
            sb.AppendLine($@"              WHERE     t0.""CardCode"" NOT IN ('CDMC-002', 'CDMC-003', 'CIMP-008') AND  t0.U_CCFACANU = 'N' AND t1.""TargetType"" <> 14 AND CAST(t0.""DocDate"" AS DATE) >= '{initial}' AND  CAST(t0.""DocDate"" AS DATE) <= '{final}' ) AS a ");
            sb.AppendLine($@"            INNER JOIN {DBSA}.OCRD b ON b.""CardCode"" = a.""CardCode"" ");
            sb.AppendLine($@"GROUP BY    a.""Subsidiary"", a.""CardCode"", b.""CardName"", a.""Division"" ");
            sb.AppendLine($@"UNION ALL ");
            sb.AppendLine($@"SELECT      a.""Subsidiary"", a.""CardCode"", b.""CardName"", a.""Division"", SUM(a.""Total"") AS ""Total"", SUM(a.""TaxlessTotal"") AS ""TaxlessTotal"", SUM(a.""Margin"") AS ""Margin"" ");
            sb.AppendLine($@"FROM        ( SELECT    'Miami' AS ""Subsidiary"", t0.""CardCode"", t1.""Quantity"" * t1.""PriceAfVAT"" AS ""Total"", t1.""Quantity"" * t1.""PriceAfVAT"" AS ""TaxlessTotal"", t1.""Quantity"" * ( t1.""PriceAfVAT"" - t1.""StockPrice"" ) AS ""Margin"" ");
            sb.AppendLine($@"                        , ( CASE WHEN LOWER(IFNULL(t4.U_LINEA, '')) IN ({lines}) THEN 'M' ELSE ( CASE WHEN LOWER(t3.""SlpName"") IN ({sellers}) THEN 'E' ELSE '' END ) END ) AS ""Division"" ");
            sb.AppendLine($@"              FROM      {DBLA}.OINV t0 ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.INV1 t1 ON t1.""DocEntry"" = t0.""DocEntry"" ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.OSLP t3 ON t3.""SlpCode"" = t0.""SlpCode"" ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.OITM t4 ON t4.""ItemCode"" = t1.""ItemCode"" ");
            sb.AppendLine($@"              WHERE     t0.""CardCode"" NOT IN ('CDMC-002', 'CDMC-003', 'CIMP-008') AND  t0.U_CCFACANU = 'N' AND t1.""TargetType"" <> 14 AND CAST(t0.""DocDate"" AS DATE) >= '{initial}' AND  CAST(t0.""DocDate"" AS DATE) <= '{final}' ) AS a ");
            sb.AppendLine($@"            INNER JOIN {DBSA}.OCRD b ON b.""CardCode"" = a.""CardCode"" ");
            sb.AppendLine($@"GROUP BY    a.""Subsidiary"", a.""CardCode"", b.""CardName"", a.""Division"" ");

            IEnumerable<BEA.ResumeByClient> items = SQLList<BEA.ResumeByClient>(sb.ToString());
            return items;
        }

        public IEnumerable<BEA.Resume> ResumeOpenDeliveryNotes(List<string> Lines, List<string> Sellers, params Enum[] Relations)
        {
            string lines = string.Join(",", Lines.Select(x => $"'{x.ToLower()}'")), sellers = string.Join(",", Sellers.Select(x => $"'{x.ToLower()}'")), query;
            query = $@"SELECT  ""Subsidiary"", ""Warehouse"", ""Division"", SUM(""Total"") AS ""Total""
FROM    ( SELECT  'Santa Cruz' AS ""Subsidiary"", t4.""WhsName"" AS ""Warehouse""
                  , ( CASE WHEN LOWER(IFNULL(t2.U_LINEA, '')) IN ( {lines} ) THEN 'M' ELSE ( CASE WHEN LOWER(t3.""SlpName"") IN ( {sellers} ) THEN 'E' ELSE '' END ) END ) AS ""Division""
                  , ""PriceAfVAT"" * ""Quantity"" AS ""Total""
          FROM    {DBSA}.ODLN t0
                  INNER JOIN {DBSA}.DLN1 t1 ON t0.""DocEntry"" = t1.""DocEntry""
                  INNER JOIN {DBSA}.OITM t2 ON t1.""ItemCode"" = t2.""ItemCode""
                  INNER JOIN {DBSA}.OSLP t3 ON t0.""SlpCode"" = t3.""SlpCode"" 
                  INNER JOIN {DBSA}.OWHS t4 ON t1.""WhsCode"" = t4.""WhsCode""
          WHERE   ""DocStatus"" = 'O' AND ""DocTotal"" > 0
          UNION 
          SELECT  'Miami' AS ""Subsidiary"", t4.""WhsName"" AS ""Warehouse""
                  , ( CASE WHEN LOWER(IFNULL(t2.U_LINEA, '')) IN ( {lines} ) THEN 'M' ELSE ( CASE WHEN LOWER(t3.""SlpName"") IN ( {sellers} ) THEN 'E' ELSE '' END ) END ) AS ""Division""
                  , ""PriceAfVAT"" * ""Quantity"" AS ""Total""
          FROM    {DBLA}.ODLN t0
                  INNER JOIN {DBLA}.DLN1 t1 ON t0.""DocEntry"" = t1.""DocEntry""
                  INNER JOIN {DBLA}.OITM t2 ON t1.""ItemCode"" = t2.""ItemCode""
                  INNER JOIN {DBLA}.OSLP t3 ON t0.""SlpCode"" = t3.""SlpCode"" 
                  INNER JOIN {DBLA}.OWHS t4 ON t1.""WhsCode"" = t4.""WhsCode""
          WHERE   ""DocStatus"" = 'O' AND ""DocTotal"" > 0
          UNION 
          SELECT  'Iquique' AS ""Subsidiary"", t4.""WhsName"" AS ""Warehouse""
                  , ( CASE WHEN LOWER(IFNULL(t2.U_LINEA, '')) IN ( {lines} ) THEN 'M' ELSE ( CASE WHEN LOWER(t3.""SlpName"") IN ( {sellers} ) THEN 'E' ELSE '' END ) END ) AS ""Division""
                  , ""PriceAfVAT"" * ""Quantity"" AS ""Total""
          FROM    {DBIQ}.ODLN t0
                  INNER JOIN {DBIQ}.DLN1 t1 ON t0.""DocEntry"" = t1.""DocEntry""
                  INNER JOIN {DBIQ}.OITM t2 ON t1.""ItemCode"" = t2.""ItemCode""
                  INNER JOIN {DBIQ}.OSLP t3 ON t0.""SlpCode"" = t3.""SlpCode"" 
                  INNER JOIN {DBIQ}.OWHS t4 ON t1.""WhsCode"" = t4.""WhsCode""
          WHERE   ""DocStatus"" = 'O' AND ""DocTotal"" > 0 ) a
GROUP BY ""Subsidiary"", ""Warehouse"", ""Division""  ";

            IEnumerable<BEA.Resume> items = SQLList(query, Relations);
            return items;
        }

        #endregion

        #region Search Methods

        #endregion

        #region Constructors

        public Resume() : base() { }

        internal Resume(HanaConnection Connection) : base(Connection) { }

        #endregion
    }
}
