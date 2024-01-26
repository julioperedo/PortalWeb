using Portal.Areas.Misc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using BE = BEntities.HumanResources;

namespace Portal.Areas.HumanResources.Models
{

	public class Request
	{
		public long Id { get; set; }

		public long IdEmployee { get; set; }

		public DateTime RequestDate { get; set; }

		public string IdType { get; set; }

		public long IdState { get; set; }
		public string StateName { get; set; }

		public string Comments { get; set; }

		public string RejectComments { get; set; }

		public int? ExternalCode { get; set; }

		public License License { get; set; }

		public Vacation Vacation { get; set; }

		public HomeOffice HomeOffice { get; set; }
		public Travel Travel { get; set; }

		public BE.Request ToEntity()
		{
			BE.Request request = new()
			{
				Id = Id,
				RequestDate = RequestDate,
				Comments = Comments,
				ExternalCode = ExternalCode,
				IdEmployee = IdEmployee,
				IdState = IdState,
				IdType = IdType,
				RejectComments = RejectComments
			};
			if (IdType == "L")
			{
				request.ListLicenses = new List<BE.License> { License.ToEntity() };
			}
			if (IdType == "V")
			{
				request.ListVacations = new List<BE.Vacation> { Vacation.ToEntity() };
			}
			if (IdType == "H")
			{
				request.ListHomeOffices = new List<BE.HomeOffice> { HomeOffice.ToEntity() };
			}
			if (IdType == "T")
			{
				request.ListTravels = new List<BE.Travel> { Travel.ToEntity() };
			}
			return request;
		}
	}

	public class RequestShort
	{
		public long Id { get; set; }
		public long IdEmployee { get; set; }
		public string IdType { get; set; }
		public DateTime Since { get; set; }
		public string SincePeriod { get; set; }
		public DateTime Until { get; set; }
		public string UntilPeriod { get; set; }
	}

	public class EmployeeResume
	{
		public string Name { get; set; }
		public decimal AvailableDays { get; set; }
	}

	public class License
	{
		public long Id { get; set; }

		public DateTime Date { get; set; }

		public TimeSpan InitialTime { get; set; }

		public TimeSpan FinalTime { get; set; }

		public long? IdReason { get; set; }
		//public string ReasonName { get; set; }

		public string ReasonDescription { get; set; }

		public BE.License ToEntity()
		{
			return new BE.License()
			{
				Date = Date,
				InitialTime = InitialTime,
				FinalTime = FinalTime,
				IdReason = IdReason,
				ReasonDescription = ReasonDescription
			};
		}
	}

	public class Vacation
	{
		public long Id { get; set; }

		public long? IdLicense { get; set; }

		public DateTime FromDate { get; set; }

		public string FromDatePeriod { get; set; }

		public DateTime ToDate { get; set; }

		public string ToDatePeriod { get; set; }

		public decimal Days { get; set; }

		public List<Replacement> Replacements { get; set; }

		public BE.Vacation ToEntity()
		{
			return new BE.Vacation
			{
				Id = Id,
				FromDate = FromDate,
				FromDatePeriod = FromDatePeriod,
				ToDate = ToDate,
				ToDatePeriod = ToDatePeriod,
				IdLicense = IdLicense,
				Days = Days,
				ListVacationReplacements = this.Replacements?.Select(x => x.ToEntity2()).ToList()
			};
		}
	}

	public class HomeOffice
	{
		public long Id { get; set; }

		public DateTime FromDate { get; set; }

		public string FromDatePeriod { get; set; }

		public DateTime ToDate { get; set; }

		public string ToDatePeriod { get; set; }

		public BE.HomeOffice ToEntity()
		{
			return new BE.HomeOffice
			{
				Id = Id,
				FromDate = FromDate,
				FromDatePeriod = FromDatePeriod,
				ToDate = ToDate,
				ToDatePeriod = ToDatePeriod
			};
		}
	}

	public class Travel
	{
		public long Id { get; set; }

		public DateTime FromDate { get; set; }

		public string FromDatePeriod { get; set; }

		public DateTime ToDate { get; set; }

		public string ToDatePeriod { get; set; }

		public string Destiny { get; set; }

		public List<Replacement> Replacements { get; set; }

		public BE.Travel ToEntity()
		{
			return new BE.Travel
			{
				Id = Id,
				FromDate = FromDate,
				FromDatePeriod = FromDatePeriod,
				ToDate = ToDate,
				ToDatePeriod = ToDatePeriod,
				Destiny = Destiny,
				ListTravelReplacements = Replacements?.Select(x => x.ToEntity()).ToList()
			};
		}
	}

	public class Replacement
	{
		public long Id { get; set; }

		public long IdReplacement { get; set; }
		public string ReplacementName { get; set; }

		public DateTime FromDate { get; set; }

		public string FromDatePeriod { get; set; }

		public DateTime ToDate { get; set; }

		public string ToDatePeriod { get; set; }

		public string IdState { get; set; }

		public BE.TravelReplacement ToEntity()
		{
			return new BE.TravelReplacement
			{
				Id = Id,
				IdReplacement = IdReplacement,
				FromDate = FromDate,
				FromDatePeriod = FromDatePeriod,
				ToDate = ToDate,
				ToDatePeriod = ToDatePeriod,
				IdState = IdState
			};
		}

		public BE.VacationReplacement ToEntity2()
		{
			return new BE.VacationReplacement
			{
				Id = Id,
				IdReplacement = IdReplacement,
				FromDate = FromDate,
				FromDatePeriod = FromDatePeriod,
				ToDate = ToDate,
				ToDatePeriod = ToDatePeriod,
				IdState = IdState
			};
		}
	}
}
