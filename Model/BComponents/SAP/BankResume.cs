using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using BEA = BEntities.SAP;
using DALH = DALayer.SAP.Hana;

namespace BComponents.SAP
{
    [Serializable()]
    public class BankResume : BCEntity
    {
        #region Search Methods

        #endregion

        #region List Methods

        public IEnumerable<BEA.BankResume> List(List<Field> FilterList, string Order)
        {
            try
            {
                IEnumerable<BEA.BankResume> BECollection = default;
                using (DALH.BankResume DALObject = new())
                {
                    BECollection = DALObject.List(FilterList, Order);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        #endregion

        #region Constructors

        public BankResume() : base() { }

        #endregion
    }
}
