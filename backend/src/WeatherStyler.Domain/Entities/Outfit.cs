using WeatherStyler.Domain.Common;

namespace WeatherStyler.Domain.Entities;

public class Outfit : DomainEntityBase
{
    public required string Name { get; set; }
    public DateTime DateCreated { get; set; }

    public User? User { get; set; }

    // Items that compose this outfit
    public ICollection<ClothingItem> ClothingItems { get; set; } = new List<ClothingItem>();

    // Usage history entries that reference this outfit (one outfit can be used on many days)
    public ICollection<UsageHistory> UsageHistories { get; set; } = new List<UsageHistory>();
}
