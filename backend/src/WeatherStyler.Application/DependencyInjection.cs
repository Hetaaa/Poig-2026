using Microsoft.Extensions.DependencyInjection;
// keep this project free of ASP.NET-specific DI helpers

namespace WeatherStyler.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register application services only. Repository implementations are registered by the Infrastructure project.
        services.AddScoped<WeatherStyler.Application.Services.ClothingItemService>();
        services.AddScoped<WeatherStyler.Application.Services.IClothingItemService, WeatherStyler.Application.Services.ClothingItemService>();
        services.AddScoped<WeatherStyler.Application.Services.LookupService>();
        services.AddScoped<WeatherStyler.Application.Services.ProgramVariableService>();
        // IHttpContextAccessor is registered by Infrastructure to avoid ASP.NET reference in this project
        // IUserService is registered by Infrastructure project
        // register WeatherService
        services.AddScoped<WeatherStyler.Application.Services.WeatherService>();
        services.AddScoped<WeatherStyler.Application.Services.HistoryService>();
        return services;
    }
}
