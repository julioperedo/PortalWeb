using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE = BEntities;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;

namespace DALayer.SAP.Hana
{
    [Serializable()]
    public class Note : DALEntity<BEA.Note>
    {

        #region Methods

        internal IEnumerable<BEA.Note> ReturnChild(IEnumerable<string> Keys, params Enum[] Relations)
        {
            StringBuilder sb = new();
            sb.AppendLine($@"SELECT  * ");
            sb.AppendLine($@"FROM    ({GetQuery()}) a ");
            sb.AppendLine($@"WHERE   (LOWER(a.""Subsidiary"") || '-' || CAST(a.""OrderId"" AS VARCHAR(10))) IN ( {string.Join(",", Keys.Select(x => $"'{x.Replace("'", "")}'"))} ) ");
            IEnumerable<BEA.Note> items = SQLList(sb.ToString(), Relations);
            return items;
        }

        protected override void LoadRelations(ref IEnumerable<BEA.Note> Items, params Enum[] Relations)
        {
            IEnumerable<BEA.NoteItem> lstNoteItems = null;
            IEnumerable<BEA.DeliveryNote> lstDeliveryNotes = null;
            IEnumerable<BEA.Order> lstOrders = null;
            IEnumerable<BEA.OrderFile> lstFiles = null;
            IEnumerable<string> Keys;

            foreach (Enum RelationEnum in Relations)
            {
                Keys = from i in Items select $"{i.Subsidiary.ToLower()}-{i.Id}";
                if (RelationEnum.Equals(BEA.relNote.NoteItems))
                {
                    using NoteItem dalItem = new();
                    lstNoteItems = dalItem.List(Keys, Relations);
                }
                if (RelationEnum.Equals(BEA.relNote.DeliveryNote))
                {
                    using DeliveryNote dalDelivery = new();
                    lstDeliveryNotes = dalDelivery.ReturnChild(Keys, Relations);
                }
                if (RelationEnum.Equals(BEA.relNote.Order))
                {
                    Keys = from i in Items where !string.IsNullOrEmpty(i.OrderId) select $"{i.Subsidiary.ToLower()}-{i.OrderId.Replace(",", "-")}";
                    using Order dalOrder = new();
                    lstOrders = dalOrder.ReturnMaster(Keys, Relations);
                }
                if (RelationEnum.Equals(BEA.relNote.Files))
                {
                    lstFiles = ListFiles(Keys);
                }
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var item in Items)
                {
                    if (lstNoteItems != null)
                    {
                        item.Items = (from i in lstNoteItems where i.Subsidiary.ToLower() == item.Subsidiary.ToLower() & i.NoteId == item.Id select i).ToList();
                    }
                    if (lstDeliveryNotes?.Count() > 0)
                    {
                        item.DeliveryNote = (from i in lstDeliveryNotes where i.Subsidiary.ToLower() == item.Subsidiary.ToLower() & i.NoteId == item.Id select i).FirstOrDefault();
                    }
                    if (lstOrders?.Count() > 0)
                    {
                        item.Order = lstOrders.FirstOrDefault(x => x.Subsidiary.ToLower() == item.Subsidiary.ToLower() & (!string.IsNullOrEmpty(item.OrderId) && item.OrderId.Contains(x.Id.ToString())));
                    }
                    if (lstFiles?.Count() > 0)
                    {
                        item.Files = lstFiles.Where(x => x.Subsidiary.ToLower() == item.Subsidiary.ToLower() && x.DocEntry == item.Id)?.ToList();
                    }
                }
            }
        }

        protected override void LoadRelations(ref BEA.Note item, params Enum[] Relations)
        {
            NoteItem dalItem;
            DeliveryNote dalDelivery;
            Order dalOrder;
            foreach (Enum RelationEnum in Relations)
            {
                string[] Keys = new[] { $"{item.Subsidiary.ToLower()}-{item.Id}" };
                if (RelationEnum.Equals(BEA.relNote.NoteItems))
                {
                    dalItem = new NoteItem();
                    item.Items = dalItem.List(Keys, Relations)?.ToList();
                }
                if (RelationEnum.Equals(BEA.relNote.DeliveryNote))
                {
                    dalDelivery = new DeliveryNote();
                    item.DeliveryNote = dalDelivery.Search(item.DocNumber, item.Subsidiary, Relations);
                }
                if (RelationEnum.Equals(BEA.relNote.Order))
                {
                    if (!string.IsNullOrEmpty(item.OrderNumber))
                    {
                        dalOrder = new Order();
                        if (item.OrderNumber.Contains(","))
                        {
                            item.Order = dalOrder.ReturnMaster(int.Parse(item.OrderNumber.Split(',').First()), item.Subsidiary, Relations);
                        }
                        else
                        {
                            item.Order = dalOrder.ReturnMaster(int.Parse(item.OrderNumber), item.Subsidiary, Relations);
                        }
                    }
                }
                if (RelationEnum.Equals(BEA.relNote.Files))
                {
                    item.Files = ListFiles(Keys)?.ToList();
                }
            }
        }

        #endregion

        #region List Methods

        public IEnumerable<BEA.Note> List(List<Field> FilterList, string OrderBy, params Enum[] Relations)
        {
            StringBuilder sb = new();
            string strFilter = FilterList?.Count > 0 ? GetFilter(FilterList?.ToArray()) : "";

            sb.AppendLine($@"SELECT  * ");
            sb.AppendLine($@"FROM    ({GetQuery()}) a ");
            if (strFilter != "") sb.AppendLine($@"WHERE   {strFilter} ");
            sb.AppendLine($@"Order By {GetOrder(OrderBy)} ");

            IEnumerable<BEA.Note> items = SQLList(sb.ToString(), Relations);
            return items;
        }

        public IEnumerable<BEA.Note> List(List<Field> ItemFilters, List<Field> InnerFilters, string OrderBy, params Enum[] Relations)
        {
            StringBuilder sb = new();
            string itemFilters = ItemFilters?.Count > 0 ? GetFilter(ItemFilters?.ToArray()) : "", innerFilters = InnerFilters?.Count > 0 ? $"AND {GetFilter(InnerFilters.ToArray())}" : "";

            sb.AppendLine($@"SELECT  * ");
            sb.AppendLine($@"FROM    ({GetQuery("", innerFilters)}) a ");
            if (ItemFilters?.Count > 0) sb.AppendLine($@"WHERE   {itemFilters} ");
            sb.AppendLine($@"Order By {GetOrder(OrderBy)} ");

            IEnumerable<BEA.Note> items = SQLList(sb.ToString(), Relations);
            return items;
        }

        public IEnumerable<BEA.OrderNote> List2(List<Field> FilterList, string OrderBy)
        {
            StringBuilder sb = new();
            string strFilter = FilterList?.Count > 0 ? GetFilter(FilterList?.ToArray()) : "";

            sb.AppendLine($@"SELECT  * ");
            sb.AppendLine($@"FROM    ({GetQuery3()}) a ");
            if (strFilter != "") sb.AppendLine($@"WHERE   {strFilter} ");
            sb.AppendLine($@"Order By {GetOrder(OrderBy)} ");

            IEnumerable<BEA.OrderNote> items = SQLList<BEA.OrderNote>(sb.ToString());
            return items;
        }

        public IEnumerable<BEA.Note> ListBySection(string Section, List<Field> FilterList, string OrderBy, params Enum[] Relations)
        {
            Base.DivisionConfig bcDivConfig = new();
            IEnumerable<BEB.DivisionConfig> lstConfigItems = bcDivConfig.List("Type, Name");
            string lines = string.Join(",", (from i in lstConfigItems where i.Type == "M" select $"'{i.Name.ToLower()}'").ToArray());
            string sellers = string.Join(",", (from i in lstConfigItems where i.Type == "E" select $"'{i.Name.ToLower()}'").ToArray());

            
            string strFilter = FilterList?.Count > 0 ? GetFilter(FilterList?.ToArray()) : "";
            string filterItem, filterInner;
            switch (Section)
            {
                case "C":
                    filterInner = $@"AND LOWER(IFNULL(t2.U_LINEA, '')) NOT IN ( {lines} )";
                    filterItem = $@"AND LOWER(IFNULL(a3.""SlpName"", '')) NOT IN ( {sellers} )";
                    break;
                case "E":
                    filterInner = $@"AND LOWER(IFNULL(t2.U_LINEA, '')) NOT IN ( {lines}) ";
                    filterItem = $@"AND LOWER(IFNULL(a3.""SlpName"", '')) IN ( {sellers} )";
                    break;
                case "M":
                    filterInner = $@"AND LOWER(IFNULL(t2.U_LINEA, '')) IN ( {lines} ) ";
                    filterItem = $@"AND LOWER(IFNULL(a3.""SlpName"", '')) NOT IN ( {sellers} )";
                    break;
                default:
                    filterInner = "";
                    filterItem = "";
                    break;
            }
            StringBuilder sb = new();
            sb.AppendLine($@"SELECT  * ");
            sb.AppendLine($@"FROM    ({GetQuery(filterItem, filterInner)}) a ");
            if (strFilter != "") sb.AppendLine($@"WHERE   {strFilter} ");
            sb.AppendLine($@"Order By {GetOrder(OrderBy)} ");

            IEnumerable<BEA.Note> items = SQLList(sb.ToString(), Relations);
            return items;
        }

        public IEnumerable<BEA.OrderNote> List2(string CardCode, string ClientOrder, string Subsidiary, string StoreHouse, DateTime? InitialDate, DateTime? FinalDate, string Salesman, string ItemCode, string Line, string Category, string Subcategory, string Order)
        {
            List<string> lstFilter = new();
            string strFilter = "", strItems = "", strInner = "";
            if (!string.IsNullOrWhiteSpace(CardCode))
            {
                lstFilter.Add($@"LOWER(""ClientCode"") = '{CardCode.ToLower()}' ");
            }
            if (!string.IsNullOrWhiteSpace(ClientOrder))
            {
                lstFilter.Add($@"LOWER(""ClientNote"") LIKE '%{ClientOrder.Trim().ToLower()}%' ");
            }
            if (!string.IsNullOrWhiteSpace(Subsidiary))
            {
                lstFilter.Add($@"LOWER(""Subsidiary"") IN ( {Subsidiary.ToLower()} ) ");
            }
            if (!string.IsNullOrWhiteSpace(StoreHouse))
            {
                lstFilter.Add($@"LOWER(IFNULL(""Warehouse"", '')) IN ( {StoreHouse.ToLower()} ) ");
            }
            if (InitialDate.HasValue)
            {
                lstFilter.Add($@"""NoteDate"" >= '{InitialDate.Value:yyyy-MM-dd}' ");
            }
            if (FinalDate.HasValue)
            {
                lstFilter.Add($@"""NoteDate"" <= '{FinalDate.Value:yyyy-MM-dd}' ");
            }
            if (!string.IsNullOrWhiteSpace(Salesman))
            {
                lstFilter.Add($@"( LOWER(""SellerCode"") = '{Salesman.ToLower()}' OR LOWER(""SellerName"") = '{Salesman.ToLower()}' ) ");
            }
            if (lstFilter.Count > 0)
            {
                strFilter = string.Join(" AND ", lstFilter);
            }

            if (!string.IsNullOrWhiteSpace(ItemCode))
            {
                strInner += $@" AND ( LOWER(t2.""ItemCode"") LIKE '%{ItemCode.Trim().ToLower()}%' OR LOWER(t2.""ItemName"") LIKE '%{ItemCode.Trim().ToLower()}%' ) ";
            }
            if (!string.IsNullOrWhiteSpace(Line))
            {
                strInner += $@" AND LOWER(IFNULL(t2.U_LINEA, '')) = '{Line.ToLower()}' ";
            }
            if (!string.IsNullOrWhiteSpace(Category))
            {
                strInner += $@" AND LOWER(IFNULL(t2.U_CATEGORIA, '')) = '{Category.ToLower()}' ";
            }
            if (!string.IsNullOrWhiteSpace(Subcategory))
            {
                strInner += $@" AND LOWER(IFNULL(t2.U_SUBCATEG, '')) = '{Subcategory.ToLower()}' ";
            }

            StringBuilder sb = new();
            sb.AppendLine($@"SELECT  * ");
            sb.AppendLine($@"FROM    ({GetQuery3(strItems, strInner)}) a ");
            if (!string.IsNullOrWhiteSpace(strFilter))
            {
                sb.AppendLine($@"WHERE   {strFilter} ");
            }
            sb.AppendLine($@"ORDER BY {GetOrder(Order)} ");

            IEnumerable<BEA.OrderNote> items = SQLList<BEA.OrderNote>(sb.ToString());
            return items;
        }

        public IEnumerable<BEA.Note> ListWithoutOrder(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sb = new();
            string strFilter = FilterList?.Count > 0 ? GetFilter(FilterList?.ToArray()) : "";
            sb.AppendLine($@"SELECT  * ");
            sb.AppendLine($@"FROM    ({GetQueryWithoutOrder()}) a ");
            if (strFilter != "") sb.AppendLine($@"WHERE   {strFilter} ");
            sb.AppendLine($@"ORDER BY {GetOrder(Order)} ");

            IEnumerable<BEA.Note> items = SQLList(sb.ToString(), Relations);
            return items;
        }

        public IEnumerable<BEA.Item> ResumeByLine(DateTime? Since, DateTime? Until)
        {
            string dateFormat = "yyyy-MM-dd";
            var sb = new StringBuilder();
            sb.AppendLine($@"SELECT ""Line"" AS ""Name"", SUM(""Total"") AS ""Total"" ");
            sb.AppendLine($@"FROM ( SELECT T0.""CardCode"", CAST(T0.""DocDate"" AS DATE) AS ""DocDate"", UPPER(IFNULL(T3.U_LINEA, '')) AS ""Line"" ");
            sb.AppendLine($@"              , (CASE WHEN ");
            sb.AppendLine($@"                    ( SELECT SUM(TA.""Quantity"") ");
            sb.AppendLine($@"                      FROM {DBSA}.RIN1 TA ");
            sb.AppendLine($@"                      WHERE TA.""DocEntry"" = TX.""DocEntry"" ");
            sb.AppendLine($@"                            AND TA.""ItemCode"" = TX.""ItemCode"" ) > 0 THEN (T2.""PriceAfVAT"" * T2.""Quantity"") - (TX.""PriceAfVAT"" * TX.""Quantity"") ELSE (T2.""PriceAfVAT"" * T2.""Quantity"") END) AS ""Total"" ");
            sb.AppendLine($@"       FROM {DBSA}.OINV T0 ");
            sb.AppendLine($@"            INNER JOIN {DBSA}.INV1 T2 ON T0.""DocEntry"" = T2.""DocEntry"" ");
            sb.AppendLine($@"            LEFT OUTER JOIN {DBSA}.RIN1 TX ON TX.""BaseRef"" = CAST(T0.""DocNum"" AS VARCHAR(20)) AND TX.""BaseType"" = 13 AND TX.""ItemCode"" = T2.""ItemCode"" ");
            sb.AppendLine($@"            INNER JOIN {DBSA}.OITM T3 ON T2.""ItemCode"" = T3.""ItemCode"" ");
            sb.AppendLine($@"       UNION ALL ");
            sb.AppendLine($@"       SELECT T0.""CardCode"", CAST(T0.""DocDate"" AS DATE) AS ""DocDate"", UPPER(IFNULL(T3.U_LINEA, '')) AS ""Line"" ");
            sb.AppendLine($@"              , (CASE WHEN ");
            sb.AppendLine($@"                    ( SELECT SUM(TA.""Quantity"") ");
            sb.AppendLine($@"                      FROM {DBLA}.RIN1 TA ");
            sb.AppendLine($@"                      WHERE TA.""DocEntry"" = TX.""DocEntry"" ");
            sb.AppendLine($@"                            AND TA.""ItemCode"" = TX.""ItemCode"" ) > 0 THEN (T2.""PriceAfVAT"" * T2.""Quantity"") - (TX.""PriceAfVAT"" * TX.""Quantity"") ELSE (T2.""PriceAfVAT"" * T2.""Quantity"") END) AS ""Total"" ");
            sb.AppendLine($@"       FROM {DBLA}.OINV T0 ");
            sb.AppendLine($@"            INNER JOIN {DBLA}.INV1 T2 ON T0.""DocEntry"" = T2.""DocEntry"" ");
            sb.AppendLine($@"            LEFT OUTER JOIN {DBLA}.RIN1 TX ON TX.""BaseRef"" = CAST(T0.""DocNum"" AS VARCHAR(20)) AND TX.""BaseType"" = 13 AND TX.""ItemCode"" = T2.""ItemCode"" ");
            sb.AppendLine($@"            INNER JOIN {DBLA}.OITM T3 ON T2.""ItemCode"" = T3.""ItemCode"" ");
            sb.AppendLine($@"       UNION ALL ");
            sb.AppendLine($@"       SELECT T0.""CardCode"", CAST(T0.""DocDate"" AS DATE) AS ""DocDate"", UPPER(IFNULL(T3.U_LINEA, '')) AS ""Line"" ");
            sb.AppendLine($@"              , (CASE WHEN ");
            sb.AppendLine($@"                    ( SELECT SUM(TA.""Quantity"") ");
            sb.AppendLine($@"                      FROM {DBIQ}.RIN1 TA ");
            sb.AppendLine($@"                      WHERE TA.""DocEntry"" = TX.""DocEntry"" ");
            sb.AppendLine($@"                            AND TA.""ItemCode"" = TX.""ItemCode"" ) > 0 THEN (T2.""PriceAfVAT"" * T2.""Quantity"") - (TX.""PriceAfVAT"" * TX.""Quantity"") ELSE (T2.""PriceAfVAT"" * T2.""Quantity"") END) AS ""Total"" ");
            sb.AppendLine($@"       FROM {DBIQ}.OINV T0 ");
            sb.AppendLine($@"            INNER JOIN {DBIQ}.INV1 T2 ON T0.""DocEntry"" = T2.""DocEntry"" ");
            sb.AppendLine($@"            LEFT OUTER JOIN {DBIQ}.RIN1 TX ON TX.""BaseRef"" = CAST(T0.""DocNum"" AS VARCHAR(20)) AND TX.""BaseType"" = 13 AND TX.""ItemCode"" = T2.""ItemCode"" ");
            sb.AppendLine($@"            INNER JOIN {DBIQ}.OITM T3 ON T2.""ItemCode"" = T3.""ItemCode"") a ");
            sb.AppendLine($@"WHERE ""CardCode"" NOT IN ('CDMC-002', 'CDMC-003', 'CIMP-008') ");
            if (Since.HasValue) sb.AppendLine($@"      AND ""DocDate"" >= '{Since.Value.ToString(dateFormat)}' ");
            if (Until.HasValue) sb.AppendLine($@"      AND ""DocDate"" <= '{Until.Value.ToString(dateFormat)}' ");
            sb.AppendLine($@"GROUP BY ""Line"" ");
            sb.AppendLine($@"ORDER BY 1 ");

            IEnumerable<BEA.Item> items = SQLList<BEA.Item>(sb.ToString());
            return items;
        }

        public IEnumerable<BEA.Item> ResumeByCategory(DateTime? Since, DateTime? Until)
        {
            string dateFormat = "yyyy-MM-dd";
            var sb = new StringBuilder();
            sb.AppendLine($@"SELECT ""Category"" AS ""Parent"", ""Subcategory"" AS ""Name"", SUM(""Total"") AS ""Total"" ");
            sb.AppendLine($@"FROM ( SELECT T0.""CardCode"", CAST(T0.""DocDate"" AS DATE) AS ""DocDate"", UPPER(IFNULL(T3.U_CATEGORIA, '')) AS ""Category"", UPPER(IFNULL(T3.U_SUBCATEG, '')) AS ""Subcategory"" ");
            sb.AppendLine($@"              , (CASE WHEN ");
            sb.AppendLine($@"                    ( SELECT SUM(TA.""Quantity"") ");
            sb.AppendLine($@"                      FROM {DBSA}.RIN1 TA ");
            sb.AppendLine($@"                      WHERE TA.""DocEntry"" = TX.""DocEntry"" ");
            sb.AppendLine($@"                            AND TA.""ItemCode"" = TX.""ItemCode"" ) > 0 THEN (T2.""PriceAfVAT"" * T2.""Quantity"") - (TX.""PriceAfVAT"" * TX.""Quantity"") ELSE (T2.""PriceAfVAT"" * T2.""Quantity"") END) AS ""Total"" ");
            sb.AppendLine($@"       FROM {DBSA}.OINV T0 ");
            sb.AppendLine($@"            INNER JOIN {DBSA}.INV1 T2 ON T0.""DocEntry"" = T2.""DocEntry"" ");
            sb.AppendLine($@"            LEFT OUTER JOIN {DBSA}.RIN1 TX ON TX.""BaseRef"" = CAST(T0.""DocNum"" AS VARCHAR(20)) AND TX.""BaseType"" = 13 AND TX.""ItemCode"" = T2.""ItemCode"" ");
            sb.AppendLine($@"            INNER JOIN {DBSA}.OITM T3 ON T2.""ItemCode"" = T3.""ItemCode"" ");
            sb.AppendLine($@"       UNION ALL ");
            sb.AppendLine($@"       SELECT T0.""CardCode"", CAST(T0.""DocDate"" AS DATE) AS ""DocDate"", UPPER(IFNULL(T3.U_CATEGORIA, '')) AS ""Category"", UPPER(IFNULL(T3.U_SUBCATEG, '')) AS ""Subcategory"" ");
            sb.AppendLine($@"              , (CASE WHEN ");
            sb.AppendLine($@"                    ( SELECT SUM(TA.""Quantity"") ");
            sb.AppendLine($@"                      FROM {DBLA}.RIN1 TA ");
            sb.AppendLine($@"                      WHERE TA.""DocEntry"" = TX.""DocEntry"" ");
            sb.AppendLine($@"                            AND TA.""ItemCode"" = TX.""ItemCode"" ) > 0 THEN (T2.""PriceAfVAT"" * T2.""Quantity"") - (TX.""PriceAfVAT"" * TX.""Quantity"") ELSE (T2.""PriceAfVAT"" * T2.""Quantity"") END) AS ""Total"" ");
            sb.AppendLine($@"       FROM {DBLA}.OINV T0 ");
            sb.AppendLine($@"            INNER JOIN {DBLA}.INV1 T2 ON T0.""DocEntry"" = T2.""DocEntry"" ");
            sb.AppendLine($@"            LEFT OUTER JOIN {DBLA}.RIN1 TX ON TX.""BaseRef"" = CAST(T0.""DocNum"" AS VARCHAR(20)) AND TX.""BaseType"" = 13 AND TX.""ItemCode"" = T2.""ItemCode"" ");
            sb.AppendLine($@"            INNER JOIN {DBLA}.OITM T3 ON T2.""ItemCode"" = T3.""ItemCode"" ");
            sb.AppendLine($@"       UNION ALL ");
            sb.AppendLine($@"       SELECT T0.""CardCode"", CAST(T0.""DocDate"" AS DATE) AS ""DocDate"", UPPER(IFNULL(T3.U_CATEGORIA, '')) AS ""Category"", UPPER(IFNULL(T3.U_SUBCATEG, '')) AS ""Subcategory"" ");
            sb.AppendLine($@"              , (CASE WHEN ");
            sb.AppendLine($@"                    ( SELECT SUM(TA.""Quantity"") ");
            sb.AppendLine($@"                      FROM {DBIQ}.RIN1 TA ");
            sb.AppendLine($@"                      WHERE TA.""DocEntry"" = TX.""DocEntry"" ");
            sb.AppendLine($@"                            AND TA.""ItemCode"" = TX.""ItemCode"" ) > 0 THEN (T2.""PriceAfVAT"" * T2.""Quantity"") - (TX.""PriceAfVAT"" * TX.""Quantity"") ELSE (T2.""PriceAfVAT"" * T2.""Quantity"") END) AS ""Total"" ");
            sb.AppendLine($@"       FROM {DBIQ}.OINV T0 ");
            sb.AppendLine($@"            INNER JOIN {DBIQ}.INV1 T2 ON T0.""DocEntry"" = T2.""DocEntry"" ");
            sb.AppendLine($@"            LEFT OUTER JOIN {DBIQ}.RIN1 TX ON TX.""BaseRef"" = CAST(T0.""DocNum"" AS VARCHAR(20)) AND TX.""BaseType"" = 13 AND TX.""ItemCode"" = T2.""ItemCode"" ");
            sb.AppendLine($@"            INNER JOIN {DBIQ}.OITM T3 ON T2.""ItemCode"" = T3.""ItemCode"") a ");
            sb.AppendLine($@"WHERE ""CardCode"" NOT IN ('CDMC-002', 'CDMC-003', 'CIMP-008') ");
            if (Since.HasValue) sb.AppendLine($@"      AND ""DocDate"" >= '{Since.Value.ToString(dateFormat)}' ");
            if (Until.HasValue) sb.AppendLine($@"      AND ""DocDate"" <= '{Until.Value.ToString(dateFormat)}' ");
            sb.AppendLine($@"GROUP BY ""Category"", ""Subcategory"" ");
            sb.AppendLine($@"ORDER BY 1, 2 ");

            IEnumerable<BEA.Item> items = SQLList<BEA.Item>(sb.ToString());
            return items;
        }

        public IEnumerable<BEA.OrderFile> ListFiles(IEnumerable<string> Keys)
        {
            string query = $@"SELECT  *
                              FROM    ( SELECT  'Santa Cruz' AS ""Subsidiary"", t0.""DocEntry"", CAST(IFNULL(T1.""trgtPath"", '') AS varchar(5000)) AS ""Path"", IFNULL(T1.""FileName"", '') AS ""FileName"", IFNULL(T1.""FileExt"", '') AS ""FileExt"" 
                                        FROM    {DBSA}.OINV t0 
                                                INNER JOIN {DBSA}.ATC1 T1 ON T1.""AbsEntry"" = t0.""AtcEntry"" 
                                        UNION 
                                        SELECT  'Iquique', t0.""DocEntry"", CAST(IFNULL(T1.""trgtPath"", '') AS varchar(5000)) AS ""Path"", IFNULL(T1.""FileName"", '') AS ""FileName"", IFNULL(T1.""FileExt"", '') AS ""FileExt"" 
                                        FROM    {DBIQ}.OINV t0 
                                                INNER JOIN {DBIQ}.ATC1 T1 ON T1.""AbsEntry"" = t0.""AtcEntry"" 
                                        UNION 
                                        SELECT  'Miami', t0.""DocEntry"", CAST(IFNULL(T1.""trgtPath"", '') AS varchar(5000)) AS ""Path"", IFNULL(T1.""FileName"", '') AS ""FileName"", IFNULL(T1.""FileExt"", '') AS ""FileExt"" 
                                        FROM    {DBLA}.OINV t0 
                                                INNER JOIN {DBLA}.ATC1 T1 ON T1.""AbsEntry"" = t0.""AtcEntry"" ) a
                              WHERE   (LOWER(a.""Subsidiary"") || '-' || CAST(a.""DocEntry"" AS VARCHAR(10))) IN ( {string.Join(",", Keys.Select(x => $"'{x.Replace("'", "")}'"))} ) ";
            IEnumerable<BEA.OrderFile> items = SQLList<BEA.OrderFile>(query);
            return items;
        }

        public IEnumerable<BEA.DocumentRelated> ListRelatedDocuments(string Subsidiary, string DocIds)
        {
            string dbName = Subsidiary.ToLower() == "santa cruz" ? DBSA : (Subsidiary.ToLower() == "iquique" ? DBIQ : DBLA);
            string query = $@"SELECT  DISTINCT '{Subsidiary}' AS ""Subsidiary"", t0.""DocEntry"" AS ""Id"", 'Orden de Venta' AS ""DocType"", t0.""DocNum"" AS ""DocNumber"", t3.""DocEntry"" AS ""BaseId""
                              FROM    {dbName}.ORDR t0
                                      INNER JOIN {dbName}.RDR1 t1 ON t0.""DocEntry"" = t1.""DocEntry""
                                      INNER JOIN {dbName}.INV1 t2 ON t0.""DocEntry"" = t2.""BaseEntry"" AND t2.""BaseType"" = 17
                                      INNER JOIN {dbName}.OINV t3 ON t2.""DocEntry"" = t3.""DocEntry"" AND t3.CANCELED = 'N' AND t3.""DocEntry"" IN ( {DocIds} ) 
                              UNION
                              SELECT  DISTINCT '{Subsidiary}' AS ""Subsidiary"", t0.""DocEntry"" AS ""Id"", 'Nota de Entrega' AS ""DocType"", t0.""DocNum"" AS ""DocNumber"", t3.""DocEntry"" AS ""BaseId""
                              FROM    {dbName}.ODLN t0
                                      INNER JOIN {dbName}.DLN1 t1 ON t0.""DocEntry"" = t1.""DocEntry""
                                      INNER JOIN {dbName}.INV1 t2 ON t0.""DocEntry"" = t2.""BaseEntry"" AND t2.""BaseType"" = 15
                                      INNER JOIN {dbName}.OINV t3 ON t2.""DocEntry"" = t3.""DocEntry"" AND t3.CANCELED = 'N' AND t3.""DocEntry"" IN ( {DocIds} )
                              UNION
                              SELECT  DISTINCT '{Subsidiary}' AS ""Subsidiary"", t0.""DocEntry"" AS ""Id"", 'Orden de Venta' AS ""DocType"", t0.""DocNum"" AS ""DocNumber"", t4.""DocEntry"" AS ""BaseId""
                              FROM    {dbName}.ORDR t0
                                      INNER JOIN {dbName}.RDR1 t1 ON t0.""DocEntry"" = t1.""DocEntry""
                                      INNER JOIN {dbName}.DLN1 t2 ON t0.""DocEntry"" = t2.""BaseEntry"" AND t2.""BaseType"" = 17
                                      INNER JOIN {dbName}.ODLN t3 ON t2.""DocEntry"" = t3.""DocEntry"" AND t3.CANCELED = 'N'
                                      INNER JOIN {dbName}.INV1 t4 ON t2.""DocEntry"" = t4.""BaseEntry"" AND t4.""BaseType"" = 15 AND t4.""DocEntry"" IN ( {DocIds} ) 
                              UNION 
                              SELECT  DISTINCT '{Subsidiary}' AS ""Subsidiary"", t0.""DocEntry"" AS ""Id"", 'Nota de Entrega' AS ""DocType"", t0.""DocNum"" AS ""DocNumber"", t3.""DocEntry"" AS ""BaseId""
                              FROM    {dbName}.ODLN t0
                                      INNER JOIN {dbName}.DLN1 t1 ON t0.""DocEntry"" = t1.""DocEntry""
                                      INNER JOIN {dbName}.INV1 t2 ON t0.""DocEntry"" = t2.""TrgetEntry"" AND t2.""TargetType"" = 15
                                      INNER JOIN {dbName}.OINV t3 ON t2.""DocEntry"" = t3.""DocEntry"" AND t3.CANCELED = 'N' AND t3.""DocEntry"" IN ( {DocIds} ) ";

            IEnumerable<BEA.DocumentRelated> items = SQLList<BEA.DocumentRelated>(query);
            return items;
        }

        #endregion

        #region Search Methods

        public BEA.Note Search(int DocNumber, string Subsidiary, params Enum[] Relations)
        {
            string strQuery = $@"SELECT * FROM ({GetQuery()}) a WHERE ""DocNumber"" = {DocNumber} AND LOWER(""Subsidiary"") = '{Subsidiary.ToLower()}' ";
            BEA.Note item = SQLSearch(strQuery, Relations);
            return item;
        }

        public BEA.Note SearchLast(string CardCode, params Enum[] Relations)
        {
            string strQuery = $@"SELECT TOP 1 * FROM ({GetQuery()}) a WHERE LOWER(""ClientCode"") = '{CardCode.ToLower()}' ORDER BY ""DocDate"" DESC ";
            BEA.Note item = SQLSearch(strQuery, Relations);
            return item;
        }

        public BEA.NoteExtended SearchExtended(int DocNumber, string Subsidiary)
        {
            string strQuery = $@"SELECT * FROM ({GetQuery2()}) a WHERE ""DocNumber"" = {DocNumber} AND LOWER(""Subsidiary"") = '{Subsidiary.ToLower()}' ";
            BEA.NoteExtended item = SQLSearch<BEA.NoteExtended>(strQuery);
            return item;
        }

        public BEA.Bill SearchBill(int DocNumber)
        {
            var sb = new StringBuilder();
            sb.AppendLine($@"SELECT  ""DocNum"" AS ""DocNumber"", CAST(""DocDate"" AS DATE) AS ""DocDate"", ""CardCode"" AS ""ClientCode"", U_RAZSOC AS ""ClientName"", U_NIT AS ""NIT"" ");
            sb.AppendLine($@"        , ""NumAtCard"" AS ""BillNumber"", ""U_NROAUTOR"" AS ""AuthorizationNumber"", U_CODCTRL AS ""ControlCode"", CAST(U_FECHALIM AS DATE) AS ""LimitDate"", ""DocTotal"" ");
            sb.AppendLine($@"        , ""SysRate"", ""DocTotalSy"" AS ""DocTotalRated"" ");
            sb.AppendLine($@"FROM    {DBSA}.OINV ");
            sb.AppendLine($@"WHERE   ""DocNum"" = {DocNumber} ");

            BEA.Bill item = SQLSearch<BEA.Bill>(sb.ToString());
            return item;
        }

        public BEA.Bill SearchElectronicBill(int DocNumber)
        {
            var sb = new StringBuilder();
            sb.AppendLine($@"SELECT  ""DocNum"" AS ""DocNumber"", CAST(""DocDate"" AS DATE) AS ""DocDate"", ""CardCode"" AS ""ClientCode"", U_RAZSOC AS ""ClientName"", U_NIT AS ""NIT"" ");
            sb.AppendLine($@"        , ""NumAtCard"" AS ""BillNumber"", ""U_B_cuf"" AS ""AuthorizationNumber"", ""DocTotal"", ""SysRate"" ");
            sb.AppendLine($@"        , ( SELECT SUM(ROUND((""GTotalSC"" / ""Quantity""), 2) * ""Quantity"") FROM {DBSA}.INV1 WHERE t0.""DocEntry"" = ""DocEntry"" ) AS ""DocTotalRated"" ");
            sb.AppendLine($@"FROM    {DBSA}.OINV t0 ");
            sb.AppendLine($@"WHERE   ""DocNum"" = {DocNumber} ");

            BEA.Bill item = SQLSearch<BEA.Bill>(sb.ToString());
            return item;
        }

        #endregion

        #region Private Methods

        private string GetQuery(string ItemFilter = null, string InnerFilter = null)
        {
            string query = $@"SELECT  'Santa Cruz' AS ""Subsidiary"", a2.""WhsName"" AS ""Warehouse"", a1.""Id"", a1.""DocNumber"", a1.""DocDate"", a1.""ClientCode"", a1.""ClientName"", a1.""ClientNote"", a1.""SlpCode"", a3.""SlpName"" AS ""SellerCode"", a3.""Memo"" AS ""SellerName"", a1.""Total"", a1.""Margin"" 
                                      , a1.""TaxlessTotal"", ( CASE a1.""DocStatus"" WHEN 'O' THEN 'Abierto' ELSE 'Cerrado' END ) AS ""State"", a4.""PymntGroup"" AS ""TermConditions"", a1.""OrderId"", a1.""OrderNumber"", a1.""BillNumber"", a1.""AuthorizationNumber"", a1.""IsDeliveryNote"", a1.""Series"" 
                              FROM    ( SELECT    ""Id"", ""DocNumber"", FIRST_VALUE(""WhsCode"" ORDER BY ""DocNumber"") AS ""WhsCode"", ""DocDate"", ""ClientCode"", ""ClientName"", STRING_AGG(""ClientNote"", ', ') AS ""ClientNote"", ""SlpCode"", SUM(""Total"") AS ""Total"", SUM(""Margin"") AS ""Margin""
                                                  , SUM(""TaxlessTotal"") AS ""TaxlessTotal"", ""DocStatus"", ""GroupNum"", STRING_AGG(""OrderId"", ', ') AS ""OrderId"", STRING_AGG(""OrderNumber"", ', ') AS ""OrderNumber"", ""BillNumber"", ""AuthorizationNumber"", ""IsDeliveryNote""
                                                  , FIRST_VALUE(b.""Series"" ORDER BY b.""Id"") AS ""Series""
                                        FROM      ( SELECT    t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", t3.""WhsCode"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"", t0.""NumAtCard"" AS ""ClientNote"" 
                                                              , t0.""SlpCode"", SUM(t1.""Quantity"" * t1.""PriceAfVAT"") AS ""Total"" 
                                                              , SUM(( CASE WHEN t1.""WhsCode"" = 'ZFSCZ' THEN t1.""Quantity"" * ( t1.""PriceAfVAT"" - t1.""StockPrice"" ) ELSE t1.""Quantity"" * (( 0.84 * t1.""PriceAfVAT"" ) - t1.""StockPrice"" ) END )) AS ""Margin"" 
                                                              , SUM(( CASE WHEN t1.""WhsCode"" = 'ZFSCZ' THEN t1.""Quantity"" * t1.""PriceAfVAT"" ELSE t1.""Quantity"" * ( 0.84 * t1.""PriceAfVAT"" ) END )) AS ""TaxlessTotal"", t0.""DocStatus"", t0.""GroupNum"", t1.""BaseEntry"" AS ""OrderId"" 
                                                              , ( CASE WHEN IFNULL(t1.""BaseRef"", '') = '' THEN NULL ELSE CAST(t1.""BaseRef"" AS INTEGER) END ) AS ""OrderNumber"", t0.U_NUM_FACT AS ""BillNumber"", t0.""U_NROAUTOR"" AS ""AuthorizationNumber""
                                                              , ( CASE FIRST_VALUE(t1.""BaseType"" ORDER BY ""LineNum"") WHEN 15 THEN 'Y' ELSE 'N' END ) AS ""IsDeliveryNote"", FIRST_VALUE(t0.""Series"" ORDER BY t0.""DocEntry"") AS ""Series"" 
                                                    FROM      {DBSA}.OINV t0 
                                                              INNER JOIN {DBSA}.INV1 t1 ON t1.""DocEntry"" = t0.""DocEntry"" 
                                                              INNER JOIN {DBSA}.OITM t2 ON t2.""ItemCode"" = t1.""ItemCode"" 
                                                              LEFT OUTER JOIN (SELECT  s0.""DocEntry"", s0.""WhsCode"" 
                                                                               FROM    {DBSA}.INV1 s0 
                                                                               WHERE   ""LineNum"" = ( SELECT MIN(""LineNum"") FROM {DBSA}.INV1 WHERE ""DocEntry"" = s0.""DocEntry"" ) ) t3 ON t3.""DocEntry"" = t0.""DocEntry"" 
                                                    WHERE     t0.CANCELED = 'N' AND  t1.""TargetType"" <> 14 AND   t0.""CardCode"" NOT IN ('CDMC-002', 'CDMC-003', 'CIMP-008') {InnerFilter} 
                                                    GROUP BY  t0.""DocEntry"", t0.""DocNum"", t3.""WhsCode"", CAST(t0.""DocDate"" AS DATE), t0.""CardCode"", t0.""CardName"", t0.""NumAtCard"" 
                                                         , t0.""SlpCode"", t0.""DocStatus"", t0.""GroupNum"", t1.""BaseEntry"", ( CASE WHEN IFNULL(t1.""BaseRef"", '') = '' THEN NULL ELSE CAST(t1.""BaseRef"" AS INTEGER)END ), t0.U_NUM_FACT, t0.""U_NROAUTOR"", t0.""isIns"" ) b
                                        GROUP BY ""Id"", ""DocNumber"", ""DocDate"", ""ClientCode"", ""ClientName"", ""SlpCode"", ""DocStatus"", ""GroupNum"", ""BillNumber"", ""AuthorizationNumber"", ""IsDeliveryNote"" ) AS a1 
                                      LEFT OUTER JOIN {DBSA}.OWHS a2 ON a2.""WhsCode"" = a1.""WhsCode"" 
                                      LEFT OUTER JOIN {DBSA}.OSLP a3 ON a3.""SlpCode"" = a1.""SlpCode"" 
                                      LEFT OUTER JOIN {DBSA}.OCTG a4 ON a4.""GroupNum"" = a1.""GroupNum"" 
                              WHERE  1 = 1 {ItemFilter} 
                              UNION ALL 
                              SELECT  'Iquique' AS ""Subsidiary"", a2.""WhsName"" AS ""Warehouse"", a1.""Id"", a1.""DocNumber"", a1.""DocDate"", a1.""ClientCode"", a1.""ClientName"", a1.""ClientNote"", a1.""SlpCode"", a3.""SlpName"" AS ""SellerCode"", a3.""Memo"" AS ""SellerName"", a1.""Total"", a1.""Margin"" 
                                      , a1.""TaxlessTotal"", ( CASE a1.""DocStatus"" WHEN 'O' THEN 'Abierto' ELSE 'Cerrado' END ) AS ""State"", a4.""PymntGroup"" AS ""TermConditions"", a1.""OrderId"", a1.""OrderNumber"", a1.""BillNumber"", a1.""AuthorizationNumber"", a1.""IsDeliveryNote"", a1.""Series"" 
                              FROM    ( SELECT    ""Id"", ""DocNumber"", FIRST_VALUE(""WhsCode"" ORDER BY ""DocNumber"") AS ""WhsCode"", ""DocDate"", ""ClientCode"", ""ClientName"", STRING_AGG(""ClientNote"", ', ') AS ""ClientNote"", ""SlpCode"", SUM(""Total"") AS ""Total"", SUM(""Margin"") AS ""Margin""
                                                  , SUM(""TaxlessTotal"") AS ""TaxlessTotal"", ""DocStatus"", ""GroupNum"", STRING_AGG(""OrderId"", ', ') AS ""OrderId"", STRING_AGG(""OrderNumber"", ', ') AS ""OrderNumber"", ""BillNumber"", ""AuthorizationNumber"", ""IsDeliveryNote""
                                                  , FIRST_VALUE(b.""Series"" ORDER BY b.""Id"") AS ""Series""
                                        FROM      ( SELECT    t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", t3.""WhsCode"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"", t0.""NumAtCard"" AS ""ClientNote"" 
                                                              , t0.""SlpCode"", SUM(t1.""Quantity"" * t1.""PriceAfVAT"") AS ""Total"", SUM(t1.""Quantity"" * ( t1.""PriceAfVAT"" - t1.""StockPrice"" )) AS ""Margin"", SUM(t1.""Quantity"" * t1.""PriceAfVAT"") AS ""TaxlessTotal"", t0.""DocStatus"", t0.""GroupNum"" 
                                                              , t1.""BaseEntry"" AS ""OrderId"", ( CASE WHEN IFNULL(t1.""BaseRef"", '') = '' THEN NULL ELSE CAST(t1.""BaseRef"" AS INTEGER) END ) AS ""OrderNumber"", t0.U_NUM_FACT AS ""BillNumber"", t0.U_NROAUTOR AS ""AuthorizationNumber"" 
                                                              , ( CASE FIRST_VALUE(t1.""BaseType"" ORDER BY ""LineNum"") WHEN 15 THEN 'Y' ELSE 'N' END ) AS ""IsDeliveryNote"", FIRST_VALUE(t0.""Series"" ORDER BY t0.""DocEntry"") AS ""Series"" 
                                                    FROM      {DBIQ}.OINV t0 
                                                              INNER JOIN {DBIQ}.INV1 t1 ON t1.""DocEntry"" = t0.""DocEntry"" 
                                                              INNER JOIN {DBIQ}.OITM t2 ON t2.""ItemCode"" = t1.""ItemCode"" 
                                                              LEFT OUTER JOIN (SELECT  s0.""DocEntry"", s0.""WhsCode"" 
                                                                               FROM    {DBIQ}.INV1 s0 
                                                                               WHERE   ""LineNum"" = ( SELECT MIN(""LineNum"") FROM {DBIQ}.INV1 WHERE ""DocEntry"" = s0.""DocEntry"" ) ) t3 ON t3.""DocEntry"" = t0.""DocEntry"" 
                                                    WHERE     t0.CANCELED = 'N' AND  t1.""TargetType"" <> 14 AND   t0.""CardCode"" NOT IN ('CDMC-002', 'CDMC-003', 'CIMP-008') {InnerFilter}
                                                    GROUP BY  t0.""DocEntry"", t0.""DocNum"", t3.""WhsCode"", CAST(t0.""DocDate"" AS DATE), t0.""CardCode"", t0.""CardName"", t0.""NumAtCard"" 
                                                         , t0.""SlpCode"", t0.""DocStatus"", t0.""GroupNum"", t1.""BaseEntry"", ( CASE WHEN IFNULL(t1.""BaseRef"", '') = '' THEN NULL ELSE CAST(t1.""BaseRef"" AS INTEGER)END ), t0.U_NUM_FACT, t0.U_NROAUTOR, t0.""isIns"" ) b 
                                        GROUP BY ""Id"", ""DocNumber"", ""DocDate"", ""ClientCode"", ""ClientName"", ""SlpCode"", ""DocStatus"", ""GroupNum"", ""BillNumber"", ""AuthorizationNumber"", ""IsDeliveryNote"" ) AS a1 
                                      LEFT OUTER JOIN {DBIQ}.OWHS a2 ON a2.""WhsCode"" = a1.""WhsCode"" 
                                      LEFT OUTER JOIN {DBIQ}.OSLP a3 ON a3.""SlpCode"" = a1.""SlpCode"" 
                                      LEFT OUTER JOIN {DBIQ}.OCTG a4 ON a4.""GroupNum"" = a1.""GroupNum"" 
                              WHERE  1 = 1 {ItemFilter} 
                              UNION ALL 
                              SELECT  'Miami' AS ""Subsidiary"", a2.""WhsName"" AS ""Warehouse"", a1.""Id"", a1.""DocNumber"", a1.""DocDate"", a1.""ClientCode"", a1.""ClientName"", a1.""ClientNote"", a1.""SlpCode"", a3.""SlpName"" AS ""SellerCode"", a3.""Memo"" AS ""SellerName"", a1.""Total"", a1.""Margin"" 
                                      , a1.""TaxlessTotal"", ( CASE a1.""DocStatus"" WHEN 'O' THEN 'Abierto' ELSE 'Cerrado' END ) AS ""State"", a4.""PymntGroup"" AS ""TermConditions"", a1.""OrderId"", a1.""OrderNumber"", a1.""BillNumber"", a1.""AuthorizationNumber"", a1.""IsDeliveryNote"", a1.""Series"" 
                              FROM    ( SELECT    ""Id"", ""DocNumber"", FIRST_VALUE(""WhsCode"" ORDER BY ""DocNumber"") AS ""WhsCode"", ""DocDate"", ""ClientCode"", ""ClientName"", STRING_AGG(""ClientNote"", ', ') AS ""ClientNote"", ""SlpCode"", SUM(""Total"") AS ""Total"", SUM(""Margin"") AS ""Margin""
                                                  , SUM(""TaxlessTotal"") AS ""TaxlessTotal"", ""DocStatus"", ""GroupNum"", STRING_AGG(""OrderId"", ', ') AS ""OrderId"", STRING_AGG(""OrderNumber"", ', ') AS ""OrderNumber"", ""BillNumber"", ""AuthorizationNumber"", ""IsDeliveryNote""
                                                  , FIRST_VALUE(b.""Series"" ORDER BY b.""Id"") AS ""Series""
                                        FROM      ( SELECT    t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", t3.""WhsCode"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"", t0.""NumAtCard"" AS ""ClientNote"" 
                                                              , t0.""SlpCode"", SUM(t1.""Quantity"" * t1.""PriceAfVAT"") AS ""Total"", SUM(t1.""Quantity"" * ( t1.""PriceAfVAT"" - t1.""StockPrice"" )) AS ""Margin"", SUM(t1.""Quantity"" * t1.""PriceAfVAT"") AS ""TaxlessTotal"", t0.""DocStatus"", t0.""GroupNum"" 
                                                              , t1.""BaseEntry"" AS ""OrderId"", ( CASE WHEN IFNULL(t1.""BaseRef"", '') = '' THEN NULL ELSE CAST(t1.""BaseRef"" AS INTEGER) END ) AS ""OrderNumber"", t0.U_NUM_FACT AS ""BillNumber"", t0.U_NROAUTOR AS ""AuthorizationNumber"" 
                                                              , ( CASE FIRST_VALUE(t1.""BaseType"" ORDER BY ""LineNum"") WHEN 15 THEN 'Y' ELSE 'N' END ) AS ""IsDeliveryNote"", FIRST_VALUE(t0.""Series"" ORDER BY t0.""DocEntry"") AS ""Series"" 
                                                    FROM      {DBLA}.OINV t0 
                                                              INNER JOIN {DBLA}.INV1 t1 ON t1.""DocEntry"" = t0.""DocEntry"" 
                                                              INNER JOIN {DBLA}.OITM t2 ON t2.""ItemCode"" = t1.""ItemCode"" 
                                                              LEFT OUTER JOIN (SELECT  s0.""DocEntry"", s0.""WhsCode"" 
                                                                               FROM    {DBLA}.INV1 s0 
                                                                               WHERE   ""LineNum"" = ( SELECT MIN(""LineNum"") FROM {DBLA}.INV1 WHERE ""DocEntry"" = s0.""DocEntry"" ) ) t3 ON t3.""DocEntry"" = t0.""DocEntry"" 
                                                    WHERE     t0.CANCELED = 'N' AND  t1.""TargetType"" <> 14 AND   t0.""CardCode"" NOT IN ('CDMC-002', 'CDMC-003', 'CIMP-008') {InnerFilter}
                                                    GROUP BY  t0.""DocEntry"", t0.""DocNum"", t3.""WhsCode"", CAST(t0.""DocDate"" AS DATE), t0.""CardCode"", t0.""CardName"", t0.""NumAtCard"" 
                                                         , t0.""SlpCode"", t0.""DocStatus"", t0.""GroupNum"", t1.""BaseEntry"", ( CASE WHEN IFNULL(t1.""BaseRef"", '') = '' THEN NULL ELSE CAST(t1.""BaseRef"" AS INTEGER)END ), t0.U_NUM_FACT, t0.U_NROAUTOR, t0.""isIns"" ) b 
                                        GROUP BY ""Id"", ""DocNumber"", ""DocDate"", ""ClientCode"", ""ClientName"", ""SlpCode"", ""DocStatus"", ""GroupNum"", ""BillNumber"", ""AuthorizationNumber"", ""IsDeliveryNote"" ) AS a1 
                                      LEFT OUTER JOIN {DBLA}.OWHS a2 ON a2.""WhsCode"" = a1.""WhsCode"" 
                                      LEFT OUTER JOIN {DBLA}.OSLP a3 ON a3.""SlpCode"" = a1.""SlpCode"" 
                                      LEFT OUTER JOIN {DBLA}.OCTG a4 ON a4.""GroupNum"" = a1.""GroupNum"" 
                              WHERE  1 = 1 {ItemFilter} ";
            return query;
        }

        private string GetQuery2(string ItemFilter = null, string InnerFilter = null)
        {
            string query = $@"SELECT  'Santa Cruz' AS ""Subsidiary"", ""Warehouse"", a1.""Id"", a1.""DocNumber"", a1.""DocDate"", a1.""ClientCode"", a1.""ClientName"", a1.""ClientNote"", a3.""SlpName"" AS ""SellerCode"", a3.""Memo"" AS ""SellerName"", a1.""Total"", a1.""Margin"" 
                                       , a1.""TaxlessTotal"", ( CASE a1.""DocStatus"" WHEN 'O' THEN 'Abierto' ELSE 'Cerrado' END ) AS ""State"", a4.""PymntGroup"" AS ""TermConditions"", a1.""OrderId"", a1.""OrderNumber"", a1.""BillNumber"", a1.""AuthorizationNumber"", a1.""IsDeliveryNote"" 
                                       , a1.""Comments"", a1.""BillingAddress"", a1.""DestinationAddress"", a1.""DestinationCode"", a1.""Incoterms"", a1.""Correlative"", a1.""Discount"", a5.""TrnspName"" AS ""Transport"", a1.""BilledTo"" 
                                       , a6.""Phone1"" AS ""Phone"", a6.""Cellular"" AS ""Cellphone"", a7.""Name"" AS ""NamePC"", a7.""Tel1"" AS ""PhonePC"", a7.""Cellolar"" AS ""CellphonePC"", a1.""Accredited"", ""Latitude"", ""Longitude"", ""BusinessName"", ""NIT"", ""FCName"", ""FCMail"", ""FCPhone"", ""FCCity"", ""FCAddress"" 
                              FROM     ( SELECT  ""Id"", ""DocNumber"", STRING_AGG(b.""WhsName"", ', ') AS ""Warehouse"", ""DocDate"", ""ClientCode"", ""ClientName"", ""ClientNote"", ""SlpCode"", SUM(""Total"") AS ""Total"", SUM(""Margin"") AS ""Margin"", SUM(""TaxlessTotal"") AS ""TaxlessTotal"", ""DocStatus"", ""GroupNum"", STRING_AGG(""OrderId"", ', ') AS ""OrderId""
                                                 , STRING_AGG(""OrderNumber"", ', ') AS ""OrderNumber"", ""BillNumber"", ""AuthorizationNumber"", ""IsDeliveryNote"", ""Comments"", ""BillingAddress"", ""DestinationAddress"", ""DestinationCode"", ""Incoterms"", ""Correlative"", SUM(""Discount"") AS ""Discount"", ""TrnspCode""
                                                 , ""BilledTo"", ""CntctCode"", ""Accredited"", ""Latitude"", ""Longitude"", ""BusinessName"", ""NIT"", ""FCName"", ""FCMail"", ""FCPhone"", ""FCCity"", ""FCAddress"" 
                                         FROM    ( SELECT   DISTINCT t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", FIRST_VALUE(t1.""WhsCode"" ORDER BY ""LineNum"") AS ""WhsCode"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"", t0.""NumAtCard"" AS ""ClientNote"" 
                                                             , t0.""SlpCode"", SUM(t1.""Quantity"" * t1.""PriceAfVAT"") AS ""Total"" 
                                                             , SUM(( CASE WHEN t1.""WhsCode"" = 'ZFSCZ' THEN t1.""Quantity"" * ( t1.""PriceAfVAT"" - t1.""StockPrice"" ) ELSE t1.""Quantity"" * (( 0.84 * t1.""PriceAfVAT"" ) - t1.""StockPrice"" ) END )) AS ""Margin"" 
                                                             , SUM(( CASE WHEN t1.""WhsCode"" = 'ZFSCZ' THEN t1.""Quantity"" * t1.""PriceAfVAT"" ELSE t1.""Quantity"" * ( 0.84 * t1.""PriceAfVAT"" ) END )) AS ""TaxlessTotal"", t0.""DocStatus"", t0.""GroupNum"", t1.""BaseEntry"" AS ""OrderId"" 
                                                             , ( CASE WHEN IFNULL(t1.""BaseRef"", '') = '' THEN NULL ELSE CAST(t1.""BaseRef"" AS INTEGER) END ) AS ""OrderNumber"", t0.U_NUM_FACT AS ""BillNumber"", t0.""U_NROAUTOR"" AS ""AuthorizationNumber"" 
                                                             , t0.""Comments"", t0.""Address"" AS ""BillingAddress"", t0.""Address2"" AS ""DestinationAddress"", t0.""ShipToCode"" AS ""DestinationCode"", t0.U_INCOTERMS AS ""Incoterms"", t0.U_CORRELATIVO AS ""Correlative"", t0.""DiscSum"" AS ""Discount"", t0.""TrnspCode"" 
                                                             , t0.U_FACTURAR AS ""BilledTo"", t0.""CntctCode"", t0.U_ACREDIT AS ""Accredited"", ( CASE FIRST_VALUE(t1.""BaseType"" ORDER BY ""LineNum"") WHEN 15 THEN 'Y' ELSE 'N' END ) AS ""IsDeliveryNote""   
                                                             , t0.U_LATITUD_DIREC_CFINAL AS ""Latitude"", t0.U_LONGITUD_DIREC_CFINAL AS ""Longitude"", t0.U_RAZSOC AS ""BusinessName"", t0.U_NIT AS ""NIT"", t0.U_NOMBRE_CFINAL AS ""FCName"", t0.U_MAIL_CFINAL AS ""FCMail"", t0.U_TELEF_CFINAL AS ""FCPhone"" 
                                                             , t0.U_CIUDAD_CFINAL AS ""FCCity"", t0.U_DIRECCION_CFINAL AS ""FCAddress""
                                                   FROM      {DBSA}.OINV t0 
                                                             INNER JOIN {DBSA}.INV1 t1 ON t1.""DocEntry"" = t0.""DocEntry"" 
                                                             INNER JOIN {DBSA}.OITM t2 ON t2.""ItemCode"" = t1.""ItemCode"" 
                                                   WHERE     t0.CANCELED = 'N' AND  t1.""TargetType"" <> 14 AND   t0.""CardCode"" NOT IN ('CDMC-002', 'CDMC-003', 'CIMP-008') {InnerFilter}
                                                   GROUP BY  t0.""DocEntry"", t0.""DocNum"", CAST(t0.""DocDate"" AS DATE), t0.""CardCode"", t0.""CardName"", t0.""NumAtCard"" 
                                                             , t0.""SlpCode"", t0.""DocStatus"", t0.""GroupNum"", t1.""BaseEntry"", ( CASE WHEN IFNULL(t1.""BaseRef"", '') = '' THEN NULL ELSE CAST(t1.""BaseRef"" AS INTEGER)END ), t0.U_NUM_FACT, t0.""U_NROAUTOR"", t0.""isIns"", t0.""Comments"", t0.""Address"", t0.""Address2"" 
                                                             , t0.""ShipToCode"", t0.U_INCOTERMS, t0.U_CORRELATIVO, t0.""DiscSum"", t0.""TrnspCode"", t0.U_FACTURAR, t0.""CntctCode"", t0.U_ACREDIT, t0.U_LATITUD_DIREC_CFINAL, t0.U_LONGITUD_DIREC_CFINAL, t0.U_RAZSOC, t0.U_NIT, t0.U_NOMBRE_CFINAL, t0.U_MAIL_CFINAL, t0.U_TELEF_CFINAL 
                                                             , t0.U_CIUDAD_CFINAL, t0.U_DIRECCION_CFINAL ) a
                                                 LEFT OUTER JOIN {DBSA}.OWHS b ON a.""WhsCode"" = b.""WhsCode""  
                                         GROUP BY ""Id"", ""DocNumber"", ""DocDate"", ""ClientCode"", ""ClientName"", ""ClientNote"", ""SlpCode"", ""DocStatus"", ""GroupNum"", ""BillNumber"", ""AuthorizationNumber"", ""IsDeliveryNote"", ""Comments"", ""BillingAddress"", ""DestinationAddress"", ""DestinationCode""
                                                  , ""Incoterms"", ""Correlative"", ""TrnspCode"", ""BilledTo"", ""CntctCode"", ""Accredited"", ""Latitude"", ""Longitude"", ""BusinessName"", ""NIT"", ""FCName"", ""FCMail"", ""FCPhone"", ""FCCity"", ""FCAddress"" ) AS a1 
                                       LEFT OUTER JOIN {DBSA}.OSLP a3 ON a3.""SlpCode"" = a1.""SlpCode"" 
                                       LEFT OUTER JOIN {DBSA}.OCTG a4 ON a4.""GroupNum"" = a1.""GroupNum"" 
                                       LEFT OUTER JOIN {DBSA}.OSHP a5 ON a1.""TrnspCode"" = a5.""TrnspCode"" 
                                       INNER JOIN {DBSA}.OCRD a6 ON a6.""CardCode"" = a1.""ClientCode"" 
                                       LEFT OUTER JOIN {DBSA}.OCPR a7 ON a7.""CntctCode"" = a1.""CntctCode"" 
                              {(string.IsNullOrEmpty(ItemFilter) ? "" : $"WHERE    {ItemFilter} ")} 
                              UNION ALL
                              SELECT   'Iquique' AS ""Subsidiary"", ""Warehouse"", a1.""Id"", a1.""DocNumber"", a1.""DocDate"", a1.""ClientCode"", a1.""ClientName"", a1.""ClientNote"", a3.""SlpName"" AS ""SellerCode"", a3.""Memo"" AS ""SellerName"", a1.""Total"", a1.""Margin"" 
                                       , a1.""TaxlessTotal"", ( CASE a1.""DocStatus"" WHEN 'O' THEN 'Abierto' ELSE 'Cerrado' END ) AS ""State"", a4.""PymntGroup"" AS ""TermConditions"", a1.""OrderId"", a1.""OrderNumber"", a1.""BillNumber"", a1.""AuthorizationNumber"", a1.""IsDeliveryNote"" 
                                       , a1.""Comments"", a1.""BillingAddress"", a1.""DestinationAddress"", a1.""DestinationCode"", a1.""Incoterms"", a1.""Correlative"", a1.""Discount"", a5.""TrnspName"" AS ""Transport"", a1.""BilledTo"" 
                                       , a6.""Phone1"" AS ""Phone"", a6.""Cellular"" AS ""Cellphone"", a7.""Name"" AS ""NamePC"", a7.""Tel1"" AS ""PhonePC"", a7.""Cellolar"" AS ""CellphonePC"", a1.""Accredited"", ""Latitude"", ""Longitude"", ""BusinessName"", ""NIT"", ""FCName"", ""FCMail"", ""FCPhone"", ""FCCity"", ""FCAddress"" 
                              FROM     ( SELECT  ""Id"", ""DocNumber"", STRING_AGG(b.""WhsName"", ', ') AS ""Warehouse"", ""DocDate"", ""ClientCode"", ""ClientName"", ""ClientNote"", ""SlpCode"", SUM(""Total"") AS ""Total"", SUM(""Margin"") AS ""Margin"", SUM(""TaxlessTotal"") AS ""TaxlessTotal"", ""DocStatus"", ""GroupNum"", STRING_AGG(""OrderId"", ', ') AS ""OrderId""
                                                 , STRING_AGG(""OrderNumber"", ', ') AS ""OrderNumber"", ""BillNumber"", ""AuthorizationNumber"", ""IsDeliveryNote"", ""Comments"", ""BillingAddress"", ""DestinationAddress"", ""DestinationCode"", ""Incoterms"", ""Correlative"", SUM(""Discount"") AS ""Discount"", ""TrnspCode""
                                                 , ""BilledTo"", ""CntctCode"", ""Accredited"", ""Latitude"", ""Longitude"", ""BusinessName"", ""NIT"", ""FCName"", ""FCMail"", ""FCPhone"", ""FCCity"", ""FCAddress""
                                         FROM    ( SELECT   DISTINCT t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", FIRST_VALUE(t1.""WhsCode"" ORDER BY ""LineNum"") AS ""WhsCode"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"", t0.""NumAtCard"" AS ""ClientNote"" 
                                                            , t0.""SlpCode"", SUM(t1.""Quantity"" * t1.""PriceAfVAT"") AS ""Total"" 
                                                            , SUM( t1.""Quantity"" * ( t1.""PriceAfVAT"" - t1.""StockPrice"" ) ) AS ""Margin"" 
                                                            , SUM( t1.""Quantity"" * t1.""PriceAfVAT"" ) AS ""TaxlessTotal"", t0.""DocStatus"", t0.""GroupNum"", t1.""BaseEntry"" AS ""OrderId"" 
                                                            , ( CASE WHEN IFNULL(t1.""BaseRef"", '') = '' THEN NULL ELSE CAST(t1.""BaseRef"" AS INTEGER) END ) AS ""OrderNumber"", t0.U_NUM_FACT AS ""BillNumber"", t0.""U_NROAUTOR"" AS ""AuthorizationNumber"" 
                                                            , t0.""Comments"", t0.""Address"" AS ""BillingAddress"", t0.""Address2"" AS ""DestinationAddress"", t0.""ShipToCode"" AS ""DestinationCode"", t0.U_INCOTERMS AS ""Incoterms"", t0.U_CORRELATIVO AS ""Correlative"", t0.""DiscSum"" AS ""Discount"", t0.""TrnspCode"" 
                                                            , t0.U_FACTURAR AS ""BilledTo"", t0.""CntctCode"", t0.U_ACREDIT AS ""Accredited"", ( CASE FIRST_VALUE(t1.""BaseType"" ORDER BY ""LineNum"") WHEN 15 THEN 'Y' ELSE 'N' END ) AS ""IsDeliveryNote"" 
                                                            , '' AS ""Latitude"", '' AS ""Longitude"", '' AS ""BusinessName"", '' AS ""NIT"", '' AS ""FCName"", '' AS ""FCMail"", '' AS ""FCPhone"", '' AS ""FCCity"", '' AS ""FCAddress""
                                                   FROM     {DBIQ}.OINV t0 
                                                            INNER JOIN {DBIQ}.INV1 t1 ON t1.""DocEntry"" = t0.""DocEntry"" 
                                                            INNER JOIN {DBIQ}.OITM t2 ON t2.""ItemCode"" = t1.""ItemCode"" 
                                                   WHERE    t0.CANCELED = 'N' AND  t1.""TargetType"" <> 14 AND   t0.""CardCode"" NOT IN ('CDMC-002', 'CDMC-003', 'CIMP-008') {InnerFilter}
                                                   GROUP BY t0.""DocEntry"", t0.""DocNum"", CAST(t0.""DocDate"" AS DATE), t0.""CardCode"", t0.""CardName"", t0.""NumAtCard"" 
                                                            , t0.""SlpCode"", t0.""DocStatus"", t0.""GroupNum"", t1.""BaseEntry"", ( CASE WHEN IFNULL(t1.""BaseRef"", '') = '' THEN NULL ELSE CAST(t1.""BaseRef"" AS INTEGER)END ), t0.U_NUM_FACT, t0.""U_NROAUTOR"", t0.""isIns"", t0.""Comments"", t0.""Address"", t0.""Address2"" 
                                                            , t0.""ShipToCode"", t0.U_INCOTERMS, t0.U_CORRELATIVO, t0.""DiscSum"", t0.""TrnspCode"", t0.U_FACTURAR, t0.""CntctCode"", t0.U_ACREDIT ) a
                                                 LEFT OUTER JOIN {DBIQ}.OWHS b ON a.""WhsCode"" = b.""WhsCode""  
                                         GROUP BY ""Id"", ""DocNumber"", ""DocDate"", ""ClientCode"", ""ClientName"", ""ClientNote"", ""SlpCode"", ""DocStatus"", ""GroupNum"", ""BillNumber"", ""AuthorizationNumber"", ""IsDeliveryNote"", ""Comments"", ""BillingAddress"", ""DestinationAddress"", ""DestinationCode""
                                                  , ""Incoterms"", ""Correlative"", ""TrnspCode"", ""BilledTo"", ""CntctCode"", ""Accredited"", ""Latitude"", ""Longitude"", ""BusinessName"", ""NIT"", ""FCName"", ""FCMail"", ""FCPhone"", ""FCCity"", ""FCAddress""  ) AS a1 
                                       LEFT OUTER JOIN {DBIQ}.OSLP a3 ON a3.""SlpCode"" = a1.""SlpCode"" 
                                       LEFT OUTER JOIN {DBIQ}.OCTG a4 ON a4.""GroupNum"" = a1.""GroupNum"" 
                                       LEFT OUTER JOIN {DBIQ}.OSHP a5 ON a1.""TrnspCode"" = a5.""TrnspCode"" 
                                       INNER JOIN {DBIQ}.OCRD a6 ON a6.""CardCode"" = a1.""ClientCode"" 
                                       LEFT OUTER JOIN {DBIQ}.OCPR a7 ON a7.""CntctCode"" = a1.""CntctCode"" 
                               {(string.IsNullOrEmpty(ItemFilter) ? "" : $"WHERE    {ItemFilter} ")} 
                               UNION ALL
                               SELECT  'Miami' AS ""Subsidiary"", ""Warehouse"", a1.""Id"", a1.""DocNumber"", a1.""DocDate"", a1.""ClientCode"", a1.""ClientName"", a1.""ClientNote"", a3.""SlpName"" AS ""SellerCode"", a3.""Memo"" AS ""SellerName"", a1.""Total"", a1.""Margin"" 
                                       , a1.""TaxlessTotal"", ( CASE a1.""DocStatus"" WHEN 'O' THEN 'Abierto' ELSE 'Cerrado' END ) AS ""State"", a4.""PymntGroup"" AS ""TermConditions"", a1.""OrderId"", a1.""OrderNumber"", a1.""BillNumber"", a1.""AuthorizationNumber"", a1.""IsDeliveryNote"" 
                                       , a1.""Comments"", a1.""BillingAddress"", a1.""DestinationAddress"", a1.""DestinationCode"", a1.""Incoterms"", a1.""Correlative"", a1.""Discount"", a5.""TrnspName"" AS ""Transport"", a1.""BilledTo"" 
                                       , a6.""Phone1"" AS ""Phone"", a6.""Cellular"" AS ""Cellphone"", a7.""Name"" AS ""NamePC"", a7.""Tel1"" AS ""PhonePC"", a7.""Cellolar"" AS ""CellphonePC"", a1.""Accredited"", ""Latitude"", ""Longitude"", ""BusinessName"", ""NIT"", ""FCName"", ""FCMail"", ""FCPhone"", ""FCCity"", ""FCAddress"" 
                               FROM    ( SELECT  ""Id"", ""DocNumber"", STRING_AGG(b.""WhsName"", ', ') AS ""Warehouse"", ""DocDate"", ""ClientCode"", ""ClientName"", ""ClientNote"", ""SlpCode"", SUM(""Total"") AS ""Total"", SUM(""Margin"") AS ""Margin"", SUM(""TaxlessTotal"") AS ""TaxlessTotal"", ""DocStatus"", ""GroupNum"", STRING_AGG(""OrderId"", ', ') AS ""OrderId""
                                                 , STRING_AGG(""OrderNumber"", ', ') AS ""OrderNumber"", ""BillNumber"", ""AuthorizationNumber"", ""IsDeliveryNote"", ""Comments"", ""BillingAddress"", ""DestinationAddress"", ""DestinationCode"", ""Incoterms"", ""Correlative"", SUM(""Discount"") AS ""Discount"", ""TrnspCode""
                                                 , ""BilledTo"", ""CntctCode"", ""Accredited"", ""Latitude"", ""Longitude"", ""BusinessName"", ""NIT"", ""FCName"", ""FCMail"", ""FCPhone"", ""FCCity"", ""FCAddress""
                                         FROM    ( SELECT   DISTINCT t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", FIRST_VALUE(t1.""WhsCode"" ORDER BY ""LineNum"") AS ""WhsCode"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"", t0.""NumAtCard"" AS ""ClientNote"" 
                                                            , t0.""SlpCode"", SUM(t1.""Quantity"" * t1.""PriceAfVAT"") AS ""Total"" 
                                                            , SUM( t1.""Quantity"" * ( t1.""PriceAfVAT"" - t1.""StockPrice"" ) ) AS ""Margin"" 
                                                            , SUM( t1.""Quantity"" * t1.""PriceAfVAT"" ) AS ""TaxlessTotal"", t0.""DocStatus"", t0.""GroupNum"", t1.""BaseEntry"" AS ""OrderId"" 
                                                            , ( CASE WHEN IFNULL(t1.""BaseRef"", '') = '' THEN NULL ELSE CAST(t1.""BaseRef"" AS INTEGER) END ) AS ""OrderNumber"", t0.U_NUM_FACT AS ""BillNumber"", t0.""U_NROAUTOR"" AS ""AuthorizationNumber"" 
                                                            , t0.""Comments"", t0.""Address"" AS ""BillingAddress"", t0.""Address2"" AS ""DestinationAddress"", t0.""ShipToCode"" AS ""DestinationCode"", t0.U_INCOTERMS AS ""Incoterms"", t0.U_CORRELATIVO AS ""Correlative"", t0.""DiscSum"" AS ""Discount"", t0.""TrnspCode"" 
                                                            , t0.U_FACTURAR AS ""BilledTo"", t0.""CntctCode"", t0.U_ACREDIT AS ""Accredited"", ( CASE FIRST_VALUE(t1.""BaseType"" ORDER BY ""LineNum"") WHEN 15 THEN 'Y' ELSE 'N' END ) AS ""IsDeliveryNote"" 
                                                            , '' AS ""Latitude"", '' AS ""Longitude"", '' AS ""BusinessName"", '' AS ""NIT"", '' AS ""FCName"", '' AS ""FCMail"", '' AS ""FCPhone"", '' AS ""FCCity"", '' AS ""FCAddress""
                                                   FROM     {DBLA}.OINV t0 
                                                            INNER JOIN {DBLA}.INV1 t1 ON t1.""DocEntry"" = t0.""DocEntry"" 
                                                            INNER JOIN {DBLA}.OITM t2 ON t2.""ItemCode"" = t1.""ItemCode"" 
                                                   WHERE    t0.CANCELED = 'N' AND  t1.""TargetType"" <> 14 AND   t0.""CardCode"" NOT IN ('CDMC-002', 'CDMC-003', 'CIMP-008') {InnerFilter}
                                                   GROUP BY t0.""DocEntry"", t0.""DocNum"", CAST(t0.""DocDate"" AS DATE), t0.""CardCode"", t0.""CardName"", t0.""NumAtCard"" 
                                                            , t0.""SlpCode"", t0.""DocStatus"", t0.""GroupNum"", t1.""BaseEntry"", ( CASE WHEN IFNULL(t1.""BaseRef"", '') = '' THEN NULL ELSE CAST(t1.""BaseRef"" AS INTEGER)END ), t0.U_NUM_FACT, t0.""U_NROAUTOR"", t0.""isIns"", t0.""Comments"", t0.""Address"", t0.""Address2"" 
                                                            , t0.""ShipToCode"", t0.U_INCOTERMS, t0.U_CORRELATIVO, t0.""DiscSum"", t0.""TrnspCode"", t0.U_FACTURAR, t0.""CntctCode"", t0.U_ACREDIT ) a
                                                 LEFT OUTER JOIN {DBLA}.OWHS b ON a.""WhsCode"" = b.""WhsCode""  
                                         GROUP BY ""Id"", ""DocNumber"", ""DocDate"", ""ClientCode"", ""ClientName"", ""ClientNote"", ""SlpCode"", ""DocStatus"", ""GroupNum"", ""BillNumber"", ""AuthorizationNumber"", ""IsDeliveryNote"", ""Comments"", ""BillingAddress"", ""DestinationAddress"", ""DestinationCode""
                                                   , ""Incoterms"", ""Correlative"", ""TrnspCode"", ""BilledTo"", ""CntctCode"", ""Accredited"", ""Latitude"", ""Longitude"", ""BusinessName"", ""NIT"", ""FCName"", ""FCMail"", ""FCPhone"", ""FCCity"", ""FCAddress""  ) AS a1 
                                       LEFT OUTER JOIN {DBLA}.OSLP a3 ON a3.""SlpCode"" = a1.""SlpCode"" 
                                       LEFT OUTER JOIN {DBLA}.OCTG a4 ON a4.""GroupNum"" = a1.""GroupNum"" 
                                       LEFT OUTER JOIN {DBLA}.OSHP a5 ON a1.""TrnspCode"" = a5.""TrnspCode"" 
                                       INNER JOIN {DBLA}.OCRD a6 ON a6.""CardCode"" = a1.""ClientCode"" 
                                       LEFT OUTER JOIN {DBLA}.OCPR a7 ON a7.""CntctCode"" = a1.""CntctCode"" 
                               {(string.IsNullOrEmpty(ItemFilter) ? "" : $"WHERE    {ItemFilter} ")} ";
            return query;
        }

        private string GetQuery3(string ItemFilter = null, string InnerFilter = null)
        {
            string query = $@"SELECT  DISTINCT 'Santa Cruz' AS ""Subsidiary"", a2.""WhsName"" AS ""Warehouse"", a1.""ClientCode"", a1.""ClientName"", a3.""SlpName"" AS ""SellerCode"", a3.""Memo"" AS ""SellerName"", a1.""Id"" AS ""NoteId"", a1.""DocNumber"" AS ""NoteNumber""
                                      , a1.""DocDate"" AS ""NoteDate"", a1.""ClientNote"" AS ""NoteClientOrder"", a1.""Total"" AS ""NoteTotal"", a1.""Margin"" AS ""NoteMargin"", a1.""TaxlessTotal"" AS ""NoteTaxlessTotal"", a4.""PymntGroup"" AS ""TermConditions"" 
                                      , a0.""BaseEntry"" AS ""OrderId"", ( CASE WHEN IFNULL(a0.""BaseRef"", '') = '' THEN NULL ELSE CAST(a0.""BaseRef"" AS INTEGER) END ) AS ""OrderNumber"", a1.""BillNumber"", a1.""AuthorizationNumber"", a1.""IsDeliveryNote"" 
                                      , ( CASE a1.""IsDeliveryNote"" WHEN 'Y' THEN a6.""NumAtCard"" ELSE a5.""NumAtCard"" END ) AS ""ClientOrder"", ( CASE a1.""IsDeliveryNote"" WHEN 'Y' THEN a6.""DocTotal"" ELSE a5.""DocTotal"" END ) AS ""OrderTotal""
                                      , ( CASE a1.""IsDeliveryNote"" WHEN 'Y' THEN CAST(a6.""DocDate"" AS DATE) ELSE CAST(a5.""DocDate"" AS DATE) END ) AS ""OrderDate""
                                      , ( CASE ( CASE a1.""IsDeliveryNote"" WHEN 'Y' THEN IFNULL(a6.""DocStatus"", '') ELSE IFNULL(a5.""DocStatus"", '') END ) WHEN '' THEN '' WHEN 'O' THEN 'Abierto' ELSE 'Cerrado' END ) AS ""OrderStatus""
                                      , ( CASE a1.""IsDeliveryNote"" WHEN 'Y' THEN a6.""Confirmed"" ELSE a5.""Confirmed"" END ) AS ""OrderAuthorized""
                                      , CAST(( CASE a1.""IsDeliveryNote"" WHEN 'Y' THEN a6.""Header"" ELSE a5.""Header"" END ) AS VARCHAR(5000)) AS ""Header""
                                      , CAST(( CASE a1.""IsDeliveryNote"" WHEN 'Y' THEN a6.""Footer"" ELSE a5.""Footer"" END ) AS VARCHAR(5000)) AS ""Footer""
                                      , ( CASE a1.""IsDeliveryNote"" WHEN 'Y' THEN ( SELECT CAST(COUNT(*) AS INT) 
                                                                                     FROM   {DBSA}.DLN1 r 
                                                                                            INNER JOIN {DBSA}.OITW w ON r.""ItemCode"" = w.""ItemCode"" AND r.""WhsCode"" = w.""WhsCode"" 
                                                                                     WHERE  ""DocEntry"" = a6.""DocEntry"" AND r.""ItemCode"" <> 'FLETES' AND r.""ItemCode"" NOT LIKE '%DMC%' AND  w.""OnHand"" < r.""OpenQty"" ) 
                                                                              ELSE ( SELECT CAST(COUNT(*) AS INT) 
                                                                                     FROM   {DBSA}.RDR1 r 
                                                                                            INNER JOIN {DBSA}.OITW w ON r.""ItemCode"" = w.""ItemCode"" AND r.""WhsCode"" = w.""WhsCode"" 
                                                                                     WHERE  ""DocEntry"" = a5.""DocEntry"" AND r.""ItemCode"" <> 'FLETES' AND r.""ItemCode"" NOT LIKE '%DMC%' AND  w.""OnHand"" < r.""OpenQty"" ) END ) AS ""NonCompleteItem""
                                      , ( CASE a1.""IsDeliveryNote"" WHEN 'Y' THEN ( SELECT SUM(r.""OpenQty"" * r.""PriceAfVAT"")
                                                                                     FROM   {DBSA}.DLN1 r 
                                                                                     WHERE  ""DocEntry"" = a6.""DocEntry"" ) 
                                                                              ELSE ( SELECT SUM(r.""OpenQty"" * r.""PriceAfVAT"")
                                                                                     FROM   {DBSA}.RDR1 r 
                                                                                     WHERE  ""DocEntry"" = a5.""DocEntry"" ) END ) AS ""OpenAmount""
                              FROM    ( SELECT    t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", t3.""WhsCode"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"", t0.""NumAtCard"" AS ""ClientNote"" 
                                                  , t0.""SlpCode"", SUM(t1.""Quantity"" * t1.""PriceAfVAT"") AS ""Total"" 
                                                  , SUM(( CASE WHEN t1.""WhsCode"" = 'ZFSCZ' THEN t1.""Quantity"" * ( t1.""PriceAfVAT"" - t1.""StockPrice"" ) ELSE t1.""Quantity"" * (( 0.84 * t1.""PriceAfVAT"" ) - t1.""StockPrice"" ) END )) AS ""Margin"" 
                                                  , SUM(( CASE WHEN t1.""WhsCode"" = 'ZFSCZ' THEN t1.""Quantity"" * t1.""PriceAfVAT"" ELSE t1.""Quantity"" * ( 0.84 * t1.""PriceAfVAT"" ) END )) AS ""TaxlessTotal"", t0.""GroupNum"", t0.U_NUM_FACT AS ""BillNumber""
                                                  , t0.""U_NROAUTOR"" AS ""AuthorizationNumber"", ( CASE FIRST_VALUE(t1.""BaseType"" ORDER BY ""LineNum"") WHEN 15 THEN 'Y' ELSE 'N' END ) AS ""IsDeliveryNote""
                                        FROM      {DBSA}.OINV t0 
                                                  INNER JOIN {DBSA}.INV1 t1 ON t1.""DocEntry"" = t0.""DocEntry"" 
                                                  INNER JOIN {DBSA}.OITM t2 ON t2.""ItemCode"" = t1.""ItemCode"" 
                                                  LEFT OUTER JOIN ( SELECT  s0.""DocEntry"", s0.""WhsCode"" 
                                                                    FROM    {DBSA}.INV1 s0 
                                                                    WHERE   ""LineNum"" = ( SELECT MIN(""LineNum"") FROM {DBSA}.INV1 WHERE ""DocEntry"" = s0.""DocEntry"" ) ) t3 ON t3.""DocEntry"" = t0.""DocEntry"" 
                                        WHERE     t0.CANCELED = 'N' AND  t1.""TargetType"" <> 14 {InnerFilter}
                                        GROUP BY  t0.""DocEntry"", t0.""DocNum"", t3.""WhsCode"", CAST(t0.""DocDate"" AS DATE), t0.""CardCode"", t0.""CardName"", t0.""NumAtCard"", t0.""SlpCode"", t0.""GroupNum"", t0.U_NUM_FACT, t0.""U_NROAUTOR"" ) AS a1 
                                      INNER JOIN {DBSA}.INV1 a0 ON a1.""Id"" = a0.""DocEntry""
                                      LEFT OUTER JOIN {DBSA}.OWHS a2 ON a2.""WhsCode"" = a1.""WhsCode"" 
                                      LEFT OUTER JOIN {DBSA}.OSLP a3 ON a3.""SlpCode"" = a1.""SlpCode"" 
                                      LEFT OUTER JOIN {DBSA}.OCTG a4 ON a4.""GroupNum"" = a1.""GroupNum"" 
                                      LEFT OUTER JOIN {DBSA}.ORDR a5 ON a0.""BaseEntry"" = a5.""DocEntry""
                                      LEFT OUTER JOIN {DBSA}.ODLN a6 ON IFNULL(a0.""BaseRef"", '') = CAST(a6.""DocNum"" AS VARCHAR(10))
                              WHERE  1 = 1 {ItemFilter} 
                              UNION ALL 
                              SELECT  DISTINCT 'Iquique' AS ""Subsidiary"", a2.""WhsName"" AS ""Warehouse"", a1.""ClientCode"", a1.""ClientName"", a3.""SlpName"" AS ""SellerCode"", a3.""Memo"" AS ""SellerName"", a1.""Id"" AS ""NoteId"", a1.""DocNumber"" AS ""NoteNumber""
                                      , a1.""DocDate"" AS ""NoteDate"", a1.""ClientNote"" AS ""NoteClientOrder"", a1.""Total"" AS ""NoteTotal"", a1.""Margin"" AS ""NoteMargin"", a1.""TaxlessTotal"" AS ""NoteTaxlessTotal"", a4.""PymntGroup"" AS ""TermConditions"" 
                                      , a0.""BaseEntry"" AS ""OrderId"", ( CASE WHEN IFNULL(a0.""BaseRef"", '') = '' THEN NULL ELSE CAST(a0.""BaseRef"" AS INTEGER) END ) AS ""OrderNumber"", a1.""BillNumber"", a1.""AuthorizationNumber"", a1.""IsDeliveryNote"" 
                                      , ( CASE a1.""IsDeliveryNote"" WHEN 'Y' THEN a6.""NumAtCard"" ELSE a5.""NumAtCard"" END ) AS ""ClientOrder"", ( CASE a1.""IsDeliveryNote"" WHEN 'Y' THEN a6.""DocTotal"" ELSE a5.""DocTotal"" END ) AS ""OrderTotal""
                                      , ( CASE a1.""IsDeliveryNote"" WHEN 'Y' THEN CAST(a6.""DocDate"" AS DATE) ELSE CAST(a5.""DocDate"" AS DATE) END ) AS ""OrderDate""
                                      , ( CASE ( CASE a1.""IsDeliveryNote"" WHEN 'Y' THEN IFNULL(a6.""DocStatus"", '') ELSE IFNULL(a5.""DocStatus"", '') END ) WHEN '' THEN '' WHEN 'O' THEN 'Abierto' ELSE 'Cerrado' END ) AS ""OrderStatus""
                                      , ( CASE a1.""IsDeliveryNote"" WHEN 'Y' THEN a6.""Confirmed"" ELSE a5.""Confirmed"" END ) AS ""OrderAuthorized""
                                      , CAST(( CASE a1.""IsDeliveryNote"" WHEN 'Y' THEN a6.""Header"" ELSE a5.""Header"" END ) AS VARCHAR(5000)) AS ""Header""
                                      , CAST(( CASE a1.""IsDeliveryNote"" WHEN 'Y' THEN a6.""Footer"" ELSE a5.""Footer"" END ) AS VARCHAR(5000)) AS ""Footer""
                                      , ( CASE a1.""IsDeliveryNote"" WHEN 'Y' THEN ( SELECT CAST(COUNT(*) AS INT) 
                                                                                     FROM   {DBIQ}.DLN1 r 
                                                                                            INNER JOIN {DBIQ}.OITW w ON r.""ItemCode"" = w.""ItemCode"" AND r.""WhsCode"" = w.""WhsCode"" 
                                                                                     WHERE  ""DocEntry"" = a6.""DocEntry"" AND r.""ItemCode"" <> 'FLETES' AND r.""ItemCode"" NOT LIKE '%DMC%' AND  w.""OnHand"" < r.""OpenQty"" ) 
                                                                              ELSE ( SELECT CAST(COUNT(*) AS INT) 
                                                                                     FROM   {DBIQ}.RDR1 r 
                                                                                            INNER JOIN {DBIQ}.OITW w ON r.""ItemCode"" = w.""ItemCode"" AND r.""WhsCode"" = w.""WhsCode"" 
                                                                                     WHERE  ""DocEntry"" = a5.""DocEntry"" AND r.""ItemCode"" <> 'FLETES' AND r.""ItemCode"" NOT LIKE '%DMC%' AND  w.""OnHand"" < r.""OpenQty"" ) END ) AS ""NonCompleteItem""
                                      , ( CASE a1.""IsDeliveryNote"" WHEN 'Y' THEN ( SELECT SUM(r.""OpenQty"" * r.""PriceAfVAT"")
                                                                                     FROM   {DBIQ}.DLN1 r 
                                                                                     WHERE  ""DocEntry"" = a6.""DocEntry"" ) 
                                                                              ELSE ( SELECT SUM(r.""OpenQty"" * r.""PriceAfVAT"")
                                                                                     FROM   {DBIQ}.RDR1 r 
                                                                                     WHERE  ""DocEntry"" = a5.""DocEntry"" ) END ) AS ""OpenAmount""
                              FROM    ( SELECT    t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", t3.""WhsCode"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"", t0.""NumAtCard"" AS ""ClientNote"" 
                                                  , t0.""SlpCode"", SUM(t1.""Quantity"" * t1.""PriceAfVAT"") AS ""Total"", SUM(t1.""Quantity"" * ( t1.""PriceAfVAT"" - t1.""StockPrice"" )) AS ""Margin"", SUM(t1.""Quantity"" * t1.""PriceAfVAT"") AS ""TaxlessTotal""
                                                  , t0.""GroupNum"", t0.U_NUM_FACT AS ""BillNumber"", t0.""U_NROAUTOR"" AS ""AuthorizationNumber"", ( CASE FIRST_VALUE(t1.""BaseType"" ORDER BY ""LineNum"") WHEN 15 THEN 'Y' ELSE 'N' END ) AS ""IsDeliveryNote""
                                        FROM      {DBIQ}.OINV t0 
                                                  INNER JOIN {DBIQ}.INV1 t1 ON t1.""DocEntry"" = t0.""DocEntry"" 
                                                  INNER JOIN {DBIQ}.OITM t2 ON t2.""ItemCode"" = t1.""ItemCode"" 
                                                  LEFT OUTER JOIN ( SELECT  s0.""DocEntry"", s0.""WhsCode"" 
                                                                    FROM    {DBIQ}.INV1 s0 
                                                                    WHERE   ""LineNum"" = ( SELECT MIN(""LineNum"") FROM {DBIQ}.INV1 WHERE ""DocEntry"" = s0.""DocEntry"" ) ) t3 ON t3.""DocEntry"" = t0.""DocEntry"" 
                                        WHERE     t0.CANCELED = 'N' AND  t1.""TargetType"" <> 14 {InnerFilter}
                                        GROUP BY  t0.""DocEntry"", t0.""DocNum"", t3.""WhsCode"", CAST(t0.""DocDate"" AS DATE), t0.""CardCode"", t0.""CardName"", t0.""NumAtCard"", t0.""SlpCode"", t0.""GroupNum"", t0.U_NUM_FACT, t0.""U_NROAUTOR"" ) AS a1 
                                      INNER JOIN {DBIQ}.INV1 a0 ON a1.""Id"" = a0.""DocEntry""
                                      LEFT OUTER JOIN {DBIQ}.OWHS a2 ON a2.""WhsCode"" = a1.""WhsCode"" 
                                      LEFT OUTER JOIN {DBIQ}.OSLP a3 ON a3.""SlpCode"" = a1.""SlpCode"" 
                                      LEFT OUTER JOIN {DBIQ}.OCTG a4 ON a4.""GroupNum"" = a1.""GroupNum"" 
                                      LEFT OUTER JOIN {DBIQ}.ORDR a5 ON a0.""BaseEntry"" = a5.""DocEntry""
                                      LEFT OUTER JOIN {DBIQ}.ODLN a6 ON IFNULL(a0.""BaseRef"", '') = CAST(a6.""DocNum"" AS VARCHAR(10))
                              WHERE  1 = 1 {ItemFilter} 
                              UNION ALL 
                              SELECT  DISTINCT 'Miami' AS ""Subsidiary"", a2.""WhsName"" AS ""Warehouse"", a1.""ClientCode"", a1.""ClientName"", a3.""SlpName"" AS ""SellerCode"", a3.""Memo"" AS ""SellerName"", a1.""Id"" AS ""NoteId"", a1.""DocNumber"" AS ""NoteNumber""
                                      , a1.""DocDate"" AS ""NoteDate"", a1.""ClientNote"" AS ""NoteClientOrder"", a1.""Total"" AS ""NoteTotal"", a1.""Margin"" AS ""NoteMargin"", a1.""TaxlessTotal"" AS ""NoteTaxlessTotal"", a4.""PymntGroup"" AS ""TermConditions"" 
                                      , a0.""BaseEntry"" AS ""OrderId"", ( CASE WHEN IFNULL(a0.""BaseRef"", '') = '' THEN NULL ELSE CAST(a0.""BaseRef"" AS INTEGER) END ) AS ""OrderNumber"", a1.""BillNumber"", a1.""AuthorizationNumber"", a1.""IsDeliveryNote"" 
                                      , ( CASE a1.""IsDeliveryNote"" WHEN 'Y' THEN a6.""NumAtCard"" ELSE a5.""NumAtCard"" END ) AS ""ClientOrder"", ( CASE a1.""IsDeliveryNote"" WHEN 'Y' THEN a6.""DocTotal"" ELSE a5.""DocTotal"" END ) AS ""OrderTotal""
                                      , ( CASE a1.""IsDeliveryNote"" WHEN 'Y' THEN CAST(a6.""DocDate"" AS DATE) ELSE CAST(a5.""DocDate"" AS DATE) END ) AS ""OrderDate""
                                      , ( CASE ( CASE a1.""IsDeliveryNote"" WHEN 'Y' THEN IFNULL(a6.""DocStatus"", '') ELSE IFNULL(a5.""DocStatus"", '') END ) WHEN '' THEN '' WHEN 'O' THEN 'Abierto' ELSE 'Cerrado' END ) AS ""OrderStatus""
                                      , ( CASE a1.""IsDeliveryNote"" WHEN 'Y' THEN a6.""Confirmed"" ELSE a5.""Confirmed"" END ) AS ""OrderAuthorized""
                                      , CAST(( CASE a1.""IsDeliveryNote"" WHEN 'Y' THEN a6.""Header"" ELSE a5.""Header"" END ) AS VARCHAR(5000)) AS ""Header""
                                      , CAST(( CASE a1.""IsDeliveryNote"" WHEN 'Y' THEN a6.""Footer"" ELSE a5.""Footer"" END ) AS VARCHAR(5000)) AS ""Footer""
                                      , ( CASE a1.""IsDeliveryNote"" WHEN 'Y' THEN ( SELECT CAST(COUNT(*) AS INT) 
                                                                                     FROM   {DBLA}.DLN1 r 
                                                                                            INNER JOIN {DBLA}.OITW w ON r.""ItemCode"" = w.""ItemCode"" AND r.""WhsCode"" = w.""WhsCode"" 
                                                                                     WHERE  ""DocEntry"" = a6.""DocEntry"" AND r.""ItemCode"" <> 'FLETES' AND r.""ItemCode"" NOT LIKE '%DMC%' AND  w.""OnHand"" < r.""OpenQty"" ) 
                                                                              ELSE ( SELECT CAST(COUNT(*) AS INT) 
                                                                                     FROM   {DBLA}.RDR1 r 
                                                                                            INNER JOIN {DBLA}.OITW w ON r.""ItemCode"" = w.""ItemCode"" AND r.""WhsCode"" = w.""WhsCode"" 
                                                                                     WHERE  ""DocEntry"" = a5.""DocEntry"" AND r.""ItemCode"" <> 'FLETES' AND r.""ItemCode"" NOT LIKE '%DMC%' AND  w.""OnHand"" < r.""OpenQty"" ) END ) AS ""NonCompleteItem""
                                      , ( CASE a1.""IsDeliveryNote"" WHEN 'Y' THEN ( SELECT SUM(r.""OpenQty"" * r.""PriceAfVAT"")
                                                                                     FROM   {DBLA}.DLN1 r 
                                                                                     WHERE  ""DocEntry"" = a6.""DocEntry"" ) 
                                                                              ELSE ( SELECT SUM(r.""OpenQty"" * r.""PriceAfVAT"")
                                                                                     FROM   {DBLA}.RDR1 r 
                                                                                     WHERE  ""DocEntry"" = a5.""DocEntry"" ) END ) AS ""OpenAmount""
                              FROM    ( SELECT    t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", t3.""WhsCode"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"", t0.""NumAtCard"" AS ""ClientNote"" 
                                                  , t0.""SlpCode"", SUM(t1.""Quantity"" * t1.""PriceAfVAT"") AS ""Total"", SUM(t1.""Quantity"" * ( t1.""PriceAfVAT"" - t1.""StockPrice"" )) AS ""Margin"", SUM(t1.""Quantity"" * t1.""PriceAfVAT"") AS ""TaxlessTotal""
                                                  , t0.""GroupNum"", t0.U_NUM_FACT AS ""BillNumber"", t0.""U_NROAUTOR"" AS ""AuthorizationNumber"", ( CASE FIRST_VALUE(t1.""BaseType"" ORDER BY ""LineNum"") WHEN 15 THEN 'Y' ELSE 'N' END ) AS ""IsDeliveryNote""
                                        FROM      {DBLA}.OINV t0 
                                                  INNER JOIN {DBLA}.INV1 t1 ON t1.""DocEntry"" = t0.""DocEntry"" 
                                                  INNER JOIN {DBLA}.OITM t2 ON t2.""ItemCode"" = t1.""ItemCode"" 
                                                  LEFT OUTER JOIN ( SELECT  s0.""DocEntry"", s0.""WhsCode"" 
                                                                    FROM    {DBLA}.INV1 s0 
                                                                    WHERE   ""LineNum"" = ( SELECT MIN(""LineNum"") FROM {DBLA}.INV1 WHERE ""DocEntry"" = s0.""DocEntry"" ) ) t3 ON t3.""DocEntry"" = t0.""DocEntry"" 
                                        WHERE     t0.CANCELED = 'N' AND  t1.""TargetType"" <> 14 {InnerFilter}
                                        GROUP BY  t0.""DocEntry"", t0.""DocNum"", t3.""WhsCode"", CAST(t0.""DocDate"" AS DATE), t0.""CardCode"", t0.""CardName"", t0.""NumAtCard"", t0.""SlpCode"", t0.""GroupNum"", t0.U_NUM_FACT, t0.""U_NROAUTOR"" ) AS a1 
                                      INNER JOIN {DBLA}.INV1 a0 ON a1.""Id"" = a0.""DocEntry""
                                      LEFT OUTER JOIN {DBLA}.OWHS a2 ON a2.""WhsCode"" = a1.""WhsCode"" 
                                      LEFT OUTER JOIN {DBLA}.OSLP a3 ON a3.""SlpCode"" = a1.""SlpCode"" 
                                      LEFT OUTER JOIN {DBLA}.OCTG a4 ON a4.""GroupNum"" = a1.""GroupNum"" 
                                      LEFT OUTER JOIN {DBLA}.ORDR a5 ON a0.""BaseEntry"" = a5.""DocEntry""
                                      LEFT OUTER JOIN {DBLA}.ODLN a6 ON IFNULL(a0.""BaseRef"", '') = CAST(a6.""DocNum"" AS VARCHAR(10))
                              WHERE  1 = 1 {ItemFilter} ";
            return query;
        }

        private string GetQueryWithoutOrder()
        {
            var sb = new StringBuilder();
            sb.AppendLine($@"SELECT  t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", 'Santa Cruz' AS ""Subsidiary"", t1.""WhsName"" AS ""Warehouse"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"" ");
            sb.AppendLine($@"        , t0.""NumAtCard"" AS ""ClientNote"", t2.""SlpName"" AS ""SellerCode"", t2.""Memo"" AS ""SellerName"", t0.""DocTotal"" AS ""Total"", ( CASE t0.""DocStatus"" WHEN 'O' THEN 'Abierto' ELSE 'Cerrado' END ) AS ""State"", t3.""PymntGroup"" AS ""TermConditions"" ");
            sb.AppendLine($@"        , NULL AS ""OrderId"", NULL AS ""OrderNumber"", t0.U_NUM_FACT AS ""BillNumber"", t0.""U_NROAUTOR"" AS ""AuthorizationNumber"", t0.""isIns"" AS ""IsDeliveryNote"" ");
            sb.AppendLine($@"FROM    {DBSA}.OINV t0 ");
            sb.AppendLine($@"        LEFT OUTER JOIN {DBSA}.OWHS t1 ON t1.""WhsCode"" = t0.U_ALMACEN ");
            sb.AppendLine($@"        INNER JOIN {DBSA}.OSLP t2 ON t0.""SlpCode"" = t2.""SlpCode"" ");
            sb.AppendLine($@"        INNER JOIN {DBSA}.OCTG t3 ON t0.""GroupNum"" = t3.""GroupNum"" ");
            sb.AppendLine($@"        INNER JOIN {DBSA}.INV1 t4 ON t4.""DocEntry"" = t0.""DocEntry"" AND ( t4.""BaseRef"" IS NULL OR t4.""BaseEntry"" IS NULL ) ");
            sb.AppendLine($@"WHERE   t0.CANCELED = 'N' AND   t4.""TargetType"" <> 14 ");
            sb.AppendLine($@"UNION ");
            sb.AppendLine($@"SELECT  t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", 'Miami' AS ""Subsidiary"", t1.""WhsName"" AS ""Warehouse"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"" ");
            sb.AppendLine($@"        , t0.""NumAtCard"" AS ""ClientNote"", t2.""SlpName"" AS ""SellerCode"", t2.""Memo"" AS ""SellerName"", t0.""DocTotal"" AS ""Total"", ( CASE t0.""DocStatus"" WHEN 'O' THEN 'Abierto' ELSE 'Cerrado' END ) AS ""State"", t3.""PymntGroup"" AS ""TermConditions"" ");
            sb.AppendLine($@"        , NULL AS ""OrderId"", NULL AS ""OrderNumber"", t0.U_NUM_FACT AS ""BillNumber"", t0.""U_NROAUTOR"" AS ""AuthorizationNumber"", t0.""isIns"" AS ""IsDeliveryNote"" ");
            sb.AppendLine($@"FROM    {DBLA}.OINV t0 ");
            sb.AppendLine($@"        LEFT OUTER JOIN {DBLA}.OWHS t1 ON t1.""WhsCode"" = t0.U_ALMACEN ");
            sb.AppendLine($@"        INNER JOIN {DBLA}.OSLP t2 ON t0.""SlpCode"" = t2.""SlpCode"" ");
            sb.AppendLine($@"        INNER JOIN {DBLA}.OCTG t3 ON t0.""GroupNum"" = t3.""GroupNum"" ");
            sb.AppendLine($@"        INNER JOIN {DBLA}.INV1 t4 ON t4.""DocEntry"" = t0.""DocEntry"" AND   ( t4.""BaseRef"" IS NULL OR t4.""BaseEntry"" IS NULL ) ");
            sb.AppendLine($@"WHERE   t0.CANCELED = 'N' AND   t4.""TargetType"" <> 14 ");
            sb.AppendLine($@"UNION ");
            sb.AppendLine($@"SELECT  t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", 'Iquique' AS ""Subsidiary"", t1.""WhsName"" AS ""Warehouse"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"" ");
            sb.AppendLine($@"        , t0.""NumAtCard"" AS ""ClientNote"", t2.""SlpName"" AS ""SellerCode"", t2.""Memo"" AS ""SellerName"", t0.""DocTotal"" AS ""Total"", ( CASE t0.""DocStatus"" WHEN 'O' THEN 'Abierto' ELSE 'Cerrado' END ) AS ""State"", t3.""PymntGroup"" AS ""TermConditions"" ");
            sb.AppendLine($@"        , NULL AS ""OrderId"", NULL AS ""OrderNumber"", t0.U_NUM_FACT AS ""BillNumber"", t0.""U_NROAUTOR"" AS ""AuthorizationNumber"", t0.""isIns"" AS ""IsDeliveryNote"" ");
            sb.AppendLine($@"FROM    {DBIQ}.OINV t0 ");
            sb.AppendLine($@"        LEFT OUTER JOIN {DBIQ}.OWHS t1 ON t1.""WhsCode"" = t0.U_ALMACEN ");
            sb.AppendLine($@"        INNER JOIN {DBIQ}.OSLP t2 ON t0.""SlpCode"" = t2.""SlpCode"" ");
            sb.AppendLine($@"        INNER JOIN {DBIQ}.OCTG t3 ON t0.""GroupNum"" = t3.""GroupNum"" ");
            sb.AppendLine($@"        INNER JOIN {DBIQ}.INV1 t4 ON t4.""DocEntry"" = t0.""DocEntry"" AND ( t4.""BaseRef"" IS NULL OR t4.""BaseEntry"" IS NULL ) ");
            sb.AppendLine($@"WHERE   t0.CANCELED = 'N' AND   t4.""TargetType"" <> 14 ");
            return sb.ToString();
        }

        #endregion

        #region Constructors

        public Note() : base() { }

        #endregion
    }
}
