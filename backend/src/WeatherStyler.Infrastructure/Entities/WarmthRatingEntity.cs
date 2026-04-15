using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeatherStyler.Infrastructure.Persistence;

namespace WeatherStyler.Infrastructure.Entities;

public class WarmthRatingEntity : WardrobeEntityBase
{
    public int ArmsLevel { get; set; }
    public int CoreLevel { get; set; }
    public int LegsLevel { get; set; }

    public Guid ClothingItemId { get; set; }
    public ClothingItemEntity? ClothingItem { get; set; }
}

internal class WarmthRatingEntityConfiguration : IEntityTypeConfiguration<WarmthRatingEntity>
{
    public void Configure(EntityTypeBuilder<WarmthRatingEntity> builder)
    {
        builder.ToTable("WarmthRatings", tableBuilder =>
        {
            tableBuilder.HasCheckConstraint("CK_WarmthRatings_ArmsLevel_Range", "ArmsLevel BETWEEN 1 AND 10");
            tableBuilder.HasCheckConstraint("CK_WarmthRatings_CoreLevel_Range", "CoreLevel BETWEEN 1 AND 10");
            tableBuilder.HasCheckConstraint("CK_WarmthRatings_LegsLevel_Range", "LegsLevel BETWEEN 1 AND 10");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired();

        builder.HasIndex(x => x.ClothingItemId).IsUnique();

        builder.HasOne(x => x.ClothingItem)
            .WithOne(x => x.WarmthRating)
            .HasForeignKey<WarmthRatingEntity>(x => x.ClothingItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
