using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WeatherStyler.Domain.Common;
using WeatherStyler.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace WeatherStyler.Infrastructure.Persistence;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using WeatherStyler.Infrastructure.Entities;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<ApplicationUser> Users => Set<ApplicationUser>();
    public DbSet<CategoryEntity> Categories => Set<CategoryEntity>();
    public DbSet<ClothingSlotEntity> ClothingSlots => Set<ClothingSlotEntity>();
    public DbSet<ClothingItemEntity> ClothingItems => Set<ClothingItemEntity>();
    public DbSet<ClothingPropertyEntity> ClothingProperties => Set<ClothingPropertyEntity>();
    // WarmthRating removed; using WarmthLevel int on ClothingItem instead
    public DbSet<StyleEntity> Styles => Set<StyleEntity>();
    public DbSet<ColorEntity> Colors => Set<ColorEntity>();
    public DbSet<OutfitEntity> Outfits => Set<OutfitEntity>();
    public DbSet<UsageHistoryEntity> UsageHistories => Set<UsageHistoryEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        ApplyGlobalSoftDeleteFilter(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    public override int SaveChanges()
    {
        ApplyAuditInformation();
        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplyAuditInformation();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditInformation();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        ApplyAuditInformation();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void ApplyAuditInformation()
    {
        var utcNow = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<IAuditable>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = utcNow;
                entry.Entity.UpdatedAt = null;

                if (entry.Entity is ISoftDeletable softDeletableEntity)
                {
                    softDeletableEntity.IsDeleted = false;
                }
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = utcNow;
            }
        }
    }

    private static void ApplyGlobalSoftDeleteFilter(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                continue;
            }

            var parameter = Expression.Parameter(entityType.ClrType, "entity");
            var isDeletedProperty = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
            var isNotDeleted = Expression.Equal(isDeletedProperty, Expression.Constant(false));
            var lambda = Expression.Lambda(isNotDeleted, parameter);

            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
        }
    }
}
