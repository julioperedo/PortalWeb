using BEntities.Filters;
using DALayer.Base;

using DALayer.Security;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BE = BEntities;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BEP = BEntities.Product;
using BES = BEntities.Security;

namespace DALayer.Product
{
    public partial class Line
    {

        #region Save Methods 

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        public IEnumerable<BEP.Line> ListExtended(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new();
            var filter = GetFilterString(FilterList.ToArray());

            sbQuery.AppendLine("SELECT  *, CAST( ( SELECT COUNT(*) FROM Product.LineDetail d WHERE d.IdLine = l.Id ) AS BIT ) AS HasDetail ");
            sbQuery.AppendLine("FROM    [Product].[Line] l ");
            if (filter != "") sbQuery.AppendLine($"WHERE     {filter} ");
            sbQuery.AppendLine("ORDER By " + Order);

            IEnumerable<BEP.Line> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        public IEnumerable<BEP.Line> ListForPriceList(string Order, params Enum[] Relations)
        {
            StringBuilder sb = new();
            sb.AppendLine(@"SELECT  DISTINCT l.* ");
            sb.AppendLine(@"FROM    Product.Line l ");
            sb.AppendLine(@"		INNER JOIN Product.LineDetail d ON l.Id = d.IdLine ");
            sb.AppendLine(@"		INNER JOIN Product.Product p ON p.Line = d.SAPLine AND p.Enabled = 1 ");
            sb.AppendLine(@"		INNER JOIN Product.Price r ON r.IdProduct = p.Id AND ( ISNULL(r.Regular, 0) > 0 OR p.ShowAlways = 1 ) ");
            sb.AppendLine(@"ORDER BY " + Order);

            IEnumerable<BEP.Line> Items = SQLList(sb.ToString(), Relations);
            return Items;
        }

        public IEnumerable<BEP.Line> ListForPriceList(bool Team, string SortingBy, params Enum[] Relations)
        {
            string filter = Team ? "" : "AND p.ShowTeamOnly = 0",
                query = $@"SELECT  DISTINCT l.*
FROM    Product.Line l
        INNER JOIN Product.LineDetail ld ON l.Id = ld.IdLine
        INNER JOIN Product.Product p ON LOWER(ld.SAPLine) = LOWER(p.Line) 
        INNER JOIN Product.Price p1 ON p.Id = p1.IdProduct
WHERE   p.Enabled = 1
        AND ( ISNULL(p1.Regular, 0) > 0 OR p.ShowAlways = 1 ) {filter}
ORDER BY {SortingBy} ";

            IEnumerable<BEP.Line> Items = SQLList(query, Relations);
            return Items;
        }

        public IEnumerable<BEP.Line> ListForPriceList(string IdProducts, string Order, params Enum[] Relations)
        {
            StringBuilder sb = new();
            sb.AppendLine(@"SELECT  DISTINCT l.* ");
            sb.AppendLine(@"FROM    Product.Line l ");
            sb.AppendLine(@"		INNER JOIN Product.LineDetail d ON l.Id = d.IdLine ");
            sb.AppendLine($@"		INNER JOIN Product.Product p ON p.Line = d.SAPLine AND p.Enabled = 1 AND p.Id IN ( {IdProducts} ) ");
            sb.AppendLine(@"		INNER JOIN Product.Price r ON r.IdProduct = p.Id AND (ISNULL(r.Regular, 0) > 0 OR p.ShowAlways = 1 ) ");
            sb.AppendLine(@"ORDER BY " + Order);

            IEnumerable<BEP.Line> Items = SQLList(sb.ToString(), Relations);
            return Items;
        }

        public IEnumerable<BEP.Line> ListForOffers()
        {           
            string query = @"SELECT  l.Id, l.Name, COUNT(*) AS OffersCount 
FROM    Product.Product p 
        INNER JOIN Product.PriceOffer po ON p.Id = po.IdProduct AND ISNULL(po.Price, 0) > 0 
        INNER JOIN Product.LineDetail ld ON p.Line = ld.SAPLine 
        INNER JOIN Product.Line l ON ld.IdLine = l.Id 
WHERE   p.Enabled = 1 
GROUP BY l.Id, l.Name 
ORDER BY l.Name ";

            IEnumerable<BEP.Line> Items = SQLList(query);
            return Items;
        }

        #endregion

        #region Search Methods 

        public BEP.Line First(params Enum[] Relations)
        {
            string strQuery = "SELECT TOP 1 * FROM Product.Line ORDER BY Name ";
            BEP.Line Item = SQLSearch(strQuery, Relations);
            return Item;
        }

        public BEP.Line SearchByProduct(long IdProduct, params Enum[] Relations)
        {
            var sb = new StringBuilder();
            sb.AppendLine(@"SELECT	DISTINCT l.* ");
            sb.AppendLine(@"FROM	Product.Product p ");
            sb.AppendLine(@"		INNER JOIN Product.LineDetail ld ON p.Line = ld.SAPLine ");
            sb.AppendLine(@"		INNER JOIN Product.Line l ON ld.IdLine = l.Id ");
            sb.AppendLine(@"WHERE	p.Id = @IdProduct ");

            BEP.Line Item = SQLSearch(sb.ToString(), new { @IdProduct = IdProduct }, Relations);
            return Item;
        }

        #endregion

    }
}