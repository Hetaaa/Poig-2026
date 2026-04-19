using WeatherStyler.Domain.Common;
using WeatherStyler.Domain.Entities;

namespace WeatherStyler.Domain.Entities;

public class ClothingItem : DomainEntityBase
{
    public required string Name { get; set; }
    public string? PhotoUrl { get; set; }

    public Guid CategoryId { get; set; }
    public Guid UserId { get; set; }

    public Category? Category { get; set; }
    public User? User { get; set; }
    // General warmth level for the item (1-10)
    public int WarmthLevel { get; set; }

    public ICollection<ClothingProperty> Properties { get; set; } = new List<ClothingProperty>();
    public ICollection<Style> Styles { get; set; } = new List<Style>();
    public ICollection<Color> Colors { get; set; } = new List<Color>();
    public ICollection<Outfit> Outfits { get; set; } = new List<Outfit>();
}
