using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeatherStyler.Infrastructure.Persistence;

namespace WeatherStyler.Infrastructure.Entities;

public class ClothingPropertyEntity : WardrobeEntityBase
{
    public required string Name { get; set; }
    public required string Value { get; set; }

    public Guid ClothingItemId { get; set; }
    public ClothingItemEntity? ClothingItem { get; set; }
}

internal class ClothingPropertyEntityConfiguration : IEntityTypeConfiguration<ClothingPropertyEntity>
{
    public void Configure(EntityTypeBuilder<ClothingPropertyEntity> builder)
    {
        builder.ToTable("ClothingProperties");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.Value)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired();

        builder.HasIndex(x => x.ClothingItemId);

        builder.HasOne(x => x.ClothingItem)
            .WithMany(x => x.Properties)
            .HasForeignKey(x => x.ClothingItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
