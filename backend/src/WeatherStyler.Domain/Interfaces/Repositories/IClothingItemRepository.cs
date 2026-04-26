using WeatherStyler.Domain.Entities;

namespace WeatherStyler.Domain.Interfaces.Repositories;

public interface IClothingItemRepository
{
    Task<IReadOnlyList<ClothingItem>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ClothingItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ClothingItem> AddAsync(ClothingItem item, CancellationToken cancellationToken = default);
    Task UpdateAsync(ClothingItem item, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    // helper methods for validation
    Task<IEnumerable<Guid>> FindMissingStyleIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    Task<IEnumerable<Guid>> FindMissingColorIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    Task<bool> CategoryExistsAsync(Guid categoryId, CancellationToken cancellationToken = default);
}
