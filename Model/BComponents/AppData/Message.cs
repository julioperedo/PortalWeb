using BComponents.Base;
using BComponents.Online;
using BComponents.Product;
using BComponents.Sales;
using BComponents.SAP;
using BComponents.Security;
using BComponents.Staff;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using BE = BEntities;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BED = BEntities.AppData;
using BEF = BEntities.Staff;
using BEL = BEntities.Sales;
using BEO = BEntities.Online;
using BEP = BEntities.Product;
using BES = BEntities.Security;
using DAL = DALayer.AppData;

namespace BComponents.AppData {
    public partial class Message {
        #region Save Methods 

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        public async Task<IEnumerable<BED.Message>> ListByUserAsync(long UserId, string SortingBy, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BED.Message> Items;
                using (DAL.Message dal = new())
                {
                    Items = await dal.ListByUserAsync(UserId, SortingBy, Relations);
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