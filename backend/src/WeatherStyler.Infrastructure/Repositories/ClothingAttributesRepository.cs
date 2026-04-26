using Microsoft.EntityFrameworkCore;
using WeatherStyler.Domain.Entities;

using WeatherStyler.Domain.Entities;
using WeatherStyler.Domain.Interfaces.Repositories;
using WeatherStyler.Infrastructure.Entities;
using WeatherStyler.Infrastructure.Persistence;

namespace WeatherStyler.Infrastructure.Repositories;


using AutoMapper;

internal class ClothingAttributesRepository : IClothingAttributesRepository
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public ClothingAttributesRepository(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    // Categories

    public async Task<IReadOnlyList<Category>> GetAllCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _db.Categories.AsNoTracking().ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<Category>>(entities);
    }


    public async Task<Category?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var e = await _db.Categories.FindAsync(new object[] { id }, cancellationToken);
        if (e is null) return null;
        return _mapper.Map<Category>(e);
    }


    public async Task<Category> AddCategoryAsync(Category category, CancellationToken cancellationToken = default)
    {
        var e = _mapper.Map<CategoryEntity>(category);
        if (e.Id == Guid.Empty) e.Id = Guid.NewGuid();
        _db.Categories.Add(e);
        await _db.SaveChangesAsync(cancellationToken);
        return _mapper.Map<Category>(e);
    }


    public async Task UpdateCategoryAsync(Category category, CancellationToken cancellationToken = default)
    {
        var e = await _db.Categories.FindAsync(new object[] { category.Id }, cancellationToken);
        if (e is null) throw new InvalidOperationException("Not found");
        _mapper.Map(category, e);
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
        var entities = await _db.Styles.AsNoTracking().ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<Style>>(entities);
    }


    public async Task<Style?> GetStyleByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var e = await _db.Styles.FindAsync(new object[] { id }, cancellationToken);
        if (e is null) return null;
        return _mapper.Map<Style>(e);
    }


    public async Task<Style> AddStyleAsync(Style style, CancellationToken cancellationToken = default)
    {
        var e = _mapper.Map<StyleEntity>(style);
        if (e.Id == Guid.Empty) e.Id = Guid.NewGuid();
        _db.Styles.Add(e);
        await _db.SaveChangesAsync(cancellationToken);
        return _mapper.Map<Style>(e);
    }


    public async Task UpdateStyleAsync(Style style, CancellationToken cancellationToken = default)
    {
        var e = await _db.Styles.FindAsync(new object[] { style.Id }, cancellationToken);
        if (e is null) throw new InvalidOperationException("Not found");
        _mapper.Map(style, e);
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
        var entities = await _db.Colors.AsNoTracking().ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<Color>>(entities);
    }


    public async Task<Color?> GetColorByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var e = await _db.Colors.FindAsync(new object[] { id }, cancellationToken);
        if (e is null) return null;
        return _mapper.Map<Color>(e);
    }


    public async Task<Color> AddColorAsync(Color color, CancellationToken cancellationToken = default)
    {
        var e = _mapper.Map<ColorEntity>(color);
        if (e.Id == Guid.Empty) e.Id = Guid.NewGuid();
        _db.Colors.Add(e);
        await _db.SaveChangesAsync(cancellationToken);
        return _mapper.Map<Color>(e);
    }


    public async Task UpdateColorAsync(Color color, CancellationToken cancellationToken = default)
    {
        var e = await _db.Colors.FindAsync(new object[] { color.Id }, cancellationToken);
        if (e is null) throw new InvalidOperationException("Not found");
        _mapper.Map(color, e);
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
        var entities = await _db.ClothingProperties.AsNoTracking().ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<ClothingProperty>>(entities);
    }


    public async Task<ClothingProperty?> GetPropertyByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var e = await _db.ClothingProperties.FindAsync(new object[] { id }, cancellationToken);
        if (e is null) return null;
        return _mapper.Map<ClothingProperty>(e);
    }


    public async Task<ClothingProperty> AddPropertyAsync(ClothingProperty property, CancellationToken cancellationToken = default)
    {
        var e = _mapper.Map<ClothingPropertyEntity>(property);
        if (e.Id == Guid.Empty) e.Id = Guid.NewGuid();
        _db.ClothingProperties.Add(e);
        await _db.SaveChangesAsync(cancellationToken);
        return _mapper.Map<ClothingProperty>(e);
    }


    public async Task UpdatePropertyAsync(ClothingProperty property, CancellationToken cancellationToken = default)
    {
        var e = await _db.ClothingProperties.FindAsync(new object[] { property.Id }, cancellationToken);
        if (e is null) throw new InvalidOperationException("Not found");
        _mapper.Map(property, e);
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
        var entities = await _db.ClothingSlots.AsNoTracking().ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<ClothingSlot>>(entities);
    }


    public async Task<ClothingSlot?> GetClothingSlotByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var e = await _db.ClothingSlots.FirstOrDefaultAsync(s => s.Name == name, cancellationToken);
        if (e is null) return null;
        return _mapper.Map<ClothingSlot>(e);
    }


    public async Task<ClothingSlot> AddClothingSlotAsync(ClothingSlot slot, CancellationToken cancellationToken = default)
    {
        var e = _mapper.Map<ClothingSlotEntity>(slot);
        if (e.Id == Guid.Empty) e.Id = Guid.NewGuid();
        _db.ClothingSlots.Add(e);
        await _db.SaveChangesAsync(cancellationToken);
        return _mapper.Map<ClothingSlot>(e);
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
