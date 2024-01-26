//using System.Collections.Generic;
//using System.IO;
//using Telerik.Reporting;
//using Telerik.Reporting.Services;

//namespace Portal.Misc
//{
//    public class CustomReportResolver : Telerik.Reporting.Services.IReportSourceResolver
//    {
//        private readonly string _path;
//        private readonly string _serviceName;
//        private const string _urlToReplace = "http://servicios.dmc.bo/ReportService";
//        public CustomReportResolver(string path, string serviceName)
//        {
//            _path = path;
//            _serviceName = serviceName;
//        }

//        public ReportSource Resolve(string reportName, OperationOrigin operationOrigin, IDictionary<string, object> currentParameterValues)
//        {
//            var reportPacker = new ReportPackager();

//            string reportPath = Path.Combine(_path, reportName);
//            using var sourceStream = System.IO.File.OpenRead(reportPath);
//            var report = (Telerik.Reporting.Report)reportPacker.UnpackageDocument(sourceStream);

//            var webServiceDataSource = (WebServiceDataSource)report.DataSource;
//            string url = webServiceDataSource.ServiceUrl;
//            url = url.Replace(_urlToReplace, _serviceName);
//            webServiceDataSource.ServiceUrl = url;

//            DetailSection detail = (DetailSection)report.Items["detailSection1"];
//            foreach (var item in detail.Items)
//            {
//                if (item is Table table)
//                {
//                    webServiceDataSource = (WebServiceDataSource)table.DataSource;
//                    url = webServiceDataSource.ServiceUrl;
//                    url = url.Replace(_urlToReplace, _serviceName);
//                    webServiceDataSource.ServiceUrl = url;
//                }
//            }

//            var irs = new InstanceReportSource { ReportDocument = report };

//            return irs;
//        }
//    }
//}
