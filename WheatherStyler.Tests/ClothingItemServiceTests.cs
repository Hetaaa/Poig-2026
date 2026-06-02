using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using WeatherStyler.Application.Services;
using WeatherStyler.Domain.Entities;
using WeatherStyler.Domain.Interfaces.Repositories;
using Xunit;

namespace WeatherStyler.Tests
{
    public class ClothingItemServiceTests
    {
        private readonly Mock<IClothingItemRepository> _repoMock;
        private readonly ClothingItemService _sut;

        public ClothingItemServiceTests()
        {
            _repoMock = new Mock<IClothingItemRepository>();
            _sut = new ClothingItemService(_repoMock.Object);
        }

        // --- TESTY DLA METODY GetAllAsync ---

        [Fact]
        public async Task GetAllAsync_ShouldReturnOnlyItemsBelongingToRequestedUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var anotherUserId = Guid.NewGuid();

            var userItems = new List<ClothingItem>
            {
                new() { Id = Guid.NewGuid(), UserId = userId, Name = "User's Shirt", Styles = new List<Style>(), Colors = new List<Color>() },
                new() { Id = Guid.NewGuid(), UserId = userId, Name = "User's Pants", Styles = new List<Style>(), Colors = new List<Color>() }
            };
            var mixedItems = new List<ClothingItem>
            {
                userItems[0],
                userItems[1],
                new() { Id = Guid.NewGuid(), UserId = anotherUserId, Name = "Stranger's Jacket", Styles = new List<Style>(), Colors = new List<Color>() }
            };

            _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(mixedItems);

            // Act
            var result = await _sut.GetAllAsync(userId);

            // Assert
            result.Should().HaveCount(2);
            result.Should().OnlyContain(i => i.UserId == userId);
            result.Should().NotContain(i => i.UserId == anotherUserId);
        }

        // --- TESTY DLA METODY GetByIdAsync ---

        [Fact]
        public async Task GetByIdAsync_WhenItemDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            _repoMock.Setup(r => r.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ClothingItem?)null);

            // Act
            var result = await _sut.GetByIdAsync(itemId, Guid.NewGuid());

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByIdAsync_WhenItemBelongsToAnotherUser_ShouldReturnNull()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();
            var requesterId = Guid.NewGuid();
            var item = new ClothingItem
            {
                Id = itemId,
                UserId = ownerId,
                Name = "Secret Dress",
                Styles = new List<Style>(),
                Colors = new List<Color>()
            };

            _repoMock.Setup(r => r.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);

            // Act
            var result = await _sut.GetByIdAsync(itemId, requesterId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByIdAsync_WhenItemExistsAndBelongsToUser_ShouldReturnItem()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var item = new ClothingItem
            {
                Id = itemId,
                UserId = userId,
                Name = "My Cap",
                Styles = new List<Style>(),
                Colors = new List<Color>()
            };

            _repoMock.Setup(r => r.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);

            // Act
            var result = await _sut.GetByIdAsync(itemId, userId);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("My Cap");
        }

        // --- TESTY DLA METODY CreateAsync (Walidacja) ---

        [Theory]
        [InlineData("")]
        [InlineData("    ")]
        [InlineData(null)]
        public async Task CreateAsync_WhenNameIsMissing_ShouldThrowArgumentException(string invalidName)
        {
            // Arrange
            var item = new ClothingItem
            {
                Name = invalidName,
                WarmthLevel = 5,
                Styles = new List<Style>(),
                Colors = new List<Color>()
            };

            // Act
            Func<Task> act = async () => await _sut.CreateAsync(item, Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<ArgumentException>().WithMessage("Name is required");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(11)]
        [InlineData(-5)]
        public async Task CreateAsync_WhenWarmthLevelIsOutOfRange_ShouldThrowArgumentOutOfRangeException(int invalidWarmth)
        {
            // Arrange
            var item = new ClothingItem
            {
                Name = "Valid Name",
                WarmthLevel = invalidWarmth,
                Styles = new List<Style>(),
                Colors = new List<Color>()
            };

            // Act
            Func<Task> act = async () => await _sut.CreateAsync(item, Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<ArgumentOutOfRangeException>().WithParameterName("WarmthLevel");
        }

        [Fact]
        public async Task CreateAsync_WhenCategoryDoesNotExist_ShouldThrowArgumentException()
        {
            // Arrange
            var item = new ClothingItem
            {
                Name = "Sweater",
                WarmthLevel = 5,
                CategoryId = Guid.NewGuid(),
                Styles = new List<Style>(),
                Colors = new List<Color>()
            };

            _repoMock.Setup(r => r.CategoryExistsAsync(item.CategoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            Func<Task> act = async () => await _sut.CreateAsync(item, Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithParameterName("CategoryId")
                .WithMessage("Category does not exist*");
        }

        [Fact]
        public async Task CreateAsync_WhenStylesOrColorsAreMissingInDb_ShouldThrowArgumentException()
        {
            // Arrange
            var item = new ClothingItem
            {
                Name = "Coat",
                WarmthLevel = 8,
                CategoryId = Guid.NewGuid(),
                Styles = new List<Style> { new() { Id = Guid.NewGuid(), Name = "Test Style" } },
                Colors = new List<Color> { new() { Id = Guid.NewGuid(), Name = "Test Color" } }
            };

            _repoMock.Setup(r => r.CategoryExistsAsync(item.CategoryId, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            _repoMock.Setup(r => r.FindMissingStyleIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Guid> { item.Styles.First().Id });

            // Act
            Func<Task> act = async () => await _sut.CreateAsync(item, Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<ArgumentException>().WithMessage("Styles not found*");
        }

        [Fact]
        public async Task CreateAsync_WhenDataIsValid_ShouldAssignUserIdAndSave()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var item = new ClothingItem
            {
                Name = "Valid Jacket",
                WarmthLevel = 6,
                CategoryId = Guid.NewGuid(),
                Styles = new List<Style>(),
                Colors = new List<Color>()
            };

            _repoMock.Setup(r => r.CategoryExistsAsync(item.CategoryId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _repoMock.Setup(r => r.FindMissingStyleIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<Guid>());
            _repoMock.Setup(r => r.FindMissingColorIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<Guid>());

            _repoMock.Setup(r => r.AddAsync(item, It.IsAny<CancellationToken>())).ReturnsAsync(item);

            // Act
            var result = await _sut.CreateAsync(item, userId);

            // Assert
            result.Should().NotBeNull();
            result.UserId.Should().Be(userId);
            _repoMock.Verify(r => r.AddAsync(item, It.IsAny<CancellationToken>()), Times.Once);
        }

        // --- TESTY DLA METODY UpdateAsync ---

        [Fact]
        public async Task UpdateAsync_WhenItemDoesNotExist_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            _repoMock.Setup(r => r.GetByIdAsync(itemId, It.IsAny<CancellationToken>())).ReturnsAsync((ClothingItem?)null);

            // Act
            Func<Task> act = async () => await _sut.UpdateAsync(itemId, new ClothingItem { Name = "Placeholder", Styles = new List<Style>(), Colors = new List<Color>() }, Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Not found");
        }

        [Fact]
        public async Task UpdateAsync_WhenUserIsNotTheOwner_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();
            var hackerId = Guid.NewGuid();
            var existingItem = new ClothingItem
            {
                Id = itemId,
                UserId = ownerId,
                Name = "Original Name",
                Styles = new List<Style>(),
                Colors = new List<Color>()
            };

            _repoMock.Setup(r => r.GetByIdAsync(itemId, It.IsAny<CancellationToken>())).ReturnsAsync(existingItem);

            // Act
            Func<Task> act = async () => await _sut.UpdateAsync(itemId, new ClothingItem { Name = "Hacked Name", Styles = new List<Style>(), Colors = new List<Color>() }, hackerId);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>().WithMessage("Not the owner");
        }

        [Fact]
        public async Task UpdateAsync_WhenUserIsOwner_ShouldMapPropertiesAndCallUpdate()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var existingItem = new ClothingItem
            {
                Id = itemId,
                UserId = userId,
                Name = "Old Name",
                WarmthLevel = 2,
                Styles = new List<Style>(),
                Colors = new List<Color>()
            };
            var updatedData = new ClothingItem
            {
                Name = "New Name",
                WarmthLevel = 5,
                Properties = new List<ClothingProperty>(),
                Styles = new List<Style>(),
                Colors = new List<Color>()
            };

            _repoMock.Setup(r => r.GetByIdAsync(itemId, It.IsAny<CancellationToken>())).ReturnsAsync(existingItem);

            // Act
            await _sut.UpdateAsync(itemId, updatedData, userId);

            // Assert
            existingItem.Name.Should().Be("New Name");
            existingItem.WarmthLevel.Should().Be(5);
            _repoMock.Verify(r => r.UpdateAsync(existingItem, It.IsAny<CancellationToken>()), Times.Once);
        }

        // --- TESTY DLA METODY DeleteAsync ---

        [Fact]
        public async Task DeleteAsync_WhenItemDoesNotExist_ShouldReturnSafelyWithoutCallingDelete()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            _repoMock.Setup(r => r.GetByIdAsync(itemId, It.IsAny<CancellationToken>())).ReturnsAsync((ClothingItem?)null);

            // Act
            Func<Task> act = async () => await _sut.DeleteAsync(itemId, Guid.NewGuid());

            // Assert
            await act.Should().NotThrowAsync();
            _repoMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WhenUserIsNotOwner_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();
            var hackerId = Guid.NewGuid();
            var existingItem = new ClothingItem
            {
                Id = itemId,
                UserId = ownerId,
                Name = "Anonymous Item",
                Styles = new List<Style>(),
                Colors = new List<Color>()
            };

            _repoMock.Setup(r => r.GetByIdAsync(itemId, It.IsAny<CancellationToken>())).ReturnsAsync(existingItem);

            // Act
            Func<Task> act = async () => await _sut.DeleteAsync(itemId, hackerId);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>().WithMessage("Not the owner");
            _repoMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WhenUserIsOwner_ShouldCallDeleteInRepository()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var existingItem = new ClothingItem
            {
                Id = itemId,
                UserId = userId,
                Name = "My Item",
                Styles = new List<Style>(),
                Colors = new List<Color>()
            };

            _repoMock.Setup(r => r.GetByIdAsync(itemId, It.IsAny<CancellationToken>())).ReturnsAsync(existingItem);

            // Act
            await _sut.DeleteAsync(itemId, userId);

            // Assert
            _repoMock.Verify(r => r.DeleteAsync(itemId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}