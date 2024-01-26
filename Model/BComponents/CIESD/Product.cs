using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using BE = BEntities;
using BEC = BEntities.CIESD;
using DAL = DALayer.CIESD;

namespace BComponents.CIESD
{
    public partial class Product
    {
        #region Save Methods 

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        public IEnumerable<BEC.Product> ListAvailable(string SortBy, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEC.Product> Items = default;
                using (DAL.Product dal = new())
                {
                    Items = dal.ListAvailable(SortBy, Relations);
                }
                return Items;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public async Task<IEnumerable<BEC.Product>> ListAvailableAsync(string SortBy, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEC.Product> Items = default;
                using (DAL.Product dal = new())
                {
                    Items = await dal.ListAvailableAsync(SortBy, Relations);
                }
                return Items;
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