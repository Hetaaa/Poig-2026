using Microsoft.EntityFrameworkCore;
using WeatherStyler.Domain.Repositories;
using WeatherStyler.Domain.Wardrobe.Entities;
using WeatherStyler.Infrastructure.Entities;
using WeatherStyler.Infrastructure.Persistence;

namespace WeatherStyler.Infrastructure.Repositories;

internal class LookupRepository : ILookupRepository
{
    private readonly AppDbContext _db;

    public LookupRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Categories.AsNoTracking().Select(c => new Category { Id = c.Id, Name = c.Name }).ToListAsync(cancellationToken);
    }

    public async Task<Category?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var e = await _db.Categories.FindAsync(new object[] { id }, cancellationToken);
        if (e is null) return null;
        return new Category { Id = e.Id, Name = e.Name };
    }

    public async Task<IReadOnlyList<Style>> GetStylesAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Styles.AsNoTracking().Select(s => new Style { Id = s.Id, Name = s.Name }).ToListAsync(cancellationToken);
    }

    public async Task<Style?> GetStyleByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var e = await _db.Styles.FindAsync(new object[] { id }, cancellationToken);
        if (e is null) return null;
        return new Style { Id = e.Id, Name = e.Name };
    }

    public async Task<IReadOnlyList<Color>> GetColorsAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Colors.AsNoTracking().Select(c => new Color { Id = c.Id, Name = c.Name }).ToListAsync(cancellationToken);
    }

    public async Task<Color?> GetColorByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var e = await _db.Colors.FindAsync(new object[] { id }, cancellationToken);
        if (e is null) return null;
        return new Color { Id = e.Id, Name = e.Name };
    }
}
