using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using BE = BEntities;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BED = BEntities.AppData;
using BEF = BEntities.Staff;
using BEG = BEntities.Logs;
using BEL = BEntities.Sales;
using BEM = BEntities.Marketing;
using BEO = BEntities.Online;
using BEP = BEntities.Product;
using BES = BEntities.Security;
using BEV = BEntities.Visits;
using BEW = BEntities.WebSite;
using DAL = DALayer.Base;

namespace BComponents.Base
{
    public partial class Classifier
    {
        #region Save Methods 

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        public List<BEB.Classifier> List(long ClassifierType, string Order, params Enum[] Relations)
        {
            try
            {
                List<BEB.Classifier> Items = null;
                List<Field> FilterList = new() { new Field { Name = "IdType", Value = ClassifierType } };
                using (DAL.Classifier dal = new())
                {
                    Items = dal.List(FilterList, Order, Relations).ToList();
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

        #endregion
    }
}