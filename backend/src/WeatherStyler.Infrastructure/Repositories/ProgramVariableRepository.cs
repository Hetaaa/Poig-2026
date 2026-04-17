using Microsoft.EntityFrameworkCore;
using WeatherStyler.Domain.Repositories;
using WeatherStyler.Infrastructure.Entities;
using WeatherStyler.Infrastructure.Persistence;

namespace WeatherStyler.Infrastructure.Repositories;

internal class ProgramVariableRepository : IProgramVariableRepository
{
    private readonly AppDbContext _db;

    public ProgramVariableRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<string?> GetValueAsync(string key, CancellationToken cancellationToken = default)
    {
        // not-scoped: return null - use overload with userId
        return null;
    }

    public async Task<string?> GetValueAsync(string key, Guid userId, CancellationToken cancellationToken = default)
    {
        var e = await _db.ProgramVariables.AsNoTracking().FirstOrDefaultAsync(p => p.Key == key && p.UserId == userId, cancellationToken);
        return e?.Value;
    }

    public async Task SetValueAsync(string key, string value, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Use SetValueAsync(key, value, userId) for user-scoped variables");
    }

    public async Task SetValueAsync(string key, string value, Guid userId, CancellationToken cancellationToken = default)
    {
        var e = await _db.ProgramVariables.FirstOrDefaultAsync(p => p.Key == key && p.UserId == userId, cancellationToken);
        if (e is null)
        {
            e = new ProgramVariableEntity { Id = Guid.NewGuid(), Key = key, Value = value, UserId = userId };
            _db.ProgramVariables.Add(e);
        }
        else
        {
            e.Value = value;
        }

        await _db.SaveChangesAsync(cancellationToken);
    }
}
