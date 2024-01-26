using DALayer.Base;
using DALayer.Online;
using DALayer.Product;
using DALayer.Sales;

using DALayer.Security;
using DALayer.Staff;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using BE = BEntities;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BED = BEntities.AppData;
using BEF = BEntities.Staff;
using BEL = BEntities.Sales;
using BEO = BEntities.Online;
using BEP = BEntities.Product;
using BES = BEntities.Security;


namespace DALayer.AppData
{
    public partial class Message
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        public async Task<IEnumerable<BED.Message>> ListByUserAsync(long UserId, string SortingBy, params Enum[] Relations)
        {
            string query = $@"SELECT  *
FROM    AppData.Message m
WHERE   m.RecipientsType = 'A' 
        OR ( m.RecipientsType = 'U' AND EXISTS ( SELECT * FROM AppData.MessageRecipients mr WHERE mr.IdMessage = m.Id AND mr.Recipient = {UserId} ) ) 
ORDER BY {SortingBy} ";
            IEnumerable<BED.Message> Items = await SQLListAsync(query, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        #endregion

    }
}