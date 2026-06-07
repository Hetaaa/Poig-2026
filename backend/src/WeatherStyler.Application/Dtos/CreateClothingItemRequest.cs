using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeatherStyler.Application.Profiles;

namespace WeatherStyler.Application.Dtos;

public record CreateClothingItemRequest(
    string Name,
    IFormFile? PhotoFile,
    Guid CategoryId,
    int WarmthLevel,
    [ModelBinder(typeof(GuidArrayBinder))]
    IEnumerable<Guid>? StyleIds,
    [ModelBinder(typeof(GuidArrayBinder))]
    IEnumerable<Guid>? ColorIds,
    [ModelBinder(typeof(JsonModelBinder))]
    IEnumerable<ClothingPropertyDto>? Properties
);
