using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeatherStyler.Infrastructure.Persistence;

namespace WeatherStyler.Infrastructure.Entities;

public class OutfitEntity : WardrobeEntityBase
{
    public required string Name { get; set; }
    public string? WeatherCondition { get; set; }
    public DateTime DateCreated { get; set; }

    public Guid UserId { get; set; }
    public ApplicationUser? User { get; set; }

    public ICollection<ClothingItemEntity> ClothingItems { get; set; } = new List<ClothingItemEntity>();
}

internal class OutfitEntityConfiguration : IEntityTypeConfiguration<OutfitEntity>
{
    public void Configure(EntityTypeBuilder<OutfitEntity> builder)
    {
        builder.ToTable("Outfits");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.WeatherCondition)
            .HasMaxLength(128);

        builder.Property(x => x.DateCreated).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired();

        builder.HasIndex(x => x.UserId);

        builder.HasOne(x => x.User)
            .WithMany(x => x.Outfits)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
