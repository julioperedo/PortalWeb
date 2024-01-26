using BComponents.Base;
using BComponents.Security;
using BEntities.Filters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Transactions;
using BE = BEntities;
using BEA = BEntities.SAP;
using DALH = DALayer.SAP.Hana;

namespace BComponents.SAP
{
    [Serializable()]
    public class User : BCEntity
    {
        #region Search Methods

        //public BEA.Seller Search(string SellerCode, params Enum[] Relations)
        //{
        //    BEA.Seller BEObject = null;
        //    try
        //    {
        //        using (DALH.Seller DALObject = new DALH.Seller())
        //        {
        //            BEObject = DALObject.Search(SellerCode, Relations);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler(ex);
        //    }
        //    return BEObject;
        //}

        #endregion

        #region List Methods

        public async Task<IEnumerable<BEA.User>> ListAsync(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.User> BECollection = default;
                using (DALH.User DALObject = new())
                {
                    BECollection = await DALObject.ListAsync(FilterList, Order, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        //public IEnumerable<BEA.Seller> ListWithClient(List<Field> FilterList, string Order, params Enum[] Relations)
        //{
        //    try
        //    {
        //        IEnumerable<BEA.Seller> BECollection = default;
        //        using (DALH.Seller DALObject = new())
        //        {
        //            BECollection = DALObject.ListWithClient(FilterList, Order, Relations);
        //        }
        //        return BECollection;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler(ex);
        //        return null;
        //    }
        //}

        //public IEnumerable<BEA.Seller> ProductManagers(List<Field> FilterList, string Order, params Enum[] Relations)
        //{
        //    try
        //    {
        //        IEnumerable<BEA.Seller> BECollection = default;
        //        using (DALH.Seller DALObject = new())
        //        {
        //            BECollection = DALObject.ProductManagers(FilterList, Order, Relations);
        //        }
        //        return BECollection;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler(ex);
        //        return null;
        //    }
        //}

        #endregion

        #region Constructors

        public User() : base() { }

        #endregion
    }
}