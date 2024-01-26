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

namespace DALayer.AppData
{
    public partial class Category
    {

        #region Save Methods

        public void DeleteAll()
        {
            string strQuery = "DELETE FROM [AppData].[Category]";
            Connection.Execute(strQuery);
        }

        #endregion

        #region Methods

        #endregion

        #region List Methods

        public List<BED.Category> ListDistinct(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT DISTINCT Name FROM [AppData].[Category] ORDER By " + Order;
            List<BED.Category> Items = SQLList(strQuery, Relations).ToList();
            return Items;
        }

        #endregion

        #region Search Methods

        #endregion

    }
}