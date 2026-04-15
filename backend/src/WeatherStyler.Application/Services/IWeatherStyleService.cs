using WeatherStyler.Application.Contracts;

namespace WeatherStyler.Application.Services;

public interface IWeatherStyleService
{
    Task<IReadOnlyList<WeatherStyleDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<WeatherStyleDto> CreateAsync(string name, string themeColor, CancellationToken cancellationToken = default);
}
