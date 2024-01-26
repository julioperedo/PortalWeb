using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using BEA = BEntities.SAP;
using DALH = DALayer.SAP.Hana;

namespace BComponents.SAP
{
    [Serializable()]
    public class Category : BCEntity
    {
        #region Search Methods

        public BEA.Item Search(int Code, params Enum[] Relations)
        {
            BEA.Item BEObject = null;
            try
            {
                using (DALH.Category DALObject = new DALH.Category())
                {
                    BEObject = DALObject.Search(Code, Relations);
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

        public IEnumerable<BEA.Item> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.Item> BECollection = default(List<BEA.Item>);
                using (DALH.Category DALObject = new DALH.Category())
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

        #endregion

        #region Constructors

        public Category() : base() { }

        #endregion
    }
}
