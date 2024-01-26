using BEntities.Filters;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using BE = BEntities;
using BES = BEntities.Security;

namespace DALayer.Security
{
    public partial class RejectedMails
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        public List<BES.RejectedMails> ListWithDesc(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            string filter = FilterList?.Count > 0 ? $"WHERE    {GetFilterString(FilterList.ToArray())} " : "";
            string query = $@"SELECT	rm.*, c.Name AS ReasonDesc
						      FROM		Security.RejectedMails rm
										INNER JOIN Base.Classifier c ON rm.Reason = c.Value AND c.IdType = 19
							  {filter}
							  ORDER BY {Order}";

            List<BES.RejectedMails> Items = SQLList(query, Relations).ToList();
            return Items;
        }

        #endregion

        #region Search Methods

        #endregion

    }
}