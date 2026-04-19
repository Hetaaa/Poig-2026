using WeatherStyler.Domain.Entities;

namespace WeatherStyler.Domain.Repositories;

public interface IUsageHistoryRepository
{
    Task<IEnumerable<UsageHistory>> GetByDateRangeAsync(Guid userId, DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<IEnumerable<UsageHistory>> GetTodayAsync(Guid userId, CancellationToken cancellationToken = default);
}

public interface IWeatherHistoryQueryRepository
{
    Task<IEnumerable<(DateTime DateFetched, string DataJson)>> GetByDateRangeAsync(Guid userId, DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<IEnumerable<(DateTime DateFetched, string DataJson)>> GetTodayAsync(Guid userId, CancellationToken cancellationToken = default);
}
