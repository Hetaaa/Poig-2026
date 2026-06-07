using Microsoft.AspNetCore.Mvc;
using WeatherStyler.Application.Profiles;

namespace WeatherStyler.Contracts;

public record CreateClothingItemRequest(
    string Name,
    string? PhotoUrl,
    Guid CategoryId,
    int WarmthLevel,
    [ModelBinder(typeof(GuidArrayBinder))]
    IEnumerable<Guid>? StyleIds,
    [ModelBinder(typeof(GuidArrayBinder))]
    IEnumerable<Guid>? ColorIds,
    IEnumerable<ClothingPropertyDto>? Properties
);
