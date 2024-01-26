using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using BEA = BEntities.SAP;
using DAL = DALayer.SAP;

namespace BComponents.SAP
{
    [Serializable()]
    public class ProviderOrder : BCEntity
    {
        #region Search Methods

        public BEA.ProviderOrder Search(string Subsidiary, int DocNum)
        {
            BEA.ProviderOrder BEObject = null;
            try
            {
                using (DAL.ProviderOrder DALObject = new DAL.ProviderOrder())
                {
                    BEObject = DALObject.Search(Subsidiary, DocNum);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
            return BEObject;
        }

        #endregion

        #region List Methods

        public IEnumerable<BEA.Item> ListProviders()
        {
            try
            {
                IEnumerable<BEA.Item> BECollection = default;
                using (DAL.ProviderOrder DALObject = new())
                {
                    BECollection = DALObject.ListProviders();
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.ProviderOrder> ListFull(List<Field> FilterList, string Order)
        {
            try
            {
                IEnumerable<BEA.ProviderOrder> BECollection = default;
                using (DAL.ProviderOrder DALObject = new())
                {
                    BECollection = DALObject.ListFull(FilterList, Order);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.ProviderOrder> ListByItemRequested(string ItemCode, string Subsidiary, string WarehouseCode)
        {
            try
            {
                IEnumerable<BEA.ProviderOrder> BECollection = default;
                using (DAL.ProviderOrder DALObject = new())
                {
                    BECollection = DALObject.ListByItemRequested(ItemCode, Subsidiary, WarehouseCode);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.ProviderOrderItem> ListByItemRequestedDetails(string ItemCode, string Subsidiary, string WarehouseCode)
        {
            try
            {
                IEnumerable<BEA.ProviderOrderItem> BECollection = default;
                using (DAL.ProviderOrder DALObject = new())
                {
                    BECollection = DALObject.ListByItemRequestedDetails(ItemCode, Subsidiary, WarehouseCode);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.ProviderOrderItem> ListItems(List<Field> FilterList, string Order)
        {
            try
            {
                IEnumerable<BEA.ProviderOrderItem> BECollection = default;
                using (DAL.ProviderOrder DALObject = new())
                {
                    BECollection = DALObject.ListItems(FilterList, Order);
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

        public ProviderOrder() : base() { }

        #endregion
    }
}
