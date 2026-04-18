using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeatherStyler.Infrastructure.Persistence;

namespace WeatherStyler.Infrastructure.Entities;

public class UsageHistoryEntity : WardrobeEntityBase
{
    public int Rating { get; set; }
    public DateTime DateWorn { get; set; }

    public Guid UserId { get; set; }
    public ApplicationUser? User { get; set; }
    // a usage history entry references an outfit that was worn that day
    public Guid? OutfitId { get; set; }
    public OutfitEntity? Outfit { get; set; }
}

internal class UsageHistoryEntityConfiguration : IEntityTypeConfiguration<UsageHistoryEntity>
{
    public void Configure(EntityTypeBuilder<UsageHistoryEntity> builder)
    {
        builder.ToTable("UsageHistories", tableBuilder =>
        {
            tableBuilder.HasCheckConstraint("CK_UsageHistories_Rating_Range", "Rating BETWEEN 1 AND 5");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DateWorn).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired();

        builder.HasIndex(x => x.UserId);

        builder.HasOne(x => x.User)
            .WithMany(x => x.UsageHistories)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Outfit)
            .WithMany(x => x.UsageHistories)
            .HasForeignKey(x => x.OutfitId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
