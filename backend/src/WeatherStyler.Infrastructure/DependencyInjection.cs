using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeatherStyler.Domain.Repositories;
using WeatherStyler.Infrastructure.Mapping;
using WeatherStyler.Infrastructure.Persistence;
using WeatherStyler.Infrastructure.Repositories;

namespace WeatherStyler.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("WeatherStylerDb")
            ?? "Data Source=weatherstyler.db";

        services.AddDbContext<WeatherStylerDbContext>(options => options.UseSqlite(connectionString));
        services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));
        services.AddAutoMapper(typeof(WardrobeMappingProfile).Assembly);
        services.AddScoped<IWeatherStyleRepository, WeatherStyleRepository>();

        return services;
    }
}
