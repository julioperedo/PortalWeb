using System;
using System.Collections.Generic;
using System.Transactions;
using BE = BEntities;
using BEP = BEntities.Product;

using DAL = DALayer.Product;
using BEntities.Filters;

namespace BComponents.Product
{
    public partial class Line
    {
        #region Save Methods 

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        public IEnumerable<BEP.Line> ListExtended(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEP.Line> BECollection;
                using (DAL.Line DALObject = new())
                {
                    BECollection = DALObject.ListExtended(FilterList, Order, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEP.Line> ListForPriceList(string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEP.Line> BECollection;
                using (DAL.Line DALObject = new())
                {
                    BECollection = DALObject.ListForPriceList(Order, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEP.Line> ListForPriceList(bool Team, string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEP.Line> BECollection;
                using (DAL.Line DALObject = new())
                {
                    BECollection = DALObject.ListForPriceList(Team, Order, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEP.Line> ListForPriceList(string IdProducts, string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEP.Line> BECollection;
                using (DAL.Line DALObject = new())
                {
                    BECollection = DALObject.ListForPriceList(IdProducts, Order, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        //public List<BEP.Line> ListForPriceListByImage(string ImageName, string Order, params Enum[] Relations)
        //{
        //    try
        //    {
        //        List<BEP.Line> BECollection;
        //        using (DAL.Line DALObject = new())
        //        {
        //            BECollection = DALObject.ListForPriceListByImage(ImageName, Order, Relations);
        //        }
        //        return BECollection;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler(ex);
        //        return null;
        //    }
        //}

        //public List<BEP.Line> ListForWeb(string Order, params Enum[] Relations)
        //{
        //    try
        //    {
        //        List<BEP.Line> BECollection;
        //        using (DAL.Line DALObject = new())
        //        {
        //            BECollection = DALObject.ListForWeb(Order, Relations);
        //        }
        //        return BECollection;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler(ex);
        //        return null;
        //    }
        //}

        public IEnumerable<BEP.Line> ListForOffers()
        {
            try
            {
                IEnumerable<BEP.Line> BECollection;
                using (DAL.Line DALObject = new())
                {
                    BECollection = DALObject.ListForOffers();
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

        #region Search Methods 

        public BEP.Line First(params Enum[] Relations)
        {
            BEP.Line BEObject = null;
            try
            {
                using DAL.Line DALObject = new();
                BEObject = DALObject.First(Relations);
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
            return BEObject;
        }

        public BEP.Line SearchByProduct(long IdProduct, params Enum[] Relations)
        {
            BEP.Line BEObject = null;
            try
            {
                using DAL.Line DALObject = new();
                BEObject = DALObject.SearchByProduct(IdProduct, Relations);
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
            }
            return BEObject;
        }

        #endregion
    }
}