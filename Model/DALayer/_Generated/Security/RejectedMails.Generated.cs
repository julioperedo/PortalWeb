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


namespace DALayer.Security
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : Security
    /// Class     : RejectedMails
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type RejectedMails 
    ///     for the service Security.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Security
    /// </remarks>
    /// <history>
    ///     [DMC]   7/3/2022 18:16:49 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class RejectedMails : DALEntity<BES.RejectedMails>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type RejectedMails
        /// </summary>
        /// <param name="Item">Business object of type RejectedMails </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BES.RejectedMails Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Security].[RejectedMails]([Email], [ClientCode], [ClientName], [RejectionDate], [Seller], [Reason], [Comments], [Reported], [LogUser], [LogDate]) VALUES(@Email, @ClientCode, @ClientName, @RejectionDate, @Seller, @Reason, @Comments, @Reported, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Security].[RejectedMails] SET [Email] = @Email, [ClientCode] = @ClientCode, [ClientName] = @ClientName, [RejectionDate] = @RejectionDate, [Seller] = @Seller, [Reason] = @Reason, [Comments] = @Comments, [Reported] = @Reported, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Security].[RejectedMails] WHERE [Id] = @Id";
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
        /// 	Saves a collection business information object of type  RejectedMails		
        /// </summary>
        /// <param name="Items">Business object of type RejectedMails para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BES.RejectedMails> Items)
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
        /// 	For use on data access layer at assembly level, return an  RejectedMails type object
        /// </summary>
        /// <param name="Id">Object Identifier RejectedMails</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type RejectedMails</returns>
        /// <remarks>
        /// </remarks>		
        internal BES.RejectedMails ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto RejectedMails de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a RejectedMails</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo RejectedMails</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BES.RejectedMails> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BES.RejectedMails> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Security].[RejectedMails] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BES.RejectedMails> Items, params Enum[] Relations)
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
        protected override void LoadRelations(ref BES.RejectedMails Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {

            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of RejectedMails
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type RejectedMails</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BES.RejectedMails> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Security].[RejectedMails] ORDER By " + Order;
            IEnumerable<BES.RejectedMails> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of RejectedMails
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BES.RejectedMails> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Security].[RejectedMails] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BES.RejectedMails> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of RejectedMails
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type RejectedMails</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BES.RejectedMails> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Security].[RejectedMails] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BES.RejectedMails> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type RejectedMails    	
        /// </summary>
        /// <param name="Id">Object identifier RejectedMails</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type RejectedMails</returns>
        /// <remarks>
        /// </remarks>    
        public BES.RejectedMails Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Security].[RejectedMails] WHERE [Id] = @Id";
            BES.RejectedMails Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public RejectedMails() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public RejectedMails(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal RejectedMails(SqlConnection connection) : base(connection) { }

        #endregion

    }
}