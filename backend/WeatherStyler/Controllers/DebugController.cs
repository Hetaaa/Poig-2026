using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using WeatherStyler.Infrastructure.Entities;
using WeatherStyler.Application.Services;
using WeatherStyler.Domain.Repositories;
using WeatherStyler.Application.Contracts;
using WeatherStyler.Infrastructure.Services;

namespace WeatherStyler.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DebugController : ControllerBase
{
    private readonly WeatherStyler.Infrastructure.Persistence.AppDbContext _db;
    private readonly InitialValuesService _initialValues;
    private readonly IClothingItemService _clothingService;
    private readonly IClothingAttributesRepository _attributesRepo;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly OutfitGeneratorService _outfitGenerator;
    private readonly IUserService _userService;
    private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
    private readonly bool _isDevelopment;

    public DebugController(
        WeatherStyler.Infrastructure.Persistence.AppDbContext db,
        InitialValuesService initialValues,
        IClothingItemService clothingService,
        IClothingAttributesRepository attributesRepo,
        UserManager<ApplicationUser> userManager,
        OutfitGeneratorService outfitGenerator,
        IUserService userService,
        Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        _db = db;
        _initialValues = initialValues;
        _clothingService = clothingService;
        _attributesRepo = attributesRepo;
        _userManager = userManager;
        _outfitGenerator = outfitGenerator;
        _userService = userService;
        _configuration = configuration;
        _isDevelopment = _configuration.GetValue<bool>("IsDevelopment");
    }

    [HttpGet("user/{id}")]
    [Authorize]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        // try UserManager lookup (applies identity/query filters)
        var mgrUser = await _userManager.FindByIdAsync(id.ToString());

        // direct DB lookup ignoring global query filters (to detect soft-deleted users)
        var dbUser = await _db.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == id);

        return Ok(new
        {
            requestedId = id,
            foundByUserManager = mgrUser != null,
            userManager = mgrUser is null ? null : new { mgrUser.Id, mgrUser.UserName },
            foundInDb = dbUser != null,
            dbUser = dbUser is null ? null : new { dbUser.Id, dbUser.UserName, dbUser.IsDeleted }
        });
    }

    [HttpPost("reset")]
    public IActionResult ResetDatabase()
    {
        try
        {
            // Drop and recreate database (development only)
            _db.Database.EnsureDeleted();
            _db.Database.Migrate();
            return Ok(new { reset = true });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("clear-clothing-data")]
    [Authorize]
    public async Task<IActionResult> ClearClothingData(CancellationToken cancellationToken = default)
    {
        if (!_isDevelopment)
            return Forbid();

        try
        {
            // Remove usage histories first (they reference outfits)
            _db.UsageHistories.RemoveRange(_db.UsageHistories);
            // Remove outfits (will clear join table entries)
            _db.Outfits.RemoveRange(_db.Outfits);
            // Remove clothing items
            _db.ClothingItems.RemoveRange(_db.ClothingItems);

            await _db.SaveChangesAsync(cancellationToken);
            return Ok(new { cleared = true });
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
        {
            var msg = FlattenException(ex);
            return StatusCode(500, new { message = msg });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        // Return current HttpContext user claims and decoded JWT (if present) to help debugging
        var user = HttpContext.User;
        var claims = user.Claims.Select(c => new { c.Type, c.Value }).ToList();
        var authHeader = HttpContext.Request.Headers["Authorization"].ToString();

        object jwtClaims = null;
        if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            var token = authHeader.Substring("Bearer ".Length).Trim();
            try
            {
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                if (handler.CanReadToken(token))
                {
                    var jwt = handler.ReadJwtToken(token);
                    jwtClaims = jwt.Claims.Select(c => new { c.Type, c.Value }).ToList();
                }
            }
            catch
            {
                // ignore
            }
        }

        return Ok(new { IsAuthenticated = user.Identity?.IsAuthenticated ?? false, Claims = claims, AuthorizationHeader = authHeader, JwtClaims = jwtClaims });
    }

    [HttpPost("seed")]
    public async Task<IActionResult> Seed()
    {
        try
        {
            await _initialValues.SeedAsync();
            return Ok(new { seeded = true });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("mock-clothes")]
    [Authorize]
    public async Task<IActionResult> SeedMockClothes(CancellationToken cancellationToken = default)
    {
        try
        {
            // only for logged-in user
            var userId = _userService.GetUserId();

            // Ensure the current user exists in the database (foreign keys require it)
            var appUser = await _userManager.FindByIdAsync(userId.ToString());
            if (appUser is null)
            {
                return BadRequest(new { message = "Current user not found in database. Ensure the user account exists before creating mock data." });
            }

            // get lookups from DB
            var categories = (await _attributesRepo.GetAllCategoriesAsync(cancellationToken)).ToList();
            var styles = (await _attributesRepo.GetAllStylesAsync(cancellationToken)).ToList();
            var colors = (await _attributesRepo.GetAllColorsAsync(cancellationToken)).ToList();
            var properties = (await _attributesRepo.GetAllPropertiesAsync(cancellationToken)).ToList();

            // Ensure categories have sensible LayerIndex and colors have IsNeutral set
            var categoryEntities = await _db.Categories.ToListAsync(cancellationToken);
            var colorEntities = await _db.Colors.ToListAsync(cancellationToken);

            // simple mapping rules for layer index: outer (3), middle (2), base (1)
            string[] outerKeywords = new[] { "Kurtka", "P³aszcz", "Marynarka" };
            string[] middleKeywords = new[] { "Sweter", "Bluza", "Kardigan", "Kamizelka" };
            var neutralColorNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Czarny", "Bia³y", "Szary", "Be¿owy", "Br¹zowy" };

            foreach (var ce in categoryEntities)
            {
                if (ce.LayerIndex == 0)
                {
                    if (outerKeywords.Any(k => ce.Name.Contains(k, StringComparison.OrdinalIgnoreCase))) ce.LayerIndex = 3;
                    else if (middleKeywords.Any(k => ce.Name.Contains(k, StringComparison.OrdinalIgnoreCase))) ce.LayerIndex = 2;
                    else ce.LayerIndex = 1;
                }
            }

            foreach (var col in colorEntities)
            {
                if (!col.IsNeutral)
                {
                    if (neutralColorNames.Contains(col.Name)) col.IsNeutral = true;
                }
            }

            try
            {
                await _db.SaveChangesAsync(cancellationToken);
            }
            catch
            {
                // ignore errors here - non-critical for mock creation
            }

            // ensure there is an 'Oczy' slot and an 'Okulary' category for sunglasses
            var eyesSlot = await _attributesRepo.GetClothingSlotByNameAsync("Oczy", cancellationToken);
            if (eyesSlot is null)
            {
                var createdSlot = await _attributesRepo.AddClothingSlotAsync(new WeatherStyler.Domain.Entities.ClothingSlot { Name = "Oczy" }, cancellationToken);
                eyesSlot = createdSlot;
                categories = (await _attributesRepo.GetAllCategoriesAsync(cancellationToken)).ToList();
            }

            var glassesCategory = (await _attributesRepo.GetAllCategoriesAsync(cancellationToken)).FirstOrDefault(c => c.Name == "Okulary" || c.Name == "Okulary przeciws³oneczne");
            if (glassesCategory is null)
            {
                var createdCat = await _attributesRepo.AddCategoryAsync(new WeatherStyler.Domain.Entities.Category { Name = "Okulary" , LayerIndex = 1}, cancellationToken);
                await _attributesRepo.AssociateCategoryWithSlotsAsync(createdCat.Id, new[] { eyesSlot.Id }, cancellationToken);
                // refresh categories list
                categories = (await _attributesRepo.GetAllCategoriesAsync(cancellationToken)).ToList();
                glassesCategory = categories.FirstOrDefault(c => c.Id == createdCat.Id);
            }

            if (!categories.Any() || !styles.Any() || !colors.Any())
                return BadRequest(new { message = "Not enough lookup data (categories/styles/colors) in database to create mock items" });

            var rng = new Random();
            var created = new List<WeatherStyler.Application.Contracts.ClothingItemDto>();

            // create 300 mock items using existing lookups
            const int targetCount = 300;
            for (int i = 0; i < targetCount; i++)
            {
                var cat = categories[rng.Next(categories.Count)];
                var styleIds = styles.OrderBy(_ => rng.Next()).Take(Math.Min(2, styles.Count)).Select(s => s.Id).ToList();
                var colorIds = colors.OrderBy(_ => rng.Next()).Take(Math.Min(2, colors.Count)).Select(c => c.Id).ToList();
                // add extra properties randomly (waterproof/windproof)
                var propDtos = new List<ClothingPropertyDto>(properties.OrderBy(_ => rng.Next()).Take(Math.Min(2, properties.Count)).Select(p => new ClothingPropertyDto(p.Name, p.Value)));
                if (rng.NextDouble() < 0.3) propDtos.Add(new ClothingPropertyDto("waterproof", "true"));
                if (rng.NextDouble() < 0.2) propDtos.Add(new ClothingPropertyDto("windproof", "true"));

                var req = new CreateClothingItemRequest(
                    Name: $"{cat.Name} Mock Item {i + 1}",
                    PhotoUrl: null,
                    CategoryId: cat.Id,
                    WarmthLevel: rng.Next(1, 11),
                    StyleIds: styleIds,
                    ColorIds: colorIds,
                    Properties: propDtos
                );

                var dto = await _clothingService.CreateAsync(req, userId, cancellationToken);
                created.Add(dto);
            }

            // Add one sunglasses item in the 'Oczy' slot for the user
            try
            {
                if (glassesCategory != null)
                {
                    var sunglassesReq = new CreateClothingItemRequest(
                        Name: "Okulary przeciws³oneczne - Mock",
                        PhotoUrl: null,
                        CategoryId: glassesCategory.Id,
                        WarmthLevel: 1,
                        StyleIds: new List<Guid>(),
                        ColorIds: new List<Guid>(),
                        Properties: new List<ClothingPropertyDto> { new ClothingPropertyDto("is_sunglasses", "true") }
                    );

                    var sunDto = await _clothingService.CreateAsync(sunglassesReq, userId, cancellationToken);
                    created.Add(sunDto);
                }
            }
            catch
            {
                // ignore sunglasses creation errors
            }

            if (created.Any())
            {
                try
                {
                    await _db.SaveChangesAsync(cancellationToken);
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException saveEx)
                {
                    var msg = FlattenException(saveEx);
                    return StatusCode(500, new { message = msg, operation = "saving usage history" });
                }
            }

            return Ok(new { createdCount = created.Count, items = created });
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
        {
            var msg = FlattenException(dbEx);
            return StatusCode(500, new { message = msg });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    private static string FlattenException(Exception ex)
    {
        var parts = new List<string>();
        var cur = ex;
        while (cur != null)
        {
            parts.Add(cur.Message);
            cur = cur.InnerException;
        }
        return string.Join(" -> ", parts);
    }

    /// <summary>
    /// Generate outfit for today based on user's location and current weather
    /// </summary>
    [HttpPost("generate-outfit-today")]
    [Authorize]
    public async Task<IActionResult> GenerateOutfitForToday(CancellationToken cancellationToken = default)
    {
        try
        {
            // Get current logged-in user
            var userId = _userService.GetUserId();

            // Generate outfit (will fetch location and weather automatically)
            var outfitResult = await _outfitGenerator.GenerateOutfitForTodayAsync(userId, cancellationToken);

            if (outfitResult.Outfit == null)
                return BadRequest(new
                {
                    message = "Could not generate outfit",
                    warnings = outfitResult.Warnings,
                    temperature = outfitResult.Temperature,
                    isWindy = outfitResult.IsWindy,
                    isSunny = outfitResult.IsSunny,
                    isRaining = outfitResult.IsRaining
                });

            // Return generated outfit now (was placeholder note previously)
                // map outfit to DTO with trimmed fields (no created/user metadata)
                var outfitDto = new
                {
                    name = outfitResult.Outfit?.Name,
                    clothingItems = outfitResult.Outfit?.ClothingItems.Select(ci => new
                    {
                        name = ci.Name,
                        photoUrl = ci.PhotoUrl,
                        categoryId = ci.CategoryId,
                        warmthLevel = ci.WarmthLevel,
                        properties = ci.Properties.Select(p => new { name = p.Name, value = p.Value }),
                        styles = ci.Styles.Select(s => new { id = s.Id, name = s.Name }),
                        colors = ci.Colors.Select(c => new { id = c.Id, name = c.Name, isNeutral = c.IsNeutral })
                    })
                };

                return Ok(new
                {
                    message = "Outfit generated",
                    outfit = outfitDto,
                    warnings = outfitResult.Warnings
                });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
