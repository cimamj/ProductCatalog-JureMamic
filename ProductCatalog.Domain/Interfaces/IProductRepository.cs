using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Domain.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IReadOnlyList<Product>> FilterAsync(string? category, decimal? minPrice, decimal? maxPrice);
        Task<IReadOnlyList<Product>> SearchAsync(string searchTerm);
    }
}
