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
    private readonly IWebHostEnvironment _env; // Wstrzykujemy środowisko webowe (zamiast IConfiguration)
    private readonly bool _isDevelopment;

    public ClothingItemsController(
        IClothingItemService service,
        IUserService userService,
        IMapper mapper,
        IWebHostEnvironment env)
    {
        _service = service;
        _userService = userService;
        _mapper = mapper;
        _env = env;
        _isDevelopment = env.IsDevelopment();
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
    public async Task<IActionResult> Create([FromForm] CreateClothingItemRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _userService.GetUserId();
            var clothingItem = _mapper.Map<ClothingItem>(request);

            if (request.StyleIds != null && request.StyleIds.Any())
            {
                // Tworzymy "puste" obiekty z samym ID, żeby Entity Framework wiedział, co powiązać
                clothingItem.Styles = request.StyleIds.Select(id => new Style { Id = id, Name = ""}).ToList();
            }

            if (request.ColorIds != null && request.ColorIds.Any())
            {
                clothingItem.Colors = request.ColorIds.Select(id => new Color { Id = id, Name = ""}).ToList();
            }

            // --- LOGIKA ZAPISU PLIKU ---
            if (request.PhotoFile != null && request.PhotoFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.ContentRootPath, "wwwroot", "images");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + request.PhotoFile.FileName.Replace(" ", "-");
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await request.PhotoFile.CopyToAsync(fileStream);
                }

                clothingItem.PhotoUrl = $"/images/{uniqueFileName}";
            }

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