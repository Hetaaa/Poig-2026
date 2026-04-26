
namespace WeatherStyler.Application.Dtos;

public record ClothingItemDto(
    Guid Id,
    string Name,
    string? PhotoUrl,
    Guid CategoryId,
    int WarmthLevel,
    IEnumerable<StyleDto> Styles,
    IEnumerable<ColorDto> Colors,
    IEnumerable<ClothingPropertyDto> Properties
);
