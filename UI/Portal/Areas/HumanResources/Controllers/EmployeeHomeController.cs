using BEntities;
using BEntities.Filters;
using BEntities.HumanResources;
using Kendo.Mvc.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Portal.Areas.HumanResources.Models;
using Portal.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using BCA = BComponents.SAP;
using BCB = BComponents.Base;
using BCH = BComponents.HumanResources;
using BCS = BComponents.Security;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BEE = BEntities.Enums;
using BEH = BEntities.HumanResources;
using BES = BEntities.Security;

namespace Portal.Areas.HumanResources.Controllers
{
	[Area("HumanResources")]
	[Authorize]
	public class EmployeeHomeController : BaseController
	{
		#region Constructores

		public EmployeeHomeController(IConfiguration config, IWebHostEnvironment env) : base(config, env)
		{
		}

		#endregion

		#region GETs

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult GetEmployeeData()
		{
			string message = "";
			try
			{
				BEA.Employee e = GetEmployee();
				if (e != null)
				{
					int years, months, days, totalDaysWorked, vacationDays = 0, remainingYears, extraVacationDays = 0; //, dayRate = 0;
					(years, months, days, totalDaysWorked) = GetTimeParts(e.StartDate, e?.TermDate ?? DateTime.Today);
					remainingYears = years;

					BCA.CustomData bcData = new();
					var data = bcData.List().ToList();
					int i = 0;
					while (remainingYears > 0)
					{
						int periodYears = data[i].FinalYear - data[i].InitialYear + 1, usedYears = periodYears > remainingYears ? remainingYears : periodYears;
						vacationDays += (int)(usedYears * data[i].Days);
						remainingYears -= usedYears;
						i++;
					}
					if (months > 0) extraVacationDays = ((int)data.First(x => x.InitialYear <= (years + 1) & x.FinalYear >= (years + 1)).Days * months) / 12;

					BCA.Vacation bcVacation = new();
					BEA.Vacation resumeV = bcVacation.SearchDaysTakenByEmployee(e.Id), resumeRejected = bcVacation.SearchDaysRejectedByEmployee(e.Id);

					var item = new { e.Id, e.Name, e.StartDate, e.TermDate, years, months, days, vacationDays, extraVacationDays, daysTaken = resumeV.Days, rejectedDays = resumeRejected.Days };

					return Json(new { message, item });
				}
			}
			catch (Exception ex)
			{
				message = GetError(ex);
			}
			return Json(new { message });
		}

		public IActionResult GetResumeYears()
		{
			string message = "";
			List<YearDetail> items = new();
			try
			{
				BEA.Employee e = GetEmployee();
				int years, months, days, totalDaysWorked;
				(years, months, days, totalDaysWorked) = GetTimeParts(e.StartDate, e.TermDate ?? DateTime.Today);

				BCA.CustomData bcData = new();
				var data = bcData.List();

				for (int i = 1; i <= (years + 1); i++)
				{
					items.Add(new YearDetail
					{
						Number = i,
						Year = e.StartDate.Year + i - 1,
						Days = (int)(data.First(x => x.InitialYear <= i & x.FinalYear >= i).Days * (i > years ? months : 12)) / 12
					});
				}
			}
			catch (Exception ex)
			{
				message = GetError(ex);
			}
			return Json(new { message, items });
		}

		public IActionResult GetResumeDaysTaken(bool Accepted)
		{
			string message = "";
			try
			{
				BEA.Employee e = GetEmployee();
				BCA.Vacation bcVacation = new();
				List<Field> filters = new() { new Field("U_EMPID", e.Id), new Field("U_ESTADO", "A", Accepted ? Operators.Equal : Operators.Different), new Field(LogicalOperators.And) };
				IEnumerable<BEA.Vacation> items = bcVacation.List(filters, "1");
				return Json(new { message, items });
			}
			catch (Exception ex)
			{
				message = GetError(ex);
			}
			return Json(new { message });
		}

		public async Task<IActionResult> GetRequestsAsync(bool OnlyPending)
		{
			BEA.Employee e = GetEmployee();
			IEnumerable<BEH.Request> requests = await GetRequestItemsAsync(e.Id, OnlyPending);
			var items = requests.Select(x => new
			{
				x.Id,
				x.IdEmployee,
				x.RequestDate,
				x.IdType,
				x.IdState,
				StateName = x.State.Name,
				x.Comments,
				x.RejectComments,
				x.ExternalCode,
				x.LogDate,
				Managers = x.Employee.ListEmployeeDepartment_Employees?.Select(d => new { Id = d.IdManager, d.Manager.Name }),
				License = x.ListLicenses?.Select(i => new { i.Id, i.Date, i.InitialTime, i.FinalTime, IdReason = i.IdReason ?? -1, i.ReasonDescription, ReasonName = i.Reason?.Name ?? "" })?.FirstOrDefault(),
				HomeOffice = x.ListHomeOffices?.Select(i => new { i.Id, i.FromDate, i.FromDatePeriod, i.ToDate, i.ToDatePeriod })?.FirstOrDefault(),
				Vacation = x.ListVacations?.Select(i => new { i.Id, i.IdLicense, i.FromDate, i.FromDatePeriod, i.ToDate, i.ToDatePeriod, i.Days, Replacements = i.ListVacationReplacements?.Select(r => new { r.Id, r.IdReplacement, ReplacementName = r.Replacement.Name, r.FromDate, r.FromDatePeriod, r.ToDate, r.ToDatePeriod, r.StatusType }) })?.FirstOrDefault(),
				Travel = x.ListTravels?.Select(i => new { i.Id, i.Destiny, i.FromDate, i.ToDate, Replacements = i.ListTravelReplacements?.Select(r => new { r.Id, r.IdReplacement, ReplacementName = r.Replacement.Name, r.FromDate, r.FromDatePeriod, r.ToDate, r.ToDatePeriod, r.StatusType }) })?.FirstOrDefault()
			});
			return Json(items);
		}

		public IActionResult GetHolidays()
		{
			string message = "";
			IEnumerable<BEA.Holiday> items = default;
			try
			{
				BCA.Holiday bcHoliday = new();
				items = bcHoliday.List();
			}
			catch (Exception ex)
			{
				message = GetError(ex);
			}
			return Json(new { message, items });
		}

		public IActionResult GetLicenseReasons()
		{
			BCB.Classifier bc = new();
			var reasons = bc.List((long)BEE.Classifiers.LicenseReasons, "1");
			reasons.Add(new BEB.Classifier { Id = -1, Name = "Otra razón" });
			var items = reasons.Select(x => new { x.Id, x.Name });
			return Json(items);
		}

		#endregion

		#region POSTs

		[HttpPost]
		public async Task<IActionResult> EditAsync(Models.Request Item)
		{
			string message = "";
			try
			{
				string dFormat = "yyyy-MM-dd";
				BEA.Employee e = GetEmployee();
				BCH.Request bc = new();

				List<Field> filters = new() {
					new Field("r.Id", Item.Id, Operators.Different),
					new Field("IdEmployee",e.Id),
					new Field("r.IdState", (long)BEE.States.VacationRequest.Aproved, Operators.Different),
					new Field("r.IdState", (long)BEE.States.VacationRequest.Sent, Operators.Different),
				};
				if (Item.IdType == "L")
				{
					filters.Add(Field.New("Date", Item.License.Date.ToString(dFormat)));
				}
				if (Item.IdType == "V")
				{
					filters.AddRange(new[] {
						Field.New("v.FromDate", Item.Vacation.ToDate.ToString(dFormat), Operators.HigherOrEqualThan),
						Field.New("v.ToDate", Item.Vacation.FromDate.ToString(dFormat), Operators.LowerOrEqualThan)
					});
				}
				if (Item.IdType == "H")
				{
					filters.AddRange(new[] {
						Field.New("ho.FromDate", Item.Vacation.ToDate.ToString(dFormat), Operators.HigherOrEqualThan),
						Field.New("ho.ToDate", Item.Vacation.FromDate.ToString(dFormat), Operators.LowerOrEqualThan)
					});
				}
				if (Item.IdType == "T")
				{
					filters.AddRange(new[] {
						Field.New("t.FromDate", Item.Vacation.ToDate.ToString(dFormat), Operators.HigherOrEqualThan),
						Field.New("t.ToDate", Item.Vacation.FromDate.ToString(dFormat), Operators.LowerOrEqualThan)
					});
				}
				CompleteFilters(ref filters);
				var results = await bc.ListAsync(filters, "1");
				if (results.Any())
				{
					message = "Ya existe una solicitud en ese periodo.";
				}
				else
				{
					var item = Item.ToEntity();

					item.IdEmployee = e.Id;
					item.StatusType = item.Id == 0 ? StatusType.Insert : StatusType.Update;
					item.IdState = item.IdState;
					item.LogUser = UserCode;
					item.LogDate = DateTime.Now;
					if (item.ListLicenses != null)
					{
						foreach (var l in item.ListLicenses)
						{
							l.StatusType = l.Id > 0 ? StatusType.Update : StatusType.Insert;
							l.LogUser = UserCode;
							l.LogDate = DateTime.Now;
						}
					}
					if (item.ListVacations != null)
					{
						foreach (var v in item.ListVacations)
						{
							v.StatusType = v.Id > 0 ? StatusType.Update : StatusType.Insert;
							v.LogUser = UserCode;
							v.LogDate = DateTime.Now;
							if (v.ListVacationReplacements != null)
							{
								foreach (var r in v.ListVacationReplacements)
								{
									r.StatusType = r.Id > 0 ? StatusType.Update : StatusType.Insert;
									r.LogUser = UserCode;
									r.LogDate = DateTime.Now;
								}
							}
						}
					}
					if (item.ListHomeOffices != null)
					{
						foreach (var h in item.ListHomeOffices)
						{
							h.StatusType = h.Id > 0 ? StatusType.Update : StatusType.Insert;
							h.LogUser = UserCode;
							h.LogDate = DateTime.Now;
						}
					}
					if (item.ListTravels != null)
					{
						foreach (var t in item.ListTravels)
						{
							t.StatusType = t.Id > 0 ? StatusType.Update : StatusType.Insert;
							t.LogUser = UserCode;
							t.LogDate = DateTime.Now;
							if (t.ListTravelReplacements != null)
							{
								foreach (var r in t.ListTravelReplacements)
								{
									r.StatusType = r.Id > 0 ? StatusType.Update : StatusType.Insert;
									r.LogUser = UserCode;
									r.LogDate = DateTime.Now;
								}
							}
						}
					}

					bc.Save(ref item);

					var requests = await GetRequestItemsAsync(e.Id, true);
					var items = requests.Select(x => new
					{
						x.Id,
						x.IdEmployee,
						x.RequestDate,
						x.IdType,
						x.IdState,
						StateName = x.State.Name,
						x.Comments,
						x.RejectComments,
						x.ExternalCode,
						x.LogDate,
						Managers = x.Employee.ListEmployeeDepartment_Employees?.Select(d => new { Id = d.IdManager, d.Manager.Name }),
						License = x.ListLicenses?.Select(i => new { i.Id, i.Date, i.InitialTime, i.FinalTime, IdReason = i.IdReason ?? -1, i.ReasonDescription, ReasonName = i.Reason?.Name ?? "" })?.FirstOrDefault(),
						HomeOffice = x.ListHomeOffices?.Select(i => new { i.Id, i.FromDate, i.FromDatePeriod, i.ToDate, i.ToDatePeriod })?.FirstOrDefault(),
						Vacation = x.ListVacations?.Select(i => new { i.Id, i.IdLicense, i.FromDate, i.FromDatePeriod, i.ToDate, i.ToDatePeriod, i.Days, Replacements = i.ListVacationReplacements?.Select(r => new { r.Id, r.IdReplacement, ReplacementName = r.Replacement.Name, r.FromDate, r.FromDatePeriod, r.ToDate, r.ToDatePeriod, r.StatusType }) })?.FirstOrDefault(),
						Travel = x.ListTravels?.Select(i => new { i.Id, i.Destiny, i.FromDate, i.ToDate, Replacements = i.ListTravelReplacements?.Select(r => new { r.Id, r.IdReplacement, ReplacementName = r.Replacement.Name, r.FromDate, r.FromDatePeriod, r.ToDate, r.ToDatePeriod, r.StatusType }) })?.FirstOrDefault()
					});
					if (item.IdState == (long)BEE.States.VacationRequest.Sent) SendMail(item.Id);

					return Json(new { message, items });
				}
			}
			catch (Exception ex)
			{
				message = GetError(ex);
			}
			return Json(new { message });
		}

		[HttpPost]
		public async Task<IActionResult> ChangeStateAsync(BEH.Request Item, long NewStateId)
		{
			string message = "";
			try
			{
				BCH.Request bc = new();
				await bc.UpdateStateAsync(Item.Id, NewStateId, UserCode, DateTime.Now);
				if (NewStateId == (long)BEE.States.VacationRequest.Sent) SendMail(Item.Id);
			}
			catch (Exception ex)
			{
				message = GetError(ex);
			}
			return Json(new { message });
		}

		[HttpPost]
		public async Task<IActionResult> DeleteRequestAsync(long Id)
		{
			string message = "";
			try
			{
				BEH.Request item = new() { Id = Id, LogDate = DateTime.Now, RequestDate = DateTime.Now, StatusType = StatusType.Delete };
				BCH.Request bc = new();
				bc.Save(ref item);

				BEA.Employee e = GetEmployee();
				var requests = await GetRequestItemsAsync(e.Id, true);
				var items = requests.Select(x => new
				{
					x.Id,
					x.IdEmployee,
					x.RequestDate,
					x.IdType,
					x.IdState,
					StateName = x.State.Name,
					x.Comments,
					x.RejectComments,
					x.ExternalCode,
					x.LogDate,
					Managers = x.Employee.ListEmployeeDepartment_Employees?.Select(d => new { Id = d.IdManager, d.Manager.Name }),
					License = x.ListLicenses?.Select(i => new { i.Id, i.Date, i.InitialTime, i.FinalTime, IdReason = i.IdReason ?? -1, i.ReasonDescription, ReasonName = i.Reason?.Name ?? "" })?.FirstOrDefault(),
					HomeOffice = x.ListHomeOffices?.Select(i => new { i.Id, i.FromDate, i.FromDatePeriod, i.ToDate, i.ToDatePeriod })?.FirstOrDefault(),
					Vacation = x.ListVacations?.Select(i => new { i.Id, i.IdLicense, i.FromDate, i.FromDatePeriod, i.ToDate, i.ToDatePeriod, i.Days, Replacements = i.ListVacationReplacements?.Select(r => new { r.Id, r.IdReplacement, ReplacementName = r.Replacement.Name, r.FromDate, r.FromDatePeriod, r.ToDate, r.ToDatePeriod, r.StatusType }) })?.FirstOrDefault(),
					Travel = x.ListTravels?.Select(i => new { i.Id, i.Destiny, i.FromDate, i.ToDate, Replacements = i.ListTravelReplacements?.Select(r => new { r.Id, r.IdReplacement, ReplacementName = r.Replacement.Name, r.FromDate, r.FromDatePeriod, r.ToDate, r.ToDatePeriod, r.StatusType }) })?.FirstOrDefault()
				});
				return Json(new { message, items });
			}
			catch (Exception ex)
			{
				message = GetError(ex);
			}
			return Json(new { message });
		}

		#endregion

		#region Private Methods   

		private static async Task<IEnumerable<BEH.Request>> GetRequestItemsAsync(long Id, bool OnlyPending)
		{
			List<Field> filters = new() { new Field("r.IdEmployee", Id) };
			if (OnlyPending)
			{
				string today = DateTime.Now.ToString("yyyy-MM-dd");
				filters.AddRange(new[] {
					Field.New("v.ToDate", today, Operators.HigherOrEqualThan),
					Field.New("t.ToDate", today, Operators.HigherOrEqualThan),
					Field.New("ho.ToDate", today, Operators.HigherOrEqualThan),
					Field.New("l.Date", today, Operators.HigherOrEqualThan),
					Field.LogicalOr(), Field.LogicalOr(), Field.LogicalOr(), Field.LogicalAnd()
				});
			}
			BCH.Request bc = new();
			IEnumerable<BEH.Request> items = await bc.ListAsync(filters, "RequestDate DESC", relRequest.State, relRequest.Vacations, relVacation.VacationReplacements, relVacationReplacement.Replacement, relRequest.Travels, relTravel.TravelReplacements, relTravelReplacement.Replacement, relRequest.Licenses, relRequest.HomeOffices, relRequest.Employee, relEmployee.EmployeeDepartment_Employees, relEmployeeDepartment.Manager, relLicense.Reason);

			return items;
		}

		private async void SendMail(long Id)
		{
			BCH.Request bc = new();
			var Item = bc.Search(Id, relRequest.State, relRequest.Vacations, relVacation.VacationReplacements, relVacationReplacement.Replacement, relRequest.Travels, relTravel.TravelReplacements, relTravelReplacement.Replacement, relRequest.Licenses, relRequest.HomeOffices, relRequest.Employee, relEmployee.EmployeeDepartment_Employees, relEmployeeDepartment.Manager, relLicense.Reason);

			string period = "", replacements = "", type = "", manager = "";
			if (Item.Employee.ListEmployeeDepartment_Employees.Count > 0)
			{
				foreach (var m in Item.Employee.ListEmployeeDepartment_Employees)
				{
					manager = $"<tr><td>Jefe:</td><td>{m.Manager?.Name ?? ""}</td></tr>";
				}
			}
			if (Item.IdType == "L" & Item.ListLicenses?.Count > 0)
			{
				var l = Item.ListLicenses.First();
				type = "Permiso";
				period = $"{l.Date:dd-MM-yyyy} ( {l.InitialTime:hh\\:mm} al {l.FinalTime:hh\\:mm} )";
			}
			if (Item.IdType == "V" & Item.ListVacations?.Count > 0)
			{
				var v = Item.ListVacations.First();
				type = "Vacaciones";
				period = $"{v.FromDate:dd-MM-yyyy} {v.FromDatePeriod} al {v.ToDate:dd-MM-yyyy} {v.ToDatePeriod}";
				if (v.ListVacationReplacements?.Count > 0)
				{
					replacements = "<tr><td>Remplazos:</td><td>";
					foreach (var r in v.ListVacationReplacements)
					{
						replacements += $"{r.Replacement.Name} &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ( {r.FromDate:dd-MM-yyyy} {r.FromDatePeriod} al {r.ToDate:dd-MM-yyyy} {r.ToDatePeriod} )<br />";
					}
					replacements += "</td></tr>";
				}
			}
			if (Item.IdType == "H" & Item.ListHomeOffices?.Count > 0)
			{
				var h = Item.ListHomeOffices.First();
				type = "Trabajo en Casa";
				period = $"{h.FromDate:dd-MM-yyyy} {h.FromDatePeriod} al {h.ToDate:dd-MM-yyyy} {h.ToDatePeriod}";
			}
			if (Item.IdType == "T" & Item.ListTravels?.Count > 0)
			{
				var t = Item.ListTravels.First();
				type = "Viaje de Trabajo";
				period = $"{t.FromDate:dd-MM-yyyy} {t.FromDatePeriod} al {t.ToDate:dd-MM-yyyy} {t.ToDatePeriod}";
				if (t.ListTravelReplacements?.Count > 0)
				{
					replacements = "<tr><td>Remplazos:</td><td>";
					foreach (var r in t.ListTravelReplacements)
					{
						replacements += $"{r.Replacement.Name} &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ( {r.FromDate:dd-MM-yyyy} {r.FromDatePeriod} al {r.ToDate:dd-MM-yyyy} {r.ToDatePeriod} )<br />";
					}
					replacements += "</td></tr>";
				}
			}

			var sb = new StringBuilder();
			sb.AppendLine(@"<!DOCTYPE html>");
			sb.AppendLine(@"<html>");
			sb.AppendLine(@"<head>");
			sb.AppendLine(@"	<meta charset=""utf-8"" />");
			sb.AppendLine(@"	<!--[if !mso]><!-->");
			sb.AppendLine(@"	<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />");
			sb.AppendLine(@"	<meta http-equiv=""X-UA-Compatible"" content=""IE=edge"" /> <!--<![endif]-->");
			sb.AppendLine(@"	<meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">");
			sb.AppendLine(@"	<title></title>");
			sb.AppendLine(@"    <link href=""http://fonts.googleapis.com/css?family=Montserrat"" rel=""stylesheet"" type=""text/css"">");
			sb.AppendLine(@"	<style type=""text/css"">");
			sb.AppendLine(@"		.align-center { margin: 0 auto; }");
			sb.AppendLine(@"		.text-align-center { text-align: center; }");
			sb.AppendLine(@"		.text-align-left { text-align: left; }");
			sb.AppendLine(@"		.text-align-right { text-align: right; }");
			sb.AppendLine(@"		a[x-apple-data-detectors=true] { text-decoration: none !important; color: inherit !important; }");
			sb.AppendLine(@"		div[style*=""margin: 16px 0""] { margin: 0 !important; }");
			sb.AppendLine(@"		@media screen and (min-width: 370px) and (max-width: 499px) {");
			sb.AppendLine(@"			.outer { width: 85% !important; margin: 0 auto !important; }");
			sb.AppendLine(@"		}");
			sb.AppendLine(@"		p { margin-bottom: 0px; }");
			sb.AppendLine(@"		.subtitle { font-size: 1.3em; font-weight: 600; }");
			sb.AppendLine(@"		.footer { text-align: center; font-size: 0.8em; }");
			sb.AppendLine(@"			.footer a { text-decoration: none; color: #48545E; font-weight: 600; }");
			sb.AppendLine(@"        .image { max-height: 220px; max-width: 220px; }");
			sb.AppendLine(@"        td { color: rgb(90, 90, 90); font-family: 'Montserrat', Verdana, Geneva, sans-serif; padding: 6px; }");
			sb.AppendLine(@"	</style>");
			sb.AppendLine(@"	<!--[if(mso)|(IE)]>");
			sb.AppendLine(@"	<style type=""text/css"">");
			sb.AppendLine(@"		table {border-collapse: collapse !important; mso-table-lspace:0pt !important; mso-table-rspace:0pt !important;}");
			sb.AppendLine(@"		table, div, td {font-family: 'Montserrat', Verdana, Geneva, sans-serif !important;}");
			sb.AppendLine(@"	</style>");
			sb.AppendLine(@"	<![endif]-->");
			sb.AppendLine(@"	<!--[if mso]>");
			sb.AppendLine(@"		<style type=""text/css"">");
			sb.AppendLine(@"			ol, ul {margin-left: 25px !important; margin-top: 0 !important;}");
			sb.AppendLine(@"		</style>");
			sb.AppendLine(@"		<xml>");
			sb.AppendLine(@"		  <o:OfficeDocumentSettings>");
			sb.AppendLine(@"			<o:AllowPNG/>");
			sb.AppendLine(@"			<o:PixelsPerInch>96</o:PixelsPerInch>");
			sb.AppendLine(@"		 </o:OfficeDocumentSettings>");
			sb.AppendLine(@"		</xml>");
			sb.AppendLine(@"		<![endif]-->");
			sb.AppendLine(@"</head>");
			sb.AppendLine(@"<body style=""margin: 0px; padding: 0px; font-family: 'Montserrat', Verdana, Geneva, sans-serif; background-color: rgb(217, 217, 214); -webkit-font-smoothing: antialiased; -moz-osx-font-smoothing: grayscale;"">");
			sb.AppendLine(@"	<table align=""center"" class=""wrapper"" style=""margin: 0px auto; width: 100%; font-family: Montserrat, Verdana, Geneva, sans-serif; table-layout: fixed; -ms-text-size-adjust: 100%;color: rgb(148, 148, 148); background-color: rgb(217, 217, 214); -webkit-text-size-adjust: 100%;"" border=""0"" cellspacing=""0"" cellpadding=""0"">");
			sb.AppendLine(@"		<tbody>");
			sb.AppendLine(@"			<tr>");
			sb.AppendLine(@"				<td align=""center"">");
			sb.AppendLine(@"					<table class=""outer"" style=""margin: 0px auto; width: 100%; max-width: 740px;"" border=""0"" cellspacing=""0"" cellpadding=""0"">");
			sb.AppendLine(@"						<tbody>");
			sb.AppendLine(@"							<tr>");
			sb.AppendLine(@"								<td id=""main-container"" bgcolor=""#ffffff""><br />");
			sb.AppendLine(@"									<table style=""text-align: left; color: rgb(148, 148, 148); line-height: 20px; font-size: 14px;"" border=""0"" cellspacing=""0"" cellpadding=""0"">");
			sb.AppendLine(@"										<tbody>");
			sb.AppendLine(@"											<tr>");
			sb.AppendLine(@"												<td>");
			sb.AppendLine(@"													<table style=""width: 100%;"">");
			sb.AppendLine(@"														<tr>");
			sb.AppendLine(@"															<td style=""width: 20px;"">&nbsp;</td>");
			sb.AppendLine(@"															<td>");
			sb.AppendLine($@"					                                            <img src=""http://www.dmc.bo/img/logo3.png"" class=""logo"" height=""70"" />");
			sb.AppendLine($@"																<div><p class=""subtitle"">Solicitud de {type}</p>&nbsp;");
			sb.AppendLine(@"                                                                <table style=""width:100%"">");
			sb.AppendLine($@"																<tr><td>Nombre:</td><td> <b>{Item.Employee.Name}</b><td></tr>");
			sb.AppendLine($@"																<tr><td>Per&iacute;odo:</td><td>{period}<td></tr>");
			sb.AppendLine($@"																{replacements}");
			sb.AppendLine($@"																{manager}");
			sb.AppendLine(@"																</table>&nbsp;");
			sb.AppendLine(@"															</td>");
			sb.AppendLine(@"															<td style=""width: 20px;"">&nbsp;</td>");
			sb.AppendLine(@"														</tr>");
			sb.AppendLine(@"													</table>");
			sb.AppendLine(@"												</td>");
			sb.AppendLine(@"											</tr>");
			sb.AppendLine(@"										</tbody>");
			sb.AppendLine(@"									</table>");
			sb.AppendLine(@"								</td>");
			sb.AppendLine(@"							</tr> ");
			sb.AppendLine(@"							<tr><td>&nbsp;</td></tr>");
			sb.AppendLine(@"						</tbody>");
			sb.AppendLine(@"					</table>");
			sb.AppendLine(@"				</td>");
			sb.AppendLine(@"			</tr>");
			sb.AppendLine(@"		</tbody>");
			sb.AppendLine(@"	</table>");
			sb.AppendLine(@"</body>");
			sb.AppendLine(@"</html>");

			List<MailAddress> destinataries = new(), copies = new(), blindCopies = new();
			FillCustomCopies("HumanResources", "Employee", "SendMail", ref copies, ref blindCopies);
			copies.Add(new MailAddress(EMail, UserName));
			if (!IsDevelopmentMode)
			{
				BCS.User bcUser = new();
				if (Item.Employee.ListEmployeeDepartment_Employees.Count > 0)
				{
					foreach (var m in Item.Employee.ListEmployeeDepartment_Employees)
					{
						string name = m.Employee.Name, email = m.Employee.Mail;
						if (!IsValidEmail(email))
						{
							var managerUser = await bcUser.GetByEmployeeIdAsync((int)m.Employee.Id);
							if (managerUser != null) email = managerUser.EMail;
						}
						if (!string.IsNullOrEmpty(email)) destinataries.Add(new MailAddress(email, name));
					}
				}
				if (Item.IdType == "V")
				{
					foreach (var r in Item.ListVacations.First().ListVacationReplacements)
					{
						string name = r.Replacement.Name, email = r.Replacement.Mail;
						if (!IsValidEmail(email))
						{
							var managerUser = await bcUser.GetByEmployeeIdAsync((int)r.Replacement.Id);
							if (managerUser != null) email = managerUser.EMail;
						}
						if (!string.IsNullOrEmpty(email)) destinataries.Add(new MailAddress(email, name));
					}
				}
				if (Item.IdType == "T")
				{
					foreach (var r in Item.ListTravels.First().ListTravelReplacements)
					{
						string name = r.Replacement.Name, email = r.Replacement.Mail;
						if (!IsValidEmail(email))
						{
							var managerUser = await bcUser.GetByEmployeeIdAsync((int)r.Replacement.Id);
							if (managerUser != null) email = managerUser.EMail;
						}
						if (!string.IsNullOrEmpty(email)) destinataries.Add(new MailAddress(email, name));
					}
				}
			}
			await SendMailAsync($"Solicitud de {type}", sb.ToString(), destinataries, copies, blindCopies);
		}

		#endregion
	}
}
