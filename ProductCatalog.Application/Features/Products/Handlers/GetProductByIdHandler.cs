using ProductCatalog.Application.Common;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Features.Products.Requests;
using ProductCatalog.Domain.Common;
using ProductCatalog.Domain.Enums;
using ProductCatalog.Domain.Interfaces;

namespace ProductCatalog.Application.Features.Products.Handlers
{
    public class GetProductByIdHandler : RequestHandler<GetProductByIdRequest, ProductDetailDto>
    {
        private readonly IProductRepository _productRepository;
        
        public GetProductByIdHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }   

        protected override async Task<Result<ProductDetailDto>> HandleRequest(GetProductByIdRequest request)
        {
            var product = await _productRepository.GetByIdAsync(request.Id);

            if (product == null)
            {
                return Result<ProductDetailDto>.Failure(ErrorCode.NotFound, $"Product with ID {request.Id} not found.");
            }

            return Result<ProductDetailDto>.Success(new ProductDetailDto
            {
                Id = product.Id,
                Title = product.Title,
                Description = product.Description,
                Category = product.Category.Name,
                Price = product.Price,
                Rating = product.Rating,
                Stock = product.Stock,
                Brand = product.Brand,
                Thumbnail = product.Thumbnail,
                Images = product.Images
            });
        }
    }
}
