using Microsoft.AspNetCore.Mvc;
using WeatherStyler.Application.Services;

namespace WeatherStyler.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LookupController : ControllerBase
{
    private readonly WeatherStyler.Application.Services.LookupService _service;

    public LookupController(WeatherStyler.Application.Services.LookupService service)
    {
        _service = service;
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
    {
        var items = await _service.GetCategoriesAsync(cancellationToken);
        return Ok(items);
    }

    [HttpGet("categories/{id}")]
    public async Task<IActionResult> GetCategory(Guid id, CancellationToken cancellationToken)
    {
        var item = await _service.GetCategoryByIdAsync(id, cancellationToken);
        if (item is null) return NotFound();
        return Ok(item);
    }

    [HttpGet("styles")]
    public async Task<IActionResult> GetStyles(CancellationToken cancellationToken)
    {
        var items = await _service.GetStylesAsync(cancellationToken);
        return Ok(items);
    }

    [HttpGet("styles/{id}")]
    public async Task<IActionResult> GetStyle(Guid id, CancellationToken cancellationToken)
    {
        var item = await _service.GetStyleByIdAsync(id, cancellationToken);
        if (item is null) return NotFound();
        return Ok(item);
    }

    [HttpGet("colors")]
    public async Task<IActionResult> GetColors(CancellationToken cancellationToken)
    {
        var items = await _service.GetColorsAsync(cancellationToken);
        return Ok(items);
    }

    [HttpGet("colors/{id}")]
    public async Task<IActionResult> GetColor(Guid id, CancellationToken cancellationToken)
    {
        var item = await _service.GetColorByIdAsync(id, cancellationToken);
        if (item is null) return NotFound();
        return Ok(item);
    }
}
