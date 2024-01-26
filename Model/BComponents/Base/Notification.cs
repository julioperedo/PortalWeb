using BComponents.Product;
using BComponents.Sales;
using BComponents.SAP;
using BComponents.Security;
using System;
using System.Collections.Generic;
using System.Transactions;
using BE = BEntities;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BEL = BEntities.Sales;
using BEP = BEntities.Product;
using BES = BEntities.Security;
using DAL = DALayer.Base;

namespace BComponents.Base
{
    public partial class Notification
    {

        #region Save Methods 

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        public List<BEB.Notification> List(string CardCode, string Name, DateTime? Since, DateTime? Until, bool? Enabled, string Order, params Enum[] Relations)
        {
            try
            {
                List<BEB.Notification> BECollection;
                using (DAL.Notification DALObject = new DAL.Notification())
                {
                    BECollection = DALObject.List(CardCode, Name, Since, Until, Enabled, Order, Relations);
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