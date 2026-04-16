using Microsoft.EntityFrameworkCore;
using WeatherStyler.Domain.Repositories;
using WeatherStyler.Domain.Wardrobe.Entities;
using WeatherStyler.Infrastructure.Entities;
using WeatherStyler.Infrastructure.Persistence;

namespace WeatherStyler.Infrastructure.Repositories;

internal class ClothingItemRepository : IClothingItemRepository
{
    private readonly AppDbContext _db;

    public ClothingItemRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<ClothingItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _db.ClothingItems
            .Include(x => x.Properties)
            .Include(x => x.Styles)
            .Include(x => x.Colors)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        // map to domain
        return entities.Select(e => new ClothingItem
        {
            Id = e.Id,
            Name = e.Name,
            PhotoUrl = e.PhotoUrl,
            CategoryId = e.CategoryId,
            UserId = e.UserId,
            WarmthLevel = e.WarmthLevel,
            Properties = e.Properties.Select(p => new ClothingProperty { Id = p.Id, Name = p.Name, Value = p.Value, ClothingItemId = p.ClothingItemId }).ToList(),
            Styles = e.Styles.Select(s => new Style { Id = s.Id, Name = s.Name }).ToList(),
            Colors = e.Colors.Select(c => new Color { Id = c.Id, Name = c.Name }).ToList()
        }).ToList();
    }

    public async Task<ClothingItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var e = await _db.ClothingItems
            .Include(x => x.Properties)
            .Include(x => x.Styles)
            .Include(x => x.Colors)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (e is null) return null;

        return new ClothingItem
        {
            Id = e.Id,
            Name = e.Name,
            PhotoUrl = e.PhotoUrl,
            CategoryId = e.CategoryId,
            UserId = e.UserId,
            WarmthLevel = e.WarmthLevel,
            Properties = e.Properties.Select(p => new ClothingProperty { Id = p.Id, Name = p.Name, Value = p.Value, ClothingItemId = p.ClothingItemId }).ToList(),
            Styles = e.Styles.Select(s => new Style { Id = s.Id, Name = s.Name }).ToList(),
            Colors = e.Colors.Select(c => new Color { Id = c.Id, Name = c.Name }).ToList()
        };
    }

    public async Task<ClothingItem> AddAsync(ClothingItem item, CancellationToken cancellationToken = default)
    {
        var e = new ClothingItemEntity
        {
            // Always generate a new Id on create to prevent client-supplied ids
            Id = Guid.NewGuid(),
            Name = item.Name,
            PhotoUrl = item.PhotoUrl,
            CategoryId = item.CategoryId,
            UserId = item.UserId,
            WarmthLevel = item.WarmthLevel
        };

        // Generate new ids for properties on create - do not trust client-supplied ids
        if (item.Properties != null)
        {
            foreach (var p in item.Properties)
            {
                e.Properties.Add(new ClothingPropertyEntity { Id = Guid.NewGuid(), Name = p.Name, Value = p.Value, ClothingItemId = e.Id });
            }
        }

        if (item.Styles != null)
        {
            foreach (var s in item.Styles)
            {
                var styleEntity = await _db.Styles.FindAsync(new object?[] { s.Id }, cancellationToken);
                if (styleEntity != null) e.Styles.Add(styleEntity);
            }
        }

        if (item.Colors != null)
        {
            foreach (var c in item.Colors)
            {
                var colorEntity = await _db.Colors.FindAsync(new object?[] { c.Id }, cancellationToken);
                if (colorEntity != null) e.Colors.Add(colorEntity);
            }
        }

        _db.ClothingItems.Add(e);
        await _db.SaveChangesAsync(cancellationToken);

        item.Id = e.Id;
        return item;
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

        e.Name = item.Name;
        e.PhotoUrl = item.PhotoUrl;
        e.CategoryId = item.CategoryId;
        e.WarmthLevel = item.WarmthLevel;

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
