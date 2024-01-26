using BEntities;
using BEntities.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Portal.Controllers;
using Portal.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BCA = BComponents.SAP;
using BCS = BComponents.Security;
using BCT = BComponents.PostSale;
using BEA = BEntities.SAP;
using BES = BEntities.Security;
using BET = BEntities.PostSale;

namespace Portal.Areas.Commercial.Controllers
{
    [Area("Commercial")]
    [Authorize]
    public class RMAController : BaseController
    {
        #region Variables Globales
        private readonly IConfiguration config;
        private readonly IWebHostEnvironment _env;
        #endregion

        #region Constructores

        public RMAController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
        {
            config = configuration;
            _env = env;
        }

        #endregion

        #region GETs

        public IActionResult Index()
        {
            if (IsAllowed(this))
            {
                ViewBag.CardCode = CardCode;
                ViewBag.UserCode = UserCode;
                int permission = GetPermission("RMA-Actividades");
                if (CardCode == HomeCardCode)
                {
                    //BCS.UserData bcUserData = new();
                    //var data = bcUserData.SearchByUser(UserCode);                    
                    ViewBag.Permission = permission;
                    return View();
                }
                else
                {
                    ViewData["Title"] = $"RMAs : {CardName}";
                    return View("IndexClient");
                }
            }
            else
            {
                return RedirectToAction("Denied");
            }
        }

        public IActionResult GetStatuses()
        {
            IEnumerable<BEA.Item> states;
            try
            {
                BCA.RMA bcRMA = new();
                states = bcRMA.ListStatuses(null, "1");
            }
            catch (Exception)
            {
                states = new List<BEA.Item>();
            }
            var items = states.Select(x => new { x.Id, x.Name });
            return Json(items);
        }

        public IActionResult GetTechnicians()
        {
            IEnumerable<BEA.Item> technicians;
            try
            {
                BCA.RMA bcRMA = new();
                technicians = bcRMA.ListTechnicians(null, "Name");
            }
            catch (Exception)
            {
                technicians = new List<BEA.Item>();
            }
            var items = technicians.Select(x => new { x.Id, x.Name });
            return Json(items);
        }

        public IActionResult GetSubjects()
        {
            IEnumerable<BEA.Item> subjects;
            try
            {
                BCA.RMA bcRMA = new();
                subjects = bcRMA.ListSubjects();
            }
            catch (Exception)
            {
                subjects = new List<BEA.Item>();
            }
            var items = subjects.Select(x => new { x.Id, x.Name });
            return Json(items);
        }

        public IActionResult GetBrands()
        {
            IEnumerable<BEA.Item> brands;
            try
            {
                BCA.RMA bcRMA = new();
                brands = bcRMA.ListBrands();
            }
            catch (Exception)
            {
                brands = new List<BEA.Item>();
            }
            var items = brands.Select(x => new { x.Name });
            return Json(items);
        }

        public IActionResult GetCountries()
        {
            IEnumerable<BEA.Item> countries;
            try
            {
                BCA.RMA bcRMA = new();
                countries = bcRMA.ListCountries();
            }
            catch (Exception)
            {
                countries = new List<BEA.Item>();
            }
            var items = countries.Select(x => new { x.Code, x.Name });
            return Json(items);
        }

        public IActionResult GetStates(string Country)
        {
            IEnumerable<BEA.Item> states;
            try
            {
                BCA.RMA bcRMA = new();
                states = bcRMA.ListStates(Country);
            }
            catch (Exception)
            {
                states = new List<BEA.Item>();
            }
            var items = states.Select(x => new { x.Code, x.Name });
            return Json(items);
        }

        public IActionResult GetCities()
        {
            IEnumerable<BEA.Item> cities;
            try
            {
                BCA.RMA bcRMA = new();
                cities = bcRMA.ListCities();
            }
            catch (Exception)
            {
                cities = new List<BEA.Item>();
            }
            var items = cities.Select(x => new { Id = x.Id.ToString(), x.Name });
            return Json(items);
        }

        public IActionResult GetLocations()
        {
            IEnumerable<BEA.Item> locations;
            try
            {
                BCA.RMA bcRMA = new();
                locations = bcRMA.ListLocations();
            }
            catch (Exception)
            {
                locations = new List<BEA.Item>();
            }
            var items = locations.Select(x => new { x.Id, x.Name });
            return Json(items);
        }

        public IActionResult GetCallTypes()
        {
            IEnumerable<BEA.Item> types;
            try
            {
                BCA.RMA bcRMA = new();
                types = bcRMA.ListCallTypes();
            }
            catch (Exception)
            {
                types = new List<BEA.Item>();
            }
            var items = types.Select(x => new { x.Id, x.Name });
            return Json(items);
        }

        public IActionResult GetOrigins()
        {
            IEnumerable<BEA.Item> origins;
            try
            {
                BCA.RMA bcRMA = new();
                origins = bcRMA.ListOrigins();
            }
            catch (Exception)
            {
                origins = new List<BEA.Item>();
            }
            var items = origins.Select(x => new { x.Id, x.Name });
            return Json(items);
        }

        public IActionResult GetProblemTypes()
        {
            IEnumerable<BEA.Item> types;
            try
            {
                BCA.RMA bcRMA = new();
                types = bcRMA.ListProblemTypes();
            }
            catch (Exception)
            {
                types = new List<BEA.Item>();
            }
            var items = types.Select(x => new { x.Id, x.Name });
            return Json(items);
        }

        public IActionResult GetSolutionStatuses()
        {
            IEnumerable<BEA.Item> types;
            try
            {
                BCA.RMA bcRMA = new();
                types = bcRMA.ListSolutionStatuses();
            }
            catch (Exception)
            {
                types = new List<BEA.Item>();
            }
            var items = types.Select(x => new { x.Id, x.Name });
            return Json(items);
        }

        public IActionResult Filter(long? Id, DateTime? InitialDate, DateTime? FinalDate, string ClientCode, string Technician, string StateCodes, string Brands, string Lines, string ProductManager, string Product, string Serial)
        {
            string message = "";
            try
            {
                List<BET.ServiceCall> items = GetItems(Id, InitialDate, FinalDate, ClientCode, Technician, StateCodes, Brands, Lines, ProductManager, Product, Serial);
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult Detail(string Id)
        {
            string message = "";
            try
            {
                List<BEA.RMAActivity> activities = new();
                List<BEA.RMAHistory> history = new();
                List<BEA.RMARepair> repairs = new();
                List<BEA.RMACost> costs = new();
                int code;

                if (Id != "0")
                {
                    BCT.ServiceCallActivity bcActivity = new();
                    BCT.ServiceCallSolution bcSolution = new();
                    if (Id.StartsWith("TEMP-"))
                    {
                        code = int.Parse(Id.Replace("TEMP-", ""));
                        List<Field> filters = new() { new Field("IdServiceCall", code) };
                        var actItems = bcActivity.List(filters, "1");
                        if (actItems?.Count() > 0)
                        {
                            foreach (var i in actItems)
                            {
                                activities.Add(i.ToSAPEntity());
                            }
                        }

                        var solItems = bcSolution.List(filters, "1");
                        if (solItems?.Count() > 0)
                        {
                            foreach (var i in solItems)
                            {
                                repairs.Add(i.ToSAPEntity());
                            }
                        }
                    }
                    else
                    {
                        code = int.Parse(Id);
                        BCA.RMA bcRMA = new();
                        activities = bcRMA.ListActivities(code, "ActivityDate")?.ToList() ?? new List<BEA.RMAActivity>();
                        history = bcRMA.ListHistory(code, "1")?.ToList() ?? new List<BEA.RMAHistory>();
                        repairs = bcRMA.ListRepairs(code)?.ToList() ?? new List<BEA.RMARepair>();
                        costs = bcRMA.ListCosts(code)?.ToList() ?? new List<BEA.RMACost>();

                        activities.ForEach(x =>
                        {
                            x.Code = x.Id.ToString();
                            x.Id = 0;
                        });
                        List<BET.ServiceCallActivity> localActivities = bcActivity.ListBySAPCode(code, "1");
                        if (localActivities?.Count > 0)
                        {
                            foreach (var i in localActivities)
                            {
                                if (i.SAPCode.HasValue) activities.RemoveAll(x => x.Code == i.SAPCode.Value.ToString());
                                activities.Add(i.ToSAPEntity());
                            }
                        }

                        repairs.ForEach(x =>
                        {
                            x.Code = x.Id.ToString();
                            x.Id = 0;
                        });
                        List<BET.ServiceCallSolution> localSolutions = bcSolution.ListBySAPCode(code, "1");
                        if (localSolutions?.Count > 0)
                        {
                            foreach (var i in localSolutions)
                            {
                                if (i.SAPCode.HasValue) repairs.RemoveAll(x => x.Code == i.SAPCode.Value.ToString());
                                repairs.Add(i.ToSAPEntity());
                            }
                        }
                    }
                }
                return Json(new { message, activities, history, repairs, costs });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult DownloadFile(string FilePath)
        {
            try
            {
                string fileName, ext, contentType, separator;
                byte[] file;
                if (!string.IsNullOrEmpty(FilePath))
                {
                    separator = FilePath.Contains("/") ? "/" : "\\";
                    fileName = FilePath.Split(separator).Last();
                    if (separator == "\\")
                    {
                        file = System.IO.File.ReadAllBytes(FilePath);
                    }
                    else
                    {
                        string fullPath = rootDirectory + "\\wwwroot\\" + FilePath; //Path.Combine(rootDirectory, "wwwroot", FilePath);
                        file = System.IO.File.ReadAllBytes(fullPath);
                    }
                    ext = fileName.Split('.').Last();
                    contentType = ext switch
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
                }
                else
                {
                    fileName = Path.Combine(rootDirectory, "wwwroot", "images", "DMC-404.jpg");
                    file = System.IO.File.ReadAllBytes(fileName);
                    contentType = "image/jpeg";
                    fileName = "DMC-404.jpg";
                }
                return File(file, contentType, fileName);
            }
            catch (FileNotFoundException)
            {
                string fullName = Path.Combine(rootDirectory, "wwwroot", "images", "DMC-404.jpg");
                byte[] file = System.IO.File.ReadAllBytes(fullName);
                string contentType = "image/jpeg";
                return File(file, contentType, "DMC-404.jpg");
            }
        }

        public IActionResult GetHistory(int Id)
        {
            string message = "";
            try
            {
                BCA.RMA bcRMA = new();
                IEnumerable<BEA.RMAHistory> items = bcRMA.ListHistory(Id, "1");
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetRMAFromSerial(int Id, string Serial)
        {
            string message = "";
            try
            {
                BCA.RMA bcRMA = new();
                List<Field> filters = new() { new Field("LOWER(SerialNumber)", Serial.ToLower()), new Field("Code", Id, Operators.Different), new Field(LogicalOperators.And) };
                var items = bcRMA.List(filters, "1");
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetUsers()
        {
            BCA.RMA bcRMA = new();
            var users = bcRMA.ListUsers();
            var items = users.Select(x => new { x.Id, Name = $"{x.Name} ( {x.Code} )" });
            return Json(items);
        }

        public IActionResult GetContacts(string CardCode)
        {
            BCA.RMA bcRMA = new();
            IEnumerable<BEA.SAPContacts> contacts = Enumerable.Empty<BEA.SAPContacts>();
            if (!string.IsNullOrEmpty(CardCode))
            {
                contacts = bcRMA.ListContacts(CardCode);
            }
            var items = contacts.Select(x => new { x.Id, x.Name });
            return Json(items);
        }

        public IActionResult GetProductCard(string SerialNumber)
        {
            string message = "";
            try
            {
                BCA.RMA bcRMA = new();
                BEA.ProductCard item = bcRMA.SearchProductCard(SerialNumber);
                bool exists = item != null;
                if (item != null)
                {
                    List<Field> filters = new() { new Field("LOWER(SerialNumber)", SerialNumber.ToLower()) };
                    var items = bcRMA.List(filters, "Code DESC");
                    var first = items?.FirstOrDefault();
                    item.LastCountedPieces = first?.CountedPieces ?? 0;
                    item.CityCode = first?.CityCode ?? "";
                    item.ReportedBy = first?.ReportedBy ?? "";
                    item.Phone = first?.FinalUserPhone ?? "";
                    item.EMail = first?.FinalUserEMail ?? "";
                    item.ExternalService = first?.ExternalService ?? "";
                    item.ExServTechnician = first?.ExternalServiceTechnician ?? "";
                    item.Address = first?.ExternalServiceAddress ?? "";
                }
                return Json(new { message, exists, item });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        #endregion

        #region POSTs

        [HttpPost]
        public IActionResult Save(BET.ServiceCall Item)
        {
            string message = "";
            try
            {
                DateTime logDate = DateTime.Now;
                BCT.ServiceCall bcRMA = new();
                Item.StatusType = Item.PortalId == 0 ? StatusType.Insert : StatusType.Update;
                Item.Id = Item.PortalId;
                if (Item.StatusCode == -1) Item.CloseDate = DateTime.Now;
                Item.LogUser = UserCode;
                Item.LogDate = logDate;
                if (Item.ListServiceCallActivitys?.Count > 0)
                {
                    foreach (var item in Item.ListServiceCallActivitys)
                    {
                        if (!string.IsNullOrEmpty(item.Code) && !item.Code.StartsWith("TEMP") && int.Parse(item.Code) > 0) item.SAPCode = int.Parse(item.Code);
                        item.Attachment ??= "";
                        item.LogUser = UserCode;
                        item.LogDate = logDate;
                    }
                }
                if (Item.ListServiceCallSolutions?.Count > 0)
                {
                    foreach (var item in Item.ListServiceCallSolutions)
                    {
                        item.Attachment ??= "";
                        item.LogUser = UserCode;
                        item.LogDate = logDate;
                    }
                }
                bcRMA.Save(ref Item);
                return Json(new { message, Item.Id });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost]
        public IActionResult Delete(string Code)
        {
            string message = "";
            try
            {
                long Id = Code.StartsWith("TEMP") ? long.Parse(Code.Replace("TEMP-", "")) : long.Parse(Code);
                DateTime logDate = DateTime.Now;
                BCT.ServiceCall bcRMA = new();
                BET.ServiceCall item = new() { Id = Id, StatusType = StatusType.Delete, CreateDate = logDate, LogDate = logDate, StartDate = logDate };
                bcRMA.Save(ref item);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost]
        public IActionResult SaveFileBase64(string FileBase64, string FileName)
        {
            string message = "";
            try
            {
                BCS.User bcUser = new();
                BES.User beUser = bcUser.Search(UserCode);
                string strFile = FileBase64.Split(',')[1];
                string virtualPath = Path.Combine("files", "rma", FileName);
                var fullPath = _env.WebRootFileProvider.GetFileInfo(virtualPath).PhysicalPath;
                byte[] bytes = Convert.FromBase64String(strFile);
                System.IO.File.WriteAllBytes(fullPath, bytes);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        #endregion

        #region Métodos Privados

        private List<BET.ServiceCall> GetItems(long? Id, DateTime? InitialDate, DateTime? FinalDate, string ClientCode, string Technician, string StateCodes, string Brands, string Lines, string ProductManager, string Product, string Serial)
        {
            List<BET.ServiceCall> items = new();

            string dateFormat = "yyyy-MM-dd";
            BCA.RMA bcRMA = new();
            BCT.ServiceCall bcTemp = new();
            List<Field> filters = new();
            if (Id.HasValue && Id.Value > 0)
            {
                filters.Add(new Field("Code", Id.Value));
            }
            else
            {
                if (InitialDate.HasValue) filters.AddRange(new[] { new Field("CreateDate", InitialDate.Value.ToString(dateFormat), Operators.HigherOrEqualThan), new Field("CloseDate", InitialDate.Value.ToString(dateFormat), Operators.HigherOrEqualThan), new Field(LogicalOperators.Or) });
                if (FinalDate.HasValue) filters.AddRange(new[] { new Field("CreateDate", FinalDate.Value.ToString(dateFormat), Operators.LowerOrEqualThan), new Field("CloseDate", FinalDate.Value.ToString(dateFormat), Operators.LowerOrEqualThan), new Field(LogicalOperators.Or) });
                if (!string.IsNullOrEmpty(ClientCode)) filters.Add(new Field("LOWER(ClientCode)", ClientCode.ToLower()));
                if (!string.IsNullOrEmpty(Technician)) filters.Add(new Field("TechnicianCode", Technician));
                if (!string.IsNullOrEmpty(StateCodes)) filters.Add(new Field("StatusCode", StateCodes == "0" ? "-1" : StateCodes, StateCodes == "0" ? Operators.Different : Operators.In));
                if (!string.IsNullOrEmpty(Brands)) filters.Add(new Field("LOWER(Brand)", Brands.ToLower(), Operators.In));
                if (!string.IsNullOrEmpty(Lines)) filters.Add(new Field("LOWER(Line)", Lines.ToLower(), Operators.In));
                if (!string.IsNullOrEmpty(ProductManager)) filters.Add(new Field("LOWER(ProductManager)", ProductManager.ToLower()));
                if (!string.IsNullOrEmpty(Serial)) filters.Add(new Field("LOWER(SerialNumber)", Serial.ToLower(), Operators.Likes));
                if (!string.IsNullOrEmpty(Product)) filters.AddRange(new[] { new Field("LOWER(ItemCode)", Product.ToLower(), Operators.Likes), new Field("LOWER(ItemName)", Product.ToLower(), Operators.Likes), new Field(LogicalOperators.Or) });
                CompleteFilters(ref filters);
            }

            items = bcRMA.List(filters, "1")?.ToList() ?? new List<BET.ServiceCall>();
            items.ForEach(x => x.SAPCode = int.Parse(x.Code));
            IEnumerable<BET.ServiceCall> itemsLocal = bcTemp.List("1", BET.relServiceCall.ServiceCallFiles);
            if (itemsLocal?.Count() > 0)
            {
                var states = bcRMA.ListStatuses(null, "1");
                foreach (var item in itemsLocal)
                {
                    item.Code = item.SAPCode?.ToString() ?? $"TEMP-{item.Id}";
                    item.Status = states.First(x => x.Id == item.StatusCode).Name;
                    item.PortalId = item.Id;
                    item.OpenDays = ((item.CloseDate ?? DateTime.Now) - item.CreateDate).Days;

                    items.RemoveAll(x => x.Code == item.SAPCode?.ToString());
                    items.Add(item);
                }
            }
            return items;
        }

        #endregion
    }
}
