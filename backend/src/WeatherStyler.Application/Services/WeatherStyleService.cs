using WeatherStyler.Application.Contracts;
using WeatherStyler.Domain.Entities;
using WeatherStyler.Domain.Repositories;

namespace WeatherStyler.Application.Services;

internal class WeatherStyleService(IWeatherStyleRepository weatherStyleRepository) : IWeatherStyleService
{
    public async Task<IReadOnlyList<WeatherStyleDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var styles = await weatherStyleRepository.GetAllAsync(cancellationToken);
        return styles.Select(x => new WeatherStyleDto(x.Id, x.Name, x.ThemeColor)).ToList();
    }

    public async Task<WeatherStyleDto> CreateAsync(string name, string themeColor, CancellationToken cancellationToken = default)
    {
        var weatherStyle = new WeatherStyle
        {
            Id = Guid.NewGuid(),
            Name = name,
            ThemeColor = themeColor
        };

        var created = await weatherStyleRepository.AddAsync(weatherStyle, cancellationToken);
        return new WeatherStyleDto(created.Id, created.Name, created.ThemeColor);
    }
}
