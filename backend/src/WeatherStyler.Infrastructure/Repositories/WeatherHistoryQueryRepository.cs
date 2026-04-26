

using WeatherStyler.Domain.Interfaces.Repositories;

namespace WeatherStyler.Infrastructure.Repositories;

internal class WeatherHistoryQueryRepository : IWeatherHistoryQueryRepository
{
    public Task<IEnumerable<(DateTime DateFetched, string DataJson)>> GetByDateRangeAsync(Guid userId, DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        // Weather history is not stored persistently in the current schema.
        // Return an empty collection to satisfy callers.
        IEnumerable<(DateTime DateFetched, string DataJson)> empty = Array.Empty<(DateTime, string)>();
        return Task.FromResult(empty);
    }

    public Task<IEnumerable<(DateTime DateFetched, string DataJson)>> GetTodayAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        IEnumerable<(DateTime DateFetched, string DataJson)> empty = Array.Empty<(DateTime, string)>();
        return Task.FromResult(empty);
    }
}
