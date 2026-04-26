using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeatherStyler.Application.Dtos;
using WeatherStyler.Application.Services;
using WeatherStyler.Domain.Interfaces.Services;

namespace WeatherStyler.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherPickerController : ControllerBase
{
    private readonly IProgramVariableService _vars;
    private readonly IUserService _userService;
    private readonly IWeatherService _weatherService;
    private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
    private readonly bool _isDevelopment;

    public WeatherPickerController(
        IProgramVariableService vars,
        IUserService userService,
        IWeatherService weatherService,
        Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        _vars = vars;
        _userService = userService;
        _weatherService = weatherService;
        _configuration = configuration;
        _isDevelopment = _configuration.GetValue<bool>("IsDevelopment");
    }

    [HttpGet("daily/{date}")]
    [Authorize]
    public async Task<IActionResult> GetDailyWeather(DateTime date, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _userService.GetUserIdOrNull();
            if (userId == null) return Unauthorized();

            var lat = await _vars.GetValueAsync("last_location_lat", userId.Value, cancellationToken);
            var lon = await _vars.GetValueAsync("last_location_lon", userId.Value, cancellationToken);
            if (lat is null || lon is null) return BadRequest(new { message = "No location set" });

            var summary = await _weatherService.GetDailySummaryAsync(double.Parse(lat), double.Parse(lon), date, cancellationToken);
            if (summary is null) return StatusCode(502, new { message = "Could not fetch weather" });

            return Ok(new
            {
                date = summary.Date,
                temperature = summary.Temperature,
                feelsLike = summary.FeelsLike,
                humidity = summary.Humidity,
                windSpeed = summary.WindSpeed,
                precipitation = summary.Precipitation,
                cloudCover = summary.CloudCover
            });
        }
        catch (Exception ex)
        {
            if (_isDevelopment) throw;
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("pick")]
    [Authorize]
    public async Task<IActionResult> Pick([FromBody] LocationDto location, CancellationToken cancellationToken)
    {
        try
        {
            // save last picked location as program variable for current user
            var userId = _userService.GetUserId();
            await _vars.SetValueAsync("last_location_lat", location.Latitude.ToString(), userId, cancellationToken);
            await _vars.SetValueAsync("last_location_lon", location.Longitude.ToString(), userId, cancellationToken);

            // here you could trigger weather fetch job or return immediate result
            return NoContent();
        }
        catch (Exception ex)
        {
            if (_isDevelopment) throw;
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("last")]
    public async Task<IActionResult> GetLast(CancellationToken cancellationToken)
    {
        try
        {
            var userId = _userService.GetUserIdOrNull();
            if (userId == null) return Unauthorized();

            var lat = await _vars.GetValueAsync("last_location_lat", userId.Value, cancellationToken);
            var lon = await _vars.GetValueAsync("last_location_lon", userId.Value, cancellationToken);
            if (lat is null || lon is null) return NotFound();
            return Ok(new { Latitude = lat, Longitude = lon });
        }
        catch (Exception ex)
        {
            if (_isDevelopment) throw;
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
