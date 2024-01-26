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


namespace DALayer.AppData
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : AppData
    /// Class     : Message
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type Message 
    ///     for the service AppData.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service AppData
    /// </remarks>
    /// <history>
    ///     [DMC]   11/9/2023 16:07:51 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class Message : DALEntity<BED.Message>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type Message
        /// </summary>
        /// <param name="Item">Business object of type Message </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BED.Message Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [AppData].[Message]([Title], [Body], [Date], [RecipientsType], [ImageUrl], [LogUser], [LogDate]) VALUES(@Title, @Body, @Date, @RecipientsType, @ImageUrl, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [AppData].[Message] SET [Title] = @Title, [Body] = @Body, [Date] = @Date, [RecipientsType] = @RecipientsType, [ImageUrl] = @ImageUrl, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [AppData].[Message] WHERE [Id] = @Id";
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
            if (Item.ListMessageRecipientss?.Count() > 0)
            {
                var list = Item.ListMessageRecipientss;
                foreach (var item in list) item.IdMessage = itemId;
                using (var dal = new MessageRecipients(Connection))
                {
                    dal.Save(ref list);
                }
                Item.ListMessageRecipientss = list;
            }
        }

        /// <summary>
        /// 	Saves a collection business information object of type  Message		
        /// </summary>
        /// <param name="Items">Business object of type Message para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BED.Message> Items)
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
        /// 	For use on data access layer at assembly level, return an  Message type object
        /// </summary>
        /// <param name="Id">Object Identifier Message</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Message</returns>
        /// <remarks>
        /// </remarks>		
        internal BED.Message ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto Message de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a Message</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo Message</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BED.Message> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BED.Message> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [AppData].[Message] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BED.Message> Items, params Enum[] Relations)
        {
            IEnumerable<long> Keys;
            IEnumerable<BE.AppData.MessageRecipients> lstMessageRecipientss = null;

            foreach (Enum RelationEnum in Relations)
            {
                Keys = from i in Items select i.Id;
                if (RelationEnum.Equals(BE.AppData.relMessage.MessageRecipientss))
                {
                    using (var dal = new MessageRecipients(Connection))
                    {
                        lstMessageRecipientss = dal.List(Keys, "IdMessage", Relations);
                    }
                }
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
                    if (lstMessageRecipientss != null)
                    {
                        Item.ListMessageRecipientss = lstMessageRecipientss.Where(x => x.IdMessage == Item.Id)?.ToList();
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
        protected override void LoadRelations(ref BED.Message Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
                long[] Keys = new[] { Item.Id };
                if (RelationEnum.Equals(BE.AppData.relMessage.MessageRecipientss))
                {
                    using (var dal = new MessageRecipients(Connection))
                    {
                        Item.ListMessageRecipientss = dal.List(Keys, "IdMessage", Relations)?.ToList();
                    }
                }
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of Message
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Message</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BED.Message> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [AppData].[Message] ORDER By " + Order;
            IEnumerable<BED.Message> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of Message
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BED.Message> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [AppData].[Message] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BED.Message> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of Message
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Message</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BED.Message> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [AppData].[Message] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BED.Message> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type Message    	
        /// </summary>
        /// <param name="Id">Object identifier Message</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Message</returns>
        /// <remarks>
        /// </remarks>    
        public BED.Message Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [AppData].[Message] WHERE [Id] = @Id";
            BED.Message Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public Message() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public Message(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal Message(SqlConnection connection) : base(connection) { }

        #endregion

    }
}