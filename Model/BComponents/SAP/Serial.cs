using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using BEA = BEntities.SAP;
using DALH = DALayer.SAP.Hana;

namespace BComponents.SAP
{

    [Serializable()]
    public class Serial : BCEntity
    {

        #region List Methods

        public IEnumerable<BEA.Serial> List(List<Field> Filters, string OrderBy)
        {
            try
            {
                IEnumerable<BEA.Serial> BECollection = default;
                using (DALH.Serial DALObject = new())
                {
                    BECollection = DALObject.List(Filters, OrderBy);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        #endregion

        #region Constructors

        public Serial() : base() { }

        #endregion
    }
}
