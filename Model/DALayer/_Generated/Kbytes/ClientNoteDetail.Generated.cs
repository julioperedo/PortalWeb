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
    /// Class     : ClientNoteDetail
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type ClientNoteDetail 
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
    public partial class ClientNoteDetail : DALEntity<BEK.ClientNoteDetail>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type ClientNoteDetail
        /// </summary>
        /// <param name="Item">Business object of type ClientNoteDetail </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEK.ClientNoteDetail Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Kbytes].[ClientNoteDetail]([IdNote], [IdProduct], [Quantity], [Total], [AcceleratedQuantity], [AcceleratedTotal], [Accelerator], [ExtraPoints], [LogUser], [LogDate]) VALUES(@IdNote, @IdProduct, @Quantity, @Total, @AcceleratedQuantity, @AcceleratedTotal, @Accelerator, @ExtraPoints, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Kbytes].[ClientNoteDetail] SET [IdNote] = @IdNote, [IdProduct] = @IdProduct, [Quantity] = @Quantity, [Total] = @Total, [AcceleratedQuantity] = @AcceleratedQuantity, [AcceleratedTotal] = @AcceleratedTotal, [Accelerator] = @Accelerator, [ExtraPoints] = @ExtraPoints, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Kbytes].[ClientNoteDetail] WHERE [Id] = @Id";
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
        /// 	Saves a collection business information object of type  ClientNoteDetail		
        /// </summary>
        /// <param name="Items">Business object of type ClientNoteDetail para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEK.ClientNoteDetail> Items)
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
        /// 	For use on data access layer at assembly level, return an  ClientNoteDetail type object
        /// </summary>
        /// <param name="Id">Object Identifier ClientNoteDetail</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type ClientNoteDetail</returns>
        /// <remarks>
        /// </remarks>		
        internal BEK.ClientNoteDetail ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto ClientNoteDetail de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a ClientNoteDetail</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo ClientNoteDetail</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEK.ClientNoteDetail> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEK.ClientNoteDetail> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Kbytes].[ClientNoteDetail] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEK.ClientNoteDetail> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Kbytes.ClientNote> lstNotes = null;
			IEnumerable<BE.Product.Product> lstProducts = null;

            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Kbytes.relClientNoteDetail.Note))
				{
					using(var dal = new ClientNote(Connection))
					{
						Keys = (from i in Items select i.IdNote).Distinct();
						lstNotes = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}				if (RelationEnum.Equals(BE.Kbytes.relClientNoteDetail.Product))
				{
					using(var dal = new Product.Product(Connection))
					{
						Keys = (from i in Items select i.IdProduct).Distinct();
						lstProducts = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstNotes != null)
					{
						Item.Note = (from i in lstNotes where i.Id == Item.IdNote select i).FirstOrDefault();
					}					if (lstProducts != null)
					{
						Item.Product = (from i in lstProducts where i.Id == Item.IdProduct select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BEK.ClientNoteDetail Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Kbytes.relClientNoteDetail.Note))
				{
					using (var dal = new ClientNote(Connection))
					{
						Item.Note = dal.ReturnMaster(Item.IdNote, Relations);
					}
				}				if (RelationEnum.Equals(BE.Kbytes.relClientNoteDetail.Product))
				{
					using (var dal = new Product.Product(Connection))
					{
						Item.Product = dal.ReturnMaster(Item.IdProduct, Relations);
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of ClientNoteDetail
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClientNoteDetail</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEK.ClientNoteDetail> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Kbytes].[ClientNoteDetail] ORDER By " + Order;
            IEnumerable<BEK.ClientNoteDetail> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of ClientNoteDetail
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEK.ClientNoteDetail> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Kbytes].[ClientNoteDetail] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEK.ClientNoteDetail> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of ClientNoteDetail
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClientNoteDetail</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEK.ClientNoteDetail> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Kbytes].[ClientNoteDetail] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEK.ClientNoteDetail> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type ClientNoteDetail    	
        /// </summary>
        /// <param name="Id">Object identifier ClientNoteDetail</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type ClientNoteDetail</returns>
        /// <remarks>
        /// </remarks>    
        public BEK.ClientNoteDetail Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Kbytes].[ClientNoteDetail] WHERE [Id] = @Id";
            BEK.ClientNoteDetail Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public ClientNoteDetail() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public ClientNoteDetail(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal ClientNoteDetail(SqlConnection connection) : base(connection) { }

        #endregion

    }
}