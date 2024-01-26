using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using BEA = BEntities.SAP;
using BET = BEntities.PostSale;
using DALH = DALayer.SAP.Hana;

namespace BComponents.SAP
{
    [Serializable()]
    public class RMA : BCEntity
    {
        #region Save Methods 

        //public void DeleteAll()
        //{
        //    this.ErrorCollection.Clear();
        //    try
        //    {
        //        using (TransactionScope BusinessTransaction = base.GenerateBusinessTransaction())
        //        {
        //            using (DAL.Category DALObject = new DAL.Category())
        //            {
        //                DALObject.DeleteAll();
        //            }
        //            BusinessTransaction.Complete();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        base.ErrorHandler(ex);
        //    }
        //}

        //public string SaveServiceCall()
        //{
        //    string newKey = "";
        //    try
        //    {
        //        using DALH.RMA dal = new();
        //        newKey = dal.SaveServiceCall();
        //    }
        //    catch (Exception ex)
        //    {
        //        base.ErrorHandler(ex);
        //    }
        //    return newKey;
        //}

        //public void SaveActivityToSAP(ref BEA.RMAActivity Item, int IdRMA)
        //{
        //    try
        //    {
        //        using DALH.RMA dal = new();
        //        dal.SaveActivityToSAP(ref Item, IdRMA);
        //    }
        //    catch (Exception ex)
        //    {
        //        base.ErrorHandler(ex);
        //    }
        //}

        //public void DeleteActivityInSAP(int Id, int ActivityId)
        //{
        //    try
        //    {
        //        using DALH.RMA dal = new();
        //        dal.DeleteActivityInSAP(Id, ActivityId);
        //    }
        //    catch (Exception ex)
        //    {
        //        base.ErrorHandler(ex);
        //    }
        //}

        #endregion

        #region Search Methods

        public BEA.Item SearchState(int Id)
        {
            BEA.Item item = null;
            try
            {
                using DALH.RMA DALObject = new();
                item = DALObject.SearchState(Id);
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
            return item;
        }

        public BEA.ProductCard SearchProductCard(string SerialNumber)
        {
            BEA.ProductCard item = null;
            try
            {
                using DALH.RMA DALObject = new();
                item = DALObject.SearchProductCard(SerialNumber);
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
            return item;
        }

        #endregion

        #region List Methods

        //public IEnumerable<BEA.RMA> List(List<Field> FilterList, string Order)
        //{
        //    try
        //    {
        //        IEnumerable<BEA.RMA> items = default;
        //        using (DALH.RMA DALObject = new())
        //        {
        //            items = DALObject.List(FilterList, Order);
        //        }
        //        return items;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler(ex);
        //        return null;
        //    }
        //}

        public IEnumerable<BET.ServiceCall> List(List<Field> FilterList, string Order)
        {
            try
            {
                IEnumerable<BET.ServiceCall> items = default;
                using (DALH.RMA DALObject = new())
                {
                    items = DALObject.List(FilterList, Order);
                }
                return items;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.Item> ListStatuses(List<Field> FilterList, string Order)
        {
            try
            {
                IEnumerable<BEA.Item> BECollection = default;
                using (DALH.RMA DALObject = new())
                {
                    BECollection = DALObject.ListStatuses(FilterList, Order);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.Item> ListTechnicians(List<Field> FilterList, string Order)
        {
            try
            {
                IEnumerable<BEA.Item> BECollection = default;
                using (DALH.RMA DALObject = new())
                {
                    BECollection = DALObject.ListTechnicians(FilterList, Order);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.Item> ListSubjects()
        {
            try
            {
                IEnumerable<BEA.Item> BECollection = default;
                using (DALH.RMA DALObject = new())
                {
                    BECollection = DALObject.ListSubjects();
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.RMAHistory> ListHistory(int Id, string Order)
        {
            try
            {
                IEnumerable<BEA.RMAHistory> BECollection = default;
                using (DALH.RMA DALObject = new())
                {
                    BECollection = DALObject.ListHistory(Id, Order);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.RMAActivity> ListActivities(int Id, string Order)
        {
            try
            {
                IEnumerable<BEA.RMAActivity> BECollection = default;
                using (DALH.RMA DALObject = new())
                {
                    BECollection = DALObject.ListActivities(Id, Order);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.RMARepair> ListRepairs(int Id)
        {
            try
            {
                IEnumerable<BEA.RMARepair> BECollection = default;
                using (DALH.RMA DALObject = new())
                {
                    BECollection = DALObject.ListRepairs(Id);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.SAPUser> ListUsers()
        {
            try
            {
                IEnumerable<BEA.SAPUser> BECollection = default;
                using (DALH.RMA DALObject = new())
                {
                    BECollection = DALObject.ListUsers();
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.SAPContacts> ListContacts(string CardCode)
        {
            try
            {
                IEnumerable<BEA.SAPContacts> BECollection = default;
                using (DALH.RMA DALObject = new())
                {
                    BECollection = DALObject.ListContacts(CardCode);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.RMACost> ListCosts(int Id)
        {
            try
            {
                IEnumerable<BEA.RMACost> BECollection = default;
                using (DALH.RMA DALObject = new())
                {
                    BECollection = DALObject.ListCosts(Id);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.Item> ListBrands()
        {
            try
            {
                IEnumerable<BEA.Item> Items = default;
                using (DALH.RMA DALObject = new())
                {
                    Items = DALObject.ListBrands();
                }
                return Items;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.Item> ListCities()
        {
            try
            {
                IEnumerable<BEA.Item> BECollection = default;
                using (DALH.RMA DALObject = new())
                {
                    BECollection = DALObject.ListCities();
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.Item> ListCountries()
        {
            try
            {
                IEnumerable<BEA.Item> BECollection = default;
                using (DALH.RMA DALObject = new())
                {
                    BECollection = DALObject.ListCountries();
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.Item> ListStates(string CountryCode)
        {
            try
            {
                IEnumerable<BEA.Item> BECollection = default;
                using (DALH.RMA DALObject = new())
                {
                    BECollection = DALObject.ListStates(CountryCode);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.Item> ListLocations()
        {
            try
            {
                IEnumerable<BEA.Item> BECollection = default;
                using (DALH.RMA DALObject = new())
                {
                    BECollection = DALObject.ListLocations();
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.Item> ListCallTypes()
        {
            try
            {
                IEnumerable<BEA.Item> BECollection = default;
                using (DALH.RMA DALObject = new())
                {
                    BECollection = DALObject.ListCallTypes();
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.Item> ListOrigins()
        {
            try
            {
                IEnumerable<BEA.Item> BECollection = default;
                using (DALH.RMA DALObject = new())
                {
                    BECollection = DALObject.ListOrigins();
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.Item> ListProblemTypes()
        {
            try
            {
                IEnumerable<BEA.Item> BECollection = default;
                using (DALH.RMA DALObject = new())
                {
                    BECollection = DALObject.ListProblemTypes();
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.Item> ListSolutionStatuses()
        {
            try
            {
                IEnumerable<BEA.Item> BECollection = default;
                using (DALH.RMA DALObject = new())
                {
                    BECollection = DALObject.ListSolutionStatuses();
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

        #region Constructors

        public RMA() : base() { }

        #endregion
    }
}
