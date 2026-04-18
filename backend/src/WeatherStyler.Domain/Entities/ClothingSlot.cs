using WeatherStyler.Domain.Common;

namespace WeatherStyler.Domain.Wardrobe.Entities;

public class ClothingSlot : DomainEntityBase
{
    public required string Name { get; set; }

    public ICollection<Category> Categories { get; set; } = new List<Category>();
}
