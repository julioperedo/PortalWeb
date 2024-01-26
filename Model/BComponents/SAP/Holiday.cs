using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEA = BEntities.SAP;
using DALH = DALayer.SAP.Hana;

namespace BComponents.SAP
{
    [Serializable()]
    public class Holiday : BCEntity
    {
        #region Search Methods

        #endregion

        #region List Methods

        public IEnumerable<BEA.Holiday> List(params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.Holiday> Items = default;
                using DALH.Holiday DALObject = new();
                Items = DALObject.List(Relations);
                return Items;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        #endregion

        #region Constructors

        public Holiday() : base() { }

        #endregion
    }
}
