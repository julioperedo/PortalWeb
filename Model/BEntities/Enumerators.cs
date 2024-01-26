namespace BEntities.Enums
{

	public enum Classifiers : long
	{
		Subsidiary = 1,
		Months = 2,
		ChartGroups = 3,
		UserRequestState = 4,
		SaleOnlineState = 5,
		MobileNotificationTopics = 6,
		GatekeeperAppParameters = 7,
		Document = 9,
		VisitReasons = 10,
		SendNotesParameters = 11,
		Transporters = 12,
		TransportSource = 13,
		TransportDestiny = 14,
		TransportType = 15,
		KbytesNotes = 16,
		UsersConfig = 17,
		ClientsNotVerified = 18,
		MailRejectionReasons = 19,
		PriceListMinimunAmounts = 20,
		VacationStates = 21, 
		PiggyBank = 22,
		LicenseReasons = 23,
	}

	public enum Subsidiaries : long
	{
		SantaCruz = 1,
		Miami = 2,
		Iquique = 3
	}

	public enum VisitReasons : long
	{
		Warehouse = 48,
		CashDesk = 49,
		Credit = 50,
		Finances = 51,
		Meeting = 52,
		RMA = 53,
		Sales = 54,
		Other = 55,
		MessengerService = 56
	}

	namespace States
	{
		public enum UserRequest : long
		{
			Created = 21,
			Accepted = 22,
			Rejected = 23,
			Deleted = 24
		}

		public enum SaleOnline : long
		{
			Created = 25,
			Sent = 26,
			Approved = 27,
			Canceled = 28,
			Finished = 29,
			Deliveried = 30
		}

		public enum VacationRequest : long
		{
			Sent = 221,
			Canceled = 222,
			Rejected = 223,
			Aproved = 224,
			Open = 225
		}
	}

	namespace Types
	{
		//public enum FirmwareUpdates : long
		//{
		//    MA = 47
		//}
		public enum Transport : long
		{
			Products = 149,
			Documents = 150,
			Spares = 151
		}
		public enum Profile : long
		{
			Administrator = 1,
			Management = 4,
			Sales = 5,
			ProductManagement = 6,
			Marketing = 9,
			FinanceManagement = 10,
			Guards = 13,
			Reception = 14,
			SpecialSales = 15,
			Finances = 17,
			TechSupport = 19
		}
	}

}