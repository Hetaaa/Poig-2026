using Microsoft.EntityFrameworkCore;
using WeatherStyler.Domain.Repositories;
using WeatherStyler.Infrastructure.Persistence;

namespace WeatherStyler.Infrastructure.Repositories;

internal class WeatherHistoryQueryRepository : IWeatherHistoryQueryRepository
{
    private readonly AppDbContext _db;

    public WeatherHistoryQueryRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<(DateTime DateFetched, string DataJson)>> GetByDateRangeAsync(Guid userId, DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var list = await _db.WeatherHistories.AsNoTracking()
            .Join(_db.UsageHistories.AsNoTracking(), w => w.UsageHistoryId, u => u.Id, (w, u) => new { w, u })
            .Where(x => x.u.UserId == userId && x.w.DateFetched >= from && x.w.DateFetched <= to)
            .Select(x => new { x.w.DateFetched, x.w.DataJson })
            .ToListAsync(cancellationToken);

        return list.Select(x => (x.DateFetched, x.DataJson));
    }

    public async Task<IEnumerable<(DateTime DateFetched, string DataJson)>> GetTodayAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        return await GetByDateRangeAsync(userId, today, today.AddDays(1).AddTicks(-1), cancellationToken);
    }
}
