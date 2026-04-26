namespace WeatherStyler.Domain.Interfaces.Services;

using WeatherStyler.Domain.Entities;

public interface IClothingItemService
{
    Task<IEnumerable<ClothingItem>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<ClothingItem?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    Task<ClothingItem> CreateAsync(ClothingItem clothingItem, Guid userId, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid id, ClothingItem clothingItem, Guid userId, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
}
