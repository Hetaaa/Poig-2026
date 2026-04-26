namespace WeatherStyler.Application.Dtos;

public record CreateClothingItemRequest(
    string Name,
    string? PhotoUrl,
    Guid CategoryId,
    int WarmthLevel,
    IEnumerable<Guid>? StyleIds,
    IEnumerable<Guid>? ColorIds,
    IEnumerable<ClothingPropertyDto>? Properties
);
