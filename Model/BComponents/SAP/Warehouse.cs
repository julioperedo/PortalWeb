using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BEA = BEntities.SAP;
using DALH = DALayer.SAP.Hana;

namespace BComponents.SAP
{
    [Serializable()]
    public class Warehouse : BCEntity
    {
        #region Search Methods

        #endregion

        #region List Methods

        public IEnumerable<BEA.Item> List(params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.Item> BECollection = default;
                using (DALH.Warehouse DALObject = new())
                {
                    BECollection = DALObject.List(Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.Item> ListIn(string Subsidiary, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.Item> BECollection = default;
                using (DALH.Warehouse DALObject = new())
                {
                    BECollection = DALObject.ListIn(Subsidiary, Relations);
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

        public Warehouse() : base() { }

        #endregion
    }
}
