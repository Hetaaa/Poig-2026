using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeatherStyler.Infrastructure.Persistence;

namespace WeatherStyler.Infrastructure.Entities;

public class ColorEntity : WardrobeEntityBase
{
    public required string Name { get; set; }

    public ICollection<ClothingItemEntity> ClothingItems { get; set; } = new List<ClothingItemEntity>();
}

internal class ColorEntityConfiguration : IEntityTypeConfiguration<ColorEntity>
{
    public void Configure(EntityTypeBuilder<ColorEntity> builder)
    {
        builder.ToTable("Colors");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired();

        builder.HasIndex(x => x.Name).IsUnique();
    }
}
