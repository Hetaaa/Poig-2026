using WeatherStyler.Domain.Wardrobe.Entities;

namespace WeatherStyler.Domain.Repositories;

public interface IWeatherHistoryRepository
{
    Task AddAsync(Guid usageHistoryId, string dataJson, DateTime dateFetched, CancellationToken cancellationToken = default);
}
