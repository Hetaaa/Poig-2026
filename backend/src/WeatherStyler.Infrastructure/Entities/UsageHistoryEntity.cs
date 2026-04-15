using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeatherStyler.Infrastructure.Persistence;

namespace WeatherStyler.Infrastructure.Entities;

public class UsageHistoryEntity : WardrobeEntityBase
{
    public int Rating { get; set; }
    public DateTime DateWorn { get; set; }

    public Guid UserId { get; set; }
    public UserEntity? User { get; set; }

    public ICollection<ClothingItemEntity> ClothingItems { get; set; } = new List<ClothingItemEntity>();
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
    }
}
