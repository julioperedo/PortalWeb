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


namespace DALayer.Sales
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : Sales
    /// Class     : TransportSent
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type TransportSent 
    ///     for the service Sales.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Sales
    /// </remarks>
    /// <history>
    ///     [DMC]   7/3/2022 18:16:47 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class TransportSent : DALEntity<BEL.TransportSent>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type TransportSent
        /// </summary>
        /// <param name="Item">Business object of type TransportSent </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEL.TransportSent Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Sales].[TransportSent]([IdTransport], [Tos], [CCs], [BCCs], [Body], [LogUser], [LogDate]) VALUES(@IdTransport, @Tos, @CCs, @BCCs, @Body, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Sales].[TransportSent] SET [IdTransport] = @IdTransport, [Tos] = @Tos, [CCs] = @CCs, [BCCs] = @BCCs, [Body] = @Body, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Sales].[TransportSent] WHERE [Id] = @Id";
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
        /// 	Saves a collection business information object of type  TransportSent		
        /// </summary>
        /// <param name="Items">Business object of type TransportSent para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEL.TransportSent> Items)
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
        /// 	For use on data access layer at assembly level, return an  TransportSent type object
        /// </summary>
        /// <param name="Id">Object Identifier TransportSent</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type TransportSent</returns>
        /// <remarks>
        /// </remarks>		
        internal BEL.TransportSent ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto TransportSent de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a TransportSent</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo TransportSent</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEL.TransportSent> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEL.TransportSent> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Sales].[TransportSent] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEL.TransportSent> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Sales.Transport> lstTransports = null;

            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Sales.relTransportSent.Transport))
				{
					using(var dal = new Transport(Connection))
					{
						Keys = (from i in Items select i.IdTransport).Distinct();
						lstTransports = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstTransports != null)
					{
						Item.Transport = (from i in lstTransports where i.Id == Item.IdTransport select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BEL.TransportSent Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Sales.relTransportSent.Transport))
				{
					using (var dal = new Transport(Connection))
					{
						Item.Transport = dal.ReturnMaster(Item.IdTransport, Relations);
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of TransportSent
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type TransportSent</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEL.TransportSent> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Sales].[TransportSent] ORDER By " + Order;
            IEnumerable<BEL.TransportSent> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of TransportSent
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEL.TransportSent> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Sales].[TransportSent] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEL.TransportSent> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of TransportSent
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type TransportSent</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEL.TransportSent> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Sales].[TransportSent] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEL.TransportSent> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type TransportSent    	
        /// </summary>
        /// <param name="Id">Object identifier TransportSent</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type TransportSent</returns>
        /// <remarks>
        /// </remarks>    
        public BEL.TransportSent Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Sales].[TransportSent] WHERE [Id] = @Id";
            BEL.TransportSent Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public TransportSent() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public TransportSent(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal TransportSent(SqlConnection connection) : base(connection) { }

        #endregion

    }
}