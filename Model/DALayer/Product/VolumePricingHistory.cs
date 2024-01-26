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
using BEP = BEntities.Product;

namespace DALayer.Product
{
    public partial class VolumePricingHistory
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        public IEnumerable<BEP.VolumePricingHistory> List2(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new();
            var filter = GetFilterString(FilterList.ToArray());

            sbQuery.AppendLine("SELECT  h.*, u.FirstName + ' ' + u.LastName AS UserName ");
            sbQuery.AppendLine("FROM    [Product].[VolumePricingHistory] h ");
            sbQuery.AppendLine("        LEFT OUTER JOIN Security.[User] u ON h.LogUser = u.Id ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEP.VolumePricingHistory> Collection = SQLList(sbQuery.ToString(), Relations);
            return Collection;
        }

        #endregion

        #region Search Methods

        #endregion

    }
}