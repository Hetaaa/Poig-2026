using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeatherStyler.Application.Services;
using WeatherStyler.Infrastructure.Services;

namespace WeatherStyler.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OutfitController : ControllerBase
{
    private readonly OutfitService _outfitService;
    private readonly IUserService _userService;
    private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
    private readonly bool _isDevelopment;

    public OutfitController(OutfitService outfitService, IUserService userService, Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        _outfitService = outfitService;
        _userService = userService;
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

            var outfits = await _outfitService.GetOutfitsAsync(userId, from, to, cancellationToken);
            return Ok(outfits);
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
            var today = DateTime.UtcNow.Date;
            var raw = await _outfitService.GetOutfitsAsync(userId, today, today.AddDays(1).AddTicks(-1), cancellationToken);
            return Ok(raw);
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
            var outfits = await _outfitService.GetFavouriteOutfitsAsync(userId, cancellationToken);
            return Ok(outfits);
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

            var outfits = await _outfitService.GetFavouriteOutfitsAsync(userId, from, to, cancellationToken);
            return Ok(outfits);
        }
        catch (Exception ex)
        {
            if (_isDevelopment) throw;
            return StatusCode(500, new { message = ex.Message });
        }
    }

    /// <summary>
    /// Toggle favourite status for a usage history entry (outfit worn on specific day)
    /// </summary>
    [HttpPut("{usageHistoryId}/favourite")]
    public async Task<IActionResult> ToggleFavourite(
        Guid usageHistoryId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = _userService.GetUserId();
            var result = await _outfitService.ToggleFavouriteAsync(usageHistoryId, userId, cancellationToken);

            if (!result)
                return NotFound(new { message = "Usage history not found" });

            return NoContent();
        }
        catch (Exception ex)
        {
            if (_isDevelopment) throw;
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
