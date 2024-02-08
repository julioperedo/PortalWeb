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


namespace DALayer.Logs
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : Logs
    /// Class     : ContactMail
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type ContactMail 
    ///     for the service Logs.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Logs
    /// </remarks>
    /// <history>
    ///     [DMC]   10/8/2022 15:01:17 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class ContactMail : DALEntity<BEG.ContactMail>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type ContactMail
        /// </summary>
        /// <param name="Item">Business object of type ContactMail </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEG.ContactMail Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Logs].[ContactMail]([Name], [EMail], [Company], [Category], [Position], [WebSiteUrl], [Region], [City], [Address], [Phone], [NIT], [ClientType], [Message], [LogUser], [LogDate]) VALUES(@Name, @EMail, @Company, @Category, @Position, @WebSiteUrl, @Region, @City, @Address, @Phone, @NIT, @ClientType, @Message, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Logs].[ContactMail] SET [Name] = @Name, [EMail] = @EMail, [Company] = @Company, [Category] = @Category, [Position] = @Position, [WebSiteUrl] = @WebSiteUrl, [Region] = @Region, [City] = @City, [Address] = @Address, [Phone] = @Phone, [NIT] = @NIT, [ClientType] = @ClientType, [Message] = @Message, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Logs].[ContactMail] WHERE [Id] = @Id";
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
        /// 	Saves a collection business information object of type  ContactMail		
        /// </summary>
        /// <param name="Items">Business object of type ContactMail para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEG.ContactMail> Items)
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
        /// 	For use on data access layer at assembly level, return an  ContactMail type object
        /// </summary>
        /// <param name="Id">Object Identifier ContactMail</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type ContactMail</returns>
        /// <remarks>
        /// </remarks>		
        internal BEG.ContactMail ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto ContactMail de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a ContactMail</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo ContactMail</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEG.ContactMail> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEG.ContactMail> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Logs].[ContactMail] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEG.ContactMail> Items, params Enum[] Relations)
        {

            foreach (Enum RelationEnum in Relations)
            {

            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {

                }
            }
        }

        /// <summary>
        /// Load Relationship of an Object
        /// </summary>
        /// <param name="Item">Given Object</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <remarks></remarks>
        protected override void LoadRelations(ref BEG.ContactMail Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {

            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of ContactMail
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ContactMail</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEG.ContactMail> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Logs].[ContactMail] ORDER By " + Order;
            IEnumerable<BEG.ContactMail> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of ContactMail
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEG.ContactMail> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Logs].[ContactMail] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEG.ContactMail> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of ContactMail
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ContactMail</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEG.ContactMail> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Logs].[ContactMail] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEG.ContactMail> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type ContactMail    	
        /// </summary>
        /// <param name="Id">Object identifier ContactMail</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type ContactMail</returns>
        /// <remarks>
        /// </remarks>    
        public BEG.ContactMail Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Logs].[ContactMail] WHERE [Id] = @Id";
            BEG.ContactMail Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public ContactMail() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public ContactMail(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal ContactMail(SqlConnection connection) : base(connection) { }

        #endregion

    }
}