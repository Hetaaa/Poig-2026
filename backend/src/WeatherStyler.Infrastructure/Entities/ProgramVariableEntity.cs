using System;

namespace WeatherStyler.Infrastructure.Entities;

public class ProgramVariableEntity
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public Guid UserId { get; set; }
}
