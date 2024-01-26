using DALayer.SAP.Hana;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BE = BEntities;
using BEA = BEntities.SAP;

namespace DALayer.SAP.Hana
{
    [Serializable()]
    public class CurrencyRate : DALEntity<BEA.CurrencyRate>
    {
        #region Methods

        protected override void LoadRelations(ref BEA.CurrencyRate Item, params Enum[] Relations)
        {
            //throw new NotImplementedException();
        }

        protected override void LoadRelations(ref IEnumerable<BEA.CurrencyRate> Items, params Enum[] Relations)
        {
            //throw new NotImplementedException();
        }

        #endregion

        #region Search Methods

        public BEA.CurrencyRate Search(DateTime Date)
        {
            string query = $@"SELECT  *
                              FROM    {DBSA}.ORTT
                              WHERE   ""Currency"" = 'Bs' AND CAST(""RateDate"" AS DATE) = '{Date:yyyy-MM-dd}' ";
            BEA.CurrencyRate item = SQLSearch(query);
            return item;
        }

        #endregion

        #region Constructors

        public CurrencyRate() : base() { }



        #endregion

    }
}
