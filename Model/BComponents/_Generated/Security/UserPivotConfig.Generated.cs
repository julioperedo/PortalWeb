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
    /// Class     : UserPivotConfig
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
    ///   [DMC]   4/3/2022 21:28:56 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class UserPivotConfig : BCEntity 
    {

        #region Search Methods 

        /// <summary>
        ///     Search for business objects of type UserPivotConfig
        /// </summary>
        /// <param name="Id">Object identifier UserPivotConfig</param>
        /// <param name="Relations">relationship enumetators</param>
        /// <returns>An object of type UserPivotConfig</returns>
        /// <remarks>
        ///     To get relationship objects, suply relationship enumetators
        /// </remarks>
        public BES.UserPivotConfig Search(long Id, params Enum[] Relations) 
        {
            BES.UserPivotConfig Item = null;
            try 
            {
                using (DAL.UserPivotConfig dal = new()) 
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
        ///     Search for collection business objects of type UserPivotConfig
        /// </summary>
        /// <param name="Order">Property column to specify collection order</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>Object collection of type UserPivotConfig</returns>
        /// <remarks>
        ///     To get relationship objects, suply relationship enumetators
        /// </remarks>
        public IEnumerable<BES.UserPivotConfig> List(string Order, params Enum[] Relations) 
        {
            try 
            {
                IEnumerable<BES.UserPivotConfig> Items;
                using (DAL.UserPivotConfig dal = new()) 
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

        public IEnumerable<BES.UserPivotConfig> List(List<Field> FilterList, string Order, params Enum[] Relations) 
        {
            try 
            {
                IEnumerable<BES.UserPivotConfig> Items = null;
                using (DAL.UserPivotConfig dal = new()) 
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
        /// <param name="Item">Object type UserPivotConfig</param>     
        /// <remarks>
        /// </remarks>
        public void Save(ref BES.UserPivotConfig Item) 
        {
            this.ErrorCollection.Clear();
            if (this.Validate(Item)) 
            {
                try 
                {
                    using (TransactionScope BusinessTransaction = base.GenerateBusinessTransaction()) 
                    {
                        using(DAL.UserPivotConfig dal = new()) 
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
        ///     Saves data object of type UserPivotConfig
        /// </summary>
        /// <param name="Items">Object type UserPivotConfig</param>
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BES.UserPivotConfig> Items) 
        {
            try 
            {
				using (TransactionScope BusinessTransaction = base.GenerateBusinessTransaction()) 
                {
					using (DAL.UserPivotConfig dal = new()) 
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
        /// <param name="Item">Object Type UserPivotConfig</param>
        /// <returns>True: if object were validated</returns>
        /// <remarks>
        /// </remarks>
        internal bool Validate(BES.UserPivotConfig Item) 
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
        public UserPivotConfig() : base() { }

        #endregion

    }
}