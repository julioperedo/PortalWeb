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


namespace DALayer.Staff
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : Staff
    /// Class     : Member
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type Member 
    ///     for the service Staff.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Staff
    /// </remarks>
    /// <history>
    ///     [DMC]   7/3/2022 18:16:51 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class Member : DALEntity<BEF.Member>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type Member
        /// </summary>
        /// <param name="Item">Business object of type Member </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEF.Member Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Staff].[Member]([Id], [Name], [ShortName], [Position], [Mail], [Photo], [Phone], [Manager], [IdDepartment], [Order], [LogUser], [LogDate]) VALUES(@Id, @Name, @ShortName, @Position, @Mail, @Photo, @Phone, @Manager, @IdDepartment, @Order, @LogUser, @LogDate)";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Staff].[Member] SET [Name] = @Name, [ShortName] = @ShortName, [Position] = @Position, [Mail] = @Mail, [Photo] = @Photo, [Phone] = @Phone, [Manager] = @Manager, [IdDepartment] = @IdDepartment, [Order] = @Order, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Staff].[Member] WHERE [Id] = @Id";
            }

            if (Item.StatusType != BE.StatusType.NoAction)
            {
				if (Item.StatusType == BE.StatusType.Insert & Item.Id <= 0) Item.Id = GenID("Member", 1);
				Connection.Execute(strQuery, Item);
                Item.StatusType = BE.StatusType.NoAction;
            }
        }

        /// <summary>
        /// 	Saves a collection business information object of type  Member		
        /// </summary>
        /// <param name="Items">Business object of type Member para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEF.Member> Items)
        {
			long lastId, currentId = 1;
			int quantity = Items.Count(i => i.StatusType == BE.StatusType.Insert & i.Id <= 0); 
			if (quantity > 0)
			{
				lastId = GenID("Member", quantity);
				currentId = lastId - quantity + 1;
				if (lastId <= 0) throw new Exception("No se puede generar el identificador " + this.GetType().FullName);
			}

            for (int i = 0; i < Items.Count; i++)
            {
                var Item = Items[i];
                if (Item.StatusType == BE.StatusType.Insert & Item.Id <= 0) Item.Id = currentId++;
                Save(ref Item);
                Items[i] = Item;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 	For use on data access layer at assembly level, return an  Member type object
        /// </summary>
        /// <param name="Id">Object Identifier Member</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Member</returns>
        /// <remarks>
        /// </remarks>		
        internal BEF.Member ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto Member de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a Member</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo Member</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEF.Member> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEF.Member> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Staff].[Member] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEF.Member> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Staff.Department> lstDepartments = null;

            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Staff.relMember.Department))
				{
					using(var dal = new Department(Connection))
					{
						Keys = (from i in Items select i.IdDepartment).Distinct();
						lstDepartments = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstDepartments != null)
					{
						Item.Department = (from i in lstDepartments where i.Id == Item.IdDepartment select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BEF.Member Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Staff.relMember.Department))
				{
					using (var dal = new Department(Connection))
					{
						Item.Department = dal.ReturnMaster(Item.IdDepartment, Relations);
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of Member
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Member</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEF.Member> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Staff].[Member] ORDER By " + Order;
            IEnumerable<BEF.Member> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of Member
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEF.Member> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Staff].[Member] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEF.Member> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of Member
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Member</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEF.Member> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Staff].[Member] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEF.Member> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type Member    	
        /// </summary>
        /// <param name="Id">Object identifier Member</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Member</returns>
        /// <remarks>
        /// </remarks>    
        public BEF.Member Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Staff].[Member] WHERE [Id] = @Id";
            BEF.Member Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public Member() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public Member(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal Member(SqlConnection connection) : base(connection) { }

        #endregion

    }
}