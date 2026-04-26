using Microsoft.EntityFrameworkCore;

using WeatherStyler.Infrastructure.Persistence;
using WeatherStyler.Domain.Entities;
using WeatherStyler.Domain.Interfaces.Repositories;

namespace WeatherStyler.Infrastructure.Repositories;

using AutoMapper;

internal class UsageHistoryRepository : IUsageHistoryRepository
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public UsageHistoryRepository(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UsageHistory>> GetByDateRangeAsync(Guid userId, DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var items = await _db.UsageHistories.AsNoTracking()
            .Where(u => u.UserId == userId && u.DateWorn >= from && u.DateWorn <= to)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<UsageHistory>>(items);
    }

    public async Task<IEnumerable<UsageHistory>> GetTodayAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        return await GetByDateRangeAsync(userId, today, today.AddDays(1).AddTicks(-1), cancellationToken);
    }
}
