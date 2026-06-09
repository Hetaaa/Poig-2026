using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WeatherStyler.Application.Services;
using WeatherStyler.Domain.Entities;
using WeatherStyler.Domain.Entities.BuisnessLogic;
using WeatherStyler.Domain.Interfaces.Repositories;
using WeatherStyler.Domain.Interfaces.Services;
using Xunit;
using Color = WeatherStyler.Domain.Entities.Color;

namespace WeatherStyler.Tests
{
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
            var configurationMock = new Mock<IConfiguration>();

            _sut = new OutfitManagerService(
                _programVarsMock.Object,
                _clothingRepoMock.Object,
                _lookupRepoMock.Object,
                _usageHistoryRepoMock.Object,
                _weatherServiceMock.Object,
                _outfitRepoMock.Object,
                configurationMock.Object
            );

            // Domyślna konfiguracja dla historii użycia — czysta historia
            _usageHistoryRepoMock.Setup(repo => repo.GetByDateRangeAsync(
                    It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<UsageHistory>());
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
        public async Task GenerateOutfitWithWeatherAsync_WithColdWeather_ShouldSelectMultipleLayers()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var weather = new WeatherDataForGeneration(5, false, false, false); // Zimno (Wymaga Core l:1,2,3, Nogi l:1 oraz Głowa l:1)

            var slotCore = new ClothingSlot { Name = "Core" };
            var slotLegs = new ClothingSlot { Name = "Nogi" };
            var slotHead = new ClothingSlot { Name = "Głowa" };

            var catCore1 = new Category { Name = "TShirt", LayerIndex = 1, ClothingSlots = new List<ClothingSlot> { slotCore } };
            var catCore2 = new Category { Name = "Sweater", LayerIndex = 2, ClothingSlots = new List<ClothingSlot> { slotCore } };
            var catCore3 = new Category { Name = "Jacket", LayerIndex = 3, ClothingSlots = new List<ClothingSlot> { slotCore } };
            var catLegs1 = new Category { Name = "Pants", LayerIndex = 1, ClothingSlots = new List<ClothingSlot> { slotLegs } };
            var catHead1 = new Category { Name = "Hat", LayerIndex = 1, ClothingSlots = new List<ClothingSlot> { slotHead } };

            var wardrobe = new List<ClothingItem>
            {
                new() { Id = Guid.NewGuid(), UserId = userId, Name = "Basic Tee", Category = catCore1, WarmthLevel = 4, Colors = new List<Color>(), Styles = new List<Style>(), Properties = new List<ClothingProperty>() },
                new() { Id = Guid.NewGuid(), UserId = userId, Name = "Wool Sweater", Category = catCore2, WarmthLevel = 4, Colors = new List<Color>(), Styles = new List<Style>(), Properties = new List<ClothingProperty>() },
                new() { Id = Guid.NewGuid(), UserId = userId, Name = "Winter Coat", Category = catCore3, WarmthLevel = 4, Colors = new List<Color>(), Styles = new List<Style>(), Properties = new List<ClothingProperty>() },
                new() { Id = Guid.NewGuid(), UserId = userId, Name = "Jeans", Category = catLegs1, WarmthLevel = 5, Colors = new List<Color>(), Styles = new List<Style>(), Properties = new List<ClothingProperty>() },
                new() { Id = Guid.NewGuid(), UserId = userId, Name = "Beanie", Category = catHead1, WarmthLevel = 3, Colors = new List<Color>(), Styles = new List<Style>(), Properties = new List<ClothingProperty>() }
            };

            _clothingRepoMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(wardrobe);

            // Act
            var result = await _sut.GenerateOutfitWithWeatherAsync(userId, weather);

            // Assert
            result.Should().NotBeNull();
            result.Outfit.Should().NotBeNull();
            result.Outfit!.ClothingItems.Should().HaveCount(5);
            result.Outfit.ClothingItems.Should().ContainSingle(i => i.Name == "Basic Tee");
            result.Outfit.ClothingItems.Should().ContainSingle(i => i.Name == "Winter Coat");
            result.Outfit.ClothingItems.Should().ContainSingle(i => i.Name == "Beanie");
        }

        [Fact]
        public async Task GenerateOutfitWithWeatherAsync_WithRainyWeather_ShouldPreferWaterproofOnOuterLayer()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var weather = new WeatherDataForGeneration(15, true, false, false); // Deszczowo i umiarkowanie (Wymaga Core l:1,2 oraz Nogi l:1).

            var slotCore = new ClothingSlot { Name = "Core" };
            var slotLegs = new ClothingSlot { Name = "Nogi" };

            var catCore1 = new Category { Name = "TShirt", LayerIndex = 1, ClothingSlots = new List<ClothingSlot> { slotCore } };
            var catCore2 = new Category { Name = "Jacket", LayerIndex = 2, ClothingSlots = new List<ClothingSlot> { slotCore } };
            var catLegs1 = new Category { Name = "Pants", LayerIndex = 1, ClothingSlots = new List<ClothingSlot> { slotLegs } };

            var tShirt = new ClothingItem { Id = Guid.NewGuid(), UserId = userId, Name = "TShirt", Category = catCore1, WarmthLevel = 3, Colors = new List<Color>(), Styles = new List<Style>(), Properties = new List<ClothingProperty>() };
            var pants = new ClothingItem { Id = Guid.NewGuid(), UserId = userId, Name = "Pants", Category = catLegs1, WarmthLevel = 3, Colors = new List<Color>(), Styles = new List<Style>(), Properties = new List<ClothingProperty>() };
            var normalJacket = new ClothingItem { Id = Guid.NewGuid(), UserId = userId, Name = "Normal Jacket", Category = catCore2, WarmthLevel = 3, Colors = new List<Color>(), Styles = new List<Style>(), Properties = new List<ClothingProperty>() };

            var waterproofJacket = new ClothingItem
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = "Raincoat",
                Category = catCore2,
                WarmthLevel = 3,
                Colors = new List<Color>(),
                Styles = new List<Style>(),
                Properties = new List<ClothingProperty> { new ClothingProperty { Name = "Waterproof", Value = "True" } }
            };

            var wardrobe = new List<ClothingItem> { tShirt, pants, normalJacket, waterproofJacket };

            _clothingRepoMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(wardrobe);

            // Act
            var result = await _sut.GenerateOutfitWithWeatherAsync(userId, weather);

            // Assert
            result.Should().NotBeNull();
            result.Outfit.Should().NotBeNull();

            result.Outfit!.ClothingItems.Should().ContainSingle(item => item.Name == "Raincoat");
            result.Outfit.ClothingItems.Should().NotContain(item => item.Name == "Normal Jacket");
        }

        [Fact]
        public async Task GenerateOutfitForTodayAsync_WhenLocationIsMissing_ShouldReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _programVarsMock.Setup(pv => pv.GetValueAsync("last_location_lat", userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((string?)null);

            // Act
            var result = await _sut.GenerateOutfitForTodayAsync(userId);

            // Assert
            result.Outfit.Should().BeNull();
            result.Warnings.Should().Contain("Brak zapisanej lokacji użytkownika. Ustaw lokację aby generować outfity.");
        }

        [Fact]
        public async Task GenerateOutfitForTodayAsync_WhenLocationExists_ShouldCallWeatherServiceAndBuildOutfit()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var weatherData = new WeatherDataForGeneration(22, false, false, false);

            _programVarsMock.Setup(pv => pv.GetValueAsync("last_location_lat", userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync("50.44");
            _programVarsMock.Setup(pv => pv.GetValueAsync("last_location_lon", userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync("18.85");

            _weatherServiceMock.Setup(ws => ws.GetWeatherForLocationAsync(50.44, 18.85, It.IsAny<CancellationToken>()))
                .ReturnsAsync(weatherData);

            var catCore = new Category { Name = "T-Shirt", LayerIndex = 1, ClothingSlots = new List<ClothingSlot> { new ClothingSlot { Name = "Core" } } };
            var catLegs = new Category { Name = "Shorts", LayerIndex = 1, ClothingSlots = new List<ClothingSlot> { new ClothingSlot { Name = "Nogi" } } };

            var wardrobe = new List<ClothingItem>
            {
                new() { Id = Guid.NewGuid(), UserId = userId, Name = "Shirt", Category = catCore, Colors = new List<Color>(), Styles = new List<Style>(), Properties = new List<ClothingProperty>() },
                new() { Id = Guid.NewGuid(), UserId = userId, Name = "Shorts", Category = catLegs, Colors = new List<Color>(), Styles = new List<Style>(), Properties = new List<ClothingProperty>() }
            };

            _clothingRepoMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(wardrobe);

            // Act
            var result = await _sut.GenerateOutfitForTodayAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result.Outfit.Should().NotBeNull();
            _weatherServiceMock.Verify(ws => ws.GetWeatherForLocationAsync(50.44, 18.85, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}