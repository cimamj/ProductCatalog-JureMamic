namespace ProductCatalog.Application.Common
{
    public static class CacheKeys
    {
        public static string ProductsFilter(string? category, decimal? minPrice, decimal? maxPrice)
            => $"products:filter:{category}:{minPrice}:{maxPrice}";

        public static string ProductsSearch(string searchTerm)
            => $"products:search:{searchTerm.ToLowerInvariant()}";
    }
}
