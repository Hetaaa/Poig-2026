using WeatherStyler.Domain.Common;

namespace WeatherStyler.Domain.Entities;

public class Style : DomainEntityBase
{
    public required string Name { get; set; }

    public ICollection<ClothingItem> ClothingItems { get; set; } = new List<ClothingItem>();
}
