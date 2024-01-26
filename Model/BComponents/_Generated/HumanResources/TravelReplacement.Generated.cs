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
using BEH = BEntities.HumanResources;
using BEI = BEntities.PiggyBank;
using BEN = BEntities.Campaign;

using DAL = DALayer.HumanResources;

namespace BComponents.HumanResources 
{
    /// -----------------------------------------------------------------------------
    /// Project   : BComponents
    /// NameSpace : HumanResources
    /// Class     : TravelReplacement
    /// Service  :  HumanResources
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///    This business component validates business rules for business component HumanResources 
    ///    for the services HumanResources
    /// </summary>
    /// <remarks>
    ///    Business component for service HumanResources
    /// </remarks>
    /// <history>
    ///   [DMC]   4/12/2023 14:06:20 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class TravelReplacement : BCEntity 
    {

        #region Search Methods 

        /// <summary>
        ///     Search for business objects of type TravelReplacement
        /// </summary>
        /// <param name="Id">Object identifier TravelReplacement</param>
        /// <param name="Relations">relationship enumetators</param>
        /// <returns>An object of type TravelReplacement</returns>
        /// <remarks>
        ///     To get relationship objects, suply relationship enumetators
        /// </remarks>
        public BEH.TravelReplacement Search(long Id, params Enum[] Relations) 
        {
            BEH.TravelReplacement Item = null;
            try 
            {
                using (DAL.TravelReplacement dal = new()) 
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
        ///     Search for collection business objects of type TravelReplacement
        /// </summary>
        /// <param name="Order">Property column to specify collection order</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>Object collection of type TravelReplacement</returns>
        /// <remarks>
        ///     To get relationship objects, suply relationship enumetators
        /// </remarks>
        public IEnumerable<BEH.TravelReplacement> List(string Order, params Enum[] Relations) 
        {
            try 
            {
                IEnumerable<BEH.TravelReplacement> Items;
                using (DAL.TravelReplacement dal = new()) 
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

        public IEnumerable<BEH.TravelReplacement> List(List<Field> FilterList, string Order, params Enum[] Relations) 
        {
            try 
            {
                IEnumerable<BEH.TravelReplacement> Items = null;
                using (DAL.TravelReplacement dal = new()) 
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
        /// <param name="Item">Object type TravelReplacement</param>     
        /// <remarks>
        /// </remarks>
        public void Save(ref BEH.TravelReplacement Item) 
        {
            this.ErrorCollection.Clear();
            if (this.Validate(Item)) 
            {
                try 
                {
                    using (TransactionScope BusinessTransaction = base.GenerateBusinessTransaction()) 
                    {
                        using(DAL.TravelReplacement dal = new()) 
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
        ///     Saves data object of type TravelReplacement
        /// </summary>
        /// <param name="Items">Object type TravelReplacement</param>
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEH.TravelReplacement> Items) 
        {
            try 
            {
				using (TransactionScope BusinessTransaction = base.GenerateBusinessTransaction()) 
                {
					using (DAL.TravelReplacement dal = new()) 
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
        /// <param name="Item">Object Type TravelReplacement</param>
        /// <returns>True: if object were validated</returns>
        /// <remarks>
        /// </remarks>
        internal bool Validate(BEH.TravelReplacement Item) 
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
        public TravelReplacement() : base() { }

        #endregion

    }
}