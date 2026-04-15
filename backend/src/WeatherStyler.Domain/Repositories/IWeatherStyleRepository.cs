using WeatherStyler.Domain.Entities;

namespace WeatherStyler.Domain.Repositories;

public interface IWeatherStyleRepository
{
    Task<IReadOnlyList<WeatherStyle>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<WeatherStyle> AddAsync(WeatherStyle weatherStyle, CancellationToken cancellationToken = default);
}
