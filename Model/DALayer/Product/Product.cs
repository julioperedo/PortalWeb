using BEntities.Filters;
using DALayer.Base;

using DALayer.Security;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using BE = BEntities;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BEP = BEntities.Product;
using BES = BEntities.Security;

namespace DALayer.Product
{
    public partial class Product
    {

        #region Save Methods 

        public void UpdateCost(BEP.Product Item)
        {
            string query = "UPDATE [Product].[Product] SET [Cost] = @Cost, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            Connection.Execute(query, Item);
        }

        public async Task SaveAsync(BEP.Product Item)
        {
            string query = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                query = "INSERT INTO [Product].[Product]([Id], [Name], [Description], [Commentaries], [ItemCode], [Link], [Warranty], [Consumables], [ExtraComments], [Enabled], [ShowAlways], [ShowInWeb], [ShowTeamOnly], [ImageURL], [Line], [Category], [SubCategory], [Brand], [BrandCode], [Editable], [EnabledDate], [IsDigital], [Cost], [LogUser], [LogDate]) VALUES(@Id, @Name, @Description, @Commentaries, @ItemCode, @Link, @Warranty, @Consumables, @ExtraComments, @Enabled, @ShowAlways, @ShowInWeb, @ShowTeamOnly, @ImageURL, @Line, @Category, @SubCategory, @Brand, @BrandCode, @Editable, @EnabledDate, @IsDigital, @Cost, @LogUser, @LogDate)";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                query = "UPDATE [Product].[Product] SET [Name] = @Name, [Description] = @Description, [Commentaries] = @Commentaries, [ItemCode] = @ItemCode, [Link] = @Link, [Warranty] = @Warranty, [Consumables] = @Consumables, [ExtraComments] = @ExtraComments, [Enabled] = @Enabled, [ShowAlways] = @ShowAlways, [ShowInWeb] = @ShowInWeb, [ShowTeamOnly] = @ShowTeamOnly, [ImageURL] = @ImageURL, [Line] = @Line, [Category] = @Category, [SubCategory] = @SubCategory, [Brand] = @Brand, [BrandCode] = @BrandCode, [Editable] = @Editable, [EnabledDate] = @EnabledDate, [IsDigital] = @IsDigital, [Cost] = @Cost, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                query = "DELETE FROM [Product].[Product] WHERE [Id] = @Id";
            }

            if (Item.StatusType != BE.StatusType.NoAction)
            {
                if (Item.StatusType == BE.StatusType.Insert & Item.Id <= 0) Item.Id = GenID("Product", 1);
                await Connection.ExecuteAsync(query, Item);
            }            
        }

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        public IEnumerable<BEP.Product> ListWithPrices(long? IdLine, string Name, string CategoryId, string SubcategoryId, string SortBy, params Enum[] Relations)
        {
            StringBuilder sbQuery = new();

            sbQuery.AppendLine("SELECT  Id, Name, Description, Commentaries, ItemCode, Link, Warranty, Consumables, Enabled, ShowAlways, ShowInWeb, ImageURL, Category, SubCategory, ");
            sbQuery.AppendLine("        Editable, EnabledDate, LogUser, LogDate, ( SELECT Name FROM Product.Line WHERE Id = ( SELECT IdLine FROM Product.LineDetail WHERE SAPLine = p.Line ) ) AS Line, Cost ");
            sbQuery.AppendLine("FROM    Product.Product p ");
            sbQuery.AppendLine("WHERE   p.Enabled = 1 AND ( p.ShowAlways = 1 OR EXISTS ( SELECT * FROM Product.Price WHERE IdProduct = p.Id AND Regular > 0 ) ) ");
            if (IdLine.HasValue && IdLine.Value > 0)
            {
                sbQuery.AppendLine($"        AND ISNULL(Line, '') IN ( SELECT SAPLine FROM Product.LineDetail WHERE IdLine = {IdLine} ) ");
            }
            if (!string.IsNullOrWhiteSpace(CategoryId))
            {
                sbQuery.AppendLine($"        AND LOWER(ISNULL(p.Category, '')) = '{CategoryId.ToLower()}' ");
            }
            if (!string.IsNullOrWhiteSpace(SubcategoryId))
            {
                sbQuery.AppendLine($"        AND LOWER(ISNULL(p.SubCategory, '')) = '{SubcategoryId.ToLower()}' ");
            }
            if (!string.IsNullOrWhiteSpace(Name))
            {
                sbQuery.AppendLine($"        AND ( p.Name LIKE '%{Name}%' OR p.Description LIKE '%{Name}%' OR p.ItemCode LIKE '%{Name}%' OR  ISNULL(p.Category, '') LIKE '%{Name}%' OR ISNULL(p.SubCategory, '') LIKE '%{Name}%' ) ");
            }
            sbQuery.AppendLine("ORDER BY " + SortBy);

            IEnumerable<BEP.Product> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        public async Task<IEnumerable<BEP.Product>> ListWithPricesAsync(string LineIds, string SortBy, params Enum[] Relations)
        {
            string query = $@"SELECT  Id, Name, Description, Commentaries, ItemCode, Link, Warranty, Consumables, Enabled, ShowAlways, ShowInWeb, ImageURL, Category, SubCategory, 
        Editable, EnabledDate, LogUser, LogDate, ( SELECT Name FROM Product.Line WHERE Id = ( SELECT IdLine FROM Product.LineDetail WHERE SAPLine = p.Line ) ) AS LineName, Cost 
FROM    Product.Product p 
WHERE   p.Enabled = 1 AND ( p.ShowAlways = 1 OR EXISTS ( SELECT * FROM Product.Price WHERE IdProduct = p.Id AND Regular > 0 ) ) 
        AND ISNULL(Line, '') IN ( SELECT SAPLine FROM Product.LineDetail WHERE IdLine IN ( {(string.IsNullOrEmpty(LineIds) ? 0 : LineIds)} ) )
ORDER BY {SortBy} ";

            IEnumerable<BEP.Product> Items = await SQLListAsync(query, Relations);
            return Items;
        }

        public IEnumerable<BEP.Product> ListWithPrices2(long? IdLine, string Name, List<string> CategoryIds, string SortBy, params Enum[] Relations)
        {
            StringBuilder sbQuery = new();

            sbQuery.AppendLine(@"SELECT  Id, Name, Description, Commentaries, ItemCode, Link, Warranty, Consumables, Enabled, ShowAlways, ShowInWeb, ImageURL, Category, SubCategory, ");
            sbQuery.AppendLine(@"        Editable, EnabledDate, LogUser, LogDate, ( SELECT Name FROM Product.Line WHERE Id = ( SELECT IdLine FROM Product.LineDetail WHERE SAPLine = p.Line ) ) AS Line ");
            sbQuery.AppendLine(@"FROM    Product.Product p ");
            sbQuery.AppendLine(@"WHERE   p.Enabled = 1 AND ( p.ShowAlways = 1 OR EXISTS ( SELECT * FROM Product.Price WHERE IdProduct = p.Id AND Regular > 0 ) ) ");
            if (IdLine.HasValue && IdLine.Value > 0)
            {
                sbQuery.AppendLine($@"        AND ISNULL(Line, '') IN ( SELECT SAPLine FROM Product.LineDetail WHERE IdLine = {IdLine.Value} ) ");
            }
            if (CategoryIds?.Count > 0)
            {
                sbQuery.AppendLine($@"        AND ISNULL(p.Category, '') IN ( {string.Join(",", CategoryIds.Select(x => $"'{x}'"))} )");
            }
            if (!string.IsNullOrWhiteSpace(Name))
            {
                sbQuery.AppendLine($@"        AND ( p.Name LIKE '%{Name}%' OR p.Description LIKE '%{Name}%' OR p.ItemCode LIKE '%{Name}%' OR  ISNULL(p.Category, '') LIKE '%{Name}%' OR ISNULL(p.SubCategory, '') LIKE '%{Name}%' ) ");
            }
            sbQuery.AppendLine(@"ORDER BY " + SortBy);

            IEnumerable<BEP.Product> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        public IEnumerable<BEP.Product> ListWithPrices3(string ItemCodes, string SortBy, params Enum[] Relations)
        {
            string query = $@"SELECT  *
                              FROM    Product.Product p
                              WHERE   p.Enabled = 1
                                      AND ( p.ShowAlways = 1 OR EXISTS ( SELECT * FROM Product.Price WHERE IdProduct = p.Id AND Regular > 0 ) )
                                      AND p.ItemCode IN ( {ItemCodes} )
                              ORDER BY {SortBy} ";

            IEnumerable<BEP.Product> Collection = SQLList(query, Relations);
            return Collection;
        }

        public IEnumerable<BEP.Product> ListWithPrices(List<Field> FilterList, string SortingBy, params Enum[] Relations)
        {
            StringBuilder sbQuery = new();
            var filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT  p.* ");
            sbQuery.AppendLine("FROM    Product.Product p ");
            sbQuery.AppendLine("WHERE   ( p.ShowAlways = 1 OR EXISTS ( SELECT * FROM Product.Price WHERE IdProduct = p.Id AND Regular > 0 ) ) ");
            if (filter != "") sbQuery.AppendLine($"        AND {filter} ");
            sbQuery.AppendLine("ORDER BY " + SortingBy);

            IEnumerable<BEP.Product> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        public IEnumerable<BEP.Product> ListWithPricesAndNew(int Days, List<Field> FilterList, string SortingBy, params Enum[] Relations)
        {
            string query, filter = FilterList?.Count > 0 ? $" AND {GetFilterString(FilterList?.ToArray())}" : "";
            query = $@"SELECT  p.*
FROM    ( SELECT  ph.IdProduct, MIN(ph.LogDate) AS LogDate
          FROM    Product.PriceHistory ph
          GROUP BY ph.IdProduct ) a
        INNER JOIN Product.Product p ON a.IdProduct = p.Id
        LEFT OUTER JOIN Product.Price p1 ON p.Id = p1.IdProduct AND p1.Regular > 0
WHERE   DATEDIFF(DAY, a.LogDate, GETDATE()) <= {Days}
        AND ( p.ShowAlways = 1 OR ISNULL(p1.Id, 0) <> 0 ) {filter} 
ORDER BY {SortingBy} ";

            IEnumerable<BEP.Product> Items = SQLList(query, Relations);
            return Items;
        }

        public IEnumerable<BEP.Product> ListWithOffer(List<Field> FilterList, string SortBy, params Enum[] Relations)
        {
            StringBuilder sbQuery = new();
            var filter = GetFilterString(FilterList.ToArray());

            sbQuery.AppendLine("SELECT  p.* ");
            sbQuery.AppendLine("FROM    Product.Product p ");
            sbQuery.AppendLine("WHERE   EXISTS ( SELECT * FROM Product.PriceOffer po WHERE po.IdProduct = p.Id AND po.Price > 0 AND po.Enabled = 1 AND ( po.Since IS NULL OR po.Since <= CAST(GETDATE() AS DATE) ) AND ( po.Until IS NULL OR po.Until >= CAST(GETDATE() AS DATE) ) ) ");
            if (filter != "") sbQuery.AppendLine($"        AND {filter} ");
            sbQuery.AppendLine("ORDER BY " + SortBy);

            IEnumerable<BEP.Product> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        public IEnumerable<BEP.Product> ListWithOfferSA(List<Field> FilterList, string SortBy, params Enum[] Relations)
        {
            StringBuilder sbQuery = new();
            var filter = GetFilterString(FilterList.ToArray());

            sbQuery.AppendLine("SELECT  p.* ");
            sbQuery.AppendLine("FROM    Product.Product p ");
            sbQuery.AppendLine("WHERE   EXISTS ( SELECT * FROM Product.PriceOffer po WHERE po.IdProduct = p.Id AND po.IdSubsidiary = 1 AND po.Price > 0 AND po.Enabled = 1 AND ( po.Since IS NULL OR po.Since <= CAST(GETDATE() AS DATE) ) AND ( po.Until IS NULL OR po.Until >= CAST(GETDATE() AS DATE) ) )  ");
            if (filter != "") sbQuery.AppendLine($"        AND {filter} ");
            sbQuery.AppendLine("ORDER BY " + SortBy);

            IEnumerable<BEP.Product> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        public IEnumerable<BEP.Product> ListWithOfferLA(List<Field> FilterList, string SortBy, params Enum[] Relations)
        {
            StringBuilder sbQuery = new();
            var filter = GetFilterString(FilterList.ToArray());

            sbQuery.AppendLine("SELECT  p.* ");
            sbQuery.AppendLine("FROM    Product.Product p ");
            sbQuery.AppendLine("WHERE   EXISTS ( SELECT * FROM Product.PriceOffer po WHERE po.IdProduct = p.Id AND po.IdSubsidiary = 2 AND po.Price > 0 AND po.Enabled = 1 AND ( po.Since IS NULL OR po.Since <= CAST(GETDATE() AS DATE) ) AND ( po.Until IS NULL OR po.Until >= CAST(GETDATE() AS DATE) ) )  ");
            if (filter != "") sbQuery.AppendLine($"        AND {filter} ");
            sbQuery.AppendLine("ORDER BY " + SortBy);

            IEnumerable<BEP.Product> Collection = SQLList(sbQuery.ToString(), Relations);
            return Collection;
        }

        public IEnumerable<BEP.Product> ListWithOfferIQ(List<Field> FilterList, string SortBy, params Enum[] Relations)
        {
            StringBuilder sbQuery = new();
            var filter = GetFilterString(FilterList.ToArray());

            sbQuery.AppendLine("SELECT  p.* ");
            sbQuery.AppendLine("FROM    Product.Product p ");
            sbQuery.AppendLine("WHERE   EXISTS ( SELECT * FROM Product.PriceOffer po WHERE po.IdProduct = p.Id AND po.IdSubsidiary = 3 AND po.Price > 0 AND po.Enabled = 1 AND ( po.Since IS NULL OR po.Since <= CAST(GETDATE() AS DATE) ) AND ( po.Until IS NULL OR po.Until >= CAST(GETDATE() AS DATE) ) )  ");
            if (filter != "") sbQuery.AppendLine($"        AND {filter} ");
            sbQuery.AppendLine("ORDER BY " + SortBy);

            IEnumerable<BEP.Product> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        public IEnumerable<string> ListCategories(long? IdLine)
        {
            StringBuilder sb = new();
            DynamicParameters parameters = new();

            sb.AppendLine("SELECT   DISTINCT Category ");
            sb.AppendLine("FROM     Product.Product ");
            sb.AppendLine("WHERE    Category IS NOT NULL ");
            sb.AppendLine("         AND Enabled = 1 ");
            if (IdLine.HasValue)
            {
                sb.AppendLine("        AND ISNULL(Line, '') IN ( SELECT SAPLine FROM Product.LineDetail WHERE IdLine = @IdLine ) ");
                parameters.Add("IdLine", IdLine.Value);
            }
            sb.AppendLine("ORDER BY 1");
            IEnumerable<BEP.Product> Items = SQLList(sb.ToString(), parameters);
            return Items?.Select(i => i.Category) ?? new List<string>();
        }

        public IEnumerable<string> ListSAPLines(long? LineId)
        {
            string strFilter = "";
            if (LineId.HasValue)
            {
                strFilter = $"WHERE IdLine <> {LineId}";
            }
            StringBuilder sb = new();
            sb.AppendLine(@"SELECT	DISTINCT Line ");
            sb.AppendLine(@"FROM	Product.Product ");
            sb.AppendLine(@"WHERE	Line IS NOT NULL ");
            sb.AppendLine($@"		AND ISNULL(Line, '') NOT IN ( SELECT SAPLine FROM Product.LineDetail {strFilter} ) ");
            sb.AppendLine(@"ORDER BY 1");

            IEnumerable<BEP.Product> Items = SQLList(sb.ToString());
            return Items?.Select(i => i.Line) ?? new List<string>();
        }

        public IEnumerable<BEP.Product> ListProducts(string SortingBy)
        {
            string query;
            query = $@"SELECT	p.*, ld.IdLine, l.Name AS LineName
                       FROM		Product.Product p
                                INNER JOIN Product.LineDetail ld ON LOWER(p.Line) = LOWER(ld.SAPLine)
                                INNER JOIN Product.Line l ON ld.IdLine = l.Id			
                       WHERE	EXISTS ( SELECT * FROM Product.Document d WHERE p.Id = d.IdProduct )
                       ORDER BY {SortingBy} ";
            IEnumerable<BEP.Product> Items = SQLList(query);
            return Items;
        }

        public IEnumerable<BEP.Product> ListEpson()
        {
            var sb = new StringBuilder();
            sb.AppendLine(@"SELECT	* ");
            sb.AppendLine(@"FROM	vwProductsEpson ");
            sb.AppendLine(@"ORDER BY 1 ");

            IEnumerable<BEP.Product> Items = SQLList(sb.ToString());
            return Items;
        }

        public IEnumerable<BEP.Product> ListSuggested(long IdUser, int Quantity, params Enum[] Relations)
        {
            string query = $@"SELECT  TOP {Quantity} *
FROM    ( SELECT  *
          FROM    ( SELECT  TOP {Quantity} p.Id,p.Name, p.Description,p.ItemCode, p.Link, p.ImageURL, p.Line, p.Category, p.SubCategory, p.IsDigital 
                    FROM    Online.Sale s
                            INNER JOIN Online.SaleDetail sd ON s.Id = sd.IdSale
                            INNER JOIN Product.RelatedProduct rp ON sd.IdProduct = rp.IdProduct
                            INNER JOIN Product.Product p ON rp.IdRelated = p.Id
                            INNER JOIN Product.Price p3 ON p.Id = p3.IdProduct AND sd.Price > 0
                    WHERE   s.IdUser = {IdUser} AND s.StateIdc = 25 AND p.Enabled = 1 AND p.ShowTeamOnly = 0
                    ORDER BY NEWID() ) a
          UNION 
          SELECT  *
          FROM    ( SELECT  TOP {Quantity} p1.Id, p1.Name, p1.Description, p1.ItemCode, p1.Link, p1.ImageURL, p1.Line, p1.Category, p1.SubCategory, p1.IsDigital
                    FROM    Online.Sale s
                            INNER JOIN Online.SaleDetail sd ON s.Id = sd.IdSale
                            INNER JOIN Product.Product p ON sd.IdProduct = p.Id
                            INNER JOIN Product.RelatedCategory rc ON p.Category = rc.Category
                            INNER JOIN Product.Product p1 ON rc.Related = p1.Category AND p.Enabled = 1
                            INNER JOIN Product.Price p2 ON p1.Id = p2.IdProduct AND p2.Regular > 0
                    WHERE   s.IdUser = {IdUser} AND s.StateIdc = 25 AND p1.Enabled = 1 AND p1.ShowTeamOnly = 0
                            AND p.Id <> p1.Id
                    ORDER BY NEWID() ) a ) b
ORDER BY NEWID() ";

            IEnumerable<BEP.Product> Items = SQLList(query, Relations);
            return Items;
        }

        public IEnumerable<BEP.Product> ListCategories()
        {
            string query = @"SELECT DISTINCT p.Category FROM Product.Product p ORDER BY 1";
            IEnumerable<BEP.Product> items = SQLList(query);
            return items;
        }

        public IEnumerable<BEP.Product> ListForCampaign(long IdCampaign, string SortingBy, params Enum[] Relations)
        {
            string query = $@"SELECT  *
FROM    Product.Product p
WHERE   ( p.ShowAlways = 1 OR EXISTS ( SELECT * FROM Product.Price p1 WHERE p.Id = p1.IdProduct ) )
        AND EXISTS ( SELECT * FROM Campaign.Product p1 WHERE LOWER(p.ItemCode) = LOWER(p1.ItemCode) AND IdCampaign = {IdCampaign} ) 
ORDER BY {SortingBy} ";
            IEnumerable<BEP.Product> Items = SQLList(query, Relations);
            return Items;
        }

        public IEnumerable<BEP.Product> ListByLine(long IdLine, string SortingBy, params Enum[] Relations)
        {
            string query;
            query = $@"SELECT	p.*
FROM	Product.Product p
        INNER JOIN Product.LineDetail ld ON LOWER(p.Line) = LOWER(ld.SAPLine)        
WHERE	p.Enabled = 1 AND ld.IdLine = {IdLine}
ORDER BY {SortingBy} ";
            IEnumerable<BEP.Product> Items = SQLList(query, Relations);
            return Items;
        }

        public IEnumerable<BEP.Product> ListForPiggyBank(string SortBy, params Enum[] Relations)
        {
            string query = $@"SELECT  p.*
FROM    Product.Product p
        INNER JOIN PiggyBank.UsedProduct up ON p.ItemCode = up.ItemCode
ORDER BY {SortBy} ";

            IEnumerable<BEP.Product> Items = SQLList(query, Relations);
            return Items;
        }

        #endregion

        #region Search Methods 

        public BEP.Product Search(string ItemCode, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Product].[Product] WHERE ItemCode = @ItemCode ";
            BEP.Product Item = Connection.QueryFirstOrDefault<BEP.Product>(strQuery, new { @ItemCode = ItemCode });
            if (Item != null) LoadRelations(ref Item, Relations);
            return Item;
        }

        public int CountWithOffer(List<Field> FilterList)
        {
            StringBuilder sbQuery = new();
            var filter = FilterList?.Count > 0 ? GetFilterString(FilterList.ToArray()) : "";

            sbQuery.AppendLine("SELECT  COUNT(*) ");
            sbQuery.AppendLine("FROM    Product.Product p ");
            sbQuery.AppendLine("WHERE   EXISTS ( SELECT * FROM Product.PriceOffer po WHERE po.IdProduct = p.Id AND po.Price > 0 AND po.Enabled = 1 AND ( po.Since IS NULL OR po.Since <= CAST(GETDATE() AS DATE) ) AND ( po.Until IS NULL OR po.Until >= CAST(GETDATE() AS DATE) ) ) ");
            if (filter != "") sbQuery.AppendLine($"        AND {filter} ");

            int count = base.SQLScalar<int>(sbQuery.ToString());
            return count;
        }

        #endregion

    }

}