namespace ProductCatalog.Application.DTOs
{
    public class ProductListResultDto
    {
        public IReadOnlyList<ProductListItemDto> Items { get; set; } = new List<ProductListItemDto>();
        public int Total { get; set; }
    }
}
