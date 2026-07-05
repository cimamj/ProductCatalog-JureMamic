namespace ProductCatalog.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public int ExternalId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public decimal Price { get; set; }
        public double Rating { get; set; }
        public int Stock { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = string.Empty;
        public List<string> Images { get; set; } = new();
    }
}
