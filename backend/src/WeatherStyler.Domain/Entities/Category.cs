using WeatherStyler.Domain.Common;
using WeatherStyler.Domain.Entities;

namespace WeatherStyler.Domain.Entities;

public class Category : DomainEntityBase
{
    public required string Name { get; set; }
    public int LayerIndex { get; set; }  // 1=Base, 2=Middle, 3=Outer

    public ICollection<ClothingItem> ClothingItems { get; set; } = new List<ClothingItem>();
    public ICollection<ClothingSlot> ClothingSlots { get; set; } = new List<ClothingSlot>();
}
