using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeatherStyler.Application;
using WeatherStyler.Infrastructure;
using WeatherStyler.Infrastructure.Persistence;

var services = new ServiceCollection();

var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
var weatherStylerFolder = Path.Combine(appDataPath, "WeatherStyler");
Directory.CreateDirectory(weatherStylerFolder);

var databasePath = Path.Combine(weatherStylerFolder, "weatherstyler.db");

var configuration = new ConfigurationBuilder()
    .AddInMemoryCollection(new Dictionary<string, string?>
    {
        ["ConnectionStrings:WeatherStylerDb"] = $"Data Source={databasePath}"
    })
    .Build();

services.AddApplication();
services.AddInfrastructure(configuration);

using var provider = services.BuildServiceProvider();
using var scope = provider.CreateScope();

var dbContext = scope.ServiceProvider.GetRequiredService<WeatherStylerDbContext>();
await dbContext.Database.MigrateAsync();

Console.WriteLine($"Migration completed. SQLite: {databasePath}");
