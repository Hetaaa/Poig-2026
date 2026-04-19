using WeatherStyler.Domain.Common;

namespace WeatherStyler.Domain.Entities;

public class ClothingProperty : DomainEntityBase
{
    public required string Name { get; set; }
    public required string Value { get; set; }

    public Guid ClothingItemId { get; set; }
    public ClothingItem? ClothingItem { get; set; }
}
