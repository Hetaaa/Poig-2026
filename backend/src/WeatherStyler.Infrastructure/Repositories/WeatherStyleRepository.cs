using Microsoft.EntityFrameworkCore;
using WeatherStyler.Domain.Entities;
using WeatherStyler.Domain.Repositories;
using WeatherStyler.Infrastructure.Persistence;

namespace WeatherStyler.Infrastructure.Repositories;

internal class WeatherStyleRepository(WeatherStylerDbContext dbContext) : IWeatherStyleRepository
{
    public async Task<IReadOnlyList<WeatherStyle>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.WeatherStyles
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<WeatherStyle> AddAsync(WeatherStyle weatherStyle, CancellationToken cancellationToken = default)
    {
        dbContext.WeatherStyles.Add(weatherStyle);
        await dbContext.SaveChangesAsync(cancellationToken);
        return weatherStyle;
    }
}
