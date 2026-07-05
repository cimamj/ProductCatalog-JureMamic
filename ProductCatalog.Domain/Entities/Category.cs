namespace ProductCatalog.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string ExternalSlug { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public List<Product> Products { get; set; } = new();
    }
}
