using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Telerik.Reporting.Cache.File;
using Telerik.Reporting.Services;
using Telerik.Reporting.Services.AspNetCore;

namespace Portal.Controllers
{
    [Route("api/reports")]
    [AllowAnonymous]
    public class ReportsController : ReportsControllerBase
    {
        readonly string reportsPath = string.Empty;
        protected string reportsCachePath = string.Empty;

        //public ReportsController(ConfigurationService configSvc) {
        //    this.reportsPath = Path.Combine(configSvc.Environment.WebRootPath, "reports"); //concatenate the path using the OS path delimiter.
        //    reportsCachePath = Path.Combine(configSvc.Environment.WebRootPath, "reportsCache");

        //    this.ReportServiceConfiguration = new ReportServiceConfiguration {
        //        ReportingEngineConfiguration = configSvc.Configuration,
        //        HostAppId = "PortalV3",
        //        Storage = new FileStorage(reportsCachePath),
        //        ReportResolver = new ReportTypeResolver().AddFallbackResolver(new ReportFileResolver(this.reportsPath)),
        //    };
        //}
        public ReportsController(IReportServiceConfiguration reportServiceConfiguration) : base(reportServiceConfiguration) { }

        [HttpGet("reportlist")]
        public IEnumerable<string> GetReports()
        {
            return Directory.GetFiles(this.reportsPath).Select(path => Path.GetFileName(path));
        }

        //[HttpPost("clients")]
        //public new IActionResult RegisterClient()
        //{
        //    try
        //    {
        //        return base.RegisterClient();
        //    }
        //    catch (System.Exception)
        //    {
        //        throw;
        //    }            
        //}

        protected override HttpStatusCode SendMailMessage(MailMessage mailMessage)
        {
            //throw new System.NotImplementedException("This method should be implemented in order to send mail messages");
            using (var smtpClient = new SmtpClient("smtp01.mycompany.com", 25))
            {
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.EnableSsl = false;

                smtpClient.Send(mailMessage);
            }
            return HttpStatusCode.OK;
        }

    }
}