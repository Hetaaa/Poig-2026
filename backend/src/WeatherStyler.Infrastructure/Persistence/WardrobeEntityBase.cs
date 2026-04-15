using WeatherStyler.Domain.Common;

namespace WeatherStyler.Infrastructure.Persistence;

public abstract class WardrobeEntityBase : IAuditable, ISoftDeletable
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
