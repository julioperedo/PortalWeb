using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using BE = BEntities;
using BEK = BEntities.Kbytes;

namespace DALayer.Kbytes
{
    public partial class ClaimedAward
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        public List<BEK.ClaimedAward> ListAllClientsCalculated(params Enum[] Relations)
        {
            string strQuery = @"SELECT	ca.CardCode, YEAR(ca.ClaimDate) AS [Year], -SUM(ca.Points) AS Points
								FROM	Kbytes.ClaimedAward ca
								GROUP BY ca.CardCode, YEAR(ca.ClaimDate) ";

            List<BEK.ClaimedAward> Items = SQLList(strQuery, Relations).ToList();
            return Items;
        }

        #endregion

        #region Search Methods

        #endregion

    }
}