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
    public class Note : BCEntity
    {
        #region Search Methods

        public BEA.Note Search(int Id, string Subsidiary, params Enum[] Relations)
        {
            BEA.Note BEObject = null;
            try
            {
                using (DALH.Note DALObject = new DALH.Note())
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

        public BEA.Note SearchLast(string CardCode, params Enum[] Relations)
        {
            BEA.Note Item = null;
            try
            {
                using DALH.Note DALObject = new DALH.Note();
                Item = DALObject.SearchLast(CardCode, Relations);
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
            }
            return Item;
        }

        public BEA.NoteExtended SearchExtended(int Id, string Subsidiary)
        {
            BEA.NoteExtended BEObject = null;
            try
            {
                using DALH.Note DALObject = new();
                BEObject = DALObject.SearchExtended(Id, Subsidiary);
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
            }
            return BEObject;
        }

        public BEA.Bill SearchBill(int NoteNumber)
        {
            BEA.Bill BEObject = null;
            try
            {
                using DALH.Note DALObject = new();
                BEObject = DALObject.SearchBill(NoteNumber);
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
            }
            return BEObject;
        }

        public BEA.Bill SearchElectronicBill(int NoteNumber)
        {
            BEA.Bill BEObject = null;
            try
            {
                using DALH.Note DALObject = new();
                BEObject = DALObject.SearchElectronicBill(NoteNumber);
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
            }
            return BEObject;

        }
        #endregion

        #region List Methods

        public IEnumerable<BEA.Note> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.Note> BECollection = default;
                using (DALH.Note DALObject = new())
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

        public IEnumerable<BEA.Note> List(List<Field> ItemFilters, List<Field> InnerFilters, string OrderBy, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.Note> BECollection = default;
                using (DALH.Note DALObject = new())
                {
                    BECollection = DALObject.List(ItemFilters, InnerFilters, OrderBy, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.OrderNote> List2(List<Field> FilterList, string Order)
        {
            try
            {
                IEnumerable<BEA.OrderNote> items = default;
                using (DALH.Note DALObject = new())
                {
                    items = DALObject.List2(FilterList, Order);
                }
                return items;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.Note> ListBySection(string Section, List<Field> FilterList, string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.Note> BECollection = default;
                string section = Section?.Length > 0 ? Section[0].ToString().ToUpper() : "";
                using (DALH.Note DALObject = new())
                {
                    BECollection = DALObject.ListBySection(section, FilterList, Order, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        //public IEnumerable<BEA.Note> List(string CardCode, string ClientOrder, string Subsidiary, string StoreHouse, DateTime? InitialDate, DateTime? FinalDate, string Salesman, string ItemCode, string Line, string Category, string Subcategory, string Order, params Enum[] Relations)
        //{
        //    try
        //    {
        //        IEnumerable<BEA.Note> BECollection = default;
        //        using (DALH.Note DALObject = new DALH.Note())
        //        {
        //            BECollection = DALObject.List(CardCode, ClientOrder, Subsidiary, StoreHouse, InitialDate, FinalDate, Salesman, ItemCode, Line, Category, Subcategory, Order, Relations);
        //        }
        //        return BECollection;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler(ex);
        //        return null;
        //    }
        //}

        public IEnumerable<BEA.OrderNote> List2(string CardCode, string ClientOrder, string Subsidiary, string StoreHouse, DateTime? InitialDate, DateTime? FinalDate, string Salesman, string ItemCode, string Line, string Category, string Subcategory, string Order)
        {
            try
            {
                IEnumerable<BEA.OrderNote> BECollection = default;
                using (DALH.Note DALObject = new())
                {
                    BECollection = DALObject.List2(CardCode, ClientOrder, Subsidiary, StoreHouse, InitialDate, FinalDate, Salesman, ItemCode, Line, Category, Subcategory, Order);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.Note> ListWithoutOrder(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.Note> BECollection = default;
                using (DALH.Note DALObject = new())
                {
                    BECollection = DALObject.ListWithoutOrder(FilterList, Order, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.NoteItem> ListItems(List<Field> Filters, string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.NoteItem> BECollection = default;
                using (DALH.NoteItem DALObject = new())
                {
                    BECollection = DALObject.List(Filters, Order, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.NoteItem> ListItems(string Subsidiary, string DocNumbers, string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.NoteItem> BECollection = default;
                List<Field> FilterList = new() { new Field { Name = "Subsidiary", Value = Subsidiary }, new Field { Name = "DocNumber", Value = DocNumbers.Split(','), Operator = Operators.In }, new Field { LogicalOperator = LogicalOperators.And } };
                using (DALH.NoteItem DALObject = new())
                {
                    BECollection = DALObject.List(FilterList, Order, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.NoteItem> ListItems(IEnumerable<string> Keys, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.NoteItem> BECollection = default;
                using (DALH.NoteItem DALObject = new DALH.NoteItem())
                {
                    BECollection = DALObject.List(Keys, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.BillItem> ListBillItems(int NoteNumber)
        {
            try
            {
                IEnumerable<BEA.BillItem> BECollection = default;
                using (DALH.NoteItem DALObject = new())
                {
                    BECollection = DALObject.ListBillItems(NoteNumber);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.BillItem> ListElectronicBillItems(int NoteNumber)
        {
            try
            {
                IEnumerable<BEA.BillItem> BECollection = default;
                using (DALH.NoteItem DALObject = new())
                {
                    BECollection = DALObject.ListElectronicBillItems(NoteNumber);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.Item> ResumeByLine(DateTime? Since, DateTime? Until)
        {
            try
            {
                IEnumerable<BEA.Item> items = default;
                using (DALH.Note DALObject = new DALH.Note())
                {
                    items = DALObject.ResumeByLine(Since, Until);
                }
                return items;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.Item> ResumeByCategory(DateTime? Since, DateTime? Until)
        {
            try
            {
                IEnumerable<BEA.Item> items = default;
                using (DALH.Note DALObject = new DALH.Note())
                {
                    items = DALObject.ResumeByCategory(Since, Until);
                }
                return items;
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
                IEnumerable<BEA.DocumentRelated> Items = new List<BEA.DocumentRelated>();
                if (!string.IsNullOrEmpty(DocIds))
                {
                    using DALH.Note DALObject = new();
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

        #endregion

        #region Constructors

        public Note() : base() { }

        #endregion
    }
}