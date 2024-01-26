using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BE = BEntities;
using BEA = BEntities.SAP;

namespace DALayer.SAP.Hana
{
    [Serializable()]
    public class Address : DALEntity<BEA.Address>
    {
        #region Global Variables

        #endregion

        #region Methods

        internal IEnumerable<BEA.Address> ReturnChild(IEnumerable<string> Keys, params Enum[] Relations)
        {
            StringBuilder sb = new();
            sb.AppendLine($@"SELECT  ""Address"" AS ""Name"", ""CardCode"", ""Street"" AS ""Address1"", ""Block"" AS ""Contact"", t0.""Country"", ""City"", t1.""Name"" AS ""State"", ""TaxCode"", ""AdresType"" AS ""Type"", ""Address2"", ""Address3"" ");
            sb.AppendLine($@"FROM    {DBSA}.CRD1 t0 ");
            sb.AppendLine($@"        LEFT OUTER JOIN {DBSA}.OCST t1 ON t0.""State"" = t1.""Code"" AND t0.""Country"" = t1.""Country"" ");
            sb.AppendLine($@"WHERE   ""CardCode"" IN ( {string.Join(",", Keys.Select(x => $"'{x}'"))} ) ");

            IEnumerable<BEA.Address> items = SQLList(sb.ToString(), Relations);
            return items;
        }

        protected override void LoadRelations(ref IEnumerable<BEA.Address> Items, params Enum[] Relations) { }

        protected override void LoadRelations(ref BEA.Address item, params Enum[] Relations) { }

        #endregion

        #region List Methods

        public IEnumerable<BEA.Address> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            var filter = GetFilter(FilterList?.ToArray());
            var sb = new StringBuilder();
            sb.AppendLine($@"SELECT  ""Address"" AS ""Name"", ""CardCode"", ""Street"" AS ""Address1"", ""Block"" AS ""Contact"", t0.""Country"", ""City"", t1.""Name"" AS ""State"", ""TaxCode"", ""AdresType"" AS ""Type"", ""Address2"", ""Address3"" ");
            sb.AppendLine($@"FROM    {DBSA}.CRD1 t0 ");
            sb.AppendLine($@"        LEFT OUTER JOIN {DBSA}.OCST t1 ON t0.""State"" = t1.""Code"" AND t0.""Country"" = t1.""Country"" ");
            sb.AppendLine($@"WHERE	{filter} ");
            sb.AppendLine($@"ORDER By {GetOrder(Order)}");

            IEnumerable<BEA.Address> items = SQLList(sb.ToString(), Relations);
            return items;
        }

        #endregion

        #region Search Methods

        public BEA.Address Search(string CardCode, string Name, string Type, params Enum[] Relations)
        {
            StringBuilder sb = new();
            sb.AppendLine($@"SELECT  ""Address"" AS ""Name"", ""CardCode"", ""Street"" AS ""Address1"", ""Block"" AS ""Contact"", t0.""Country"", ""City"", t1.""Name"" AS ""State"", ""TaxCode"", ""AdresType"" AS ""Type"", ""Address2"", ""Address3"" ");
            sb.AppendLine($@"FROM    {DBSA}.CRD1 t0 ");
            sb.AppendLine($@"        LEFT OUTER JOIN {DBSA}.OCST t1 ON t0.""State"" = t1.""Code"" AND t0.""Country"" = t1.""Country"" ");
            sb.AppendLine($@"WHERE	""CardCode"" = '{CardCode}' AND ""Address"" = '{Name}' AND ""AdresType"" = '{Type}' ");

            BEA.Address item = SQLSearch(sb.ToString(), Relations);
            return item;
        }

        #endregion

        #region Constructors

        public Address() : base() { }

        #endregion
    }
}
