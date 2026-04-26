using Microsoft.EntityFrameworkCore;

using WeatherStyler.Domain.Entities;
using WeatherStyler.Domain.Interfaces.Repositories;
using WeatherStyler.Infrastructure.Persistence;

namespace WeatherStyler.Infrastructure.Repositories;


using AutoMapper;

internal class ClothingPropertyRepository : IClothingPropertyRepository
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public ClothingPropertyRepository(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ClothingProperty>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _db.ClothingProperties.AsNoTracking().ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<ClothingProperty>>(entities);
    }

    public async Task<ClothingProperty?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var e = await _db.ClothingProperties.FindAsync(new object[] { id }, cancellationToken);
        if (e is null) return null;
        return _mapper.Map<ClothingProperty>(e);
    }
}
