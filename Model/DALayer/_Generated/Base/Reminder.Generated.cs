using BEntities.Filters;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using BE = BEntities;
using BED = BEntities.AppData;
using BEB = BEntities.Base;
using BEK = BEntities.Kbytes;
using BEG = BEntities.Logs;
using BEM = BEntities.Marketing;
using BEO = BEntities.Online;
using BET = BEntities.PostSale;
using BEP = BEntities.Product;
using BEL = BEntities.Sales;
using BEA = BEntities.SAP;
using BES = BEntities.Security;
using BEF = BEntities.Staff;
using BEV = BEntities.Visits;
using BEW = BEntities.WebSite;
using BEC = BEntities.CIESD;


namespace DALayer.Base
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : Base
    /// Class     : Reminder
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type Reminder 
    ///     for the service Base.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Base
    /// </remarks>
    /// <history>
    ///     [DMC]   7/3/2022 18:16:36 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class Reminder : DALEntity<BEB.Reminder>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type Reminder
        /// </summary>
        /// <param name="Item">Business object of type Reminder </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEB.Reminder Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Base].[Reminder]([Name], [Enabled], [LogUser], [LogDate]) VALUES(@Name, @Enabled, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Base].[Reminder] SET [Name] = @Name, [Enabled] = @Enabled, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Base].[Reminder] WHERE [Id] = @Id";
            }

            if (Item.StatusType != BE.StatusType.NoAction)
            {
				if (Item.StatusType == BE.StatusType.Insert)
					Item.Id = Convert.ToInt64(Connection.ExecuteScalar(strQuery, Item));
				else
					Connection.Execute(strQuery, Item);
                Item.StatusType = BE.StatusType.NoAction;
            }
			long itemId = Item.Id;
			if (Item.ListReminderDetails?.Count() > 0)
			{
				var list = Item.ListReminderDetails;
				foreach (var item in list) item.IdReminder = itemId;
				using (var dal = new ReminderDetail(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListReminderDetails = list;
			}
        }

        /// <summary>
        /// 	Saves a collection business information object of type  Reminder		
        /// </summary>
        /// <param name="Items">Business object of type Reminder para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEB.Reminder> Items)
        {

            for (int i = 0; i < Items.Count; i++)
            {
                var Item = Items[i];
                
                Save(ref Item);
                Items[i] = Item;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 	For use on data access layer at assembly level, return an  Reminder type object
        /// </summary>
        /// <param name="Id">Object Identifier Reminder</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Reminder</returns>
        /// <remarks>
        /// </remarks>		
        internal BEB.Reminder ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto Reminder de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a Reminder</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo Reminder</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEB.Reminder> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEB.Reminder> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Base].[Reminder] WHERE Id IN ( {string.Join(",", Keys)} ) ";
                Items = SQLList(strQuery, Relations);
            }
            return Items;
        }

        /// <summary>
        /// Carga las Relations de la Collection dada
        /// </summary>
        /// <param name="Items">Lista de objetos para cargar las Relations</param>
        /// <param name="Relations">Enumerador de Relations a cargar</param>
        /// <remarks></remarks>
        protected override void LoadRelations(ref IEnumerable<BEB.Reminder> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Base.ReminderDetail> lstReminderDetails = null; 

            foreach (Enum RelationEnum in Relations)
            {
				Keys = from i in Items select i.Id;
				if (RelationEnum.Equals(BE.Base.relReminder.ReminderDetails))
				{
					using (var dal = new ReminderDetail(Connection))
					{
						lstReminderDetails = dal.List(Keys, "IdReminder", Relations);
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstReminderDetails != null)
					{
						Item.ListReminderDetails = lstReminderDetails.Where(x => x.IdReminder == Item.Id)?.ToList();
					}
                }
            }
        }

        /// <summary>
        /// Load Relationship of an Object
        /// </summary>
        /// <param name="Item">Given Object</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <remarks></remarks>
        protected override void LoadRelations(ref BEB.Reminder Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				long[] Keys = new[] { Item.Id };
				if (RelationEnum.Equals(BE.Base.relReminder.ReminderDetails))
				{
					using (var dal = new ReminderDetail(Connection))
					{
						Item.ListReminderDetails = dal.List(Keys, "IdReminder", Relations)?.ToList();
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of Reminder
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Reminder</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEB.Reminder> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Base].[Reminder] ORDER By " + Order;
            IEnumerable<BEB.Reminder> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of Reminder
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEB.Reminder> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Base].[Reminder] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEB.Reminder> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of Reminder
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Reminder</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEB.Reminder> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Base].[Reminder] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEB.Reminder> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type Reminder    	
        /// </summary>
        /// <param name="Id">Object identifier Reminder</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Reminder</returns>
        /// <remarks>
        /// </remarks>    
        public BEB.Reminder Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Base].[Reminder] WHERE [Id] = @Id";
            BEB.Reminder Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public Reminder() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public Reminder(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal Reminder(SqlConnection connection) : base(connection) { }

        #endregion

    }
}