using Microsoft.AspNetCore.Http;

namespace WeatherStyler.Application.Dtos;

public record CreateClothingItemRequest(
    string Name,
    IFormFile? PhotoFile,
    Guid CategoryId,
    int WarmthLevel,
    IEnumerable<Guid>? StyleIds,
    IEnumerable<Guid>? ColorIds,
    IEnumerable<ClothingPropertyDto>? Properties
);
