using BEntities.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ProviderService.Interfaces;
using ProviderService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using BCA = BComponents.SAP;
using BCL = Bukimedia.PrestaSharp.Factories;
using BCP = BComponents.Product;
using BEA = BEntities.SAP;
using BEL = Bukimedia.PrestaSharp.Entities;
using BEP = BEntities.Product;

namespace ProviderService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EpsonController : ControllerBase
    {
        #region Global Variables

        private readonly string _BaseURL = "";
        private readonly string _Account = "";
        private readonly string _Password = "";
        private readonly ICustomLogger _logger;

        #endregion

        #region Constructors

        public EpsonController(IConfiguration configuration, ICustomLogger logger)
        {
            var config = configuration.GetSection("PrestaSettings").Get<PrestaSettings>();
            _BaseURL = config.BaseUrl;
            _Account = config.Account;
            _logger = logger;
        }

        #endregion

        #region GETs

        [HttpGet("Catalog")]
        public IActionResult Catalog()
        {
            string message = "";
            try
            {
                BCP.Product bcProduct = new();
                IEnumerable<BEP.Product> lstProducts = bcProduct.ListEpson();
                BCA.Product bcSAPProduct = new();
                List<Field> lstFilter = new() { new Field("ItemCode", string.Join(",", lstProducts.Select(p => $"'{p.ItemCode}'")), Operators.In) };
                IEnumerable<BEA.Product> lstSAPProducts = bcSAPProduct.ListFactoryCodes(lstFilter);

                //traer datos de PrestaShop
                BCL.ManufacturerFactory bcWebLine = new(_BaseURL, _Account, _Password);
                List<BEL.manufacturer> manufacturers = bcWebLine.GetAll();
                List<BEL.product> products = new();
                BCL.ProductFactory bcWebProduct = new(_BaseURL, _Account, _Password);
                foreach (var item in manufacturers)
                {
                    if (item.name.ToLower().Contains("epson"))
                    {
                        Dictionary<string, string> filter = new() { { "id_manufacturer", item.id.ToString() } };
                        products.AddRange(bcWebProduct.GetByFilter(filter, null, null));
                    }
                }
                string strURL = "http://www.dmc.bo/index.php?id_product={0}&controller=product";
                var lstItems = from p in lstProducts
                               select new EpsonProduct
                               {
                                   Id = p.ItemCode,
                                   Brand = "Epson",
                                   MPN = (from s in lstSAPProducts where s.Code.ToLower() == p.ItemCode.ToLower() select s.FactoryCode).FirstOrDefault(),
                                   GTIN = 0,
                                   Name = p.Name,
                                   Url = string.Format(strURL, (from i in products where i.reference.ToLower() == p.ItemCode.ToLower() select i.id).FirstOrDefault()),
                                   Category = p.Category + "|" + p.SubCategory,
                                   Price = 0m
                               };
                return Ok(lstItems);
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
                _ = _logger.ReportErrorByMail($"Se ha producido el siguiente error al generar el catálogo de EPSON: {Environment.NewLine}{message}{Environment.NewLine}{ex.StackTrace}");
            }            
            return Ok(new { message });
        }

        #endregion

        #region Private Methods

        #endregion
    }
}
