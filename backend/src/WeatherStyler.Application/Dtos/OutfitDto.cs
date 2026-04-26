using System;
using System.Collections.Generic;

namespace WeatherStyler.Application.Dtos;


public record OutfitClothingItemDto(
    Guid Id,
    string Name,
    string? PhotoUrl,
    Guid CategoryId,
    int WarmthLevel,
    IEnumerable<StyleDto> Styles,
    IEnumerable<ColorDto> Colors,
    IEnumerable<ClothingPropertyDto> Properties
);

public record OutfitDto(
    string Name,
    IEnumerable<OutfitClothingItemDto> ClothingItems
);
