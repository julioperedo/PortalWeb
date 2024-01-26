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
using BEP = BEntities.Product;
using BEL = BEntities.Sales;
using BEA = BEntities.SAP;
using BES = BEntities.Security;
using BEF = BEntities.Staff;
using BEV = BEntities.Visits;
using BEW = BEntities.WebSite;
using BEX = BEntities.CIESD;

using DAL = DALayer.CIESD;

namespace BComponents.CIESD 
{
    /// -----------------------------------------------------------------------------
    /// Project   : BComponents
    /// NameSpace : CIESD
    /// Class     : Purchase
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
    ///   [DMC]   07/06/2022 10:15:28 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class Purchase : BCEntity 
    {

        #region Search Methods 

        /// <summary>
        ///     Search for business objects of type Purchase
        /// </summary>
        /// <param name="Id">Object identifier Purchase</param>
        /// <param name="Relations">relationship enumetators</param>
        /// <returns>An object of type Purchase</returns>
        /// <remarks>
        ///     To get relationship objects, suply relationship enumetators
        /// </remarks>
        public BEX.Purchase Search(long Id, params Enum[] Relations) 
        {
            BEX.Purchase Item = null;
            try 
            {
                using (DAL.Purchase dal = new()) 
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
        ///     Search for collection business objects of type Purchase
        /// </summary>
        /// <param name="Order">Property column to specify collection order</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>Object collection of type Purchase</returns>
        /// <remarks>
        ///     To get relationship objects, suply relationship enumetators
        /// </remarks>
        public IEnumerable<BEX.Purchase> List(string Order, params Enum[] Relations) 
        {
            try 
            {
                IEnumerable<BEX.Purchase> Items;
                using (DAL.Purchase dal = new()) 
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

        public IEnumerable<BEX.Purchase> List(List<Field> FilterList, string Order, params Enum[] Relations) 
        {
            try 
            {
                IEnumerable<BEX.Purchase> Items = null;
                using (DAL.Purchase dal = new()) 
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
        /// <param name="Item">Object type Purchase</param>     
        /// <remarks>
        /// </remarks>
        public void Save(ref BEX.Purchase Item) 
        {
            this.ErrorCollection.Clear();
            if (this.Validate(Item)) 
            {
                try 
                {
                    using (TransactionScope BusinessTransaction = base.GenerateBusinessTransaction()) 
                    {
                        using(DAL.Purchase dal = new()) 
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
        ///     Saves data object of type Purchase
        /// </summary>
        /// <param name="Items">Object type Purchase</param>
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEX.Purchase> Items) 
        {
            try 
            {
				using (TransactionScope BusinessTransaction = base.GenerateBusinessTransaction()) 
                {
					using (DAL.Purchase dal = new()) 
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
        /// <param name="Item">Object Type Purchase</param>
        /// <returns>True: if object were validated</returns>
        /// <remarks>
        /// </remarks>
        internal bool Validate(BEX.Purchase Item) 
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
        public Purchase() : base() { }

        #endregion

    }
}