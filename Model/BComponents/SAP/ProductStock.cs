using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using BEA = BEntities.SAP;
using DALH = DALayer.SAP.Hana;

namespace BComponents.SAP
{
    [Serializable()]
    public class ProductStock : BCEntity
    {
        #region Search Methods

        public BEA.ProductStock Search(string Id)
        {
            BEA.ProductStock BEObject = default;
            try
            {
                using (DALH.ProductStock DALObject = new DALH.ProductStock())
                {
                    BEObject = DALObject.Search(Id);
                }
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
            }
            return BEObject;
        }

        #endregion

        #region List Methods

        public IEnumerable<BEA.ProductStock> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.ProductStock> BECollection = default;
                using (DALH.ProductStock DALObject = new())
                {
                    BECollection = DALObject.List(FilterList, Order, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.ProductStock> ListDisabledStock(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.ProductStock> BECollection = default;
                using (DALH.ProductStock DALObject = new())
                {
                    BECollection = DALObject.ListDisabledStock(FilterList, Order, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.ProductStock> ListWithCost(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.ProductStock> BECollection = default;
                using (DALH.ProductStock DALObject = new())
                {
                    BECollection = DALObject.ListWithCost(FilterList, Order, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.ProductStock> ListBalance(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.ProductStock> BECollection = default;
                using (DALH.ProductStock DALObject = new())
                {
                    BECollection = DALObject.ListBalance(FilterList, Order, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.ProductStock> ListFlow(string ItemCode, string DBName)
        {
            try
            {
                IEnumerable<BEA.ProductStock> BECollection = default;
                using (DALH.ProductStock DALObject = new())
                {
                    BECollection = DALObject.ListFlow(ItemCode, DBName);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.ProductStock> ListServiceMany(string ItemCodes)
        {
            try
            {
                IEnumerable<BEA.ProductStock> BECollection = default;
                using (DALH.ProductStock DALObject = new())
                {
                    BECollection = DALObject.ListServiceMany(ItemCodes);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.ProductStock> ListShort(List<Field> FilterList, string Order)
        {
            try
            {
                IEnumerable<BEA.ProductStock> BECollection = default;
                using (DALH.ProductStock DALObject = new())
                {
                    BECollection = DALObject.ListShort(FilterList, Order);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.ProductStock> ListRelated(string Categories, string ItemCodes, string Warehouses, int Quantity)
        {
            try
            {
                IEnumerable<BEA.ProductStock> BECollection = default;
                using (DALH.ProductStock DALObject = new())
                {
                    BECollection = DALObject.ListRelated(Categories, ItemCodes, Warehouses, Quantity);
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

        public ProductStock() : base() { }

        #endregion
    }
}
