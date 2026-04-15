using WeatherStyler.Domain.Common;

namespace WeatherStyler.Domain.Wardrobe.Entities;

public class UsageHistory : DomainEntityBase
{
    public int Rating { get; set; }
    public DateTime DateWorn { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }

    public ICollection<ClothingItem> ClothingItems { get; set; } = new List<ClothingItem>();
}
