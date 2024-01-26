using BEntities.Filters;
using DALayer.Base;
using DALayer.Sales;

using DALayer.Security;
using DALayer.Staff;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BE = BEntities;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BEF = BEntities.Staff;
using BEL = BEntities.Sales;
using BEP = BEntities.Product;
using BES = BEntities.Security;

namespace DALayer.Product
{
    public partial class PriceHistory
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        public IEnumerable<BEP.PriceHistory> List2(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new();
            var (filter, parameters) = GetFilter(FilterList.ToArray());

            sbQuery.AppendLine("SELECT  h.*, u.FirstName + ' ' + u.LastName AS UserName ");
            sbQuery.AppendLine("FROM    [Product].[PriceHistory] h ");
            sbQuery.AppendLine("        LEFT OUTER JOIN [Security].[User] u ON h.LogUser = u.Id ");
            if (filter != "") sbQuery.AppendLine($"WHERE     {filter} ");
            sbQuery.AppendLine("ORDER By " + Order);

            IEnumerable<BEP.PriceHistory> Items = SQLList(sbQuery.ToString(), parameters, Relations);
            return Items;
        }

        public IEnumerable<BEP.PriceHistory> ListFirst(List<long> Ids)
        {
            DynamicParameters parameters = new();
            var sb = new StringBuilder();
            sb.AppendLine("SELECT	IdProduct, MIN(LogDate) AS LogDate ");
            sb.AppendLine("FROM	    Product.PriceHistory ");
            if (Ids?.Count > 0)
            {
                sb.AppendLine($"WHERE    IdProduct IN @Ids ");
                parameters.Add("Ids", Ids);
            }
            sb.AppendLine("GROUP BY IdProduct ");
            sb.AppendLine("ORDER  BY IdProduct ");

            IEnumerable<BEP.PriceHistory> Items = SQLList(sb.ToString(), parameters);
            return Items;
        }

        #endregion

        #region Search Methods

        #endregion

    }
}