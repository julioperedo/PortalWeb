using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE = BEntities;
using BEA = BEntities.SAP;

namespace DALayer.SAP.Hana
{
    [Serializable()]
    public class CustomData : DALEntity<BEA.CustomData>
    {
        #region Methods

        protected override void LoadRelations(ref BEA.CustomData Item, params Enum[] Relations) { }

        protected override void LoadRelations(ref IEnumerable<BEA.CustomData> Items, params Enum[] Relations) { }

        #endregion

        #region List Methods

        public IEnumerable<BEA.CustomData> List()
        {
            string query = $@"SELECT  ""U_ValProF1"" AS ""Days"", CAST(""U_ValProF2"" AS INT) AS ""InitialYear"", CAST(""U_ValProF3"" AS INT) AS ""FinalYear"" 
FROM    ""{DBSA_PL}"".""@ADPR"" 
WHERE   ""U_Entidad"" = 'PLVAC' 
ORDER BY 2 ";
            IEnumerable<BEA.CustomData> items = SQLList(query);
            return items;
        }

        #endregion

        #region Search Methods

        #endregion

        #region Constructors

        public CustomData() : base() { }



        #endregion
    }
}
