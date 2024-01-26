using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BE = BEntities;
using BEA = BEntities.SAP;

namespace DALayer.SAP.Hana
{
    [Serializable()]
    public class DeliveryNote : DALEntity<BEA.DeliveryNote>
    {
        #region Global Variables
        public string Query { get; set; }
        #endregion

        #region Methods

        internal IEnumerable<BEA.DeliveryNote> ReturnChild(IEnumerable<string> Keys, params Enum[] Relations)
        {
            StringBuilder sb = new();
            sb.AppendLine($@"SELECT  * ");
            sb.AppendLine($@"FROM    ({Query}) a ");
            sb.AppendLine($@"WHERE   (LOWER(a.""Subsidiary"") || '-' || CAST(a.""NoteId"" AS VARCHAR(10))) IN ( {string.Join(",", Keys.Select(x => $"'{x.Replace("'", "")}'"))} ) ");
            IEnumerable<BEA.DeliveryNote> items = SQLList(sb.ToString(), Relations);
            return items;
        }

        protected override void LoadRelations(ref IEnumerable<BEA.DeliveryNote> Items, params Enum[] Relations)
        {
            IEnumerable<BEA.Note> lstNotes = null;
            IEnumerable<BEA.DeliveryNoteItem> lstItems = null;
            IEnumerable<BEA.OrderFile> lstFiles = null;

            IEnumerable<string> Keys;
            foreach (Enum RelationEnum in Relations)
            {
                Keys = from i in Items select $"{i.Subsidiary.ToLower()}-{i.Id}";
                if (RelationEnum.Equals(BEA.relDeliveryNote.Items))
                {
                    lstItems = ListItems2(Keys, "1")?.ToList();
                }
                if (RelationEnum.Equals(BEA.relDeliveryNote.Notes))
                {
                    using Note dalNote = new();
                    lstNotes = dalNote.ReturnChild(Keys, Relations);
                }
                if (RelationEnum.Equals(BEA.relNote.Files))
                {
                    lstFiles = ListFiles(Keys)?.ToList();
                }
            }

            if (Relations.GetLength(0) > 0 && Items?.Count() > 0)
            {
                foreach (var item in Items)
                {
                    if (lstItems?.Count() > 0)
                    {
                        item.Items = (from i in lstItems where i.Id == item.Id select i)?.ToList();
                    }
                    if (lstNotes?.Count() > 0)
                    {
                        item.Notes = lstNotes.Where(i => i.Subsidiary.ToLower() == item.Subsidiary.ToLower() & i.OrderId.Contains(item.Id.ToString()))?.ToList();
                    }
                    if (lstFiles?.Count() > 0)
                    {
                        item.Files = lstFiles.Where(x => x.Subsidiary.ToLower() == item.Subsidiary.ToLower() && x.DocEntry == item.Id)?.ToList();
                    }
                }
            }
        }

        protected override void LoadRelations(ref BEA.DeliveryNote item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
                string[] Keys = new[] { $"{item.Subsidiary.ToLower()}-{item.Id}" };
                if (RelationEnum.Equals(BEA.relDeliveryNote.Items))
                {
                    item.Items = ListItems2(Keys, "1")?.ToList();
                }
                if (RelationEnum.Equals(BEA.relDeliveryNote.Notes))
                {
                    using Note dalNote = new();
                    item.Notes = dalNote.ReturnChild(Keys, Relations)?.ToList();
                }
                if (RelationEnum.Equals(BEA.relDeliveryNote.Files))
                {
                    item.Files = ListFiles(Keys)?.ToList();
                }
            }
        }

        #endregion

        #region List Methods

        public IEnumerable<BEA.DeliveryNote> List(List<Field> FilterList, string OrderBy, params Enum[] Relations)
        {
            StringBuilder sb = new();
            var filter = GetFilter(FilterList?.ToArray());
            sb.AppendLine($@"SELECT  * ");
            sb.AppendLine($@"FROM    ({Query}) a ");
            sb.AppendLine($@"WHERE   {filter} ");
            sb.AppendLine($@"Order By {GetOrder(OrderBy)} ");

            IEnumerable<BEA.DeliveryNote> items = SQLList(sb.ToString(), Relations);
            return items;
        }

        public IEnumerable<BEA.DeliveryNote> ListOpen(List<Field> Filters, List<Field> InnerFilters, string OrderBy, params Enum[] Relations)
        {
            string filter = Filters?.Count > 0 ? GetFilter(Filters?.ToArray()) : "1 = 1", innerFilter = InnerFilters?.Count > 0 ? $" AND {GetFilter(InnerFilters?.ToArray())}" : "", query;
            query = $@"SELECT  *
FROM    ( SELECT  t0.""DocEntry"" AS ""Id"", ""DocNum"" AS ""DocNumber"", 'Santa Cruz' AS ""Subsidiary"", t2.""WhsName"" AS ""Warehouse"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", ""CardCode"" AS ""ClientCode""
                  , ""CardName"" AS ""ClientName"", t3.""SlpName"" AS ""SellerCode"", t3.""Memo"" AS ""SellerName"", ""DocTotal"" AS ""Total"", ""Margin"", ""TaxlessTotal""
          FROM    {DBSA}.ODLN t0
                  INNER JOIN ( SELECT s0.""DocEntry"", FIRST_VALUE(""WhsCode"" ORDER BY ""LineNum"") AS ""WhsCode""
                                       , SUM(( CASE WHEN ""WhsCode"" = 'ZFSCZ' THEN ""Quantity"" * ( ""PriceAfVAT"" - ""StockPrice"" ) ELSE ""Quantity"" * (( 0.84 * ""PriceAfVAT"" ) - ""StockPrice"" ) END )) AS ""Margin""
                                       , SUM(( CASE WHEN ""WhsCode"" = 'ZFSCZ' THEN ""Quantity"" * ""PriceAfVAT"" ELSE ""Quantity"" * ( 0.84 * ""PriceAfVAT"" ) END )) AS ""TaxlessTotal""
                               FROM   {DBSA}.DLN1 s0
                                      INNER JOIN {DBSA}.OITM s1 ON s0.""ItemCode"" = s1.""ItemCode"" 
                               WHERE  ""BaseType"" = 17
                               GROUP BY s0.""DocEntry"" ) t1 ON t1.""DocEntry"" = t0.""DocEntry""
                  INNER JOIN {DBSA}.OWHS t2 ON t1.""WhsCode"" = t2.""WhsCode""
                  INNER JOIN {DBSA}.OSLP t3 ON t0.""SlpCode"" = t3.""SlpCode"" 
          WHERE   ""DocStatus"" = 'O'
                  AND ""DocTotal"" > 0
                  AND EXISTS ( SELECT * FROM {DBSA}.DLN1 s0 INNER JOIN {DBSA}.OITM s1 ON s0.""ItemCode"" = s1.""ItemCode"" WHERE s0.""DocEntry"" = t0.""DocEntry"" {innerFilter} ) 
          UNION
          SELECT  t0.""DocEntry"" AS ""Id"", ""DocNum"" AS ""DocNumber"", 'Miami' AS ""Subsidiary"", t2.""WhsName"" AS ""Warehouse"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", ""CardCode"" AS ""ClientCode""
                  , ""CardName"" AS ""ClientName"", t3.""SlpName"" AS ""SellerCode"", t3.""Memo"" AS ""SellerName"", ""DocTotal"" AS ""Total"", ""Margin"", ""TaxlessTotal""
          FROM    {DBLA}.ODLN t0
                  INNER JOIN ( SELECT s0.""DocEntry"", FIRST_VALUE(""WhsCode"" ORDER BY ""LineNum"") AS ""WhsCode""
                                       , SUM(( CASE WHEN ""WhsCode"" = 'ZFSCZ' THEN ""Quantity"" * ( ""PriceAfVAT"" - ""StockPrice"" ) ELSE ""Quantity"" * (( 0.84 * ""PriceAfVAT"" ) - ""StockPrice"" ) END )) AS ""Margin""
                                       , SUM(( CASE WHEN ""WhsCode"" = 'ZFSCZ' THEN ""Quantity"" * ""PriceAfVAT"" ELSE ""Quantity"" * ( 0.84 * ""PriceAfVAT"" ) END )) AS ""TaxlessTotal""
                               FROM   {DBLA}.DLN1 s0
                                      INNER JOIN {DBLA}.OITM s1 ON s0.""ItemCode"" = s1.""ItemCode"" 
                               WHERE  ""BaseType"" = 17
                               GROUP BY s0.""DocEntry"" ) t1 ON t1.""DocEntry"" = t0.""DocEntry""
                  INNER JOIN {DBLA}.OWHS t2 ON t1.""WhsCode"" = t2.""WhsCode""
                  INNER JOIN {DBLA}.OSLP t3 ON t0.""SlpCode"" = t3.""SlpCode"" 
          WHERE   ""DocStatus"" = 'O'
                  AND ""DocTotal"" > 0
                  AND EXISTS ( SELECT * FROM {DBLA}.DLN1 s0 INNER JOIN {DBLA}.OITM s1 ON s0.""ItemCode"" = s1.""ItemCode"" WHERE s0.""DocEntry"" = t0.""DocEntry"" {innerFilter} )
          UNION
          SELECT  t0.""DocEntry"" AS ""Id"", ""DocNum"" AS ""DocNumber"", 'Iquique' AS ""Subsidiary"", t2.""WhsName"" AS ""Warehouse"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", ""CardCode"" AS ""ClientCode""
                  , ""CardName"" AS ""ClientName"", t3.""SlpName"" AS ""SellerCode"", t3.""Memo"" AS ""SellerName"", ""DocTotal"" AS ""Total"", ""Margin"", ""TaxlessTotal""
          FROM    {DBIQ}.ODLN t0
                  INNER JOIN ( SELECT s0.""DocEntry"", FIRST_VALUE(""WhsCode"" ORDER BY ""LineNum"") AS ""WhsCode""
                                       , SUM(( CASE WHEN ""WhsCode"" = 'ZFSCZ' THEN ""Quantity"" * ( ""PriceAfVAT"" - ""StockPrice"" ) ELSE ""Quantity"" * (( 0.84 * ""PriceAfVAT"" ) - ""StockPrice"" ) END )) AS ""Margin""
                                       , SUM(( CASE WHEN ""WhsCode"" = 'ZFSCZ' THEN ""Quantity"" * ""PriceAfVAT"" ELSE ""Quantity"" * ( 0.84 * ""PriceAfVAT"" ) END )) AS ""TaxlessTotal""
                               FROM   {DBIQ}.DLN1 s0
                                      INNER JOIN {DBIQ}.OITM s1 ON s0.""ItemCode"" = s1.""ItemCode"" 
                               WHERE  ""BaseType"" = 17
                               GROUP BY s0.""DocEntry"" ) t1 ON t1.""DocEntry"" = t0.""DocEntry""
                  INNER JOIN {DBIQ}.OWHS t2 ON t1.""WhsCode"" = t2.""WhsCode""
                  INNER JOIN {DBIQ}.OSLP t3 ON t0.""SlpCode"" = t3.""SlpCode"" 
          WHERE   ""DocStatus"" = 'O'
                  AND ""DocTotal"" > 0
                  AND EXISTS ( SELECT * FROM {DBIQ}.DLN1 s0 INNER JOIN {DBIQ}.OITM s1 ON s0.""ItemCode"" = s1.""ItemCode"" WHERE s0.""DocEntry"" = t0.""DocEntry"" {innerFilter} ) ) a
WHERE   {filter}
ORDER BY {GetOrder(OrderBy)} ";

            IEnumerable<BEA.DeliveryNote> items = SQLList(query, Relations);
            return items;
        }

        public IEnumerable<BEA.DeliveryNote> List(List<Field> FilterList, List<Field> ItemFilters, string OrderBy, params Enum[] Relations)
        {
            string filter = GetFilter(FilterList?.ToArray()), itemFilter = ItemFilters?.Count > 0 ? $"AND {GetFilter(ItemFilters?.ToArray())}" : "";
            string query = $@"SELECT  *
                              FROM    ( SELECT  t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", 'Santa Cruz' AS ""Subsidiary"", t1.""WhsName"" AS ""Warehouse"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"" 
                                                , t0.""SlpCode"", t2.""SlpName"" AS ""SellerCode"", t2.""Memo"" AS ""SellerName"", t0.""DocTotal"" AS ""Total"", t4.""Margin"", t4.""TaxlessTotal""
                                        FROM    {DBSA}.ODLN t0 
                                                INNER JOIN {DBSA}.OSLP t2 ON t0.""SlpCode"" = t2.""SlpCode"" 
                                                INNER JOIN ( SELECT s0.""DocEntry"", FIRST_VALUE(""WhsCode"" ORDER BY ""LineNum"") AS ""WhsCode""
                                                                     , SUM(( CASE WHEN ""WhsCode"" = 'ZFSCZ' THEN ""Quantity"" * ( ""PriceAfVAT"" - ""StockPrice"" ) ELSE ""Quantity"" * (( 0.84 * ""PriceAfVAT"" ) - ""StockPrice"" ) END )) AS ""Margin""
                                                                     , SUM(( CASE WHEN ""WhsCode"" = 'ZFSCZ' THEN ""Quantity"" * ""PriceAfVAT"" ELSE ""Quantity"" * ( 0.84 * ""PriceAfVAT"" ) END )) AS ""TaxlessTotal""
                                                             FROM   {DBSA}.DLN1 s0
                                                                    INNER JOIN {DBSA}.OITM s1 ON s0.""ItemCode"" = s1.""ItemCode"" {itemFilter}
                                                             WHERE  ""BaseType"" IN ( 13, 17 )
                                                             GROUP BY s0.""DocEntry"" ) t4 ON t4.""DocEntry"" = t0.""DocEntry""
                                                INNER JOIN {DBSA}.OWHS t1 ON t4.""WhsCode"" = t1.""WhsCode""
                                        WHERE   t0.CANCELED = 'N'
                                        UNION 
                                        SELECT  t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", 'Miami' AS ""Subsidiary"", t1.""WhsName"" AS ""Warehouse"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"" 
                                                , t0.""SlpCode"", t2.""SlpName"" AS ""SellerCode"", t2.""Memo"" AS ""SellerName"", t0.""DocTotal"" AS ""Total"", t4.""Margin"", t4.""TaxlessTotal""
                                        FROM    {DBLA}.ODLN t0                               
                                                INNER JOIN {DBLA}.OSLP t2 ON t0.""SlpCode"" = t2.""SlpCode"" 
                                                INNER JOIN ( SELECT s0.""DocEntry"", FIRST_VALUE(""WhsCode"" ORDER BY ""LineNum"") AS ""WhsCode"", SUM(""Quantity"" * ( ""PriceAfVAT"" - ""StockPrice"" )) AS ""Margin"", SUM(""Quantity"" * ""PriceAfVAT"") AS ""TaxlessTotal""
                                                             FROM   {DBLA}.DLN1 s0
                                                                    INNER JOIN {DBLA}.OITM s1 ON s0.""ItemCode"" = s1.""ItemCode"" {itemFilter}
                                                             WHERE  ""BaseType"" IN ( 13, 17 )
                                                             GROUP BY s0.""DocEntry"" ) t4 ON t4.""DocEntry"" = t0.""DocEntry""
                                                INNER JOIN {DBLA}.OWHS t1 ON t4.""WhsCode"" = t1.""WhsCode"" 
                                        WHERE   t0.CANCELED = 'N' 
                                        UNION 
                                        SELECT  t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", 'Iquique' AS ""Subsidiary"", t1.""WhsName"" AS ""Warehouse"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"" 
                                                , t0.""SlpCode"", t2.""SlpName"" AS ""SellerCode"", t2.""Memo"" AS ""SellerName"", t0.""DocTotal"" AS ""Total"", t4.""Margin"", t4.""TaxlessTotal""
                                        FROM    {DBIQ}.ODLN t0                               
                                                INNER JOIN {DBIQ}.OSLP t2 ON t0.""SlpCode"" = t2.""SlpCode"" 
                                                INNER JOIN ( SELECT s0.""DocEntry"", FIRST_VALUE(""WhsCode"" ORDER BY ""LineNum"") AS ""WhsCode"", SUM(""Quantity"" * ( ""PriceAfVAT"" - ""StockPrice"" )) AS ""Margin"", SUM(""Quantity"" * ""PriceAfVAT"") AS ""TaxlessTotal""
                                                             FROM   {DBIQ}.DLN1 s0 
                                                                    INNER JOIN {DBIQ}.OITM s1 ON s0.""ItemCode"" = s1.""ItemCode"" {itemFilter}
                                                             WHERE  ""BaseType"" IN ( 13, 17 )
                                                             GROUP BY s0.""DocEntry"" ) t4 ON t4.""DocEntry"" = t0.""DocEntry""
                                                INNER JOIN {DBIQ}.OWHS t1 ON t4.""WhsCode"" = t1.""WhsCode"" 
                                        WHERE   t0.CANCELED = 'N' ) a
                              WHERE   {filter} 
                              ORDER BY {GetOrder(OrderBy)} ";

            IEnumerable<BEA.DeliveryNote> items = SQLList(query, Relations);
            return items;
        }

        public IEnumerable<BEA.DeliveryNoteItem> ListItems(List<Field> FilterList, string OrderBy)
        {
            var filter = GetFilter(FilterList?.ToArray());
            var sb = new StringBuilder();
            sb.AppendLine($@"SELECT  * ");
            sb.AppendLine($@"FROM    ( SELECT    t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", t1.""BaseRef"" AS ""NoteNumber"", 'Santa Cruz' AS ""Subsidiary"", t1.""ItemCode"", t2.""SuppCatNum"" AS ""BrandCode"", t1.""Dscription"" AS ""ItemName"" ");
            sb.AppendLine($@"					, CAST(t1.""Quantity"" AS INT) AS ""Quantity"", t1.""unitMsr"" AS ""Unit"", t1.""PriceAfVAT"" AS ""Price"", t1.U_TPOGTA AS ""Warranty"", t1.""LineNum"" ");
            sb.AppendLine($@"          FROM      {DBSA}.ODLN t0 ");
            sb.AppendLine($@"          			INNER JOIN {DBSA}.DLN1 t1 ON t1.""DocEntry"" = t0.""DocEntry"" ");
            sb.AppendLine($@"          			INNER JOIN {DBSA}.OITM t2 ON t2.""ItemCode"" = t1.""ItemCode"" ");
            sb.AppendLine($@"          WHERE     t0.CANCELED = 'N' ");
            sb.AppendLine($@"                    AND t1.""TargetType"" <> 1 ");
            sb.AppendLine($@"          UNION ALL ");
            sb.AppendLine($@"          SELECT    t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", t1.""BaseRef"" AS ""NoteNumber"", 'Iquique' AS ""Subsidiary"", t1.""ItemCode"", t2.""SuppCatNum"" AS ""BrandCode"", t1.""Dscription"" AS ""ItemName"" ");
            sb.AppendLine($@"					, CAST(t1.""Quantity"" AS INT) AS ""Quantity"", t1.""unitMsr"" AS ""Unit"", t1.""PriceAfVAT"" AS ""Price"", t1.U_TPOGTA AS ""Warranty"", t1.""LineNum"" ");
            sb.AppendLine($@"          FROM      {DBIQ}.ODLN t0 ");
            sb.AppendLine($@"          			INNER JOIN {DBIQ}.DLN1 t1 ON t1.""DocEntry"" = t0.""DocEntry"" ");
            sb.AppendLine($@"          			INNER JOIN {DBIQ}.OITM t2 ON t2.""ItemCode"" = t1.""ItemCode"" ");
            sb.AppendLine($@"          WHERE     t0.CANCELED = 'N' ");
            sb.AppendLine($@"                    AND t1.""TargetType"" <> 1 ");
            sb.AppendLine($@"          UNION ALL ");
            sb.AppendLine($@"          SELECT    t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", t1.""BaseRef"" AS ""NoteNumber"", 'Miami' AS ""Subsidiary"", t1.""ItemCode"", t2.""SuppCatNum"" AS ""BrandCode"", t1.""Dscription"" AS ""ItemName"" ");
            sb.AppendLine($@"					, CAST(t1.""Quantity"" AS INT) AS ""Quantity"", t1.""unitMsr"" AS ""Unit"", t1.""PriceAfVAT"" AS ""Price"", t1.U_TPOGTA AS ""Warranty"", t1.""LineNum"" ");
            sb.AppendLine($@"          FROM      {DBLA}.ODLN t0 ");
            sb.AppendLine($@"          			INNER JOIN {DBLA}.DLN1 t1 ON t1.""DocEntry"" = t0.""DocEntry"" ");
            sb.AppendLine($@"          			INNER JOIN {DBLA}.OITM t2 ON t2.""ItemCode"" = t1.""ItemCode"" ");
            sb.AppendLine($@"          WHERE     t0.CANCELED = 'N' ");
            sb.AppendLine($@"                    AND t1.""TargetType"" <> 1 ) a    ");
            sb.AppendLine($@"WHERE   {filter} ");
            sb.AppendLine($@"Order By {GetOrder(OrderBy)} ");

            IEnumerable<BEA.DeliveryNoteItem> items = SQLList<BEA.DeliveryNoteItem>(sb.ToString());
            return items;
        }

        public IEnumerable<BEA.DeliveryNoteItem> ListItems2(IEnumerable<string> Keys, string OrderBy)
        {
            string query = $@"SELECT  * 
                              FROM    ( SELECT    t1.""LineNum"", t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", t1.""BaseRef"" AS ""NoteNumber"", 'Santa Cruz' AS ""Subsidiary"", t3.""WhsName"" AS ""Warehouse"", t1.""ItemCode"", t2.""SuppCatNum"" AS ""BrandCode""
                                                  , t1.""Dscription"" AS ""ItemName"", t2.""U_LINEA"" AS ""Line"", t1.U_TPOGTA AS ""Warranty"", CAST(t1.""Quantity"" AS INT) AS ""Quantity"", CAST(IFNULL(t4.""OnHand"", 0) AS INT) AS ""Stock"", t1.""unitMsr"" AS ""Unit""
                                                  , t1.""PriceAfVAT"" AS ""Price"", t1.""PriceAfVAT"" * CAST(t1.""Quantity"" AS INT) AS ""Total""
                                                  , CASE WHEN t1.""WhsCode"" = 'ZFSCZ' THEN t1.""Quantity"" * ( t1.""PriceAfVAT"" - t1.""StockPrice"" ) ELSE t1.""Quantity"" * (( 0.84 * t1.""PriceAfVAT"" ) - t1.""StockPrice"" ) END AS ""Margin""
                                                  , CASE WHEN t1.""WhsCode"" = 'ZFSCZ' THEN t1.""PriceAfVAT"" * t1.""Quantity"" ELSE t1.""PriceAfVAT"" * t1.""Quantity"" * 0.84 END AS ""TaxlessTotal""
                                        FROM      {DBSA}.ODLN t0 
                                                  INNER JOIN {DBSA}.DLN1 t1 ON t1.""DocEntry"" = t0.""DocEntry"" 
                                                  INNER JOIN {DBSA}.OITM t2 ON t2.""ItemCode"" = t1.""ItemCode"" 
                                                  INNER JOIN {DBSA}.OWHS t3 ON t1.""WhsCode"" = t3.""WhsCode""
                                                  LEFT OUTER JOIN {DBSA}.OITW t4 ON t4.""ItemCode"" = t1.""ItemCode"" AND t4.""WhsCode"" = t1.""WhsCode""
                                        WHERE     t0.CANCELED = 'N' 
                                                  AND t1.""TargetType"" <> 1 
                                        UNION ALL 
                                        SELECT    t1.""LineNum"", t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", t1.""BaseRef"" AS ""NoteNumber"", 'Iquique' AS ""Subsidiary"", t3.""WhsName"" AS ""Warehouse"", t1.""ItemCode"", t2.""SuppCatNum"" AS ""BrandCode""
                                                  , t1.""Dscription"" AS ""ItemName"", t2.""U_LINEA"" AS ""Line"", t1.U_TPOGTA AS ""Warranty"", CAST(t1.""Quantity"" AS INT) AS ""Quantity"", CAST(IFNULL(t4.""OnHand"", 0) AS INT) AS ""Stock"", t1.""unitMsr"" AS ""Unit""
                                                  , t1.""PriceAfVAT"" AS ""Price"", t1.""PriceAfVAT"" * CAST(t1.""Quantity"" AS INT) AS ""Total"", t1.""Quantity"" * ( t1.""PriceAfVAT"" - t1.""StockPrice"" ) AS ""Margin""
                                                  , t1.""PriceAfVAT"" * t1.""Quantity"" AS ""TaxlessTotal""
                                        FROM      {DBIQ}.ODLN t0 
                                                  INNER JOIN {DBIQ}.DLN1 t1 ON t1.""DocEntry"" = t0.""DocEntry"" 
                                                  INNER JOIN {DBIQ}.OITM t2 ON t2.""ItemCode"" = t1.""ItemCode""
                                                  INNER JOIN {DBIQ}.OWHS t3 ON t1.""WhsCode"" = t3.""WhsCode""
                                                  LEFT OUTER JOIN {DBIQ}.OITW t4 ON t4.""ItemCode"" = t1.""ItemCode"" AND t4.""WhsCode"" = t1.""WhsCode""
                                        WHERE     t0.CANCELED = 'N' 
                                                  AND t1.""TargetType"" <> 1 
                                        UNION ALL 
                                        SELECT    t1.""LineNum"", t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", t1.""BaseRef"" AS ""NoteNumber"", 'Miami' AS ""Subsidiary"", t3.""WhsName"" AS ""Warehouse"", t1.""ItemCode"", t2.""SuppCatNum"" AS ""BrandCode""
                                                  , t1.""Dscription"" AS ""ItemName"", t2.""U_LINEA"" AS ""Line"", t1.U_TPOGTA AS ""Warranty"", CAST(t1.""Quantity"" AS INT) AS ""Quantity"", CAST(IFNULL(t4.""OnHand"", 0) AS INT) AS ""Stock"", t1.""unitMsr"" AS ""Unit""
                                                  , t1.""PriceAfVAT"" AS ""Price"", t1.""PriceAfVAT"" * CAST(t1.""Quantity"" AS INT) AS ""Total"", t1.""Quantity"" * ( t1.""PriceAfVAT"" - t1.""StockPrice"" ) AS ""Margin""
                                                  , t1.""PriceAfVAT"" * t1.""Quantity"" AS ""TaxlessTotal""
                                        FROM      {DBLA}.ODLN t0 
                                                  INNER JOIN {DBLA}.DLN1 t1 ON t1.""DocEntry"" = t0.""DocEntry"" 
                                                  INNER JOIN {DBLA}.OITM t2 ON t2.""ItemCode"" = t1.""ItemCode"" 
                                                  INNER JOIN {DBLA}.OWHS t3 ON t1.""WhsCode"" = t3.""WhsCode""
                                                  LEFT OUTER JOIN {DBLA}.OITW t4 ON t4.""ItemCode"" = t1.""ItemCode"" AND t4.""WhsCode"" = t1.""WhsCode""
                                        WHERE     t0.CANCELED = 'N' 
                                                  AND t1.""TargetType"" <> 1 ) a    
                              WHERE    LOWER(""Subsidiary"") || '-' || CAST(""Id"" AS VARCHAR(10)) IN ( {string.Join(",", Keys)} ) 
                              ORDER BY {GetOrder(OrderBy)} ";

            IEnumerable<BEA.DeliveryNoteItem> items = SQLList<BEA.DeliveryNoteItem>(query);
            return items;
        }

        public IEnumerable<BEA.DocumentRelated> ListRelatedDocuments(string Subsidiary, string DocIds)
        {
            string dbName = Subsidiary.ToLower() == "santa cruz" ? DBSA : (Subsidiary.ToLower() == "iquique" ? DBIQ : DBLA);
            string query = $@"SELECT  DISTINCT '{Subsidiary}' AS ""Subsidiary"", t0.""DocEntry"" AS ""Id"", 'Orden de Venta' AS ""DocType"", t0.""DocNum"" AS ""DocNumber"", t2.""DocEntry"" AS ""BaseId"", 0 AS ""Series""
                              FROM    {dbName}.ORDR t0
                                      INNER JOIN {dbName}.RDR1 t1 ON t0.""DocEntry"" = t1.""DocEntry"" AND t1.""TargetType"" = 15
                                      INNER JOIN {dbName}.DLN1 t2 ON t1.""TrgetEntry"" = t2.""DocEntry"" AND t2.""BaseType"" = 17 AND t2.""DocEntry"" IN ( {DocIds} )   
                              WHERE   t0.CANCELED = 'N'            
                              UNION
                              SELECT  DISTINCT '{Subsidiary}' AS ""Subsidiary"", t0.""DocEntry"" AS ""Id"", 'Nota de Venta' AS ""DocType"", t0.""DocNum"" AS ""DocNumber"", t2.""DocEntry"" AS ""BaseId"", t0.""Series""
                              FROM    {dbName}.OINV t0
                                      INNER JOIN {dbName}.INV1 t1 ON t0.""DocEntry"" = t1.""DocEntry"" AND t1.""BaseType"" = 15
                                      INNER JOIN {dbName}.DLN1 t2 ON t1.""BaseEntry"" = t2.""DocEntry"" AND t2.""TargetType"" = 13 AND t2.""DocEntry"" IN ( {DocIds} ) 
                              WHERE   t0.CANCELED = 'N' 
                              UNION
                              SELECT  DISTINCT '{Subsidiary}' AS ""Subsidiary"", t0.""DocEntry"" AS ""Id"", 'Nota de Venta' AS ""DocType"", t0.""DocNum"" AS ""DocNumber"", t2.""DocEntry"" AS ""BaseId"", t0.""Series""
                              FROM    {dbName}.OINV t0
                                      INNER JOIN {dbName}.INV1 t1 ON t0.""DocEntry"" = t1.""DocEntry"" AND t1.""TargetType"" = 15
                                      INNER JOIN {dbName}.DLN1 t2 ON t1.""TrgetEntry"" = t2.""DocEntry"" AND t2.""BaseType"" = 13 AND t2.""DocEntry"" IN ( {DocIds} ) 
                              WHERE   t0.CANCELED = 'N' ";
            IEnumerable<BEA.DocumentRelated> items = SQLList<BEA.DocumentRelated>(query);
            return items;
        }

        public IEnumerable<BEA.OrderFile> ListFiles(IEnumerable<string> Keys)
        {
            string query = $@"SELECT  *
                              FROM    ( SELECT  'Santa Cruz' AS ""Subsidiary"", t0.""DocEntry"", CAST(IFNULL(T1.""trgtPath"", '') AS varchar(5000)) AS ""Path"", IFNULL(T1.""FileName"", '') AS ""FileName"", IFNULL(T1.""FileExt"", '') AS ""FileExt"" 
                                        FROM    {DBSA}.ODLN t0 
                                                INNER JOIN {DBSA}.ATC1 T1 ON T1.""AbsEntry"" = t0.""AtcEntry"" 
                                        UNION 
                                        SELECT  'Iquique', t0.""DocEntry"", CAST(IFNULL(T1.""trgtPath"", '') AS varchar(5000)) AS ""Path"", IFNULL(T1.""FileName"", '') AS ""FileName"", IFNULL(T1.""FileExt"", '') AS ""FileExt"" 
                                        FROM    {DBIQ}.ODLN t0 
                                                INNER JOIN {DBIQ}.ATC1 T1 ON T1.""AbsEntry"" = t0.""AtcEntry"" 
                                        UNION 
                                        SELECT  'Miami', t0.""DocEntry"", CAST(IFNULL(T1.""trgtPath"", '') AS varchar(5000)) AS ""Path"", IFNULL(T1.""FileName"", '') AS ""FileName"", IFNULL(T1.""FileExt"", '') AS ""FileExt"" 
                                        FROM    {DBLA}.ODLN t0 
                                                INNER JOIN {DBLA}.ATC1 T1 ON T1.""AbsEntry"" = t0.""AtcEntry"" ) a
                              WHERE   (LOWER(a.""Subsidiary"") || '-' || CAST(a.""DocEntry"" AS VARCHAR(10))) IN ( {string.Join(",", Keys.Select(x => $"'{x}'"))} ) ";
            IEnumerable<BEA.OrderFile> items = SQLList<BEA.OrderFile>(query);
            return items;
        }

        #endregion

        #region Search Methods

        public BEA.DeliveryNote Search(int DocNumber, string Subsidiary, params Enum[] Relations)
        {
            string strQuery = $@"SELECT * FROM ({Query}) a WHERE ""DocNumber"" = {DocNumber} AND LOWER(""Subsidiary"") = '{Subsidiary.ToLower()}' ";
            BEA.DeliveryNote item = SQLSearch(strQuery, Relations);
            return item;
        }

        public BEA.DeliveryNote SearchById(int Id, string Subsidiary, params Enum[] Relations)
        {
            string strQuery = $@"SELECT * FROM ({Query}) a WHERE ""Id"" = {Id} AND LOWER(""Subsidiary"") = '{Subsidiary.ToLower()}' ";
            BEA.DeliveryNote item = SQLSearch(strQuery, Relations);
            return item;
        }

        public BEA.DeliveryNote SearchBySaleNote(int DocNumber, string Subsidiary, params Enum[] Relations)
        {
            string strQuery = $@"SELECT * FROM ({Query}) a WHERE ""NoteNumber"" = '{DocNumber}' AND LOWER(""Subsidiary"") = '{Subsidiary.ToLower()}' ";
            BEA.DeliveryNote item = SQLSearch(strQuery, Relations);
            return item;
        }

        #endregion

        #region Private Methods

        //private string GetQuery()
        //{
        //    var sb = new StringBuilder();
        //    sb.AppendLine($@"SELECT  t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", 'Santa Cruz' AS ""Subsidiary"", t1.""WhsName"" AS ""Warehouse"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"" ");
        //    sb.AppendLine($@"        , t0.""NumAtCard"" AS ""ClientNote"", t2.""SlpName"" AS ""SellerCode"", t2.""Memo"" AS ""SellerName"", t0.""DocTotal"" AS ""Total"", t4.""BaseEntry"" AS ""NoteId"", t4.""BaseRef"" AS ""NoteNumber"" ");
        //    sb.AppendLine($@"        , t0.U_RAZSOC AS ""BillName"", t0.U_NIT AS ""NIT"", t3.""Phone1"" AS ""Phone"", t3.""Cellular"" AS ""Mobile"", t0.U_ACREDIT AS ""Accredited"", t0.""Address"" AS ""ClientAddress"", t0.""Comments"" ");
        //    sb.AppendLine($@"        , t0.""Address2"" AS ""DeliveryAddress"", t5.""Name"" AS ""DeliveryName"", t5.""Tel1"" AS ""DeliveryPhone"", t5.""Cellolar"" AS ""DeliveryMobile"", t0.U_INCOTERMS AS ""Incoterms"" ");
        //    sb.AppendLine($@"        , t0.U_CORRELATIVO AS ""Correlative"", t6.""TrnspName"" AS ""Transport"", t7.""PymntGroup"" AS ""Terms"", t0.""DiscSum"" AS ""Discount"" ");
        //    sb.AppendLine($@"FROM    {DBSA}.ODLN t0 ");
        //    sb.AppendLine($@"        INNER JOIN {DBSA}.OWHS t1 ON t1.""WhsCode"" = t0.U_ALMACEN ");
        //    sb.AppendLine($@"        INNER JOIN {DBSA}.OSLP t2 ON t0.""SlpCode"" = t2.""SlpCode"" ");
        //    sb.AppendLine($@"        INNER JOIN {DBSA}.OCRD t3 ON t3.""CardCode"" = t0.""CardCode"" ");
        //    sb.AppendLine($@"        INNER JOIN {DBSA}.DLN1 t4 ON t4.""DocEntry"" = t0.""DocEntry"" ");
        //    sb.AppendLine($@"        LEFT OUTER JOIN {DBSA}.OCPR t5 ON t5.""CntctCode"" = t0.""CntctCode"" ");
        //    sb.AppendLine($@"        LEFT OUTER JOIN {DBSA}.OSHP t6 ON t6.""TrnspCode"" = t0.""TrnspCode"" ");
        //    sb.AppendLine($@"        INNER JOIN {DBSA}.OCTG t7 ON t7.""GroupNum"" = t0.""GroupNum"" ");
        //    sb.AppendLine($@"WHERE   t0.CANCELED = 'N' AND   t4.""TargetType"" <> 14 ");
        //    sb.AppendLine($@"UNION ");
        //    sb.AppendLine($@"SELECT  t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", 'Miami' AS ""Subsidiary"", t1.""WhsName"" AS ""Warehouse"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"" ");
        //    sb.AppendLine($@"        , t0.""NumAtCard"" AS ""ClientNote"", t2.""SlpName"" AS ""SellerCode"", t2.""Memo"" AS ""SellerName"", t0.""DocTotal"" AS ""Total"", t4.""BaseEntry"" AS ""NoteId"", t4.""BaseRef"" AS ""NoteNumber"" ");
        //    sb.AppendLine($@"        , '' AS ""BillName"", t0.U_NIT AS ""NIT"", t3.""Phone1"" AS ""Phone"", t3.""Cellular"" AS ""Mobile"", t0.U_ACREDIT AS ""Accredited"", t0.""Address"" AS ""ClientAddress"", t0.""Comments"" ");
        //    sb.AppendLine($@"        , t0.""Address2"" AS ""DeliveryAddress"", t5.""Name"" AS ""DeliveryName"", t5.""Tel1"" AS ""DeliveryPhone"", t5.""Cellolar"" AS ""DeliveryMobile"", t0.U_INCOTERMS AS ""Incoterms"" ");
        //    sb.AppendLine($@"        , t0.U_CORRELATIVO AS ""Correlative"", t6.""TrnspName"" AS ""Transport"", t7.""PymntGroup"" AS ""Terms"", t0.""DiscSum"" AS ""Discount"" ");
        //    sb.AppendLine($@"FROM    {DBLA}.ODLN t0 ");
        //    sb.AppendLine($@"        INNER JOIN {DBLA}.OWHS t1 ON t1.""WhsCode"" = t0.U_ALMACEN ");
        //    sb.AppendLine($@"        INNER JOIN {DBLA}.OSLP t2 ON t0.""SlpCode"" = t2.""SlpCode"" ");
        //    sb.AppendLine($@"        INNER JOIN {DBLA}.OCRD t3 ON t3.""CardCode"" = t0.""CardCode"" ");
        //    sb.AppendLine($@"        INNER JOIN {DBLA}.DLN1 t4 ON t4.""DocEntry"" = t0.""DocEntry"" ");
        //    sb.AppendLine($@"        LEFT OUTER JOIN {DBLA}.OCPR t5 ON t5.""CntctCode"" = t0.""CntctCode"" ");
        //    sb.AppendLine($@"        LEFT OUTER JOIN {DBLA}.OSHP t6 ON t6.""TrnspCode"" = t0.""TrnspCode"" ");
        //    sb.AppendLine($@"        INNER JOIN {DBLA}.OCTG t7 ON t7.""GroupNum"" = t0.""GroupNum"" ");
        //    sb.AppendLine($@"WHERE   t0.CANCELED = 'N' AND   t4.""TargetType"" <> 14 ");
        //    sb.AppendLine($@"UNION ");
        //    sb.AppendLine($@"SELECT  t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", 'Iquique' AS ""Subsidiary"", t1.""WhsName"" AS ""Warehouse"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"" ");
        //    sb.AppendLine($@"        , t0.""NumAtCard"" AS ""ClientNote"", t2.""SlpName"" AS ""SellerCode"", t2.""Memo"" AS ""SellerName"", t0.""DocTotal"" AS ""Total"", t4.""BaseEntry"" AS ""NoteId"", t4.""BaseRef"" AS ""NoteNumber"" ");
        //    sb.AppendLine($@"        , '' AS ""BillName"", t0.U_NIT AS ""NIT"", t3.""Phone1"" AS ""Phone"", t3.""Cellular"" AS ""Mobile"", t0.U_ACREDIT AS ""Accredited"", t0.""Address"" AS ""ClientAddress"", t0.""Comments"" ");
        //    sb.AppendLine($@"        , t0.""Address2"" AS ""DeliveryAddress"", t5.""Name"" AS ""DeliveryName"", t5.""Tel1"" AS ""DeliveryPhone"", t5.""Cellolar"" AS ""DeliveryMobile"", t0.U_INCOTERMS AS ""Incoterms"" ");
        //    sb.AppendLine($@"        , t0.U_CORRELATIVO AS ""Correlative"", t6.""TrnspName"" AS ""Transport"", t7.""PymntGroup"" AS ""Terms"", t0.""DiscSum"" AS ""Discount"" ");
        //    sb.AppendLine($@"FROM    {DBIQ}.ODLN t0 ");
        //    sb.AppendLine($@"        INNER JOIN {DBIQ}.OWHS t1 ON t1.""WhsCode"" = t0.U_ALMACEN ");
        //    sb.AppendLine($@"        INNER JOIN {DBIQ}.OSLP t2 ON t0.""SlpCode"" = t2.""SlpCode"" ");
        //    sb.AppendLine($@"        INNER JOIN {DBIQ}.OCRD t3 ON t3.""CardCode"" = t0.""CardCode"" ");
        //    sb.AppendLine($@"        INNER JOIN {DBIQ}.DLN1 t4 ON t4.""DocEntry"" = t0.""DocEntry"" ");
        //    sb.AppendLine($@"        LEFT OUTER JOIN {DBIQ}.OCPR t5 ON t5.""CntctCode"" = t0.""CntctCode"" ");
        //    sb.AppendLine($@"        LEFT OUTER JOIN {DBIQ}.OSHP t6 ON t6.""TrnspCode"" = t0.""TrnspCode"" ");
        //    sb.AppendLine($@"        INNER JOIN {DBIQ}.OCTG t7 ON t7.""GroupNum"" = t0.""GroupNum"" ");
        //    sb.AppendLine($@"WHERE   t0.CANCELED = 'N' AND   t4.""TargetType"" <> 14 ");
        //    return sb.ToString();
        //}

        #endregion

        #region Constructors

        public DeliveryNote() : base()
        {
            Query = $@"SELECT  t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", 'Santa Cruz' AS ""Subsidiary"", t1.""WhsName"" AS ""Warehouse"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"" 
                               , t0.""NumAtCard"" AS ""ClientNote"", t2.""SlpName"" AS ""SellerCode"", t2.""Memo"" AS ""SellerName"", t0.""DocTotal"" AS ""Total"", t8.""DocEntry"" AS ""NoteId"", CAST(t9.""DocNum"" AS VARCHAR(10)) AS ""NoteNumber"" 
                               , t0.U_RAZSOC AS ""BillName"", t0.U_NIT AS ""NIT"", t3.""Phone1"" AS ""Phone"", t3.""Cellular"" AS ""Mobile"", t0.U_ACREDIT AS ""Accredited"", t0.""Address"" AS ""ClientAddress"", t0.""Comments"" 
                               , t0.""Address2"" AS ""DeliveryAddress"", t5.""Name"" AS ""DeliveryName"", t5.""Tel1"" AS ""DeliveryPhone"", t5.""Cellolar"" AS ""DeliveryMobile"", t0.U_INCOTERMS AS ""Incoterms"" 
                               , t0.U_CORRELATIVO AS ""Correlative"", t6.""TrnspName"" AS ""Transport"", t7.""PymntGroup"" AS ""Terms"", t0.""DiscSum"" AS ""Discount"", t4.""Margin"", t4.""TaxlessTotal"" 
                       FROM    {DBSA}.ODLN t0 
                               INNER JOIN {DBSA}.OSLP t2 ON t0.""SlpCode"" = t2.""SlpCode"" 
                               INNER JOIN {DBSA}.OCRD t3 ON t3.""CardCode"" = t0.""CardCode"" 
                               INNER JOIN ( SELECT s0.""DocEntry"", SUM( CASE s0.""WhsCode"" WHEN 'ZFSCZ' THEN s0.""Quantity"" * ( s0.""PriceAfVAT"" - s0.""StockPrice"" ) ELSE s0.""Quantity"" * ( ( 0.84 * s0.""PriceAfVAT"" ) - s0.""StockPrice"" ) END ) AS ""Margin""
                                                    , SUM( CASE s0.""WhsCode"" WHEN 'ZFSCZ' THEN s0.""Quantity"" * s0.""PriceAfVAT"" ELSE s0.""Quantity"" * 0.84 * s0.""PriceAfVAT"" END ) AS ""TaxlessTotal"", FIRST_VALUE(s0.""WhsCode"" ORDER BY ""LineNum"") AS ""WhsCode"" 
                                            FROM    {DBSA}.DLN1 s0
                                            WHERE   s0.""TargetType"" <> 14 
                                            GROUP BY s0.""DocEntry"" ) t4 ON t4.""DocEntry"" = t0.""DocEntry""
                               INNER JOIN {DBSA}.OWHS t1 ON t4.""WhsCode"" = t1.""WhsCode""
                               LEFT OUTER JOIN {DBSA}.OCPR t5 ON t5.""CntctCode"" = t0.""CntctCode"" 
                               LEFT OUTER JOIN {DBSA}.OSHP t6 ON t6.""TrnspCode"" = t0.""TrnspCode"" 
                               INNER JOIN {DBSA}.OCTG t7 ON t7.""GroupNum"" = t0.""GroupNum"" 
                               LEFT OUTER JOIN {DBSA}.INV1 t8 ON CAST(t0.""DocNum"" AS VARCHAR(10)) = IFNULL(t8.""BaseRef"", '') AND t8.""BaseType"" = 15
                               LEFT OUTER JOIN {DBSA}.OINV t9 ON t8.""DocEntry"" = t9.""DocEntry""
                       WHERE   t0.CANCELED = 'N' 
                       UNION 
                       SELECT  t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", 'Miami' AS ""Subsidiary"", t1.""WhsName"" AS ""Warehouse"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"" 
                               , t0.""NumAtCard"" AS ""ClientNote"", t2.""SlpName"" AS ""SellerCode"", t2.""Memo"" AS ""SellerName"", t0.""DocTotal"" AS ""Total"", t8.""DocEntry"" AS ""NoteId"", CAST(t9.""DocNum"" AS VARCHAR(10)) AS ""NoteNumber"" 
                               , '' AS ""BillName"", t0.U_NIT AS ""NIT"", t3.""Phone1"" AS ""Phone"", t3.""Cellular"" AS ""Mobile"", t0.U_ACREDIT AS ""Accredited"", t0.""Address"" AS ""ClientAddress"", t0.""Comments"" 
                               , t0.""Address2"" AS ""DeliveryAddress"", t5.""Name"" AS ""DeliveryName"", t5.""Tel1"" AS ""DeliveryPhone"", t5.""Cellolar"" AS ""DeliveryMobile"", t0.U_INCOTERMS AS ""Incoterms"" 
                               , t0.U_CORRELATIVO AS ""Correlative"", t6.""TrnspName"" AS ""Transport"", t7.""PymntGroup"" AS ""Terms"", t0.""DiscSum"" AS ""Discount"", t4.""Margin"", t4.""TaxlessTotal""  
                       FROM    {DBLA}.ODLN t0                               
                               INNER JOIN {DBLA}.OSLP t2 ON t0.""SlpCode"" = t2.""SlpCode"" 
                               INNER JOIN {DBLA}.OCRD t3 ON t3.""CardCode"" = t0.""CardCode"" 
                               INNER JOIN ( SELECT s0.""DocEntry"", SUM( s0.""Quantity"" * ( s0.""PriceAfVAT"" - s0.""StockPrice"" ) ) AS ""Margin"", SUM( s0.""Quantity"" * s0.""PriceAfVAT"" ) AS ""TaxlessTotal"", FIRST_VALUE(s0.""WhsCode"" ORDER BY ""LineNum"") AS ""WhsCode"" 
                                            FROM    {DBLA}.DLN1 s0
                                            WHERE   s0.""TargetType"" <> 14 
                                            GROUP BY s0.""DocEntry"" ) t4 ON t4.""DocEntry"" = t0.""DocEntry""
                               INNER JOIN {DBLA}.OWHS t1 ON t4.""WhsCode"" = t1.""WhsCode"" 
                               LEFT OUTER JOIN {DBLA}.OCPR t5 ON t5.""CntctCode"" = t0.""CntctCode"" 
                               LEFT OUTER JOIN {DBLA}.OSHP t6 ON t6.""TrnspCode"" = t0.""TrnspCode"" 
                               INNER JOIN {DBLA}.OCTG t7 ON t7.""GroupNum"" = t0.""GroupNum"" 
                               LEFT OUTER JOIN {DBLA}.INV1 t8 ON CAST(t0.""DocNum"" AS VARCHAR(10)) = IFNULL(t8.""BaseRef"", '') AND t8.""BaseType"" = 15
                               LEFT OUTER JOIN {DBLA}.OINV t9 ON t8.""DocEntry"" = t9.""DocEntry""
                       WHERE   t0.CANCELED = 'N' 
                       UNION 
                       SELECT  t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", 'Iquique' AS ""Subsidiary"", t1.""WhsName"" AS ""Warehouse"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"" 
                               , t0.""NumAtCard"" AS ""ClientNote"", t2.""SlpName"" AS ""SellerCode"", t2.""Memo"" AS ""SellerName"", t0.""DocTotal"" AS ""Total"", t8.""DocEntry"" AS ""NoteId"", CAST(t9.""DocNum"" AS VARCHAR(10)) AS ""NoteNumber"" 
                               , '' AS ""BillName"", t0.U_NIT AS ""NIT"", t3.""Phone1"" AS ""Phone"", t3.""Cellular"" AS ""Mobile"", t0.U_ACREDIT AS ""Accredited"", t0.""Address"" AS ""ClientAddress"", t0.""Comments"" 
                               , t0.""Address2"" AS ""DeliveryAddress"", t5.""Name"" AS ""DeliveryName"", t5.""Tel1"" AS ""DeliveryPhone"", t5.""Cellolar"" AS ""DeliveryMobile"", t0.U_INCOTERMS AS ""Incoterms"" 
                               , t0.U_CORRELATIVO AS ""Correlative"", t6.""TrnspName"" AS ""Transport"", t7.""PymntGroup"" AS ""Terms"", t0.""DiscSum"" AS ""Discount"", t4.""Margin"", t4.""TaxlessTotal""  
                       FROM    {DBIQ}.ODLN t0                               
                               INNER JOIN {DBIQ}.OSLP t2 ON t0.""SlpCode"" = t2.""SlpCode"" 
                               INNER JOIN {DBIQ}.OCRD t3 ON t3.""CardCode"" = t0.""CardCode"" 
                               INNER JOIN ( SELECT s0.""DocEntry"", SUM( s0.""Quantity"" * ( s0.""PriceAfVAT"" - s0.""StockPrice"" ) ) AS ""Margin"", SUM( s0.""Quantity"" * s0.""PriceAfVAT"" ) AS ""TaxlessTotal"", FIRST_VALUE(s0.""WhsCode"" ORDER BY ""LineNum"") AS ""WhsCode"" 
                                            FROM    {DBIQ}.DLN1 s0
                                            WHERE   s0.""TargetType"" <> 14 
                                            GROUP BY s0.""DocEntry"" ) t4 ON t4.""DocEntry"" = t0.""DocEntry""
                               INNER JOIN {DBIQ}.OWHS t1 ON t4.""WhsCode"" = t1.""WhsCode"" 
                               LEFT OUTER JOIN {DBIQ}.OCPR t5 ON t5.""CntctCode"" = t0.""CntctCode"" 
                               LEFT OUTER JOIN {DBIQ}.OSHP t6 ON t6.""TrnspCode"" = t0.""TrnspCode"" 
                               INNER JOIN {DBIQ}.OCTG t7 ON t7.""GroupNum"" = t0.""GroupNum"" 
                               LEFT OUTER JOIN {DBIQ}.INV1 t8 ON CAST(t0.""DocNum"" AS VARCHAR(10)) = IFNULL(t8.""BaseRef"", '') AND t8.""BaseType"" = 15
                               LEFT OUTER JOIN {DBIQ}.OINV t9 ON t8.""DocEntry"" = t9.""DocEntry""
                       WHERE   t0.CANCELED = 'N' ";
        }

        #endregion
    }
}
