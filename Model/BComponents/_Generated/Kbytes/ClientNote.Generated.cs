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

using DAL = DALayer.Kbytes;

namespace BComponents.Kbytes 
{
    /// -----------------------------------------------------------------------------
    /// Project   : BComponents
    /// NameSpace : Kbytes
    /// Class     : ClientNote
    /// Service  :  Kbytes
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///    This business component validates business rules for business component Kbytes 
    ///    for the services Kbytes
    /// </summary>
    /// <remarks>
    ///    Business component for service Kbytes
    /// </remarks>
    /// <history>
    ///   [DMC]   4/3/2022 21:28:27 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class ClientNote : BCEntity 
    {

        #region Search Methods 

        /// <summary>
        ///     Search for business objects of type ClientNote
        /// </summary>
        /// <param name="Id">Object identifier ClientNote</param>
        /// <param name="Relations">relationship enumetators</param>
        /// <returns>An object of type ClientNote</returns>
        /// <remarks>
        ///     To get relationship objects, suply relationship enumetators
        /// </remarks>
        public BEK.ClientNote Search(long Id, params Enum[] Relations) 
        {
            BEK.ClientNote Item = null;
            try 
            {
                using (DAL.ClientNote dal = new()) 
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
        ///     Search for collection business objects of type ClientNote
        /// </summary>
        /// <param name="Order">Property column to specify collection order</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>Object collection of type ClientNote</returns>
        /// <remarks>
        ///     To get relationship objects, suply relationship enumetators
        /// </remarks>
        public IEnumerable<BEK.ClientNote> List(string Order, params Enum[] Relations) 
        {
            try 
            {
                IEnumerable<BEK.ClientNote> Items;
                using (DAL.ClientNote dal = new()) 
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

        public IEnumerable<BEK.ClientNote> List(List<Field> FilterList, string Order, params Enum[] Relations) 
        {
            try 
            {
                IEnumerable<BEK.ClientNote> Items = null;
                using (DAL.ClientNote dal = new()) 
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
        /// <param name="Item">Object type ClientNote</param>     
        /// <remarks>
        /// </remarks>
        public void Save(ref BEK.ClientNote Item) 
        {
            this.ErrorCollection.Clear();
            if (this.Validate(Item)) 
            {
                try 
                {
                    using (TransactionScope BusinessTransaction = base.GenerateBusinessTransaction()) 
                    {
                        using(DAL.ClientNote dal = new()) 
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
        ///     Saves data object of type ClientNote
        /// </summary>
        /// <param name="Items">Object type ClientNote</param>
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEK.ClientNote> Items) 
        {
            try 
            {
				using (TransactionScope BusinessTransaction = base.GenerateBusinessTransaction()) 
                {
					using (DAL.ClientNote dal = new()) 
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
        /// <param name="Item">Object Type ClientNote</param>
        /// <returns>True: if object were validated</returns>
        /// <remarks>
        /// </remarks>
        internal bool Validate(BEK.ClientNote Item) 
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
        public ClientNote() : base() { }

        #endregion

    }
}