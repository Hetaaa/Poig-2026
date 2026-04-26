

using WeatherStyler.Domain.Entities;
using WeatherStyler.Domain.Interfaces.Repositories;
using WeatherStyler.Domain.Interfaces.Services;

namespace WeatherStyler.Application.Services;

public class HistoryService : IHistoryService
{
    private readonly IUsageHistoryRepository _usageRepo;
    private readonly IWeatherHistoryQueryRepository _weatherRepo;

    public HistoryService(IUsageHistoryRepository usageRepo, IWeatherHistoryQueryRepository weatherRepo)
    {
        _usageRepo = usageRepo;
        _weatherRepo = weatherRepo;
    }

    public async Task<IEnumerable<UsageHistory>> GetUsageHistoryAsync(Guid userId, DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        return await _usageRepo.GetByDateRangeAsync(userId, from, to, cancellationToken);
    }

    public async Task<IEnumerable<UsageHistory>> GetUsageHistoryTodayAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _usageRepo.GetTodayAsync(userId, cancellationToken);
    }
}
