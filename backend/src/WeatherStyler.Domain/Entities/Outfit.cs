using WeatherStyler.Domain.Common;

namespace WeatherStyler.Domain.Wardrobe.Entities;

public class Outfit : DomainEntityBase
{
    public required string Name { get; set; }
    public string? WeatherCondition { get; set; }
    public DateTime DateCreated { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }

    public ICollection<ClothingItem> ClothingItems { get; set; } = new List<ClothingItem>();
}
