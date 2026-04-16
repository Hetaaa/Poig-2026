namespace WeatherStyler.Application.Contracts;

public record ClothingItemDto(
    Guid Id,
    string Name,
    string? PhotoUrl,
    Guid CategoryId,
    int WarmthLevel,
    IEnumerable<Guid> StyleIds,
    IEnumerable<Guid> ColorIds,
    IEnumerable<ClothingPropertyDto> Properties
);
