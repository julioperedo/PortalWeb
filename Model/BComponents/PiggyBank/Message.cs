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
using System.Threading.Tasks;
using BEntities.Filters;
using System.Linq;

namespace BComponents.PiggyBank
{
    public partial class Message
    {
        #region Save Methods 

        public async Task<long> SaveAsync(BEI.Message Item)
        {
            long Id = 0;
            this.ErrorCollection.Clear();
            if (this.Validate(Item))
            {
                try
                {
                    using TransactionScope bt = base.GenerateBusinessTransaction(true);
                    using DAL.Message dal = new();
                    Id = await dal.SaveAsync(Item);

                    using DAL.MessageRecipients dalRecipients = new();
                    foreach (var item in Item.ListMessageRecipientss)
                    {
                        item.IdMessage = Id;
                    }
                    await dalRecipients.SaveAsync(Item.ListMessageRecipientss.ToArray());

                    bt.Complete();
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
            return Id;
        }

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        public async Task<IEnumerable<BEI.Message>> ListAsync(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEI.Message> Items = null;
                using (DAL.Message dal = new())
                {
                    Items = await dal.ListAsync(FilterList, Order, Relations);
                }
                return Items;
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
                return null;
            }
        }

        public async Task<IEnumerable<BEI.Message>> ListByUserAsync(long UserId, string Order, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEI.Message> Items = null;
                using (DAL.Message dal = new())
                {
                    Items = await dal.ListByUserAsync(UserId, Order, Relations);
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

        #region Search Methods 

        public async Task<BEI.Message> SearchAsync(long Id, params Enum[] Relations)
        {
            BEI.Message Item = null;
            try
            {
                using DAL.Message dal = new();
                Item = await dal.SearchAsync(Id, Relations);
            }
            catch (Exception ex)
            {
                base.ErrorHandler(ex);
            }
            return Item;
        }

        #endregion
    }
}