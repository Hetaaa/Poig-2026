using System.Net.Http;
using WeatherStyler.Domain.Repositories;

namespace WeatherStyler.Application.Services;

public class WeatherService
{
    private readonly HttpClient _http = new HttpClient();

    public WeatherService()
    {
    }

    /// <summary>
    /// Pobierz dane pogodowe dla danej lokacji
    /// </summary>
    public async Task<string?> GetWeatherForLocationAsync(double latitude, double longitude, CancellationToken cancellationToken = default)
    {
        try
        {
            var url = $"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}&daily=temperature_2m_max,temperature_2m_min,precipitation_sum&timezone=UTC&forecast_days=1";
            var resp = await _http.GetAsync(url, cancellationToken);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadAsStringAsync(cancellationToken);
        }
        catch
        {
            return null;
        }
    }
}

