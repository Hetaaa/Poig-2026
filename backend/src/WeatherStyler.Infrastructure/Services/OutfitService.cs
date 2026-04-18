using WeatherStyler.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace WeatherStyler.Infrastructure.Services;

public class OutfitService
{
    private readonly AppDbContext _db;

    public OutfitService(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Get outfits (via usage history) for user within date range
    /// </summary>
    public async Task<IEnumerable<object>> GetOutfitsAsync(Guid userId, DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var usageHistories = await _db.UsageHistories
            .Where(u => u.UserId == userId && u.DateWorn >= from && u.DateWorn <= to)
            .Include(u => u.Outfit)
            .ThenInclude(o => o.ClothingItems)
            .ThenInclude(ci => ci.Styles)
            .ToListAsync(cancellationToken);

        return usageHistories
            .Where(u => u.Outfit != null)
            .Select(u => new
            {
                u.Id,
                u.DateWorn,
                u.Rating,
                u.IsFavourite,
                OutfitId = u.Outfit!.Id,
                OutfitName = u.Outfit!.Name,
                ItemCount = u.Outfit!.ClothingItems.Count
            })
            .ToList();
    }

    /// <summary>
    /// Get all favourite outfits for user
    /// </summary>
    public async Task<IEnumerable<object>> GetFavouriteOutfitsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var usageHistories = await _db.UsageHistories
            .Where(u => u.UserId == userId && u.IsFavourite && !u.IsDeleted)
            .Include(u => u.Outfit)
            .ThenInclude(o => o.ClothingItems)
            .OrderByDescending(u => u.DateWorn)
            .ToListAsync(cancellationToken);

        return usageHistories
            .Where(u => u.Outfit != null)
            .Select(u => new
            {
                u.Id,
                u.DateWorn,
                u.Rating,
                u.IsFavourite,
                OutfitId = u.Outfit!.Id,
                OutfitName = u.Outfit!.Name,
                ItemCount = u.Outfit!.ClothingItems.Count
            })
            .ToList();
    }

    /// <summary>
    /// Get favourite outfits for user within date range
    /// </summary>
    public async Task<IEnumerable<object>> GetFavouriteOutfitsAsync(Guid userId, DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var usageHistories = await _db.UsageHistories
            .Where(u => u.UserId == userId && u.IsFavourite && u.DateWorn >= from && u.DateWorn <= to && !u.IsDeleted)
            .Include(u => u.Outfit)
            .ThenInclude(o => o.ClothingItems)
            .OrderByDescending(u => u.DateWorn)
            .ToListAsync(cancellationToken);

        return usageHistories
            .Where(u => u.Outfit != null)
            .Select(u => new
            {
                u.Id,
                u.DateWorn,
                u.Rating,
                u.IsFavourite,
                OutfitId = u.Outfit!.Id,
                OutfitName = u.Outfit!.Name,
                ItemCount = u.Outfit!.ClothingItems.Count
            })
            .ToList();
    }

    /// <summary>
    /// Mark/unmark usage history as favourite
    /// </summary>
    public async Task<bool> ToggleFavouriteAsync(Guid usageHistoryId, Guid userId, CancellationToken cancellationToken = default)
    {
        var usage = await _db.UsageHistories
            .FirstOrDefaultAsync(u => u.Id == usageHistoryId && u.UserId == userId, cancellationToken);

        if (usage == null)
            return false;

        usage.IsFavourite = !usage.IsFavourite;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
