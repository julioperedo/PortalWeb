using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BE = BEntities;
using BEB = BEntities.Base;

namespace DALayer.Base
{
    public partial class Menu
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        public List<BEB.Menu> ListByProfile(long IdProfile, string Order, params Enum[] Relations)
        {
            StringBuilder sb = new();
            sb.AppendLine("SELECT   m.* ");
            sb.AppendLine("FROM     Base.Menu m ");
            sb.AppendLine("WHERE    m.IdPage IS NULL OR m.IdPage IN ( SELECT IdPage FROM Security.ProfilePage WHERE IdProfile = @IdProfile ) ");
            sb.AppendLine($"ORDER BY {Order} ");
            List<BEB.Menu> Items = SQLList(sb.ToString(), new { @IdProfile = IdProfile }, Relations).AsList();
            return Items;
        }

        #endregion

        #region Search Methods

        #endregion

    }
}