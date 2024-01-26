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
    /// Class     : VacationReplacement
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
    public partial class VacationReplacement : BCEntity 
    {

        #region Search Methods 

        /// <summary>
        ///     Search for business objects of type VacationReplacement
        /// </summary>
        /// <param name="Id">Object identifier VacationReplacement</param>
        /// <param name="Relations">relationship enumetators</param>
        /// <returns>An object of type VacationReplacement</returns>
        /// <remarks>
        ///     To get relationship objects, suply relationship enumetators
        /// </remarks>
        public BEH.VacationReplacement Search(long Id, params Enum[] Relations) 
        {
            BEH.VacationReplacement Item = null;
            try 
            {
                using (DAL.VacationReplacement dal = new()) 
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
        ///     Search for collection business objects of type VacationReplacement
        /// </summary>
        /// <param name="Order">Property column to specify collection order</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>Object collection of type VacationReplacement</returns>
        /// <remarks>
        ///     To get relationship objects, suply relationship enumetators
        /// </remarks>
        public IEnumerable<BEH.VacationReplacement> List(string Order, params Enum[] Relations) 
        {
            try 
            {
                IEnumerable<BEH.VacationReplacement> Items;
                using (DAL.VacationReplacement dal = new()) 
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

        public IEnumerable<BEH.VacationReplacement> List(List<Field> FilterList, string Order, params Enum[] Relations) 
        {
            try 
            {
                IEnumerable<BEH.VacationReplacement> Items = null;
                using (DAL.VacationReplacement dal = new()) 
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
        /// <param name="Item">Object type VacationReplacement</param>     
        /// <remarks>
        /// </remarks>
        public void Save(ref BEH.VacationReplacement Item) 
        {
            this.ErrorCollection.Clear();
            if (this.Validate(Item)) 
            {
                try 
                {
                    using (TransactionScope BusinessTransaction = base.GenerateBusinessTransaction()) 
                    {
                        using(DAL.VacationReplacement dal = new()) 
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
        ///     Saves data object of type VacationReplacement
        /// </summary>
        /// <param name="Items">Object type VacationReplacement</param>
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEH.VacationReplacement> Items) 
        {
            try 
            {
				using (TransactionScope BusinessTransaction = base.GenerateBusinessTransaction()) 
                {
					using (DAL.VacationReplacement dal = new()) 
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
        /// <param name="Item">Object Type VacationReplacement</param>
        /// <returns>True: if object were validated</returns>
        /// <remarks>
        /// </remarks>
        internal bool Validate(BEH.VacationReplacement Item) 
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
        public VacationReplacement() : base() { }

        #endregion

    }
}