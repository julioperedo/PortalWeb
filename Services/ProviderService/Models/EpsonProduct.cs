namespace ProviderService.Models
{
    public class EpsonProduct
    {
        public string Id { get; set; }
        public string Brand { get; set; }
        public string MPN { get; set; }
        public int GTIN { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
    }
}
