using WeatherStyler.Application.Dtos;

namespace WeatherStyler;

public record OutfitItemDto(
    Guid Id,
    string Name,
    int Rating,
    DateTime DateWorn,
    bool IsFavourite,
    Guid? OutfitId,
    List<ClothingItemDto> Items
);
