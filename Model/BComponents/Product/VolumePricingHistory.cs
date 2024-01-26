using System;
using System.Collections.Generic;
using System.Transactions;
using BE = BEntities;
using BEP = BEntities.Product;

using DAL = DALayer.Product;
using BEntities.Filters;

namespace BComponents.Product
{
    public partial class VolumePricingHistory
    {
        #region Save Methods 

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        public IEnumerable<BEP.VolumePricingHistory> List2(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEP.VolumePricingHistory> BECollection;
                using (DAL.VolumePricingHistory DALObject = new())
                {
                    BECollection = DALObject.List2(FilterList, Order, Relations);
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

        #region Search Methods 

        #endregion
    }
}