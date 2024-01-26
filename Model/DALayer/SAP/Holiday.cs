using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEA = BEntities.SAP;

namespace DALayer.SAP.Hana
{
    [Serializable()]
    public class Holiday : DALEntity<BEA.Holiday>
    {
        #region Global Variables

        //readonly string queryBase;

        #endregion

        #region Methods

        protected override void LoadRelations(ref BEA.Holiday Item, params Enum[] Relations) { }

        protected override void LoadRelations(ref IEnumerable<BEA.Holiday> Items, params Enum[] Relations) { }

        #endregion

        #region List Methods

        public IEnumerable<BEA.Holiday> List(params Enum[] Relations)
        {
            string query = $@"SELECT  ""HldCode"" AS ""Year"", CAST(""StrDate"" AS DATE) AS ""Since"", CAST(""EndDate"" AS DATE) AS ""Until"", ""Rmrks"" AS ""Name""  
FROM    {DBSA}.HLD1 
WHERE   CAST(""StrDate"" AS DATE) >= CURRENT_DATE  
ORDER BY 2 ";

            IEnumerable<BEA.Holiday> items = SQLList(query, Relations);
            return items;
        }

        #endregion

        #region Search Methods

        #endregion

        #region Private Methods

        #endregion

        #region Constructors

        public Holiday() : base()
        {
            //            queryBase = $@"SELECT  ""empID"" AS ""Id"", TRIM(""firstName"" || ' ' || IFNULL(""middleName"", '')) AS ""FirstName"", TRIM(""lastName"" || ' ' || IFNULL(""U_ApMaterno"", '')) AS ""LastName"", CAST(""startDate"" AS DATE) AS ""StartDate""
            //		, CAST(""termDate"" AS DATE) AS ""TermDate"", ""picture"" AS ""Picture""
            //FROM    {DBSA}.OHEM ";
        }

        #endregion


    }
}
