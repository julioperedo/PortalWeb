using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using BE = BEntities;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BED = BEntities.AppData;
using BEF = BEntities.Staff;
using BEG = BEntities.Logs;
using BEL = BEntities.Sales;
using BEM = BEntities.Marketing;
using BEO = BEntities.Online;
using BEP = BEntities.Product;
using BES = BEntities.Security;
using BEV = BEntities.Visits;
using BEW = BEntities.WebSite;
using DAL = DALayer.Product;

namespace BComponents.Product
{
    public partial class Price
    {
        #region Save Methods 

        public async Task SaveAsync(BEP.Price Item)
        {
            this.ErrorCollection.Clear();
            if (this.Validate(Item))
            {
                try
                {
                    using DAL.Price dal = new();
                    await dal.SaveAsync(Item);
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

        //public List<BEP.Price> List(long IdLine, string Order, params Enum[] Relations)
        //{
        //    try
        //    {
        //        List<BEP.Price> BECollection;
        //        using (DAL.Price DALObject = new DAL.Price())
        //        {
        //            BECollection = DALObject.List(IdLine, Order, Relations);
        //        }
        //        return BECollection;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler(ex);
        //        return null;
        //    }
        //}

        public List<BEP.Price> List2(long IdLine, string Order, params Enum[] Relations)
        {
            try
            {
                List<BEP.Price> BECollection;
                using (DAL.Price DALObject = new DAL.Price())
                {
                    BECollection = DALObject.List2(IdLine, Order, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEP.Price> ListByCode(string ItemCodes, string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEP.Price> BECollection;
                if (!string.IsNullOrEmpty(ItemCodes))
                {
                    using DAL.Price DALObject = new();
                    BECollection = DALObject.ListByCode(ItemCodes, Order, Relations);
                }
                else
                {
                    BECollection = Enumerable.Empty<BEP.Price>();
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEP.Price> ListBySubsidiary(long IdSubsidiary, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEP.Price> BECollection = Enumerable.Empty<BEP.Price>();
                using (DAL.Price DALObject = new())
                {
                    BECollection = DALObject.ListBySubsidiary(IdSubsidiary, Relations);
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

        public BEP.Price Search(string ItemCode, long IdSubsidiary, params Enum[] Relations)
        {
            BEP.Price BEObject = null;

            try
            {
                using (DAL.Price DALObject = new DAL.Price())
                {
                    BEObject = DALObject.Search(ItemCode, IdSubsidiary, Relations);
                }
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
            }
            return BEObject;
        }

        #endregion
    }
}