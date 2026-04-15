using Microsoft.Extensions.DependencyInjection;
using WeatherStyler.Application.Services;

namespace WeatherStyler.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IWeatherStyleService, WeatherStyleService>();
        return services;
    }
}
