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
    /// Class     : Message
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
    ///   [DMC]   6/9/2023 10:50:35 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class Message : BCEntity
    {

        #region Search Methods 

        /// <summary>
        ///     Search for business objects of type Message
        /// </summary>
        /// <param name="Id">Object identifier Message</param>
        /// <param name="Relations">relationship enumetators</param>
        /// <returns>An object of type Message</returns>
        /// <remarks>
        ///     To get relationship objects, suply relationship enumetators
        /// </remarks>
        public BEI.Message Search(long Id, params Enum[] Relations)
        {
            BEI.Message Item = null;
            try
            {
                using (DAL.Message dal = new())
                {
                    Item = dal.Search(Id, Relations);
                }
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
            }
            return Item;
        }

        #endregion

        #region List Methods 

        /// <summary>
        ///     Search for collection business objects of type Message
        /// </summary>
        /// <param name="Order">Property column to specify collection order</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>Object collection of type Message</returns>
        /// <remarks>
        ///     To get relationship objects, suply relationship enumetators
        /// </remarks>
        public IEnumerable<BEI.Message> List(string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEI.Message> Items;
                using (DAL.Message dal = new())
                {
                    Items = dal.List(Order, Relations);
                }
                return Items;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEI.Message> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEI.Message> Items = null;
                using (DAL.Message dal = new())
                {
                    Items = dal.List(FilterList, Order, Relations);
                }
                return Items;
            }
            catch (Exception ex)
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
        /// <param name="Item">Object type Message</param>     
        /// <remarks>
        /// </remarks>
        public void Save(ref BEI.Message Item)
        {
            this.ErrorCollection.Clear();
            if (this.Validate(Item))
            {
                try
                {
                    using (TransactionScope BusinessTransaction = base.GenerateBusinessTransaction())
                    {
                        using (DAL.Message dal = new())
                        {
                            dal.Save(ref Item);
                        }
                        BusinessTransaction.Complete();
                    }
                }
                catch (Exception ex)
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
        ///     Saves data object of type Message
        /// </summary>
        /// <param name="Items">Object type Message</param>
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEI.Message> Items)
        {
            try
            {
                using (TransactionScope BusinessTransaction = base.GenerateBusinessTransaction())
                {
                    using (DAL.Message dal = new())
                    {
                        dal.Save(ref Items);
                    }
                    BusinessTransaction.Complete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///     Validates a business object before save it 
        /// </summary>
        /// <param name="Item">Object Type Message</param>
        /// <returns>True: if object were validated</returns>
        /// <remarks>
        /// </remarks>
        internal bool Validate(BEI.Message Item)
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
        public Message() : base() { }

        #endregion

    }
}