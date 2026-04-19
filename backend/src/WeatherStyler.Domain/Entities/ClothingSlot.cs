using WeatherStyler.Domain.Common;
using WeatherStyler.Domain.Entities;

namespace WeatherStyler.Domain.Entities;

public class ClothingSlot : DomainEntityBase
{
    public required string Name { get; set; }

    public ICollection<Category> Categories { get; set; } = new List<Category>();
}
