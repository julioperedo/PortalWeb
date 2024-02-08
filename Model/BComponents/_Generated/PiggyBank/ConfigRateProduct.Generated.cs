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

using DAL = DALayer.PiggyBank;

namespace BComponents.PiggyBank 
{
    /// -----------------------------------------------------------------------------
    /// Project   : BComponents
    /// NameSpace : PiggyBank
    /// Class     : ConfigRateProduct
    /// Service  :  PiggyBank
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///    This business component validates business rules for business component PiggyBank 
    ///    for the services PiggyBank
    /// </summary>
    /// <remarks>
    ///    Business component for service PiggyBank
    /// </remarks>
    /// <history>
    ///   [DMC]   5/2/2024 11:19:35 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class ConfigRateProduct : BCEntity 
    {

        #region Search Methods 

        /// <summary>
        ///     Search for business objects of type ConfigRateProduct
        /// </summary>
        /// <param name="Id">Object identifier ConfigRateProduct</param>
        /// <param name="Relations">relationship enumetators</param>
        /// <returns>An object of type ConfigRateProduct</returns>
        /// <remarks>
        ///     To get relationship objects, suply relationship enumetators
        /// </remarks>
        public BEI.ConfigRateProduct Search(long Id, params Enum[] Relations) 
        {
            BEI.ConfigRateProduct Item = null;
            try 
            {
                using (DAL.ConfigRateProduct dal = new()) 
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
        ///     Search for collection business objects of type ConfigRateProduct
        /// </summary>
        /// <param name="Order">Property column to specify collection order</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>Object collection of type ConfigRateProduct</returns>
        /// <remarks>
        ///     To get relationship objects, suply relationship enumetators
        /// </remarks>
        public IEnumerable<BEI.ConfigRateProduct> List(string Order, params Enum[] Relations) 
        {
            try 
            {
                IEnumerable<BEI.ConfigRateProduct> Items;
                using (DAL.ConfigRateProduct dal = new()) 
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

        public IEnumerable<BEI.ConfigRateProduct> List(List<Field> FilterList, string Order, params Enum[] Relations) 
        {
            try 
            {
                IEnumerable<BEI.ConfigRateProduct> Items = null;
                using (DAL.ConfigRateProduct dal = new()) 
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
        /// <param name="Item">Object type ConfigRateProduct</param>     
        /// <remarks>
        /// </remarks>
        public void Save(ref BEI.ConfigRateProduct Item) 
        {
            this.ErrorCollection.Clear();
            if (this.Validate(Item)) 
            {
                try 
                {
                    using (TransactionScope BusinessTransaction = base.GenerateBusinessTransaction()) 
                    {
                        using(DAL.ConfigRateProduct dal = new()) 
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
        ///     Saves data object of type ConfigRateProduct
        /// </summary>
        /// <param name="Items">Object type ConfigRateProduct</param>
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEI.ConfigRateProduct> Items) 
        {
            try 
            {
				using (TransactionScope BusinessTransaction = base.GenerateBusinessTransaction()) 
                {
					using (DAL.ConfigRateProduct dal = new()) 
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
        /// <param name="Item">Object Type ConfigRateProduct</param>
        /// <returns>True: if object were validated</returns>
        /// <remarks>
        /// </remarks>
        internal bool Validate(BEI.ConfigRateProduct Item) 
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
        public ConfigRateProduct() : base() { }

        #endregion

    }
}