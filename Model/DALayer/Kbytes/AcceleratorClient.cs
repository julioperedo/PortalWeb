using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using BE = BEntities;
using BEK = BEntities.Kbytes;

namespace DALayer.Kbytes
{
    public partial class AcceleratorClient
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        public List<BEK.AcceleratorClient> List(string CardCode, DateTime CurrentDate, params Enum[] Relations)
        {
            string query = $@"SELECT  *
							  FROM    Kbytes.AcceleratorClient ac
							  WHERE   ac.Enabled = 1 
									  AND ac.CardCode = '{CardCode}' 
							  		  AND '{CurrentDate:yyyy-MM-dd}' BETWEEN ac.InitialDate AND ac.FinalDate ";
            List<BEK.AcceleratorClient> Items = SQLList(query, Relations).ToList();
            return Items;
        }

        #endregion

        #region Search Methods

        #endregion

    }
}