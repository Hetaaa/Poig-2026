using WeatherStyler.Domain.Entities;

namespace WeatherStyler.Domain.Interfaces.Services;

public interface ILookupService
{
    Task<IEnumerable<Category>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<Category?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Style>> GetStylesAsync(CancellationToken cancellationToken = default);
    Task<Style?> GetStyleByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Color>> GetColorsAsync(CancellationToken cancellationToken = default);
    Task<Color?> GetColorByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
