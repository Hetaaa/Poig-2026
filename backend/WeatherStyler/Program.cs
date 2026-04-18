using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using WeatherStyler.Application;
// OpenAPI transformer removed to avoid direct dependency on Microsoft.OpenApi types
using WeatherStyler.Infrastructure;
using WeatherStyler.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
var weatherStylerFolder = Path.Combine(appDataPath, "WeatherStyler");
Directory.CreateDirectory(weatherStylerFolder);

var databasePath = Path.Combine(weatherStylerFolder, "weatherstyler.db");
builder.Configuration["ConnectionStrings:WeatherStylerDb"] = $"Data Source={databasePath}";

// Add services to the container.

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
builder.Services.AddInfrastructure(builder.Configuration);
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Moje API")
            .WithTheme(ScalarTheme.DeepSpace)
            ;
    });
}

app.UseCors("LocalhostPolicy");
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    // Seed initial readonly values (slots, categories, colors) once if database is empty
    var anyCategories = db.Categories.Any();
    if (!anyCategories)
    {
        var seeder = scope.ServiceProvider.GetService<WeatherStyler.Application.Services.InitialValuesService>();
        try
        {
            seeder?.SeedAsync(CancellationToken.None).GetAwaiter().GetResult();
        }
        catch
        {
            // swallow seed exceptions during startup to avoid blocking app startup; inspect logs in real app
        }
    }
}

app.Run();
