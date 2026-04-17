using WeatherStyler.Domain.Repositories;
using WeatherStyler.Infrastructure.Persistence;
using WeatherStyler.Infrastructure.Entities;

namespace WeatherStyler.Infrastructure.Repositories;

internal class WeatherHistoryRepository : IWeatherHistoryRepository
{
    private readonly AppDbContext _db;

    public WeatherHistoryRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(Guid usageHistoryId, string dataJson, DateTime dateFetched, CancellationToken cancellationToken = default)
    {
        var entity = new WeatherHistoryEntity { Id = Guid.NewGuid(), UsageHistoryId = usageHistoryId, DataJson = dataJson, DateFetched = dateFetched };
        _db.WeatherHistories.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
