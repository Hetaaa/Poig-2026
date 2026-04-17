using WeatherStyler.Application.Contracts;

namespace WeatherStyler.Application.Services;

public interface IClothingItemService
{
    Task<IEnumerable<ClothingItemDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ClothingItemDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ClothingItemDto> CreateAsync(CreateClothingItemRequest request, Guid userId, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid id, CreateClothingItemRequest request, Guid userId, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
}
