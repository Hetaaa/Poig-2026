using WeatherStyler.Domain.Repositories;

namespace WeatherStyler.Application.Services;

public class ProgramVariableService
{
    private readonly IProgramVariableRepository _repo;

    public ProgramVariableService(IProgramVariableRepository repo)
    {
        _repo = repo;
    }

    public async Task<string?> GetValueAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _repo.GetValueAsync(key, cancellationToken);
    }

    public async Task<string?> GetValueAsync(string key, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _repo.GetValueAsync(key, userId, cancellationToken);
    }

    public async Task SetValueAsync(string key, string value, CancellationToken cancellationToken = default)
    {
        await _repo.SetValueAsync(key, value, cancellationToken);
    }

    public async Task SetValueAsync(string key, string value, Guid userId, CancellationToken cancellationToken = default)
    {
        await _repo.SetValueAsync(key, value, userId, cancellationToken);
    }
}
