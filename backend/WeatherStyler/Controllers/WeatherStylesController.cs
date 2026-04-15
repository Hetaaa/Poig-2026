using Microsoft.AspNetCore.Mvc;
using WeatherStyler.Application.Services;
using WeatherStyler.Contracts;

namespace WeatherStyler.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherStylesController(IWeatherStyleService weatherStyleService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var result = await weatherStyleService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateWeatherStyleRequest request, CancellationToken cancellationToken)
    {
        var created = await weatherStyleService.CreateAsync(request.Name, request.ThemeColor, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }
}
