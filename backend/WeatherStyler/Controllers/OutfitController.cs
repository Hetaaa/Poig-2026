using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeatherStyler.Application.Dtos;
using WeatherStyler.Domain.Interfaces.Services;
using WeatherStyler.Domain.Interfaces.Repositories;

namespace WeatherStyler.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OutfitController : ControllerBase
{
    private readonly IOutfitRepository _outfitRepository;
    private readonly IUserService _userService;
    private readonly IOutfitManagerService _outfitManager;
    private readonly IMapper _mapper;
    private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
    private readonly bool _isDevelopment;

    public OutfitController(
        IOutfitRepository outfitRepository,
        IUserService userService,
        IOutfitManagerService outfitManager,
        IMapper mapper,
        Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        _outfitRepository = outfitRepository;
        _userService = userService;
        _outfitManager = outfitManager;
        _mapper = mapper;
        _configuration = configuration;
        _isDevelopment = _configuration.GetValue<bool>("IsDevelopment");
    }

    /// <summary>
    /// Get outfits for user within date range
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetOutfits(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = _userService.GetUserId();

            if (from > to)
                return BadRequest(new { message = "From date must be before To date" });

            var outfits = await _outfitRepository.GetOutfitsAsync(userId, from, to, cancellationToken);
            var dtos = outfits.Select(o => _mapper.Map<OutfitDto>(o));
            return Ok(dtos);
        }
        catch (Exception ex)
        {
            if (_isDevelopment) throw;
            return StatusCode(500, new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get outfits worn today
    /// </summary>
    [HttpGet("today")]
    public async Task<IActionResult> GetOutfitsForToday(CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = _userService.GetUserId();

            // delegate to outfit manager
            var result = await _outfitManager.GetOrGenerateTodayAsync(userId, cancellationToken);
            if (result.Outfit == null)
                return BadRequest(new { message = "Could not generate outfit", warnings = result.Warnings });

            var dto = _mapper.Map<OutfitDto>(result.Outfit);
            return Ok(new { outfit = dto, warnings = result.Warnings });
        }
        catch (Exception ex)
        {
            if (_isDevelopment) throw;
            return StatusCode(500, new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get all favourite outfits for current user
    /// </summary>
    [HttpGet("favourite")]
    public async Task<IActionResult> GetFavouriteOutfits(CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = _userService.GetUserId();
            var outfits = await _outfitRepository.GetFavouriteOutfitsAsync(userId, cancellationToken);
            var dtos = outfits.Select(o => _mapper.Map<OutfitDto>(o));
            return Ok(dtos);
        }
        catch (Exception ex)
        {
            if (_isDevelopment) throw;
            return StatusCode(500, new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get favourite outfits for user within date range
    /// </summary>
    [HttpGet("favourite/range")]
    public async Task<IActionResult> GetFavouriteOutfits(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = _userService.GetUserId();

            if (from > to)
                return BadRequest(new { message = "From date must be before To date" });

            var outfits = await _outfitRepository.GetFavouriteOutfitsAsync(userId, from, to, cancellationToken);
            var dtos = outfits.Select(o => _mapper.Map<OutfitDto>(o));
            return Ok(dtos);
        }
        catch (Exception ex)
        {
            if (_isDevelopment) throw;
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
