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


namespace DALayer.Kbytes
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : Kbytes
    /// Class     : Status
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type Status 
    ///     for the service Kbytes.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Kbytes
    /// </remarks>
    /// <history>
    ///     [DMC]   2/2/2024 14:27:47 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class Status : DALEntity<BEK.Status>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type Status
        /// </summary>
        /// <param name="Item">Business object of type Status </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEK.Status Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Kbytes].[Status]([Name], [MinAmount], [Amount], [Points], [Enabled], [LogUser], [LogDate]) VALUES(@Name, @MinAmount, @Amount, @Points, @Enabled, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Kbytes].[Status] SET [Name] = @Name, [MinAmount] = @MinAmount, [Amount] = @Amount, [Points] = @Points, [Enabled] = @Enabled, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Kbytes].[Status] WHERE [Id] = @Id";
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
			if (Item.ListClientStatuss?.Count() > 0)
			{
				var list = Item.ListClientStatuss;
				foreach (var item in list) item.IdStatus = itemId;
				using (var dal = new ClientStatus(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListClientStatuss = list;
			}
			if (Item.ListStatusDetail_Statuss?.Count() > 0)
			{
				var list = Item.ListStatusDetail_Statuss;
				foreach (var item in list) item.IdStatus = itemId;
				using (var dal = new StatusDetail(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListStatusDetail_Statuss = list;
			}
			if (Item.ListStatusDetail_StatusUseds?.Count() > 0)
			{
				var list = Item.ListStatusDetail_StatusUseds;
				foreach (var item in list) item.IdStatusUsed = itemId;
				using (var dal = new StatusDetail(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListStatusDetail_StatusUseds = list;
			}
        }

        /// <summary>
        /// 	Saves a collection business information object of type  Status		
        /// </summary>
        /// <param name="Items">Business object of type Status para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEK.Status> Items)
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
        /// 	For use on data access layer at assembly level, return an  Status type object
        /// </summary>
        /// <param name="Id">Object Identifier Status</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Status</returns>
        /// <remarks>
        /// </remarks>		
        internal BEK.Status ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto Status de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a Status</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo Status</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEK.Status> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEK.Status> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Kbytes].[Status] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEK.Status> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Kbytes.ClientStatus> lstClientStatuss = null; 
			IEnumerable<BE.Kbytes.StatusDetail> lstStatusDetail_Statuss = null; 
			IEnumerable<BE.Kbytes.StatusDetail> lstStatusDetail_StatusUseds = null; 

            foreach (Enum RelationEnum in Relations)
            {
				Keys = from i in Items select i.Id;
				if (RelationEnum.Equals(BE.Kbytes.relStatus.ClientStatuss))
				{
					using (var dal = new ClientStatus(Connection))
					{
						lstClientStatuss = dal.List(Keys, "IdStatus", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Kbytes.relStatus.StatusDetail_Statuss))
				{
					using (var dal = new StatusDetail(Connection))
					{
						lstStatusDetail_Statuss = dal.List(Keys, "IdStatus", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Kbytes.relStatus.StatusDetail_StatusUseds))
				{
					using (var dal = new StatusDetail(Connection))
					{
						lstStatusDetail_StatusUseds = dal.List(Keys, "IdStatusUsed", Relations);
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstClientStatuss != null)
					{
						Item.ListClientStatuss = lstClientStatuss.Where(x => x.IdStatus == Item.Id)?.ToList();
					}
					if (lstStatusDetail_Statuss != null)
					{
						Item.ListStatusDetail_Statuss = lstStatusDetail_Statuss.Where(x => x.IdStatus == Item.Id)?.ToList();
					}
					if (lstStatusDetail_StatusUseds != null)
					{
						Item.ListStatusDetail_StatusUseds = lstStatusDetail_StatusUseds.Where(x => x.IdStatusUsed == Item.Id)?.ToList();
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
        protected override void LoadRelations(ref BEK.Status Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				long[] Keys = new[] { Item.Id };
				if (RelationEnum.Equals(BE.Kbytes.relStatus.ClientStatuss))
				{
					using (var dal = new ClientStatus(Connection))
					{
						Item.ListClientStatuss = dal.List(Keys, "IdStatus", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Kbytes.relStatus.StatusDetail_Statuss))
				{
					using (var dal = new StatusDetail(Connection))
					{
						Item.ListStatusDetail_Statuss = dal.List(Keys, "IdStatus", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Kbytes.relStatus.StatusDetail_StatusUseds))
				{
					using (var dal = new StatusDetail(Connection))
					{
						Item.ListStatusDetail_StatusUseds = dal.List(Keys, "IdStatusUsed", Relations)?.ToList();
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of Status
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Status</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEK.Status> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Kbytes].[Status] ORDER By " + Order;
            IEnumerable<BEK.Status> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of Status
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEK.Status> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Kbytes].[Status] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEK.Status> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of Status
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Status</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEK.Status> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Kbytes].[Status] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEK.Status> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type Status    	
        /// </summary>
        /// <param name="Id">Object identifier Status</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Status</returns>
        /// <remarks>
        /// </remarks>    
        public BEK.Status Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Kbytes].[Status] WHERE [Id] = @Id";
            BEK.Status Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public Status() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public Status(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal Status(SqlConnection connection) : base(connection) { }

        #endregion

    }
}