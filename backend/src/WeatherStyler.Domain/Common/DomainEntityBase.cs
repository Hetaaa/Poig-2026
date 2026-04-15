namespace WeatherStyler.Domain.Common;

public abstract class DomainEntityBase : IAuditable, ISoftDeletable
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
