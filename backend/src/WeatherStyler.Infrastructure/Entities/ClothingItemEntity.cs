using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeatherStyler.Infrastructure.Persistence;

namespace WeatherStyler.Infrastructure.Entities;

public class ClothingItemEntity : WardrobeEntityBase
{
    public required string Name { get; set; }
    public string? PhotoUrl { get; set; }

    public Guid CategoryId { get; set; }
    public Guid UserId { get; set; }

    public CategoryEntity? Category { get; set; }
    public UserEntity? User { get; set; }
    public WarmthRatingEntity? WarmthRating { get; set; }

    public ICollection<ClothingPropertyEntity> Properties { get; set; } = new List<ClothingPropertyEntity>();
    public ICollection<StyleEntity> Styles { get; set; } = new List<StyleEntity>();
    public ICollection<ColorEntity> Colors { get; set; } = new List<ColorEntity>();
    public ICollection<OutfitEntity> Outfits { get; set; } = new List<OutfitEntity>();
    public ICollection<UsageHistoryEntity> UsageHistories { get; set; } = new List<UsageHistoryEntity>();
}

internal class ClothingItemEntityConfiguration : IEntityTypeConfiguration<ClothingItemEntity>
{
    public void Configure(EntityTypeBuilder<ClothingItemEntity> builder)
    {
        builder.ToTable("ClothingItems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.PhotoUrl)
            .HasMaxLength(500);

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired();

        builder.HasIndex(x => x.Name);
        builder.HasIndex(x => x.CategoryId);
        builder.HasIndex(x => x.UserId);

        builder.HasOne(x => x.Category)
            .WithMany(x => x.ClothingItems)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
            .WithMany(x => x.ClothingItems)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Styles)
            .WithMany(x => x.ClothingItems)
            .UsingEntity<Dictionary<string, object>>(
                "ClothingItemStyles",
                right => right.HasOne<StyleEntity>().WithMany().HasForeignKey("StyleId").OnDelete(DeleteBehavior.Cascade),
                left => left.HasOne<ClothingItemEntity>().WithMany().HasForeignKey("ClothingItemId").OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.ToTable("ClothingItemStyles");
                    join.HasKey("ClothingItemId", "StyleId");
                });

        builder.HasMany(x => x.Colors)
            .WithMany(x => x.ClothingItems)
            .UsingEntity<Dictionary<string, object>>(
                "ClothingItemColors",
                right => right.HasOne<ColorEntity>().WithMany().HasForeignKey("ColorId").OnDelete(DeleteBehavior.Cascade),
                left => left.HasOne<ClothingItemEntity>().WithMany().HasForeignKey("ClothingItemId").OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.ToTable("ClothingItemColors");
                    join.HasKey("ClothingItemId", "ColorId");
                });

        builder.HasMany(x => x.Outfits)
            .WithMany(x => x.ClothingItems)
            .UsingEntity<Dictionary<string, object>>(
                "OutfitClothingItems",
                right => right.HasOne<OutfitEntity>().WithMany().HasForeignKey("OutfitId").OnDelete(DeleteBehavior.Cascade),
                left => left.HasOne<ClothingItemEntity>().WithMany().HasForeignKey("ClothingItemId").OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.ToTable("OutfitClothingItems");
                    join.HasKey("OutfitId", "ClothingItemId");
                });

        builder.HasMany(x => x.UsageHistories)
            .WithMany(x => x.ClothingItems)
            .UsingEntity<Dictionary<string, object>>(
                "UsageHistoryClothingItems",
                right => right.HasOne<UsageHistoryEntity>().WithMany().HasForeignKey("UsageHistoryId").OnDelete(DeleteBehavior.Cascade),
                left => left.HasOne<ClothingItemEntity>().WithMany().HasForeignKey("ClothingItemId").OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.ToTable("UsageHistoryClothingItems");
                    join.HasKey("UsageHistoryId", "ClothingItemId");
                });
    }
}
