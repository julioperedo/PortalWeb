using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using BEA = BEntities.SAP;
using DALH = DALayer.SAP.Hana;

namespace BComponents.SAP
{
    [Serializable()]
    public class OrdersResume : BCEntity
    {
        #region Search Methods

        #endregion

        #region List Methods

        public IEnumerable<BEA.OrdersResume> List(DateTime InitialDate, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.OrdersResume> BECollection = default(List<BEA.OrdersResume>);
                using (DALH.OrdersResume DALObject = new())
                {
                    BECollection = DALObject.List(InitialDate, Relations);
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

        public OrdersResume() : base() { }

        #endregion
    }
}
