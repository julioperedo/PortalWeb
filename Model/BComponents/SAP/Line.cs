using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using BEA = BEntities.SAP;
using DALH = DALayer.SAP.Hana;

namespace BComponents.SAP
{
    [Serializable()]
    public class Line : BCEntity
    {
        #region Search Methods

        #endregion

        #region List Methods

        public IEnumerable<BEA.Item> List(params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.Item> items = default;
                using (DALH.Line DALObject = new())
                {
                    items = DALObject.List(Relations);
                }
                return items;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        #endregion

        #region Constructors

        public Line() : base() { }

        #endregion
    }
}
