using BComponents.Base;
using BComponents.Security;
using BEntities.Filters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Transactions;
using BE = BEntities;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BES = BEntities.Security;
using DALH = DALayer.SAP.Hana;

namespace BComponents.SAP
{
    [Serializable()]
    public class DeliveryNote : BCEntity
    {
        #region Search Methods

        public BEA.DeliveryNote Search(int Id, string Subsidiary, params Enum[] Relations)
        {
            BEA.DeliveryNote BEObject = default;
            try
            {
                using (DALH.DeliveryNote DALObject = new DALH.DeliveryNote())
                {
                    BEObject = DALObject.Search(Id, Subsidiary, Relations);
                }
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
            }
            return BEObject;
        }

        public BEA.DeliveryNote SearchById(int Id, string Subsidiary, params Enum[] Relations)
        {
            BEA.DeliveryNote BEObject = default;
            try
            {
                using (DALH.DeliveryNote DALObject = new DALH.DeliveryNote())
                {
                    BEObject = DALObject.SearchById(Id, Subsidiary, Relations);
                }
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
            }
            return BEObject;
        }

        public BEA.DeliveryNote SearchBySaleNote(int Id, string Subsidiary, params Enum[] Relations)
        {
            BEA.DeliveryNote BEObject = default;
            try
            {
                using (DALH.DeliveryNote DALObject = new DALH.DeliveryNote())
                {
                    BEObject = DALObject.SearchBySaleNote(Id, Subsidiary, Relations);
                }
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
            }
            return BEObject;
        }

        #endregion

        #region List Methods

        public IEnumerable<BEA.DeliveryNote> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.DeliveryNote> BECollection = default;
                using (DALH.DeliveryNote DALObject = new())
                {
                    BECollection = DALObject.List(FilterList, Order, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.DeliveryNote> List(List<Field> FilterList, List<Field> ItemFilters, string OrderBy, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.DeliveryNote> BECollection = default;
                using (DALH.DeliveryNote DALObject = new())
                {
                    BECollection = DALObject.List(FilterList, ItemFilters, OrderBy, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.DocumentRelated> ListRelatedDocuments(string Subsidiary, string DocIds)
        {
            try
            {
                IEnumerable<BEA.DocumentRelated> Items = default;
                if (!string.IsNullOrEmpty(DocIds))
                {
                    using DALH.DeliveryNote DALObject = new();
                    Items = DALObject.ListRelatedDocuments(Subsidiary, DocIds);
                }
                return Items;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        //public List<BEA.Note> ListBySection(string Section, List<Field> FilterList, string Order, params Enum[] Relations) {
        //    try {
        //        List<BEA.Note> BECollection = default(List<BEA.Note>);
        //        string section = Section?.Length > 0 ? Section[0].ToString().ToUpper() : "";
        //        using (DALH.Note DALObject = new DALH.Note()) {
        //            BECollection = DALObject.ListBySection(section, FilterList, Order, Relations);
        //        }
        //        return BECollection;
        //    } catch (Exception ex) {
        //        ErrorHandler(ex);
        //        return null;
        //    }
        //}

        //public List<BEA.Note> List(string CardCode, string ClientOrder, string Subsidiary, string StoreHouse, DateTime? InitialDate, DateTime? FinalDate, string Salesman, string ItemCode, string Line, string Category, string Subcategory, string Order, params Enum[] Relations) {
        //    try {
        //        List<BEA.Note> BECollection = default(List<BEA.Note>);
        //        using (DALH.Note DALObject = new DALH.Note()) {
        //            BECollection = DALObject.List(CardCode, ClientOrder, Subsidiary, StoreHouse, InitialDate, FinalDate, Salesman, ItemCode, Line, Category, Subcategory, Order, Relations);
        //        }
        //        return BECollection;
        //    } catch (Exception ex) {
        //        ErrorHandler(ex);
        //        return null;
        //    }
        //}

        //public List<BEA.Note> ListWithoutOrder(List<Field> FilterList, string Order, params Enum[] Relations) {
        //    try {
        //        List<BEA.Note> BECollection = default;
        //        using (DALH.Note DALObject = new DALH.Note()) {
        //            BECollection = DALObject.ListWithoutOrder(FilterList, Order, Relations);
        //        }
        //        return BECollection;
        //    } catch (Exception ex) {
        //        base.ErrorHandler(ex);
        //        return null;
        //    }
        //}

        public IEnumerable<BEA.DeliveryNoteItem> ListItems(List<Field> FilterList, string Order)
        {
            try
            {
                IEnumerable<BEA.DeliveryNoteItem> BECollection = default;
                using (DALH.DeliveryNote DALObject = new())
                {
                    BECollection = DALObject.ListItems(FilterList, Order);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.DeliveryNoteItem> ListItems2(IEnumerable<string> Keys, string Order)
        {
            try
            {
                IEnumerable<BEA.DeliveryNoteItem> BECollection = default;
                using (DALH.DeliveryNote DALObject = new())
                {
                    BECollection = DALObject.ListItems2(Keys, Order);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.DeliveryNote> ListOpen(List<Field> Filters, List<Field> InnerFilters, string OrderBy, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.DeliveryNote> items = default;
                using (DALH.DeliveryNote DALObject = new())
                {
                    items = DALObject.ListOpen(Filters, InnerFilters, OrderBy, Relations);
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

        public DeliveryNote() : base() { }

        #endregion
    }
}