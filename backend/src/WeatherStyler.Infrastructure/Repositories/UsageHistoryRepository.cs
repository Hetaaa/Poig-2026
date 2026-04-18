using Microsoft.EntityFrameworkCore;
using WeatherStyler.Domain.Repositories;
using WeatherStyler.Infrastructure.Persistence;
using WeatherStyler.Domain.Wardrobe.Entities;

namespace WeatherStyler.Infrastructure.Repositories;

internal class UsageHistoryRepository : IUsageHistoryRepository
{
    private readonly AppDbContext _db;

    public UsageHistoryRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<UsageHistory>> GetByDateRangeAsync(Guid userId, DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var items = await _db.UsageHistories.AsNoTracking()
            .Where(u => u.UserId == userId && u.DateWorn >= from && u.DateWorn <= to)
            .ToListAsync(cancellationToken);

        return items.Select(e => new UsageHistory { Id = e.Id, DateWorn = e.DateWorn, Rating = e.Rating, UserId = e.UserId });
    }

    public async Task<IEnumerable<UsageHistory>> GetTodayAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        return await GetByDateRangeAsync(userId, today, today.AddDays(1).AddTicks(-1), cancellationToken);
    }
}
