using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using BEA = BEntities.SAP;
using DALH = DALayer.SAP.Hana;

namespace BComponents.SAP
{
    [Serializable()]
    public class CreditNote : BCEntity
    {
        #region Search Methods

        public BEA.CreditNote Search(string Subsidiary, long DocNumber)
        {
            BEA.CreditNote item = null;
            try
            {
                using (DALH.CreditNote DALObject = new DALH.CreditNote())
                {
                    item = DALObject.Search(Subsidiary, DocNumber);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
            return item;
        }

        #endregion

        #region List Methods

        public IEnumerable<BEA.CreditNote> List(List<Field> Filters, string SortingBy)
        {
            try
            {
                IEnumerable<BEA.CreditNote> items = default;
                using (DALH.CreditNote DALObject = new())
                {
                    items = DALObject.List(Filters, SortingBy);
                }
                return items;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.CreditNoteItem> ListItems(string Subsidiary, long DocNumber)
        {
            try
            {
                IEnumerable<BEA.CreditNoteItem> items = default;
                using (DALH.CreditNote DALObject = new())
                {
                    items = DALObject.ListItems(Subsidiary, DocNumber);
                }
                return items;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        #endregion

        #region Constructors

        public CreditNote() : base() { }

        #endregion
    }
}
