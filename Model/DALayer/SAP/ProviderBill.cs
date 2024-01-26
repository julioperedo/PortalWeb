using System;
using System.Collections.Generic;
using System.Text;
using BEA = BEntities.SAP;

namespace DALayer.SAP
{
    [Serializable()]
    public class ProviderBill : Hana.DALEntity<BEA.ProviderBill>
    {
        #region Methods

        protected override void LoadRelations(ref BEA.ProviderBill Item, params Enum[] Relations) { }

        protected override void LoadRelations(ref IEnumerable<BEA.ProviderBill> Items, params Enum[] Relations) { }

        #endregion

        #region Search Methods

        public BEA.ProviderBill Search(string Subsidiary, int DocNumber, params Enum[] Relations)
        {
            var sb = new StringBuilder();
            sb.AppendLine($@"SELECT  * ");
            sb.AppendLine($@"FROM    (   SELECT  DISTINCT ");
            sb.AppendLine($@"                    'Santa Cruz' AS ""Subsidiary"" ");
            sb.AppendLine($@"                    , T1.""DocNum"" AS ""DocNumber"", CAST(T1.""DocDate"" AS DATE) AS ""DocDate"", T6.""CardName"" AS ""ProviderName"", T1.""Address"" AS ""ProviderAddress"", T1.""Address2"" AS ""BillingAddress"" ");
            sb.AppendLine($@"                    , T2.""BaseRef"" AS ""OrderNumber"", T1.""NumAtCard"" AS ""Reference"", T3.""WhsName"" AS ""Warehouse"", T1.""CardCode"" AS ""ProviderCode"", T4.""SlpName"" AS ""SellerCode"" ");
            sb.AppendLine($@"                    , T5.""PymntGroup"" AS ""TermConditions"", T1.""TotalExpns"" AS ""OtherCosts"", T1.""DocTotal"" AS ""Total"", T1.""PaidToDate"", T1.""Comments"", T1.""JrnlMemo"" AS ""DailyComments"" ");
            sb.AppendLine($@"            FROM    {DBSA}.OPCH T1 ");
            sb.AppendLine($@"                    INNER JOIN {DBSA}.PCH1 T2 ON T1.""DocEntry"" = T2.""DocEntry"" ");
            sb.AppendLine($@"                    LEFT OUTER JOIN {DBSA}.OWHS T3 ON T2.""WhsCode"" = T3.""WhsCode"" ");
            sb.AppendLine($@"                    INNER JOIN {DBSA}.OSLP T4 ON T1.""SlpCode"" = T4.""SlpCode"" ");
            sb.AppendLine($@"                    INNER JOIN {DBSA}.OCTG T5 ON T1.""GroupNum"" = T5.""GroupNum"" ");
            sb.AppendLine($@"                    INNER JOIN {DBSA}.OCRD T6 ON T1.""CardCode"" = T6.""CardCode"" ");
            sb.AppendLine($@"            UNION ALL ");
            sb.AppendLine($@"            SELECT  DISTINCT ");
            sb.AppendLine($@"                    'Miami', T1.""DocNum"", CAST(T1.""DocDate"" AS DATE), T6.""CardName"", T1.""Address"", T1.""Address2"", T2.""BaseRef"", T1.""NumAtCard"", T3.""WhsName"", T1.""CardCode"", T4.""SlpName"" ");
            sb.AppendLine($@"                    , T5.""PymntGroup"", T1.""TotalExpns"", T1.""DocTotal"" AS ""Total"", T1.""PaidToDate"", T1.""Comments"", T1.""JrnlMemo"" ");
            sb.AppendLine($@"            FROM    {DBLA}.OPCH T1 ");
            sb.AppendLine($@"                    INNER JOIN {DBLA}.PCH1 T2 ON T1.""DocEntry"" = T2.""DocEntry"" ");
            sb.AppendLine($@"                    LEFT OUTER JOIN {DBLA}.OWHS T3 ON T2.""WhsCode"" = T3.""WhsCode"" ");
            sb.AppendLine($@"                    INNER JOIN {DBLA}.OSLP T4 ON T1.""SlpCode"" = T4.""SlpCode"" ");
            sb.AppendLine($@"                    INNER JOIN {DBLA}.OCTG T5 ON T1.""GroupNum"" = T5.""GroupNum"" ");
            sb.AppendLine($@"                    INNER JOIN {DBLA}.OCRD T6 ON T1.""CardCode"" = T6.""CardCode"" ");
            sb.AppendLine($@"            UNION ALL ");
            sb.AppendLine($@"            SELECT  DISTINCT ");
            sb.AppendLine($@"                    'Iquique', T1.""DocNum"", CAST(T1.""DocDate"" AS DATE), T6.""CardName"", T1.""Address"", T1.""Address2"", T2.""BaseRef"", T1.""NumAtCard"", T3.""WhsName"", T1.""CardCode"", T4.""SlpName"" ");
            sb.AppendLine($@"                    , T5.""PymntGroup"", T1.""TotalExpns"", T1.""DocTotal"" AS ""Total"", T1.""PaidToDate"", T1.""Comments"", T1.""JrnlMemo"" ");
            sb.AppendLine($@"            FROM    {DBIQ}.OPCH T1 ");
            sb.AppendLine($@"                    INNER JOIN {DBIQ}.PCH1 T2 ON T1.""DocEntry"" = T2.""DocEntry"" ");
            sb.AppendLine($@"                    LEFT OUTER JOIN {DBIQ}.OWHS T3 ON T2.""WhsCode"" = T3.""WhsCode"" ");
            sb.AppendLine($@"                    INNER JOIN {DBIQ}.OSLP T4 ON T1.""SlpCode"" = T4.""SlpCode"" ");
            sb.AppendLine($@"                    INNER JOIN {DBIQ}.OCTG T5 ON T1.""GroupNum"" = T5.""GroupNum"" ");
            sb.AppendLine($@"                    INNER JOIN {DBIQ}.OCRD T6 ON T1.""CardCode"" = T6.""CardCode"" ) a ");
            sb.AppendLine($@"WHERE   LOWER(""Subsidiary"") = '{Subsidiary.ToLower()}' ");
            sb.AppendLine($@"        AND ""DocNumber"" = {DocNumber} ");

            BEA.ProviderBill item = SQLSearch(sb.ToString(), Relations);
            return item;
        }

        #endregion

        #region Lists

        public IEnumerable<BEA.ProviderBillItem> ListItems(string Subsidiary, int DocNumber, string OrderBy)
        {
            var sb = new StringBuilder();
            sb.AppendLine($@"SELECT      * ");
            sb.AppendLine($@"FROM        (   SELECT  DISTINCT ");
            sb.AppendLine($@"                        'Santa Cruz' AS ""Subsidiary"", T1.""DocNum"" AS ""DocNumber"", CAST(T2.""Quantity"" AS INT) AS ""Quantity"", T2.""ItemCode"", T2.U_COD_FAB AS ""BrandCode"", T2.""Dscription"" AS ""Description"" ");
            sb.AppendLine($@"                        , T2.""TaxCode"", T2.""Price"", T2.""GTotal"" AS ""Total"", T2.""LineNum"" ");
            sb.AppendLine($@"                FROM    {DBSA}.OPCH T1 ");
            sb.AppendLine($@"                        INNER JOIN {DBSA}.PCH1 T2 ON T1.""DocEntry"" = T2.""DocEntry"" ");
            sb.AppendLine($@"                UNION ALL ");
            sb.AppendLine($@"                SELECT  DISTINCT ");
            sb.AppendLine($@"                        'Miami', T1.""DocNum"", CAST(T2.""Quantity"" AS INT), T2.""ItemCode"", T2.U_COD_FAB, T2.""Dscription"", T2.""TaxCode"", T2.""Price"", T2.""GTotal"", T2.""LineNum"" ");
            sb.AppendLine($@"                FROM    {DBLA}.OPCH T1 ");
            sb.AppendLine($@"                        INNER JOIN {DBLA}.PCH1 T2 ON T1.""DocEntry"" = T2.""DocEntry"" ");
            sb.AppendLine($@"                UNION ALL ");
            sb.AppendLine($@"                SELECT  DISTINCT ");
            sb.AppendLine($@"                        'Iquique', T1.""DocNum"", CAST(T2.""Quantity"" AS INT), T2.""ItemCode"", T2.U_COD_FAB, T2.""Dscription"", T2.""TaxCode"", T2.""Price"", T2.""GTotal"", T2.""LineNum"" ");
            sb.AppendLine($@"                FROM    {DBIQ}.OPCH T1 ");
            sb.AppendLine($@"                        INNER JOIN {DBIQ}.PCH1 T2 ON T1.""DocEntry"" = T2.""DocEntry"" ) a ");
            sb.AppendLine($@"WHERE       LOWER(""Subsidiary"") = '{Subsidiary.ToLower()}' AND ""DocNumber"" = '{DocNumber}' ");
            sb.AppendLine($@"ORDER BY    {GetOrder(OrderBy)} ");

            IEnumerable<BEA.ProviderBillItem> items = SQLList<BEA.ProviderBillItem>(sb.ToString());
            return items;
        }

        #endregion

        #region Constructors

        public ProviderBill() : base() { }

        #endregion
    }
}
