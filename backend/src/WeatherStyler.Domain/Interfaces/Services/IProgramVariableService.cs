namespace WeatherStyler.Domain.Interfaces.Services;

public interface IProgramVariableService
{
    Task<string?> GetValueAsync(string key, CancellationToken cancellationToken = default);
    Task<string?> GetValueAsync(string key, Guid userId, CancellationToken cancellationToken = default);
    Task SetValueAsync(string key, string value, CancellationToken cancellationToken = default);
    Task SetValueAsync(string key, string value, Guid userId, CancellationToken cancellationToken = default);
}
