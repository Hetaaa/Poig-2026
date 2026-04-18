using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeatherStyler.Application.Contracts;
using WeatherStyler.Application.Services;

namespace WeatherStyler.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClothingItemsController : ControllerBase
{
    private readonly IClothingItemService _service;
    private readonly WeatherStyler.Application.Services.IUserService _userService;

    public ClothingItemsController(WeatherStyler.Application.Services.IClothingItemService service, WeatherStyler.Application.Services.IUserService userService)
    {
        _service = service;
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var userId = _userService.GetUserId();
        var dtos = await _service.GetAllAsync(userId, cancellationToken);
        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var userId = _userService.GetUserId();
        var dto = await _service.GetByIdAsync(id, userId, cancellationToken);
        if (dto is null) return NotFound();
        return Ok(dto);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateClothingItemRequest request, CancellationToken cancellationToken)
    {
        var userId = _userService.GetUserId();
        var dto = await _service.CreateAsync(request, userId, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateClothingItemRequest request, CancellationToken cancellationToken)
    {
        var userId = _userService.GetUserId();
        await _service.UpdateAsync(id, request, userId, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var userId = _userService.GetUserId();
        await _service.DeleteAsync(id, userId, cancellationToken);
        return NoContent();
    }

}
