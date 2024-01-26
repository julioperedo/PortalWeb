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

using DAL = DALayer.AppData;

namespace BComponents.AppData 
{
    /// -----------------------------------------------------------------------------
    /// Project   : BComponents
    /// NameSpace : AppData
    /// Class     : MessageRecipients
    /// Service  :  AppData
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///    This business component validates business rules for business component AppData 
    ///    for the services AppData
    /// </summary>
    /// <remarks>
    ///    Business component for service AppData
    /// </remarks>
    /// <history>
    ///   [DMC]   11/9/2023 16:07:51 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class MessageRecipients : BCEntity 
    {

        #region Search Methods 

        /// <summary>
        ///     Search for business objects of type MessageRecipients
        /// </summary>
        /// <param name="Id">Object identifier MessageRecipients</param>
        /// <param name="Relations">relationship enumetators</param>
        /// <returns>An object of type MessageRecipients</returns>
        /// <remarks>
        ///     To get relationship objects, suply relationship enumetators
        /// </remarks>
        public BED.MessageRecipients Search(long Id, params Enum[] Relations) 
        {
            BED.MessageRecipients Item = null;
            try 
            {
                using (DAL.MessageRecipients dal = new()) 
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
        ///     Search for collection business objects of type MessageRecipients
        /// </summary>
        /// <param name="Order">Property column to specify collection order</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>Object collection of type MessageRecipients</returns>
        /// <remarks>
        ///     To get relationship objects, suply relationship enumetators
        /// </remarks>
        public IEnumerable<BED.MessageRecipients> List(string Order, params Enum[] Relations) 
        {
            try 
            {
                IEnumerable<BED.MessageRecipients> Items;
                using (DAL.MessageRecipients dal = new()) 
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

        public IEnumerable<BED.MessageRecipients> List(List<Field> FilterList, string Order, params Enum[] Relations) 
        {
            try 
            {
                IEnumerable<BED.MessageRecipients> Items = null;
                using (DAL.MessageRecipients dal = new()) 
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
        /// <param name="Item">Object type MessageRecipients</param>     
        /// <remarks>
        /// </remarks>
        public void Save(ref BED.MessageRecipients Item) 
        {
            this.ErrorCollection.Clear();
            if (this.Validate(Item)) 
            {
                try 
                {
                    using (TransactionScope BusinessTransaction = base.GenerateBusinessTransaction()) 
                    {
                        using(DAL.MessageRecipients dal = new()) 
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
        ///     Saves data object of type MessageRecipients
        /// </summary>
        /// <param name="Items">Object type MessageRecipients</param>
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BED.MessageRecipients> Items) 
        {
            try 
            {
				using (TransactionScope BusinessTransaction = base.GenerateBusinessTransaction()) 
                {
					using (DAL.MessageRecipients dal = new()) 
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
        /// <param name="Item">Object Type MessageRecipients</param>
        /// <returns>True: if object were validated</returns>
        /// <remarks>
        /// </remarks>
        internal bool Validate(BED.MessageRecipients Item) 
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
        public MessageRecipients() : base() { }

        #endregion

    }
}