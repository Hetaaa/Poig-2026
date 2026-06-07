using Microsoft.AspNetCore.Mvc;
using WeatherStyler.Application.Dtos;
using WeatherStyler.Application.Profiles;

namespace WeatherStyler.Contracts;

public record CreateClothingItemRequest(
    string Name,
    string? PhotoUrl,
    Guid CategoryId,
    int WarmthLevel,
    [ModelBinder(typeof(JsonModelBinder))]
    IEnumerable<Guid>? StyleIds,
    [ModelBinder(typeof(JsonModelBinder))]
    IEnumerable<Guid>? ColorIds,
    [ModelBinder(typeof(JsonModelBinder))]
    IEnumerable<ClothingPropertyDto>? Properties
);
