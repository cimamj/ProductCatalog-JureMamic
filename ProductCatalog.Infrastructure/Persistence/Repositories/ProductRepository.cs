using Microsoft.EntityFrameworkCore;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Interfaces;

namespace ProductCatalog.Infrastructure.Persistence.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductCatalogDbContext _context;

        public ProductRepository(ProductCatalogDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IReadOnlyList<Product>> FilterAsync(string? category, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.Products.Include(p => p.Category).AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(p => p.Category.ExternalSlug == category);

            if (minPrice is not null)
                query = query.Where(p => p.Price >= minPrice);

            if (maxPrice is not null)
                query = query.Where(p => p.Price <= maxPrice);

            return await query.ToListAsync();
        }

        public async Task<IReadOnlyList<Product>> SearchAsync(string? searchTerm)
        {
            return await _context.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .Where(p => EF.Functions.Like(p.Title, $"%{searchTerm}%") ||
                            EF.Functions.Like(p.Description, $"%{searchTerm}%") ||
                            EF.Functions.Like(p.Brand, $"%{searchTerm}%")
                )
                .ToListAsync();
        }

        public async Task UpsertRangeAsync(IEnumerable<Product> productsFromService)
        {
            var productsFromServiceList = productsFromService.ToList();
            var externalIdsFromService = productsFromServiceList.Select(p => p.ExternalId).ToList();

            var productsInDatabase = await _context.Products
                .Where(p => externalIdsFromService.Contains(p.ExternalId))
                .ToListAsync();

            var productsToInsert = new List<Product>();

            foreach (var productFromService in productsFromServiceList)
            {
                var matchingProductInDatabase = productsInDatabase
                    .FirstOrDefault(p => p.ExternalId == productFromService.ExternalId);

                if (matchingProductInDatabase != null)
                {
                    matchingProductInDatabase.Title = productFromService.Title;
                    matchingProductInDatabase.Description = productFromService.Description;
                    matchingProductInDatabase.CategoryId = productFromService.CategoryId;
                    matchingProductInDatabase.Price = productFromService.Price;
                    matchingProductInDatabase.Rating = productFromService.Rating;
                    matchingProductInDatabase.Stock = productFromService.Stock;
                    matchingProductInDatabase.Brand = productFromService.Brand;
                    matchingProductInDatabase.Thumbnail = productFromService.Thumbnail;
                    matchingProductInDatabase.Images = productFromService.Images;
                }
                else
                {
                    productsToInsert.Add(productFromService);
                }
            }

            if (productsToInsert.Count > 0)
            {
                _context.Products.AddRange(productsToInsert);
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteMissingAsync(IEnumerable<int> currentExternalIds)
        {
            var externalIdsFromService = currentExternalIds.ToList();

            var toRemove = await _context.Products
                .Where(p => !externalIdsFromService.Contains(p.ExternalId))
                .ToListAsync();

            if (toRemove.Count == 0)
                return;

            _context.Products.RemoveRange(toRemove);
            await _context.SaveChangesAsync();
        }
    }
}
