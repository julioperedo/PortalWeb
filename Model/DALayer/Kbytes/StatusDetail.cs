using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BE = BEntities;
using BEK = BEntities.Kbytes;

namespace DALayer.Kbytes
{
    public partial class StatusDetail
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        public List<BEK.StatusDetail> List(string CardCode, int Year, string Order, params Enum[] Relations)
        {
            string query = $@"SELECT  *
                            FROM    Kbytes.StatusDetail sd
                            WHERE   EXISTS ( SELECT * FROM Kbytes.ClientNote cn WHERE cn.CardCode = '{CardCode}' AND sd.IdNote = cn.Id AND YEAR(cn.Date) = {Year} ) OR 
                                    EXISTS ( SELECT * FROM Kbytes.ClaimedAward ca WHERE ca.CardCode = '{CardCode}' AND ca.Id = sd.IdAward AND YEAR(ca.ClaimDate) = {Year} ) 
                            ORDER BY {Order} ";

            List<BEK.StatusDetail> Collection = SQLList(query, Relations).ToList();
            return Collection;
        }

        public List<BEK.StatusDetail> ListNotes(string CardCode, int Year, string Order, params Enum[] Relations)
        {
            string query = $@"SELECT  *
                            FROM    Kbytes.StatusDetail sd
                            WHERE   EXISTS ( SELECT * FROM Kbytes.ClientNote cn WHERE LOWER(cn.CardCode) = '{CardCode.ToLower()}' AND sd.IdNote = cn.Id AND YEAR(cn.Date) = {Year} ) 
                            ORDER BY {Order} ";

            List<BEK.StatusDetail> Collection = SQLList(query, Relations).ToList();
            return Collection;
        }

        public List<BEK.StatusDetail> ListNotes(string CardCode, string Order, params Enum[] Relations)
        {
            string query = $@"SELECT  *
                            FROM    Kbytes.StatusDetail sd
                            WHERE   EXISTS ( SELECT * FROM Kbytes.ClientNote cn WHERE LOWER(cn.CardCode) = '{CardCode.ToLower()}' AND sd.IdNote = cn.Id ) 
                            ORDER BY {Order} ";

            List<BEK.StatusDetail> Collection = SQLList(query, Relations).ToList();
            return Collection;
        }


        #endregion

        #region Search Methods

        public BEK.StatusDetail SearchLast(string CardCode, int Year, params Enum[] Relations)
        {
            string query = $@"SELECT  *
                              FROM    Kbytes.StatusDetail sd
                              WHERE   sd.Id = ( SELECT MAX(Id)
                                                FROM Kbytes.StatusDetail sd
                                                WHERE EXISTS ( SELECT * FROM Kbytes.ClientNote cn WHERE cn.CardCode = '{CardCode}' AND sd.IdNote = cn.Id AND YEAR(cn.Date) = {Year} )
                                                      OR EXISTS ( SELECT * FROM Kbytes.ClaimedAward ca WHERE ca.CardCode = '{CardCode}' AND ca.Id = sd.IdAward AND YEAR(ca.ClaimDate) = {Year} ) ) ";

            BEK.StatusDetail BEStatusDetail = SQLSearch(query, Relations);
            return BEStatusDetail;
        }

        #endregion

    }
}