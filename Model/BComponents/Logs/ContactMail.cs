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

using DAL = DALayer.Logs;
using System.Threading.Tasks;

namespace BComponents.Logs
{
    public partial class ContactMail
    {
        #region Save Methods 

        //public async Task<long> SaveAsync(BEG.ContactMail Item)
        //{
        //    this.ErrorCollection.Clear();
        //    long Id = Item.Id;
        //    if (this.Validate(Item))
        //    {
        //        try
        //        {
        //            using DAL.ContactMail dal = new();
        //            Id = await dal.SaveAsync(Item);
        //        }
        //        catch (Exception ex)
        //        {
        //            base.ErrorHandler(ex);
        //        }
        //    }
        //    else
        //    {
        //        base.ErrorHandler(new BCException(this.ErrorCollection));
        //    }
        //    return Id;
        //}

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        #endregion

        #region Search Methods 

        #endregion
    }
}