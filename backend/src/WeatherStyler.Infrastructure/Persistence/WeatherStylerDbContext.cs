using Microsoft.EntityFrameworkCore;
using WeatherStyler.Domain.Entities;

namespace WeatherStyler.Infrastructure.Persistence;

public class WeatherStylerDbContext(DbContextOptions<WeatherStylerDbContext> options) : DbContext(options)
{
    public DbSet<WeatherStyle> WeatherStyles => Set<WeatherStyle>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WeatherStyle>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
            entity.Property(x => x.ThemeColor).HasMaxLength(16).IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}
