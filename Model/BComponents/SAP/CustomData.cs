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
    public class CustomData : BCEntity
    {

        #region List Methods

        public IEnumerable<BEA.CustomData> List()
        {
            try
            {
                IEnumerable<BEA.CustomData> items = default;
                using (DALH.CustomData DALObject = new())
                {
                    items = DALObject.List();
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

        public CustomData() { }

        #endregion
    }
}
