using DALayer.Product;
using DALayer.Sales;

using DALayer.Security;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BE = BEntities;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BEL = BEntities.Sales;
using BEP = BEntities.Product;
using BES = BEntities.Security;

namespace DALayer.Base
{
    public partial class Notification
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        public List<BEB.Notification> List(string CardCode, string Name, DateTime? Since, DateTime? Until, bool? Enabled, string Order, params Enum[] Relations)
        {
            var sb = new StringBuilder();
            sb.AppendLine("SELECT  * ");
            sb.AppendLine("FROM    Base.Notification n ");
            sb.AppendLine("WHERE   NOT EXISTS ( SELECT * ");
            sb.AppendLine("                     FROM   Base.NotificationDetail d ");
            sb.AppendLine("                     WHERE  IdNotification = n.Id ");
            sb.AppendLine($"                            AND IdLine IN ( SELECT IdLine FROM Security.LineNotAllowed WHERE CardCode = '{CardCode}' ) ) ");
            if (!string.IsNullOrEmpty(Name))
            {
                sb.AppendLine($"         AND ( n.Name LIKE '%{Name}%' OR n.Description LIKE '%{Name}%' ) ");
            }
            if (Since.HasValue)
            {
                sb.AppendLine($"         AND ISNULL(n.FinalDate, '{Since.Value:yyyy/MM/dd}') >= '{Since.Value:yyyy/MM/dd}' ");
            }
            if (Until.HasValue)
            {
                sb.AppendLine($"         AND ISNULL(n.InitialDate, '{Until.Value:yyyy/MM/dd}') <= '{Until.Value:yyyy/MM/dd}' ");
            }
            if (Enabled.HasValue)
            {
                sb.AppendLine($"         AND n.Enabled = {(Enabled.Value ? "1" : "0")} ");
            }
            sb.AppendLine($"ORDER BY {Order} ");

            List<BEB.Notification> Items = SQLList(sb.ToString(), Relations).AsList(); 
            return Items;
        }

        #endregion

        #region Search Methods

        #endregion

    }
}