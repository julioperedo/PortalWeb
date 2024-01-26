using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using BE = BEntities;
using BEK = BEntities.Kbytes;

namespace DALayer.Kbytes
{
    public partial class AcceleratorLot
    {

        #region Save Methods

        public void UpdateQuantity(long Id, int Quantity)
        {
            string query = $@"UPDATE Kbytes.AcceleratorLot
                              SET Quantity = {Quantity}
                              WHERE Id = {Id} ";
            Connection.Execute(query);
        }

        #endregion

        #region Methods

        #endregion

        #region List Methods

        #endregion

        #region Search Methods

        public List<BEK.AcceleratorLot> List(string ProductIds, DateTime CurrentDate, params Enum[] Relations)
        {
            string query = $@"SELECT  *
							  FROM    Kbytes.AcceleratorLot al
							  WHERE   al.Enabled = 1
								      AND '{CurrentDate:yyy-MM-dd}' BETWEEN al.InitialDate AND al.FinalDate
									  AND al.IdProduct IN ( {ProductIds} )
									  AND al.Quantity > 0 ";
            List<BEK.AcceleratorLot> Items = SQLList(query, Relations).ToList();
            return Items;
        }

        #endregion

    }
}