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


namespace DALayer.Kbytes
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : Kbytes
    /// Class     : StatusDetail
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type StatusDetail 
    ///     for the service Kbytes.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Kbytes
    /// </remarks>
    /// <history>
    ///     [DMC]   7/3/2022 18:16:38 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class StatusDetail : DALEntity<BEK.StatusDetail>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type StatusDetail
        /// </summary>
        /// <param name="Item">Business object of type StatusDetail </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEK.StatusDetail Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Kbytes].[StatusDetail]([IdStatus], [IdStatusUsed], [IdAward], [IdNote], [Points], [ExtraPoints], [ExtraPointsPeriod], [AcceleratorPeriod], [TotalPoints], [Amount], [TotalAmount], [LogUser], [LogDate]) VALUES(@IdStatus, @IdStatusUsed, @IdAward, @IdNote, @Points, @ExtraPoints, @ExtraPointsPeriod, @AcceleratorPeriod, @TotalPoints, @Amount, @TotalAmount, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Kbytes].[StatusDetail] SET [IdStatus] = @IdStatus, [IdStatusUsed] = @IdStatusUsed, [IdAward] = @IdAward, [IdNote] = @IdNote, [Points] = @Points, [ExtraPoints] = @ExtraPoints, [ExtraPointsPeriod] = @ExtraPointsPeriod, [AcceleratorPeriod] = @AcceleratorPeriod, [TotalPoints] = @TotalPoints, [Amount] = @Amount, [TotalAmount] = @TotalAmount, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Kbytes].[StatusDetail] WHERE [Id] = @Id";
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
        /// 	Saves a collection business information object of type  StatusDetail		
        /// </summary>
        /// <param name="Items">Business object of type StatusDetail para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEK.StatusDetail> Items)
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
        /// 	For use on data access layer at assembly level, return an  StatusDetail type object
        /// </summary>
        /// <param name="Id">Object Identifier StatusDetail</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type StatusDetail</returns>
        /// <remarks>
        /// </remarks>		
        internal BEK.StatusDetail ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto StatusDetail de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a StatusDetail</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo StatusDetail</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEK.StatusDetail> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEK.StatusDetail> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Kbytes].[StatusDetail] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEK.StatusDetail> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Kbytes.ClaimedAward> lstAwards = null;
			IEnumerable<BE.Kbytes.ClientNote> lstNotes = null;
			IEnumerable<BE.Kbytes.Status> lstStatuss = null;
			IEnumerable<BE.Kbytes.Status> lstStatusUseds = null;

            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Kbytes.relStatusDetail.Award))
				{
					using(var dal = new ClaimedAward(Connection))
					{
						Keys = (from i in Items where i.IdAward.HasValue select i.IdAward.Value).Distinct();
						lstAwards = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}				if (RelationEnum.Equals(BE.Kbytes.relStatusDetail.Note))
				{
					using(var dal = new ClientNote(Connection))
					{
						Keys = (from i in Items where i.IdNote.HasValue select i.IdNote.Value).Distinct();
						lstNotes = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}				if (RelationEnum.Equals(BE.Kbytes.relStatusDetail.Status))
				{
					using(var dal = new Status(Connection))
					{
						Keys = (from i in Items select i.IdStatus).Distinct();
						lstStatuss = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}				if (RelationEnum.Equals(BE.Kbytes.relStatusDetail.StatusUsed))
				{
					using(var dal = new Status(Connection))
					{
						Keys = (from i in Items select i.IdStatusUsed).Distinct();
						lstStatusUseds = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstAwards != null)
					{
						Item.Award = (from i in lstAwards where i.Id == Item.IdAward select i).FirstOrDefault();
					}					if (lstNotes != null)
					{
						Item.Note = (from i in lstNotes where i.Id == Item.IdNote select i).FirstOrDefault();
					}					if (lstStatuss != null)
					{
						Item.Status = (from i in lstStatuss where i.Id == Item.IdStatus select i).FirstOrDefault();
					}					if (lstStatusUseds != null)
					{
						Item.StatusUsed = (from i in lstStatusUseds where i.Id == Item.IdStatusUsed select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BEK.StatusDetail Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Kbytes.relStatusDetail.Award))
				{
					using (var dal = new ClaimedAward(Connection))
					{
						if (Item.IdAward.HasValue)
						{
							Item.Award = dal.ReturnMaster(Item.IdAward.Value, Relations);
						}
					}
				}				if (RelationEnum.Equals(BE.Kbytes.relStatusDetail.Note))
				{
					using (var dal = new ClientNote(Connection))
					{
						if (Item.IdNote.HasValue)
						{
							Item.Note = dal.ReturnMaster(Item.IdNote.Value, Relations);
						}
					}
				}				if (RelationEnum.Equals(BE.Kbytes.relStatusDetail.Status))
				{
					using (var dal = new Status(Connection))
					{
						Item.Status = dal.ReturnMaster(Item.IdStatus, Relations);
					}
				}				if (RelationEnum.Equals(BE.Kbytes.relStatusDetail.StatusUsed))
				{
					using (var dal = new Status(Connection))
					{
						Item.StatusUsed = dal.ReturnMaster(Item.IdStatusUsed, Relations);
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of StatusDetail
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type StatusDetail</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEK.StatusDetail> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Kbytes].[StatusDetail] ORDER By " + Order;
            IEnumerable<BEK.StatusDetail> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of StatusDetail
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEK.StatusDetail> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Kbytes].[StatusDetail] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEK.StatusDetail> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of StatusDetail
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type StatusDetail</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEK.StatusDetail> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Kbytes].[StatusDetail] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEK.StatusDetail> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type StatusDetail    	
        /// </summary>
        /// <param name="Id">Object identifier StatusDetail</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type StatusDetail</returns>
        /// <remarks>
        /// </remarks>    
        public BEK.StatusDetail Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Kbytes].[StatusDetail] WHERE [Id] = @Id";
            BEK.StatusDetail Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public StatusDetail() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public StatusDetail(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal StatusDetail(SqlConnection connection) : base(connection) { }

        #endregion

    }
}