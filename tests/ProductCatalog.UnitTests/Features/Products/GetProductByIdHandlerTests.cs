using Moq;
using ProductCatalog.Application.Features.Products.Handlers;
using ProductCatalog.Application.Features.Products.Requests;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Enums;
using ProductCatalog.Domain.Interfaces;

namespace ProductCatalog.UnitTests.Features.Products
{
    public class GetProductByIdHandlerTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock = new();
        private readonly GetProductByIdHandler _handler;

        public GetProductByIdHandlerTests()
        {
            _handler = new GetProductByIdHandler(_productRepositoryMock.Object);
        }

        [Fact]
        public async Task HandleRequest_ReturnsNotFound_WhenProductDoesNotExist()
        {
            _productRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Product?)null);

            var result = await _handler.HandleAsync(new GetProductByIdRequest { Id = 99 });

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NotFound, result.ErrorCode);
        }

        [Fact]
        public async Task HandleRequest_ReturnsProductDetail_WhenProductExists()
        {
            var category = new Category { Id = 1, ExternalSlug = "beauty", Name = "Beauty" };
            var product = new Product
            {
                Id = 1,
                Title = "Lipstick",
                Description = "A lipstick",
                CategoryId = 1,
                Category = category,
                Price = 12,
                Rating = 4.5,
                Stock = 10,
                Brand = "BrandX",
                Thumbnail = "thumb.jpg",
                Images = new List<string> { "img1.jpg" }
            };

            _productRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

            var result = await _handler.HandleAsync(new GetProductByIdRequest { Id = 1 });

            Assert.True(result.IsSuccess);
            Assert.Equal("Lipstick", result.Value!.Title);
            Assert.Equal("Beauty", result.Value.Category);
        }
    }
}
