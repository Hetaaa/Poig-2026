using WeatherStyler.Domain.Repositories;
using WeatherStyler.Application.Contracts;

namespace WeatherStyler.Application.Services;

public class LookupService
{
    private readonly ILookupRepository _lookupRepo;
    private readonly IClothingPropertyRepository _propertyRepo;

    public LookupService(ILookupRepository lookupRepo, IClothingPropertyRepository propertyRepo)
    {
        _lookupRepo = lookupRepo;
        _propertyRepo = propertyRepo;
    }

    public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var cats = await _lookupRepo.GetCategoriesAsync(cancellationToken);
        return cats.Select(c => new CategoryDto(c.Id, c.Name, c.LayerIndex));
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var c = await _lookupRepo.GetCategoryByIdAsync(id, cancellationToken);
        return c is null ? null : new CategoryDto(c.Id, c.Name, c.LayerIndex);
    }

    public async Task<IEnumerable<StyleDto>> GetStylesAsync(CancellationToken cancellationToken = default)
    {
        var styles = await _lookupRepo.GetStylesAsync(cancellationToken);
        return styles.Select(s => new StyleDto(s.Id, s.Name));
    }

    public async Task<StyleDto?> GetStyleByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var s = await _lookupRepo.GetStyleByIdAsync(id, cancellationToken);
        return s is null ? null : new StyleDto(s.Id, s.Name);
    }

    public async Task<IEnumerable<ColorDto>> GetColorsAsync(CancellationToken cancellationToken = default)
    {
        var colors = await _lookupRepo.GetColorsAsync(cancellationToken);
        return colors.Select(c => new ColorDto(c.Id, c.Name, c.IsNeutral));
    }

    public async Task<ColorDto?> GetColorByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var c = await _lookupRepo.GetColorByIdAsync(id, cancellationToken);
        return c is null ? null : new ColorDto(c.Id, c.Name, c.IsNeutral);
    }

    public async Task<IEnumerable<ClothingPropertyDto>> GetPropertiesAsync(CancellationToken cancellationToken = default)
    {
        var props = await _propertyRepo.GetAllAsync(cancellationToken);
        return props.Select(p => new ClothingPropertyDto(p.Name, p.Value));
    }

    public async Task<ClothingPropertyDto?> GetPropertyByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var p = await _propertyRepo.GetByIdAsync(id, cancellationToken);
        return p is null ? null : new ClothingPropertyDto(p.Name, p.Value);
    }
}
