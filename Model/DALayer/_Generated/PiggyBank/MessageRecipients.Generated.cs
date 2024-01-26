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


namespace DALayer.PiggyBank
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : PiggyBank
    /// Class     : MessageRecipients
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type MessageRecipients 
    ///     for the service PiggyBank.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service PiggyBank
    /// </remarks>
    /// <history>
    ///     [DMC]   16/9/2023 20:22:11 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class MessageRecipients : DALEntity<BEI.MessageRecipients>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type MessageRecipients
        /// </summary>
        /// <param name="Item">Business object of type MessageRecipients </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEI.MessageRecipients Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [PiggyBank].[MessageRecipients]([IdMessage], [Recipient], [LogUser], [LogDate]) VALUES(@IdMessage, @Recipient, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [PiggyBank].[MessageRecipients] SET [IdMessage] = @IdMessage, [Recipient] = @Recipient, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [PiggyBank].[MessageRecipients] WHERE [Id] = @Id";
            }

            if (Item.StatusType != BE.StatusType.NoAction)
            {
				if (Item.StatusType == BE.StatusType.Insert)
					Item.Id = Convert.ToInt64(Connection.ExecuteScalar(strQuery, Item));
				else
					Connection.Execute(strQuery, Item);
                Item.StatusType = BE.StatusType.NoAction;
            }
        }

        /// <summary>
        /// 	Saves a collection business information object of type  MessageRecipients		
        /// </summary>
        /// <param name="Items">Business object of type MessageRecipients para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEI.MessageRecipients> Items)
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
        /// 	For use on data access layer at assembly level, return an  MessageRecipients type object
        /// </summary>
        /// <param name="Id">Object Identifier MessageRecipients</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type MessageRecipients</returns>
        /// <remarks>
        /// </remarks>		
        internal BEI.MessageRecipients ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto MessageRecipients de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a MessageRecipients</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo MessageRecipients</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEI.MessageRecipients> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEI.MessageRecipients> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [PiggyBank].[MessageRecipients] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEI.MessageRecipients> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.PiggyBank.Message> lstMessages = null;

            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.PiggyBank.relMessageRecipients.Message))
				{
					using(var dal = new Message(Connection))
					{
						Keys = (from i in Items select i.IdMessage).Distinct();
						lstMessages = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstMessages != null)
					{
						Item.Message = (from i in lstMessages where i.Id == Item.IdMessage select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BEI.MessageRecipients Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.PiggyBank.relMessageRecipients.Message))
				{
					using (var dal = new Message(Connection))
					{
						Item.Message = dal.ReturnMaster(Item.IdMessage, Relations);
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of MessageRecipients
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type MessageRecipients</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEI.MessageRecipients> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [PiggyBank].[MessageRecipients] ORDER By " + Order;
            IEnumerable<BEI.MessageRecipients> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of MessageRecipients
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEI.MessageRecipients> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [PiggyBank].[MessageRecipients] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEI.MessageRecipients> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of MessageRecipients
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type MessageRecipients</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEI.MessageRecipients> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [PiggyBank].[MessageRecipients] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEI.MessageRecipients> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type MessageRecipients    	
        /// </summary>
        /// <param name="Id">Object identifier MessageRecipients</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type MessageRecipients</returns>
        /// <remarks>
        /// </remarks>    
        public BEI.MessageRecipients Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [PiggyBank].[MessageRecipients] WHERE [Id] = @Id";
            BEI.MessageRecipients Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public MessageRecipients() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public MessageRecipients(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal MessageRecipients(SqlConnection connection) : base(connection) { }

        #endregion

    }
}