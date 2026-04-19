using Microsoft.EntityFrameworkCore;
using WeatherStyler.Domain.Repositories;
using WeatherStyler.Domain.Entities;
using WeatherStyler.Infrastructure.Persistence;

namespace WeatherStyler.Infrastructure.Repositories;

internal class ClothingPropertyRepository : IClothingPropertyRepository
{
    private readonly AppDbContext _db;

    public ClothingPropertyRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<ClothingProperty>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _db.ClothingProperties.AsNoTracking().Select(p => new ClothingProperty { Id = p.Id, Name = p.Name, Value = p.Value, ClothingItemId = p.ClothingItemId }).ToListAsync(cancellationToken);
    }

    public async Task<ClothingProperty?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var e = await _db.ClothingProperties.FindAsync(new object[] { id }, cancellationToken);
        if (e is null) return null;
        return new ClothingProperty { Id = e.Id, Name = e.Name, Value = e.Value, ClothingItemId = e.ClothingItemId };
    }
}
