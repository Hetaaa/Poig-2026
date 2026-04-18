using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using WeatherStyler.Infrastructure.Entities;
using WeatherStyler.Application.Services;
using WeatherStyler.Domain.Repositories;
using WeatherStyler.Application.Contracts;

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
    private readonly WeatherStyler.Application.Services.WeatherService _weatherService;

    public DebugController(
        WeatherStyler.Infrastructure.Persistence.AppDbContext db,
        InitialValuesService initialValues,
        IClothingItemService clothingService,
        IClothingAttributesRepository attributesRepo,
        UserManager<ApplicationUser> userManager,
        WeatherStyler.Application.Services.WeatherService weatherService)
    {
        _db = db;
        _initialValues = initialValues;
        _clothingService = clothingService;
        _attributesRepo = attributesRepo;
        _userManager = userManager;
        _weatherService = weatherService;
    }

    [HttpPost("reset")]
    public IActionResult ResetDatabase()
    {
        // Drop and recreate database (development only)
        _db.Database.EnsureDeleted();
        _db.Database.Migrate();
        return Ok(new { reset = true });
    }

    [HttpPost("seed")]
    public async Task<IActionResult> Seed()
    {
        await _initialValues.SeedAsync();
        return Ok(new { seeded = true });
    }

    [HttpPost("mock-clothes")]
    public async Task<IActionResult> SeedMockClothes([FromQuery] string userName = "debug@local")
    {
        // ensure user exists
        var user = await _userManager.FindByNameAsync(userName) ?? new ApplicationUser { UserName = userName };
        if (user.Id == Guid.Empty)
        {
            var pw = "Password123!";
            var res = await _userManager.CreateAsync(user, pw);
            if (!res.Succeeded) return BadRequest(res.Errors.Select(e => e.Description));
        }

        var userId = user.Id;

        // get lookups
        var categories = await _attributesRepo.GetAllCategoriesAsync();
        var styles = await _attributesRepo.GetAllStylesAsync();
        var colors = await _attributesRepo.GetAllColorsAsync();

        Guid? catTshirt = categories.FirstOrDefault(c => c.Name == "T-shirt")?.Id;
        Guid? catJeans = categories.FirstOrDefault(c => c.Name == "Jeansy")?.Id;
        Guid? catCap = categories.FirstOrDefault(c => c.Name == "Czapka")?.Id;
        Guid? catSweater = categories.FirstOrDefault(c => c.Name == "Sweter")?.Id;
        Guid? catJacket = categories.FirstOrDefault(c => c.Name == "Kurtka")?.Id;

        var styleCasual = styles.FirstOrDefault(s => s.Name == "Casual")?.Id;
        var styleSport = styles.FirstOrDefault(s => s.Name == "Sportowy")?.Id;

        var colorBlack = colors.FirstOrDefault(c => c.Name == "Czarny")?.Id;
        var colorBlue = colors.FirstOrDefault(c => c.Name == "Granatowy")?.Id;

        var created = new List<WeatherStyler.Application.Contracts.ClothingItemDto>();

        if (catTshirt.HasValue)
        {
            var req = new CreateClothingItemRequest("Basic Tee", null, catTshirt.Value, 3, new[] { styleCasual }.Where(x => x.HasValue).Select(x => x.Value), new[] { colorBlack }.Where(x => x.HasValue).Select(x => x.Value), Enumerable.Empty<WeatherStyler.Application.Contracts.ClothingPropertyDto>());
            var dto = await _clothingService.CreateAsync(req, userId);
            created.Add(dto);
        }

        if (catJeans.HasValue)
        {
            var req = new CreateClothingItemRequest("Blue Jeans", null, catJeans.Value, 5, new[] { styleCasual }.Where(x => x.HasValue).Select(x => x.Value), new[] { colorBlue }.Where(x => x.HasValue).Select(x => x.Value), Enumerable.Empty<WeatherStyler.Application.Contracts.ClothingPropertyDto>());
            var dto = await _clothingService.CreateAsync(req, userId);
            created.Add(dto);
        }

        if (catCap.HasValue)
        {
            var req = new CreateClothingItemRequest("Cap", null, catCap.Value, 1, new[] { styleSport }.Where(x => x.HasValue).Select(x => x.Value), new[] { colorBlack }.Where(x => x.HasValue).Select(x => x.Value), Enumerable.Empty<WeatherStyler.Application.Contracts.ClothingPropertyDto>());
            var dto = await _clothingService.CreateAsync(req, userId);
            created.Add(dto);
        }

        if (catSweater.HasValue)
        {
            var req = new CreateClothingItemRequest("Wool Sweater", null, catSweater.Value, 7, new[] { styleCasual }.Where(x => x.HasValue).Select(x => x.Value), new[] { colorBlack }.Where(x => x.HasValue).Select(x => x.Value), Enumerable.Empty<WeatherStyler.Application.Contracts.ClothingPropertyDto>());
            var dto = await _clothingService.CreateAsync(req, userId);
            created.Add(dto);
        }

        if (catJacket.HasValue)
        {
            var req = new CreateClothingItemRequest("Light Jacket", null, catJacket.Value, 8, new[] { styleCasual }.Where(x => x.HasValue).Select(x => x.Value), new[] { colorBlue }.Where(x => x.HasValue).Select(x => x.Value), Enumerable.Empty<WeatherStyler.Application.Contracts.ClothingPropertyDto>());
            var dto = await _clothingService.CreateAsync(req, userId);
            created.Add(dto);
        }

        // create an outfit using created items
        if (created.Any())
        {
            var outfit = new OutfitEntity { Id = Guid.NewGuid(), Name = "Debug Outfit", DateCreated = DateTime.UtcNow, UserId = userId };
            foreach (var it in created)
            {
                var ci = await _db.ClothingItems.FindAsync(new object[] { it.Id });
                if (ci != null) outfit.ClothingItems.Add(ci);
            }

            _db.Outfits.Add(outfit);
            await _db.SaveChangesAsync();

            // create usage history entry for today and fetch weather
            var usage = new UsageHistoryEntity { Id = Guid.NewGuid(), DateWorn = DateTime.UtcNow.Date, Rating = 5, UserId = userId, OutfitId = outfit.Id };
            _db.UsageHistories.Add(usage);
            await _db.SaveChangesAsync();

            // fetch weather for this usage entry (uses user's saved location)
            try
            {
                await _weatherService.FetchAndSaveForUser(userId, usage.Id);
            }
            catch
            {
                // ignore errors in debug seed
            }
        }

        return Ok(new { createdCount = created.Count, items = created });
    }
}
