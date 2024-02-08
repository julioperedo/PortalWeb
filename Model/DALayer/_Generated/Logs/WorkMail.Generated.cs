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
    /// Class     : WorkMail
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type WorkMail 
    ///     for the service Logs.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Logs
    /// </remarks>
    /// <history>
    ///     [DMC]   10/8/2022 11:13:48 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class WorkMail : DALEntity<BEG.WorkMail>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type WorkMail
        /// </summary>
        /// <param name="Item">Business object of type WorkMail </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEG.WorkMail Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Logs].[WorkMail]([FirstName], [LastName], [Birthday], [MaritalStatus], [IdentitynDoc], [City], [Address], [Cellphone], [Phone], [EMail], [MeetRequirements], [AcademicTraining], [Experience], [Languages], [Hobbies], [AboutYourself], [References], [WhyUs], [Achievements], [LeavingReason], [LaboralExperience], [Position], [SalaryPretension], [TravelAvailability], [LinkCV], [LogUser], [LogDate]) VALUES(@FirstName, @LastName, @Birthday, @MaritalStatus, @IdentitynDoc, @City, @Address, @Cellphone, @Phone, @EMail, @MeetRequirements, @AcademicTraining, @Experience, @Languages, @Hobbies, @AboutYourself, @References, @WhyUs, @Achievements, @LeavingReason, @LaboralExperience, @Position, @SalaryPretension, @TravelAvailability, @LinkCV, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Logs].[WorkMail] SET [FirstName] = @FirstName, [LastName] = @LastName, [Birthday] = @Birthday, [MaritalStatus] = @MaritalStatus, [IdentitynDoc] = @IdentitynDoc, [City] = @City, [Address] = @Address, [Cellphone] = @Cellphone, [Phone] = @Phone, [EMail] = @EMail, [MeetRequirements] = @MeetRequirements, [AcademicTraining] = @AcademicTraining, [Experience] = @Experience, [Languages] = @Languages, [Hobbies] = @Hobbies, [AboutYourself] = @AboutYourself, [References] = @References, [WhyUs] = @WhyUs, [Achievements] = @Achievements, [LeavingReason] = @LeavingReason, [LaboralExperience] = @LaboralExperience, [Position] = @Position, [SalaryPretension] = @SalaryPretension, [TravelAvailability] = @TravelAvailability, [LinkCV] = @LinkCV, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Logs].[WorkMail] WHERE [Id] = @Id";
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
        /// 	Saves a collection business information object of type  WorkMail		
        /// </summary>
        /// <param name="Items">Business object of type WorkMail para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEG.WorkMail> Items)
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
        /// 	For use on data access layer at assembly level, return an  WorkMail type object
        /// </summary>
        /// <param name="Id">Object Identifier WorkMail</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type WorkMail</returns>
        /// <remarks>
        /// </remarks>		
        internal BEG.WorkMail ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto WorkMail de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a WorkMail</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo WorkMail</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEG.WorkMail> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEG.WorkMail> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Logs].[WorkMail] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEG.WorkMail> Items, params Enum[] Relations)
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
        protected override void LoadRelations(ref BEG.WorkMail Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {

            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of WorkMail
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type WorkMail</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEG.WorkMail> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Logs].[WorkMail] ORDER By " + Order;
            IEnumerable<BEG.WorkMail> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of WorkMail
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEG.WorkMail> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Logs].[WorkMail] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEG.WorkMail> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of WorkMail
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type WorkMail</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEG.WorkMail> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Logs].[WorkMail] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEG.WorkMail> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type WorkMail    	
        /// </summary>
        /// <param name="Id">Object identifier WorkMail</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type WorkMail</returns>
        /// <remarks>
        /// </remarks>    
        public BEG.WorkMail Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Logs].[WorkMail] WHERE [Id] = @Id";
            BEG.WorkMail Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public WorkMail() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public WorkMail(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal WorkMail(SqlConnection connection) : base(connection) { }

        #endregion

    }
}