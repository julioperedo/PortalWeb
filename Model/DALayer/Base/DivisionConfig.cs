using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BE = BEntities;
using BEB = BEntities.Base;

namespace DALayer.Base
{
    public partial class DivisionConfig
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        public async Task<List<BEB.DivisionConfig>> ListAsync(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Base].[DivisionConfig] ORDER By " + Order;
            List<BEB.DivisionConfig> Items = (await SQLListAsync(strQuery, Relations)).AsList();
            return Items;
        }

        #endregion

        #region Search Methods

        #endregion

    }
}