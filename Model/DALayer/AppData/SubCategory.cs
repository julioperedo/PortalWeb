using DALayer.Base;
using DALayer.Product;
using DALayer.Sales;

using DALayer.Security;
using DALayer.Staff;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using BE = BEntities;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BED = BEntities.AppData;
using BEF = BEntities.Staff;
using BEL = BEntities.Sales;
using BEP = BEntities.Product;
using BES = BEntities.Security;

namespace DALayer.AppData {
    public partial class SubCategory {

        #region Save Methods

        public void DeleteAll() {
            string strQuery = "DELETE FROM [AppData].[SubCategory]";
            Connection.Execute(strQuery);
        }

        #endregion

        #region Methods

        #endregion

        #region List Methods

        public List<BED.SubCategory> ListDistinct(long CategoryId, string Order, params Enum[] Relations)
        {
            string query = $@"SELECT  sc.*
                              FROM    AppData.SubCategory sc
                                      INNER JOIN AppData.Category c ON sc.IdCategory = c.Id
                              WHERE   c.Name = ( SELECT c1.Name FROM AppData.Category c1 WHERE c1.Id = {CategoryId} )
                              ORDER BY {Order} ";
            List<BED.SubCategory> Items = SQLList(query, Relations).ToList();
            return Items;
        }

        #endregion

        #region Search Methods

        #endregion

    }
}