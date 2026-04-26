using AutoMapper;
using WeatherStyler.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using WeatherStyler.Infrastructure.Entities;
using WeatherStyler.Domain.Interfaces.Repositories;
using WeatherStyler.Domain.Entities;

namespace WeatherStyler.Infrastructure.Services;

public class OutfitService : IOutfitRepository
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public OutfitService(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task SaveGeneratedOutfitAsync(Guid outfitId, string name, DateTime dateCreated, Guid userId, IEnumerable<Guid> clothingItemIds, DateTime dateWorn, CancellationToken cancellationToken = default)
    {
        // create outfit entity
        var outfitEntity = new OutfitEntity
        {
            Id = outfitId,
            Name = name,
            DateCreated = dateCreated,
            UserId = userId
        };

        foreach (var cid in clothingItemIds)
        {
            var item = await _db.ClothingItems.FindAsync(new object[] { cid }, cancellationToken);
            if (item != null)
            {
                _db.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
                outfitEntity.ClothingItems.Add(item);
            }
        }

        // save in transaction
        using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            _db.Outfits.Add(outfitEntity);

            var usage = new UsageHistoryEntity
            {
                Id = Guid.NewGuid(),
                DateWorn = dateWorn,
                // Rating must be within 1..5 (DB CHECK constraint)
                Rating = 1,
                UserId = userId,
                OutfitId = outfitEntity.Id
            };

            _db.UsageHistories.Add(usage);
            await _db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<bool> UserExistsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _db.Users.AnyAsync(u => u.Id == userId, cancellationToken);
    }

    public async Task<IEnumerable<Outfit>> GetOutfitsAsync(Guid userId, DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var usageHistories = await _db.UsageHistories
            .Where(u => u.UserId == userId && u.DateWorn >= from && u.DateWorn <= to)
            .Include(u => u.Outfit)
                .ThenInclude(o => o.ClothingItems)
                    .ThenInclude(ci => ci.Styles)
            .Include(u => u.Outfit)
                .ThenInclude(o => o.ClothingItems)
                    .ThenInclude(ci => ci.Colors)
            .Include(u => u.Outfit)
                .ThenInclude(o => o.ClothingItems)
                    .ThenInclude(ci => ci.Properties)
            .ToListAsync(cancellationToken);

        return usageHistories
            .Where(u => u.Outfit != null)
            .Select(u => _mapper.Map<Outfit>(u.Outfit))
            .ToList();
    }

    public async Task<IEnumerable<Outfit>> GetFavouriteOutfitsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var usageHistories = await _db.UsageHistories
            .Where(u => u.UserId == userId && u.IsFavourite && !u.IsDeleted)
            .Include(u => u.Outfit)
                .ThenInclude(o => o.ClothingItems)
                    .ThenInclude(ci => ci.Styles)
            .Include(u => u.Outfit)
                .ThenInclude(o => o.ClothingItems)
                    .ThenInclude(ci => ci.Colors)
            .Include(u => u.Outfit)
                .ThenInclude(o => o.ClothingItems)
                    .ThenInclude(ci => ci.Properties)
            .OrderByDescending(u => u.DateWorn)
            .ToListAsync(cancellationToken);

        return usageHistories
            .Where(u => u.Outfit != null)
            .Select(u => _mapper.Map<Outfit>(u.Outfit))
            .ToList();
    }

    public async Task<IEnumerable<Outfit>> GetFavouriteOutfitsAsync(Guid userId, DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var usageHistories = await _db.UsageHistories
            .Where(u => u.UserId == userId && u.IsFavourite && u.DateWorn >= from && u.DateWorn <= to && !u.IsDeleted)
            .Include(u => u.Outfit)
                .ThenInclude(o => o.ClothingItems)
                    .ThenInclude(ci => ci.Styles)
            .Include(u => u.Outfit)
                .ThenInclude(o => o.ClothingItems)
                    .ThenInclude(ci => ci.Colors)
            .Include(u => u.Outfit)
                .ThenInclude(o => o.ClothingItems)
                    .ThenInclude(ci => ci.Properties)
            .OrderByDescending(u => u.DateWorn)
            .ToListAsync(cancellationToken);

        return usageHistories
            .Where(u => u.Outfit != null)
            .Select(u => _mapper.Map<Outfit>(u.Outfit))
            .ToList();
    }

    public async Task<IEnumerable<UsageHistory>> GetOutfitsSummaryAsync(Guid userId, DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var usageHistories = await _db.UsageHistories
            .Where(u => u.UserId == userId && u.DateWorn >= from && u.DateWorn <= to)
            .Include(u => u.Outfit)
                .ThenInclude(o => o.ClothingItems)
                    .ThenInclude(ci => ci.Styles)
            .Include(u => u.Outfit)
                .ThenInclude(o => o.ClothingItems)
                    .ThenInclude(ci => ci.Colors)
            .Include(u => u.Outfit)
                .ThenInclude(o => o.ClothingItems)
                    .ThenInclude(ci => ci.Properties)
            .ToListAsync(cancellationToken);

        return usageHistories
            .Select(u => _mapper.Map<UsageHistory>(u))
            .ToList();
    }
}