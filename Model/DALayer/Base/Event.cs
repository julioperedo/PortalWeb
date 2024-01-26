using DALayer.Product;
using DALayer.Sales;

using DALayer.Security;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using BE = BEntities;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BEL = BEntities.Sales;
using BEP = BEntities.Product;
using BES = BEntities.Security;

namespace DALayer.Base
{

    public partial class Event
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        public List<BEB.Event> ListLast(params Enum[] Relations)
        {
            string strQuery = "SELECT TOP 10 * FROM [Base].[Event] ORDER By [Date] DESC";
            List<BEB.Event> Items = SQLList(strQuery, Relations).AsList();
            return Items;
        }

        public List<BEB.Event> List(int InitialIndex, int FinalIndex, string Order, params Enum[] Relations)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("SELECT  * ");
            sb.AppendLine($"FROM    ( SELECT ROW_NUMBER () OVER (ORDER BY {Order}) AS RowIndex, * FROM [Base].[Event] ) a ");
            sb.AppendLine($"WHERE   RowIndex BETWEEN {InitialIndex} AND {FinalIndex} ");
            List<BEB.Event> Items = SQLList(sb.ToString(), Relations).AsList();
            return Items;
        }

        #endregion

        #region Search Methods

        #endregion

    }

}