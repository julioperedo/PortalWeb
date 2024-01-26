using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using BEA = BEntities.SAP;
using DALH = DALayer.SAP.Hana;

namespace BComponents.SAP
{
    [Serializable()]
    public class BalanceDetail : BCEntity
    {
        #region Search Methods

        #endregion

        #region List Methods

        public IEnumerable<BEA.BalanceDetail> ListClientsSA(List<Field> FilterList, string Order)
        {
            try
            {
                IEnumerable<BEA.BalanceDetail> BECollection = default;
                using (DALH.BalanceDetail DALObject = new())
                {
                    BECollection = DALObject.ListClientsSA(FilterList, Order);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.BalanceDetail> ListProvidersSA(List<Field> FilterList, string Order)
        {
            try
            {
                IEnumerable<BEA.BalanceDetail> BECollection = default;
                using (DALH.BalanceDetail DALObject = new())
                {
                    BECollection = DALObject.ListProvidersSA(FilterList, Order);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.BalanceDetail> ListClientsIQ(List<Field> FilterList, string Order)
        {
            try
            {
                IEnumerable<BEA.BalanceDetail> BECollection = default;
                using (DALH.BalanceDetail DALObject = new())
                {
                    BECollection = DALObject.ListClientsIQ(FilterList, Order);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.BalanceDetail> ListProvidersIQ(List<Field> FilterList, string Order)
        {
            try
            {
                IEnumerable<BEA.BalanceDetail> BECollection = default;
                using (DALH.BalanceDetail DALObject = new DALH.BalanceDetail())
                {
                    BECollection = DALObject.ListProvidersIQ(FilterList, Order);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.BalanceDetail> ListClientsLA(List<Field> FilterList, string Order)
        {
            try
            {
                IEnumerable<BEA.BalanceDetail> BECollection = default;
                using (DALH.BalanceDetail DALObject = new DALH.BalanceDetail())
                {
                    BECollection = DALObject.ListClientsLA(FilterList, Order);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.BalanceDetail> ListProvidersLA(List<Field> FilterList, string Order)
        {
            try
            {
                IEnumerable<BEA.BalanceDetail> BECollection = default;
                using (DALH.BalanceDetail DALObject = new DALH.BalanceDetail())
                {
                    BECollection = DALObject.ListProvidersLA(FilterList, Order);
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

        public BalanceDetail() : base() { }

        #endregion
    }
}
