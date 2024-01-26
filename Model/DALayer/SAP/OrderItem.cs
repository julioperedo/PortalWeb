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
    public class OrderItem : DALEntity<BEA.OrderItem>
    {

        #region Methods

        internal IEnumerable<BEA.OrderItem> ReturnChild(IEnumerable<string> Keys, params Enum[] Relations)
        {
            StringBuilder sb = new();
            sb.AppendLine($@"SELECT  a.""Id"" AS ""OrderId"", a.""ItemCode"", a.""ItemName"", a.""Quantity"", a.""OpenQuantity"", a.""DeliveredQuantity"", a.""Price"", a.""ItemTotal"", a.""Complete"", a.""Subsidiary"", a.""Warehouse"", a.""Stock"", a.""Margin"", a.""CalculedTotal"" ");
            sb.AppendLine($@"FROM    ({GetQuery()}) a ");
            sb.AppendLine($@"WHERE   (LOWER(a.""Subsidiary"") || '-' || CAST(a.""Id"" AS VARCHAR(10))) IN ( {string.Join(",", Keys.Select(x => $"'{x}'"))} ) ");
            IEnumerable<BEA.OrderItem> items = SQLList(sb.ToString(), Relations);
            return items;
        }

        protected override void LoadRelations(ref IEnumerable<BEA.OrderItem> Items, params Enum[] Relations)
        {
            IEnumerable<BEA.Product> lstProducts = null;

            IEnumerable<string> Keys;

            foreach (Enum RelationEnum in Relations)
            {
                Keys = from i in Items select i.ItemCode;
                if (RelationEnum.Equals(BEA.relOrderItem.Product))
                {
                    using Product dalProduct = new();
                    lstProducts = dalProduct.ReturnMasters(Keys, Relations);
                }
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var item in Items)
                {
                    if (lstProducts?.Count() > 0)
                    {
                        item.Product = lstProducts.FirstOrDefault(x => x.Code == item.ItemCode);
                    }
                }
            }
        }

        protected override void LoadRelations(ref BEA.OrderItem item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
                if (RelationEnum.Equals(BEA.relOrderItem.Product))
                {
                    using Product dalProduct = new();
                    item.Product = dalProduct.ReturnMaster(item.ItemCode, Relations);
                }
            }
        }

        #endregion

        #region List Methods

        public IEnumerable<BEA.OrderItem> List(List<Field> Filters, string Order, params Enum[] Relations)
        {
            string strFilter = Filters?.Count > 0 ? GetFilter(Filters?.ToArray()) : "";
            StringBuilder sb = new();
            sb.AppendLine($@"SELECT   a.""Id"" AS ""OrderId"", a.""DocNumber"" AS ""OrderNumber"", a.""ItemCode"", a.""ItemName"", a.""Quantity"", a.""OpenQuantity"", a.""DeliveredQuantity"", a.""Price"", a.""ItemTotal"", a.""Complete"", a.""Subsidiary"", a.""Warehouse"", a.""Stock"", a.""Margin"", a.""CalculedTotal"" AS ""TaxlessTotal"", a.""Line"", a.""BrandCode"" ");
            sb.AppendLine($@"FROM     ({GetQuery()}) a ");
            sb.AppendLine($@"WHERE    {strFilter} ");
            sb.AppendLine($@"ORDER By {GetOrder(Order)} ");

            IEnumerable<BEA.OrderItem> items = SQLList(sb.ToString(), Relations);
            return items;
        }

        public IEnumerable<BEA.OrderItem> List(IEnumerable<string> Keys, params Enum[] Relations)
        {
            StringBuilder sb = new();
            sb.AppendLine($@"SELECT  a.""Id"" AS ""OrderId"", a.""DocNumber"" AS ""OrderNumber"", a.""ItemCode"", a.""ItemName"", a.""Quantity"", a.""OpenQuantity"", a.""DeliveredQuantity"", a.""Price"", a.""ItemTotal"", a.""Complete"", a.""Subsidiary"", a.""Warehouse"", a.""Stock"", a.""Margin"", a.""CalculedTotal"" AS ""TaxlessTotal"", a.""Line"" ");
            sb.AppendLine($@"FROM    ({GetQuery()}) a ");
            sb.AppendLine($@"WHERE   (LOWER(a.""Subsidiary"") || '-' || CAST(a.""Id"" AS VARCHAR(10))) IN ( {string.Join(",", Keys.Select(x => $"'{x.Replace("'", "")}'"))} ) ");
            sb.AppendLine($@"ORDER BY ""LineNum"" ");

            IEnumerable<BEA.OrderItem> items = SQLList(sb.ToString(), Relations);
            return items;
        }

        public IEnumerable<BEA.ReservedItem> ListReserved(string Subsidiary, string Warehouse, string ItemCode)
        {
            var sb = new StringBuilder();
            sb.AppendLine($@"SELECT  * ");
            sb.AppendLine($@"FROM    (   SELECT  'Santa Cruz' AS ""Subsidiary"", t3.""WhsName"" AS ""Warehouse"", t2.""SlpName"" AS ""SellerCode"", t2.""Memo"" AS ""SellerName"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"" ");
            sb.AppendLine($@"                    , t0.""DocEntry"", t0.""DocNum"", t0.""DocDate"", t1.""ItemCode"", CAST(t1.""OpenQty"" AS INT) AS ""Quantity"", t1.""PriceAfVAT"" AS ""Price"", t0.""Confirmed"" AS ""Authorized"" ");
            sb.AppendLine($@"                    , t0.U_CORRELATIVO AS ""Correlative"" ");
            sb.AppendLine($@"            FROM    {DBSA}.ORDR t0 ");
            sb.AppendLine($@"                    INNER JOIN {DBSA}.RDR1 t1 ON t1.""DocEntry"" = t0.""DocEntry"" ");
            sb.AppendLine($@"                    INNER JOIN {DBSA}.OSLP t2 ON t2.""SlpCode"" = t0.""SlpCode"" ");
            sb.AppendLine($@"                    INNER JOIN {DBSA}.OWHS t3 ON t3.""WhsCode"" = t1.""WhsCode"" ");
            sb.AppendLine($@"            WHERE   CANCELED = 'N' ");
            sb.AppendLine($@"                    AND t0.""DocStatus"" = 'O' ");
            sb.AppendLine($@"                    AND t1.""OpenQty"" > 0 ");
            sb.AppendLine($@"            UNION ALL ");
            sb.AppendLine($@"            SELECT  'Iquique' AS ""Subsidiary"", t3.""WhsName"" AS ""Warehouse"", t2.""SlpName"" AS ""SellerCode"", t2.""Memo"" AS ""SellerName"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"" ");
            sb.AppendLine($@"                    , t0.""DocEntry"", t0.""DocNum"", t0.""DocDate"", t1.""ItemCode"", CAST(t1.""OpenQty"" AS INT) AS ""Quantity"", t1.""PriceAfVAT"" AS ""Price"", t0.""Confirmed"" ");
            sb.AppendLine($@"                    , t0.U_CORRELATIVO AS ""Correlative"" ");
            sb.AppendLine($@"            FROM    {DBIQ}.ORDR t0 ");
            sb.AppendLine($@"                    INNER JOIN {DBIQ}.RDR1 t1 ON t1.""DocEntry"" = t0.""DocEntry"" ");
            sb.AppendLine($@"                    INNER JOIN {DBIQ}.OSLP t2 ON t2.""SlpCode"" = t0.""SlpCode"" ");
            sb.AppendLine($@"                    INNER JOIN {DBIQ}.OWHS t3 ON t3.""WhsCode"" = t1.""WhsCode"" ");
            sb.AppendLine($@"            WHERE   CANCELED = 'N' ");
            sb.AppendLine($@"                    AND t0.""DocStatus"" = 'O' ");
            sb.AppendLine($@"                    AND t1.""OpenQty"" > 0 ");
            sb.AppendLine($@"            UNION ALL ");
            sb.AppendLine($@"            SELECT  'Miami' AS ""Subsidiary"", t3.""WhsName"" AS ""Warehouse"", t2.""SlpName"" AS ""SellerCode"", t2.""Memo"" AS ""SellerName"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"" ");
            sb.AppendLine($@"                    , t0.""DocEntry"", t0.""DocNum"", t0.""DocDate"", t1.""ItemCode"", CAST(t1.""OpenQty"" AS INT) AS ""Quantity"", t1.""PriceAfVAT"" AS ""Price"", t0.""Confirmed"" ");
            sb.AppendLine($@"                    , t0.U_CORRELATIVO AS ""Correlative"" ");
            sb.AppendLine($@"            FROM    {DBLA}.ORDR t0 ");
            sb.AppendLine($@"                    INNER JOIN {DBLA}.RDR1 t1 ON t1.""DocEntry"" = t0.""DocEntry"" ");
            sb.AppendLine($@"                    INNER JOIN {DBLA}.OSLP t2 ON t2.""SlpCode"" = t0.""SlpCode"" ");
            sb.AppendLine($@"                    INNER JOIN {DBLA}.OWHS t3 ON t3.""WhsCode"" = t1.""WhsCode"" ");
            sb.AppendLine($@"            WHERE   CANCELED = 'N' ");
            sb.AppendLine($@"                    AND t0.""DocStatus"" = 'O' ");
            sb.AppendLine($@"                    AND t1.""OpenQty"" > 0 ) a ");
            sb.AppendLine($@"WHERE   LOWER(""ItemCode"") = '{ItemCode.ToLower()}' AND LOWER(""Subsidiary"") = '{Subsidiary.ToLower()}' AND LOWER(""Warehouse"") = '{Warehouse.ToLower()}' ");
            IEnumerable<BEA.ReservedItem> items = SQLList<BEA.ReservedItem>(sb.ToString());
            return items;
        }

        #endregion

        #region Search Methods

        #endregion

        #region Private Methods

        private string GetQuery()
        {
            var sb = new StringBuilder();
            sb.AppendLine($@"SELECT  t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", 'Santa Cruz' AS ""Subsidiary"", t1.""WhsName"" AS ""Warehouse"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"", t7.U_LINEA AS ""Line"" ");
            sb.AppendLine($@"        , t0.""NumAtCard"" AS ""ClientOrder"", T3.""SlpName"" AS ""SellerCode"", T3.""Memo"" AS ""SellerName"", t0.""DocTotal"" AS ""Total"", ( CASE t0.""DocStatus"" WHEN 'O' THEN 'Abierto' ELSE 'Cerrado' END ) AS ""State"", t0.""Confirmed"" AS ""Authorized"" ");
            sb.AppendLine($@"        , T5.""PymntGroup"" AS ""TermConditions"", t4.""ItemCode"", t4.""Dscription"" AS ""ItemName"", CAST(t4.""Quantity"" AS int) AS ""Quantity"", CAST(t4.""OpenQty"" AS int) AS ""OpenQuantity"", CAST(t4.""DelivrdQty"" AS int) AS ""DeliveredQuantity"", t4.""PriceAfVAT"" AS ""Price"" ");
            sb.AppendLine($@"        , t4.""PriceAfVAT"" * t4.""Quantity"" AS ""ItemTotal"", CAST(( CASE WHEN IFNULL(t6.""OnHand"", 0) >= t4.""OpenQty"" THEN 1 ELSE 0 END ) AS tinyint) AS ""Complete"", CAST(IFNULL(t6.""OnHand"", 0) AS int) AS ""Stock"" ");
            sb.AppendLine($@"        , CASE WHEN t1.""WhsCode"" = 'ZFSCZ' THEN t4.""Quantity"" * ( t4.""PriceAfVAT"" - t4.""StockPrice"" ) ELSE t4.""Quantity"" * (( 0.84 * t4.""PriceAfVAT"" ) - t4.""StockPrice"" ) END AS ""Margin"" ");
            sb.AppendLine($@"        , CASE WHEN t1.""WhsCode"" = 'ZFSCZ' THEN t4.""PriceAfVAT"" * t4.""Quantity"" ELSE t4.""PriceAfVAT"" * t4.""Quantity"" * 0.84 END AS ""CalculedTotal"", t7.""SuppCatNum"" AS ""BrandCode"", t4.""LineNum"" ");
            sb.AppendLine($@"FROM    {DBSA}.ORDR t0 ");
            sb.AppendLine($@"        INNER JOIN {DBSA}.OSLP T3 ON t0.""SlpCode"" = T3.""SlpCode"" ");
            sb.AppendLine($@"        INNER JOIN {DBSA}.OCTG T5 ON t0.""GroupNum"" = T5.""GroupNum"" ");
            sb.AppendLine($@"        INNER JOIN {DBSA}.RDR1 t4 ON t4.""DocEntry"" = t0.""DocEntry"" ");
            sb.AppendLine($@"        INNER JOIN {DBSA}.OWHS t1 ON t1.""WhsCode"" = t4.""WhsCode"" ");
            sb.AppendLine($@"        INNER JOIN {DBSA}.OITM t7 ON t7.""ItemCode"" = t4.""ItemCode"" ");
            sb.AppendLine($@"        LEFT OUTER JOIN {DBSA}.OITW t6 ON t6.""ItemCode"" = t4.""ItemCode"" AND t6.""WhsCode"" = t4.""WhsCode"" AND t6.""ItemCode"" <> 'FLETES' AND  t6.""ItemCode"" NOT LIKE '%DMC%' ");
            sb.AppendLine($@"WHERE   t0.CANCELED = 'N' ");
            sb.AppendLine($@"UNION ");
            sb.AppendLine($@"SELECT  t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", 'Miami' AS ""Subsidiary"", t1.""WhsName"" AS ""Warehouse"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"", t7.U_LINEA AS ""Line"" ");
            sb.AppendLine($@"        , t0.""NumAtCard"" AS ""ClientOrder"", T3.""SlpName"" AS ""SellerCode"", T3.""Memo"" AS ""SellerName"", t0.""DocTotal"" AS ""Total"", ( CASE t0.""DocStatus"" WHEN 'O' THEN 'Abierto' ELSE 'Cerrado' END ) AS ""State"", t0.""Confirmed"" AS ""Authorized"" ");
            sb.AppendLine($@"        , T5.""PymntGroup"" AS ""TermConditions"", t4.""ItemCode"", t4.""Dscription"" AS ""ItemName"", CAST(t4.""Quantity"" AS int), CAST(t4.""OpenQty"" AS int) AS ""OpenQuantity"", CAST(t4.""DelivrdQty"" AS int) AS ""DeliveredQuantity"", t4.""PriceAfVAT"" AS ""Price"" ");
            sb.AppendLine($@"        , t4.""PriceAfVAT"" * t4.""Quantity"" AS ""ItemTotal"", CAST(( CASE WHEN IFNULL(t6.""OnHand"", 0) >= t4.""OpenQty"" THEN 1 ELSE 0 END ) AS tinyint) AS ""Complete"", CAST(IFNULL(t6.""OnHand"", 0) AS int) AS ""Stock"" ");
            sb.AppendLine($@"        , t4.""Quantity"" * ( t4.""PriceAfVAT"" - t4.""StockPrice"" ) AS ""Margin"", t4.""PriceAfVAT"" * t4.""Quantity"" AS ""CalculedTotal"", t7.""SuppCatNum"" AS ""BrandCode"", t4.""LineNum"" ");
            sb.AppendLine($@"FROM    {DBLA}.ORDR t0 ");
            sb.AppendLine($@"        INNER JOIN {DBLA}.OSLP T3 ON t0.""SlpCode"" = T3.""SlpCode"" ");
            sb.AppendLine($@"        INNER JOIN {DBLA}.OCTG T5 ON t0.""GroupNum"" = T5.""GroupNum"" ");
            sb.AppendLine($@"        INNER JOIN {DBLA}.RDR1 t4 ON t4.""DocEntry"" = t0.""DocEntry"" ");
            sb.AppendLine($@"        INNER JOIN {DBLA}.OWHS t1 ON t1.""WhsCode"" = t4.""WhsCode"" ");
            sb.AppendLine($@"        INNER JOIN {DBLA}.OITM t7 ON t7.""ItemCode"" = t4.""ItemCode"" ");
            sb.AppendLine($@"        LEFT OUTER JOIN {DBLA}.OITW t6 ON t6.""ItemCode"" = t4.""ItemCode"" AND  t6.""WhsCode"" = t4.""WhsCode"" AND t6.""ItemCode"" <> 'FLETES' AND   t6.""ItemCode"" NOT LIKE '%DMC%' ");
            sb.AppendLine($@"WHERE   t0.CANCELED = 'N' ");
            sb.AppendLine($@"UNION ");
            sb.AppendLine($@"SELECT  t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", 'Iquique' AS ""Subsidiary"", t1.""WhsName"" AS ""Warehouse"", CAST(t0.""DocDate"" AS DATE) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"", t7.U_LINEA AS ""Line"" ");
            sb.AppendLine($@"        , t0.""NumAtCard"" AS ""ClientOrder"", T3.""SlpName"" AS ""SellerCode"", T3.""Memo"" AS ""SellerName"", t0.""DocTotal"" AS ""Total"", ( CASE t0.""DocStatus"" WHEN 'O' THEN 'Abierto' ELSE 'Cerrado' END ) AS ""State"", t0.""Confirmed"" AS ""Authorized"" ");
            sb.AppendLine($@"        , T5.""PymntGroup"" AS ""TermConditions"", t4.""ItemCode"", t4.""Dscription"" AS ""ItemName"", CAST(t4.""Quantity"" AS int), CAST(t4.""OpenQty"" AS int) AS ""OpenQuantity"", CAST(t4.""DelivrdQty"" AS int) AS ""DeliveredQuantity"", t4.""PriceAfVAT"" AS ""Price"" ");
            sb.AppendLine($@"        , t4.""PriceAfVAT"" * t4.""Quantity"" AS ""ItemTotal"", CAST(( CASE WHEN IFNULL(t6.""OnHand"", 0) >= t4.""OpenQty"" THEN 1 ELSE 0 END ) AS tinyint) AS ""Complete"", CAST(IFNULL(t6.""OnHand"", 0) AS int) AS ""Stock"" ");
            sb.AppendLine($@"        , t4.""Quantity"" * ( t4.""PriceAfVAT"" - t4.""StockPrice"" ) AS ""Margin"", t4.""PriceAfVAT"" * t4.""Quantity"" AS ""CalculedTotal"", t7.""SuppCatNum"" AS ""BrandCode"", t4.""LineNum"" ");
            sb.AppendLine($@"FROM    {DBIQ}.ORDR t0 ");
            sb.AppendLine($@"        INNER JOIN {DBIQ}.OSLP T3 ON t0.""SlpCode"" = T3.""SlpCode"" ");
            sb.AppendLine($@"        INNER JOIN {DBIQ}.OCTG T5 ON t0.""GroupNum"" = T5.""GroupNum"" ");
            sb.AppendLine($@"        INNER JOIN {DBIQ}.RDR1 t4 ON t4.""DocEntry"" = t0.""DocEntry"" ");
            sb.AppendLine($@"        INNER JOIN {DBIQ}.OWHS t1 ON t1.""WhsCode"" = t4.""WhsCode"" ");
            sb.AppendLine($@"        INNER JOIN {DBIQ}.OITM t7 ON t7.""ItemCode"" = t4.""ItemCode"" ");
            sb.AppendLine($@"        LEFT OUTER JOIN {DBIQ}.OITW t6 ON t6.""ItemCode"" = t4.""ItemCode"" AND   t6.""WhsCode"" = t4.""WhsCode"" AND t6.""ItemCode"" <> 'FLETES' AND   t6.""ItemCode"" NOT LIKE '%DMC%' ");
            sb.AppendLine($@"WHERE   t0.CANCELED = 'N' ");

            return sb.ToString();
        }

        #endregion

        #region Constructors

        public OrderItem() : base() { }

        #endregion
    }
}
