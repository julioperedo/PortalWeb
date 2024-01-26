using DALayer.AppData;
using DALayer.Base;
using DALayer.Online;
using DALayer.Sales;

using DALayer.Security;
using DALayer.Staff;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BE = BEntities;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BED = BEntities.AppData;
using BEF = BEntities.Staff;
using BEL = BEntities.Sales;
using BEO = BEntities.Online;
using BEP = BEntities.Product;
using BES = BEntities.Security;

namespace DALayer.Product {
    public partial class PriceExternalSite {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        #endregion

        #region Search Methods

        public BEP.PriceExternalSite SearchByProduct(long IdProduct, params Enum[] Relations) {
            string strQuery = "SELECT * FROM [Product].[PriceExternalSite] WHERE IdProduct = @IdProduct";
            BEP.PriceExternalSite Item = Connection.QueryFirstOrDefault<BEP.PriceExternalSite>(strQuery, new { @IdProduct = IdProduct });
            if(Item != null) LoadRelations(ref Item, Relations);
            return Item;
        }

        #endregion

    }
}