using System;
using System.Collections.Generic;
using System.Data;
using BE = BEntities;
using BEC = BEntities.CIESD;

namespace DALayer.CIESD
{
    public partial class Price
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        #endregion

        #region Search Methods

        public BEC.Price SearchByDate(long IdProduct, DateTime CustomDate, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [CIESD].[Price] WHERE IdProduct = {IdProduct} AND CAST(ValidFrom AS DATE) <= '{CustomDate:yyyy-MM-dd}' AND CAST(ValidTo AS DATE) >= '{CustomDate:yyyy-MM-dd}' ";
            BEC.Price Item = SQLSearch(strQuery, Relations);
            return Item;
        }

        #endregion

    }
}