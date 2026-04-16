using Microsoft.Extensions.DependencyInjection;

namespace WeatherStyler.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register application services only. Repository implementations are registered by the Infrastructure project.
        services.AddScoped<WeatherStyler.Application.Services.ClothingItemService>();
        services.AddScoped<WeatherStyler.Application.Services.IClothingItemService, WeatherStyler.Application.Services.ClothingItemService>();
        return services;
    }
}
