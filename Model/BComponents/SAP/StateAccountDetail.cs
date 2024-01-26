using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using BEA = BEntities.SAP;
using DALH = DALayer.SAP.Hana;

namespace BComponents.SAP
{
    [Serializable()]
    public class StateAccountDetail : BCEntity
    {
        #region Search Methods

        #endregion

        #region List Methods

        public IEnumerable<BEA.StateAccountDetail> List(List<Field> FilterList, string Order)
        {
            try
            {
                IEnumerable<BEA.StateAccountDetail> BECollection = default;
                using (DALH.StateAccountDetail DALObject = new())
                {
                    BECollection = DALObject.List(FilterList, Order);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.StateAccountDetail> List2(List<Field> FilterList, string Order)
        {
            try
            {
                IEnumerable<BEA.StateAccountDetail> BECollection = default;
                using (DALH.StateAccountDetail DALObject = new())
                {
                    BECollection = DALObject.List2(FilterList, Order);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.StateAccountDetail> ListResume(List<Field> FilterList, string Order)
        {
            try
            {
                IEnumerable<BEA.StateAccountDetail> BECollection = default;
                using (DALH.StateAccountDetail DALObject = new())
                {
                    BECollection = DALObject.ListResume(FilterList, Order);
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

        public StateAccountDetail() : base() { }

        #endregion
    }
}
