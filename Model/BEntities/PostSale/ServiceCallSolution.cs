using System;
using System.Collections.Generic;
using BE = BEntities;
using BEntities.AppData;
using BEntities.Base;
using BEntities.Kbytes;
using BEntities.Logs;
using BEntities.Marketing;
using BEntities.Online;
using BEntities.Product;
using BEntities.Sales;
using BEntities.SAP;
using BEntities.Security;
using BEntities.Staff;
using BEntities.Visits;
using BEntities.WebSite;

namespace BEntities.PostSale
{
    public partial class ServiceCallSolution
    {

        #region Properties 

        #endregion

        #region Additional Properties 

        #endregion

        #region Contructors 

        #endregion

        #region Override members

        public override string ToString()
        {
            //TODO: Sobreescribir la propiedad mas utilizada
            return base.ToString();
        }

        public RMARepair ToSAPEntity()
        {
            RMARepair repair = new()
            {
                Id = (int)Id,
                ItemCode = ItemCode,
                StatusCode = StatusCode,
                Status = Status,
                OwnerCode = OwnerCode,
                Owner = Owner,
                CreateDate = CreateDate,
                UpdateDate = UpdateDate,
                UpdatedBy = UpdatedBy,
                UpdatedByCode = UpdatedByCode,
                Subject = Subject,
                Symptom = Symtom,
                Cause = Cause,
                Description = Description,
                Attachment = Attachment,
                Code = SAPCode.HasValue ? SAPCode.Value.ToString() : $"TEMP-{Id}"
            };

            return repair;
        }

        #endregion
    }
}