using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using BEA = BEntities.SAP;
using DALH = DALayer.SAP.Hana;

namespace BComponents.SAP
{
    [Serializable()]
    public class Attachment : BCEntity
    {
        #region Search Methods

        public BEA.Attachment Search(string CardCode, int Line, params Enum[] Relations)
        {
            BEA.Attachment BEObject = null;
            try
            {
                using DALH.Attachment DALObject = new();
                BEObject = DALObject.Search(CardCode, Line, Relations);
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
            return BEObject;
        }

        #endregion

        #region List Methods

        public IEnumerable<BEA.Attachment> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.Attachment> BECollection = default(List<BEA.Attachment>);
                using (DALH.Attachment DALObject = new())
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

        public Attachment() : base() { }

        #endregion
    }
}