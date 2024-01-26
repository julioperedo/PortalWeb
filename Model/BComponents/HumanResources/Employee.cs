using System;
using System.Collections.Generic;
using System.Transactions;
using BE = BEntities;
using BED = BEntities.AppData;
using BEB = BEntities.Base;
using BEK = BEntities.Kbytes;
using BEG = BEntities.Logs;
using BEM = BEntities.Marketing;
using BEO = BEntities.Online;
using BEP = BEntities.Product;
using BEL = BEntities.Sales;
using BEA = BEntities.SAP;
using BES = BEntities.Security;
using BEF = BEntities.Staff;
using BEV = BEntities.Visits;
using BEW = BEntities.WebSite;
using BEX = BEntities.CIESD;
using BEH = BEntities.HumanResources;
using BEI = BEntities.PiggyBank;
using BEN = BEntities.Campaign;

using DAL = DALayer.HumanResources;
using System.Threading.Tasks;
using BComponents.Security;

namespace BComponents.HumanResources
{
    public partial class Employee
    {
        #region Save Methods 

        public async Task SaveAsync(BEH.Employee Item)
        {
            this.ErrorCollection.Clear();
            if (this.Validate(Item))
            {
                try
                {
                    using TransactionScope BusinessTransaction = base.GenerateBusinessTransaction(true);
                    using DAL.Employee dal = new();
                    await dal.SaveAsync(Item);

                    if (Item.ListEmployeeDepartment_Employees.Count > 0)
                    {
                        using DAL.EmployeeDepartment dalDetail = new();
                        foreach (var item in Item.ListEmployeeDepartment_Employees)
                        {
                            item.Id = await dalDetail.SaveAsync(item);
                        }
                    }

                    BusinessTransaction.Complete();
                }
                catch (Exception ex)
                {
                    base.ErrorHandler(ex);
                }
            }
            else
            {
                base.ErrorHandler(new BCException(this.ErrorCollection));
            }
        }

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        public async Task<IEnumerable<BEH.Employee>> ListManagersAsync(long UserCode, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEH.Employee> Items;
                using (DAL.Employee dal = new())
                {
                    Items = await dal.ListManagersAsync(UserCode, Relations);
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