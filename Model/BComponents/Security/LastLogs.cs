using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BE = BEntities;
using BEB = BEntities.Base;
using BEA = BEntities.SAP;
using BES = BEntities.Security;

using DAL = DALayer.Security;
using BComponents.Base;
using BComponents.Security;

namespace BComponents.Security {
    [Serializable]
    public class LastLogs : BCEntity {
        #region List Methods 

        public List<BES.LastLogs> List(DateTime? Until) {
            try {
                List<BES.LastLogs> BECollection = default(List<BES.LastLogs>);
                using(DAL.LastLogs DALObject = new DAL.LastLogs()) {
                    BECollection = DALObject.List(Until);
                }
                return BECollection;
            } catch(Exception ex) {
                base.ErrorHandler(ex);
                return null;
            }
        }

        #endregion

        #region Constructors

        public LastLogs() : base() { }

        #endregion

    }
}
