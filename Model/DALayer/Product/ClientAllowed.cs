using DALayer.Base;

using DALayer.Security;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using BE = BEntities;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BEP = BEntities.Product;
using BES = BEntities.Security;

namespace DALayer.Product
{
    public partial class ClientAllowed
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        #endregion

        #region Search Methods

        public bool IsAllowed(string CardCode)
        {
            string strQuery = $"SELECT COUNT(*) FROM [Product].[ClientAllowed] WHERE CardCode = '{CardCode}' ";
            int count = SQLScalar<int>(strQuery);
            return count > 0;
        }

        #endregion

    }
}