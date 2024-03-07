using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Transactions;
using BE = BEntities;
using BED = BEntities.AppData;
using BEB = BEntities.Base;
using BEK = BEntities.Kbytes;
using BEG = BEntities.Logs;
using BEM = BEntities.Marketing;
using BEO = BEntities.Online;
using BET = BEntities.PostSale;
using BEP = BEntities.Product;
using BEL = BEntities.Sales;
using BEA = BEntities.SAP;
using BES = BEntities.Security;
using BEF = BEntities.Staff;
using BEV = BEntities.Visits;
using BEW = BEntities.WebSite;
using BEC = BEntities.CIESD;

using DAL = DALayer.Product;

namespace BComponents.Product
{
    /// -----------------------------------------------------------------------------
    /// Project   : BComponents
    /// NameSpace : Product
    /// Class     : WarehouseAllowed
    /// Service  :  Product
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///    This business component validates business rules for business component Product 
    ///    for the services Product
    /// </summary>
    /// <remarks>
    ///    Business component for service Product
    /// </remarks>
    /// <history>
    ///   [DMC]   4/3/2022 21:28:43 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class WarehouseAllowed : BCEntity
    {

        #region Search Methods 

        /// <summary>
        ///     Search for business objects of type WarehouseAllowed
        /// </summary>
        /// <param name="Id">Object identifier WarehouseAllowed</param>
        /// <param name="Relations">relationship enumetators</param>
        /// <returns>An object of type WarehouseAllowed</returns>
        /// <remarks>
        ///     To get relationship objects, suply relationship enumetators
        /// </remarks>
        public BEP.WarehouseAllowed Search(long Id, params Enum[] Relations)
        {
            BEP.WarehouseAllowed Item = null;
            try
            {
                using (DAL.WarehouseAllowed dal = new())
                {
                    Item = dal.Search(Id, Relations);
                }
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
            }
            return Item;
        }

        #endregion

        #region List Methods 

        /// <summary>
        ///     Search for collection business objects of type WarehouseAllowed
        /// </summary>
        /// <param name="Order">Property column to specify collection order</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>Object collection of type WarehouseAllowed</returns>
        /// <remarks>
        ///     To get relationship objects, suply relationship enumetators
        /// </remarks>
        public IEnumerable<BEP.WarehouseAllowed> List(string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEP.WarehouseAllowed> Items;
                using (DAL.WarehouseAllowed dal = new())
                {
                    Items = dal.List(Order, Relations);
                }
                return Items;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEP.WarehouseAllowed> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEP.WarehouseAllowed> Items = null;
                using (DAL.WarehouseAllowed dal = new())
                {
                    Items = dal.List(FilterList, Order, Relations);
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

        #region Save Methods 

        /// <summary>
        ///     Saves data object of type 
        /// </summary>
        /// <param name="Item">Object type WarehouseAllowed</param>     
        /// <remarks>
        /// </remarks>
        public void Save(ref BEP.WarehouseAllowed Item)
        {
            this.ErrorCollection.Clear();
            if (this.Validate(Item))
            {
                try
                {
                    using (TransactionScope BusinessTransaction = base.GenerateBusinessTransaction())
                    {
                        using (DAL.WarehouseAllowed dal = new())
                        {
                            dal.Save(ref Item);
                        }
                        BusinessTransaction.Complete();
                    }
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

        /// <summary>
        ///     Saves data object of type WarehouseAllowed
        /// </summary>
        /// <param name="Items">Object type WarehouseAllowed</param>
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEP.WarehouseAllowed> Items)
        {
            try
            {
                using (TransactionScope BusinessTransaction = base.GenerateBusinessTransaction())
                {
                    using (DAL.WarehouseAllowed dal = new())
                    {
                        dal.Save(ref Items);
                    }
                    BusinessTransaction.Complete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///     Validates a business object before save it 
        /// </summary>
        /// <param name="Item">Object Type WarehouseAllowed</param>
        /// <returns>True: if object were validated</returns>
        /// <remarks>
        /// </remarks>
        internal bool Validate(BEP.WarehouseAllowed Item)
        {
            bool bolOk = true;
            if (Item.StatusType != BE.StatusType.NoAction)
            {
                if (Item.StatusType != BE.StatusType.Insert)
                {
                    if (Item.Id == 0)
                    {
                        base.ErrorCollection.Add("No se ha proporcionado el Identificador");
                        bolOk = false;
                    }

                }
            }
            return bolOk;
        }

        #endregion

        #region Constructors 

        /// <summary>
        ///  Default Constructors 
        /// </summary>
        /// <remarks>
        /// </remarks>
        public WarehouseAllowed() : base() { }

        #endregion

    }
}