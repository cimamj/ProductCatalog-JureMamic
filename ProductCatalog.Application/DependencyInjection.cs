using Microsoft.Extensions.DependencyInjection;
using ProductCatalog.Application.Features.Auth.Handlers;
using ProductCatalog.Application.Features.Products.Handlers;

namespace ProductCatalog.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<GetAllProductsHandler>();
        services.AddScoped<GetProductByIdHandler>();
        services.AddScoped<FilterProductsHandler>();
        services.AddScoped<SearchProductsHandler>();
        services.AddScoped<LoginHandler>();

        return services;
    }
}