
global using Microsoft.EntityFrameworkCore;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Identity;
global using WeatherStyler.Application;
global using WeatherStyler.Infrastructure;
global using WeatherStyler.Infrastructure.Persistence;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using WeatherStyler.Application;
using WeatherStyler.Infrastructure;
using WeatherStyler.Infrastructure.Entities;
using WeatherStyler.Infrastructure.Persistence;



var builder = WebApplication.CreateBuilder(args);


System.Runtime.InteropServices.NativeLibrary.SetDllImportResolver(
    typeof(SQLitePCL.raw).Assembly,
    (libraryName, assembly, searchPath) =>
    {
        if (libraryName == "e_sqlite3")
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var dllPath = Path.Combine(baseDir, "e_sqlite3.dll");

            if (File.Exists(dllPath))
            {
                return System.Runtime.InteropServices.NativeLibrary.Load(dllPath);
            }
        }
        return IntPtr.Zero;
    });

builder.Configuration["Urls"] = "http://127.0.0.1:5267";

// ─── BEZPIECZNE ŚCIEŻKI W APPDATA ───────────────────────────────────────────
var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
var weatherStylerFolder = Path.Combine(appDataPath, "WeatherStyler");
Directory.CreateDirectory(weatherStylerFolder);

// Baza danych
var databasePath = Path.Combine(weatherStylerFolder, "weatherstyler.db");
builder.Configuration["ConnectionStrings:WeatherStylerDb"] = $"Data Source={databasePath}";

// Zdjęcia (wewnątrz folderu AppData)
var imagesPath = Path.Combine(weatherStylerFolder, "images");
if (!Directory.Exists(imagesPath))
{
    Directory.CreateDirectory(imagesPath);
}

// ─── SERWISY APLIKACJI ───────────────────────────────────────────────────────
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalhostPolicy", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:1420",
                "http://localhost:5173",
                "http://localhost:3000",
                "tauri://localhost",
                "https://tauri.localhost"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration, builder.Environment.IsDevelopment());

// ─── USEREXISTS POLICY ────────────────────────────────────────────────────────
builder.Services.AddScoped<IAuthorizationHandler, UserExistsHandler>();
builder.Services.AddAuthorization(options =>
{
    var userExistsPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .AddRequirements(new UserExistsRequirement())
        .Build();

    options.AddPolicy("UserExists", userExistsPolicy);
    options.DefaultPolicy = userExistsPolicy;
});

// ─── OPENAPI + JWT ────────────────────────────────────────────────────────────
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes.Add(
            "Bearer",
            new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Wprowadź token JWT (bez prefiksu 'Bearer')"
            });

        document.Security =
        [
            new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            }
        ];

        return Task.CompletedTask;
    });
});

var app = builder.Build();

// ─── MIDDLEWARE ──────────────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("WeatherStyler API")
            .WithTheme(ScalarTheme.DeepSpace)
            .AddPreferredSecuritySchemes("Bearer")
            .AddHttpAuthentication("Bearer", auth =>
            {
                auth.Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJlNjNjMDA4MC1jMzIzLTQzYTgtYWJkOC1hYzA1Yjc0ZTVlZTAiLCJ1bmlxdWVfbmFtZSI6Inh4eCIsImp0aSI6IjdlZWQ0OTNlLTA4NTctNGZjNi04YjBhLTc3MWYwZTllNzAyMCIsImV4cCI6MTc4MDgyMDkzMywiaXNzIjoiV2VhdGhlclN0eWxlciJ9.jmJMc-SkvN89OQhEWtFcA7lzPvYlM8ZV-vYfZACoR1I";
            });
    });
}

// Statyczne pliki serwowane z AppData
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(imagesPath),
    RequestPath = "/images"
});

app.UseCors("LocalhostPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ─── MIGRACJA I SEEDER BAZY ──────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    if (!db.Categories.Any())
    {
        var seeder = scope.ServiceProvider
            .GetService<WeatherStyler.Application.Services.InitialValuesService>();
        try
        {
            seeder?.SeedAsync(CancellationToken.None).GetAwaiter().GetResult();
        }
        catch
        {
            // swallow seed exceptions during startup
        }
    }
}

app.Run();