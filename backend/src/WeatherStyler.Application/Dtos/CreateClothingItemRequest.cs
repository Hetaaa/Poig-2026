using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeatherStyler.Application.Profiles;

namespace WeatherStyler.Application.Dtos;

public record CreateClothingItemRequest(
    string Name,
    IFormFile? PhotoFile,
    Guid CategoryId,
    int WarmthLevel,
    [ModelBinder(typeof(JsonModelBinder))]
    IEnumerable<Guid>? StyleIds,
    [ModelBinder(typeof(JsonModelBinder))]
    IEnumerable<Guid>? ColorIds,
    [ModelBinder(typeof(JsonModelBinder))]
    IEnumerable<ClothingPropertyDto>? Properties
);
