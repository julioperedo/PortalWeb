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

using DAL = DALayer.CIESD;

namespace BComponents.CIESD 
{
    /// -----------------------------------------------------------------------------
    /// Project   : BComponents
    /// NameSpace : CIESD
    /// Class     : PurchaseReturn
    /// Service  :  CIESD
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///    This business component validates business rules for business component CIESD 
    ///    for the services CIESD
    /// </summary>
    /// <remarks>
    ///    Business component for service CIESD
    /// </remarks>
    /// <history>
    ///   [DMC]   4/3/2022 21:28:23 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class PurchaseReturn : BCEntity 
    {

        #region Search Methods 

        /// <summary>
        ///     Search for business objects of type PurchaseReturn
        /// </summary>
        /// <param name="Id">Object identifier PurchaseReturn</param>
        /// <param name="Relations">relationship enumetators</param>
        /// <returns>An object of type PurchaseReturn</returns>
        /// <remarks>
        ///     To get relationship objects, suply relationship enumetators
        /// </remarks>
        public BEC.PurchaseReturn Search(long Id, params Enum[] Relations) 
        {
            BEC.PurchaseReturn Item = null;
            try 
            {
                using (DAL.PurchaseReturn dal = new()) 
                {
                    Item = dal.Search(Id, Relations);
                }
            } 
            catch(Exception ex) 
            {
                base.ErrorHandler(ex);
            }
            return Item;
        }

        #endregion

        #region List Methods 

        /// <summary>
        ///     Search for collection business objects of type PurchaseReturn
        /// </summary>
        /// <param name="Order">Property column to specify collection order</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>Object collection of type PurchaseReturn</returns>
        /// <remarks>
        ///     To get relationship objects, suply relationship enumetators
        /// </remarks>
        public IEnumerable<BEC.PurchaseReturn> List(string Order, params Enum[] Relations) 
        {
            try 
            {
                IEnumerable<BEC.PurchaseReturn> Items;
                using (DAL.PurchaseReturn dal = new()) 
                {
                    Items = dal.List(Order, Relations);
                }
                return Items;
            } 
            catch(Exception ex) 
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEC.PurchaseReturn> List(List<Field> FilterList, string Order, params Enum[] Relations) 
        {
            try 
            {
                IEnumerable<BEC.PurchaseReturn> Items = null;
                using (DAL.PurchaseReturn dal = new()) 
                {
                    Items = dal.List(FilterList, Order, Relations);
                }
                return Items;
            } 
            catch(Exception ex) 
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
        /// <param name="Item">Object type PurchaseReturn</param>     
        /// <remarks>
        /// </remarks>
        public void Save(ref BEC.PurchaseReturn Item) 
        {
            this.ErrorCollection.Clear();
            if (this.Validate(Item)) 
            {
                try 
                {
                    using (TransactionScope BusinessTransaction = base.GenerateBusinessTransaction()) 
                    {
                        using(DAL.PurchaseReturn dal = new()) 
                        {
                            dal.Save(ref Item);
                        }
                        BusinessTransaction.Complete();
                    }
                } 
                catch(Exception ex) 
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
        ///     Saves data object of type PurchaseReturn
        /// </summary>
        /// <param name="Items">Object type PurchaseReturn</param>
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEC.PurchaseReturn> Items) 
        {
            try 
            {
				using (TransactionScope BusinessTransaction = base.GenerateBusinessTransaction()) 
                {
					using (DAL.PurchaseReturn dal = new()) 
                    {
						dal.Save(ref Items);
					}
					BusinessTransaction.Complete();
                }
            } 
            catch(Exception ex) 
            {
                throw ex;
            }
        }

        /// <summary>
        ///     Validates a business object before save it 
        /// </summary>
        /// <param name="Item">Object Type PurchaseReturn</param>
        /// <returns>True: if object were validated</returns>
        /// <remarks>
        /// </remarks>
        internal bool Validate(BEC.PurchaseReturn Item) 
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
        public PurchaseReturn() : base() { }

        #endregion

    }
}