using WeatherStyler.Domain.Entities;

namespace WeatherStyler.Domain.Interfaces.Services;

public interface IHistoryService
{
    Task<IEnumerable<UsageHistory>> GetUsageHistoryAsync(Guid userId, DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<IEnumerable<UsageHistory>> GetUsageHistoryTodayAsync(Guid userId, CancellationToken cancellationToken = default);
}
