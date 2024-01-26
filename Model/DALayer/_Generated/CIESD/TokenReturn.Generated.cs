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


namespace DALayer.CIESD
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : CIESD
    /// Class     : TokenReturn
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type TokenReturn 
    ///     for the service CIESD.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service CIESD
    /// </remarks>
    /// <history>
    ///     [DMC]   07/06/2022 10:15:28 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class TokenReturn : DALEntity<BEX.TokenReturn>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type TokenReturn
        /// </summary>
        /// <param name="Item">Business object of type TokenReturn </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEX.TokenReturn Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [CIESD].[TokenReturn]([IdToken], [ServiceTransactionId], [ClientTransactionId], [ReturnDate], [OrderNumber], [LogUser], [LogDate]) VALUES(@IdToken, @ServiceTransactionId, @ClientTransactionId, @ReturnDate, @OrderNumber, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [CIESD].[TokenReturn] SET [IdToken] = @IdToken, [ServiceTransactionId] = @ServiceTransactionId, [ClientTransactionId] = @ClientTransactionId, [ReturnDate] = @ReturnDate, [OrderNumber] = @OrderNumber, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [CIESD].[TokenReturn] WHERE [Id] = @Id";
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
        /// 	Saves a collection business information object of type  TokenReturn		
        /// </summary>
        /// <param name="Items">Business object of type TokenReturn para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEX.TokenReturn> Items)
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
        /// 	For use on data access layer at assembly level, return an  TokenReturn type object
        /// </summary>
        /// <param name="Id">Object Identifier TokenReturn</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type TokenReturn</returns>
        /// <remarks>
        /// </remarks>		
        internal BEX.TokenReturn ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto TokenReturn de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a TokenReturn</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo TokenReturn</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEX.TokenReturn> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEX.TokenReturn> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [CIESD].[TokenReturn] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEX.TokenReturn> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.CIESD.Token> lstTokens = null;

            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.CIESD.relTokenReturn.Token))
				{
					using(var dal = new Token(Connection))
					{
						Keys = (from i in Items select i.IdToken).Distinct();
						lstTokens = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstTokens != null)
					{
						Item.Token = (from i in lstTokens where i.Id == Item.IdToken select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BEX.TokenReturn Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.CIESD.relTokenReturn.Token))
				{
					using (var dal = new Token(Connection))
					{
						Item.Token = dal.ReturnMaster(Item.IdToken, Relations);
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of TokenReturn
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type TokenReturn</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEX.TokenReturn> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [CIESD].[TokenReturn] ORDER By " + Order;
            IEnumerable<BEX.TokenReturn> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of TokenReturn
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEX.TokenReturn> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [CIESD].[TokenReturn] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEX.TokenReturn> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of TokenReturn
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type TokenReturn</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEX.TokenReturn> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [CIESD].[TokenReturn] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEX.TokenReturn> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type TokenReturn    	
        /// </summary>
        /// <param name="Id">Object identifier TokenReturn</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type TokenReturn</returns>
        /// <remarks>
        /// </remarks>    
        public BEX.TokenReturn Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [CIESD].[TokenReturn] WHERE [Id] = @Id";
            BEX.TokenReturn Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public TokenReturn() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public TokenReturn(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal TokenReturn(SqlConnection connection) : base(connection) { }

        #endregion

    }
}