using System;
using BE = BEntities;

namespace BEntities
{
    [Serializable()]
    public abstract class BEntity
    {

        #region Private Variables
        #endregion

        #region Properties 

        public StatusType StatusType { get; set; }

        #endregion

        #region Constructors 

        public BEntity()
        {
            StatusType = BE.StatusType.NoAction;
        }

        #endregion

        #region Methods

        #endregion
    }
}
