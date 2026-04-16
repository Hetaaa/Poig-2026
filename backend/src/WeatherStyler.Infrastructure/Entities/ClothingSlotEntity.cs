using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeatherStyler.Infrastructure.Persistence;

namespace WeatherStyler.Infrastructure.Entities;

public class ClothingSlotEntity : WardrobeEntityBase
{
    public required string Name { get; set; }

    public ICollection<CategoryEntity> Categories { get; set; } = new List<CategoryEntity>();
}

internal class ClothingSlotEntityConfiguration : IEntityTypeConfiguration<ClothingSlotEntity>
{
    public void Configure(EntityTypeBuilder<ClothingSlotEntity> builder)
    {
        builder.ToTable("ClothingSlots");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired();

        builder.HasIndex(x => x.Name).IsUnique();

        builder.HasMany(x => x.Categories)
            .WithMany(x => x.ClothingSlots)
            .UsingEntity<Dictionary<string, object>>(
                "ClothingSlotCategories",
                right => right.HasOne<CategoryEntity>().WithMany().HasForeignKey("CategoryId").OnDelete(DeleteBehavior.Cascade),
                left => left.HasOne<ClothingSlotEntity>().WithMany().HasForeignKey("ClothingSlotId").OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.ToTable("ClothingSlotCategories");
                    join.HasKey("ClothingSlotId", "CategoryId");
                });
    }
}
