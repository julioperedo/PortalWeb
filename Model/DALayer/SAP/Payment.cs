using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using BE = BEntities;
using BEA = BEntities.SAP;

namespace DALayer.SAP
{
    [Serializable()]
    public class Payment : Hana.DALEntity<BEA.Payment>
    {

        #region Methods

        protected override void LoadRelations(ref BEA.Payment Item, params Enum[] Relations) { }

        protected override void LoadRelations(ref IEnumerable<BEA.Payment> Items, params Enum[] Relations) { }

        #endregion

        #region List Methods

        public IEnumerable<BEA.Payment> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new();
            string strFilter = "";
            if (FilterList?.Count > 0) strFilter = GetFilter(FilterList.ToArray());

            sbQuery.AppendLine($@"SELECT  * ");
            sbQuery.AppendLine($@"FROM    ({GetQuery()}) a ");
            if (strFilter != "") sbQuery.AppendLine($@"WHERE   {strFilter} ");
            sbQuery.AppendLine($@"ORDER BY {GetOrder(Order)} ");

            IEnumerable<BEA.Payment> items = SQLList(sbQuery.ToString(), Relations);
            return items;
        }

        public IEnumerable<BEA.Payment> List(string ClientCode, DateTime? Initial, DateTime? Final, int? ReceiptCode, int? NoteCode, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new();
            sbQuery.AppendLine($@"SELECT  * ");
            sbQuery.AppendLine($@"FROM    ({GetQuery()}) a ");
            sbQuery.AppendLine($@"WHERE   ""Subsidiary"" IS NOT NULL ");
            if (!string.IsNullOrWhiteSpace(ClientCode))
            {
                sbQuery.AppendLine($@"        AND ""ClientCode"" = '{ClientCode}' ");
            }
            if (Initial.HasValue)
            {
                sbQuery.AppendLine($@"        AND ""DocDate"" >= '{Initial.Value:yyyy-MM-dd}' ");
            }
            if (Final.HasValue)
            {
                sbQuery.AppendLine($@"        AND ""DocDate"" <= '{Final.Value:yyyy-MM-dd}' ");
            }
            if (ReceiptCode.HasValue && ReceiptCode.Value > 0)
            {
                sbQuery.AppendLine($@"        AND ""DocNumber"" = {ReceiptCode.Value} ");
            }
            if (NoteCode.HasValue && NoteCode.Value > 0)
            {
                sbQuery.AppendLine($@"        AND ""DocNumber"" IN ( SELECT ""ReceiptNumber"" FROM ({GetSubQuery()}) b WHERE ""NoteNumber"" = {NoteCode.Value} ) ");
            }
            sbQuery.AppendLine($@"ORDER By {GetOrder(Order)} ");

            IEnumerable<BEA.Payment> items = SQLList(sbQuery.ToString(), Relations);
            return items;
        }

        public IEnumerable<BEA.Payment> ListAdjustments(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            string strFilter = FilterList?.Count > 0 ? GetFilter(FilterList.ToArray()) : "1 = 1",
            query = $@"SELECT  * 
FROM    ( SELECT    'Santa Cruz' AS ""Subsidiary"", T1.""ShortName"" AS ""ClientCode"", T2.""CardName"" AS ""ClientName"", T0.""TransId"" AS ""Id"" 
                    , CAST(T0.""RefDate"" AS DATE) AS ""DocDate"", ( CASE WHEN T1.""Credit"" > 0 THEN T1.""Credit"" ELSE -T1.""Debit"" END ) AS ""TotalReceipt"" 
                    , ( CASE WHEN T1.""BalDueCred"" = 0 THEN 'Reconciliado' ELSE '' END ) AS ""State"", T0.""Memo"" AS ""Comments"", '' AS ""JournalComments"" 
          FROM      {DBSA}.OJDT T0 
                    INNER JOIN {DBSA}.JDT1 T1 ON T0.""TransId"" = T1.""TransId"" 
                    INNER JOIN {DBSA}.OCRD T2 ON T1.""ShortName"" = T2.""CardCode"" 
          WHERE     T0.""TransType"" = '30' AND ( T1.""Credit"" > 0 OR T1.""Debit"" > 0 ) 
          UNION 
          SELECT    'Miami', T1.""ShortName"", T2.""CardName"", T0.""TransId"", CAST(T0.""RefDate"" AS DATE) 
                    , ( CASE WHEN T1.""Credit"" > 0 THEN T1.""Credit"" ELSE -T1.""Debit"" END ), ( CASE WHEN T1.""BalDueCred"" = 0 THEN 'Reconciliado' ELSE '' END ), T0.""Memo"", ''  
          FROM      {DBLA}.OJDT T0 
                    INNER JOIN {DBLA}.JDT1 T1 ON T0.""TransId"" = T1.""TransId"" 
                    INNER JOIN {DBLA}.OCRD T2 ON T1.""ShortName"" = T2.""CardCode"" 
          WHERE     T0.""TransType"" = '30' AND ( T1.""Credit"" > 0 OR T1.""Debit"" > 0 ) 
          UNION 
          SELECT    'Iquique', T1.""ShortName"", T2.""CardName"", T0.""TransId"", CAST(T0.""RefDate"" AS DATE) 
                    , ( CASE WHEN T1.""Credit"" > 0 THEN T1.""Credit"" ELSE -T1.""Debit"" END ), ( CASE WHEN T1.""BalDueCred"" = 0 THEN 'Reconciliado' ELSE '' END ), T0.""Memo"", ''  
          FROM      {DBIQ}.OJDT T0 
                    INNER JOIN {DBIQ}.JDT1 T1 ON T0.""TransId"" = T1.""TransId"" 
                    INNER JOIN {DBIQ}.OCRD T2 ON T1.""ShortName"" = T2.""CardCode"" 
          WHERE     T0.""TransType"" = '30' AND ( T1.""Credit"" > 0 OR T1.""Debit"" > 0 ) 
          UNION 
          SELECT    'Santa Cruz', ""CardCode"", ""CardName"", ""DocNum"", CAST(""DocDate"" AS DATE), ""DocTotal"", 'Ajustes', IFNULL(""NumAtCard"", '') || ' ' || IFNULL(""Comments"", ''), ""JrnlMemo"" || ' - ' || ""TransId"" 
          FROM      {DBSA}.ORIN 
          WHERE     ""DocType"" = 'S' 
          UNION 
          SELECT    'Miami', ""CardCode"", ""CardName"", ""DocNum"", CAST(""DocDate"" AS DATE), ""DocTotal"", 'Ajustes', IFNULL(""NumAtCard"", '') || ' ' || IFNULL(""Comments"", ''), ""JrnlMemo"" || ' - ' || ""TransId"" 
          FROM      {DBLA}.ORIN 
          WHERE     ""DocType"" = 'S' 
          UNION 
          SELECT    'Iquique', ""CardCode"", ""CardName"", ""DocNum"", CAST(""DocDate"" AS DATE), ""DocTotal"", 'Ajustes', IFNULL(""NumAtCard"", '') || ' ' || IFNULL(""Comments"", ''), ""JrnlMemo"" || ' - ' || ""TransId"" 
          FROM      {DBIQ}.ORIN 
          WHERE     ""DocType"" = 'S' ) A 
WHERE    {strFilter}
ORDER BY {GetOrder(Order)} ";

            IEnumerable<BEA.Payment> Collection = SQLList(query, Relations);
            return Collection;
        }

        public IEnumerable<BEA.PaymentItem> ListAdjustmentItems(string Subsidiary, int DocNum, string SortingBy)
        {
            string query = $@"SELECT  *
FROM    ( SELECT  'Santa Cruz' AS ""Subsidiary"", t1.""LineNum"", t0.""DocNum"", t1.""Dscription"" AS ""Description"", t1.""AcctCode"" AS ""AccountCode"", t1.U_NOMBRE_CUENTA AS ""AccountName"", t1.""LineTotal"" AS ""Total""
          FROM    {DBSA}.ORIN t0
                  INNER JOIN {DBSA}.RIN1 t1 ON t0.""DocEntry"" = t1.""DocEntry"" 
          UNION
          SELECT  'Iquique' AS ""Subsidiary"", t1.""LineNum"", t0.""DocNum"", t1.""Dscription"" AS ""Description"", t1.""AcctCode"" AS ""AccountCode"", t1.U_NOMBRE_CUENTA AS ""AccountName"", t1.""LineTotal"" AS ""Total""
          FROM    {DBIQ}.ORIN t0
                  INNER JOIN {DBIQ}.RIN1 t1 ON t0.""DocEntry"" = t1.""DocEntry"" 
          UNION
          SELECT  'Miami' AS ""Subsidiary"", t1.""LineNum"", t0.""DocNum"", t1.""Dscription"" AS ""Description"", t1.""AcctCode"" AS ""AccountCode"", t1.U_NOMBRE_CUENTA AS ""AccountName"", t1.""LineTotal"" AS ""Total""
          FROM    {DBLA}.ORIN t0
                  INNER JOIN {DBLA}.RIN1 t1 ON t0.""DocEntry"" = t1.""DocEntry""
        ) a 
WHERE   ""Subsidiary"" = '{Subsidiary}' AND ""DocNum"" = {DocNum} 
ORDER BY {GetOrder(SortingBy)} ";

            IEnumerable<BEA.PaymentItem> Items = SQLList<BEA.PaymentItem>(query);
            return Items;
        }

        public IEnumerable<BEA.Payment> ListAdjustment(string Subsidiary, int AdjustCode, string State, params Enum[] Relations)
        {
            var sb = new StringBuilder();
            if (State == "Reconciliado")
            {
                sb.AppendLine(@"SELECT  * ");
                sb.AppendLine(@"FROM    ( SELECT    'Santa Cruz' AS ""Subsidiary"", t1.""ShortName"" AS ""ClientCode"", t2.""CardName"" AS ""ClientName"", t0.""TransId"" AS ""Id"" ");
                sb.AppendLine(@"                    , CAST(t0.""RefDate"" AS DATE) AS ""DocDate"", ( CASE WHEN t1.""Credit"" > 0 THEN t1.""Credit"" ELSE -t1.""Debit"" END ) AS ""TotalReceipt"" ");
                sb.AppendLine(@"                    , ( CASE WHEN t1.""BalDueCred"" = 0 THEN 'Reconciliado' ELSE '' END ) AS ""State"" ");
                sb.AppendLine(@"          FROM      DMC_SA.OJDT t0 ");
                sb.AppendLine(@"                    INNER JOIN DMC_SA.JDT1 t1 ON t1.""TransId"" = t0.""TransId"" ");
                sb.AppendLine(@"                    INNER JOIN DMC_SA.OCRD t2 ON t2.""CardCode"" = t1.""ShortName"" ");
                sb.AppendLine(@"          WHERE     t0.""TransType"" = 30 ");
                sb.AppendLine(@"                    AND ( t1.""Credit"" > 0 OR t1.""Debit"" > 0 ) ");
                sb.AppendLine(@"          UNION ALL ");
                sb.AppendLine(@"          SELECT    'Iquique' AS ""Subsidiary"", t1.""ShortName"" AS ""ClientCode"", t2.""CardName"" AS ""ClientName"", t0.""TransId"" AS ""Id"" ");
                sb.AppendLine(@"                    , CAST(t0.""RefDate"" AS DATE) AS ""DocDate"", ( CASE WHEN t1.""Credit"" > 0 THEN t1.""Credit"" ELSE -t1.""Debit"" END ) AS ""TotalReceipt"" ");
                sb.AppendLine(@"                    , ( CASE WHEN t1.""BalDueCred"" = 0 THEN 'Reconciliado' ELSE '' END ) AS ""State"" ");
                sb.AppendLine(@"          FROM      DMC_IQUIQUE.OJDT t0 ");
                sb.AppendLine(@"                    INNER JOIN DMC_IQUIQUE.JDT1 t1 ON t1.""TransId"" = t0.""TransId"" ");
                sb.AppendLine(@"                    INNER JOIN DMC_IQUIQUE.OCRD t2 ON t2.""CardCode"" = t1.""ShortName"" ");
                sb.AppendLine(@"          WHERE     t0.""TransType"" = 30 ");
                sb.AppendLine(@"                    AND ( t1.""Credit"" > 0 OR t1.""Debit"" > 0 ) ");
                sb.AppendLine(@"          UNION ALL ");
                sb.AppendLine(@"          SELECT    'Miami' AS ""Subsidiary"", t1.""ShortName"" AS ""ClientCode"", t2.""CardName"" AS ""ClientName"", t0.""TransId"" AS ""Id"" ");
                sb.AppendLine(@"                    , CAST(t0.""RefDate"" AS DATE) AS ""DocDate"", ( CASE WHEN t1.""Credit"" > 0 THEN t1.""Credit"" ELSE -t1.""Debit"" END ) AS ""TotalReceipt"" ");
                sb.AppendLine(@"                    , ( CASE WHEN t1.""BalDueCred"" = 0 THEN 'Reconciliado' ELSE '' END ) AS ""State"" ");
                sb.AppendLine(@"          FROM      DMC_LATINAMERICA.OJDT t0 ");
                sb.AppendLine(@"                    INNER JOIN DMC_LATINAMERICA.JDT1 t1 ON t1.""TransId"" = t0.""TransId"" ");
                sb.AppendLine(@"                    INNER JOIN DMC_LATINAMERICA.OCRD t2 ON t2.""CardCode"" = t1.""ShortName"" ");
                sb.AppendLine(@"          WHERE     t0.""TransType"" = 30 ");
                sb.AppendLine(@"                    AND ( t1.""Credit"" > 0 OR t1.""Debit"" > 0 )) a ");
                sb.AppendLine($@"WHERE	LOWER(""Subsidiary"") = '{Subsidiary.ToLower()}' AND Id = {AdjustCode} ");
            }
            else
            {
                sb.AppendLine(@"SELECT  * ");
                sb.AppendLine(@"FROM    ( SELECT    'Santa Cruz' AS ""Subsidiary"", ""CardCode"" AS ""ClientCode"", ""CardName"" AS ""ClientName"", ""DocNum"" AS ""DocNumber"" ");
                sb.AppendLine(@"                    , CAST(""DocDate"" AS DATE) AS ""DocDate"", ""DocTotal"", 'Ajustes' AS ""State"" ");
                sb.AppendLine(@"          FROM      DMC_SA.ORIN  ");
                sb.AppendLine(@"          WHERE     ""DocType"" = 'S' ");
                sb.AppendLine(@"          UNION ALL ");
                sb.AppendLine(@"          SELECT    'Iquique' AS ""Subsidiary"", ""CardCode"" AS ""ClientCode"", ""CardName"" AS ""ClientName"", ""DocNum"" AS ""DocNumber"" ");
                sb.AppendLine(@"                    , CAST(""DocDate"" AS DATE) AS ""DocDate"", ""DocTotal"", 'Ajustes' AS ""State"" ");
                sb.AppendLine(@"          FROM      DMC_IQUIQUE.ORIN  ");
                sb.AppendLine(@"          WHERE     ""DocType"" = 'S' ");
                sb.AppendLine(@"          UNION ALL ");
                sb.AppendLine(@"          SELECT    'Miami' AS ""Subsidiary"", ""CardCode"" AS ""ClientCode"", ""CardName"" AS ""ClientName"", ""DocNum"" AS ""DocNumber"" ");
                sb.AppendLine(@"                    , CAST(""DocDate"" AS DATE) AS ""DocDate"", ""DocTotal"", 'Ajustes' AS ""State"" ");
                sb.AppendLine(@"          FROM      DMC_LATINAMERICA.ORIN ");
                sb.AppendLine(@"                    WHERE     ""DocType"" = 'S' ) a ");
                sb.AppendLine($@"WHERE	LOWER(""Subsidiary"") = '{Subsidiary.ToLower()}' AND ""DocNumber"" = {AdjustCode} ");
            }
            IEnumerable<BEA.Payment> lstResult = SQLList(sb.ToString(), Relations);
            return lstResult;
        }

        #endregion

        #region Search Methods

        public int SearchAVGDuedays(string CardCode)
        {
            string query = $@"SELECT  IFNULL(CAST(AVG(""DueDays"") AS INT), 0)  
                              FROM    ( SELECT  'SANTA CRUZ' AS ""Subsidiary"", t0.""DocNum"" AS ""DocNumber"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", (CASE WHEN (t0.""PayNoDoc"" = 'Y' AND t0.""OpenBal"" > 0) THEN 'Sin aplicar' ELSE 'Aplicado' END) AS ""State"" 
                                                , t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"", t2.""DocNum"" AS ""NoteNumber"", t2.""DocTotal"" AS ""NoteTotal"", CAST(t2.""DocDate"" AS DATE) AS ""NoteDate"", t3.""PymntGroup"" AS ""Terms"" 
                                                , DAYS_BETWEEN(t2.""DocDueDate"", t0.""DocDate"") AS ""DueDays"", t1.""SumApplied"" AS ""NotePaidAmount"", (CASE WHEN (t1.""InvoiceId"" = 0 OR t1.""InvoiceId"" IS NULL) THEN t0.""NoDocSum"" ELSE 0 END) AS ""OnAccount"" 
                                                , IFNULL(t1.""SumApplied"", 0) + IFNULL((CASE WHEN (t1.""InvoiceId"" = 0 OR t1.""InvoiceId"" IS NULL) THEN t0.""NoDocSum"" ELSE 0 END), 0) AS ""Total"", t0.""DocTotal"" AS ""TotalReceipt"" 
                                                , (CASE WHEN ((t1.""InvoiceId"" = 0 AND t0.""OpenBal"" > 0) OR (t1.""InvoiceId"" IS NULL AND t0.""OpenBal"" > 0)) THEN t0.""OpenBal"" ELSE 0 END) AS ""NotAppliedTotal"", t0.""Comments"", t2.""isIns"" AS ""IsDeliveryNote"" 
                                        FROM    DMC_SA.ORCT t0 
                                                LEFT OUTER JOIN DMC_SA.RCT2 t1 ON t0.""DocEntry"" = t1.""DocNum"" 
                                                LEFT OUTER JOIN DMC_SA.OINV t2 ON t2.""DocEntry"" = t1.""DocEntry"" AND t1.""InvType"" = 13 
                                                LEFT OUTER JOIN DMC_SA.OCTG t3 ON t3.""GroupNum"" = t2.""GroupNum"" 
                                        WHERE   t0.""Canceled"" = 'N' AND t0.""DocType"" <> 'A' 
                                        UNION ALL 
                                        SELECT  'MIAMI' AS ""Subsidiary"", t0.""DocNum"" AS ""DocNumber"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", (CASE WHEN (t0.""PayNoDoc"" = 'Y' AND t0.""OpenBal"" > 0) THEN 'Sin aplicar' ELSE 'Aplicado' END) AS ""State"" 
                                                , t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"", t2.""DocNum"" AS ""NoteNumber"", t2.""DocTotal"" AS ""NoteTotal"", CAST(t2.""DocDate"" AS DATE) AS ""NoteDate"", t3.""PymntGroup"" AS ""Terms"" 
                                                , DAYS_BETWEEN(t2.""DocDueDate"", t0.""DocDate"") AS ""DueDays"", t1.""SumApplied"" AS ""NotePaidAmount"", (CASE WHEN (t1.""InvoiceId"" = 0 OR t1.""InvoiceId"" IS NULL) THEN t0.""NoDocSum"" ELSE 0 END) AS ""OnAccount"" 
                                                , IFNULL(t1.""SumApplied"", 0) + IFNULL((CASE WHEN (t1.""InvoiceId"" = 0 OR t1.""InvoiceId"" IS NULL) THEN t0.""NoDocSum"" ELSE 0 END), 0) AS ""Total"", t0.""DocTotal"" AS ""TotalReceipt"" 
                                                , (CASE WHEN ((t1.""InvoiceId"" = 0 AND t0.""OpenBal"" > 0) OR (t1.""InvoiceId"" IS NULL AND t0.""OpenBal"" > 0)) THEN t0.""OpenBal"" ELSE 0 END) AS ""NotAppliedTotal"", t0.""Comments"", t2.""isIns"" AS ""IsDeliveryNote"" 
                                        FROM    DMC_LATINAMERICA.ORCT t0 
                                                LEFT OUTER JOIN DMC_LATINAMERICA.RCT2 t1 ON t0.""DocEntry"" = t1.""DocNum"" 
                                                LEFT OUTER JOIN DMC_LATINAMERICA.OINV t2 ON t2.""DocEntry"" = t1.""DocEntry"" AND t1.""InvType"" = 13 
                                                LEFT OUTER JOIN DMC_LATINAMERICA.OCTG t3 ON t3.""GroupNum"" = t2.""GroupNum"" 
                                        WHERE   t0.""Canceled"" = 'N' AND t0.""DocType"" <> 'A' 
                                        UNION ALL 
                                        SELECT  'IQUIQUE' AS ""Subsidiary"", t0.""DocNum"" AS ""DocNumber"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", (CASE WHEN (t0.""PayNoDoc"" = 'Y' AND t0.""OpenBal"" > 0) THEN 'Sin aplicar' ELSE 'Aplicado' END) AS ""State"" 
                                                , t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"", t2.""DocNum"" AS ""NoteNumber"", t2.""DocTotal"" AS ""NoteTotal"", CAST(t2.""DocDate"" AS DATE) AS ""NoteDate"", t3.""PymntGroup"" AS ""Terms"" 
                                                , DAYS_BETWEEN(t2.""DocDueDate"", t0.""DocDate"") AS ""DueDays"", t1.""SumApplied"" AS ""NotePaidAmount"", (CASE WHEN (t1.""InvoiceId"" = 0 OR t1.""InvoiceId"" IS NULL) THEN t0.""NoDocSum"" ELSE 0 END) AS ""OnAccount"" 
                                                , IFNULL(t1.""SumApplied"", 0) + IFNULL((CASE WHEN (t1.""InvoiceId"" = 0 OR t1.""InvoiceId"" IS NULL) THEN t0.""NoDocSum"" ELSE 0 END), 0) AS ""Total"", t0.""DocTotal"" AS ""TotalReceipt"" 
                                                , (CASE WHEN ((t1.""InvoiceId"" = 0 AND t0.""OpenBal"" > 0) OR (t1.""InvoiceId"" IS NULL AND t0.""OpenBal"" > 0)) THEN t0.""OpenBal"" ELSE 0 END) AS ""NotAppliedTotal"", t0.""Comments"", t2.""isIns"" AS ""IsDeliveryNote"" 
                                        FROM    DMC_IQUIQUE.ORCT t0 
                                                LEFT OUTER JOIN DMC_IQUIQUE.RCT2 t1 ON t0.""DocEntry"" = t1.""DocNum"" 
                                                LEFT OUTER JOIN DMC_IQUIQUE.OINV t2 ON t2.""DocEntry"" = t1.""DocEntry"" AND t1.""InvType"" = 13 
                                                LEFT OUTER JOIN DMC_IQUIQUE.OCTG t3 ON t3.""GroupNum"" = t2.""GroupNum"" 
                                        WHERE   t0.""Canceled"" = 'N' AND t0.""DocType"" <> 'A'  ) a 
                              WHERE   LOWER(""ClientCode"") = '{CardCode.ToLower()}'";
            int avgDueDays = (int)Value(query);
            return avgDueDays;
        }

        #endregion

        #region Private Methods

        private string GetQuery()
        {
            var sb = new StringBuilder();
            sb.AppendLine(@"SELECT  'SANTA CRUZ' AS ""Subsidiary"", t0.""DocNum"" AS ""DocNumber"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", (CASE WHEN (t0.""PayNoDoc"" = 'Y' AND t0.""OpenBal"" > 0) THEN 'Sin aplicar' ELSE 'Aplicado' END) AS ""State"" ");
            sb.AppendLine(@"        , t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"", t2.""DocNum"" AS ""NoteNumber"", t2.""DocTotal"" AS ""NoteTotal"", CAST(t2.""DocDate"" AS DATE) AS ""NoteDate"", t3.""PymntGroup"" AS ""Terms"" ");
            sb.AppendLine(@"        , DAYS_BETWEEN(t2.""DocDueDate"", t0.""DocDate"") AS ""DueDays"", t1.""SumApplied"" AS ""NotePaidAmount"", (CASE WHEN (t1.""InvoiceId"" = 0 OR t1.""InvoiceId"" IS NULL) THEN t0.""NoDocSum"" ELSE 0 END) AS ""OnAccount"" ");
            sb.AppendLine(@"        , IFNULL(t1.""SumApplied"", 0) + IFNULL((CASE WHEN (t1.""InvoiceId"" = 0 OR t1.""InvoiceId"" IS NULL) THEN t0.""NoDocSum"" ELSE 0 END), 0) AS ""Total"", t0.""DocTotal"" AS ""TotalReceipt"" ");
            sb.AppendLine(@"        , (CASE WHEN ((t1.""InvoiceId"" = 0 AND t0.""OpenBal"" > 0) OR (t1.""InvoiceId"" IS NULL AND t0.""OpenBal"" > 0)) THEN t0.""OpenBal"" ELSE 0 END) AS ""NotAppliedTotal"", t0.""Comments"", t2.""isIns"" AS ""IsDeliveryNote"" ");
            sb.AppendLine(@"FROM    DMC_SA.ORCT t0 ");
            sb.AppendLine(@"        LEFT OUTER JOIN DMC_SA.RCT2 t1 ON t0.""DocEntry"" = t1.""DocNum"" ");
            sb.AppendLine(@"        LEFT OUTER JOIN DMC_SA.OINV t2 ON t2.""DocEntry"" = t1.""DocEntry"" AND t1.""InvType"" = 13 ");
            sb.AppendLine(@"        LEFT OUTER JOIN DMC_SA.OCTG t3 ON t3.""GroupNum"" = t2.""GroupNum"" ");
            sb.AppendLine(@"WHERE   t0.""Canceled"" = 'N' AND t0.""DocType"" <> 'A' ");
            sb.AppendLine(@"UNION ALL ");
            sb.AppendLine(@"SELECT  'MIAMI' AS ""Subsidiary"", t0.""DocNum"" AS ""DocNumber"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", (CASE WHEN (t0.""PayNoDoc"" = 'Y' AND t0.""OpenBal"" > 0) THEN 'Sin aplicar' ELSE 'Aplicado' END) AS ""State"" ");
            sb.AppendLine(@"        , t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"", t2.""DocNum"" AS ""NoteNumber"", t2.""DocTotal"" AS ""NoteTotal"", CAST(t2.""DocDate"" AS DATE) AS ""NoteDate"", t3.""PymntGroup"" AS ""Terms"" ");
            sb.AppendLine(@"        , DAYS_BETWEEN(t2.""DocDueDate"", t0.""DocDate"") AS ""DueDays"", t1.""SumApplied"" AS ""NotePaidAmount"", (CASE WHEN (t1.""InvoiceId"" = 0 OR t1.""InvoiceId"" IS NULL) THEN t0.""NoDocSum"" ELSE 0 END) AS ""OnAccount"" ");
            sb.AppendLine(@"        , IFNULL(t1.""SumApplied"", 0) + IFNULL((CASE WHEN (t1.""InvoiceId"" = 0 OR t1.""InvoiceId"" IS NULL) THEN t0.""NoDocSum"" ELSE 0 END), 0) AS ""Total"", t0.""DocTotal"" AS ""TotalReceipt"" ");
            sb.AppendLine(@"        , (CASE WHEN ((t1.""InvoiceId"" = 0 AND t0.""OpenBal"" > 0) OR (t1.""InvoiceId"" IS NULL AND t0.""OpenBal"" > 0)) THEN t0.""OpenBal"" ELSE 0 END) AS ""NotAppliedTotal"", t0.""Comments"", t2.""isIns"" AS ""IsDeliveryNote"" ");
            sb.AppendLine(@"FROM    DMC_LATINAMERICA.ORCT t0 ");
            sb.AppendLine(@"        LEFT OUTER JOIN DMC_LATINAMERICA.RCT2 t1 ON t0.""DocEntry"" = t1.""DocNum"" ");
            sb.AppendLine(@"        LEFT OUTER JOIN DMC_LATINAMERICA.OINV t2 ON t2.""DocEntry"" = t1.""DocEntry"" AND t1.""InvType"" = 13 ");
            sb.AppendLine(@"        LEFT OUTER JOIN DMC_LATINAMERICA.OCTG t3 ON t3.""GroupNum"" = t2.""GroupNum"" ");
            sb.AppendLine(@"WHERE   t0.""Canceled"" = 'N' AND t0.""DocType"" <> 'A' ");
            sb.AppendLine(@"UNION ALL ");
            sb.AppendLine(@"SELECT  'IQUIQUE' AS ""Subsidiary"", t0.""DocNum"" AS ""DocNumber"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", (CASE WHEN (t0.""PayNoDoc"" = 'Y' AND t0.""OpenBal"" > 0) THEN 'Sin aplicar' ELSE 'Aplicado' END) AS ""State"" ");
            sb.AppendLine(@"        , t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"", t2.""DocNum"" AS ""NoteNumber"", t2.""DocTotal"" AS ""NoteTotal"", CAST(t2.""DocDate"" AS DATE) AS ""NoteDate"", t3.""PymntGroup"" AS ""Terms"" ");
            sb.AppendLine(@"        , DAYS_BETWEEN(t2.""DocDueDate"", t0.""DocDate"") AS ""DueDays"", t1.""SumApplied"" AS ""NotePaidAmount"", (CASE WHEN (t1.""InvoiceId"" = 0 OR t1.""InvoiceId"" IS NULL) THEN t0.""NoDocSum"" ELSE 0 END) AS ""OnAccount"" ");
            sb.AppendLine(@"        , IFNULL(t1.""SumApplied"", 0) + IFNULL((CASE WHEN (t1.""InvoiceId"" = 0 OR t1.""InvoiceId"" IS NULL) THEN t0.""NoDocSum"" ELSE 0 END), 0) AS ""Total"", t0.""DocTotal"" AS ""TotalReceipt"" ");
            sb.AppendLine(@"        , (CASE WHEN ((t1.""InvoiceId"" = 0 AND t0.""OpenBal"" > 0) OR (t1.""InvoiceId"" IS NULL AND t0.""OpenBal"" > 0)) THEN t0.""OpenBal"" ELSE 0 END) AS ""NotAppliedTotal"", t0.""Comments"", t2.""isIns"" AS ""IsDeliveryNote"" ");
            sb.AppendLine(@"FROM    DMC_IQUIQUE.ORCT t0 ");
            sb.AppendLine(@"        LEFT OUTER JOIN DMC_IQUIQUE.RCT2 t1 ON t0.""DocEntry"" = t1.""DocNum"" ");
            sb.AppendLine(@"        LEFT OUTER JOIN DMC_IQUIQUE.OINV t2 ON t2.""DocEntry"" = t1.""DocEntry"" AND t1.""InvType"" = 13 ");
            sb.AppendLine(@"        LEFT OUTER JOIN DMC_IQUIQUE.OCTG t3 ON t3.""GroupNum"" = t2.""GroupNum"" ");
            sb.AppendLine(@"WHERE   t0.""Canceled"" = 'N' AND t0.""DocType"" <> 'A' ");

            return sb.ToString();
        }

        private string GetSubQuery()
        {
            var sb = new StringBuilder();
            sb.AppendLine(@"SELECT  t0.""DocNum"" AS ""ReceiptNumber"", t2.""DocNum"" AS ""NoteNumber"" ");
            sb.AppendLine(@"FROM    DMC_SA.ORCT t0 ");
            sb.AppendLine(@"        INNER JOIN DMC_SA.OINV t2 ON t2.""DocEntry"" = t0.""DocEntry"" ");
            sb.AppendLine(@"WHERE   t0.""Canceled"" = 'N' AND t0.""DocType"" <> 'A' ");
            sb.AppendLine(@"UNION ALL ");
            sb.AppendLine(@"SELECT  t0.""DocNum"" AS ""ReceiptNumber"", t2.""DocNum"" AS ""NoteNumber"" ");
            sb.AppendLine(@"FROM    DMC_LATINAMERICA.ORCT T0 ");
            sb.AppendLine(@"        INNER JOIN DMC_LATINAMERICA.OINV t2 ON t2.""DocEntry"" = t0.""DocEntry"" ");
            sb.AppendLine(@"WHERE   t0.""Canceled"" = 'N' AND t0.""DocType"" <> 'A' ");
            sb.AppendLine(@"UNION ALL ");
            sb.AppendLine(@"SELECT  t0.""DocNum"" AS ""ReceiptNumber"", t2.""DocNum"" AS ""NoteNumber"" ");
            sb.AppendLine(@"FROM    DMC_IQUIQUE.ORCT t0 ");
            sb.AppendLine(@"        INNER JOIN DMC_IQUIQUE.OINV t2 ON t2.""DocEntry"" = t0.""DocEntry"" ");
            sb.AppendLine(@"WHERE   t0.""Canceled"" = 'N' AND t0.""DocType"" <> 'A' ");
            return sb.ToString();
        }

        #endregion

        #region Constructors

        public Payment() : base() { }

        #endregion
    }
}
