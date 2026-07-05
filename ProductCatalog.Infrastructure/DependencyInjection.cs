using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Domain.Interfaces;
using ProductCatalog.Infrastructure.ExternalSource;
using ProductCatalog.Infrastructure.Persistence;
using ProductCatalog.Infrastructure.Persistence.Repositories;
using ProductCatalog.Infrastructure.Services;

namespace ProductCatalog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ProductCatalogDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        services.AddHttpClient<DummyJsonService>(client =>
            client.BaseAddress = new Uri(configuration["DummyJson:BaseUrl"]!));
        services.AddScoped<IAuthService>(sp => sp.GetRequiredService<DummyJsonService>());
        services.AddScoped<CatalogSyncService>();
        services.AddHostedService<CatalogSyncBackgroundService>();

        services.AddScoped<IJwtService, JwtService>();

        services.AddMemoryCache();
        services.AddSingleton<ICacheService, MemoryCacheService>();

        return services;
    }
}