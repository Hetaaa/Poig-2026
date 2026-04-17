using WeatherStyler.Domain.Wardrobe.Entities;

namespace WeatherStyler.Domain.Repositories;

public interface ILookupRepository
{
    Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<Category?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Style>> GetStylesAsync(CancellationToken cancellationToken = default);
    Task<Style?> GetStyleByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Color>> GetColorsAsync(CancellationToken cancellationToken = default);
    Task<Color?> GetColorByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
