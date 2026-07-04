using ProductCatalog.Application.Common;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Features.Products.Requests;
using ProductCatalog.Domain.Common;
using ProductCatalog.Domain.Enums;
using ProductCatalog.Domain.Interfaces;

namespace ProductCatalog.Application.Features.Products.Handlers
{
    public class SearchProductsHandler : RequestHandler<SearchProductsRequest, IReadOnlyList<ProductListItemDto>>
    {
        private readonly IProductRepository productRepository;
        public SearchProductsHandler(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        protected override async Task<Result<IReadOnlyList<ProductListItemDto>>> HandleRequest(SearchProductsRequest request)
        {
            if(string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                return Result<IReadOnlyList<ProductListItemDto>>.Failure(ErrorCode.InvalidInput, "Search request cannot be empty.");
            }

            var products = await productRepository.SearchAsync(request.SearchTerm);
            
            var searchedProducts = products.Select(p => p.ToListItemDto()).ToList();

            return Result<IReadOnlyList<ProductListItemDto>>.Success(searchedProducts);
        }
    }
}
