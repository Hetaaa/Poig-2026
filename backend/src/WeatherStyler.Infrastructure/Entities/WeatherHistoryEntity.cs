using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeatherStyler.Infrastructure.Persistence;

namespace WeatherStyler.Infrastructure.Entities;

public class WeatherHistoryEntity : WardrobeEntityBase
{
    public Guid UsageHistoryId { get; set; }
    public UsageHistoryEntity? UsageHistory { get; set; }

    public DateTime DateFetched { get; set; }
    public string DataJson { get; set; } = string.Empty;
}

internal class WeatherHistoryEntityConfiguration : IEntityTypeConfiguration<WeatherHistoryEntity>
{
    public void Configure(EntityTypeBuilder<WeatherHistoryEntity> builder)
    {
        builder.ToTable("WeatherHistories");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.DateFetched).IsRequired();
        builder.Property(x => x.DataJson).IsRequired();

        builder.HasIndex(x => x.UsageHistoryId).IsUnique();

        builder.HasOne(x => x.UsageHistory)
            .WithOne()
            .HasForeignKey<WeatherHistoryEntity>(x => x.UsageHistoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
