using BEntities.Filters;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using Portal.Controllers;
using Portal.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Telerik.Reporting.Processing;
using BCA = BComponents.SAP;
using BCB = BComponents.Base;
using BCP = BComponents.Product;
using BCS = BComponents.Security;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BEE = BEntities.Enums;
using BEP = BEntities.Product;
using BES = BEntities.Security;

namespace Portal.Areas.Product.Controllers
{
    [Area("Product")]
    [Authorize]
    public class DocumentController : BaseController
    {

        #region Global Variables

        private readonly IConfiguration config;

        #endregion

        #region Constructors

        public DocumentController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
        {
            config = configuration;
        }

        #endregion

        #region GETs

        // GET: Product/MAUpdates
        public IActionResult Index()
        {
            if (IsAllowed(this))
            {
                ViewData["DeleteDocument"] = GetPermission("DeleteDocument") > 0 ? "Y" : "N";
                if (CardCode == HomeCardCode)
                {
                    return View();
                }
                else
                {
                    return View("IndexClient");
                }
            }
            else
            {
                return RedirectToAction("Denied", "Home", new { area = "" });
            }
        }

        public IActionResult GetAvailableProducts()
        {
            BCP.Document bcProDoc = new();
            IEnumerable<BEP.Product> products = bcProDoc.ListProducts("1");
            var items = products.Select(x => new { x.Id, x.ItemCode, x.Name, x.IdLine, x.LineName });
            return Json(items);
        }

        public IActionResult GetTypes()
        {
            BCB.Classifier bcClassifier = new();
            List<BEB.Classifier> lstTypes = bcClassifier.List((long)BEE.Classifiers.Document, "1");
            var lstItems = from i in lstTypes orderby i.Name select new { i.Id, i.Name };
            return Json(lstItems);
        }

        public IActionResult GetSAPLines()
        {
            BCP.Line bcLine = new();
            IEnumerable<BEP.Line> lstLines = bcLine.List("Name");
            var lstResult = lstLines.Select(l => new { l.Id, l.Name });
            return Json(lstResult);
        }

        public IActionResult GetProducts(long LineId, string Filter)
        {
            BCP.Product bcProduct = new();
            List<Field> lstFilter = new() { new Field("Line", $"SELECT SAPLine FROM Product.LineDetail WHERE IdLine = {LineId}", Operators.In) };
            if (!string.IsNullOrEmpty(Filter))
            {
                lstFilter.AddRange(new[]
                {
                    new Field("LOWER(ItemCode)", Filter.ToLower(), Operators.Likes),
                    new Field("LOWER(Name)", Filter.ToLower(), Operators.Likes),
                    new Field(LogicalOperators.Or),
                    new Field(LogicalOperators.And)
                });
            }

            IEnumerable<BEP.Product> lstItems = bcProduct.List(lstFilter, "ItemCode", BEP.relProduct.Prices);
            var items = lstItems.Select(x => new { x.Id, Name = $"{x.ItemCode} - {x.Name}" });
            return Json(items);
        }

        public IActionResult Filter(long IdProduct)
        {
            string message = "";
            try
            {
                var documents = GetItems(IdProduct);
                return Json(new { message, documents });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetAllReceivers()
        {
            string message = "";
            try
            {
                BCP.DocumentReceiver bcReceivers = new();
                List<Field> filters = new();
                IEnumerable<BEP.DocumentReceiver> receivers = bcReceivers.List(filters, "CardCode, Name");
                string codes = string.Join(",", receivers.Select(x => $"'{x.CardCode.ToLower()}'").Distinct());
                BCA.Client bcClient = new();
                filters.Add(new Field("LOWER(CardCode)", codes, Operators.In));
                var clients = bcClient.ListShort(filters, "1");
                var items = from r in receivers
                            join c in clients on r.CardCode.ToLower() equals c.CardCode.ToLower()
                            select new { r.Id, r.CardCode, c.CardName, r.Name, r.EMail, r.Enabled };
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetDocuments(long IdProduct)
        {
            string message = "";
            try
            {
                BCP.Document bcProDoc = new();
                List<Field> filters = new() { new Field("IdProduct", IdProduct), new Field("Enabled", 1), new Field("FilesCount", 0, Operators.HigherThan), new Field(LogicalOperators.And), new Field(LogicalOperators.And) };
                IEnumerable<BEP.Document> documents = bcProDoc.ListExtended(filters, "ReleaseDate DESC", BEP.relDocument.Type, BEP.relDocument.DocumentFiles);
                var items = documents.Select(x => new { x.Id, x.Name, x.ReleaseDate, Description = SetHTMLSafe(x.Description), x.ProductName, x.TypeName, Files = x.ListDocumentFiles.Select(y => new { y.Id, y.FileURL }) });
                //var items = from d in documents
                //            group d by d.TypeName into g
                //            select new { TypeName = g.Key, items = g.Select(x => new { x.Id, x.Name, x.ReleaseDate, x.FileURL, Description = SetHTMLSafe(x.Description), x.ProductName }) };
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetConfigMail(long Id)
        {
            string message = "";
            try
            {
                List<Field> filters = new();
                BCP.DocumentReceiver bcReceiver = new();
                IEnumerable<BEP.DocumentReceiver> lstReceivers = bcReceiver.ListNotBlackListed(filters, "1");
                //var receivers = lstReceivers.Select(x => new Models.Item { Id = x.Id, Name = $"{x.Name} ( {x.EMail} )", Selected = false });
                string codes = string.Join(",", lstReceivers.Select(x => $"'{x.CardCode.ToLower()}'").Distinct());
                BCA.Client bcClient = new();
                filters.Add(new Field("LOWER(CardCode)", codes, Operators.In));
                var clients = bcClient.ListShort(filters, "1");
                var receivers = from r in lstReceivers
                                join c in clients on r.CardCode.ToLower() equals c.CardCode.ToLower()
                                select new { r.Id, r.CardCode, c.CardName, Name = $"{r.Name} ( {r.EMail} )", r.EMail, r.Enabled, Selected = false };

                BCP.Document bcDoc = new();
                filters = new();
                IEnumerable<BEP.Document> lstDocuments = bcDoc.ListExtended(filters, "1");
                var documents = lstDocuments.Select(x => new Models.Item { Id = x.Id, Name = $"{x.TypeName} - {x.Name}", Selected = x.Id == Id });

                return Json(new { message, receivers, documents });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult SendDocument(string Subject, string DocumentIds, string ClientsIds, bool WithSignature)
        {
            string message = "", type = "";
            try
            {
                BCP.Document bcProDoc = new();
                BCP.DocumentReceiver bcReceiver = new();
                List<Field> filters = new() { new Field("Id", ClientsIds, Operators.In) };
                IEnumerable<BEP.DocumentReceiver> lstReceivers = bcReceiver.List(filters, "Name");
                if (lstReceivers?.Count() > 0)
                {
                    filters = new() { new Field("Id", DocumentIds, Operators.In) };
                    IEnumerable<BEP.Document> docs = bcProDoc.List(filters, "1", BEP.relDocument.DocumentFiles);
                    if (docs?.Count() > 0)
                    {
                        List<MailAddress> lstTo = lstReceivers.Select(x => new MailAddress(x.EMail, x.Name)).ToList();
                        string body = MailBody(docs, WithSignature);
                        var settings = config.GetSection("MailSettings").Get<MailSettings>();
                        var actionTask = settings.Tasks?.FirstOrDefault(i => i.Area == "Product" & i.Controller == "Document" & i.Name == "SendDocument");
                        MailAddress sender = actionTask != null ? new(actionTask.Sender.EMail, actionTask.Sender.Name) : new("Soporte.Portal@dmc.bo", "Soporte");
                        _ = base.SendMailAsync(Subject ?? "Actualización de Documentos", body, null, null, lstTo, sender);
                    }
                    else
                    {
                        type = "error";
                        message = "Al parecer dicha Actualización ha sido eliminada";
                    }
                }
                else
                {
                    message = "No existen clientes registrados para recibir el correo.";
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex);
                type = "error";
            }
            return Json(new { message, type });
        }

        public IActionResult RemoveFile(string[] fileNames)
        {
            // The parameter of the Remove action must be called "fileNames"
            foreach (var strFileName in fileNames)
            {
                string strPhysicalPath = Path.Combine(rootDirectory, "wwwroot", "files", "product-docs", strFileName);
                if (System.IO.File.Exists(strPhysicalPath))
                    System.IO.File.Delete(strPhysicalPath);
            }
            return Content("");
        }

        public async Task<IActionResult> SaveFile(IEnumerable<IFormFile> files)
        {
            // The Name of the Upload component is "files"
            try
            {
                if (files != null)
                {
                    foreach (var file in files)
                    {
                        var fileContent = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
                        var fileName = Path.GetFileName(fileContent.FileName.ToString().Trim('"'));
                        var physicalPath = Path.Combine(rootDirectory, "wwwroot", "files", "product-docs", fileName);
                        using var fileStream = new FileStream(physicalPath, FileMode.Create);
                        await file.CopyToAsync(fileStream);
                    }
                }
            }
            catch (Exception) { }
            // Return an empty string to signify success
            return Content("");
        }

        public IActionResult GetMailConfig()
        {
            string message = "";
            try
            {
                BCP.DocumentConfig bcConfig = new();
                BEP.DocumentConfig item = bcConfig.Search(1) ?? new BEP.DocumentConfig { Id = 0, Header = "", Footer = "" };
                return Json(new { message, item });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [AllowAnonymous]
        public IActionResult DownloadFile(string FileName)
        {
            try
            {
                string fullName = Path.Combine(rootDirectory, "wwwroot", "files", "product-docs", FileName);
                byte[] file = System.IO.File.ReadAllBytes(fullName);
                string fileExt = FileName.Contains(".") ? FileName.Split(".").Last() : "";
                string contentType = fileExt switch
                {
                    "htm" or "html" => "text/HTML",
                    "txt" => "text/plain",
                    "doc" or "rtf" or "docx" => "Application/msword",
                    "xls" or "xlsx" => "Application/x-msexcel",
                    "jpg" or "jpeg" => "image/jpeg",
                    "gif" => "image/GIF",
                    "pdf" => "application/pdf",
                    "msg" => "application/vnd.ms-outlook",
                    _ => "application/octet-stream",
                };
                return File(file, contentType, FileName);
            }
            catch (FileNotFoundException)
            {
                string fullName = Path.Combine(rootDirectory, "wwwroot", "images", "DMC-404.jpg");
                byte[] file = System.IO.File.ReadAllBytes(fullName);
                string contentType = "image/jpeg";
                return File(file, contentType, "DMC-404.jpg");
            }
        }

        [AllowAnonymous]
        public IActionResult DownloadFiles(long Id)
        {
            BCP.Document bcDocument = new();
            BEP.Document doc = bcDocument.Search(Id, BEP.relDocument.DocumentFiles);

            var zipFileName = $"{doc.Name.Replace(" ", "-")}.zip";
            var tempOutput = Path.Combine(rootDirectory, "wwwroot", "files", "product-docs", zipFileName);

            using (ZipOutputStream IzipOutputStream = new(System.IO.File.Create(tempOutput)))
            {
                IzipOutputStream.SetLevel(9);
                byte[] buffer = new byte[4096];
                string filePath;
                foreach (var item in doc.ListDocumentFiles)
                {
                    filePath = Path.Combine(rootDirectory, "wwwroot", "files", "product-docs", item.FileURL);
                    ZipEntry entry = new(Path.GetFileName(filePath)) { DateTime = DateTime.Now, IsUnicodeText = true };
                    IzipOutputStream.PutNextEntry(entry);

                    using FileStream oFileStream = System.IO.File.OpenRead(filePath);
                    int sourceBytes;
                    do
                    {
                        sourceBytes = oFileStream.Read(buffer, 0, buffer.Length);
                        IzipOutputStream.Write(buffer, 0, sourceBytes);
                    } while (sourceBytes > 0);
                }

                IzipOutputStream.Finish();
                IzipOutputStream.Flush();
                IzipOutputStream.Close();
            }

            byte[] finalResult = System.IO.File.ReadAllBytes(tempOutput);
            //if (System.IO.File.Exists(tempOutput))
            //{
            //    System.IO.File.Delete(tempOutput);
            //}
            if (finalResult == null || !finalResult.Any())
            {
                throw new Exception(String.Format("Nothing found"));

            }

            return File(finalResult, "application/zip", zipFileName);
        }

        #endregion

        #region POSTs

        [HttpPost]
        public IActionResult Edit(BEP.Document Item)
        {
            string message = "";
            try
            {
                BCP.Document bcProDoc = new();
                Item.StatusType = Item.Id == 0 ? BEntities.StatusType.Insert : BEntities.StatusType.Update;
                Item.LogUser = UserCode;
                Item.LogDate = DateTime.Now;
                if (Item.ListDocumentFiles?.Any() ?? false)
                {
                    foreach (var item in Item.ListDocumentFiles)
                    {
                        item.LogUser = UserCode;
                        item.LogDate = DateTime.Now;
                    }
                }
                bcProDoc.Save(ref Item);

                var items = GetItems(Item.IdProduct);
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost]
        public IActionResult Delete(long Id)
        {
            string message = "";
            try
            {
                BCP.Document bcProDoc = new();
                BEP.Document beItem = new() { StatusType = BEntities.StatusType.Delete, Id = Id, ReleaseDate = DateTime.Now, LogDate = DateTime.Now };
                bcProDoc.Save(ref beItem);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost]
        public IActionResult EditReceiver(BEP.DocumentReceiver Item)
        {
            string message = "";
            try
            {
                BCP.DocumentReceiver bcProDoc = new();
                Item.StatusType = Item.Id == 0 ? BEntities.StatusType.Insert : BEntities.StatusType.Update;
                Item.LogUser = UserCode;
                Item.LogDate = DateTime.Now;
                bcProDoc.Save(ref Item);
                //var (documents, receivers) = GetItems(Item.IdProduct);
                return Json(new { message });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost]
        public IActionResult DeleteReceiver(long Id)
        {
            string message = "";
            IEnumerable<BEP.DocumentReceiver> items = null;
            try
            {
                BCP.DocumentReceiver bcProDoc = new();
                BEP.DocumentReceiver beItem = new() { StatusType = BEntities.StatusType.Delete, Id = Id, LogDate = DateTime.Now };
                bcProDoc.Save(ref beItem);
                items = bcProDoc.List("1");
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message, items });
        }

        [HttpPost]
        public IActionResult SaveMailConfig(BEP.DocumentConfig Item)
        {
            string message = "";
            try
            {
                BCP.DocumentConfig bcConfig = new();
                Item.StatusType = Item.Id > 0 ? BEntities.StatusType.Update : BEntities.StatusType.Insert;
                Item.LogUser = UserCode;
                Item.LogDate = DateTime.Now;
                bcConfig.Save(ref Item);
                return Json(new { message });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        #endregion

        #region Private Methods

        private IEnumerable<BEP.Document> GetItems(long IdProduct)
        {
            BCP.Document bcProDoc = new();
            List<Field> filters = new() { new Field("IdProduct", IdProduct) };
            IEnumerable<BEP.Document> documents = bcProDoc.ListExtended(filters, "ReleaseDate DESC", BEP.relDocument.DocumentFiles);

            return documents;
        }

        private string MailBody(IEnumerable<BEP.Document> Items, bool WithSignature)
        {
            BCP.DocumentConfig bcConfig = new();
            var mailConfig = bcConfig.Search(1);

            StringBuilder sb = new();
            sb.AppendLine(@"	<style> ");
            sb.AppendLine(@"		body { background-color: #FFF; font-family: Verdana, Geneva, sans-serif; font-size: 12px; } ");
            sb.AppendLine(@"		img { margin: 20px 15px; }");
            sb.AppendLine(@"		td { padding: 0 8px; line-height: 18px; }");
            sb.AppendLine(@"	</style>");
            sb.AppendLine(@"	<div style=""background-color: #DDD; margin: 10px; padding: 15px; border-radius: 15px;"" >");
            sb.AppendLine(@"		<table style=""background-color: #DDD; margin: 10px; width: 97%; font-size: 12px; border-collapse: collapse;"">");
            sb.AppendLine(@"			<tr>");
            sb.AppendLine(@"                <td style=""width: 20px;"">&nbsp;</td>");
            sb.AppendLine(@"				<td>");
            sb.AppendLine($@"					<br /><img src=""http://www.dmc.bo/img/logo3.png"" class=""logo"" height=""70"" />");
            sb.AppendLine(@"				</td>");
            sb.AppendLine(@"                <td style=""width: 20px;"">&nbsp;</td>");
            sb.AppendLine(@"			</tr>");
            sb.AppendLine(@"			<tr>");
            sb.AppendLine(@"                <td style=""width: 20px;"">&nbsp;</td>");
            sb.AppendLine(@"				<td>");
            if (!string.IsNullOrEmpty(mailConfig?.Header))
            {
                sb.AppendLine($"<p>{SetHTMLSafe(mailConfig.Header.Replace("/images/", "http://portal.dmc.bo/images/"))}</p>");
            }
            foreach (var item in Items)
            {
                sb.AppendLine($@"                   <p>{item.Name}<br />");
                sb.AppendLine($@"                   {SetHTMLSafe(item.Description).Replace(@"src=""/", @"src=""http://portal.dmc.bo/")}</p>");
                if (item.ListDocumentFiles?.Count > 0)
                {
                    foreach (var file in item.ListDocumentFiles)
                    {
                        //sb.AppendLine($@"                   <p><a href=""http://portal.dmc.bo/Product/Document/DownloadFile?FileName={file.FileURL}"">{file.FileURL}</a></p>");
                        sb.AppendLine($@"                   <p><a href=""{Url.ActionLink("DownloadFile", "Document", new { FileName = file.FileURL })}"">{file.FileURL}</a></p>");
                    }
                }
                if (Items.Last().Id != item.Id) sb.AppendLine("<hr>");
            }
            if (!string.IsNullOrEmpty(mailConfig?.Footer))
            {
                sb.AppendLine($"<p>{SetHTMLSafe(mailConfig.Footer.Replace("/images/", "http://portal.dmc.bo/images/"))}</p>");
            }
            sb.AppendLine(@"				<br /></td>");
            sb.AppendLine(@"                <td style=""width: 20px;"">&nbsp;</td>");
            sb.AppendLine(@"			</tr>");
            sb.AppendLine(@"		</table>");
            sb.AppendLine(@"	</div>");
            if (WithSignature)
            {
                BCS.User bcUser = new();
                BES.User beUser = bcUser.Search(UserCode);
                BCS.UserData bcData = new();
                BES.UserData beData = bcData.SearchByUser(UserCode);
                if (!string.IsNullOrEmpty(beData?.Signature))
                {
                    List<string> toBeReplace = new() { "/Portal/Content/UserData/", "/Content/UserData/", "/Portal/images/userdata/", "/images/userdata/" };
                    string toReplaceWith = "http://portal.dmc.bo/images/userdata/", signature = beData.Signature;
                    toBeReplace.ForEach(x => signature = signature.Replace(x, toReplaceWith));
                    sb.AppendLine($"<div>{SetHTMLSafe(signature)}</div>");
                }
                else
                {
                    sb.AppendLine("<p>Atentamente</p>");
                    sb.AppendLine($"<p>{(!string.IsNullOrEmpty(beUser?.Name) ? beUser.Name : "El equipo de DMC")}</p>");
                }
            }

            return sb.ToString();
        }

        #endregion
    }
}
