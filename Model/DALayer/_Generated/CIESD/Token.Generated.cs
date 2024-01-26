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
    /// Class     : Token
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type Token 
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
    public partial class Token : DALEntity<BEX.Token>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type Token
        /// </summary>
        /// <param name="Item">Business object of type Token </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEX.Token Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [CIESD].[Token]([IdPurchase], [Line], [TransactionNumber], [SequenceNumber], [Type], [Description], [Code], [LogUser], [LogDate]) VALUES(@IdPurchase, @Line, @TransactionNumber, @SequenceNumber, @Type, @Description, @Code, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [CIESD].[Token] SET [IdPurchase] = @IdPurchase, [Line] = @Line, [TransactionNumber] = @TransactionNumber, [SequenceNumber] = @SequenceNumber, [Type] = @Type, [Description] = @Description, [Code] = @Code, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [CIESD].[Token] WHERE [Id] = @Id";
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
			if (Item.ListTokenReturns?.Count() > 0)
			{
				var list = Item.ListTokenReturns;
				foreach (var item in list) item.IdToken = itemId;
				using (var dal = new TokenReturn(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListTokenReturns = list;
			}
        }

        /// <summary>
        /// 	Saves a collection business information object of type  Token		
        /// </summary>
        /// <param name="Items">Business object of type Token para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEX.Token> Items)
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
        /// 	For use on data access layer at assembly level, return an  Token type object
        /// </summary>
        /// <param name="Id">Object Identifier Token</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Token</returns>
        /// <remarks>
        /// </remarks>		
        internal BEX.Token ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto Token de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a Token</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo Token</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEX.Token> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEX.Token> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [CIESD].[Token] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEX.Token> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.CIESD.TokenReturn> lstTokenReturns = null; 
			IEnumerable<BE.CIESD.Purchase> lstPurchases = null;

            foreach (Enum RelationEnum in Relations)
            {
				Keys = from i in Items select i.Id;
				if (RelationEnum.Equals(BE.CIESD.relToken.TokenReturns))
				{
					using (var dal = new TokenReturn(Connection))
					{
						lstTokenReturns = dal.List(Keys, "IdToken", Relations);
					}
				}
				if (RelationEnum.Equals(BE.CIESD.relToken.Purchase))
				{
					using(var dal = new Purchase(Connection))
					{
						Keys = (from i in Items select i.IdPurchase).Distinct();
						lstPurchases = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstTokenReturns != null)
					{
						Item.ListTokenReturns = lstTokenReturns.Where(x => x.IdToken == Item.Id)?.ToList();
					}
					if (lstPurchases != null)
					{
						Item.Purchase = (from i in lstPurchases where i.Id == Item.IdPurchase select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BEX.Token Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				long[] Keys = new[] { Item.Id };
				if (RelationEnum.Equals(BE.CIESD.relToken.TokenReturns))
				{
					using (var dal = new TokenReturn(Connection))
					{
						Item.ListTokenReturns = dal.List(Keys, "IdToken", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.CIESD.relToken.Purchase))
				{
					using (var dal = new Purchase(Connection))
					{
						Item.Purchase = dal.ReturnMaster(Item.IdPurchase, Relations);
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of Token
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Token</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEX.Token> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [CIESD].[Token] ORDER By " + Order;
            IEnumerable<BEX.Token> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of Token
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEX.Token> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [CIESD].[Token] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEX.Token> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of Token
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Token</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEX.Token> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [CIESD].[Token] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEX.Token> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type Token    	
        /// </summary>
        /// <param name="Id">Object identifier Token</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Token</returns>
        /// <remarks>
        /// </remarks>    
        public BEX.Token Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [CIESD].[Token] WHERE [Id] = @Id";
            BEX.Token Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public Token() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public Token(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal Token(SqlConnection connection) : base(connection) { }

        #endregion

    }
}