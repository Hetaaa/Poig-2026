namespace WeatherStyler.Domain.Common;

public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
}
