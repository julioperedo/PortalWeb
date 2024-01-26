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


namespace DALayer.Sales
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : Sales
    /// Class     : BankAccount
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type BankAccount 
    ///     for the service Sales.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Sales
    /// </remarks>
    /// <history>
    ///     [DMC]   2/3/2023 15:27:27 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class BankAccount : DALEntity<BEL.BankAccount>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type BankAccount
        /// </summary>
        /// <param name="Item">Business object of type BankAccount </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEL.BankAccount Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Sales].[BankAccount]([Subsidiary], [Holder], [Bank], [Currency], [Number], [Country], [Type], [ABANumber], [Swift], [Comments], [QR], [LogUser], [LogDate]) VALUES(@Subsidiary, @Holder, @Bank, @Currency, @Number, @Country, @Type, @ABANumber, @Swift, @Comments, @QR, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Sales].[BankAccount] SET [Subsidiary] = @Subsidiary, [Holder] = @Holder, [Bank] = @Bank, [Currency] = @Currency, [Number] = @Number, [Country] = @Country, [Type] = @Type, [ABANumber] = @ABANumber, [Swift] = @Swift, [Comments] = @Comments, [QR] = @QR, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Sales].[BankAccount] WHERE [Id] = @Id";
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
        /// 	Saves a collection business information object of type  BankAccount		
        /// </summary>
        /// <param name="Items">Business object of type BankAccount para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEL.BankAccount> Items)
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
        /// 	For use on data access layer at assembly level, return an  BankAccount type object
        /// </summary>
        /// <param name="Id">Object Identifier BankAccount</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type BankAccount</returns>
        /// <remarks>
        /// </remarks>		
        internal BEL.BankAccount ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto BankAccount de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a BankAccount</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo BankAccount</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEL.BankAccount> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEL.BankAccount> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Sales].[BankAccount] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEL.BankAccount> Items, params Enum[] Relations)
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
        protected override void LoadRelations(ref BEL.BankAccount Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {

            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of BankAccount
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type BankAccount</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEL.BankAccount> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Sales].[BankAccount] ORDER By " + Order;
            IEnumerable<BEL.BankAccount> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of BankAccount
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEL.BankAccount> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Sales].[BankAccount] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEL.BankAccount> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of BankAccount
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type BankAccount</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEL.BankAccount> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Sales].[BankAccount] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEL.BankAccount> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type BankAccount    	
        /// </summary>
        /// <param name="Id">Object identifier BankAccount</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type BankAccount</returns>
        /// <remarks>
        /// </remarks>    
        public BEL.BankAccount Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Sales].[BankAccount] WHERE [Id] = @Id";
            BEL.BankAccount Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public BankAccount() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public BankAccount(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal BankAccount(SqlConnection connection) : base(connection) { }

        #endregion

    }
}