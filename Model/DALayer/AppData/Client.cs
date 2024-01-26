using DALayer.Base;
using DALayer.Online;
using DALayer.Product;
using DALayer.Sales;

using DALayer.Security;
using DALayer.Staff;
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
using Dapper;

namespace DALayer.AppData
{
    public partial class Client
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        #endregion

        #region Search Methods

        public BED.Client Search(string Code, params Enum[] Relations)
        {
            string strQuery = $@"SELECT * FROM [AppData].[Client] WHERE LOWER(Code) = '{Code.ToLower()}' ";
            BED.Client Item = Connection.QueryFirstOrDefault<BED.Client>(strQuery);
            if (Item != null) LoadRelations(ref Item, Relations);
            return Item;
        }

        #endregion

    }
}