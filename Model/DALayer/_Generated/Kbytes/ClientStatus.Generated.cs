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
    /// Class     : ClientStatus
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type ClientStatus 
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
    public partial class ClientStatus : DALEntity<BEK.ClientStatus>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type ClientStatus
        /// </summary>
        /// <param name="Item">Business object of type ClientStatus </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEK.ClientStatus Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Kbytes].[ClientStatus]([CardCode], [Year], [Quarter], [IdStatus], [Amount], [Points], [LogUser], [LogDate]) VALUES(@CardCode, @Year, @Quarter, @IdStatus, @Amount, @Points, @LogUser, @LogDate)";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Kbytes].[ClientStatus] SET [IdStatus] = @IdStatus, [Amount] = @Amount, [Points] = @Points, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [CardCode] = @CardCode AND [Year] = @Year AND [Quarter] = @Quarter";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Kbytes].[ClientStatus] WHERE [CardCode] = @CardCode AND [Year] = @Year AND [Quarter] = @Quarter";
            }

            if (Item.StatusType != BE.StatusType.NoAction)
            {
				Connection.Execute(strQuery, Item);
                Item.StatusType = BE.StatusType.NoAction;
            }
        }

        /// <summary>
        /// 	Saves a collection business information object of type  ClientStatus		
        /// </summary>
        /// <param name="Items">Business object of type ClientStatus para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEK.ClientStatus> Items)
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
        /// 	For use on data access layer at assembly level, return an  ClientStatus type object
        /// </summary>
        /// <param name="Id">Object Identifier ClientStatus</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type ClientStatus</returns>
        /// <remarks>
        /// </remarks>		
        internal BEK.ClientStatus ReturnMaster(string CardCode, int Year, int Quarter, params Enum[] Relations)
        {
            return Search(CardCode, Year, Quarter, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto ClientStatus de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a ClientStatus</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo ClientStatus</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEK.ClientStatus> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEK.ClientStatus> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Kbytes].[ClientStatus] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEK.ClientStatus> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Kbytes.Status> lstStatuss = null;

            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Kbytes.relClientStatus.Status))
				{
					using(var dal = new Status(Connection))
					{
						Keys = (from i in Items select i.IdStatus).Distinct();
						lstStatuss = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstStatuss != null)
					{
						Item.Status = (from i in lstStatuss where i.Id == Item.IdStatus select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BEK.ClientStatus Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Kbytes.relClientStatus.Status))
				{
					using (var dal = new Status(Connection))
					{
						Item.Status = dal.ReturnMaster(Item.IdStatus, Relations);
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of ClientStatus
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClientStatus</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEK.ClientStatus> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Kbytes].[ClientStatus] ORDER By " + Order;
            IEnumerable<BEK.ClientStatus> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of ClientStatus
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEK.ClientStatus> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Kbytes].[ClientStatus] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEK.ClientStatus> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of ClientStatus
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClientStatus</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEK.ClientStatus> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Kbytes].[ClientStatus] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEK.ClientStatus> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type ClientStatus    	
        /// </summary>
        /// <param name="Id">Object identifier ClientStatus</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type ClientStatus</returns>
        /// <remarks>
        /// </remarks>    
        public BEK.ClientStatus Search(string CardCode, int Year, int Quarter, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Kbytes].[ClientStatus] WHERE [CardCode] = @CardCode AND [Year] = @Year AND [Quarter] = @Quarter";
            BEK.ClientStatus Item = SQLSearch(strQuery, new { @CardCode = CardCode, @Year = Year, @Quarter = Quarter }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public ClientStatus() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public ClientStatus(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal ClientStatus(SqlConnection connection) : base(connection) { }

        #endregion

    }
}