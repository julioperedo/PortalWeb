using DALayer.AppData;
using DALayer.Base;
using DALayer.Product;
using DALayer.Sales;

using DALayer.Security;
using DALayer.Staff;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BE = BEntities;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BED = BEntities.AppData;
using BEE = BEntities.Enums;
using BEF = BEntities.Staff;
using BEL = BEntities.Sales;
using BEO = BEntities.Online;
using BEP = BEntities.Product;
using BES = BEntities.Security;

namespace DALayer.Online
{
    public partial class Sale
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        #endregion

        #region Search Methods

        public BEO.Sale SearchCurrent(long IdUser, params Enum[] Relations)
        {
            string query = $@"SELECT	s.*, c.Name AS StateDesc 
            FROM	Online.Sale s 
            		INNER JOIN Base.Classifier c ON s.StateIdc = c.Id 
            WHERE   s.IdUser = {IdUser} AND s.StateIdc = {(long)BEE.States.SaleOnline.Created} ";

            BEO.Sale item = SQLSearch(query, Relations); 
            return item;
        }

        #endregion

    }
}