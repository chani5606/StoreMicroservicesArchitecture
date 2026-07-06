using Microsoft.EntityFrameworkCore;
using OrderService.Models;

namespace OrderService.Data;

public sealed class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options)
        : base(options)
    {
    }

    public DbSet<Basket> Baskets => Set<Basket>();

    public DbSet<Sale> Sales => Set<Sale>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Basket>(entity =>
        {
            entity.HasKey(basket => basket.Id);
            entity.Property(basket => basket.CorrelationId).IsRequired();
            entity.Property(basket => basket.ProductId).IsRequired();
            entity.Property(basket => basket.UserId).IsRequired();
            entity.Property(basket => basket.Quantity).IsRequired();
            entity.Property(basket => basket.Status).HasMaxLength(30).IsRequired();
            entity.Property(basket => basket.UnitPrice).HasColumnType("decimal(18,2)");
            entity.Property(basket => basket.CreatedAtUtc).IsRequired();
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasKey(sale => sale.Id);
            entity.Property(sale => sale.Name).HasMaxLength(100).IsRequired();
            entity.Property(sale => sale.StartDateUtc).IsRequired();
            entity.Property(sale => sale.EndDateUtc).IsRequired();
        });
    }
}