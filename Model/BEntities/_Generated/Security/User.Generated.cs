using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.Security 
{
	/// -----------------------------------------------------------------------------
	/// Project   : BEntities
	/// NameSpace : Security
	/// Class     : User
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Security
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  15/2/2024 13:33:52 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class User : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string FirstName { get; set; }

        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string LastName { get; set; }

        [StringLength(150, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string EMail { get; set; }

        [StringLength(200, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Address { get; set; }

        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Phone { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(150, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Login { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Password { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(15, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string CardCode { get; set; }

        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Position { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdProfile { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public bool Enabled { get; set; }

        public string Commentaries { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public bool AccountHolder { get; set; }

        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Picture { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public bool AllowLinesBlocked { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public bool RequiredLogOff { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Profile Profile { get; set; } 

        public IList<BE.AppData.UserToken> ListUserTokens { get; set; }

        public IList<BE.Online.QuoteSent> ListQuoteSents { get; set; }

        public IList<BE.Online.Sale> ListSales { get; set; }

        public IList<BE.Online.TempSale> ListTempSales { get; set; }

        public IList<BE.Product.Loan> ListLoans { get; set; }

        public IList<BE.Product.Request> ListRequests { get; set; }

        public IList<BE.Sales.Quote> ListQuotes { get; set; }

        public IList<Sellers> ListSellerss { get; set; }

        public IList<SessionHistory> ListSessionHistorys { get; set; }

        public IList<UserActivity> ListUserActivitys { get; set; }

        public IList<UserClient> ListUserClients { get; set; }

        public IList<UserData> ListUserDatas { get; set; }

        public IList<UserPerson> ListUserPersons { get; set; }

        public IList<UserPivotConfig> ListUserPivotConfigs { get; set; }

        public IList<UserProfile> ListUserProfiles { get; set; }


		#endregion

		#region Contructors 

		public User() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator User
    /// </summary>
    /// <remarks></remarks>
    public enum relUser 
     { 
        Profile, UserTokens, QuoteSents, Sales, TempSales, Loans, Requests, Quotes, Sellerss, SessionHistorys, UserActivitys, UserClients, UserDatas, UserPersons, UserPivotConfigs, UserProfiles
	}

}