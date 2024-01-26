using BEntities.Filters;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BE = BEntities;
using BEK = BEntities.Kbytes;

namespace DALayer.Kbytes
{
	public partial class ClientStatus
	{

		#region Save Methods

		#endregion

		#region Methods

		#endregion

		#region List Methods

		public List<BEK.ClientStatus> ListCalculated(string CardCode)
		{
			string query = $@"SELECT  a.CardCode, a.Year, a.Quarter, SUM(a.Points) AS Points, SUM(a.Amount) AS Amount
							  FROM	 ( SELECT cn.CardCode, DATEPART(YEAR, cn.Date) AS Year, DATEPART(QUARTER, cn.Date) AS Quarter, sd.Points + sd.ExtraPoints + sd.ExtraPointsPeriod AS Points, sd.Amount
										FROM   Kbytes.StatusDetail sd
											   INNER JOIN Kbytes.ClientNote cn ON sd.IdNote = cn.Id
										WHERE  cn.Enabled = 1
										UNION ALL
										SELECT ca.CardCode, DATEPART(YEAR, ca.ClaimDate) AS Year, DATEPART(QUARTER, ca.ClaimDate) AS Quarter, sd.Points, sd.Amount
										FROM   Kbytes.StatusDetail sd
											   INNER JOIN Kbytes.ClaimedAward ca ON sd.IdAward = ca.Id ) a
							  WHERE   LOWER(a.CardCode) = '{CardCode.ToLower()}'
							  GROUP BY a.CardCode, a.Year, a.Quarter ";
			List<BEK.ClientStatus> Items = Connection.Query<BEK.ClientStatus>(query).ToList();
			return Items;
		}

		public List<BEK.ClientStatus> ListCalculatedByYear(string CardCode)
		{
			string query = $@"SELECT  a.CardCode, a.Year, SUM(a.Points) AS Points, SUM(a.Amount) AS Amount
							  FROM	 ( SELECT cn.CardCode, DATEPART(YEAR, cn.Date) AS Year, sd.Points + sd.ExtraPoints + sd.ExtraPointsPeriod AS Points, sd.Amount
										FROM   Kbytes.StatusDetail sd
											   INNER JOIN Kbytes.ClientNote cn ON sd.IdNote = cn.Id
										WHERE  cn.Enabled = 1
										/*UNION ALL
										SELECT ca.CardCode, DATEPART(YEAR, ca.ClaimDate) AS Year, sd.Points, sd.Amount
										FROM   Kbytes.StatusDetail sd
											   INNER JOIN Kbytes.ClaimedAward ca ON sd.IdAward = ca.Id*/ ) a
							  WHERE   LOWER(a.CardCode) = '{CardCode.ToLower()}'
							  GROUP BY a.CardCode, a.Year ";
			List<BEK.ClientStatus> Items = Connection.Query<BEK.ClientStatus>(query).ToList();
			return Items;
		}

		public List<BEK.ClientStatus> ListCalculated()
		{
			string query = @"SELECT  YEAR(cn.Date) AS [Year], SUM(sd.Points + sd.ExtraPoints + sd.ExtraPointsPeriod) AS Points, SUM(sd.Amount) AS Amount
							 FROM    Kbytes.StatusDetail sd
									 INNER JOIN Kbytes.ClientNote cn ON sd.IdNote = cn.Id
							 WHERE   cn.Enabled = 1
							 GROUP BY YEAR(cn.Date) 
							 ORDER BY 1 DESC ";
			List<BEK.ClientStatus> Items = Connection.Query<BEK.ClientStatus>(query).ToList();
			return Items;
		}

		public List<BEK.ClientStatus> ListCalculated(int Year)
		{
			string query = $@"SELECT  a.CardCode, a.Year, a.Quarter, SUM(a.Points) AS Points, SUM(a.Amount) AS Amount
							  FROM	 ( SELECT cn.CardCode, DATEPART(YEAR, cn.Date) AS Year, DATEPART(QUARTER, cn.Date) AS Quarter, sd.Points + sd.ExtraPoints + sd.ExtraPointsPeriod AS Points, sd.Amount
										FROM   Kbytes.StatusDetail sd
											   INNER JOIN Kbytes.ClientNote cn ON sd.IdNote = cn.Id
										WHERE  cn.Enabled = 1
										UNION ALL
										SELECT ca.CardCode, DATEPART(YEAR, ca.ClaimDate) AS Year, DATEPART(QUARTER, ca.ClaimDate) AS Quarter, sd.Points, sd.Amount
										FROM   Kbytes.StatusDetail sd
											   INNER JOIN Kbytes.ClaimedAward ca ON sd.IdAward = ca.Id ) a
							  WHERE   a.Year = {Year} OR a.Year = {Year - 1}
							  GROUP BY a.CardCode, a.Year, a.Quarter ";
			List<BEK.ClientStatus> Items = Connection.Query<BEK.ClientStatus>(query).ToList();
			return Items;
		}

		public List<BEK.ClientStatus> ListAllClientsCalculated()
		{
			string query = @"SELECT  cn.CardCode, YEAR(cn.Date) AS [Year], SUM(sd.Points + sd.ExtraPoints + sd.ExtraPointsPeriod) AS Points, SUM(sd.Amount) AS Amount
							 FROM	 Kbytes.StatusDetail sd
							   INNER JOIN Kbytes.ClientNote cn ON sd.IdNote = cn.Id
							 WHERE	 cn.Enabled = 1
							 GROUP BY cn.CardCode, YEAR(cn.Date) ";
			//string query = @"SELECT  a.CardCode, a.Year, a.Quarter, SUM(a.Points) AS Points, SUM(a.Amount) AS Amount
			//                 FROM	 ( SELECT cn.CardCode, DATEPART(YEAR, cn.Date) AS Year, DATEPART(QUARTER, cn.Date) AS Quarter, sd.Points + sd.ExtraPoints + sd.ExtraPointsPeriod AS Points, sd.Amount
			//                     FROM	  Kbytes.StatusDetail sd
			//                   INNER JOIN Kbytes.ClientNote cn ON sd.IdNote = cn.Id
			//                     WHERE  cn.Enabled = 1
			//                     UNION ALL
			//                     SELECT ca.CardCode, DATEPART(YEAR, ca.ClaimDate) AS Year, DATEPART(QUARTER, ca.ClaimDate) AS Quarter, sd.Points, sd.Amount
			//                     FROM	  Kbytes.StatusDetail sd
			//                   INNER JOIN Kbytes.ClaimedAward ca ON sd.IdAward = ca.Id ) a
			//                 GROUP BY a.CardCode, a.Year, a.Quarter ";

			List<BEK.ClientStatus> Items = Connection.Query<BEK.ClientStatus>(query).ToList();
			return Items;
		}

		#endregion

		#region Search Methods

		#endregion

	}
}