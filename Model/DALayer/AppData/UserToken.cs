using DALayer.Base;
using DALayer.Online;
using DALayer.Product;
using DALayer.Sales;

using DALayer.Security;
using DALayer.Staff;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using BE = BEntities;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BED = BEntities.AppData;
using BEF = BEntities.Staff;
using BEL = BEntities.Sales;
using BEO = BEntities.Online;
using BEP = BEntities.Product;
using BES = BEntities.Security;

namespace DALayer.AppData
{
    public partial class UserToken
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        #endregion

        #region Search Methods

        public BED.UserToken SearchByUser(long Id, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [AppData].[UserToken] WHERE IdUser = " + Id.ToString();
            BED.UserToken BEUserToken = Connection.QueryFirstOrDefault<BED.UserToken>(strQuery);
            if (BEUserToken != null) LoadRelations(ref BEUserToken, Relations);
            return BEUserToken;
        }

        #endregion

    }
}