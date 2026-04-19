using WeatherStyler.Domain.Entities;

namespace WeatherStyler.Domain.Repositories;

public interface IClothingPropertyRepository
{
    Task<IReadOnlyList<ClothingProperty>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ClothingProperty?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
