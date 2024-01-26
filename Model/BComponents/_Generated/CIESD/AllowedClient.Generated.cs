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
    /// Class     : AllowedClient
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
    ///   [DMC]   20/4/2022 16:15:35 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class AllowedClient : BCEntity 
    {

        #region Search Methods 

        /// <summary>
        ///     Search for business objects of type AllowedClient
        /// </summary>
        /// <param name="Id">Object identifier AllowedClient</param>
        /// <param name="Relations">relationship enumetators</param>
        /// <returns>An object of type AllowedClient</returns>
        /// <remarks>
        ///     To get relationship objects, suply relationship enumetators
        /// </remarks>
        public BEC.AllowedClient Search(string CardCode, params Enum[] Relations) 
        {
            BEC.AllowedClient Item = null;
            try 
            {
                using (DAL.AllowedClient dal = new()) 
                {
                    Item = dal.Search(CardCode, Relations);
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
        ///     Search for collection business objects of type AllowedClient
        /// </summary>
        /// <param name="Order">Property column to specify collection order</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>Object collection of type AllowedClient</returns>
        /// <remarks>
        ///     To get relationship objects, suply relationship enumetators
        /// </remarks>
        public IEnumerable<BEC.AllowedClient> List(string Order, params Enum[] Relations) 
        {
            try 
            {
                IEnumerable<BEC.AllowedClient> Items;
                using (DAL.AllowedClient dal = new()) 
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

        public IEnumerable<BEC.AllowedClient> List(List<Field> FilterList, string Order, params Enum[] Relations) 
        {
            try 
            {
                IEnumerable<BEC.AllowedClient> Items = null;
                using (DAL.AllowedClient dal = new()) 
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
        /// <param name="Item">Object type AllowedClient</param>     
        /// <remarks>
        /// </remarks>
        public void Save(ref BEC.AllowedClient Item) 
        {
            this.ErrorCollection.Clear();
            if (this.Validate(Item)) 
            {
                try 
                {
                    using (TransactionScope BusinessTransaction = base.GenerateBusinessTransaction()) 
                    {
                        using(DAL.AllowedClient dal = new()) 
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
        ///     Saves data object of type AllowedClient
        /// </summary>
        /// <param name="Items">Object type AllowedClient</param>
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEC.AllowedClient> Items) 
        {
            try 
            {
				using (TransactionScope BusinessTransaction = base.GenerateBusinessTransaction()) 
                {
					using (DAL.AllowedClient dal = new()) 
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
        /// <param name="Item">Object Type AllowedClient</param>
        /// <returns>True: if object were validated</returns>
        /// <remarks>
        /// </remarks>
        internal bool Validate(BEC.AllowedClient Item) 
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
        public AllowedClient() : base() { }

        #endregion

    }
}