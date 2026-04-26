using System.Net.Http.Json;
using System.Text.Json;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using WeatherStyler.Domain.Entities.BuisnessLogic;

namespace WeatherStyler.Application.Services;

public class WeatherService : IWeatherService
{
    private readonly HttpClient _http = new();
    private readonly IConfiguration _configuration;

    public WeatherService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<WeatherDataForGeneration?> GetWeatherForLocationAsync(string lat, string lon, CancellationToken cancellationToken = default)
    {
        if (!double.TryParse(lat, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var la)) return null;
        if (!double.TryParse(lon, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var lo)) return null;
        return await GetWeatherForLocationAsync(la, lo, cancellationToken);
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

    public async Task<DailyWeather?> GetDailySummaryAsync(double latitude, double longitude, DateTime date, CancellationToken cancellationToken = default)
    {
        try
        {
            var day = date.Date;
            var url = $"https://api.open-meteo.com/v1/forecast?latitude={latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}&longitude={longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}&hourly=temperature_2m,apparent_temperature,relativehumidity_2m,wind_speed_10m,precipitation,cloud_cover&start_date={day:yyyy-MM-dd}&end_date={day:yyyy-MM-dd}&timezone=auto";
            var response = await _http.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(cancellationToken));
            var hourly = doc.RootElement.GetProperty("hourly");

            var timeElems = hourly.GetProperty("time").EnumerateArray().Select(e => e.GetString()).ToArray();
            var temps = hourly.GetProperty("temperature_2m").EnumerateArray().Select(e => e.GetDouble()).ToArray();
            var feels = hourly.GetProperty("apparent_temperature").EnumerateArray().Select(e => e.GetDouble()).ToArray();
            var hums = hourly.GetProperty("relativehumidity_2m").EnumerateArray().Select(e => e.GetDouble()).ToArray();
            var winds = hourly.GetProperty("wind_speed_10m").EnumerateArray().Select(e => e.GetDouble()).ToArray();
            var prec = hourly.GetProperty("precipitation").EnumerateArray().Select(e => e.GetDouble()).ToArray();
            var clouds = hourly.GetProperty("cloud_cover").EnumerateArray().Select(e => e.GetDouble()).ToArray();

            // select hours between 08:00 and 19:00 local time
            var indices = new List<int>();
            for (int i = 0; i < timeElems.Length; i++)
            {
                var tstr = timeElems[i];
                if (string.IsNullOrWhiteSpace(tstr)) continue;
                if (!DateTime.TryParse(tstr, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                {
                    // try parse with roundtrip
                    if (!DateTime.TryParse(tstr, out dt)) continue;
                }
                if (dt.Hour >= 8 && dt.Hour <= 19)
                    indices.Add(i);
            }

            if (!indices.Any()) return null;

            double avgTemp = indices.Select(i => temps[i]).Average();
            double avgFeels = indices.Select(i => feels[i]).Average();
            double avgHum = indices.Select(i => hums[i]).Average();
            double avgWind = indices.Select(i => winds[i]).Average();
            double avgPrec = indices.Select(i => prec[i]).Average();
            double avgCloud = indices.Select(i => clouds[i]).Average();

            return new DailyWeather(day, avgTemp, avgFeels, avgHum, avgWind, avgPrec, avgCloud);
        }
        catch
        {
            return null;
        }
    }
}