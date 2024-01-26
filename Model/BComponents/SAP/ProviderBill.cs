using System;
using System.Collections.Generic;
using System.Text;
using BEA = BEntities.SAP;
using DAL = DALayer.SAP;

namespace BComponents.SAP
{
    [Serializable()]
    public class ProviderBill : BCEntity
    {

        #region List Methods

        public IEnumerable<BEA.ProviderBillItem> List(string Subsidiary, int DocNumber, string OrderBy)
        {
            try
            {
                IEnumerable<BEA.ProviderBillItem> BECollection = default;
                using (DAL.ProviderBill DALObject = new())
                {
                    BECollection = DALObject.ListItems(Subsidiary, DocNumber, OrderBy);
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

        #region Search Methods

        public BEA.ProviderBill Search(string Subsidiary, int DocNumber)
        {
            BEA.ProviderBill BEObject = null;
            try
            {
                using DAL.ProviderBill DALObject = new();
                BEObject = DALObject.Search(Subsidiary, DocNumber);
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
            return BEObject;
        }

        #endregion
    }
}
