using WeatherStyler.Domain.Repositories;

namespace WeatherStyler.Application.Services;

public class HistoryService
{
    private readonly IUsageHistoryRepository _usageRepo;
    private readonly IWeatherHistoryQueryRepository _weatherRepo;

    public HistoryService(IUsageHistoryRepository usageRepo, IWeatherHistoryQueryRepository weatherRepo)
    {
        _usageRepo = usageRepo;
        _weatherRepo = weatherRepo;
    }

    public async Task<IEnumerable<object>> GetUsageHistoryAsync(Guid userId, DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var items = await _usageRepo.GetByDateRangeAsync(userId, from, to, cancellationToken);
        return items.Select(i => new { i.Id, i.DateWorn, i.Rating });
    }

    public async Task<IEnumerable<object>> GetUsageHistoryTodayAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var items = await _usageRepo.GetTodayAsync(userId, cancellationToken);
        return items.Select(i => new { i.Id, i.DateWorn, i.Rating });
    }

    public async Task<IEnumerable<object>> GetWeatherHistoryAsync(Guid userId, DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var items = await _weatherRepo.GetByDateRangeAsync(userId, from, to, cancellationToken);
        return items.Select(i => new { DateFetched = i.DateFetched, DataJson = i.DataJson });
    }

    public async Task<IEnumerable<object>> GetWeatherHistoryTodayAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var items = await _weatherRepo.GetTodayAsync(userId, cancellationToken);
        return items.Select(i => new { DateFetched = i.DateFetched, DataJson = i.DataJson });
    }
}
