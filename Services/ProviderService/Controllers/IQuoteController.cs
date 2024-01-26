using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using BCA = BComponents.SAP;
using BCP = BComponents.Product;
using BEA = BEntities.SAP;
using BEP = BEntities.Product;
using BEntities.Filters;
using System.IO;
using Microsoft.Extensions.Configuration;
using ProviderService.Models;
using ProviderService.Interfaces;

namespace ProviderService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class IQuoteController : ControllerBase
    {
        #region Private Variables

        private readonly ICustomLogger _logger;

        #endregion

        #region Constructors

        public IQuoteController(ICustomLogger logger)
        {
            _logger = logger;
        }

        #endregion

        #region GETs

        [HttpGet("NotificateRequest")]
        public async Task<IActionResult> NotificateRequest(int rqid)
        {
            StringBuilder sb = new();
            string message = "";
            try
            {
                sb.AppendLine("iQuote, Registro de Solicitudes");
                sb.AppendLine($"Se ha ingresado una petición con Id = {rqid}");

                var client = new IQuoteReference.I_iQuoteAPIClient(IQuoteReference.I_iQuoteAPIClient.EndpointConfiguration.WSHttpBinding_I_iQuoteAPI);
                var data = await client.getPendingPnaRequestAsync(rqid);

                if (data != null)
                {
                    sb.AppendLine("Objeto recibido: " + Newtonsoft.Json.JsonConvert.SerializeObject(data));
                    if (data.distSkus?.Length > 0)
                    {
                        BCA.Product bcProductSAP = new();
                        string strCodes = string.Join(",", data.distSkus.Select(i => $"'{i.ToLower()}'"));
                        List<Field> lstFilter = new()
                        {
                            new Field("LOWER(SuppCatNum)", strCodes, Operators.In),
                            new Field("LOWER(ItemCode)", strCodes, Operators.In),
                            new Field(LogicalOperators.Or)
                        };
                        IEnumerable<BEA.Product> lstSAPProducts = bcProductSAP.ListFactoryCodes(lstFilter);
                        sb.AppendLine($"Se ha consultado códigos para los siguientes skus: {strCodes}");
                        sb.AppendLine($"Devolvió lo siguiente: {Newtonsoft.Json.JsonConvert.SerializeObject(lstSAPProducts)}");

                        List<IQuoteReference.clsIquoteAPIclsPnaItem> items = new();
                        if (lstSAPProducts?.Count() > 0)
                        {
                            BCA.ProductStock bcInvetory = new();
                            strCodes = string.Join(",", (from p in lstSAPProducts select $"'{p.Code.ToLower()}'").ToArray());
                            lstFilter = new List<Field>
                            {
                                new Field("LOWER(ItemCode)", strCodes, Operators.In),
                                new Field("LOWER(Subsidiary)", "santa cruz"),
                                new Field("LOWER(Warehouse)", "'santa cruz', 'la paz'", Operators.In),
                                new Field("Available2", 0, Operators.HigherThan)
                            };
                            int intCount = lstFilter.Count - 1;
                            for (int i = 0; i < intCount; i++)
                            {
                                lstFilter.Add(new Field { LogicalOperator = LogicalOperators.And });
                            }
                            IEnumerable<BEA.ProductStock> lstInventory = bcInvetory.ListShort(lstFilter, "1");
                            sb.AppendLine($"Inventario obtenido: {Newtonsoft.Json.JsonConvert.SerializeObject(lstInventory)}");

                            BCP.Product bcProduct = new();
                            lstFilter = new List<Field> { new Field("LOWER(ItemCode)", strCodes, Operators.In) };
                            IEnumerable<BEP.Product> lstProducts = bcProduct.List(lstFilter, "1");

                            BCP.PriceExternalSite bcPrice = new();
                            lstFilter = new() { new Field("IdProduct", string.Join(",", lstProducts.Select(x => x.Id)), Operators.In) };
                            IEnumerable<BEP.PriceExternalSite> lstPrices = lstProducts?.Count() > 0 ? bcPrice.List(lstFilter, "1") : Enumerable.Empty<BEP.PriceExternalSite>();
                            sb.AppendLine($"Precios obtenidos: {Newtonsoft.Json.JsonConvert.SerializeObject(lstPrices)}");


                            List<IQuoteReference.clsIquoteAPIclsShipment> lstStock;
                            foreach (var item in lstPrices)
                            {
                                if (item.Price > 0)
                                {
                                    var beProduct = lstProducts.FirstOrDefault(p => p.Id == item.IdProduct);
                                    var beSAPProduct = lstSAPProducts.FirstOrDefault(p => p.Code.ToLower() == beProduct.ItemCode.ToLower());
                                    var lstItemInventory = lstInventory.Where(i => i.ItemCode.ToLower() == beProduct.ItemCode.ToLower());
                                    lstStock = (from i in lstItemInventory
                                                group i by i.ItemCode into g
                                                select new IQuoteReference.clsIquoteAPIclsShipment { quantity = g.Sum(p => p.Available2), arrival = DateTime.Now.AddMonths(-1), price = item.Price }).ToList();
                                    if ((lstStock == null || lstStock.Count == 0) & item.ShowAlways)
                                    {
                                        lstStock = new List<IQuoteReference.clsIquoteAPIclsShipment>
                                    {
                                        new IQuoteReference.clsIquoteAPIclsShipment { quantity = 1, arrival = DateTime.Now.AddMonths(-1), price = item.Price }
                                    };
                                    }
                                    var price = new IQuoteReference.clsIquoteAPIclsPnaItem { price = (float)item.Price, message = item.Commentaries, status = "", SKU = beSAPProduct.Code, stock = lstStock.ToArray() };
                                    items.Add(price);
                                }
                            }
                        }

                        message = await client.sendPnaResponseAsync(rqid, items.ToArray());
                        sb.AppendLine($"Se devolvieron los siguientes datos: {Newtonsoft.Json.JsonConvert.SerializeObject(items)}");
                        sb.AppendLine($"Con el siguiente mensaje de retorno: {message}");
                    }
                    else
                    {
                        message = $"The Id = {rqid} does not return any skus associated";
                    }
                }
                else
                {
                    message = $"The Id = {rqid} does not return any data";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                Exception e = ex.InnerException;
                while (e != null)
                {
                    message += Environment.NewLine + e.Message;
                    e = e.InnerException;
                }
                sb.AppendLine("Se ha producido el siguiente error: " + message + Environment.NewLine + ex.StackTrace);
                _ = _logger.ReportErrorByMail(sb.ToString());
            }
            finally
            {
                _logger.Log(sb.ToString(), "iQuote");
            }
            return Ok(new { message });
        }

        #endregion
    }
}
