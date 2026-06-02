using FluentAssertions;
using Moq;
using WeatherStyler.Application.Services;
using WeatherStyler.Domain.Entities;
using WeatherStyler.Domain.Entities.BuisnessLogic;
using WeatherStyler.Domain.Interfaces.Repositories;
using WeatherStyler.Domain.Interfaces.Services;
using Xunit;

namespace WeatherStyler.Tests.Services;

public class OutfitManagerServiceTests
{
    private readonly Mock<IProgramVariableRepository> _programVarsMock;
    private readonly Mock<IClothingItemRepository> _clothingRepoMock;
    private readonly Mock<ILookupRepository> _lookupRepoMock;
    private readonly Mock<IUsageHistoryRepository> _usageHistoryRepoMock;
    private readonly Mock<IWeatherService> _weatherServiceMock;
    private readonly Mock<IOutfitRepository> _outfitRepoMock;
    private readonly OutfitManagerService _sut;

    public OutfitManagerServiceTests()
    {
        _programVarsMock = new Mock<IProgramVariableRepository>();
        _clothingRepoMock = new Mock<IClothingItemRepository>();
        _lookupRepoMock = new Mock<ILookupRepository>();
        _usageHistoryRepoMock = new Mock<IUsageHistoryRepository>();
        _weatherServiceMock = new Mock<IWeatherService>();
        _outfitRepoMock = new Mock<IOutfitRepository>();

        _sut = new OutfitManagerService(
            _programVarsMock.Object,
            _clothingRepoMock.Object,
            _lookupRepoMock.Object,
            _usageHistoryRepoMock.Object,
            _weatherServiceMock.Object,
            _outfitRepoMock.Object
        );
    }

    [Fact]
    public async Task GenerateOutfitWithWeatherAsync_WhenWardrobeIsEmpty_ShouldReturnFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var weather = new WeatherDataForGeneration(15, false, false, true);

        _clothingRepoMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ClothingItem>());

        // Act
        var result = await _sut.GenerateOutfitWithWeatherAsync(userId, weather);

        // Assert
        result.Should().NotBeNull();
        result.Outfit.Should().BeNull();
        result.Warnings.Should().Contain("Szafa użytkownika jest pusta. Dodaj ubrania aby generować outfity.");
    }

    [Fact]
    public async Task GenerateOutfitWithWeatherAsync_WithWarmWeather_ShouldSelectOneLayer()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var weather = new WeatherDataForGeneration(25, false, false, true); // Warm weather

        var categoryCore = new Category { Name = "TShirt", LayerIndex = 1, ClothingSlots = new List<ClothingSlot> { new ClothingSlot { Name = "Core" } } };
        var categoryLegs = new Category { Name = "Shorts", LayerIndex = 1, ClothingSlots = new List<ClothingSlot> { new ClothingSlot { Name = "Nogi" } } };

        var tShirt = new ClothingItem { Id = Guid.NewGuid(), UserId = userId, Name = "White TShirt", Category = categoryCore, WarmthLevel = 1, Colors = new List<Color>(), Styles = new List<Style>(), Properties = new List<ClothingProperty>() };
        var shorts = new ClothingItem { Id = Guid.NewGuid(), UserId = userId, Name = "Blue Shorts", Category = categoryLegs, WarmthLevel = 1, Colors = new List<Color>(), Styles = new List<Style>(), Properties = new List<ClothingProperty>() };

        var wardrobe = new List<ClothingItem> { tShirt, shorts };

        _clothingRepoMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(wardrobe);

        _usageHistoryRepoMock.Setup(repo => repo.GetByDateRangeAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UsageHistory>());

        // Act
        var result = await _sut.GenerateOutfitWithWeatherAsync(userId, weather);

        // Assert
        result.Should().NotBeNull();
        result.Outfit.Should().NotBeNull();
        result.Outfit!.ClothingItems.Should().HaveCount(2);
        result.Outfit.ClothingItems.Should().Contain(tShirt);
        result.Outfit.ClothingItems.Should().Contain(shorts);
    }
    
    [Fact]
    public async Task GenerateOutfitWithWeatherAsync_WithColdWeather_ShouldSelectMultipleLayers()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var weather = new WeatherDataForGeneration(0, false, false, false); // Cold weather

        var categoryCore1 = new Category { Name = "TShirt", LayerIndex = 1, ClothingSlots = new List<ClothingSlot> { new ClothingSlot { Name = "Core" } } };
        var categoryCore2 = new Category { Name = "Sweater", LayerIndex = 2, ClothingSlots = new List<ClothingSlot> { new ClothingSlot { Name = "Core" } } };
        var categoryCore3 = new Category { Name = "Jacket", LayerIndex = 3, ClothingSlots = new List<ClothingSlot> { new ClothingSlot { Name = "Core" } } };
        var categoryLegs1 = new Category { Name = "Pants", LayerIndex = 1, ClothingSlots = new List<ClothingSlot> { new ClothingSlot { Name = "Nogi" } } };
        var categoryHead1 = new Category { Name = "Hat", LayerIndex = 1, ClothingSlots = new List<ClothingSlot> { new ClothingSlot { Name = "Głowa" } } };

        var tShirt = new ClothingItem { Id = Guid.NewGuid(), UserId = userId, Name = "White TShirt", Category = categoryCore1, WarmthLevel = 2, Colors = new List<Color>(), Styles = new List<Style>(), Properties = new List<ClothingProperty>() };
        var sweater = new ClothingItem { Id = Guid.NewGuid(), UserId = userId, Name = "Wool Sweater", Category = categoryCore2, WarmthLevel = 5, Colors = new List<Color>(), Styles = new List<Style>(), Properties = new List<ClothingProperty>() };
        var jacket = new ClothingItem { Id = Guid.NewGuid(), UserId = userId, Name = "Winter Jacket", Category = categoryCore3, WarmthLevel = 8, Colors = new List<Color>(), Styles = new List<Style>(), Properties = new List<ClothingProperty>() };
        var pants = new ClothingItem { Id = Guid.NewGuid(), UserId = userId, Name = "Jeans", Category = categoryLegs1, WarmthLevel = 5, Colors = new List<Color>(), Styles = new List<Style>(), Properties = new List<ClothingProperty>() };
        var hat = new ClothingItem { Id = Guid.NewGuid(), UserId = userId, Name = "Beanie", Category = categoryHead1, WarmthLevel = 3, Colors = new List<Color>(), Styles = new List<Style>(), Properties = new List<ClothingProperty>() };

        var wardrobe = new List<ClothingItem> { tShirt, sweater, jacket, pants, hat };

        _clothingRepoMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(wardrobe);

        _usageHistoryRepoMock.Setup(repo => repo.GetByDateRangeAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UsageHistory>());

        // Act
        var result = await _sut.GenerateOutfitWithWeatherAsync(userId, weather);

        // Assert
        result.Should().NotBeNull();
        result.Outfit.Should().NotBeNull();
        result.Outfit!.ClothingItems.Should().HaveCount(5);
        result.Outfit.ClothingItems.Should().Contain(new[] { tShirt, sweater, jacket, pants, hat });
    }

    [Fact]
    public async Task GenerateOutfitWithWeatherAsync_WithRainyWeather_ShouldPreferWaterproof()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var weather = new WeatherDataForGeneration(15, true, false, false); // Rain

        var categoryCore2 = new Category { Name = "Jacket", LayerIndex = 2, ClothingSlots = new List<ClothingSlot> { new ClothingSlot { Name = "Core" } } };
        var categoryCore1 = new Category { Name = "TShirt", LayerIndex = 1, ClothingSlots = new List<ClothingSlot> { new ClothingSlot { Name = "Core" } } };
        var categoryLegs1 = new Category { Name = "Pants", LayerIndex = 1, ClothingSlots = new List<ClothingSlot> { new ClothingSlot { Name = "Nogi" } } };

        var tShirt = new ClothingItem { Id = Guid.NewGuid(), UserId = userId, Name = "TShirt", Category = categoryCore1, WarmthLevel = 3, Colors = new List<Color>(), Styles = new List<Style>(), Properties = new List<ClothingProperty>() };
        var pants = new ClothingItem { Id = Guid.NewGuid(), UserId = userId, Name = "Pants", Category = categoryLegs1, WarmthLevel = 3, Colors = new List<Color>(), Styles = new List<Style>(), Properties = new List<ClothingProperty>() };
        
        var normalJacket = new ClothingItem { Id = Guid.NewGuid(), UserId = userId, Name = "Normal Jacket", Category = categoryCore2, WarmthLevel = 3, Colors = new List<Color>(), Styles = new List<Style>(), Properties = new List<ClothingProperty>() };
        var waterproofJacket = new ClothingItem { 
            Id = Guid.NewGuid(), 
            UserId = userId, 
            Name = "Raincoat", 
            Category = categoryCore2, 
            WarmthLevel = 3, 
            Colors = new List<Color>(), 
            Styles = new List<Style>(), 
            Properties = new List<ClothingProperty> { new ClothingProperty { Name = "Waterproof" } } 
        };

        var wardrobe = new List<ClothingItem> { tShirt, pants, normalJacket, waterproofJacket };

        _clothingRepoMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(wardrobe);

        _usageHistoryRepoMock.Setup(repo => repo.GetByDateRangeAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UsageHistory>());

        // Act
        var result = await _sut.GenerateOutfitWithWeatherAsync(userId, weather);

        // Assert
        result.Should().NotBeNull();
        result.Outfit.Should().NotBeNull();
        result.Outfit!.ClothingItems.Should().Contain(waterproofJacket);
        result.Outfit.ClothingItems.Should().NotContain(normalJacket);
    }
}
