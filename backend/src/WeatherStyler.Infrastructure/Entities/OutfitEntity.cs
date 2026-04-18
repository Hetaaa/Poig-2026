using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeatherStyler.Infrastructure.Persistence;

namespace WeatherStyler.Infrastructure.Entities;

public class OutfitEntity : WardrobeEntityBase
{
    public required string Name { get; set; }
    public DateTime DateCreated { get; set; }

    public Guid UserId { get; set; }
    public ApplicationUser? User { get; set; }

    // items that compose this outfit
    public ICollection<ClothingItemEntity> ClothingItems { get; set; } = new List<ClothingItemEntity>();
    // usage history entries that reference this outfit (one outfit can be used on many days)
    public ICollection<UsageHistoryEntity> UsageHistories { get; set; } = new List<UsageHistoryEntity>();
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

        // weather condition removed - weather handled separately in WeatherHistory

        builder.Property(x => x.DateCreated).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired();

        builder.HasIndex(x => x.UserId);

        builder.HasOne(x => x.User)
            .WithMany(x => x.Outfits)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.UsageHistories)
            .WithOne(x => x.Outfit)
            .HasForeignKey(x => x.OutfitId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.ClothingItems)
            .WithMany(x => x.Outfits)
            .UsingEntity<Dictionary<string, object>>(
                "OutfitClothingItems",
                right => right.HasOne<ClothingItemEntity>().WithMany().HasForeignKey("ClothingItemId").OnDelete(DeleteBehavior.Cascade),
                left => left.HasOne<OutfitEntity>().WithMany().HasForeignKey("OutfitId").OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.ToTable("OutfitClothingItems");
                    join.HasKey("OutfitId", "ClothingItemId");
                });
    }
}
