using WeatherStyler.Domain.Common;

namespace WeatherStyler.Domain.Wardrobe.Entities;

public class WarmthRating : DomainEntityBase
{
    public int ArmsLevel { get; set; }
    public int CoreLevel { get; set; }
    public int LegsLevel { get; set; }

    public Guid ClothingItemId { get; set; }
    public ClothingItem? ClothingItem { get; set; }
}
