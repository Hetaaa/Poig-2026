namespace WeatherStyler.Domain.Entities;

public class ProgramVariable
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    // owner of the variable - per-user scope
    public Guid UserId { get; set; }
}
