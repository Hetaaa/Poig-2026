
using WeatherStyler.Application.Dtos;
using WeatherStyler.Domain.Entities;
using WeatherStyler.Domain.Interfaces.Repositories;
using WeatherStyler.Domain.Interfaces.Services;

namespace WeatherStyler.Application.Services;

public class LookupService : ILookupService
{
    private readonly ILookupRepository _lookupRepo;
    private readonly IClothingPropertyRepository _propertyRepo;

    public LookupService(ILookupRepository lookupRepo, IClothingPropertyRepository propertyRepo)
    {
        _lookupRepo = lookupRepo;
        _propertyRepo = propertyRepo;
    }

    public async Task<IEnumerable<Category>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _lookupRepo.GetCategoriesAsync(cancellationToken);
    }

    public async Task<Category?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _lookupRepo.GetCategoryByIdAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<Style>> GetStylesAsync(CancellationToken cancellationToken = default)
    {
        return await _lookupRepo.GetStylesAsync(cancellationToken);
    }

    public async Task<Style?> GetStyleByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _lookupRepo.GetStyleByIdAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<Color>> GetColorsAsync(CancellationToken cancellationToken = default)
    {
        return await _lookupRepo.GetColorsAsync(cancellationToken);
    }

    public async Task<Color?> GetColorByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _lookupRepo.GetColorByIdAsync(id, cancellationToken);
        
    }

    public async Task<IEnumerable<ClothingProperty>> GetPropertiesAsync(CancellationToken cancellationToken = default)
    {
        return await _propertyRepo.GetAllAsync(cancellationToken);

    }

    public async Task<ClothingProperty?> GetPropertyByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _propertyRepo.GetByIdAsync(id, cancellationToken);
    }
}
