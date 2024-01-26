using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using BEA = BEntities.SAP;
using DALH = DALayer.SAP.Hana;

namespace BComponents.SAP
{
    [Serializable()]
    public class Address : BCEntity
    {
        #region Search Methods

        public BEA.Address Search(string CardCode, string Name, string Type, params Enum[] Relations)
        {
            BEA.Address BEObject = null;
            try
            {
                using DALH.Address DALObject = new();
                BEObject = DALObject.Search(CardCode, Name, Type, Relations);
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
            return BEObject;
        }

        #endregion

        #region List Methods

        public IEnumerable<BEA.Address> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.Address> BECollection = default(List<BEA.Address>);
                using (DALH.Address DALObject = new())
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

        public Address() : base() { }

        #endregion
    }
}
