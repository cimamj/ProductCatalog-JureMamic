using Microsoft.EntityFrameworkCore;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Interfaces;

namespace ProductCatalog.Infrastructure.Persistence.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ProductCatalogDbContext _context;

        public CategoryRepository(ProductCatalogDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Category>> GetAllAsync()
        {
            return await _context.Categories.AsNoTracking().ToListAsync();
        }

        public async Task UpsertRangeAsync(IEnumerable<Category> categoriesFromService)
        {
            var categoriesFromServiceList = categoriesFromService.ToList();
            var externalSlugsFromService = categoriesFromServiceList.Select(c => c.ExternalSlug).ToList();

            var categoriesInDatabase = await _context.Categories
                .Where(c => externalSlugsFromService.Contains(c.ExternalSlug))
                .ToListAsync();

            var categoriesToInsert = new List<Category>();

            foreach (var categoryFromService in categoriesFromServiceList)
            {
                var matchingCategoryInDatabase = categoriesInDatabase
                    .FirstOrDefault(c => c.ExternalSlug == categoryFromService.ExternalSlug);

                if (matchingCategoryInDatabase != null)
                {
                    matchingCategoryInDatabase.Name = categoryFromService.Name;
                }
                else
                {
                    categoriesToInsert.Add(categoryFromService);
                }
            }

            if (categoriesToInsert.Count > 0)
            {
                _context.Categories.AddRange(categoriesToInsert);
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteMissingAsync(IEnumerable<string> currentExternalSlugs)
        {
            var externalSlugsFromService = currentExternalSlugs.ToList();

            var toRemove = await _context.Categories
                .Where(c => !externalSlugsFromService.Contains(c.ExternalSlug))
                .ToListAsync();

            if (toRemove.Count == 0)
                return;

            _context.Categories.RemoveRange(toRemove);
            await _context.SaveChangesAsync();
        }
    }
}
