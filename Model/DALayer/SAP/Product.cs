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
    public class Product : DALEntity<BEA.Product>
    {
        #region Global Variables

        #endregion

        #region Methods

        internal BEA.Product ReturnMaster(string Id, params Enum[] Relations)
        {
            return SearchSC(Id, Relations);
        }

        internal IEnumerable<BEA.Product> ReturnMasters(IEnumerable<string> Keys, params Enum[] Relations)
        {
            List<Field> lstFilter = new() { new Field { Name = "Code", Value = Keys, Operator = Operators.In } };
            IEnumerable<BEA.Product> Items = ListCatalog(lstFilter, "1", Relations);
            return Items;
        }

        internal IEnumerable<BEA.Product> List(IEnumerable<string> Keys, string ColumnName, params Enum[] Relations)
        {
            List<Field> lstFilter = new() { new Field { Name = ColumnName, Value = Keys, Operator = Operators.In } };
            IEnumerable<BEA.Product> Items = ListCatalog(lstFilter, "1", Relations);
            return Items;
        }

        protected override void LoadRelations(ref IEnumerable<BEA.Product> Items, params Enum[] Relations)
        {
            IEnumerable<string> Keys;
            IEnumerable<BEA.ProductStock> lstStock = null;
            foreach (Enum RelationEnum in Relations)
            {
                Keys = from i in Items select i.Code;
                if (RelationEnum.Equals(BEA.relProduct.ProductStocks))
                {
                    using var dal = new ProductStock();
                    lstStock = dal.List(Keys, "ItemCode", Relations);
                }
            }
            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
                    if (lstStock != null)
                    {
                        Item.Stock = lstStock.Where(x => x.ItemCode.ToLower() == Item.Code.ToLower())?.ToList();
                    }
                }
            }
        }

        protected override void LoadRelations(ref BEA.Product Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
                string[] Keys = new[] { Item.Code };
                if (RelationEnum.Equals(BEA.relProduct.ProductStocks))
                {
                    using var dal = new ProductStock();
                    Item.Stock = dal.List(Keys, "ItemCode", Relations)?.ToList();
                }
            }
        }

        #endregion

        #region List Methods

        public IEnumerable<BEA.Product> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new();
            string strFilter = FilterList?.Count > 0 ? GetFilter(FilterList.ToArray()) : "";
            var sb = new StringBuilder();
            sb.AppendLine($@"SELECT  ""ItemCode"" AS ""Code"", ""ItemName"" AS ""Name"", IFNULL(U_CATEGORIA, 'Sin Categoría') AS ""Category"", IFNULL(U_SUBCATEG, 'Sin Subcategoría') AS ""Subcategory"", U_LINEA AS ""Line"" ");
            sb.AppendLine($@"        , IFNULL(""FrgnName"", '') AS ""Description"", IFNULL(U_GPRODUCT, 'NINGUNO') AS ""ProductManager"" ");
            sb.AppendLine($@"FROM    {DBSA}.OITM ");
            if (strFilter != "") sb.AppendLine($@"WHERE  {strFilter} ");
            sb.AppendLine($@"ORDER By {GetOrder(Order)} ");

            IEnumerable<BEA.Product> items = SQLList(sbQuery.ToString(), Relations);
            return items;
        }

        public IEnumerable<BEA.Product> ListCatalog(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new();
            string strFilter = "";
            if (FilterList?.Count > 0) strFilter = GetFilter(FilterList.ToArray());

            sbQuery.AppendLine($@"SELECT  DISTINCT ""Code"", ""Name"", IFNULL(""Category"", 'Sin Categoría') AS ""Category"", IFNULL(""Subcategory"", 'Sin Subcategoría') AS ""Subcategory"", ""Line"", IFNULL(""Description"", '') AS ""Description"", ""ProductManager"", ""Blocked"", ""Brand"", ""Rotation"" ");
            sbQuery.AppendLine($@"FROM    ({GetFullCatalogQuery()}) a ");
            if (strFilter != "") sbQuery.AppendLine($@"WHERE   {strFilter} ");
            sbQuery.AppendLine($@"ORDER By {GetOrder(Order)} ");

            IEnumerable<BEA.Product> items = SQLList(sbQuery.ToString(), Relations);
            return items;
        }

        public IEnumerable<BEA.Product> ListCatalog2(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new();
            string strFilter = FilterList?.Count > 0 ? GetFilter(FilterList.ToArray()) : "";
            sbQuery.AppendLine($@"SELECT  DISTINCT ""Code"", ""Name"", IFNULL(""Category"", 'Sin Categoría') AS ""Category"", IFNULL(""Subcategory"", 'Sin Subcategoría') AS ""Subcategory"", ""Line"", IFNULL(""Description"", '') AS ""Description"", ""ProductManager"", ""Picture"", ""SuppCatNum"", U_MARCA AS ""Brand"", ""Commentaries"", ""CodeBars"" ");
            sbQuery.AppendLine($@"FROM    ({GetCatalogSAQuery()}) a ");
            if (!string.IsNullOrEmpty(strFilter)) sbQuery.AppendLine($@"WHERE   {strFilter} ");
            sbQuery.AppendLine($@"ORDER By {GetOrder(Order)} ");

            IEnumerable<BEA.Product> items = SQLList(sbQuery.ToString(), Relations);
            return items;
        }

        public IEnumerable<BEA.Product> ListSC(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new();
            string strFilter = "";
            if (FilterList?.Count > 0)
            {
                strFilter = GetFilter(FilterList.ToArray());
            }
            sbQuery.AppendLine($@"SELECT  * ");
            sbQuery.AppendLine($@"FROM    ({GetQuery("SA")}) a ");
            if (strFilter != "") sbQuery.AppendLine($@"WHERE   {strFilter} ");
            sbQuery.AppendLine($@"ORDER By {GetOrder(Order)} ");

            IEnumerable<BEA.Product> items = SQLList(sbQuery.ToString(), Relations);
            return items;
        }

        public IEnumerable<BEA.Product> ListLA(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new();
            string strFilter = "";
            if (FilterList?.Count > 0)
            {
                strFilter = GetFilter(FilterList.ToArray());
            }
            sbQuery.AppendLine($@"SELECT  * ");
            sbQuery.AppendLine($@"FROM    ({GetQuery("LA")}) a ");
            if (strFilter != "") sbQuery.AppendLine($@"WHERE   {strFilter} ");
            sbQuery.AppendLine($@"ORDER By {GetOrder(Order)} ");

            IEnumerable<BEA.Product> items = SQLList(sbQuery.ToString(), Relations);
            return items;
        }

        public IEnumerable<BEA.Product> ListIQQ(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new();
            string strFilter = "";
            if (FilterList?.Count > 0)
            {
                strFilter = GetFilter(FilterList.ToArray());
            }
            sbQuery.AppendLine($@"SELECT  * ");
            sbQuery.AppendLine($@"FROM    ({GetQuery("IQ")}) a ");
            if (strFilter != "") sbQuery.AppendLine($@"WHERE   {strFilter} ");
            sbQuery.AppendLine($@"ORDER By {GetOrder(Order)} ");

            IEnumerable<BEA.Product> items = SQLList(sbQuery.ToString(), Relations);
            return items;
        }

        public IEnumerable<BEA.Product> ListOnlyLA(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sb = new();
            string strFilter = "";
            if (FilterList?.Count > 0)
            {
                strFilter = GetFilter(FilterList.ToArray());
            }
            sb.AppendLine($@"SELECT   * ");
            sb.AppendLine($@"FROM     ({GetQuery("LA")}) a ");
            sb.AppendLine($@"WHERE    ""Code"" NOT IN ( SELECT ""Code"" FROM ({GetQuery("SA")}) b ) ");
            sb.AppendLine($@"         AND ""Code"" NOT IN ( SELECT ""Code"" FROM ({GetQuery("IQ")}) b ) ");
            if (strFilter != "") sb.AppendLine($@"         AND {strFilter} ");
            sb.AppendLine($@"ORDER By {GetOrder(Order)} ");

            IEnumerable<BEA.Product> items = SQLList(sb.ToString(), Relations);
            return items;
        }

        public IEnumerable<BEA.Product> ListOnlyIQQ(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sb = new();
            string strFilter = "";
            if (FilterList?.Count > 0)
            {
                strFilter = GetFilter(FilterList.ToArray());
            }
            sb.AppendLine($@"SELECT   * ");
            sb.AppendLine($@"FROM     ({GetQuery("IQ")}) a ");
            sb.AppendLine($@"WHERE    ""Code"" NOT IN ( SELECT ""Code"" FROM ({GetQuery("SA")}) b ) ");
            if (strFilter != "") sb.AppendLine($@"         AND {strFilter} ");
            sb.AppendLine($@"ORDER By {GetOrder(Order)} ");

            IEnumerable<BEA.Product> items = SQLList(sb.ToString(), Relations);
            return items;
        }

        public IEnumerable<BEA.Product> ListPMs(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new();
            string strFilter = FilterList?.Count > 0 ? GetFilter(FilterList.ToArray()) : "";
            sbQuery.AppendLine($@"SELECT  ""ItemCode"" AS ""Code"", U_GPRODUCT AS ""ProductManager"", U_CODGRT AS ""ProductManagerCode"" ");
            sbQuery.AppendLine($@"FROM    {DBSA}.OITM ");
            if (strFilter != "") sbQuery.AppendLine($@"WHERE   {strFilter} ");
            sbQuery.AppendLine($@"ORDER By {GetOrder(Order)} ");

            IEnumerable<BEA.Product> items = SQLList(sbQuery.ToString(), Relations);
            return items;
        }

        public IEnumerable<BEA.Product> ListTopSales(string Codes, params Enum[] Relations)
        {
            int intMonths = 2;
            var sb = new StringBuilder();
            sb.AppendLine($@"SELECT  TOP 100 ""ItemCode"" AS ""Code"" ");
            sb.AppendLine($@"FROM    (   SELECT  t2.""ItemCode"", t2.""ItemName"", t2.U_CATEGORIA, t2.U_SUBCATEG, SUM(t1.""Quantity"") AS ""Total"" ");
            sb.AppendLine($@"            FROM    {DBSA}.OINV t0 ");
            sb.AppendLine($@"                    INNER JOIN {DBSA}.INV1 t1 ON t0.""DocEntry"" = t1.""DocEntry"" ");
            sb.AppendLine($@"                    INNER JOIN {DBSA}.OITM t2 ON t1.""ItemCode"" = t2.""ItemCode"" AND LTRIM(RTRIM(IFNULL(t2.""frozenFor"", ''))) <> 'Y' AND ""ItmsGrpCod"" <> 117 AND U_MARCA <> 'DMC' AND U_CATEGORIA <> 'REPUESTOS' ");
            sb.AppendLine($@"            WHERE   t0.CANCELED <> 'Y' AND t0.""DocDate"" >= ADD_MONTHS(NOW(), -{intMonths}) AND t0.""CardCode"" NOT IN ('CDMC-002','CDMC-003','CIMP-008') ");
            sb.AppendLine($@"            GROUP BY t2.""ItemCode"", t2.""ItemName"", t2.U_CATEGORIA, t2.U_SUBCATEG ");
            sb.AppendLine($@"            UNION ALL ");
            sb.AppendLine($@"            SELECT  t2.""ItemCode"", t2.""ItemName"", t2.U_CATEGORIA, t2.U_SUBCATEG, SUM(t1.""Quantity"") AS ""Total"" ");
            sb.AppendLine($@"            FROM    {DBIQ}.OINV t0 ");
            sb.AppendLine($@"                    INNER JOIN {DBIQ}.INV1 t1 ON t0.""DocEntry"" = t1.""DocEntry"" ");
            sb.AppendLine($@"                    INNER JOIN {DBIQ}.OITM t2 ON t1.""ItemCode"" = t2.""ItemCode"" AND LTRIM(RTRIM(IFNULL(t2.""frozenFor"", ''))) <> 'Y' AND ""ItmsGrpCod"" <> 117 AND U_MARCA <> 'DMC' AND U_CATEGORIA <> 'REPUESTOS' ");
            sb.AppendLine($@"            WHERE   t0.CANCELED <> 'Y' AND t0.""DocDate"" >= ADD_MONTHS(NOW(), -{intMonths}) AND t0.""CardCode"" NOT IN ('CDMC-002','CDMC-003','CIMP-008') ");
            sb.AppendLine($@"            GROUP BY t2.""ItemCode"", t2.""ItemName"", t2.U_CATEGORIA, t2.U_SUBCATEG ");
            sb.AppendLine($@"            UNION ALL ");
            sb.AppendLine($@"            SELECT  t2.""ItemCode"", t2.""ItemName"", t2.U_CATEGORIA, t2.U_SUBCATEG, SUM(t1.""Quantity"") AS ""Total"" ");
            sb.AppendLine($@"            FROM    {DBLA}.OINV t0 ");
            sb.AppendLine($@"                    INNER JOIN {DBLA}.INV1 t1 ON t0.""DocEntry"" = t1.""DocEntry"" ");
            sb.AppendLine($@"                    INNER JOIN {DBLA}.OITM t2 ON t1.""ItemCode"" = t2.""ItemCode"" AND LTRIM(RTRIM(IFNULL(t2.""frozenFor"", ''))) <> 'Y' AND ""ItmsGrpCod"" <> 117 AND U_MARCA <> 'DMC' AND U_CATEGORIA <> 'REPUESTOS' ");
            sb.AppendLine($@"            WHERE   t0.CANCELED <> 'Y' AND t0.""DocDate"" >= ADD_MONTHS(NOW(), -{intMonths}) AND t0.""CardCode"" NOT IN ('CDMC-002','CDMC-003','CIMP-008') ");
            sb.AppendLine($@"            GROUP BY t2.""ItemCode"", t2.""ItemName"", t2.U_CATEGORIA, t2.U_SUBCATEG ");
            sb.AppendLine($@"        ) AS a ");
            if (!string.IsNullOrWhiteSpace(Codes))
            {
                sb.AppendLine($@"WHERE   ""ItemCode"" IN ( {Codes} ) ");
            }
            sb.AppendLine($@"ORDER BY ""Total"" DESC ");

            IEnumerable<BEA.Product> items = SQLList(sb.ToString(), Relations);
            return items;
        }

        public IEnumerable<BEA.Product> ListFactoryCodes(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new();
            string strFilter = FilterList?.Count > 0 ? GetFilter(FilterList.ToArray()) : "";
            sbQuery.AppendLine($@"SELECT  ""ItemCode"" AS ""Code"", ""SuppCatNum"" AS ""FactoryCode"" ");
            sbQuery.AppendLine($@"FROM    {DBSA}.OITM ");
            if (strFilter != "") sbQuery.AppendLine($@"WHERE   {strFilter} ");
            sbQuery.AppendLine($@"ORDER By {GetOrder(Order)} ");

            IEnumerable<BEA.Product> items = SQLList(sbQuery.ToString(), Relations);
            return items;
        }

        #endregion

        #region Search Methods

        public BEA.Product SearchSC(string Id, params Enum[] Relations)
        {
            string strQuery = $@"SELECT * FROM ({GetQuery("SC")}) a WHERE LOWER(""Code"") = '{Id.ToLower()}' ";
            BEA.Product item = SQLSearch(strQuery, Relations);
            return item;
        }

        public BEA.Product SearchLA(string Id, params Enum[] Relations)
        {
            string strQuery = $@"SELECT * FROM ({GetQuery("LA")}) a WHERE LOWER(""Code"") = '{Id.ToLower()}' ";
            BEA.Product item = SQLSearch(strQuery, Relations);
            return item;
        }

        public BEA.Product SearchIQQ(string Id, params Enum[] Relations)
        {
            string strQuery = $@"SELECT * FROM ({GetQuery("IQ")}) a WHERE LOWER(""Code"") = '{Id.ToLower()}' ";
            BEA.Product item = SQLSearch(strQuery, Relations);
            return item;
        }

        public BEA.Product SearchFlow(string Id, string DBName)
        {
            string query = $@"SELECT  ""ItemCode"" AS ""Code"", ""SuppCatNum"", ""ItemName"" AS ""Name"", U_CATEGORIA AS ""Category"", U_SUBCATEG AS ""Subcategory"", U_LINEA AS ""Line"", IFNULL(""FrgnName"", '') AS ""Description"", IFNULL(""UserText"", '') AS ""Commentaries"" 
                                      , U_GRTIAVENT AS ""Warranty"", ""PicturName"" AS ""Picture"", (SELECT TOP 1 CAST(IFNULL(""BitmapPath"", '') AS varchar(5000)) FROM {DBName}.OADP) AS ""FilePath"", IFNULL(U_GPRODUCT, 'NINGUNO') AS ""ProductManager"" 
                              FROM    {DBName}.OITM 
                              WHERE   LTRIM(RTRIM(IFNULL(""frozenFor"", ''))) <> 'Y' AND ""ItemCode"" = '{Id}' ";
            BEA.Product item = base.SQLSearch(query);
            return item;
        }

        #endregion

        #region Private Methods

        private string GetQuery(string Subsidiary)
        {
            string database = Subsidiary == "SC" ? DBSA : (Subsidiary == "IQ" ? DBIQ : DBLA);
            var sb = new StringBuilder();
            sb.AppendLine($@"SELECT  ""ItemCode"" AS ""Code"", ""ItemName"" AS ""Name"", U_CATEGORIA AS ""Category"", U_SUBCATEG AS ""Subcategory"", U_LINEA AS ""Line"", IFNULL(""FrgnName"", '') AS ""Description"", IFNULL(""UserText"", '') AS ""Commentaries"" ");
            sb.AppendLine($@"        , U_GRTIAVENT AS ""Warranty"", ""PicturName"" AS ""Picture"", (SELECT TOP 1 CAST(IFNULL(""BitmapPath"", '') AS varchar(8000)) FROM {database}.OADP) AS ""FilePath"", IFNULL(U_GPRODUCT, 'NINGUNO') AS ""ProductManager"" ");
            sb.AppendLine($@"        , U_CODGRT AS ""ProductManagerCode"", ""SuppCatNum"" AS ""FactoryCode"", IFNULL(U_MARCA, '') AS ""Brand"" ");
            sb.AppendLine($@"FROM    {database}.OITM ");
            return sb.ToString();
        }

        private string GetFullCatalogQuery()
        {
            var sb = new StringBuilder();
            sb.AppendLine($@"SELECT  ""ItemCode"" AS ""Code"", ""ItemName"" AS ""Name"", U_CATEGORIA AS ""Category"", U_SUBCATEG AS ""Subcategory"", U_LINEA AS ""Line"", IFNULL(""FrgnName"", '') AS ""Description"", CAST(IFNULL(""UserText"", '') AS varchar(5000)) AS ""Commentaries"" ");
            sb.AppendLine($@"        , U_GRTIAVENT AS ""Warranty"", ""PicturName"" AS ""Picture"", (SELECT TOP 1 CAST(IFNULL(""BitmapPath"", '') AS varchar(5000)) FROM {DBSA}.OADP) AS ""FilePath"", IFNULL(U_GPRODUCT, 'NINGUNO') AS ""ProductManager"" ");
            sb.AppendLine($@"        , U_CODGRT AS ""ProductManagerCode"", ""SuppCatNum"" AS ""FactoryCode"", IFNULL(""frozenFor"", 'N') AS ""Blocked"", IFNULL(U_MARCA, '') AS ""Brand"", IFNULL(U_ROTACION, '') AS ""Rotation"" ");
            sb.AppendLine($@"FROM    {DBSA}.OITM ");
            sb.AppendLine($@"UNION ");
            sb.AppendLine($@"SELECT  ""ItemCode"" AS ""Code"", ""ItemName"" AS ""Name"", U_CATEGORIA AS ""Category"", U_SUBCATEG AS ""Subcategory"", U_LINEA AS ""Line"", IFNULL(""FrgnName"", '') AS ""Description"", CAST(IFNULL(""UserText"", '') AS varchar(5000)) AS ""Commentaries"" ");
            sb.AppendLine($@"        , U_GRTIAVENT AS ""Warranty"", ""PicturName"" AS ""Picture"", (SELECT TOP 1 CAST(IFNULL(""BitmapPath"", '') AS varchar(5000)) FROM {DBLA}.OADP) AS ""FilePath"", IFNULL(U_GPRODUCT, 'NINGUNO') AS ""ProductManager"" ");
            sb.AppendLine($@"        , U_CODGRT AS ""ProductManagerCode"", ""SuppCatNum"" AS ""FactoryCode"", IFNULL(""frozenFor"", 'N') AS ""Blocked"", IFNULL(U_MARCA, '') AS ""Brand"", IFNULL(U_ROTACION, '') AS ""Rotation"" ");
            sb.AppendLine($@"FROM    {DBLA}.OITM ");
            sb.AppendLine($@"UNION ");
            sb.AppendLine($@"SELECT  ""ItemCode"" AS ""Code"", ""ItemName"" AS ""Name"", U_CATEGORIA AS ""Category"", U_SUBCATEG AS ""Subcategory"", U_LINEA AS ""Line"", IFNULL(""FrgnName"", '') AS ""Description"", CAST(IFNULL(""UserText"", '') AS varchar(5000)) AS ""Commentaries"" ");
            sb.AppendLine($@"        , U_GRTIAVENT AS ""Warranty"", ""PicturName"" AS ""Picture"", (SELECT TOP 1 CAST(IFNULL(""BitmapPath"", '') AS varchar(5000)) FROM {DBIQ}.OADP) AS ""FilePath"", IFNULL(U_GPRODUCT, 'NINGUNO') AS ""ProductManager"" ");
            sb.AppendLine($@"        , U_CODGRT AS ""ProductManagerCode"", ""SuppCatNum"" AS ""FactoryCode"", IFNULL(""frozenFor"", 'N') AS ""Blocked"", IFNULL(U_MARCA, '') AS ""Brand"", IFNULL(U_ROTACION, '') AS ""Rotation"" ");
            sb.AppendLine($@"FROM    {DBIQ}.OITM ");
            return sb.ToString();
        }

        private string GetCatalogSAQuery()
        {
            var sb = new StringBuilder();
            sb.AppendLine($@"SELECT  ""ItemCode"" AS ""Code"", ""CodeBars"", ""SuppCatNum"", ""ItemName"" AS ""Name"", U_CATEGORIA AS ""Category"", U_SUBCATEG AS ""Subcategory"", U_LINEA AS ""Line"", IFNULL(""FrgnName"", '') AS ""Description"", IFNULL(CAST(""UserText"" AS VARCHAR(8000)), '') AS ""Commentaries"" ");
            sb.AppendLine($@"        , U_GRTIAVENT AS ""Warranty"", ""PicturName"" AS ""Picture"", (SELECT TOP 1 CAST(IFNULL(""BitmapPath"", '') AS varchar(5000)) FROM {DBSA}.OADP) AS ""FilePath"", IFNULL(U_GPRODUCT, 'NINGUNO') AS ""ProductManager"", U_MARCA ");
            sb.AppendLine($@"FROM    {DBSA}.OITM ");
            sb.AppendLine($@"WHERE   LTRIM(RTRIM(IFNULL(""frozenFor"", ''))) <> 'Y' ");
            return sb.ToString();
        }

        #endregion

        #region Constructors

        public Product() : base() { }

        #endregion
    }
}
