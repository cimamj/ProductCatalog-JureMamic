using Moq;
using ProductCatalog.Application.Features.Products.Handlers;
using ProductCatalog.Application.Features.Products.Requests;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Enums;
using ProductCatalog.Domain.Interfaces;

namespace ProductCatalog.UnitTests.Features.Products
{
    public class FilterProductsHandlerTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock = new();
        private readonly Mock<ICacheService> _cacheServiceMock = new();
        private readonly FilterProductsHandler _handler;

        public FilterProductsHandlerTests()
        {
            _cacheServiceMock
                .Setup(c => c.GetOrCreateAsync(
                    It.IsAny<string>(),
                    It.IsAny<Func<Task<IReadOnlyList<Product>>>>(),
                    It.IsAny<TimeSpan?>()))
                .Returns((string _, Func<Task<IReadOnlyList<Product>>> factory, TimeSpan? _) => factory());

            _handler = new FilterProductsHandler(_productRepositoryMock.Object, _cacheServiceMock.Object);
        }

        [Fact]
        public async Task HandleRequest_ReturnsFailure_WhenMinPriceGreaterThanMaxPrice()
        {
            var request = new FilterProductsRequest { MinPrice = 50, MaxPrice = 10 };

            var result = await _handler.HandleAsync(request);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.InvalidInput, result.ErrorCode);
        }

        [Fact]
        public async Task HandleRequest_ReturnsItemsAndTotal_WhenRepositoryHasMatches()
        {
            var category = new Category { Id = 1, ExternalSlug = "beauty", Name = "Beauty" };
            var products = new List<Product>
            {
                new() { Id = 1, Title = "Lipstick", Price = 12, CategoryId = 1, Category = category },
                new() { Id = 2, Title = "Perfume", Price = 30, CategoryId = 1, Category = category }
            };

            _productRepositoryMock
                .Setup(r => r.FilterAsync("beauty", 10, 50))
                .ReturnsAsync(products);

            var request = new FilterProductsRequest { Category = "beauty", MinPrice = 10, MaxPrice = 50 };

            var result = await _handler.HandleAsync(request);

            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value!.Total);
            Assert.Equal(2, result.Value.Items.Count);
        }
    }
}
