using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using WeatherStyler.Domain.Entities.BuisnessLogic;

namespace WeatherStyler.Application.Services;

public class WeatherService
{
    private readonly HttpClient _http = new();
    private readonly IConfiguration _configuration;

    public WeatherService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<WeatherDataForGeneration?> GetWeatherForLocationAsync(double latitude, double longitude, CancellationToken cancellationToken = default)
    {
        try
        {
            // Pobieramy progi z appsettings.json
            var thresholds = _configuration.GetSection("WeatherThresholds").Get<WeatherThresholds>() ?? new WeatherThresholds();

            // Link uwzględniający dane godzinowe: temp, opady, zachmurzenie i wiatr
            var url = $"https://api.open-meteo.com/v1/forecast?latitude={latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}&longitude={longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}&hourly=temperature_2m,precipitation,cloud_cover,wind_speed_10m&forecast_days=1&timezone=auto";

            var response = await _http.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(cancellationToken));
            var hourly = doc.RootElement.GetProperty("hourly");

            // Indeksy 8-19 odpowiadają godzinom 8:00 - 19:00
            var temps = hourly.GetProperty("temperature_2m").EnumerateArray().ToArray();
            var rains = hourly.GetProperty("precipitation").EnumerateArray().ToArray();
            var clouds = hourly.GetProperty("cloud_cover").EnumerateArray().ToArray();
            var winds = hourly.GetProperty("wind_speed_10m").EnumerateArray().ToArray();

            // Obliczanie średniej temperatury (8:00 - 19:00)
            float sumTemp = 0;
            bool isRaining = false;
            bool isWindy = false;
            float sumClouds = 0;

            for (int i = 8; i <= 19; i++)
            {
                sumTemp += temps[i].GetSingle();
                sumClouds += clouds[i].GetSingle();

                if (rains[i].GetSingle() > thresholds.RainThreshold) isRaining = true;
                if (winds[i].GetSingle() > thresholds.WindThreshold) isWindy = true;
            }

            int avgTemp = (int)Math.Round(sumTemp / 12);
            float avgClouds = sumClouds / 12;

            return new WeatherDataForGeneration(
                Temperature: avgTemp,
                IsRaining: isRaining,
                IsWindy: isWindy,
                IsSunny: avgClouds < thresholds.SunnyCloudThreshold
            );
        }
        catch
        {
            return null;
        }
    }
}