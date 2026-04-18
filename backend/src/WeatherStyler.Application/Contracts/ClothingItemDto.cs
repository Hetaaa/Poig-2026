namespace WeatherStyler.Application.Contracts;

public record StylePreviewDto(Guid Id, string Name);
public record ColorPreviewDto(Guid Id, string Name, bool IsNeutral);

public record ClothingItemDto(
    Guid Id,
    string Name,
    string? PhotoUrl,
    Guid CategoryId,
    int WarmthLevel,
    IEnumerable<StylePreviewDto> Styles,
    IEnumerable<ColorPreviewDto> Colors,
    IEnumerable<ClothingPropertyDto> Properties
);
