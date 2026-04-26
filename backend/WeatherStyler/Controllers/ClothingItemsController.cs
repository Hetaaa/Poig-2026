using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeatherStyler.Application.Dtos;
using WeatherStyler.Domain.Entities;
using WeatherStyler.Domain.Interfaces.Services;

namespace WeatherStyler.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClothingItemsController : ControllerBase
{
    private readonly IClothingItemService _service;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
    private readonly bool _isDevelopment;

    public ClothingItemsController(IClothingItemService service, IUserService userService, IMapper mapper, IConfiguration configuration)
    {
        _service = service;
        _userService = userService;
        _mapper = mapper;
        _configuration = configuration;
        _isDevelopment = _configuration.GetValue<bool>("IsDevelopment");
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var userId = _userService.GetUserId();
            var items = await _service.GetAllAsync(userId, cancellationToken);
            var dtos = items.Select(item => _mapper.Map<ClothingItemDto>(item));
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
            var item = await _service.GetByIdAsync(id, userId, cancellationToken);
            if (item is null) return NotFound();
            var dto = _mapper.Map<ClothingItemDto>(item);
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
            var clothingItem = _mapper.Map<ClothingItem>(request);
            var created = await _service.CreateAsync(clothingItem, userId, cancellationToken);
            var dto = _mapper.Map<ClothingItemDto>(created);
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
            var clothingItem = _mapper.Map<ClothingItem>(request);
            await _service.UpdateAsync(id, clothingItem, userId, cancellationToken);
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
