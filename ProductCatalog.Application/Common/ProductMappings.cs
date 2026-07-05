using ProductCatalog.Application.DTOs;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.Common
{
    public static class ProductMappings
    {
        public static ProductListItemDto ToListItemDto(this Product product)
        {
            return new ProductListItemDto
            {
                Id = product.Id,
                Title = product.Title,
                Description = product.Description.Length > 100
                ? product.Description[..100]
                : product.Description,
                Price = product.Price,
                Thumbnail = product.Thumbnail,
                Category = product.Category?.Name ?? string.Empty
            };
        }
    }
}
