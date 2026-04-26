using Microsoft.EntityFrameworkCore;

using WeatherStyler.Domain.Entities;
using WeatherStyler.Domain.Interfaces.Repositories;
using WeatherStyler.Infrastructure.Entities;
using WeatherStyler.Infrastructure.Persistence;

namespace WeatherStyler.Infrastructure.Repositories;


using AutoMapper;

internal class ClothingItemRepository : IClothingItemRepository
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public ClothingItemRepository(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ClothingItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _db.ClothingItems
            .Include(x => x.Properties)
            .Include(x => x.Styles)
            .Include(x => x.Colors)
            .Include(x => x.Category)
                .ThenInclude(c => c.ClothingSlots)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return _mapper.Map<IReadOnlyList<ClothingItem>>(entities);
    }

    public async Task<ClothingItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var e = await _db.ClothingItems
            .Include(x => x.Properties)
            .Include(x => x.Styles)
            .Include(x => x.Colors)
            .Include(x => x.Category)
                .ThenInclude(c => c.ClothingSlots)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (e is null) return null;
        return _mapper.Map<ClothingItem>(e);
    }

    public async Task<ClothingItem> AddAsync(ClothingItem item, CancellationToken cancellationToken = default)
    {
        var e = _mapper.Map<ClothingItemEntity>(item);
        // Always generate a new Id on create to prevent client-supplied ids
        e.Id = Guid.NewGuid();

        // Generate new ids for properties on create - do not trust client-supplied ids
        if (e.Properties != null)
        {
            foreach (var p in e.Properties)
            {
                p.Id = Guid.NewGuid();
                p.ClothingItemId = e.Id;
            }
        }

        // Attach existing styles
        if (item.Styles != null)
        {
            e.Styles.Clear();
            foreach (var s in item.Styles)
            {
                var styleEntity = await _db.Styles.FindAsync(new object?[] { s.Id }, cancellationToken);
                if (styleEntity != null) e.Styles.Add(styleEntity);
            }
        }

        // Attach existing colors
        if (item.Colors != null)
        {
            e.Colors.Clear();
            foreach (var c in item.Colors)
            {
                var colorEntity = await _db.Colors.FindAsync(new object?[] { c.Id }, cancellationToken);
                if (colorEntity != null) e.Colors.Add(colorEntity);
            }
        }

        _db.ClothingItems.Add(e);
        await _db.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ClothingItem>(e);
    }

    public async Task<IEnumerable<Guid>> FindMissingStyleIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        if (ids == null) return Enumerable.Empty<Guid>();
        var existing = await _db.Styles.Where(s => ids.Contains(s.Id)).Select(s => s.Id).ToListAsync(cancellationToken);
        return ids.Except(existing).ToList();
    }

    public async Task<IEnumerable<Guid>> FindMissingColorIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        if (ids == null) return Enumerable.Empty<Guid>();
        var existing = await _db.Colors.Where(c => ids.Contains(c.Id)).Select(c => c.Id).ToListAsync(cancellationToken);
        return ids.Except(existing).ToList();
    }

    public async Task<bool> CategoryExistsAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await _db.Categories.AnyAsync(c => c.Id == categoryId, cancellationToken);
    }

    public async Task UpdateAsync(ClothingItem item, CancellationToken cancellationToken = default)
    {
        var e = await _db.ClothingItems
            .Include(x => x.Properties)
            .Include(x => x.Styles)
            .Include(x => x.Colors)
            .FirstOrDefaultAsync(x => x.Id == item.Id, cancellationToken);

        if (e is null) throw new InvalidOperationException("Not found");

        // Map simple properties
        _mapper.Map(item, e);

        // replace properties
        _db.ClothingProperties.RemoveRange(e.Properties);
        e.Properties.Clear();
        if (item.Properties != null)
        {
            foreach (var p in item.Properties)
            {
                e.Properties.Add(new ClothingPropertyEntity { Id = p.Id == Guid.Empty ? Guid.NewGuid() : p.Id, Name = p.Name, Value = p.Value, ClothingItemId = e.Id });
            }
        }

        // replace styles
        e.Styles.Clear();
        if (item.Styles != null)
        {
            foreach (var s in item.Styles)
            {
                var styleEntity = await _db.Styles.FindAsync(new object?[] { s.Id }, cancellationToken);
                if (styleEntity != null) e.Styles.Add(styleEntity);
            }
        }

        // replace colors
        e.Colors.Clear();
        if (item.Colors != null)
        {
            foreach (var c in item.Colors)
            {
                var colorEntity = await _db.Colors.FindAsync(new object?[] { c.Id }, cancellationToken);
                if (colorEntity != null) e.Colors.Add(colorEntity);
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var e = await _db.ClothingItems.FindAsync(new object?[] { id }, cancellationToken);
        if (e is null) return;

        _db.ClothingItems.Remove(e);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
