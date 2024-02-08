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
    /// Class     : ClientNote
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type ClientNote 
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
    public partial class ClientNote : DALEntity<BEK.ClientNote>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type ClientNote
        /// </summary>
        /// <param name="Item">Business object of type ClientNote </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEK.ClientNote Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Kbytes].[ClientNote]([Subsidiary], [Number], [CardCode], [Date], [Amount], [Terms], [Enabled], [LogUser], [LogDate]) VALUES(@Subsidiary, @Number, @CardCode, @Date, @Amount, @Terms, @Enabled, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Kbytes].[ClientNote] SET [Subsidiary] = @Subsidiary, [Number] = @Number, [CardCode] = @CardCode, [Date] = @Date, [Amount] = @Amount, [Terms] = @Terms, [Enabled] = @Enabled, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Kbytes].[ClientNote] WHERE [Id] = @Id";
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
			if (Item.ListClientNoteDetails?.Count() > 0)
			{
				var list = Item.ListClientNoteDetails;
				foreach (var item in list) item.IdNote = itemId;
				using (var dal = new ClientNoteDetail(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListClientNoteDetails = list;
			}
			if (Item.ListStatusDetails?.Count() > 0)
			{
				var list = Item.ListStatusDetails;
				foreach (var item in list) item.IdNote = itemId;
				using (var dal = new StatusDetail(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListStatusDetails = list;
			}
        }

        /// <summary>
        /// 	Saves a collection business information object of type  ClientNote		
        /// </summary>
        /// <param name="Items">Business object of type ClientNote para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEK.ClientNote> Items)
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
        /// 	For use on data access layer at assembly level, return an  ClientNote type object
        /// </summary>
        /// <param name="Id">Object Identifier ClientNote</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type ClientNote</returns>
        /// <remarks>
        /// </remarks>		
        internal BEK.ClientNote ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto ClientNote de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a ClientNote</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo ClientNote</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEK.ClientNote> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEK.ClientNote> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Kbytes].[ClientNote] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEK.ClientNote> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Kbytes.ClientNoteDetail> lstClientNoteDetails = null; 
			IEnumerable<BE.Kbytes.StatusDetail> lstStatusDetails = null; 

            foreach (Enum RelationEnum in Relations)
            {
				Keys = from i in Items select i.Id;
				if (RelationEnum.Equals(BE.Kbytes.relClientNote.ClientNoteDetails))
				{
					using (var dal = new ClientNoteDetail(Connection))
					{
						lstClientNoteDetails = dal.List(Keys, "IdNote", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Kbytes.relClientNote.StatusDetails))
				{
					using (var dal = new StatusDetail(Connection))
					{
						lstStatusDetails = dal.List(Keys, "IdNote", Relations);
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstClientNoteDetails != null)
					{
						Item.ListClientNoteDetails = lstClientNoteDetails.Where(x => x.IdNote == Item.Id)?.ToList();
					}
					if (lstStatusDetails != null)
					{
						Item.ListStatusDetails = lstStatusDetails.Where(x => x.IdNote == Item.Id)?.ToList();
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
        protected override void LoadRelations(ref BEK.ClientNote Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				long[] Keys = new[] { Item.Id };
				if (RelationEnum.Equals(BE.Kbytes.relClientNote.ClientNoteDetails))
				{
					using (var dal = new ClientNoteDetail(Connection))
					{
						Item.ListClientNoteDetails = dal.List(Keys, "IdNote", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Kbytes.relClientNote.StatusDetails))
				{
					using (var dal = new StatusDetail(Connection))
					{
						Item.ListStatusDetails = dal.List(Keys, "IdNote", Relations)?.ToList();
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of ClientNote
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClientNote</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEK.ClientNote> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Kbytes].[ClientNote] ORDER By " + Order;
            IEnumerable<BEK.ClientNote> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of ClientNote
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEK.ClientNote> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Kbytes].[ClientNote] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEK.ClientNote> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of ClientNote
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClientNote</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEK.ClientNote> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Kbytes].[ClientNote] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEK.ClientNote> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type ClientNote    	
        /// </summary>
        /// <param name="Id">Object identifier ClientNote</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type ClientNote</returns>
        /// <remarks>
        /// </remarks>    
        public BEK.ClientNote Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Kbytes].[ClientNote] WHERE [Id] = @Id";
            BEK.ClientNote Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public ClientNote() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public ClientNote(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal ClientNote(SqlConnection connection) : base(connection) { }

        #endregion

    }
}