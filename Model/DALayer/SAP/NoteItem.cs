using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using BE = BEntities;
using BEA = BEntities.SAP;

namespace DALayer.SAP.Hana
{
    [Serializable()]
    public class NoteItem : DALEntity<BEA.NoteItem>
    {

        #region Methods

        internal IEnumerable<BEA.NoteItem> ReturnChild(IEnumerable<string> Keys, params Enum[] Relations)
        {
            StringBuilder sb = new();
            sb.AppendLine($@"SELECT  a.""Id"" AS ""NoteId"", a.""ItemCode"", a.""ItemName"", a.""Quantity"", a.""OpenQuantity"", a.""DeliveredQuantity"", a.""Price"", a.""ItemTotal"", a.""Complete"", a.""Subsidiary"", a.""Warehouse"", a.""Margin"", a.""CalculedTotal"", a.""Stock"" ");
            sb.AppendLine($@"FROM    ({GetQuery()}) a ");
            sb.AppendLine($@"WHERE   (LOWER(a.""Subsidiary"") || '-' || CAST(a.""Id"" AS VARCHAR(10))) IN ( {string.Join(",", Keys.Select(x => $"'{x}'"))} ) ");
            IEnumerable<BEA.NoteItem> items = SQLList(sb.ToString(), Relations);
            return items;
        }

        protected override void LoadRelations(ref IEnumerable<BEA.NoteItem> Items, params Enum[] Relations)
        {
            IEnumerable<BEA.Product> lstProducts = null;
            IEnumerable<string> Keys;

            foreach (Enum RelationEnum in Relations)
            {
                Keys = from i in Items select i.ItemCode;
                if (RelationEnum.Equals(BEA.relNoteItem.Product))
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

        protected override void LoadRelations(ref BEA.NoteItem item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
                if (RelationEnum.Equals(BEA.relNoteItem.Product))
                {
                    using Product dalProduct = new();
                    item.Product = dalProduct.ReturnMaster(item.ItemCode, Relations);
                }
            }
        }

        #endregion

        #region List Methods

        public IEnumerable<BEA.NoteItem> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new();
            string strFilter = "";
            if (FilterList?.Count > 0)
            {
                strFilter = GetFilter(FilterList?.ToArray());
            }
            sbQuery.AppendLine($@"SELECT  a.""Id"" AS ""NoteId"", a.""ItemCode"", a.""ItemName"", a.""Quantity"", a.""OpenQuantity"", a.""DeliveredQuantity"", a.""Price"", a.""ItemTotal"", a.""Complete"", a.""Subsidiary"", a.""Warehouse"", a.""Margin"", a.""CalculedTotal"", CAST(a.""Stock"" AS INT) AS ""Stock"", a.""BrandCode"", a.""Unit"", a.""Line"" ");
            sbQuery.AppendLine($@"FROM    ({GetQuery()}) a ");
            if (strFilter != "") sbQuery.AppendLine($@"WHERE   {strFilter} ");
            sbQuery.AppendLine($@"ORDER By {GetOrder(Order)} ");

            IEnumerable<BEA.NoteItem> items = SQLList(sbQuery.ToString(), Relations);
            return items;
        }

        public IEnumerable<BEA.NoteItem> List(IEnumerable<string> Keys, params Enum[] Relations)
        {
            StringBuilder sb = new();
            sb.AppendLine($@"SELECT  a.""Id"" AS ""NoteId"", a.""ItemCode"", a.""ItemName"", a.""Quantity"", a.""OpenQuantity"", a.""DeliveredQuantity"", a.""Price"", a.""ItemTotal"", a.""Complete"", a.""Subsidiary"", a.""Warehouse"", a.""Margin"", a.""CalculedTotal"", CAST(a.""Stock"" AS INT) AS ""Stock"", a.""BrandCode"", a.""Unit"", a.""Warranty"" ");
            sb.AppendLine($@"FROM    ({GetQuery()}) a ");
            sb.AppendLine($@"WHERE   (LOWER(a.""Subsidiary"") || '-' || CAST(a.""Id"" AS VARCHAR(10))) IN ( {string.Join(",", Keys.Select(x => $"'{x.Replace("'", "")}'"))} ) ");
            sb.AppendLine($@"ORDER BY ""LineNum"" ");

            IEnumerable<BEA.NoteItem> items = SQLList(sb.ToString(), Relations);
            return items;
        }

        public IEnumerable<BEA.BillItem> ListBillItems(int NoteNumber)
        {
            var sb = new StringBuilder();
            sb.AppendLine($@"SELECT  t1.""ItemCode"", t1.""Dscription"" AS ""ItemName"", t1.""VendorNum"" AS ""ExternalCode"", CAST(t1.""Quantity"" AS INT) AS ""Quantity"", t1.""PriceAfVAT"" AS ""Price"", t1.""GTotal"" AS ""Subtotal"" ");
            sb.AppendLine($@"FROM    {DBSA}.OINV t0 ");
            sb.AppendLine($@"        INNER JOIN {DBSA}.INV1 t1 ON t0.""DocEntry"" = t1.""DocEntry"" ");
            sb.AppendLine($@"WHERE   t0.""DocNum"" = {NoteNumber} ");
            sb.AppendLine($@"ORDER BY t1.""LineNum"" ");

            IEnumerable<BEA.BillItem> items = SQLList<BEA.BillItem>(sb.ToString());
            return items;
        }

        public IEnumerable<BEA.BillItem> ListElectronicBillItems(int NoteNumber)
        {
            string query = 
$@"SELECT  t1.""ItemCode"", t1.""Dscription"" AS ""ItemName"", t1.""VendorNum"" AS ""ExternalCode"", CAST(t1.""Quantity"" AS INT) AS ""Quantity"", t1.""PriceAfVAT"" AS ""Price"", t1.""GTotalSC"" / t1.""Quantity"" AS ""PriceLocal""
        , t1.""GTotal"" AS ""Subtotal"", ROUND((t1.""GTotalSC"" / t1.""Quantity""), 2) * t1.""Quantity"" AS ""SubtotalLocal"", t3.""Name"" AS ""Unit"", t2.""FrgnName"" AS ""Description""
FROM    {DBSA}.OINV t0 
        INNER JOIN {DBSA}.INV1 t1 ON t0.""DocEntry"" = t1.""DocEntry"" 
        INNER JOIN {DBSA}.OITM t2 ON t1.""ItemCode"" = t2.""ItemCode""
        LEFT OUTER JOIN {DBSA}.""@B_UNIMED"" t3 ON t2.""U_B_unimed"" = t3.""Code""
WHERE   t0.""DocNum"" = {NoteNumber} 
ORDER BY t1.""LineNum"" ";

            IEnumerable<BEA.BillItem> items = SQLList<BEA.BillItem>(query);
            return items;
        }

        #endregion

        #region Private Methods

        private string GetQuery()
        {
            var sb = new StringBuilder();
            sb.AppendLine($@"SELECT  t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", 'Santa Cruz' AS ""Subsidiary"", t1.""WhsName"" AS ""Warehouse"", CAST(t0.""DocDate"" AS date) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"", t7.U_LINEA AS ""Line"" ");
            sb.AppendLine($@"        , t0.""NumAtCard"" AS ""ClientOrder"", t2.""SlpName"" AS ""SellerCode"", t2.""Memo"" AS ""SellerName"", t0.""DocTotal"" AS ""Total"", (CASE t0.""DocStatus"" WHEN 'O' THEN 'Abierto' ELSE 'Cerrado' END) AS ""State"" ");
            sb.AppendLine($@"        , t3.""PymntGroup"" AS ""TermConditions"", t4.""ItemCode"", t4.""Dscription"" AS ""ItemName"", CAST(t4.""Quantity"" AS integer) AS ""Quantity"", CAST(t4.""OpenQty"" AS integer) AS ""OpenQuantity"" ");
            sb.AppendLine($@"        , CAST(t4.""DelivrdQty"" AS integer) AS ""DeliveredQuantity"", t4.""PriceAfVAT"" AS ""Price"", t4.""PriceAfVAT"" * t4.""Quantity"" AS ""ItemTotal"", CAST((CASE WHEN IFNULL(t5.""OnHand"", 0) >= t4.""OpenQty"" THEN 1 ELSE 0 END) AS tinyint) AS ""Complete"" ");
            sb.AppendLine($@"        , IFNULL(t5.""OnHand"", 0) AS ""Stock"", CASE WHEN t1.""WhsCode"" = 'ZFSCZ' THEN t4.""Quantity"" * (t4.""PriceAfVAT"" - t4.""StockPrice"") ELSE t4.""Quantity"" * ((0.84 * t4.""PriceAfVAT"") - t4.""StockPrice"") END AS ""Margin"" ");
            sb.AppendLine($@"        , CASE WHEN t1.""WhsCode"" = 'ZFSCZ' THEN t4.""PriceAfVAT"" * t4.""Quantity"" ELSE t4.""PriceAfVAT"" * t4.""Quantity"" * 0.84 END AS ""CalculedTotal"", t7.""SuppCatNum"" AS ""BrandCode"", t4.""unitMsr"" AS ""Unit"", t4.U_TPOGTA AS ""Warranty"", t4.""LineNum"" ");
            sb.AppendLine($@"FROM    {DBSA}.OINV t0 ");
            sb.AppendLine($@"        INNER JOIN {DBSA}.OSLP t2 ON t0.""SlpCode"" = t2.""SlpCode"" ");
            sb.AppendLine($@"        INNER JOIN {DBSA}.OCTG t3 ON t0.""GroupNum"" = t3.""GroupNum"" ");
            sb.AppendLine($@"        INNER JOIN {DBSA}.INV1 t4 ON t4.""DocEntry"" = t0.""DocEntry"" ");
            sb.AppendLine($@"        INNER JOIN {DBSA}.OWHS t1 ON t1.""WhsCode"" = t4.""WhsCode"" ");
            sb.AppendLine($@"        INNER JOIN {DBSA}.OITM t7 ON t7.""ItemCode"" = t4.""ItemCode"" ");
            sb.AppendLine($@"        LEFT OUTER JOIN {DBSA}.OITW t5 ON t5.""ItemCode"" = t4.""ItemCode"" AND t5.""WhsCode"" = t4.""WhsCode"" AND t5.""ItemCode"" <> 'FLETES' AND t5.""ItemCode"" NOT LIKE '%DMC%' ");
            sb.AppendLine($@"WHERE   t0.CANCELED = 'N' AND t4.""TargetType"" <> 14 ");
            sb.AppendLine($@"UNION ");
            sb.AppendLine($@"SELECT  t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", 'Miami' AS ""Subsidiary"", t1.""WhsName"" AS ""Warehouse"", CAST(t0.""DocDate"" AS date) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"", t7.U_LINEA AS ""Line"" ");
            sb.AppendLine($@"        , t0.""NumAtCard"" AS ""ClientOrder"", t2.""SlpName"" AS ""SellerCode"", t2.""Memo"" AS ""SellerName"", t0.""DocTotal"" AS ""Total"", (CASE t0.""DocStatus"" WHEN 'O' THEN 'Abierto' ELSE 'Cerrado' END) AS ""State"" ");
            sb.AppendLine($@"        , t3.""PymntGroup"" AS ""TermConditions"", t4.""ItemCode"", t4.""Dscription"" AS ""ItemName"", CAST(t4.""Quantity"" AS integer) AS ""Quantity"", CAST(t4.""OpenQty"" AS integer) AS ""OpenQuantity"" ");
            sb.AppendLine($@"        , CAST(t4.""DelivrdQty"" AS integer) AS ""DeliveredQuantity"", t4.""PriceAfVAT"" AS ""Price"", t4.""PriceAfVAT"" * t4.""Quantity"" AS ""ItemTotal"", CAST((CASE WHEN IFNULL(t5.""OnHand"", 0) >= t4.""OpenQty"" THEN 1 ELSE 0 END) AS tinyint) AS ""Complete"" ");
            sb.AppendLine($@"        , IFNULL(t5.""OnHand"", 0) AS ""Stock"", t4.""Quantity"" * (t4.""PriceAfVAT"" - t4.""StockPrice"") AS ""Margin"", t4.""PriceAfVAT"" * t4.""Quantity"" AS ""CalculedTotal"", t7.""SuppCatNum"" AS ""BrandCode"", t4.""unitMsr"" AS ""Unit"", t4.U_TPOGTA AS ""Warranty"", t4.""LineNum"" ");
            sb.AppendLine($@"FROM    {DBLA}.OINV t0 ");
            sb.AppendLine($@"        INNER JOIN {DBLA}.OSLP t2 ON t0.""SlpCode"" = t2.""SlpCode"" ");
            sb.AppendLine($@"        INNER JOIN {DBLA}.OCTG t3 ON t0.""GroupNum"" = t3.""GroupNum"" ");
            sb.AppendLine($@"        INNER JOIN {DBLA}.INV1 t4 ON t4.""DocEntry"" = t0.""DocEntry"" ");
            sb.AppendLine($@"        INNER JOIN {DBLA}.OWHS t1 ON t1.""WhsCode"" = t4.""WhsCode"" ");
            sb.AppendLine($@"        INNER JOIN {DBLA}.OITM t7 ON t7.""ItemCode"" = t4.""ItemCode"" ");
            sb.AppendLine($@"        LEFT OUTER JOIN {DBLA}.OITW t5 ON t5.""ItemCode"" = t4.""ItemCode"" AND t5.""WhsCode"" = t4.""WhsCode"" AND t5.""ItemCode"" <> 'FLETES' AND t5.""ItemCode"" NOT LIKE '%DMC%' ");
            sb.AppendLine($@"WHERE   t0.CANCELED = 'N' AND t4.""TargetType"" <> 14 ");
            sb.AppendLine($@"UNION ");
            sb.AppendLine($@"SELECT  t0.""DocEntry"" AS ""Id"", t0.""DocNum"" AS ""DocNumber"", 'Iquique' AS ""Subsidiary"", t1.""WhsName"" AS ""Warehouse"", CAST(t0.""DocDate"" AS date) AS ""DocDate"", t0.""CardCode"" AS ""ClientCode"", t0.""CardName"" AS ""ClientName"", t7.U_LINEA AS ""Line"" ");
            sb.AppendLine($@"        , t0.""NumAtCard"" AS ""ClientOrder"", t2.""SlpName"" AS ""SellerCode"", t2.""Memo"" AS ""SellerName"", t0.""DocTotal"" AS ""Total"", (CASE t0.""DocStatus"" WHEN 'O' THEN 'Abierto' ELSE 'Cerrado' END) AS ""State"" ");
            sb.AppendLine($@"        , t3.""PymntGroup"" AS ""TermConditions"", t4.""ItemCode"", t4.""Dscription"" AS ""ItemName"", CAST(t4.""Quantity"" AS integer) AS ""Quantity"", CAST(t4.""OpenQty"" AS integer) AS ""OpenQuantity"" ");
            sb.AppendLine($@"        , CAST(t4.""DelivrdQty"" AS integer) AS ""DeliveredQuantity"", t4.""PriceAfVAT"" AS ""Price"", t4.""PriceAfVAT"" * t4.""Quantity"" AS ""ItemTotal"", CAST((CASE WHEN IFNULL(t5.""OnHand"", 0) >= t4.""OpenQty"" THEN 1 ELSE 0 END) AS tinyint) AS ""Complete"" ");
            sb.AppendLine($@"        , IFNULL(t5.""OnHand"", 0) AS ""Stock"", t4.""Quantity"" * (t4.""PriceAfVAT"" - t4.""StockPrice"") AS ""Margin"", t4.""PriceAfVAT"" * t4.""Quantity"" AS ""CalculedTotal"", t7.""SuppCatNum"" AS ""BrandCode"", t4.""unitMsr"" AS ""Unit"", t4.U_TPOGTA AS ""Warranty"", t4.""LineNum"" ");
            sb.AppendLine($@"FROM    {DBIQ}.OINV t0 ");
            sb.AppendLine($@"        INNER JOIN {DBIQ}.OSLP t2 ON t0.""SlpCode"" = t2.""SlpCode"" ");
            sb.AppendLine($@"        INNER JOIN {DBIQ}.OCTG t3 ON t0.""GroupNum"" = t3.""GroupNum"" ");
            sb.AppendLine($@"        INNER JOIN {DBIQ}.INV1 t4 ON t4.""DocEntry"" = t0.""DocEntry"" ");
            sb.AppendLine($@"        INNER JOIN {DBIQ}.OWHS t1 ON t1.""WhsCode"" = t4.""WhsCode"" ");
            sb.AppendLine($@"        INNER JOIN {DBIQ}.OITM t7 ON t7.""ItemCode"" = t4.""ItemCode"" ");
            sb.AppendLine($@"        LEFT OUTER JOIN {DBIQ}.OITW t5 ON t5.""ItemCode"" = t4.""ItemCode"" AND t5.""WhsCode"" = t4.""WhsCode"" AND t5.""ItemCode"" <> 'FLETES' AND t5.""ItemCode"" NOT LIKE '%DMC%' ");
            sb.AppendLine($@"WHERE   t0.CANCELED = 'N' AND t4.""TargetType"" <> 14 ");
            return sb.ToString();
        }

        #endregion

        #region Constructors

        public NoteItem() : base() { }

        #endregion
    }
}