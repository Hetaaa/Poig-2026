using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using WeatherStyler.Application;
using WeatherStyler.Infrastructure;
using WeatherStyler.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
var weatherStylerFolder = Path.Combine(appDataPath, "WeatherStyler");
Directory.CreateDirectory(weatherStylerFolder);

var databasePath = Path.Combine(weatherStylerFolder, "weatherstyler.db");
builder.Configuration["ConnectionStrings:WeatherStylerDb"] = $"Data Source={databasePath}";

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalhostPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:1420", "http://localhost:5173", "http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration); // JWT jest tutaj

// ─── OpenAPI + JWT ────────────────────────────────────────────────────────────
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
                auth.Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIzYzA3NWRhNy1jY2ViLTQ0NTQtYTY2Zi1kZTk2NTIwMTA5MWIiLCJ1bmlxdWVfbmFtZSI6IngiLCJqdGkiOiJiYWM0MzFjOS0yYjY4LTQ0MWItOTgxNC04Yjc2Mjk4MDFkZjQiLCJleHAiOjE3NzY2Nzk3NzUsImlzcyI6IldlYXRoZXJTdHlsZXIifQ.T_ZfH029liG4pxZn_JIpBIEOb2S3f2axDdwsIsFARks";
            });
    });
}

app.UseCors("LocalhostPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

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