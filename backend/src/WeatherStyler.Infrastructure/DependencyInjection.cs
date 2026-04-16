using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeatherStyler.Infrastructure.Mapping;
using WeatherStyler.Infrastructure.Persistence;
using WeatherStyler.Infrastructure.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace WeatherStyler.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("WeatherStylerDb")
            ?? "Data Source=weatherstyler.db";

        services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));
        services.AddAutoMapper(typeof(WardrobeMappingProfile).Assembly);
        // register infra repositories
        services.AddScoped<WeatherStyler.Domain.Repositories.IClothingItemRepository, WeatherStyler.Infrastructure.Repositories.ClothingItemRepository>();

        // Identity - use ApplicationUser with GUID primary key and IdentityRole<Guid>
        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options => { })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        var jwtKey = configuration["Jwt:Key"] ?? "PLEASE_CHANGE_THIS_SECRET";
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
