using WeatherStyler.Domain.Common;

namespace WeatherStyler.Domain.Entities;

public class UsageHistory : DomainEntityBase
{
    public int Rating { get; set; }
    public DateTime DateWorn { get; set; }
    public bool IsFavourite { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }

    // Usage history references an outfit worn on that day
    public Guid? OutfitId { get; set; }
    public Outfit? Outfit { get; set; }

    // Legacy: kept for backward compatibility, but UsageHistory now works with Outfit
    public ICollection<ClothingItem> ClothingItems { get; set; } = new List<ClothingItem>();
}
