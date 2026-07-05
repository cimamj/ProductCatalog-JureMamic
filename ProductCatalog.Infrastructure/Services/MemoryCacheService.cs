using Microsoft.Extensions.Caching.Memory;
using ProductCatalog.Application.Interfaces;

namespace ProductCatalog.Infrastructure.Services
{
    public class MemoryCacheService : ICacheService
    {
        private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(5);
        private readonly IMemoryCache _cache;

        public MemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
        {
            if (_cache.TryGetValue(key, out T? cached) && cached is not null)
                return cached;

            var value = await factory();

            _cache.Set(key, value, expiration ?? DefaultExpiration);

            return value;
        }
    }
}