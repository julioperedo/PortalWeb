using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BE = BEntities;
using BEP = BEntities.Product;

namespace DALayer.Product
{
    public partial class OpenBox
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        public List<BEP.OpenBox> ListByProduct(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            string strFilter = FilterList?.Count > 0 ? base.GetFilterString(FilterList.ToArray()) : "1 = 1";
            var sb = new StringBuilder();
            sb.AppendLine(@"SELECT	t0.* ");
            sb.AppendLine(@"FROM	Product.OpenBox t0 ");
            sb.AppendLine(@"		INNER JOIN Product.Product t1 ON t1.Id = t0.IdProduct ");
            sb.AppendLine($@"WHERE  {strFilter} ");
            sb.AppendLine($@" ORDER By {Order} ");

            List<BEP.OpenBox> Collection = SQLList(sb.ToString(), Relations).ToList();
            return Collection;
        }

        #endregion

        #region Search Methods

        #endregion

    }
}