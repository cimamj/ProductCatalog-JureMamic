using ProductCatalog.Application.Common;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Features.Products.Requests;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Domain.Common;
using ProductCatalog.Domain.Interfaces;

namespace ProductCatalog.Application.Features.Products.Handlers
{
    public class SearchProductsHandler : RequestHandler<SearchProductsRequest, ProductListResultDto>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICacheService _cacheService;
        public SearchProductsHandler(IProductRepository productRepository, ICacheService cacheService)
        {
            _productRepository = productRepository;
            _cacheService = cacheService;
        }

        protected override async Task<Result<ProductListResultDto>> HandleRequest(SearchProductsRequest request)
        {
            var cacheKey = CacheKeys.ProductsSearch(string.IsNullOrWhiteSpace(request.SearchTerm) ? string.Empty : request.SearchTerm);

            var products = await _cacheService.GetOrCreateAsync(cacheKey,
                () => _productRepository.SearchAsync(request.SearchTerm));

            var searchedProducts = products.Select(p => p.ToListItemDto()).ToList();

            return Result<ProductListResultDto>.Success(new ProductListResultDto
            {
                Items = searchedProducts,
                Total = searchedProducts.Count
            });
        }
    }
}
