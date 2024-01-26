using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using BEA = BEntities.SAP;
using DAL = DALayer.SAP;

namespace BComponents.SAP
{
    [Serializable()]
    public class StateAccount : BCEntity
    {
        #region Search Methods

        #endregion

        #region List Methods

        public IEnumerable<BEA.StateAccountItem> List(List<Field> FilterList, string Order)
        {
            try
            {
                IEnumerable<BEA.StateAccountItem> BECollection = default;
                using (DAL.StateAccount DALObject = new())
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

        public IEnumerable<BEA.Seller> ListProductManagers()
        {
            try
            {
                IEnumerable<BEA.Seller> BECollection = default;
                using (DAL.StateAccount DALObject = new())
                {
                    BECollection = DALObject.ListProductManagers();
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.ProviderStateAccountItem> ListStateAccountProvider(List<Field> FilterList, string Order)
        {
            try
            {
                IEnumerable<BEA.ProviderStateAccountItem> BECollection = default;
                using (DAL.StateAccount DALObject = new())
                {
                    BECollection = DALObject.ListStateAccountProvider(FilterList, Order);
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

        public StateAccount() : base() { }

        #endregion
    }
}
