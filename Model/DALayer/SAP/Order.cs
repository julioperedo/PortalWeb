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
    public class Order : DALEntity<BEA.Order>
    {

        #region Methods

        internal BEA.Order ReturnMaster(int Id, string Subsidiary, params Enum[] Relations)
        {
            return Search(Id, Subsidiary, Relations);
        }

        internal IEnumerable<BEA.Order> ReturnMaster(IEnumerable<string> Keys, params Enum[] Relations)
        {
            StringBuilder sb = new();
            sb.AppendLine($@"SELECT  * ");
            sb.AppendLine($@"FROM    ({GetQuery()}) a ");
            sb.AppendLine($@"WHERE   LOWER(a.""Subsidiary"") || '-' || CAST(a.""Id"" AS VARCHAR(5000)) IN ( {string.Join(",", Keys.Select(x => $"'{x.Replace("'", "")}'"))} ) ");

            IEnumerable<BEA.Order> items = SQLList(sb.ToString(), Relations);
            return items;
        }

        protected override void LoadRelations(ref IEnumerable<BEA.Order> Items, params Enum[] Relations)
        {
            IEnumerable<BEA.OrderItem> lstOrderItems = null;
            IEnumerable<BEA.Note> lstNotes = null;
            IEnumerable<BEA.OrderFile> lstFiles = null;

            IEnumerable<string> Keys;
            foreach (Enum RelationEnum in Relations)
            {
                Keys = from i in Items select $"{i.Subsidiary.ToLower()}-{i.Id}";
                if (RelationEnum.Equals(BEA.relOrder.OrderItems))
                {
                    using OrderItem dalItem = new();
                    lstOrderItems = dalItem.List(Keys, Relations);
                }
                if (RelationEnum.Equals(BEA.relOrder.Notes))
                {
                    using Note dalNote = new();
                    lstNotes = dalNote.ReturnChild(Keys, Relations);
                }
                if (RelationEnum.Equals(BEA.relOrder.Files))
                {
                    using OrderFile dalFile = new();
                    lstFiles = dalFile.ReturnChild(Keys, Relations);
                }
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var item in Items)
                {
                    if (lstOrderItems?.Count() > 0)
                    {
                        item.Items = (from i in lstOrderItems where i.Subsidiary.ToLower() == item.Subsidiary.ToLower() & i.OrderId == item.Id select i).ToList();
                    }
                    if (lstNotes?.Count() > 0)
                    {
                        item.Notes = lstNotes.Where(i => i.Subsidiary.ToLower() == item.Subsidiary.ToLower() & i.OrderId.Contains(item.Id.ToString()))?.ToList();
                    }
                    if (lstFiles?.Count() > 0)
                    {
                        item.Files = lstFiles.Where(i => i.Subsidiary.ToLower() == item.Subsidiary.ToLower() & i.DocEntry == item.Id)?.ToList();
                    }
                }
            }
        }

        protected override void LoadRelations(ref BEA.Order item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
                string[] Keys = new[] { $"{item.Subsidiary.ToLower()}-{item.Id}" };
                if (RelationEnum.Equals(BEA.relOrder.OrderItems))
                {
                    using OrderItem dalItem = new();
                    item.Items = dalItem.List(Keys, Relations)?.ToList();
                }
                if (RelationEnum.Equals(BEA.relOrder.Notes))
                {
                    using Note dalNote = new();
                    item.Notes = dalNote.ReturnChild(Keys, Relations)?.ToList();
                }
                if (RelationEnum.Equals(BEA.relOrder.Files))
                {
                    using OrderFile dalFile = new();
                    item.Files = dalFile.ReturnChild(Keys, Relations)?.ToList();
                }
            }
        }

        #endregion

        #region List Methods

        public IEnumerable<BEA.Order> List(List<Field> Filters, string Order, params Enum[] Relations)
        {
            string strFilter = Filters?.Count > 0 ? GetFilter(Filters?.ToArray()) : "";
            StringBuilder sb = new();
            sb.AppendLine($@"SELECT   * ");
            sb.AppendLine($@"FROM     ({GetQuery()}) a ");
            if (strFilter != "") sb.AppendLine($@"WHERE    {strFilter} ");
            sb.AppendLine($@"ORDER By {GetOrder(Order)} ");

            IEnumerable<BEA.Order> items = SQLList(sb.ToString(), Relations);
            return items;
        }

        public IEnumerable<BEA.Order> List2(List<Field> Filters, string Order, params Enum[] Relations)
        {
            string strFilter = "";
            if (Filters?.Count > 0)
            {
                strFilter = GetFilter(Filters?.ToArray());
            }
            StringBuilder sb = new();
            sb.AppendLine($@"SELECT   * ");
            sb.AppendLine($@"FROM     ({GetQuery2()}) a ");
            if (strFilter != "") sb.AppendLine($@"WHERE    {strFilter} ");
            sb.AppendLine($@"ORDER By {GetOrder(Order)} ");

            IEnumerable<BEA.Order> items = SQLList(sb.ToString(), Relations);
            return items;
        }

        public IEnumerable<BEA.Order> List4(List<Field> Filters, List<Field> InnerFilters, string Order, params Enum[] Relations)
        {
            string filter = Filters?.Count > 0 ? GetFilter(Filters?.ToArray()) : "";
            string innerFilter = InnerFilters?.Count > 0 ? $" AND {GetFilter(InnerFilters?.ToArray())}" : "";

            StringBuilder sb = new();
            sb.AppendLine($@"SELECT   * ");
            sb.AppendLine($@"FROM     ({GetQueryFiltered(innerFilter)}) a ");
            if (filter != "") sb.AppendLine($@"WHERE    {filter} ");
            sb.AppendLine($@"ORDER By {GetOrder(Order)} ");

            IEnumerable<BEA.Order> items = SQLList(sb.ToString(), Relations);
            return items;
        }

        public IEnumerable<BEA.OrderDestination> ListDestinations(string DocNumbers)
        {
            string query = $@"SELECT  ""DocNum"" AS ""DocNumber"", t0.""CardCode"" AS ""ClientCode"", t1.""E_Mail"" AS ""EMail""
                              FROM    {DBSA}.ORDR t0
                                      INNER JOIN {DBSA}.OCRD t1 ON t0.""CardCode"" = t1.""CardCode""
                              WHERE   ""DocNum"" IN ( {DocNumbers} ) ";
            IEnumerable<BEA.OrderDestination> items = SQLList<BEA.OrderDestination>(query);
            return items;
        }

        public IEnumerable<BEA.DocumentRelated> ListRelatedDocuments(string Subsidiary, string DocIds)
        {
            string dbName = Subsidiary.ToLower() == "santa cruz" ? DBSA : (Subsidiary.ToLower() == "iquique" ? DBIQ : DBLA);
            string query = $@"SELECT  DISTINCT '{Subsidiary}' AS ""Subsidiary"", t0.""DocEntry"" AS ""Id"", 'Nota de Entrega' AS ""DocType"", t0.""DocNum"" AS ""DocNumber"", t1.""BaseEntry"" AS ""BaseId"", 0 AS ""Series""
                              FROM    {dbName}.ODLN t0
                                      INNER JOIN {dbName}.DLN1 t1 ON T0.""DocEntry"" = T1.""DocEntry""
                              WHERE   T0.CANCELED = 'N' AND t0.""DocEntry"" IN ( SELECT ""TrgetEntry"" FROM {dbName}.RDR1 WHERE ""TargetType"" = 15 AND ""DocEntry"" IN ( {DocIds} ) )         
                              UNION
                              SELECT  DISTINCT '{Subsidiary}' AS ""Subsidiary"", t0.""DocEntry"" AS ""Id"", 'Nota de Venta' AS ""DocType"", t0.""DocNum"" AS ""DocNumber"", t1.""BaseEntry"" AS ""BaseId"", t0.""Series""
                              FROM    {dbName}.OINV t0
                                      INNER JOIN {dbName}.INV1 t1 ON T0.""DocEntry"" = T1.""DocEntry"" AND t1.""TargetType"" <> 14
                              WHERE   T0.CANCELED = 'N' AND t0.""DocEntry"" IN ( SELECT ""TrgetEntry"" FROM {dbName}.RDR1 WHERE ""TargetType"" = 13 AND ""DocEntry"" IN ( {DocIds} ) )      
                              UNION
                              SELECT  DISTINCT '{Subsidiary}' AS ""Subsidiary"", t0.""DocEntry"" AS ""Id"", 'Nota de Venta' AS ""DocType"", t0.""DocNum"" AS ""DocNumber"", t4.""DocEntry"" AS ""BaseId"", t0.""Series""
                              FROM    {dbName}.OINV t0
                                      INNER JOIN {dbName}.INV1 t1 ON T0.""DocEntry"" = T1.""DocEntry"" AND t1.""TargetType"" <> 14
                                      INNER JOIN {dbName}.DLN1 t2 ON t0.""DocEntry"" = t2.""TrgetEntry"" AND t2.""TargetType"" = 13
                                      INNER JOIN {dbName}.ODLN t3 ON t2.""DocEntry"" = t3.""DocEntry"" AND t3.CANCELED = 'N'
                                      INNER JOIN {dbName}.RDR1 t4 ON t2.""DocEntry"" = t4.""TrgetEntry"" AND t4.""TargetType"" = 15 AND t4.""DocEntry"" IN ( {DocIds} )
                              WHERE   T0.CANCELED = 'N' ";
            //UNION
            //SELECT  DISTINCT '{Subsidiary}' AS ""Subsidiary"", t0.""DocEntry"" AS ""Id"", 'Nota de Entrega' AS ""DocType"", t0.""DocNum"" AS ""DocNumber"", t4.""DocEntry"" AS ""BaseId""
            //FROM    {dbName}.ODLN t0
            //        INNER JOIN {dbName}.DLN1 t1 ON t0.""DocEntry"" = t1.""DocEntry""
            //        INNER JOIN {dbName}.INV1 t2 ON t0.""DocEntry"" = t2.""TrgetEntry"" AND t2.""TargetType"" = 15
            //        INNER JOIN {dbName}.OINV t3 ON t2.""DocEntry"" = t3.""DocEntry"" AND t3.CANCELED = 'N'
            //        INNER JOIN {dbName}.RDR1 t4 ON t2.""DocEntry"" = t4.""TrgetEntry"" AND t4.""TargetType"" = 13 AND t4.""DocEntry"" IN ( {DocIds} )
            //WHERE   t0.CANCELED = 'N' ";
            IEnumerable<BEA.DocumentRelated> items = SQLList<BEA.DocumentRelated>(query);
            return items;
        }

        #endregion

        #region Search Methods

        public BEA.Order Search(int DocNumber, string Subsidiary, params Enum[] Relations)
        {
            StringBuilder sb = new();
            sb.AppendLine($@"SELECT   * ");
            sb.AppendLine($@"FROM     ({GetQuery()}) a ");
            sb.AppendLine($@"WHERE    ""DocNumber"" = {DocNumber} AND LOWER(""Subsidiary"") = '{Subsidiary.ToLower()}' ");

            BEA.Order item = SQLSearch(sb.ToString(), Relations);
            return item;
        }

        public BEA.Order Search2(int DocNUmber, string Subsidiary, params Enum[] Relations)
        {
            StringBuilder sb = new();
            sb.AppendLine($@"SELECT   * ");
            sb.AppendLine($@"FROM     ({GetQuery4()}) a ");
            sb.AppendLine($@"WHERE    ""DocNumber"" = {DocNUmber} AND LOWER(""Subsidiary"") = '{Subsidiary.ToLower()}' ");

            BEA.Order item = SQLSearch(sb.ToString(), Relations);
            return item;
        }

        public BEA.OrderExtended SearchExtended(int DocNUmber, string Subsidiary)
        {
            StringBuilder sb = new();
            sb.AppendLine($@"SELECT   * ");
            sb.AppendLine($@"FROM     ({GetQuery3()}) a ");
            sb.AppendLine($@"WHERE    ""DocNumber"" = {DocNUmber} AND LOWER(""Subsidiary"") = '{Subsidiary.ToLower()}' ");

            BEA.OrderExtended item = SQLSearch<BEA.OrderExtended>(sb.ToString());
            return item;
        }

        #endregion

        #region Private Methods

        private string GetQuery()
        {
            string query = $@"SELECT  t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", 'Santa Cruz' AS ""Subsidiary"", t1.""WhsName"" AS ""Warehouse"", CAST(t0.""DocDate"" AS date) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"" 
                                      , t0.""NumAtCard"" AS ""ClientOrder"", T3.""SlpName"" AS ""SellerCode"", T3.""Memo"" AS ""SellerName"", t0.""DocTotal"" AS ""Total"", (CASE t0.""DocStatus"" WHEN 'O' THEN 'Abierto' ELSE 'Cerrado' END) AS ""State"" 
                                      , t0.""Confirmed"" AS ""Authorized"", T5.""PymntGroup"" AS ""TermConditions"" 
                              FROM    {DBSA}.ORDR t0 
                                      LEFT OUTER JOIN {DBSA}.OWHS t1 ON t1.""WhsCode"" = t0.U_ALMACEN 
                                      INNER JOIN {DBSA}.OSLP T3 ON t0.""SlpCode"" = T3.""SlpCode"" 
                                      LEFT OUTER JOIN {DBSA}.OCTG T5 ON t0.""GroupNum"" = T5.""GroupNum"" 
                              WHERE   t0.CANCELED = 'N' 
                              UNION 
                              SELECT  t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", 'Miami' AS ""Subsidiary"", t1.""WhsName"" AS ""Warehouse"", CAST(t0.""DocDate"" AS date) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"" 
                                      , t0.""NumAtCard"" AS ""ClientOrder"", T3.""SlpName"" AS ""SellerCode"", T3.""Memo"" AS ""SellerName"", t0.""DocTotal"" AS ""Total"", (CASE t0.""DocStatus"" WHEN 'O' THEN 'Abierto' ELSE 'Cerrado' END) AS ""State"" 
                                      , t0.""Confirmed"" AS ""Authorized"", T5.""PymntGroup"" AS ""TermConditions"" 
                              FROM    {DBLA}.ORDR t0 
                                      LEFT OUTER JOIN {DBLA}.OWHS t1 ON t1.""WhsCode"" = t0.U_ALMACEN 
                                      INNER JOIN {DBLA}.OSLP T3 ON t0.""SlpCode"" = T3.""SlpCode"" 
                                      LEFT OUTER JOIN {DBLA}.OCTG T5 ON t0.""GroupNum"" = T5.""GroupNum"" 
                              WHERE   t0.CANCELED = 'N' 
                              UNION 
                              SELECT  t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", 'Iquique' AS ""Subsidiary"", t1.""WhsName"" AS ""Warehouse"", CAST(t0.""DocDate"" AS date) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"" 
                                      , t0.""NumAtCard"" AS ""ClientOrder"", T3.""SlpName"" AS ""SellerCode"", T3.""Memo"" AS ""SellerName"", t0.""DocTotal"" AS ""Total"", (CASE t0.""DocStatus"" WHEN 'O' THEN 'Abierto' ELSE 'Cerrado' END) AS ""State"" 
                                      , t0.""Confirmed"" AS ""Authorized"", T5.""PymntGroup"" AS ""TermConditions"" 
                              FROM    {DBIQ}.ORDR t0 
                                      LEFT OUTER JOIN {DBIQ}.OWHS t1 ON t1.""WhsCode"" = t0.U_ALMACEN 
                                      INNER JOIN {DBIQ}.OSLP T3 ON t0.""SlpCode"" = T3.""SlpCode"" 
                                      LEFT OUTER JOIN {DBIQ}.OCTG T5 ON t0.""GroupNum"" = T5.""GroupNum"" ";
            return query;
        }

        private string GetQuery2()
        {
            string query = $@"SELECT  t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", 'Santa Cruz' AS ""Subsidiary"", t1.""WhsName"" AS ""Warehouse"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"" 
                                     , t0.""NumAtCard"" AS ""ClientOrder"", t2.""SlpName"" AS ""SellerCode"", t2.""Memo"" AS ""SellerName"", t0.""DocTotal"" AS ""Total"", ( CASE t0.""DocStatus"" WHEN 'O' THEN 'Abierto' ELSE 'Cerrado' END ) AS ""State"" 
                                     , t0.""Confirmed"" AS ""Authorized"", t3.""PymntGroup"" AS ""TermConditions"", CAST(t0.""Header"" AS varchar(5000)) AS ""Header"", CAST(t0.""Footer"" AS varchar(5000)) AS ""Footer"" 
                                     , ( SELECT CAST(COUNT(*) AS INT) 
                                         FROM   {DBSA}.RDR1 r 
                                                INNER JOIN {DBSA}.OITW w ON r.""ItemCode"" = w.""ItemCode"" AND r.""WhsCode"" = w.""WhsCode"" 
                                                INNER JOIN {DBSA}.OITM o ON r.""ItemCode"" = o.""ItemCode""
                                         WHERE  r.""DocEntry"" = t0.""DocEntry"" AND r.""ItemCode"" <> 'FLETES' AND r.""ItemCode"" NOT LIKE '%DMC%' AND r.""ItemCode"" <> 'ENVIO' AND o.U_LINEA <> 'DMC' AND o.U_CATEGORIA <> 'SERVICIOS' AND r.""OpenQty"" > 0 AND w.""OnHand"" < r.""OpenQty"" ) AS ""NonCompleteItem"" 
                                     , ( SELECT SUM(""OpenQty"" * ""PriceAfVAT"") FROM {DBSA}.RDR1 WHERE ""DocEntry"" = t0.""DocEntry"" ) AS ""OpenAmount""
                             FROM    {DBSA}.ORDR t0 
                                     LEFT OUTER JOIN {DBSA}.OSLP t2 ON t0.""SlpCode"" = t2.""SlpCode"" 
                                     LEFT OUTER JOIN {DBSA}.OCTG t3 ON t0.""GroupNum"" = t3.""GroupNum"" 
                                     INNER JOIN (SELECT  s0.""DocEntry"", s1.""WhsName""
                                                 FROM    {DBSA}.RDR1 s0
                                                         INNER JOIN {DBSA}.OWHS s1 ON s1.""WhsCode"" = s0.""WhsCode""
                                                 WHERE   ""LineNum"" = ( SELECT MIN(""LineNum"") FROM {DBSA}.RDR1 WHERE ""DocEntry"" = s0.""DocEntry"" ) ) t1 ON t1.""DocEntry"" = t0.""DocEntry""
                             WHERE   t0.CANCELED = 'N' 
                             UNION 
                             SELECT  t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", 'Miami' AS ""Subsidiary"", t1.""WhsName"" AS ""Warehouse"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"" 
                                     , t0.""NumAtCard"" AS ""ClientOrder"", t2.""SlpName"" AS ""SellerCode"", t2.""Memo"" AS ""SellerName"", t0.""DocTotal"" AS ""Total"", ( CASE t0.""DocStatus"" WHEN 'O' THEN 'Abierto' ELSE 'Cerrado' END ) AS ""State"" 
                                     , t0.""Confirmed"" AS ""Authorized"", t3.""PymntGroup"" AS ""TermConditions"", CAST(t0.""Header"" AS varchar(5000)) AS ""Header"", CAST(t0.""Footer"" AS varchar(5000)) AS ""Footer"" 
                                     , ( SELECT CAST(COUNT(*) AS INT) 
                                         FROM   {DBLA}.RDR1 r 
                                                INNER JOIN {DBLA}.OITW w ON r.""ItemCode"" = w.""ItemCode"" AND   r.""WhsCode"" = w.""WhsCode"" 
                                                INNER JOIN {DBLA}.OITM o ON r.""ItemCode"" = o.""ItemCode""
                                         WHERE  r.""DocEntry"" = t0.""DocEntry"" AND r.""ItemCode"" <> 'FLETES' AND r.""ItemCode"" NOT LIKE '%DMC%' AND r.""ItemCode"" <> 'ENVIO' AND o.U_LINEA <> 'DMC' AND o.U_CATEGORIA <> 'SERVICIOS' AND r.""OpenQty"" > 0 AND w.""OnHand"" < r.""OpenQty"" ) AS ""NonCompleteItem"" 
                                     , ( SELECT SUM(""OpenQty"" * ""PriceAfVAT"") FROM {DBLA}.RDR1 WHERE ""DocEntry"" = t0.""DocEntry"" ) AS ""OpenAmount""
                             FROM    {DBLA}.ORDR t0 
                                     LEFT OUTER JOIN {DBLA}.OSLP t2 ON t0.""SlpCode"" = t2.""SlpCode"" 
                                     LEFT OUTER JOIN {DBLA}.OCTG t3 ON t0.""GroupNum"" = t3.""GroupNum"" 
                                     INNER JOIN (SELECT  s0.""DocEntry"", s1.""WhsName""
                                                 FROM    {DBLA}.RDR1 s0
                                                         INNER JOIN {DBLA}.OWHS s1 ON s1.""WhsCode"" = s0.""WhsCode""
                                                 WHERE   ""LineNum"" = ( SELECT MIN(""LineNum"") FROM {DBLA}.RDR1 WHERE ""DocEntry"" = s0.""DocEntry"" ) ) t1 ON t1.""DocEntry"" = t0.""DocEntry""
                             WHERE   t0.CANCELED = 'N' 
                             UNION 
                             SELECT  t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", 'Iquique' AS ""Subsidiary"", t1.""WhsName"" AS ""Warehouse"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"" 
                                     , t0.""NumAtCard"" AS ""ClientOrder"", t2.""SlpName"" AS ""SellerCode"", t2.""Memo"" AS ""SellerName"", t0.""DocTotal"" AS ""Total"", ( CASE t0.""DocStatus"" WHEN 'O' THEN 'Abierto' ELSE 'Cerrado' END ) AS ""State"" 
                                     , t0.""Confirmed"" AS ""Authorized"", t3.""PymntGroup"" AS ""TermConditions"", CAST(t0.""Header"" AS varchar(5000)) AS ""Header"", CAST(t0.""Footer"" AS varchar(5000)) AS ""Footer"" 
                                     , ( SELECT CAST(COUNT(*) AS INT) 
                                         FROM   {DBIQ}.RDR1 r 
                                                INNER JOIN {DBIQ}.OITW w ON r.""ItemCode"" = w.""ItemCode"" AND   r.""WhsCode"" = w.""WhsCode"" 
                                                INNER JOIN {DBIQ}.OITM o ON r.""ItemCode"" = o.""ItemCode""
                                         WHERE  r.""DocEntry"" = t0.""DocEntry"" AND r.""ItemCode"" <> 'FLETES' AND r.""ItemCode"" NOT LIKE '%DMC%' AND r.""ItemCode"" <> 'ENVIO' AND o.U_LINEA <> 'DMC' AND o.U_CATEGORIA <> 'SERVICIOS' AND r.""OpenQty"" > 0 AND w.""OnHand"" < r.""OpenQty"" ) AS ""NonCompleteItem"" 
                                     , ( SELECT SUM(""OpenQty"" * ""PriceAfVAT"") FROM {DBIQ}.RDR1 WHERE ""DocEntry"" = t0.""DocEntry"" ) AS ""OpenAmount""
                             FROM    {DBIQ}.ORDR t0 
                                     LEFT OUTER JOIN {DBIQ}.OSLP t2 ON t0.""SlpCode"" = t2.""SlpCode"" 
                                     LEFT OUTER JOIN {DBIQ}.OCTG t3 ON t0.""GroupNum"" = t3.""GroupNum"" 
                                     INNER JOIN (SELECT  s0.""DocEntry"", s1.""WhsName""
                                                 FROM    {DBIQ}.RDR1 s0
                                                         INNER JOIN {DBIQ}.OWHS s1 ON s1.""WhsCode"" = s0.""WhsCode""
                                                 WHERE   ""LineNum"" = ( SELECT MIN(""LineNum"") FROM {DBIQ}.RDR1 WHERE ""DocEntry"" = s0.""DocEntry"" ) ) t1 ON t1.""DocEntry"" = t0.""DocEntry""
                             WHERE   t0.CANCELED = 'N' ";
            return query;
        }

        private string GetQueryFiltered(string FilterItems)
        {
            string query = $@"SELECT  t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", 'Santa Cruz' AS ""Subsidiary"", t1.""WhsName"" AS ""Warehouse"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"" 
                                      , t0.""NumAtCard"" AS ""ClientOrder"", t0.""SlpCode"", t2.""SlpName"" AS ""SellerCode"", t2.""Memo"" AS ""SellerName"", t0.""DocTotal"" AS ""Total"", ( CASE t0.""DocStatus"" WHEN 'O' THEN 'Abierto' ELSE 'Cerrado' END ) AS ""State"" 
                                      , t0.""Confirmed"" AS ""Authorized"", t3.""PymntGroup"" AS ""TermConditions"", CAST(t0.""Header"" AS varchar(5000)) AS ""Header"", CAST(t0.""Footer"" AS varchar(5000)) AS ""Footer"" 
                                      , ( SELECT CAST(COUNT(*) AS INT) 
                                          FROM   {DBSA}.RDR1 r 
                                                 INNER JOIN {DBSA}.OITW w ON r.""ItemCode"" = w.""ItemCode"" AND r.""WhsCode"" = w.""WhsCode""
                                                 INNER JOIN {DBSA}.OITM o ON r.""ItemCode"" = o.""ItemCode""
                                          WHERE  r.""DocEntry"" = t0.""DocEntry"" AND r.""ItemCode"" <> 'FLETES' AND r.""ItemCode"" NOT LIKE '%DMC%' AND r.""ItemCode"" <> 'ENVIO' AND o.U_LINEA <> 'DMC' AND o.U_CATEGORIA <> 'SERVICIOS' AND r.""OpenQty"" > 0 AND w.""OnHand"" < r.""OpenQty"" ) AS ""NonCompleteItem"" 
                                      , ( SELECT CAST(COUNT(*) AS INT) 
                                          FROM   {DBSA}.RDR1 r 
                                                 INNER JOIN {DBSA}.OITM o ON r.""ItemCode"" = o.""ItemCode"" 
                                          WHERE  r.""DocEntry"" = t0.""DocEntry"" AND r.""ItemCode"" <> 'FLETES' AND r.""ItemCode"" NOT LIKE '%DMC%' AND r.""ItemCode"" <> 'ENVIO' AND o.U_LINEA <> 'DMC' AND o.U_CATEGORIA <> 'SERVICIOS' AND r.""OpenQty"" > 0 ) AS ""ItemsCount""
                                      , IFNULL(t0.U_CORRELATIVO, '') AS ""Correlative"", t4.""OpenAmount"", t4.""Margin"", t4.""TaxlessTotal"" 
                              FROM    {DBSA}.ORDR t0 
                                      LEFT OUTER JOIN (SELECT  s0.""DocEntry"", s1.""WhsName"" 
                                                       FROM    {DBSA}.RDR1 s0 
                                                               INNER JOIN {DBSA}.OWHS s1 ON s1.""WhsCode"" = s0.""WhsCode"" 
                                                       WHERE   ""LineNum"" = ( SELECT MIN(""LineNum"") FROM {DBSA}.RDR1 WHERE ""DocEntry"" = s0.""DocEntry"" ) ) t1 ON t1.""DocEntry"" = t0.""DocEntry"" 
                                      INNER JOIN {DBSA}.OSLP t2 ON t0.""SlpCode"" = t2.""SlpCode"" 
                                      LEFT OUTER JOIN {DBSA}.OCTG t3 ON t0.""GroupNum"" = t3.""GroupNum"" 
                                      INNER JOIN ( SELECT   s0.""DocEntry"", SUM(s0.""PriceAfVAT"" * s0.""OpenQty"") AS ""OpenAmount""
                                                            , SUM(( CASE WHEN s0.""WhsCode"" = 'ZFSCZ' THEN s0.""Quantity"" * ( s0.""PriceAfVAT"" - s0.""StockPrice"" ) ELSE s0.""Quantity"" * (( 0.84 * s0.""PriceAfVAT"" ) - s0.""StockPrice"" ) END )) AS ""Margin""
                                                            , SUM(( CASE WHEN s0.""WhsCode"" = 'ZFSCZ' THEN s0.""Quantity"" * s0.""PriceAfVAT"" ELSE s0.""Quantity"" * ( 0.84 * s0.""PriceAfVAT"" ) END )) AS ""TaxlessTotal""
                                                   FROM     {DBSA}.RDR1 s0
                                                   GROUP BY s0.""DocEntry"" ) t4 ON t1.""DocEntry"" = t4.""DocEntry""
                              WHERE   t0.CANCELED = 'N' 
                                      AND EXISTS ( SELECT * FROM {DBSA}.RDR1 s0 INNER JOIN {DBSA}.OITM s1 ON s0.""ItemCode"" = s1.""ItemCode"" WHERE s0.""DocEntry"" = t0.""DocEntry"" {FilterItems} ) 
                              UNION 
                              SELECT  t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", 'Miami' AS ""Subsidiary"", t1.""WhsName"" AS ""Warehouse"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"" 
                                      , t0.""NumAtCard"" AS ""ClientOrder"", t0.""SlpCode"", t2.""SlpName"" AS ""SellerCode"", t2.""Memo"" AS ""SellerName"", t0.""DocTotal"" AS ""Total"", ( CASE t0.""DocStatus"" WHEN 'O' THEN 'Abierto' ELSE 'Cerrado' END ) AS ""State"" 
                                      , t0.""Confirmed"" AS ""Authorized"", t3.""PymntGroup"" AS ""TermConditions"", CAST(t0.""Header"" AS varchar(5000)) AS ""Header"", CAST(t0.""Footer"" AS varchar(5000)) AS ""Footer"" 
                                      , ( SELECT CAST(COUNT(*) AS INT) 
                                          FROM   {DBLA}.RDR1 r 
                                                 INNER JOIN {DBLA}.OITW w ON r.""ItemCode"" = w.""ItemCode"" AND   r.""WhsCode"" = w.""WhsCode"" 
                                                 INNER JOIN {DBLA}.OITM o ON r.""ItemCode"" = o.""ItemCode""
                                          WHERE  r.""DocEntry"" = t0.""DocEntry"" AND r.""ItemCode"" <> 'FLETES' AND r.""ItemCode"" NOT LIKE '%DMC%' AND r.""ItemCode"" <> 'ENVIO' AND o.U_LINEA <> 'DMC' AND o.U_CATEGORIA <> 'SERVICIOS' AND r.""OpenQty"" > 0 AND w.""OnHand"" < r.""OpenQty"" ) AS ""NonCompleteItem"" 
                                      , ( SELECT CAST(COUNT(*) AS INT) 
                                          FROM   {DBLA}.RDR1 r 
                                                 INNER JOIN {DBLA}.OITM o ON r.""ItemCode"" = o.""ItemCode"" 
                                          WHERE  r.""DocEntry"" = t0.""DocEntry"" AND r.""ItemCode"" <> 'FLETES' AND r.""ItemCode"" NOT LIKE '%DMC%' AND r.""ItemCode"" <> 'ENVIO' AND o.U_LINEA <> 'DMC' AND o.U_CATEGORIA <> 'SERVICIOS' AND r.""OpenQty"" > 0 ) AS ""ItemsCount""
                                      , IFNULL(t0.U_CORRELATIVO, '') AS ""Correlative"", t4.""OpenAmount"", t4.""Margin"", t4.""TaxlessTotal"" 
                              FROM    {DBLA}.ORDR t0 
                                      LEFT OUTER JOIN (SELECT  s0.""DocEntry"", s1.""WhsName"" 
                                                       FROM    {DBLA}.RDR1 s0 
                                                               INNER JOIN {DBLA}.OWHS s1 ON s1.""WhsCode"" = s0.""WhsCode"" 
                                                       WHERE   ""LineNum"" = ( SELECT MIN(""LineNum"") FROM {DBLA}.RDR1 WHERE ""DocEntry"" = s0.""DocEntry"" ) ) t1 ON t1.""DocEntry"" = t0.""DocEntry"" 
                                      INNER JOIN {DBLA}.OSLP t2 ON t0.""SlpCode"" = t2.""SlpCode"" 
                                      LEFT OUTER JOIN {DBLA}.OCTG t3 ON t0.""GroupNum"" = t3.""GroupNum"" 
                                      INNER JOIN ( SELECT   s0.""DocEntry"", SUM(s0.""PriceAfVAT"" * s0.""OpenQty"") AS ""OpenAmount"", SUM(s0.""Quantity"" * ( s0.""PriceAfVAT"" - s0.""StockPrice"" )) AS ""Margin"", SUM(s0.""Quantity"" * s0.""PriceAfVAT"") AS ""TaxlessTotal""
                                                   FROM     {DBLA}.RDR1 s0
                                                   GROUP BY s0.""DocEntry"" ) t4 ON t1.""DocEntry"" = t4.""DocEntry""
                              WHERE   t0.CANCELED = 'N' 
                                      AND EXISTS ( SELECT * FROM {DBLA}.RDR1 s0 INNER JOIN {DBLA}.OITM s1 ON s0.""ItemCode"" = s1.""ItemCode"" WHERE s0.""DocEntry"" = t0.""DocEntry"" {FilterItems} ) 
                              UNION 
                              SELECT  t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", 'Iquique' AS ""Subsidiary"", t1.""WhsName"" AS ""Warehouse"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"" 
                                      , t0.""NumAtCard"" AS ""ClientOrder"", t0.""SlpCode"", t2.""SlpName"" AS ""SellerCode"", t2.""Memo"" AS ""SellerName"", t0.""DocTotal"" AS ""Total"", ( CASE t0.""DocStatus"" WHEN 'O' THEN 'Abierto' ELSE 'Cerrado' END ) AS ""State"" 
                                      , t0.""Confirmed"" AS ""Authorized"", t3.""PymntGroup"" AS ""TermConditions"", CAST(t0.""Header"" AS varchar(5000)) AS ""Header"", CAST(t0.""Footer"" AS varchar(5000)) AS ""Footer"" 
                                      , ( SELECT CAST(COUNT(*) AS INT) 
                                          FROM   {DBIQ}.RDR1 r 
                                                 INNER JOIN {DBIQ}.OITW w ON r.""ItemCode"" = w.""ItemCode"" AND   r.""WhsCode"" = w.""WhsCode"" 
                                                 INNER JOIN {DBIQ}.OITM o ON r.""ItemCode"" = o.""ItemCode""
                                          WHERE  r.""DocEntry"" = t0.""DocEntry"" AND r.""ItemCode"" <> 'FLETES' AND r.""ItemCode"" NOT LIKE '%DMC%' AND r.""ItemCode"" <> 'ENVIO' AND o.U_LINEA <> 'DMC' AND o.U_CATEGORIA <> 'SERVICIOS' AND r.""OpenQty"" > 0 AND w.""OnHand"" < r.""OpenQty"" ) AS ""NonCompleteItem"" 
                                      , ( SELECT CAST(COUNT(*) AS INT) 
                                          FROM   {DBIQ}.RDR1 r 
                                                 INNER JOIN {DBIQ}.OITM o ON r.""ItemCode"" = o.""ItemCode"" 
                                          WHERE  r.""DocEntry"" = t0.""DocEntry"" AND r.""ItemCode"" <> 'FLETES' AND r.""ItemCode"" NOT LIKE '%DMC%' AND r.""ItemCode"" <> 'ENVIO' AND o.U_LINEA <> 'DMC' AND o.U_CATEGORIA <> 'SERVICIOS' AND r.""OpenQty"" > 0 ) AS ""ItemsCount""
                                      , IFNULL(t0.U_CORRELATIVO, '') AS ""Correlative"", t4.""OpenAmount"", t4.""Margin"", t4.""TaxlessTotal"" 
                              FROM    {DBIQ}.ORDR t0 
                                      LEFT OUTER JOIN (SELECT  s0.""DocEntry"", s1.""WhsName"" 
                                                       FROM    {DBIQ}.RDR1 s0 
                                                               INNER JOIN {DBIQ}.OWHS s1 ON s1.""WhsCode"" = s0.""WhsCode"" 
                                                       WHERE   ""LineNum"" = ( SELECT MIN(""LineNum"") FROM {DBIQ}.RDR1 WHERE ""DocEntry"" = s0.""DocEntry"" ) ) t1 ON t1.""DocEntry"" = t0.""DocEntry"" 
                                      INNER JOIN {DBIQ}.OSLP t2 ON t0.""SlpCode"" = t2.""SlpCode"" 
                                      LEFT OUTER JOIN {DBIQ}.OCTG t3 ON t0.""GroupNum"" = t3.""GroupNum"" 
                                      INNER JOIN ( SELECT   s0.""DocEntry"", SUM(s0.""PriceAfVAT"" * s0.""OpenQty"") AS ""OpenAmount"", SUM(s0.""Quantity"" * ( s0.""PriceAfVAT"" - s0.""StockPrice"" )) AS ""Margin"", SUM(s0.""Quantity"" * s0.""PriceAfVAT"") AS ""TaxlessTotal""
                                                   FROM     {DBIQ}.RDR1 s0
                                                   GROUP BY s0.""DocEntry"" ) t4 ON t1.""DocEntry"" = t4.""DocEntry""
                              WHERE   t0.CANCELED = 'N' 
                                      AND EXISTS ( SELECT * FROM {DBIQ}.RDR1 s0 INNER JOIN {DBIQ}.OITM s1 ON s0.""ItemCode"" = s1.""ItemCode"" WHERE s0.""DocEntry"" = t0.""DocEntry"" {FilterItems} ) ";
            return query;
        }

        private string GetQuery3()
        {
            string query = $@"SELECT  t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", 'Santa Cruz' AS ""Subsidiary"", t1.""WhsName"" AS ""Warehouse"", CAST(t0.""DocDate"" AS date) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"" 
                                      , t0.""NumAtCard"" AS ""ClientOrder"", T3.""SlpName"" AS ""SellerCode"", T3.""Memo"" AS ""SellerName"", t0.""DocTotal"" AS ""Total"", (CASE t0.""DocStatus"" WHEN 'O' THEN 'Abierto' ELSE 'Cerrado' END) AS ""State"" 
                                      , t0.""Confirmed"" AS ""Authorized"", T5.""PymntGroup"" AS ""TermConditions"" 
                                      , t0.""Comments"", t0.""Address"" AS ""BillingAddress"", t0.""Address2"" AS ""DestinationAddress"", t0.""ShipToCode"" AS ""DestinationCode"", t0.U_INCOTERMS AS ""Incoterms"", t0.U_CORRELATIVO AS ""Correlative"", t4.""TrnspName"" AS ""Transport"" 
                                      , t0.U_LATITUD_DIREC_CFINAL AS ""Latitude"", t0.U_LONGITUD_DIREC_CFINAL AS ""Longitude"", t0.U_RAZSOC AS ""BusinessName"", t0.U_NIT AS ""NIT"", t0.U_NOMBRE_CFINAL AS ""FCName"", t0.U_MAIL_CFINAL AS ""FCMail"", t0.U_TELEF_CFINAL AS ""FCPhone"" 
                                      , t0.U_CIUDAD_CFINAL AS ""FCCity"", t0.U_DIRECCION_CFINAL AS ""FCAddress""
                              FROM    {DBSA}.ORDR t0 
                                      LEFT OUTER JOIN ( SELECT  s0.""DocEntry"", s0.""WhsCode"" 
                                                        FROM    {DBSA}.RDR1 s0 
                                                        WHERE   ""LineNum"" = ( SELECT MIN(""LineNum"") FROM {DBSA}.RDR1 WHERE ""DocEntry"" = s0.""DocEntry"" ) ) t2 ON t2.""DocEntry"" = t0.""DocEntry"" 
                                      LEFT OUTER JOIN {DBSA}.OWHS t1 ON t1.""WhsCode"" = t2.""WhsCode"" 
                                      INNER JOIN {DBSA}.OSLP T3 ON t0.""SlpCode"" = T3.""SlpCode"" 
                                      LEFT OUTER JOIN {DBSA}.OCTG T5 ON t0.""GroupNum"" = T5.""GroupNum"" 
                                      LEFT OUTER JOIN {DBSA}.OSHP t4 ON t0.""TrnspCode"" = t4.""TrnspCode"" 
                              WHERE   t0.CANCELED = 'N' 
                              UNION 
                              SELECT  t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", 'Miami' AS ""Subsidiary"", t1.""WhsName"" AS ""Warehouse"", CAST(t0.""DocDate"" AS date) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"" 
                                      , t0.""NumAtCard"" AS ""ClientOrder"", T3.""SlpName"" AS ""SellerCode"", T3.""Memo"" AS ""SellerName"", t0.""DocTotal"" AS ""Total"", (CASE t0.""DocStatus"" WHEN 'O' THEN 'Abierto' ELSE 'Cerrado' END) AS ""State"" 
                                      , t0.""Confirmed"" AS ""Authorized"", T5.""PymntGroup"" AS ""TermConditions"" 
                                      , t0.""Comments"", t0.""Address"" AS ""BillingAddress"", t0.""Address2"" AS ""DestinationAddress"", t0.""ShipToCode"" AS ""DestinationCode"", t0.U_INCOTERMS AS ""Incoterms"", t0.U_CORRELATIVO AS ""Correlative"", t4.""TrnspName"" AS ""Transport"" 
                                      , '' AS ""Latitude"", '' AS ""Longitude"", '' AS ""BusinessName"", '' AS ""NIT"", '' AS ""FCName"", '' AS ""FCMail"", '' AS ""FCPhone"", '' AS ""FCCity"", '' AS ""FCAddress""
                              FROM    {DBLA}.ORDR t0 
                                      LEFT OUTER JOIN ( SELECT  s0.""DocEntry"", s0.""WhsCode"" 
                                                        FROM    {DBLA}.RDR1 s0 
                                                        WHERE   ""LineNum"" = ( SELECT MIN(""LineNum"") FROM {DBLA}.RDR1 WHERE ""DocEntry"" = s0.""DocEntry"" ) ) t2 ON t2.""DocEntry"" = t0.""DocEntry"" 
                                      LEFT OUTER JOIN {DBLA}.OWHS t1 ON t1.""WhsCode"" = t2.""WhsCode"" 
                                      INNER JOIN {DBLA}.OSLP T3 ON t0.""SlpCode"" = T3.""SlpCode"" 
                                      LEFT OUTER JOIN {DBLA}.OCTG T5 ON t0.""GroupNum"" = T5.""GroupNum"" 
                                      LEFT OUTER JOIN {DBLA}.OSHP t4 ON t0.""TrnspCode"" = t4.""TrnspCode"" 
                              WHERE   t0.CANCELED = 'N' 
                              UNION 
                              SELECT  t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", 'Iquique' AS ""Subsidiary"", t1.""WhsName"" AS ""Warehouse"", CAST(t0.""DocDate"" AS date) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"" 
                                      , t0.""NumAtCard"" AS ""ClientOrder"", T3.""SlpName"" AS ""SellerCode"", T3.""Memo"" AS ""SellerName"", t0.""DocTotal"" AS ""Total"", (CASE t0.""DocStatus"" WHEN 'O' THEN 'Abierto' ELSE 'Cerrado' END) AS ""State"" 
                                      , t0.""Confirmed"" AS ""Authorized"", T5.""PymntGroup"" AS ""TermConditions"" 
                                      , t0.""Comments"", t0.""Address"" AS ""BillingAddress"", t0.""Address2"" AS ""DestinationAddress"", t0.""ShipToCode"" AS ""DestinationCode"", t0.U_INCOTERMS AS ""Incoterms"", t0.U_CORRELATIVO AS ""Correlative"", t4.""TrnspName"" AS ""Transport"" 
                                      , '' AS ""Latitude"", '' AS ""Longitude"", '' AS ""BusinessName"", '' AS ""NIT"", '' AS ""FCName"", '' AS ""FCMail"", '' AS ""FCPhone"", '' AS ""FCCity"", '' AS ""FCAddress"" 
                              FROM    {DBIQ}.ORDR t0 
                                      LEFT OUTER JOIN ( SELECT  s0.""DocEntry"", s0.""WhsCode"" 
                                                        FROM    {DBIQ}.RDR1 s0 
                                                        WHERE   ""LineNum"" = ( SELECT MIN(""LineNum"") FROM {DBIQ}.RDR1 WHERE ""DocEntry"" = s0.""DocEntry"" ) ) t2 ON t2.""DocEntry"" = t0.""DocEntry"" 
                                      LEFT OUTER JOIN {DBIQ}.OWHS t1 ON t1.""WhsCode"" = t2.""WhsCode"" 
                                      INNER JOIN {DBIQ}.OSLP T3 ON t0.""SlpCode"" = T3.""SlpCode"" 
                                      LEFT OUTER JOIN {DBIQ}.OCTG T5 ON t0.""GroupNum"" = T5.""GroupNum"" 
                                      LEFT OUTER JOIN {DBIQ}.OSHP t4 ON t0.""TrnspCode"" = t4.""TrnspCode"" 
                              WHERE   t0.CANCELED = 'N' ";
            return query;
        }

        private string GetQuery4()
        {
            string query;
            query = $@"SELECT  t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", 'Santa Cruz' AS ""Subsidiary"", t1.""WhsName"" AS ""Warehouse"", CAST(t0.""DocDate"" AS date) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"" 
                               , t0.""NumAtCard"" AS ""ClientOrder"", T3.""SlpName"" AS ""SellerCode"", T3.""Memo"" AS ""SellerName"", t0.""DocTotal"" AS ""Total"", (CASE t0.""DocStatus"" WHEN 'O' THEN 'Abierto' ELSE 'Cerrado' END) AS ""State"" 
                               , t0.""Confirmed"" AS ""Authorized"", T5.""PymntGroup"" AS ""TermConditions"", t2.""Margin"", t2.""TaxlessTotal"", t2.""OpenAmount""
                       FROM    {DBSA}.ORDR t0 
                               INNER JOIN {DBSA}.OSLP T3 ON t0.""SlpCode"" = T3.""SlpCode"" 
                               LEFT OUTER JOIN {DBSA}.OCTG T5 ON t0.""GroupNum"" = T5.""GroupNum"" 
                               INNER JOIN ( SELECT s0.""DocEntry"", SUM( CASE s0.""WhsCode"" WHEN 'ZFSCZ' THEN s0.""Quantity"" * ( s0.""PriceAfVAT"" - s0.""StockPrice"" ) ELSE s0.""Quantity"" * ( ( 0.84 * s0.""PriceAfVAT"" ) - s0.""StockPrice"" ) END ) AS ""Margin""
                                                   , SUM( CASE s0.""WhsCode"" WHEN 'ZFSCZ' THEN s0.""Quantity"" * s0.""PriceAfVAT"" ELSE s0.""Quantity"" * 0.84 * s0.""PriceAfVAT"" END ) AS ""TaxlessTotal"", SUM( s0.""OpenQty"" * s0.""PriceAfVAT"" ) AS ""OpenAmount""
                                                   , FIRST_VALUE(s0.""WhsCode"" ORDER BY ""LineNum"") AS ""WhsCode""
                                            FROM   {DBSA}.RDR1 s0 
                                            GROUP BY s0.""DocEntry"" ) t2 ON t0.""DocEntry"" = t2.""DocEntry""
                               LEFT OUTER JOIN {DBSA}.OWHS t1 ON t1.""WhsCode"" = t2.""WhsCode"" 
                       WHERE   t0.CANCELED = 'N' 
                       UNION
                       SELECT  t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", 'Miami' AS ""Subsidiary"", t1.""WhsName"" AS ""Warehouse"", CAST(t0.""DocDate"" AS date) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"" 
                               , t0.""NumAtCard"" AS ""ClientOrder"", T3.""SlpName"" AS ""SellerCode"", T3.""Memo"" AS ""SellerName"", t0.""DocTotal"" AS ""Total"", (CASE t0.""DocStatus"" WHEN 'O' THEN 'Abierto' ELSE 'Cerrado' END) AS ""State"" 
                               , t0.""Confirmed"" AS ""Authorized"", T5.""PymntGroup"" AS ""TermConditions"", t2.""Margin"", t2.""TaxlessTotal"", t2.""OpenAmount""
                       FROM    {DBLA}.ORDR t0 
                               INNER JOIN {DBLA}.OSLP T3 ON t0.""SlpCode"" = T3.""SlpCode"" 
                               LEFT OUTER JOIN {DBLA}.OCTG T5 ON t0.""GroupNum"" = T5.""GroupNum"" 
                               INNER JOIN ( SELECT s0.""DocEntry"", SUM( s0.""Quantity"" * ( s0.""PriceAfVAT"" - s0.""StockPrice"" ) ) AS ""Margin"", SUM( s0.""Quantity"" * s0.""PriceAfVAT"" ) AS ""TaxlessTotal"", SUM( s0.""OpenQty"" * s0.""PriceAfVAT"" ) AS ""OpenAmount""
                                                   , FIRST_VALUE(s0.""WhsCode"" ORDER BY ""LineNum"") AS ""WhsCode""
                                            FROM   {DBLA}.RDR1 s0 
                                            GROUP BY s0.""DocEntry"" ) t2 ON t0.""DocEntry"" = t2.""DocEntry""
                               LEFT OUTER JOIN {DBLA}.OWHS t1 ON t1.""WhsCode"" = t2.""WhsCode"" 
                       WHERE   t0.CANCELED = 'N'
                       UNION
                       SELECT  t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", 'Iquique' AS ""Subsidiary"", t1.""WhsName"" AS ""Warehouse"", CAST(t0.""DocDate"" AS date) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"" 
                               , t0.""NumAtCard"" AS ""ClientOrder"", T3.""SlpName"" AS ""SellerCode"", T3.""Memo"" AS ""SellerName"", t0.""DocTotal"" AS ""Total"", (CASE t0.""DocStatus"" WHEN 'O' THEN 'Abierto' ELSE 'Cerrado' END) AS ""State"" 
                               , t0.""Confirmed"" AS ""Authorized"", T5.""PymntGroup"" AS ""TermConditions"", t2.""Margin"", t2.""TaxlessTotal"", t2.""OpenAmount""
                       FROM    {DBIQ}.ORDR t0 
                               INNER JOIN {DBIQ}.OSLP T3 ON t0.""SlpCode"" = T3.""SlpCode"" 
                               LEFT OUTER JOIN {DBIQ}.OCTG T5 ON t0.""GroupNum"" = T5.""GroupNum"" 
                               INNER JOIN ( SELECT s0.""DocEntry"", SUM( s0.""Quantity"" * ( s0.""PriceAfVAT"" - s0.""StockPrice"" ) ) AS ""Margin"", SUM( s0.""Quantity"" * s0.""PriceAfVAT"" ) AS ""TaxlessTotal"", SUM( s0.""OpenQty"" * s0.""PriceAfVAT"" ) AS ""OpenAmount""
                                                   , FIRST_VALUE(s0.""WhsCode"" ORDER BY ""LineNum"") AS ""WhsCode""
                                            FROM   {DBIQ}.RDR1 s0 
                                            GROUP BY s0.""DocEntry"" ) t2 ON t0.""DocEntry"" = t2.""DocEntry""
                               LEFT OUTER JOIN {DBIQ}.OWHS t1 ON t1.""WhsCode"" = t2.""WhsCode"" 
                       WHERE   t0.CANCELED = 'N' ";
            //string query = $@"SELECT    DISTINCT 'Santa Cruz' AS ""Subsidiary"", a2.""WhsName"" AS ""Warehouse"", a1.""ClientCode"", a1.""ClientName"", a3.""SlpName"" AS ""SellerCode"", a3.""Memo"" AS ""SellerName"", a1.""Id"" AS ""OrderId"", a1.""DocNumber"" AS ""OrderNumber""
            //                            , a1.""DocDate"" AS ""OrderDate"", a1.""ClientOrder"", a1.""Total"" AS ""OrderTotal"", a1.""OpenAmount"", a4.""PymntGroup"" AS ""TermConditions"", ( CASE a1.""DocStatus"" WHEN 'O' THEN 'Abierto' ELSE 'Cerrado' END ) AS ""OrderStatus""
            //                            , a1.""Confirmed"" AS ""OrderAuthorized"", a1.""Header"", a1.""Footer"", a5.""DocEntry"" AS ""NoteId"", a5.""DocNum"" AS ""NoteNumber"", a5.""NumAtCard"" AS ""BillNumber"", a5.U_NROAUTOR AS ""AuthorizationNumber""
            //                            , a5.""DocDate"" AS ""NoteDate"", a5.""Total"" AS ""NoteTotal"", a5.""Margin"" AS ""NoteMargin"", a5.""TaxlessTotal"" AS ""NoteTaxlessTotal"", 'N' AS ""IsDeliveryNote""
            //                            , ( SELECT CAST(COUNT(*) AS INT) 
            //                                FROM   {DBSA}.RDR1 r 
            //                                       INNER JOIN {DBSA}.OITW w ON r.""ItemCode"" = w.""ItemCode"" AND r.""WhsCode"" = w.""WhsCode"" 
            //                                       INNER JOIN {DBSA}.OITM o ON r.""ItemCode"" = o.""ItemCode"" 
            //                                WHERE  r.""DocEntry"" = a1.""Id"" AND r.""ItemCode"" <> 'FLETES' AND r.""ItemCode"" <> 'ENVIO' AND o.U_LINEA <> 'DMC' AND o.U_CATEGORIA <> 'SERVICIOS' AND w.""OnHand"" < r.""OpenQty"" ) AS ""NonCompleteItem""
            //                            , ( SELECT CAST(COUNT(*) AS INT) 
            //                                FROM   {DBSA}.RDR1 r 
            //                                       INNER JOIN {DBSA}.OITM o ON r.""ItemCode"" = o.""ItemCode"" 
            //                                WHERE  r.""DocEntry"" = a1.""Id"" AND r.""ItemCode"" <> 'FLETES' AND r.""ItemCode"" <> 'ENVIO' AND o.U_LINEA <> 'DMC' AND o.U_CATEGORIA <> 'SERVICIOS' ) AS ""ItemsCount""
            //                  FROM      ( SELECT    t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", FIRST_VALUE(t1.""WhsCode"" ORDER BY ""LineNum"") AS ""WhsCode"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode""
            //                                        , t0.""CardName"" AS ""ClientName"", t0.""NumAtCard"" AS ""ClientOrder"", t0.""SlpCode"", SUM(t1.""Quantity"" * t1.""PriceAfVAT"") AS ""Total"", SUM(t1.""OpenQty"" * t1.""PriceAfVAT"") AS ""OpenAmount""
            //                                        , t0.""GroupNum"", t0.""DocStatus"", t0.""Confirmed"", CAST(t0.""Header"" AS VARCHAR(5000)) AS ""Header"", CAST(t0.""Footer"" AS VARCHAR(5000)) AS ""Footer""
            //                              FROM      {DBSA}.ORDR t0 
            //                                        INNER JOIN {DBSA}.RDR1 t1 ON t1.""DocEntry"" = t0.""DocEntry"" 
            //                                        INNER JOIN {DBSA}.OITM t2 ON t2.""ItemCode"" = t1.""ItemCode"" 
            //                              WHERE     t0.CANCELED = 'N' AND t1.""TargetType"" <> 14 
            //                              GROUP BY  t0.""DocEntry"", t0.""DocNum"", CAST(t0.""DocDate"" AS DATE), t0.""CardCode"", t0.""CardName"", t0.""NumAtCard"", t0.""SlpCode"", t0.""GroupNum"", t0.""DocStatus"", t0.""Confirmed"", CAST(t0.""Header"" AS VARCHAR(5000))
            //                                        , CAST(t0.""Footer"" AS VARCHAR(5000)) ) a1
            //                            INNER JOIN {DBSA}.RDR1 a0 ON a1.""Id"" = a0.""DocEntry""
            //                            INNER JOIN {DBSA}.OITM a6 ON a0.""ItemCode"" = a6.""ItemCode""
            //                            LEFT OUTER JOIN {DBSA}.OWHS a2 ON a2.""WhsCode"" = a1.""WhsCode"" 
            //                            LEFT OUTER JOIN {DBSA}.OSLP a3 ON a3.""SlpCode"" = a1.""SlpCode"" 
            //                            LEFT OUTER JOIN {DBSA}.OCTG a4 ON a4.""GroupNum"" = a1.""GroupNum"" 
            //                            LEFT OUTER JOIN ( SELECT    t0.""DocEntry"", t0.""DocNum"", t1.""BaseEntry"", t0.""NumAtCard"", t0.U_NROAUTOR, CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", SUM(t1.""Quantity"" * t1.""PriceAfVAT"") AS ""Total"" 
            //                                                        , SUM(( CASE WHEN t1.""WhsCode"" = 'ZFSCZ' THEN t1.""Quantity"" * ( t1.""PriceAfVAT"" - t1.""StockPrice"" ) ELSE t1.""Quantity"" * (( 0.84 * t1.""PriceAfVAT"" ) - t1.""StockPrice"" ) END )) AS ""Margin"" 
            //                                                        , SUM(( CASE WHEN t1.""WhsCode"" = 'ZFSCZ' THEN t1.""Quantity"" * t1.""PriceAfVAT"" ELSE t1.""Quantity"" * ( 0.84 * t1.""PriceAfVAT"" ) END )) AS ""TaxlessTotal""
            //                                              FROM      {DBSA}.OINV t0
            //                                                        INNER JOIN {DBSA}.INV1 t1 ON t0.""DocEntry"" = t1.""DocEntry""
            //                                              WHERE     t0.CANCELED = 'N' AND  t1.""TargetType"" <> 14
            //                                              GROUP BY t0.""DocEntry"", t0.""DocNum"", t1.""BaseEntry"", t0.""NumAtCard"", t0.U_NROAUTOR, CAST(t0.""DocDate"" AS DATE) ) a5 ON a1.""Id"" = a5.""BaseEntry""
            //                  WHERE   1 = 1 {InnerFilter}
            //                  UNION
            //                  SELECT    DISTINCT 'Iquique' AS ""Subsidiary"", a2.""WhsName"" AS ""Warehouse"", a1.""ClientCode"", a1.""ClientName"", a3.""SlpName"" AS ""SellerCode"", a3.""Memo"" AS ""SellerName"", a1.""Id"" AS ""OrderId"", a1.""DocNumber"" AS ""OrderNumber""
            //                            , a1.""DocDate"" AS ""OrderDate"", a1.""ClientOrder"", a1.""Total"" AS ""OrderTotal"", a1.""OpenAmount"", a4.""PymntGroup"" AS ""TermConditions"", ( CASE a1.""DocStatus"" WHEN 'O' THEN 'Abierto' ELSE 'Cerrado' END ) AS ""OrderStatus""
            //                            , a1.""Confirmed"" AS ""OrderAuthorized"", a1.""Header"", a1.""Footer"", a5.""DocEntry"" AS ""NoteId"", a5.""DocNum"" AS ""NoteNumber"", a5.""NumAtCard"" AS ""BillNumber"", a5.U_NROAUTOR AS ""AuthorizationNumber""
            //                            , a5.""DocDate"" AS ""NoteDate"", a5.""Total"" AS ""NoteTotal"", a5.""Margin"" AS ""NoteMargin"", a5.""TaxlessTotal"" AS ""NoteTaxlessTotal"", 'N' AS ""IsDeliveryNote""
            //                            , ( SELECT CAST(COUNT(*) AS INT) 
            //                                FROM   {DBIQ}.RDR1 r 
            //                                       INNER JOIN {DBIQ}.OITW w ON r.""ItemCode"" = w.""ItemCode"" AND r.""WhsCode"" = w.""WhsCode"" 
            //                                       INNER JOIN {DBIQ}.OITM o ON r.""ItemCode"" = o.""ItemCode"" 
            //                                WHERE  r.""DocEntry"" = a1.""Id"" AND r.""ItemCode"" <> 'FLETES' AND r.""ItemCode"" <> 'ENVIO' AND o.U_LINEA <> 'DMC' AND o.U_CATEGORIA <> 'SERVICIOS' AND w.""OnHand"" < r.""OpenQty"" ) AS ""NonCompleteItem""
            //                            , ( SELECT CAST(COUNT(*) AS INT) 
            //                                FROM   {DBIQ}.RDR1 r 
            //                                       INNER JOIN {DBIQ}.OITM o ON r.""ItemCode"" = o.""ItemCode"" 
            //                                WHERE  r.""DocEntry"" = a1.""Id"" AND r.""ItemCode"" <> 'FLETES' AND r.""ItemCode"" <> 'ENVIO' AND o.U_LINEA <> 'DMC' AND o.U_CATEGORIA <> 'SERVICIOS' ) AS ""ItemsCount""
            //                  FROM      ( SELECT    t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", FIRST_VALUE(t1.""WhsCode"" ORDER BY ""LineNum"") AS ""WhsCode"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode""
            //                                        , t0.""CardName"" AS ""ClientName"", t0.""NumAtCard"" AS ""ClientOrder"", t0.""SlpCode"", SUM(t1.""Quantity"" * t1.""PriceAfVAT"") AS ""Total"", SUM(t1.""OpenQty"" * t1.""PriceAfVAT"") AS ""OpenAmount""
            //                                        , t0.""GroupNum"", t0.""DocStatus"", t0.""Confirmed"", CAST(t0.""Header"" AS VARCHAR(5000)) AS ""Header"", CAST(t0.""Footer"" AS VARCHAR(5000)) AS ""Footer""
            //                              FROM      {DBIQ}.ORDR t0 
            //                                        INNER JOIN {DBIQ}.RDR1 t1 ON t1.""DocEntry"" = t0.""DocEntry"" 
            //                                        INNER JOIN {DBIQ}.OITM t2 ON t2.""ItemCode"" = t1.""ItemCode"" 
            //                              WHERE     t0.CANCELED = 'N' AND t1.""TargetType"" <> 14 
            //                              GROUP BY  t0.""DocEntry"", t0.""DocNum"", CAST(t0.""DocDate"" AS DATE), t0.""CardCode"", t0.""CardName"", t0.""NumAtCard"", t0.""SlpCode"", t0.""GroupNum"", t0.""DocStatus"", t0.""Confirmed"", CAST(t0.""Header"" AS VARCHAR(5000))
            //                                        , CAST(t0.""Footer"" AS VARCHAR(5000)) ) a1
            //                            INNER JOIN {DBIQ}.RDR1 a0 ON a1.""Id"" = a0.""DocEntry""
            //                            INNER JOIN {DBIQ}.OITM a6 ON a0.""ItemCode"" = a6.""ItemCode""
            //                            LEFT OUTER JOIN {DBIQ}.OWHS a2 ON a2.""WhsCode"" = a1.""WhsCode"" 
            //                            LEFT OUTER JOIN {DBIQ}.OSLP a3 ON a3.""SlpCode"" = a1.""SlpCode"" 
            //                            LEFT OUTER JOIN {DBIQ}.OCTG a4 ON a4.""GroupNum"" = a1.""GroupNum"" 
            //                            LEFT OUTER JOIN ( SELECT    t0.""DocEntry"", t0.""DocNum"", t1.""BaseEntry"", t0.""NumAtCard"", t0.U_NROAUTOR, CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", SUM(t1.""Quantity"" * t1.""PriceAfVAT"") AS ""Total"" 
            //                                                        , SUM(t1.""Quantity"" * ( t1.""PriceAfVAT"" - t1.""StockPrice"" )) AS ""Margin"", SUM(t1.""Quantity"" * t1.""PriceAfVAT"") AS ""TaxlessTotal""
            //                                              FROM      {DBIQ}.OINV t0
            //                                                        INNER JOIN {DBIQ}.INV1 t1 ON t0.""DocEntry"" = t1.""DocEntry""
            //                                              WHERE     t0.CANCELED = 'N' AND  t1.""TargetType"" <> 14
            //                                              GROUP BY t0.""DocEntry"", t0.""DocNum"", t1.""BaseEntry"", t0.""NumAtCard"", t0.U_NROAUTOR, CAST(t0.""DocDate"" AS DATE) ) a5 ON a1.""Id"" = a5.""BaseEntry""
            //                  WHERE   1 = 1 {InnerFilter}
            //                  UNION
            //                  SELECT    DISTINCT 'Miami' AS ""Subsidiary"", a2.""WhsName"" AS ""Warehouse"", a1.""ClientCode"", a1.""ClientName"", a3.""SlpName"" AS ""SellerCode"", a3.""Memo"" AS ""SellerName"", a1.""Id"" AS ""OrderId"", a1.""DocNumber"" AS ""OrderNumber""
            //                            , a1.""DocDate"" AS ""OrderDate"", a1.""ClientOrder"", a1.""Total"" AS ""OrderTotal"", a1.""OpenAmount"", a4.""PymntGroup"" AS ""TermConditions"", ( CASE a1.""DocStatus"" WHEN 'O' THEN 'Abierto' ELSE 'Cerrado' END ) AS ""OrderStatus""
            //                            , a1.""Confirmed"" AS ""OrderAuthorized"", a1.""Header"", a1.""Footer"", a5.""DocEntry"" AS ""NoteId"", a5.""DocNum"" AS ""NoteNumber"", a5.""NumAtCard"" AS ""BillNumber"", a5.U_NROAUTOR AS ""AuthorizationNumber""
            //                            , a5.""DocDate"" AS ""NoteDate"", a5.""Total"" AS ""NoteTotal"", a5.""Margin"" AS ""NoteMargin"", a5.""TaxlessTotal"" AS ""NoteTaxlessTotal"", 'N' AS ""IsDeliveryNote""
            //                            , ( SELECT CAST(COUNT(*) AS INT) 
            //                                FROM   {DBLA}.RDR1 r 
            //                                       INNER JOIN {DBLA}.OITW w ON r.""ItemCode"" = w.""ItemCode"" AND r.""WhsCode"" = w.""WhsCode"" 
            //                                       INNER JOIN {DBLA}.OITM o ON r.""ItemCode"" = o.""ItemCode"" 
            //                                WHERE  r.""DocEntry"" = a1.""Id"" AND r.""ItemCode"" <> 'FLETES' AND r.""ItemCode"" <> 'ENVIO' AND o.U_LINEA <> 'DMC' AND o.U_CATEGORIA <> 'SERVICIOS' AND w.""OnHand"" < r.""OpenQty"" ) AS ""NonCompleteItem""
            //                            , ( SELECT CAST(COUNT(*) AS INT) 
            //                                FROM   {DBLA}.RDR1 r 
            //                                       INNER JOIN {DBLA}.OITM o ON r.""ItemCode"" = o.""ItemCode"" 
            //                                WHERE  r.""DocEntry"" = a1.""Id"" AND r.""ItemCode"" <> 'FLETES' AND r.""ItemCode"" <> 'ENVIO' AND o.U_LINEA <> 'DMC' AND o.U_CATEGORIA <> 'SERVICIOS' ) AS ""ItemsCount""
            //                  FROM      ( SELECT    t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", FIRST_VALUE(t1.""WhsCode"" ORDER BY ""LineNum"") AS ""WhsCode"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode""
            //                                        , t0.""CardName"" AS ""ClientName"", t0.""NumAtCard"" AS ""ClientOrder"", t0.""SlpCode"", SUM(t1.""Quantity"" * t1.""PriceAfVAT"") AS ""Total"", SUM(t1.""OpenQty"" * t1.""PriceAfVAT"") AS ""OpenAmount""
            //                                        , t0.""GroupNum"", t0.""DocStatus"", t0.""Confirmed"", CAST(t0.""Header"" AS VARCHAR(5000)) AS ""Header"", CAST(t0.""Footer"" AS VARCHAR(5000)) AS ""Footer""
            //                              FROM      {DBLA}.ORDR t0 
            //                                        INNER JOIN {DBLA}.RDR1 t1 ON t1.""DocEntry"" = t0.""DocEntry"" 
            //                                        INNER JOIN {DBLA}.OITM t2 ON t2.""ItemCode"" = t1.""ItemCode"" 
            //                              WHERE     t0.CANCELED = 'N' AND t1.""TargetType"" <> 14  
            //                              GROUP BY  t0.""DocEntry"", t0.""DocNum"", CAST(t0.""DocDate"" AS DATE), t0.""CardCode"", t0.""CardName"", t0.""NumAtCard"", t0.""SlpCode"", t0.""GroupNum"", t0.""DocStatus"", t0.""Confirmed"", CAST(t0.""Header"" AS VARCHAR(5000))
            //                                        , CAST(t0.""Footer"" AS VARCHAR(5000)) ) a1
            //                            INNER JOIN {DBLA}.RDR1 a0 ON a1.""Id"" = a0.""DocEntry""
            //                            INNER JOIN {DBLA}.OITM a6 ON a0.""ItemCode"" = a6.""ItemCode""
            //                            LEFT OUTER JOIN {DBLA}.OWHS a2 ON a2.""WhsCode"" = a1.""WhsCode"" 
            //                            LEFT OUTER JOIN {DBLA}.OSLP a3 ON a3.""SlpCode"" = a1.""SlpCode"" 
            //                            LEFT OUTER JOIN {DBLA}.OCTG a4 ON a4.""GroupNum"" = a1.""GroupNum"" 
            //                            LEFT OUTER JOIN ( SELECT    t0.""DocEntry"", t0.""DocNum"", t1.""BaseEntry"", t0.""NumAtCard"", t0.U_NROAUTOR, CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", SUM(t1.""Quantity"" * t1.""PriceAfVAT"") AS ""Total"" 
            //                                                        , SUM(t1.""Quantity"" * ( t1.""PriceAfVAT"" - t1.""StockPrice"" )) AS ""Margin"", SUM(t1.""Quantity"" * t1.""PriceAfVAT"") AS ""TaxlessTotal""
            //                                              FROM      {DBLA}.OINV t0
            //                                                        INNER JOIN {DBLA}.INV1 t1 ON t0.""DocEntry"" = t1.""DocEntry""
            //                                              WHERE     t0.CANCELED = 'N' AND  t1.""TargetType"" <> 14
            //                                              GROUP BY t0.""DocEntry"", t0.""DocNum"", t1.""BaseEntry"", t0.""NumAtCard"", t0.U_NROAUTOR, CAST(t0.""DocDate"" AS DATE) ) a5 ON a1.""Id"" = a5.""BaseEntry""
            //                  WHERE   1 = 1 {InnerFilter} ";
            return query;
        }

        #endregion

        #region Constructors

        public Order() : base() { }

        #endregion
    }
}
