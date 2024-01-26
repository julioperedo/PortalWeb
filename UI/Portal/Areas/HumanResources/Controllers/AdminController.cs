using BEntities;
using BEntities.Filters;
using BEntities.HumanResources;
using BEntities.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Portal.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using BCA = BComponents.SAP;
using BCH = BComponents.HumanResources;
using BCS = BComponents.Security;
using BEA = BEntities.SAP;
using BEE = BEntities.Enums;
using BES = BEntities.Security;

namespace Portal.Areas.HumanResources.Controllers
{
	[Area("HumanResources")]
	[Authorize]
	public class AdminController : BaseController
	{
		#region Global Variables

		private readonly IConfiguration _config;

		#endregion

		#region Constructor

		public AdminController(IConfiguration configuration, IWebHostEnvironment hEnviroment) : base(configuration, hEnviroment)
		{
			_config = configuration;
		}

		#endregion

		#region GETs

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult GetEmployeesResume()
		{
			var message = "";
			List<Models.EmployeeVacation> items = new();
			try
			{
				items = GetEmployeesResumeItems("");
			}
			catch (Exception ex)
			{
				message = GetError(ex);
			}
			return Json(new { message, items });
		}

		public async Task<IActionResult> GetRequestsAsync()
		{
			try
			{
				var today = DateTime.Now.ToString("yyyy-MM-dd");
				List<Field> filters = new();
				filters.AddRange(new[] {
					Field.New("IdState", (long)BEE.States.VacationRequest.Sent),
					Field.New("v.ToDate", today, Operators.HigherOrEqualThan),
					Field.New("t.ToDate", today, Operators.HigherOrEqualThan),
					Field.New("ho.ToDate", today, Operators.HigherOrEqualThan),
					Field.New("l.Date", today, Operators.HigherOrEqualThan),
					Field.LogicalOr(), Field.LogicalOr(), Field.LogicalOr(), Field.LogicalAnd(), Field.LogicalAnd()
				});
				BCH.Request bc = new();
				var requests = await bc.ListAsync(filters, "RequestDate DESC", relRequest.State, relRequest.Vacations, relVacation.VacationReplacements, relVacationReplacement.Replacement, relRequest.Travels, relTravel.TravelReplacements, relTravelReplacement.Replacement, relRequest.Licenses, relRequest.HomeOffices, relRequest.Employee, relEmployee.EmployeeDepartment_Employees, relEmployeeDepartment.Manager, relEmployeeDepartment.Department, relLicense.Reason);

				List<long> replaceCodes = new();
				foreach (var r in requests)
				{
					if (r.IdType == "T" & r.ListTravels?.Count > 0)
					{
						replaceCodes.AddRange(r.ListTravels.First().ListTravelReplacements?.Select(x => x.IdReplacement)!);
					}
					if (r.IdType == "V" & r.ListVacations?.Count > 0)
					{
						replaceCodes.AddRange(r.ListVacations.First().ListVacationReplacements?.Select(x => x.IdReplacement)!);
					}
				}
				var replaceRequests = Enumerable.Empty<Request>();
				if (replaceCodes.Count > 0)
				{
					filters = new List<Field> {
						Field.New("IdEmployee", string.Join(",", replaceCodes), Operators.In),
						Field.New("IdState", (long)BEE.States.VacationRequest.Aproved),
						Field.New("v.ToDate", today, Operators.HigherOrEqualThan),
						Field.New("t.ToDate", today, Operators.HigherOrEqualThan),
						Field.LogicalOr(), Field.LogicalAnd(), Field.LogicalAnd()
					};
					replaceRequests = await bc.ListAsync(filters, "1");
				}

				var employeeIds = string.Join(",", requests.Select(x => x.IdEmployee).Distinct());
				var resume = GetEmployeesResumeItems(employeeIds);

				var items = from r in requests
							join e in resume on r.IdEmployee equals e.Id
							select new
							{
								r.Id,
								r.IdEmployee,
								r.IdType,
								r.IdState,
								r.Comments,
								r.RequestDate,
								EmployeeName = r.Employee.Name,
								AvailableDays = e.Remaining,
								Vacation = r.ListVacations?.Select(x => new { x.Id, x.FromDate, x.FromDatePeriod, x.ToDate, x.ToDatePeriod, x.Days, Replacements = x.ListVacationReplacements?.Select(i => new { i.Id, i.IdReplacement, ReplacementName = i.Replacement.Name, i.FromDate, i.FromDatePeriod, i.ToDate, i.ToDatePeriod, i.StatusType, Available = !replaceRequests.Any(rr => rr.IdEmployee == i.IdReplacement & (rr.ListVacations?.Count(rrv => rrv.FromDate <= i.ToDate & rrv.ToDate >= i.FromDate) > 0 || rr.ListTravels?.Count(rrt => rrt.FromDate <= i.ToDate & rrt.ToDate >= i.FromDate) > 0)) }) })?.FirstOrDefault(),
								License = r.ListLicenses?.Select(x => new { x.Id, x.Date, x.InitialTime, x.FinalTime, x.IdReason, x.ReasonDescription, ReasonName = x.Reason?.Name ?? "" })?.FirstOrDefault(),
								HomeOffice = r.ListHomeOffices?.Select(x => new { x.Id, x.FromDate, x.FromDatePeriod, x.ToDate, x.ToDatePeriod })?.FirstOrDefault(),
								Travel = r.ListTravels?.Select(x => new { x.Id, x.FromDate, x.FromDatePeriod, x.ToDate, x.ToDatePeriod, x.Destiny, Replacements = x.ListTravelReplacements?.Select(i => new { i.Id, i.IdReplacement, ReplacementName = i.Replacement.Name, i.FromDate, i.FromDatePeriod, i.ToDate, i.ToDatePeriod, i.StatusType, Available = !replaceRequests.Any(rr => rr.IdEmployee == i.IdReplacement & (rr.ListVacations?.Count(rrv => rrv.FromDate <= i.ToDate & rrv.ToDate >= i.FromDate) > 0 || rr.ListTravels?.Count(rrt => rrt.FromDate <= i.ToDate & rrt.ToDate >= i.FromDate) > 0)) }) })?.FirstOrDefault(),
								Managers = r.Employee.ListEmployeeDepartment_Employees?.Select(d => new { Id = d.IdManager, d.Manager.Name }),
							};

				return Json(items);
			}
			catch (Exception) { }
			return Content("");
		}

		#endregion

		#region POSTs

		[HttpPost]
		public IActionResult Approve(long Id, string Comments)
		{
			string message = "";
			bool withErrors = false;
			try
			{
				BCH.Request bc = new();
				var item = bc.Search(Id, relRequest.Employee, relRequest.Licenses, relRequest.Vacations, relRequest.HomeOffices, relRequest.Travels, relVacation.VacationReplacements, relVacationReplacement.Replacement, relTravel.TravelReplacements, relTravelReplacement.Replacement, relEmployee.EmployeeDepartment_Employees, relEmployeeDepartment.Manager);
				if (item?.IdState == (long)BEE.States.VacationRequest.Sent)
				{
					item.IdState = (long)BEE.States.VacationRequest.Aproved;
					item.Comments = Comments;
					item.LogUser = UserCode;
					item.LogDate = DateTime.Now;
					item.StatusType = StatusType.Update;

					//guardar en SAP la aprovación
					if (item.IdType == "V" || item.IdType == "L")
					{
						var (saved, sapMessage) = SaveInSap(item);
						if (saved)
						{
							item.ExternalCode = int.Parse(sapMessage);
							bc.Save(ref item);
							SendApproveMail(item);
						}
						else
						{
							message = $"Error en SAP: {sapMessage}";
							withErrors = true;
						}
					}
					else
					{
						bc.Save(ref item);
						SendApproveMail(item);
					}
				}
				else
				{
					message = "La solicitud no puede ser aprobada en este momento porque está en edición.";
				}
			}
			catch (Exception ex)
			{
				withErrors = true;
				message = GetError(ex);
			}
			return Json(new { withErrors, message });
		}

		[HttpPost]
		public IActionResult Reject(long Id, string RejectReason)
		{
			string message = "";
			bool withErrors = false;
			try
			{
				BCH.Request bc = new();
				var item = bc.Search(Id, relRequest.Employee, relRequest.Licenses, relRequest.Vacations, relRequest.HomeOffices, relRequest.Travels, relVacation.VacationReplacements, relVacationReplacement.Replacement, relTravel.TravelReplacements, relTravelReplacement.Replacement, relEmployee.EmployeeDepartment_Employees, relEmployeeDepartment.Manager);
				if (item?.IdState == (long)BEE.States.VacationRequest.Sent)
				{
					item.IdState = (long)BEE.States.VacationRequest.Rejected;
					item.RejectComments = RejectReason;
					item.LogUser = UserCode;
					item.LogDate = DateTime.Now;
					item.StatusType = StatusType.Update;

					//guardar en SAP el rechazo
					if (item.IdType == "V" || item.IdType == "L")
					{
						var (saved, sapMessage) = SaveInSap(item);
						if (saved)
						{
							item.ExternalCode = int.Parse(sapMessage);
							bc.Save(ref item);
							SendRejectMail(item);
						}
						else
						{
							message = $"Error en SAP: {sapMessage}";
							withErrors = true;
						}
					}
					else
					{
						bc.Save(ref item);
						SendRejectMail(item);
					}
				}
				else
				{
					message = "La solicitud no puede ser rechazada en este momento porque está en edición.";
				}
			}
			catch (Exception ex)
			{
				message = GetError(ex);
			}
			return Json(new { withErrors, message });
		}

		#endregion

		#region Private Methods

		private static List<Models.EmployeeVacation> GetEmployeesResumeItems(string EmployeeCodes)
		{
			List<Models.EmployeeVacation> items = new();
			int years, months, days, totalDaysWorked, remainingYears;

			BCA.CustomData bcData = new();
			var data = bcData.List().ToList();

			BCA.Employee bcEmployee = new();
			IEnumerable<BEA.EmployeeResume> resume = bcEmployee.ListResumeDaysTaken(EmployeeCodes);
			List<Field> filters = new() { Field.New("startDate", "", Operators.IsNotNull), Field.New("termDate", "", Operators.IsNull), Field.LogicalAnd() };
			if (!string.IsNullOrEmpty(EmployeeCodes))
			{
				filters.AddRange(new[] { Field.New("empID", EmployeeCodes, Operators.In), Field.LogicalAnd() });
			}
			var employees = bcEmployee.List(filters, "LastName, FirstName");
			foreach (var e in employees)
			{
				(years, months, days, totalDaysWorked) = GetTimeParts(e.StartDate, e.TermDate ?? DateTime.Today);
				Models.EmployeeVacation emp = new(e) { Days = 0, Years = years, Months = months };
				remainingYears = years;

				int i = 0;
				while (remainingYears > 0)
				{
					int periodYears = data[i].FinalYear - data[i].InitialYear + 1, usedYears = periodYears > remainingYears ? remainingYears : periodYears;
					emp.Days += (int)(usedYears * data[i].Days);
					remainingYears -= usedYears;
					i++;
				}
				if (months > 0) emp.ExtraDays = ((int)data.First(x => x.InitialYear <= (years + 1) & x.FinalYear >= (years + 1)).Days * months) / 12;

				var r = resume.FirstOrDefault(x => x.EmployeeId == e.Id);
				emp.Taken = r?.Days ?? 0;

				items.Add(emp);
			}
			return items;
		}

		private async void SendApproveMail(Request Item)
		{
			string period = "", replacement = "", manager = "", title = "";
			if (Item.IdType == "L")
			{
				if (Item.ListLicenses.Count > 0)
				{
					var l = Item.ListLicenses.First();
					period += $"<tr><td>Per&iacute;odo:</td><td> <b>{l.Date:dd-MM-yyyy} ( {l.InitialTime:hh\\:mm} - {l.FinalTime:hh\\:mm} )</b><td></tr>";
					title = "Solicitud de Licencia Aprobada";
				}
			}
			if (Item.IdType == "V")
			{
				if (Item.ListVacations.Count > 0)
				{
					var v = Item.ListVacations.First();
					period += $"<tr><td>Per&iacute;odo:</td><td> <b>{v.FromDate:dd-MM-yyyy} {v.FromDatePeriod} - {v.ToDate:dd-MM-yyyy} {v.ToDatePeriod} ( {v.Days} d&iacute;as )</b><td></tr>";
					if (v.ListVacationReplacements.Count > 0)
					{
						foreach (var r in v.ListVacationReplacements)
						{
							replacement += $"<tr><td>Remplazo:</td><td> <b>{r.Replacement.Name}</b><td></tr>";
						}
					}
					title = "Solicitud de Vacaciones Aprobada";
				}
			}
			if (Item.IdType == "H")
			{
				if (Item.ListHomeOffices.Count > 0)
				{
					var h = Item.ListHomeOffices.First();
					period += $"<tr><td>Per&iacute;odo:</td><td> <b>{h.FromDate:dd-MM-yyyy} {h.FromDatePeriod} - {h.ToDate:dd-MM-yyyy} {h.ToDatePeriod}</b><td></tr>";
					title = "Solicitud de Trabajo en Casa Aprobada";
				}
			}
			if (Item.IdType == "T")
			{
				if (Item.ListTravels.Count > 0)
				{
					var t = Item.ListTravels.First();
					period += $"<tr><td>Per&iacute;odo:</td><td> <b>{t.FromDate:dd-MM-yyyy} {t.FromDatePeriod} - {t.ToDate:dd-MM-yyyy} {t.ToDatePeriod}</b><td></tr>";
					if (t.ListTravelReplacements.Count > 0)
					{
						foreach (var r in t.ListTravelReplacements)
						{
							replacement += $"<tr><td>Remplazo:</td><td> <b>{r.Replacement.Name}</b><td></tr>";
						}
					}
					title = "Solicitud de Viaje de Trabajo Aprobada";
				}
			}
			if (Item.Employee.ListEmployeeDepartment_Employees.Count > 0)
			{
				foreach (var d in Item.Employee.ListEmployeeDepartment_Employees)
				{
					manager += $"<tr><td>Jefe:</td><td> <b>{d.Manager.Name}</b><td></tr>";
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
			sb.AppendLine($@"																<div><p class=""subtitle"">{title}</p>&nbsp;");
			sb.AppendLine(@"                                                                <table style=""width:100%"">");
			sb.AppendLine($@"																<tr><td>Nombre:</td><td> <b>{Item.Employee.Name}</b><td></tr>");
			sb.AppendLine($@"																{period}");
			sb.AppendLine($@"																{replacement}");
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
			FillCustomCopies("HumanResources", "Admin", "SendApproveMail", ref copies, ref blindCopies);
			copies.Add(new MailAddress(EMail, UserName));
			if (!IsDevelopmentMode)
			{
				BCS.User bcUser = new();
				BES.User temp = await bcUser.GetByEmployeeIdAsync(Item.IdEmployee);
				destinataries.Add(new MailAddress(temp.EMail, temp.Name));
				if (Item.Employee.ListEmployeeDepartment_Employees.Count > 0)
				{
					foreach (var d in Item.Employee.ListEmployeeDepartment_Employees)
					{
						temp = await bcUser.GetByEmployeeIdAsync(d.IdManager);
						if (temp != null) copies.Add(new MailAddress(temp.EMail, temp.Name));
					}
				}
				if (Item.IdType == "V" & Item.ListVacations?.Count > 0)
				{
					if (Item.ListVacations.First().ListVacationReplacements.Count > 0)
					{
						foreach (var r in Item.ListVacations.First().ListVacationReplacements)
						{
							temp = await bcUser.GetByEmployeeIdAsync(r.IdReplacement);
							if (temp != null) copies.Add(new MailAddress(temp.EMail, temp.Name));
						}
					}
				}
				if (Item.IdType == "T" & Item.ListTravels?.Count > 0)
				{
					if (Item.ListTravels.First().ListTravelReplacements.Count > 0)
					{
						foreach (var r in Item.ListTravels.First().ListTravelReplacements)
						{
							temp = await bcUser.GetByEmployeeIdAsync(r.IdReplacement);
							if (temp != null) copies.Add(new MailAddress(temp.EMail, temp.Name));
						}
					}
				}
			}
			await SendMailAsync(title, sb.ToString(), destinataries, copies, blindCopies);
		}

		private async void SendRejectMail(Request Item)
		{
			string period = "", replacement = "", manager = "", title = "";
			if (Item.IdType == "L")
			{
				if (Item.ListLicenses.Count > 0)
				{
					var l = Item.ListLicenses.First();
					period += $"<tr><td>Per&iacute;odo:</td><td> <b>{l.Date:dd-MM-yyyy} ( {l.InitialTime:hh\\:mm} - {l.FinalTime:hh\\:mm} )</b><td></tr>";
					title = "Solicitud de Licencia Rechazada";
				}
			}
			if (Item.IdType == "V")
			{
				if (Item.ListVacations.Count > 0)
				{
					var v = Item.ListVacations.First();
					period += $"<tr><td>Per&iacute;odo:</td><td> <b>{v.FromDate:dd-MM-yyyy} {v.FromDatePeriod} - {v.ToDate:dd-MM-yyyy} {v.ToDatePeriod} ( {v.Days} d&iacute;as )</b><td></tr>";
					if (v.ListVacationReplacements.Count > 0)
					{
						foreach (var r in v.ListVacationReplacements)
						{
							replacement += $"<tr><td>Remplazo:</td><td> <b>{r.Replacement.Name}</b><td></tr>";
						}
					}
					title = "Solicitud de Vacaciones Rechazada";
				}
			}
			if (Item.IdType == "H")
			{
				if (Item.ListHomeOffices.Count > 0)
				{
					var h = Item.ListHomeOffices.First();
					period += $"<tr><td>Per&iacute;odo:</td><td> <b>{h.FromDate:dd-MM-yyyy} {h.FromDatePeriod} - {h.ToDate:dd-MM-yyyy} {h.ToDatePeriod}</b><td></tr>";
					title = "Solicitud de Trabajo en Casa Rechazada";
				}
			}
			if (Item.IdType == "T")
			{
				if (Item.ListTravels.Count > 0)
				{
					var t = Item.ListTravels.First();
					period += $"<tr><td>Per&iacute;odo:</td><td> <b>{t.FromDate:dd-MM-yyyy} {t.FromDatePeriod} - {t.ToDate:dd-MM-yyyy} {t.ToDatePeriod}</b><td></tr>";
					if (t.ListTravelReplacements.Count > 0)
					{
						foreach (var r in t.ListTravelReplacements)
						{
							replacement += $"<tr><td>Remplazo:</td><td> <b>{r.Replacement.Name}</b><td></tr>";
						}
					}
					title = "Solicitud de Viaje de Trabajo Rechazada";
				}
			}
			if (Item.Employee.ListEmployeeDepartment_Employees.Count > 0)
			{
				foreach (var d in Item.Employee.ListEmployeeDepartment_Employees)
				{
					manager += $"<tr><td>Jefe:</td><td> <b>{d.Manager.Name}</b><td></tr>";
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
			sb.AppendLine(@"		.desc { font-size: 0.9em; }");
			sb.AppendLine(@"		.brand { background-color: #48545E; color: #FFF; font-size: 1.5em; font-weight: 600; line-height: 30px; }");
			sb.AppendLine(@"		p { margin-bottom: 0px; }");
			sb.AppendLine(@"		.subtitle { font-size: 1.3em; font-weight: 600; }");
			sb.AppendLine(@"		.footer { text-align: center; font-size: 0.8em; }");
			sb.AppendLine(@"			.footer a { text-decoration: none; color: #48545E; font-weight: 600; }");
			sb.AppendLine(@"        .image { max-height: 220px; max-width: 220px; }");
			sb.AppendLine(@"        td { color: rgb(90, 90, 90); font-family: 'Montserrat', Verdana, Geneva, sans-serif; padding: 6px; }");
			sb.AppendLine(@"	</style>");
			sb.AppendLine(@"	<!--[if(mso)|(IE)]>");
			sb.AppendLine(@"	<style type=""text/css"">");
			sb.AppendLine(@"		table { border-collapse: collapse !important; mso-table-lspace:0pt !important; mso-table-rspace:0pt !important; }");
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
			sb.AppendLine($@"																<div><p class=""subtitle"">{title}</p>&nbsp;");
			sb.AppendLine(@"                                                                <table style=""width:100%"">");
			sb.AppendLine($@"																<tr><td>Nombre:</td><td> <b>{Item.Employee.Name}</b><td></tr>");
			sb.AppendLine($@"																{period}");
			sb.AppendLine($@"																{replacement}");
			sb.AppendLine($@"																{manager}");
			sb.AppendLine($@"																<tr><td>Motivo Rechazo:</td><td>{Item.RejectComments}</td></tr>");
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
			FillCustomCopies("HumanResources", "Admin", "SendRejectMail", ref copies, ref blindCopies);
			copies.Add(new MailAddress(EMail, UserName));
			if (!IsDevelopmentMode)
			{
				BCS.User bcUser = new();
				BES.User temp = await bcUser.GetByEmployeeIdAsync(Item.IdEmployee);
				destinataries.Add(new MailAddress(temp.EMail, temp.Name));
				if (Item.Employee.ListEmployeeDepartment_Employees.Count > 0)
				{
					foreach (var d in Item.Employee.ListEmployeeDepartment_Employees)
					{
						temp = await bcUser.GetByEmployeeIdAsync(d.IdManager);
						if (temp != null) copies.Add(new MailAddress(temp.EMail, temp.Name));
					}
				}
				if (Item.IdType == "V" & Item.ListVacations?.Count > 0)
				{
					if (Item.ListVacations.First().ListVacationReplacements.Count > 0)
					{
						foreach (var r in Item.ListVacations.First().ListVacationReplacements)
						{
							temp = await bcUser.GetByEmployeeIdAsync(r.IdReplacement);
							if (temp != null) copies.Add(new MailAddress(temp.EMail, temp.Name));
						}
					}
				}
				if (Item.IdType == "T" & Item.ListTravels?.Count > 0)
				{
					if (Item.ListTravels.First().ListTravelReplacements.Count > 0)
					{
						foreach (var r in Item.ListTravels.First().ListTravelReplacements)
						{
							temp = await bcUser.GetByEmployeeIdAsync(r.IdReplacement);
							if (temp != null) copies.Add(new MailAddress(temp.EMail, temp.Name));
						}
					}
				}
			}
			await SendMailAsync(title, sb.ToString(), copies, null, blindCopies);
		}

		private (bool, string) SaveInSap(Request Item)
		{
			bool saved = false;
			string message = "";
			try
			{
				BCS.UserData bcData = new();
				var userData = bcData.SearchByUser(UserCode);

				var config = _config.GetSection("SAPSettings").Get<SAPSettings>();
				BCA.Employee bcEmployee = new();
				int lastId = bcEmployee.GetLastId(), newId = lastId + 1;
				var employee = bcEmployee.Search(Item.IdEmployee);
				var daysTaken = bcEmployee.SearchDaysTaken(Item.IdEmployee);

				int year = 1, totalDays = 0;
				var (totalYears, months, _, _) = GetTimeParts(employee.StartDate, employee?.TermDate ?? DateTime.Today);

				BCA.CustomData bcCustomData = new();
				var daysConfig = bcCustomData.List();

				while (year < totalYears & totalDays < daysTaken.Days)
				{
					var d = daysConfig.First(x => x.InitialYear <= year & x.FinalYear >= year);
					totalDays += (int)d.Days;
					year++;
				}
		
				DateTime fromDate, toDate;
				float days = 0;
				if (Item.IdType == "V")
				{
					fromDate = Item.ListVacations.First().FromDate;
					toDate = Item.ListVacations.First().ToDate;
					days = (float)Item.ListVacations.First().Days;
				}
				else
				{
					fromDate = Item.ListLicenses.First().Date.Add(Item.ListLicenses.First().InitialTime);
					toDate = Item.ListLicenses.First().Date.Add(Item.ListLicenses.First().FinalTime);
				}

				BEA.Vacation vacation = new() { Id = newId.ToString("D8"), DocDate = Item.RequestDate, EmployeeId = (int)Item.IdEmployee, Author = userData.SAPUserId ?? 0, Since = fromDate, Until = toDate, Days = (decimal)days, Year = year, Type = Item.IdType == "V" ? "V" : "P", State = Item.IdState == (long)BEE.States.VacationRequest.Aproved ? "A" : "R", StartDate = employee.StartDate, Comments = Item.IdState == (long)BEE.States.VacationRequest.Aproved ? Item.Comments : Item.RejectComments };
				BCA.Vacation bcVacation = new();

				bcVacation.Save(vacation);
				saved = true;
				message = vacation.Id;
			}
			catch (Exception ex)
			{
				message = GetError(ex);
			}
			return (saved, message);
		}

		#endregion
	}
}
