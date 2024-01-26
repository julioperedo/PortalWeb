using BEntities.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using BCA = BComponents.SAP;
using BCB = BComponents.Base;
using BCK = BComponents.Kbytes;
using BCL = BComponents.Sales;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BEE = BEntities.Enums;
using BEK = BEntities.Kbytes;
using BEL = BEntities.Sales;
using MobileService.Models;
using System.Threading.Tasks;

namespace MobileService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinantialController : BaseController
    {

        #region GETs

        [HttpGet("orders")]
        public IActionResult Orders(string Code = null, string CardCode = null, string State = null, string Subsidiary = null, DateTime? InitialDate = null, DateTime? FinalDate = null, string Seller = null, string Complete = null, string Line = null, string Category = null)
        {
            string message = "";
            try
            {
                BCA.Order bcOrder = new();
                List<Field> filters = new(), itemFilters = new();
                if (!string.IsNullOrEmpty(Code))
                {
                    filters.AddRange(new[] { new Field("DocNumber", Code), new Field("LOWER(ClientOrder)", Code.ToLower(), Operators.Likes), new Field(LogicalOperators.Or) });
                }
                if (!string.IsNullOrEmpty(Subsidiary))
                {
                    filters.Add(new Field("LOWER(Subsidiary)", Subsidiary.ToLower()));
                }
                //if (!string.IsNullOrEmpty(Warehouse))
                //{
                //    filters.Add(new Field("LOWER(Warehouse)", Warehouse.ToLower()));
                //}
                if (!string.IsNullOrEmpty(CardCode))
                {
                    filters.Add(new Field("LOWER(ClientCode)", CardCode.ToLower()));
                }
                if (InitialDate.HasValue)
                {
                    filters.Add(new Field("DocDate", InitialDate.Value.ToString("yyyy-MM-dd"), Operators.HigherOrEqualThan));
                }
                if (FinalDate.HasValue)
                {
                    filters.Add(new Field("DocDate", FinalDate.Value.ToString("yyyy-MM-dd"), Operators.LowerOrEqualThan));
                }
                if (!string.IsNullOrEmpty(Seller))
                {
                    filters.Add(new Field("LOWER(SellerCode)", Seller.ToLower()));
                }
                //if (!string.IsNullOrEmpty(ProductManager))
                //{
                //    itemFilters.Add(new Field("LOWER(s1.U_GPRODUCT)", ProductManager.ToLower()));
                //}
                if (!string.IsNullOrEmpty(State))
                {
                    filters.Add(new Field("State", State == "O" ? "Abierto" : "Cerrado"));
                }
                if (!string.IsNullOrEmpty(Complete))
                {
                    List<string> codes = Complete.Split(',').ToList();
                    if (codes.Count == 2)
                    {
                        if (codes.Contains("I") & codes.Contains("PI"))
                        {
                            filters.AddRange(new[] { new Field("NonCompleteItem", 0, Operators.HigherThan) });
                        }
                        if (codes.Contains("I") & codes.Contains("C"))
                        {
                            filters.AddRange(new[] { new Field("NonCompleteItem", 0), new Field("ItemsCount - NonCompleteItem", 0), new Field(LogicalOperators.Or) });
                        }
                        if (codes.Contains("PI") & codes.Contains("C"))
                        {
                            filters.AddRange(new[] { new Field("NonCompleteItem", 0), new Field("ItemsCount - NonCompleteItem", 0, Operators.Different), new Field(LogicalOperators.Or) });
                        }
                    }
                    if (codes.Count == 1)
                    {
                        if (codes.Contains("I")) filters.AddRange(new[] { new Field("NonCompleteItem", 0, Operators.HigherThan), new Field("ItemsCount - NonCompleteItem", 0), new Field(LogicalOperators.And) });
                        if (codes.Contains("PI")) filters.AddRange(new[] { new Field("NonCompleteItem", 0, Operators.HigherThan), new Field("ItemsCount - NonCompleteItem", 0, Operators.Different), new Field(LogicalOperators.And) });
                        if (codes.Contains("C")) filters.AddRange(new[] { new Field("NonCompleteItem", 0) });
                    }
                }
                //if (!string.IsNullOrEmpty(ItemCode))
                //{
                //    itemFilters.AddRange(new[] { new Field("LOWER(s1.ItemCode)", ItemCode.ToLower(), Operators.Likes), new Field("LOWER(s1.ItemName)", ItemCode.ToLower(), Operators.Likes), new Field(LogicalOperators.Or) });
                //}
                if (!string.IsNullOrEmpty(Category))
                {
                    itemFilters.Add(new Field("LOWER(s1.U_CATEGORIA)", Category.ToLower()));
                }
                //if (!string.IsNullOrEmpty(Subcategory))
                //{
                //    itemFilters.Add(new Field("LOWER(s1.U_SUBCATEG)", Subcategory.ToLower()));
                //}
                if (!string.IsNullOrEmpty(Line))
                {
                    itemFilters.Add(new Field("LOWER(s1.U_LINEA)", Line.ToLower()));
                }
                CompleteFilters(ref filters);
                CompleteFilters(ref itemFilters);
                var orders = bcOrder.List4(filters, itemFilters, "1", BEA.relOrder.Files);
                if (orders?.Count() > 0)
                {
                    string ids = "";
                    List<BEA.DocumentRelated> relatedDocs = new();
                    ids = string.Join(",", from o in orders where o.Subsidiary.ToLower() == "santa cruz" select o.Id);
                    relatedDocs.AddRange(bcOrder.ListRelatedDocuments("Santa Cruz", ids));
                    ids = string.Join(",", from o in orders where o.Subsidiary.ToLower() == "iquique" select o.Id);
                    relatedDocs.AddRange(bcOrder.ListRelatedDocuments("Iquique", ids));
                    ids = string.Join(",", from o in orders where o.Subsidiary.ToLower() == "miami" select o.Id);
                    relatedDocs.AddRange(bcOrder.ListRelatedDocuments("Miami", ids));

                    ids = string.Join(",", from o in orders where o.Subsidiary.ToLower() == "santa cruz" select o.DocNumber);
                    BCL.Transport bcTransport = new();
                    var transportList = !string.IsNullOrEmpty(ids) ? bcTransport.List(ids, "1", BEL.relTransport.Destination, BEL.relTransport.Source, BEL.relTransport.Transporter) : new List<BEL.Transport>();

                    var items = from x in orders
                                orderby x.Subsidiary descending
                                group x by x.Subsidiary into g
                                select new
                                {
                                    name = g.Key,
                                    items = from i in g
                                            orderby i.DocNumber
                                            select new
                                            {
                                                i.Id,
                                                Authorized = i.Authorized == "Y",
                                                ClientCode = i.ClientCode ?? "",
                                                ClientName = i.ClientName ?? "",
                                                ClientOrder = i.ClientOrder ?? "",
                                                Header = i.Header?.Trim() ?? "",
                                                Footer = i.Footer?.Trim() ?? "",
                                                Complete = i.NonCompleteItem == 0,
                                                i.DocDate,
                                                i.DocNumber,
                                                Terms = i.TermConditions,
                                                DocType = "Orden de Venta",
                                                Files = i.Files.Select(i => new { i.Path, i.FileName, i.FileExt }),
                                                i.Margin,
                                                i.TaxlessTotal,
                                                Margin0100 = i.TaxlessTotal > 0 ? 100 * (i.Margin / i.TaxlessTotal) : 0,
                                                i.OpenAmount,
                                                SellerName = i.SellerName ?? "",
                                                i.State,
                                                Subsidiary = ToTitle(i.Subsidiary),
                                                Warehouse = ToTitle(i.Warehouse ?? ""),
                                                i.Total,
                                                RelatedDocs = from r in relatedDocs orderby r.DocType 
                                                              where r.BaseId == i.Id & r.Subsidiary.ToLower() == i.Subsidiary.ToLower() 
                                                              select new { r.DocType, r.DocNumber },
                                                Transport = (from t in transportList
                                                             where t.StringValues == i.DocNumber.ToString() & i.Subsidiary.ToLower() == "santa cruz"
                                                             select new
                                                             {
                                                                 t.DocNumber,
                                                                 t.Date,
                                                                 Source = t.Source.Name,
                                                                 Destination = t.Destination.Name,
                                                                 Transporter = t.Transporter.Name,
                                                                 t.DeliveryTo,
                                                                 Observations = t.Observations ?? "",
                                                                 t.Weight,
                                                                 t.QuantityPieces,
                                                                 t.RemainingAmount
                                                             }).Distinct()
                                            }
                                };
                    return Ok(new { message, items });
                }
                else
                {
                    return Ok(new { message, items = orders });
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Ok(new { message });
        }

        [HttpGet("notes")]
        public IActionResult Notes(string Code = null, string CardCode = null, string Subsidiary = null, DateTime? InitialDate = null, DateTime? FinalDate = null, string Seller = null, string Line = null, string Category = null)
        {
            string message = "";
            try
            {
                BCA.Note bcNote = new();
                List<Field> filters = new(), itemFilters = new();
                if (!string.IsNullOrEmpty(Code))
                {
                    filters.Add(new Field("DocNumber", Code));
                }
                if (!string.IsNullOrEmpty(Subsidiary))
                {
                    filters.Add(new Field("LOWER(Subsidiary)", Subsidiary.ToLower()));
                }
                //if (!string.IsNullOrEmpty(Warehouse))
                //{
                //    filters.Add(new Field("LOWER(Warehouse)", Warehouse.ToLower()));
                //}
                if (!string.IsNullOrEmpty(CardCode))
                {
                    filters.Add(new Field("LOWER(ClientCode)", CardCode.ToLower()));
                }
                if (InitialDate.HasValue)
                {
                    filters.Add(new Field("DocDate", InitialDate.Value.ToString("yyyy-MM-dd"), Operators.HigherOrEqualThan));
                }
                if (FinalDate.HasValue)
                {
                    filters.Add(new Field("DocDate", FinalDate.Value.ToString("yyyy-MM-dd"), Operators.LowerOrEqualThan));
                }
                if (!string.IsNullOrEmpty(Seller))
                {
                    filters.Add(new Field("LOWER(SellerCode)", Seller.ToLower()));
                }
                //if (!string.IsNullOrEmpty(ProductManager))
                //{
                //    itemFilters.Add(new Field("LOWER(t2.U_GPRODUCT)", ProductManager.ToLower()));
                //}
                //if (!string.IsNullOrEmpty(ItemCode))
                //{
                //    itemFilters.AddRange(new[] { new Field("LOWER(t2.ItemCode)", ItemCode.ToLower(), Operators.Likes), new Field("LOWER(t2.ItemName)", ItemCode.ToLower(), Operators.Likes), new Field(LogicalOperators.Or) });
                //}
                if (!string.IsNullOrEmpty(Category))
                {
                    itemFilters.Add(new Field("LOWER(t2.U_CATEGORIA)", Category.ToLower()));
                }
                //if (!string.IsNullOrEmpty(Subcategory))
                //{
                //    itemFilters.Add(new Field("LOWER(t2.U_SUBCATEG)", Subcategory.ToLower()));
                //}
                if (!string.IsNullOrEmpty(Line))
                {
                    itemFilters.Add(new Field("LOWER(t2.U_LINEA)", Line.ToLower()));
                }
                CompleteFilters(ref filters);
                CompleteFilters(ref itemFilters);
                var notes = bcNote.List(filters, itemFilters, "1", BEA.relNote.Files);
                if (notes?.Count() > 0)
                {
                    string ids = "";
                    List<BEA.DocumentRelated> relatedDocs = new();
                    ids = string.Join(",", from o in notes where o.Subsidiary.ToLower() == "santa cruz" select o.Id);
                    relatedDocs.AddRange(bcNote.ListRelatedDocuments("Santa Cruz", ids));
                    ids = string.Join(",", from o in notes where o.Subsidiary.ToLower() == "iquique" select o.Id);
                    relatedDocs.AddRange(bcNote.ListRelatedDocuments("Iquique", ids));
                    ids = string.Join(",", from o in notes where o.Subsidiary.ToLower() == "miami" select o.Id);
                    relatedDocs.AddRange(bcNote.ListRelatedDocuments("Miami", ids));

                    ids = string.Join(",", from o in relatedDocs where o.Subsidiary.ToLower() == "santa cruz" & o.DocType == "Orden de Venta" select o.DocNumber);
                    BCL.Transport bcTransport = new();
                    var transportList = !string.IsNullOrEmpty(ids) ? bcTransport.List(ids, "1", BEL.relTransport.Destination, BEL.relTransport.Source, BEL.relTransport.Transporter) : new List<BEL.Transport>();

                    var items = from t in notes
                                orderby t.Subsidiary descending
                                group t by t.Subsidiary into g
                                select new
                                {
                                    name = g.Key,
                                    items = from x in g
                                            orderby x.DocNumber
                                            select new
                                            {
                                                x.Id,
                                                ClientCode = x.ClientCode ?? "",
                                                ClientName = x.ClientName ?? "",
                                                x.DocDate,
                                                x.DocNumber,
                                                DocType = "Nota de Venta",
                                                Terms = x.TermConditions,
                                                Files = x.Files.Select(i => new { i.Path, i.FileName, i.FileExt }),
                                                x.Margin,
                                                x.TaxlessTotal,
                                                Margin0100 = x.TaxlessTotal > 0 ? 100 * (x.Margin / x.TaxlessTotal) : 0,
                                                SellerName = x.SellerName ?? "",
                                                Subsidiary = ToTitle(x.Subsidiary),
                                                Warehouse = ToTitle(x.Warehouse ?? ""),
                                                x.Total,
                                                OpenAmount = 0,
                                                RelatedDocs = (from r in relatedDocs orderby r.DocType where r.BaseId == x.Id & r.Subsidiary.ToLower() == x.Subsidiary.ToLower() select new { r.DocType, r.DocNumber }).ToList(), //relatedDocs.FindAll(r => r.BaseId == x.Id & r.Subsidiary.ToLower() == x.Subsidiary.ToLower()),
                                                Transport = (from t in transportList
                                                             where (from r in relatedDocs where r.BaseId == x.Id & r.Subsidiary.ToLower() == x.Subsidiary.ToLower() & r.DocType == "Orden de Venta" select r.DocNumber.ToString()).Contains(t.StringValues)
                                                             select new
                                                             {
                                                                 t.DocNumber,
                                                                 t.Date,
                                                                 Source = t.Source.Name,
                                                                 Destination = t.Destination.Name,
                                                                 Transporter = t.Transporter.Name,
                                                                 t.DeliveryTo,
                                                                 Observations = t.Observations ?? "",
                                                                 t.Weight,
                                                                 t.QuantityPieces,
                                                                 t.RemainingAmount,
                                                                 t.StringValues
                                                             }).Distinct()
                                            }
                                };
                    return Ok(new { message, items });
                }
                else
                {
                    return Ok(new { message, items = notes });
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Ok(new { message });
        }

        [HttpGet("deliverynotes")]
        public IActionResult DeliveryNotes(string Code = null, string CardCode = null, string Subsidiary = null, DateTime? InitialDate = null, DateTime? FinalDate = null, string Seller = null, string Line = null, string Category = null)
        {
            string message = "";
            try
            {
                BCA.DeliveryNote bcNote = new();
                List<Field> filters = new(), itemFilters = new();
                if (!string.IsNullOrEmpty(Code))
                {
                    filters.Add(new Field("DocNumber", Code));
                }
                if (!string.IsNullOrEmpty(Subsidiary))
                {
                    filters.Add(new Field("LOWER(Subsidiary)", Subsidiary.ToLower()));
                }
                //if (!string.IsNullOrEmpty(Warehouse))
                //{
                //    filters.Add(new Field("LOWER(Warehouse)", Warehouse.ToLower()));
                //}
                if (!string.IsNullOrEmpty(CardCode))
                {
                    filters.Add(new Field("LOWER(ClientCode)", CardCode.ToLower()));
                }
                if (InitialDate.HasValue)
                {
                    filters.Add(new Field("DocDate", InitialDate.Value.ToString("yyyy-MM-dd"), Operators.HigherOrEqualThan));
                }
                if (FinalDate.HasValue)
                {
                    filters.Add(new Field("DocDate", FinalDate.Value.ToString("yyyy-MM-dd"), Operators.LowerOrEqualThan));
                }
                if (!string.IsNullOrEmpty(Seller))
                {
                    filters.Add(new Field("LOWER(SellerCode)", Seller.ToLower()));
                }
                //if (!string.IsNullOrEmpty(ProductManager))
                //{
                //    itemFilters.Add(new Field("LOWER(s1.U_GPRODUCT)", ProductManager.ToLower()));
                //}
                //if (!string.IsNullOrEmpty(ItemCode))
                //{
                //    itemFilters.AddRange(new[] { new Field("LOWER(s1.ItemCode)", ItemCode.ToLower(), Operators.Likes), new Field("LOWER(s1.ItemName)", ItemCode.ToLower(), Operators.Likes), new Field(LogicalOperators.Or) });
                //}
                if (!string.IsNullOrEmpty(Category))
                {
                    itemFilters.Add(new Field("LOWER(s1.U_CATEGORIA)", Category.ToLower()));
                }
                //if (!string.IsNullOrEmpty(Subcategory))
                //{
                //    itemFilters.Add(new Field("LOWER(s1.U_SUBCATEG)", Subcategory.ToLower()));
                //}
                if (!string.IsNullOrEmpty(Line))
                {
                    itemFilters.Add(new Field("LOWER(s1.U_LINEA)", Line.ToLower()));
                }
                CompleteFilters(ref filters);
                CompleteFilters(ref itemFilters);
                var notes = bcNote.List(filters, itemFilters, "1", BEA.relDeliveryNote.Files);
                if (notes?.Count() > 0)
                {
                    string ids = "";
                    List<BEA.DocumentRelated> relatedDocs = new();
                    ids = string.Join(",", from o in notes where o.Subsidiary.ToLower() == "santa cruz" select o.Id);
                    relatedDocs.AddRange(bcNote.ListRelatedDocuments("Santa Cruz", ids));
                    ids = string.Join(",", from o in notes where o.Subsidiary.ToLower() == "iquique" select o.Id);
                    relatedDocs.AddRange(bcNote.ListRelatedDocuments("Iquique", ids));
                    ids = string.Join(",", from o in notes where o.Subsidiary.ToLower() == "miami" select o.Id);
                    relatedDocs.AddRange(bcNote.ListRelatedDocuments("Miami", ids));

                    ids = string.Join(",", from o in relatedDocs where o.Subsidiary.ToLower() == "santa cruz" & o.DocType == "Orden de Venta" select o.DocNumber);
                    BCL.Transport bcTransport = new();
                    var transportList = !string.IsNullOrEmpty(ids) ? bcTransport.List(ids, "1", BEL.relTransport.Destination, BEL.relTransport.Source, BEL.relTransport.Transporter) : new List<BEL.Transport>();

                    var items = from t in notes
                                orderby t.Subsidiary descending
                                group t by t.Subsidiary into g
                                select new
                                {
                                    name = g.Key,
                                    items = from x in g
                                            orderby x.DocNumber
                                            select new
                                            {
                                                x.Id,
                                                x.ClientCode,
                                                x.ClientName,
                                                x.DocDate,
                                                x.DocNumber,
                                                DocType = "Nota de Entrega",
                                                x.Terms,
                                                Files = x.Files.Select(i => new { i.Path, i.FileName, i.FileExt }),
                                                x.Margin,
                                                x.TaxlessTotal,
                                                Margin0100 = x.TaxlessTotal > 0 ? 100 * (x.Margin / x.TaxlessTotal) : 0,
                                                x.SellerName,
                                                Subsidiary = ToTitle(x.Subsidiary),
                                                Warehouse = ToTitle(x.Warehouse),
                                                x.Total,
                                                OpenAmount = 0,
                                                RelatedDocs = from r in relatedDocs orderby r.DocType where r.BaseId == x.Id & r.Subsidiary.ToLower() == x.Subsidiary.ToLower() select new { r.DocType, r.DocNumber },
                                                Transport = (from t in transportList
                                                             where (from r in relatedDocs where r.BaseId == x.Id & r.Subsidiary.ToLower() == x.Subsidiary.ToLower() & r.DocType == "Orden de Venta" select r.DocNumber.ToString()).Contains(t.StringValues)
                                                             select new
                                                             {
                                                                 t.DocNumber,
                                                                 t.Date,
                                                                 Source = t.Source.Name,
                                                                 Destination = t.Destination.Name,
                                                                 Transporter = t.Transporter.Name,
                                                                 t.DeliveryTo,
                                                                 Observations = t.Observations ?? "",
                                                                 t.Weight,
                                                                 t.QuantityPieces,
                                                                 t.RemainingAmount,
                                                                 t.StringValues
                                                             }).Distinct()
                                            }
                                };
                    return Ok(new { message, items });
                }
                else
                {
                    return Ok(new { message, items = notes });
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Ok(new { message });
        }

        [HttpGet("documentdetails")]
        public IActionResult DocumentDetails(string Subsidiary, int DocNumber, string State, string DocType)
        {
            string message = "";
            try
            {
                decimal? decNull = null;
                if (DocType == "Orden de Venta")
                {
                    BCA.Order bcOrder = new();
                    var lstOrderItems = bcOrder.ListItems(Subsidiary, DocNumber, "LineNum", BEA.relOrderItem.Product);
                    var items = lstOrderItems.Select(x => new { x.ItemCode, x.ItemName, x.Warehouse, x.Line, x.Quantity, x.OpenQuantity, x.Stock, x.Price, x.ItemTotal, Margin0100 = (decimal?)(x.TaxlessTotal != 0 ? x.Margin / x.TaxlessTotal : decNull), Complete = State != "Abierto" || x.OpenQuantity <= x.Stock }).ToList();
                    return Ok(new { message, items });
                }
                if (DocType == "Nota de Venta")
                {
                    BCA.Note bcNote = new();
                    List<Field> filters = new() { new Field("LOWER(Subsidiary)", Subsidiary.ToLower()), new Field("Id", DocNumber), new Field(LogicalOperators.And) };
                    var noteItems = bcNote.ListItems(filters, "LineNum", BEA.relNoteItem.Product);
                    var items = noteItems.Select(x => new { x.ItemCode, x.ItemName, x.Warehouse, x.Line, x.Quantity, x.Price, x.ItemTotal, Margin0100 = (decimal?)(x.CalculedTotal != 0 ? x.Margin / x.CalculedTotal : decNull) }).ToList();
                    return Ok(new { message, items });
                }
                if (DocType == "Nota de Entrega")
                {
                    BCA.DeliveryNote bcNote = new();
                    var filters = new[] { $"'{Subsidiary.ToLower()}-{DocNumber}'" };
                    var noteItems = bcNote.ListItems2(filters, "LineNum");
                    var items = noteItems.Select(x => new { x.ItemCode, x.ItemName, x.Warehouse, x.Line, x.Quantity, x.Price, ItemTotal = x.Total, Margin0100 = (decimal?)(x.TaxlessTotal != 0 ? x.Margin / x.TaxlessTotal : decNull) }).ToList();
                    return Ok(new { message, items });
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Ok(new { message });
        }

        [HttpGet("document")]
        public IActionResult Document(string Subsidiary, int DocNumber, string DocType)
        {
            string message = "";
            try
            {
                decimal? decNull = null;
                if (DocType == "Orden de Venta")
                {
                    BCA.Order bcOrder = new();
                    var order = bcOrder.Search2(DocNumber, Subsidiary, BEA.relOrder.OrderItems, BEA.relOrder.Files);
                    if (order?.Id > 0)
                    {
                        IEnumerable<BEA.DocumentRelated> relatedDocs = bcOrder.ListRelatedDocuments(Subsidiary, $"'{order.Id}'");
                        string ids = string.Join(",", from o in relatedDocs where o.Subsidiary.ToLower() == "santa cruz" & o.DocType == "Orden de Venta" select o.DocNumber);
                        BCL.Transport bcTransport = new();
                        var transportList = !string.IsNullOrEmpty(ids) ? bcTransport.List(ids, "1", BEL.relTransport.Destination, BEL.relTransport.Source, BEL.relTransport.Transporter) : new List<BEL.Transport>();

                        var item = new
                        {
                            order.Id,
                            order.ClientCode,
                            order.ClientName,
                            order.DocDate,
                            order.DocNumber,
                            order.State,
                            Authorized = order.Authorized == "Y",
                            complete = order.NonCompleteItem == 0,
                            ClientOrder = order.ClientOrder ?? "",
                            DocType,
                            header = order.Header ?? "",
                            Footer = order.Footer ?? "",
                            Files = order.Files.Select(i => new { i.Path, i.FileName, i.FileExt }),
                            order.Margin,
                            order.TaxlessTotal,
                            Margin0100 = order.TaxlessTotal > 0 ? 100 * (order.Margin / order.TaxlessTotal) : 0,
                            order.SellerName,
                            Subsidiary = ToTitle(order.Subsidiary),
                            Warehouse = ToTitle(order.Warehouse),
                            order.Total,
                            order.OpenAmount,
                            terms = order.TermConditions,
                            RelatedDocs = (from r in relatedDocs orderby r.DocType where r.BaseId == order.Id & r.Subsidiary.ToLower() == order.Subsidiary.ToLower() select new { r.DocType, r.DocNumber }).ToList(),
                            Transport = (from t in transportList
                                         where t.StringValues == order.DocNumber.ToString()
                                         select new
                                         {
                                             t.DocNumber,
                                             t.Date,
                                             Source = t.Source.Name,
                                             Destination = t.Destination.Name,
                                             Transporter = t.Transporter.Name,
                                             t.DeliveryTo,
                                             Observations = t.Observations ?? "",
                                             t.Weight,
                                             t.QuantityPieces,
                                             t.RemainingAmount,
                                             t.StringValues
                                         }).Distinct(),
                            items = order.Items.Select(x => new { x.ItemCode, x.ItemName, x.Warehouse, x.Line, x.Quantity, x.Price, x.ItemTotal, Margin0100 = (decimal)(x.TaxlessTotal != 0 ? x.Margin / x.TaxlessTotal : decNull), x.Complete, x.OpenQuantity })
                        };
                        return Ok(new { message, item });
                    }
                    else
                    {
                        message = "Orden de Venta no encontrada.";
                    }
                }
                if (DocType == "Nota de Venta")
                {
                    BCA.Note bcNote = new();
                    var note = bcNote.Search(DocNumber, Subsidiary, BEA.relNote.NoteItems, BEA.relNote.Files);
                    if (note?.Id > 0)
                    {
                        IEnumerable<BEA.DocumentRelated> relatedDocs = bcNote.ListRelatedDocuments(Subsidiary, $"'{note.Id}'");
                        string ids = string.Join(",", from o in relatedDocs where o.Subsidiary.ToLower() == "santa cruz" & o.DocType == "Orden de Venta" select o.DocNumber);
                        BCL.Transport bcTransport = new();
                        var transportList = !string.IsNullOrEmpty(ids) ? bcTransport.List(ids, "1", BEL.relTransport.Destination, BEL.relTransport.Source, BEL.relTransport.Transporter) : new List<BEL.Transport>();
                        var item = new
                        {
                            note.Id,
                            note.ClientCode,
                            note.ClientName,
                            note.DocDate,
                            note.DocNumber,
                            DocType,
                            Files = note.Files.Select(i => new { i.Path, i.FileName, i.FileExt }),
                            note.Margin,
                            note.TaxlessTotal,
                            Margin0100 = note.TaxlessTotal > 0 ? 100 * (note.Margin / note.TaxlessTotal) : 0,
                            note.SellerName,
                            Subsidiary = ToTitle(note.Subsidiary),
                            Warehouse = ToTitle(note.Warehouse),
                            note.Total,
                            OpenAmount = 0,
                            terms = note.TermConditions,
                            RelatedDocs = from r in relatedDocs orderby r.DocType where r.BaseId == note.Id & r.Subsidiary.ToLower() == note.Subsidiary.ToLower() select new { r.DocType, r.DocNumber },
                            Transport = (from t in transportList
                                         where (from r in relatedDocs where r.BaseId == note.Id & r.Subsidiary.ToLower() == note.Subsidiary.ToLower() & r.DocType == "Orden de Venta" select r.DocNumber.ToString()).Contains(t.StringValues)
                                         select new
                                         {
                                             t.DocNumber,
                                             t.Date,
                                             Source = t.Source.Name,
                                             Destination = t.Destination.Name,
                                             Transporter = t.Transporter.Name,
                                             t.DeliveryTo,
                                             Observations = t.Observations ?? "",
                                             t.Weight,
                                             t.QuantityPieces,
                                             t.RemainingAmount,
                                             t.StringValues
                                         }).Distinct(),
                            items = note.Items.Select(x => new { x.ItemCode, x.ItemName, x.Warehouse, x.Line, x.Quantity, x.Price, x.ItemTotal, Margin0100 = (decimal)(x.CalculedTotal != 0 ? x.Margin / x.CalculedTotal : decNull) })
                        };
                        return Ok(new { message, item });
                    }
                    else
                    {
                        message = "Nota de Venta no encontrada.";
                    }
                }
                if (DocType == "Nota de Entrega")
                {
                    BCA.DeliveryNote bcNote = new();
                    var note = bcNote.Search(DocNumber, Subsidiary, BEA.relDeliveryNote.Items, BEA.relDeliveryNote.Files);
                    if (note?.Id > 0)
                    {
                        IEnumerable<BEA.DocumentRelated> relatedDocs = bcNote.ListRelatedDocuments(Subsidiary, $"'{note.Id}'");
                        string ids = string.Join(",", from o in relatedDocs where o.Subsidiary.ToLower() == "santa cruz" & o.DocType == "Orden de Venta" select o.DocNumber);
                        BCL.Transport bcTransport = new();
                        var transportList = !string.IsNullOrEmpty(ids) ? bcTransport.List(ids, "1", BEL.relTransport.Destination, BEL.relTransport.Source, BEL.relTransport.Transporter) : new List<BEL.Transport>();
                        var item = new
                        {
                            note.Id,
                            note.ClientCode,
                            note.ClientName,
                            note.DocDate,
                            note.DocNumber,
                            DocType,
                            Files = note.Files.Select(i => new { i.Path, i.FileName, i.FileExt }),
                            note.Margin,
                            note.TaxlessTotal,
                            Margin0100 = note.TaxlessTotal > 0 ? 100 * (note.Margin / note.TaxlessTotal) : 0,
                            note.SellerName,
                            Subsidiary = ToTitle(note.Subsidiary),
                            Warehouse = ToTitle(note.Warehouse),
                            note.Total,
                            OpenAmount = 0,
                            RelatedDocs = from r in relatedDocs orderby r.DocType where r.BaseId == note.Id & r.Subsidiary.ToLower() == note.Subsidiary.ToLower() select new { r.DocType, r.DocNumber },
                            Transport = (from t in transportList
                                         where (from r in relatedDocs where r.BaseId == note.Id & r.Subsidiary.ToLower() == note.Subsidiary.ToLower() & r.DocType == "Orden de Venta" select r.DocNumber.ToString()).Contains(t.StringValues)
                                         select new
                                         {
                                             t.DocNumber,
                                             t.Date,
                                             Source = t.Source.Name,
                                             Destination = t.Destination.Name,
                                             Transporter = t.Transporter.Name,
                                             t.DeliveryTo,
                                             Observations = t.Observations ?? "",
                                             t.Weight,
                                             t.QuantityPieces,
                                             t.RemainingAmount,
                                             t.StringValues
                                         }).Distinct(),
                            items = note.Items.Select(x => new { x.ItemCode, x.ItemName, x.Warehouse, x.Line, x.Quantity, x.Price, x.Total, Margin0100 = (decimal)(x.TaxlessTotal != 0 ? x.Margin / x.TaxlessTotal : decNull) })
                        };
                        return Ok(new { message, item });
                    }
                    else
                    {
                        message = "Nota de Venta no encontrada.";
                    }
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Ok(new { message });
        }

        //[HttpGet("orderdetail")]
        //public IActionResult OrderDetail(string Subsidiary, int DocNumber)
        //{
        //    string message = "";
        //    try
        //    {
        //        BCA.Order bcOrder = new BCA.Order();
        //        BEA.Order order = bcOrder.Search(DocNumber, Subsidiary, BEA.relOrder.OrderItems, BEA.relOrder.Notes, BEA.relNote.NoteItems);
        //        if (order != null)
        //        {
        //            var item = new
        //            {
        //                subsidiary = order.Subsidiary,
        //                warehouse = ToTitle(order.Warehouse),
        //                docNumber = order.DocNumber,
        //                date = order.DocDate,
        //                order.ClientCode,
        //                clientName = ToTitle(order.ClientName),
        //                clientOrder = order.ClientOrder,
        //                order.Total,
        //                seller = ToTitle(order.SellerName),
        //                state = order.State,
        //                terms = order.TermConditions,
        //                totalBilled = order.Notes?.Count > 0 ? order.Notes?.Sum(i => i.Items.Sum(x => x.ItemTotal)) : 0,
        //                authorized = order.Authorized == "Y",
        //                margin = order.Notes?.Count > 0 ? 100 * order.Notes?.Sum(i => i.Items.Sum(x => x.Margin)) / order.Notes?.Sum(i => i.Items.Sum(x => x.CalculedTotal)) : 0,
        //                Notes = order.Notes?.Count > 0 ? string.Join(",", order.Notes.Select(i => i.DocNumber)) : "",
        //                noteDates = order.Notes?.Count > 0 ? string.Join(",", order.Notes.Select(i => i.DocDate.ToString("dd-MM-yyyy"))) : "",
        //                complete = order.Items.Count(x => x.Complete == false) == 0 | order.State == "Cerrado",
        //                items = (from d in order.Items
        //                         orderby d.ItemCode
        //                         select new { d.ItemCode, d.ItemName, d.Quantity, d.OpenQuantity, d.Price, subtotal = d.ItemTotal, complete = order.State == "Cerrado" | d.Complete }).ToList()
        //            };
        //            return Ok(new { message, item });
        //        }
        //        else
        //        {
        //            message = "No hay resultado para el criterio de Búsqueda.";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        message = GetError(ex);
        //    }
        //    return Ok(new { message });
        //}

        [HttpGet("paymentreceipts")]
        public IActionResult PaymentReceipts(string ClientCode = null, DateTime? InitialDate = null, DateTime? FinalDate = null, int? ReceiptCode = null, int? NoteCode = null)
        {
            string message = "";
            try
            {
                BCA.Payment bcPayment = new();
                IEnumerable<BEA.Payment> lstTemp = bcPayment.List(ClientCode, InitialDate, FinalDate, ReceiptCode, NoteCode, "3 DESC");
                List<Field> filters = new();
                if (!string.IsNullOrWhiteSpace(ClientCode)) filters.Add(new Field("ClientCode", ClientCode.Trim()));
                if (InitialDate.HasValue) filters.Add(new Field("DocDate", InitialDate.Value.ToString("yyyy-MM-dd"), Operators.HigherOrEqualThan));
                if (FinalDate.HasValue) filters.Add(new Field("DocDate", FinalDate.Value.ToString("yyyy-MM-dd"), Operators.LowerOrEqualThan));
                if (ReceiptCode.HasValue && ReceiptCode.Value > 0) filters.Add(new Field("Id", ReceiptCode));
                CompleteFilters(ref filters);
                IEnumerable<BEA.Payment> lstAdjust = bcPayment.ListAdjustments(filters, "DocDate");
                List<Models.Receipt> lstItems = new();
                lstItems = (from i in lstTemp
                            orderby i.DocDate descending
                            group i by i.DocNumber into g
                            select new Receipt
                            {
                                DocNumber = g.Key,
                                Subsidiary = ToTitle(g.First().Subsidiary),
                                DocDate = g.First().DocDate,
                                State = g.First().State,
                                ClientCode = g.First().ClientCode,
                                ClientName = g.First().ClientName,
                                TotalReceipt = g.First().TotalReceipt,
                                OnAccount = g.First().OnAccount,
                                NotAppliedTotal = g.First().NotAppliedTotal,
                                Comments = g.First().Comments,
                                TotalDueDays = (from d in g select d.DueDays).Sum(),
                                TotalBilled = g.Count(),
                                InDue = (from d in g where d.DueDays >= 10 select d).Count() > 0,
                                Notes = (from d in g
                                         where d.NoteNumber.HasValue && d.NoteNumber.Value > 0
                                         select new Note { NoteNumber = d.NoteNumber.Value, AmountPaid = d.NotePaidAmount, Total = d.Total, Days = d.DueDays, DocDate = d.NoteDate, Terms = d.Terms, IsDelivery = d.IsDeliveryNote }).ToList()
                            }).ToList();
                if (lstAdjust?.Count() > 0)
                {
                    lstItems.AddRange(lstAdjust.Select(i => new Receipt(i) { Subsidiary = ToTitle(i.Subsidiary) }));
                }
                var items = from i in lstItems
                            group i by i.Subsidiary into g
                            select new
                            {
                                name = ToTitle(g.Key),
                                dueDaysAVG = g.Average(x => x.Notes?.Average(y => y.Days) ?? 0),
                                items = from d in g
                                        select new
                                        {
                                            d.ClientCode,
                                            ClientName = ToTitle(d.ClientName),
                                            receipt = d.DocNumber,
                                            date = d.DocDate,
                                            state = d.State ?? "",
                                            total = d.TotalReceipt,
                                            inAdvance = d.OnAccount,
                                            notApplied = d.NotAppliedTotal,
                                            noteNumbers = d.Notes?.Count > 0 ? string.Join(",", d.Notes?.Select(x => $"{x.NoteNumber} ({x.Days})")) : "",
                                            adjust = d.Adjust
                                        }
                            };
                return Ok(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Ok(new { message });
        }

        [HttpGet("paymentreceipt")]
        public IActionResult PaymentReceipt(string Subsidiary, int ReceiptCode, string State)
        {
            string message = "";
            try
            {
                IEnumerable<BEA.Payment> lstTemp = Enumerable.Empty<BEA.Payment>();
                BCA.Payment bcPayment = new();
                if (State == "Reconciliado" | State == "Ajustes")
                {
                    lstTemp = bcPayment.ListAdjustment(Subsidiary, ReceiptCode, State);
                }
                else
                {
                    List<Field> lstFilter = new() { new Field { Name = "DocNumber", Value = ReceiptCode }, new Field { Name = "LOWER(Subsidiary)", Value = Subsidiary.ToLower() }, new Field { LogicalOperator = LogicalOperators.And } };
                    lstTemp = bcPayment.List(lstFilter, "1");
                }
                var item = (from i in lstTemp
                            orderby i.DocDate descending
                            group i by new { i.Subsidiary, i.DocNumber, i.DocDate, i.State, i.ClientCode, i.ClientName, i.TotalReceipt } into g
                            select new
                            {
                                Receipt = g.Key.DocNumber,
                                Subsidiary = ToTitle(g.Key.Subsidiary),
                                Date = g.Key.DocDate,
                                g.Key.State,
                                g.Key.ClientCode,
                                ClientName = ToTitle(g.Key.ClientName),
                                Total = g.Key.TotalReceipt,
                                inAdvance = g.First().OnAccount,
                                NotApplied = g.First().NotAppliedTotal,
                                Notes = (from d in g
                                         where d.NoteNumber > 0
                                         select new { d.NoteNumber, PaidAmount = d.NotePaidAmount, d.Total, Days = d.DueDays, Date = d.NoteDate, d.Terms, d.NoteTotal }).ToList()
                            }).FirstOrDefault();
                return Ok(new { message, item });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Ok(new { message });
        }

        [HttpGet("stateaccount")]
        public IActionResult StateAccount(string CardCode)
        {
            string message = "";
            try
            {
                IEnumerable<BEA.StateAccountItem> lstItems = GetResults(CardCode);
                var details = from r in lstItems
                              orderby r.DocDate
                              group r by r.Subsidiary into g
                              select new
                              {
                                  name = ToTitle(g.Key),
                                  balance = (from d in g select d.Balance).Sum(),
                                  items = from d in g
                                          select new
                                          {
                                              warehouse = ToTitle(d.Warehouse),
                                              type = d.Type,
                                              date = d.DocDate,
                                              docNumber = d.DocNum,
                                              saleNote = d.SaleNote,
                                              saleOrder = d.SaleOrder,
                                              clientOrder = d.ClientOrder,
                                              dueDate = d.DueDate,
                                              total = d.Total,
                                              balance = d.Balance,
                                              margin = Math.Round(d.PercetageMargin, 2),
                                              days = d.Days
                                          }
                              };

                BEA.ClientResume item = new();
                List<Field> lstFilter = new();
                BCA.Client bcClient = new();
                if (!string.IsNullOrWhiteSpace(CardCode))
                {
                    lstFilter.Add(new Field("CardCode", CardCode));
                }
                CompleteFilters(ref lstFilter);
                var lstTemp = bcClient.ListBalance(lstFilter, "1");

                item = (from t in lstTemp
                        group t by t.CardCode into g
                        select new BEA.ClientResume
                        {
                            ClientCode = g.Key,
                            ClientName = g.First().CardName,
                            CreditLimit = g.First().CreditLimit,
                            BalanceSA = g.FirstOrDefault(d => d.Subsidiary == "SANTA CRUZ")?.Balance ?? 0,
                            BalanceLA = g.FirstOrDefault(d => d.Subsidiary == "MIAMI")?.Balance ?? 0,
                            BalanceIQQ = g.FirstOrDefault(d => d.Subsidiary == "IQUIQUE")?.Balance ?? 0,
                            BalanceTotal = g.Sum(d => d.Balance),
                            OrdersSA = g.FirstOrDefault(d => d.Subsidiary == "SANTA CRUZ")?.OrdersBalance ?? 0,
                            OrdersLA = g.FirstOrDefault(d => d.Subsidiary == "MIAMI")?.OrdersBalance ?? 0,
                            OrdersIQQ = g.FirstOrDefault(d => d.Subsidiary == "IQUIQUE")?.OrdersBalance ?? 0,
                            OrdersTotal = g.Sum(d => d.OrdersBalance),
                            AvailableBalance = (g.FirstOrDefault()?.CreditLimit ?? 0) - g.Sum(d => d.Balance),
                            TermsSA = g.FirstOrDefault(d => d.Subsidiary == "SANTA CRUZ")?.Terms ?? "",
                            TermsLA = g.FirstOrDefault(d => d.Subsidiary == "MIAMI")?.Terms ?? "",
                            TermsIQQ = g.FirstOrDefault(d => d.Subsidiary == "IQUIQUE")?.Terms ?? ""
                        }).FirstOrDefault();

                return Ok(new { message, item, details });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Ok(new { message });
        }

        [HttpGet("stateaccount2")]
        public IActionResult StateAccount2(string CardCode)
        {
            string message = "";
            try
            {
                IEnumerable<BEA.StateAccountItem> lstItems = GetResults(CardCode);
                var details = from r in lstItems
                              orderby r.DocDate
                              group r by r.Subsidiary into g
                              select new
                              {
                                  name = ToTitle(g.Key),
                                  balance = (from d in g select d.Balance).Sum(),
                                  items = from d in g
                                          select new
                                          {
                                              warehouse = ToTitle(d.Warehouse),
                                              type = d.Type,
                                              date = d.DocDate,
                                              docNumber = d.DocNum,
                                              saleNote = d.SaleNote,
                                              saleOrder = d.SaleOrder,
                                              clientOrder = d.ClientOrder,
                                              dueDate = d.DueDate,
                                              total = d.Total,
                                              balance = d.Balance,
                                              margin = Math.Round(d.PercetageMargin, 2),
                                              days = d.Days
                                          }
                              };

                List<Field> lstFilter = new();
                BCA.Client bcClient = new();
                if (!string.IsNullOrWhiteSpace(CardCode))
                {
                    lstFilter.Add(new Field("CardCode", CardCode));
                }
                CompleteFilters(ref lstFilter);
                var temp = bcClient.ListBalance(lstFilter, "1");
                var items = temp.Select(x => new
                {
                    x.Subsidiary,
                    x.CreditLimit,
                    available = x.CreditLimit - x.Balance,
                    x.Balance,
                    x.Terms,
                    x.OrdersBalance,
                    dueAmount = (from i in lstItems where i.Days > 0 & x.Subsidiary.ToLower() == i.Subsidiary.ToLower() & i.State?.ToLower() == "en mora" select i.Balance).Sum(),
                    dueItems = from i in lstItems where i.Days > 0 & x.Subsidiary.ToLower() == i.Subsidiary.ToLower() & i.State?.ToLower() == "en mora" select new { i.Days, i.Balance }
                });

                return Ok(new { message, items, details });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Ok(new { message });
        }

        [HttpGet("stateaccountdetail")]
        public IActionResult StateAccountDetail(string Subsidiary, int DocNumber, string Type)
        {
            string message = "";
            try
            {
                IEnumerable<BEA.StateAccountDetail> lstItems = Enumerable.Empty<BEA.StateAccountDetail>();
                BCA.StateAccountDetail bcAccount = new();
                List<Field> lstFilter = new()
                {
                    new Field { Name = "LOWER(Subsidiary)", Value = Subsidiary.ToLower() },
                    new Field { Name = "LOWER(Type)", Value = Type.ToLower() },
                    new Field { Name = "DocNum", Value = DocNumber },
                    new Field { LogicalOperator = LogicalOperators.And },
                    new Field { LogicalOperator = LogicalOperators.And }
                };
                lstItems = bcAccount.List(lstFilter, "CAST(DocDate AS DATE)");
                int? intNUll = null;
                decimal? decNull = null;
                if (lstItems?.Count() > 0)
                {
                    var item = (from d in lstItems
                                orderby d.ItemCode
                                group d by new { d.Type, d.Subsidiary, d.DocNum } into g
                                select new
                                {
                                    subsidiary = ToTitle(g.Key.Subsidiary),
                                    warehouse = ToTitle(g.First().Warehouse),
                                    type = g.Key.Type,
                                    date = g.First().DocDate,
                                    docNumber = g.Key.DocNum,
                                    saleNote = g.Key.Type == "Factura" ? g.Key.DocNum : intNUll,
                                    saleOrder = g.First().DocBase,
                                    dueDate = g.First().DueDate,
                                    total = g.First().DocTotal,
                                    balance = g.First().Balance,
                                    margin = g.First().ItemCode != null ? Math.Round((from d in g select (d.Quantity.Value * ((d.Price.Value * d.Factor) - d.StockPrice.Value))).Sum() / (from d in g select (d.Quantity.Value * d.Price.Value * d.Factor)).Sum() * 100, 2) : decNull,
                                    days = g.First().Days,
                                    state = g.First().State,
                                    seller = g.First().Seller,
                                    clientOrder = g.First().ClientOrder,
                                    terms = g.First().Terms,
                                    items = g.First().ItemCode != null ?
                                    (from d in g
                                     select new
                                     {
                                         code = d.ItemCode,
                                         name = d.ItemName,
                                         quantity = d.Quantity,
                                         price = d.Price,
                                         margin = Math.Round((1 - ((d.StockPrice.HasValue ? d.StockPrice.Value : 0) / (d.Factor * (d.Price.HasValue && d.Price.Value > 0 ? d.Price.Value : 1)))) * 100, 2)
                                     }).ToList() : null
                                }).FirstOrDefault();
                    return Ok(new { message, item });
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Ok(new { Message = message });
        }

        [HttpGet("getresumesales")]
        public IActionResult GetResumeSales(DateTime? InitialDate, DateTime? FinalDate)
        {
            string message = "";
            try
            {
                BCB.DivisionConfig bcDivConfig = new();
                IEnumerable<BEB.DivisionConfig> lstConfigItems = bcDivConfig.List("Type, Name");
                var lines = (from i in lstConfigItems where i.Type == "M" select i.Name).ToList();
                var sellers = (from i in lstConfigItems where i.Type == "E" select i.Name).ToList();
                BCA.Resume bcResume = new();
                var stockList = bcResume.ResumeStock(lines);
                var salesList = bcResume.ResumeSaleByPeriod(InitialDate.Value, FinalDate.Value, lines, sellers);
                var salesTodayList = bcResume.ResumeSaleByPeriod(DateTime.Today, DateTime.Today, lines, sellers);
                var authorizedList = bcResume.AuthorizedOrders(lines, sellers);
                var opensList = bcResume.OpenAmounts(lines, sellers);

                var stock = from s in stockList
                            group s by new { s.Subsidiary, s.Division } into g
                            select new { g.Key.Subsidiary, g.Key.Division, Stock = g.Sum(x => x.Total) };
                var sales = from s in salesList
                            group s by new { s.Subsidiary, s.Division } into g
                            select new { g.Key.Subsidiary, g.Key.Division, Total = g.Sum(x => x.Total), Margin = g.Sum(x => x.Margin), TaxlessTotal = g.Sum(x => x.TaxlessTotal) };
                var salesToday = from s in salesTodayList
                                 group s by new { s.Subsidiary, s.Division } into g
                                 select new { g.Key.Subsidiary, g.Key.Division, Total = g.Sum(x => x.Total) };
                var authorized = from a in authorizedList
                                 group a by new { a.Subsidiary, a.Division } into g
                                 select new { g.Key.Subsidiary, g.Key.Division, Total = g.Sum(x => x.Total) };
                var opens = from o in opensList
                            group o by new { o.Subsidiary, o.Division } into g
                            select new { g.Key.Subsidiary, g.Key.Division, Total = g.Sum(x => x.Total) };

                List<string> divisions = new() { "Consumer", "Enterprise", "Mobile" };
                BCB.Classifier bcClassifier = new();
                List<BEB.Classifier> subsidiaries = bcClassifier.List((long)BEE.Classifiers.Subsidiary, "Value");
                List<BEL.Projection> tempProjection = new();
                if (InitialDate.HasValue & FinalDate.HasValue)
                {
                    //if (InitialDate.Value.Day == 1 & InitialDate.Value.Year == FinalDate.Value.Year & InitialDate.Value.Month == FinalDate.Value.Month)
                    //{
                    //    List<Field> filters = new List<Field> { new Field("[Year]", InitialDate.Value.Year), new Field("[Month]", InitialDate.Value.Month), new Field(LogicalOperators.And) };
                    //    BCL.Projection bcProyection = new BCL.Projection();
                    //    tempProjection = bcProyection.List(filters, "1");
                    //}
                    BCL.Projection bcProyection = new();
                    tempProjection = bcProyection.List(InitialDate.Value.Year, InitialDate.Value.Month, FinalDate.Value.Year, FinalDate.Value.Month, "1");
                }
                List<SalesProjection> tempProjection2 = new();
                foreach (var s in subsidiaries)
                {
                    foreach (var d in divisions)
                    {
                        //var item = tempProjection.FirstOrDefault(x => x.Subsidiary.ToLower() == s.Name.ToLower() & x.Division.ToLower() == d.ToLower());
                        //if (item == null)
                        //{
                        //    tempProjection2.Add(new SalesProjection(0, ToTitle(s.Name), d, 0, 0, 0));
                        //}
                        //else
                        //{
                        //    tempProjection2.Add(new SalesProjection(item));
                        //}
                        decimal amount = (from x in tempProjection where x.Subsidiary.ToLower() == s.Name.ToLower() & x.Division.ToLower() == d.ToLower() select x.Amount).Sum();
                        tempProjection2.Add(new SalesProjection(0, ToTitle(s.Name), d, 0, 0, amount));
                    }
                }
                var projection = tempProjection2.Select(x => new { x.Subsidiary, Division = x.Division.Substring(0, 1), total = x.Amount });

                return Ok(new { message, stock, sales, salesToday, authorized, opens, projection });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Ok(new { message });
        }

        [HttpGet("billedperiod")]
        public IActionResult BilledPeriod(string Subsidiary, string Division, DateTime? InitialDate, DateTime? FinalDate)
        {
            string message = "";
            try
            {
                BCA.Note bcNote = new();
                List<Field> filters = new() { new Field("LOWER(Subsidiary)", Subsidiary.ToLower()) };
                if (InitialDate.HasValue)
                {
                    filters.Add(new Field("DocDate", InitialDate.Value.ToString("yyyy-MM-dd"), Operators.HigherOrEqualThan));
                }
                if (FinalDate.HasValue)
                {
                    filters.Add(new Field("DocDate", FinalDate.Value.ToString("yyyy-MM-dd"), Operators.LowerOrEqualThan));
                }
                CompleteFilters(ref filters);
                var notes = bcNote.ListBySection(Division, filters, "1");
                var items = notes.Select(x => new
                {
                    x.Warehouse,
                    NoteNumber = x.DocNumber,
                    docNumber = !string.IsNullOrEmpty(x.OrderNumber) ? int.Parse(x.OrderNumber) : 0,
                    Date = x.DocDate,
                    x.ClientName,
                    Seller = x.SellerName,
                    x.Total,
                    Margin = x.TaxlessTotal > 0 ? 100 * (x.Margin / x.TaxlessTotal) : 0,
                });
                return Ok(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Ok(new { message });
        }

        [HttpGet("openovs")]
        public IActionResult OpenOVs(string Subsidiary, string Division, bool Authorized)
        {
            string message = "";
            try
            {
                BCB.DivisionConfig bcDivConfig = new();
                IEnumerable<BEB.DivisionConfig> lstConfigItems = bcDivConfig.List("Type, Name");
                string lines = string.Join(",", (from i in lstConfigItems where i.Type == "M" select $"'{i.Name.ToLower()}'").ToArray());
                string sellers = string.Join(",", (from i in lstConfigItems where i.Type == "E" select $"'{i.Name.ToLower()}'").ToArray());

                BCA.Order bcOrder = new();
                List<Field> filters = new() { new Field("Subsidiary", Subsidiary), new Field("OpenAmount", 0, Operators.HigherThan) }, innerFilters = new List<Field>();
                if (Authorized)
                {
                    filters.AddRange(new[] { new Field("State", "Abierto"), new Field("Authorized", "Y"), new Field("Correlative", "NO PROCESAR", Operators.NotLikes) });
                }
                if (Division == "C")
                {
                    filters.Add(new Field("LOWER(IFNULL(SellerCode, ''))", sellers, Operators.NotIn));
                    innerFilters.Add(new Field("LOWER(IFNULL(U_LINEA, ''))", lines, Operators.NotIn));
                }
                else if (Division == "E")
                {
                    filters.Add(new Field("LOWER(IFNULL(SellerCode, ''))", sellers, Operators.In));
                }
                else if (Division == "M")
                {
                    innerFilters.Add(new Field("LOWER(IFNULL(U_LINEA, ''))", lines, Operators.In));
                }
                CompleteFilters(ref filters);
                var orders = bcOrder.List4(filters, innerFilters, "1");

                var items = from o in orders
                            select new
                            {
                                o.Warehouse,
                                o.DocNumber,
                                Date = o.DocDate,
                                o.ClientOrder,
                                o.ClientName,
                                Seller = o.SellerName,
                                o.Total,
                                o.OpenAmount,
                                Authorized = o.Authorized == "Y",
                                Complete = o.NonCompleteItem == 0
                            };
                return Ok(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Ok(new { message });
        }

        [HttpGet("stockdetail")]
        public IActionResult StockDetail(string Subsidiary, string Division)
        {
            string message = "";
            try
            {
                BCB.DivisionConfig bcDivConfig = new();
                IEnumerable<BEB.DivisionConfig> lstConfigItems = bcDivConfig.List("Type, Name");
                var lines = (from i in lstConfigItems where i.Type == "M" select i.Name).ToList();

                BCA.Resume bcResume = new();
                var stock = bcResume.ResumeStock(lines);
                var items = from i in stock
                            where i.Subsidiary.ToLower() == Subsidiary.ToLower() & (Division != "M" || i.Division == "M")
                            group i by i.Warehouse into g
                            select new { Warehouse = g.Key, Total = g.Sum(x => x.Total) };

                return Ok(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Ok(new { message });
        }

        [HttpGet("getclientkbytes")]
        public IActionResult GetClientKbytes(string CardCode)
        {
            string message = "";
            try
            {
                var (beClient, years, claimedPoints, availablePoints) = GetItems(CardCode);
                return Ok(new { message, item = new { years = years.Select(x => new { x.Year, x.Amount, x.Points, Status = x.Status.Name }), claimedPoints, availablePoints } });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Ok(new { message });
        }

        [HttpGet("getkbytenotes")]
        public IActionResult GetKbyteNotes(string CardCode, int Year)
        {
            string message = "";
            try
            {
                BCK.StatusDetail bcDetail = new BCK.StatusDetail();
                var details = bcDetail.ListNotes(CardCode, Year, "Id", BEK.relStatusDetail.Note, BEK.relStatusDetail.Status, BEK.relStatusDetail.StatusUsed, BEK.relClientNote.ClientNoteDetails, BEK.relClientNoteDetail.Product);

                var items = details.Select(x => new
                {
                    x.Note.Id,
                    x.Note.Subsidiary,
                    x.Note.Number,
                    Date = x.Note.Date,
                    x.Note?.Amount,
                    Status = x.Status.Name,
                    StatusUsed = x.StatusUsed.Name,
                    x.Points,
                    x.ExtraPoints,
                    x.ExtraPointsPeriod,
                    x.AcceleratorPeriod,
                    ItemPoints = x.Points + x.ExtraPoints + x.ExtraPointsPeriod,
                    x.TotalPoints,
                    x.TotalAmount,
                    items = x.Note.ListClientNoteDetails?.Where(i => i.AcceleratedQuantity > 0).Select(i => new { i.Product.ItemCode, i.Product.Name, i.Product.Line, i.Quantity, i.Total, i.AcceleratedQuantity, i.AcceleratedTotal, i.Accelerator, i.ExtraPoints })
                });
                return Ok(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Ok(new { message });
        }

        [HttpGet("getawardsclaimed")]
        public IActionResult GetAwardsClaimed(string CardCode)
        {
            string message = "";
            try
            {
                BCK.ClaimedAward bcAwards = new();
                List<Field> filters = new() { new Field("CardCode", CardCode) };
                IEnumerable<BEK.ClaimedAward> awards = bcAwards.List(filters, "ClaimDate DESC", BEK.relClaimedAward.Award);
                var items = awards.Select(x => new { award = x.Award.Name, x.Quantity, x.ClaimDate, x.Points });
                return Ok(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Ok(new { message });
        }

        #endregion

        #region Private Methods

        private IEnumerable<BEA.StateAccountItem> GetResults(string CardCode)
        {
            IEnumerable<BEA.StateAccountItem> lstItems = Enumerable.Empty<BEA.StateAccountItem>();
            BCA.StateAccount bcAccount = new();
            List<Field> filters = new();
            if (!string.IsNullOrEmpty(CardCode))
            {
                filters.Add(new Field { Name = "ClientCode", Value = CardCode });
            }
            CompleteFilters(ref filters);

            lstItems = bcAccount.List(filters, "CAST(DocDate AS DATE)");
            foreach (var i in lstItems)
            {
                i.ClientName = $"{i.ClientCode} - {i.ClientName}";
                i.PercetageMargin = (i.Margin / (i.TaxlessToral != 0 ? i.TaxlessToral : 1)) * 100;
                i.Header = i.Header?.Replace("\r", "\r") ?? "";
                i.Footer = i.Footer?.Replace("\r", "\r") ?? "";
                i.SellerName = ToTitle(i.SellerName ?? "");
                i.Warehouse = ToTitle(i.Warehouse ?? "");
            }
            lstItems = from i in lstItems orderby i.DocDate, i.DocNum select i;
            return lstItems;
        }

        private (BEA.Client, List<BEK.ClientStatus>, long, long) GetItems(string CardCode)
        {
            BEA.Client client;
            IEnumerable<BEK.ClientStatus> years;
            long availablePoints = 0, pointsClaimed = 0, points;

            BCA.Client bcClient = new();
            client = bcClient.Search(CardCode) ?? new BEA.Client();

            BCK.ClientStatus bcStatus = new();
            List<Field> filters = new() { new Field("CardCode", CardCode) };
            years = bcStatus.List(filters, "Year DESC", BEK.relClientStatus.Status);

            BCK.ClaimedAward bcAwards = new();
            IEnumerable<BEK.ClaimedAward> awards = bcAwards.List(filters, "1", BEK.relClaimedAward.Award);
            pointsClaimed = awards?.Count() > 0 ? awards.Sum(x => x.Points) : 0;

            var calculated = bcStatus.ListCalculatedByYear(CardCode);
            //foreach (var item in calculated)
            //{
            //    var year = years.FirstOrDefault(x => x.Year == item.Year);
            //    if (year != null)
            //    {
            //        year.Amount = item.Amount;
            //        year.Points = Math.Round(item.Points);
            //    }
            //}
            foreach (var item in years)
            {
                var year = calculated.FirstOrDefault(x => x.Year == item.Year);
                if (year == null)
                {
                    calculated.Add(new BEK.ClientStatus { Year = item.Year, Amount = item.Amount, Points = item.Points, Status = item.Status });
                }
                else
                {
                    year.Status = item.Status;
                }
            }

            points = (long)calculated?.Sum(x => (long)x.Points);
            availablePoints = points - pointsClaimed;
            calculated = (from x in calculated orderby x.Year descending select x).ToList();
            return (client, calculated, pointsClaimed, availablePoints);
        }

        #endregion

    }
}