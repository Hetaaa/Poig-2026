using Microsoft.EntityFrameworkCore;
using WeatherStyler.Domain.Entities;
using WeatherStyler.Domain.Repositories;
using WeatherStyler.Domain.Entities;
using WeatherStyler.Infrastructure.Entities;
using WeatherStyler.Infrastructure.Persistence;

namespace WeatherStyler.Infrastructure.Repositories;

internal class ClothingAttributesRepository : IClothingAttributesRepository
{
    private readonly AppDbContext _db;

    public ClothingAttributesRepository(AppDbContext db)
    {
        _db = db;
    }

    // Categories
    public async Task<IReadOnlyList<Category>> GetAllCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Categories.AsNoTracking().Select(c => new Category { Id = c.Id, Name = c.Name, LayerIndex = c.LayerIndex }).ToListAsync(cancellationToken);
    }

    public async Task<Category?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var e = await _db.Categories.FindAsync(new object[] { id }, cancellationToken);
        if (e is null) return null;
        return new Category { Id = e.Id, Name = e.Name, LayerIndex = e.LayerIndex };
    }

    public async Task<Category> AddCategoryAsync(Category category, CancellationToken cancellationToken = default)
    {
        var e = new CategoryEntity { Id = category.Id == Guid.Empty ? Guid.NewGuid() : category.Id, Name = category.Name, LayerIndex = category.LayerIndex };
        _db.Categories.Add(e);
        await _db.SaveChangesAsync(cancellationToken);
        category.Id = e.Id;
        return category;
    }

    public async Task UpdateCategoryAsync(Category category, CancellationToken cancellationToken = default)
    {
        var e = await _db.Categories.FindAsync(new object[] { category.Id }, cancellationToken);
        if (e is null) throw new InvalidOperationException("Not found");
        e.Name = category.Name;
        e.LayerIndex = category.LayerIndex;
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var e = await _db.Categories.FindAsync(new object[] { id }, cancellationToken);
        if (e is null) return;
        _db.Categories.Remove(e);
        await _db.SaveChangesAsync(cancellationToken);
    }

    // Styles
    public async Task<IReadOnlyList<Style>> GetAllStylesAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Styles.AsNoTracking().Select(s => new Style { Id = s.Id, Name = s.Name }).ToListAsync(cancellationToken);
    }

    public async Task<Style?> GetStyleByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var e = await _db.Styles.FindAsync(new object[] { id }, cancellationToken);
        if (e is null) return null;
        return new Style { Id = e.Id, Name = e.Name };
    }

    public async Task<Style> AddStyleAsync(Style style, CancellationToken cancellationToken = default)
    {
        var e = new StyleEntity { Id = style.Id == Guid.Empty ? Guid.NewGuid() : style.Id, Name = style.Name };
        _db.Styles.Add(e);
        await _db.SaveChangesAsync(cancellationToken);
        style.Id = e.Id;
        return style;
    }

    public async Task UpdateStyleAsync(Style style, CancellationToken cancellationToken = default)
    {
        var e = await _db.Styles.FindAsync(new object[] { style.Id }, cancellationToken);
        if (e is null) throw new InvalidOperationException("Not found");
        e.Name = style.Name;
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteStyleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var e = await _db.Styles.FindAsync(new object[] { id }, cancellationToken);
        if (e is null) return;
        _db.Styles.Remove(e);
        await _db.SaveChangesAsync(cancellationToken);
    }

    // Colors
    public async Task<IReadOnlyList<Color>> GetAllColorsAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Colors.AsNoTracking().Select(c => new Color { Id = c.Id, Name = c.Name, IsNeutral = c.IsNeutral }).ToListAsync(cancellationToken);
    }

    public async Task<Color?> GetColorByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var e = await _db.Colors.FindAsync(new object[] { id }, cancellationToken);
        if (e is null) return null;
        return new Color { Id = e.Id, Name = e.Name, IsNeutral = e.IsNeutral };
    }

    public async Task<Color> AddColorAsync(Color color, CancellationToken cancellationToken = default)
    {
        var e = new ColorEntity { Id = color.Id == Guid.Empty ? Guid.NewGuid() : color.Id, Name = color.Name, IsNeutral = color.IsNeutral };
        _db.Colors.Add(e);
        await _db.SaveChangesAsync(cancellationToken);
        color.Id = e.Id;
        return color;
    }

    public async Task UpdateColorAsync(Color color, CancellationToken cancellationToken = default)
    {
        var e = await _db.Colors.FindAsync(new object[] { color.Id }, cancellationToken);
        if (e is null) throw new InvalidOperationException("Not found");
        e.Name = color.Name;
        e.IsNeutral = color.IsNeutral;
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteColorAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var e = await _db.Colors.FindAsync(new object[] { id }, cancellationToken);
        if (e is null) return;
        _db.Colors.Remove(e);
        await _db.SaveChangesAsync(cancellationToken);
    }

    // Clothing properties
    public async Task<IReadOnlyList<ClothingProperty>> GetAllPropertiesAsync(CancellationToken cancellationToken = default)
    {
        return await _db.ClothingProperties.AsNoTracking().Select(p => new ClothingProperty { Id = p.Id, Name = p.Name, Value = p.Value, ClothingItemId = p.ClothingItemId }).ToListAsync(cancellationToken);
    }

    public async Task<ClothingProperty?> GetPropertyByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var e = await _db.ClothingProperties.FindAsync(new object[] { id }, cancellationToken);
        if (e is null) return null;
        return new ClothingProperty { Id = e.Id, Name = e.Name, Value = e.Value, ClothingItemId = e.ClothingItemId };
    }

    public async Task<ClothingProperty> AddPropertyAsync(ClothingProperty property, CancellationToken cancellationToken = default)
    {
        var e = new ClothingPropertyEntity { Id = property.Id == Guid.Empty ? Guid.NewGuid() : property.Id, Name = property.Name, Value = property.Value, ClothingItemId = property.ClothingItemId };
        _db.ClothingProperties.Add(e);
        await _db.SaveChangesAsync(cancellationToken);
        property.Id = e.Id;
        return property;
    }

    public async Task UpdatePropertyAsync(ClothingProperty property, CancellationToken cancellationToken = default)
    {
        var e = await _db.ClothingProperties.FindAsync(new object[] { property.Id }, cancellationToken);
        if (e is null) throw new InvalidOperationException("Not found");
        e.Name = property.Name;
        e.Value = property.Value;
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeletePropertyAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var e = await _db.ClothingProperties.FindAsync(new object[] { id }, cancellationToken);
        if (e is null) return;
        _db.ClothingProperties.Remove(e);
        await _db.SaveChangesAsync(cancellationToken);
    }

    // Clothing slots
    public async Task<IReadOnlyList<ClothingSlot>> GetAllClothingSlotsAsync(CancellationToken cancellationToken = default)
    {
        return await _db.ClothingSlots.AsNoTracking().Select(s => new ClothingSlot { Id = s.Id, Name = s.Name }).ToListAsync(cancellationToken);
    }

    public async Task<ClothingSlot?> GetClothingSlotByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var e = await _db.ClothingSlots.FirstOrDefaultAsync(s => s.Name == name, cancellationToken);
        if (e is null) return null;
        return new ClothingSlot { Id = e.Id, Name = e.Name };
    }

    public async Task<ClothingSlot> AddClothingSlotAsync(ClothingSlot slot, CancellationToken cancellationToken = default)
    {
        var e = new ClothingSlotEntity { Id = slot.Id == Guid.Empty ? Guid.NewGuid() : slot.Id, Name = slot.Name };
        _db.ClothingSlots.Add(e);
        await _db.SaveChangesAsync(cancellationToken);
        slot.Id = e.Id;
        return slot;
    }

    public async Task AssociateCategoryWithSlotsAsync(Guid categoryId, IEnumerable<Guid> slotIds, CancellationToken cancellationToken = default)
    {
        var category = await _db.Categories.Include(c => c.ClothingSlots).FirstOrDefaultAsync(c => c.Id == categoryId, cancellationToken);
        if (category is null) throw new InvalidOperationException("Category not found");

        category.ClothingSlots.Clear();
        foreach (var sid in slotIds)
        {
            var s = await _db.ClothingSlots.FindAsync(new object[] { sid }, cancellationToken);
            if (s != null) category.ClothingSlots.Add(s);
        }

        await _db.SaveChangesAsync(cancellationToken);
    }
}
