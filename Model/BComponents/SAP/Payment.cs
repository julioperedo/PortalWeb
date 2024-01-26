using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using BEA = BEntities.SAP;
using DAL = DALayer.SAP;

namespace BComponents.SAP
{
    [Serializable()]
    public class Payment : BCEntity
    {
        #region Search Methods
        public int SearchAVGDuedays(string CardCode)
        {
            int result;
            try
            {
                using DAL.Payment DALObject = new();
                result = DALObject.SearchAVGDuedays(CardCode);
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return 0;
            }
            return result;
        }
        #endregion

        #region List Methods

        public IEnumerable<BEA.Payment> List(List<Field> FilterList, string Order)
        {
            try
            {
                IEnumerable<BEA.Payment> BECollection = default;
                using (DAL.Payment DALObject = new())
                {
                    BECollection = DALObject.List(FilterList, Order);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.Payment> List(string BillCode, DateTime? Initial, DateTime? Final, int? ReceiptCode, int? NoteCode, string Order)
        {
            try
            {
                IEnumerable<BEA.Payment> BECollection = default;
                using (DAL.Payment DALObject = new())
                {
                    BECollection = DALObject.List(BillCode, Initial, Final, ReceiptCode, NoteCode, Order);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.Payment> ListAdjustments(List<Field> FilterList, string Order)
        {
            try
            {
                IEnumerable<BEA.Payment> BECollection = default;
                using (DAL.Payment DALObject = new())
                {
                    BECollection = DALObject.ListAdjustments(FilterList, Order);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.Payment> ListAdjustment(string Subsidiary, int AdjustCode, string State)
        {
            try
            {
                IEnumerable<BEA.Payment> lstResult = default;
                using (DAL.Payment DALObject = new())
                {
                    lstResult = DALObject.ListAdjustment(Subsidiary, AdjustCode, State);
                }
                return lstResult;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.PaymentItem> ListAdjustmentItems(string Subsidiary, int DocNum, string SortingBy)
        {
            try
            {
                IEnumerable<BEA.PaymentItem> BECollection = default;
                using (DAL.Payment DALObject = new())
                {
                    BECollection = DALObject.ListAdjustmentItems(Subsidiary, DocNum , SortingBy);
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

        #region Constructors

        public Payment() : base() { }

        #endregion
    }
}
