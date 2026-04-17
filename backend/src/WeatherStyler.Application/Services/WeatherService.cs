using System.Net.Http.Json;
using System.Net.Http.Json;
using System.Net.Http;
using WeatherStyler.Domain.Repositories;

namespace WeatherStyler.Application.Services;

public class WeatherService
{
    private readonly IProgramVariableRepository _vars;
    private readonly IWeatherHistoryRepository _repo;
    private readonly HttpClient _http = new HttpClient();

    public WeatherService(IProgramVariableRepository vars, IWeatherHistoryRepository repo)
    {
        _vars = vars;
        _repo = repo;
    }

    public async Task FetchAndSaveForUser(Guid userId, Guid usageHistoryId, CancellationToken cancellationToken = default)
    {
        var lat = await _vars.GetValueAsync("last_location_lat", userId, cancellationToken);
        var lon = await _vars.GetValueAsync("last_location_lon", userId, cancellationToken);
        if (lat is null || lon is null) return;

        var url = $"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}&daily=temperature_2m_max,temperature_2m_min,precipitation_sum&timezone=UTC&forecast_days=4";
        var resp = await _http.GetAsync(url, cancellationToken);
        resp.EnsureSuccessStatusCode();
        var json = await resp.Content.ReadAsStringAsync(cancellationToken);

        await _repo.AddAsync(usageHistoryId, json, DateTime.UtcNow, cancellationToken);
    }
}
