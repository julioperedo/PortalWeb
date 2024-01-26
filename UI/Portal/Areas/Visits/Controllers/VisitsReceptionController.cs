using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BEntities.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Portal.Controllers;
using BCA = BComponents.SAP;
using BCB = BComponents.Base;
using BCD = BComponents.AppData;
using BCV = BComponents.Visits;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BED = BEntities.AppData;
using BEV = BEntities.Visits;

namespace Portal.Areas.Visits.Controllers
{
    [Area("Visits")]
    [Authorize]
    public class VisitsReceptionController : BaseController
    {
        #region Variables Globales
        private readonly IConfiguration config;
        #endregion

        #region Constructores

        public VisitsReceptionController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { config = configuration; }

        #endregion

        #region GETs

        // GET: Visits/VisitsReception
        public IActionResult Index()
        {
            if (IsAllowed("Visits", "VisitsReception", "Index"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Denied");
            }
        }

        public IActionResult Edit(long Id)
        {
            BEV.VisitReception item;
            Models.Visit visit;
            string form = "Edit";
            if (Id == 0)
            {
                visit = new Models.Visit();
            }
            else
            {
                BCV.VisitReception bcVisit = new();
                item = bcVisit.Search(Id, BEV.relVisitReception.Visitor, BEV.relVisitReception.Staff, BEV.relVisitReception.Reason) ?? new BEV.VisitReception();
                visit = new Models.Visit(item);

                BCA.Client bcClient = new();
                BEA.Client beClient = bcClient.Search(item.CardCode);
                visit.ClientName = $"({beClient.CardCode}) {beClient.CardName}";
                if (DateTime.ParseExact(item.InitialDate.ToString("yyyy-MM-dd"), "yyyy-M-d", null) < DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd"), "yyyy-M-d", null))
                {
                    form = "See";
                    if (item.IdReason != 55)
                    {
                        visit.ReasonVisit = item.Reason.Name;
                    }
                }
            }
            return PartialView(form, visit);
        }

        public IActionResult GetStaff()
        {
            var bcStaff = new BCD.StaffMembers();
            var localStaff = bcStaff.List("1");
            var lstItems = (from d in localStaff where d.Active orderby d.FullName select new { d.Id, Name = d.FullName }).ToList();
            return Json(lstItems);
        }

        public IActionResult GetVisitors()
        {
            BCV.Person bcPerson = new();
            IEnumerable<BEV.Person> lstPeople = bcPerson.List("1");
            var lstItems = (from i in lstPeople orderby i.FullName select new { i.Id, Name = $"{i.DocumentId} - {i.FullName}" }).ToList();
            return Json(lstItems);
        }

        public IActionResult GetReasons()
        {
            var bcClassifier = new BCB.Classifier();
            List<Field> filters = new() { new Field("IdType", (long)BEntities.Enums.Classifiers.VisitReasons) };
            var lstClassifiers = bcClassifier.List(filters, "1");
            var lstItems = (from c in lstClassifiers orderby c.Value select new { c.Id, c.Name }).ToList();
            return Json(lstItems);
        }

        public IActionResult Filter(string CardCode, string Visitor, long Staff, DateTime? Since, DateTime? Until, bool NotFinished)
        {
            string message = "";
            try
            {
                var items = GetItems(CardCode, Visitor, Staff, Since, Until, NotFinished);
                return Json(new { message, items });
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
        public IActionResult AddVisitor(string DocumentId, string FirstName, string LastName, string Phone)
        {
            string message = "";
            try
            {
                BCV.Person bcPerson = new BCV.Person();
                BEV.Person bePerson = new BEV.Person { StatusType = BEntities.StatusType.Insert, DocumentId = DocumentId, FirstName = FirstName, LastName = LastName, Phone = Phone, LogUser = UserCode, LogDate = DateTime.Now };
                bcPerson.Save(ref bePerson);
                return Json(new { message, id = bePerson.Id });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost]
        public IActionResult Edit(Models.Visit Item)
        {
            string message = "";
            try
            {
                BCV.VisitReception bcVisit = new BCV.VisitReception();
                BEV.VisitReception beVisit; // = Item.ToVisit();
                //beVisit.StatusType = beVisit.Id == 0 ? BEntities.StatusType.Insert : BEntities.StatusType.Update;
                if (Item.Id == 0)
                {
                    beVisit = Item.ToVisitReception();
                    beVisit.StatusType = BEntities.StatusType.Insert;
                    beVisit.InitialDate = DateTime.Now;
                }
                else
                {
                    beVisit = Item.ToVisitReception(bcVisit.Search(Item.Id));
                    beVisit.StatusType = BEntities.StatusType.Update;
                }
                beVisit.LogUser = UserCode;
                beVisit.LogDate = DateTime.Now;
                bcVisit.Save(ref beVisit);
                return Json(new { message, id = beVisit.Id });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost]
        public IActionResult FinishVisit(long Id)
        {
            string message = "";
            try
            {
                BCV.VisitReception bcVisit = new BCV.VisitReception();
                BEV.VisitReception beVisit = bcVisit.Search(Id);
                beVisit.StatusType = BEntities.StatusType.Update;
                beVisit.FinalDate = DateTime.Now;
                beVisit.LogDate = DateTime.Now;
                beVisit.LogUser = UserCode;
                bcVisit.Save(ref beVisit);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        #endregion

        #region Private Methods

        private List<BEV.VisitReception> GetItems(string CardCode, string Visitor, long Staff, DateTime? Since, DateTime? Until, bool NotFinished)
        {
            BCV.VisitReception bcVisit = new BCV.VisitReception();
            List<BEV.VisitReception> lstItems = new List<BEV.VisitReception>();
            List<Field> lstFilters = new List<Field>();
            int cont = 0;
            if (Since.HasValue)
            {
                lstFilters.Add(new Field { Name = "CAST(ISNULL(v.FinalDate, v.InitialDate) AS DATE)", Value = Since.Value.ToString("yyyy-MM-dd"), Operator = Operators.HigherOrEqualThan });
                cont++;
            }
            if (Until.HasValue & !NotFinished)
            {
                lstFilters.Add(new Field { Name = "CAST(v.InitialDate AS DATE)", Value = Until.Value.ToString("yyyy-MM-dd"), Operator = Operators.LowerOrEqualThan });
                cont++;
            }
            if (NotFinished)
            {
                lstFilters.Add(new Field { Name = "v.FinalDate", Value = "NULL", Operator = Operators.IsNull });
                cont++;
            }
            if (!string.IsNullOrWhiteSpace(CardCode))
            {
                lstFilters.Add(new Field { Name = "v.CardCode", Value = CardCode });
                cont++;
            }
            if (Staff != 0)
            {
                lstFilters.Add(new Field { Name = "v.StaffId", Value = Staff });
                cont++;
            }
            if (!string.IsNullOrWhiteSpace(Visitor))
            {
                lstFilters.Add(new Field { Name = "p.DocumentId", Value = Visitor, Operator = Operators.Likes });
                lstFilters.Add(new Field { Name = "p.FirstName", Value = Visitor, Operator = Operators.Likes });
                lstFilters.Add(new Field { Name = "p.LastName", Value = Visitor, Operator = Operators.Likes });
                lstFilters.Add(new Field { LogicalOperator = LogicalOperators.Or });
                lstFilters.Add(new Field { LogicalOperator = LogicalOperators.Or });
                cont++;
            }
            for (int i = 1; i < cont; i++)
            {
                lstFilters.Add(new Field { LogicalOperator = LogicalOperators.And });
            }
            lstItems = bcVisit.List2(lstFilters, "v.InitialDate", BEV.relVisitReception.Staff, BEV.relVisitReception.Visitor);

            return lstItems;
        }

        #endregion
    }
}