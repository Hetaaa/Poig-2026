using WeatherStyler.Application.Dtos;
using WeatherStyler.Domain.Entities;
using WeatherStyler.Domain.Interfaces.Repositories;
using WeatherStyler.Domain.Interfaces.Services;
namespace WeatherStyler.Application.Services;

public class ClothingItemService : IClothingItemService
{
    private readonly IClothingItemRepository _repo;

    public ClothingItemService(IClothingItemRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<ClothingItem>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var items = await _repo.GetAllAsync(cancellationToken);
        return items.Where(i => i.UserId == userId);
    }

    public async Task<ClothingItem?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var item = await _repo.GetByIdAsync(id, cancellationToken);
        if (item is null) return null;
        if (item.UserId != userId) return null;
        return item;
    }

    public async Task<ClothingItem> CreateAsync(ClothingItem clothingItem, Guid userId, CancellationToken cancellationToken = default)
    {
        // validation
        if (string.IsNullOrWhiteSpace(clothingItem.Name)) throw new ArgumentException("Name is required");
        if (clothingItem.WarmthLevel < 1 || clothingItem.WarmthLevel > 10) throw new ArgumentOutOfRangeException(nameof(clothingItem.WarmthLevel));

        // validate category
        if (!await _repo.CategoryExistsAsync(clothingItem.CategoryId, cancellationToken))
            throw new ArgumentException("Category does not exist", nameof(clothingItem.CategoryId));

        // validate styles & colors
        var styleIds = clothingItem.Styles?.Select(s => s.Id) ?? Enumerable.Empty<Guid>();
        var missingStyles = (await _repo.FindMissingStyleIdsAsync(styleIds, cancellationToken)).ToList();
        if (missingStyles.Any())
            throw new ArgumentException($"Styles not found: {string.Join(',', missingStyles)}");

        var colorIds = clothingItem.Colors?.Select(c => c.Id) ?? Enumerable.Empty<Guid>();
        var missingColors = (await _repo.FindMissingColorIdsAsync(colorIds, cancellationToken)).ToList();
        if (missingColors.Any())
            throw new ArgumentException($"Colors not found: {string.Join(',', missingColors)}");

        clothingItem.UserId = userId;
        var created = await _repo.AddAsync(clothingItem, cancellationToken);
        return created;
    }

    public async Task UpdateAsync(Guid id, ClothingItem clothingItem, Guid userId, CancellationToken cancellationToken = default)
    {
        var existing = await _repo.GetByIdAsync(id, cancellationToken);
        if (existing is null) throw new InvalidOperationException("Not found");
        if (existing.UserId != userId) throw new UnauthorizedAccessException("Not the owner");

        existing.Name = clothingItem.Name;
        existing.PhotoUrl = clothingItem.PhotoUrl;
        existing.CategoryId = clothingItem.CategoryId;
        existing.WarmthLevel = clothingItem.WarmthLevel;
        existing.Properties = clothingItem.Properties;
        existing.Styles = clothingItem.Styles;
        existing.Colors = clothingItem.Colors;

        await _repo.UpdateAsync(existing, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var existing = await _repo.GetByIdAsync(id, cancellationToken);
        if (existing is null) return;
        if (existing.UserId != userId) throw new UnauthorizedAccessException("Not the owner");

        await _repo.DeleteAsync(id, cancellationToken);
    }
}
