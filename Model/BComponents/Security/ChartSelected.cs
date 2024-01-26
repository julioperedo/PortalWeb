using BComponents.Base;
using BComponents.Security;
using BEntities.Filters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Transactions;
using BE = BEntities;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BES = BEntities.Security;
using DAL = DALayer.Security;

namespace BComponents.Security {

    [Serializable()]
    public class ChartSelected : BCEntity {

        #region Search Methods

        #endregion

        #region List Methods

        public List<BES.ChartSelected> List(long IdProfile) {
            try {
                List<BES.ChartSelected> BECollection = default;
                using(DAL.ChartSelected DALObject = new DAL.ChartSelected()) {
                    BECollection = DALObject.List(IdProfile);
                }
                return BECollection;
            } catch(Exception ex) {
                ErrorHandler(ex);
                return null;
            }
        }

        public List<BES.ChartSelected> List(List<Field> FilterList, string Order) {
            try {
                List<BES.ChartSelected> BECollection = default;
                using(DAL.ChartSelected DALObject = new DAL.ChartSelected()) {
                    BECollection = DALObject.List(FilterList, Order);
                }
                return BECollection;
            } catch(Exception ex) {
                ErrorHandler(ex);
                return null;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        ///  Default Constructors 
        /// </summary>
        /// <remarks>
        /// </remarks>
        public ChartSelected() : base() { }

        #endregion

    }

}