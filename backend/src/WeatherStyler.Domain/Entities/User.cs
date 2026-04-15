using WeatherStyler.Domain.Common;

namespace WeatherStyler.Domain.Wardrobe.Entities;

public class User : DomainEntityBase
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }

    public ICollection<ClothingItem> ClothingItems { get; set; } = new List<ClothingItem>();
    public ICollection<Outfit> Outfits { get; set; } = new List<Outfit>();
    public ICollection<UsageHistory> UsageHistories { get; set; } = new List<UsageHistory>();
}
