using WeatherStyler.Domain.Common;

namespace WeatherStyler.Domain.Wardrobe.Entities;

public class Color : DomainEntityBase
{
    public required string Name { get; set; }

    public ICollection<ClothingItem> ClothingItems { get; set; } = new List<ClothingItem>();
}
