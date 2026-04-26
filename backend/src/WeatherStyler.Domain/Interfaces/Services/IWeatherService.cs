using WeatherStyler.Domain.Entities.BuisnessLogic;

namespace WeatherStyler.Application.Services
{
    public interface IWeatherService
    {
        Task<DailyWeather?> GetDailySummaryAsync(double latitude, double longitude, DateTime date, CancellationToken cancellationToken = default);
        Task<WeatherDataForGeneration?> GetWeatherForLocationAsync(double latitude, double longitude, CancellationToken cancellationToken = default);
        Task<WeatherDataForGeneration?> GetWeatherForLocationAsync(string lat, string lon, CancellationToken cancellationToken = default);
    }
}