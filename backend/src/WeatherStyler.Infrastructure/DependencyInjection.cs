using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeatherStyler.Infrastructure.Mapping;
using WeatherStyler.Domain.Repositories;
using WeatherStyler.Infrastructure.Repositories;
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
        services.AddScoped<IClothingItemRepository, ClothingItemRepository>();
        services.AddScoped<ILookupRepository, LookupRepository>();
        services.AddScoped<IClothingPropertyRepository, ClothingPropertyRepository>();
        // Clothing attributes repository (categories, styles, colors, properties CRUD)
        services.AddScoped<IClothingAttributesRepository, ClothingAttributesRepository>();
        services.AddScoped<IProgramVariableRepository, ProgramVariableRepository>();
        // ensure IHttpContextAccessor available to application
        services.AddSingleton(typeof(Microsoft.AspNetCore.Http.IHttpContextAccessor), typeof(Microsoft.AspNetCore.Http.HttpContextAccessor));
        // register IUserService implementation
        services.AddScoped<WeatherStyler.Application.Services.IUserService, WeatherStyler.Infrastructure.Services.HttpContextUserService>();
        // register initial values service
        services.AddScoped<WeatherStyler.Application.Services.InitialValuesService>();
        services.AddScoped<WeatherStyler.Application.Services.IUserAccountService, WeatherStyler.Infrastructure.Services.UserAccountService>();
        services.AddScoped<WeatherStyler.Domain.Repositories.IUsageHistoryRepository, WeatherStyler.Infrastructure.Repositories.UsageHistoryRepository>();
        services.AddScoped<WeatherStyler.Infrastructure.Services.OutfitService>();

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
