using DALayer.Product;

using DALayer.Security;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using BE = BEntities;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BEP = BEntities.Product;
using BES = BEntities.Security;

namespace DALayer.Base {
    public partial class Classifier {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        public List<BEB.Classifier> List(BE.Enums.Classifiers Type, string Order, params Enum[] Relations) {
            string strQuery = $"SELECT * FROM [Base].[Classifier] WHERE IdType = @IdType ORDER By {Order}";
            List<BEB.Classifier> Collection = SQLList(strQuery, new { @IdType = (long)Type }, Relations).AsList();
            return Collection;
        }

        #endregion

        #region Search Methods

        #endregion

    }
}