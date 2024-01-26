using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using BE = BEntities;
using BEA = BEntities.SAP;

namespace DALayer.SAP.Hana
{
    [Serializable()]
    public class ContactPerson : DALEntity<BEA.ContactPerson>
    {
        #region Global Variables

        #endregion

        #region Methods

        internal IEnumerable<BEA.ContactPerson> ReturnChild(IEnumerable<string> Keys, params Enum[] Relations)
        {
            StringBuilder sb = new();
            sb.AppendLine($@"SELECT  ""CntctCode"" AS ""Id"", ""CardCode"", ""Name"", ""Position"", ""Address"", ""Tel1"" AS ""Phone1"", ""Tel2"" AS ""Phone2"", ""Cellolar"" AS ""Cellular"", ""Fax"", ""E_MailL"" AS ""EMail"", ""Pager"" AS ""IdentityCard"", ""Title"", ""Active"" AS ""Enabled"", ""FirstName"", ""MiddleName"", ""LastName"" ");
            sb.AppendLine($@"FROM    {DBSA}.OCPR ");
            sb.AppendLine($@"WHERE   ""CntctCode"" IN ( {string.Join(",", Keys)} ) ");

            IEnumerable<BEA.ContactPerson> items = SQLList(sb.ToString(), Relations);
            return items;
        }

        protected override void LoadRelations(ref IEnumerable<BEA.ContactPerson> Items, params Enum[] Relations) { }

        protected override void LoadRelations(ref BEA.ContactPerson item, params Enum[] Relations) { }

        #endregion

        #region List Methods

        public IEnumerable<BEA.ContactPerson> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            var filter = GetFilter(FilterList?.ToArray());
            StringBuilder sb = new();
            sb.AppendLine($@"SELECT  ""CntctCode"" AS ""Id"", ""CardCode"", ""Name"", ""Position"", ""Address"", ""Tel1"" AS ""Phone1"", ""Tel2"" AS ""Phone2"", ""Cellolar"" AS ""Cellular"", ""Fax"", ""E_MailL"" AS ""EMail"", ""Pager"" AS ""IdentityCard"", ""Title"", ""Active"" AS ""Enabled"", ""FirstName"", ""MiddleName"", ""LastName"" ");
            sb.AppendLine($@"FROM    {DBSA}.OCPR ");
            sb.AppendLine($@"WHERE   {filter} ");
            sb.AppendLine($@"ORDER By {GetOrder(Order)}");

            IEnumerable<BEA.ContactPerson> items = SQLList(sb.ToString(), Relations);
            return items;
        }

        #endregion

        #region Search Methods

        public BEA.ContactPerson Search(int Code, params Enum[] Relations)
        {
            StringBuilder sb = new();
            sb.AppendLine($@"SELECT  ""CntctCode"" AS ""Id"", ""CardCode"", ""Name"", ""Position"", ""Address"", ""Tel1"" AS ""Phone1"", ""Tel2"" AS ""Phone2"", ""Cellolar"" AS ""Cellular"", ""Fax"", ""E_MailL"" AS ""EMail"", ""Pager"" AS ""IdentityCard"", ""Title"", ""Active"" AS ""Enabled"", ""FirstName"", ""MiddleName"", ""LastName"" ");
            sb.AppendLine($@"FROM    {DBSA}.OCPR ");
            sb.AppendLine($@"WHERE   ""CntctCode"" = {Code} ");

            BEA.ContactPerson item = SQLSearch(sb.ToString(), Relations);
            return item;
        }

        #endregion

        #region Constructors

        public ContactPerson() : base() { }

        #endregion
    }
}
