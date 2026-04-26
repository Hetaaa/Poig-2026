
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WeatherStyler.Domain.Entities;

namespace WeatherStyler.Domain.Interfaces.Repositories;

public interface IOutfitRepository
{
    Task<IEnumerable<Outfit>> GetOutfitsAsync(Guid userId, DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<IEnumerable<Outfit>> GetFavouriteOutfitsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Outfit>> GetFavouriteOutfitsAsync(Guid userId, DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task SaveGeneratedOutfitAsync(Guid outfitId, string name, DateTime dateCreated, Guid userId, IEnumerable<Guid> clothingItemIds, DateTime dateWorn, CancellationToken cancellationToken = default);
    Task<bool> UserExistsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<UsageHistory>> GetOutfitsSummaryAsync(Guid userId, DateTime from, DateTime to, CancellationToken cancellationToken = default);
}
