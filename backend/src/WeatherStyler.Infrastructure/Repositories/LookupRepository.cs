using Microsoft.EntityFrameworkCore;
using WeatherStyler.Domain.Entities;

using WeatherStyler.Domain.Entities;
using WeatherStyler.Domain.Interfaces.Repositories;
using WeatherStyler.Infrastructure.Entities;
using WeatherStyler.Infrastructure.Persistence;

namespace WeatherStyler.Infrastructure.Repositories;


using AutoMapper;

internal class LookupRepository : ILookupRepository
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public LookupRepository(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _db.Categories
            .Include(c => c.ClothingSlots)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<Category>>(entities);
    }

    public async Task<Category?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var e = await _db.Categories
            .Include(c => c.ClothingSlots)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (e is null) return null;
        return _mapper.Map<Category>(e);
    }

    public async Task<IReadOnlyList<Style>> GetStylesAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _db.Styles.AsNoTracking().ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<Style>>(entities);
    }

    public async Task<Style?> GetStyleByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var e = await _db.Styles.FindAsync(new object[] { id }, cancellationToken);
        if (e is null) return null;
        return _mapper.Map<Style>(e);
    }

    public async Task<IReadOnlyList<Color>> GetColorsAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _db.Colors.AsNoTracking().ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<Color>>(entities);
    }

    public async Task<Color?> GetColorByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var e = await _db.Colors.FindAsync(new object[] { id }, cancellationToken);
        if (e is null) return null;
        return _mapper.Map<Color>(e);
    }
}
