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

using DAL = DALayer.Security;

namespace BComponents.Security 
{
    /// -----------------------------------------------------------------------------
    /// Project   : BComponents
    /// NameSpace : Security
    /// Class     : LastNotification
    /// Service  :  Security
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///    This business component validates business rules for business component Security 
    ///    for the services Security
    /// </summary>
    /// <remarks>
    ///    Business component for service Security
    /// </remarks>
    /// <history>
    ///   [DMC]   4/3/2022 21:28:50 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class LastNotification : BCEntity 
    {

        #region Search Methods 

        /// <summary>
        ///     Search for business objects of type LastNotification
        /// </summary>
        /// <param name="Id">Object identifier LastNotification</param>
        /// <param name="Relations">relationship enumetators</param>
        /// <returns>An object of type LastNotification</returns>
        /// <remarks>
        ///     To get relationship objects, suply relationship enumetators
        /// </remarks>
        public BES.LastNotification Search(long IdNotification, long LogUser, params Enum[] Relations) 
        {
            BES.LastNotification Item = null;
            try 
            {
                using (DAL.LastNotification dal = new()) 
                {
                    Item = dal.Search(IdNotification, LogUser, Relations);
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
        ///     Search for collection business objects of type LastNotification
        /// </summary>
        /// <param name="Order">Property column to specify collection order</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>Object collection of type LastNotification</returns>
        /// <remarks>
        ///     To get relationship objects, suply relationship enumetators
        /// </remarks>
        public IEnumerable<BES.LastNotification> List(string Order, params Enum[] Relations) 
        {
            try 
            {
                IEnumerable<BES.LastNotification> Items;
                using (DAL.LastNotification dal = new()) 
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

        public IEnumerable<BES.LastNotification> List(List<Field> FilterList, string Order, params Enum[] Relations) 
        {
            try 
            {
                IEnumerable<BES.LastNotification> Items = null;
                using (DAL.LastNotification dal = new()) 
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
        /// <param name="Item">Object type LastNotification</param>     
        /// <remarks>
        /// </remarks>
        public void Save(ref BES.LastNotification Item) 
        {
            this.ErrorCollection.Clear();
            if (this.Validate(Item)) 
            {
                try 
                {
                    using (TransactionScope BusinessTransaction = base.GenerateBusinessTransaction()) 
                    {
                        using(DAL.LastNotification dal = new()) 
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
        ///     Saves data object of type LastNotification
        /// </summary>
        /// <param name="Items">Object type LastNotification</param>
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BES.LastNotification> Items) 
        {
            try 
            {
				using (TransactionScope BusinessTransaction = base.GenerateBusinessTransaction()) 
                {
					using (DAL.LastNotification dal = new()) 
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
        /// <param name="Item">Object Type LastNotification</param>
        /// <returns>True: if object were validated</returns>
        /// <remarks>
        /// </remarks>
        internal bool Validate(BES.LastNotification Item) 
        {
            bool bolOk = true;
            if (Item.StatusType != BE.StatusType.NoAction) 
            {
                if (Item.StatusType != BE.StatusType.Insert) 
                {

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
        public LastNotification() : base() { }

        #endregion

    }
}