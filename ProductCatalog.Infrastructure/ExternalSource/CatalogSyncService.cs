using Microsoft.Extensions.Logging;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Interfaces;
using ProductCatalog.Infrastructure.DTOs;

namespace ProductCatalog.Infrastructure.ExternalSource
{
    public class CatalogSyncService
    {
        private readonly DummyJsonService _dummyJsonService;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<CatalogSyncService> _logger;

        public CatalogSyncService(
            DummyJsonService dummyJsonService,
            ICategoryRepository categoryRepository,
            IProductRepository productRepository,
            ILogger<CatalogSyncService> logger)
        {
            _dummyJsonService = dummyJsonService;
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task SyncAsync()
        {
            var categoryDtos = await SyncCategoriesAsync();
            var syncedProductCount = await SyncProductsAsync(categoryDtos);

            await _categoryRepository.DeleteMissingAsync(categoryDtos.Select(c => c.Slug));


            _logger.LogInformation(
                "Catalog sync complete: {CategoryCount} categories, {ProductCount} products, {UserCount} users.",
                categoryDtos.Count, syncedProductCount, 2);
        }

        private async Task<IReadOnlyList<DummyJsonCategoryDto>> SyncCategoriesAsync()
        {
            var categoryDtos = await _dummyJsonService.GetCategoriesAsync();

            await _categoryRepository.UpsertRangeAsync(categoryDtos.Select(dto => new Category
            {
                ExternalSlug = dto.Slug,
                Name = dto.Name
            }));

            return categoryDtos;
        }

        private async Task<int> SyncProductsAsync(IReadOnlyList<DummyJsonCategoryDto> categoryDtos)
        {
            var productDtos = await _dummyJsonService.GetAllProductsAsync();
            var categoryIdBySlug = (await _categoryRepository.GetAllAsync())
                .ToDictionary(c => c.ExternalSlug, c => c.Id);

            var knownProductDtos = new List<DummyJsonProductDto>();
            var products = new List<Product>();

            foreach (var dto in productDtos)
            {
                if (!categoryIdBySlug.TryGetValue(dto.Category, out var categoryId))
                {
                    _logger.LogWarning(
                        "Skipping product {ProductId} ('{Title}') - unknown category slug '{CategorySlug}'.",
                        dto.Id, dto.Title, dto.Category);
                    continue;
                }

                knownProductDtos.Add(dto);
                products.Add(new Product
                {
                    ExternalId = dto.Id,
                    Title = dto.Title,
                    Description = dto.Description,
                    CategoryId = categoryId,
                    Price = dto.Price,
                    Rating = dto.Rating,
                    Stock = dto.Stock,
                    Brand = dto.Brand,
                    Thumbnail = dto.Thumbnail,
                    Images = dto.Images
                });
            }

            await _productRepository.UpsertRangeAsync(products);

            await _productRepository.DeleteMissingAsync(productDtos.Select(p => p.Id));

            return knownProductDtos.Count;
        }


    }
}
