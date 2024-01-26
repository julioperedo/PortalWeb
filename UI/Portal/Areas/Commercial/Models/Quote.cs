using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Areas.Commercial.Models {
    public class Quote {
        public long Id { get; set; }

        [Required(ErrorMessage = "*")]
        public long IdSeller { get; set; }

        public string QuoteCode { get; set; }

        [Required(ErrorMessage = "*")]
        public System.DateTime QuoteDate { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "*")]
        [StringLength(10, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string CardCode { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "*")]
        [StringLength(250, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string CardName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "*")]
        [StringLength(250, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string ClientName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "*")]
        [StringLength(250, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string ClientMail { get; set; }

        public string Header { get; set; }

        public string Footer { get; set; }

        public List<QuoteDetail> Details { get; set; }

        public List<QuoteSent> SentMails { get; set; }

        public Quote() {
            Details = new List<QuoteDetail>();
            SentMails = new List<QuoteSent>();
        }

        public BEntities.Sales.Quote ToEntity() {
            BEntities.Sales.Quote beQuote = new BEntities.Sales.Quote {
                Id = Id, IdSeller = IdSeller, QuoteDate = QuoteDate, QuoteCode = QuoteCode, CardCode = CardCode, CardName = CardName, ClientName = ClientName, ClientMail = ClientMail, Header = Header, Footer = Footer,
                ListQuoteDetails = new List<BEntities.Sales.QuoteDetail>()
            };

            if (Details != null && Details.Count > 0) {
                foreach (var detail in Details) {
                    BEntities.Sales.QuoteDetail beDetail = new BEntities.Sales.QuoteDetail {
                        Id = detail.Id, IdProduct = detail.IdProduct, IdQuote = detail.IdQuote, ProductCode = detail.ProductCode, ProductName = detail.ProductName, ProductDescription = detail.ProductDescription,
                        ProductImageURL = detail.ProductImageURL, ProductLink = detail.ProductLink, ListQuoteDetailPricess = new List<BEntities.Sales.QuoteDetailPrices>()
                    };
                    if (detail.Prices != null && detail.Prices.Count > 0) {
                        foreach (var price in detail.Prices) {
                            BEntities.Sales.QuoteDetailPrices bePrice = new BEntities.Sales.QuoteDetailPrices {
                                Id = price.Id, IdDetail = price.IdDetail, IdSubsidiary = price.IdSubsidiary, Price = price.Price, Observations = price.Observations, Selected = price.Selected, Quantity = price.Quantity
                            };
                            beDetail.ListQuoteDetailPricess.Add(bePrice);
                        }
                    }

                    beQuote.ListQuoteDetails.Add(beDetail);
                }
            }

            return beQuote;
        }
    }

    public class QuoteDetail {

        public long Id { get; set; }
        public long IdQuote { get; set; }
        public long IdProduct { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string ProductImageURL { get; set; }
        public string ProductLink { get; set; }
        public List<QuoteDetailPrices> Prices { get; set; }
        public List<QuoteDetailStock> Stock { get; set; }
        public List<QuoteVolumePrices> Volume { get; set; }

        public QuoteDetail() {
            Prices = new List<QuoteDetailPrices>();
            Stock = new List<QuoteDetailStock>();
            Volume = new List<QuoteVolumePrices>();
        }

    }

    public class QuoteDetailPrices {

        public long Id { get; set; }
        public long IdDetail { get; set; }
        public long IdSubsidiary { get; set; }
        public string Subsidiary { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Observations { get; set; }
        public bool Selected { get; set; }

        public QuoteDetailPrices() {

        }
    }

    public class QuoteVolumePrices {
        //public long Id { get; set; }
        //public long IdDetail { get; set; }
        //public long IdProduct { get; set; }
        //public long IdSubsidiary { get; set; }
        public string Subsidiary { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Observations { get; set; }
    }

    public class QuoteDetailStock {
        public string Warehouse { get; set; }
        public int Available { get; set; }
        public int InTransit { get; set; }
    }

    public class QuoteSent {
        public string CardDeatil { get; set; }
        public string ClientDetail { get; set; }
        public string Date { get; set; }
    }

}
