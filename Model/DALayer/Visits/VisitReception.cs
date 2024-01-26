using BEntities.Filters;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BE = BEntities;
using BEV = BEntities.Visits;

namespace DALayer.Visits
{
    public partial class VisitReception
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        public List<BEV.VisitReception> List2(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            var (filter, parameters) = GetFilter(FilterList.ToArray());
            var sb = new StringBuilder();
            sb.AppendLine(@"SELECT	DISTINCT v.*, c.Name AS ClientName ");
            sb.AppendLine(@"FROM	Visits.VisitReception v ");
            sb.AppendLine(@"		INNER JOIN Visits.Person p ON v.VisitorId = p.Id ");
            sb.AppendLine(@"		INNER JOIN AppData.StaffMembers s ON v.StaffId = s.Id ");
            sb.AppendLine(@"        INNER JOIN AppData.Client c ON v.CardCode = c.Code ");
            if (filter != "") sb.AppendLine($@"WHERE  {filter} ");
            sb.AppendLine($@"ORDER By {Order}");

            List<BEV.VisitReception> Items = SQLList(sb.ToString(), parameters, Relations).AsList();
            return Items;
        }

        #endregion

        #region Search Methods

        #endregion

    }
}