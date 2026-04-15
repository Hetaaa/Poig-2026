using Microsoft.EntityFrameworkCore;
using WeatherStyler.Application;
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
}

app.UseCors("LocalhostPolicy");
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WeatherStylerDbContext>();
    db.Database.Migrate();
}

app.Run();
