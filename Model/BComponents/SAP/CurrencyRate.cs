using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEA = BEntities.SAP;
using DALH = DALayer.SAP.Hana;

namespace BComponents.SAP
{
    public class CurrencyRate : BCEntity
    {
        #region Search Methods

        public BEA.CurrencyRate Search(DateTime Date)
        {
            BEA.CurrencyRate item = null;
            try
            {
                using DALH.CurrencyRate DALObject = new();
                item = DALObject.Search(Date);
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
            }
            return item;
        }

        #endregion

        #region Constructors

        public CurrencyRate() : base() { }

        #endregion
    }
}
