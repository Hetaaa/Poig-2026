using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WeatherStyler.Domain.Common;

namespace WeatherStyler.Infrastructure.Entities;

// ApplicationUser uses GUID primary key to match domain entities
public class ApplicationUser : IdentityUser<Guid>, IAuditable, ISoftDeletable
{
    // IAuditable
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // ISoftDeletable
    public bool IsDeleted { get; set; }

    public ICollection<ClothingItemEntity> ClothingItems { get; set; } = new List<ClothingItemEntity>();
    public ICollection<OutfitEntity> Outfits { get; set; } = new List<OutfitEntity>();
    public ICollection<UsageHistoryEntity> UsageHistories { get; set; } = new List<UsageHistoryEntity>();
}

internal class ApplicationUserConfiguration : Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserName)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(x => x.PasswordHash)
            .HasMaxLength(512);

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired();

        builder.HasIndex(x => x.UserName).IsUnique();
    }
}
