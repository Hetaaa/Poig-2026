using WeatherStyler.Domain.Common;

namespace WeatherStyler.Domain.Entities;

public class UsageHistory : DomainEntityBase
{
    public int Rating { get; set; }
    public DateTime DateWorn { get; set; }
    public bool IsFavourite { get; set; }

    public User? User { get; set; }

    public Outfit? Outfit { get; set; }

}
