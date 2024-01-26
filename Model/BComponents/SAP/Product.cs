using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using BEA = BEntities.SAP;
using DALH = DALayer.SAP.Hana;

namespace BComponents.SAP
{
    [Serializable()]
    public class Product : BCEntity
    {
        #region Search Methods

        public BEA.Product SearchSC(string Id, params Enum[] Relations)
        {
            BEA.Product BEObject = null;
            try
            {
                using DALH.Product DALObject = new();
                BEObject = DALObject.SearchSC(Id, Relations);
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
            }
            return BEObject;
        }

        public BEA.Product SearchLA(string Id, params Enum[] Relations)
        {
            BEA.Product BEObject = null;

            try
            {
                using DALH.Product DALObject = new();
                BEObject = DALObject.SearchLA(Id, Relations);
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
            }
            return BEObject;
        }

        public BEA.Product SearchIQQ(string Id, params Enum[] Relations)
        {
            BEA.Product BEObject = null;

            try
            {
                using DALH.Product DALObject = new();
                BEObject = DALObject.SearchIQQ(Id, Relations);
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
            }
            return BEObject;
        }

        public BEA.Product SearchFlow(string Id, string DBName)
        {
            BEA.Product BEObject = default;
            try
            {
                using DALH.Product DALObject = new();
                BEObject = DALObject.SearchFlow(Id, DBName);
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
            }
            return BEObject;
        }

        #endregion

        #region List Methods

        public IEnumerable<BEA.Product> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.Product> BECollection = default;
                using (DALH.Product DALObject = new())
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

        public IEnumerable<BEA.Product> ListCatalog(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.Product> BECollection = default;
                using (DALH.Product DALObject = new())
                {
                    BECollection = DALObject.ListCatalog(FilterList, Order, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.Product> ListCatalog2(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.Product> BECollection = default;
                using (DALH.Product DALObject = new())
                {
                    BECollection = DALObject.ListCatalog2(FilterList, Order, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.Product> ListSC(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.Product> BECollection = default;
                using (DALH.Product DALObject = new())
                {
                    BECollection = DALObject.ListSC(FilterList, Order, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.Product> ListLA(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.Product> BECollection = default;
                using (DALH.Product DALObject = new())
                {
                    BECollection = DALObject.ListLA(FilterList, Order, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.Product> ListIQQ(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.Product> BECollection = default;
                using (DALH.Product DALObject = new())
                {
                    BECollection = DALObject.ListIQQ(FilterList, Order, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.Product> ListOnlyLA(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.Product> BECollection = default;
                using (DALH.Product DALObject = new())
                {
                    BECollection = DALObject.ListOnlyLA(FilterList, Order, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.Product> ListOnlyIQQ(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.Product> BECollection = default;
                using (DALH.Product DALObject = new())
                {
                    BECollection = DALObject.ListOnlyIQQ(FilterList, Order, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.Product> ListPMs(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.Product> BECollection = default;
                using (DALH.Product DALObject = new())
                {
                    BECollection = DALObject.ListPMs(FilterList, Order, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.Product> ListTopSales(string Codes, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.Product> BECollection = default;
                using (DALH.Product DALObject = new())
                {
                    BECollection = DALObject.ListTopSales(Codes, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.Product> ListFactoryCodes(List<Field> FilterList, string Order = "1", params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.Product> BECollection = default;
                using (DALH.Product DALObject = new())
                {
                    BECollection = DALObject.ListFactoryCodes(FilterList, Order, Relations);
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

        public Product() : base() { }

        #endregion
    }
}
