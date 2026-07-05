using ProductCatalog.Application.Common;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Features.Products.Requests;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Domain.Common;
using ProductCatalog.Domain.Interfaces;

namespace ProductCatalog.Application.Features.Products.Handlers
{
    public class FilterProductsHandler : RequestHandler<FilterProductsRequest, ProductListResultDto>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICacheService _cacheService;
        public FilterProductsHandler(IProductRepository productRepository, ICacheService cacheService)
        {
            _productRepository = productRepository;
            _cacheService = cacheService;
        }

        protected override async Task<Result<ProductListResultDto>> HandleRequest(FilterProductsRequest request)
        {
            if(request.MinPrice < 0 || request.MaxPrice < 0 || request.MinPrice > request.MaxPrice)
            {
                return Result<ProductListResultDto>.Failure(Domain.Enums.ErrorCode.InvalidInput, "Invalid price range.");
            }

            var cacheKey = CacheKeys.ProductsFilter(request.Category, request.MinPrice, request.MaxPrice);

            var products = await _cacheService.GetOrCreateAsync(cacheKey,
                () => _productRepository.FilterAsync(request.Category, request.MinPrice, request.MaxPrice));

            var filteredProductsDtos = products.Select(p => p.ToListItemDto()).ToList();

            return Result<ProductListResultDto>.Success(new ProductListResultDto
            {
                Items = filteredProductsDtos,
                Total = filteredProductsDtos.Count
            });
        }
    }
}
