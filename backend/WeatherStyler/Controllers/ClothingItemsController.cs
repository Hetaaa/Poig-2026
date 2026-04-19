using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeatherStyler.Application.Contracts;
using WeatherStyler.Application.Services;

namespace WeatherStyler.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClothingItemsController : ControllerBase
{
    private readonly IClothingItemService _service;
    private readonly WeatherStyler.Application.Services.IUserService _userService;
    private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
    private readonly bool _isDevelopment;

    public ClothingItemsController(WeatherStyler.Application.Services.IClothingItemService service, WeatherStyler.Application.Services.IUserService userService, Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        _service = service;
        _userService = userService;
        _configuration = configuration;
        _isDevelopment = _configuration.GetValue<bool>("IsDevelopment");
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var userId = _userService.GetUserId();
            var dtos = await _service.GetAllAsync(userId, cancellationToken);
            return Ok(dtos);
        }
        catch (Exception ex)
        {
            if (_isDevelopment) throw;
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _userService.GetUserId();
            var dto = await _service.GetByIdAsync(id, userId, cancellationToken);
            if (dto is null) return NotFound();
            return Ok(dto);
        }
        catch (Exception ex)
        {
            if (_isDevelopment) throw;
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateClothingItemRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _userService.GetUserId();
            var dto = await _service.CreateAsync(request, userId, cancellationToken);
            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
        }
        catch (Exception ex)
        {
            if (_isDevelopment) throw;
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateClothingItemRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _userService.GetUserId();
            await _service.UpdateAsync(id, request, userId, cancellationToken);
            return NoContent();
        }
        catch (Exception ex)
        {
            if (_isDevelopment) throw;
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _userService.GetUserId();
            await _service.DeleteAsync(id, userId, cancellationToken);
            return NoContent();
        }
        catch (Exception ex)
        {
            if (_isDevelopment) throw;
            return StatusCode(500, new { message = ex.Message });
        }
    }

}
