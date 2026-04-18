using WeatherStyler.Domain.Common;

namespace WeatherStyler.Domain.Wardrobe.Entities;

public class Color : DomainEntityBase
{
    public required string Name { get; set; }
    public bool IsNeutral { get; set; }  // True for neutral colors (black, white, gray, beige)

    public ICollection<ClothingItem> ClothingItems { get; set; } = new List<ClothingItem>();
}
