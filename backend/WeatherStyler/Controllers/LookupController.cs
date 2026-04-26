using Microsoft.AspNetCore.Mvc;
using WeatherStyler.Domain.Interfaces.Services;

namespace WeatherStyler.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LookupController : ControllerBase
{
    private readonly ILookupService _service;
    private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
    private readonly bool _isDevelopment;

    public LookupController(ILookupService service, Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        _service = service;
        _configuration = configuration;
        _isDevelopment = _configuration.GetValue<bool>("IsDevelopment");
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
    {
        try
        {
            var items = await _service.GetCategoriesAsync(cancellationToken);
            return Ok(items);
        }
        catch (Exception ex)
        {
            if (_isDevelopment) throw;
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("categories/{id}")]
    public async Task<IActionResult> GetCategory(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var item = await _service.GetCategoryByIdAsync(id, cancellationToken);
            if (item is null) return NotFound();
            return Ok(item);
        }
        catch (Exception ex)
        {
            if (_isDevelopment) throw;
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("styles")]
    public async Task<IActionResult> GetStyles(CancellationToken cancellationToken)
    {
        try
        {
            var items = await _service.GetStylesAsync(cancellationToken);
            return Ok(items);
        }
        catch (Exception ex)
        {
            if (_isDevelopment) throw;
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("styles/{id}")]
    public async Task<IActionResult> GetStyle(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var item = await _service.GetStyleByIdAsync(id, cancellationToken);
            if (item is null) return NotFound();
            return Ok(item);
        }
        catch (Exception ex)
        {
            if (_isDevelopment) throw;
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("colors")]
    public async Task<IActionResult> GetColors(CancellationToken cancellationToken)
    {
        try
        {
            var items = await _service.GetColorsAsync(cancellationToken);
            return Ok(items);
        }
        catch (Exception ex)
        {
            if (_isDevelopment) throw;
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("colors/{id}")]
    public async Task<IActionResult> GetColor(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var item = await _service.GetColorByIdAsync(id, cancellationToken);
            if (item is null) return NotFound();
            return Ok(item);
        }
        catch (Exception ex)
        {
            if (_isDevelopment) throw;
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
