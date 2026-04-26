using Microsoft.Extensions.DependencyInjection;
using WeatherStyler.Application.Profiles;
using WeatherStyler.Application.Services;
using WeatherStyler.Domain.Interfaces.Services;
// keep this project free of ASP.NET-specific DI helpers

namespace WeatherStyler.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IClothingItemService, ClothingItemService>();
        services.AddScoped<ILookupService, LookupService>();
        services.AddScoped<IProgramVariableService, ProgramVariableService>();
        services.AddScoped<IWeatherService, WeatherService>();
        services.AddScoped<IHistoryService, HistoryService>();
        services.AddScoped<IOutfitManagerService, OutfitManagerService>();
        services.AddAutoMapper(cfg => cfg.AddMaps(typeof(DomainToDtoProfile).Assembly));
        services.AddScoped<InitialValuesService>();
        return services;
    }
}
