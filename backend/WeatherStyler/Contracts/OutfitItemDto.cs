using WeatherStyler.Application.Contracts;

namespace WeatherStyler.Contracts;

public record OutfitItemDto(
    Guid Id,
    string Name,
    int Rating,
    DateTime DateWorn,
    bool IsFavourite,
    Guid? OutfitId,
    List<ClothingItemDto> Items
);
