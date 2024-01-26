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
    public partial class Visit
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        public List<BEV.Visit> List2(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            var (filter, parameteres) = GetFilter(FilterList.ToArray());

            var sb = new StringBuilder();
            sb.AppendLine(@"SELECT	DISTINCT v.*, c.Name AS ClientName, ISNULL(i.Name, '') AS ClientPhoto, ISNULL(d.Name, '') AS ClientDocId, ISNULL(dr.Name, '') AS ClientDocIdRev ");
            sb.AppendLine(@"FROM	Visits.Visit v ");
            sb.AppendLine(@"		INNER JOIN Visits.Person p ON v.VisitorId = p.Id ");
            sb.AppendLine(@"		INNER JOIN AppData.StaffMembers s ON v.StaffId = s.Id ");
            sb.AppendLine(@"        INNER JOIN AppData.Client c ON v.CardCode = c.Code ");
            sb.AppendLine(@"        LEFT OUTER JOIN Visits.Picture i ON v.VisitorId = i.IdPerson AND i.Type = 1 ");
            sb.AppendLine(@"        LEFT OUTER JOIN Visits.Picture d ON v.VisitorId = d.IdPerson AND d.Type = 2 ");
            sb.AppendLine(@"        LEFT OUTER JOIN Visits.Picture dr ON v.VisitorId = dr.IdPerson AND dr.Type = 3 ");
            if (filter != "") sb.AppendLine($@"WHERE  {filter} ");
            sb.AppendLine($@"ORDER By {Order}");

            List<BEV.Visit> Items = SQLList(sb.ToString(), parameteres, Relations).AsList();
            return Items;
        }

        #endregion

        #region Search Methods

        public BEV.Visit Search2(long Id, params Enum[] Relations)
        {
            var sb = new StringBuilder();
            sb.AppendLine(@"SELECT	DISTINCT v.*, c.Name AS ClientName, ISNULL(i.Name, '') AS ClientPhoto, ISNULL(d.Name, '') AS ClientDocId, ISNULL(dr.Name, '') AS ClientDocIdRev ");
            sb.AppendLine(@"FROM	Visits.Visit v ");
            sb.AppendLine(@"		INNER JOIN Visits.Person p ON v.VisitorId = p.Id ");
            sb.AppendLine(@"		INNER JOIN AppData.StaffMembers s ON v.StaffId = s.Id ");
            sb.AppendLine(@"        INNER JOIN AppData.Client c ON v.CardCode = c.Code ");
            sb.AppendLine(@"        LEFT OUTER JOIN Visits.Picture i ON v.VisitorId = i.IdPerson AND i.Type = 1 ");
            sb.AppendLine(@"        LEFT OUTER JOIN Visits.Picture d ON v.VisitorId = d.IdPerson AND d.Type = 2 ");
            sb.AppendLine(@"        LEFT OUTER JOIN Visits.Picture dr ON v.VisitorId = dr.IdPerson AND dr.Type = 3 ");
            sb.AppendLine($@"WHERE  v.Id = {Id} ");

            BEV.Visit Item = SQLSearch(sb.ToString(), Relations);
            return Item;
        }

        #endregion

    }
}