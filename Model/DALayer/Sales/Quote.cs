using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using BE = BEntities;
using BEL = BEntities.Sales;

namespace DALayer.Sales {
    public partial class Quote {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        #endregion

        #region Search Methods

        public BEL.Quote Search(DateTime Date) {
            string strQuery = "SELECT MAX(QuoteCode) AS QuoteCode FROM [Sales].[Quote] WHERE QuoteDate = @QuoteDate ";
            BEL.Quote BEQuote = Connection.QueryFirstOrDefault<BEL.Quote>(strQuery, new { @QuoteDate = Date.ToString("yyyy/MM/dd") });
            return BEQuote;
        }

        #endregion

    }
}