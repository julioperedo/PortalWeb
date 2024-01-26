using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using BEA = BEntities.SAP;
using DALH = DALayer.SAP.Hana;

namespace BComponents.SAP
{
    [Serializable()]
    public class Subcategory : BCEntity
    {
        #region Search Methods

        #endregion

        #region List Methods

        public IEnumerable<BEA.Item> List(int IdCategory, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.Item> BECollection = default;
                using (DALH.Subcategory DALObject = new())
                {
                    BECollection = DALObject.List(IdCategory, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.Item> List(string Category, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.Item> BECollection = default;
                using (DALH.Subcategory DALObject = new())
                {
                    BECollection = DALObject.List(Category, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.Item> ListIn(string Categories, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.Item> BECollection = default;
                using (DALH.Subcategory DALObject = new())
                {
                    BECollection = DALObject.ListIn(Categories, Relations);
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

        public Subcategory() : base() { }

        #endregion
    }
}
