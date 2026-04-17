using WeatherStyler.Application.Contracts;
using WeatherStyler.Domain.Repositories;
using WeatherStyler.Domain.Wardrobe.Entities;

namespace WeatherStyler.Application.Services;

public class ClothingItemService : IClothingItemService
{
    private readonly IClothingItemRepository _repo;

    public ClothingItemService(IClothingItemRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<ClothingItemDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await _repo.GetAllAsync(cancellationToken);
        return items.Select(MapToDto);
    }

    public async Task<ClothingItemDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _repo.GetByIdAsync(id, cancellationToken);
        return item is null ? null : MapToDto(item);
    }

    public async Task<ClothingItemDto> CreateAsync(CreateClothingItemRequest request, Guid userId, CancellationToken cancellationToken = default)
    {
        // validation
        if (string.IsNullOrWhiteSpace(request.Name)) throw new ArgumentException("Name is required");
        if (request.WarmthLevel < 1 || request.WarmthLevel > 10) throw new ArgumentOutOfRangeException(nameof(request.WarmthLevel));

        // validate category
        if (!await _repo.CategoryExistsAsync(request.CategoryId, cancellationToken))
            throw new ArgumentException("Category does not exist", nameof(request.CategoryId));

        // validate styles & colors
        var styleIds = request.StyleIds ?? Enumerable.Empty<Guid>();
        var missingStyles = (await _repo.FindMissingStyleIdsAsync(styleIds, cancellationToken)).ToList();
        if (missingStyles.Any())
            throw new ArgumentException($"Styles not found: {string.Join(',', missingStyles)}");

        var colorIds = request.ColorIds ?? Enumerable.Empty<Guid>();
        var missingColors = (await _repo.FindMissingColorIdsAsync(colorIds, cancellationToken)).ToList();
        if (missingColors.Any())
            throw new ArgumentException($"Colors not found: {string.Join(',', missingColors)}");

        var item = new ClothingItem
        {
            Name = request.Name,
            PhotoUrl = request.PhotoUrl,
            CategoryId = request.CategoryId,
            WarmthLevel = request.WarmthLevel,
            UserId = userId,
            Properties = request.Properties?.Select(p => new ClothingProperty { Id = Guid.NewGuid(), Name = p.Name, Value = p.Value }).ToList() ?? new List<ClothingProperty>(),
            // populate minimal Style/Color placeholders (Name is required in domain so set empty string)
            Styles = request.StyleIds?.Select(id => new Style { Id = id, Name = string.Empty }).ToList() ?? new List<Style>(),
            Colors = request.ColorIds?.Select(id => new Color { Id = id, Name = string.Empty }).ToList() ?? new List<Color>()
        };

        var created = await _repo.AddAsync(item, cancellationToken);
        return MapToDto(created);
    }

    public async Task UpdateAsync(Guid id, CreateClothingItemRequest request, Guid userId, CancellationToken cancellationToken = default)
    {
        var existing = await _repo.GetByIdAsync(id, cancellationToken);
        if (existing is null) throw new InvalidOperationException("Not found");
        if (existing.UserId != userId) throw new UnauthorizedAccessException("Not the owner");

        existing.Name = request.Name;
        existing.PhotoUrl = request.PhotoUrl;
        existing.CategoryId = request.CategoryId;
        existing.WarmthLevel = request.WarmthLevel;
        existing.Properties = request.Properties?.Select(p => new ClothingProperty { Id = Guid.NewGuid(), Name = p.Name, Value = p.Value }).ToList() ?? new List<ClothingProperty>();
        existing.Styles = request.StyleIds?.Select(id => new Style { Id = id, Name = string.Empty }).ToList() ?? new List<Style>();
        existing.Colors = request.ColorIds?.Select(id => new Color { Id = id, Name = string.Empty }).ToList() ?? new List<Color>();

        await _repo.UpdateAsync(existing, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var existing = await _repo.GetByIdAsync(id, cancellationToken);
        if (existing is null) return;
        if (existing.UserId != userId) throw new UnauthorizedAccessException("Not the owner");

        await _repo.DeleteAsync(id, cancellationToken);
    }

    private ClothingItemDto MapToDto(ClothingItem item)
    {
        return new ClothingItemDto(
            item.Id,
            item.Name,
            item.PhotoUrl,
            item.CategoryId,
            item.WarmthLevel,
            item.Styles?.Select(s => s.Id) ?? Enumerable.Empty<Guid>(),
            item.Colors?.Select(c => c.Id) ?? Enumerable.Empty<Guid>(),
            item.Properties?.Select(p => new ClothingPropertyDto(p.Name, p.Value)) ?? Enumerable.Empty<ClothingPropertyDto>()
        );
    }
}
