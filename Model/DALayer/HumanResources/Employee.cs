using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BE = BEntities;
using BEH = BEntities.HumanResources;

namespace DALayer.HumanResources 
{
    public partial class Employee 
    {

        #region Save Methods

        public async Task SaveAsync(BEH.Employee Item)
        {
            string query = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                query = "INSERT INTO [HumanResources].[Employee]([Id], [Name], [ShortName], [Position], [HierarchyLevel], [Mail], [Photo], [Phone], [Enabled], [LogUser], [LogDate]) VALUES(@Id, @Name, @ShortName, @Position, @HierarchyLevel, @Mail, @Photo, @Phone, @Enabled, @LogUser, @LogDate)";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                query = "UPDATE [HumanResources].[Employee] SET [Name] = @Name, [ShortName] = @ShortName, [Position] = @Position, [HierarchyLevel] = @HierarchyLevel, [Mail] = @Mail, [Photo] = @Photo, [Phone] = @Phone, [Enabled] = @Enabled, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                query = "DELETE FROM [HumanResources].[Employee] WHERE [Id] = @Id";
            }

            if (Item.StatusType != BE.StatusType.NoAction)
            {
                if (Item.StatusType == BE.StatusType.Insert & Item.Id <= 0) Item.Id = GenID("Employee", 1);
                await Connection.ExecuteAsync(query, Item);
            }
        }

        //public void Save(ref IList<BEH.Employee> Items)
        //{
        //    long lastId, currentId = 1;
        //    int quantity = Items.Count(i => i.StatusType == BE.StatusType.Insert & i.Id <= 0);
        //    if (quantity > 0)
        //    {
        //        lastId = GenID("Employee", quantity);
        //        currentId = lastId - quantity + 1;
        //        if (lastId <= 0) throw new Exception("No se puede generar el identificador " + this.GetType().FullName);
        //    }

        //    for (int i = 0; i < Items.Count; i++)
        //    {
        //        var Item = Items[i];
        //        if (Item.StatusType == BE.StatusType.Insert & Item.Id <= 0) Item.Id = currentId++;
        //        Save(ref Item);
        //        Items[i] = Item;
        //    }
        //}

        #endregion

        #region Methods

        #endregion

        #region List Methods

        public async Task<IEnumerable<BEH.Employee>> ListManagersAsync(long UserCode, params Enum[] Relations)
        {
            string strQuery = $@"SELECT  e1.*
FROM    HumanResources.Employee e
        INNER JOIN HumanResources.EmployeeDepartment ed ON e.Id = ed.IdEmployee
        INNER JOIN HumanResources.Employee e1 ON ed.IdManager = e1.Id
        INNER JOIN Security.UserData ud ON e.Id = ud.IdEmployee
WHERE   ud.IdUser = {UserCode}
ORDER BY e1.Name ";
            IEnumerable<BEH.Employee> Items = await SQLListAsync(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        #endregion

    }
}