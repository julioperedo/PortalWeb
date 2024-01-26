using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using BCA = BComponents.SAP;
using BCB = BComponents.Base;
using BCD = BComponents.AppData;
using BCP = BComponents.Product;
using BCS = BComponents.Security;
using BCV = BComponents.Visits;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BED = BEntities.AppData;
using BEE = BEntities.Enums;
using BEP = BEntities.Product;
using BES = BEntities.Security;
using BEV = BEntities.Visits;

namespace MobileService.Controllers
{
    public class VisitsController : BaseController
    {

        //[HttpGet]
        //public HttpResponseMessage GetPersons(string Codes) {
        //    string strMessage = "";
        //    try {
        //        BCV.Person bcPerson = new BCV.Person();
        //        List<Field> lstFilters = new List<Field>();
        //        if(!string.IsNullOrWhiteSpace(Codes)) {
        //            lstFilters.Add(new Field { Name = "Id", Value = Codes, Operator = Operators.NotIn });
        //        }
        //        List<BEV.Person> lstPersons = bcPerson.List(lstFilters, "1");
        //        var items = (from p in lstPersons select new { id = p.Id, firstName = p.FirstName, lastName = p.LastName, documentId = p.DocumentId }).ToList();
        //        return Request.CreateResponse(HttpStatusCode.OK, new { Message = strMessage, Items = items });
        //    } catch(Exception ex) {
        //        strMessage = GetError(ex);
        //    }
        //    return Request.CreateResponse(HttpStatusCode.OK, new { Message = strMessage });
        //}

        //[HttpGet]
        //public HttpResponseMessage GetPictures(string Codes) {
        //    string strMessage = "";
        //    try {
        //        BCV.Picture bcPicture = new BCV.Picture();
        //        List<Field> lstFilters = new List<Field>();
        //        if(!string.IsNullOrWhiteSpace(Codes)) {
        //            lstFilters.Add(new Field { Name = "Id", Value = Codes, Operator = Operators.NotIn });
        //        }
        //        List<BEV.Picture> lstPictures = bcPicture.List(lstFilters, "1");
        //        var items = (from p in lstPictures select new { id = p.Id, fileName = p.Name, type = p.Type, personId = p.IdPerson }).ToList();
        //        return Request.CreateResponse(HttpStatusCode.OK, new { Message = strMessage, Items = items });
        //    } catch(Exception ex) {
        //        strMessage = GetError(ex);
        //    }
        //    return Request.CreateResponse(HttpStatusCode.OK, new { Message = strMessage });
        //}

        //[HttpGet]
        //public HttpResponseMessage GetVisits(string Codes) {
        //    string strMessage = "";
        //    try {
        //        BCV.Visit bcVisit = new BCV.Visit();
        //        List<Field> lstFilters = new List<Field>();
        //        if(!string.IsNullOrWhiteSpace(Codes)) {
        //            lstFilters.Add(new Field { Name = "Id", Value = Codes, Operator = Operators.NotIn });
        //        }
        //        List<BEV.Visit> lstVisits = bcVisit.List(lstFilters, "1");
        //        string dateFormat = "yyyy-MM-dd HH:mm:ss";
        //        var items = (from v in lstVisits
        //                     select new {
        //                         id = v.Id, cardCode = v.CardCode, visitorId = v.VisitorId, initialDate = v.InitialDate.ToString(dateFormat), finalDate = v.FinalDate.HasValue ? v.FinalDate.Value.ToString(dateFormat) : null,
        //                         staffId = v.StaffId, reasonVisit = v.ReasonVisit, licensePlate = v.LicencePlate
        //                     }).ToList();
        //        return Request.CreateResponse(HttpStatusCode.OK, new { Message = strMessage, Items = items });
        //    } catch(Exception ex) {
        //        strMessage = GetError(ex);
        //    }
        //    return Request.CreateResponse(HttpStatusCode.OK, new { Message = strMessage });
        //}

        //[HttpGet]
        //public HttpResponseMessage SavePerson(string DocumentId, string FirstName, string LastName, long UserId) {
        //    string strMessage = "";
        //    try {
        //        BCV.Person bcPerson = new BCV.Person();
        //        BEV.Person bePerson;
        //        List<Field> lstFilters = new List<Field> { new Field { Name = "DocumentId", Value = DocumentId } };
        //        var lstTemp = bcPerson.List(lstFilters, "1");
        //        if(lstTemp.Count > 0) {
        //            bePerson = lstTemp.First();
        //        } else {
        //            bePerson = new BEV.Person { StatusType = BEntities.StatusType.Insert, FirstName = FirstName, LastName = LastName, DocumentId = DocumentId, Phone = "", LogUser = UserId, LogDate = DateTime.Now };
        //        }
        //        bcPerson.Save(ref bePerson);
        //        return Request.CreateResponse(HttpStatusCode.OK, new { Message = strMessage, bePerson.Id });
        //    } catch(Exception ex) {
        //        strMessage = GetError(ex);
        //    }
        //    return Request.CreateResponse(HttpStatusCode.OK, new { Message = strMessage });
        //}

        //[HttpGet]
        //public HttpResponseMessage SavePicture(string Name, int Type, long PersonId, long UserId) {
        //    string strMessage = "";
        //    try {
        //        BCV.Picture bcPicture = new BCV.Picture();
        //        BEV.Picture bePicture = new BEV.Picture { StatusType = BEntities.StatusType.Insert, Name = Name, Type = Type, IdPerson = PersonId, LogUser = UserId, LogDate = DateTime.Now };
        //        bcPicture.Save(ref bePicture);
        //        return Request.CreateResponse(HttpStatusCode.OK, new { Message = strMessage, bePicture.Id });
        //    } catch(Exception ex) {
        //        strMessage = GetError(ex);
        //    }
        //    return Request.CreateResponse(HttpStatusCode.OK, new { Message = strMessage });
        //}

        //[HttpGet]
        //public HttpResponseMessage SaveVisit(long Id, string CardCode, long VisitorId, string InitialDate, string FinalDate, long StaffId, string ReasonVisit, string LicensePLate, string Commentaries, long UserId) {
        //    string strMessage = "";
        //    try {
        //        string dateFormat = "yyyy-M-d HH:mm:ss";
        //        var initialDate = DateTime.ParseExact(InitialDate, dateFormat, null);
        //        DateTime? finalDate = null;
        //        if(!string.IsNullOrWhiteSpace(FinalDate)) finalDate = DateTime.ParseExact(FinalDate, dateFormat, null);
        //        BCV.Visit bcVisit = new BCV.Visit();
        //        BEV.Visit beVisit;
        //        if(Id == 0) {
        //            List<Field> lstFilter = new List<Field>();
        //            lstFilter.Add(new Field { Name = "VisitorId", Value = VisitorId });
        //            lstFilter.Add(new Field { Name = "InitialDate", Value = $"{InitialDate}.000" });
        //            lstFilter.Add(new Field { LogicalOperator = LogicalOperators.And });
        //            var lstVisits = bcVisit.List(lstFilter, "1");
        //            if(lstVisits?.Count > 0) {
        //                beVisit = lstVisits.First();
        //                beVisit.FinalDate = finalDate;
        //                beVisit.StatusType = BEntities.StatusType.Update;
        //            } else {
        //                beVisit = new BEV.Visit {
        //                    StatusType = BEntities.StatusType.Insert, CardCode = CardCode, VisitorId = VisitorId, InitialDate = initialDate, FinalDate = finalDate, StaffId = StaffId, ReasonVisit = ReasonVisit,
        //                    LicencePlate = LicensePLate, Commentaries = Commentaries
        //                };
        //            }
        //        } else {
        //            beVisit = bcVisit.Search(Id);
        //            beVisit.StatusType = BEntities.StatusType.Update;
        //            beVisit.FinalDate = finalDate;
        //        }
        //        beVisit.LogUser = UserId;
        //        beVisit.LogDate = DateTime.Now;
        //        bcVisit.Save(ref beVisit);
        //        return Request.CreateResponse(HttpStatusCode.OK, new { Message = strMessage, beVisit.Id });
        //    } catch(Exception ex) {
        //        strMessage = GetError(ex);
        //    }
        //    return Request.CreateResponse(HttpStatusCode.OK, new { Message = strMessage });
        //}
    }
}
