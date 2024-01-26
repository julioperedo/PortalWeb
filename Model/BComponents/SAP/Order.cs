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
    public class Order : BCEntity
    {
        #region Search Methods

        public BEA.Order Search(int Id, string Subsidiary, params Enum[] Relations)
        {
            BEA.Order BEObject = null;
            try
            {
                using DALH.Order DALObject = new();
                BEObject = DALObject.Search(Id, Subsidiary, Relations);
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
            }
            return BEObject;
        }

        public BEA.Order Search2(int Id, string Subsidiary, params Enum[] Relations)
        {
            BEA.Order BEObject = null;
            try
            {
                using DALH.Order DALObject = new();
                BEObject = DALObject.Search2(Id, Subsidiary, Relations);
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
            }
            return BEObject;
        }

        public BEA.OrderExtended SearchExtended(int Id, string Subsidiary)
        {
            BEA.OrderExtended Item = null;
            try
            {
                using DALH.Order DALObject = new();
                Item = DALObject.SearchExtended(Id, Subsidiary);
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
            }
            return Item;
        }

        #endregion

        #region List Methods

        public IEnumerable<BEA.Order> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.Order> Items = default;
                using DALH.Order DALObject = new();
                Items = DALObject.List(FilterList, Order, Relations);
                return Items;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.Order> List2(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.Order> BECollection = default;
                using DALH.Order DALObject = new();
                BECollection = DALObject.List2(FilterList, Order, Relations);
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.Order> List4(List<Field> Filters, List<Field> InnerFilters, string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.Order> BECollection = default;
                using DALH.Order DALObject = new();
                BECollection = DALObject.List4(Filters, InnerFilters, Order, Relations);
                return BECollection;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.OrderItem> ListItems(List<Field> Filters, string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.OrderItem> BECollection = default;
                using (DALH.OrderItem DALObject = new())
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

        public IEnumerable<BEA.OrderItem> ListItems(string Subsidiary, int Id, string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.OrderItem> BECollection = default;
                List<Field> FilterList = new() { new Field { Name = "Subsidiary", Value = Subsidiary }, new Field { Name = "Id", Value = Id }, new Field { LogicalOperator = LogicalOperators.And } };
                using (DALH.OrderItem DALObject = new())
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

        public IEnumerable<BEA.OrderItem> ListItems(IEnumerable<string> Keys, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.OrderItem> BECollection = default;
                using (DALH.OrderItem DALObject = new())
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

        public IEnumerable<BEA.DocumentRelated> ListRelatedDocuments(string Subsidiary, string DocIds)
        {
            try
            {
                IEnumerable<BEA.DocumentRelated> Items = new List<BEA.DocumentRelated>();
                if (!string.IsNullOrEmpty(DocIds))
                {
                    using DALH.Order DALObject = new();
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

        public IEnumerable<BEA.OrderItem> ListItems2(string Subsidiary, int Id, string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.OrderItem> BECollection = default;
                List<Field> FilterList = new() { new Field { Name = "Subsidiary", Value = Subsidiary }, new Field { Name = "DocNumber", Value = Id }, new Field { LogicalOperator = LogicalOperators.And } };
                using (DALH.OrderItem DALObject = new())
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

        public IEnumerable<BEA.ReservedItem> ListReserved(string Subsidiary, string Warehouse, string ItemCode)
        {
            try
            {
                IEnumerable<BEA.ReservedItem> Items = default;
                using (DALH.OrderItem DALObject = new())
                {
                    Items = DALObject.ListReserved(Subsidiary, Warehouse, ItemCode);
                }
                return Items;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.OrderDestination> ListDestinations(string DocNumbers)
        {
            try
            {
                IEnumerable<BEA.OrderDestination> Items = default;
                using (DALH.Order DALObject = new())
                {
                    Items = DALObject.ListDestinations(DocNumbers);
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

        public Order() : base() { }

        #endregion
    }
}