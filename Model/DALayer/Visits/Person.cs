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
    public partial class Person
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        public List<BEV.Person> List2(string CardCode, string Order, params Enum[] Relations)
        {
            var sb = new StringBuilder();
            sb.AppendLine(@"SELECT	DISTINCT p.*, ISNULL(p1.Name, '') AS PhotoURL, ISNULL(p2.Name, '') AS DocIdURL, ISNULL(p3.Name, '') AS DocIdRevURL ");
            sb.AppendLine(@"FROM	Visits.Person p ");
            sb.AppendLine(@"		LEFT OUTER JOIN Visits.Picture p1 ON p.Id = p1.IdPerson AND p1.Type = 1 ");
            sb.AppendLine(@"		LEFT OUTER JOIN Visits.Picture p2 ON p.Id = p2.IdPerson AND p2.Type = 2 ");
            sb.AppendLine(@"		LEFT OUTER JOIN Visits.Picture p3 ON p.Id = p3.IdPerson AND p3.Type = 3 ");
            sb.AppendLine(@"WHERE	NOT EXISTS (SELECT * FROM Security.UserPerson WHERE IdPerson = p.Id) ");
            sb.AppendLine($@"		AND (EXISTS (SELECT * FROM Visits.Visit WHERE VisitorId = p.Id AND (CardCode = '{CardCode}' OR CardCode = 'CVAR-001')) ");
            sb.AppendLine(@"			 OR NOT EXISTS (SELECT * FROM Visits.Visit WHERE VisitorId = p.Id)) ");
            sb.AppendLine($@"ORDER By {Order} ");

            List<BEV.Person> Items = SQLList(sb.ToString(), Relations).AsList();
            return Items;
        }

        public List<BEV.Person> List3(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new();
            var (filter, parameters) = GetFilter(FilterList.ToArray());

            sbQuery.AppendLine(@"SELECT  p.*, CAST((CASE ISNULL(u.Id, 0) WHEN 0 THEN 0 ELSE 1 END) AS BIT) AS HasUser ");
            sbQuery.AppendLine(@"FROM    [Visits].[Person] p ");
            sbQuery.AppendLine($@"       LEFT OUTER JOIN Security.UserPerson u ON p.Id = u.IdPerson ");
            if (filter != "") sbQuery.AppendLine($@"WHERE    {filter} ");
            sbQuery.AppendLine(@"ORDER By " + Order);

            List<BEV.Person> Collection = SQLList(sbQuery.ToString(), parameters, Relations).AsList();
            return Collection;
        }

        #endregion

        #region Search Methods

        #endregion

    }
}