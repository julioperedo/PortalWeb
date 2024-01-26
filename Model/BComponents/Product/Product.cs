using System;
using System.Collections.Generic;
using System.Transactions;
using BE = BEntities;
using BEP = BEntities.Product;

using DAL = DALayer.Product;
using BEntities.Filters;
using DocumentFormat.OpenXml.Vml.Office;
using System.Threading.Tasks;

namespace BComponents.Product
{
    public partial class Product
    {
        #region Save Methods 

        public void UpdateCosts(IList<BEP.Product> Items)
        {
            try
            {
                using TransactionScope transaction = base.GenerateBusinessTransaction();
                using DAL.Product dal = new();
                foreach (var item in Items)
                {
                    dal.UpdateCost(item);
                }
                transaction.Complete();
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
            }
        }

        public async Task SaveAsync(BEP.Product Item)
        {
            this.ErrorCollection.Clear();
            if (this.Validate(Item))
            {
                try
                {
                    using TransactionScope BusinessTransaction = base.GenerateBusinessTransaction();
                    using (DAL.Product dal = new())
                    {
                        await dal.SaveAsync(Item);
                    }
                    BusinessTransaction.Complete();
                }
                catch (Exception ex)
                {
                    base.ErrorHandler(ex);
                }
            }
            else
            {
                base.ErrorHandler(new BCException(this.ErrorCollection));
            }
        }

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        public IEnumerable<BEP.Product> ListWithPrices(long? IdLine, string Name, string CategoryId, string SubcategoryId, string SortBy, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEP.Product> BECollection;
                using (DAL.Product DALObject = new())
                {
                    BECollection = DALObject.ListWithPrices(IdLine, Name, CategoryId, SubcategoryId, SortBy, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public async Task<IEnumerable<BEP.Product>> ListWithPricesAsync(string LineIds, string SortBy, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEP.Product> BECollection;
                using (DAL.Product DALObject = new())
                {
                    BECollection = await DALObject.ListWithPricesAsync(LineIds, SortBy, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEP.Product> ListWithPrices2(long? IdLine, string Name, List<string> CategoryId, string SortBy, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEP.Product> BECollection;
                using (DAL.Product DALObject = new())
                {
                    BECollection = DALObject.ListWithPrices2(IdLine, Name, CategoryId, SortBy, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEP.Product> ListWithPrices(List<Field> FilterList, string SortBy, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEP.Product> BECollection;
                using (DAL.Product DALObject = new())
                {
                    BECollection = DALObject.ListWithPrices(FilterList, SortBy, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEP.Product> ListWithPrices3(string ItemCodes, string SortBy, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEP.Product> BECollection;
                using (DAL.Product DALObject = new())
                {
                    BECollection = DALObject.ListWithPrices3(ItemCodes, SortBy, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEP.Product> ListWithPricesAndNew(int Days, List<Field> FilterList, string SortingBy, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEP.Product> BECollection;
                using (DAL.Product DALObject = new())
                {
                    BECollection = DALObject.ListWithPricesAndNew(Days, FilterList, SortingBy, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEP.Product> ListWithOffer(List<Field> FilterList, string SortBy, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEP.Product> BECollection;
                using (DAL.Product DALObject = new())
                {
                    BECollection = DALObject.ListWithOffer(FilterList, SortBy, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEP.Product> ListWithOfferSA(List<Field> FilterList, string SortBy, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEP.Product> BECollection;
                using (DAL.Product DALObject = new())
                {
                    BECollection = DALObject.ListWithOfferSA(FilterList, SortBy, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEP.Product> ListWithOfferLA(List<Field> FilterList, string SortBy, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEP.Product> BECollection;
                using (DAL.Product DALObject = new())
                {
                    BECollection = DALObject.ListWithOfferLA(FilterList, SortBy, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEP.Product> ListWithOfferIQ(List<Field> FilterList, string SortBy, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEP.Product> BECollection;
                using (DAL.Product DALObject = new())
                {
                    BECollection = DALObject.ListWithOfferIQ(FilterList, SortBy, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<string> ListCategories(long? IdLine)
        {
            try
            {
                IEnumerable<string> collection;
                using (DAL.Product DALObject = new())
                {
                    collection = DALObject.ListCategories(IdLine);
                }
                return collection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<string> ListSAPLines(long? LineId)
        {
            try
            {
                IEnumerable<string> BECollection;
                using (DAL.Product DALObject = new())
                {
                    BECollection = DALObject.ListSAPLines(LineId);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        //public List<BEP.Product> ListIdWithPrices(long? IdLine, string Name, string CategoryId)
        //{
        //    try
        //    {
        //        List<BEP.Product> BECollection;
        //        using (DAL.Product DALObject = new DAL.Product())
        //        {
        //            BECollection = DALObject.ListIdWithPrices(IdLine, Name, CategoryId);
        //        }
        //        return BECollection;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler(ex);
        //        return null;
        //    }
        //}

        //public List<BEP.Product> ListWithPrices(List<Field> FilterList, string Order)
        //{
        //    try
        //    {
        //        List<BEP.Product> BECollection;

        //        using (DAL.Product DALObject = new DAL.Product())
        //        {
        //            BECollection = DALObject.ListWithPrices(FilterList, Order);
        //        }
        //        return BECollection;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler(ex);
        //        return null;
        //    }
        //}

        //public List<BEP.Product> ListWithOutPrices(List<Field> FilterList, string order)
        //{
        //    try
        //    {
        //        List<BEP.Product> BECollection;

        //        using (DAL.Product DALObject = new DAL.Product())
        //        {
        //            BECollection = DALObject.ListWithOutPrices(FilterList, order);
        //        }
        //        return BECollection;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler(ex);
        //        return null;
        //    }
        //}

        //public List<BEP.Product> ListWithOfferForLine(long LineId, string SortBy, params Enum[] Relations)
        //{
        //    try
        //    {
        //        List<BEP.Product> BECollection;
        //        using (DAL.Product DALObject = new DAL.Product())
        //        {
        //            BECollection = DALObject.ListWithOfferForLine(LineId, SortBy, Relations);
        //        }
        //        return BECollection;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler(ex);
        //        return null;
        //    }
        //}

        //public List<string> ListSubcategories(long? IdLine, string Category)
        //{
        //    try
        //    {
        //        List<string> collection;
        //        using (DAL.Product DALObject = new DAL.Product())
        //        {
        //            collection = DALObject.ListSubcategories(IdLine, Category);
        //        }
        //        return collection;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler(ex);
        //        return null;
        //    }
        //}

        //public List<BEP.Product> ListSubcategories(long IdLine)
        //{
        //    try
        //    {
        //        List<BEP.Product> collection;
        //        using (DAL.Product DALObject = new DAL.Product())
        //        {
        //            collection = DALObject.ListSubcategories(IdLine);
        //        }
        //        return collection;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler(ex);
        //        return null;
        //    }
        //}

        //public List<BEP.Product> ListFullStructure()
        //{
        //    try
        //    {
        //        List<BEP.Product> BECollection;
        //        using (DAL.Product DALObject = new DAL.Product())
        //        {
        //            BECollection = DALObject.ListFullStructure();
        //        }
        //        return BECollection;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler(ex);
        //        return null;
        //    }
        //}

        //public List<BEP.Product> ListFullCategories()
        //{
        //    try
        //    {
        //        List<BEP.Product> BECollection;
        //        using (DAL.Product DALObject = new DAL.Product())
        //        {
        //            BECollection = DALObject.ListFullCategories();
        //        }
        //        return BECollection;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler(ex);
        //        return null;
        //    }
        //}

        public IEnumerable<BEP.Product> ListEpson()
        {
            try
            {
                IEnumerable<BEP.Product> BECollection;
                using (DAL.Product DALObject = new())
                {
                    BECollection = DALObject.ListEpson();
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return new List<BEP.Product>();
            }
        }

        //public IEnumerable<BEP.Product> ListHPE(params Enum[] Relations)
        //{
        //    try
        //    {
        //        IEnumerable<BEP.Product> BECollection;
        //        using (DAL.Product DALObject = new DAL.Product())
        //        {
        //            BECollection = DALObject.ListHPE(Relations);
        //        }
        //        return BECollection;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler(ex);
        //        return null;
        //    }
        //}

        public IEnumerable<BEP.Product> ListSuggested(long IdUser, int Quantity, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEP.Product> BECollection;
                using (DAL.Product DALObject = new())
                {
                    BECollection = DALObject.ListSuggested(IdUser, Quantity, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return new List<BEP.Product>();
            }
        }

        public IEnumerable<BEP.Product> ListCategories()
        {
            try
            {
                IEnumerable<BEP.Product> BECollection;
                using (DAL.Product DALObject = new())
                {
                    BECollection = DALObject.ListCategories();
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return new List<BEP.Product>();
            }
        }

        public IEnumerable<BEP.Product> ListForCampaign(long IdCampaign, string SortingBy, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEP.Product> Items;
                using (DAL.Product dal = new())
                {
                    Items = dal.ListForCampaign(IdCampaign, SortingBy, Relations);
                }
                return Items;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEP.Product> ListByLine(long IdLine, string SortingBy, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEP.Product> Items;
                using (DAL.Product dal = new())
                {
                    Items = dal.ListByLine(IdLine, SortingBy, Relations);
                }
                return Items;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEP.Product> ListForPiggyBank(string SortingBy, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEP.Product> Items;
                using (DAL.Product dal = new())
                {
                    Items = dal.ListForPiggyBank(SortingBy, Relations);
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

        public BEP.Product Search(string ItemCode, params Enum[] Relations)
        {
            BEP.Product BEObject = null;

            try
            {
                using DAL.Product DALObject = new();
                BEObject = DALObject.Search(ItemCode, Relations);
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
            }
            return BEObject;
        }

        public int CountWithOffer(List<Field> FilterList)
        {
            int count = 0;
            try
            {
                using DAL.Product dal = new();
                count = dal.CountWithOffer(FilterList);
            }
            catch (Exception ex)
            {
                count = 0;
                base.ErrorHandler(ex);
            }
            return count;
        }

        #endregion
    }
}