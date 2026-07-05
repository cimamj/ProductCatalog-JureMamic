using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Domain.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IReadOnlyList<Category>> GetAllAsync();
        Task UpsertRangeAsync(IEnumerable<Category> categories);
        Task DeleteMissingAsync(IEnumerable<string> currentExternalSlugs);
    }
}
