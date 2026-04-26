using WeatherStyler.Domain.Entities;

namespace WeatherStyler.Domain.Interfaces.Repositories;

public interface IWeatherHistoryRepository
{
    Task AddAsync(Guid usageHistoryId, string dataJson, DateTime dateFetched, CancellationToken cancellationToken = default);
}
