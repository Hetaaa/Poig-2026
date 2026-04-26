using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WeatherStyler.Domain.Interfaces.Repositories;
using WeatherStyler.Domain.Interfaces.Services;
using WeatherStyler.Infrastructure.Entities;
using WeatherStyler.Infrastructure.Mapping;
using WeatherStyler.Infrastructure.Persistence;
using WeatherStyler.Infrastructure.Repositories;
using WeatherStyler.Infrastructure.Services;

namespace WeatherStyler.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("WeatherStylerDb")
            ?? "Data Source=weatherstyler.db";

        services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));
        services.AddAutoMapper(cfg => cfg.AddMaps(typeof(WardrobeMappingProfile).Assembly));

        // Register infrastructure repositories
        services.AddScoped<IClothingItemRepository, ClothingItemRepository>();
        services.AddScoped<ILookupRepository, LookupRepository>();
        services.AddScoped<IClothingPropertyRepository, ClothingPropertyRepository>();
        services.AddScoped<IClothingAttributesRepository, ClothingAttributesRepository>();
        services.AddScoped<IProgramVariableRepository, ProgramVariableRepository>();
        services.AddScoped<IUsageHistoryRepository, UsageHistoryRepository>();
        services.AddScoped<IWeatherHistoryQueryRepository, WeatherHistoryQueryRepository>();
        services.AddScoped<IOutfitRepository, OutfitService>();

        // Register infrastructure services
        services.AddHttpContextAccessor();
        services.AddScoped<IUserService, HttpContextUserService>();
        services.AddScoped<IUserAccountService, UserAccountService>();
        services.AddScoped<OutfitService>();

        // Identity - use ApplicationUser with GUID primary key and IdentityRole<Guid>
        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options => { })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        var jwtKey = configuration["Jwt:Key"] ?? "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";
        var jwtIssuer = configuration["Jwt:Issuer"] ?? "WeatherStyler";

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
            };
        });

        return services;
    }
}
