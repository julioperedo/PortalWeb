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
    public class ProductStock : DALEntity<BEA.ProductStock>
    {
        #region Global Variables

        #endregion

        #region Methods

        internal IEnumerable<BEA.ProductStock> List(IEnumerable<string> Keys, string ColumnName, params Enum[] Relations)
        {
            List<Field> lstFilter = new() { new Field { Name = ColumnName, Value = Keys, Operator = Operators.In } };
            IEnumerable<BEA.ProductStock> Items = List(lstFilter, "1", Relations);
            return Items;
        }

        protected override void LoadRelations(ref IEnumerable<BEA.ProductStock> Items, params Enum[] Relations)
        {
            IEnumerable<string> Keys;
            IEnumerable<BEA.Product> lstProducts = null;
            foreach (Enum RelationEnum in Relations)
            {
                Keys = from i in Items select i.ItemCode;
                if (RelationEnum.Equals(BEA.relProductStock.Product))
                {
                    using var dal = new Product();
                    lstProducts = dal.ReturnMasters(Keys, Relations);
                }
            }
            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
                    if (lstProducts != null)
                    {
                        Item.Product = (from i in lstProducts where i.Code == Item.ItemCode select i).FirstOrDefault();
                    }
                }
            }
        }

        protected override void LoadRelations(ref BEA.ProductStock Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
                if (RelationEnum.Equals(BEA.relProductStock.Product))
                {
                    using var dal = new Product();
                    Item.Product = dal.ReturnMaster(Item.ItemCode, Relations);
                }
            }
        }

        #endregion

        #region List Methods

        public IEnumerable<BEA.ProductStock> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            var filter = GetFilter(FilterList?.ToArray());
            StringBuilder sb = new();
            sb.AppendLine($@"SELECT	""Subsidiary"", ""Warehouse"", ""ItemCode"", ""Stock"", ""Reserved"", ""Requested"", ""Available"", ""Available2"", ""Blocked"", ""Rotation"" ");
            sb.AppendLine($@"FROM	({GetQuery()}) a ");
            sb.AppendLine($@"WHERE	{filter} ");
            sb.AppendLine($@"ORDER By {GetOrder(Order)}");
            IEnumerable<BEA.ProductStock> items = SQLList(sb.ToString(), Relations);
            return items;
        }

        public IEnumerable<BEA.ProductStock> ListDisabledStock(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            var filter = GetFilter(FilterList?.ToArray());
            StringBuilder sb = new();
            sb.AppendLine($@"SELECT	""Subsidiary"", ""Warehouse"", ""ItemCode"", ""Stock"", ""Reserved"", ""Requested"", ""Available"", ""Available2"" ");
            sb.AppendLine($@"FROM	({GetQuery("D")}) a ");
            sb.AppendLine($@"WHERE	{filter} ");
            sb.AppendLine($@"ORDER By {GetOrder(Order)}");
            IEnumerable<BEA.ProductStock> items = SQLList(sb.ToString(), Relations);
            return items;
        }

        public IEnumerable<BEA.ProductStock> ListWithCost(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            var filter = GetFilter(FilterList?.ToArray());
            var sb = new StringBuilder();
            sb.AppendLine($@"SELECT  ""Subsidiary"", ""Warehouse"", ""ItemCode"", ""Stock"", ""Reserved"", ""Requested"", ""Available"", ""Available2"", ""PriceReal"", ""TotalReal"", ""PriceModified"", ""TotalModified"", ""Warning"" ");
            sb.AppendLine($@"FROM    ( SELECT    'Santa Cruz' AS ""Subsidiary"", t2.""WhsName"" AS ""Warehouse"", t1.""ItemCode"", CAST(t1.""OnHand"" AS INTEGER) AS ""Stock"", CAST(t1.""IsCommited"" AS INTEGER) AS ""Reserved"", CAST(t1.""OnOrder"" AS INTEGER) AS ""Requested"" ");
            sb.AppendLine($@"                    , CAST(t1.""OnHand"" - t1.""IsCommited"" + t1.""OnOrder"" AS INTEGER) AS ""Available"", CAST(t1.""OnHand"" - t1.""IsCommited"" AS INTEGER) AS ""Available2"", IFNULL(t3.PU, 0) AS ""PriceReal"", IFNULL(t3.PU * t1.""OnHand"", 0) AS ""TotalReal"" ");
            sb.AppendLine($@"                    , IFNULL(( CASE WHEN LENGTH(t4.""Propiedad"") = 0 OR LENGTH(t4.""Propiedad"") > 2 THEN t3.PU / 0.975 ELSE t3.PU / ( CAST(t4.""Propiedad"" AS DECIMAL(19, 2)) / 100 ) END ), 0) AS ""PriceModified"" ");
            sb.AppendLine($@"                    , IFNULL(t1.""OnHand"" * ( CASE WHEN LENGTH(t4.""Propiedad"") = 0 OR LENGTH(t4.""Propiedad"") > 2 THEN t3.PU / 0.975 ELSE t3.PU / ( CAST(t4.""Propiedad"" AS DECIMAL(19, 2)) / 100 ) END ), 0) AS ""TotalModified"" ");
            sb.AppendLine($@"                    , ( CASE WHEN LENGTH(t4.""Propiedad"") > 2 THEN 'ERROR' ELSE '' END ) AS ""Warning"", IFNULL(t0.""frozenFor"", 'N') AS ""Blocked"", t0.""ItemName"", IFNULL(t0.U_CATEGORIA, '') AS ""Category"", IFNULL(t0.U_SUBCATEG, '') AS ""Subcategory"", IFNULL(t0.U_LINEA, '') AS ""Line"" ");
            sb.AppendLine($@"          FROM      {DBSA}.OITW t1 ");
            sb.AppendLine($@"                    INNER JOIN {DBSA}.OWHS t2 ON t1.""WhsCode"" = t2.""WhsCode"" ");
            sb.AppendLine($@"                    INNER JOIN {DBSA}.OITM t0 ON t0.""ItemCode"" = t1.""ItemCode"" ");
            sb.AppendLine($@"                    LEFT OUTER JOIN ( SELECT    ""Warehouse"", ""ItemCode"", SUM(""OpenValue"") / SUM(""OpenQty"") AS ""PU"" FROM {DBSA}.OINM WHERE  ""OpenQty"" > 0 GROUP BY  ""Warehouse"", ""ItemCode"" ) AS t3 ON t3.""ItemCode"" = t1.""ItemCode"" AND t3.""Warehouse"" = t1.""WhsCode"" ");
            sb.AppendLine($@"                    LEFT OUTER JOIN {DBSA}.PROPERTY t4 ON t4.""Item"" = t1.""ItemCode"" ");
            sb.AppendLine($@"          UNION ");
            sb.AppendLine($@"          SELECT    'Miami' AS ""Subsidiary"", t2.""WhsName"", t1.""ItemCode"", CAST(t1.""OnHand"" AS INTEGER), CAST(t1.""IsCommited"" AS INTEGER), CAST(t1.""OnOrder"" AS INTEGER), CAST(t1.""OnHand"" - t1.""IsCommited"" + t1.""OnOrder"" AS INTEGER) ");
            sb.AppendLine($@"                    , CAST(t1.""OnHand"" - t1.""IsCommited"" AS INTEGER), IFNULL(t3.PU, 0), IFNULL(t3.PU * t1.""OnHand"", 0) ");
            sb.AppendLine($@"                    , IFNULL(( CASE WHEN LENGTH(t4.""Propiedad"") = 0 OR  LENGTH(t4.""Propiedad"") > 2 THEN t3.PU / 0.975 ELSE t3.PU / ( CAST(t4.""Propiedad"" AS DECIMAL(19, 2)) / 100 ) END ), 0) ");
            sb.AppendLine($@"                    , IFNULL(t1.""OnHand"" * ( CASE WHEN LENGTH(t4.""Propiedad"") = 0 OR   LENGTH(t4.""Propiedad"") > 2 THEN t3.PU / 0.975 ELSE t3.PU / ( CAST(t4.""Propiedad"" AS decimal(19, 2)) / 100 ) END ), 0) ");
            sb.AppendLine($@"                    , ( CASE WHEN LENGTH(t4.""Propiedad"") > 2 THEN 'ERROR' ELSE '' END ), IFNULL(t0.""frozenFor"", 'N'), t0.""ItemName"", IFNULL(t0.U_CATEGORIA, ''), IFNULL(t0.U_SUBCATEG, ''), IFNULL(t0.U_LINEA, '') ");
            sb.AppendLine($@"          FROM      {DBLA}.OITW t1 ");
            sb.AppendLine($@"                    INNER JOIN {DBLA}.OWHS t2 ON t1.""WhsCode"" = t2.""WhsCode"" ");
            sb.AppendLine($@"                    INNER JOIN {DBLA}.OITM t0 ON t0.""ItemCode"" = t1.""ItemCode"" ");
            sb.AppendLine($@"                    LEFT OUTER JOIN ( SELECT    ""Warehouse"", ""ItemCode"", SUM(""OpenValue"") / SUM(""OpenQty"") AS ""PU"" FROM {DBLA}.OINM WHERE ""OpenQty"" > 0 GROUP BY ""Warehouse"", ""ItemCode"" ) AS t3 ON t3.""ItemCode"" = t1.""ItemCode"" AND t3.""Warehouse"" = t1.""WhsCode"" ");
            sb.AppendLine($@"                    LEFT OUTER JOIN {DBLA}.PROPERTY t4 ON t4.""Item"" = t1.""ItemCode"" ");
            sb.AppendLine($@"          UNION ");
            sb.AppendLine($@"          SELECT    'Iquique' AS ""Subsidiary"", t2.""WhsName"", t1.""ItemCode"", CAST(t1.""OnHand"" AS INTEGER), CAST(t1.""IsCommited"" AS INTEGER), CAST(t1.""OnOrder"" AS INTEGER), CAST(t1.""OnHand"" - t1.""IsCommited"" + t1.""OnOrder"" AS INTEGER) ");
            sb.AppendLine($@"                    , CAST(t1.""OnHand"" - t1.""IsCommited"" AS INTEGER), IFNULL(t3.PU, 0), IFNULL(t3.PU * t1.""OnHand"", 0) ");
            sb.AppendLine($@"                    , IFNULL(( CASE WHEN LENGTH(t4.""Propiedad"") = 0 OR LENGTH(t4.""Propiedad"") > 2 THEN t3.PU / 0.975 ELSE t3.PU / ( CAST(t4.""Propiedad"" AS DECIMAL(19, 2)) / 100 ) END ), 0) ");
            sb.AppendLine($@"                    , IFNULL(t1.""OnHand"" * ( CASE WHEN LENGTH(t4.""Propiedad"") = 0 OR  LENGTH(t4.""Propiedad"") > 2 THEN t3.PU / 0.975 ELSE t3.PU / ( CAST(t4.""Propiedad"" AS decimal(19, 2)) / 100 ) END ), 0) ");
            sb.AppendLine($@"                    , ( CASE WHEN LENGTH(t4.""Propiedad"") > 2 THEN 'ERROR' ELSE '' END ), IFNULL(t0.""frozenFor"", 'N'), t0.""ItemName"", IFNULL(t0.U_CATEGORIA, ''), IFNULL(t0.U_SUBCATEG, ''), IFNULL(t0.U_LINEA, '') ");
            sb.AppendLine($@"          FROM      {DBIQ}.OITW t1 ");
            sb.AppendLine($@"                    INNER JOIN {DBIQ}.OWHS t2 ON t1.""WhsCode"" = t2.""WhsCode"" ");
            sb.AppendLine($@"                    INNER JOIN {DBIQ}.OITM t0 ON t0.""ItemCode"" = t1.""ItemCode"" ");
            sb.AppendLine($@"                    LEFT OUTER JOIN ( SELECT    ""Warehouse"", ""ItemCode"", SUM(""OpenValue"") / SUM(""OpenQty"") AS ""PU"" FROM {DBIQ}.OINM WHERE ""OpenQty"" > 0 GROUP BY  ""Warehouse"", ""ItemCode"" ) AS t3 ON t3.""ItemCode"" = t1.""ItemCode"" AND t3.""Warehouse"" = t1.""WhsCode"" ");
            sb.AppendLine($@"                    LEFT OUTER JOIN {DBIQ}.PROPERTY t4 ON t4.""Item"" = t1.""ItemCode"" ) AS a ");
            sb.AppendLine($@"WHERE   ( a.""Stock"" > 0 OR  a.""Reserved"" > 0 OR a.""Requested"" > 0 ) AND {filter} ");
            sb.AppendLine($@"ORDER By {GetOrder(Order)}");
            IEnumerable<BEA.ProductStock> items = SQLList(sb.ToString(), Relations);
            return items;
        }

        public IEnumerable<BEA.ProductStock> ListBalance(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            string query, filter = GetFilter(FilterList?.ToArray());
            query = $@"SELECT  * 
FROM    ( SELECT  'Santa Cruz' AS ""Subsidiary"", T4.""WhsName"" AS ""Warehouse"", T0.""ItemCode"", T1.""ItemName"", T1.U_CATEGORIA AS ""Category"", T1.U_SUBCATEG AS ""Subcategory"", T3.""Memo"" AS ""ProductManager"" 
                  , IFNULL(T1.U_LINEA, 'Ninguna') AS ""Line"", CAST(T0.""OpenQty"" AS integer) AS ""Stock"", (T0.""OpenValue"" / T0.""OpenQty"") AS ""PriceReal"", T0.""OpenValue"" AS ""TotalReal"" 
                  , IFNULL(T1.U_MARCA, 'Ninguna') AS ""Brand"", CAST(T0.""DocDate"" AS DATE) AS ""Date"", IFNULL(T1.U_ROTACION, '') AS ""Rotation""
                  , CAST( ( CASE T0.""TransType"" 
                                WHEN 69 THEN (SELECT TA.U_FECHALLEGADA FROM {DBSA}.OIPF TA WHERE TA.""DocNum"" = T0.""BASE_REF"" ) -- Precio Entrega
                                WHEN 18 THEN (SELECT FIRST_VALUE(TZ.""U_FECHAR2"" ORDER BY TZ.""U_FECHAR2"") FROM {DBSA}.OPCH TZ WHERE TZ.""DocNum"" = T0.""BASE_REF"" ) -- Fact Proveedores
                                WHEN 67 THEN (SELECT TZ.U_FECHAR2 FROM {DBSA}.OWTR TZ WHERE TZ.""DocNum"" = T0.""BASE_REF"") -- Transferencia de Stock
                                WHEN 20 THEN (SELECT  FIRST_VALUE(TZ.""U_FECHAR2"" ORDER BY TZ.""U_FECHAR2"") FROM {DBSA}.OPDN TZ WHERE TZ.""DocNum"" = T0.""BASE_REF"") -- Entrada Mercaderia OP AS ""FECHA LLEGADA IC"" 
                                ELSE '' END ) AS DATE ) AS ""ArrivalDate""
          FROM    {DBSA}.OINM T0 
                  INNER JOIN {DBSA}.OITM T1 ON T1.""ItemCode"" = T0.""ItemCode"" 
                  INNER JOIN {DBSA}.OITW T2 ON T1.""ItemCode"" = T2.""ItemCode"" AND T2.""WhsCode"" = T0.""Warehouse"" 
                  LEFT OUTER JOIN {DBSA}.OSLP T3 ON T1.U_CODGRT = T3.""SlpName"" 
                  INNER JOIN {DBSA}.OWHS T4 ON T4.""WhsCode"" = T2.""WhsCode"" 
          WHERE   T2.""OnHand"" > 0 AND T0.""OpenQty"" > 0
          UNION ALL 
          SELECT  'Miami', T4.""WhsName"", T0.""ItemCode"", T1.""ItemName"", T1.U_CATEGORIA, T1.U_SUBCATEG, T3.""Memo"" 
                  , IFNULL(T1.U_LINEA, 'Ninguna'), CAST(T0.""OpenQty"" AS integer), (T0.""OpenValue"" / T0.""OpenQty""), T0.""OpenValue"", IFNULL(T1.U_MARCA, 'Ninguna'), CAST(T0.""DocDate"" AS DATE)
                  , IFNULL(T1.U_ROTACION, '') AS ""Rotation"", NULL
          FROM    {DBLA}.OINM T0 
                  INNER JOIN {DBLA}.OITM T1 ON T1.""ItemCode"" = T0.""ItemCode"" 
                  INNER JOIN {DBLA}.OITW T2 ON T1.""ItemCode"" = T2.""ItemCode"" AND T2.""WhsCode"" = T0.""Warehouse"" 
                  LEFT OUTER JOIN {DBLA}.OSLP T3 ON T1.U_CODGRT = T3.""SlpName"" 
                  INNER JOIN {DBLA}.OWHS T4 ON T4.""WhsCode"" = T2.""WhsCode"" 
          WHERE   T2.""OnHand"" > 0 AND T0.""OpenQty"" > 0
          UNION ALL 
          SELECT  'Iquique', T4.""WhsName"", T0.""ItemCode"", T1.""ItemName"", T1.U_CATEGORIA, T1.U_SUBCATEG, T3.""Memo"" 
                  , IFNULL(T1.U_LINEA, 'Ninguna'), CAST(T0.""OpenQty"" AS integer), (T0.""OpenValue"" / T0.""OpenQty""), T0.""OpenValue"", IFNULL(T1.U_MARCA, 'Ninguna'), CAST(T0.""DocDate"" AS DATE)
                  , IFNULL(T1.U_ROTACION, '') AS ""Rotation"", NULL
          FROM    {DBIQ}.OINM T0 
                  INNER JOIN {DBIQ}.OITM T1 ON T1.""ItemCode"" = T0.""ItemCode"" 
                  INNER JOIN {DBIQ}.OITW T2 ON T1.""ItemCode"" = T2.""ItemCode"" AND T2.""WhsCode"" = T0.""Warehouse"" 
                  LEFT OUTER JOIN {DBIQ}.OSLP T3 ON T1.U_CODGRT = T3.""SlpName"" 
                  INNER JOIN {DBIQ}.OWHS T4 ON T4.""WhsCode"" = T2.""WhsCode"" 
          WHERE   T2.""OnHand"" > 0 AND T0.""OpenQty"" > 0 ) AS a 
WHERE   {filter} 
ORDER BY {GetOrder(Order)} ";

            IEnumerable<BEA.ProductStock> items = SQLList(query, Relations);
            return items;
        }

        public IEnumerable<BEA.ProductStock> ListFlow(string ItemCode, string DBName)
        {
            string query = $@"SELECT  'Santa Cruz' AS ""Subsidiary"", T2.""WhsName"" AS ""Warehouse"", T0.""ItemCode"", CAST(T1.""OnHand"" AS integer) AS ""Stock"", CAST(T1.""IsCommited"" AS integer) AS ""Reserved""
                                      , CAST(T1.""OnOrder"" AS integer) AS ""Requested"", CAST(T1.""OnHand"" - T1.""IsCommited"" + T1.""OnOrder"" AS integer) AS ""Available""
                                      , CAST(T1.""OnHand"" - T1.""IsCommited"" AS integer) AS ""Available2"", IFNULL(T0.U_ROTACION, '') AS ""Rotation""
                              FROM    {DBName}.OITM T0 
                                      INNER JOIN {DBName}.OITW T1 ON T0.""ItemCode"" = T1.""ItemCode"" 
                                      INNER JOIN {DBName}.OWHS T2 ON T1.""WhsCode"" = T2.""WhsCode"" 
                                      INNER JOIN {DBName}.OITB T3 ON T0.""ItmsGrpCod"" = T3.""ItmsGrpCod"" 
                              WHERE   IFNULL(T0.""frozenFor"", 'N') <> 'Y' AND LOWER(T0.""ItemCode"") = '{ItemCode.ToLower()}' 
                                      AND ( T1.""OnHand"" - T1.""IsCommited"" ) > 0 ";

            IEnumerable<BEA.ProductStock> items = base.SQLList(query);
            return items;
        }

        public IEnumerable<BEA.ProductStock> ListServiceMany(string ItemCodes)
        {
            string query = $@"SELECT  'Santa Cruz' AS ""Subsidiary"", T2.""WhsName"" AS ""Warehouse"", T0.""ItemCode"", CAST(T1.""OnHand"" AS integer) AS ""Stock"", CAST(T1.""IsCommited"" AS integer) AS ""Reserved""
                                      , CAST(T1.""OnOrder"" AS integer) AS ""Requested"", CAST(T1.""OnHand"" - T1.""IsCommited"" + T1.""OnOrder"" AS integer) AS ""Available""
                                      , CAST(T1.""OnHand"" - T1.""IsCommited"" AS integer) AS ""Available2"", IFNULL(T0.U_ROTACION, '') AS ""Rotation""
                              FROM    {DBSA}.OITM T0 
                                      INNER JOIN {DBSA}.OITW T1 ON T0.""ItemCode"" = T1.""ItemCode"" 
                                      INNER JOIN {DBSA}.OWHS T2 ON T1.""WhsCode"" = T2.""WhsCode"" 
                                      INNER JOIN {DBSA}.OITB T3 ON T0.""ItmsGrpCod"" = T3.""ItmsGrpCod"" 
                              WHERE   IFNULL(T0.""frozenFor"", 'N') <> 'Y' AND LOWER(T0.""ItemCode"") IN ( {ItemCodes.ToLower()} ) 
                                      AND ( T1.""OnHand"" - T1.""IsCommited"" ) > 0 ";

            IEnumerable<BEA.ProductStock> items = SQLList(query);
            return items;
        }

        public IEnumerable<BEA.ProductStock> ListShort(List<Field> FilterList, string Order)
        {
            StringBuilder sbQuery = new();
            string strFilter = "";
            if (FilterList?.Count > 0)
            {
                strFilter = GetFilter(FilterList.ToArray());
            }
            sbQuery.AppendLine($@"SELECT  ""Subsidiary"", ""Warehouse"", ""ItemCode"", ""Stock"", ""Reserved"", ""Requested"", ""Available"", ""Available2"", ""Rotation"" ");
            sbQuery.AppendLine($@"FROM    ({GetQuery()}) a ");
            if (strFilter != "") sbQuery.AppendLine($@"WHERE   {strFilter} ");
            sbQuery.AppendLine($@"ORDER By {GetOrder(Order)} ");

            IEnumerable<BEA.ProductStock> items = base.SQLList(sbQuery.ToString());
            return items;
        }

        public IEnumerable<BEA.ProductStock> ListRelated(string Categories, string ItemCodes, string Warehouses, int Quantity)
        {
            string query = $@"SELECT  TOP {Quantity} ""Subsidiary"", ""ItemCode"", SUM(""Stock"") AS ""Stock"", SUM(""Reserved"") AS ""Reserved"", SUM(""Requested"") AS ""Requested"", SUM(""Available"") AS ""Available"", SUM(""Available2"") AS ""Available2""
FROM    ( SELECT  'Santa Cruz' AS ""Subsidiary"", t2.""WhsName"" AS ""Warehouse"", t1.""ItemCode"", CAST(t1.""OnHand"" AS INTEGER) AS ""Stock"", CAST(t1.""IsCommited"" AS INTEGER) AS ""Reserved"", CAST(t1.""OnOrder"" AS INTEGER) AS ""Requested"" 
                  , CAST(t1.""OnHand"" - t1.""IsCommited"" + t1.""OnOrder"" AS INTEGER) AS ""Available"", CAST(t1.""OnHand"" - t1.""IsCommited"" AS INTEGER) AS ""Available2"" 
                  , IFNULL(t0.""frozenFor"", 'N') AS ""Blocked"", t0.""ItemName"", IFNULL(t0.U_CATEGORIA, '') AS ""Category"" 
          FROM    {DBSA}.OITW t1 
                  INNER JOIN {DBSA}.OITM t0 ON t0.""ItemCode"" = t1.""ItemCode""  
                  INNER JOIN {DBSA}.OWHS t2 ON t1.""WhsCode"" = t2.""WhsCode"" 
          UNION 
          SELECT  'Miami' AS ""Subsidiary"", t2.""WhsName"" AS ""Warehouse"", t1.""ItemCode"", CAST(t1.""OnHand"" AS INTEGER), CAST(t1.""IsCommited"" AS INTEGER), CAST(t1.""OnOrder"" AS INTEGER), CAST(t1.""OnHand"" - t1.""IsCommited"" + t1.""OnOrder"" AS INTEGER) 
                  , CAST(t1.""OnHand"" - t1.""IsCommited"" AS INTEGER), IFNULL(t0.""frozenFor"", 'N'), t0.""ItemName"", IFNULL(t0.U_CATEGORIA, '')
          FROM    {DBLA}.OITW t1 
                  INNER JOIN {DBLA}.OITM t0 ON t0.""ItemCode"" = t1.""ItemCode"" 
                  INNER JOIN {DBLA}.OWHS t2 ON t1.""WhsCode"" = t2.""WhsCode"" 
          UNION 
          SELECT  'Iquique' AS ""Subsidiary"", t2.""WhsName"" AS ""Warehouse"", t1.""ItemCode"", CAST(t1.""OnHand"" AS INTEGER), CAST(t1.""IsCommited"" AS INTEGER), CAST(t1.""OnOrder"" AS INTEGER), CAST(t1.""OnHand"" - t1.""IsCommited"" + t1.""OnOrder"" AS INTEGER) 
                  , CAST(t1.""OnHand"" - t1.""IsCommited"" AS INTEGER), IFNULL(t0.""frozenFor"", 'N'), t0.""ItemName"", IFNULL(t0.U_CATEGORIA, '')
          FROM    {DBIQ}.OITW t1 
                  INNER JOIN {DBIQ}.OITM t0 ON t0.""ItemCode"" = t1.""ItemCode""
                  INNER JOIN {DBIQ}.OWHS t2 ON t1.""WhsCode"" = t2.""WhsCode"" ) a
WHERE    ""Blocked"" = 'N' AND ( ( ""Subsidiary"" = 'Santa Cruz' AND ""Stock"" > 0 ) OR ( ""Subsidiary"" <> 'Santa Cruz' AND ""Available2"" > 0 ) )  
         AND LOWER(""Warehouse"") IN ( {Warehouses.ToLower()} )
         AND ( LOWER(""Category"") IN ( {(string.IsNullOrEmpty(Categories) ? "''" : Categories.ToLower())} ) OR LOWER(""ItemCode"") IN ( {(string.IsNullOrEmpty(ItemCodes) ? "''" : ItemCodes.ToLower())} ) )
GROUP BY ""Subsidiary"", ""ItemCode""      
ORDER BY RAND()  ";
            IEnumerable<BEA.ProductStock> items = base.SQLList(query);
            return items;
        }

        public IEnumerable<BEA.ProductStock> ListForLoan()
        {
            string query = $@"SELECT 	o2.""ItemCode"", ""ItemName"", IFNULL(U_CATEGORIA, 'Sin Categoría') AS ""Category"", IFNULL(U_SUBCATEG, 'Sin Subcategoría') AS ""Subcategory"", U_MARCA AS ""Brand"", CAST(o.""OnHand"" AS INTEGER) AS ""Stock""
FROM 	{DBSA}.OITW o
        INNER JOIN {DBSA}.OITM o2 ON o.""ItemCode"" = o2.""ItemCode"" 	
WHERE 	""WhsCode"" = 'ZFLPB' AND o.""OnHand"" > 0 ";
            IEnumerable<BEA.ProductStock> items = SQLList(query);
            return items;
        }

        #endregion

        #region Search Methods

        public BEA.ProductStock Search(string CardCode, string Name, string Type, params Enum[] Relations)
        {
            StringBuilder sb = new();
            sb.AppendLine($@"SELECT  ""ProductStock"" AS ""Name"", ""CardCode"", ""Street"" AS ""ProductStock1"", ""Block"" AS ""Contact"", t0.""Country"", ""City"", t1.""Name"" AS ""State"", ""TaxCode"", ""AdresType"" AS ""Type"", ""ProductStock2"", ""ProductStock3"" ");
            sb.AppendLine($@"FROM    {DBSA}.CRD1 t0 ");
            sb.AppendLine($@"        LEFT OUTER JOIN {DBSA}.OCST t1 ON t0.""State"" = t1.""Code"" AND t0.""Country"" = t1.""Country"" ");
            sb.AppendLine($@"WHERE	""CardCode"" = '{CardCode}' AND ""ProductStock"" = '{Name}' AND ""AdresType"" = '{Type}' ");

            BEA.ProductStock item = SQLSearch(sb.ToString(), Relations);
            return item;
        }

        public BEA.ProductStock Search(string Id)
        {
            StringBuilder sbQuery = new();
            sbQuery.AppendLine($@"SELECT  * ");
            sbQuery.AppendLine($@"FROM    ({GetQuery()}) a ");
            sbQuery.AppendLine($@"WHERE   ""ItemCode"" = '{Id}' ");
            BEA.ProductStock beItem = base.SQLSearch(sbQuery.ToString());
            return beItem;
        }

        #endregion

        #region Private Methods

        private string GetQuery(string State = "A")
        {
            string filter = State == "E" ? @"AND LTRIM(RTRIM(IFNULL(t0.""frozenFor"", 'N'))) <> 'Y'" : (State == "D" ? @"AND LTRIM(RTRIM(ISNULL(t0.""frozenFor"", 'N'))) <> 'N'" : "");
            var sb = new StringBuilder();
            sb.AppendLine($@"SELECT  'Santa Cruz' AS ""Subsidiary"", t2.""WhsName"" AS ""Warehouse"", t1.""ItemCode"", CAST(t1.""OnHand"" AS INTEGER) AS ""Stock"", CAST(t1.""IsCommited"" AS INTEGER) AS ""Reserved"", CAST(t1.""OnOrder"" AS INTEGER) AS ""Requested"" ");
            sb.AppendLine($@"        , CAST(t1.""OnHand"" - t1.""IsCommited"" + t1.""OnOrder"" AS INTEGER) AS ""Available"", CAST(t1.""OnHand"" - t1.""IsCommited"" AS INTEGER) AS ""Available2"" ");
            sb.AppendLine($@"        , IFNULL(t0.""frozenFor"", 'N') AS ""Blocked"", t0.""ItemName"", IFNULL(t0.U_CATEGORIA, '') AS ""Category"", IFNULL(t0.U_SUBCATEG, '') AS ""Subcategory"", IFNULL(t0.U_LINEA, '') AS ""Line"", t0.U_GPRODUCT AS ""ProductManager"" ");
            sb.AppendLine($@"        , IFNULL(t0.U_ROTACION, '') AS ""Rotation"" ");
            sb.AppendLine($@"FROM    {DBSA}.OITW t1 ");
            sb.AppendLine($@"        INNER JOIN {DBSA}.OWHS t2 ON t1.""WhsCode"" = t2.""WhsCode"" ");
            sb.AppendLine($@"        INNER JOIN {DBSA}.OITM t0 ON t0.""ItemCode"" = t1.""ItemCode"" {filter} ");
            sb.AppendLine($@"UNION ");
            sb.AppendLine($@"SELECT  'Miami' AS ""Subsidiary"", t2.""WhsName"", t1.""ItemCode"", CAST(t1.""OnHand"" AS INTEGER), CAST(t1.""IsCommited"" AS INTEGER), CAST(t1.""OnOrder"" AS INTEGER), CAST(t1.""OnHand"" - t1.""IsCommited"" + t1.""OnOrder"" AS INTEGER) ");
            sb.AppendLine($@"        , CAST(t1.""OnHand"" - t1.""IsCommited"" AS INTEGER) ");
            sb.AppendLine($@"        , IFNULL(t0.""frozenFor"", 'N'), t0.""ItemName"", IFNULL(t0.U_CATEGORIA, ''), IFNULL(t0.U_SUBCATEG, ''), IFNULL(t0.U_LINEA, ''), t0.U_GPRODUCT ");
            sb.AppendLine($@"        , IFNULL(t0.U_ROTACION, '') AS ""Rotation"" ");
            sb.AppendLine($@"FROM    {DBLA}.OITW t1 ");
            sb.AppendLine($@"        INNER JOIN {DBLA}.OWHS t2 ON t1.""WhsCode"" = t2.""WhsCode"" ");
            sb.AppendLine($@"        INNER JOIN {DBLA}.OITM t0 ON t0.""ItemCode"" = t1.""ItemCode"" {filter} ");
            sb.AppendLine($@"UNION ");
            sb.AppendLine($@"SELECT  'Iquique' AS ""Subsidiary"", t2.""WhsName"", t1.""ItemCode"", CAST(t1.""OnHand"" AS INTEGER), CAST(t1.""IsCommited"" AS INTEGER), CAST(t1.""OnOrder"" AS INTEGER), CAST(t1.""OnHand"" - t1.""IsCommited"" + t1.""OnOrder"" AS INTEGER) ");
            sb.AppendLine($@"        , CAST(t1.""OnHand"" - t1.""IsCommited"" AS INTEGER) ");
            sb.AppendLine($@"        , IFNULL(t0.""frozenFor"", 'N'), t0.""ItemName"", IFNULL(t0.U_CATEGORIA, ''), IFNULL(t0.U_SUBCATEG, ''), IFNULL(t0.U_LINEA, ''), t0.U_GPRODUCT ");
            sb.AppendLine($@"        , IFNULL(t0.U_ROTACION, '') AS ""Rotation"" ");
            sb.AppendLine($@"FROM    {DBIQ}.OITW t1 ");
            sb.AppendLine($@"        INNER JOIN {DBIQ}.OWHS t2 ON t1.""WhsCode"" = t2.""WhsCode"" ");
            sb.AppendLine($@"        INNER JOIN {DBIQ}.OITM t0 ON t0.""ItemCode"" = t1.""ItemCode"" {filter} ");
            return sb.ToString();
        }

        #endregion

        #region Constructors

        public ProductStock() : base() { }

        #endregion
    }
}
