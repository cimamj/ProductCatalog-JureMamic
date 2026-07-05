using Moq;
using ProductCatalog.Application.Features.Products.Handlers;
using ProductCatalog.Application.Features.Products.Requests;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Interfaces;

namespace ProductCatalog.UnitTests.Features.Products
{
    public class SearchProductsHandlerTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock = new();
        private readonly Mock<ICacheService> _cacheServiceMock = new();
        private readonly SearchProductsHandler _handler;

        public SearchProductsHandlerTests()
        {
            _cacheServiceMock
                .Setup(c => c.GetOrCreateAsync(
                    It.IsAny<string>(),
                    It.IsAny<Func<Task<IReadOnlyList<Product>>>>(),
                    It.IsAny<TimeSpan?>()))
                .Returns((string _, Func<Task<IReadOnlyList<Product>>> factory, TimeSpan? _) => factory());

            _handler = new SearchProductsHandler(_productRepositoryMock.Object, _cacheServiceMock.Object);
        }

        [Fact]
        public async Task HandleRequest_ReturnsItemsAndTotal_WhenRepositoryHasMatches()
        {
            var category = new Category { Id = 1, ExternalSlug = "beauty", Name = "Beauty" };
            var products = new List<Product>
            {
                new() { Id = 1, Title = "Lipstick", Price = 12, CategoryId = 1, Category = category }
            };

            _productRepositoryMock
                .Setup(r => r.SearchAsync("lip"))
                .ReturnsAsync(products);

            var request = new SearchProductsRequest { SearchTerm = "lip" };

            var result = await _handler.HandleAsync(request);

            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.Value!.Total);
            Assert.Equal("Lipstick", result.Value.Items[0].Title);
        }
    }
}
