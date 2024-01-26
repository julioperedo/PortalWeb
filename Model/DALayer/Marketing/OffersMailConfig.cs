using DALayer.AppData;
using DALayer.Base;
using DALayer.Logs;
using DALayer.Online;
using DALayer.Product;
using DALayer.Sales;

using DALayer.Security;
using DALayer.Staff;
using DALayer.Visits;
using DALayer.WebSite;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BE = BEntities;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BED = BEntities.AppData;
using BEF = BEntities.Staff;
using BEG = BEntities.Logs;
using BEL = BEntities.Sales;
using BEM = BEntities.Marketing;
using BEO = BEntities.Online;
using BEP = BEntities.Product;
using BES = BEntities.Security;
using BEV = BEntities.Visits;
using BEW = BEntities.WebSite;

namespace DALayer.Marketing
{
    public partial class OffersMailConfig
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        public IEnumerable<BEM.OffersMailConfig> ListWithOffers(string Order, params Enum[] Relations)
        {
            var sb = new StringBuilder();
            sb.AppendLine(@"SELECT	o.Id, o.IdLine, o.WeekDay, COUNT(*) AS OffersCount ");
            sb.AppendLine(@"FROM	Product.Product p ");
            sb.AppendLine(@"		INNER JOIN Product.Price r ON p.Id = r.IdProduct ");
            sb.AppendLine(@"        INNER JOIN Product.PriceOffer po ON p.Id = po.IdProduct AND ISNULL(po.Price, 0) > 0 ");
            sb.AppendLine(@"		INNER JOIN Product.LineDetail ld ON p.Line = ld.SAPLine ");
            sb.AppendLine(@"		INNER JOIN Marketing.OffersMailConfig o ON ld.IdLine = o.IdLine ");
            sb.AppendLine(@"WHERE	p.Enabled = 1 ");
            sb.AppendLine(@"GROUP BY o.Id, o.IdLine, o.WeekDay ");
            sb.AppendLine($@"ORDER BY {Order} ");

            IEnumerable<BEM.OffersMailConfig> Collection = SQLList(sb.ToString(), Relations);
            return Collection;
        }

        #endregion

        #region Search Methods

        #endregion

    }
}