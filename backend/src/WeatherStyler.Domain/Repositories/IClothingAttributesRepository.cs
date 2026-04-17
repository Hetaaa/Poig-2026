using WeatherStyler.Domain.Wardrobe.Entities;

namespace WeatherStyler.Domain.Repositories;

public interface IClothingAttributesRepository
{
    // Categories
    Task<IReadOnlyList<Category>> GetAllCategoriesAsync(CancellationToken cancellationToken = default);
    Task<Category?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Category> AddCategoryAsync(Category category, CancellationToken cancellationToken = default);
    Task UpdateCategoryAsync(Category category, CancellationToken cancellationToken = default);
    Task DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default);

    // Styles
    Task<IReadOnlyList<Style>> GetAllStylesAsync(CancellationToken cancellationToken = default);
    Task<Style?> GetStyleByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Style> AddStyleAsync(Style style, CancellationToken cancellationToken = default);
    Task UpdateStyleAsync(Style style, CancellationToken cancellationToken = default);
    Task DeleteStyleAsync(Guid id, CancellationToken cancellationToken = default);

    // Colors
    Task<IReadOnlyList<Color>> GetAllColorsAsync(CancellationToken cancellationToken = default);
    Task<Color?> GetColorByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Color> AddColorAsync(Color color, CancellationToken cancellationToken = default);
    Task UpdateColorAsync(Color color, CancellationToken cancellationToken = default);
    Task DeleteColorAsync(Guid id, CancellationToken cancellationToken = default);

    // Clothing properties
    Task<IReadOnlyList<ClothingProperty>> GetAllPropertiesAsync(CancellationToken cancellationToken = default);
    Task<ClothingProperty?> GetPropertyByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ClothingProperty> AddPropertyAsync(ClothingProperty property, CancellationToken cancellationToken = default);
    Task UpdatePropertyAsync(ClothingProperty property, CancellationToken cancellationToken = default);
    Task DeletePropertyAsync(Guid id, CancellationToken cancellationToken = default);

    // Clothing slots
    Task<IReadOnlyList<ClothingSlot>> GetAllClothingSlotsAsync(CancellationToken cancellationToken = default);
    Task<ClothingSlot?> GetClothingSlotByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<ClothingSlot> AddClothingSlotAsync(ClothingSlot slot, CancellationToken cancellationToken = default);
    Task AssociateCategoryWithSlotsAsync(Guid categoryId, IEnumerable<Guid> slotIds, CancellationToken cancellationToken = default);
}
