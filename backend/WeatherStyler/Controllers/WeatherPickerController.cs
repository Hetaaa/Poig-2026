using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeatherStyler.Application.Services;
using WeatherStyler.Application.Contracts;

namespace WeatherStyler.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherPickerController : ControllerBase
{
    private readonly ProgramVariableService _vars;
    private readonly WeatherStyler.Application.Services.IUserService _userService;

    public WeatherPickerController(ProgramVariableService vars, WeatherStyler.Application.Services.IUserService userService)
    {
        _vars = vars;
        _userService = userService;
    }

    [HttpPost("pick")]
    [Authorize]
    public async Task<IActionResult> Pick([FromBody] LocationDto location, CancellationToken cancellationToken)
    {
        // save last picked location as program variable for current user
        var userId = _userService.GetUserId();
        await _vars.SetValueAsync("last_location_lat", location.Latitude.ToString(), userId, cancellationToken);
        await _vars.SetValueAsync("last_location_lon", location.Longitude.ToString(), userId, cancellationToken);

        // here you could trigger weather fetch job or return immediate result
        return NoContent();
    }

    [HttpGet("last")]
    public async Task<IActionResult> GetLast(CancellationToken cancellationToken)
    {
        var userId = _userService.GetUserIdOrNull();
        if (userId == null) return Unauthorized();

        var lat = await _vars.GetValueAsync("last_location_lat", userId.Value, cancellationToken);
        var lon = await _vars.GetValueAsync("last_location_lon", userId.Value, cancellationToken);
        if (lat is null || lon is null) return NotFound();
        return Ok(new { Latitude = lat, Longitude = lon });
    }
}
