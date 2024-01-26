using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using BEA = BEntities.SAP;
using DALH = DALayer.SAP.Hana;

namespace BComponents.SAP
{
    [Serializable()]
    public class ContactPerson : BCEntity
    {
        #region Search Methods

        public BEA.ContactPerson Search(int Code, params Enum[] Relations)
        {
            BEA.ContactPerson BEObject = null;
            try
            {
                using (DALH.ContactPerson DALObject = new DALH.ContactPerson())
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

        public IEnumerable<BEA.ContactPerson> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.ContactPerson> BECollection = default(List<BEA.ContactPerson>);
                using (DALH.ContactPerson DALObject = new DALH.ContactPerson())
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

        public ContactPerson() : base() { }

        #endregion
    }
}
